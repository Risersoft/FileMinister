Imports FileMinister.Client.WinApp.Auth
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.cloud

Public Class Login
    Dim strProviderName As String = String.Empty
    Dim strOtp As String = String.Empty
    Public AccessToken As String
    Private _accessToken As String
    Public GUser As New Users()
    Dim Accountid As Integer
    Dim mUserId As Integer
    Dim mUserEmail As String
    Dim isTwoFactorAuthentication As Boolean
    Dim PhoneNumber As String

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddHandler AuthProvider.AccountSelected, AddressOf OnAccountSelected

        pnlOTP.Visible = False
        pnlPassword.Visible = False

#If Not DEBUG Then
        lblMACAddressesLabel.Visible = False
        lbMACAddresses.Visible = False
#End If

#If DEBUG Then
        For Each item In CommonUtils.Helper.GetMacAddresses()
            lbMACAddresses.Items.Add(item)
        Next
#End If
        'lblMACAddress.Text = CommonUtils.Helper.getMacAddress()
    End Sub

    Private Async Sub btnAuthCheck_Click(sender As Object, e As EventArgs) Handles btnAuthCheck.Click
        If txtUserName.Text.Trim().Length = 0 Then
            MessageBoxHelper.ShowInfoMessage("Please Enter User Name")
            Exit Sub
        End If
        Try

            Using accountClient As New Client.Common.AccountClient()
                Dim result = Await accountClient.GetAuthProviderAsync(txtUserName.Text.Trim())
                If (result.StatusCode = 200) Then
                    Dim users = result.Data

                    If Not String.IsNullOrWhiteSpace(users.ErrorMessage) Then
                        MessageBoxHelper.ShowErrorMessage(users.ErrorMessage)
                        Exit Sub
                    End If

                    strProviderName = users.ProviderName
                    If (Not String.IsNullOrEmpty(users.phone)) Then
                        users.PhoneNumber = users.phone
                    End If

                    PhoneNumber = users.PhoneNumber
                    GUser.PhoneNumber = PhoneNumber
                    GUser.UserId = users.UserId
                    GUser.Email = users.EmailID
                    GUser.ProviderName = users.ProviderName
                    GUser.Password = users.Password
                    ProviderKey = users.ProviderKey
                    isTwoFactorAuthentication = users.IsTwoFactorAuthentication
                    GUser.IsTwoFactorAuthentication = users.IsTwoFactorAuthentication

                    pnlPassword.Visible = False
                    btnLogin.Visible = False

                    If strProviderName = "Facebook" Then
                        btnFaceBook.Visible = True
                    ElseIf strProviderName = "LinkedIn" Then
                        btnLinkedIn.Visible = True
                    ElseIf strProviderName = "Google" Then
                        btnGoogle.Visible = True
                    ElseIf strProviderName = "AzureAd" Then
                        btnAzureAd.Visible = True
                    Else
                        pnlPassword.Visible = True
                        txtPassword.Focus()

                        btnLogin.Visible = True
                    End If

                    btnEnd.Visible = True
                    btnAuthCheck.Visible = False
                    gpAuthorisation.Location = New System.Drawing.Point(6, 12)
                Else
                    Dim message As String = Nothing
                    If result.Message IsNot Nothing Then
                        message = result.Message
                    Else
                        message = "Something went wrong. Please try again after sometime"
                    End If
                    MessageBoxHelper.ShowErrorMessage(message)
                End If
            End Using


        Catch ex As Exception
            MessageBoxHelper.ShowErrorMessage(ex.Message)
        End Try
    End Sub

    Private Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Me.Cursor = Cursors.WaitCursor

        Try
            If txtUserName.Text.Trim().Length = 0 Then
                MessageBoxHelper.ShowInfoMessage("Please Enter User Name")
                Me.Cursor = Cursors.Default
                Exit Sub
            Else
                If Not EmailValidation(txtUserName) Then
                    Me.Cursor = Cursors.Default
                    Exit Sub
                End If
            End If
            Dim username = txtUserName.Text, password = txtPassword.Text

            Dim userData = Await AuthenticateUserAsync(username, password)
            If userData Is Nothing Then
                MessageBoxHelper.ShowErrorMessage("User not found")
                Me.Cursor = Cursors.Default
                Exit Sub
            ElseIf userData.SessionId Is Nothing Then
                MessageBoxHelper.ShowErrorMessage(userData.ErrorMessage.ToString())
                Me.Cursor = Cursors.Default
                Exit Sub
            End If

            If userData.UserTypeId = 2 Then
                MessageBoxHelper.ShowErrorMessage("No Account exist for this user")
                Me.Cursor = Cursors.Default
                Exit Sub
            End If

            userData.AuthenticationProviderId = 1
            userData.Id = userData.UserId
            GUser.AccountId = userData.AccountId
            GUser.Email = userData.Email
            GUser.SessionId = userData.SessionId
            GUser.Id = userData.Id
            GUser.UserId = userData.UserId
            GUser.AccessToken = userData.access_token
            CommonModule.SessionId = userData.SessionId
            CommonModule.UserId = userData.UserId

            If strProviderName = "Self" Then
                If (Not GUser.IsTwoFactorAuthentication) Then
                    AuthProvider.OnUserAuthenticated(Nothing, userData)
                Else
                    pnlOTP.Visible = True
                    txtPassword.Enabled = False
                    pnlOTP.Focus()

                    btnOtpVerify.Visible = True
                    pnlLogin.Visible = False

                    Using accountClient As New Client.Common.AccountClient()
                        Dim result = Await accountClient.SendOTPAsync(GUser)
                        If (result.StatusCode = 200) Then
                            Dim users = result.Data
                            GUser = userData
                            CommonModule.LoginOtp = users.otp
                        End If
                    End Using
                End If

            End If

            Me.Cursor = Cursors.Default

        Catch ex As Exception
            MessageBoxHelper.ShowErrorMessage(ex.Message)
        End Try
    End Sub

    Private Sub btnOtpVerify_Click(sender As Object, e As EventArgs) Handles btnOtpVerify.Click
        If CommonModule.LoginOtp = txtOTP.Text.Trim() Then
            AuthProvider.OnUserAuthenticated(Nothing, GUser)
        Else
            MessageBox.Show("OTP Mismatch")
        End If
    End Sub

    Public Sub LoadAccount()
        GetUserAccountAsync()
    End Sub

    Private Async Sub GetUserAccountAsync(Optional ByVal userEmail As String = "")
        Dim myDeserializedObjList As List(Of ActiveUser) = Nothing
        Using userClient As New Common.AccountClient
            Dim result = Await userClient.GetAccountsAsync()
            If (result.StatusCode = 200) Then
                myDeserializedObjList = result.Data
            Else
                If Not String.IsNullOrWhiteSpace(result.Message) Then
                    MessageBoxHelper.ShowErrorMessage(result.Message)
                    Exit Sub
                End If
            End If
        End Using


        If (myDeserializedObjList Is Nothing OrElse myDeserializedObjList.Count = 0) Then
            MsgBox("No Account exist for this user")
            pnlLogin.Visible = True
            Exit Sub
        End If

        cbAccounts.DisplayMember = "AccountName"
        cbAccounts.ValueMember = "AccountId"
        cbAccounts.DataSource = myDeserializedObjList

        gpAuthorisation.Visible = False
        pnlAccount.Visible = True

    End Sub

    Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

    Private Sub ExternalLogin_Clicked(sender As Object, e As EventArgs) Handles btnFaceBook.Click, btnGoogle.Click, btnLinkedIn.Click, btnAzureAd.Click
        Dim form As New ExternalLoginDialog()
        form.Email = txtUserName.Text
        form.Provider = strProviderName
        form.ShowDialog()
    End Sub

    Private Sub btnAccountNext_Click(sender As Object, e As EventArgs) Handles btnAccountNext.Click
        If String.IsNullOrWhiteSpace(cbAccounts.SelectedValue) Then
            MessageBoxHelper.ShowInfoMessage("Please select account")
            Exit Sub
        End If

        Dim selectedAccount = CType(cbAccounts.SelectedItem, ActiveUser)

        If selectedAccount IsNot Nothing Then
            GUser.AccountId = selectedAccount.AccountId
            GUser.AccountName = selectedAccount.AccountName

            AuthData.User.AccountId = selectedAccount.AccountId
            AuthData.User.AccountName = selectedAccount.AccountName
            AuthData.User.OrganisationServiceURL = selectedAccount.OrganisationServiceURL
            AuthData.User.CloudSyncServiceUrl = selectedAccount.ServiceUrl
            AuthData.User.CloudSyncSyncServiceUrl = selectedAccount.SyncServiceUrl

            Using client As New Common.OrganizationClient()
                Dim result = client.GetRoleByAccount(AuthData.User.AccountId)
                If result.StatusCode = 200 AndAlso result.Data IsNot Nothing Then
                    AuthData.User.RoleId = CType(result.Data.Id, Role)

                    AuthProvider.OnAccountSelected(Nothing, AuthData.User)
                Else
                    MessageBoxHelper.ShowErrorMessage(If(result.Message IsNot Nothing, result.Message, String.Format("No role found for account '{0}'", selectedAccount.AccountName)))
                End If
            End Using
        Else
            MessageBoxHelper.ShowErrorMessage("No account selected")
            Return
        End If


    End Sub

    Private Sub btnNextAgentDetail_Click(sender As Object, e As EventArgs) Handles btnNextAgentDetail.Click
        If String.IsNullOrWhiteSpace(txtAgent.Text) Then 'Or String.IsNullOrWhiteSpace(txtMACAddress.Text) Then
            MessageBoxHelper.ShowWarningMessage("Agent and Secret Key are required")
            Exit Sub
        End If

        Using service As New AgentClient()
            'Dim result = service.ValidateAgentWithMAC(txtAgent.Text.Trim(), txtMACAddress.Text.Trim())
            Dim secretKeyEncrypted = CommonUtils.Helper.Encrypt(txtSecretKey.Text.Trim())
            Dim result = service.ValidateAgentWithMAC(txtAgent.Text.Trim(), CommonUtils.Helper.GetMacAddresses(), secretKeyEncrypted)
            If result.Data Is Nothing Then
                MessageBoxHelper.ShowErrorMessage("Invalid Agent or Secret Key")
                Exit Sub
            End If

            If (result.Data IsNot Nothing) Then
                AuthData.User.AgentId = result.Data.AgentId
                AuthData.User.AgentName = result.Data.AgentName

                Dim data = result.Data
                GUser.AgentId = data.AgentId
                GUser.AgentName = data.AgentName
                GUser.MACAddress = data.AgentMacAddress
                Me.Close()

                AuthProvider.OnAgentValidated(Nothing, GUser)
            Else
                MessageBoxHelper.ShowErrorMessage("Oops! something went wrong. Please try again after sometime.")
            End If

        End Using
    End Sub

    Public Sub ShowAddAgent()
        GUser.AccountId = AuthData.User.AccountId
        GUser.AccountName = AuthData.User.AccountName

        pnlLogin.Visible = False
        pnlAccount.Visible = False
        pnlAgentDetail.Visible = True

        Me.Text = "Agent"

        ShowDialog()
    End Sub

    Public Sub ShowSwitchAccountAsync()
        pnlLogin.Visible = False
        pnlAccount.Visible = True

        LoadAccount()

        pnlAgentDetail.Visible = False
        ShowDialog()
    End Sub

    Private Sub OnAccountSelected(sender As Object, user As WorkSpaceInfo)
        If user.RoleId = Role.User Then
            pnlAccount.Visible = False
            pnlAgentDetail.Visible = True
        Else
            'AuthProvider.OnAccountSelection(Nothing, user)
        End If
    End Sub
End Class