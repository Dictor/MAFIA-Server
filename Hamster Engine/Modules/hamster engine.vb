Imports System.Exception
Imports HamsterEngine.Words
Imports HamsterEngine.Networkmanager
Imports System.Net.Sockets
Imports System.Net
Imports System.Text

' Engine.ServerListenEvent(i, CompleteData)
Public Class Engine
    Public Shared Version As New HamsterVersion("Hamster Engine", 1, 0, 140125, 0)
    Public Shared ProjVersion As New HamsterVersion("MAFIA Server x86", 1, 1, 150201, 8) '이 부분을 수정하여 프로젝트 버젼을 수정하세요

    '여기부터 프로젝트 변수
    Private Shared ServerSocket As New MafiaServerSocket '서버 소켓
    Public Shared Nickname(0) As String, ClientName(0) As String, ClientVersion(0) As String, ClientSocketNum(0) As String '플레이어 변수
    Private Shared GamestartVoteAmount As Integer = 0 '게임 시작 투표 량
    Private Shared rRandom As New Random

    Private Shared GameTime As Integer = 60
    Private Shared GameTimeRefTimer As New System.Timers.Timer(1024)
    Private Shared Days As Integer = 1, IsDay As Boolean = False, IsVoted As Boolean = False
    Private Shared GameStarted As Boolean = False

    Private Const StandardDaytime As Integer = 200, StandardNighttime As Integer = 60, StandardVotetime As Integer = 50

    Private Shared deadPlayer As Integer = -1
    Private Shared ghostPlayer As Integer = -1 '영매와 이어질 플레이어

    Private Shared VoteList(11) As Byte

    Private Shared PlayernumTOClisockNum As Integer()  '플레이어 번호에 대한 클라소켓 번호(이배열은 1부터 시작합니다)

    Private Shared Job As String() '플레이어 직업(이배열은 1부터 시작합니다)
    Private Shared sortingjob() As String '배정될 플레이어 직업

    Private Shared computerPlayer As Integer

    Private Shared Policeskillcache(2) As String '경찰의 스킬 캐시입니다 (스킬 대상 플레이어,대상의 세력, 경찰의 소켓번호)

    Private Shared MafiaAmount As Integer = 0, CitizenAmount As Integer = 0

    Private Shared AdminPassword As String = "ADMINPASS" '관리자 권한 얻기 비밀번호
    Private Shared GetAdminPlayer As String '관리자 권한 얻기 시도중 플레이어
    Private Shared AdminPlayer As String = -1 '관리자 플레이어


    Public Shared Sub initialization()
        '이 부분에는 초기화가 필요한 메서드, 변수등을 초기화 할 내용을 적어주세요
        'start 함수에서 시작됩니다
        HamsterEngine.Console.Log.Start()
    End Sub

    Public Shared Sub start()
        '이 부분에는 엔진을 시작할때 필요한 내용을 적어주세요
        initialization()
        ServerStart()
        Policeskillcache(0) = -1
    End Sub

    Public Shared Sub shutdown()
        '이 부분에는 엔진이 종료될떄 필요한 내용을 적어주세요
        Console.Log.write("엔진 셧다운 / " & Console.projruntime & "ms")
        End
    End Sub

    Public Shared Sub ServerStart() '서버 시작
        ServerSocket.Init(True, "127.0.0.1", 63322, 254)
        ServerSocket.SetListen()
        AddHandler GameTimeRefTimer.Elapsed, AddressOf RefGameTime
    End Sub

    Public Shared Sub ServerListenEvent(ClientNumber As Integer, Data As String)
        Data = Replace(Data, Words.ParseString, "๑")
        Dim pData = Split(Data, "๑") '파싱된 데이터
        Try
            If pData(0) = "JOIN" Then
                If GameStarted Then
                    ServerSocket.tSend("JOINFAIL" & Words.ParseString & "GAMEALREADYSTARTED", ClientNumber)
                    Exit Sub
                End If
                ReDim Preserve Nickname(UBound(Nickname) + 1)
                ReDim Preserve ClientName(UBound(ClientName) + 1)
                ReDim Preserve ClientVersion(UBound(ClientVersion) + 1)
                ReDim Preserve ClientSocketNum(UBound(ClientSocketNum) + 1)

                Nickname(UBound(Nickname) - 1) = pData(1)
                ClientName(UBound(ClientName) - 1) = pData(2)
                ClientVersion(UBound(ClientVersion) - 1) = pData(3)
                ClientSocketNum(UBound(ClientVersion) - 1) = ClientNumber

                Console.Print("Server", 0, "클라이언트가 접속했습니다!!;클라 소켓번호는 " & ClientSocketNum(UBound(ClientVersion) - 1) & "번이고, 닉네임 : '" & Nickname(UBound(Nickname) - 1) & "' 이고 클라 버젼은 " & ClientVersion(UBound(ClientVersion) - 1) & ",클라 이름은 " & ClientName(UBound(ClientName) - 1) & "이고 클라 정보는 " & UBound(ClientSocketNum) - 1 & "번에 할당되었습니다", "ListenEvent")
                ServerSocket.tSend("JOINOK", ClientNumber)

                Dim i As Integer
                Dim SPlayer = "REFPLAYER", SVersion = "REFVERSION", SCliname = "REFCLINAME"

                Do While True
                    If Not i >= UBound(Nickname) Then
                        SPlayer = SPlayer + ParseString + Nickname(i)
                        SVersion = SVersion + ParseString + ClientVersion(i)
                        SCliname = SCliname + ParseString + ClientName(i)
                        Console.Print("Engine", 2, SPlayer, "ServerListenEvent")
                    Else
                        Exit Do
                    End If
                    i = i + 1
                Loop

                Dim i2 As Integer = 0

                SendAllClient(SPlayer)
                System.Threading.Thread.Sleep(200)
                SendAllClient(SCliname)
                System.Threading.Thread.Sleep(200)
                SendAllClient(SVersion)

            ElseIf pData(0) = "CHAT" Then
                If Not GameStarted Then

                    If LCase(pData(1)) = "getadmin" Then
                        If AdminPlayer = -1 Then
                            ServerSocket.tSend("CHAT" & ParseString & "[서버(관리자)]관리자 권한을 얻기 위해서는 비밀번호를 입력해주세요", ClientNumber)
                            SendAllClient("CHAT" & ParseString & "[서버]" & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & "님이 관리자 권한을 얻으려 시도하고 있습니다")
                            GetAdminPlayer = ClientNumber
                            Exit Sub
                        Else
                            ServerSocket.tSend("CHAT" & ParseString & "[서버(관리자)]이미 관리자가 존재합니다", ClientNumber)
                            Exit Sub
                        End If
                    End If

                    If ClientNumber = GetAdminPlayer And AdminPlayer = -1 Then
                        If pData(1) = AdminPassword Then
                            AdminPlayer = GetAdminPlayer
                            SendAllClient("CHAT" & ParseString & "[서버]" & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & "님은 이제 관리자 입니다")
                            Exit Sub
                        End If
                    End If

                    Try
                        Dim pcommand = Split(pData(1))
                        If ClientNumber = AdminPlayer Then
                            If LCase(pcommand(0)) = "list" Then

                                Dim i As Integer

                                Do While True
                                    ServerSocket.tSend("CHAT" & ParseString & "[서버 - LIST(관리자)]" & ClientSocketNum(i) & " - " & Nickname(i) & " - " & ServerSocket.ClientSocCache(ClientSocketNum(i)).RemoteEndPoint.ToString, AdminPlayer)
                                    i = i + 1

                                    If i > UBound(ClientSocketNum) - 1 Then
                                        Exit Do
                                    End If
                                Loop
                                Exit Sub
                            ElseIf LCase(pcommand(0)) = "kick" Then
                                ServerSocket.dropPlayer(pcommand(1), 1)
                                Exit Sub
                            End If


                        End If
                    Catch
                    End Try

                    SendAllClient("CHAT" & ParseString & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & " : " & pData(1))
                Else
                    Dim i As Integer = 1
                    Dim j As Integer = 1
                    Do While True
                        If PlayernumTOClisockNum(i) = ClientNumber Then
                            If IsDay Then
                                SendAllClient("CHAT" & ParseString & i & "플레이어 : " & pData(1))
                            Else
                                If pData(2) = "MAFIA" Then
                                    Do While True
                                        If j > UBound(Job) Then
                                            Exit Do
                                        End If
                                        If Job(j) = "MAFIA" Then
                                            ServerSocket.tSend("CHAT" & ParseString & i & "플레이어 : " & pData(1), PlayernumTOClisockNum(j))
                                        End If
                                        j = j + 1
                                    Loop
                                ElseIf pData(2) = "GHOSTMAN" Or pData(2) = "GHOST" Then
                                    j = 1
                                    Do While True
                                        If j > UBound(Job) Then
                                            Exit Do
                                        End If
                                        If Job(j) = "GHOSTMAN" Or Job(j) = "GHOST" Then
                                            ServerSocket.tSend("CHAT" & ParseString & i & "플레이어 : " & pData(1), PlayernumTOClisockNum(j))
                                        End If
                                        j = j + 1
                                    Loop
                                End If
                            End If
                            If pData(2) = "DEATH" Then
                                j = 1
                                Do While True
                                    If j > UBound(Job) Then
                                        Exit Do
                                    End If
                                    If Job(j) = "DEATH" Then
                                        ServerSocket.tSend("CHAT" & ParseString & "[사망]" & i & "플레이어(" & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & ") : " & pData(1), PlayernumTOClisockNum(j))
                                    End If
                                    j = j + 1
                                Loop
                            End If
                            Exit Do
                        End If
                    Loop
                End If
            ElseIf pData(0) = "EXIT" Then
                ServerSocket.dropPlayer(ClientNumber, 2)
            ElseIf pData(0) = "INFO" Then
                ServerSocket.tSend("INFO" & ParseString & "서버 이름 : " & ProjVersion.GetName & vbCrLf & "서버 버젼 : " & ProjVersion.GetVersion(True) & vbCrLf & "서버 시간 : " & Now.ToString, ClientNumber)
            ElseIf pData(0) = "VOTE" Then
                GamestartVoteAmount = GamestartVoteAmount + 1
                SendAllClient("CHAT" & ParseString & "[서버]" & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & "님이 게임을 시작하자고 투표하셨습니다.")
                SendAllClient("VOTERES" & ParseString & GamestartVoteAmount.ToString)
                If GamestartVoteAmount = UBound(Nickname) Then
                    If GamestartVoteAmount < 5 Then
                        System.Threading.Thread.Sleep(200)
                        SendAllClient("CHAT" & ParseString & "[서버]만장일치로 투표가 인정되나, 전체 플레이어가 5명 이하로 게임을 시작할수 없습니다!")
                    ElseIf GamestartVoteAmount >= 5 And GamestartVoteAmount <= 6 Then
                        SendAllClient("CHAT" & ParseString & "[서버]만장일치로 게임을 시작합니다!")
                        GameStart(0, UBound(Nickname), 1, UBound(Nickname) - 3, {1, 1})
                    End If
                End If
            ElseIf pData(0) = "VOTECANCLE" Then
                GamestartVoteAmount = GamestartVoteAmount - 1
                SendAllClient("CHAT" & ParseString & "[서버]" & Nickname(SearchStringAtArray(ClientSocketNum, ClientNumber.ToString)) & "님이 게임을 시작 투표를 취소하셨습니다")
                SendAllClient("VOTERES" & ParseString & GamestartVoteAmount.ToString)
                System.Threading.Thread.Sleep(30)
            ElseIf pData(0) = "TIMEPLUS" Then
                GameTime = GameTime + 45
                SendAllClient("TIMEREF" & ParseString & GameTime)
                Dim i As Integer = 1
                Do While True
                    If PlayernumTOClisockNum(i) = ClientNumber Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & i & "플레이어가 시간을 연장했습니다")
                        Exit Do
                    End If
                    i = i + 1
                Loop
            ElseIf pData(0) = "TIMEMINUS" Then
                GameTime = GameTime - 45
                SendAllClient("TIMEREF" & ParseString & GameTime)
                Dim i As Integer = 1
                Do While True
                    If PlayernumTOClisockNum(i) = ClientNumber Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & i & "플레이어가 시간을 단축했습니다")
                        Exit Do
                    End If
                    i = i + 1
                Loop
            ElseIf pData(0) = "DVOTERES" Then
                Dim voteRes As Integer = pData(1)
                VoteList(voteRes - 1) = VoteList(voteRes - 1) + 1
                SendAllClient("DVOTEREF" & ParseString & VoteList(0) & ParseString & VoteList(1) & ParseString & VoteList(2) & ParseString & VoteList(3) & ParseString & VoteList(4) & ParseString & VoteList(5) & ParseString & VoteList(6) & ParseString & VoteList(7) & ParseString & VoteList(8) & ParseString & VoteList(9) & ParseString & VoteList(10) & ParseString & VoteList(11))

                Dim i As Integer = 1
                Do While True
                    If PlayernumTOClisockNum(i) = ClientNumber Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & i & "플레이어가 " & voteRes & "플레이어에게 투표했습니다")
                        Exit Do
                    End If
                    i = i + 1
                Loop

            ElseIf pData(0) = "MEMO" Then
                Dim i As Integer = 1
                Do While True
                    If PlayernumTOClisockNum(i) = ClientNumber Then
                        SendAllClient("MEMO" & ParseString & "[" & i & "] " & pData(1))
                        Exit Do
                    End If
                    i = i + 1
                Loop


            ElseIf pData(0) = "SKILL" Then
                If pData(1) = "MAFIA" Then
                    deadPlayer = pData(2)
                ElseIf pData(0) = "MEDIC" Then
                    If Not deadPlayer = -1 Then
                        If deadPlayer = pData(2) Then
                            deadPlayer = -2
                        End If
                    End If
                ElseIf pData(0) = "POLICE" Then
                    Policeskillcache(0) = pData(2)
                    Policeskillcache(2) = ClientNumber
                    If Job(pData(2)) = "MAFIA" Then
                        Policeskillcache(1) = 0
                    Else
                        Policeskillcache(1) = 1
                    End If
                End If
            ElseIf pData(0) = "SCHAT" Then
                ServerSocket.tSend("[YOU->" & pData(2) & "]" & pData(1), ClientNumber)
                Dim i As Integer = 1
                Do While True
                    If PlayernumTOClisockNum(i) = ClientNumber Then
                        ServerSocket.tSend("[" & i & "-> YOU]" & pData(1), PlayernumTOClisockNum(pData(2)))
                        Exit Do
                    End If
                    i = i + 1
                Loop

            End If
        Catch ex As Exception
            If pData(0) = "JOIN" Then
                ServerSocket.tSend("JOINFAIL" & Words.ParseString & "INTERNALSERVERERR", ClientNumber)
            End If
            Console.Print("Server", 0, ClientNumber & "번 소켓과 연결 도중 오류발생!")
            Console.Print("Server", 0, ex.ToString)
        End Try

    End Sub



    Public Shared Sub GameStart(mode As Byte, playerAmount As Integer, Mafia As Integer, citizen As Integer, Optional otherJob() As Integer = Nothing)
        'mode 가 0 = 기본 모드
        'otherjob (의사,경찰,영매)

        ReDim PlayernumTOClisockNum(playerAmount)
        ReDim Job(playerAmount)
        Console.Print("Server", 0, "게임시작")
        Console.Print("Server", 0, "플레이어 숫자 : " & playerAmount & " 마피아 숫자 : " & Mafia & " 시민 숫자 : " & citizen)
        GameStarted = True
        MafiaAmount = Mafia
        CitizenAmount = citizen + otherJob(0) + otherJob(1) + otherJob(2)
        Dim playernum As Integer = 1
        If mode = 0 Then
            Dim i As Integer
            Do While True
                If i < Mafia Then
                    sortingjob(playernum) = "MAFIA"
                ElseIf i < Mafia + citizen Then
                    sortingjob(playernum) = "CITIZEN"
                ElseIf i < Mafia + citizen + otherJob(0) Then
                    sortingjob(playernum) = "MEDIC"
                ElseIf i < Mafia + citizen + otherJob(0) + otherJob(1) Then
                    sortingjob(playernum) = "POLICE"
                ElseIf i < Mafia + citizen + otherJob(0) + otherJob(1) + otherJob(2) Then
                    sortingjob(playernum) = "GHOSTMAN"
                End If
                If i > playerAmount - 1 Then
                    Exit Do
                End If
                i = i + 1
                playernum = playernum + 1
                computerPlayer = playerAmount + 1
            Loop

            Dim jobrand As New Random()
            Dim j As Integer, f As Integer = 0
            playernum = 1

            Do While True
                j = jobrand.Next(1, UBound(sortingjob))
                Job(f + 1) = sortingjob(j)
                Console.Print("Server", 0, (f + 1) & "플레이어의 직업 : " & Job(f + 1))
                sortingjob = RemoveArrayData(sortingjob, j)
                f = f + 1
            Loop
            ' ServerSocket.tSend("GAMESTART" & ParseString & "MAFIA" & ParseString & UBound(Nickname) + 1 & ParseString & i, ClientSocketNum(i))
            'Console.Print("Server", 0, playernum & "번 플레이어는 마피아 입니다")
            'ClisockTOPlayernum(playernum) = ClientSocketNum(i)


            IsDay = False
            System.Threading.Thread.Sleep(200)
            SendAllClient("NIGHT" & ParseString & Days & ParseString & GameTime)
            GameTimeRefTimer.Start()
        End If
    End Sub

    Private Shared Sub RefGameTime()
        GameTime = GameTime - 1
        If GameTime < 0 Then
            GameTimeRefTimer.Stop()
            Console.Print("Server", 0, "isday : " & IsDay & " isvoted : " & IsVoted & " days : " & Days)
            Console.Print("Server", 0, "MAFIA : " & MafiaAmount & " CITIZEN : " & CitizenAmount)
            If Days = 1 Then
                SendAllClient("DEATH" & ParseString & computerPlayer)
                System.Threading.Thread.Sleep(200)
                SendAllClient("MEMO" & ParseString & "<사망 - " & computerPlayer & ">")
                CitizenAmount = CitizenAmount - 1
            End If
            If IsDay And IsVoted Then
                SendAllClient("ENDDVOTE" & ParseString & GameTime)
                System.Threading.Thread.Sleep(200)
                Dim i As Integer, highest As Integer, votedplayer As Integer
                Do While True
                    If highest < VoteList(i) Then
                        highest = VoteList(i)
                        votedplayer = i + 1
                    End If
                    i = i + 1

                    If i > UBound(VoteList) Then
                        Exit Do
                    End If
                Loop

                SendAllClient("DEATH" & ParseString & votedplayer)
                System.Threading.Thread.Sleep(200)
                SendAllClient("YOUDEATH" & ParseString & PlayernumTOClisockNum(votedplayer))
                System.Threading.Thread.Sleep(200)
                SendAllClient("MEMO" & ParseString & "<사형 - " & votedplayer & ">")
                If Job(votedplayer) = "MAFIA" Then
                    MafiaAmount = MafiaAmount - 1
                Else
                    CitizenAmount = CitizenAmount - 1
                End If
                System.Threading.Thread.Sleep(200)
                IsDay = False
                IsVoted = False
                GameTime = StandardNighttime
                SendAllClient("NIGHT" & ParseString & Days & ParseString & GameTime)
            ElseIf IsDay And Not IsVoted Then
                IsVoted = True
                GameTime = StandardVotetime
                SendAllClient("TIMEREF" & ParseString & GameTime)
                System.Threading.Thread.Sleep(200)
                SendAllClient("STARTDVOTE" & ParseString & GameTime)
                System.Threading.Thread.Sleep(200)
                SendAllClient("CHAT" & ParseString & "[서버]투표를 시작합니다.")
            ElseIf Not IsDay Then
                Days = Days + 1
                GameTime = StandardDaytime
                System.Threading.Thread.Sleep(500)
                SendAllClient("DAY" & ParseString & Days & ParseString & GameTime)
                IsDay = True
                If Not deadPlayer = -1 Then
                    If Days = 2 Then
                        ghostPlayer = deadPlayer
                    End If
                    SendAllClient("DEATH" & ParseString & deadPlayer)
                    System.Threading.Thread.Sleep(200)
                    Job(deadPlayer) = "DEATH"
                    ServerSocket.tSend("YOUDEATH", PlayernumTOClisockNum(deadPlayer)) '이부분 클라에는 작업이 안되어있음, 작업할것!! 그리고 전체전송이 아니라 개인전송으로 고칠것
                    ServerSocket.tSend("CHAT" & ParseString & "[서버]당신은 사망했습니다", PlayernumTOClisockNum(deadPlayer))
                    System.Threading.Thread.Sleep(200)
                    SendAllClient("MEMO" & ParseString & "<사망 - " & deadPlayer & ">")
                    ServerSocket.tSend("CHAT" & ParseString & "[서버]당신은 이제부터 밤마다 영매와 대화할수 있습니다!", PlayernumTOClisockNum(deadPlayer))
                    If Job(deadPlayer) = "MAFIA" Then
                        MafiaAmount = MafiaAmount - 1
                    Else
                        CitizenAmount = CitizenAmount - 1
                    End If
                    deadPlayer = -1


                ElseIf deadPlayer = -2 Then
                    SendAllClient("MEMO" & ParseString & "<사망 - 치료됨>")
                    System.Threading.Thread.Sleep(200)
                    SendAllClient("MEDICSKILL")
                End If

                If MafiaAmount = 0 Then
                    SendAllClient("CHAT" & ParseString & "[서버]시민 승리!.")
                    System.Threading.Thread.Sleep(200)
                    Gameover()
                ElseIf CitizenAmount <= MafiaAmount Then
                    SendAllClient("CHAT" & ParseString & "[서버]마피아 승리!.")
                    System.Threading.Thread.Sleep(200)
                    Gameover()
                End If

                If Not Policeskillcache(0) = -1 Then
                    If Policeskillcache(1) = 0 Then
                        ServerSocket.tSend("CHAT" & ParseString & "[서버-스킬사용]" & Policeskillcache(0) & "플레이어는 마피아 세력입니다", Policeskillcache(2))
                        System.Threading.Thread.Sleep(200)
                        ServerSocket.tSend("SKILLRES" & ParseString & "POLICE" & ParseString & Policeskillcache(0) & ParseString & 0, Policeskillcache(2))
                    Else
                        ServerSocket.tSend("CHAT" & ParseString & "[서버-스킬사용]" & Policeskillcache(0) & "플레이어는 시민 세력입니다", Policeskillcache(2))
                        System.Threading.Thread.Sleep(200)
                        ServerSocket.tSend("SKILLRES" & ParseString & "POLICE" & ParseString & Policeskillcache(0) & ParseString & 1, Policeskillcache(2))
                    End If
                    Policeskillcache(0) = -1
                End If
            End If
        End If
        GameTimeRefTimer.Start()
    End Sub

    Public Shared Sub Gameover()
        Dim gameoverstr As String = "GAMEOVER"
        Dim i As Integer = 1

        Do While True
            gameoverstr = gameoverstr & ParseString & Job(i)
            i = i + 1

            If i > UBound(Job) Then
                Exit Do
            End If
        Loop

        SendAllClient(gameoverstr)
    End Sub


    Public Shared Function makeRandom(dataamount As Integer, max As Integer, Optional min As Integer = 0) As Integer()
        Try
            Dim RandomData(dataamount - 1) As Integer
            Dim i As Integer

            Do While True
