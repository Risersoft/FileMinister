<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLocalShare
    Inherits frmMax

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_WindowsUser = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_Password = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_SharePathLocal = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel5 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_SharePathUNC = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.Panel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.cmb_FileShareID = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_IndexServerCatalog = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_WindowsUser, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_Password, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_SharePathLocal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_SharePathUNC, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.ClientArea.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.cmb_FileShareID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_IndexServerCatalog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraLabel2
        '
        Appearance1.FontData.BoldAsString = "False"
        Appearance1.TextHAlignAsString = "Right"
        Appearance1.TextVAlignAsString = "Middle"
        Me.UltraLabel2.Appearance = Appearance1
        Me.UltraLabel2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel2.Location = New System.Drawing.Point(37, 23)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(86, 17)
        Me.UltraLabel2.TabIndex = 4
        Me.UltraLabel2.Text = "User Name"
        '
        'txt_WindowsUser
        '
        Appearance2.FontData.SizeInPoints = 10.0!
        Me.txt_WindowsUser.Appearance = Appearance2
        Me.txt_WindowsUser.Location = New System.Drawing.Point(129, 18)
        Me.txt_WindowsUser.Name = "txt_WindowsUser"
        Me.txt_WindowsUser.Size = New System.Drawing.Size(276, 24)
        Me.txt_WindowsUser.TabIndex = 5
        Me.txt_WindowsUser.Text = "UltraTextEditor1"
        '
        'UltraLabel3
        '
        Appearance3.FontData.BoldAsString = "False"
        Appearance3.TextHAlignAsString = "Right"
        Appearance3.TextVAlignAsString = "Middle"
        Me.UltraLabel3.Appearance = Appearance3
        Me.UltraLabel3.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel3.Location = New System.Drawing.Point(35, 59)
        Me.UltraLabel3.Name = "UltraLabel3"
        Me.UltraLabel3.Size = New System.Drawing.Size(86, 17)
        Me.UltraLabel3.TabIndex = 6
        Me.UltraLabel3.Text = "Password"
        '
        'txt_Password
        '
        Appearance4.FontData.SizeInPoints = 10.0!
        Me.txt_Password.Appearance = Appearance4
        Me.txt_Password.Location = New System.Drawing.Point(129, 53)
        Me.txt_Password.Name = "txt_Password"
        Me.txt_Password.Size = New System.Drawing.Size(276, 24)
        Me.txt_Password.TabIndex = 7
        Me.txt_Password.Text = "UltraTextEditor1"
        '
        'UltraLabel4
        '
        Appearance5.FontData.BoldAsString = "False"
        Appearance5.TextHAlignAsString = "Right"
        Appearance5.TextVAlignAsString = "Middle"
        Me.UltraLabel4.Appearance = Appearance5
        Me.UltraLabel4.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel4.Location = New System.Drawing.Point(12, 94)
        Me.UltraLabel4.Name = "UltraLabel4"
        Me.UltraLabel4.Size = New System.Drawing.Size(111, 17)
        Me.UltraLabel4.TabIndex = 8
        Me.UltraLabel4.Text = "Folder Path Local"
        '
        'txt_SharePathLocal
        '
        Appearance6.FontData.SizeInPoints = 10.0!
        Me.txt_SharePathLocal.Appearance = Appearance6
        Me.txt_SharePathLocal.Location = New System.Drawing.Point(129, 89)
        Me.txt_SharePathLocal.Name = "txt_SharePathLocal"
        Me.txt_SharePathLocal.Size = New System.Drawing.Size(276, 24)
        Me.txt_SharePathLocal.TabIndex = 9
        Me.txt_SharePathLocal.Text = "UltraTextEditor1"
        '
        'UltraLabel5
        '
        Appearance7.FontData.BoldAsString = "False"
        Appearance7.TextHAlignAsString = "Right"
        Appearance7.TextVAlignAsString = "Middle"
        Me.UltraLabel5.Appearance = Appearance7
        Me.UltraLabel5.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel5.Location = New System.Drawing.Point(22, 130)
        Me.UltraLabel5.Name = "UltraLabel5"
        Me.UltraLabel5.Size = New System.Drawing.Size(100, 17)
        Me.UltraLabel5.TabIndex = 10
        Me.UltraLabel5.Text = "Folder Path UNC"
        '
        'txt_SharePathUNC
        '
        Appearance8.FontData.SizeInPoints = 10.0!
        Me.txt_SharePathUNC.Appearance = Appearance8
        Me.txt_SharePathUNC.Location = New System.Drawing.Point(129, 124)
        Me.txt_SharePathUNC.Name = "txt_SharePathUNC"
        Me.txt_SharePathUNC.Size = New System.Drawing.Size(276, 24)
        Me.txt_SharePathUNC.TabIndex = 11
        Me.txt_SharePathUNC.Text = "UltraTextEditor1"
        '
        'Panel1
        '
        '
        'Panel1.ClientArea
        '
        Me.Panel1.ClientArea.Controls.Add(Me.btnSave)
        Me.Panel1.ClientArea.Controls.Add(Me.btnCancel)
        Me.Panel1.ClientArea.Controls.Add(Me.btnOK)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 254)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(435, 36)
        Me.Panel1.TabIndex = 12
        '
        'btnSave
        '
        Appearance9.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance9
        Me.btnSave.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnSave.Location = New System.Drawing.Point(231, 0)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(68, 36)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        '
        'btnCancel
        '
        Appearance10.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance10
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnCancel.Location = New System.Drawing.Point(299, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(68, 36)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'btnOK
        '
        Appearance11.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance11
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnOK.Location = New System.Drawing.Point(367, 0)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(68, 36)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(25, 164)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(96, 16)
        Me.Label9.TabIndex = 13
        Me.Label9.Text = "File Share"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmb_FileShareID
        '
        Appearance12.FontData.BoldAsString = "False"
        Me.cmb_FileShareID.Appearance = Appearance12
        Me.cmb_FileShareID.Location = New System.Drawing.Point(129, 161)
        Me.cmb_FileShareID.Name = "cmb_FileShareID"
        Me.cmb_FileShareID.Size = New System.Drawing.Size(168, 22)
        Me.cmb_FileShareID.TabIndex = 14
        Me.cmb_FileShareID.Text = "UltraCombo5"
        '
        'UltraLabel1
        '
        Appearance13.FontData.BoldAsString = "False"
        Appearance13.TextHAlignAsString = "Right"
        Appearance13.TextVAlignAsString = "Middle"
        Me.UltraLabel1.Appearance = Appearance13
        Me.UltraLabel1.AutoSize = True
        Me.UltraLabel1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel1.Location = New System.Drawing.Point(12, 195)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(111, 14)
        Me.UltraLabel1.TabIndex = 15
        Me.UltraLabel1.Text = "Index Server Catalog"
        '
        'txt_IndexServerCatalog
        '
        Appearance14.FontData.SizeInPoints = 10.0!
        Me.txt_IndexServerCatalog.Appearance = Appearance14
        Me.txt_IndexServerCatalog.Location = New System.Drawing.Point(129, 189)
        Me.txt_IndexServerCatalog.Name = "txt_IndexServerCatalog"
        Me.txt_IndexServerCatalog.Size = New System.Drawing.Size(160, 24)
        Me.txt_IndexServerCatalog.TabIndex = 16
        Me.txt_IndexServerCatalog.Text = "UltraTextEditor1"
        '
        'frmLocalShare
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Caption = "Local Share"
        Me.ClientSize = New System.Drawing.Size(435, 290)
        Me.Controls.Add(Me.UltraLabel1)
        Me.Controls.Add(Me.txt_IndexServerCatalog)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.cmb_FileShareID)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.UltraLabel5)
        Me.Controls.Add(Me.txt_SharePathUNC)
        Me.Controls.Add(Me.UltraLabel4)
        Me.Controls.Add(Me.txt_SharePathLocal)
        Me.Controls.Add(Me.UltraLabel3)
        Me.Controls.Add(Me.txt_Password)
        Me.Controls.Add(Me.UltraLabel2)
        Me.Controls.Add(Me.txt_WindowsUser)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmLocalShare"
        Me.Text = "Local Share"
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_WindowsUser, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_Password, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_SharePathLocal, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_SharePathUNC, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ClientArea.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.cmb_FileShareID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_IndexServerCatalog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_WindowsUser As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_Password As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_SharePathLocal As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel5 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_SharePathUNC As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents Panel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Label9 As Label
    Friend WithEvents cmb_FileShareID As Infragistics.Win.UltraWinGrid.UltraCombo
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_IndexServerCatalog As Infragistics.Win.UltraWinEditors.UltraTextEditor
End Class
