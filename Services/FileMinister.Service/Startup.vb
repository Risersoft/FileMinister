Imports Owin
Imports Microsoft.Owin
Imports System.Web.Http
Imports Microsoft.Owin.Security.OAuth
Imports FileMinister.Repo.Util
Imports Microsoft.Owin.Security
Imports System.Threading.Tasks
Imports System.Security.Claims
Imports Microsoft.Owin.Security.DataHandler
Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Net.Http.Headers

<Assembly: OwinStartup(GetType(Startup))>

Partial Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        Dim config = ConfigureWebApi()

        'Enable Cors
        config.EnableCors(New Cors.EnableCorsAttribute("*", "*", "*"))

        Dim OAuthBearerOptions = New OAuthBearerAuthenticationOptions()
        app.UseOAuthBearerAuthentication(OAuthBearerOptions)

        app.UseWebApi(config)

        ServerDataSyncManager.Instance.Start()

        Dim properties = New BuilderProperties.AppProperties(app.Properties)
        Dim token = properties.OnAppDisposing
        If (token <> Threading.CancellationToken.None) Then

            token.Register(Sub()
                               ServerDataSyncManager.Instance.Stop()
                           End Sub)
        End If
    End Sub

    Public Function ConfigureWebApi() As HttpConfiguration
        Dim config = New HttpConfiguration()

        config.Filters.Add(New RSAuthorizeAttribute())
        config.EnableCors(New Cors.EnableCorsAttribute("*", "*", "*"))
        config.MessageHandlers.Add(New CustomHeaderHandler())

        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{id}",
            New With {.id = RouteParameter.Optional}
        )

        UnityConfig.RegisterComponents(config)

        Return config
    End Function
End Class
Public Class CustomHeaderHandler
    Inherits DelegatingHandler
    Protected Overrides Async Function SendAsync(request As HttpRequestMessage, cancellationToken As CancellationToken) As Task(Of HttpResponseMessage)
        Dim response = Await MyBase.SendAsync(request, cancellationToken)
        response.Headers.CacheControl = New CacheControlHeaderValue() With {.NoCache = True}
        Return response
    End Function
End Class
'https://www.codeproject.com/Articles/682296/Setting-Cache-Control-HTTP-Headers-in-Web-API-Cont
'https://forums.asp.net/t/2094391.aspx?Adding+default+header+CACHE+CONTROL+NO+CACHE+for+OWIN+self+host