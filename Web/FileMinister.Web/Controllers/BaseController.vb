Imports System.Web.Mvc
Imports Microsoft.Owin.Infrastructure
Imports risersoft.shared.cloud.common
Imports risersoft.shared.web

Namespace Controllers
    Public MustInherit Class BaseController
        Inherits clsMvcControllerBase
        ' GET: Base
        Protected ReadOnly Property AuthorityUrl As String
            Get
                Return CommonModuleBase.GetConfigSetting("authority")
            End Get
        End Property
        Protected ReadOnly Property RedirectUrl As String
            Get
                Dim path As String = CommonModuleBase.GetBaseUrl
                Dim dic1 = New Dictionary(Of String, String)()
                Dim str1 As String = WebUtilities.AddQueryString(path + "/account", dic1)
                Return str1
            End Get
        End Property
        Protected ReadOnly Property ClientId As String
            Get
                Return CommonModuleBase.GetConfigSetting("clientid")
            End Get
        End Property

        Protected ReadOnly Property ClientKey As String
            Get
                Return CommonModuleBase.GetConfigSetting("clientsecret")
            End Get
        End Property
        Protected ReadOnly Property WebWorkSpaceId As String
            Get
                Return ConfigUtility.ReadSetting("webWorkSpaceId")
            End Get
        End Property
        Protected ReadOnly Property LocalUrl As String
            Get
                Return CommonModuleBase.GetBaseUrl
            End Get
        End Property

        Protected Friend Sub SetViewBagProperty()
            ViewBag.authorityUrl = AuthorityUrl
            ViewBag.redirectUrl = RedirectUrl
            ViewBag.webWorkSpaceId = WebWorkSpaceId
            ViewBag.clientId = ClientId
            ViewBag.localUrl = LocalUrl
        End Sub

    End Class
End Namespace