Imports System.Text
Imports HamsterEngine.Engine
Imports System.Timers.Timer

Public Class Console
    Public Version As New HamsterVersion("Console", 1, 3, 140126, 0)

    Public converlist As String '콘솔 버젼 리스트
    Public Shared projruntime As Long '엔진 시작 시간
    Private concommand() As String '콘솔 명령어 파싱 배열
    Public Shared permissionlevel As Byte '0=DEFAULT, 1=ERROR, 2=INFO 3=DEBUG

    Private lasttext As String '콘솔창에 마지막으로 입력한 항목

    Private txtinpushed As Boolean 'getstring 함수를 위한 consoleinput 버튼 눌림 상태
    Private waitforinput As Boolean 'getstring 함수를 위한 consoleinput 의 함수 무시 허용 변수

    Private ConNetworkSoc As New Networkmanager ' 콘솔 전용 네트워크 매니저

    Delegate Sub Conprint(caller As String, level As Byte, message As String, subcaller As String) '크로스 스레드를 위한 대리자
    Delegate Sub warnprint(level As Byte, message As String, eraseSec As Integer) '크로스 스레드를 위한 대리자



    Public Shared Sub Print(caller As String, level As Byte, message As String, Optional subcaller As String = "General")

        SyncLock (conlist)

            Try


                If conlist.InvokeRequired Then
                    Dim printdel As Conprint = New Conprint(AddressOf Print)

                    conlist.BeginInvoke(printdel, caller, level, message, subcaller)

                Else
                    If message = "" Then
                        conlist.Items.Add("")
                    End If

                    If level = 0 And NowPermissionLevel(False) >= level Then
                        conlist.Items.Add(permissiontostring(level) + "-" + "[" + caller + "\" + subcaller + "]/" + projruntime.ToString + "ms > " + message)
                    ElseIf level = 1 And NowPermissionLevel(False) >= level Then

                        conlist.Items.Add(permissiontostring(level) + "-" + "[" + caller + "\" + subcaller + "]/" + projruntime.ToString + "ms > " + message)
                    ElseIf level = 2 And NowPermissionLevel(False) >= level Then
                        conlist.Items.Add(permissiontostring(level) + "-" + "[" + caller + "\" + subcaller + "]/" + projruntime.ToString + "ms > " + message)
                    ElseIf level > 5 Then
                        conlist.Items.Add("내부오류!!" + caller + "가 호출한 " + level.ToString + "권한 레벨은 표시할수 없는 레벨입니다")
                        HamsterEngine.Console.Log.write(caller + "의 세부경로 " + subcaller + " 가 호출했고, 권한은 " + permissiontostring(level) + ", 현재권한은 " + NowPermissionLevel(True) + "이므로 표시 불가")
                    End If

                    conlist.TopIndex = conlist.Items.Count - 1

                    If subcaller = "Network" Then
                        MsgBox(message + " / " + level.ToString + " / " + projruntime.ToString)
                    End If

                    HamsterEngine.Console.Log.write(permissiontostring(level) + "-" + "[" + caller + "\" + subcaller + "]/" + projruntime.ToString + "ms > " + message)
                End If
            Catch exp As Exception
                ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "Print")
            End Try
        End SyncLock

    End Sub

    Public Sub Printarrange(caller As String, level As Byte, message As String(), Optional subcaller As String = "General", Optional frontDefMsg As String = "", Optional lastDefMsg As String = "")
        Try


            Dim returnmsg As String
            Dim repeat As Integer

            Do Until repeat = UBound(message)
                returnmsg = message(repeat)
                If level = 0 And NowPermissionLevel(False) >= level Then
                    conlist.Items.Add(permissiontostring(level) & "-" & "[" & caller & "\" & subcaller & "]/" & projruntime & "ms > " & frontDefMsg & returnmsg & lastDefMsg)
                ElseIf level = 1 And NowPermissionLevel(False) >= level Then
                    conlist.Items.Add(permissiontostring(level) & "-" & "[" & caller & "\" & subcaller & "]/" & projruntime & "ms > " & frontDefMsg & returnmsg & lastDefMsg)
                ElseIf level = 2 And NowPermissionLevel(False) >= level Then
                    conlist.Items.Add(permissiontostring(level) & "-" & "[" & caller & "\" & subcaller & "]/" & projruntime & "ms > " & frontDefMsg & returnmsg & lastDefMsg)
               ElseIf level > 5 Then
                    conlist.Items.Add("내부오류!!" + caller + "가 호출한 " + level.ToString + "권한 레벨은 표시할수 없는 레벨입니다")
                    HamsterEngine.Console.Log.write(caller + "의 세부경로 " + subcaller + " 가 호출했고, 권한은 " + permissiontostring(level) + ", 현재권한은 " + NowPermissionLevel(True) + "이므로 표시 불가")
                End If

                conlist.TopIndex = conlist.Items.Count - 1
                HamsterEngine.Console.Log.write(permissiontostring(level) & "-" & "[" & caller & "]/" & projruntime & "ms > " & frontDefMsg & returnmsg & lastDefMsg)
                repeat = repeat + 1


            Loop
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "Printarrange")
        End Try


    End Sub

    Public Shared Function NowPermissionLevel(returnstring As Boolean)
        '0=정보 1=에러 2=디버그
        If returnstring = True Then
            If permissionlevel = 0 Then
                Return "INFO"
            ElseIf permissionlevel = 1 Then
                Return "ERROR"
            ElseIf permissionlevel = 2 Then
                Return "DEBUG"
            End If
        ElseIf returnstring = False Then
            Return permissionlevel
        End If

    End Function

    Public Shared Function permissiontostring(level As Byte) As String
        '0=정보 1=에러 2=디버그
        If level = 0 Then
            Return "INFO"
        ElseIf level = 1 Then
            Return "ERROR"
        ElseIf level = 2 Then
            Return "DEBUG"
        End If
    End Function

    Private Sub Console_Closing(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
       
    End Sub

    Private Sub Console_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Engine.start()
        Try

            Reftimer.Interval = 1
            permissionlevel = 2


            If Debugger.IsAttached Then
                permissionlevel = 4
                Print("Console", 2, "디버그 모드에서 실행중!! 자동으로 DEBUG 권한을 얻습니다")
                WarningMsg.Print(0, "디버거에서 실행중입니다", 10)
            End If

            Print("Console", 0, "콘솔 시작 시간 : " & Now)
            Print("Console", 0, "콘솔 로드 완료 ")
            Print("Console", 0, "햄스터 엔진 콘솔입니다! 환영합니다(제작 : 김정현)")
            Print("Console", 0, "프로젝트 " & ProjVersion.GetVersion(True) & " / " & ProjVersion.GetName)
            Print("Console", 0, "엔진 버젼 " & HamsterEngine.Engine.Version.GetVersion(True) & " (콘솔버젼 : " & Me.Version.GetVersion(True) & " )")
            Print("Console", 0, "네트워크 " & ConNetworkSoc.version.GetVersion(True))
            Print("Console", 0, "파일 매니져 " & Filemanager.Version.GetVersion(True))
            Print("Console", 0, "예외 처리기 " & ErrorReporter.Version.GetVersion(True))
            Print("Console", 2, "OS = " & My.Computer.Info.OSFullName)
            Print("Console", 2, "OS Platform = " & My.Computer.Info.OSPlatform)
            Print("Console", 2, "OS Version = " & My.Computer.Info.OSVersion)
            Print("Console", 2, "현재 실행파일 경로 = " & Application.ExecutablePath)
            Print("Console", 2, "현재 실행 경로 = " & Environment.CurrentDirectory)

        Catch exp As Exception
            ErrorReporter.ReportError(exp, "Console", "Load")
        End Try
    End Sub

    Private Sub consoleinput_Click(sender As System.Object, e As System.EventArgs) Handles consoleinput.Click
        Try
            '0=정보 1=에러 2=디버그




            lasttext = txtin.Text

            If waitforinput = True Then
                txtinpushed = True
                Exit Sub
            End If

            HamsterEngine.Console.Log.write("[콘솔 입력 감시자] /" & projruntime & "ms / '" & lasttext & "' 콘솔창에 입력됨")

            If txtin.Text = "" Then
                Print("Console", 0, "내용을 입력하신후 버튼을 클릭하세요.")
            ElseIf LCase(txtin.Text) = "exit" Then
                Print("Console", 0, "콘솔을 종료합니다")
                Me.Hide()
            ElseIf LCase(txtin.Text) = "gamestart" Then
                Engine.SendAllClient("CHAT" & Words.ParseString & "[서버]관리자에 의해 게임을 강제로 시작합니다.")
                Engine.GameStart(0, UBound(Engine.Nickname), 1, UBound(Engine.Nickname) - 3, {1, 1})
            ElseIf LCase(txtin.Text) = "runtime" Then
                Print("Console", 0, "엔진 시작 시간 = " & projruntime & "ms(밀리 초)")
            ElseIf LCase(txtin.Text) = "clear" Then
                conlist.Items.Clear()
                Print("Console", 0, "콘솔 명령창 청소 완료")
            ElseIf LCase(txtin.Text) = "projexit" Then
                Print("Console", 0, "프로젝트 종료")
                ConNetworkSoc.Shutdown()
                Engine.shutdown()
            ElseIf LCase(txtin.Text) = "getstring" And NowPermissionLevel(False) >= 3 Then
                Print("Console", 0, getstring(True, "getstring 를 테스트 합니다") & " 값을 반환받았습니다.")
            ElseIf LCase(txtin.Text) = "getadminpermission" And NowPermissionLevel(False) >= 3 Or LCase(txtin.Text) = "gap" And NowPermissionLevel(False) >= 3 Then
                Print("Console", 0, "관리자 권한을 취득합니다(UAC 가용 OS 만)")
                getadminwinpermission()
            ElseIf LCase(txtin.Text) = "debug" Then
                Print("Console", 0, "Debug 권한을 취득합니다")
                permissionlevel = 4
                Print("Console", 0, "현재 권한은 " & NowPermissionLevel(True) & " 입니다")
            ElseIf LCase(txtin.Text) = "start" Then
                Print("Console", 0, "서버 시작")
                Engine.ServerStart()
            ElseIf LCase(txtin.Text) = "stop" Then
                Print("Console", 0, "서버 종료")
                Engine.ServerStop()
            ElseIf LCase(txtin.Text) = "help" Then
                Print("Console", 0, "------------")
                Print("Console", 0, "콘솔 명령어 도움말")
                Print("Console", 0, "명령어 - 뜻")
                Print("Console", 0, "------------")
                Print("Console", 0, "exit - 콘솔을 종료 합니다")
                Print("Console", 0, "runtime - 엔진 작동 시간을 보여줍니다")
                Print("Console", 0, "clear - 콘솔창을 청소합니다")
                Print("Console", 0, "projexit - 프로그램을 완전히 종료합니다")
                Print("Console", 0, "echo <내용:문자열> - 메세지를 보여줍니다")
                Print("Console", 0, "debug - 디버그 권한을 취득합니다, 고급사용자에만 권장합니다")
                Print("Console", 2, "getstring - getstring 함수를 테스트 합니다")
                Print("Console", 0, "getadminpermission 또는 gap - 관리자 권한을 취득하여 프로그램을 재시작합니다")


                '여기까지 콘솔 명령어
                Print("Console", 0, "file 또는 f - 햄스터 파일 관리자를 호출합니다")
                Print("Console", 0, "network 또는 n - 네트워크 관리자 호출")
            Else

                concommand = Split(txtin.Text, " ")


                If LCase(concommand(0)) = "file" Or LCase(concommand(0)) = "f" Then

                    If LCase(txtin.Text) = "file" Then
                        GoTo hhelp
                    End If



                    If LCase(concommand(1)) = "help" Or LCase(concommand(1)) = "h" Then
hhelp:
                        HamsterEngine.File.HelpString()
                        Exit Sub
                    ElseIf LCase(concommand(1)) = "read" Or LCase(concommand(1)) = "r" Then
                        HamsterEngine.File.Readtext(concommand(2))
                    ElseIf LCase(concommand(1)) = "check" Or LCase(concommand(1)) = "c" Then
                        HamsterEngine.File.Check(concommand(2), False)
                    ElseIf LCase(concommand(1)) = "created" Or LCase(concommand(1)) = "crd" Then
                        HamsterEngine.File.Directory.Create(concommand(2))
                    ElseIf LCase(concommand(1)) = "deleted" Or LCase(concommand(1)) = "ded" Then
                        HamsterEngine.File.Directory.Delete(concommand(2), concommand(3))
                    ElseIf LCase(concommand(1)) = "getcreatetimed" Or LCase(concommand(1)) = "gctd" Then
                        HamsterEngine.File.Directory.GetCreatedTime(concommand(2), concommand(3))
                    ElseIf LCase(concommand(1)) = "exists" Or LCase(concommand(1)) = "e" Then
                        HamsterEngine.File.Directory.Check(concommand(2))
                    ElseIf LCase(concommand(1)) = "getdirectories" Or LCase(concommand(1)) = "gdrt" Then
                        HamsterEngine.File.Directory.GetChildDirectories(concommand(2))
                    ElseIf LCase(concommand(1)) = "getfiles" Or LCase(concommand(1)) = "gfls" Then
                        HamsterEngine.File.Directory.GetChildFiles(concommand(2))
                    ElseIf LCase(concommand(1)) = "getentries" Or LCase(concommand(1)) = "gent" Then
                        HamsterEngine.File.Directory.GetEntries(concommand(2))
                    ElseIf LCase(concommand(1)) = "getparentdirectory" Or LCase(concommand(1)) = "gpdt" Then
                        HamsterEngine.File.Directory.GetParentDirectory(concommand(2))
                    ElseIf LCase(concommand(1)) = "movedirectory" Or LCase(concommand(1)) = "m" Then
                        HamsterEngine.File.Directory.Move(concommand(2), concommand(3))
                    Else

                    End If
                ElseIf LCase(concommand(0)) = "network" Or LCase(concommand(0)) = "n" Then

                    If LCase(txtin.Text) = "network" Then
                        GoTo nhelp
                    End If

                    If LCase(concommand(1)) = "help" Or LCase(concommand(1)) = "h" Then
nhelp:
                        ConNetworkSoc.HelpString()
                        Exit Sub
                    ElseIf LCase(concommand(1)) = "Init" Or LCase(concommand(1)) = "i" Then
                        Dim listener As Boolean

                        If LCase(getstring(True, "소켓을 Server 로 초기화 합니까?, 거부시 Client (Y/N)")) = "y" Then
                            listener = True
                        Else
                            listener = False

                        End If

                        WarningMsg.release()
                        If Not listener Then
                            If concommand(2) = "s" Then
                                Print("Console", 1, "Server의 IP 모드로 Client를 초기화 할수 없습니다!")
                                Exit Sub
                            End If
                            ConNetworkSoc.Init(listener, concommand(2), concommand(3), Words.hNetwork.Socket.Client)
                        Else
                            If concommand(4) = "c" Then
                                Print("Console", 1, "Client의 클라이언트 허용 갯수 모드로 Server를 초기화 할수 없습니다!")
                                Exit Sub
                            End If
                            If LCase(concommand(4)) = "max" Then
                                ConNetworkSoc.Init(listener, Words.hNetwork.Socket.Server, concommand(3), HamsterEngine.Words.hNetwork.Socket.AcceptMAXClient)
                            ElseIf LCase(concommand(4)) = "min" Then
                                ConNetworkSoc.Init(listener, Words.hNetwork.Socket.Server, concommand(3), HamsterEngine.Words.hNetwork.Socket.AcceptMINClient)
                            Else
                                ConNetworkSoc.Init(listener, Words.hNetwork.Socket.Server, concommand(3), concommand(4))
                            End If
                        End If
                    ElseIf LCase(concommand(1)) = "send" Or LCase(concommand(1)) = "s" Then
                        ConNetworkSoc.Send(concommand(2), concommand(3))
                    ElseIf LCase(concommand(1)) = "connect" Or LCase(concommand(1)) = "c" Then
                        ConNetworkSoc.Connect()
                    ElseIf LCase(concommand(1)) = "listen" Or LCase(concommand(1)) = "l" Then
                        ConNetworkSoc.SetListen()
                    ElseIf LCase(concommand(1)) = "shutdown" Or LCase(concommand(1)) = "shdw" Then
                        ConNetworkSoc.Shutdown()
                    End If


                ElseIf LCase(concommand(0)) = "echo" Then
                    If UBound(concommand) > 2 Then
                        Print("Console", 1, "인자 갯수가 너무 많습니다! 메세지를 표시할수 없습니다")
                        Exit Sub


                    End If
                    Print("Console", 0, concommand(1))

                ElseIf LCase(concommand(0)) = "msg" Then
                    Engine.SendAllClient(Words.ParseString & "MSG" & Words.ParseString & concommand(1))
                Else
                    Print("Console", 0, lasttext & " 는 정의되지 않은 명령입니다.")
                End If




            End If
            txtinpushed = True



            txtin.Text = ""
            txtinpushed = False
        Catch ex As IndexOutOfRangeException
            Print("Console", 2, ex.ToString)
            Print("Console", 1, "정의되지 않은 명령이거나 명령 인수가 너무 적거나 많습니다")
        Catch ex As Exception
            'Print("Console", 1, "오류 " & Str(Err.Number) & "번 " & Err.Source & " 에 의해 발생됨, 사유 : " & Err.Description)
            ErrorReporter.ReportError(ex, Words.hName.ModuleName.Console, "ConsoleInputCheak")
            Print("Console", 1, "오류 발생, 자세한 내용은 예외처리기를 확인하세요")

        Finally
            txtin.Text = ""
        End Try

    End Sub

    Private Sub txtin_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtin.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then
                If Not txtin.Text = "" Then
                    Call consoleinput_Click(Nothing, Nothing)

                End If
            ElseIf e.KeyCode = Keys.Up Then
                txtin.Text = lasttext

            End If
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "txtIn_Keydown")
        End Try
    End Sub

    Private Sub txtin_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtin.TextChanged

    End Sub

    Private Sub Reftimer_Tick(sender As System.Object, e As System.EventArgs) Handles Reftimer.Tick
        projruntime = projruntime + 1

    End Sub

    Public Sub enalbeobject(objecttype As Object, objectname As Object, fromcommand As Boolean)
        'objecttype = 개체 형식 인수 : form,timer
        'objectname = 개체이름
        'fromobject = 명령어에 의한 호출인지 정의
    End Sub


    Public Function getstring(showhelpmsg As Boolean, helpmsg As String)
        Try
            Dim returntext

            If Not lasttext = Words.hString.NullString Then
                lasttext = Words.hString.NullString
                Print("Console", 2, "lasttext를 " & Words.hString.NullString & "값으로 재 초기화 했습니다")
            End If

            If showhelpmsg = True Then
                Print("Console", 0, helpmsg)
                WarningMsg.Print(1, "계속 진행하려면, 콘솔에 값을 입력하세요")
            End If
            txtin.Text = ""
            waitforinput = True
            Do Until txtinpushed
                Application.DoEvents()
            Loop
            waitforinput = False

            txtinpushed = False

            returntext = lasttext
            lasttext = Words.hString.NullString

            Return returntext
            Exit Function
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "getString")
        End Try
    End Function

    Public Function getadminwinpermission()
        Try
            Dim user = System.Security.Principal.WindowsIdentity.GetCurrent
            Dim winpermission As System.Security.Principal.WindowsPrincipal = New System.Security.Principal.WindowsPrincipal(user)

            Dim request As String

            If Not winpermission.IsInRole("Administrators") Then
                Dim procInfo = New ProcessStartInfo()
                procInfo.UseShellExecute = True
                procInfo.FileName = Application.ExecutablePath
                procInfo.WorkingDirectory = Environment.CurrentDirectory
                procInfo.Verb = "runas"


                request = getstring(True, "관리자 권한을 취득하기 위해 프로그램을 정말 재시작 하시겠습니까? (Y/N)")
                If LCase(request) = "y" Then
                    Process.Start(procInfo)
                    End
                ElseIf LCase(request) = "n" Then
                    Print("Console", 0, "작업은 진행되지 않았습니다")
                Else
                    Print("Console", 1, "알수 없는 응답입니다")
                End If
            End If
        Catch exp As Exception
            ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "gap")
        End Try
    End Function

    Public Class WarningMsg
        Private Shared EraseTimer As System.Timers.Timer
        Delegate Sub Trelease()

        Public Shared Function Print(level As Byte, msg As String, Optional eraseSec As Byte = 0)
            '콘솔에 경고 메세지를 표시합니다, eraseSec는 선택적 인수로, 인스턴트 메세지를 표시하고 싶을떄 인수의 초후 release 됩니다
            '0 = 긍정, 1 = 경고, 2 = 치명적 경고
            If conlist.InvokeRequired Then
                Dim printdel As warnprint = New warnprint(AddressOf Print)
                conlist.BeginInvoke(printdel, level, msg, eraseSec)

            Else
                If eraseSec > 0 Then
                    EraseTimer = New System.Timers.Timer(eraseSec * 1000)
                    EraseTimer.AutoReset = False
                    AddHandler EraseTimer.Elapsed, AddressOf EraseTimerEVT
                    EraseTimer.Enabled = True
                End If

                Console.warnmsg.Text = msg

                If level = 0 Then
                    Console.warnmsg.BackColor = Color.Blue
                ElseIf level = 1 Then
                    Console.warnmsg.BackColor = Color.Yellow
                ElseIf level = 2 Then
                    Console.warnmsg.BackColor = Color.Red
                Else
                    Return Words.hException.General
                End If
            End If
        End Function

        Private Shared Sub EraseTimerEVT()
            'EraseTimer과 관련된 함수입니다, 수정하지 마십시요
            '수정시 정상적인 동작을 보증하지 않습니다

            release()
        End Sub

        Public Shared Sub release()
            '크로스 스레드 동작을 위한 invoke 문이 포함되어 있습니다, 수정하지 마십시요
            '수정시 정상적인 동작을 보증하지 않습니다
            If warnmsg.InvokeRequired Then
                Dim para As Object() = {}
                Dim rel As Trelease = New Trelease(AddressOf release)
                warnmsg.BeginInvoke(rel)

            Else
                Console.warnmsg.Text = Nothing
                Console.warnmsg.BackColor = Color.Transparent
            End If
        End Sub
    End Class




    Public Class Log
        Private Shared startlog As Boolean = False ' 로그 작성여부
        Public Shared lognum As Integer = 1 ' 로그 번호
        Public Shared logpath As String ' 로그 경로

        Public Shared Sub Start()
            Try
                If HamsterEngine.File.Directory.Check(Application.StartupPath & "\log") Then
                    Console.Print("Console", 0, "log 디렉터리 발견됨, 경로에 로그를 작성합니다")
                Else
                    Console.Print("Console", 0, "log 디렉터리 찾을수 없음, log 디렉터리를 생성합니다")
                    HamsterEngine.File.Directory.Create(Application.StartupPath & "\log")
                End If

                Do While True
                    If HamsterEngine.File.Check(Application.StartupPath & "\log\" & DateString & "-" & lognum & ".log", True) Then

                        lognum = lognum + 1
                    Else
                        Exit Do
                    End If
                Loop

                logpath = Application.StartupPath & "\log\" & DateString & "-" & lognum & ".log"
                HamsterEngine.File.writetext(logpath, "로그파일 생성됨, 시간: " & Now & " 로그번호 : " & lognum, False, True)
                Console.Print("Console", 0, Log.logpath & " 에서 로그 시작")


                startlog = True
            Catch exp As Exception
                ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "Log\start")
            End Try
        End Sub

        Public Shared Sub write(data As String)
            Try
                If startlog Then
                    HamsterEngine.File.writetext(logpath, data, True, True)
                End If
            Catch exp As Exception
                ErrorReporter.ReportError(exp, Words.hName.ModuleName.Console, "Log\write")
            End Try
        End Sub

    End Class


End Class