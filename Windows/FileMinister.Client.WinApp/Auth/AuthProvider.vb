Imports FileMinister.Client.Common
Imports FileMinister.Client.Common.Model
Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.portable.Proxies
Imports risersoft.shared
Imports risersoft.shared.win
Namespace Auth
    Public Class AuthProvider

        Public Shared Property Parent As IForm
        Shared SelectAgent As SelectAgent


        Public Shared Event AgentValidated As EventHandler(Of AgentMacAddressInfo)
        Public Shared Event AuthenticationComplete As EventHandler(Of LocalWorkSpaceInfo)
        Public Shared Event SharedConfigured As EventHandler(Of LocalWorkSpaceInfo)
        Public Shared Event UserValidationCompleted As EventHandler(Of ShareSummaryInfo)
        'Public Shared Event OnNoShareFound As EventHandler(Of LocalWorkSpaceInfo)

        Shared Sub New()

            AddHandler AgentValidated, AddressOf OnAgentValidation
            AddHandler AuthData.Police.UserAuthenticated, AddressOf OnAuthenticated
            AddHandler AuthData.Police.AccountSelected, AddressOf OnAccountSelected

        End Sub

        Public Shared Sub OnAgentValidated(sender As Object, agent As AgentMacAddressInfo)
            RaiseEvent AgentValidated(sender, agent)
        End Sub
        Public Shared Sub ValidateUser(sender As Object)
            AuthProvider.OnAccountSelected(sender, AuthData.Police.UserAccount)
        End Sub
        Public Shared Sub OnAccountSelected(sender As Object, user As UserAccountProxy)
            Dim winUser = GetWindowUserName()
            Dim dbPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim wkSpace As New LocalWorkSpaceInfo
            With wkSpace
                .UserId = user.UserId
                .AccountId = user.AccountId
                .UserAccountId = user.UserAccountId
                .AccountName = user.AccountName
                .Email = user.Email
                .FullName = user.FullName
                .RoleId = user.UserTypeId
                .WindowsUserSId = winUser
                .AccessToken = AuthData.Police.AccessToken
                .LocalDatabaseName = dbPath
                .CloudSyncServiceUrl = AuthData.FileWebApiUrl
                .CloudSyncSyncServiceUrl = AuthData.SyncServiceUrl
                .OrganisationServiceURL = AuthData.AuthorityUrl
                .WindowsUserSId = winUser
            End With

            AuthData.User = wkSpace

            Using service As New LocalUserClient()
                Dim result = service.GetUserConfigured(user.UserId, user.AccountName, winUser)
                If (result.Status = 200) Then
                    wkSpace = result.Data
                    ConfigureClient(wkSpace)
                    Exit Sub
                ElseIf result.Status = 401 Then
                    ShowLogin()
                    Exit Sub
                End If
            End Using


            If user.UserTypeId = Role.AccountAdmin Then
                ConfigureClient(wkSpace)
            Else
                LogUser(wkSpace)
                ShowAgent()
            End If

        End Sub

        Public Shared Sub OnAuthenticationComplete(sender As Object, user As LocalWorkSpaceInfo)
            RaiseEvent AuthenticationComplete(sender, user)
        End Sub

        Public Shared Sub ShowAgent()
            SelectAgent = New SelectAgent
            SelectAgent.ShowDialog(Parent)
        End Sub

        Public Shared Sub ShowLogin(Optional switchOption As UserSwitchOptions = UserSwitchOptions.SwitchUser)
            If switchOption = UserSwitchOptions.SwitchAccount Then
                AuthData.Police.SwitchAccount(Parent)
            Else
                AuthData.Police.SwitchUser(Parent)
            End If
        End Sub

        Private Shared Sub OnAuthenticated(sender As Object, e As RSUser)
            'Do nothing and wait for account selection
        End Sub

        Private Shared Sub OnAgentValidation(sender As Object, e As AgentMacAddressInfo)
            AuthData.User.SelectedAgentId = e.FileAgentId
            AuthData.User.SelectedAgentName = e.AgentName
            ConfigureClient(AuthData.User)
        End Sub

        Private Shared Sub ConfigureAgent(AgentId As Guid)
            Using shareClient As New LocalShareClient()

                Dim shareSummary As ShareSummaryInfo = Nothing

                Dim result = shareClient.ShareMappedSummary

                If result.Status = 200 Then
                    shareSummary = result.Data
                End If

                If shareSummary Is Nothing OrElse (shareSummary.AccountShareCount = 0 AndAlso shareSummary.MappedShareCount = 0) Then
                    If SelectAgent IsNot Nothing Then
                        SelectAgent.Close()
                    End If
                Else
                    If shareSummary.MappedShareCount > 0 Then
                        'validationComplete = True
                        If SelectAgent IsNot Nothing Then
                            SelectAgent.Close()
                        End If
                    Else
                        If SelectAgent IsNot Nothing Then
                            SelectAgent.Hide()
                        End If
                        Dim config As New Config(AgentId)
                        config.ShowDialog(Parent)

                        If SelectAgent IsNot Nothing Then
                            SelectAgent.Close()
                        End If

                        RaiseEvent SharedConfigured(Nothing, AuthData.User)

                        Dim shareResult = shareClient.ShareMappedSummary
                        If shareResult.Status = 200 Then
                            shareSummary = shareResult.Data
                        End If
                    End If
                End If

                Using client As New SyncClient()
                    client.SyncServerData()
                End Using

                RaiseEvent UserValidationCompleted(Nothing, shareSummary)

            End Using
        End Sub

        Private Shared Sub ConfigureClient(user As LocalWorkSpaceInfo)
            LogUser(user)
            OnAuthenticationComplete(Nothing, user)
            SyncShares()
            ConfigureAgent(user.SelectedAgentId)

        End Sub

        Shared Sub LogUser(wkSpace As LocalWorkSpaceInfo)

            Using localUserClient As New LocalUserClient()

                Dim logResult = localUserClient.Log(wkSpace)
                If logResult.Status = 200 AndAlso logResult.Data IsNot Nothing Then
                    AuthData.User.FullName = logResult.Data.FullName
                    AuthData.User.UserAccountId = logResult.Data.UserAccountId
                    AuthData.User.AccountName = logResult.Data.AccountName
                    AuthData.User.RoleId = CType(logResult.Data.RoleId, Role)
                    AuthData.User.WindowsUserSId = logResult.Data.WindowsUserSId
                    AuthData.User.OrganisationServiceURL = logResult.Data.OrganisationServiceURL
                    AuthData.User.WorkSpaceId = logResult.Data.WorkSpaceId
                    AuthData.User.CloudSyncServiceUrl = logResult.Data.CloudSyncServiceUrl
                    AuthData.User.CloudSyncSyncServiceUrl = logResult.Data.CloudSyncSyncServiceUrl
                End If
            End Using
        End Sub



        Private Shared Sub SyncShares()
            Using localAgent As New LocalAgentClient()
                localAgent.SyncShares()
            End Using
        End Sub

        Private Shared Function GetWindowUserName() As String
            Dim winUser = Environment.UserDomainName + "\" + Environment.UserName
            Return winUser
        End Function
    End Class

    ' Public Delegate Sub UserAuthenticatedHandler(sender As Object, user As Users)


    Public Enum UserSwitchOptions
        SwitchUser
        SwitchAccount
    End Enum
End Namespace
