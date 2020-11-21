Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

Public Class UserFilePermissionRepository
    Inherits ServerRepositoryBase(Of UserFilePermissionInfo, Guid)
    Implements IUserFilePermissionRepository

    ''' <summary>
    ''' Get Users And Groups Having Exclusive Permissions On Latest FileVersions ParentHierarchy
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllUsersAndGroups(fileid As Guid) As ResultInfo(Of List(Of UserGroupInfo), Status) Implements IUserFilePermissionRepository.GetAllUsersAndGroups
        Try
            Using service = GetServerEntity()

                Dim resultObj = service.GetUsersAndGroupsHavingExclusivePermissionsOnLatestFileVersionParentHierarchy(fileid)
                Dim ObjLstUserGroupInfo As List(Of UserGroupInfo) = New List(Of UserGroupInfo)()

                For Each element In resultObj
                    Dim obj = New UserGroupInfo
                    obj.Id = element.UserOrGroupId
                    If (element.IsUserId = True) Then
                        obj.Type = UserGroupType.User
                    Else
                        obj.Type = UserGroupType.Group
                    End If
                    obj.Name = element.Name
                    ObjLstUserGroupInfo.Add(obj)
                Next
                Return BuildResponse(ObjLstUserGroupInfo)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of UserGroupInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get User Permission Matrix On Latest FileVersions
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="userid">UserId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [GetPermissionByFileAndUser](fileid As Guid, userid As Guid) As ResultInfo(Of UserFilePermissionInfo, Status) Implements IUserFilePermissionRepository.GetPermissionByFileAndUser
        Try
            Using service = GetServerEntity()

                Dim resultObj = service.GetUserPermissionMatrixOnLatestFileVersionParentHierarchy(userid, fileid)
                Dim obj = New UserFilePermissionInfo()

                For Each element In resultObj
                    obj.Id = userid
                    obj.EffectiveAllow = element.InheritedAllowedPermissions
                    obj.EffectiveDeny = element.InheritedDeniedPermissions
                    obj.ExclusiveAllow = element.ExclusivelyAllowedPermissions
                    obj.ExclusiveDeny = element.ExclusivelyDeniedPermissions
                    obj.Type = UserGroupType.User
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
    ''' Delete Permission based on file and user
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="groupid">GroupId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [DeletePermissionByFileAndUser](fileid As Guid, groupId As Guid) As ResultInfo(Of Boolean, Status) Implements IUserFilePermissionRepository.DeletePermissionByFileAndUser
        Try
            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (FileRepository.GetEffectivePermission(service, Me.User, fileid) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim userfilepermission = service.UserFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.UserId = groupId)
                    If userfilepermission IsNot Nothing Then
                        userfilepermission.DeletedByUserId = Me.User.UserId
                        userfilepermission.DeletedOnUTC = DateTime.UtcNow()
                        userfilepermission.IsDeleted = True
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
    ''' Update Permissions
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="userid">UserId</param>
    ''' <param name="permissionAllowed">Allowed Permission</param>
    ''' <param name="permissionDenied">Denied Permissions</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [UpdatePermissionByFileAndUser](fileid As Guid, userId As Guid, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status) Implements IUserFilePermissionRepository.UpdatePermissionByFileAndUser
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
                    Dim userfilepermission = service.UserFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.UserId = userId)
                    If userfilepermission IsNot Nothing Then
                        'UPDATE CASE
                        'If typeOfPermission = 1 Then
                        'Set permissionAllowed Value

                        userfilepermission.AllowedPermissions = permissionAllowed
                        'Else
                        userfilepermission.DeniedPermissions = permissionDenied
                        ' End If
                        userfilepermission.IsDeleted = False
                        userfilepermission.DeletedByUserId = Nothing
                        userfilepermission.DeletedOnUTC = Nothing

                        service.SaveChanges()
                    Else
                        'INSERT CASE
                        Dim ufpObj = New UserFileEntryPermission()

                        ufpObj.UserFileEntryPermissionId = Guid.NewGuid()

                        'If typeOfPermission = 1 Then
                        ufpObj.AllowedPermissions = permissionAllowed
                        'Else
                        ufpObj.DeniedPermissions = permissionDenied
                        'End If
                        ufpObj.IsDeleted = False
                        ufpObj.CreatedByUserId = Me.User.UserId
                        ufpObj.CreatedOnUTC = DateTime.UtcNow()
                        ufpObj.FileEntryId = fileid
                        ufpObj.UserId = userId

                        service.UserFileEntryPermissions.Add(ufpObj)

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
    ''' Update Permission
    ''' </summary>
    ''' <param name="data">Permission Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateFilePermission(data As Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo))) As ResultInfo(Of Boolean, Status) Implements IUserFilePermissionRepository.UpdateFilePermission
        Try
            Dim fileId = data.Item1
            Dim removedUserAndGroupList = data.Item2
            Dim updatedUserAndGroupPermissions = data.Item3

            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (FileRepository.GetEffectivePermission(service, Me.User, fileId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    For Each updatedUserOrGroupPermissions In updatedUserAndGroupPermissions
                        If updatedUserOrGroupPermissions.ExclusiveAllow = 0 AndAlso updatedUserOrGroupPermissions.ExclusiveDeny = 0 Then
                            'remove user or group
                            RemovePermissionByFileAndUser(fileId, updatedUserOrGroupPermissions.Type, updatedUserOrGroupPermissions.Id, service)
                        ElseIf (16 And updatedUserOrGroupPermissions.ExclusiveAllow) = 16 Then
                            'remove user or group and add in case of shared admin
                            RemovePermissionByFileAndUser(fileId, updatedUserOrGroupPermissions.Type, updatedUserOrGroupPermissions.Id, service)
                            AddUpdateFilePermission(fileId, updatedUserOrGroupPermissions, service)
                        Else
                            AddUpdateFilePermission(fileId, updatedUserOrGroupPermissions, service)
                        End If
                    Next
                    For Each removedUserOrGroupList In removedUserAndGroupList
                        RemovePermissionByFileAndUser(fileId, removedUserOrGroupList.Type, removedUserOrGroupList.Id, service)
                    Next
                    service.SaveChanges()
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
    ''' Add or Update Permission
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <param name="updatedUserOrGroupPermissions">Allowed/Denied Permission List</param>
    ''' <param name="service"></param>
    ''' <remarks></remarks>
    Private Sub AddUpdateFilePermission(fileId As Guid, updatedUserOrGroupPermissions As UserFilePermissionInfo, service As FileMinisterEntities)
        Dim permissionAllowed = updatedUserOrGroupPermissions.ExclusiveAllow
        Dim permissionDenied = updatedUserOrGroupPermissions.ExclusiveDeny
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
        If (updatedUserOrGroupPermissions.Type = UserGroupType.User) Then
            Dim userfilepermission = service.UserFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileId AndAlso p.UserId = updatedUserOrGroupPermissions.Id)
            If userfilepermission IsNot Nothing Then
                'UPDATE CASE
                userfilepermission.AllowedPermissions = permissionAllowed
                userfilepermission.DeniedPermissions = permissionDenied
                userfilepermission.IsDeleted = False
                userfilepermission.DeletedByUserId = Nothing
                userfilepermission.DeletedOnUTC = Nothing
            Else
                'INSERT CASE
                Dim ufpObj = New UserFileEntryPermission()
                ufpObj.UserFileEntryPermissionId = Guid.NewGuid()
                ufpObj.AllowedPermissions = permissionAllowed
                ufpObj.DeniedPermissions = permissionDenied
                ufpObj.IsDeleted = False
                ufpObj.CreatedByUserId = Me.User.UserId
                ufpObj.CreatedOnUTC = DateTime.UtcNow()
                ufpObj.FileEntryId = fileId
                ufpObj.UserId = updatedUserOrGroupPermissions.Id

                service.UserFileEntryPermissions.Add(ufpObj)
            End If
        Else
            Dim groupfilepermission = service.GroupFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileId AndAlso p.GroupId = updatedUserOrGroupPermissions.Id)
            If groupfilepermission IsNot Nothing Then
                'UPDATE CASE
                groupfilepermission.AllowedPermissions = permissionAllowed
                groupfilepermission.DeniedPermissions = permissionDenied
                groupfilepermission.IsDeleted = False
                groupfilepermission.DeletedByUserId = Nothing
                groupfilepermission.DeletedOnUTC = Nothing
            Else
                'INSERT CASE
                Dim gfpObj = New GroupFileEntryPermission()
                gfpObj.GroupFileEntryPermissionId = Guid.NewGuid()
                gfpObj.AllowedPermissions = permissionAllowed
                gfpObj.DeniedPermissions = permissionDenied
                gfpObj.IsDeleted = False
                gfpObj.CreatedByUserId = Me.User.UserId
                gfpObj.CreatedOnUTC = DateTime.UtcNow()
                gfpObj.FileEntryId = fileId
                gfpObj.GroupId = updatedUserOrGroupPermissions.Id

                service.GroupFileEntryPermissions.Add(gfpObj)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Remove Permission
    ''' </summary>
    ''' <param name="fileid">FileId</param>
    ''' <param name="type">Permission Type</param>
    ''' <param name="id">User Id</param>
    ''' <param name="service"></param>
    ''' <remarks></remarks>
    Private Sub RemovePermissionByFileAndUser(fileid As Guid, type As UserGroupType, id As Guid, service As FileMinisterEntities)
        If (type = UserGroupType.User) Then
            Dim userfilepermission = service.UserFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.UserId = id)
            If userfilepermission IsNot Nothing Then
                userfilepermission.DeletedByUserId = Me.User.UserId
                userfilepermission.DeletedOnUTC = DateTime.UtcNow()
                userfilepermission.IsDeleted = True
            End If
        Else
            Dim groupfilepermission = service.GroupFileEntryPermissions.FirstOrDefault(Function(p) p.FileEntryId = fileid AndAlso p.GroupId = id)
            If groupfilepermission IsNot Nothing Then
                groupfilepermission.DeletedByUserId = Me.User.UserId
                groupfilepermission.DeletedOnUTC = DateTime.UtcNow()
                groupfilepermission.IsDeleted = True
            End If
        End If
    End Sub

    Public Function GetUserFilePermissionsForShare(shareId As Short) As ResultInfo(Of List(Of FilePermissionInfo), Status) Implements IUserFilePermissionRepository.GetUserFilePermissionsForShare
        Dim result As List(Of FilePermissionInfo) = Nothing

        Try
            Using service = GetServerEntity()
                Dim query = From f In service.FileEntries
                            Join up In service.UserFileEntryPermissions
                                On f.FileEntryId Equals up.FileEntryId
                            Join um In service.UserMappings
                                On up.UserId Equals um.UserID
                            Let fv = service.GetLatestFileVersion(f.FileEntryId).FirstOrDefault()
                            Where f.FileShareId = shareId AndAlso Not f.IsDeleted AndAlso Not up.IsDeleted AndAlso Not um.IsDeleted AndAlso um.SID IsNot Nothing AndAlso um.SID.Trim() <> String.Empty
                            Select f, up, fv, um

                If query IsNot Nothing AndAlso query.Count > 0 Then
                    result = New List(Of FilePermissionInfo)()
                    For Each obj In query
                        result.Add(New FilePermissionInfo With
                                {
                                .FileEntryId = obj.f.FileEntryId,
                                .ExclusiveAllow = obj.up.AllowedPermissions,
                                .ExclusiveDeny = obj.up.DeniedPermissions,
                                .FileEntryTypeId = obj.f.FileEntryTypeId,
                                .FileShareId = obj.f.FileShareId,
                                .RelativePath = If(obj.fv Is Nothing, "", obj.fv.FileEntryRelativePath),
                                .SID = obj.um.SID,
                                .Type = UserGroupType.User,
                                .UserGroupId = obj.up.UserId
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
