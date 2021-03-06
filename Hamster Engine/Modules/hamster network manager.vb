﻿Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports HamsterEngine.Engine
Imports HamsterEngine.Words

Public Class Networkmanager
    Public version As New HamsterVersion("Network Manager", 2, 4, 140815, 0)

    Private Socket As Socket '소켓
    Private clientSoc As Socket '클라이언트 소켓 인스턴스
    Public ClientSocCache(255) As Socket '클라이언트 소켓 버퍼

    Private IPA As IPEndPoint '모듈 공용 ipa

    Public hlistner As Boolean '모듈 공용 상태 변수

    Private ClientAmo As Byte '모듈 공용 클라이언트 연결 제한 갯수

    Private ListenThread As Thread '소켓 리슨용 스레드
    Private ConnectThread As Thread '소켓 연결용 스레드
    Private ConnectAcceptThread As Thread '연결 요청 허가용 스레드(서버)

    Private ClientSocAmount As Integer = 0 '클라이언트 소켓 갯수
    Private databuffer(1024) As Byte  '데이터 버퍼
    Private tdatabuffer(255, 1024) As Byte  '데이터 버퍼
    Private CompleteData As String '데이터 버퍼에서 잉여 문자 처리한 최종 데이터





    Public Sub HelpString()
        Console.Print("Console", 0, "-------------Hamster Network Manager 모드 설정 명령어")
        Console.Print("Console", 0, "init 또는 i <IP주소 : 마침표로 구분된 문자열 (서버인 경우  's')> <포트 : 정수> <연결 가능한 최대클라이언트 수 (클라이언트인 경우 'c' 이며 최대값은 'max', 최소값은 'min') : 정수 또는 문자열> - 소켓을 초기화 합니다")
        Console.Print("Console", 0, "connect 또는 c - 초기화된 정보로 연결합니다")
        Console.Print("Console", 0, "listen 또는 l - 초기화된 정보로 대기합니다")
        Console.Print("Console", 0, "shutdown 또는 shdw - 종료합니다")
    End Sub


    Public Function Init(ByVal listen As Boolean, ByVal IP As String, ByVal Port As Integer, ByVal CanAcceptClientAmoount As Byte) As Boolean
        Try
            Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            If listen Then
                Console.Print("Network", 2, "서버 모드로 소켓을 초기화")
                Dim IPep = New IPEndPoint(IPAddress.Any, Port)

                Socket.Bind(IPep)
                ClientAmo = CanAcceptClientAmoount

                hlistner = listen

                IPA = IPep
                Socket.ReceiveTimeout = 1
                Console.Print("Network", 2, "초기화 완료됨 " + IPA.Address.ToString + ":" + IPA.Port.ToString)
                Return True
            Else
                Console.Print("Network", 2, "클라이언트 모드로 소켓을 초기화")
                Dim IPep = New IPEndPoint(IPAddress.Parse(IP), Port)

                'Socket.bind(IPep) //제거시 문제 없으면 이 줄 삭제

                hlistner = listen

                IPA = IPep
                Console.Print("Network", 2, "초기화 완료됨 " + IPA.Address.ToString + ":" + IPA.Port.ToString)
                Return True
            End If
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.NetworkManager, "Init")
            Return False
        End Try

    End Function

    Public Function Connect() As Boolean
        Try
            Console.Print("Network", 2, "스레드 초기화...")
            ConnectThread = New Thread(AddressOf tConnect)
            Console.Print("Network", 2, "스레드 초기화 완료")

            Console.Print("Network", 2, "소켓 연결를 위한 스레드를 생성합니다")
            Console.Print("Network", 2, "스레드 해시 : " + ConnectThread.GetHashCode.ToString + ", 스레드 상태 : " + ConnectThread.ThreadState.ToString)
            Console.Print("Network", 2, "스레드 시작을 시도합니다")
            ConnectThread.Start()

            Console.Print("Network", 2, "스레드 시작을 위해 잠시 대기합니다")

            Thread.Sleep(100)

            Console.Print("Network", 2, "스레드 시작 성공!")
            Return True

        Catch EX As Exception
            ErrorReporter.ReportError(EX, Words.hName.ModuleName.NetworkManager, "Connect")
            Console.Print("Network", 2, "스레드 생성중 오류 발생!")
            Return False

            If ConnectThread.ThreadState = ThreadState.Unstarted Or ConnectThread.Equals(Nothing) Then
                Console.Print("Network", 2, "스레드가 생성되지 않았으므로 스레드를 제거 작업을 시도하지 않습니다")
            Else
                Console.Print("Network", 2, "스레드를 제거합니다")
                ConnectThread.Abort()
                ConnectThread.Join()
            End If
        Finally


            Console.Print("Network", 2, "스레드 해시 : " + ConnectThread.GetHashCode.ToString + ", 스레드 상태 : " + ConnectThread.ThreadState.ToString)

        End Try

    End Function

    Public Function tConnect() As Boolean

        Try
            If hlistner = False Then
                Console.Print("Network", 2, IPA.Address.ToString + ":" + IPA.Port.ToString + "로 접속합니다", "Connect")
                Socket.Connect(IPA)
                Console.Print("Network", 2, IPA.Address.ToString + ":" + IPA.Port.ToString + "로 접속했습니다", "Connect")
            Else
                Console.Print("Network", 1, "서버 모드에서 연결할수 없습니다")
            End If


        Catch ex As NullReferenceException
            Console.Print("Network", 1, "소켓을 먼저 초기화해주십시요!")
        Catch ex As SocketException
            Console.Print("Network", 1, "연결 시도도중 오류가 발생했습니다! : " + ex.Message)
            Console.WarningMsg.Print(2, ex.Message, 10)
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.NetworkManager, "Connect")

        Finally
            ConnectThread.Abort()
            ConnectThread.Join()
        End Try
    End Function

    Public Function Send(data As String, targetSocketAddress As Integer) As Boolean

        Dim databuffer() As Byte = Encoding.UTF8.GetBytes(data)
        Dim i As Integer

        If hlistner Then
            Console.Print("Network", 2, IPAddress.Parse(CType(ClientSocCache(targetSocketAddress).RemoteEndPoint, IPEndPoint).Address.ToString()).ToString + ":" + CType(ClientSocCache(targetSocketAddress).RemoteEndPoint, IPEndPoint).Port.ToString() & "로 " + data + "' 를 보냅니다", "Send")
            ClientSocCache(targetSocketAddress).Send(databuffer)
            Do While True
                i = i + 1
                If i > UBound(databuffer) Then
                    Exit Do
                End If
            Loop
        Else
            Console.Print("Network", 2, IPAddress.Parse(CType(Socket.RemoteEndPoint, IPEndPoint).Address.ToString()).ToString + ":" + CType(Socket.RemoteEndPoint, IPEndPoint).Port.ToString() & "로 " + data + "' 를 보냅니다", "Send")
            Do While True
                i = i + 1
                If i > UBound(databuffer) Then
                    Exit Do
                End If
            Loop

            Socket.Send(databuffer)
        End If

        Return True



    End Function


    Public Sub SetListen()
        Try
            Console.Print("Network", 2, "스레드 초기화..")
            ListenThread = New Thread(AddressOf Listen)
            ConnectAcceptThread = New Thread(AddressOf ConnectAcceptListen)
            Console.Print("Network", 2, "스레드 초기화 완료..")

            Console.Print("Network", 2, "대기 스레드 해시 : " + ListenThread.GetHashCode.ToString + ", 스레드 상태 : " + ListenThread.ThreadState.ToString)
            Console.Print("Network", 2, "연결 요청 대기 스레드 해시 : " + ConnectAcceptThread.GetHashCode.ToString + ", 스레드 상태 : " + ConnectAcceptThread.ThreadState.ToString)
            Console.Print("Network", 2, "스레드 시작을 시도합니다")

            If hlistner Then
                ConnectAcceptThread.Start()
            End If

            ListenThread.Start()

            Console.Print("Network", 2, "스레드 시작을 위해 잠시 대기합니다")

            Thread.Sleep(100)

            Console.Print("Network", 2, "스레드 시작 성공!")

        Catch EX As Exception
            ErrorReporter.ReportError(EX, Words.hName.ModuleName.NetworkManager, "SetListen")

            Console.Print("Network", 2, "스레드 생성중 오류 발생!")

            If ListenThread.ThreadState = ThreadState.Unstarted Or ListenThread.Equals(Nothing) Then
                Console.Print("Network", 2, "스레드가 생성되지 않았으므로 스레드를 제거 작업을 시도하지 않습니다")
            Else
                Console.Print("Network", 2, "스레드를 제거합니다")
                ListenThread.Abort()
                ConnectAcceptThread.Abort()
                ListenThread.Join()
                ConnectAcceptThread.Join()
            End If
        Finally


            Console.Print("Network", 2, "대기 스레드 해시 : " + ListenThread.GetHashCode.ToString + ", 스레드 상태 : " + ListenThread.ThreadState.ToString)
            Console.Print("Network", 2, "연결 요청 스레드 해시 : " + ConnectAcceptThread.GetHashCode.ToString + ", 스레드 상태 : " + ConnectAcceptThread.ThreadState.ToString)
        End Try

    End Sub


    Private Sub Listen()
        Dim trycount As ULong
        Dim i As Integer = -1

        Console.Print("Network", 0, "대기 시작", "ListenThread")
        Try
            If hlistner Then



                Do While True

                    Try
