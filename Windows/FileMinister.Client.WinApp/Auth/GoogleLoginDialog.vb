
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Google
Imports Google.Apis
Imports Google.Apis.Auth.OAuth2.Responses
Imports Google.Apis.Auth.OAuth2.Flows
Imports Google.Apis.Auth.OAuth2
Imports System.Threading
Imports Google.Apis.Oauth2.v2
Imports Google.Apis.Oauth2.v2.Data
Imports System.IO


Public Class GoogleLoginDialog
    Public googleLiginUri As Uri
    Private _loginUri As Uri

    Private _callBackUrl As String
    Private _OAuthVerifierToken As String
    Private Const redirectUri As String = "urn:ietf:wg:oauth:2.0:oob"
    Private clientId As String = "425307548588-b3dobkv1lukavh0u3ccfqh44sc13vea8.apps.googleusercontent.com"
    Private clientSecrate As String = "uTKrWOLQm2AZ1P76dPCXF8GT"
    Public ReadOnly Property OAuthVerifierToken() As String
        Get
            Return _OAuthVerifierToken
        End Get
    End Property

    Private Sub GoogleLoginDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'Dim strRedirectUri As String = "http://localhost:63599/product/maximprise"
            ' Dim strRedirectUri As String = "http://localhost:63599/product/maximprise"

            'Dim strRedirectUri As String = "http://maximprisev1.azurewebsites.net/product/maximprise"

            Dim strRedirectUri As String = redirectUri ' "urn:ietf:wg:oauth:2.0:oob"
            'Dim strRedirectUri As String = "urn:ietf:wg:oauth:2.0:auto"


            Dim strClientId As String = clientId ' "425307548588-b3dobkv1lukavh0u3ccfqh44sc13vea8.apps.googleusercontent.com"
            Dim strState As String = clientSecrate '"uTKrWOLQm2AZ1P76dPCXF8GT"
            Dim strUrl As String = "https://accounts.google.com/o/oauth2/auth?client_id=" & strClientId & "&response_type=code&scope=email&redirect_uri=" & strRedirectUri & "&state=" & strState & ""
            ''Dim strUrl As String = "https://accounts.google.com/o/oauth2/auth?client_id=425307548588-b3dobkv1lukavh0u3ccfqh44sc13vea8.apps.googleusercontent.com&response_type=code&scope=email&redirect_uri=https://localhost:44301/&state=jSlNE0mVGHyG6hX8Mf6yanJl"
            ''Dim strUrl As String = "https://accounts.google.com/o/oauth2/v2/auth?client_id=755166673635-3ognitjrnfmaokg97n1cm3422aos0hq9.apps.googleusercontent.com&response_type=code&scope=email&redirect_uri=https://maximpriseauthv1.azurewebsites.net/signin-google&state=jSlNE0mVGHyG6hX8Mf6yanJl"
            ''Dim aa = "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=756lc6ivjz990i&redirect_uri=http://maximprisev1.azurewebsites.net/product/maximprise/signin-linkedin&state=5vmVbEClTGNBPqEP&scope=r_basicprofile"
            Dim _uri As New Uri(strUrl)
            GoogleBrowser.Url = _uri
            'AddHandler GoogleBrowser.DocumentCompleted, AddressOf DocComplete
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
    End Sub

    Private Sub GoogleBrowser_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles GoogleBrowser.Navigated
        '  Dim token As TokenResponse
        Dim aa As Uri = e.Url
        Dim code As String = String.Empty
        Dim gCode As String = String.Empty

        If aa.ToString().Contains("code=") Or GoogleBrowser.DocumentTitle.Contains("code=") Then
            If aa.ToString().Contains("code=") Then
                code = aa.Query.Split("&")(1).Split("=")(1)
            ElseIf GoogleBrowser.DocumentTitle.Contains("code=") Then
                code = GoogleBrowser.DocumentTitle.Split("&")(1).Split("=")(1)
            End If

            'Dim redirectURI As String = "http://localhost:63599/product/maximprise"
            'Dim redirectURI As String = "http://maximprisev1.azurewebsites.net/product/maximprise"


            Dim redirectURI As String = "urn:ietf:wg:oauth:2.0:oob"
            Dim tokenRequest = DirectCast(WebRequest.Create("https://accounts.google.com/o/oauth2/token"), HttpWebRequest)
            'authorization_
            Dim postData As String = String.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", code, "425307548588-b3dobkv1lukavh0u3ccfqh44sc13vea8.apps.googleusercontent.com", "uTKrWOLQm2AZ1P76dPCXF8GT", redirectURI)
            Dim data = Encoding.ASCII.GetBytes(postData)
            tokenRequest.Method = "POST"
            tokenRequest.ContentType = "application/x-www-form-urlencoded"
            tokenRequest.ContentLength = data.Length
            Using stream = tokenRequest.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using
            Dim tokenResponse = DirectCast(tokenRequest.GetResponse(), HttpWebResponse)
            Dim responseString = New StreamReader(tokenResponse.GetResponseStream()).ReadToEnd()

            Dim tokenResult = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(responseString)
            Dim access_token = tokenResult("access_token") '  "ya29.ZgJAY_QywA7t4oXx777p4MZLONPE6eL-p2vZSeqQZrivQMH7Y7hqybuKrparep37-78x" ' tokenResult("access_token")
            CommonModule.GoogleToken = access_token
            Dim service As New Oauth2Service(New Google.Apis.Services.BaseClientService.Initializer())
            Dim request2 As Oauth2Service.TokeninfoRequest = service.Tokeninfo()
            request2.AccessToken = access_token '"ya29.ZgJAY_QywA7t4oXx777p4MZLONPE6eL-p2vZSeqQZrivQMH7Y7hqybuKrparep37-78x"
            Dim info As Tokeninfo = request2.Execute()
            DialogResult = If(tokenResponse.StatusCode = HttpStatusCode.OK, DialogResult.OK, DialogResult.No)
            If (DialogResult = Windows.Forms.DialogResult.OK) Then
                GoogleLogin = True
                ProviderKey = info.UserId
                If (CommonModule.CUserProvider.EmailID.ToLower() = info.Email.ToLower()) Then
                    LoggedinUser = MaximpriseAuthServiceLoggedin(info.Email)
                 


                    Me.Close()
                Else
                    GoogleLogin = False
                End If
            End If

        End If


        ' Dim title As String = DirectCast(GoogleBrowser.iInvokeScript("eval", "document.title.toString()"), String)

        'If title.StartsWith("Success") Then
        '    Dim authorizationCode As String = title.Substring(title.IndexOf("="c) + 1)
        '    PhoneApplicationService.Current.State("OAuth_Demo.AuthorizationCode") = authorizationCode
        '    App.loggedin = True
        '    'MessageBox.Show(authorizationCode);
        '    NavigationService.GoBack()
        'End If

    End Sub

    Private Sub DocComplete(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
        'Dim gUsername As String = ""
        'Dim gPassword As String = ""
        'Dim gCode As String = ""
        'Dim browser As WebBrowser = sender
        'Dim elementsInput As HtmlElementCollection = browser.Document.GetElementsByTagName("INPUT")
        'For Each element As HtmlElement In elementsInput
        '    If element.Name = "Email" Then element.SetAttribute("Value", gUsername)
        '    If element.Name = "Passwd" Then element.SetAttribute("Value", gPassword)
        'Next
        'Dim obj As HtmlDocument = browser.Document
        'Dim btn As HtmlElement = obj.GetElementById("signIn")
        'Dim btn2 As HtmlElement = obj.GetElementById("submit_approve_access")
        'If Not btn Is Nothing Then btn.InvokeMember("click")
        'If Not btn2 Is Nothing Then btn2.InvokeMember("click")
        'If obj.Title.Length > 12 Then
        '    If Microsoft.VisualBasic.Left(obj.Title, 12) = "Success code" Then
        '        gCode = Microsoft.VisualBasic.Right(obj.Title, obj.Title.Length - 13)

        '    End If

        'End If

    End Sub

End Class