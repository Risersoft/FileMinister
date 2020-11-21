Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Http
Imports System.Web.Http.Cors

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Web API configuration and services

        UnityConfig.RegisterComponents(config)

        ' Web API routes
        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )

        ' config.Routes.MapHttpRoute(
        '    name:="DefaultApiGet",
        '    routeTemplate:="api/{controller}/{action}/{fileId}/{userId}",
        '    defaults:=New With {.action = "get", .fileId = RouteParameter.Optional}
        ')

        '  config.Routes.MapHttpRoute(
        '    name:="DefaultApiGet",
        '    routeTemplate:="api/{controller}/{action}/{fileId}/{userId}/{permission}/{typeOfPermission}/{changedBy}",
        '    defaults:=New With {.action = "get", .fileId = RouteParameter.Optional, .userId = RouteParameter.Optional, .permission = RouteParameter.Optional, .typeOfPermission = RouteParameter.Optional, .changedBy = RouteParameter.Optional}
        ')



    End Sub
End Module
