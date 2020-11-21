Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports LogicNP.FileViewControl
Namespace Explorer

    Public Class Form1
        Inherits System.Windows.Forms.Form
        Friend WithEvents imageList1 As System.Windows.Forms.ImageList
        Friend GoUp As System.Windows.Forms.ToolBarButton
        Friend separator1 As System.Windows.Forms.ToolBarButton
        Friend Cut As System.Windows.Forms.ToolBarButton
        Friend Copy As System.Windows.Forms.ToolBarButton
        Friend Paste As System.Windows.Forms.ToolBarButton
        Friend separator3 As System.Windows.Forms.ToolBarButton
        Friend Delete As System.Windows.Forms.ToolBarButton
        Friend separator2 As System.Windows.Forms.ToolBarButton
        Friend ViewStyles As System.Windows.Forms.ToolBarButton
        Private WithEvents shCmbBox As LogicNP.ShComboBoxControl.ShComboBox
        Private WithEvents fldrView As LogicNP.FolderViewControl.FolderView
        Private WithEvents flView As LogicNP.FileViewControl.FileView
        Friend WithEvents formToolBar As System.Windows.Forms.ToolBar
        Private WithEvents formStatusBar As System.Windows.Forms.StatusBar
        Private WithEvents statusBarPanel1 As System.Windows.Forms.StatusBarPanel
        Private WithEvents ViewStyleMenu As System.Windows.Forms.ContextMenu
        Private WithEvents menuLargeIcons As System.Windows.Forms.MenuItem
        Private WithEvents menuList As System.Windows.Forms.MenuItem
        Private WithEvents menuReport As System.Windows.Forms.MenuItem
        Private WithEvents mainMenu1 As System.Windows.Forms.MainMenu
        Private WithEvents menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuDelete As System.Windows.Forms.MenuItem
        Private WithEvents menuRename As System.Windows.Forms.MenuItem
        Private WithEvents menuProperties As System.Windows.Forms.MenuItem
        Private WithEvents menuClose As System.Windows.Forms.MenuItem
        Private WithEvents menuEdit As System.Windows.Forms.MenuItem
        Private WithEvents menuCut As System.Windows.Forms.MenuItem
        Private WithEvents menuCopy As System.Windows.Forms.MenuItem
        Private WithEvents menuPaste As System.Windows.Forms.MenuItem
        Private WithEvents menuView As System.Windows.Forms.MenuItem
        Private WithEvents menuLargeIcon As System.Windows.Forms.MenuItem
        Private WithEvents menuListStyle As System.Windows.Forms.MenuItem
        Private WithEvents menuReportStyle As System.Windows.Forms.MenuItem
        Private WithEvents menuRefresh As System.Windows.Forms.MenuItem
        Private WithEvents menuTools As System.Windows.Forms.MenuItem
        Private WithEvents menuOptions As System.Windows.Forms.MenuItem
        Private WithEvents menuHelp As System.Windows.Forms.MenuItem
        Private WithEvents menuAbout As System.Windows.Forms.MenuItem
        Private components As System.ComponentModel.IContainer

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub
        Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
        Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
        Private WithEvents menuThumbnails As System.Windows.Forms.MenuItem
        Private WithEvents menuThumbnail As System.Windows.Forms.MenuItem

        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
            Me.shCmbBox = New LogicNP.ShComboBoxControl.ShComboBox()
            Me.fldrView = New LogicNP.FolderViewControl.FolderView()
            Me.flView = New LogicNP.FileViewControl.FileView(Me.components)
            Me.imageList1 = New System.Windows.Forms.ImageList(Me.components)
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
            Me.formStatusBar = New System.Windows.Forms.StatusBar()
            Me.statusBarPanel1 = New System.Windows.Forms.StatusBarPanel()
            Me.mainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
            Me.menuFile = New System.Windows.Forms.MenuItem()
            Me.menuDelete = New System.Windows.Forms.MenuItem()
            Me.menuRename = New System.Windows.Forms.MenuItem()
            Me.menuProperties = New System.Windows.Forms.MenuItem()
            Me.MenuItem1 = New System.Windows.Forms.MenuItem()
            Me.menuClose = New System.Windows.Forms.MenuItem()
            Me.menuEdit = New System.Windows.Forms.MenuItem()
            Me.menuCut = New System.Windows.Forms.MenuItem()
            Me.menuCopy = New System.Windows.Forms.MenuItem()
            Me.menuPaste = New System.Windows.Forms.MenuItem()
            Me.menuView = New System.Windows.Forms.MenuItem()
            Me.menuLargeIcon = New System.Windows.Forms.MenuItem()
            Me.menuThumbnail = New System.Windows.Forms.MenuItem()
            Me.menuListStyle = New System.Windows.Forms.MenuItem()
            Me.menuReportStyle = New System.Windows.Forms.MenuItem()
            Me.MenuItem2 = New System.Windows.Forms.MenuItem()
            Me.menuRefresh = New System.Windows.Forms.MenuItem()
            Me.menuTools = New System.Windows.Forms.MenuItem()
            Me.menuOptions = New System.Windows.Forms.MenuItem()
            Me.menuHelp = New System.Windows.Forms.MenuItem()
            Me.menuAbout = New System.Windows.Forms.MenuItem()
            CType(Me.statusBarPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'shCmbBox
            '
            Me.shCmbBox.Location = New System.Drawing.Point(13, 21)
            Me.shCmbBox.Name = "shCmbBox"
            Me.shCmbBox.Size = New System.Drawing.Size(207, 22)
            Me.shCmbBox.TabIndex = 0
            Me.shCmbBox.Text = "shComboBox1"
            '
            'fldrView
            '
            Me.fldrView.Location = New System.Drawing.Point(13, 55)
            Me.fldrView.Name = "fldrView"
            Me.fldrView.Size = New System.Drawing.Size(207, 312)
            Me.fldrView.TabIndex = 1
            Me.fldrView.Text = "folderView1"
            '
            'flView
            '
            Me.flView.Location = New System.Drawing.Point(240, 55)
            Me.flView.Name = "flView"
            Me.flView.Size = New System.Drawing.Size(387, 319)
            Me.flView.TabIndex = 2
            Me.flView.Text = "fileView1"
            Me.flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
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
            'formToolBar
            '
            Me.formToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat
            Me.formToolBar.Buttons.AddRange(New System.Windows.Forms.ToolBarButton() {Me.GoUp, Me.separator1, Me.Cut, Me.Copy, Me.Paste, Me.separator3, Me.Delete, Me.separator2, Me.ViewStyles})
            Me.formToolBar.ButtonSize = New System.Drawing.Size(24, 24)
            Me.formToolBar.Divider = False
            Me.formToolBar.Dock = System.Windows.Forms.DockStyle.None
            Me.formToolBar.DropDownArrows = True
            Me.formToolBar.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.formToolBar.ImageList = Me.imageList1
            Me.formToolBar.Location = New System.Drawing.Point(240, 14)
            Me.formToolBar.Name = "formToolBar"
            Me.formToolBar.ShowToolTips = True
            Me.formToolBar.Size = New System.Drawing.Size(298, 34)
            Me.formToolBar.TabIndex = 7
            Me.formToolBar.Tag = ""
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
            'formStatusBar
            '
            Me.formStatusBar.Location = New System.Drawing.Point(0, 437)
            Me.formStatusBar.Name = "formStatusBar"
            Me.formStatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.statusBarPanel1})
            Me.formStatusBar.ShowPanels = True
            Me.formStatusBar.Size = New System.Drawing.Size(760, 19)
            Me.formStatusBar.TabIndex = 8
            Me.formStatusBar.Text = "statusBar1"
            '
            'statusBarPanel1
            '
            Me.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
            Me.statusBarPanel1.Name = "statusBarPanel1"
            Me.statusBarPanel1.Text = "statusBarPanel1"
            Me.statusBarPanel1.Width = 743
            '
            'mainMenu1
            '
            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.menuEdit, Me.menuView, Me.menuTools, Me.menuHelp})
            '
            'menuFile
            '
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuDelete, Me.menuRename, Me.menuProperties, Me.MenuItem1, Me.menuClose})
            Me.menuFile.Text = "File"
            '
            'menuDelete
            '
            Me.menuDelete.Index = 0
            Me.menuDelete.Text = "Delete"
            '
            'menuRename
            '
            Me.menuRename.Index = 1
            Me.menuRename.Text = "Rename"
            '
            'menuProperties
            '
            Me.menuProperties.Index = 2
            Me.menuProperties.Text = "Properties"
            '
            'MenuItem1
            '
            Me.MenuItem1.Index = 3
            Me.MenuItem1.Text = "-"
            '
            'menuClose
            '
            Me.menuClose.Index = 4
            Me.menuClose.Text = "Close"
            '
            'menuEdit
            '
            Me.menuEdit.Index = 1
            Me.menuEdit.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuCut, Me.menuCopy, Me.menuPaste})
            Me.menuEdit.Text = "Edit"
            '
            'menuCut
            '
            Me.menuCut.Index = 0
            Me.menuCut.Text = "Cut"
            '
            'menuCopy
            '
            Me.menuCopy.Index = 1
            Me.menuCopy.Text = "Copy"
            '
            'menuPaste
            '
            Me.menuPaste.Index = 2
            Me.menuPaste.Text = "Paste"
            '
            'menuView
            '
            Me.menuView.Index = 2
            Me.menuView.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuLargeIcon, Me.menuThumbnail, Me.menuListStyle, Me.menuReportStyle, Me.MenuItem2, Me.menuRefresh})
            Me.menuView.Text = "View"
            '
            'menuLargeIcon
            '
            Me.menuLargeIcon.Index = 0
            Me.menuLargeIcon.Text = "Large Icons"
            '
            'menuThumbnail
            '
            Me.menuThumbnail.Index = 1
            Me.menuThumbnail.Text = "Thumbnails"
            '
            'menuListStyle
            '
            Me.menuListStyle.Index = 2
            Me.menuListStyle.Text = "List"
            '
            'menuReportStyle
            '
            Me.menuReportStyle.Index = 3
            Me.menuReportStyle.Text = "Report"
            '
            'MenuItem2
            '
            Me.MenuItem2.Index = 4
            Me.MenuItem2.Text = "-"
            '
            'menuRefresh
            '
            Me.menuRefresh.Index = 5
            Me.menuRefresh.Text = "Refresh"
            '
            'menuTools
            '
            Me.menuTools.Index = 3
            Me.menuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuOptions})
            Me.menuTools.Text = "Tools"
            '
            'menuOptions
            '
            Me.menuOptions.Index = 0
            Me.menuOptions.Text = "Options.."
            '
            'menuHelp
            '
            Me.menuHelp.Index = 4
            Me.menuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuAbout})
            Me.menuHelp.Text = "Help"
            '
            'menuAbout
            '
            Me.menuAbout.Index = 0
            Me.menuAbout.Text = "About.."
            '
            'Form1
            '
            Me.ClientSize = New System.Drawing.Size(760, 456)
            Me.Controls.Add(Me.formStatusBar)
            Me.Controls.Add(Me.formToolBar)
            Me.Controls.Add(Me.flView)
            Me.Controls.Add(Me.fldrView)
            Me.Controls.Add(Me.shCmbBox)
            Me.Menu = Me.mainMenu1
            Me.Name = "Form1"
            Me.Text = "Windows Explorer Sample Application"
            Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
            CType(Me.statusBarPanel1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        <STAThread()> _
        Shared Sub Main()
            Application.EnableVisualStyles()
            Application.Run(New Form1())
        End Sub

        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            shCmbBox.FolderView = fldrView
            fldrView.FileView = flView
            Form1_SizeChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub Form1_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        End Sub

        Private Sub fldrView_ContextMenuHint(ByVal sender As System.Object, ByVal e As LogicNP.FolderViewControl.FolderViewContextMenuHintEventArgs) Handles fldrView.ContextMenuHint
            formStatusBar.Panels(0).Text = e.Hint
        End Sub

        Private Sub flView_ContextMenuHint(ByVal sender As System.Object, ByVal e As LogicNP.FileViewControl.ContextMenuHintEventArgs) Handles flView.ContextMenuHint
            formStatusBar.Panels(0).Text = e.Hint
        End Sub

        Private Sub ExecuteShellCommand(ByVal cmd As ShellCommands)
            If fldrView.Focused Then
                fldrView.SelectedNode.ExecuteShellCommand(CType(cmd, LogicNP.FolderViewControl.ShellCommands))
            Else
                If (cmd = ShellCommands.Paste) Then
                    flView.ExecuteCmdForFolder(cmd)
                Else
                    flView.ExecuteCmdForAllSelected(cmd)
                End If
            End If
        End Sub

        Private Sub formToolBar_ButtonClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs) Handles formToolBar.ButtonClick
            If e.Button Is GoUp Then
                flView.GoUp()
            Else
                If e.Button Is Cut Then
                    ExecuteShellCommand(ShellCommands.Cut)
                Else
                    If e.Button Is Copy Then
                        ExecuteShellCommand(ShellCommands.Copy)
                    Else
                        If e.Button Is Paste Then
                            ExecuteShellCommand(ShellCommands.Paste)
                        Else
                            If e.Button Is Delete Then
                                ExecuteShellCommand(ShellCommands.Delete)
                            Else
                                If e.Button Is ViewStyles Then
                                    If (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon) Then
                                        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails
                                    ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails) Then
                                        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List
                                    ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List) Then
                                        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
                                    ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report) Then
                                        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub menuLargeIcons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuLargeIcons.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon
        End Sub

        Private Sub menuThumbnails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuThumbnails.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails
        End Sub

        Private Sub menuList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuList.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List
        End Sub

        Private Sub menuReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReport.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
        End Sub

        Private Sub menuReportStyle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportStyle.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
        End Sub

        Private Sub menuDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuDelete.Click
            ExecuteShellCommand(ShellCommands.Delete)
        End Sub

        Private Sub menuRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuRename.Click
            ExecuteShellCommand(ShellCommands.Rename)
        End Sub

        Private Sub menuProperties_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuProperties.Click
            ExecuteShellCommand(ShellCommands.Properties)
        End Sub

        Private Sub menuClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuClose.Click
            Application.Exit()
        End Sub

        Private Sub menuCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuCut.Click
            ExecuteShellCommand(ShellCommands.Cut)
        End Sub

        Private Sub menuCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuCopy.Click
            ExecuteShellCommand(ShellCommands.Copy)
        End Sub

        Private Sub menuPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPaste.Click
            ExecuteShellCommand(ShellCommands.Paste)
        End Sub

        Private Sub menuLargeIcon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuLargeIcon.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon
        End Sub

        Private Sub menuRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuRefresh.Click
            fldrView.RefreshView()
        End Sub

        Private Sub menuThumbnail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuThumbnail.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails
        End Sub

        Private Sub menuListStyle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuListStyle.Click
            flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List
        End Sub

        Private Sub menuAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAbout.Click
            MessageBox.Show("Windows Explorer Sample Application" & Microsoft.VisualBasic.Chr(10) & "@LogicNP Software", "About Windows Explorer Sample")
        End Sub

        Private Sub fldrView_AfterSelect(ByVal sender As System.Object, ByVal e As LogicNP.FolderViewControl.FolderViewEventArgs) Handles fldrView.AfterSelect
            Me.Text = e.Node.DisplayName
        End Sub

        Private Sub Form1_SizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged

            formToolBar.Bounds = New Rectangle(300, 0, Me.ClientSize.Width - 300, 30)
            shCmbBox.Bounds = New Rectangle(0, 3, 300, 25)
            Dim availHeight As Integer = Me.ClientSize.Height - formToolBar.Height - formStatusBar.Height
            fldrView.Bounds = New Rectangle(0, formToolBar.Height, 300, availHeight)
            flView.Bounds = New Rectangle(301, formToolBar.Height, Me.ClientSize.Width - 300, availHeight)

        End Sub

        Private Sub menuOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuOptions.Click
            MessageBox.Show("Options menu item clicked", "Windows Explorer Sample")
        End Sub
    End Class
End Namespace
