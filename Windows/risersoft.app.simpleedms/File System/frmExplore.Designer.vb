Imports Infragistics.Win.UltraWinGrid
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmExplore
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
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExplore))
        Dim UltraExplorerBarGroup2 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup()
        Dim UltraExplorerBarGroup4 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup()
        Dim UltraExplorerBarGroup3 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup()
        Dim UltraExplorerBarGroup1 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup()
        Dim UltraExplorerBarGroup5 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup()
        Me.UltraExplorerBarContainerControl5 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl()
        Me.UltraExplorerBarContainerControl3 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl()
        Me.UltraExplorerBarContainerControl4 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl()
        Me.UltraExplorerBarContainerControl1 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl()
        Me.UltraExplorerBarContainerControl2 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.flView = New LogicNP.FileViewControl.FileView(Me.components)
        Me.formToolBar = New System.Windows.Forms.ToolBar()
        Me.GoUp = New System.Windows.Forms.ToolBarButton()
        Me.separator1 = New System.Windows.Forms.ToolBarButton()
        Me.Cut = New System.Windows.Forms.ToolBarButton()
        Me.Copy = New System.Windows.Forms.ToolBarButton()
        Me.Paste = New System.Windows.Forms.ToolBarButton()
        Me.separator3 = New System.Windows.Forms.ToolBarButton()
        Me.Delete = New System.Windows.Forms.ToolBarButton()
        Me.separator2 = New System.Windows.Forms.ToolBarButton()
        Me.ViewStyles = New System.Windows.Forms.ToolBarButton()
        Me.ViewStyleMenu = New System.Windows.Forms.ContextMenu()
        Me.menuLargeIcons = New System.Windows.Forms.MenuItem()
        Me.menuThumbnails = New System.Windows.Forms.MenuItem()
        Me.menuList = New System.Windows.Forms.MenuItem()
        Me.menuReport = New System.Windows.Forms.MenuItem()
        Me.imageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.UltraSplitter2 = New Infragistics.Win.Misc.UltraSplitter()
        Me.UltraExplorerBar1 = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar()
        Me.UltraSplitter3 = New Infragistics.Win.Misc.UltraSplitter()
        Me.fldrView = New LogicNP.FolderViewControl.FolderView()
        Me.UltraSplitter1 = New Infragistics.Win.Misc.UltraSplitter()
        Me.PanelDetails = New System.Windows.Forms.Panel()
        Me.formStatusBar = New System.Windows.Forms.StatusBar()
        Me.statusBarPanel1 = New System.Windows.Forms.StatusBarPanel()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.UltraExplorerBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraExplorerBar1.SuspendLayout()
        CType(Me.statusBarPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraExplorerBarContainerControl5
        '
        Me.UltraExplorerBarContainerControl5.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraExplorerBarContainerControl5.Name = "UltraExplorerBarContainerControl5"
        Me.UltraExplorerBarContainerControl5.Size = New System.Drawing.Size(212, 263)
        Me.UltraExplorerBarContainerControl5.TabIndex = 4
        Me.UltraExplorerBarContainerControl5.Visible = False
        '
        'UltraExplorerBarContainerControl3
        '
        Me.UltraExplorerBarContainerControl3.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraExplorerBarContainerControl3.Name = "UltraExplorerBarContainerControl3"
        Me.UltraExplorerBarContainerControl3.Size = New System.Drawing.Size(180, 305)
        Me.UltraExplorerBarContainerControl3.TabIndex = 2
        Me.UltraExplorerBarContainerControl3.Visible = False
        '
        'UltraExplorerBarContainerControl4
        '
        Me.UltraExplorerBarContainerControl4.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraExplorerBarContainerControl4.Name = "UltraExplorerBarContainerControl4"
        Me.UltraExplorerBarContainerControl4.Size = New System.Drawing.Size(212, 263)
        Me.UltraExplorerBarContainerControl4.TabIndex = 3
        Me.UltraExplorerBarContainerControl4.Visible = False
        '
        'UltraExplorerBarContainerControl1
        '
        Me.UltraExplorerBarContainerControl1.Location = New System.Drawing.Point(6, 106)
        Me.UltraExplorerBarContainerControl1.Name = "UltraExplorerBarContainerControl1"
        Me.UltraExplorerBarContainerControl1.Size = New System.Drawing.Size(212, 263)
        Me.UltraExplorerBarContainerControl1.TabIndex = 0
        '
        'UltraExplorerBarContainerControl2
        '
        Me.UltraExplorerBarContainerControl2.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraExplorerBarContainerControl2.Name = "UltraExplorerBarContainerControl2"
        Me.UltraExplorerBarContainerControl2.Size = New System.Drawing.Size(212, 263)
        Me.UltraExplorerBarContainerControl2.TabIndex = 5
        Me.UltraExplorerBarContainerControl2.Visible = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.flView)
        Me.Panel2.Controls.Add(Me.formToolBar)
        Me.Panel2.Controls.Add(Me.UltraSplitter2)
        Me.Panel2.Controls.Add(Me.UltraExplorerBar1)
        Me.Panel2.Controls.Add(Me.UltraSplitter3)
        Me.Panel2.Controls.Add(Me.fldrView)
        Me.Panel2.Controls.Add(Me.UltraSplitter1)
        Me.Panel2.Controls.Add(Me.PanelDetails)
        Me.Panel2.Controls.Add(Me.formStatusBar)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(879, 486)
        Me.Panel2.TabIndex = 16
        '
        'flView
        '
        Me.flView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flView.Location = New System.Drawing.Point(215, 34)
        Me.flView.Name = "flView"
        Me.flView.Size = New System.Drawing.Size(432, 366)
        Me.flView.TabIndex = 27
        Me.flView.Text = "fileView1"
        Me.flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
        '
        'formToolBar
        '
        Me.formToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat
        Me.formToolBar.Buttons.AddRange(New System.Windows.Forms.ToolBarButton() {Me.GoUp, Me.separator1, Me.Cut, Me.Copy, Me.Paste, Me.separator3, Me.Delete, Me.separator2, Me.ViewStyles})
        Me.formToolBar.ButtonSize = New System.Drawing.Size(24, 24)
        Me.formToolBar.Divider = False
        Me.formToolBar.DropDownArrows = True
        Me.formToolBar.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.formToolBar.ImageList = Me.imageList1
        Me.formToolBar.Location = New System.Drawing.Point(215, 0)
        Me.formToolBar.Name = "formToolBar"
        Me.formToolBar.ShowToolTips = True
        Me.formToolBar.Size = New System.Drawing.Size(432, 34)
        Me.formToolBar.TabIndex = 26
        '
        'GoUp
        '
        Me.GoUp.ImageIndex = 0
        Me.GoUp.Name = "GoUp"
        Me.GoUp.ToolTipText = "Go Up"
        '
        'separator1
        '
        Me.separator1.Name = "separator1"
        Me.separator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator
        '
        'Cut
        '
        Me.Cut.ImageIndex = 1
        Me.Cut.Name = "Cut"
        Me.Cut.ToolTipText = "Cut"
        '
        'Copy
        '
        Me.Copy.ImageIndex = 2
        Me.Copy.Name = "Copy"
        Me.Copy.ToolTipText = "Copy"
        '
        'Paste
        '
        Me.Paste.ImageIndex = 3
        Me.Paste.Name = "Paste"
        Me.Paste.ToolTipText = "Paste"
        '
        'separator3
        '
        Me.separator3.Name = "separator3"
        Me.separator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator
        '
        'Delete
        '
        Me.Delete.ImageIndex = 4
        Me.Delete.Name = "Delete"
        Me.Delete.ToolTipText = "Delete"
        '
        'separator2
        '
        Me.separator2.Name = "separator2"
        Me.separator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator
        '
        'ViewStyles
        '
        Me.ViewStyles.DropDownMenu = Me.ViewStyleMenu
        Me.ViewStyles.ImageIndex = 5
        Me.ViewStyles.Name = "ViewStyles"
        Me.ViewStyles.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton
        Me.ViewStyles.ToolTipText = "View Styles"
        '
        'ViewStyleMenu
        '
        Me.ViewStyleMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuLargeIcons, Me.menuThumbnails, Me.menuList, Me.menuReport})
        '
        'menuLargeIcons
        '
        Me.menuLargeIcons.Index = 0
        Me.menuLargeIcons.Text = "Large Icons"
        '
        'menuThumbnails
        '
        Me.menuThumbnails.Index = 1
        Me.menuThumbnails.Text = "Thumbnails"
        '
        'menuList
        '
        Me.menuList.Index = 2
        Me.menuList.Text = "List"
        '
        'menuReport
        '
        Me.menuReport.Index = 3
        Me.menuReport.Text = "Report"
        '
        'imageList1
        '
        Me.imageList1.ImageStream = CType(resources.GetObject("imageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imageList1.TransparentColor = System.Drawing.Color.Magenta
        Me.imageList1.Images.SetKeyName(0, "")
        Me.imageList1.Images.SetKeyName(1, "")
        Me.imageList1.Images.SetKeyName(2, "")
        Me.imageList1.Images.SetKeyName(3, "")
        Me.imageList1.Images.SetKeyName(4, "")
        Me.imageList1.Images.SetKeyName(5, "")
        '
        'UltraSplitter2
        '
        Me.UltraSplitter2.BackColor = System.Drawing.SystemColors.Control
        Me.UltraSplitter2.Dock = System.Windows.Forms.DockStyle.Right
        Me.UltraSplitter2.Location = New System.Drawing.Point(647, 0)
        Me.UltraSplitter2.Name = "UltraSplitter2"
        Me.UltraSplitter2.RestoreExtent = 196
        Me.UltraSplitter2.Size = New System.Drawing.Size(8, 400)
        Me.UltraSplitter2.TabIndex = 25
        '
        'UltraExplorerBar1
        '
        Me.UltraExplorerBar1.Controls.Add(Me.UltraExplorerBarContainerControl1)
        Me.UltraExplorerBar1.Controls.Add(Me.UltraExplorerBarContainerControl3)
        Me.UltraExplorerBar1.Controls.Add(Me.UltraExplorerBarContainerControl4)
        Me.UltraExplorerBar1.Controls.Add(Me.UltraExplorerBarContainerControl5)
        Me.UltraExplorerBar1.Controls.Add(Me.UltraExplorerBarContainerControl2)
        Me.UltraExplorerBar1.Dock = System.Windows.Forms.DockStyle.Right
        UltraExplorerBarGroup2.Container = Me.UltraExplorerBarContainerControl5
        UltraExplorerBarGroup2.Key = "info"
        UltraExplorerBarGroup2.Text = "Info"
        UltraExplorerBarGroup4.Container = Me.UltraExplorerBarContainerControl3
        UltraExplorerBarGroup4.Key = "perm"
        UltraExplorerBarGroup4.Text = "Locks"
        UltraExplorerBarGroup3.Container = Me.UltraExplorerBarContainerControl4
        UltraExplorerBarGroup3.Key = "act"
        UltraExplorerBarGroup3.Text = "Activity"
        UltraExplorerBarGroup1.Container = Me.UltraExplorerBarContainerControl1
        UltraExplorerBarGroup1.Key = "preview"
        UltraExplorerBarGroup1.Text = "Preview"
        UltraExplorerBarGroup5.Container = Me.UltraExplorerBarContainerControl2
        UltraExplorerBarGroup5.Key = "backup"
        UltraExplorerBarGroup5.Text = "Backups"
        Me.UltraExplorerBar1.Groups.AddRange(New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup() {UltraExplorerBarGroup2, UltraExplorerBarGroup4, UltraExplorerBarGroup3, UltraExplorerBarGroup1, UltraExplorerBarGroup5})
        Me.UltraExplorerBar1.GroupSettings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer
        Me.UltraExplorerBar1.Location = New System.Drawing.Point(655, 0)
        Me.UltraExplorerBar1.Name = "UltraExplorerBar1"
        Me.UltraExplorerBar1.Size = New System.Drawing.Size(224, 400)
        Me.UltraExplorerBar1.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.Listbar
        Me.UltraExplorerBar1.TabIndex = 22
        '
        'UltraSplitter3
        '
        Me.UltraSplitter3.Location = New System.Drawing.Point(207, 0)
        Me.UltraSplitter3.Name = "UltraSplitter3"
        Me.UltraSplitter3.RestoreExtent = 0
        Me.UltraSplitter3.Size = New System.Drawing.Size(8, 400)
        Me.UltraSplitter3.TabIndex = 21
        '
        'fldrView
        '
        Me.fldrView.Dock = System.Windows.Forms.DockStyle.Left
        Me.fldrView.Location = New System.Drawing.Point(0, 0)
        Me.fldrView.Name = "fldrView"
        Me.fldrView.Size = New System.Drawing.Size(207, 400)
        Me.fldrView.TabIndex = 19
        Me.fldrView.Text = "folderView1"
        '
        'UltraSplitter1
        '
        Me.UltraSplitter1.BackColor = System.Drawing.SystemColors.Control
        Me.UltraSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraSplitter1.Location = New System.Drawing.Point(0, 400)
        Me.UltraSplitter1.Name = "UltraSplitter1"
        Me.UltraSplitter1.RestoreExtent = 42
        Me.UltraSplitter1.Size = New System.Drawing.Size(879, 8)
        Me.UltraSplitter1.TabIndex = 15
        '
        'PanelDetails
        '
        Me.PanelDetails.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelDetails.Location = New System.Drawing.Point(0, 408)
        Me.PanelDetails.Name = "PanelDetails"
        Me.PanelDetails.Size = New System.Drawing.Size(879, 59)
        Me.PanelDetails.TabIndex = 14
        '
        'formStatusBar
        '
        Me.formStatusBar.Location = New System.Drawing.Point(0, 467)
        Me.formStatusBar.Name = "formStatusBar"
        Me.formStatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.statusBarPanel1})
        Me.formStatusBar.ShowPanels = True
        Me.formStatusBar.Size = New System.Drawing.Size(879, 19)
        Me.formStatusBar.TabIndex = 12
        Me.formStatusBar.Text = "statusBar1"
        '
        'statusBarPanel1
        '
        Me.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
        Me.statusBarPanel1.Name = "statusBarPanel1"
        Me.statusBarPanel1.Text = "statusBarPanel1"
        Me.statusBarPanel1.Width = 862
        '
        'frmExplore
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "SimpleEDMS Explorer"
        Me.ClientSize = New System.Drawing.Size(879, 486)
        Me.Controls.Add(Me.Panel2)
        Me.Name = "frmExplore"
        Me.Text = "SimpleEDMS Explorer"
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.UltraExplorerBar1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraExplorerBar1.ResumeLayout(False)
        CType(Me.statusBarPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents ViewStyleMenu As System.Windows.Forms.ContextMenu
    Private WithEvents menuLargeIcons As System.Windows.Forms.MenuItem
    Private WithEvents menuThumbnails As System.Windows.Forms.MenuItem
    Private WithEvents menuList As System.Windows.Forms.MenuItem
    Private WithEvents menuReport As System.Windows.Forms.MenuItem
    Friend WithEvents imageList1 As System.Windows.Forms.ImageList
    Private WithEvents formStatusBar As System.Windows.Forms.StatusBar
    Private WithEvents statusBarPanel1 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents PanelDetails As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents UltraSplitter3 As Infragistics.Win.Misc.UltraSplitter
    Private WithEvents fldrView As LogicNP.FolderViewControl.FolderView
    Friend WithEvents UltraSplitter1 As Infragistics.Win.Misc.UltraSplitter
    Private WithEvents flView As LogicNP.FileViewControl.FileView
    Friend WithEvents formToolBar As System.Windows.Forms.ToolBar
    Friend WithEvents GoUp As System.Windows.Forms.ToolBarButton
    Friend WithEvents separator1 As System.Windows.Forms.ToolBarButton
    Friend WithEvents Cut As System.Windows.Forms.ToolBarButton
    Friend WithEvents Copy As System.Windows.Forms.ToolBarButton
    Friend WithEvents Paste As System.Windows.Forms.ToolBarButton
    Friend WithEvents separator3 As System.Windows.Forms.ToolBarButton
    Friend WithEvents Delete As System.Windows.Forms.ToolBarButton
    Friend WithEvents separator2 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ViewStyles As System.Windows.Forms.ToolBarButton
    Friend WithEvents UltraSplitter2 As Infragistics.Win.Misc.UltraSplitter
    Friend WithEvents UltraExplorerBar1 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar
    Friend WithEvents UltraExplorerBarContainerControl1 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl
    Friend WithEvents UltraExplorerBarContainerControl3 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl
    Friend WithEvents UltraExplorerBarContainerControl4 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl
    Friend WithEvents UltraExplorerBarContainerControl5 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl
    Friend WithEvents UltraExplorerBarContainerControl2 As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl

#End Region
End Class

