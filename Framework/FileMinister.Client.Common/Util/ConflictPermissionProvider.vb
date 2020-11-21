Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports FileMinister.Repo
Imports FileMinister.Models

Public Class ConflictPermissionProvider

    Public Shared Function CanRequestConflictFileUpload(user As LocalWorkSpaceInfo, conflictInfo As FileVersionConflictInfo, permissions As PermissionType) As Boolean
        If (conflictInfo.FileVersionConflictTypeId = Enums.FileVersionConflictType.ClientDelete OrElse conflictInfo.IsResolved) Then
            Return False
        End If
        'admin and has no request already
        If ((user.RoleId = Role.AccountAdmin OrElse (permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) AndAlso conflictInfo.FileVersionConflictRequestStatusId = 0) Then
            Return True
        End If
        Return False
    End Function

    Public Shared Function CanResolveOtherConflictUsingServer(user As LocalWorkSpaceInfo, conflictInfo As FileVersionConflictInfo, permissions As PermissionType) As Boolean
        If (conflictInfo.IsResolved) Then
            Return False
        End If
        If (user.RoleId = Role.AccountAdmin OrElse (permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
            Return True
        End If
        Return False
    End Function

    Public Shared Function CanResolveOtherConflictUsingUser(user As LocalWorkSpaceInfo, conflictInfo As FileVersionConflictInfo, permissions As PermissionType) As Boolean
        If (conflictInfo.IsResolved) Then
            Return False
        End If
        'admin and either clientdelete conflict or user file is uploaded
        If ((user.RoleId = Role.AccountAdmin OrElse (permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) _
            AndAlso (conflictInfo.FileVersionConflictTypeId = Enums.FileVersionConflictType.ClientDelete OrElse conflictInfo.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded)) Then
            Return True
        End If
        Return False
    End Function
End Class
