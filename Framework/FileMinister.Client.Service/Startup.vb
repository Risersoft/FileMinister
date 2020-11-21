Imports System.Net
Imports System.Security.Claims
Imports System.Text
Imports System.Web.Http
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.OAuth
Imports Newtonsoft.Json
Imports Owin
Imports risersoft.shared.messaging
Imports risersoft.shared.web
Public Class Startup

    Public Sub Configuration(app As IAppBuilder)
        Dim config = ConfigureWebApi()

        config.Filters.Add(New RSAuthorizeAttribute())

        Dim OAuthBearerOptions = New OAuthBearerAuthenticationOptions()
        OAuthBearerOptions.Provider = New CustomBearerAuthenticationProvider()
        OAuthBearerOptions.AuthenticationMode = AuthenticationMode.Active
        app.UseOAuthBearerAuthentication(OAuthBearerOptions)

        'ConfigureLoggers()

        app.Use(Of ValidateRequestMiddleware)()
        app.UseWebApi(config)

        MessageClientProvider.CreateInstance("client_service", Initiator.ClientService)

    End Sub

    Public Function ConfigureWebApi() As HttpConfiguration
        Dim config = New HttpConfiguration()

        config.MapHttpAttributeRoutes()
        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{id}",
            New With {.id = RouteParameter.Optional}
        )

        UnityConfig.RegisterComponents(config)

        Return config
    End Function




    Class CustomBearerAuthenticationProvider
        Inherits OAuthBearerAuthenticationProvider

        Public Overrides Function RequestToken(context As OAuthRequestTokenContext) As Task
            If context.Token IsNot Nothing Then
                Try
                    Dim access_token = context.Token

                    Dim tokenProtecter As New TokenProtector()

                    Dim claims = tokenProtecter.Unprotect(access_token)

                    Dim identity As New ClaimsIdentity(claims, "Bearer")
                    Dim principal As New ClaimsPrincipal(identity)
                    context.OwinContext.Authentication.User = principal
                Catch ex As Exception
                    context.Response.StatusCode = HttpStatusCode.BadRequest
                End Try
            End If

            Return MyBase.RequestToken(context)
        End Function
    End Class

    Private Class TokenProtector

        Public Function Unprotect(protectedText As String) As List(Of Claim)
            Dim bytes = Convert.FromBase64String(protectedText)
            Dim data = ASCIIEncoding.ASCII.GetString(bytes)
            Dim dic = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(data)

            Dim claims = dic.Select(Function(vp) New Claim(vp.Key, vp.Value)).ToList()

            Return claims
        End Function
    End Class

End Class

