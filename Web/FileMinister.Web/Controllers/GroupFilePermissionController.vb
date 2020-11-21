Imports FileMinister.Models.Sync
Imports System.Web.Http

''' <summary>
''' Group File Permission Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/GroupFilePermission")>
Public Class GroupFilePermissionController
    Inherits ServerApiController(Of UserFilePermissionInfo, Guid, IGroupFilePermissionRepository)

    Public Sub New(repository As IGroupFilePermissionRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get Permission based on File and Group
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <param name="groupId">GroupId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GPermission")>
    <HttpGet>
    Public Function [GetPermissionByFileAndGroup](fileId As Guid, groupId As Guid) As IHttpActionResult
        Dim result = repository.GetPermissionByFileAndGroup(fileId, groupId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UpdateGroupFilePermission")>
    <HttpPost>
    Public Function [UpdateGroupFilePermission](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileId As Guid = New Guid(data("fileId").ToString())
        Dim groupId As Guid = New Guid(data("groupId").ToString)
        Dim permissionAllowed As Integer = data("permissionAllowed")
        Dim permissionDenied As Integer = data("permissionDenied")
        Dim result = repository.UpdatePermissionByFileAndGroup(fileId, groupId, permissionAllowed, permissionDenied)
        Return Ok(result)
    End Function

    <Route("DeleteGroupFilePermission")>
    <HttpPost>
    Public Function [DeleteGroupFilePermission](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileId As Guid = New Guid(data("fileId").ToString())
        Dim groupId As Guid = New Guid(data("groupId").ToString)
        Dim result = repository.DeletePermissionByFileAndGroup(fileId, groupId)
        Return Ok(result)
    End Function

    '<Route("GetGroupFilePermissionsForShare/{shareId}")>
    '<HttpGet>
    'Public Function GetGroupFilePermissionsForShare(shareId As Short) As IHttpActionResult
    '    Dim result = repository.GetGroupFilePermissionsForShare(shareId)
    '    Return Ok(result)
    'End Function

End Class
