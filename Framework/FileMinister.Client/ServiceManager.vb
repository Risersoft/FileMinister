Imports FileMinister.Client.Service
Imports FileMinister.Client.SyncService
Imports risersoft.shared.portable.Model
Imports risersoft.shared.messaging
Imports risersoft.shared.web

Public Class ServiceManager
    Private Shared shares As Concurrent.BlockingCollection(Of LocalConfigInfo)
    Private Shared service As ServiceProvider(Of Startup) = Nothing
    Private Shared messageService As MessagingService
    Public Shared Sub Start()

        Globals.myApp = New clsWebApiApp(New clsAppExtenderFS)
        ConfigureLoggers()

        service = ServiceProvider(Of Startup).GetProvider()
        service.Start(FileMinister.Client.Common.Constants.LOCAL_SERVER_URI)

        messageService = New MessagingService()

        Dim messagingClient = MessageClientProvider.CreateInstance("win_service", Initiator.WindowService)
        AddHandler messagingClient.OnShowMessage, AddressOf OnMessageReceived

        FileMinisterService.GetInstance().StartService()
        AddHandler FileMinisterService.GetInstance().OnDemandSyncFinished, AddressOf FileSyncCompleted
        AddHandler FileSyncModule.GetInstance().FileDownloaded, AddressOf FileDowloaded
        AddHandler FileSyncModule.GetInstance().FileUploaded, AddressOf FileUploaded 'check in case
        AddHandler FileSyncModule.GetInstance().FileCheckedOut, AddressOf FileCheckedOut 'check in case
        AddHandler FileSyncModule.GetInstance().FileMoveDownloadedFinishedForShare, AddressOf ShareDownloadComplete
        AddHandler FileSyncModule.GetInstance().FileUploadFinishedForShare, AddressOf FileUploadFinishedForShare

        StartWatchers()
    End Sub

    Private Shared Sub StartWatchers()
        ConfigureWatchers()
        ''AddHandler FileMinisterFileSystemWatcher.Instance.Changed, AddressOf OnFileChanged
        ''AddHandler FileMinisterFileSystemWatcher.Instance.Created, AddressOf OnFileCreated
        ''AddHandler FileMinisterFileSystemWatcher.Instance.Deleted, AddressOf OnFileDeleted
        ''AddHandler FileMinisterFileSystemWatcher.Instance.Error, AddressOf OnFileError
        ''AddHandler FileMinisterFileSystemWatcher.Instance.Renamed, AddressOf OnFileRenamed

        'AddHandler TimerPrivider.Instance.TimeElapsed, AddressOf TimeElapsed

    End Sub

    Private Shared Sub OnMessageReceived(sender As Object, e As InboundMessageHandler.ShowMessageEventArgs)
        Dim message = e.message
        If message.Initiator <> Initiator.WindowService Then
            If (message.ActionType = ActionType.LogIn OrElse message.ActionType = ActionType.LogOut OrElse message.ActionType = ActionType.ConfigureShare) AndAlso message.ActionStatus = ActionStatus.Completed Then
                ConfigureWatchers()
            Else
                If message.MessageType = MessageType.Action AndAlso message.ActionStatus = ActionStatus.None Then
                    If message.ActionType = ActionType.SyncData Then
                        FileHelper.SyncData(e.message)
                    ElseIf message.ActionType = ActionType.CheckOut Then
                        Dim share = shares.FirstOrDefault(Function(p) p.UserAccountId = e.message.User.UserAccountId)
                        If share IsNot Nothing Then
                            FileHelper.CheckOutFile(share, e.message)
                        End If
                    ElseIf message.ActionType = ActionType.UndoCheckout Then
                        Dim share = shares.FirstOrDefault(Function(p) p.UserAccountId = e.message.User.UserAccountId)
                        If share IsNot Nothing Then
                            FileHelper.UndoCheckOutFile(share, e.message)
                        End If
                    ElseIf message.ActionType = ActionType.CheckIn Then
                        Dim share = shares.FirstOrDefault(Function(p) p.UserAccountId = e.message.User.UserAccountId)
                        If share IsNot Nothing Then
                            FileHelper.CheckInFile(share, e.message)
                        End If
                    ElseIf message.ActionType = ActionType.UndoDelete Then
                        Dim share = shares.FirstOrDefault(Function(p) p.UserAccountId = e.message.User.UserAccountId)
                        If share IsNot Nothing Then
                            FileHelper.UndoDelete(share, e.message)
                        End If
                    End If
                End If

            End If
        End If
    End Sub

    Private Shared Sub ConfigureWatchers()
        ''Dim fileSystemWatcherInstance = FileMinisterFileSystemWatcher.Instance
        shares = New Concurrent.BlockingCollection(Of LocalConfigInfo)()
        Using configClient As New LocalShareClient()
            Dim configResult = configClient.GetAccountShares()
            If configResult.Status = 200 Then
                Dim configurations = configResult.Data
                If configurations IsNot Nothing Then
                    For Each share In configurations
                        shares.Add(share)
                    Next
                    ''fileSystemWatcherInstance.ConfigureWatchers(configurations)
                    'TimerPrivider.Instance.ConfigureTimers(configurations)
                End If
            End If
        End Using

        'TimerPrivider.Instance.Start()
    End Sub

    'Private Shared Sub OnFileChanged(sender As Object, e As FileSystemEventArgs)
    '    If FIleHelper.IsValidFile(e.FullPath) Then
    '        Console.WriteLine("OnFileChanged: {0}", e.FullPath)
    '        Dim obj = CType(sender, FileSystemWatcherBase)
    '        FIleHelper.Save(obj, e.FullPath, e.Name, Nothing)
    '    End If
    'End Sub

    'Private Shared Sub OnFileCreated(sender As Object, e As FileSystemEventArgs)
    '    If FIleHelper.IsValidFile(e.FullPath) Then
    '        Console.WriteLine("OnFileCreated: {0}", e.FullPath)
    '        Dim obj = CType(sender, FileSystemWatcherBase)
    '        FIleHelper.Save(obj, e.FullPath, e.Name, Nothing)

    '        Dim message As MessageInfo = New MessageInfo() With {
    '                .MessageType = MessageType.Action,
    '                .ActionType = ActionType.FileCreated,
    '                .Message = "File created"
    '            }

    '        MessageClientProvider.Instance.BroadcastMessage(message)

    '    End If
    'End Sub

    'Private Shared Sub OnFileDeleted(sender As Object, e As FileSystemEventArgs)
    '    If FIleHelper.IsValidFile(e.FullPath) Then
    '        Console.WriteLine("OnFileDeleted: {0}", e.FullPath)
    '        Dim obj = CType(sender, FileSystemWatcherBase)
    '        FIleHelper.Delete(obj, e.Name)
    '    End If
    'End Sub

    'Private Shared Sub OnFileError(sender As Object, e As ErrorEventArgs)

    'End Sub

    'Private Shared Sub OnFileRenamed(sender As Object, e As RenamedEventArgs)
    '    If Util.Helper.IsValidFile(e.FullPath) Then

    '        Dim effectiveName As String = e.OldName
    '        If Not Util.Helper.IsValidFile(e.OldName) Then
    '            effectiveName = e.Name
    '        End If
    '        Console.WriteLine("OnFileRenamed: New: {0}, Old: {1}", e.FullPath, e.OldFullPath)
    '        Dim obj = CType(sender, FileSystemWatcherBase)
    '        FIleHelper.Save(obj, e.FullPath, e.Name, effectiveName)
    '    End If
    'End Sub

    Private Shared Sub TimeElapsed(timer As FileMinisterTimer)

        'Using client As New LocalSyncClient(timer.ShareInfo.User)
        '    Dim result = client.UpdateFileSystemCache(timer.ShareInfo, 10)
        '    If (Not result.Data AndAlso result.Status = FileMinister.Common.Status.PathNotValid) Then
        '        Throw New Exception("Share Missing")
        '    End If
        'End Using

        'FileMinisterService.GetInstance().DirectorySync(FileBackgroundWorkerArguments:=New FileMinisterService.FileBackgroundWorkerArguments With {
        '                                             .IsRunInServiceMode = True,
        '                                             .UserAccountId = timer.ShareInfo.UserAccountId,
        '                                             .AccountId = timer.ShareInfo.AccountId,
        '                                             .LocalDatabaseName = timer.ShareInfo.User.LocalDatabaseName,
        '                                             .AccessToken = Common.Util.Helper.Decrypt(timer.ShareInfo.User.AccessToken),
        '                                             .UserId = timer.ShareInfo.User.Id,
        '                                             .ShareId = timer.ShareInfo.ShareId,
        '                                             .SharePath = timer.ShareInfo.SharePath,
        '                                             .WindowsUser = timer.ShareInfo.WindowsUser,
        '                                             .WindowsUserName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(1)),
        '                                             .DomainName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(0)),
        '                                             .Password = Client.Common.Util.Helper.Decrypt(timer.ShareInfo.Password),
        '                                             .OnDemandSyncFileSystemEntryId = Nothing,
        '                                             .RoleId = CInt(timer.ShareInfo.User.Role),
        '.CloudSyncServiceURL = timer.ShareInfo.User.SyncServiceUrl
        '                                         })
        'Dim DoWorkEventArgs = New DoWorkEventArgs(argument:=New FileMinisterService.FileBackgroundWorkerArguments With {
        '                                                                                                                .IsRunInServiceMode = True,
        '                                                                                                                .UserAccountId = timer.ShareInfo.UserAccountId,
        '                                                                                                                .AccountId = timer.ShareInfo.AccountId,
        '                                                                                                                .LocalDatabaseName = timer.ShareInfo.User.LocalDatabaseName,
        '                                                                                                                .AccessToken = Common.Util.Helper.Decrypt(timer.ShareInfo.User.AccessToken),
        '                                                                                                                .WorkSpaceId = timer.ShareInfo.User.WorkSpaceId,
        '                                                                                                                .UserId = timer.ShareInfo.User.Id,
        '                                                                                                                .ShareId = timer.ShareInfo.ShareId,
        '                                                                                                                .SharePath = timer.ShareInfo.SharePath,
        '                                                                                                                .WindowsUser = timer.ShareInfo.WindowsUser,
        '                                                                                                                .WindowsUserName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(1)),
        '                                                                                                                .DomainName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(0)),
        '                                                                                                                .Password = Client.Common.Util.Helper.Decrypt(timer.ShareInfo.Password)
        '                                                                                                            })
        'FileMinisterService.GetInstance().ResolveConflictsResolvedOnServer(Nothing, DoWorkEventArgs)
        'FileMinisterService.GetInstance().BackgroundWorkerFileSyncMoveDownloaded_DoWork(Sender:=Nothing, DoWorkEventArgs:=DoWorkEventArgs)
    End Sub

    Private Shared Sub FileSyncCompleted(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
        SendFileSyncMessage(UserAccountId, ShareId, FileSystemEntryId, ActionType.SyncData, ActionStatus.Completed)
    End Sub

    Private Shared Sub FileDowloaded(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
        SendFileSyncMessage(UserAccountId, ShareId, FileSystemEntryId, ActionType.DownloadFiles, ActionStatus.Completed)
    End Sub

    Private Shared Sub FileUploaded(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
        SendFileSyncMessage(UserAccountId, ShareId, FileSystemEntryId, ActionType.CheckIn, ActionStatus.Completed)
    End Sub

    Private Shared Sub FileCheckedOut(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid, IsCheckedOutByDifferentUser As Boolean)
        Dim dic As New Dictionary(Of String, Object)()
        dic.Add("IsCheckedOutByDifferentUser", IsCheckedOutByDifferentUser)
        SendFileSyncMessage(UserAccountId, ShareId, FileSystemEntryId, ActionType.CheckOut, ActionStatus.Completed, otherData:=dic)
    End Sub

    Private Shared Sub ShareDownloadComplete(UserAccountId As Integer, ShareId As Integer)
        SendFileSyncMessage(UserAccountId, ShareId, Nothing, ActionType.DownloadShare, ActionStatus.Completed)
    End Sub

    Private Shared Sub FileUploadFinishedForShare(UserAccountId As Integer, ShareId As Integer)
        SendFileSyncMessage(UserAccountId, ShareId, Nothing, ActionType.UploadShare, ActionStatus.Completed)
    End Sub

    Private Shared Sub SendFileSyncMessage(userAccountId As Integer, shareId As Integer, fileSystemEntryId As Guid?, actionType As ActionType, actionStatus As ActionStatus, Optional msg As String = Nothing, Optional otherData As Dictionary(Of String, Object) = Nothing)
        If shares IsNot Nothing Then
            Dim share = shares.FirstOrDefault(Function(p) p.UserAccountId = userAccountId AndAlso p.FileShareId = shareId)

            If share IsNot Nothing Then
                Dim message As New MessageInfo()
                Dim data = message.Data
                If fileSystemEntryId.HasValue Then
                    data.Add("FileSystemEntryId", fileSystemEntryId)
                End If

                If otherData IsNot Nothing Then
                    For Each item In otherData
                        data.Add(item)
                    Next
                End If

                With message
                    .Share = share
                    .User = share.User
                    .MessageType = MessageType.Action
                    .ActionType = actionType
                    .ActionStatus = actionStatus
                    .Message = msg
                End With
                MessageClientProvider.Instance.BroadcastMessage(message)
            End If
        End If
    End Sub

    Public Shared Sub [Stop]()

        'Dim file As System.IO.StreamWriter
        'file = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
        'file.WriteLine("STOP 1 ")
        'file.Close()

        Try
            TimerPrivider.Instance.Stop()
            FileMinisterService.GetInstance().StopService()
            'Dim file1 As System.IO.StreamWriter
            'file1 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file1.WriteLine("STOP 11")
            'file1.Close()
        Catch ex As Exception
            'Dim file1 As System.IO.StreamWriter
            'file1 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file1.WriteLine("ERROR 1 ")
            'file1.Close()
        End Try

        Try
            If service IsNot Nothing Then
                service.Stop()
            End If
            'Dim file1 As System.IO.StreamWriter
            'file1 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file1.WriteLine("STOP 2")
            'file1.Close()
        Catch ex As Exception
            'Dim file2 As System.IO.StreamWriter
            'file2 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file2.WriteLine("ERROR 2 ")
            'file2.Close()
        End Try

        Try
            If messageService IsNot Nothing Then
                messageService.Close()
            End If
            'Dim file1 As System.IO.StreamWriter
            'file1 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file1.WriteLine("STOP 3")
            'file1.Close()
        Catch ex As Exception
            'Dim file3 As System.IO.StreamWriter
            'file3 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file3.WriteLine("ERROR 3 ")
            'file3.Close()
        End Try

        Try
            If MessageClientProvider.Instance IsNot Nothing Then
                MessageClientProvider.Instance.Unregister()
            End If
            'Dim file1 As System.IO.StreamWriter
            'file1 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file1.WriteLine("STOP 4")
            'file1.Close()
        Catch ex As Exception
            'Dim file4 As System.IO.StreamWriter
            'file4 = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
            'file4.WriteLine("ERROR 4 ")
            'file4.Close()
        End Try


    End Sub
    Public Shared Sub ConfigureLoggers()
        LogMaster.Configure("SyncService")
        LogMaster.Configure("SyncCommand")
        LogMaster.Configure("DatabaseSync")
        LogMaster.Configure("FileSync")
        LogMaster.Configure("CheckInProcess")
        LogMaster.Configure("CheckOutProcess")
        LogMaster.Configure("FileWatcher")

    End Sub

End Class
