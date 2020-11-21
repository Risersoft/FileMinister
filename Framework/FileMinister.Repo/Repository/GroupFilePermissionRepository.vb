Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

''' <summary>
''' Group File Permission Repository
''' </summary>
''' <remarks></remarks>
Public Class GroupFilePermissionRepository
    Inherits ServerRepositoryBase(Of UserFilePermissionInfo, Guid)
    Implements IGroupFilePermissionRepository

    ''' <summary>
    ''' Get Permission based on File and Group
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="groupid">GroupId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [GetPermissionByFileAndGroup](fileid As Guid, groupid As Guid) As ResultInfo(Of UserFilePermissionInfo, Status) Implements IGroupFilePermissionRepository.GetPermissionByFileAndGroup
        Try
            Using service = GetServerEntity()
                Dim resultObj = service.GetGroupPermissionMatrixOnLatestFileVersionParentHierarchy(groupid, fileid)
                Dim obj = New UserFilePermissionInfo()

                For Each element In resultObj
                    obj.Id = groupid
                    obj.EffectiveAllow = element.InheritedAllowedPermissions
                    obj.EffectiveDeny = element.InheritedDeniedPermissions
                    obj.ExclusiveAllow = element.ExclusivelyAllowedPermissions
                    obj.ExclusiveDeny = element.ExclusivelyDeniedPermissions
                    obj.Type = UserGroupType.Group
                    obj.EffectivePermissionsSources = element.InheritedPermissionsSources

                    Exit For
                Next

                Return BuildResponse(obj)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of UserFilePermissionInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete Permission based on File and Group 
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="groupid">GroupId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [DeletePermissionByFileAndGroup](fileid As Guid, groupId As Guid) As ResultInfo(Of Boolean, Status) Implements IGroupFilePermissionRepository.DeletePermissionByFileAndGroup
        Try
            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (FileRepository.GetEffectivePermission(service, Me.User, fileid) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim groupfilepermission = service.GroupFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.GroupId = groupId)
                    If groupfilepermission IsNot Nothing Then
                        groupfilepermission.DeletedByUserId = Me.User.UserId
                        groupfilepermission.DeletedOnUTC = DateTime.UtcNow()
                        groupfilepermission.IsDeleted = True
                        service.SaveChanges()
                    End If
                    Return BuildResponse(True)
                Else
                    Return BuildResponse(False, Status.AccessDenied)
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Update Permission based on File and Group 
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="groupid">GroupId</param>
    ''' <param name="permissionAllowed"></param>
    ''' <param name="permissionDenied"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [UpdatePermissionByFileAndGroup](fileid As Guid, groupId As Guid, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status) Implements IGroupFilePermissionRepository.UpdatePermissionByFileAndGroup
        Try
            If permissionAllowed = 16 Then
                permissionAllowed = 23
            ElseIf permissionAllowed = 4 Then
                permissionAllowed = 7
            ElseIf permissionAllowed = 2 Then
                permissionAllowed = 3
            End If
            'Set Permission Denied Value
            If permissionDenied = 1 Then
                permissionDenied = 7
            ElseIf permissionDenied = 2 Then
                permissionDenied = 6
            End If

            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (FileRepository.GetEffectivePermission(service, Me.User, fileid) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim groupfilepermission = service.GroupFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.GroupId = groupId)
                    If groupfilepermission IsNot Nothing Then
                        'UPDATE CASE
                        groupfilepermission.AllowedPermissions = permissionAllowed
                        groupfilepermission.DeniedPermissions = permissionDenied
                        groupfilepermission.IsDeleted = False
                        groupfilepermission.DeletedByUserId = Nothing
                        groupfilepermission.DeletedOnUTC = Nothing

                        service.SaveChanges()
                    Else
                        'INSERT CASE
                        Dim gfpObj = New GroupFileEntryPermission()

                        gfpObj.GroupFileEntryPermissionId = Guid.NewGuid()

                        gfpObj.AllowedPermissions = permissionAllowed
                        gfpObj.DeniedPermissions = permissionDenied
                        gfpObj.IsDeleted = False
                        gfpObj.CreatedByUserId = Me.User.UserId
                        gfpObj.CreatedOnUTC = DateTime.UtcNow()
                        gfpObj.FileEntryId = fileid
                        gfpObj.GroupId = groupId

                        service.GroupFileEntryPermissions.Add(gfpObj)

                        service.SaveChanges()

                    End If
                Else
                    Return BuildResponse(False, Status.AccessDenied)
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function GetGroupFilePermissionsForShare(shareId As Short) As ResultInfo(Of List(Of FilePermissionInfo), Status) Implements IGroupFilePermissionRepository.GetGroupFilePermissionsForShare
        Dim result As List(Of FilePermissionInfo) = Nothing

        Try
            Using service = GetServerEntity()
                Dim query = From f In service.FileEntries
                            Join gp In service.GroupFileEntryPermissions
                                On f.FileEntryId Equals gp.FileEntryId
                            Join um In service.UserMappings
                                On gp.GroupId Equals um.GroupID
                            Let fv = service.GetLatestFileVersion(f.FileEntryId).FirstOrDefault()
                            Where f.FileShareId = shareId AndAlso Not f.IsDeleted AndAlso Not gp.IsDeleted AndAlso Not um.IsDeleted AndAlso um.SID IsNot Nothing AndAlso um.SID.Trim() <> String.Empty
                            Select f, gp, um, fv

                If query IsNot Nothing AndAlso query.Count > 0 Then
                    result = New List(Of FilePermissionInfo)()
                    For Each obj In query
                        result.Add(New FilePermissionInfo With
                                {
                                .FileEntryId = obj.f.FileEntryId,
                                .ExclusiveAllow = obj.gp.AllowedPermissions,
                                .ExclusiveDeny = obj.gp.DeniedPermissions,
                                .FileEntryTypeId = obj.f.FileEntryTypeId,
                                .FileShareId = obj.f.FileShareId,
                                .RelativePath = If(obj.fv Is Nothing, "", obj.fv.FileEntryRelativePath),
                                .SID = obj.um.SID,
                                .Type = UserGroupType.Group,
                                .UserGroupId = obj.gp.GroupId
                                })
                    Next
                    result = result.OrderBy(Function(x) Len(x.RelativePath)).ToList()
                    Return BuildResponse(result, Status.Success)

                Else
                    Return BuildResponse(result, Status.NotFound)
                End If

            End Using

        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FilePermissionInfo))(ex)
        End Try

        Return BuildResponse(result, Status.Error)
    End Function

End Class
