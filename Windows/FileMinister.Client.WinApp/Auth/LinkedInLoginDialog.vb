Imports LinkedIn
Imports LinkedIn.NET
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class LinkedInLoginDialog

    Private _loginUrl As Uri
    Protected _ln As LinkedInClient
    Dim strRedirectUri As String = "http://maximprisev1.azurewebsites.net/product/maximprise/signin-linkedin"
    'Dim strRedirectUri As String = "http://localhost:63591/signin-linkedin"
    Dim strClientId As String = "75cbj6dc6ogpiz"
    Dim strState As String = "b6PC2uWqf7YWMVzK"
    Dim strScope As String = "r_emailaddress"

    Private Sub LinkedInLoginDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'Dim strRedirectUri As String = "http://maximprisev1.azurewebsites.net/product/maximprise/signin-linkedin"
            'Dim strClientId As String = "756lc6ivjz990i"
            'Dim strState As String = "5vmVbEClTGNBPqEP"
            'Dim strScope As String = "r_basicprofile"
            Dim strUrl As String = "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=" & strClientId & "&redirect_uri=" & strRedirectUri & "&state=" & strState & "&scope=" & strScope & ""
            ' Dim strUrl As String = "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=756lc6ivjz990i&redirect_uri=http://maximprisev1.azurewebsites.net/product/maximprise/signin-linkedin&state=5vmVbEClTGNBPqEP&scope=r_basicprofile"
            'Dim strUrl As String = "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=75cbj6dc6ogpiz&redirect_uri=https://maximpriseauthv1.azurewebsites.net/signin-linkedin&state=b6PC2uWqf7YWMVzK&scope=r_basicprofile"
            Dim _uri As New Uri(strUrl)
            linkedInBrowser.Url = _uri

        Catch ex As Exception

        End Try
    End Sub

    Private Sub linkedInBrowser_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles linkedInBrowser.Navigated
        Dim aa As Uri = e.Url
        Dim code As String = String.Empty
        Dim users As New Users
        If aa.ToString().Contains("code=") Then
            code = aa.Query.Split("&")(0).Split("=")(1)
            Dim ln As New LinkedInClient(strClientId, strState)
            Dim lnResponse As LinkedInResponse(Of LinkedInOauth) = ln.GetAccessToken(code, strRedirectUri)
 
            DialogResult = If(lnResponse.Status = LinkedInResponseStatus.OK, DialogResult.OK, DialogResult.No)
            If (DialogResult = Windows.Forms.DialogResult.OK) Then
                LinkedInLogIn = True
                ProviderKey = ln.CurrentUser.Id

                LoggedinUser = MaximpriseAuthServiceLoggedin(LinkedinEmail)




           

        Else
            LinkedInLogIn = False
        End If
            ' MessageBox.Show("Hi" & ln.CurrentUser.FirstName & " " & ln.CurrentUser.LastName)
        End If

        Dim l As Integer = aa.ToString().Length
    End Sub
End Class