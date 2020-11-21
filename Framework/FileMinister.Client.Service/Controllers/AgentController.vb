Imports System.Web.Http
Imports risersoft.shared.portable.Model

''' <summary>
''' Agent Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/agent")>
Public Class AgentController
    Inherits LocalApiController(Of FileAgentInfo, Guid, IAgentRepository)


    Public Sub New(repository As IAgentRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Synchrozing all shares against an agent
    ''' </summary>
    ''' <param name="agentId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{agentId}/sync-share")>
    <HttpPost>
    Public Function SyncShares(agentId As Guid) As IHttpActionResult
        Dim result = repository.SyncShares(agentId)
        Return Ok(result)
    End Function

End Class
