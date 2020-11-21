Imports System.Data.SqlClient
Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.shared.web
Imports risersoft.shared.portable.Enums

Public Class UserRepository
    Inherits ClientRepositoryBase(Of LocalWorkSpaceInfo, Integer)
    Implements IUserRepository

    ''' <summary>
    ''' Get DB Template path
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetDBTemplatePath() As String
        Dim commonPath = Globals.myApp.objAppExtender.CommonAppDataPath
        Return System.IO.Path.Combine(commonPath, "FileMinisterClient")
    End Function

    ''' <summary>
    ''' Get all users
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of LocalWorkSpaceInfo), Status)
        Try
            Using service = GetClientCommonEntity()
                Dim users = service.UserAccounts _
                            .Select(Function(p) MapUser(p)).ToList()
                Return BuildResponse(users)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of LocalWorkSpaceInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get UserAccount by window user id
    ''' </summary>
    ''' <param name="windowUserSID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByWindowUserSID(windowUserSID As String) As ResultInfo(Of LocalWorkSpaceInfo, Status) Implements IUserRepository.GetByWindowUserSID
        Try
            Using service = GetClientCommonEntity()
                Dim userAccount = service.UserAccounts _
                .Where(Function(p) p.WindowsUserSID = windowUserSID) _
                .OrderByDescending(Function(p) p.LastLoggedInUTC).FirstOrDefault()
                If userAccount IsNot Nothing Then
                    Return BuildResponse(MapUser(userAccount))
                End If
            End Using
            Return BuildResponse(Of LocalWorkSpaceInfo)(Nothing, Status.NotFound)
        Catch ex As Exception
            Return BuildExceptionResponse(Of LocalWorkSpaceInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Create Local Db, Do the provisioning, add UserAccount if not exist, add UserAgent if not exist
    ''' </summary>
    ''' <param name="userAccount"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Log(userAccount As LocalWorkSpaceInfo) As ResultInfo(Of LocalWorkSpaceInfo, Status) Implements IUserRepository.Log
        Try
            Using service = GetClientCommonEntity()

                Dim userAccountDB = service.UserAccounts.Include("UserAgents") _
                                    .FirstOrDefault(Function(ua) ua.UserId = userAccount.UserId And ua.AccountId = userAccount.AccountId And ua.WindowsUserSID = userAccount.WindowsUserSId)
                Dim userAgent As UserAgent = Nothing
                Dim createUserAgentData = False
                If userAccountDB Is Nothing Then
                    Dim destFolderPath = userAccount.LocalDatabaseName + "\FileMinister\"
                    If Not IO.Directory.Exists(destFolderPath) Then
                        IO.Directory.CreateDirectory(destFolderPath)
                    End If

                    Dim dbTempName As String = Guid.NewGuid().ToString().Replace("-", "")
                    Dim sourcePath = GetDBTemplatePath()
                    Dim dbName As String = destFolderPath + dbTempName + ".mdf"
                    Dim dbLogName As String = destFolderPath + dbTempName + "_log.ldf"

                    IO.File.Copy(sourcePath + ".mdf", dbName)
                    IO.File.Copy(sourcePath + "_log.ldf", dbLogName)

                    ProvisionDatabase(dbName)

                    userAccountDB = New UserAccount()
                    With userAccountDB
                        .UserId = userAccount.UserId
                        .AccountId = userAccount.AccountId
                        .AccountName = userAccount.AccountName
                        .RoleId = userAccount.RoleId
                        .WindowsUserSID = userAccount.WindowsUserSId
                        .WorkSpaceId = Guid.NewGuid()
                        .CloudSyncServiceURL = userAccount.CloudSyncServiceUrl
                        .CloudSyncSyncServiceURL = userAccount.CloudSyncSyncServiceUrl
                        .OrganisationServiceURL = userAccount.OrganisationServiceURL
                        .CreatedOnUTC = DateTime.UtcNow
                        .LocalDatabaseName = dbName
                    End With

                    service.UserAccounts.Add(userAccountDB)

                    createUserAgentData = userAccount.SelectedAgentId <> Guid.Empty
                ElseIf userAccount.SelectedAgentId <> Guid.Empty Then
                    userAgent = userAccountDB.UserAgents.FirstOrDefault(Function(p) p.AgentId = userAccount.SelectedAgentId)
                    If userAgent Is Nothing Then
                        createUserAgentData = True
                    End If
                End If

                If createUserAgentData Then
                    userAgent = New UserAgent()
                    With userAgent
                        .UserAgentId = Guid.NewGuid()
                        .AgentId = userAccount.SelectedAgentId
                        .AgentName = userAccount.SelectedAgentName
                        .UserAccount = userAccountDB
                        .CreatedOnUTC = DateTime.UtcNow
                    End With
                    userAccountDB.UserAgents.Add(userAgent)
                End If

                'Dim userAgent = service.UserAgents.Include("UserAccount") _
                '                .FirstOrDefault(Function(ua) ua.UserAccount.UserId = userAccount.UserId And ua.UserAccount.AccountId = userAccount.AccountId And ua.AgentId = userAccount.AgentId And ua.UserAccount.WindowsUserSID = userAccount.WindowsUserSID)
                'If (userAgent Is Nothing) Then


                '    userAgent = New UserAgent()
                '    With userAgent
                '        .UserAgentId = Guid.NewGuid()
                '        .AgentId = userAccount.AgentId
                '        .AgentName = userAccount.AgentName
                '        .UserAccount = ua
                '        .CreatedOnUTC = DateTime.UtcNow
                '    End With
                '    service.UserAgents.Add(userAgent)
                'End If

                userAccountDB.LastLoggedInUTC = DateTime.UtcNow
                userAccountDB.AccessToken = CommonUtils.Helper.Encrypt(userAccount.AccessToken)

                For Each ua In userAccountDB.UserAgents
                    ua.LastLoggedInUTC = DateTime.UtcNow
                Next

                service.SaveChanges()

                Dim result As New LocalWorkSpaceInfo()

                With result
                    .UserAccountId = userAccountDB.UserAccountId
                    .AccountId = userAccountDB.AccountId
                    .AccountName = userAccountDB.AccountName
                    .RoleId = userAccountDB.RoleId
                    ''.AgentId = userAgent.AgentId
                    ''.AgentName = userAgent.AgentName
                    .WindowsUserSId = userAccountDB.WindowsUserSID
                    .WorkSpaceId = userAccountDB.WorkSpaceId
                    .CloudSyncServiceUrl = userAccountDB.CloudSyncServiceURL
                    .CloudSyncSyncServiceUrl = userAccountDB.CloudSyncSyncServiceURL
                    .OrganisationServiceURL = userAccountDB.OrganisationServiceURL
                End With

                Using clientService = GetClientEntity()
                    Dim _user = clientService.Users.FirstOrDefault(Function(p) p.UserId = userAccountDB.UserId)
                    If _user IsNot Nothing Then
                        result.Email = _user.UserName
                    End If
                End Using

                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of LocalWorkSpaceInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get UserAccount By User id and window user id
    ''' </summary>
    ''' <param name="userId"></param>
    ''' <param name="windowUserName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserConfigured(userId As Guid, AccountName As String, windowUserName As String) As ResultInfo(Of LocalWorkSpaceInfo, Status) Implements IUserRepository.GetUserConfigured
        Try
            Using service = GetClientCommonEntity()

                Dim userAccount = service.UserAccounts.FirstOrDefault(Function(p) p.UserId = userId AndAlso p.WindowsUserSID = windowUserName AndAlso p.AccountName = AccountName)
                If userAccount IsNot Nothing Then
                    Dim user = New LocalWorkSpaceInfo()
                    With user
                        .UserId = userAccount.UserId
                        .UserAccountId = userAccount.UserAccountId
                        .AccountId = userAccount.AccountId
                        .WindowsUserSId = userAccount.WindowsUserSID
                        .WorkSpaceId = userAccount.WorkSpaceId
                    End With
                    Return BuildResponse(user)
                End If

                'Dim userAgent = service.UserAgents _
                '                .FirstOrDefault(Function(ua) ua.UserAccount.UserId = userId AndAlso ua.UserAccount.WindowsUserSID = windowUserName)
                'If (userAgent IsNot Nothing) Then
                '    Dim user = New LocalWorkSpaceInfo()
                '    With user
                '        .Id = userAgent.UserAccount.UserId
                '        .UserAccountId = userAgent.UserAccountId
                '        .AccountId = userAgent.UserAccount.AccountId
                '        .AgentId = userAgent.AgentId
                '        .WindowsUser = userAgent.UserAccount.WindowsUserSID
                '        .WorkSpaceId = userAgent.UserAccount.WorkSpaceId
                '    End With
                '    Return BuildResponse(user)
                'End If
            End Using
            Return BuildResponse(Of LocalWorkSpaceInfo)(Nothing, Status.NotFound)
        Catch ex As Exception
            Return BuildExceptionResponse(Of LocalWorkSpaceInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get UserAccount by a user
    ''' </summary>
    ''' <param name="user"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserAccount(user As LocalWorkSpaceInfo) As ResultInfo(Of LocalWorkSpaceInfo, Status) Implements IUserRepository.GetUserAccount
        Try
            Using service = GetClientCommonEntity()
                Dim ua = service.UserAccounts _
                                     .FirstOrDefault(Function(p) p.UserId = user.UserId AndAlso p.AccountId = user.AccountId AndAlso p.WindowsUserSID = user.WindowsUserSId)
                Dim result As LocalWorkSpaceInfo = Nothing

                If ua IsNot Nothing Then
                    result = New LocalWorkSpaceInfo()
                    With result
                        .UserAccountId = ua.UserAccountId
                        .UserId = ua.UserId
                        .AccountId = ua.AccountId
                        ''.AgentId = ua.AgentId
                        .WindowsUserSId = ua.WindowsUserSID
                        .WorkSpaceId = ua.WorkSpaceId
                        .LocalDatabaseName = ua.LocalDatabaseName
                        .CloudSyncSyncServiceUrl = ua.CloudSyncSyncServiceURL
                        .CloudSyncServiceUrl = ua.CloudSyncServiceURL
                        .OrganisationServiceURL = ua.OrganisationServiceURL
                        .AccessToken = ua.AccessToken
                        .CreatedOnUTC = ua.CreatedOnUTC
                        .LastLoggedInUTC = ua.LastLoggedInUTC
                    End With
                End If

                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of LocalWorkSpaceInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get the effective permissions on a file for a user
    ''' </summary>
    ''' <param name="fileId"></param>
    ''' <param name="includeDeleted"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEffectivePermissionsOnFile(fileId As Guid, includeDeleted As Boolean) As ResultInfo(Of Integer, Status) Implements IUserRepository.GetEffectivePermissionsOnFile
        Try
            Using service = GetClientEntity()

                Dim paramUserId = New SqlParameter("@UserId", SqlDbType.Int)
                paramUserId.Value = Me.User.UserId

                Dim paramFileSystemEntryId = New SqlParameter("@FileSystemEntryId", SqlDbType.UniqueIdentifier)
                paramFileSystemEntryId.Value = fileId

                Dim paramIncludeDeleted = New SqlParameter("@IncludeDeleted", SqlDbType.Bit)
                paramIncludeDeleted.Value = includeDeleted

                'Dim permissions = service.Database.ExecuteSqlCommand("exec dbo.GetEffectiveUserPermissionsOnFileSystemEntry @UserId = @UserId, @FileSystemEntryId = @FileSystemEntryId", paramUserId, paramFileSystemEntryId)

                Dim sqlQuery As String = "SELECT dbo.GetEffectiveUserPermissionsOnFileSystemEntryClient ({0},{1},{2})"
                Dim parameters As [Object]() = {Me.User.UserId, fileId, includeDeleted}
                Dim permissions As Byte = service.Database.SqlQuery(Of Byte)(sqlQuery, parameters).FirstOrDefault()

                ''Dim permissions = service.GetEffectiveUserPermissionsOnFileSystemEntry(Me.User.Id, fileId).FirstOrDefault()
                ''Return BuildResponse(If(permissions Is Nothing, 0, permissions.Value))
                Return BuildResponse(CType(permissions, Integer))
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get UserAccount by user's account id
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserByUserAccountId() As ResultInfo(Of LocalWorkSpaceInfo, Status) Implements IUserRepository.GetUserByUserAccountId
        Try
            Using service = GetClientCommonEntity()
                Dim result As New ResultInfo(Of LocalWorkSpaceInfo, Status)
                Dim userAccount = service.UserAccounts.FirstOrDefault(Function(p) p.UserAccountId = Me.User.UserAccountId)
                If userAccount IsNot Nothing Then
                    result.Data = MapUser(userAccount)
                    result.Status = Status.Success
                Else
                    result.Status = Status.NotFound
                End If
                Return result
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of LocalWorkSpaceInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Expire the access token for a user
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LogOut() As ResultInfo(Of Boolean, Status) Implements IUserRepository.LogOut
        Try
            Using service = GetClientCommonEntity()
                Dim userAccount = service.UserAccounts.FirstOrDefault(Function(p) p.UserAccountId = Me.User.UserAccountId)
                If userAccount IsNot Nothing Then
                    userAccount.AccessToken = Nothing
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' local db provisioning
    ''' </summary>
    ''' <param name="dbPath"></param>
    ''' <remarks></remarks>
    Private Sub ProvisionDatabase(dbPath As String)
        Dim commonPath = Globals.myApp.objAppExtender.CommonAppDataPath
        Dim scriptPath = System.IO.Path.Combine(commonPath, "Scripts")
        Dim scriptFiles = IO.Directory.GetFiles(scriptPath, "*.sql")

        If scriptFiles.Length > 0 Then
            Dim connStr = String.Format(ConnectionStringTemplate, EntityNameClient, dbPath)
            Using service = New FileMinisterClientCommonEntities(connStr)
                For Each filePath In scriptFiles
                    Dim content = IO.File.ReadAllText(filePath)
                    Dim id = Guid.NewGuid().ToString()
                    Dim formattedContent = String.Format(content, id)
                    For Each line In formattedContent.Split(New String() {Environment.NewLine + "GO" + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                        If line.ToLower() <> Environment.NewLine Then
                            service.Database.ExecuteSqlCommand(line)
                        End If
                    Next
                Next
            End Using
        End If
    End Sub

    ''' <summary>
    ''' map entity to model
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As UserAgent) As LocalWorkSpaceInfo
        Dim t = New LocalWorkSpaceInfo With {
                .UserId = s.UserAccount.UserId,
                .UserAccountId = s.UserAccountId,
                .AccountId = s.UserAccount.AccountId,
                .WindowsUserSId = s.UserAccount.WindowsUserSID,
                .WorkSpaceId = s.UserAccount.WorkSpaceId
             }
        Return t
    End Function

    ''' <summary>
    ''' map model to entity
    ''' </summary>
    ''' <param name="userAccount"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MapUser(userAccount As UserAccount) As LocalWorkSpaceInfo
        Dim user As New LocalWorkSpaceInfo()
        With user
            .UserId = userAccount.UserId
            .AccountId = userAccount.AccountId
            .UserAccountId = userAccount.UserAccountId
            '.AgentId = userAccount.UserAgents.FirstOrDefault().AgentId
            .WindowsUserSId = userAccount.WindowsUserSID
            .WorkSpaceId = userAccount.WorkSpaceId
            .CloudSyncServiceUrl = userAccount.CloudSyncServiceURL
            .CloudSyncSyncServiceUrl = userAccount.CloudSyncSyncServiceURL
            .OrganisationServiceURL = userAccount.OrganisationServiceURL
            .AccessToken = CommonUtils.Helper.Decrypt(userAccount.AccessToken)
        End With

        'If Not String.IsNullOrWhiteSpace(user.AccessToken) Then
        '    Dim secureDataFormat = New TicketDataFormat(New TokenProtector())
        '    Dim ticket As AuthenticationTicket = secureDataFormat.Unprotect(user.AccessToken)

        '    Dim cGivenName = ticket.Identity.Claims.FirstOrDefault(Function(p) p.Type = ClaimTypes.GivenName)
        '    If cGivenName IsNot Nothing Then
        '        user.Name = cGivenName.Value
        '    End If

        '    Dim cEmail = ticket.Identity.Claims.FirstOrDefault(Function(p) p.Type = ClaimTypes.Email)
        '    If cEmail IsNot Nothing Then
        '        user.Email = cEmail.Value
        '    End If

        '    Dim cRole = ticket.Identity.Claims.FirstOrDefault(Function(p) p.Type = ClaimTypes.Role)
        '    If cRole IsNot Nothing Then
        '        user.Role = CType(cRole.Value, Role)
        '    End If
        'End If

        Return user
    End Function

End Class