retry:
                RandomData(i) = rRandom.Next(min, max)

                Dim i2 As Integer

                Do While True
                    If Not UBound(RandomData) = 0 Then
                        If i2 > i Then
                            Exit Do
                        End If
                    End If

                    If RandomData(i) = RandomData(i2) And i = i2 = 0 Then
                        GoTo retry
                    End If

                    i2 = i2 + 1




                Loop

                i = i + 1

                If i = dataamount Then
                    Return RandomData
                    Exit Function
                End If
            Loop
        Catch ex As Exception
            ErrorReporter.ReportError(ex, "Server")
        End Try
    End Function

    Public Shared Function RemoveNullArray(source() As String) As String()
        Dim i As Integer
        Dim NotnullDataNum(1024) As Integer
        Dim NotnullDataAmount As Integer

        Do While True
            If i > UBound(source) Then
                Exit Do
            End If

            If Not source(i) = Nothing Then
                NotnullDataNum(NotnullDataAmount) = i
                NotnullDataAmount = NotnullDataAmount + 1
                Console.Print("Server", 2, "Null이 아닌 데이터 : " & source(i) & " , 번호 : " & i)
            End If
            i = i + 1
        Loop

        i = 0
        Dim RemovedData(NotnullDataAmount) As String

        Do While True
            RemovedData(i) = source(NotnullDataNum(i))
            i = i + 1

            If i > UBound(NotnullDataNum) Then
                Exit Do
            End If
            Return RemovedData
        Loop
    End Function

    Private Shared Function SearchStringAtArray(data As String(), searchstring As String) As Integer
        Dim i As Integer = 0
        Dim Dataindex As Integer

        Do While True
            If data(i) = searchstring Then
                Dataindex = i
                Return Dataindex
                Exit Do
            End If

            i = i + 1

            If i > UBound(data) Then
                Dataindex = -1
            End If

        Loop
    End Function

    Public Shared Function RemoveArrayData(source As Object(), index As Integer)
        Dim returnArray(UBound(source) - 1) As Object

        Dim si As Integer, ri As Integer

        Do While True

            If si = index Then
                si = si + 1
            Else
                returnArray(ri) = source(si)
                si = si + 1
                ri = ri + 1
            End If

            If si > UBound(source) Then
                Return returnArray
                Exit Do
                Exit Function
            End If
        Loop

    End Function

    Public Shared Function RemoveArrayintData(source As Integer(), index As Integer)
        Dim returnArray(UBound(source) - 1) As Object

        Dim si As Integer, ri As Integer

        Do While True

            If si = index Then
                si = si + 1
            Else
                returnArray(ri) = source(si)
                si = si + 1
                ri = ri + 1
            End If

            If si > UBound(source) Then
                Return returnArray
                Exit Do
                Exit Function
            End If
        Loop

    End Function

    Public Shared Sub SendAllClient(data As String)
        Dim i As Integer


        Do While True
            If i > UBound(ClientSocketNum) - 1 Then
                Exit Do
            End If

            ServerSocket.tSend(data, ClientSocketNum(i))
            i = i + 1



        Loop

    End Sub

    Public Shared Sub ServerStop() '서버 종료
        ServerSocket.Shutdown()
    End Sub


    Public Class HamsterVersion '버전정보를 담기위한 클래스
        Private StrVersion As String '문자열 버젼
        Private MajorVersion, MinorVersion, BuildNumber, ChangeNumber As Integer '메이저버젼,마이너버전,빌드번호,수정번호
        Private ModuleName As String '모듈이름

        Private setStatus As Boolean = False '초기화 되지 않았거나, 정의하지 않았거나, 비포함의 경우 FALSE
        Private setStatusEXP As Byte = 0 'setStatus가 false일경우, 0=초기화 되지않음, 1=정의하지 않음, 2=포함하지 않음


        Public Sub New(ModuleNam As String, MajorVer As Byte, MinorVer As Byte, BuildNum As ULong, ChangeNum As Integer)
            '버젼을 초기화 합니다         
            setStatus = True

            ModuleName = ModuleNam
            MajorVersion = MajorVer
            MinorVersion = MinorVer
            BuildNumber = BuildNum
            ChangeNumber = ChangeNum
        End Sub


        Public Sub SetVersion(NotDefine As Boolean)
            '정의하지 않았거나, 포함하지 않았을 경우 FALSE를 넘겨주면 비포함, TRUE는 정의되지 않음 입니다
            MajorVersion = 0
            MinorVersion = 0
            BuildNumber = 0
            ChangeNumber = 0

            If NotDefine Then
                setStatus = False
                setStatusEXP = 1
            Else
                setStatus = False
                setStatusEXP = 2
            End If
        End Sub

        Public Function GetVersion(returnString As Boolean)
            'returnString이 TRUE 일경우 문자열로 버젼을 반환하며, FALSE 일 경우 정수 배열로 반환합니다
            If setStatusEXP = 0 And setStatus = False Then
                Throw New VersionNotSetException
            End If

            If returnString Then
                Dim ver As String = MajorVersion & "." & MinorVersion & "." & BuildNumber & "." & ChangeNumber
                Return ver
            Else
                Dim ver As Integer() = {MajorVersion, MinorVersion, BuildNumber, ChangeNumber}
                Return ver
            End If
        End Function

        Public Function GetName()
            Return ModuleName
        End Function

    End Class

    '햄스터 엔진 커스텀 예외
    Public Class VersionNotSetException
        Inherits Exception

        Public Const expMessage As String = "[햄스터 엔진 내부 예외]버젼이 초기화되지 않았습니다, 초기화되지 않은 버젼객체에 엑세스하려고 했습니다"

        Sub New()
            MyBase.New(expMessage)
        End Sub


    End Class


    Public Class MafiaServerSocket
        Inherits Networkmanager

        Public Sub tSend(data As String, targetSocketAddress As Integer)
            Try
                Send(data, targetSocketAddress)
            Catch ex As SocketException
                dropPlayer(targetSocketAddress, 0)
            Catch exp As Exception
                ErrorReporter.ReportError(exp, Words.hName.ModuleName.NetworkManager, "Send")
            End Try
        End Sub

        Public Sub dropPlayer(targetSocketAddress As Integer, reason As Byte)
            Dim i3 As Integer = 0
            Do While True
                If ClientSocketNum(i3) = targetSocketAddress Then
                    Try
                        Send("KICK", targetSocketAddress)
                    Catch
                        Console.Print("NetworkManager", 1, targetSocketAddress & "번 클라이언트 강퇴 신호 전송 실패")
                    End Try
                    Dim quitName As String = Nickname(i3)
                    Dim i5 As Integer = 1
                    Do While True
                        If PlayernumTOClisockNum(i5) = ClientSocketNum(i3) Then
                            PlayernumTOClisockNum = RemoveArrayintData(PlayernumTOClisockNum, i5)
                            Job = RemoveArrayData(Job, i5)
                            Exit Do
                        End If
                        i5 = i5 + 1
                    Loop

                    Nickname = RemoveArrayData(Nickname, i3)
                    ClientName = RemoveArrayData(ClientName, i3)
                    ClientVersion = RemoveArrayData(ClientVersion, i3)
                    ClientSocketNum = RemoveArrayData(ClientSocketNum, i3)

                    CloseClient(targetSocketAddress)

                    If reason = 0 Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & quitName & "님이 퇴장하셨습니다.(오류로 인해 서버에서 강제로 드랍됨)")
                    ElseIf reason = 1 Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & quitName & "님이 퇴장하셨습니다.(관리자에 의해 강퇴됨)")
                    ElseIf reason = 2 Then
                        SendAllClient("CHAT" & ParseString & "[서버]" & quitName & "님이 퇴장하셨습니다.(플레이어가 연결을 끊음)")
                    End If


                    System.Threading.Thread.Sleep(100)

                    Dim i4 As Integer
                    Dim SPlayer = "REFPLAYER", SVersion = "REFVERSION", SCliname = "REFCLINAME"

                    Do While True
                        If Not i4 >= UBound(Nickname) Then
                            SPlayer = SPlayer + ParseString + Nickname(i4)
                            SVersion = SVersion + ParseString + ClientVersion(i4)
                            SCliname = SCliname + ParseString + ClientName(i4)
                            Console.Print("Engine", 2, SPlayer, "ServerListenEvent")
                        Else
                            Exit Do
                        End If
                        i4 = i4 + 1
                    Loop

                    SendAllClient(SPlayer)
                    System.Threading.Thread.Sleep(300)
                    SendAllClient(SCliname)
                    System.Threading.Thread.Sleep(300)
                    SendAllClient(SVersion)
                End If
                i3 = i3 + 1
            Loop
        End Sub

        Public Sub CloseClient(socketnum As Integer)
            ClientSocCache(socketnum).Shutdown(SocketShutdown.Both)
            ClientSocCache(socketnum).Close()
            ClientSocCache(socketnum).Dispose()
        End Sub
    End Class
End Class