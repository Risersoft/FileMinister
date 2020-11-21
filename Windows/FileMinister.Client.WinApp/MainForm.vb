Imports FileMinister.Client.WinApp.Auth
Imports risersoft.shared.portable.Model
Imports OSIcon
Imports Microsoft.VisualBasic.FileIO
Imports FileMinister.Client.Common
Imports System.IO
Imports FileMinister.Client.Common.Model
Imports System.Net.Http
Imports System.Timers
Imports risersoft.shared.messaging
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud
Imports risersoft
Imports risersoft.app
Imports risersoft.shared.portable.Proxies

Public Class MainForm

#Region "Local Variable Declaration"

    Dim messageClient As MessagingClient
    Dim nameAgent As String
    Dim mouseClickedNode As TreeNode
    Dim driveImageIndex As Integer = 2
    Const KEY_CTRL As Integer = 9
    Dim selectedItem As ListViewItem = Nothing
    Private cutCopyList As New List(Of FileEntryInfo)()
    Private cutOrCopy1 = CutOrCopy.None
    Private m_IconManager As IconManager
    Private showLgn As Boolean = False
    Public Property isHardDelete = False
    Dim shares As List(Of ConfigInfo)
    Dim m_FileVersionConflictInfo As FileVersionConflictInfo
    Private Delegate Sub TakeActionOnMessageHandler(message As MessageInfo)
    Private Delegate Sub OnConnectionStateChangedHandler(isOnline As Boolean)
    Dim m_takeActionOnMessage As TakeActionOnMessageHandler = AddressOf TakeActionOnMessage
    Dim m_OnConnectionStateChanged As OnConnectionStateChangedHandler = AddressOf UpdateConnectionState

    Dim connectionStateTimer As Timers.Timer

    Enum CutOrCopy
        None = 0
        Cut = 1
        Copy = 2
    End Enum

#End Region

#Region "Constructor, Load, On Account Selected, On Shares Configured, On User Validation"

    Public Sub New()

        connectionStateTimer = New Timers.Timer(20 * 1000)
        AddHandler connectionStateTimer.Elapsed, AddressOf ConnectionStateTimerElapsed

        connectionStateTimer.Start()

        InitializeComponent()

        'tsmiUser.Visible = False

        tsmiUser.Text = "Log In"


        tsmiAdmin.Visible = False

        tsProgressBar.Visible = False

        AuthProvider.Parent = Me

        AddHandler OnBeginValidateResponse, AddressOf BeginValidateResponse
        AddHandler OnError, AddressOf ErrorReceived

        AddHandler AuthData.Police.UserAuthenticated, AddressOf UserAuthenticated
        AddHandler AuthProvider.AuthenticationComplete, AddressOf OnAuthenticationComplete
        AddHandler AuthData.Police.AccountSelected, AddressOf OnAccountSelected
        AddHandler AuthProvider.SharedConfigured, AddressOf OnSharesConfigured
        AddHandler AuthProvider.UserValidationCompleted, AddressOf OnUserValidationComplete


        StartPosition = FormStartPosition.WindowsDefaultBounds
        ConfigControls()
        IconManager = New IconManager(True, True, True, True, True)
        tvDriveExplorer.ImageList = IconManager.ImageList(IconSize.Small)
        lvItems.SmallImageList = IconManager.ImageList(IconSize.Small)

        ShowNavMessage("Loading...")

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        DisableAllMenu()

        CheckConnectivity()

        AuthProvider.ValidateUser(Me)

        Dim logicalDriveCount = System.IO.Directory.GetLogicalDrives().Count()
        If logicalDriveCount > 1 Then
            driveImageIndex = 3
        End If

        IconManager.AddFolder()

        IconManager.AddComputerDrives()

        Task.Run(Sub()
                     RunTrayApp()
                 End Sub)

    End Sub

    Sub RunTrayApp()
        Dim processes = Process.GetProcessesByName("FileMinister.Client.TrayApp")
        If (processes.Length = 0) Then

#If DEBUG Then
            Dim appRoot = Path.GetFullPath("..\..\..\..\FileMinister\FileMinister.Client.TrayApp\bin\debug")
#End If
#If Not DEBUG Then

            Dim appRoot = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
