Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports risersoft.shared.messaging

Public Class Form1

    Dim proxy As MessagingClient
    Private Delegate Sub TakeActionOnMessageHandler(message As MessageInfo)
    Dim m_takeActionOnMessage As TakeActionOnMessageHandler = AddressOf TakeActionOnMessage


    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Init()

    End Sub

    Private Sub Init()
        Task.Run(Sub()
                     Threading.Thread.Sleep(60 * 1000)
                     RegisterMessaging()
                 End Sub)
    End Sub

    Protected Overrides Sub SetVisibleCore(ByVal value As Boolean)
        If Not Me.IsHandleCreated Then
            Me.CreateHandle()
            value = False
        End If
        MyBase.SetVisibleCore(value)
    End Sub

    Private Sub NotifyIcon1_DoubleClick(sender As Object, e As EventArgs) Handles NotifyIcon1.DoubleClick
        Show()
        WindowState = FormWindowState.Normal
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If (FormWindowState.Minimized = WindowState) Then
            Hide()
        End If
    End Sub

#Region "Messaging"

    Private Sub RegisterMessaging()
        If MessageClientProvider.Instance Is Nothing Then
            Dim winUser = Environment.UserDomainName + "\" + Environment.UserName
            Dim messageClientName As String = String.Format("WU{0}_tray_app", winUser.Replace("\", "_"))
            Dim messagingClient = MessageClientProvider.CreateInstance(messageClientName, Initiator.TrayApplication)

            AddHandler messagingClient.OnShowMessage, AddressOf OnMessageReceived
        End If
    End Sub

    Private Sub OnMessageReceived(sender As Object, e As InboundMessageHandler.ShowMessageEventArgs)

        Dim strMsg = Newtonsoft.Json.JsonConvert.SerializeObject(e.message)
        Diagnostics.Debug.WriteLine(strMsg, "FileMinister Tray")

        Dim message = e.message
        If message.ActionStatus = ActionStatus.Completed OrElse message.ActionStatus = ActionStatus.Error Then
            If (message.ActionType = ActionType.CheckIn OrElse message.ActionType = ActionType.CheckOut OrElse message.ActionType = ActionType.Conflict) Then
                Dim winUser = Environment.UserDomainName + "\" + Environment.UserName
                If message.User.WindowsUserSId.Equals(winUser, StringComparison.InvariantCultureIgnoreCase) Then
                    If Me.InvokeRequired Then
                        Me.Invoke(m_takeActionOnMessage, message)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub TakeActionOnMessage(message As MessageInfo)
        If message.ActionStatus = ActionStatus.Completed Then

            If (message.ActionType = ActionType.CheckIn Or message.ActionType = ActionType.CheckOut Or message.ActionType = ActionType.Conflict) Then

                Dim notification = ParseMessage(message)

                NotifyIcon1.Visible = False

                NotifyIcon1.Visible = True
                NotifyIcon1.BalloonTipIcon = notification.NotificationType
                NotifyIcon1.BalloonTipTitle = notification.Title
                NotifyIcon1.BalloonTipText = notification.Message

                NotifyIcon1.ShowBalloonTip(0)

            End If
        End If
    End Sub

    Private Function ParseMessage(message As MessageInfo) As NotificationInfo
        'Dim outputStr(1) As String

        Dim notification As New NotificationInfo()
        notification.NotificationType = ToolTipIcon.Info
        Select Case message.ActionType
            'Case ActionType.UploadFiles
            '    Dim fs = GetFileById(message)
            '    outputStr(0) = "Uploaded Successfully"
            '    If (fs IsNot Nothing AndAlso fs.FileSystemEntryVersion IsNot Nothing) Then
            '        outputStr(1) = fs.FileSystemEntryVersion.FileSystemEntryNameWithExtension + " Uploaded Successfully"
            '    Else
            '        outputStr(1) = "Uploaded Successfully"
            '    End If
            '    Exit Select
            'Case ActionType.DownloadFiles
            '    Dim fs = GetFileById(message)
            '    outputStr(0) = "Downloaded Successfully"
            '    If (fs IsNot Nothing AndAlso fs.FileSystemEntryVersion IsNot Nothing) Then
            '        outputStr(1) = fs.FileSystemEntryVersion.FileSystemEntryNameWithExtension + " Downloaded Successfully"
            '    Else
            '        outputStr(1) = "Downloaded Successfully"
            '    End If
            '    Exit Select

            Case ActionType.CheckIn
                Dim fs = GetFileById(message)
                notification.Title = "Checked In Successfully"
                If (fs IsNot Nothing AndAlso fs.FileVersion IsNot Nothing) Then
                    notification.Message = fs.FileVersion.FileEntryNameWithExtension + " Checked In Successfully"
                Else
                    notification.Message = "Checked In Successfully"
                End If
                Exit Select
            Case ActionType.CheckOut
                Dim fs = GetFileById(message)

                Dim isCheckedOutByDifferentUser As Boolean = False
                If message.Data.ContainsKey("IsCheckedOutByDifferentUser") Then
                    isCheckedOutByDifferentUser = message.Data("IsCheckedOutByDifferentUser")
                End If

                If isCheckedOutByDifferentUser Then
                    notification.NotificationType = ToolTipIcon.Error
                    notification.Title = "Checked Out By Different User"
                    If (fs IsNot Nothing AndAlso fs.FileVersion IsNot Nothing) Then
                        notification.Message = fs.FileVersion.FileEntryNameWithExtension + " Checked Out By Different User"
                    Else
                        notification.Message = "Checked Out By Different User"
                    End If
                Else
                    notification.Title = "Checked Out Successfully"
                    If (fs IsNot Nothing AndAlso fs.FileVersion IsNot Nothing) Then
                        notification.Message = fs.FileVersion.FileEntryNameWithExtension + " Checked Out Successfully"
                    Else
                        notification.Message = "Checked Out Successfully"
                    End If
                End If
                Exit Select
            Case ActionType.Conflict
                Dim fs = GetFileById(message)
                notification.NotificationType = ToolTipIcon.Error
                notification.Title = "Conflict Occurred"
                If (fs IsNot Nothing AndAlso fs.FileVersion IsNot Nothing) Then
                    notification.Message = fs.FileVersion.FileEntryNameWithExtension + " is in conflict"
                Else
                    notification.Message = "Conflict Occurred"
                End If
                Exit Select

                'Case ActionType.UndoCheckout
                '    Dim fs = GetFileById(message)
                '    outputStr(0) = "UndoCheckedOut Successfully"
                '    If (fs IsNot Nothing AndAlso fs.FileSystemEntryVersion IsNot Nothing) Then
                '        outputStr(1) = fs.FileSystemEntryVersion.FileSystemEntryNameWithExtension + " UndoCheckedOut Successfully"
                '    Else
                '        outputStr(1) = "UndoCheckedOut Successfully"
                '    End If
                '    Exit Select
                'Case ActionType.ConfigureShare
                '    outputStr(0) = "Share Configured"
                '    outputStr(1) = "Share Configured"
                '    Exit Select
                'Case ActionType.DownloadShare
                '    outputStr(0) = "Share Downloaded"
                '    outputStr(1) = message.Share.ShareName + " Downloaded Successfully"
                '    Exit Select
                'Case ActionType.UploadShare
                '    outputStr(0) = "Share Uploaded"
                '    outputStr(1) = message.Share.ShareName + " Uploaded Successfully"
                '    Exit Select
                'Case ActionType.None
                '    Exit Select
                'Case ActionType.SyncData
                '    outputStr(0) = "Sync Data"
                '    outputStr(1) = "Sync Data"
                '    Exit Select
                'Case ActionType.UndoDelete
                '    outputStr(0) = "Undo Delete"
                '    outputStr(1) = "Undo Delete"
                '    Exit Select
                'Case ActionType.SyncUserDetail
                '    outputStr(0) = "Sync User Details"
                '    outputStr(1) = "Sync User Details"
                '    Exit Select
        End Select

        Return notification
    End Function

    Public Function GetFileById(message As MessageInfo) As FileEntryInfo
        Using client As New FileMinister.Client.Common.LocalFileClient(message.User)
            Dim r = client.GetFile(message.Data("FileSystemEntryId"))
            Dim fs = r.Data
            Return fs
        End Using
    End Function

    Private Sub SendFileActionMessage(actionType As ActionType)
        Dim message As New MessageInfo()
        With message
            .MessageType = MessageType.Action
            .ActionType = actionType
            '.User = AuthData.User
        End With
        MessageClientProvider.Instance.BroadcastMessage(message)
    End Sub

#End Region



    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick

        cmsMain.Items.Clear()

        If e.Button = Windows.Forms.MouseButtons.Right Then

            Me.Show() 'Shows the Form that is the parent of "traymenu"
            Me.Activate() 'Set the Form to "Active", that means that that will be the "selected" window
            Me.Width = 1 'Set the Form width to 1 pixel, that is needed because later we will set it behind the "traymenu"
            Me.Height = 1 'Set the Form Height to 1 pixel, for the same reason as above

            Dim tsmiSettings As New ToolStripMenuItem("Open Cloud Sync Settings")
            tsmiSettings.Tag = CommandType.CloudSettings
            cmsMain.Items.Add(tsmiSettings)

            Dim tsmiUI As New ToolStripMenuItem("Open Cloud Sync UI")
            tsmiUI.Tag = CommandType.CloudUI
            cmsMain.Items.Add(tsmiUI)

            Dim tsmiExit As New ToolStripMenuItem("Exit")
            tsmiExit.Tag = CommandType.TrayExit
            cmsMain.Items.Add(tsmiExit)

            cmsMain.Show(Cursor.Position)
        End If
    End Sub

    Private Sub CmsMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles cmsMain.ItemClicked
        cmsMain.Visible = False
        Dim commandType__1 = If(e.ClickedItem.Tag IsNot Nothing, DirectCast(e.ClickedItem.Tag, CommandType), CommandType.None)
        Select Case commandType__1
            Case CommandType.CloudSettings
                RunUIProcess()
                SendFileActionMessage(ActionType.OpenConfig)
                Exit Select
            Case CommandType.CloudUI
                RunUIProcess()
                Exit Select
            Case CommandType.TrayExit
                MessageClientProvider.Instance.Unregister()
                Me.Close()
                Exit Select
        End Select
    End Sub

    Private Sub RunUIProcess()
        Dim installationPath As String = Environment.GetEnvironmentVariable("FileMinister", EnvironmentVariableTarget.Machine)
        Dim filePath As String = installationPath + "\" + "FileMinister.Client.WinApp.exe"
        Process.Start(filePath)
    End Sub

    Private Class NotificationInfo
        Public Property Title As String
        Public Property Message As String
        Public Property NotificationType As ToolTipIcon
    End Class

End Class
