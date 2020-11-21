Imports ug = Infragistics.Win.UltraWinGrid
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmOptions
    Inherits frmMax


#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitForm()
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.chk_ShowLog = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.btnChangeDB = New Infragistics.Win.Misc.UltraButton()
        Me.LabelDB = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBoxFileServer = New System.Windows.Forms.GroupBox()
        Me.btnChangeFile = New Infragistics.Win.Misc.UltraButton()
        Me.LabelFile = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextCompanyAddress = New System.Windows.Forms.TextBox()
        Me.TextCompanyName = New System.Windows.Forms.TextBox()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.chk_ShowLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBoxFileServer.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.btnSave)
        Me.Panel4.Controls.Add(Me.btnCancel)
        Me.Panel4.Controls.Add(Me.btnOK)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 496)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(458, 48)
        Me.Panel4.TabIndex = 63
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance6.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance6
        Me.btnSave.Location = New System.Drawing.Point(170, 8)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(88, 32)
        Me.btnSave.TabIndex = 2
        Me.btnSave.Text = "&Save"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance2.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance2
        Me.btnCancel.Location = New System.Drawing.Point(266, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 32)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance4.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance4
        Me.btnOK.Location = New System.Drawing.Point(362, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(88, 32)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chk_ShowLog)
        Me.GroupBox1.Controls.Add(Me.btnChangeDB)
        Me.GroupBox1.Controls.Add(Me.LabelDB)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(458, 135)
        Me.GroupBox1.TabIndex = 64
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = " Database Server"
        '
        'chk_ShowLog
        '
        Appearance10.FontData.BoldAsString = "False"
        Appearance10.FontData.SizeInPoints = 8.0!
        Me.chk_ShowLog.Appearance = Appearance10
        Me.chk_ShowLog.Location = New System.Drawing.Point(143, 99)
        Me.chk_ShowLog.Name = "chk_ShowLog"
        Me.chk_ShowLog.Size = New System.Drawing.Size(255, 17)
        Me.chk_ShowLog.TabIndex = 351
        Me.chk_ShowLog.Text = "Show Database creation log"
        '
        'btnChangeDB
        '
        Appearance7.FontData.SizeInPoints = 9.0!
        Me.btnChangeDB.Appearance = Appearance7
        Me.btnChangeDB.Location = New System.Drawing.Point(21, 93)
        Me.btnChangeDB.Name = "btnChangeDB"
        Me.btnChangeDB.Size = New System.Drawing.Size(88, 25)
        Me.btnChangeDB.TabIndex = 18
        Me.btnChangeDB.Text = "Change"
        '
        'LabelDB
        '
        Me.LabelDB.AutoSize = True
        Me.LabelDB.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LabelDB.Location = New System.Drawing.Point(18, 59)
        Me.LabelDB.Name = "LabelDB"
        Me.LabelDB.Size = New System.Drawing.Size(238, 14)
        Me.LabelDB.TabIndex = 17
        Me.LabelDB.Text = "Database: Server = xxxx, DB Version = yyyy"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 14)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Selected Server"
        '
        'GroupBoxFileServer
        '
        Me.GroupBoxFileServer.Controls.Add(Me.btnChangeFile)
        Me.GroupBoxFileServer.Controls.Add(Me.LabelFile)
        Me.GroupBoxFileServer.Controls.Add(Me.Label3)
        Me.GroupBoxFileServer.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBoxFileServer.Location = New System.Drawing.Point(0, 135)
        Me.GroupBoxFileServer.Name = "GroupBoxFileServer"
        Me.GroupBoxFileServer.Size = New System.Drawing.Size(458, 144)
        Me.GroupBoxFileServer.TabIndex = 65
        Me.GroupBoxFileServer.TabStop = False
        Me.GroupBoxFileServer.Text = " File Server"
        '
        'btnChangeFile
        '
        Appearance1.FontData.SizeInPoints = 9.0!
        Me.btnChangeFile.Appearance = Appearance1
        Me.btnChangeFile.Location = New System.Drawing.Point(37, 100)
        Me.btnChangeFile.Name = "btnChangeFile"
        Me.btnChangeFile.Size = New System.Drawing.Size(88, 25)
        Me.btnChangeFile.TabIndex = 21
        Me.btnChangeFile.Text = "Change"
        '
        'LabelFile
        '
        Me.LabelFile.AutoSize = True
        Me.LabelFile.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LabelFile.Location = New System.Drawing.Point(34, 68)
        Me.LabelFile.Name = "LabelFile"
        Me.LabelFile.Size = New System.Drawing.Size(43, 14)
        Me.LabelFile.TabIndex = 20
        Me.LabelFile.Text = "Path = "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(34, 35)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 14)
        Me.Label3.TabIndex = 19
        Me.Label3.Text = "Selected Server"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Controls.Add(Me.TextCompanyAddress)
        Me.GroupBox3.Controls.Add(Me.TextCompanyName)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.Location = New System.Drawing.Point(0, 279)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(458, 171)
        Me.GroupBox3.TabIndex = 66
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = " System Config"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 154)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(336, 14)
        Me.Label5.TabIndex = 22
        Me.Label5.Text = "Company Name and Address will be printed at the top of all printouts"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 84)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(96, 14)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Company Address"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 14)
        Me.Label2.TabIndex = 20
        Me.Label2.Text = "Company Name"
        '
        'TextCompanyAddress
        '
        Me.TextCompanyAddress.Location = New System.Drawing.Point(6, 101)
        Me.TextCompanyAddress.Multiline = True
        Me.TextCompanyAddress.Name = "TextCompanyAddress"
        Me.TextCompanyAddress.Size = New System.Drawing.Size(438, 42)
        Me.TextCompanyAddress.TabIndex = 1
        '
        'TextCompanyName
        '
        Me.TextCompanyName.Location = New System.Drawing.Point(6, 51)
        Me.TextCompanyName.Name = "TextCompanyName"
        Me.TextCompanyName.Size = New System.Drawing.Size(438, 20)
        Me.TextCompanyName.TabIndex = 0
        '
        'frmOptions
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "Options"
        Me.ClientSize = New System.Drawing.Size(458, 544)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBoxFileServer)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Panel4)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOptions"
        Me.Text = "Options"
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.chk_ShowLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBoxFileServer.ResumeLayout(False)
        Me.GroupBoxFileServer.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBoxFileServer As System.Windows.Forms.GroupBox
    Friend WithEvents LabelDB As System.Windows.Forms.Label
    Friend WithEvents LabelFile As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnChangeDB As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnChangeFile As Infragistics.Win.Misc.UltraButton
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextCompanyAddress As System.Windows.Forms.TextBox
    Friend WithEvents TextCompanyName As System.Windows.Forms.TextBox
    Friend WithEvents chk_ShowLog As Infragistics.Win.UltraWinEditors.UltraCheckEditor

#End Region
End Class

