Imports System.Net.Http
Imports System.Web.Mvc
Imports Microsoft.Owin
Imports Newtonsoft.Json
Imports risersoft.shared.web

Namespace Controllers
    Public Class AccountController
        Inherits BaseController

        ' GET: Account
        Function Index(code As String) As ActionResult

            Dim objAuthencation As FileMinisterAuthentication = New FileMinisterAuthentication(Me.ClientId, Me.ClientKey, code, Me.AuthorityUrl, Me.LocalUrl)

            'get access token,refresh token and assign to viewbag.
            Dim accessTokenInfo As AccessTokenInfo = objAuthencation.GetAccessToken()
            If accessTokenInfo IsNot Nothing Then
                Dim context As IOwinContext = HttpContext.GetOwinContext
                OwinHelper.SigninJwt(context, Host, accessTokenInfo.AccessToken.ToString(), OwinHelper.Application)
                'ViewBag.access_token = accessTokenInfo.AccessToken
                'ViewBag.refresh_token = accessTokenInfo.RefreshToken
            End If

            Me.SetViewBagProperty()
            Return View()
        End Function

        Function Refresh(refreshToken As String) As ActionResult
            Dim objAuthencation As FileMinisterAuthentication = New FileMinisterAuthentication(Me.ClientId, Me.ClientKey, Me.AuthorityUrl)
            'get access token,refresh token and assign to viewbag.
            Dim accessTokenInfo As AccessTokenInfo = objAuthencation.RefreshAccessToken(refreshToken)
            If accessTokenInfo IsNot Nothing Then
                ViewBag.access_token = accessTokenInfo.AccessToken
                ViewBag.refresh_token = accessTokenInfo.RefreshToken
            End If

            Me.SetViewBagProperty()
            Return View("Index")

        End Function

        Public Function Logout() As ActionResult
            Dim ctx As IOwinContext = Request.GetOwinContext()
            Dim str3 = OwinHelper.Logout(ctx, provider, Me.Host)
            Me.DeleteXSRFCookie()
            Return Redirect2(str3)
        End Function

    End Class
End Namespace