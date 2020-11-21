Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage.File
Imports risersoft.shared.portable.Enums
Imports FileMinister.Models.Sync
Imports System.Web.Http
Imports System.Web.Http.Results
Imports FileMinister.Models.Enums

''' <summary>
''' File Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/File")>
Public Class FileController
    Inherits ServerApiController(Of FileEntryInfo, Guid, IFileRepository)

    Public Sub New(repository As IFileRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Soft Delete a file/folder - mark IsDeleted true in delta operation of each version and remove version physically
    ''' </summary>
    ''' <param name="data">File Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("SoftDelete")>
    <HttpPost>
    Public Function [SoftDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim FileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim localFileVersionNumber As Integer = Convert.ToInt32(data("localFileVersionNumber").ToString())
        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.AccessDenied}
        res = repository.SoftDelete(FileEntryId, localFileVersionNumber)

        Return Ok(res)
    End Function


    <Route("SoftDeleteMultiple")>
    <HttpPost>
    Public Function [SoftDelete](data As List(Of Dictionary(Of String, Object))) As IHttpActionResult

        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.AccessDenied}

        For Each file In data
            Dim FileEntryId As Guid = New Guid(file("fileEntryId").ToString())
            Dim localFileVersionNumber As Integer = Convert.ToInt32(file("localFileVersionNumber").ToString())
            res = repository.SoftDelete(FileEntryId, localFileVersionNumber)
            If Not res.Data Then
                Exit For
            End If
        Next

        Return Ok(res)
    End Function

    ''' <summary>
    ''' Hard Delete a file/folder - get the hierarchy, delete/update links if exists, undo any check out, mark IsDeleted and IsPermanentlyDeleted on a file
    ''' Remove delta operation against each version of each file
    ''' For each version against each file mark IsDeleted and remove version physically
    ''' </summary>
    ''' <param name="data">File Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("HardDelete")>
    <HttpPost>
    Public Function [HardDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId As Guid = New Guid(data("fileEntryId").ToString())

        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.AccessDenied}

        'Dim actionResult = CanSoftDelete(FileEntryId:=fileEntryId)
        'Dim contentResult = DirectCast(actionResult, OkNegotiatedContentResult(Of ResultInfo(Of Boolean, Status)))
        'If (contentResult.Content.Data = True) Then

        ''Delete first from azure file, if done successfully, in that case do the soft delete from db and then continue
        'res = Util.Helper.DeleteFile(FileEntryId:=fileEntryId, ShareName:=Guid.NewGuid, user:=Me.repository.User)
        'If (res.Status = Status.Error) Then Return Ok(res)

        res = repository.HardDelete(fileEntryId)

        'End If
        Return Ok(res)

    End Function

    ''' <summary>
    ''' Hard Delete a file/folder - get the hierarchy, delete/update links if exists, undo any check out, mark IsDeleted and IsPermanentlyDeleted on a file
    ''' Remove delta operation against each version of each file
    ''' For each version against each file mark IsDeleted and remove version physically
    ''' </summary>
    ''' <param name="data">File Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("HardDeleteMultiple")>
    <HttpPost>
    Public Function [HardDelete](data As List(Of Dictionary(Of String, Object))) As IHttpActionResult

        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.AccessDenied}

        For Each file In data
            Dim FileEntryId As Guid = New Guid(file("fileEntryId").ToString())
            'Dim localFileVersionNumber As Integer = Convert.ToInt32(file("localFileVersionNumber").ToString())
            res = repository.HardDelete(FileEntryId)
            If Not res.Data Then
                Exit For
            End If
        Next

        Return Ok(res)

    End Function

    ''' <summary>
    ''' For each version of a file(deleted), insert either new delta operation (if not exist) or update existing one with isDeletd false 
    ''' and update FileEntryStatusId according to max version number
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UndoDelete")>
    <HttpPost>
    Public Function [UndoDelete](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim res = repository.UndoDelete(fileEntryId)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' Add Version Without Upload
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddVersionWithoutUpload")>
    <HttpPost>
    Public Function [AddVersionWithoutUpload](data As Tuple(Of FileEntryInfo, FileVersionInfo, List(Of FileEntryLinkInfo))) As IHttpActionResult
        Dim res = repository.AddVersionWithoutUpload(data.Item1, data.Item2, data.Item3)
        Return Ok(res)
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
        Dim FileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim localFileVersionNumber As Integer = Convert.ToInt32(data("localFileVersionNumber").ToString())
        Dim workSpaceId As Guid = New Guid(data("workSpaceId").ToString())
        Dim res = repository.CheckOut(FileEntryId, localFileVersionNumber, workSpaceId)
        Return Ok(res)
    End Function

    ''' <summary>
    '''  set checked out flag false and set checkedout user null, workspace null, version against a file version and then update delta operation of that version
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("CheckIn")>
    <HttpPost>
    Public Function [CheckIn](data As Tuple(Of FileVersionInfo, Integer)) As IHttpActionResult
        Dim res = repository.CheckIn(data.Item1, data.Item2)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' Add File and Checkout
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddFileAndcheckout")>
    <HttpPost>
    Public Function [AddFileAndcheckout](data As Tuple(Of FileEntryInfo, FileVersionInfo, Guid)) As IHttpActionResult
        Dim res = repository.AddFileAndCheckout(data.Item1, data.Item2, data.Item3)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' Add New Folder
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddFolder")>
    <HttpPost>
    Public Function [AddFolder](data As Tuple(Of FileEntryInfo, FileVersionInfo)) As IHttpActionResult
        Dim res = repository.AddFolder(data.Item1, data.Item2)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddAndDelete")>
    <HttpPost>
    Public Function [AddAndDelete](data As Tuple(Of FileEntryInfo, FileVersionInfo)) As IHttpActionResult
        Dim res = repository.AddAndDelete(data.Item1, data.Item2)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' Get All files
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AllFilesForLinking")>
    <HttpPost>
    Public Function [GetAllFilesForLinking](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim fileShareId As Integer = Convert.ToInt32(data("fileShareId").ToString())
        Dim result = repository.AllFilesForLinking(fileEntryId, fileShareId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Add File Link
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("AddFileLink")>
    <HttpPost>
    Public Function [AddFileLink](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim linkedFileEntryId As Guid = New Guid(data("linkedFileEntryId").ToString())
        Dim result = repository.AddFileLink(fileEntryId, linkedFileEntryId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Remove File Link
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("RemoveFileLink")>
    <HttpPost>
    Public Function RemoveFileLink(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim result = repository.RemoveFileLink(fileEntryId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Set checked out flag false and set checkedout user and workspace null
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UndoCheckOut")>
    <HttpPost>
    Public Function [UndoCheckOut](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim FileEntryId As Guid = New Guid(data("fileEntryId").ToString())
        Dim res = repository.UndoCheckOut(FileEntryId)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' Get All User FileShares
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("GetAllUserShares")>
    Public Function [GetAllUserShares]() As IHttpActionResult
        ' Dim FileEntryId As Guid = New Guid(data("FileEntryId").ToString())
        Dim result = repository.GetAllUserShares()
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get all children of node such as FileShare or folder
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost>
    <Route("GetAllChildren")>
    Public Function [GetAllChildren](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim id As Guid = New Guid(data("id").ToString())

        Dim result
        If (Not data("filter") Is Nothing) Then
            'Dim filter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(data("filter").ToString())
            Dim filter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of FileSearch)(data("filter").ToString())

            Dim fileSearch As New FileSearch()
            'Dim tags As New Dictionary(Of String, String)
            If Not filter.Tags Is Nothing Then
                'tags = DirectCast(filter.Tags, Dictionary(Of String, String))
                fileSearch.Tags = filter.Tags
            End If
            fileSearch.StartFileId = filter.StartFileId
            fileSearch.SearchText = filter.SearchText
            fileSearch.IsAdvancedSearch = filter.IsAdvancedSearch
            result = repository.FileSearch(fileSearch)
        Else
            result = repository.GetAllChildren(id, 0)
        End If


        Return Ok(result)
    End Function

    ''' <summary>
    ''' Check For File Existance
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("CheckFileExists")>
    Public Function [CheckFileExists](id As Guid, fileName As String) As IHttpActionResult
        Dim result = repository.GetLatestFileEntryVersionByPath(id, fileName)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost>
    <Route("AddFileAndCheckoutWeb")>
    Public Function [AddFileAndCheckoutWeb](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim id As Guid = New Guid(data("id").ToString())
        Dim blobName As Guid = New Guid(data("blobName").ToString())
        Dim fileName As String = data("fileName").ToString()
        Dim fileSize As Long = Long.Parse(data("fileSize").ToString())
        Dim res = repository.AddAndCheckoutWeb(id, fileName, blobName, fileSize)

        Return Ok(res)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost>
    <Route("CheckInWeb")>
    Public Function [CheckInWeb](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim id As Guid = New Guid(data("id").ToString())
        Dim localFileVersionNumber As Integer = Int32.Parse(data("version").ToString())
        Dim fileSize As Long = Int64.Parse(data("fileSize").ToString())
        Dim blobName As Guid = New Guid(data("blobName").ToString())
        Dim fileHash As String = data("fileHash")

        Dim res = repository.CheckInWeb(id, localFileVersionNumber, fileSize, fileHash, blobName)
        Return Ok(res)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost>
    <Route("AddFolderWeb")>
    Public Function [AddFolderWeb](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim parentFolderId As Guid = New Guid(data("id").ToString())
        Dim folderName As String = data("name").ToString()
        Dim res = repository.AddFolderWeb(parentFolderId, folderName)
        Return Ok(res)
    End Function

    '<HttpPost>
    '<Route("search")>
    'Public Function [Search](data As Dictionary(Of String, Object)) As IHttpActionResult
    '    Dim tags As New Dictionary(Of String, String)
    '    If Not data("tags") Is Nothing Then
    '        tags = DirectCast(data("tags"), Dictionary(Of String, String))
    '    End If
    '    Dim fileSearch As New FileSearch()
    '    fileSearch.Tags = tags
    '    fileSearch.StartFileId = New Guid(data("StartFileId").ToString())
    '    fileSearch.SearchText = data("SearchText").ToString()
    '    fileSearch.IsAdvancedSearch = Convert.ToBoolean(data("IsAdvancedSearch").ToString())

    '    Dim res = repository.FileSearch(fileSearch)
    '    Return Ok(res)
    'End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("RequestConflictFileUpload")>
    <HttpPost>
    Public Function RequestConflictFileUpload(data As Tuple(Of Guid, Guid, Guid)) As IHttpActionResult
        'Dim FileEntryId As Guid = New Guid(data("FileEntryId").ToString())
        'Dim FileVersionConflictId As Guid = New Guid(data("FileVersionConflictId").ToString())
        Dim result = repository.RequestConflictFileUpload(data.Item1, data.Item2, data.Item3)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GetConflictFilePendingUpload")>
    <HttpGet>
    Public Function GetConflictFilePendingUpload(workSpaceId As Guid) As IHttpActionResult
        Dim result = repository.GetConflictFilePendingUpload(workSpaceId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FileEntryId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("{FileEntryId}/GetOtherUsersUnresolvedConflicts")>
    <HttpGet>
    Public Function GetOtherUsersUnresolvedConflicts(FileEntryId As Guid) As IHttpActionResult
        Dim result = repository.GetOtherUsersUnresolvedConflicts(FileEntryId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("UpdateConflictUploadStatus")>
    <HttpPost>
    Public Function UpdateConflictUploadStatus(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileSize As Integer = data("fileSize")
        Dim FileVersionConflictId As Guid = New Guid(data("FileVersionConflictId").ToString())
        Dim fileHash As String = data("fileHash")
        Dim result = repository.UpdateConflictUploadStatus(FileVersionConflictId, fileSize, fileHash)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("ResolveOthersConflictUsingOthers")>
    <HttpPost>
    Public Function ResolveOthersConflictUsingOthers(data As Tuple(Of Integer, Guid, Guid)) As IHttpActionResult
        'Dim FileShareId As Integer = data("FileShareId")
        'Dim FileEntryId As Guid = New Guid(data("FileEntryId").ToString())
        'Dim FileVersionConflictId As Guid = New Guid(data("FileVersionConflictId").ToString())
        Dim result = repository.ResolveOthersConflictUsingOthers(data.Item1, data.Item2, data.Item3)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("ResolveOthersConflictUsingServer")>
    <HttpPost>
    Public Function ResolveOthersConflictUsingServer(data As Tuple(Of Integer, Guid, Guid)) As IHttpActionResult
        'Dim FileShareId As Integer = data("FileShareId")
        'Dim FileEntryId As Guid = New Guid(data("FileEntryId").ToString())
        'Dim FileVersionConflictId As Guid = New Guid(data("FileVersionConflictId").ToString())
        Dim result = repository.ResolveOthersConflictUsingServer(data.Item1, data.Item2, data.Item3)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("RenameFile")>
    <HttpPost>
    Public Function RenameFile(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId = New Guid(data("fileEntryId").ToString())
        Dim versionNumber = CType(data("versionNumber"), Integer)
        Dim newName = data("newName").ToString()

        Dim result = repository.RenameFile(fileEntryId, versionNumber, newName)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("MoveFile")>
    <HttpPost>
    Public Function MoveFile(data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim fileEntryId = New Guid(data("fileEntryId").ToString())
        Dim destinationFileEntryId = New Guid(data("destinationFileEntryId").ToString())
        Dim isReplaceExistingFile = CType(data("isReplaceExistingFile"), Boolean)
        Dim newFileName = data("newFileName").ToString()

        Dim result = repository.MoveFile(fileEntryId, destinationFileEntryId, isReplaceExistingFile, newFileName)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get all children of node such as FileShare or folder
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost>
    <Route("GetAllDeletedChildren")>
    Public Function [GetAllDeletedChildren](data As Dictionary(Of String, Object)) As IHttpActionResult
        Dim id As Guid = New Guid(data("id").ToString())
        Dim result As ResultInfo(Of List(Of FileEntryInfo), Status)
        result = repository.GetAllDeletedChildren(id)
        Return Ok(result)
    End Function
    <Route("CheckViewDeletedFiles")>
    <HttpGet>
    Public Function CanViewDeletedFiles() As IHttpActionResult
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim allowed As Boolean = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        result.Data = allowed
        result.Status = IIf(result.Data = False, Status.AccessDenied, Status.Success)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' This method is being used to check share admin permission
    ''' </summary>
    ''' <returns></returns>
    <Route("ChecksShareAdminPermission")>
    <HttpGet>
    Public Function CanChangeShareAdminPermission() As IHttpActionResult
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)() With {.Data = (user.UserAccount.UserTypeId = Role.AccountAdmin), .Status = Status.Success}
        Return Ok(result)
    End Function

    ''' <summary>
    ''' This method is being used to check Tag Permission
    ''' </summary>
    ''' <param name="FileEntryId">File Entry Id</param>
    ''' <returns>Return a HttpResponseMessage</returns>
    <Route("{FileEntryId}/CheckChangeTagPermission")>
    <HttpGet>
    Public Function CanChangeTag(FileEntryId As Guid) As IHttpActionResult
        ' admin or change tag permission
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim allowed = False
        allowed = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        If (Not allowed) Then
            Dim permissions = repository.GetEffectivePermission(user, FileEntryId)
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                      ((permissions And PermissionType.Tag) = PermissionType.Tag)
        End If

        Dim file = repository.Get(FileEntryId)

        If (allowed) AndAlso file IsNot Nothing AndAlso file.Data.FileVersion IsNot Nothing Then
            If (file.Data.IsPermanentlyDeleted OrElse file.Data.IsDeleted OrElse ((file.Data.FileVersion.PreviousFileVersionId Is Nothing) AndAlso ((file.Data.FileVersion.VersionNumber Is Nothing OrElse file.Data.FileVersion.VersionNumber = 0)))) Then
                allowed = False
            End If
        End If

        result.Data = allowed
        result.Status = IIf(allowed = False, Status.AccessDenied, Status.Success)
        result.Message = IIf(allowed = False, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION, "")

        Return Ok(result)
    End Function

    ''' <summary>
    ''' This method being used to check View permission
    ''' </summary>
    ''' <param name="FileEntryId">File Entry Id</param>
    ''' <returns>Return a HttpResponseMessage</returns>
    <Route("{FileEntryId}/CheckViewPermission")>
    <HttpGet>
    Public Function CanViewPermission(FileEntryId As Guid) As IHttpActionResult
        ' Only account admin and share admin can view permission
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim allowed = False
        allowed = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = repository.GetEffectivePermission(user, FileEntryId)
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If

        result.Data = allowed
        result.Status = IIf(allowed = False, Status.AccessDenied, Status.Success)
        result.Message = IIf(allowed = False, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION, "")

        Return Ok(result)

    End Function

    ''' <summary>
    ''' This method check can user delete file version or not
    ''' </summary>
    ''' <param name="FileEntryId">File Entry Id</param>
    ''' <returns>Return a HttpResponseMessage</returns>
    <Route("{FileEntryId}/CheckDeleteFileVersionPermission")>
    <HttpGet>
    Public Function CanDeleteFileVersion(FileEntryId As Guid) As IHttpActionResult
        ' Only account admin and share admin can delete version
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim allowed = False
        allowed = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = repository.GetEffectivePermission(user, FileEntryId)
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If

        result.Data = allowed
        result.Status = IIf(allowed = False, Status.AccessDenied, Status.Success)
        result.Message = IIf(allowed = False, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION, "")

        Return Ok(result)
    End Function

    ''' <summary>
    ''' This method will return permissions such as edit and Share
    ''' </summary>
    ''' <param name="FileEntryId">Entry Id of the File</param>
    ''' <returns>Return a HttpResponseMessage</returns>
    <Route("{FileEntryId}/CheckEditAndChangeShareAdminPermission")>
    <HttpGet>
    Public Function CanEditAndChangeShareAdminPermission(FileEntryId As Guid) As IHttpActionResult
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Dictionary(Of String, Boolean), Status) = New ResultInfo(Of Dictionary(Of String, Boolean), Status)()
        Dim list As Dictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)()

        Dim actionResult = Me.CanViewPermission(FileEntryId)
        Dim contentResult = DirectCast(actionResult, OkNegotiatedContentResult(Of ResultInfo(Of Boolean, Status)))
        list.Add("CanEdit", contentResult.Content.Data)
        list.Add("CanShare", (user.UserAccount.UserTypeId = Role.AccountAdmin))
        result.Data = list
        result.Status = Status.Success

        Return Ok(result)
    End Function

    Private Function CanHardDelete(FileEntryId As Guid) As IHttpActionResult
        ' Only account admin and share admin can permanent delete
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim allowed = False
        allowed = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        If (allowed = False) Then
            Dim permissions = repository.GetEffectivePermission(user, FileEntryId)
            allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)
        End If

        result.Data = allowed
        result.Status = IIf(allowed = False, Status.AccessDenied, Status.Success)
        result.Message = IIf(allowed = False, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION, "")

        Return Ok(result)
    End Function

    Private Function CanSoftDelete(FileEntryId As Guid) As IHttpActionResult
        ' File not already deleted and (admin or write permission)
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()

        Dim file = repository.Get(FileEntryId)
        If file Is Nothing Then
            result.Data = False
            result.Status = Status.AccessDenied
            Return Ok(result)
        End If

        Dim allowed = Not (file.Data.IsDeleted Or file.Data.IsPermanentlyDeleted)
        If (allowed) Then
            allowed = (user.UserAccount.UserTypeId = Role.AccountAdmin)
            If (allowed = False) Then
                Dim permissions = repository.GetEffectivePermission(user, FileEntryId:=FileEntryId)
                allowed = ((permissions And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) OrElse
                          ((permissions And PermissionType.Write) = PermissionType.Write)
            End If
        End If

        result.Data = allowed
        result.Status = IIf(allowed = False, Status.AccessDenied, Status.Success)
        result.Message = IIf(allowed = False, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION, "")

        Return Ok(result)

    End Function

    ''' <summary>
    ''' This function will check whether the login user is Account admin or not
    ''' </summary>
    ''' <returns>Return a HttpResponseMessage</returns>
    <Route("AccountAdmin")>
    <HttpGet>
    Public Function IsAccountAdmin() As IHttpActionResult
        Dim user = Me.repository.User
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)()
        Dim isAdmin = (user.UserAccount.UserTypeId = Role.AccountAdmin)
        result.Data = isAdmin
        result.Status = Status.Success
        Return Ok(result)
    End Function


End Class
