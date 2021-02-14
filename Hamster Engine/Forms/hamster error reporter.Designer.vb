<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ErrorReporter
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기를 사용하여 수정하지 마십시오.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.txtsubject = New System.Windows.Forms.Label()
        Me.Blink = New System.Windows.Forms.Timer(Me.components)
        Me.txterr = New System.Windows.Forms.Label()
        Me.txtexp = New System.Windows.Forms.Label()
        Me.txtinfo = New System.Windows.Forms.Label()
        Me.btnexp = New System.Windows.Forms.Button()
        Me.btnexpmsg = New System.Windows.Forms.Button()
        Me.btnshdw = New System.Windows.Forms.Button()
        Me.btncause = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtsubject
        '
        Me.txtsubject.AutoSize = True
        Me.txtsubject.Font = New System.Drawing.Font("굴림", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.txtsubject.Location = New System.Drawing.Point(7, 11)
        Me.txtsubject.Name = "txtsubject"
        Me.txtsubject.Size = New System.Drawing.Size(477, 15)
        Me.txtsubject.TabIndex = 0
        Me.txtsubject.Text = "처리할수 없거나, 정의되지 않은 예외또는 오류가 발생했습니다!!"
        '
        'Blink
        '
        Me.Blink.Enabled = True
        Me.Blink.Interval = 800
        '
        'txterr
        '
        Me.txterr.Location = New System.Drawing.Point(7, 134)
        Me.txterr.Name = "txterr"
        Me.txterr.Size = New System.Drawing.Size(474, 73)
        Me.txterr.TabIndex = 10
        Me.txterr.Text = "에러 발생 내용 :"
        '
        'txtexp
        '
        Me.txtexp.Location = New System.Drawing.Point(7, 222)
        Me.txtexp.Name = "txtexp"
        Me.txtexp.Size = New System.Drawing.Size(474, 186)
        Me.txtexp.TabIndex = 11
        Me.txtexp.Text = "예외 발생 내용 :"
        '
        'txtinfo
        '
        Me.txtinfo.Location = New System.Drawing.Point(10, 40)
        Me.txtinfo.Name = "txtinfo"
        Me.txtinfo.Size = New System.Drawing.Size(474, 84)
        Me.txtinfo.TabIndex = 12
        Me.txtinfo.Text = "정보"
        '
        'btnexp
        '
        Me.btnexp.Location = New System.Drawing.Point(10, 411)
        Me.btnexp.Name = "btnexp"
        Me.btnexp.Size = New System.Drawing.Size(116, 23)
        Me.btnexp.TabIndex = 13
        Me.btnexp.Text = "예외정보요약"
        Me.btnexp.UseVisualStyleBackColor = True
        '
        'btnexpmsg
        '
        Me.btnexpmsg.Location = New System.Drawing.Point(132, 411)
        Me.btnexpmsg.Name = "btnexpmsg"
        Me.btnexpmsg.Size = New System.Drawing.Size(116, 23)
        Me.btnexpmsg.TabIndex = 14
        Me.btnexpmsg.Text = "오류 메세지"
        Me.btnexpmsg.UseVisualStyleBackColor = True
        '
        'btnshdw
        '
        Me.btnshdw.Location = New System.Drawing.Point(254, 410)
        Me.btnshdw.Name = "btnshdw"
        Me.btnshdw.Size = New System.Drawing.Size(116, 24)
        Me.btnshdw.TabIndex = 15
        Me.btnshdw.Text = "프로그램 종료"
        Me.btnshdw.UseVisualStyleBackColor = True
        '
        'btncause
        '
        Me.btncause.BackColor = System.Drawing.Color.Red
        Me.btncause.Location = New System.Drawing.Point(392, 410)
        Me.btncause.Name = "btncause"
        Me.btncause.Size = New System.Drawing.Size(89, 24)
        Me.btncause.TabIndex = 16
        Me.btncause.Text = "예외 발생"
        Me.btncause.UseVisualStyleBackColor = False
        '
        'ErrorReporter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(493, 562)
        Me.Controls.Add(Me.btncause)
        Me.Controls.Add(Me.btnshdw)
        Me.Controls.Add(Me.btnexpmsg)
        Me.Controls.Add(Me.btnexp)
        Me.Controls.Add(Me.txtinfo)
        Me.Controls.Add(Me.txtexp)
        Me.Controls.Add(Me.txterr)
        Me.Controls.Add(Me.txtsubject)
        Me.Font = New System.Drawing.Font("굴림", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(509, 600)
        Me.MinimumSize = New System.Drawing.Size(509, 600)
        Me.Name = "ErrorReporter"
        Me.Text = "Hamster 예외 처리기"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtsubject As System.Windows.Forms.Label
    Friend WithEvents Blink As System.Windows.Forms.Timer
    Friend WithEvents txterr As System.Windows.Forms.Label
    Friend WithEvents txtexp As System.Windows.Forms.Label
    Friend WithEvents txtinfo As System.Windows.Forms.Label
    Friend WithEvents btnexp As System.Windows.Forms.Button
    Friend WithEvents btnexpmsg As System.Windows.Forms.Button
    Friend WithEvents btnshdw As System.Windows.Forms.Button
    Friend WithEvents btncause As System.Windows.Forms.Button
End Class
