Imports FileMinister.Models.Sync
Imports System.Web.Http

''' <summary>
''' File Version Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/FileVersion")>
Public Class FileVersionController
    Inherits ServerApiController(Of FileVersionInfo, Guid, IFileVersionRepository)

    Public Sub New(repository As IFileVersionRepository)
        MyBase.New(repository)
    End Sub


    ''' <summary>
    ''' Get Checkout File Details
    ''' </summary>
    ''' <param name="userId">UserId</param>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("GetCheckoutFileDetails")>
    Public Function [GetCheckoutFileDetails](userId As Guid, FileShareId As Integer) As IHttpActionResult
        Dim result = repository.GetCheckoutFileDetails(userId, FileShareId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get Checkin File Details
    ''' </summary>
    ''' <param name="userId">UserId</param>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <param name="fromDate">FromDate</param>
    ''' <param name="toDate">ToDate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("GetChekinsFileDetails")>
    Public Function [GetChekinsFileDetails](userId As Guid, FileShareId As Integer, fromDate As String, toDate As String) As IHttpActionResult
        Dim result = repository.GetChekinsFileDetails(userId, FileShareId, fromDate, toDate)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Delete FileVersion
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("DeleteFileVersion")>
    <HttpPost>
    Public Function [DeleteFileVersion](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim FileVersionId As Guid = New Guid(data("FileVersionId").ToString())

        Dim result = repository.DeleteFileVersion(FileVersionId)
        Return Ok(result)

    End Function

    ''' <summary>
    ''' Get File Version History
    ''' </summary>
    ''' <param name="id">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("GetVersionHistory")>
    Public Function [GetVersionHistory](id As Guid) As IHttpActionResult
        Dim result = repository.GetVersionHistory(id)
        Return Ok(result)
    End Function

End Class
