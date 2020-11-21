Imports System.Web.Http
Imports FileMinister.Client.Service.IRepository
Imports risersoft.shared.portable.Model

Namespace Controllers
    <RoutePrefix("api/File")>
    Public Class FileController
        Inherits LocalApiController(Of FileEntryInfo, Guid, IFileRepository)


        Public Sub New(repository As IFileRepository)
            MyBase.New(repository)
        End Sub

        ''' <summary>
        ''' Get all folders records against a share
        ''' </summary>
        ''' <param name="shareId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("Share/{shareId:int}")>
        <HttpGet>
        Public Function GetByShareId(shareId As Integer) As IHttpActionResult
            Dim result = repository.GetByShareId(shareId)
            Return Ok(result)
        End Function

        ''' <summary>
        '''  Get an unresolved Conflict on a file by a user and workspace
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("MyConflict/{fileSystemEntryId}")>
        <HttpGet>
        Public Function GetMyConflict(fileSystemEntryId As Guid) As IHttpActionResult
            Dim result = repository.GetMyConflict(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Resolve a conflict on a file using server 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("ResolveConflictUsingTheirs")>
        <HttpPost>
        Public Function [ResolveConflictUsingTheirs](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim shareId As Integer = CType(data("shareId"), Integer)
            Dim result = repository.ResolveConflictUsingTheirs(fileSystemEntryId, shareId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Resolve a conflict on a file using client 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("ResolveConflictUsingMine")>
        <HttpPost>
        Public Function [ResolveConflictUsingMine](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.ResolveConflictUsingMine(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Soft Delete a file/folder - mark IsDeleted true in delta operation of each version and remove version physically
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("SoftDelete")>
        <HttpPost>
        Public Function [SoftDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim isForce As Boolean = CType(data("isForce"), Boolean)
            Dim result = repository.SoftDelete(fileSystemEntryId, isForce)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Hard Delete a file/folder - get the hierarchy, delete/update links if exists, undo any check out, mark IsDeleted and IsPermanentlyDeleted on a file
        ''' Remove delta operation against each version of each file
        ''' For each version against each file mark IsDeleted and remove version physically
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("HardDelete")>
        <HttpPost>
        Public Function [HardDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.HardDelete(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Remove records from DeltaOperations, FileSystemEntryVersionConflicts, FileSystemEntryVersions, Tags, GroupFileSystemEntryPermissionAssignments,
        ''' UserFileSystemEntryPermissionAssignments, FileSystemEntryLinks, FileSystemEntries against a file
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("DeleteFile")>
        <HttpPost>
        Public Function [DeleteFile](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.DeleteFile(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Get the file against a path and then get the hierarchy of that file and remove all related records using DeleteFile() function
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("DeleteFiles")>
        <HttpPost>
        Public Function [DeleteFiles](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim baseRelativePath As String = data("baseRelativePath").ToString()
            Dim shareId As Integer = CInt(data("shareId"))
            Dim result = repository.DeleteFiles(baseRelativePath, shareId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' For each version of a file(deleted), insert either new delta operation (if not exist) or update existing one with isDeletd false 
        ''' and update FileSystemEntryStatusId according to max version number
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("UndoDelete")>
        <HttpPost>
        Public Function [UndoDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.UndoDelete(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Get all child records against a file id
        ''' </summary>
        ''' <param name="parentId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("{parentId}/children")>
        <HttpGet>
        Function GetByParentId(parentId As Guid) As IHttpActionResult
            Dim result = repository.GetByParentId(parentId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Get all the files based on search text and tag value
        ''' </summary>
        ''' <param name="search"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("FileSearch")>
        <HttpPost>
        Public Function [FileSearch](search As FileSearch) As IHttpActionResult
            Dim result = repository.FileSearch(search)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' set checked out flag and checked out user on a file
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("CheckOut")>
        <HttpPost>
        Public Function [CheckOut](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim workSpaceId As Guid = New Guid(data("workSpaceId").ToString())
            Dim result = repository.CheckOut(fileSystemEntryId, workSpaceId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' set checked out flag false and set checkedout user and workspace null
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("UndoCheckOut")>
        <HttpPost>
        Public Function UndoCheckOut(data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.UndoCheckOut(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' set checked out flag false and set checkedout user null, workspace null, version against a file version and then update delta operation of that version
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("CheckIn")>
        <HttpPost>
        Public Function [CheckIn](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryVersionId").ToString())
            Dim versionNumber As Int32 = Convert.ToInt32(data("versionNumber").ToString())
            Dim result = repository.CheckIn(fileSystemEntryId, versionNumber)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Get all links against a file 
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("{fileSystemEntryId}/Links")>
        <HttpGet>
        Public Function GetFileLinks(fileSystemEntryId As Guid) As IHttpActionResult
            Dim result = repository.GetFileLinks(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Remove all records from delta operation against a file
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("DeleteDeltaOperationsForFile")>
        <HttpPost>
        Public Function DeleteDeltaOperationsForFile(data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim result = repository.DeleteDeltaOperationsForFile(fileSystemEntryId)
            Return Ok(result)
        End Function

        ''' <summary>
        ''' Check whether file is previously linked with any other file
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Route("IsFilePreviouslyLinked")>
        <HttpPost>
        Public Function [IsFilePreviouslyLinked](data As Dictionary(Of String, Object)) As IHttpActionResult
            Dim fileSystemEntryId As Guid = New Guid(data("fileSystemEntryId").ToString())
            Dim res = repository.IsFilePreviouslyLinked(fileSystemEntryId)
            Return Ok(res)
        End Function

    End Class
End Namespace

