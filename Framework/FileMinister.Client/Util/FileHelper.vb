Imports risersoft.shared.portable.Model
Imports FileMinister.Client.SyncService
Imports risersoft.shared.messaging
Imports risersoft.shared.portable.Enums

Public Class FileHelper
    Friend Shared Sub SyncData(message As MessageInfo)
        Dim data = message.Data
        If data IsNot Nothing Then
            Dim fileSystemEntryId As Guid = CType(data("FileSystemEntryId"), Guid)
            If fileSystemEntryId <> Guid.Empty Then

                Task.Run(Sub()
                             FileMinisterService.GetInstance().StartOnDemandSync(message.Share.UserAccountId, message.Share.FileShareId, fileSystemEntryId)
                         End Sub)

            End If
        End If
    End Sub

    Friend Shared Sub CheckOutFile(share As LocalConfigInfo, message As MessageInfo)
        Dim data = message.Data
        If data IsNot Nothing Then
            Dim fileSystemEntryId As Guid = CType(data("FileSystemEntryId"), Guid)
            If fileSystemEntryId <> Guid.Empty Then
                Dim currentVersionNumber As Integer = CType(data("CurrentVersionNumber"), Integer)
                Task.Run(Sub()
                             Dim user = message.User
                             Dim uploadProcess As New UploadProcess(share.FileShareId, user)
                             Dim result = uploadProcess.CheckoutFile(fileSystemEntryId, currentVersionNumber)

                             Dim actionType As ActionType = ActionType.CheckOut
                             Dim actionStatus As ActionStatus = ActionStatus.None
                             Dim msg As String = Nothing
                             If result.Status = Status.Success AndAlso result.Data Then
                                 actionStatus = ActionStatus.Completed
                             Else
                                 actionStatus = ActionStatus.Error
                                 msg = result.Message
                             End If
                             SendMessage(user, share, actionType, actionStatus, result.Status, message.Data, msg)
                         End Sub)

            End If
        End If
    End Sub

    Friend Shared Sub CheckInFile(share As LocalConfigInfo, message As MessageInfo)
        Dim data = message.Data
        If data IsNot Nothing Then
            Dim fileSystemEntryVersionId As Guid = CType(data("FileSystemEntryVersionId"), Guid)
            If fileSystemEntryVersionId <> Guid.Empty Then
                Dim versionNumber As Integer = CType(data("CurrentVersionNumber"), Integer)
                Task.Run(Sub()
                             Dim user = message.User
                             Dim uploadProcess As New UploadProcess(share.FileShareId, user)
                             Using client As New LocalFileVersionClient(user)
                                 Dim fileResult = client.Get(Of FileVersionInfo)(id:=fileSystemEntryVersionId)
                                 If fileResult.Status = Status.Success Then

                                     Dim result = uploadProcess.CheckInFile(fileResult.Data, versionNumber)

                                     Dim actionType As ActionType = ActionType.CheckIn
                                     Dim actionStatus As ActionStatus = ActionStatus.None
                                     Dim msg As String = Nothing
                                     If result.Status = Status.Success AndAlso result.Data Then
                                         actionStatus = ActionStatus.Completed
                                     Else
                                         actionStatus = ActionStatus.Error
                                         msg = result.Message
                                     End If
                                     SendMessage(user, share, actionType, actionStatus, result.Status, message.Data, msg)
                                 End If
                             End Using
                         End Sub)

            End If
        End If
    End Sub

    Friend Shared Sub UndoDelete(share As LocalConfigInfo, message As MessageInfo)
        Dim data = message.Data
        If data IsNot Nothing Then
            Dim fileSystemEntryId As Guid = CType(data("FileSystemEntryId"), Guid)
            If fileSystemEntryId <> Guid.Empty Then
                Task.Run(Sub()
                             Dim user = message.User
                             Dim uploadProcess As New UploadProcess(share.FileShareId, user)
                             Dim result = uploadProcess.UndoDelete(fileSystemEntryId)
                             Dim actionType As ActionType = ActionType.UndoDelete
                             Dim actionStatus As ActionStatus = ActionStatus.None
                             Dim msg As String = Nothing
                             If result.Status = Status.Success AndAlso result.Data Then
                                 actionStatus = ActionStatus.Completed
                             Else
                                 actionStatus = ActionStatus.Error
                                 msg = result.Message
                             End If
                             SendMessage(user, share, actionType, actionStatus, result.Status, message.Data, msg)

                         End Sub)

            End If
        End If
    End Sub

    Friend Shared Sub UndoCheckOutFile(share As LocalConfigInfo, message As MessageInfo)
        Dim data = message.Data
        If data IsNot Nothing Then
            Dim fileSystemEntryId As Guid = CType(data("FileSystemEntryId"), Guid)
            If fileSystemEntryId <> Guid.Empty Then
                Task.Run(Sub()
                             Dim user = message.User
                             Dim uploadProcess As New UploadProcess(share.FileShareId, user)
                             Dim result = uploadProcess.UndoCheckoutFile(fileSystemEntryId)

                             Dim actionType As ActionType = ActionType.UndoCheckout
                             Dim actionStatus As ActionStatus = ActionStatus.None
                             Dim msg As String = Nothing
                             If result.Status = Status.Success AndAlso result.Data Then
                                 actionStatus = ActionStatus.Completed
                             Else
                                 actionStatus = ActionStatus.Error
                                 msg = result.Message
                             End If
                             SendMessage(user, share, actionType, actionStatus, result.Status, message.Data, msg)
                         End Sub)

            End If
        End If
    End Sub

    Private Shared Sub SendMessage(user As LocalWorkSpaceInfo, share As LocalConfigInfo, actionType As ActionType, actionStatus As ActionStatus, status As Status, Optional data As IDictionary(Of String, Object) = Nothing, Optional msg As String = Nothing)
        If share IsNot Nothing Then
            Dim message As New MessageInfo()
            message.Data = data
            With message
                .Share = share
                .User = user
                .MessageType = MessageType.Action
                .ActionType = actionType
                .ActionStatus = actionStatus
                .Status = status
                .Message = msg
            End With
            MessageClientProvider.Instance.BroadcastMessage(message)
        End If
    End Sub

    'Public Shared Function Save(obj As FileSystemWatcherBase, fullPath As String, filename As String, oldFilename As String) As FileVersionInfo
    '    SyncLock obj

    '        Dim FileVersionInfo As New FileVersionInfo()
    '        Dim fileType As FileType = fileType.File
    '        Dim file As New IO.FileInfo(fullPath)
    '        Dim hash As String = String.Empty
    '        Dim fileLength As Long = 0
    '        If (file.Attributes And IO.FileAttributes.Directory) = IO.FileAttributes.Directory Then
    '            fileType = fileType.Folder
    '        Else
    '            fileLength = file.Length
    '            hash = HashCalculator.hash_generator("md5", fullPath)
    '        End If
    '        Dim status As FileSystemEntryStatus = FileSystemEntryStatus.NewModified
    '        If (oldFilename IsNot Nothing) Then
    '            status = FileSystemEntryStatus.MoveOrRename
    '        End If

    '        With FileVersionInfo
    '            .FileSystemEntryExtension = IO.Path.GetExtension(filename)
    '            .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(filename)
    '            .FileSystemEntryRelativePath = "\" + filename
    '            .FileSystemEntryOldRelativePath = "\" + If(oldFilename Is Nothing, filename, oldFilename)
    '            .FileSystemEntrySize = fileLength
    '            .FileSystemEntryHash = hash
    '            '.ServerFileSystemEntryName =
    '            .FileSystemEntry = New FileSystemEntryInfo() With {
    '                    .ShareId = obj.ShareInfo.ShareId,
    '                    .FileSystemEntryTypeId = fileType
    '                }
    '            .DeltaOperation = New DeltaOperationInfo() With {
    '                    .FileSystemEntryStatusId = status,
    '                    .LocalFileSystemEntryExtension = IO.Path.GetExtension(filename),
    '                    .LocalFileSystemEntryName = IO.Path.GetFileNameWithoutExtension(filename),
    '                    .LocalCreatedOnUTC = file.LastWriteTimeUtc
    '                }
    '        End With

    '        Using client As New LocalFileVersionClient(obj.ShareInfo.User)
    '            Dim response = client.Post(Of FileVersionInfo, Boolean)(FileVersionInfo)
    '            If response.StatusCode = 200 Then
    '                Return Nothing
    '            End If
    '            Return Nothing
    '        End Using
    '    End SyncLock
    'End Function

    'Public Shared Function Delete(obj As FileSystemWatcherBase, filename As String) As Boolean
    '    SyncLock obj
    '        Using client As New LocalFileVersionClient(obj.ShareInfo.User)
    '            Dim response = client.DeleteFileByName(obj.ShareInfo.ShareId, "\" + filename)
    '            If response.StatusCode = 200 Then
    '                Return response.Data
    '            End If
    '            Return False
    '        End Using
    '    End SyncLock
    'End Function
End Class