#End If

            Dim trayAppPath = Path.Combine(appRoot, "FileMinister.Client.TrayApp.exe")
            Process.Start(trayAppPath)
        End If
    End Sub

    Private Sub OnSharesConfigured(sender As Object, user As LocalWorkSpaceInfo)
        BroadcastMessage(MessageType.Action, ActionType.ConfigureShare, ActionStatus.Completed, "User has configured the shares")
    End Sub

    Private Sub OnUserValidationComplete(sender As Object, shareSummary As ShareSummaryInfo)
        LoadData(shareSummary)
    End Sub

    ''' <summary>
    ''' IconManager Instance
    ''' </summary>
    Public Property IconManager() As IconManager
        Get
            Return m_IconManager
        End Get
        Private Set(value As IconManager)
            m_IconManager = value
        End Set
    End Property

#Region "Validate Menu Items"

    Private Function CanCreateNewFolder() As Boolean
        Return True
    End Function

    Private Function CanCut() As Boolean
        Return True
    End Function

    Private Function CanCopy() As Boolean
        Return True
    End Function

    Private Function CanPaste() As Boolean
        Return cutCopyList.Count > 0
    End Function

    Private Function CanDelete() As Boolean
        Return lvItems.SelectedItems.Count > 0
    End Function

#End Region

    Private Sub UserAuthenticated(sender As Object, e As RSUser)
        CleanData()
    End Sub

    Private Sub OnAccountSelected(sender As Object, user As UserAccountProxy)
        CleanData()
    End Sub

    Sub CleanData()
        txtbreadcrum.Text = String.Empty
        txt_search.Text = String.Empty
        tvDriveExplorer.Nodes.Clear()
        lvItems.Items.Clear()
        tsmiAdmin.Visible = (AuthData.User.RoleId = Role.AccountAdmin)

        For Each item As ToolStripItem In tsmiUser.DropDownItems
            item.Visible = True
        Next
    End Sub

    Private Sub OnAuthenticationComplete(sender As Object, user As LocalWorkSpaceInfo)

        BroadcastMessage(MessageType.Action, ActionType.LogIn, ActionStatus.Completed, "user has logged in")

        tsmiAdmin.Visible = (AuthData.User.RoleId = Role.AccountAdmin)

        CreateUserOptionLabel()
        RegisterMessaging()
        'LoadData(shareSummary)
    End Sub

#End Region

#Region "Context Menu Strip, List View, TreeView's  Click And Other Events"

    Private Sub ChangeContextMenuRightScreen(clickType As Integer)

        cmsMain.Items.Clear()
        Dim lvitemselected = False

        Dim file As FileEntryInfo = Nothing
        If lvItems.SelectedItems.Count > 0 Then
            lvitemselected = True
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        Else
            Dim selectedItem = tvDriveExplorer.SelectedNode
            If (selectedItem IsNot Nothing AndAlso selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        End If

        If (file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty) Then

            Dim isFile As Boolean = file.FileEntryTypeId = FileType.File
            Dim isFolder As Boolean = file.FileEntryTypeId = FileType.Folder

            Dim permissionProvider As PermissionProvider = New PermissionProvider(file)


            tsmiHome.Enabled = True
            tsmiHistory.Enabled = True
            tsmiClipboard.Enabled = True

            tsmiCut.Enabled = False
            tsmiDelete.Enabled = False
            tsmiCopy.Enabled = False


            If isFile Then
                tsmiNewFolder.Enabled = False
                tsmiPaste.Enabled = False
            Else
                tsmiProperties.Enabled = False
                tsmiLink.Enabled = False
                HistoryToolStripMenuItem.Enabled = False
                tsmiCheckIn.Enabled = False
                tsmiCheckout.Enabled = False

                If (isFolder) Then
                    HistoryToolStripMenuItem.Enabled = True
                End If

            End If


            If (isFile) Then

                tsmiNewFolder.Enabled = False
                tsmiPaste.Enabled = False

                If (permissionProvider.CanUndoCheckOut() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiUndoCheckOut As New ToolStripMenuItem("Undo Check Out")
                        tsmiUndoCheckOut.Tag = CommandType.UndoCheckOut
                        cmsMain.Items.Add(tsmiUndoCheckOut)
                    End If
                    tsmiUndoCheckout.Enabled = True
                Else
                    tsmiUndoCheckout.Enabled = False
                End If

                If (permissionProvider.CanResolveConflict() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiResolveConflictM As New ToolStripMenuItem("Resolve Conflict Using Mine")
                        tsmiResolveConflictM.Tag = CommandType.ResolveConflictMine
                        cmsMain.Items.Add(tsmiResolveConflictM)

                        Dim tsmiResolveConflictT As New ToolStripMenuItem("Resolve Conflict Using Theirs")
                        tsmiResolveConflictT.Tag = CommandType.ResolveConflictTheirs
                        cmsMain.Items.Add(tsmiResolveConflictT)

                    End If
                End If

                If (permissionProvider.CanRequestConflictHelp()) Then
                    If (clickType = 1) Then
                        m_FileVersionConflictInfo = permissionProvider.GetConflictData(file.FileEntryId)
                        Dim tsmiRequestUserfile As New ToolStripMenuItem("Upload Conflicted File")
                        tsmiRequestUserfile.Tag = CommandType.RequestUserfile
                        cmsMain.Items.Add(tsmiRequestUserfile)
                    End If
                End If

                If (permissionProvider.CanViewHistory() = True) Then
                    If (clickType = 1) Then
                        Dim tsmHistory As New ToolStripMenuItem("History")
                        tsmHistory.Tag = CommandType.History
                        cmsMain.Items.Add(tsmHistory)
                    End If
                    HistoryToolStripMenuItem.Enabled = True
                Else
                    HistoryToolStripMenuItem.Enabled = False
                End If

                If (permissionProvider.CanLinkFile() = True) Then
                    If (clickType = 1) Then
                        Dim tsmLink As New ToolStripMenuItem("Link")
                        tsmLink.Tag = CommandType.Link
                        cmsMain.Items.Add(tsmLink)
                    End If
                    tsmiLink.Enabled = True
                Else
                    tsmiLink.Enabled = False
                End If

            Else

                If (isFolder) Then
                    If (permissionProvider.CanViewHistory() = True) Then
                        If (clickType = 1) Then
                            Dim tsmHistory As New ToolStripMenuItem("History")
                            tsmHistory.Tag = CommandType.History
                            cmsMain.Items.Add(tsmHistory)
                        End If
                        HistoryToolStripMenuItem.Enabled = True
                    Else
                        HistoryToolStripMenuItem.Enabled = False
                    End If
                End If

                If (clickType = 1) Then

                    Dim tsmiRefresh As New ToolStripMenuItem("Refresh")
                    tsmiRefresh.Tag = CommandType.Refresh
                    cmsMain.Items.Add(tsmiRefresh)
                End If
                tsmiRefresh.Enabled = True

                If (clickType = 1) Then
                    Dim tsmNewFolder As New ToolStripMenuItem("New Folder")
                    tsmNewFolder.Tag = CommandType.NewFolder
                    cmsMain.Items.Add(tsmNewFolder)
                End If
                tsmiNewFolder.Enabled = True

                If (permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                    If (clickType = 1) Then
                        Dim tsmiPaste As New ToolStripMenuItem("Paste")
                        tsmiPaste.Tag = CommandType.Paste
                        cmsMain.Items.Add(tsmiPaste)
                    End If
                    tsmiPaste.Enabled = True
                Else
                    tsmiPaste.Enabled = False
                End If
            End If

            If (lvitemselected) Then

                If (permissionProvider.CanCopy() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiCut As New ToolStripMenuItem("Cut")
                        tsmiCut.Tag = CommandType.Cut
                        cmsMain.Items.Add(tsmiCut)
                    End If
                    tsmiCut.Enabled = True
                Else
                    tsmiCut.Enabled = False
                End If

                If (permissionProvider.CanCopy() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiCopy As New ToolStripMenuItem("Copy")
                        tsmiCopy.Tag = CommandType.Copy
                        cmsMain.Items.Add(tsmiCopy)
                    End If
                    tsmiCopy.Enabled = True
                Else
                    tsmiCopy.Enabled = False
                End If

                If (permissionProvider.CanSoftDelete() = True OrElse permissionProvider.CanHardDelete() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiSoftDelete As New ToolStripMenuItem("Delete")
                        tsmiSoftDelete.Tag = CommandType.Delete
                        cmsMain.Items.Add(tsmiSoftDelete)
                    End If
                    tsmiDelete.Enabled = True
                Else
                    tsmiDelete.Enabled = False
                End If
                isHardDelete = permissionProvider.CanHardDelete()

                If (permissionProvider.CanUndoDelete() = True) Then
                    If (clickType = 1) Then
                        Dim tsmiUndoDelete As New ToolStripMenuItem("Undo Delete")
                        tsmiUndoDelete.Tag = CommandType.UndoDelete
                        cmsMain.Items.Add(tsmiUndoDelete)
                    End If
                End If
            End If

            If (permissionProvider.CanViewProperties() = True) Then
                If (clickType = 1) Then
                    Dim sep1 As New ToolStripSeparator()
                    cmsMain.Items.Add(sep1)

                    Dim tsmProperties As New ToolStripMenuItem("Properties")
                    tsmProperties.Tag = CommandType.Properties
                    cmsMain.Items.Add(tsmProperties)
                End If
                tsmiProperties.Enabled = True
            Else
                tsmiProperties.Enabled = False
            End If

            If (Not file.IsDeleted AndAlso clickType = 1) Then
                Dim sep1 As New ToolStripSeparator()
                cmsMain.Items.Add(sep1)

                Dim tsmiOpenFolderInExplorer As New ToolStripMenuItem("Open Folder In Explorer")
                tsmiOpenFolderInExplorer.Tag = CommandType.OpenFolderInExplorer
                cmsMain.Items.Add(tsmiOpenFolderInExplorer)
            End If
        End If
    End Sub

    Private Sub ChangeContextMenuRightScreenOld(clickType As Integer)

        cmsMain.Items.Clear()
        Dim isPasteAdded = False
        Dim lvitemselected = False

        Dim file As FileEntryInfo = Nothing
        If lvItems.SelectedItems.Count > 0 Then
            lvitemselected = True
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        Else
            Dim selectedItem = tvDriveExplorer.SelectedNode
            If (selectedItem IsNot Nothing AndAlso selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        End If
        If file IsNot Nothing Then
            If file.FileEntryId <> Guid.Empty Then
                Dim isFile As Boolean = file.FileEntryTypeId = FileType.File
                If isFile Then
                    tsmiNewFolder.Enabled = False
                    tsmiPaste.Enabled = False
                Else
                    tsmiProperties.Enabled = False
                    tsmiLink.Enabled = False
                    tsmiHistory.Enabled = False
                    tsmiCheckIn.Enabled = False
                    tsmiCheckout.Enabled = False
                End If

                Dim permissionProvider As PermissionProvider = New PermissionProvider(file)



                'Dim tsmiGetLatest As New ToolStripMenuItem("Get Latest")
                'tsmiGetLatest.Tag = CommandType.GetLatest
                'cmsMain.Items.Add(tsmiGetLatest)
                If (lvitemselected = False) Then
                    If (clickType = 1) Then

                        Dim tsmiRefresh As New ToolStripMenuItem("Refresh")
                        tsmiRefresh.Tag = CommandType.Refresh
                        cmsMain.Items.Add(tsmiRefresh)

                        Dim tsmNewFolder As New ToolStripMenuItem("New Folder")
                        tsmNewFolder.Tag = CommandType.NewFolder
                        cmsMain.Items.Add(tsmNewFolder)
                    End If
                End If
                tsmiNewFolder.Enabled = True


                If isFile Then
                    'If (permissionProvider.CanCheckIn() = True) Then
                    '    If (clickType = 1) Then
                    '        Dim tsmiCheckIn As New ToolStripMenuItem("Check In")
                    '        tsmiCheckIn.Tag = CommandType.CheckIn
                    '        cmsMain.Items.Add(tsmiCheckIn)
                    '    End If
                    '    tsmiCheckIn.Enabled = True
                    'Else
                    '    tsmiCheckIn.Enabled = False
                    'End If

                    'If (permissionProvider.CanCheckOut() = True) Then
                    '    If (clickType = 1) Then
                    '        Dim tsmiCheckOut As New ToolStripMenuItem("Check Out")
                    '        tsmiCheckOut.Tag = CommandType.CheckOut
                    '        cmsMain.Items.Add(tsmiCheckOut)
                    '    End If
                    '    tsmiCheckout.Enabled = True
                    'Else
                    '    tsmiCheckout.Enabled = False
                    'End If

                    If (permissionProvider.CanUndoCheckOut() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiUndoCheckOut As New ToolStripMenuItem("Undo Check Out")
                            tsmiUndoCheckOut.Tag = CommandType.UndoCheckOut
                            cmsMain.Items.Add(tsmiUndoCheckOut)
                        End If
                        tsmiUndoCheckout.Enabled = True
                    Else
                        tsmiUndoCheckout.Enabled = False
                    End If

                    If (permissionProvider.CanResolveConflict() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiResolveConflictM As New ToolStripMenuItem("Resolve Conflict Using Mine")
                            tsmiResolveConflictM.Tag = CommandType.ResolveConflictMine
                            cmsMain.Items.Add(tsmiResolveConflictM)

                            Dim tsmiResolveConflictT As New ToolStripMenuItem("Resolve Conflict Using Theirs")
                            tsmiResolveConflictT.Tag = CommandType.ResolveConflictTheirs
                            cmsMain.Items.Add(tsmiResolveConflictT)

                        End If
                    End If

                    If (permissionProvider.CanRequestConflictHelp()) Then
                        If (clickType = 1) Then
                            m_FileVersionConflictInfo = permissionProvider.GetConflictData(file.FileEntryId)
                            Dim tsmiRequestUserfile As New ToolStripMenuItem("Upload Conflicted File")
                            tsmiRequestUserfile.Tag = CommandType.RequestUserfile
                            cmsMain.Items.Add(tsmiRequestUserfile)
                        End If
                    End If

                    If (permissionProvider.CanViewHistory() = True) Then
                        If (clickType = 1) Then
                            Dim tsmHistory As New ToolStripMenuItem("History")
                            tsmHistory.Tag = CommandType.History
                            cmsMain.Items.Add(tsmHistory)
                        End If
                        tsmiHistory.Enabled = True
                    Else
                        tsmiHistory.Enabled = False
                    End If

                    If (permissionProvider.CanLinkFile() = True) Then
                        If (clickType = 1) Then
                            Dim tsmLink As New ToolStripMenuItem("Link")
                            tsmLink.Tag = CommandType.Link
                            cmsMain.Items.Add(tsmLink)
                        End If
                        tsmiLink.Enabled = True
                    Else
                        tsmiLink.Enabled = False
                    End If

                Else
                    If (permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                        If (clickType = 1) Then
                            Dim tsmiPaste As New ToolStripMenuItem("Paste")
                            tsmiPaste.Tag = CommandType.Paste
                            cmsMain.Items.Add(tsmiPaste)
                            isPasteAdded = True
                        End If
                        tsmiPaste.Enabled = True
                    Else
                        tsmiPaste.Enabled = False
                    End If

                End If

                If (lvItems.SelectedItems.Count > 0) Then



                    If (permissionProvider.CanCopy() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiCut As New ToolStripMenuItem("Cut")
                            tsmiCut.Tag = CommandType.Cut
                            cmsMain.Items.Add(tsmiCut)
                        End If
                        tsmiCut.Enabled = True
                    Else
                        tsmiCut.Enabled = False
                    End If

                    If (permissionProvider.CanCopy() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiCopy As New ToolStripMenuItem("Copy")
                            tsmiCopy.Tag = CommandType.Copy
                            cmsMain.Items.Add(tsmiCopy)
                        End If
                        tsmiCopy.Enabled = True
                    Else
                        tsmiCopy.Enabled = False
                    End If

                    If (Not isPasteAdded AndAlso permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                        If (clickType = 1) Then
                            Dim tsmiPaste As New ToolStripMenuItem("Paste")
                            tsmiPaste.Tag = CommandType.Paste
                            cmsMain.Items.Add(tsmiPaste)
                        End If
                        tsmiPaste.Enabled = True
                    Else
                        tsmiPaste.Enabled = False
                    End If

                    If (permissionProvider.CanSoftDelete() = True OrElse permissionProvider.CanHardDelete() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiSoftDelete As New ToolStripMenuItem("Delete")
                            tsmiSoftDelete.Tag = CommandType.Delete
                            cmsMain.Items.Add(tsmiSoftDelete)
                        End If
                        tsmiDelete.Enabled = True
                    Else
                        tsmiDelete.Enabled = False
                    End If
                    isHardDelete = permissionProvider.CanHardDelete()

                    If (permissionProvider.CanUndoDelete() = True) Then
                        If (clickType = 1) Then
                            Dim tsmiUndoDelete As New ToolStripMenuItem("Undo Delete")
                            tsmiUndoDelete.Tag = CommandType.UndoDelete
                            cmsMain.Items.Add(tsmiUndoDelete)
                        End If
                    End If


                End If



                If (Not file.IsDeleted AndAlso clickType = 1) Then

                    Dim sep1 As New ToolStripSeparator()
                    cmsMain.Items.Add(sep1)

                    Dim tsmiOpenFolderInExplorer As New ToolStripMenuItem("Open Folder In Explorer")
                    tsmiOpenFolderInExplorer.Tag = CommandType.OpenFolderInExplorer
                    cmsMain.Items.Add(tsmiOpenFolderInExplorer)
                End If

                If (permissionProvider.CanViewProperties() = True) Then
                    If (clickType = 1) Then
                        Dim sep1 As New ToolStripSeparator()
                        cmsMain.Items.Add(sep1)


                        Dim tsmProperties As New ToolStripMenuItem("Properties")
                        tsmProperties.Tag = CommandType.Properties
                        cmsMain.Items.Add(tsmProperties)
                    End If
                    tsmiProperties.Enabled = True
                Else
                    tsmiProperties.Enabled = False
                End If
            Else
                DisableButtons()
            End If
        Else
            DisableButtons()
        End If
    End Sub

    Private Async Sub CmsMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles cmsMain.ItemClicked
        cmsMain.Visible = False
        Dim tv As TreeView = Nothing
        Dim lv As ListView = Nothing
        Dim cms = TryCast(sender, ContextMenuStrip)
        If cms IsNot Nothing AndAlso cms.SourceControl IsNot Nothing Then
            tv = TryCast(cms.SourceControl, TreeView)
            lv = TryCast(cms.SourceControl, ListView)
        End If
        Dim commandType__1 = If(e.ClickedItem.Tag IsNot Nothing, DirectCast(e.ClickedItem.Tag, CommandType), CommandType.None)
        Select Case commandType__1
            Case CommandType.Refresh
                LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
                Exit Select
            Case CommandType.GetLatest
                Exit Select
            Case CommandType.Link
                ShowLink()
                Exit Select
            Case CommandType.History
                ShowHistory()
                Exit Select
            Case CommandType.Properties
                ShowProperties()
                Exit Select
            Case CommandType.CheckIn
                SendFileActionMessage(ActionType.CheckIn)
                Exit Select
            Case CommandType.CheckOut
                SendFileActionMessage(ActionType.CheckOut)
                Exit Select
            Case CommandType.ResolveConflictMine
                ResolveUsingMine()
                Exit Select
            Case CommandType.ResolveConflictTheirs
                ResolveUsingTheirs()
                Exit Select
            Case CommandType.UndoCheckOut
                SendFileActionMessage(ActionType.UndoCheckout)
                Exit Select
            Case CommandType.Delete
                Delete()
                Exit Select
            Case CommandType.UndoDelete
                SendFileActionMessage(ActionType.UndoDelete)
                Exit Select
            Case CommandType.NewFolder
                CreateNewFolder(tv, lv)
                Exit Select
            Case CommandType.Paste
                PasteItem()
                Exit Select
            Case CommandType.Copy
                CutCopyItem(CutOrCopy.Copy)
                Exit Select
            Case CommandType.Cut
                CutCopyItem(CutOrCopy.Cut)
                Exit Select
            Case CommandType.OpenFolderInExplorer
                OpenFolderInExplorer(tv, lv)
                Exit Select
            Case CommandType.RequestUserfile
                Await RequestUserFileAsync()
                Exit Select
        End Select


        ''LoadFolderData(tvDriveExplorer.SelectedNode)
        ''RefreshFolder(Nothing)

    End Sub

    'Private Sub tvDriveExplorer_NodeMouseClick1(sender As Object, e As TreeViewEventArgs) Handles tvDriveExplorer.AfterSelect

    'End Sub

    Private Sub tvDriveExplorer_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvDriveExplorer.NodeMouseClick

        'If e.Button = Windows.Forms.MouseButtons.Right Then
        tvDriveExplorer.SelectedNode = e.Node
        ' End If

        lvItems.SelectedItems.Clear()

        tsmiHome.Enabled = True
        tsmiHistory.Enabled = True
        tsmiClipboard.Enabled = True

        tsmiProperties.Enabled = False
        HistoryToolStripMenuItem.Enabled = False
        tsmiLink.Enabled = False

        tsmiCut.Enabled = False
        tsmiCopy.Enabled = False
        tsmiPaste.Enabled = False
        tsmiDelete.Enabled = False

        tsmiCheckIn.Enabled = False
        tsmiCheckout.Enabled = False
        tsmiUndoCheckout.Enabled = False

        tsmiNewFolder.Enabled = False
        tsmiRefresh.Enabled = True

        Dim file = CType(tvDriveExplorer.SelectedNode.Tag, FileEntryInfo)
        Dim prmsnprovider As PermissionProvider = New PermissionProvider(file)

        If (prmsnprovider.CanCreate() = True) Then
            If (file.FileVersion.VersionNumber IsNot Nothing) Then
                tsmiNewFolder.Enabled = True
            End If
        End If

        If (prmsnprovider.CanViewProperties() = True) Then
            tsmiProperties.Enabled = True
        Else
            tsmiProperties.Enabled = False
        End If

        If (prmsnprovider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
            tsmiPaste.Enabled = True
        End If

    End Sub

    Private Sub tvDriveExplorer_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvDriveExplorer.AfterSelect
        lvItems.Items.Clear()
        Dim node = e.Node
        If node IsNot Nothing Then
            LoadFolderDataAsync(node)
        End If
    End Sub

    Private Sub ListView1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lvItems.MouseDown
        Dim info As ListViewHitTestInfo = lvItems.HitTest(e.X, e.Y)
        If Not IsNothing(info.SubItem) Then
            If (info.SubItem.Name = "OC") Then
                If (info.SubItem.Text.ToUpper() = "TRUE") Then
                    Dim file = TryCast(lvItems.Items(info.Item.Index).Tag, FileEntryInfo)
                    Dim winUnResConflicts = New UnResolvedConflicts(file)
                    winUnResConflicts.ShowDialog()
                    LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
                End If
            End If
        End If
    End Sub

    Private Sub lvItems_MouseUp(sender As Object, e As MouseEventArgs) Handles lvItems.MouseUp
        mouseClickedNode = Nothing
        If e.Button = MouseButtons.Right Then
            ChangeContextMenuRightScreen(1)
        Else
            ChangeContextMenuRightScreen(0)
        End If
    End Sub

    Private Sub lvItems_ItemActivate(sender As Object, e As EventArgs) Handles lvItems.ItemActivate
        For Each item As ListViewItem In lvItems.SelectedItems
            Dim file = TryCast(item.Tag, FileEntryInfo)
            If file IsNot Nothing AndAlso file.IsPhysicalFile = True Then
                If file.FileEntryTypeId = FileType.File Then
                    System.Diagnostics.Process.Start(file.FileVersion.FileEntryRelativePath)
                ElseIf file.FileEntryTypeId = FileType.Folder Then
                    Dim isFound = False
                    Dim selectedNode = tvDriveExplorer.SelectedNode
                    For Each node As TreeNode In selectedNode.Nodes
                        Dim f = TryCast(node.Tag, FileEntryInfo)
                        If f.FileVersion.FileEntryRelativePath = file.FileVersion.FileEntryRelativePath Then
                            node.EnsureVisible()
                            node.ExpandAll()
                            tvDriveExplorer.SelectedNode = node
                            isFound = True
                            Exit For
                        End If
                    Next

                    'handle search case when next level node click
                    If isFound <> True Then
                        LoadFolderDataAsync(selectedNode, file)
                    End If

                End If
            End If
        Next

    End Sub

#Region "Drap & Drop Event handlers"

    Private Sub tvDriveExplorer_DragDrop(sender As Object, e As DragEventArgs) Handles tvDriveExplorer.DragDrop
        e.Effect = If(e.KeyState = KEY_CTRL, DragDropEffects.Copy, DragDropEffects.Move)
        Dim p As Point = tvDriveExplorer.PointToClient(New Point(e.X, e.Y))
        Dim node As TreeNode = tvDriveExplorer.GetNodeAt(p)
        node.BackColor = Color.White
    End Sub

    Private Sub tvDriveExplorer_DragEnter(sender As Object, e As DragEventArgs) Handles tvDriveExplorer.DragEnter
        e.Effect = If(e.KeyState = KEY_CTRL, DragDropEffects.Copy, DragDropEffects.Move)
    End Sub

    Private Sub tvDriveExplorer_DragOver(sender As Object, e As DragEventArgs) Handles tvDriveExplorer.DragOver

        e.Effect = If(e.KeyState = KEY_CTRL, DragDropEffects.Copy, DragDropEffects.Move)

        Dim p As Point = tvDriveExplorer.PointToClient(New Point(e.X, e.Y))
        Dim node As TreeNode = tvDriveExplorer.GetNodeAt(p)
        If node.PrevVisibleNode IsNot Nothing Then
            node.PrevVisibleNode.BackColor = Color.White
        End If
        If node.NextVisibleNode IsNot Nothing Then
            node.NextVisibleNode.BackColor = Color.White
        End If
        node.BackColor = Color.LightBlue
    End Sub

    Private Sub tvDriveExplorer_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles tvDriveExplorer.DragLeave
        tvDriveExplorer.DoDragDrop(e.Item, DragDropEffects.All)
    End Sub

    Private Sub lvItems_DragDrop(sender As Object, e As DragEventArgs) Handles lvItems.DragDrop
        e.Effect = If(e.KeyState = KEY_CTRL, DragDropEffects.Copy, DragDropEffects.Move)
        If selectedItem IsNot Nothing Then
            selectedItem.SubItems(0).BackColor = Color.White
            selectedItem = Nothing
        End If
        Dim p As Point = lvItems.PointToClient(New Point(e.X, e.Y))
        Dim item = lvItems.GetItemAt(p.X, p.Y)
        item.UseItemStyleForSubItems = False
        item.SubItems(0).BackColor = Color.White
    End Sub

    Private Sub lvItems_DragOver(sender As Object, e As DragEventArgs) Handles lvItems.DragOver
        e.Effect = If(e.KeyState = KEY_CTRL, DragDropEffects.Copy, DragDropEffects.Move)

        If selectedItem IsNot Nothing Then
            selectedItem.BackColor = Color.White
        End If

        Dim p As Point = lvItems.PointToClient(New Point(e.X, e.Y))
        Dim item = lvItems.GetItemAt(p.X, p.Y)
        selectedItem = item
        If item IsNot Nothing Then
            item.UseItemStyleForSubItems = False
            item.SubItems(0).BackColor = Color.LightSeaGreen
        End If
    End Sub

    Private Sub lvItems_ItemDrag(sender As Object, e As ItemDragEventArgs)
        lvItems.DoDragDrop(lvItems.SelectedItems, DragDropEffects.Move Or DragDropEffects.Copy)
    End Sub

#End Region

#End Region

#Region "Ribbon Button Click Events"

    Private Sub tsmiReport_Click(sender As Object, e As EventArgs) Handles tsmiReports.Click
        If (AuthData.User Is Nothing) Then
            Exit Sub
        End If
        Dim report = New ReportForm()
        report.ShowDialog()
    End Sub

    Private Sub tsmiConfig_Click(sender As Object, e As EventArgs) Handles tsmiConfig.Click, btnConfigureShare.Click
        If (AuthData.User Is Nothing) Then
            Exit Sub
        End If

        Me.WindowState = FormWindowState.Maximized

        Dim frm = Me.OwnedForms.Where(Function(p) p.GetType() Is GetType(Config)).FirstOrDefault()
        If frm IsNot Nothing Then
            frm.Focus()
            Me.Focus()
            Return
        End If

        Dim frmconfig = New Config()
        'If (Config.TheForm.Visible) Then
        '    Return
        'End If


        frmconfig.Focus()
        frmconfig.ShowDialog(Me)
        If frmconfig.IsConfigured Then

            Using syncClient As New LocalSyncClient()
                syncClient.ResetSyncTimestampAsync()
            End Using

            Using shareClient As New LocalShareClient()
                Dim result = shareClient.ShareMappedSummary
                If result.Status = 200 Then
                    Dim shareSummary = result.Data

                    LoadData(shareSummary)
                    Dim message As New MessageInfo()
                    With message
                        .MessageType = MessageType.Action
                        .ActionType = ActionType.ConfigureShare
                        .ActionStatus = ActionStatus.Completed
                        .Message = "User has changed the share's configurations"
                        .User = AuthData.User
                    End With
                    MessageClientProvider.Instance.BroadcastMessage(message)
                End If
            End Using
        End If

    End Sub

    Private Sub tsmiRefresh_Click(sender As Object, e As EventArgs) Handles tsmiRefresh.Click
        LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
    End Sub

    Private Sub tsmiCheckIn_Click(sender As Object, e As EventArgs) Handles tsmiCheckIn.Click
        SendFileActionMessage(ActionType.CheckIn)
    End Sub

    Private Sub tsmiCheckOut_Click(sender As Object, e As EventArgs) Handles tsmiCheckout.Click
        SendFileActionMessage(ActionType.CheckOut)
    End Sub

    Private Sub tsmiUndoCheckOut_Click(sender As Object, e As EventArgs) Handles tsmiUndoCheckout.Click
        SendFileActionMessage(ActionType.UndoCheckout)
    End Sub

    Private Sub tsmiProperties_Click(sender As Object, e As EventArgs) Handles tsmiProperties.Click
        ShowProperties()
    End Sub

    Private Sub tsmiHistory_Click(sender As Object, e As EventArgs) Handles HistoryToolStripMenuItem.Click
        ShowHistory()
    End Sub

    Private Sub tsmiLink_Click(sender As Object, e As EventArgs) Handles tsmiLink.Click
        ShowLink()
    End Sub

    Private Sub tsmiNewFolder_Click(sender As Object, e As EventArgs) Handles tsmiNewFolder.Click
        Dim tv As TreeView = Nothing
        Dim lv As ListView = Nothing
        CreateNewFolder(tv, lv)
        LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
    End Sub

    Private Sub tsmiCut_Click(sender As Object, e As EventArgs) Handles tsmiCut.Click
        CutCopyItem(CutOrCopy.Cut)
    End Sub

    Private Sub tsmiCopy_Click(sender As Object, e As EventArgs) Handles tsmiCopy.Click
        CutCopyItem(CutOrCopy.Copy)
    End Sub

    Private Sub tsmiPaste_Click(sender As Object, e As EventArgs) Handles tsmiPaste.Click
        PasteItem()
    End Sub

    Private Sub tsmiDelete_Click(sender As Object, e As EventArgs) Handles tsmiDelete.Click
        Delete()
    End Sub

#End Region

#Region "Messaging"

    Private Sub RegisterMessaging()
        If MessageClientProvider.Instance Is Nothing Then
            Dim user = AuthData.User
            Dim messageClientName As String = String.Format("U{0}_UA{1}_WU{2}_app", user.UserId, user.AccountId, user.WindowsUserSId.Replace("\", "_"))
            Dim messagingClient = MessageClientProvider.CreateInstance(messageClientName, Initiator.WindowsApplication)

            AddHandler messagingClient.OnShowMessage, AddressOf OnMessageReceived
        End If
    End Sub

    Private Sub OnMessageReceived(sender As Object, e As InboundMessageHandler.ShowMessageEventArgs)
        If AuthData.User IsNot Nothing Then
            Dim strMsg = Newtonsoft.Json.JsonConvert.SerializeObject(e.message)
            Diagnostics.Debug.WriteLine(strMsg, "FileMinister")

            Dim message = e.message
            If (message.ActionStatus = ActionStatus.Completed OrElse message.ActionStatus = ActionStatus.Error OrElse e.message.ActionType = ActionType.OpenConfig) Then
                If (message.User IsNot Nothing AndAlso AuthData.User.UserAccountId = message.User.UserAccountId) Then
                    If Me.InvokeRequired Then
                        Me.Invoke(m_takeActionOnMessage, message)
                    End If
                Else
                    If (e.message.ActionType = ActionType.OpenConfig) Then
                        If Me.InvokeRequired Then
                            Me.Invoke(m_takeActionOnMessage, message)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub TakeActionOnMessage(message As MessageInfo)
        If message.ActionStatus = ActionStatus.Completed Then
            If message.ActionType = ActionType.CheckOut OrElse message.ActionType = ActionType.CheckIn OrElse message.ActionType = ActionType.UndoCheckout OrElse message.ActionType = ActionType.UndoDelete Then
                If message.Data.ContainsKey("FileSystemEntryId") Then
                    Dim fileSystemEntryId As Guid = CType(message.Data("FileSystemEntryId"), Guid)
                    If fileSystemEntryId <> Guid.Empty Then
                        For i As Integer = 0 To lvItems.Items.Count - 1
                            Dim item = lvItems.Items(i)
                            If item.Tag IsNot Nothing Then
                                Dim obj = CType(item.Tag, FileEntryInfo)
                                If obj.FileEntryId = fileSystemEntryId Then
                                    LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                End If
            ElseIf message.ActionType = ActionType.DownloadShare Then
                OnShareDownloadComplete(message)
            End If
        ElseIf message.ActionStatus = ActionStatus.Error Then
            HandleMessageError(message)
        ElseIf message.ActionType = ActionType.OpenConfig Then
            tsmiConfig_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub SendFileActionMessage(actionType As ActionType)
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim message As New MessageInfo()
                    message.Data("FileSystemEntryId") = file.FileEntryId
                    message.Data("FileSystemEntryVersionId") = file.FileVersion.FileVersionId
                    message.Data("CurrentVersionNumber") = file.CurrentVersionNumber
                    With message
                        .MessageType = MessageType.Action
                        .ActionType = actionType
                        .User = AuthData.User
                    End With
                    MessageClientProvider.Instance.BroadcastMessage(message)

                End If
            End If
        End If
    End Sub

    Private Sub HandleMessageError(message As MessageInfo)
        Dim msg As String = message.Message
        Dim refreshRequired As Boolean = False
        Select Case message.Status
            Case Status.NotFound
            Case Status.FileCheckedOut
            Case Status.FileCheckedOutByDifferentUser
            Case Status.PermanentlyDeleted
            Case Status.VersionNotLatest
                refreshRequired = True
                Exit Select
        End Select

        If Not String.IsNullOrWhiteSpace(msg) Then
            MessageBoxHelper.ShowErrorMessage(msg)
        End If
        If refreshRequired Then
            LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
        End If

    End Sub

    Private Sub OnShareDownloadComplete(message As MessageInfo)
        Using shareClient As New LocalShareClient()
            Dim result = shareClient.ShareMappedSummary
            If result.Status = 200 Then
                Dim shareSummary = result.Data
                'Dim shareId = message.Share.ShareId

                'LoadData(shareSummary)
                ReloadShare(message.Share)
            End If
        End Using
    End Sub

    Private Async Sub ReloadShare(share As ConfigInfo)
        Dim node = tvDriveExplorer.Nodes.Cast(Of TreeNode).FirstOrDefault(Function(n) CType(n.Tag, FileEntryInfo).FileShareId = share.FileShareId)
        If node IsNot Nothing Then
            Dim file = CType(node.Tag, FileEntryInfo)
            ValidateIfShareExists(node, share)
            If file.FileEntryId = Guid.Empty OrElse node.Nodes.Count = 0 Then
                Await GetShareFoldersAsync(node, share, file)
            End If
        End If
    End Sub

    Public Async Function GetShareFoldersAsync(node As TreeNode, share As ConfigInfo, file As FileEntryInfo) As Task(Of ResultInfo(Of List(Of FileEntryInfo), Status))
        If file.IsDeleted AndAlso file.FileEntryTypeId = FileType.Share Then
            node.Nodes.Clear()
            lvItems.Items.Clear()
            Return Nothing
        End If
        Using client As New LocalFileClient()
            Dim result = Await client.GetByShareIdAsync(share.FileShareId)
            If ValidateResponse(result) Then
                Dim file2 = result.Data.FirstOrDefault(Function(p) p.FileEntryTypeId = FileType.Share)
                If file2 IsNot Nothing Then

                    If file2.FileVersion Is Nothing Then
                        file2.FileVersion = New FileVersionInfo()
                        With file2.FileVersion
                            .FileEntryId = file.FileEntryId
                            .FileEntryName = share.ShareName
                            .FileEntryNameWithExtension = share.ShareName
                            .FileEntryRelativePath = share.SharePath
                        End With
                    End If
                    node.Tag = file2
                    node.Text = file2.FileVersion.FileEntryNameWithExtension
                    BuildDirectoryStructure(node, file2, result.Data)

                    If tvDriveExplorer.SelectedNode Is node Then
                        LoadFolderDataAsync(node, file2)
                    End If

                End If
            End If
            Return result
        End Using
    End Function

#End Region

#Region "Private Methods"

    Private Sub ConfigControls()
        'ValidateContextMenu();

        Dim chName As New ColumnHeader()
        chName.Text = "Name"
        chName.Width = 250

        Dim chVersion As New ColumnHeader()
        chVersion.Text = "Version"
        chVersion.Width = 60

        Dim chCheckedOutBy As New ColumnHeader()
        chCheckedOutBy.Text = "Checked Out By"
        chCheckedOutBy.Width = 150

        Dim chLastCheckedIn As New ColumnHeader()
        chLastCheckedIn.Text = "Last Checked In"
        chLastCheckedIn.Width = 150

        Dim chStatus As New ColumnHeader()
        chStatus.Text = "Status"
        chStatus.Width = 100


        Dim chOthersConflict As New ColumnHeader()
        chOthersConflict.Text = "Other's Conflict"
        chOthersConflict.Width = 100
        chOthersConflict.Name = "OC"

        lvItems.Columns.AddRange(New ColumnHeader() {chName, chVersion, chCheckedOutBy, chLastCheckedIn, chStatus, chOthersConflict})


    End Sub

    Private Sub CreateUserOptionLabel()
        tsmiUser.Visible = True
        tsmiUser.Text = "Welcome " + AuthData.User.FullName + " - " + AuthData.User.AccountName
    End Sub

    Private Sub LoadData(shareSummary As ShareSummaryInfo)
        DisableAllMenu()
        ShowProgress()
        ShowNavMessage("Loading...")
        If shareSummary Is Nothing OrElse (shareSummary.AccountShareCount = 0 AndAlso shareSummary.MappedShareCount = 0) Then

            lvItems.Items.Clear()

            Dim message As String = Nothing
            If AuthData.User.RoleId = Role.AccountAdmin Then
                message = "No shares found for this account"
                lblAddAgent.Visible = True
                lblAddShare.Visible = True
            Else
                message = "No shares found for this account. Please contact your account admin"
                lblAddAgent.Visible = False
                lblAddShare.Visible = False
            End If
            ShowNavMessage("No shares configured")

            lblMessageRight.Text = message
            ShowHideDetailPane(False)
            HideProgress()
        Else
            If shareSummary.MappedShareCount > 0 Then
                ShowDetailPaneMessage("Loading...")

                Dim async = New AsyncProvider()
                async.AddMethod("Shares", Function() New Common.LocalShareClient().GetAllShareByAccount())
                async.AddMethod("Files", Function() New Common.LocalFileClient().GetFiles())
                async.OnCompletion = Sub(list As IDictionary)
                                         IntialiseFileExplorer(list)
                                     End Sub
                async.Execute()
            Else
                lvItems.Items.Clear()
                tvDriveExplorer.Nodes.Clear()
                lblMessageRight.Text = My.Resources.MessageFolderEmpty
                lblAddAgent.Visible = False
                lblAddShare.Visible = False
                ShowHideDetailPane(False)
                ShowHideNavShareConfiguration(False)
                HideProgress()
            End If
        End If

        If AuthData.User.RoleId = Role.AccountAdmin Then
            tsmiAdmin.Enabled = True
            tsmiShares.Enabled = True
            tsmiAgents.Enabled = True
        End If

    End Sub

    Private Sub ShowNavMessage(msg As String)
        tvDriveExplorer.Visible = False
        pnlNav.Visible = True
        btnConfigureShare.Visible = False
        lblMessageLeft.Text = msg
        lblMessageLeft.Visible = True
    End Sub

    Private Sub ShowDetailPaneMessage(msg As String)
        pnlDetailTop.Visible = True
        lblAddAgent.Visible = False
        lblAddShare.Visible = False
        lblMessageRight.Text = msg
        lblMessageRight.Visible = True
    End Sub

    Private Sub IntialiseFileExplorer(list As IDictionary)

        Dim configResult = CType(list("Shares"), ResultInfo(Of List(Of ConfigInfo), Status))
        Dim fileResult = CType(list("Files"), ResultInfo(Of List(Of FileEntryInfo), Status))
        If ValidateResponse(configResult) AndAlso ValidateResponse(fileResult) Then
            shares = configResult.Data
            ShowHideNavShareConfiguration(shares.Count > 0)
            ShowHideDetailPane(True)

            If (tvDriveExplorer.IsDisposed = False) Then
                tvDriveExplorer.BeginUpdate()
                tvDriveExplorer.Nodes.Clear()
            End If
            MapDrives(configResult.Data, fileResult.Data)
            tvDriveExplorer.EndUpdate()
            DisableButtons()
            HideProgress()
        End If
    End Sub

    Private Sub ShowHideNavShareConfiguration(visible As Boolean)
        tvDriveExplorer.Visible = visible
        pnlNav.Visible = Not visible
        btnConfigureShare.Visible = Not visible
        lblMessageLeft.Visible = False
    End Sub

    Private Sub ShowHideDetailPane(visible As Boolean)
        pnlDetailTop.Visible = Not visible
    End Sub

    Private Sub MapDrives(configuration As List(Of ConfigInfo), files As List(Of FileEntryInfo))

        Dim q = files.Where(Function(p) p.FileEntryTypeId = FileType.Share)

        For Each share In configuration
            Dim file = q.FirstOrDefault(Function(p) p.FileShareId = share.FileShareId)
            If file Is Nothing Then
                file = New FileEntryInfo()
                file.FileShareId = share.FileShareId
                file.FileEntryTypeId = FileType.Share
            End If



            If file.FileVersion Is Nothing Then
                Dim fileSystemEntryVersion As New FileVersionInfo()
                With fileSystemEntryVersion
                    .FileEntryId = file.FileEntryId
                    .FileEntryName = share.ShareName
                    .FileEntryNameWithExtension = share.ShareName
                    .FileEntryRelativePath = share.SharePath
                End With
                file.FileVersion = fileSystemEntryVersion
            End If

            Dim node = CreateNode(tvDriveExplorer.Nodes, file)
            node.Tag = file
            ValidateIfShareExists(node, share)
            If Not file.IsDeleted Then
                BuildDirectoryStructure(node, file, files)
            End If
        Next


        ''Filelist contain only shares as file entity
        'Dim fileList = From c In configuration
        '               From f In files
        '               Where c.ShareId = f.ShareId And f.FileSystemEntryTypeId = FileType.Share
        '               Select New FileSystemEntryInfo With
        '                {
        '                    .FileSystemEntryId = f.FileSystemEntryId,
        '                    .ShareId = c.ShareId,
        '                    .FileSystemEntryTypeId = f.FileSystemEntryTypeId,
        '                    .FileSystemEntryVersion = New FileVersionInfo() With {
        '                        .FileSystemEntryName = c.ShareName,
        '                        .FileSystemEntryNameWithExtension = c.ShareName,
        '                        .FileSystemEntryId = f.FileSystemEntryId,
        '                        .FileSystemEntryRelativePath = c.SharePath
        '                    }
        '                }

        ''Create TV-first by share node then their children one by one
        'For Each file In fileList.ToList()
        '    Dim node = CreateNode(tvDriveExplorer.Nodes, file)
        '    node.Tag = file
        '    BuildDirectoryStructure(node, file, files)
        'Next

        If tvDriveExplorer.Nodes.Count > 0 Then
            tvDriveExplorer.SelectedNode = tvDriveExplorer.Nodes(0)
        End If
    End Sub

    Private Function CreateNode(parentNode As TreeNodeCollection, file As FileEntryInfo) As TreeNode
        Dim name As String = file.FileVersion.FileEntryName
        If file.FileEntryId <> Guid.Empty AndAlso file.FileEntryTypeId = FileType.Share AndAlso file.IsDeleted Then
            name = name + " (Missing)"
        ElseIf file.FileEntryId = Guid.Empty Then
            name = name + " (Loading...)"
        End If
        Dim node = parentNode.Add(file.FileEntryId.ToString(), name)
        Dim index = 1
        If file.FileEntryTypeId = FileType.Share Then
            index = driveImageIndex
            node.SelectedImageIndex = driveImageIndex
        ElseIf file.FileEntryTypeId = FileType.Folder Then
            index = 1
        End If

        node.ImageIndex = index
        Return node
    End Function

    Private Sub BuildDirectoryStructure(parent As TreeNode, file As FileEntryInfo, files As IList(Of FileEntryInfo))
        If parent Is Nothing Then
            Return
        End If
        Dim parentNode = parent.Nodes
        Dim nodes = parentNode.Cast(Of TreeNode)()
        Dim fFiles = files.Where(Function(p) p.FileVersion IsNot Nothing AndAlso Not p.IsDeleted AndAlso p.FileVersion.ParentFileEntryId = file.FileEntryId AndAlso p.FileEntryTypeId = FileType.Folder).ToList()

        Dim directoryExists As Boolean = False

        Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)
        Dim localFolders As New List(Of String)()
        Using CommonUtils.Helper.Impersonate(share)
            directoryExists = System.IO.Directory.Exists(file.FileVersion.FileEntryRelativePath)
            If directoryExists Then
                localFolders = System.IO.Directory.GetDirectories(file.FileVersion.FileEntryRelativePath).Where(Function(p) Not p.ToLower().EndsWith(Enums.Constants.TEMP_FOLDER_NAME.ToLower())).ToList()
            End If
        End Using

        If (directoryExists) Then
            Dim q = From d In localFolders
                    Group Join n In nodes.Where(Function(p) p.Tag IsNot Nothing) On d.ToLower() Equals CType(n.Tag, FileEntryInfo).FileVersion.FileEntryRelativePath.ToLower() Into Group
                    From n In Group.DefaultIfEmpty()
                    Where n Is Nothing
                    Select d

            Dim qq = From n In nodes.Where(Function(p) p.Tag IsNot Nothing)
                     From f In fFiles
                     Where CType(n.Tag, FileEntryInfo).FileVersion.FileEntryRelativePath = f.FileVersion.FileEntryRelativePath
                     Select _node = n, _file = f

            For Each ff In qq
                ff._node.Tag = ff._file
            Next

            'Added local foders and matching files on TV
            For Each d In q
                Dim name = System.IO.Path.GetFileName(d)
                Dim ff = fFiles.FirstOrDefault(Function(p) p.FileVersion.FileEntryNameWithExtension.ToLower() = name.ToLower())
                'Local Folder
                If ff Is Nothing Then
                    ff = New FileEntryInfo()
                    ff.FileEntryTypeId = FileType.Folder
                    ff.IsLocalFile = True
                    ff.IsPhysicalFile = True
                    'ff.FileSystemEntryId = file.FileSystemEntryId
                    ff.FileVersion = New FileVersionInfo()
                End If

                ff.FileVersion.FileEntryRelativePath = d

                ''add node in ascending order
                'Dim index As Integer = 0
                'For index = 0 To parentNode.Count - 1
                '    If name.ToLower() <parentNode(index).Text.ToLower() Then
                '        Exit For
                '    End If
                'Next

                'Dim node = parentNode.Insert(index, name)

                Dim node = parentNode.Add(name)
                node.Tag = ff
                node.ImageIndex = 1
                BuildDirectoryStructure(node, ff, files)
            Next

            Dim q2 = From n In nodes.Where(Function(p) p.Tag IsNot Nothing)
                     Group Join d In localFolders On d.ToLower() Equals CType(n.Tag, FileEntryInfo).FileVersion.FileEntryRelativePath.ToLower() Into Group
                     From d In Group.DefaultIfEmpty()
                     Where d Is Nothing
                     Select n

            For Each n In q2.ToList()
                parentNode.Remove(n)
            Next

            'Dim fFiles = files.Where(Function(p) p.FileSystemEntryVersion IsNot Nothing AndAlso p.FileSystemEntryVersion.ParentFileSystemEntryId = file.FileSystemEntryId AndAlso p.FileSystemEntryTypeId = FileType.Folder).ToList()
            'Dim localFolders = System.IO.Directory.GetDirectories(file.FileSystemEntryVersion.FileSystemEntryRelativePath)

            ''Added local foders and matching files on TV
            'For Each d In localFolders
            '    Dim name = System.IO.Path.GetFileName(d)
            '    Dim ff = fFiles.FirstOrDefault(Function(p) p.FileSystemEntryVersion.FileSystemEntryNameWithExtension.ToLower() = name.ToLower())
            '    'Local Folder
            '    If ff Is Nothing Then
            '        ff = New FileSystemEntryInfo()
            '        ff.FileSystemEntryTypeId = FileType.Folder
            '        ff.IsLocalFile = True
            '        ff.FileSystemEntryId = file.FileSystemEntryId
            '        ff.FileSystemEntryVersion = New FileVersionInfo()
            '    End If

            '    ff.FileSystemEntryVersion.FileSystemEntryRelativePath = d

            '    Dim node = parentNode.Add(name)
            '    node.Tag = ff
            '    node.ImageIndex = 1
            '    BuildDirectoryStructure(node.Nodes, ff, files)
            'Next


            ''Add remaining folder from server if any
            'For Each sFile In fFiles
            '    sFile.FileSystemEntryVersion.FileSystemEntryRelativePath = file.FileSystemEntryVersion.FileSystemEntryRelativePath + "\" + sFile.FileVersion.FileSystemEntryNameWithExtension
            '    Dim ff = Nothing
            '    If localFolders IsNot Nothing Then
            '        ff = localFolders.FirstOrDefault(Function(p) p = sFile.FileSystemEntryVersion.FileSystemEntryRelativePath)
            '    End If
            '    If ff Is Nothing Then
            '        Dim serverFile = New FileSystemEntryInfo()
            '        serverFile.IsLocalFile = False
            '        serverFile.ParentFileId = sFile.FileSystemEntryId
            '        serverFile.FileSystemEntryTypeId = FileType.Folder
            '        serverFile.FileSystemEntryVersion.FileSystemEntryRelativePath = sFile.FileSystemEntryVersion.FileSystemEntryRelativePath + "\" + sFile.FileVersion.FileSystemEntryNameWithExtension

            '        Dim node = parentNode.Add(sFile.FileVersion.FileSystemEntryName)
            '        node.Tag = sFile
            '        node.ImageIndex = 1
            '        BuildDirectoryStructure(node.Nodes, serverFile, files)
            '    End If

            'Next
        Else
            LoadFolderDataAsync(parent.Parent)
        End If
    End Sub

    Private Async Sub BuildDirectoryStructureAsync()
        Dim selectedNode As TreeNode = mouseClickedNode
        If selectedNode Is Nothing Then
            selectedNode = tvDriveExplorer.SelectedNode
        End If

        If selectedNode IsNot Nothing Then
            Dim file = TryCast(selectedNode.Tag, FileEntryInfo)
            If file IsNot Nothing Then
                'Dim async = New AsyncProvider()
                'async.AddMethod("Files", Function() New Common.LocalFileClient().GetFiles())
                'async.OnCompletion = Sub(list As IDictionary)
                '                         Dim fileResult = CType(list("Files"), ResultInfo(Of List(Of FileSystemEntryInfo)))
                '                         If ValidateResponse(fileResult) AndAlso ValidateResponse(fileResult) Then
                '                             tvDriveExplorer.BeginUpdate()
                '                             selectedNode.Nodes.Clear()
                '                             BuildDirectoryStructure(selectedNode.Nodes, file, fileResult.Data)
                '                             tvDriveExplorer.EndUpdate()
                '                         End If
                '                     End Sub
                'async.Execute()

                Using client As New Common.LocalFileClient()
                    Dim result = Await client.GetFilesAsync()
                    If ValidateResponse(result) Then
                        tvDriveExplorer.BeginUpdate()
                        selectedNode.Nodes.Clear()
                        BuildDirectoryStructure(selectedNode, file, result.Data)
                        tvDriveExplorer.EndUpdate()
                    End If
                End Using

            End If
        End If
    End Sub

    Private Async Sub LoadFolderDataAsync(node As TreeNode, Optional file As FileEntryInfo = Nothing)
        'clear search box
        If txt_search.Text.Length > 0 Then
            txt_search.Text = String.Empty
        End If

        ShowHideDetailPane(True)

        If file Is Nothing AndAlso node IsNot Nothing AndAlso node.Tag IsNot Nothing Then
            file = TryCast(node.Tag, FileEntryInfo)
        End If

        Dim files As IList(Of FileEntryInfo) = Nothing
        If file Is Nothing OrElse file.FileEntryId = Guid.Empty Then
            files = New List(Of FileEntryInfo)()
            UpdateFolderData(node, file, files)
        Else
            ShowProgress()
            Using client As New Common.LocalFileClient()
                Dim result = Await client.GetFilesByParentIdAsync(file.FileEntryId)
                If ValidateResponse(result) Then
                    UpdateFolderData(node, file, result.Data)
                    HideProgress()
                Else
                    lblMessageRight.Text = My.Resources.ErrorGeneric
                    ShowHideDetailPane(False)
                End If
            End Using
        End If
    End Sub

    Private Sub UpdateFolderData(node As TreeNode, file As FileEntryInfo, files As List(Of FileEntryInfo))
        If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
            Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)
            ValidateIfShareExists(node, share)
            Using CommonUtils.Helper.Impersonate(share)
                If Directory.Exists(file.FileVersion.FileEntryRelativePath) Then

                    lvItems.BeginUpdate()
                    lvItems.Items.Clear()
                    lvItems.Tag = node

                    txtbreadcrum.Text = file.FileVersion.FileEntryRelativePath



                    Dim dbWithDirectories = MergeDbWIthDirectory(file, files)
                    Dim dbWithFiles = MergeDbWIthFiles(file, files)

                    Dim list As New List(Of FileEntryInfo)()
                    list.AddRange(dbWithDirectories)
                    list.AddRange(dbWithFiles)

                    For Each f In list
                        If f.FileVersion.DeltaOperation IsNot Nothing AndAlso f.FileVersion.DeltaOperation.IsConflicted Then
                            f.FileVersion.FileEntryStatusDisplayName = "Conflict"
                        ElseIf f.FileVersion.DeltaOperation IsNot Nothing AndAlso f.FileVersion.DeltaOperation.FileEntryStatusId = 0 Then
                            f.FileVersion.FileEntryStatusDisplayName = ""
                        ElseIf f.IsDeleted Then
                            f.FileVersion.FileEntryStatusDisplayName = "Deleted"
                        ElseIf f.IsCheckedOut Then
                            f.FileVersion.FileEntryStatusDisplayName = "Checked-Out"
                        End If
                    Next

                    Dim items As IEnumerable(Of ListViewItem) = lvItems.Items.Cast(Of ListViewItem)()

                    '' delete missing items
                    Dim queryDelete = From item In items
                                      Group Join f In list On item.Text.ToLower() Equals f.FileVersion.FileEntryNameWithExtension.ToLower() Into Group
                                      From f In Group.DefaultIfEmpty()
                                      Where f Is Nothing
                                      Select item


                    For Each item In queryDelete
                        lvItems.Items.Remove(item)
                    Next

                    '' update existing items
                    Dim queryUpdate = From item In items
                                      Join f In list On item.Text.ToLower() Equals f.FileVersion.FileEntryNameWithExtension.ToLower()
                                      Select _item = item, _file = f




                    For Each fi In queryUpdate
                        Dim f = fi._file
                        Dim item = fi._item
                        AddItemToListView(item, f)
                    Next

                    '' add missing items
                    Dim queryAdd = From f In list
                                   Group Join item In items On f.FileVersion.FileEntryNameWithExtension.ToLower() Equals item.Text.ToLower() Into Group
                                   From item In Group.DefaultIfEmpty()
                                   Where item Is Nothing
                                   Select f

                    For Each f In queryAdd
                        Dim item As New ListViewItem(f.FileVersion.FileEntryNameWithExtension)
                        AddItemToListView(item, f)
                        lvItems.Items.Add(item)
                    Next

                    'For Each d In System.IO.Directory.GetDirectories(file.FileSystemEntryVersion.FileSystemEntryRelativePath)
                    '    Dim name As String = System.IO.Path.GetFileName(d)
                    '    Dim f = files.FirstOrDefault(Function(p) p.FileSystemEntryVersion.FileSystemEntryNameWithExtension.ToLower() = name.ToLower())
                    '    Dim data = New String(3) {}

                    '    If f IsNot Nothing Then
                    '        data(0) = f.FileSystemEntryVersion.VersionNumber.ToString()
                    '        data(1) = f.CheckedOutByUserName
                    '        data(2) = f.FileSystemEntryVersion.CreatedOnUTC.ToLocalTime().ToString()
                    '        If f.FileSystemEntryVersion.DeltaOperation.IsConflicted Then
                    '            data(3) = "Conflict"
                    '        Else
                    '            data(3) = f.FileSystemEntryVersion.FileSystemEntryStatusDisplayName
                    '        End If
                    '    Else
                    '        f = New FileSystemEntryInfo()
                    '        f.FileSystemEntryTypeId = FileType.Folder
                    '        f.IsLocalFile = True

                    '        f.FileSystemEntryVersion = New FileVersionInfo()
                    '        f.FileSystemEntryVersion.ParentFileSystemEntryId = file.FileSystemEntryId

                    '        data(0) = String.Empty
                    '        data(1) = String.Empty
                    '        data(2) = String.Empty

                    '        data(3) = "New"

                    '    End If

                    '    f.FileSystemEntryVersion.FileSystemEntryRelativePath = d

                    '    Dim lvi As New ListViewItem(name)

                    '    lvi.SubItems.AddRange(data)
                    '    Dim item = lvItems.Items.Add(lvi)
                    '    item.Tag = f
                    '    item.ImageIndex = 1
                    'Next

                    'For Each ff In System.IO.Directory.GetFiles(file.FileSystemEntryVersion.FileSystemEntryRelativePath)
                    '    Dim name As String = System.IO.Path.GetFileName(ff)
                    '    Dim f = files.FirstOrDefault(Function(p) p.FileSystemEntryVersion.FileSystemEntryNameWithExtension.ToLower() = name.ToLower())
                    '    Dim data = New String(3) {}

                    '    If f IsNot Nothing Then
                    '        data(0) = f.FileSystemEntryVersion.VersionNumber.ToString()
                    '        data(1) = f.CheckedOutByUserName
                    '        data(2) = f.FileSystemEntryVersion.CreatedOnUTC.ToLocalTime().ToString()
                    '        If f.FileSystemEntryVersion.DeltaOperation.IsConflicted Then
                    '            data(3) = "Conflict"
                    '        Else
                    '            data(3) = f.FileSystemEntryVersion.FileSystemEntryStatusDisplayName
                    '        End If
                    '    Else
                    '        f = New FileSystemEntryInfo()
                    '        f.FileSystemEntryTypeId = FileType.File
                    '        f.IsLocalFile = True
                    '        f.FileSystemEntryVersion = New FileVersionInfo()
                    '        f.FileSystemEntryVersion.ParentFileSystemEntryId = file.FileSystemEntryId
                    '        data(0) = String.Empty
                    '        data(1) = String.Empty
                    '        data(2) = String.Empty
                    '        If (Not name.Contains("##")) Then
                    '            data(3) = "New"
                    '        End If
                    '    End If

                    '    f.FileSystemEntryVersion.FileSystemEntryRelativePath = ff

                    '    Dim lvi As New ListViewItem(name)
                    '    lvi.SubItems.AddRange(data)
                    '    Dim item = lvItems.Items.Add(lvi)
                    '    item.Tag = f
                    '    IconManager.AddEx(System.IO.Path.GetExtension(ff), IconSize.Small)
                    '    item.ImageIndex = IconManager.GetIconIndex(System.IO.Path.GetExtension(ff), IconSize.Small)
                    'Next


                    Dim permprov As PermissionProvider = New PermissionProvider(file.FileEntryId)
                    If (Not permprov.CanShowOtherConflictsColumns) Then
                        Dim oCol = lvItems.Columns("OC")
                        If (oCol IsNot Nothing) Then
                            lvItems.Columns.Remove(oCol)
                        End If
                    End If

                    lvItems.EndUpdate()
                End If

                If lvItems.Items.Count = 0 Then
                    lblMessageRight.Text = My.Resources.MessageFolderEmpty
                    ShowHideDetailPane(False)
                End If
            End Using
        End If

        BuildDirectoryStructure(node, file, files)
    End Sub

    Private Sub AddItemToListView(item As ListViewItem, f As FileEntryInfo)
        item.Tag = f
        If f.FileEntryTypeId = FileType.Folder Then
            item.ImageIndex = 1
        Else
            IconManager.AddEx(System.IO.Path.GetExtension(f.FileVersion.FileEntryNameWithExtension), IconSize.Small)
            item.ImageIndex = IconManager.GetIconIndex(System.IO.Path.GetExtension(f.FileVersion.FileEntryNameWithExtension), IconSize.Small)
        End If

        Dim data = New String(4) {}
        data(0) = If(f.FileVersion.VersionNumber.HasValue AndAlso f.FileVersion.VersionNumber.Value > 0, f.FileVersion.VersionNumber.ToString(), f.FileVersion.PreviousVersionNumber.ToString())
        data(1) = If(f.IsCheckedOut, f.CheckedOutByUserName, "")
        data(2) = If(f.FileVersion.CreatedOnUTC = DateTime.MinValue, "", f.FileVersion.CreatedOnUTC.ToLocalTime().ToString())
        data(3) = f.FileVersion.FileEntryStatusDisplayName
        If (f.IsOtherConflicts) Then
            data(4) = f.IsOtherConflicts
        Else
            data(4) = ""
        End If



        Dim text = item.Text
        item.SubItems.Clear()
        item.Text = text
        item.SubItems.AddRange(data)
        item.SubItems(5).Name = "OC"
    End Sub

    Private Function MergeDbWIthDirectory(file As FileEntryInfo, files As List(Of FileEntryInfo)) As IEnumerable(Of FileEntryInfo)
        Dim directories = System.IO.Directory.GetDirectories(file.FileVersion.FileEntryRelativePath).Where(Function(d) Not d.ToLower().EndsWith(Enums.Constants.TEMP_FOLDER_NAME.ToLower()))

        Dim q = files.Where(Function(p) p.FileEntryTypeId = FileType.Folder)

        Dim gquery = q.GroupBy(Function(p) p.FileVersion.FileEntryNameWithExtension.ToLower()).Select(Function(p) New With {.Key = p.Key, .Count = p.Count()}).ToList()

        Dim names = gquery.Where(Function(p) p.Count = 1).Select(Function(p) p.Key).ToList()
        Dim q2 = q.Where(Function(p) names.Contains(p.FileVersion.FileEntryNameWithExtension.ToLower()) AndAlso ((Not p.IsDeleted AndAlso Not p.FileVersion.DeltaOperation.IsConflicted.Value) OrElse (Not p.IsDeleted AndAlso p.FileVersion.DeltaOperation.IsConflicted.HasValue AndAlso p.FileVersion.DeltaOperation.IsConflicted.Value) OrElse (p.IsDeleted AndAlso Not p.IsPermanentlyDeleted AndAlso (AuthData.User.RoleId = Role.AccountAdmin OrElse (New PermissionProvider(p).GetPermissions() And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)) OrElse (p.IsDeleted AndAlso p.FileVersion.DeltaOperation.IsConflicted.HasValue AndAlso p.FileVersion.DeltaOperation.IsConflicted.Value)))

        Dim query = From f In q2
                    Group Join d In directories On f.FileVersion.FileEntryNameWithExtension.ToLower() Equals System.IO.Path.GetFileName(d).ToLower() Into Group
                    From d In Group.DefaultIfEmpty()
                    Where d Is Nothing OrElse Not d.ToLower().EndsWith(Enums.Constants.TEMP_FOLDER_NAME.ToLower())
                    Select _file = f, _d = d



        Dim list As New List(Of FileEntryInfo)()
        For Each item In query
            Dim f = item._file
            Dim d = item._d

            If d IsNot Nothing Then
                f.IsPhysicalFile = True
            End If

            f.FileVersion.FileEntryRelativePath = d
            list.Add(f)
        Next

        '
        '1. If Not f.IsDeleted && Delta = Pending Delete                then New
        '2. If Not f.IsDeleted && Delta != Pending Delete               then Delta Status
        '3. If f.IsDeleted && Delta = null                              then New
        '4. If f.IsDeleted && Delta != Pending Delete                   then To be deleted
        '5. If f.IsDeleted && Delta = Pending Delete                    then New

        Dim duplicateNames = gquery.Where(Function(p) p.Count > 1).Select(Function(p) p.Key).ToList()
        Dim q3 = q.Where(Function(p) duplicateNames.Contains(p.FileVersion.FileEntryNameWithExtension.ToLower()))

        Dim qq = From f In q3
                 From d In directories
                 Where f.FileVersion.FileEntryNameWithExtension.ToLower() = System.IO.Path.GetFileName(d).ToLower() AndAlso Not f.IsDeleted AndAlso f.FileVersion.DeltaOperation IsNot Nothing
                 Select _file = f, _d = d

        For Each item In qq
            Dim f = item._file
            f.IsPhysicalFile = True
            f.FileVersion.FileEntryRelativePath = item._d
            f.FileVersion.FileEntryStatusDisplayName = If(f.FileVersion.DeltaOperation.FileEntryStatus <> FileEntryStatus.PendingFileSystemEntryDelete, f.FileVersion.DeltaOperation.FileEntryStatus, "New")
            list.Add(f)
        Next

        Dim qq2 = From f In q3
                  From d In directories
                  Where f.FileVersion.FileEntryNameWithExtension.ToLower() = System.IO.Path.GetFileName(d).ToLower() AndAlso f.IsDeleted AndAlso (AuthData.User.RoleId = Role.AccountAdmin OrElse (New PermissionProvider(f).GetPermissions() And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
                  Select _file = f, _d = d


        For Each item In qq2.GroupBy(Function(p) p._file.FileVersion.FileEntryNameWithExtension.ToLower())
            Dim item2 = item.OrderByDescending(Function(p) p._file.FileVersion.CreatedOnUTC).FirstOrDefault()
            Dim f = item2._file
            f.IsPhysicalFile = True
            f.FileVersion.FileEntryRelativePath = item2._d
            f.FileVersion.FileEntryStatusDisplayName = If(f.FileVersion.DeltaOperation Is Nothing, "New", If(f.FileVersion.DeltaOperation.FileEntryStatus <> FileEntryStatus.PendingFileSystemEntryDelete, "To be deleted", "New"))
            list.Add(f)
        Next


        Dim restDirectoriesQuery = From d In directories
                                   Group Join dn In duplicateNames On System.IO.Path.GetFileName(d).ToLower() Equals dn.ToLower() Into Group
                                   From dn In Group.DefaultIfEmpty()
                                   Where dn Is Nothing
                                   Select d

        Dim query2 = From d In restDirectoriesQuery
                     Group Join f In q On System.IO.Path.GetFileName(d).ToLower() Equals f.FileVersion.FileEntryNameWithExtension.ToLower() Into Group
                     From f In Group.DefaultIfEmpty()
                     Where f Is Nothing
                     Select d

        For Each item In query2

            Dim f = New FileEntryInfo()
            f.FileEntryTypeId = FileType.Folder
            f.IsLocalFile = True
            f.IsPhysicalFile = True

            f.FileVersion = New FileVersionInfo()
            f.FileVersion.ParentFileEntryId = file.FileEntryId

            f.FileVersion.FileEntryName = IO.Path.GetFileNameWithoutExtension(item)
            Dim filename = IO.Path.GetFileName(item)
            f.FileVersion.FileEntryNameWithExtension = filename
            f.FileVersion.FileEntryStatusDisplayName = If(filename.StartsWith("##"), "Temporary File", "New")

            f.FileVersion.FileEntryRelativePath = item
            list.Add(f)
        Next

        list = list.OrderBy(Function(p) p.FileVersion.FileEntryNameWithExtension).ToList()

        Return list
    End Function

    Private Function MergeDbWIthFiles(file As FileEntryInfo, files As List(Of FileEntryInfo)) As IEnumerable(Of FileEntryInfo)
        Dim ff = System.IO.Directory.GetFiles(file.FileVersion.FileEntryRelativePath)

        Dim q = files.Where(Function(p) p.FileEntryTypeId = FileType.File)


        Dim gquery = q.GroupBy(Function(p) p.FileVersion.FileEntryNameWithExtension.ToLower()).Select(Function(p) New With {.Key = p.Key, .Count = p.Count()}).ToList()

        Dim names = gquery.Where(Function(p) p.Count = 1).Select(Function(p) p.Key).ToList()
        Dim q2 = q.Where(Function(p) names.Contains(p.FileVersion.FileEntryNameWithExtension.ToLower())).Where(Function(p) (Not p.IsDeleted AndAlso Not p.FileVersion.DeltaOperation.IsConflicted.Value) OrElse (Not p.IsDeleted AndAlso p.FileVersion.DeltaOperation.IsConflicted.HasValue AndAlso p.FileVersion.DeltaOperation.IsConflicted.Value) OrElse (p.IsDeleted AndAlso Not p.IsPermanentlyDeleted AndAlso (AuthData.User.RoleId = Role.AccountAdmin OrElse (New PermissionProvider(p).GetPermissions() And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)) OrElse (p.IsDeleted AndAlso p.FileVersion.DeltaOperation.IsConflicted.HasValue AndAlso p.FileVersion.DeltaOperation.IsConflicted.Value))

        Dim query = From f In q2
                    Group Join d In ff On f.FileVersion.FileEntryNameWithExtension.ToLower() Equals System.IO.Path.GetFileName(d).ToLower() Into Group
                    From d In Group.DefaultIfEmpty()
                    Select _file = f, _d = d


        Dim list As New List(Of FileEntryInfo)()
        For Each item In query
            Dim f = item._file
            Dim d = item._d

            If d IsNot Nothing Then
                f.IsPhysicalFile = True
            End If

            f.FileVersion.FileEntryRelativePath = d
            list.Add(f)
        Next

        '
        '1. If Not f.IsDeleted && Delta = Pending Delete                then New
        '2. If Not f.IsDeleted && Delta != Pending Delete               then Delta Status
        '3. If f.IsDeleted && Delta = null                              then New
        '4. If f.IsDeleted && Delta != Pending Delete                   then To be deleted
        '5. If f.IsDeleted && Delta = Pending Delete                    then New

        Dim duplicateNames = gquery.Where(Function(p) p.Count > 1).Select(Function(p) p.Key).ToList()
        Dim q3 = q.Where(Function(p) duplicateNames.Contains(p.FileVersion.FileEntryNameWithExtension.ToLower()))

        Dim qq = From f In q3
                 From d In ff
                 Where f.FileVersion.FileEntryNameWithExtension.ToLower() = System.IO.Path.GetFileName(d).ToLower() AndAlso Not f.IsDeleted AndAlso f.FileVersion.DeltaOperation IsNot Nothing
                 Select _file = f, _d = d

        For Each item In qq
            Dim f = item._file
            f.IsPhysicalFile = True
            f.FileVersion.FileEntryRelativePath = item._d
            f.FileVersion.FileEntryStatusDisplayName = If(f.FileVersion.DeltaOperation.FileEntryStatus <> FileEntryStatus.PendingFileSystemEntryDelete, f.FileVersion.DeltaOperation.FileEntryStatus, "New")
            list.Add(f)
        Next

        Dim qq2 = From f In q3
                  From d In ff
                  Where f.FileVersion.FileEntryNameWithExtension.ToLower() = System.IO.Path.GetFileName(d).ToLower() AndAlso f.IsDeleted AndAlso (AuthData.User.RoleId = Role.AccountAdmin OrElse (New PermissionProvider(f).GetPermissions() And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
                  Select _file = f, _d = d


        For Each item In qq2.GroupBy(Function(p) p._file.FileVersion.FileEntryNameWithExtension.ToLower())
            Dim item2 = item.OrderByDescending(Function(p) p._file.FileVersion.CreatedOnUTC).FirstOrDefault()
            Dim f = item2._file
            f.IsPhysicalFile = True
            f.FileVersion.FileEntryRelativePath = item2._d
            f.FileVersion.FileEntryStatusDisplayName = If(f.FileVersion.DeltaOperation Is Nothing, "New", If(f.FileVersion.DeltaOperation.FileEntryStatus <> FileEntryStatus.PendingFileSystemEntryDelete, "To be deleted", "New"))
            list.Add(f)
        Next

        Dim restFilesQuery = From f In ff
                             Group Join dn In duplicateNames On System.IO.Path.GetFileName(f).ToLower() Equals dn.ToLower() Into Group
                             From dn In Group.DefaultIfEmpty()
                             Where dn Is Nothing
                             Select f

        Dim query2 = From d In restFilesQuery
                     Group Join f In q On System.IO.Path.GetFileName(d).ToLower() Equals f.FileVersion.FileEntryNameWithExtension.ToLower() Into Group
                     From f In Group.DefaultIfEmpty()
                     Where f Is Nothing
                     Select d

        For Each item In query2

            Dim f = New FileEntryInfo()
            f.FileEntryTypeId = FileType.File
            f.IsLocalFile = True
            f.IsPhysicalFile = True

            f.FileVersion = New FileVersionInfo()
            f.FileVersion.ParentFileEntryId = file.FileEntryId

            f.FileVersion.FileEntryName = IO.Path.GetFileNameWithoutExtension(item)
            Dim filename = IO.Path.GetFileName(item)
            f.FileVersion.FileEntryNameWithExtension = filename
            f.FileVersion.FileEntryStatusDisplayName = If(filename.StartsWith("##"), "Temporary File", "New")

            f.FileVersion.FileEntryRelativePath = item
            list.Add(f)
        Next

        list = list.OrderBy(Function(p) p.FileVersion.FileEntryNameWithExtension).ToList()

        Return list
    End Function

    Public Sub ShowProgress(Optional style As ProgressBarStyle = ProgressBarStyle.Marquee)
        tsProgressBar.Visible = True
        tsProgressBar.Style = style
        tsslStatus.BackColor = Color.Empty
        tsslStatus.Text = "Loading..."
    End Sub

    Public Sub HideProgress()
        tsProgressBar.Visible = False
        tsProgressBar.Value = 0
        tsslStatus.Text = ""
    End Sub

    Private Sub DisableButtons()

        tsmiCheckIn.Enabled = False
        tsmiCheckout.Enabled = False
        tsmiUndoCheckout.Enabled = False
        tsmiHistory.Enabled = False
        tsmiLink.Enabled = False

        tsmiCut.Enabled = False
        tsmiCopy.Enabled = False
        tsmiPaste.Enabled = False
        tsmiDelete.Enabled = False

    End Sub

    Private Sub ShowProperties()
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim prop As Properties = New Properties(file)
                    prop.ShowDialog()
                End If
            End If
        Else
            If tvDriveExplorer.SelectedNode IsNot Nothing AndAlso tvDriveExplorer.SelectedNode.Tag IsNot Nothing Then
                Dim file = CType(tvDriveExplorer.SelectedNode.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim prop As Properties = New Properties(file)
                    prop.ShowDialog()
                End If
            End If
        End If
    End Sub

    Private Sub ShowHistory()
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim histry As History = New History(file)
                    histry.ShowDialog()
                End If
            End If
        Else
            If tvDriveExplorer.SelectedNode IsNot Nothing AndAlso tvDriveExplorer.SelectedNode.Tag IsNot Nothing Then
                Dim file = CType(tvDriveExplorer.SelectedNode.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim histry As History = New History(file)
                    histry.ShowDialog()
                End If
            End If
        End If
    End Sub

    Private Sub ShowLink()
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Dim lnk As Link = New Link(file)
                    lnk.ShowDialog()
                End If
            End If
        End If
    End Sub

    Private Sub Delete()
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim selectedNode = tvDriveExplorer.SelectedNode
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If (file.FileVersion.FileEntryRelativePath IsNot Nothing AndAlso file.FileVersion.FileEntryRelativePath.StartsWith("##")) Then

                    Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)
                    Using CommonUtils.Helper.Impersonate(share)
                        If (System.IO.File.Exists(file.FileVersion.FileEntryRelativePath) = True) Then
                            System.IO.File.Delete(file.FileVersion.FileEntryRelativePath)
                        End If
                    End Using
                Else
                    If (file.IsDeleted) Then
                        If (isHardDelete = True) Then
                            Dim result1 As Integer = MessageBoxHelper.ShowQuestion(My.Resources.QuestionDeleteItemPermanently)
                            If result1 = DialogResult.Yes Then
                                If (HardDeleteServer() = True) Then
                                    'HardDeleteClient()
                                    LoadFolderDataAsync(selectedNode)
                                End If
                            End If
                        End If
                    Else
                        Dim result As Integer = MessageBoxHelper.ShowQuestion(My.Resources.QuestionDeleteItem)
                        If result = DialogResult.Yes Then
                            If (isHardDelete = True) Then
                                Dim result1 As Integer = MessageBoxHelper.ShowQuestion(My.Resources.QuestionDeleteItemPermanently)
                                If result1 = DialogResult.Yes Then
                                    If (HardDeleteServer() = True) Then
                                        'HardDeleteClient()
                                        LoadFolderDataAsync(selectedNode)
                                    End If
                                ElseIf result1 = DialogResult.No Then
                                    SoftDeleteClient()
                                    LoadFolderDataAsync(selectedNode)
                                End If
                            Else
                                SoftDeleteClient()
                                LoadFolderDataAsync(selectedNode)
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Function SoftDeleteClient() As Boolean
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New LocalFileClient()
                        Dim res = Proxy.SoftDelete(file.FileEntryId, False)
                        If res.Status = Status.Success Then
                            Return True
                        Else
                            If (res.Status = Status.FileCheckedOut) Then
                                Dim result As Integer = MessageBoxHelper.ShowQuestion(res.Message)
                                If result = DialogResult.Yes Then
                                    Dim resForce = Proxy.SoftDelete(file.FileEntryId, True)
                                    If resForce.Status = Status.Success Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                End If
                            Else
                                Return False
                            End If
                        End If
                    End Using
                End If
            End If
        End If
        Return False
    End Function

    Private Function HardDeleteServer() As Boolean
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New FileClient()
                        Dim res = Proxy.HardDelete(file.FileEntryId)
                        If res.Status = Status.Success OrElse res.Status = Status.NotFound Then
                            Return True
                        Else
                            Return False
                        End If
                    End Using
                End If
            End If
        End If
        Return False
    End Function

    Private Function HardDeleteClient() As Boolean
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New LocalFileClient()
                        Dim res = Proxy.HardDelete(file.FileEntryId)
                        If res.Status = Status.Success OrElse res.Status = Status.NotFound Then
                            Return True
                        Else
                            Return False
                        End If
                    End Using
                End If
            End If
        End If
        Return False
    End Function

    Private Sub UndoDelete()
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New FileClient()
                        Dim res = Proxy.UndoDelete(file.FileEntryId)
                        If res.Status = Status.Success Then
                            'TODO REFRESH CODE
                        Else
                            MessageBox.Show(res.Message)
                        End If
                    End Using
                End If
            End If
        End If
    End Sub

    Private Function ResolveUsingTheirs() As Boolean
        Dim node = tvDriveExplorer.SelectedNode
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New LocalFileClient()
                        Dim res = Proxy.ResolveConflictUsingTheirs(file.FileEntryId, file.FileShareId, file.FileVersion.FileEntryRelativePath, file.FileVersion.FileEntryNameWithExtension)
                        LoadFolderDataAsync(node)
                        If res.Status = Status.Success Then
                            Return True
                        Else
                            Return False
                        End If
                    End Using
                End If
            End If
        End If
        Return False
    End Function

    Private Function ResolveUsingMine() As Boolean
        Dim node = tvDriveExplorer.SelectedNode
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    Using Proxy = New LocalFileClient()
                        Dim res = Proxy.ResolveConflictUsingMine(file.FileEntryId)
                        LoadFolderDataAsync(node)
                        If res.Status = Status.Success Then
                            Return True
                        Else
                            Return False
                        End If
                    End Using
                End If
            End If
        End If
        Return False
    End Function

    Private Async Function RequestUserFileAsync() As Task(Of Boolean)
        Dim node = tvDriveExplorer.SelectedNode
        If lvItems.SelectedItems.Count > 0 Then
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                Dim file = CType(selectedItem.Tag, FileEntryInfo)
                If file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty Then
                    If (m_FileVersionConflictInfo IsNot Nothing) Then
                        Using Proxy = New FileClient()
                            Dim res = Await Proxy.RequestConflictFileUploadAsync(file.FileEntryId, m_FileVersionConflictInfo.FileVersionConflictId)
                            LoadFolderDataAsync(node)
                            If res.Status = Status.Success Then
                                Return True
                            Else
                                Return False
                            End If
                        End Using
                    End If
                End If
            End If
        End If
        Return False
    End Function

    Private Sub CreateNewFolder(tv As TreeView, lv As ListView)
        Dim selectedNode = If(mouseClickedNode IsNot Nothing, mouseClickedNode, tvDriveExplorer.SelectedNode)
        If (selectedNode IsNot Nothing) Then
            Dim file = TryCast(selectedNode.Tag, FileEntryInfo)
            If file IsNot Nothing Then
                Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)
                If share Is Nothing Then
                    MessageBoxHelper.ShowErrorMessage("Share not found")
                    Return
                End If

                ValidateIfShareExists(selectedNode, share)
                If Not file.IsDeleted Then
                    txtbreadcrum.Text = file.FileVersion.FileEntryRelativePath
                    Dim oNewFolder As New NewFolderForm(share, file.FileVersion.FileEntryRelativePath)
                    oNewFolder.ShowDialog(Me)
                    Dim result = oNewFolder.Result

                    If result Then
                        BuildDirectoryStructureAsync()
                        'If lv IsNot Nothing OrElse (tvDriveExplorer.SelectedNode IsNot Nothing AndAlso tvDriveExplorer.SelectedNode.Equals(mouseClickedNode)) Then
                        '    LoadFolderDataAsync(selectedNode, file)
                        'End If
                        LoadFolderDataAsync(selectedNode, file)
                    End If
                End If
            End If
        End If
    End Sub

    Private Function GetCurrentDropEffect() As DragDropEffects
        Dim format As String = "Preferred DropEffect"
        Dim obj As [Object] = Clipboard.GetData(format)
        If obj IsNot Nothing Then
            Dim ms As MemoryStream = DirectCast(obj, MemoryStream)
            Dim br As New BinaryReader(ms)
            Dim de = DirectCast(br.ReadInt32(), DragDropEffects)
            Return de
        End If
        Return DragDropEffects.None
    End Function

    Private Sub PasteItem()
        Dim node = If(mouseClickedNode IsNot Nothing, mouseClickedNode, tvDriveExplorer.SelectedNode)
        Dim destFile = TryCast(node.Tag, FileEntryInfo)
        Try
            Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = destFile.FileShareId)
            Using CommonUtils.Helper.Impersonate(share)
                If (System.Windows.Forms.Clipboard.ContainsFileDropList() = True) Then
                    If (CType(GetCurrentDropEffect() And DragDropEffects.Copy, Int32) = CType(DragDropEffects.Copy, Int32)) Then
                        For Each f In System.Windows.Forms.Clipboard.GetFileDropList()
                            Dim fileName = System.IO.Path.GetFileName(f)
                            Dim path = System.IO.Path.Combine(destFile.FileVersion.FileEntryRelativePath, fileName)
                            If (System.IO.File.Exists(f)) Then
                                FileSystem.CopyFile(f, path)
                            Else
                                If (System.IO.Directory.Exists(f)) Then
                                    FileSystem.CopyDirectory(f, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                                End If
                            End If
                        Next
                    Else
                        If (CType(GetCurrentDropEffect() And DragDropEffects.Move, Int32) = CType(DragDropEffects.Move, Int32)) Then
                            For Each f In System.Windows.Forms.Clipboard.GetFileDropList()
                                Dim fileName = System.IO.Path.GetFileName(f)
                                Dim path = System.IO.Path.Combine(destFile.FileVersion.FileEntryRelativePath, fileName)
                                If (System.IO.File.Exists(f)) Then
                                    FileSystem.MoveFile(f, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                                Else
                                    If (System.IO.Directory.Exists(f)) Then
                                        FileSystem.MoveDirectory(f, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                                    End If
                                End If
                            Next

                        End If
                    End If
                    System.Windows.Forms.Clipboard.Clear()
                Else
                    For Each f In cutCopyList
                        Dim fileName = System.IO.Path.GetFileName(f.FileVersion.FileEntryRelativePath)
                        Dim path = System.IO.Path.Combine(destFile.FileVersion.FileEntryRelativePath, fileName)
                        If cutOrCopy1 = CutOrCopy.Cut Then
                            If f.FileEntryTypeId = FileType.File Then
                                FileSystem.MoveFile(f.FileVersion.FileEntryRelativePath, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                            ElseIf f.FileEntryTypeId = FileType.Folder Then
                                FileSystem.MoveDirectory(f.FileVersion.FileEntryRelativePath, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                            End If
                        Else
                            If f.FileEntryTypeId = FileType.File Then
                                FileSystem.CopyFile(f.FileVersion.FileEntryRelativePath, path)
                            ElseIf f.FileEntryTypeId = FileType.Folder Then
                                FileSystem.CopyDirectory(f.FileVersion.FileEntryRelativePath, path, UIOption.AllDialogs, UICancelOption.ThrowException)
                            End If
                        End If
                    Next
                End If
            End Using
        Catch ex As Exception
            cmsMain.Close()
            MessageBoxHelper.ShowErrorMessage(ex.Message)
        End Try
        BuildDirectoryStructureAsync()
        LoadFolderDataAsync(tvDriveExplorer.SelectedNode)
        cutOrCopy1 = CutOrCopy.None
        cutCopyList.Clear()
        tsmiPaste.Enabled = False

    End Sub

    Private Sub CutCopyItem(ccp As CutOrCopy)
        cutCopyList.Clear()
        cutOrCopy1 = CutOrCopy.None
        If lvItems IsNot Nothing Then
            For Each item As ListViewItem In lvItems.SelectedItems
                Dim f = TryCast(item.Tag, FileEntryInfo)
                If f IsNot Nothing Then
                    cutCopyList.Add(f)

                    Dim DropEffectData(3) As Byte
                    Select Case ccp
                        Case 2 'Copy
                            DropEffectData(0) = 5
                        Case 1 'Move
                            DropEffectData(0) = 2
                        Case Else 'Copy
                            DropEffectData(0) = 5
                    End Select
                    DropEffectData(1) = 0
                    DropEffectData(2) = 0
                    DropEffectData(3) = 0

                    Dim mstream As Stream = New MemoryStream(4)
                    mstream.Write(DropEffectData, 0, DropEffectData.Length)

                    Dim sFiles(0) As String
                    sFiles(0) = f.FileVersion.FileEntryRelativePath
                    Dim data_object As New DataObject()
                    data_object.SetData("FileDrop", True, sFiles)
                    data_object.SetData("Preferred DropEffect", mstream)

                    Clipboard.Clear()
                    Clipboard.SetDataObject(data_object, True)


                End If
            Next
        End If
        cutOrCopy1 = ccp
    End Sub

    Private Sub OpenFolderInExplorer(tv As TreeView, lv As ListView)
        Dim path As String = ""
        Dim file As FileEntryInfo = Nothing
        If lvItems IsNot Nothing And lvItems.SelectedItems IsNot Nothing And lvItems.SelectedItems.Count > 0 Then
            file = TryCast(lvItems.SelectedItems(0).Tag, FileEntryInfo)
            path = file.FileVersion.FileEntryRelativePath
        ElseIf tvDriveExplorer.SelectedNode IsNot Nothing Then
            file = TryCast(tvDriveExplorer.SelectedNode.Tag, FileEntryInfo)
            path = file.FileVersion.FileEntryRelativePath
        End If

        If file IsNot Nothing Then
            Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)

            Using CommonUtils.Helper.Impersonate(share)
                If Not Directory.Exists(path) Then
                    path = IO.Path.GetDirectoryName(path)
                End If

                If (path IsNot Nothing) Then
                    System.Diagnostics.Process.Start(path)
                End If
            End Using
        End If
    End Sub

#End Region

#Region "Search"
    Private Sub btnsearch_Click(sender As Object, e As EventArgs) Handles btnsearch.Click
        Dim selectedNode As TreeNode = mouseClickedNode
        If selectedNode Is Nothing Then
            selectedNode = tvDriveExplorer.SelectedNode
        End If

        If selectedNode IsNot Nothing Then
            Dim file = TryCast(selectedNode.Tag, FileEntryInfo)
            Dim ParentId = file.FileEntryId
            Dim searchText = txt_search.Text

            If String.IsNullOrEmpty(searchText) Then
                Return
            End If

            Dim searchObj = New FileSearch()
            searchObj.StartFileId = ParentId
            searchObj.FileShareId = file.FileShareId
            searchObj.SearchText = searchText
            searchObj.IsAdvancedSearch = False

            Dim share = shares.Where(Function(p) p.FileShareId = file.FileShareId).FirstOrDefault()
            If share IsNot Nothing Then
                searchObj.SharePath = share.SharePath
            End If

            Dim async = New AsyncProvider()
            async.AddMethod("SearchFiles", Function() New Common.LocalFileClient().FileSearch(searchObj))
            async.OnCompletion = Sub(list As IDictionary)
                                     ShowSearchFiles(list, file)
                                 End Sub
            async.Execute()
        End If

    End Sub

    Private Sub btnAdvancedSearch_Click(sender As Object, e As EventArgs) Handles btnAdvancedSearch.Click
        Dim oAdvanceSearch As New AdvanceSearchForm()
        oAdvanceSearch.ShowDialog(Me)

        If oAdvanceSearch.isSearchCanceled = False Then
            Dim oMetadata = oAdvanceSearch.dgvMetadata
            'Search text
            Dim searchText = oAdvanceSearch.txt_search.Text
            'MetaData
            Dim Tags As New List(Of FileTag)()
            For Each row In oMetadata.Rows
                Dim key = String.Empty
                Dim value = String.Empty
                If Not row.Cells(0).Value Is Nothing Then
                    key = row.Cells(0).Value.ToString()
                End If

                If Not row.Cells(1).Value Is Nothing Then
                    value = row.Cells(1).Value.ToString()
                End If

                If (Not String.IsNullOrEmpty(key) OrElse (Not String.IsNullOrEmpty(value))) Then
                    Dim tag As New FileTag()
                    tag.TagName = key
                    tag.TagValue = value

                    Tags.Add(tag)
                End If
            Next

            If String.IsNullOrEmpty(searchText) AndAlso Tags.Count() = 0 Then
                Return
            End If

            'ParentNode
            Dim selectedNode As TreeNode = mouseClickedNode
            If selectedNode Is Nothing Then
                selectedNode = tvDriveExplorer.SelectedNode
            End If

            If selectedNode IsNot Nothing Then
                Dim file = TryCast(selectedNode.Tag, FileEntryInfo)
                Dim ParentId = file.FileEntryId

                Dim searchObj = New FileSearch()
                searchObj.StartFileId = ParentId
                searchObj.SearchText = searchText
                searchObj.Tags = Tags
                searchObj.IsAdvancedSearch = True
                searchObj.FileShareId = file.FileShareId

                Dim share = shares.Where(Function(p) p.FileShareId = file.FileShareId).FirstOrDefault()
                If share IsNot Nothing Then
                    searchObj.SharePath = share.SharePath
                End If

                Dim async = New AsyncProvider()
                async.AddMethod("SearchFiles", Function() New Common.LocalFileClient().FileSearch(searchObj))
                async.OnCompletion = Sub(list As IDictionary)
                                         ShowSearchFiles(list, file)
                                     End Sub
                async.Execute()
            End If
        End If

    End Sub

    Private Sub ShowSearchFiles(list As IDictionary, file As FileEntryInfo)
        Dim result = CType(list("SearchFiles"), ResultInfo(Of List(Of FileEntryInfo), Status))
        If ValidateResponse(result) Then
            lvItems.BeginUpdate()
            lvItems.Items.Clear()
            For Each searchFile In result.Data
                If searchFile.FileEntryId <> file.FileEntryId Then

                    'check for physical file
                    If searchFile.FileEntryTypeId = FileType.Folder Then
                        Dim directoryExists = System.IO.Directory.Exists(file.FileVersion.FileEntryRelativePath)
                        If directoryExists Then
                            searchFile.IsPhysicalFile = True
                        End If
                    Else
                        Dim localFileExist = System.IO.File.Exists(file.FileVersion.FileEntryRelativePath)
                        If localFileExist Then
                            searchFile.IsPhysicalFile = True
                        End If
                    End If

                    Dim Data = New String(3) {}
                    Data(0) = If(searchFile.FileVersion.VersionNumber.HasValue AndAlso searchFile.FileVersion.VersionNumber.Value > 0, searchFile.FileVersion.VersionNumber.ToString(), searchFile.FileVersion.PreviousVersionNumber.ToString())
                    Data(1) = searchFile.CheckedOutByUserName
                    Data(2) = searchFile.FileVersion.CreatedOnUTC.ToLocalTime().ToString()
                    If searchFile.FileVersion.DeltaOperation IsNot Nothing AndAlso searchFile.FileVersion.DeltaOperation.IsConflicted Then
                        Data(3) = "Conflict"
                    Else
                        Data(3) = searchFile.FileVersion.FileEntryStatusDisplayName
                    End If

                    Dim lvi As New ListViewItem(searchFile.FileVersion.FileEntryNameWithExtension)
                    lvi.SubItems.AddRange(Data)
                    lvi.Tag = searchFile
                    Dim item = lvItems.Items.Add(lvi)
                    If searchFile.FileEntryTypeId = FileType.Folder Then
                        item.ImageIndex = 1
                    Else
                        IconManager.AddEx(System.IO.Path.GetExtension(searchFile.FileVersion.FileEntryRelativePath), IconSize.Small)
                        item.ImageIndex = IconManager.GetIconIndex(System.IO.Path.GetExtension(searchFile.FileVersion.FileEntryRelativePath), IconSize.Small)
                    End If
                End If
            Next
            lvItems.EndUpdate()
        End If
    End Sub

#End Region

#Region "Share And Agents"

    Private Sub tsmiAgentList_Click(sender As Object, e As EventArgs) Handles tsmiAgents.Click
        ShowAgent()
    End Sub

    Private Sub tsmiShareList_Click(sender As Object, e As EventArgs) Handles tsmiShares.Click
        ShowShare()
    End Sub


    Private Sub lblAddShare_Click(sender As Object, e As EventArgs) Handles lblAddShare.Click
        ShowShare()
    End Sub

    Private Sub lblAddAgent_Click(sender As Object, e As EventArgs) Handles lblAddAgent.Click
        ShowAgent()
    End Sub

    Private Sub ShowAgent()
        Dim frm As frmAgent = New frmAgent
        If frm.PrepForm(myView, risersoft.shared.EnumfrmMode.acAddM, "") Then frm.ShowDialog()
    End Sub

    Private Sub ShowShare()
        Dim frm As frmShare = New frmShare
        If frm.PrepForm(myView, risersoft.shared.EnumfrmMode.acAddM, "") Then frm.ShowDialog()


        Using syncClient As New LocalSyncClient()
            syncClient.ResetSyncTimestampAsync()
        End Using

        Using shareClient As New LocalShareClient()
            Dim result = shareClient.ShareMappedSummary
            If result.Status = 200 Then
                Dim shareSummary = result.Data

                LoadData(shareSummary)
                '    Dim message As New MessageInfo()
                '    With message
                '        .MessageType = MessageType.Action
                '        .ActionType = ActionType.ConfigureShare
                '        .ActionStatus = ActionStatus.Completed
                '        .Message = "User has changed the share's configurations"
                '        .User = AuthData.User
                '    End With
                '    MessageClientProvider.Instance.BroadcastMessage(message)
            End If
        End Using


    End Sub

#End Region

#Region "NOT IN USE"

    Private Function GetRandomColor(r As Random) As Color
        If r Is Nothing Then
            r = New Random(DateTime.Now.Millisecond)
        End If
        Return Color.FromKnownColor(DirectCast(r.[Next](1, 150), KnownColor))
    End Function



    Private Sub ValidateContextMenu()
        Dim section1 As Boolean = False, section2 As Boolean = False, section3 As Boolean = False, section4 As Boolean = False
        cmsMain.Items.Clear()


        Dim tsmiRefresh As New ToolStripMenuItem("Refresh")
        tsmiRefresh.ShortcutKeys = Keys.F5
        tsmiRefresh.Tag = CommandType.Refresh
        cmsMain.Items.Add(tsmiRefresh)


        Dim tsmiCheckIn As New ToolStripMenuItem("Check In")
        tsmiCheckIn.Tag = CommandType.CheckIn
        cmsMain.Items.Add(tsmiCheckIn)

        Dim tsmiCheckOut As New ToolStripMenuItem("Check Out")
        tsmiCheckOut.Tag = CommandType.CheckOut
        cmsMain.Items.Add(tsmiCheckOut)

        Dim tsmiResolveConflict As New ToolStripMenuItem("Resolve Conflict")
        tsmiResolveConflict.Tag = CommandType.ResolveConflictMine
        cmsMain.Items.Add(tsmiResolveConflict)

        Dim tsmiHistory As New ToolStripMenuItem("History")
        tsmiHistory.Tag = CommandType.History
        cmsMain.Items.Add(tsmiHistory)

        Dim tss1 As New ToolStripSeparator()
        cmsMain.Items.Add(tss1)

        If CanCreateNewFolder() Then
            Dim tsmiNewFolder As New ToolStripMenuItem("New Folder")
            tsmiNewFolder.Tag = CommandType.NewFolder
            cmsMain.Items.Add(tsmiNewFolder)

            Dim tss2 As New ToolStripSeparator()
            cmsMain.Items.Add(tss2)
        End If

        If CanCut() Then
            Dim tsmiCut As New ToolStripMenuItem("Cut")
            tsmiCut.ShortcutKeys = Keys.Control Or Keys.X
            tsmiCut.Tag = CommandType.Cut
            cmsMain.Items.Add(tsmiCut)

            section2 = True
        End If

        If CanCopy() Then
            Dim tsmiCopy As New ToolStripMenuItem("Copy")
            tsmiCopy.ShortcutKeys = Keys.Control Or Keys.C
            tsmiCopy.Tag = CommandType.Copy
            cmsMain.Items.Add(tsmiCopy)

            section2 = True
        End If

        If CanPaste() Then
            Dim tsmiPaste As New ToolStripMenuItem("Paste")
            tsmiPaste.ShortcutKeys = Keys.Control Or Keys.V
            tsmiPaste.Tag = CommandType.Paste
            cmsMain.Items.Add(tsmiPaste)

            section2 = True
        End If

        If CanDelete() Then
            Dim tsmiDelete As New ToolStripMenuItem("Delete")
            tsmiDelete.ShortcutKeys = Keys.Delete
            tsmiDelete.Tag = CommandType.Delete
            cmsMain.Items.Add(tsmiDelete)
            section2 = True
        End If

        If section2 Then
            Dim tss3 As New ToolStripSeparator()
            cmsMain.Items.Add(tss3)
        End If

        Dim tsmiOpenFolderInExplorer As New ToolStripMenuItem("Open Folder In Explorer")
        tsmiOpenFolderInExplorer.Tag = CommandType.OpenFolderInExplorer
        cmsMain.Items.Add(tsmiOpenFolderInExplorer)

        Dim tss4 As New ToolStripSeparator()
        cmsMain.Items.Add(tss4)

        Dim tsmiProperties As New ToolStripMenuItem("Properties")
        tsmiProperties.ShortcutKeys = Keys.Alt Or Keys.Enter
        tsmiProperties.Tag = CommandType.Properties
        cmsMain.Items.Add(tsmiProperties)
    End Sub

    Private Sub CutCopyItemOld(tv As TreeView, lv As ListView, ccp As CutOrCopy)
        cutCopyList.Clear()
        cutOrCopy1 = CutOrCopy.None
        If lv IsNot Nothing Then
            For Each item As ListViewItem In lv.SelectedItems
                Dim f = TryCast(item.Tag, FileEntryInfo)
                If f IsNot Nothing Then
                    cutCopyList.Add(f)
                End If
            Next
        Else
            Dim node = If(mouseClickedNode IsNot Nothing, mouseClickedNode, tv.SelectedNode)
            cutCopyList.Add(TryCast(node.Tag, FileEntryInfo))
        End If
        cutOrCopy1 = ccp
    End Sub

    Private Sub tsmiSwitchUser_Click(sender As Object, e As EventArgs) Handles tsmiSwitchUser.Click
        AuthProvider.ShowLogin(UserSwitchOptions.SwitchUser)
    End Sub

    Private Sub tsmiSwitchAccount_Click(sender As Object, e As EventArgs) Handles tsmiSwitchAccount.Click
        AuthProvider.ShowLogin(UserSwitchOptions.SwitchAccount)
    End Sub

    Private Sub tsmiAddAgent_Click(sender As Object, e As EventArgs) Handles tsmiAddAgent.Click
        AuthProvider.ShowAgent()
    End Sub

    Private Sub Reset()
        tsProgressBar.Visible = False
        tsslStatus.BackColor = Color.Empty
        tsslStatus.Text = Nothing
        tsslStatus.Tag = Nothing
        tsslStatus.ToolTipText = Nothing
    End Sub

    Private Sub BeginValidateResponse(sender As Object, e As EventArgs)
        Reset()
    End Sub

    Private Sub ErrorReceived(sender As Object, e As app2.shared.ErrorEventArgs)
        HideProgress()
        Dim msg = e.Data
        If msg.Length > 50 Then
            msg = msg.Substring(0, 47) + "..."
        End If
        tsslStatus.BackColor = Color.Red
        tsslStatus.Text = msg
        tsslStatus.Tag = e.Data
        tsslStatus.ToolTipText = e.Data
    End Sub

    Private Sub tsslStatus_Click(sender As Object, e As EventArgs) Handles tsslStatus.Click
        If tsslStatus.Tag IsNot Nothing Then
            Dim msg As String = tsslStatus.Tag
            MessageBoxHelper.ShowErrorMessage(msg)
        End If
    End Sub

    Private Async Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Dim node = tvDriveExplorer.SelectedNode
        If node.Tag IsNot Nothing Then
            Dim file = CType(node.Tag, FileEntryInfo)
            Dim share = shares.FirstOrDefault(Function(p) p.FileShareId = file.FileShareId)

            ValidateIfShareExists(node, share)

            node.Text = file.FileVersion.FileEntryNameWithExtension + If(file.IsDeleted, " (Missing)", " (Refreshing...)")

            Await GetShareFoldersAsync(node, share, file)
        End If
    End Sub

    Private Sub cmsTree_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsTree.Opening
        Dim node = tvDriveExplorer.SelectedNode
        If node.Level <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Async Sub LogOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogOutToolStripMenuItem.Click


        Dim dialogResult = MessageBoxHelper.ShowQuestion("Are you sure you want to log out?")

        If dialogResult = DialogResult.Yes Then
            Using client = New LocalUserClient()
                Dim result = Await client.LogOutAsync()
                If ValidateResponse(result) Then

                    BroadcastMessage(MessageType.Action, ActionType.LogOut, ActionStatus.Completed, "user has logged out")

                    For Each item As ToolStripItem In tsmiUser.DropDownItems
                        item.Visible = False
                    Next

                    tsmiUser.Text = "Log In"

                    tvDriveExplorer.Nodes.Clear()
                    lvItems.Items.Clear()

                    AuthData.User = Nothing

                    DisableAllMenu()

                    AuthProvider.ValidateUser(sender)
                End If
            End Using
        End If
    End Sub

    Private Sub tsmiUser_Click(sender As Object, e As EventArgs) Handles tsmiUser.Click
        If AuthData.User Is Nothing Then
            AuthProvider.ValidateUser(sender)
        End If
    End Sub

    Private Async Sub CheckConnectivity()
        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(5)
            Dim isOnline As Boolean = False
            Try
                Dim response = Await client.GetAsync("https://google.com")
                If response.StatusCode = Net.HttpStatusCode.OK Then
                    isOnline = True
                End If
            Catch ex As Exception

            End Try
            If Me.InvokeRequired Then
                Me.Invoke(m_OnConnectionStateChanged, isOnline)
            Else
                UpdateConnectionState(isOnline)
            End If
        End Using
    End Sub

    Private Sub UpdateConnectionState(isOnline As Boolean)
        tsslConnectionStatus.Text = If(isOnline, "Online", "Offline")
        tsslConnectionStatus.Image = If(isOnline, fsconfig.My.Resources.circle_green_32x32, fsconfig.My.Resources.circle_red_32x32)
    End Sub

    Private Sub ConnectionStateTimerElapsed(sender As Object, e As ElapsedEventArgs)
        CheckConnectivity()
    End Sub

#End Region

    Private Sub msMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles msMain.ItemClicked
        If (AuthData.User Is Nothing) Then
            DisableAllMenu()
        End If
    End Sub

    Private Sub DisableAllMenu()
        'tsmiHome.Enabled = False
        'tsmiRefresh.Enabled = False
        'tsmiNewFolder.Enabled = False
        'tsmiCheckIn.Enabled = False
        'tsmiCheckout.Enabled = False
        'tsmiCheckout.Enabled = False
        'tsmiUndoCheckout.Enabled = False
        'tsmiConfig.Enabled = False

        'tsmiHistory.Enabled = False
        'tsmiProperties.Enabled = False
        'HistoryToolStripMenuItem.Enabled = False
        'tsmiLink.Enabled = False

        'tsmiClipboard.Enabled = False
        'tsmiCut.Enabled = False
        'tsmiCopy.Enabled = False
        'tsmiPaste.Enabled = False
        'tsmiDelete.Enabled = False

        'tsmiAdmin.Enabled = False
        'tsmiAgents.Enabled = False
        'tsmiShares.Enabled = False

        'tsmiReports.Enabled = False

        'txtbreadcrum.Text = ""
    End Sub

    Private Sub EnableAllMenu()
        ' DisableAllMenu()

        If AuthData.User.RoleId = Role.AccountAdmin Then
            tsmiAdmin.Enabled = True
            tsmiShares.Enabled = True
            tsmiAgents.Enabled = True
            tsmiReports.Enabled = True
        End If

        'Dim file As FileSystemEntryInfo = Nothing
        'If lvItems.SelectedItems.Count > 0 Then
        '    Dim selectedItem = lvItems.SelectedItems(0)
        '    If (selectedItem.Tag IsNot Nothing) Then
        '        file = CType(selectedItem.Tag, FileSystemEntryInfo)
        '    End If
        'Else
        '    Dim selectedItem = tvDriveExplorer.SelectedNode
        '    If (selectedItem IsNot Nothing AndAlso selectedItem.Tag IsNot Nothing) Then
        '        file = CType(selectedItem.Tag, FileSystemEntryInfo)
        '    End If
        'End If

        'If file IsNot Nothing Then
        '    If file.FileSystemEntryId <> Guid.Empty Then

        '        txtbreadcrum.Text = file.FileSystemEntryVersion.FileSystemEntryRelativePath
        '    End If
        'End If

        'ChangeContextMenuRightScreen(0)
    End Sub

    Private Sub EnableAllMenuOld()

        DisableAllMenu()

        If AuthData.User.RoleId = Role.AccountAdmin Then
            tsmiAdmin.Enabled = True
            tsmiShares.Enabled = True
            tsmiAgents.Enabled = True
            tsmiReports.Enabled = True
        End If


        Dim lvitemselected = False

        Dim file As FileEntryInfo = Nothing
        If lvItems.SelectedItems.Count > 0 Then
            lvitemselected = True
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        Else
            Dim selectedItem = tvDriveExplorer.SelectedNode
            If (selectedItem IsNot Nothing AndAlso selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        End If

        If file IsNot Nothing Then
            If file.FileEntryId <> Guid.Empty Then

                txtbreadcrum.Text = file.FileVersion.FileEntryRelativePath

                Dim isFile As Boolean = file.FileEntryTypeId = FileType.File
                Dim permissionProvider As PermissionProvider = New PermissionProvider(file)

                tsmiHome.Enabled = True

                tsmiRefresh.Enabled = True
                tsmiNewFolder.Enabled = True
                tsmiConfig.Enabled = True

                tsmiHistory.Enabled = True

                tsmiClipboard.Enabled = True

                If isFile Then
                    If (permissionProvider.CanUndoCheckOut() = True) Then
                        tsmiUndoCheckout.Enabled = True
                    End If
                    If (permissionProvider.CanViewHistory() = True) Then
                        tsmiHistory.Enabled = True
                    End If
                    If (permissionProvider.CanLinkFile() = True) Then
                        tsmiLink.Enabled = True
                    End If
                Else
                    If (permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                        tsmiPaste.Enabled = True
                    End If
                End If

                If (lvItems.SelectedItems.Count > 0) Then
                    If (permissionProvider.CanCopy() = True) Then
                        tsmiCut.Enabled = True
                    End If

                    If (permissionProvider.CanCopy() = True) Then
                        tsmiCopy.Enabled = True
                    End If

                    If (permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                        tsmiPaste.Enabled = True
                    End If

                    If (permissionProvider.CanSoftDelete() = True OrElse permissionProvider.CanHardDelete() = True) Then
                        tsmiDelete.Enabled = True
                    End If
                    isHardDelete = permissionProvider.CanHardDelete()
                End If

                If (permissionProvider.CanViewProperties() = True) Then
                    tsmiProperties.Enabled = True
                End If

            End If
        End If
    End Sub

    Private Function ValidateIfShareExists(node As TreeNode, share As ConfigInfo) As FileEntryInfo
        Dim exists As Boolean = True
        Using CommonUtils.Helper.Impersonate(share)
            exists = Directory.Exists(share.SharePath)
        End Using
        Dim rootNode = GetRootNode(node)
        Dim file As FileEntryInfo = rootNode.Tag
        If file IsNot Nothing Then
            file.IsDeleted = Not exists
            If file.IsDeleted Then
                DisableAllMenu()
                lvItems.Items.Clear()
                rootNode.Text = file.FileVersion.FileEntryNameWithExtension + " (Missing)"
                rootNode.Nodes.Clear()
            Else
                EnableAllMenu()
                Dim fileName As String = file.FileVersion.FileEntryNameWithExtension
                If file.FileEntryId = Guid.Empty Then
                    fileName = fileName + " (Loading...)"
                End If
                rootNode.Text = fileName
            End If
        End If
        Return file
    End Function

    Private Function GetRootNode(node As TreeNode) As TreeNode
        If node.Parent Is Nothing Then
            Return node
        End If
        Return GetRootNode(node.Parent)
    End Function


    Private Sub tsmiHome_DropDownOpening(sender As Object, e As EventArgs) Handles tsmiHome.DropDownOpening
        ChangeMainMenu()
    End Sub

    Private Sub tsmiHistory_DropDownOpening(sender As Object, e As EventArgs) Handles tsmiHistory.DropDownOpening
        ChangeMainMenu()
    End Sub

    Private Sub tsmiClipboard_DropDownOpening(sender As Object, e As EventArgs) Handles tsmiClipboard.DropDownOpening
        ChangeMainMenu()
    End Sub

    Public Sub ChangeMainMenu()

        Diagnostics.Debug.WriteLine("1111", "FileMinister")

        cmsMain.Items.Clear()
        Dim lvitemselected = False

        Dim file As FileEntryInfo = Nothing
        If lvItems.SelectedItems.Count > 0 Then
            lvitemselected = True
            Dim selectedItem = lvItems.SelectedItems(0)
            If (selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        Else
            Dim selectedItem = tvDriveExplorer.SelectedNode
            If (selectedItem IsNot Nothing AndAlso selectedItem.Tag IsNot Nothing) Then
                file = CType(selectedItem.Tag, FileEntryInfo)
            End If
        End If

        tsmiConfig.Enabled = True

        tsmiRefresh.Enabled = False
        tsmiNewFolder.Enabled = False
        tsmiCheckIn.Enabled = False
        tsmiCheckout.Enabled = False
        tsmiUndoCheckout.Enabled = False

        tsmiProperties.Enabled = False
        HistoryToolStripMenuItem.Enabled = False
        tsmiLink.Enabled = False

        tsmiCut.Enabled = False
        tsmiDelete.Enabled = False
        tsmiCopy.Enabled = False
        tsmiPaste.Enabled = False

        If (file IsNot Nothing AndAlso file.FileEntryId <> Guid.Empty) Then

            Dim isFile As Boolean = file.FileEntryTypeId = FileType.File
            Dim isFolder As Boolean = file.FileEntryTypeId = FileType.Folder
            Dim isShare As Boolean = file.FileEntryTypeId = FileType.Share

            Dim permissionProvider As PermissionProvider = New PermissionProvider(file)



            If (isFile) Then

                tsmiNewFolder.Enabled = False
                tsmiPaste.Enabled = False

                If (permissionProvider.CanUndoCheckOut() = True) Then
                    tsmiUndoCheckout.Enabled = True
                Else
                    tsmiUndoCheckout.Enabled = False
                End If

                If (permissionProvider.CanViewHistory() = True) Then
                    HistoryToolStripMenuItem.Enabled = True
                Else
                    HistoryToolStripMenuItem.Enabled = False
                End If

                If (permissionProvider.CanLinkFile() = True) Then
                    tsmiLink.Enabled = True
                Else
                    tsmiLink.Enabled = False
                End If

            Else

                If (isFolder) Then
                    If (permissionProvider.CanViewHistory() = True) Then
                        HistoryToolStripMenuItem.Enabled = True
                    Else
                        HistoryToolStripMenuItem.Enabled = False
                    End If
                End If

                tsmiRefresh.Enabled = True

                If (permissionProvider.CanCreate() = True) Then
                    If (file.FileVersion.VersionNumber IsNot Nothing OrElse isShare) Then
                        tsmiNewFolder.Enabled = True
                    End If
                End If

                If (permissionProvider.CanPaste() = True And ((System.Windows.Forms.Clipboard.GetFileDropList() IsNot Nothing And System.Windows.Forms.Clipboard.GetFileDropList().Count() > 0) Or (cutCopyList.Count() > 0))) Then
                    tsmiPaste.Enabled = True
                Else
                    tsmiPaste.Enabled = False
                End If
            End If

            If (lvitemselected) Then
                If (permissionProvider.CanCopy() = True) Then
                    tsmiCut.Enabled = True
                Else
                    tsmiCut.Enabled = False
                End If

                If (permissionProvider.CanCopy() = True) Then
                    tsmiCopy.Enabled = True
                Else
                    tsmiCopy.Enabled = False
                End If

                If (permissionProvider.CanSoftDelete() = True OrElse permissionProvider.CanHardDelete() = True) Then
                    tsmiDelete.Enabled = True
                Else
                    tsmiDelete.Enabled = False
                End If
                isHardDelete = permissionProvider.CanHardDelete()
            End If

            If (permissionProvider.CanViewProperties() = True) Then
                tsmiProperties.Enabled = True
            Else
                tsmiProperties.Enabled = False
            End If

        End If

    End Sub

    Private Sub BroadcastMessage(messageType As MessageType, actionType As ActionType, actionStatus As ActionStatus, messageStr As String)
        If MessageClientProvider.Instance IsNot Nothing Then
            Dim message As New MessageInfo()
            With message
                .MessageType = messageType
                .ActionType = actionType
                .ActionStatus = actionStatus
                .Message = messageStr
                .User = AuthData.User
            End With
            MessageClientProvider.Instance.BroadcastMessage(message)
        End If
    End Sub

    Private Sub tsmiUser_DropDownOpening(sender As Object, e As EventArgs) Handles tsmiUser.DropDownOpening
        tsmiUser.DropDownItems.Clear()
        If AuthData.User IsNot Nothing Then
            tsmiUser.DropDownItems.Add(tsmiSwitchUser)
            tsmiUser.DropDownItems.Add(tsmiSwitchAccount)
            tsmiUser.DropDownItems.Add(tsmiAddAgent)
            tsmiUser.DropDownItems.Add(LogOutToolStripMenuItem)
        End If
    End Sub
End Class
