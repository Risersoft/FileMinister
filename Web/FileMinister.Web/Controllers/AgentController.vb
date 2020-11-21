Imports FileMinister.Models.Sync
Imports System.Web.Http
Imports Newtonsoft.Json

''' <summary>
''' FileAgent Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/FileAgent")>
Public Class AgentController
    Inherits ServerApiController(Of FileAgentInfo, Guid, IAgentRepository)

    Public Sub New(repository As IAgentRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Validate FileAgent
    ''' </summary>
    ''' <param name="data">AgentName,Secret Key and Mac Address List</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("validate")>
    <HttpPost>
    Public Function ValidateAgentWithMAC(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim agentId = New Guid(data("AgentId").ToString())
        Dim secretKey = data("SecretKey").ToString()
        'Dim macAddresses = CType(data("MacAddresses"), List(Of String))
        Dim macAddresses = JsonConvert.DeserializeObject(Of List(Of String))(data("MacAddresses").ToString())
        Dim result = repository.ValidateAgentWithMAC(agentId, secretKey, macAddresses)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get all FileShares Mapped to FileAgent
    ''' </summary>
    ''' <param name="FileAgentId">FileAgentId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{FileAgentId}/FileShares")>
    <HttpGet>
    Public Function GetSharesByAgentId(FileAgentId As Guid) As IHttpActionResult
        Dim result = repository.GetSharesByAgentId(FileAgentId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get All FileAgent FileShare Mapped Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AgentMapDetails")>
    <HttpGet>
    Public Function AgentMapDetails() As IHttpActionResult
        Dim result = repository.AgentMapDetails()
        Return Ok(result)
    End Function
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="workSpaceId"></param>
    '''' <returns></returns>
    '<Route("{workSpaceId}/localshares")>
    '<HttpGet>
    'Public Function GetLocalShares(workSpaceId As String) As IHttpActionResult
    '    Dim id As Guid = New Guid(workSpaceId)
    '    Dim result = repository.GetLocalShares(id)
    '    Return Ok(result)
    'End Function

End Class
