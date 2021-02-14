Imports System.Text
Imports System.IO
Imports System.IO.Directory
Imports System.IO.DirectoryInfo
Imports System.Security
Imports HamsterEngine.Engine

Module Filemanager
    Public Version As New HamsterVersion("File Manager", 1, 0, 119, 0)

    Public Class File

        Public Shared filemanagererr As Boolean '파일 모듈 에러
        Public VFMREAD As String '읽은 파일 한줄 저장 변수
        Public VFMLOG As String

        Public Shared Sub HelpString()
            Console.Print("Console", 0, "-------------Hamster File Manager 모드 설정 명령어")
            Console.Print("Console", 0, "접근을 위한 첫 명령어는 file 입니다")
            Console.Print("Console", 0, "file, help 또는 h - 도움말")
            Console.Print("Console", 0, "read <경로:문자열> 또는 r <경로:문자열> - VFM 형식의 파일을 읽습니다")
            Console.Print("Console", 0, "read <경로:문자열> 또는 r <경로:문자열> - VFM 형식의 파일을 읽습니다")
            Console.Print("Console", 0, "check <경로:문자열> 또는 c <경로:문자열> - 파일의 존재유무를 확인합니다")
            Console.Print("Console", 0, "created <경로:문자열> 또는 crd <경로:문자열> - 디렉터리를 생성합니다")
            Console.Print("Console", 0, "deleted <경로:문자열> <하위 디렉터리 삭제:True 또는 False> 또는 ded <경로:문자열> <하위 디렉터리 삭제:True 또는 False> - 디렉터리를 삭제합니다")
            Console.Print("Console", 0, "getcreatedtimed <경로:문자열> <UTC 시간 반환 여부:True 또는 False> 또는 gctd <경로:문자열> <UTC 시간 반환 여부:True 또는 False> - 디렉터리의 생성시간을 반환합니다")
            Console.Print("Console", 0, "getdirectories <경로:문자열> 또는 gdrt <경로:문자열> - 하위 디렉터리를 반환합니다")
            Console.Print("Console", 0, "getfiles <경로:문자열> 또는 gfls <경로:문자열> - 경로 디렉터리의 하위 파일을 반환합니다")
            Console.Print("Console", 0, "getentries <경로:문자열> 또는 gent <경로:문자열> - 경로 디렉터리의 하위 파일과 디렉터리를 반환합니다")
            Console.Print("Console", 0, "getparentdirectory <경로:문자열> 또는 gpdt <경로:문자열> - 경로 디렉터리의 부모 디렉터리를 반환합니다")
            Console.Print("Console", 0, "movedirectory <이동할 경로:문자열> <목적지 경로:문자열> 또는 m <이동할 경로:문자열> <목적지 경로:문자열> - 이동할 경로 인수의 디렉터리를 목적지 경로 인수의 디렉터리로 이동합니다")
        End Sub

        Public Shared Function Delete(path As String)
            Console.Print("HFM", 2, path & " 파일을 삭제합니다")
            Try
                My.Computer.FileSystem.DeleteFile(path)
            Catch exp As PathTooLongException
                Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 파일은 삭제되지 않았습니다")
                Return 3
                Exit Function
            Catch exp As DirectoryNotFoundException
                Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 파일은 삭제되지 않았습니다")
                Return 3
                Exit Function
            Catch exp As IOException
                Console.Print("HFM", 1, "경로 인수의 경로에 이름및 위치가 같은 파일이 있거나, 파일이 읽기전용 또는 현재 프로그램이 실행중인 파일입니다, 파일은 삭제되지 않았습니다")
                Return 3
                Exit Function
            Catch exp As ArgumentNullException
                Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 파일은 삭제되지 않았습니다")
                Return 3
                Exit Function
            Catch exp As ArgumentException
                Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 파일은 삭제되지 않았습니다")
                Return 3
                Exit Function
            Catch exp As UnauthorizedAccessException
                Console.Print("HFM", 1, "파일을 삭제하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 파일은 삭제되지 않았습니다")
                Return 4
                Exit Function
            End Try
            Console.Print("HFM", 2, path & " 파일이 삭제됬습니다")
        End Function

        Public Shared Function Check(path As String, log As Boolean)
            If Not log Then
                Console.Print("HFM", 2, path & " 의 유무를 점검합니다")
            End If
            If Not System.IO.File.Exists(path) Then
                filemanagererr = True
                Console.Print("HFM", 2, "파일을 찾을수 없습니다")
                Return False
                Exit Function
            Else
                Console.Print("HFM", 2, "파일이 존재 합니다")
                Return True
                Exit Function
            End If
        End Function


        Public Shared Function Readtext(filename As String)
            Try
                Dim data = My.Computer.FileSystem.ReadAllText(filename, Encoding.UTF8)
                Return data
            Catch exp As PathTooLongException
                Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 파일은 읽혀지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As DirectoryNotFoundException
                Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 파일은 읽혀지지 않았습니다")
                Return Words.hException.File.IlegalEntry
                Exit Function
            Catch exp As FileNotFoundException
                Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 파일은 읽혀지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As IOException
                Console.Print("HFM", 1, "파일을 다른프로세서가 사용중이거나, IO 에러가 발생했습니다, 파일은 읽혀지지 않았습니다")
                Return Words.hException.File.IO
                Exit Function
            Catch exp As ArgumentNullException
                Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 파일은 읽혀지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As ArgumentException
                Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.File.IlegalEntry
                Exit Function
            Catch exp As SecurityException
                Console.Print("HFM", 1, "디렉터리를 보기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 파일은 쓰여지지 않았습니다")
                Return Words.hException.NoPermission
                Exit Function
            Catch exp As NotSupportedException
                Console.Print("HFM", 1, "경로 인수에 드라이브 레이블의 일부가 아닌 : 문자가 포함되어 있습니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            End Try
        End Function

        Public Shared Function writetext(path As String, data As String, overwrite As Boolean, log As Boolean)
            Try
                If Not log Then
                    Console.Print("HFM", 2, "텍스트 파일을 씁니다, 경로 : " & path & " 덮어쓰기 : " & overwrite & " 내용 : " & data)
                End If

                My.Computer.FileSystem.WriteAllText(path, vbCrLf & data, overwrite, System.Text.Encoding.UTF8)
            Catch exp As PathTooLongException
                Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As DirectoryNotFoundException
                Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.File.IlegalEntry
                Exit Function
            Catch exp As FileNotFoundException
                Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As IOException
                Console.Print("HFM", 1, "파일을 다른프로세서가 사용중이거나, 읽기 전용이거나, IO 에러가 발생했습니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.File.IO
                Exit Function
            Catch exp As ArgumentNullException
                Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            Catch exp As ArgumentException
                Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.File.IlegalEntry
                Exit Function
            Catch exp As SecurityException
                Console.Print("HFM", 1, "디렉터리를 생성하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 파일은 쓰여지지 않았습니다")
                Return Words.hException.NoPermission
                Exit Function
            Catch exp As NotSupportedException
                Console.Print("HFM", 1, "경로 인수에 드라이브 레이블의 일부가 아닌 : 문자가 포함되어 있습니다, 파일은 쓰여지지 않았습니다")
                Return Words.hException.IlegalArgument
                Exit Function
            End Try
        End Function

        Public Class Directory

            Public Shared Function Create(path As String)

                Console.Print("HFM", 2, path & " 디렉터리를 생성합니다")
                Try
                    System.IO.Directory.CreateDirectory(path)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로가 읽기 전용입니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리를 생성하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 디렉터리는 생성되지 않았습니다")
                    Return 4
                    Exit Function
                Catch exp As NotSupportedException
                    Console.Print("HFM", 1, "경로 인수에 드라이브 레이블의 일부가 아닌 : 문자가 포함되어 있습니다, 디렉터리는 생성되지 않았습니다")
                    Return 3
                    Exit Function
                End Try
                Console.Print("HFM", 2, path & " 디렉터리가 생성됬습니다")
            End Function

            Public Shared Function Delete(path As String, deletechild As Boolean)
                Console.Print("HFM", 2, path & " 디렉터리를 삭제합니다")
                Try
                    System.IO.Directory.Delete(path, deletechild)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 디렉터리는 삭제되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 디렉터리는 삭제되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로에 이름및 위치가 같은 파일이 있거나, 디렉터리가 읽기전용 또는 현재 프로그램이 실행중인 디렉터리입니다, 디렉터리는 삭제되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 디렉터리는 삭제되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 디렉터리는 삭제되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리를 삭제하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 디렉터리는 삭제되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 2, path & " 디렉터리가 삭제됬습니다")
            End Function

            Public Shared Function Check(path As String)
                Console.Print("HFM", 2, path & " 디렉터리가 존재하는지 검사합니다")
                Dim exist As Boolean
                exist = System.IO.Directory.Exists(path)

                If exist = True Then
                    Console.Print("HFM", 1, path & " 디렉터리가 존재합니다")
                    Return True
                ElseIf exist = False Then
                    Console.Print("HFM", 1, path & " 디렉터리가 존재하지 않습니다")
                    Return False
                Else
                    Console.Print("HFM", 1, path & " 디렉터리 검사에 따른 반환값이 올바르지 않습니다, 정의되지 않은 오류입니다")
                    Return 0
                End If
            End Function

            Public Shared Function GetCreatedTime(path As String, utc As Boolean)
                Dim createdtime As New Date

                Console.Print("HFM", 2, path & " 디렉터리가 만들어진 시간을 반환합니다")
                Try
                    If utc = False Then
                        createdtime = System.IO.Directory.GetCreationTime(path)
                        Console.Print("HFM", 2, "시간을 UTC 시간이 아닌 현지시간(컴퓨터 시간)으로 반환합니다")
                    ElseIf utc = True Then
                        createdtime = System.IO.Directory.GetCreationTimeUtc(path)
                        Console.Print("HFM", 2, "시간을 UTC 시간으로 반환합니다")
                    Else
                        Console.Print("HFM", 1, "UTC 인수값이 올바르지 않습니다, 정보는 반환되지 않았습니다")
                        Return 3
                        Exit Function
                    End If

                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리에 접근하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 2, path & " 디렉터리가 만들어진 시간은 " & createdtime & " 입니다")
                Return createdtime
            End Function

            Public Shared Function GetChildDirectories(path As String)
                Dim directory As String()
                Console.Print("HFM", 2, path & " 경로인수의 모든 하위 디렉터리 이름을 반환합니다.")
                Try
                    directory = System.IO.Directory.GetDirectories(path)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로가 파일이름 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리에 접근하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 3, UBound(directory) & "개의 항목을 찾았습니다")
                Console.Printarrange("HFM", 3, directory)
                Console.Print("HFM", 2, path & " 의 하위디렉터리 정보가 반환되었습니다")



                Return directory
            End Function

            Public Shared Function GetChildFiles(path As String)
                Dim directory As String()
                Console.Print("HFM", 2, path & " 경로인수 디렉터리의 모든 하위 파일 이름을 반환합니다.")
                Try
                    directory = System.IO.Directory.GetFiles(path)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로가 파일이름 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리에 접근하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 2, UBound(directory) & "개의 항목을 찾았습니다")
                Console.Printarrange("HFM", 2, directory)
                Console.Print("HFM", 2, path & " 디렉터리의 하위 파일 정보가 반환되었습니다")



                Return directory
            End Function

            Public Shared Function GetEntries(path As String)
                Dim directory As String()
                Console.Print("HFM", 2, path & " 경로인수 디렉터리의 모든 하위 파일과 디렉터리 이름을 반환합니다.")

                Try
                    directory = System.IO.Directory.GetFileSystemEntries(path)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로가 파일이름 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리에 접근하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 3, UBound(directory) & "개의 항목을 찾았습니다")
                Console.Printarrange("HFM", 3, directory)
                Console.Print("HFM", 2, path & " 디렉터리의 하위 파일과 디렉터리 정보가 반환되었습니다")

                Return directory
            End Function

            Public Shared Function GetParentDirectory(path As String)
                Dim directory As System.IO.DirectoryInfo
                Console.Print("2", False, path & " 디렉터리의 부모 디렉터리를 반환합니다")

                Try
                    directory = System.IO.Directory.GetParent(path)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "경로 인수의 경로가 잘못되었습니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "경로 인수의 경로가 파일이름 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리에 접근하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 3, path & "의 부모 디렉터리는 " & directory.FullName & " 입니다")
                Console.Print("HFM", 2, path & " 디렉터리의 부모 디렉터리를 반환했습니다")

                Return directory.FullName
            End Function

            Public Shared Function Move(sourcepath As String, destpath As String)
                Console.Print("HFM", 2, sourcepath & " 디렉터리를 " & destpath & " 디렉터리로 이동합니다")

                Try
                    System.IO.Directory.Move(sourcepath, destpath)
                Catch exp As PathTooLongException
                    Console.Print("HFM", 1, "이동할 경로, 목적지 경로 인수의 경로에는 248자 미만의 문자를 사용해야 합니다, 디렉터리는 이동되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As DirectoryNotFoundException
                    Console.Print("HFM", 1, "이동할 경로 인수의 경로가 잘못되었습니다, 디렉터리는 이동되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As IOException
                    Console.Print("HFM", 1, "디렉터리를 다른볼륨으로 이동하거나, 목적지 경로가 있거나, 이동할 경로와 목적지 경로가 같은 파일이나 디렉터리를 참조합니다, 디렉터리는 이동되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentNullException
                    Console.Print("HFM", 1, "이동할 경로, 목적지 경로 인수가 Nothing 입니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As ArgumentException
                    Console.Print("HFM", 1, "이동할 경로, 목적지 경로 인수의 길이가 0 이거나, 공백만 존재하거나, 잘못된 문자가 포함 또는 : 문자로 시작합니다, 정보는 반환되지 않았습니다")
                    Return 3
                    Exit Function
                Catch exp As UnauthorizedAccessException
                    Console.Print("HFM", 1, "디렉터리를 이동하기 위한 권한이 없습니다. 프로그램을 관리자 권한으로 재시작 하십시요, 정보는 반환되지 않았습니다")
                    Return 4
                    Exit Function
                End Try
                Console.Print("HFM", 2, sourcepath & " 디렉터리를 성공적으로 " & destpath & " 로 이동했습니다")


            End Function
        End Class
    End Class

End Module
