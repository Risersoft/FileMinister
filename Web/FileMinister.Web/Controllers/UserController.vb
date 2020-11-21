Imports System.Web.Http
Imports FileMinister.Models.Sync
Imports risersoft.shared.portable.Model

<RoutePrefix("api/user")>
Public Class UserController
    Inherits ServerApiController(Of RSCallerInfo, Integer, IUserRepository)

    Public Sub New(repository As IUserRepository)
        MyBase.New(repository)
    End Sub
    <Route("GetUserBySID/{domainId}/{sId}")>
    <HttpGet>
    Public Function GetUserBySID(sId As String, domainId As Integer) As IHttpActionResult
        Dim result = repository.GetUserBySID(sId, domainId)
        Return Ok(result)
    End Function

    <Route("{domainId}/Users")>
    <HttpGet>
    Public Function GetDomainUsers(domainId As Integer) As IHttpActionResult
        Dim result = repository.GetDomainUsers(domainId)
        Return Ok(result)
    End Function

    <Route("GetAccountAdmins")>
    <HttpGet>
    Public Function GetAccountAdmins() As IHttpActionResult
        Dim result = repository.GetAccountAdmins()
        Return Ok(result)
    End Function
End Class
