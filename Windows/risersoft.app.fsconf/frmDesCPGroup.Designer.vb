<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Partial Class frmDesCPGroup
    Inherits frmMax

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        InitForm()
        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents UltraLabel9 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_GroupCode As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_GroupName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.btnSave = New Infragistics.Win.Misc.UltraButton
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton
        Me.btnOK = New Infragistics.Win.Misc.UltraButton
        Me.UltraLabel9 = New Infragistics.Win.Misc.UltraLabel
        Me.txt_GroupCode = New Infragistics.Win.UltraWinEditors.UltraTextEditor
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel
        Me.txt_GroupName = New Infragistics.Win.UltraWinEditors.UltraTextEditor
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        CType(Me.txt_GroupCode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_GroupName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.btnSave)
        Me.Panel4.Controls.Add(Me.btnCancel)
        Me.Panel4.Controls.Add(Me.btnOK)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 182)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(592, 48)
        Me.Panel4.TabIndex = 3
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance1.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance1
        Me.btnSave.Location = New System.Drawing.Point(304, 8)
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
        Me.btnCancel.Location = New System.Drawing.Point(400, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 32)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance3.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance3
        Me.btnOK.Location = New System.Drawing.Point(496, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(88, 32)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        '
        'UltraLabel9
        '
        Appearance4.TextHAlign = Infragistics.Win.HAlign.Right
        Me.UltraLabel9.Appearance = Appearance4
        Me.UltraLabel9.Location = New System.Drawing.Point(32, 24)
        Me.UltraLabel9.Name = "UltraLabel9"
        Me.UltraLabel9.Size = New System.Drawing.Size(80, 16)
        Me.UltraLabel9.TabIndex = 155
        Me.UltraLabel9.Text = "Code"
        '
        'txt_GroupCode
        '
        Appearance5.FontData.BoldAsString = "False"
        Appearance5.FontData.ItalicAsString = "False"
        Appearance5.FontData.Name = "Arial"
        Appearance5.FontData.SizeInPoints = 8.25!
        Appearance5.FontData.StrikeoutAsString = "False"
        Appearance5.FontData.UnderlineAsString = "False"
        Me.txt_GroupCode.Appearance = Appearance5
        Me.txt_GroupCode.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_GroupCode.Location = New System.Drawing.Point(120, 24)
        Me.txt_GroupCode.Name = "txt_GroupCode"
        Me.txt_GroupCode.Size = New System.Drawing.Size(200, 21)
        Me.txt_GroupCode.TabIndex = 154
        '
        'UltraLabel2
        '
        Appearance6.FontData.BoldAsString = "True"
        Appearance6.FontData.SizeInPoints = 9.0!
        Appearance6.TextHAlign = Infragistics.Win.HAlign.Right
        Me.UltraLabel2.Appearance = Appearance6
        Me.UltraLabel2.Location = New System.Drawing.Point(32, 56)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(80, 16)
        Me.UltraLabel2.TabIndex = 153
        Me.UltraLabel2.Text = "Name"
        '
        'txt_GroupName
        '
        Appearance7.FontData.BoldAsString = "True"
        Appearance7.FontData.ItalicAsString = "False"
        Appearance7.FontData.Name = "Arial"
        Appearance7.FontData.SizeInPoints = 9.0!
        Appearance7.FontData.StrikeoutAsString = "False"
        Appearance7.FontData.UnderlineAsString = "False"
        Me.txt_GroupName.Appearance = Appearance7
        Me.txt_GroupName.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_GroupName.Location = New System.Drawing.Point(120, 56)
        Me.txt_GroupName.Name = "txt_GroupName"
        Me.txt_GroupName.Size = New System.Drawing.Size(416, 22)
        Me.txt_GroupName.TabIndex = 152
        '
        'frmDesCPGroup
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(592, 230)
        Me.Controls.Add(Me.UltraLabel9)
        Me.Controls.Add(Me.txt_GroupCode)
        Me.Controls.Add(Me.UltraLabel2)
        Me.Controls.Add(Me.txt_GroupName)
        Me.Controls.Add(Me.Panel4)
        Me.Name = "frmDesCPGroup"
        Me.Text = "Check Point Group"
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        CType(Me.txt_GroupCode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_GroupName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region
End Class

