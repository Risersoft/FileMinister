
Imports System.Web.Http
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable

Namespace Controllers
    <RoutePrefix("api/FileVersion")>
    Public Class FileVersionController
        Inherits LocalApiController(Of FileVersionInfo, Guid, IFileVersionRepository)


        Public Sub New(repository As IFileVersionRepository)
            MyBase.New(repository)
        End Sub

        ''' <summary>
        ''' Get all file version against a file
        ''' </summary>
        ''' <param name="fileId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <HttpGet>
        <Route("{fileId}/FileVersions")>
        Public Function [GetAllFileVersions](fileId As Guid) As IHttpActionResult
            Dim result = repository.GetAllFileVersions(fileId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Set IsDeleted true against a file version
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("DeleteFileVersion")>
        <HttpPost>
        Public Function [DeleteFileVersion](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim Id As Guid = New Guid(data("Id").ToString())
            Dim result = repository.DeleteFileVersion(Id)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Delete File by file path
        ''' </summary>
        ''' <param name="shareId"></param>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("{shareId}/deleteFile")>
        <HttpDelete>
        Public Function DeleteFileByName(shareId As Short, filename As String) As IHttpActionResult
            Dim result = repository.DeleteFileByFilename(shareId, filename)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Update File Status in DelatOperation
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("UpdateFileVersionStatus")>
        <HttpPost>
        Public Function [UpdateFileVersionStatus](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryVersionId As Guid = New Guid(data("fileSystemEntryVersionId").ToString())
            Dim fileSystemEntryStatus As Enums.FileEntryStatus = Convert.ToByte(data("fileSystemEntryStatus").ToString())
            Dim result = repository.UpdateFileVersionStatus(fileSystemEntryVersionId, fileSystemEntryStatus)
            Return Ok(result)
        End Function

    End Class
End Namespace