retry:
                        i = i + 1

                        If i = 255 Then
                            i = 0
                        End If

                        If i = ClientSocAmount Then
                            i = 0
                        End If

                        If Not ClientSocCache(i).Available = 0 Then
                            ClientSocCache(i).Receive(databuffer)
                            CompleteData = Encoding.UTF8.GetString(databuffer, 0, ReturnWithoutNulldataLength(databuffer))
                            Console.Print("Network", 0, "데이터 받음, 길이 " & CompleteData.Length & " 내용 = '" & CompleteData & "'", Words.hThread.Threadname.ListenThread)
                            Engine.ServerListenEvent(i, CompleteData)

                        End If
                    Catch
                    Finally

                        ReDim databuffer(1024)



                    End Try



                Loop


            Else
                Do While True
                    Try
                        If Not (Socket.Receive(databuffer) = 0) Then '
                            CompleteData = Encoding.UTF8.GetString(databuffer, 0, ReturnWithoutNulldataLength(databuffer))
                            Console.Print("Network", 0, "데이터 받음, 길이 " & CompleteData.Length & " 내용 = '" & CompleteData & "'", Words.hThread.Threadname.ListenThread)

                        End If
                        ReDim databuffer(1024)
                        If i > 255 Then
                            i = 0
                        End If
                    Catch ex As Exception
                        ErrorReporter.ReportError(ex, "Listen")
                    End Try

                    trycount = trycount + 1

                    If (trycount Mod 500 = 0) Then
                        Console.Print("Nerwork", 2, trycount + "번째 대기 시도", Words.hThread.Threadname.ListenThread)
                    End If
                Loop
            End If
        Catch ex As NullReferenceException
            Console.Print("Network", 1, "소켓을 먼저 초기화해주십시요!", Words.hThread.Threadname.ListenThread)
            Shutdown()
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.NetworkManager, "ListenThread\Listen")
        End Try

    End Sub



    Private Function ReturnWithoutNulldataLength(data As Byte()) As Integer
        Dim i As Integer = 0
        Dim Datalength As Integer

        Do While True
            If data(i) = 0 Then
                Datalength = i
                Exit Do
            End If

            i = i + 1
        Loop

        Return Datalength
    End Function

    Private Sub ConnectAcceptListen()
        Try
            Socket.Listen(ClientAmo)
