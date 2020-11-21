Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports FileMinister.Repo
Imports FileMinister.Models

Public Class PermissionProvider

    Private file As FileEntryInfo
    Private permissions As PermissionType?
    Private conflict As FileVersionConflictInfo
    Private m_user As LocalWorkSpaceInfo
    Public Sub New(fileId As Guid, user As LocalWorkSpaceInfo)
        m_user = user
        GetFileDetail(fileId)
        If (user.RoleId <> Role.AccountAdmin) Then
            GetPermissions()
        End If
    End Sub

    Public Sub New(file As FileEntryInfo, user As LocalWorkSpaceInfo)
        m_user = user
        Me.file = file
        If (user.RoleId <> Role.AccountAdmin) Then
            GetPermissions()
        End If
    End Sub

    Public Function GetPermissions() As PermissionType
        If Not permissions.HasValue Then
            Using userClient As New LocalUserClient()
                Dim filePermissionResult = userClient.GetEffectivePermissionsOnFile(file.FileEntryId, True)
                If filePermissionResult.Status = Status.Success Then
                    permissions = CType(filePermissionResult.Data, PermissionType)
                End If
            End Using
        End If
        Return If(permissions.HasValue, permissions.Value, 0)
    End Function

    Private Sub GetFileDetail(fileId As Guid)
        Using client As New LocalFileClient
            Dim result = client.Get(Of FileEntryInfo)(fileId.ToString())
            If result.Status = Status.Success Then
                Me.file = result.Data
                If Me.file Is Nothing Then
                    Throw New Exception("file not found")
                End If
            Else
                Throw New Exception("Error is fetching file data")
            End If
        End Using
    End Sub

    Public Function GetConflictData(fileId As Guid) As FileVersionConflictInfo
        If (conflict Is Nothing) Then
            Using client As New LocalFileClient
                Dim result = client.GetMyConflict(fileId)
                If result.Status = Status.Success Then
                    Me.conflict = result.Data
                ElseIf result.Status = Status.NotFound Then
                    Me.conflict = Nothing
                Else
                    Throw New Exception("Error is fetching conflict data. " + result.Message)
                End If
            End Using

        End If
        Return conflict
    End Function

    Public Function CanViewProperties()
        Return Not file.IsDeleted
    End Function

    Public Function CanViewHistory()
        Return Not file.IsDeleted
    End Function

    Public Function CanLinkFile()
        ' Only account admin and share admin can link
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If
        ' Deleted file cannot be linked
        If (allowed) Then
            allowed = Not (file.IsDeleted)
            'previously linked file cannot be relinked
            If (allowed) Then
                Using fsProxy As New LocalFileClient()
                    Dim res = fsProxy.IsFilePreviouslyLinked(file.FileEntryId)
                    allowed = Not res.Data
                End Using
            End If
        End If
        Return allowed
    End Function

    Public Function CanViewPermission() As Boolean
        ' Only account admin and share admin can view permission
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If
        Return allowed
    End Function

    Public Function CanDeleteFileVersion() As Boolean
        ' Only account admin and share admin can delete version
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If
        Return allowed
    End Function

    Public Function CanCheckOut() As Boolean
        ' file not checked out and account admin/share admin or Write permission
        Dim allowed = False
        allowed = Not (file.IsCheckedOut) AndAlso Not file.IsDeleted AndAlso Not file.IsPermanentlyDeleted
        If (allowed) Then
            allowed = (m_user.RoleId = Role.AccountAdmin)
            If (allowed = False) Then
                Dim permissions = GetPermissions()
                allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                        ((permissions And PermissionType.Write) = PermissionType.Write)
            End If
        End If
        Return allowed
    End Function

    Public Function CanCheckIn() As Boolean
        ' file checked out by me and account admin/share admin or Write permission
        Dim allowed = False
        ' if file checked out by me
        allowed = (file.CheckedOutByUserId.HasValue AndAlso file.CheckedOutByUserId = m_user.UserId) AndAlso Not file.IsDeleted AndAlso Not file.IsPermanentlyDeleted
        If (allowed) Then
            allowed = (m_user.RoleId = Role.AccountAdmin)
            If (allowed = False) Then
                Dim permissions = GetPermissions()
                allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                        ((permissions And PermissionType.Write) = PermissionType.Write)
            End If
        End If
        Return allowed
    End Function

    Public Function CanUndoCheckOut()
        ' File is checked out by me or (File is checked by someone but I am an admin)
        Dim allowed = False
        allowed = file.IsCheckedOut AndAlso Not file.IsDeleted AndAlso Not file.IsPermanentlyDeleted
        If (allowed) Then
            ' see if file checked out by me
            allowed = (file.CheckedOutByUserId = m_user.UserId)
            If (Not allowed) Then
                allowed = (m_user.RoleId = Role.AccountAdmin)
                If (allowed = False) Then
                    Dim permissions = GetPermissions()
                    allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
                End If
            End If
        End If
        Return allowed
    End Function

    Public Function CanCopy() As Boolean
        Dim allowed = Not file.IsDeleted AndAlso Not file.IsPermanentlyDeleted AndAlso file.IsPhysicalFile
        Return allowed
    End Function

    ''' <summary>
    ''' caller needs to check at their end (System.Windows.Clipboard.GetFileDropList() <> Nothing AND CanPaste())
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanPaste() As Boolean
        ' admin or write permission and something in clipboard
        'Dim allowed = (System.Windows.Clipboard.GetFileDropList() = Nothing)
        'If (allowed) Then
        Dim allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                        ((permissions And PermissionType.Write) = PermissionType.Write)
        End If
        'End If
        Return allowed
    End Function

    Public Function CanCreate() As Boolean
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                        ((permissions And PermissionType.Write) = PermissionType.Write)
        End If
        Return allowed
    End Function

    Public Function CanSoftDelete() As Boolean
        ' File not already deleted and (admin or write permission)
        Dim allowed = Not (file.IsDeleted Or file.IsPermanentlyDeleted)
        If (allowed) Then
            allowed = (m_user.RoleId = Role.AccountAdmin)
            If (allowed = False) Then
                Dim permissions = GetPermissions()
                allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                          ((permissions And PermissionType.Write) = PermissionType.Write)
            End If
        End If
        Return CanCheckOut()


        'Return True
    End Function

    Public Function CanHardDelete() As Boolean
        ' Only account admin and share admin can permanent delete
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If
        Return allowed

        'Return True
    End Function

    Public Function CanUndoDelete() As Boolean
        ' Only account admin and share admin can undo delete and file.isdeleted is true
        Dim allowed = False
        allowed = file.IsDeleted AndAlso (Not file.IsPermanentlyDeleted) AndAlso file.FileVersion.DeltaOperation.DeltaOperationId = Guid.Empty
        If allowed Then
            allowed = (m_user.RoleId = Role.AccountAdmin)
            If (allowed = False) Then
                Dim permissions = GetPermissions()
                allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
            End If
        End If
        Return allowed
    End Function

    Public Shared Function CanChangeShareAdminPermission(user As LocalWorkSpaceInfo) As Boolean
        Return (user.RoleId = Role.AccountAdmin)
    End Function

    Public Function CanChangeTag() As Boolean
        ' admin or change tag permission
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (Not allowed) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                      ((permissions And PermissionType.Tag) = PermissionType.Tag)
        End If
        If (allowed) Then
            If (file.IsPermanentlyDeleted OrElse file.IsDeleted OrElse (file.FileVersion.PreviousFileVersionId Is Nothing AndAlso (file.FileVersion.VersionNumber Is Nothing OrElse file.FileVersion.VersionNumber = 0))) Then
                allowed = False
            End If
        End If
        Return allowed
    End Function

    Public Function CanResolveConflict() As Boolean
        ' admin or write permission
        Dim allowed = False
        If (file.FileVersion.DeltaOperation IsNot Nothing AndAlso file.FileVersion.DeltaOperation.IsConflicted IsNot Nothing) Then
            allowed = file.FileVersion.DeltaOperation.IsConflicted
            If (allowed) Then
                allowed = (m_user.RoleId = Role.AccountAdmin)
                If (allowed = False) Then
                    Dim permissions = GetPermissions()
                    allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                              ((permissions And PermissionType.Write) = PermissionType.Write)
                End If
            End If
        End If
        Return allowed
    End Function

    Public Function CanShowOtherConflictsColumns() As Boolean
        Dim allowed = False
        allowed = (m_user.RoleId = Role.AccountAdmin)
        If (Not allowed) Then
            Dim permissions = GetPermissions()
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If
        Return allowed
    End Function

    Public Function CanRequestConflictHelp() As Boolean
        'account admin and share admin not needed
        Dim user = Me.m_user

        Dim allowed = (user.RoleId <> Role.AccountAdmin)
        If (allowed) Then
            If (file.FileVersion.DeltaOperation IsNot Nothing AndAlso file.FileVersion.DeltaOperation.IsConflicted IsNot Nothing) Then
                allowed = file.FileVersion.DeltaOperation.IsConflicted
                If (allowed) Then
                    Dim permissions = GetPermissions()
                    allowed = ((permissions And PermissionType.ShareAdmin) <> PermissionType.ShareAdmin) AndAlso
                              ((permissions And PermissionType.Write) = PermissionType.Write)
                End If
            End If
        End If
        If (allowed) Then
            If (conflict Is Nothing) Then
                GetConflictData(file.FileEntryId)
            End If
            If (conflict IsNot Nothing) Then
                Return conflict.FileVersionConflictTypeId <> Enums.FileVersionConflictType.ClientDelete AndAlso Not conflict.IsClientUploadRequested
            End If
        End If
        Return False
    End Function
End Class
