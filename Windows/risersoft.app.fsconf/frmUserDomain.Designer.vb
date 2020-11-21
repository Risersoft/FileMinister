<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class frmUserDomain
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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraTab8 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Me.UltraTabPageControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridUserMap = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnDelUser = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddUser = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabPageControl2 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridGroupMap = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraPanel2 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnDelGroup = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddGroup = New Infragistics.Win.Misc.UltraButton()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.UltraPanel3 = New Infragistics.Win.Misc.UltraPanel()
        Me.txt_BDCServer = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.txt_PDCServer = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel9 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_DomainName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabControl2 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage2 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl1.SuspendLayout()
        CType(Me.UltraGridUserMap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraPanel1.ClientArea.SuspendLayout()
        Me.UltraPanel1.SuspendLayout()
        Me.UltraTabPageControl2.SuspendLayout()
        CType(Me.UltraGridGroupMap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraPanel2.ClientArea.SuspendLayout()
        Me.UltraPanel2.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.UltraPanel3.ClientArea.SuspendLayout()
        Me.UltraPanel3.SuspendLayout()
        CType(Me.txt_BDCServer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_PDCServer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_DomainName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        CType(Me.UltraTabControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl2.SuspendLayout()
        Me.SuspendLayout()
        '
        'UltraTabPageControl1
        '
        Me.UltraTabPageControl1.Controls.Add(Me.UltraGridUserMap)
        Me.UltraTabPageControl1.Controls.Add(Me.UltraPanel1)
        Me.UltraTabPageControl1.Location = New System.Drawing.Point(1, 23)
        Me.UltraTabPageControl1.Name = "UltraTabPageControl1"
        Me.UltraTabPageControl1.Size = New System.Drawing.Size(600, 106)
        '
        'UltraGridUserMap
        '
        Me.UltraGridUserMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridUserMap.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridUserMap.Name = "UltraGridUserMap"
        Me.UltraGridUserMap.Size = New System.Drawing.Size(600, 78)
        Me.UltraGridUserMap.TabIndex = 0
        '
        'UltraPanel1
        '
        '
        'UltraPanel1.ClientArea
        '
        Me.UltraPanel1.ClientArea.Controls.Add(Me.btnDelUser)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.btnAddUser)
        Me.UltraPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraPanel1.Location = New System.Drawing.Point(0, 78)
        Me.UltraPanel1.Name = "UltraPanel1"
        Me.UltraPanel1.Size = New System.Drawing.Size(600, 28)
        Me.UltraPanel1.TabIndex = 1
        '
        'btnDelUser
        '
        Me.btnDelUser.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelUser.Location = New System.Drawing.Point(447, 0)
        Me.btnDelUser.Name = "btnDelUser"
        Me.btnDelUser.Size = New System.Drawing.Size(83, 28)
        Me.btnDelUser.TabIndex = 2
        Me.btnDelUser.Text = "Delete"
        '
        'btnAddUser
        '
        Me.btnAddUser.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddUser.Location = New System.Drawing.Point(530, 0)
        Me.btnAddUser.Name = "btnAddUser"
        Me.btnAddUser.Size = New System.Drawing.Size(70, 28)
        Me.btnAddUser.TabIndex = 3
        Me.btnAddUser.Text = "Add New"
        '
        'UltraTabPageControl2
        '
        Me.UltraTabPageControl2.Controls.Add(Me.UltraGridGroupMap)
        Me.UltraTabPageControl2.Controls.Add(Me.UltraPanel2)
        Me.UltraTabPageControl2.Location = New System.Drawing.Point(1, 23)
        Me.UltraTabPageControl2.Name = "UltraTabPageControl2"
        Me.UltraTabPageControl2.Size = New System.Drawing.Size(600, 118)
        '
        'UltraGridGroupMap
        '
        Me.UltraGridGroupMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridGroupMap.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridGroupMap.Name = "UltraGridGroupMap"
        Me.UltraGridGroupMap.Size = New System.Drawing.Size(600, 90)
        Me.UltraGridGroupMap.TabIndex = 0
        '
        'UltraPanel2
        '
        '
        'UltraPanel2.ClientArea
        '
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnDelGroup)
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnAddGroup)
        Me.UltraPanel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraPanel2.Location = New System.Drawing.Point(0, 90)
        Me.UltraPanel2.Name = "UltraPanel2"
        Me.UltraPanel2.Size = New System.Drawing.Size(600, 28)
        Me.UltraPanel2.TabIndex = 1
        '
        'btnDelGroup
        '
        Me.btnDelGroup.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelGroup.Location = New System.Drawing.Point(447, 0)
        Me.btnDelGroup.Name = "btnDelGroup"
        Me.btnDelGroup.Size = New System.Drawing.Size(83, 28)
        Me.btnDelGroup.TabIndex = 2
        Me.btnDelGroup.Text = "Delete"
        '
        'btnAddGroup
        '
        Me.btnAddGroup.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddGroup.Location = New System.Drawing.Point(530, 0)
        Me.btnAddGroup.Name = "btnAddGroup"
        Me.btnAddGroup.Size = New System.Drawing.Size(70, 28)
        Me.btnAddGroup.TabIndex = 3
        Me.btnAddGroup.Text = "Add New"
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.btnSave)
        Me.Panel4.Controls.Add(Me.btnCancel)
        Me.Panel4.Controls.Add(Me.btnOK)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 368)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(604, 48)
        Me.Panel4.TabIndex = 3
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance1.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance1
        Me.btnSave.Location = New System.Drawing.Point(316, 8)
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
        Me.btnCancel.Location = New System.Drawing.Point(412, 8)
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
        Me.btnOK.Location = New System.Drawing.Point(508, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(88, 32)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        '
        'UltraPanel3
        '
        '
        'UltraPanel3.ClientArea
        '
        Me.UltraPanel3.ClientArea.Controls.Add(Me.txt_BDCServer)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.txt_PDCServer)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.UltraLabel3)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.UltraLabel4)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.UltraLabel9)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.txt_DomainName)
        Me.UltraPanel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.UltraPanel3.Location = New System.Drawing.Point(0, 0)
        Me.UltraPanel3.Name = "UltraPanel3"
        Me.UltraPanel3.Size = New System.Drawing.Size(604, 92)
        Me.UltraPanel3.TabIndex = 163
        '
        'txt_BDCServer
        '
        Appearance4.FontData.BoldAsString = "False"
        Appearance4.FontData.ItalicAsString = "False"
        Appearance4.FontData.Name = "Arial"
        Appearance4.FontData.SizeInPoints = 8.25!
        Appearance4.FontData.StrikeoutAsString = "False"
        Appearance4.FontData.UnderlineAsString = "False"
        Me.txt_BDCServer.Appearance = Appearance4
        Me.txt_BDCServer.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_BDCServer.Location = New System.Drawing.Point(124, 59)
        Me.txt_BDCServer.Name = "txt_BDCServer"
        Me.txt_BDCServer.Size = New System.Drawing.Size(238, 21)
        Me.txt_BDCServer.TabIndex = 173
        '
        'txt_PDCServer
        '
        Appearance5.FontData.BoldAsString = "False"
        Appearance5.FontData.ItalicAsString = "False"
        Appearance5.FontData.Name = "Arial"
        Appearance5.FontData.SizeInPoints = 8.25!
        Appearance5.FontData.StrikeoutAsString = "False"
        Appearance5.FontData.UnderlineAsString = "False"
        Me.txt_PDCServer.Appearance = Appearance5
        Me.txt_PDCServer.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_PDCServer.Location = New System.Drawing.Point(125, 33)
        Me.txt_PDCServer.Name = "txt_PDCServer"
        Me.txt_PDCServer.Size = New System.Drawing.Size(237, 21)
        Me.txt_PDCServer.TabIndex = 172
        '
        'UltraLabel3
        '
        Appearance6.TextHAlignAsString = "Right"
        Me.UltraLabel3.Appearance = Appearance6
        Me.UltraLabel3.Location = New System.Drawing.Point(37, 59)
        Me.UltraLabel3.Name = "UltraLabel3"
        Me.UltraLabel3.Size = New System.Drawing.Size(80, 16)
        Me.UltraLabel3.TabIndex = 171
        Me.UltraLabel3.Text = "BDC Server"
        '
        'UltraLabel4
        '
        Appearance7.TextHAlignAsString = "Right"
        Me.UltraLabel4.Appearance = Appearance7
        Me.UltraLabel4.Location = New System.Drawing.Point(39, 33)
        Me.UltraLabel4.Name = "UltraLabel4"
        Me.UltraLabel4.Size = New System.Drawing.Size(80, 16)
        Me.UltraLabel4.TabIndex = 170
        Me.UltraLabel4.Text = "PDC Server"
        '
        'UltraLabel9
        '
        Appearance8.TextHAlignAsString = "Right"
        Me.UltraLabel9.Appearance = Appearance8
        Me.UltraLabel9.Location = New System.Drawing.Point(37, 7)
        Me.UltraLabel9.Name = "UltraLabel9"
        Me.UltraLabel9.Size = New System.Drawing.Size(80, 16)
        Me.UltraLabel9.TabIndex = 169
        Me.UltraLabel9.Text = "Domain Name"
        '
        'txt_DomainName
        '
        Appearance9.FontData.BoldAsString = "False"
        Appearance9.FontData.ItalicAsString = "False"
        Appearance9.FontData.Name = "Arial"
        Appearance9.FontData.SizeInPoints = 8.25!
        Appearance9.FontData.StrikeoutAsString = "False"
        Appearance9.FontData.UnderlineAsString = "False"
        Me.txt_DomainName.Appearance = Appearance9
        Me.txt_DomainName.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_DomainName.Location = New System.Drawing.Point(125, 7)
        Me.txt_DomainName.Name = "txt_DomainName"
        Me.txt_DomainName.Size = New System.Drawing.Size(343, 21)
        Me.txt_DomainName.TabIndex = 168
        '
        'UltraTabSharedControlsPage1
        '
        Me.UltraTabSharedControlsPage1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        Me.UltraTabSharedControlsPage1.Size = New System.Drawing.Size(600, 106)
        '
        'UltraTabControl1
        '
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl1)
        Me.UltraTabControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.UltraTabControl1.Location = New System.Drawing.Point(0, 92)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.Size = New System.Drawing.Size(604, 132)
        Me.UltraTabControl1.TabIndex = 164
        UltraTab8.Key = "user"
        UltraTab8.TabPage = Me.UltraTabPageControl1
        UltraTab8.Text = "User Mapping"
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab8})
        '
        'UltraTabControl2
        '
        Me.UltraTabControl2.Controls.Add(Me.UltraTabSharedControlsPage2)
        Me.UltraTabControl2.Controls.Add(Me.UltraTabPageControl2)
        Me.UltraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl2.Location = New System.Drawing.Point(0, 224)
        Me.UltraTabControl2.Name = "UltraTabControl2"
        Me.UltraTabControl2.SharedControlsPage = Me.UltraTabSharedControlsPage2
        Me.UltraTabControl2.Size = New System.Drawing.Size(604, 144)
        Me.UltraTabControl2.TabIndex = 165
        UltraTab1.Key = "group"
        UltraTab1.TabPage = Me.UltraTabPageControl2
        UltraTab1.Text = "Group Mapping"
        Me.UltraTabControl2.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1})
        '
        'UltraTabSharedControlsPage2
        '
        Me.UltraTabSharedControlsPage2.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage2.Name = "UltraTabSharedControlsPage2"
        Me.UltraTabSharedControlsPage2.Size = New System.Drawing.Size(600, 118)
        '
        'frmUserDomain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "User Domain"
        Me.ClientSize = New System.Drawing.Size(604, 416)
        Me.Controls.Add(Me.UltraTabControl2)
        Me.Controls.Add(Me.UltraTabControl1)
        Me.Controls.Add(Me.UltraPanel3)
        Me.Controls.Add(Me.Panel4)
        Me.Name = "frmUserDomain"
        Me.Text = "User Domain"
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl1.ResumeLayout(False)
        CType(Me.UltraGridUserMap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraPanel1.ClientArea.ResumeLayout(False)
        Me.UltraPanel1.ResumeLayout(False)
        Me.UltraTabPageControl2.ResumeLayout(False)
        CType(Me.UltraGridGroupMap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraPanel2.ClientArea.ResumeLayout(False)
        Me.UltraPanel2.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.UltraPanel3.ClientArea.ResumeLayout(False)
        Me.UltraPanel3.ClientArea.PerformLayout()
        Me.UltraPanel3.ResumeLayout(False)
        CType(Me.txt_BDCServer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_PDCServer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_DomainName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        CType(Me.UltraTabControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UltraPanel3 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents txt_BDCServer As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_PDCServer As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel9 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_DomainName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraTabPageControl1 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridUserMap As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelUser As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddUser As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabControl2 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage2 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl2 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridGroupMap As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents UltraPanel2 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelGroup As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddGroup As Infragistics.Win.Misc.UltraButton

#End Region
End Class

