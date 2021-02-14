<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Console
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
        Me.conlist = New System.Windows.Forms.ListBox()
        Me.txtin = New System.Windows.Forms.TextBox()
        Me.consoleinput = New System.Windows.Forms.Button()
        Me.Reftimer = New System.Windows.Forms.Timer(Me.components)
        Me.warnmsg = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'conlist
        '
        Me.conlist.ColumnWidth = 700000000
        Me.conlist.Font = New System.Drawing.Font("굴림", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.conlist.FormattingEnabled = True
        Me.conlist.HorizontalExtent = 5500
        Me.conlist.HorizontalScrollbar = True
        Me.conlist.ItemHeight = 12
        Me.conlist.Location = New System.Drawing.Point(4, 6)
        Me.conlist.MaximumSize = New System.Drawing.Size(553, 352)
        Me.conlist.MinimumSize = New System.Drawing.Size(553, 352)
        Me.conlist.Name = "conlist"
        Me.conlist.Size = New System.Drawing.Size(553, 352)
        Me.conlist.TabIndex = 0
        '
        'txtin
        '
        Me.txtin.Location = New System.Drawing.Point(4, 390)
        Me.txtin.Name = "txtin"
        Me.txtin.Size = New System.Drawing.Size(472, 21)
        Me.txtin.TabIndex = 1
        '
        'consoleinput
        '
        Me.consoleinput.Font = New System.Drawing.Font("굴림", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.consoleinput.Location = New System.Drawing.Point(482, 390)
        Me.consoleinput.Name = "consoleinput"
        Me.consoleinput.Size = New System.Drawing.Size(75, 23)
        Me.consoleinput.TabIndex = 2
        Me.consoleinput.Text = "입력"
        Me.consoleinput.UseVisualStyleBackColor = True
        '
        'Reftimer
        '
        Me.Reftimer.Enabled = True
        Me.Reftimer.Interval = 1
        '
        'warnmsg
        '
        Me.warnmsg.BackColor = System.Drawing.SystemColors.Control
        Me.warnmsg.Font = New System.Drawing.Font("굴림", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.warnmsg.Location = New System.Drawing.Point(2, 361)
        Me.warnmsg.Name = "warnmsg"
        Me.warnmsg.Size = New System.Drawing.Size(555, 26)
        Me.warnmsg.TabIndex = 4
        '
        'Console
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(561, 414)
        Me.ControlBox = False
        Me.Controls.Add(Me.warnmsg)
        Me.Controls.Add(Me.consoleinput)
        Me.Controls.Add(Me.txtin)
        Me.Controls.Add(Me.conlist)
        Me.MaximumSize = New System.Drawing.Size(577, 452)
        Me.MinimumSize = New System.Drawing.Size(577, 452)
        Me.Name = "Console"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Hamster Engine Console"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtin As System.Windows.Forms.TextBox
    Friend WithEvents consoleinput As System.Windows.Forms.Button
    Friend WithEvents Reftimer As System.Windows.Forms.Timer
    Public Shared WithEvents conlist As System.Windows.Forms.ListBox '이 개체는 반드시 Public Shared로 선언되야 합니다
    Public Shared WithEvents warnmsg As System.Windows.Forms.Label '이 개체는 반드시 Public Shared로 선언되야 합니다
End Class