reListen:
            Dim i As Integer
            Console.Print("Network", 0, "대기 시작", "ConnectAcceptThread")
            Do While True
                If hlistner Then
                    clientSoc = Socket.Accept()
                    Console.Print("Network", 0, "연결됨!! " + IPAddress.Parse(CType(clientSoc.RemoteEndPoint, IPEndPoint).Address.ToString()).ToString + ":" + CType(clientSoc.RemoteEndPoint, IPEndPoint).Port.ToString(), "ConnectAcceptThread")

                    ClientSocAmount = ClientSocAmount + 1
                    ClientSocCache(ClientSocAmount - 1) = clientSoc
                    ClientSocCache(ClientSocAmount - 1).ReceiveTimeout = 1
                    Console.Print("Network", 2, "클라이언트 소켓 배열 주소 : " & ClientSocAmount - 1)
                    Console.Print("Network", 2, "클라이언트 소켓 갯수 : " & ClientSocAmount)

                    i = 0

                    Do While True
                        Try
                            Dim clisoc = ClientSocCache(i)
                            Console.Print("Network", 2, i & "번 주소의 클라 소켓의 주소 : " & IPAddress.Parse(CType(clisoc.RemoteEndPoint, IPEndPoint).Address.ToString()).ToString() & ":" & CType(clisoc.RemoteEndPoint, IPEndPoint).Port.ToString(), "ConnectAcceptThread")
                            i = i + 1

                            If i > ClientSocAmount - 1 Then
                                Exit Do
                            End If
                        Catch ex As NullReferenceException
                            Console.Print("Network", 2, i & "번 주소의 클라 소켓은 NULL")
                            GoTo reListen
                        End Try
                    Loop
                Else
                    Exit Do
                End If
            Loop
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.NetworkManager, "ConnectAcceptThread")
            GoTo reListen
        End Try

    End Sub



    Public Sub Shutdown()
        Try
            ReDim ClientSocCache(1024)
            If Not Socket.Equals(Nothing) Then
                Socket.Shutdown(SocketShutdown.Both)
                Socket.Close()
            Else
                Console.Print("Network", 1, "소켓이 초기화 되지 않았습니다! 제거할수 없습니다")
            End If

            If Not clientSoc.Equals(Nothing) Then
                Socket.Shutdown(SocketShutdown.Both)
                clientSoc.Close()
            Else
                Console.Print("Network", 1, "클라이언트 소켓이 초기화 되지 않았습니다! 제거할수 없습니다")
            End If


            Console.Print("Network", 2, "스레드를 제거합니다")

            If ListenThread.ThreadState = ThreadState.Unstarted Or ListenThread.Equals(Nothing) Then
                Console.Print("Network", 1, "스레드가 생성되지 않았으므로 스레드를 제거 작업을 시도하지 않습니다")
            Else
                Console.Print("Network", 0, "스레드를 제거합니다")
                ListenThread.Abort()
                ListenThread.Join()
            End If
        Catch ex As NullReferenceException
            Console.Print("Network", 1, "소켓 또는 클라이언트가 실행되지 않았습니다, 제거할수 없습니다")
        Catch ex As Exception
            ErrorReporter.ReportError(ex, Words.hName.ModuleName.NetworkManager, "shutdown")
            Console.Print("Network", 1, "스레드와 소켓을 제거하는중 오류가 발생했습니다!, 제거할수 없습니다")
        Finally


            ' Console.Print("Network", 2, "대기 스레드 해시 : " + ListenThread.GetHashCode.ToString + ", 스레드 상태 : " + ListenThread.ThreadState.ToString)
            'Console.Print("Network", 2, "연결 요청 스레드 해시 : " + ConnectAcceptThread.GetHashCode.ToString + ", 스레드 상태 : " + ConnectAcceptThread.ThreadState.ToString)
        End Try

    End Sub


End Class
