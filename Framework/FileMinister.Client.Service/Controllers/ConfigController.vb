Imports System.Web.Http
Imports risersoft.shared.portable.Model

''' <summary>
''' Share Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/Share")>
Public Class ConfigController
    Inherits LocalApiController(Of ConfigInfo, Integer, IConfigRepository)

    Public Sub New(repository As IConfigRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get all shares for all accounts
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AllowAnonymous>
    <Route("Account/Shares")>
    <HttpGet>
    Public Function GetAccountShares() As IHttpActionResult
        Dim result = repository.GetAccountShares()
        Return Ok(result)
    End Function

    ''' <summary>
    '''  Get all mapped share for current user and account by role
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GetShares")>
    <HttpGet>
    Public Function GetShares() As IHttpActionResult
        Dim result = repository.GetShares()
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Add UserShares
    ''' </summary>
    ''' <param name="lst"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddAll")>
    <HttpPost>
    Public Function AddAll(lst As List(Of ConfigInfo)) As IHttpActionResult
        Dim result = repository.AddAll(lst)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get all mapped shares against a user, account and role
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("MappedSummary")>
    <HttpGet>
    Public Function ShareMappedSummary() As IHttpActionResult
        Dim result = repository.ShareMappedSummary()
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get all shares by a user account
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GetAllShareByAccount")>
    <HttpGet>
    Public Function GetAllShareByAccount() As IHttpActionResult
        Dim result = repository.AllShareByAccount()
        Return Ok(result)
    End Function

    ''' <summary>
    '''  Delete a share and corresponding refrence records by share id
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteShare")>
    <HttpPost>
    Public Function DeleteShare(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim Id As Integer = data("shareId")
        Dim result = repository.DeleteShare(Id)
        Return Ok(result)
    End Function

    ''' <summary>
    '''  Delete a share and corresponding refrence records by share id
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteShareMapping")>
    <HttpPost>
    Public Function DeleteShareMapping(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim Id As Integer = data("shareId")
        Dim result = repository.DeleteShareMapping(Id)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Check if any existing share's path overlap with the paths in input list and vice versa
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("IsAnyPathExists")>
    <HttpPost>
    Public Function IsAnyPathExists(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim paths As List(Of String) = CType(data("paths"), Newtonsoft.Json.Linq.JArray).ToObject(Of List(Of String))()
        'CType(data("paths"), List(Of String))
        Dim accountId As Integer = CType(data("accountId"), Integer)
        Dim result = repository.IsAnyPathExists(paths, accountId)
        Return Ok(result)
    End Function

    '<Route("{userId}/{agentId}/AllSharesMapped")>
    '<HttpGet>
    'Public Function AllSharesMapped(userId As Integer, agentId As Guid) As IHttpActionResult
    '    Dim result = repository.AllSharesMapped(userId, agentId)
    '    Return Ok(result)
    'End Function

    '<Route("{agentId}/SharesMapped")>
    '<HttpGet>
    'Public Function IsShareMapped(agentId As Guid) As IHttpActionResult
    '    Dim result = repository.IsShareMapped(agentId)
    '    Return Ok(result)
    'End Function

End Class
