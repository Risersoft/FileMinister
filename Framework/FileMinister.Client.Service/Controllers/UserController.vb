Imports System.Web.Http
Imports risersoft.shared.portable.Model

<RoutePrefix("api/user")>
Public Class UserController
    Inherits LocalApiController(Of LocalWorkSpaceInfo, Integer, IUserRepository)

    Public Sub New(repository As IUserRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get UserAccount by window user id
    ''' </summary>
    ''' <param name="windowUserName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("verify/user")>
    <HttpGet>
    <AllowAnonymous>
    Public Function GetByWindowUserSID(windowUserName As String) As IHttpActionResult
        Dim result = repository.GetByWindowUserSID(windowUserName)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Create Local Db, Do the provisioning, add UserAccount if not exist, add UserAgent if not exist
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="user"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{id}/log")>
    <HttpPost>
    Public Function Log(id As Integer, user As LocalWorkSpaceInfo) As IHttpActionResult
        Dim result = repository.Log(user)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get UserAccount By User id and window user id
    ''' </summary>
    ''' <param name="userId"></param>
    ''' <param name="windowUserName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{userId}/configured")>
    <HttpGet>
    Public Function GetUserConfigured(userId As Guid, AccountName As String, windowUserName As String) As IHttpActionResult
        Dim result = repository.GetUserConfigured(userId, AccountName, windowUserName)
        Return Ok(result)
    End Function

    ''' <summary>
    '''  Get the effective permissions on a file for a user
    ''' </summary>
    ''' <param name="fileId"></param>
    ''' <param name="includeDeleted"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{fileId}/{includeDeleted}/EffectivePermissions")>
    <HttpGet>
    Public Function GetEffectivePermissionsOnFile(fileId As Guid, includeDeleted As Boolean) As IHttpActionResult
        Dim result = repository.GetEffectivePermissionsOnFile(fileId, includeDeleted)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Expire the access token for a user
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("LogOut")>
    <HttpPost>
    Public Function LogOut() As IHttpActionResult
        Dim result = repository.LogOut()
        Return Ok(result)
    End Function

End Class
