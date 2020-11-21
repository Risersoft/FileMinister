Imports FileMinister.Models.Sync
Imports System.Web.Http

''' <summary>
''' User File Permission Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/UserFilePermission")>
Public Class UserFilePermissionController
    Inherits ServerApiController(Of UserFilePermissionInfo, Guid, IUserFilePermissionRepository)

    Public Sub New(repository As IUserFilePermissionRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get Users And Groups Having Exclusive Permissions On Latest FileVersions
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("{fileId}/UsersGroups")>
    Public Function [GetAllUsersAndGroups](fileId As Guid) As IHttpActionResult
        Dim result = repository.GetAllUsersAndGroups(fileId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get User Permission Matrix On Latest FileVersions
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <param name="userId">UserId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UPermission")>
    <HttpGet>
    Public Function [GetPermissionByFileAndUser](fileId As Guid, userId As Guid) As IHttpActionResult
        Dim result = repository.GetPermissionByFileAndUser(fileId, userId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Update Permissions based on file and user
    ''' </summary>
    ''' <param name="data">Permission Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UpdateUserFilePermission")>
    <HttpPost>
    Public Function [UpdateUserFilePermission](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileId As Guid = New Guid(data("fileId").ToString())
        Dim userId As Guid = New Guid(data("userId").ToString)
        Dim permissionAllowed As Integer = data("permissionAllowed")
        Dim permissionDenied As Integer = data("permissionDenied")
        Dim result = repository.UpdatePermissionByFileAndUser(fileId, userId, permissionAllowed, permissionDenied)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Delete Permission based on file and user
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteUserFilePermission")>
    <HttpPost>
    Public Function [DeleteUserFilePermission](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileId As Guid = New Guid(data("fileId").ToString())
        Dim userId As Guid = New Guid(data("userId").ToString)
        Dim result = repository.DeletePermissionByFileAndUser(fileId, userId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Update Permission
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UpdateFilePermission")>
    <HttpPost>
    Public Function UpdateFilePermission(data As Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo))) As IHttpActionResult
        'Dim fileId As Guid = New Guid(data("fileId").ToString())
        'Dim removedUserAndGroupList As List(Of UserGroupInfo) = data("removedUserAndGroupList")
        'Dim updatedUserAndGroupPermissions As List(Of UserFilePermissionInfo) = data("updatedUserAndGroupPermissions")
        Dim result = repository.UpdateFilePermission(data)

        Return Ok(result)
    End Function
    
    ''' <summary>
    ''' Update Permission for Users/Groups
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UpdateFilePermissionForUsersGroups")>
    <HttpPost>
    Public Function UpdateFilePermission(data As Dictionary(Of String, Object)) As IHttpActionResult

        Dim tupleData As Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo))
        Dim fileId As Guid = New Guid(data("item1").ToString())
        Dim addFilePermissionInfo As List(Of UserFilePermissionInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of UserFilePermissionInfo))(data("item2").ToString())
        Dim updateFilePermissionInfo As List(Of UserFilePermissionInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of UserFilePermissionInfo))(data("item3").ToString())
        Dim removeFilePermissionInfo As List(Of UserGroupInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of UserGroupInfo))(data("item4").ToString())

        addFilePermissionInfo.AddRange(updateFilePermissionInfo)

        tupleData = Tuple.Create(fileId, removeFilePermissionInfo, addFilePermissionInfo)

        Dim result = repository.UpdateFilePermission(tupleData)

        Return Ok(result)
    End Function

    <Route("GetUserFilePermissionsForShare/{shareId}")>
    <HttpGet>
    Public Function GetUserFilePermissionsForShare(shareId As Short) As IHttpActionResult
        Dim result = repository.GetUserFilePermissionsForShare(shareId)
        Return Ok(result)
    End Function

End Class
