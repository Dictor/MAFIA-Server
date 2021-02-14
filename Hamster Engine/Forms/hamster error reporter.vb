Imports HamsterEngine.Engine

Public Class ErrorReporter
    Public lastcallexp As Exception
    Public Version As New HamsterVersion("Error Reporter", 1, 0, 140125, 0)

    Private hexception As Exception

    Public Sub ReportError(exception As Exception, caller As String, Optional subcaller As String = "General")
        Me.Show()
        lastcallexp = exception
        txtinfo.Text = "상황 정보" & vbCrLf & "런타임 = " & Console.projruntime & "ms, 예외처리기 버젼 : " & Me.Version.GetVersion(True) & vbCrLf & "로그 파일 : " & Console.Log.logpath
        txterr.Text = "에러 정보" & vbCrLf & "번호 " & Err.Number & " 에러가 " & Err.Source & " 의 줄 " & Err.Erl & "번에서 발생하였습니다." & vbCrLf & Err.Description & vbCrLf & "라이브러리 : " & Err.LastDllError
        txtexp.Text = "예외 정보" & vbCrLf & exception.Source & "의" & vbCrLf & exception.StackTrace & "에서 <" & vbCrLf & exception.Message & "> 예외가 발생했습니다"

        '& vbCrLf & "예외 " & exception.Data.ToString & " 가 " & vbCrLf & exception.Source & " 의 함수 " & exception.TargetSite.ToString & "에서 발생하였습니다." & vbCrLf & exception.Message & vbCrLf & "라이브러리 : " ' & exception.InnerException.ToString
        HamsterEngine.Console.Log.write("[햄스터 예외 처리기]/" & Console.projruntime & "ms / 예외 감지됨, 내용 요약 : " & exception.ToString)

        hexception = exception
    End Sub
    Private Sub Label1_Click(sender As System.Object, e As System.EventArgs) Handles txtsubject.Click

    End Sub

    Private Sub Blink_Tick(sender As System.Object, e As System.EventArgs) Handles Blink.Tick

        If txtsubject.Visible = True Then
            txtsubject.Visible = False
        Else
            txtsubject.Visible = True
        End If

    End Sub

    Private Sub ErrorReporter_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        txtsubject.ForeColor = Color.Red
    End Sub

    Private Sub Label1_Click_1(sender As System.Object, e As System.EventArgs)

    End Sub

    Private Sub txtexp_Click(sender As System.Object, e As System.EventArgs) Handles txtexp.Click

    End Sub

    Private Sub Label1_Click_2(sender As System.Object, e As System.EventArgs) Handles txtinfo.Click

    End Sub

    Private Sub btnexp_Click(sender As System.Object, e As System.EventArgs) Handles btnexp.Click
        MsgBox(lastcallexp.ToString)
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles btnexpmsg.Click
        MsgBox(ErrorToString(Err.Number))
    End Sub

    Private Sub btnshdw_Click(sender As System.Object, e As System.EventArgs) Handles btnshdw.Click
        End
    End Sub

    Private Sub Button1_Click_1(sender As System.Object, e As System.EventArgs) Handles btncause.Click
        If MsgBox("디버그를 위해 예외를 의도적으로 발생시킵니다" + vbCrLf + "심각한 피해를 초래할수 있으므로 일반 사용자는 사용하지 마십시요, 개발자 용으로 설계되었습니다", MsgBoxStyle.OkCancel, "경고") = MsgBoxResult.Ok Then
            Console.WarningMsg.Print(2, "예외 발생됨, 프로그램의 정상적 동작을 절대 보증하지 않습니다", 20)
            Throw hexception
        End If
    End Sub
End Class