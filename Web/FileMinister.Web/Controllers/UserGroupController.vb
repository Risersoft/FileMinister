Imports FileMinister.Models.Sync
Imports System.Web.Http

''' <summary>
''' User Group Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/UserGroup")>
Public Class UserGroupController
    Inherits ServerApiController(Of UserGroupAssignmentsInfo, Guid, IUserGroupRepository)

    Public Sub New(repository As IUserGroupRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Delete Group by GroupId
    ''' </summary>
    ''' <param name="data">Group Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteByGroup")>
    <HttpPost>
    Public Function [DeleteByGroup](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim groupId As Guid = New Guid(data("groupId").ToString)
        Dim result = repository.DeleteByGroup(groupId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Delete Group by UserId
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteByUser")>
    <HttpPost>
    Public Function [DeleteByUser](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim userId As Guid = New Guid(data("userId").ToString)
        Dim result = repository.DeleteByUser(userId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Add All Groups
    ''' </summary>
    ''' <param name="lst">Group List</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddAll")>
    <HttpPost>
    Public Function AddAll(lst As List(Of UserGroupAssignmentsInfo)) As IHttpActionResult
        Dim result = repository.AddAll(lst)
        Return Ok(result)
    End Function


End Class
