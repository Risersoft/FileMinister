Imports System
Imports System.Dynamic
Imports System.Windows.Forms
Imports Facebook
Imports System.Security.Claims
Imports Newtonsoft.Json

Public Class FacebookLoginDialog
    Private _loginUrl As Uri
    Protected _fb As FacebookClient
    'Private Const AppId As String = "1026586267392992"
    'Private Const AppId As String = "1043259222371900"
    Private Const AppId As String = "126505051048706"
    Private Const ExtendedPermissions As String = "public_profile"
    Private Const RedirectUri As String = "https://maximpriseauthv1.azurewebsites.net"




    Public Property FacebookOAuthResult() As FacebookOAuthResult
        Get
            Return m_FacebookOAuthResult
        End Get
        Private Set(value As FacebookOAuthResult)
            m_FacebookOAuthResult = value
        End Set
    End Property
    Private m_FacebookOAuthResult As FacebookOAuthResult

    'Public Sub New(appId As String, extendedPermissions As String)
    '    'Me.New(New FacebookClient(), appId, extendedPermissions)
    '    'Me.New(appId, extendedPermissions)
    'End Sub
    Public Sub FacebookLoginDialog(fb As FacebookClient, appId As String, extendedPermissions As String)
        If fb Is Nothing Then
            'Throw New ArgumentNullException("fb")
            fb = New FacebookClient()
        End If
        If String.IsNullOrWhiteSpace(appId) Then
            Throw New ArgumentNullException("appId")
        End If
        _fb = fb
        _loginUrl = GenerateLoginUrl(appId, extendedPermissions)
    End Sub
    ''' <summary>
    ''' Generating login Url to get token 
    ''' </summary>
    ''' <param name="appId"></param>
    ''' <param name="extendedPermissions"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateLoginUrl(appId As String, extendedPermissions As String) As Uri
        ' for .net 3.5
        ' var parameters = new Dictionary<string,object>
        ' parameters["client_id"] = appId;
        ' Option Strict  Off
        Dim parameters As Object = New ExpandoObject()
        parameters.client_id = appId
        ' parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";
        'parameters.redirect_uri = "https://localhost:44301/"
        ' parameters.redirect_uri = "https://maximpriseauthv1.azurewebsites.net"
        parameters.redirect_uri = RedirectUri
        ' The requested response: an access token (token), an authorization code (code), or both (code token).
        parameters.response_type = "token"
        ' list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
        parameters.display = "popup"

        ' add the 'scope' parameter only if we have extendedPermissions.
        If Not String.IsNullOrWhiteSpace(extendedPermissions) Then
            parameters.scope = extendedPermissions
        End If

        ' when the Form is loaded navigate to the login url.
        _fb = New FacebookClient()
        _
        Return _fb.GetLoginUrl(parameters)
    End Function

    ''' <summary>
    ''' Generate logout Url
    ''' </summary>
    ''' <param name="appId"></param>
    ''' <param name="extendedPermissions"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateLogoutUrl(appId As String, extendedPermissions As String) As Uri
        ' for .net 3.5
        ' var parameters = new Dictionary<string,object>
        ' parameters["client_id"] = appId;
        ' Option Strict  Off
        Dim parameters As Object = New ExpandoObject()
        parameters.client_id = appId
        ' parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";
        'parameters.redirect_uri = "https://localhost:44301/"
        'parameters.redirect_uri = "https://maximpriseauthv1.azurewebsites.net"
        parameters.redirect_uri = RedirectUri
        ' The requested response: an access token (token), an authorization code (code), or both (code token).
        parameters.response_type = "token"

        ' list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
        parameters.display = "popup"

        ' add the 'scope' parameter only if we have extendedPermissions.
        If Not String.IsNullOrWhiteSpace(extendedPermissions) Then
            parameters.scope = extendedPermissions
        End If

        ' when the Form is loaded navigate to the login url.
        _fb = New FacebookClient()
        _
        Return _fb.GetLogoutUrl(parameters) '   _fb.GetLoginUrl(parameters)
    End Function
    Private Sub FacebookLoginDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CommonModule.FacebookLookedIn = False
        Dim fb As New FacebookClient()
        _loginUrl = GenerateLoginUrl(AppId, ExtendedPermissions)
        ' make sure to use AbsoluteUri.
        webBrowser.Navigate(_loginUrl.AbsoluteUri)
    End Sub

    Private Sub webBrowser_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles webBrowser.Navigated
        ' whenever the browser navigates to a new url, try parsing the url.
        ' the url may be the result of OAuth 2.0 authentication.
        Dim oauthResult As FacebookOAuthResult
        If _fb.TryParseOAuthCallbackUrl(e.Url, oauthResult) Then
            ' The url is the result of OAuth 2.0 authentication
            FacebookOAuthResult = oauthResult
            MyOauthResult = oauthResult
            DialogResult = If(FacebookOAuthResult.IsSuccess, DialogResult.OK, DialogResult.No)
            If (DialogResult = Windows.Forms.DialogResult.OK) Then
                Dim fb As New FacebookClient(FacebookOAuthResult.AccessToken)
                Dim myProviderKey As String = fb.Get("me?fields=id").id.ToString()  ' to get provider key 
                'Dim result1 As Object = fb.[Get]("/me")
                'Dim myUser As New UserProviderinfo
                'myUser = JsonConvert.DeserializeObject(Of UserProviderinfo)(fb.Get("me?fields=id"))
                FacebookLookedIn = True
                ProviderKey = myProviderKey
                LoggedinUser = MaximpriseAuthServiceLoggedin(LinkedinEmail)

            Else
                FacebookLookedIn = False
            End If
        Else
            FacebookOAuthResult = Nothing
        End If
    End Sub
End Class