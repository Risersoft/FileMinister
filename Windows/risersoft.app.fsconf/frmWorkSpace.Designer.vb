Imports Infragistics.Win.UltraWinGrid
Imports System.Xml
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class frmWorkSpace
    Inherits frmMax

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call

        Me.initForm()


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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.UltraTabPageControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridLocal = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.Panel4 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnEdit = New Infragistics.Win.Misc.UltraButton()
        Me.btnDel = New Infragistics.Win.Misc.UltraButton()
        Me.btnAdd = New Infragistics.Win.Misc.UltraButton()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.cmb_UserDomainID = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_MacAddresses = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.txt_ServerName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.Panel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_IndexServerUserName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.txt_IndexServerName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel5 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_IndexServerPassword = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl1.SuspendLayout()
        CType(Me.UltraGridLocal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.ClientArea.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.UltraPanel1.ClientArea.SuspendLayout()
        Me.UltraPanel1.SuspendLayout()
        CType(Me.cmb_UserDomainID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_MacAddresses, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_ServerName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.ClientArea.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        CType(Me.txt_IndexServerUserName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_IndexServerName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_IndexServerPassword, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraTabPageControl1
        '
        Me.UltraTabPageControl1.Controls.Add(Me.UltraGridLocal)
        Me.UltraTabPageControl1.Controls.Add(Me.Panel4)
        Me.UltraTabPageControl1.Location = New System.Drawing.Point(1, 23)
        Me.UltraTabPageControl1.Name = "UltraTabPageControl1"
        Me.UltraTabPageControl1.Size = New System.Drawing.Size(660, 291)
        '
        'UltraGridLocal
        '
        Me.UltraGridLocal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridLocal.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridLocal.Name = "UltraGridLocal"
        Me.UltraGridLocal.Size = New System.Drawing.Size(660, 263)
        Me.UltraGridLocal.TabIndex = 0
        '
        'Panel4
        '
        '
        'Panel4.ClientArea
        '
        Me.Panel4.ClientArea.Controls.Add(Me.btnEdit)
        Me.Panel4.ClientArea.Controls.Add(Me.btnDel)
        Me.Panel4.ClientArea.Controls.Add(Me.btnAdd)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 263)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(660, 28)
        Me.Panel4.TabIndex = 1
        '
        'btnEdit
        '
        Me.btnEdit.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnEdit.Location = New System.Drawing.Point(424, 0)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(83, 28)
        Me.btnEdit.TabIndex = 6
        Me.btnEdit.Text = "Edit"
        '
        'btnDel
        '
        Me.btnDel.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDel.Location = New System.Drawing.Point(507, 0)
        Me.btnDel.Name = "btnDel"
        Me.btnDel.Size = New System.Drawing.Size(83, 28)
        Me.btnDel.TabIndex = 2
        Me.btnDel.Text = "Delete"
        '
        'btnAdd
        '
        Me.btnAdd.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAdd.Location = New System.Drawing.Point(590, 0)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(70, 28)
        Me.btnAdd.TabIndex = 3
        Me.btnAdd.Text = "Add New"
        '
        'UltraPanel1
        '
        '
        'UltraPanel1.ClientArea
        '
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel5)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_IndexServerPassword)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel3)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel4)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_IndexServerUserName)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_IndexServerName)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.Label9)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.cmb_UserDomainID)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel2)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel1)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_MacAddresses)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_ServerName)
        Me.UltraPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.UltraPanel1.Location = New System.Drawing.Point(0, 0)
        Me.UltraPanel1.Name = "UltraPanel1"
        Me.UltraPanel1.Size = New System.Drawing.Size(664, 159)
        Me.UltraPanel1.TabIndex = 0
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(66, 73)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(68, 14)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "User Domain"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmb_UserDomainID
        '
        Appearance5.FontData.BoldAsString = "False"
        Me.cmb_UserDomainID.Appearance = Appearance5
        Me.cmb_UserDomainID.Location = New System.Drawing.Point(140, 69)
        Me.cmb_UserDomainID.Name = "cmb_UserDomainID"
        Me.cmb_UserDomainID.Size = New System.Drawing.Size(160, 22)
        Me.cmb_UserDomainID.TabIndex = 9
        Me.cmb_UserDomainID.Text = "UltraCombo5"
        '
        'UltraLabel2
        '
        Appearance6.FontData.BoldAsString = "False"
        Appearance6.TextHAlignAsString = "Right"
        Appearance6.TextVAlignAsString = "Middle"
        Me.UltraLabel2.Appearance = Appearance6
        Me.UltraLabel2.AutoSize = True
        Me.UltraLabel2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel2.Location = New System.Drawing.Point(64, 48)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(70, 14)
        Me.UltraLabel2.TabIndex = 2
        Me.UltraLabel2.Text = "Mac Address"
        '
        'UltraLabel1
        '
        Appearance7.FontData.BoldAsString = "False"
        Appearance7.TextHAlignAsString = "Right"
        Appearance7.TextVAlignAsString = "Middle"
        Me.UltraLabel1.Appearance = Appearance7
        Me.UltraLabel1.AutoSize = True
        Me.UltraLabel1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel1.Location = New System.Drawing.Point(63, 18)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(71, 14)
        Me.UltraLabel1.TabIndex = 0
        Me.UltraLabel1.Text = "Server Name"
        '
        'txt_MacAddresses
        '
        Me.txt_MacAddresses.Location = New System.Drawing.Point(140, 42)
        Me.txt_MacAddresses.Name = "txt_MacAddresses"
        Me.txt_MacAddresses.Size = New System.Drawing.Size(450, 21)
        Me.txt_MacAddresses.TabIndex = 3
        Me.txt_MacAddresses.Text = "UltraTextEditor1"
        '
        'txt_ServerName
        '
        Appearance8.FontData.SizeInPoints = 10.0!
        Me.txt_ServerName.Appearance = Appearance8
        Me.txt_ServerName.Location = New System.Drawing.Point(140, 12)
        Me.txt_ServerName.Name = "txt_ServerName"
        Me.txt_ServerName.Size = New System.Drawing.Size(160, 24)
        Me.txt_ServerName.TabIndex = 1
        Me.txt_ServerName.Text = "UltraTextEditor1"
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
        Me.Panel1.Location = New System.Drawing.Point(0, 476)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(664, 33)
        Me.Panel1.TabIndex = 2
        '
        'btnSave
        '
        Appearance9.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance9
        Me.btnSave.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnSave.Location = New System.Drawing.Point(460, 0)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(68, 33)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        '
        'btnCancel
        '
        Appearance10.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance10
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnCancel.Location = New System.Drawing.Point(528, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(68, 33)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'btnOK
        '
        Appearance11.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance11
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnOK.Location = New System.Drawing.Point(596, 0)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(68, 33)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        '
        'UltraTabControl1
        '
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl1)
        Me.UltraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl1.Location = New System.Drawing.Point(0, 159)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.Size = New System.Drawing.Size(664, 317)
        Me.UltraTabControl1.TabIndex = 1
        UltraTab1.Key = "dir"
        UltraTab1.TabPage = Me.UltraTabPageControl1
        UltraTab1.Text = "Shares"
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1})
        '
        'UltraTabSharedControlsPage1
        '
        Me.UltraTabSharedControlsPage1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        Me.UltraTabSharedControlsPage1.Size = New System.Drawing.Size(660, 291)
        '
        'UltraLabel3
        '
        Appearance2.FontData.BoldAsString = "False"
        Appearance2.TextHAlignAsString = "Right"
        Appearance2.TextVAlignAsString = "Middle"
        Me.UltraLabel3.Appearance = Appearance2
        Me.UltraLabel3.AutoSize = True
        Me.UltraLabel3.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel3.Location = New System.Drawing.Point(39, 133)
        Me.UltraLabel3.Name = "UltraLabel3"
        Me.UltraLabel3.Size = New System.Drawing.Size(95, 14)
        Me.UltraLabel3.TabIndex = 12
        Me.UltraLabel3.Text = "Index Server User"
        '
        'UltraLabel4
        '
        Appearance3.FontData.BoldAsString = "False"
        Appearance3.TextHAlignAsString = "Right"
        Appearance3.TextVAlignAsString = "Middle"
        Me.UltraLabel4.Appearance = Appearance3
        Me.UltraLabel4.AutoSize = True
        Me.UltraLabel4.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel4.Location = New System.Drawing.Point(32, 103)
        Me.UltraLabel4.Name = "UltraLabel4"
        Me.UltraLabel4.Size = New System.Drawing.Size(102, 14)
        Me.UltraLabel4.TabIndex = 10
        Me.UltraLabel4.Text = "Index Server Name"
        '
        'txt_IndexServerUserName
        '
        Me.txt_IndexServerUserName.Location = New System.Drawing.Point(140, 127)
        Me.txt_IndexServerUserName.Name = "txt_IndexServerUserName"
        Me.txt_IndexServerUserName.Size = New System.Drawing.Size(160, 21)
        Me.txt_IndexServerUserName.TabIndex = 13
        Me.txt_IndexServerUserName.Text = "UltraTextEditor1"
        '
        'txt_IndexServerName
        '
        Appearance4.FontData.SizeInPoints = 10.0!
        Me.txt_IndexServerName.Appearance = Appearance4
        Me.txt_IndexServerName.Location = New System.Drawing.Point(140, 97)
        Me.txt_IndexServerName.Name = "txt_IndexServerName"
        Me.txt_IndexServerName.Size = New System.Drawing.Size(160, 24)
        Me.txt_IndexServerName.TabIndex = 11
        Me.txt_IndexServerName.Text = "UltraTextEditor1"
        '
        'UltraLabel5
        '
        Appearance1.FontData.BoldAsString = "False"
        Appearance1.TextHAlignAsString = "Right"
        Appearance1.TextVAlignAsString = "Middle"
        Me.UltraLabel5.Appearance = Appearance1
        Me.UltraLabel5.AutoSize = True
        Me.UltraLabel5.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel5.Location = New System.Drawing.Point(385, 131)
        Me.UltraLabel5.Name = "UltraLabel5"
        Me.UltraLabel5.Size = New System.Drawing.Size(54, 14)
        Me.UltraLabel5.TabIndex = 14
        Me.UltraLabel5.Text = "Password"
        '
        'txt_IndexServerPassword
        '
        Me.txt_IndexServerPassword.Location = New System.Drawing.Point(445, 127)
        Me.txt_IndexServerPassword.Name = "txt_IndexServerPassword"
        Me.txt_IndexServerPassword.Size = New System.Drawing.Size(160, 21)
        Me.txt_IndexServerPassword.TabIndex = 15
        Me.txt_IndexServerPassword.Text = "UltraTextEditor1"
        '
        'frmWorkSpace
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "Work Space"
        Me.ClientSize = New System.Drawing.Size(664, 509)
        Me.Controls.Add(Me.UltraTabControl1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.UltraPanel1)
        Me.Name = "frmWorkSpace"
        Me.Text = "Work Space"
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl1.ResumeLayout(False)
        CType(Me.UltraGridLocal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ClientArea.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.PerformLayout()
        Me.UltraPanel1.ResumeLayout(False)
        CType(Me.cmb_UserDomainID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_MacAddresses, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_ServerName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ClientArea.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        CType(Me.txt_IndexServerUserName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_IndexServerName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_IndexServerPassword, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents txt_MacAddresses As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_ServerName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Panel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl1 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridLocal As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents Panel4 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAdd As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Label9 As Label
    Friend WithEvents cmb_UserDomainID As UltraCombo
    Friend WithEvents btnEdit As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraLabel5 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_IndexServerPassword As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_IndexServerUserName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_IndexServerName As Infragistics.Win.UltraWinEditors.UltraTextEditor

#End Region
End Class

