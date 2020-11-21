Imports FileMinister.Client.WinApp.Auth
Imports risersoft.shared.portable.Model

Public Class ExternalLoginDialog

    Public Property Email As String
    Public Property Provider As String

    Dim serviceUrl As String = New Client.Common.AccountClient().GetServiceUrl()
    Dim urlTemplate As String = serviceUrl + "/api/account/externalLogin?provider={0}&source={1}&authId={2}&tempuserId={3}&facebookemail={4}&response_type=token&client_id=AuthApp&redirect_uri={5}"
    Dim redirectURI As String = serviceUrl + "/mmm" ''"urn:ietf:wg:oauth:2.0:oob"

    Private Sub ExternalLoginDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim url = GenerateLoginUrl()
        wbExternalLogin.Navigate(url)

    End Sub

    Private Function GenerateLoginUrl() As String
        Dim url As String = String.Format(urlTemplate, Provider, "login", 0, 0, Me.Email, redirectURI)
        Return url
    End Function

    Private Sub wbExternalLogin_DocumentTitleChanged(sender As Object, e As EventArgs) Handles wbExternalLogin.DocumentTitleChanged
        Me.Text = wbExternalLogin.DocumentTitle
    End Sub

    Private Sub wbExternalLogin_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles wbExternalLogin.Navigated
        If e.Url.AbsoluteUri.StartsWith(redirectURI) Then
            Dim url = e.Url.AbsoluteUri
            Dim qs = url.Split("#")(1).Split("&").FirstOrDefault(Function(p) p.StartsWith("access_token="))
            Dim access_token = qs.Split("=")(1)

            'AuthData.AccessToken = access_token
            Using userClient As New Common.OrganizationClient(New WorkSpaceInfo With {.AccessToken = access_token, .OrganisationServiceURL = serviceUrl})
                Dim result = userClient.GetUserDetail()
                If (result.StatusCode = 200) Then
                    Dim user = result.Data
                    LoggedinUser = user
                    user.AccessToken = access_token
                    SessionId = user.SessionId
                    UserId = user.UserId
                    AccountId = user.AccountId

                    Me.Close()

                    AuthProvider.OnUserAuthenticated(Nothing, user)
                Else
                    MessageBoxHelper.ShowErrorMessage("Something went wrong. Please try again after sometime")
                End If
            End Using


        End If
    End Sub

    'Private Sub wbExternalLogin_NavigateError(
    '       ByVal sender As Object,
    '       ByVal e As WebBrowserNavigateErrorEventArgs) _
    '       Handles wbExternalLogin.NavigateError

    '    ' Display an error message to the user.
    '    ' MessageBox.Show("Cannot navigate to " + e.Url)

    'End Sub

End Class