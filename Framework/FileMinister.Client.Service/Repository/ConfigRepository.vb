Imports FileMinister.Client.Common.Model
Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.shared.portable.Enums

Public Class ConfigRepository
    Inherits ClientRepositoryBase(Of ConfigInfo, Integer)
    Implements IConfigRepository

    Public Overrides Function GetAll() As ResultInfo(Of List(Of ConfigInfo), Status)
        Try
            Using service = GetClientCommonEntity()
                Dim query = From us In service.UserShares
                            From ac In service.AccountShares
                            Where us.ShareId = ac.ShareId AndAlso us.UserAccount.AccountId = ac.AccountId
                            Select New ConfigInfo() With {
                                .FileShareId = ac.ShareId,
                                .ShareName = ac.ShareName,
                                .SharePath = us.SharePath,
                                .WindowsUser = us.WindowsUser,
                                .Password = us.Password
                            }

                Dim data = query.ToList()
                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of ConfigInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all shares for all accounts
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAccountShares() As ResultInfo(Of List(Of LocalConfigInfo), Status) Implements IConfigRepository.GetAccountShares
        Try
            Using service = GetClientCommonEntity()
                Dim q = From us In service.UserShares
                        From ua In service.UserAccounts
                        From ash In service.AccountShares
                        Where us.UserAccountId = ua.UserAccountId AndAlso ua.AccountId = ash.AccountId AndAlso us.ShareId = ash.ShareId
                        Select New LocalConfigInfo With {
                            .UserAccountId = ua.UserAccountId,
                            .AccountId = ua.AccountId,
                            .FileShareId = ash.ShareId,
                            .ShareName = ash.ShareName,
                            .SharePath = us.SharePath,
                            .WindowsUser = us.WindowsUser,
                            .Password = us.Password,
                            .User = New LocalWorkSpaceInfo With {
                                .UserId = ua.UserId,
                                .WorkSpaceId = ua.WorkSpaceId,
                                .AccountId = ash.AccountId,
                                .UserAccountId = ua.UserAccountId,
                                .AccountName = ua.AccountName,
                                .AccessToken = ua.AccessToken,
                                .LocalDatabaseName = ua.LocalDatabaseName,
                                .WindowsUserSId = ua.WindowsUserSID,
                                .CloudSyncServiceUrl = ua.CloudSyncServiceURL,
                                .CloudSyncSyncServiceUrl = ua.CloudSyncSyncServiceURL
                            }
                         }
                Dim shares = q.ToList()
                For Each share In shares
                    share.User.AccessToken = CommonUtils.Helper.Decrypt(share.User.AccessToken)
                Next
                Return BuildResponse(shares)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of LocalConfigInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all mapped share for current user and account by role
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetShares() As ResultInfo(Of List(Of ConfigInfo), Status) Implements IConfigRepository.GetShares
        Try
            Using service = GetClientCommonEntity()

                Dim shares = service.GetMappedSharesForUser(Me.User.UserId, Me.User.WindowsUserSId, Me.User.AccountId, Me.User.RoleId)

                Dim lstConfig As List(Of ConfigInfo) = New List(Of ConfigInfo)

                For Each e In shares
                    Dim obj As ConfigInfo = New ConfigInfo()
                    obj.FileShareId = e.ShareId
                    obj.ShareName = e.ShareName
                    obj.SharePath = e.SharePath
                    obj.WindowsUser = e.WindowsUser
                    obj.Password = e.Password
                    lstConfig.Add(obj)
                Next
                Return BuildResponse(lstConfig)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of ConfigInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Add UserShares if not exists
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddAll(data As List(Of ConfigInfo)) As ResultInfo(Of Boolean, Status) Implements IConfigRepository.AddAll
        Try
            'Insert record to client common DB
            Using serviceCommon = GetClientCommonEntity()
                For Each item In data
                    Dim isShareExist = serviceCommon.UserShares.Where(Function(p) p.UserAccountId = Me.User.UserAccountId And p.ShareId = item.FileShareId).Any()
                    If (Not isShareExist) Then
                        Dim shares = MapFromObject(item)
                        shares.UserAccountId = Me.User.UserAccountId
                        serviceCommon.UserShares.Add(shares)
                    End If
                Next
                serviceCommon.SaveChanges()
            End Using

            Return BuildResponse(True)
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    '''  Delete a share and corresponding refrence records by share id
    ''' </summary>
    ''' <param name="shareId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteShareMaping(shareId As Integer) As ResultInfo(Of Boolean, Status) Implements IConfigRepository.DeleteShareMapping

        Try


            Using service = GetClientEntity()


                Dim objShare = service.Shares.FirstOrDefault(Function(p) p.ShareId = shareId)

                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntry.ShareId = shareId AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId AndAlso Not p.IsResolved))
                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntry.ShareId = shareId AndAlso p.VersionNumber Is Nothing))
                'service.Tags.RemoveRange(service.Tags.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.GroupFileSystemEntryPermissionAssignments.RemoveRange(service.GroupFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.UserFileSystemEntryPermissionAssignments.RemoveRange(service.UserFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.FileSystemEntries.RemoveRange(service.FileSystemEntries.Where(Function(p) p.ShareId = share.ShareId))
                'service.Shares.Remove(objShare)
                service.SaveChanges()


                Using commonService = GetClientCommonEntity()
                    commonService.UserShares.RemoveRange(commonService.UserShares.Where(Function(p) p.ShareId = shareId AndAlso p.UserAccountId = Me.User.UserAccountId))
                    'commonService.AgentShares.RemoveRange(commonService.AgentShares.Where(Function(p) p.ShareId = shareId AndAlso (Me.User.AgentId = Guid.Empty OrElse p.AgentId = Me.User.AgentId)))
                    'commonService.AccountShares.RemoveRange(commonService.AccountShares.Where(Function(p) p.ShareId = shareId AndAlso p.AccountId = Me.User.AccountId))
                    commonService.SaveChanges()
                End Using
                Return BuildResponse(True)
            End Using


            'Using service = GetClientCommonEntity()
            '    Dim share = service.UserShares.FirstOrDefault(Function(p) p.ShareId = shareId And p.UserAccountId = Me.User.UserAccountId)
            '    If share IsNot Nothing Then
            '        service.UserShares.Remove(share)
            '        service.SaveChanges()
            '        Return BuildResponse(True)
            '    Else
            '        Return BuildResponse(False)
            '    End If
            'End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try


    End Function

    ''' <summary>
    ''' Delete a share and corresponding refrence records by share id
    ''' </summary>
    ''' <param name="shareId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteShare(shareId As Integer) As ResultInfo(Of Boolean, Status) Implements IConfigRepository.DeleteShare
        Try


            Using service = GetClientEntity()


                Dim objShare = service.Shares.FirstOrDefault(Function(p) p.ShareId = shareId)

                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.Tags.RemoveRange(service.Tags.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.GroupFileSystemEntryPermissionAssignments.RemoveRange(service.GroupFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.UserFileSystemEntryPermissionAssignments.RemoveRange(service.UserFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntry.ShareId = shareId))
                service.FileSystemEntries.RemoveRange(service.FileSystemEntries.Where(Function(p) p.ShareId = shareId))
                If (objShare IsNot Nothing) Then
                    service.Shares.Remove(objShare)
                End If
                service.SaveChanges()
                Using commonService = GetClientCommonEntity()
                    commonService.UserShares.RemoveRange(commonService.UserShares.Where(Function(p) p.ShareId = shareId AndAlso p.UserAccount.AccountId = Me.User.AccountId))
                    Dim agents = commonService.UserAgents.Where(Function(p) p.UserAccount.AccountId = Me.User.AccountId).Select(Function(p) p.AgentId).ToList
                    commonService.AgentShares.RemoveRange(commonService.AgentShares.Where(Function(p) p.ShareId = shareId AndAlso agents.Contains(p.AgentId)))
                    commonService.AccountShares.RemoveRange(commonService.AccountShares.Where(Function(p) p.ShareId = shareId AndAlso p.AccountId = Me.User.AccountId))
                    commonService.SaveChanges()
                End Using
                Return BuildResponse(True)
            End Using


            'Using service = GetClientCommonEntity()
            '    Dim share = service.UserShares.FirstOrDefault(Function(p) p.ShareId = shareId And p.UserAccountId = Me.User.UserAccountId)
            '    If share IsNot Nothing Then
            '        service.UserShares.Remove(share)
            '        service.SaveChanges()
            '        Return BuildResponse(True)
            '    Else
            '        Return BuildResponse(False)
            '    End If
            'End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all mapped shares against a user, account and role
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ShareMappedSummary() As ResultInfo(Of ShareSummaryInfo, Status) Implements IConfigRepository.ShareMappedSummary
        Try
            Using service = GetClientCommonEntity()
                Dim shares = service.GetMappedSharesForUser(Me.User.UserId, Me.User.WindowsUserSId, Me.User.AccountId, Me.User.RoleId).ToList()
                Dim result = New ShareSummaryInfo() With {.AccountShareCount = shares.Count(), .MappedShareCount = shares.Where(Function(p) p.SharePath IsNot Nothing).Count()}
                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of ShareSummaryInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all shares by a user account
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AllShareByAccount() As ResultInfo(Of List(Of ConfigInfo), Status) Implements IConfigRepository.AllShareByAccount
        Try
            Using service = GetClientCommonEntity()

                Dim q = From us In service.UserShares
                        From ua In service.UserAccounts.Where(Function(p) p.UserAccountId = Me.User.UserAccountId)
                        From ash In service.AccountShares
                        Where us.UserAccountId = ua.UserAccountId AndAlso ua.AccountId = ash.AccountId AndAlso us.ShareId = ash.ShareId
                        Select New ConfigInfo With {
                            .UserAccountId = ua.UserAccountId,
                            .AccountId = ash.AccountId,
                            .FileShareId = ash.ShareId,
                            .ShareName = ash.ShareName,
                            .SharePath = us.SharePath,
                            .WindowsUser = us.WindowsUser,
                            .Password = us.Password
                         }
                Dim shares = q.ToList()
                Return BuildResponse(shares)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of ConfigInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Check if any existing share's path overlap with the paths in input list and vice versa
    ''' </summary>
    ''' <param name="paths"></param>
    ''' <param name="accountId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsAnyPathExists(paths As List(Of String), accountId As Integer) As ResultInfo(Of Boolean, Status) Implements IConfigRepository.IsAnyPathExists
        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()

        Dim found = False
        Using Service = GetClientCommonEntity()
            Dim lstUserShares = Service.UserShares.ToList()
            For Each path In paths
                Dim pp = path
                If Not pp.EndsWith("\") Then
                    pp = path + "\"
                End If
                If (lstUserShares.Where(Function(p) If(p.SharePath.EndsWith("\"), p.SharePath, p.SharePath + "\").StartsWith(pp)).Count > 0) Then
                    found = True
                    Exit For
                End If
            Next
            If (found = False) Then
                For Each usershare In lstUserShares
                    Dim pp = usershare.SharePath
                    If Not pp.EndsWith("\") Then
                        pp = usershare.SharePath + "\"
                    End If
                    If (paths.Where(Function(p) If(p.EndsWith("\"), p, p + "\").StartsWith(pp)).Count > 0) Then
                        found = True
                        Exit For
                    End If
                Next
            End If
            'Dim lstUserShares = Service.UserShares.Where(Function(p) p.UserAccount.AccountId <> accountId).ToList()
            'For Each path In paths
            '    If (lstUserShares.Where(Function(p) p.SharePath.StartsWith(path)).Count > 0) Then
            '        found = True
            '        Exit For
            '    End If
            'Next
            'If (found = False) Then
            '    For Each usershare In lstUserShares
            '        If (paths.Where(Function(p) p.StartsWith(usershare.SharePath)).Count > 0) Then
            '            found = True
            '            Exit For
            '        End If
            '    Next
            'End If
            'If (found = False) Then
            '    Dim lstUserSharesForSameAccount = Service.UserShares.Where(Function(p) p.UserAccount.AccountId = accountId).ToList()
            '    For Each path In paths
            '        If (lstUserSharesForSameAccount.Where(Function(p) p.SharePath.StartsWith(path)).Count > 0) Then
            '            found = True
            '            Exit For
            '        End If
            '    Next
            '    If (found = False) Then
            '        For Each usershare In lstUserSharesForSameAccount
            '            If (paths.Where(Function(p) p.StartsWith(usershare.SharePath)).Count > 0) Then
            '                found = True
            '                Exit For
            '            End If
            '        Next
            '    End If
            'End If
        End Using

        res.Data = found

        Return res
    End Function

    ''' <summary>
    ''' Map from entity to model 
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As Share) As ConfigInfo
        Dim t = New ConfigInfo With {
               .FileShareId = s.ShareId,
                .ShareName = s.ShareName
        }
        '.SharePath = s.SharePath
        '}
        Return t
    End Function

    ''' <summary>
    ''' Map from model to UserShare entity
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="t"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromObject(s As ConfigInfo, Optional t As UserShare = Nothing) As UserShare
        Dim tt = t
        If tt Is Nothing Then
            tt = New UserShare()
        End If
        If s.UserAccountId > 0 Then
            tt.UserAccountId = s.UserAccountId
        End If
        If s.FileShareId > 0 Then
            tt.ShareId = s.FileShareId
        End If
        If s.SharePath IsNot Nothing Then
            tt.SharePath = s.SharePath
        End If
        If s.WindowsUser IsNot Nothing Then
            tt.WindowsUser = s.WindowsUser
        End If
        If s.Password IsNot Nothing Then
            tt.Password = s.Password
        End If
        If s.CreatedOnUTC <> Date.MinValue Then
            tt.CreatedOnUTC = s.CreatedOnUTC
        End If
        If s.LastSyncedUTC <> Date.MinValue Then
            tt.LastSyncedUTC = s.LastSyncedUTC
        End If

        Return tt
    End Function

    ''' <summary>
    ''' Map from model to Share entity
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="t"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromShareObject(s As ConfigInfo, Optional t As Share = Nothing) As Share
        Dim tt = t
        If tt Is Nothing Then
            tt = New Share()
        End If
        If s.FileShareId > 0 Then
            tt.ShareId = s.FileShareId
        End If
        If s.ShareName IsNot Nothing Then
            tt.ShareName = s.ShareName
        End If
        'If s.SharePath <> Nothing Then
        '    tt.SharePath = s.SharePath
        'End If
        If s.CreatedOnUTC <> Date.MinValue Then
            tt.CreatedOnUTC = DateTime.UtcNow
        End If
        If Not s.CreatedByUserId.Equals(Guid.Empty) Then
            tt.CreatedByUserId = Me.User.UserId
        End If

        Return tt
    End Function

    Public Overrides Function Add(data As ConfigInfo) As ResultInfo(Of Boolean, Status)
        Try
            'Insert record to client common DB
            Using serviceCommon = GetClientCommonEntity()
                Dim isShareExist = serviceCommon.UserShares.Where(Function(p) p.UserAccountId = data.UserAccountId And p.ShareId = data.FileShareId).Any()
                If (Not isShareExist) Then
                    Dim shares = MapFromObject(data)
                    serviceCommon.UserShares.Add(shares)
                    serviceCommon.SaveChanges()
                End If
            End Using

            'Insert record to client DB
            Using service = GetClientEntity()
                Dim isShareExist = service.Shares.Where(Function(p) p.ShareId = data.FileShareId).Any()
                If (Not isShareExist) Then

                    Dim fileObj = New FileSystemEntry()
                    fileObj.FileSystemEntryId = Guid.NewGuid()
                    fileObj.FileSystemEntryTypeId = 1
                    fileObj.CurrentVersionNumber = 0
                    fileObj.IsCheckedOut = 0
                    fileObj.IsDeleted = 0

                    'for entry in file table 
                    Dim share = MapFromShareObject(data)
                    share.FileSystemEntries.Add(fileObj)

                    service.Shares.Add(share)
                    service.SaveChanges()
                    Return BuildResponse(True)
                Else
                    Return BuildResponse(False)
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    'Public Function AllSharesMapped(userId As Integer, agentId As Guid) As Boolean Implements IConfigRepository.AllSharesMapped
    '    Using service = GetClientCommonEntity()
    '        'Dim q = From ash In service.AgentShares
    '        '        From s In service.MappedShares
    '        '        Where ash.AgentId = agent And ash.ShareId = s.MappedShareId

    '        If (service.AgentShares.Count() = 0) Then ' as no shares found for agent probably database is not synced yet
    '            Return False
    '        End If

    '        Dim query = From a In service.AgentShares.Where(Function(p) p.AgentId = agentId)
    '                    Group Join b In service.UserShares.Where(Function(p) p.UserId = userId) On a.ShareId Equals b.ShareId Into Group
    '                    From b In Group.DefaultIfEmpty()
    '                    Select New With {a.ShareId, .Path = If(b Is Nothing, Nothing, b.SharePath)}
    '        Dim query2 = From a In query
    '                     Where a.Path Is Nothing

    '        Return (query2.Count() = 0)
    '    End Using
    'End Function

    'Public Function IsShareMapped(agentId As Guid) As Boolean Implements IConfigRepository.IsShareMapped
    '    Using service = GetClientCommonEntity()
    '        Dim q = From us In service.UserShares.Where(Function(p) p.UserId = Me.User.Id)
    '                From [as] In service.UserAccounts.Where(Function(p) p.AccountId = Me.User.AccountId AndAlso p.UserId = Me.User.Id AndAlso p.WindowsUserSID = Me.User.WindowsUser)
    '                Where us.UserId = [as].UserId
    '                Select us
    '        Return q.Count() > 0
    '    End Using
    'End Function

    'Public Overrides Function Update(id As Guid, data As ConfigInfo) As Boolean
    '    Using service = GetClientCommonEntity()
    '        'Dim share = service.Shares.FirstOrDefault(Function(p) p.ShareId = id)
    '        'If share IsNot Nothing Then
    '        '    MapFromObject(data, share)
    '        '    service.SaveChanges()
    '        'End If
    '        Return True
    '    End Using
    'End Function

    'Public Overrides Function Delete(id As Guid) As Boolean
    '    Using service = GetClientCommonEntity()
    '        'Dim share = service.Shares.FirstOrDefault(Function(p) p.ShareId = id)
    '        'If share IsNot Nothing Then
    '        '    share.IsDeactivated = True
    '        '    service.SaveChanges()
    '        'End If
    '        Return True
    '    End Using
    'End Function

    'Public Overrides Function [Get](id As Guid) As ConfigInfo
    '    Using service = GetClientCommonEntity()
    '        'Dim share = service.MappedShares.FirstOrDefault(Function(p) p.MappedShareId = id)
    '        Dim obj = New ConfigInfo
    '        'If share IsNot Nothing Then
    '        '    obj = MapToObject(share)
    '        'End If
    '        Return obj
    '    End Using
    'End Function

End Class
