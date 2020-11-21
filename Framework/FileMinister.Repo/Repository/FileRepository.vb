Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable
Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage.File

Imports risersoft.shared.portable.Enums
Imports FileMinister.Repo.Util
Imports Model = FileMinister.Models.Sync
Imports F_Enums = FileMinister.Models.Enums
Imports System.Configuration

''' <summary>
''' File Repository
''' </summary>
''' <remarks></remarks>
Partial Public Class FileRepository
    Inherits ServerRepositoryBase(Of FileEntryInfo, Guid)
    Implements IFileRepository

    ''' <summary>
    ''' Soft Delete File
    ''' </summary>
    ''' <param name="FileEntryId">FileId</param>
    ''' <param name="localFileVersionNumber">Local File Version</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SoftDelete(FileEntryId As Guid, localFileVersionNumber As Integer) As ResultInfo(Of Boolean, Status) Implements IFileRepository.SoftDelete
        'todo: conflicted file also be deleted and later restored?
        Try
            Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Dim userId As Guid = Me.User.UserId
            Using service = GetServerEntity()

                If Not IsFilePathValid(service, FileEntryId) Then
                    result.Data = False
                    result.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    result.Status = Status.PathNotValid
                    Return result
                End If

                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)
                Dim shareName As String = String.Empty
                If (file IsNot Nothing) Then
                    If (file.IsDeleted = False) Then

                        If (file.FileEntryTypeId = FileType.Folder) Then

                            If (HasWritePermission(service, Me.User, FileEntryId)) Then

                                'Delete from File Storage (Cloud)
                                result = DeleteFolderFileFromAzureFileStorage(service, file)
                                If (Not result.Data) Then
                                    Return result
                                End If

                                Dim undelChildren = service.GetLatestFileVersionChildrenHierarchy(file.FileEntryId) 'GetLatestFileVersionChildrens(file.FileEntryId)

                                'Collect Folder's childern and store in staging table dbo.FileEntryDeleteGroupHierarchy
                                result = AddFileEntryDeleteGroupHierarchy(service, file, undelChildren.ToList())

                                'Mark all childern of the selected folder to Deleted and then folder itself.
                                DeleteFileVersionEntry(service, undelChildren, file.FileEntryId)

                                'Mark Folder itself delete
                                file.IsCheckedOut = False
                                file.CheckedOutByUserId = Nothing
                                file.CheckedOutOnUTC = Nothing
                                file.CheckedOutWorkSpaceId = Nothing

                                file.IsDeleted = True
                                file.DeletedByUserId = userId
                                file.DeletedOnUTC = DateTime.UtcNow()

                                Dim fileVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
                                If (fileVersion IsNot Nothing) Then
                                    fileVersion.IsDeleted = True
                                    fileVersion.DeletedByUserId = userId
                                    fileVersion.DeletedOnUTC = DateTime.UtcNow()
                                End If

                                service.SaveChanges()

                                result.Data = True
                                result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                result.Status = Status.Success

                            Else
                                result.Data = False
                                result.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                result.Status = Status.AccessDenied

                            End If

                        Else

                            If (file.FileEntryTypeId = FileType.File) Then
                                If (file.IsCheckedOut = False OrElse (file.IsCheckedOut = True AndAlso file.CheckedOutByUserId = userId)) Then
                                    If (localFileVersionNumber = file.CurrentVersionNumber) Then
                                        If (HasWritePermission(service, Me.User, FileEntryId)) Then

                                            'Delete first from azure folder/file, if done successfully, in that case do the soft delete from db and then continue
                                            If (file.FileShare IsNot Nothing) Then shareName = file.FileShare.ShareContainerName
                                            'Delete first from azure folder/file, if done successfully, in that case do the soft delete from db and then continue
                                            result = Util.Helper.DeleteFile(FileEntryId:=FileEntryId, ShareName:=shareName, FileShareId:=file.FileShareId, user:=Me.User)
                                            If (result.Status = Status.Error) Then
                                                Return result
                                            End If

                                            'Collect Folder's childern and store in staging table dbo.FileEntryDeleteGroupHierarchy
                                            result = AddFileEntryDeleteGroupHierarchy(service, file, Nothing)

                                            Dim fileVersion = service.FileVersions.Where(Function(p) p.FileEntryId = file.FileEntryId And p.VersionNumber = file.CurrentVersionNumber).FirstOrDefault()

                                            If (fileVersion IsNot Nothing) Then
                                                fileVersion.IsDeleted = True
                                                fileVersion.DeletedByUserId = userId
                                                fileVersion.DeletedOnUTC = DateTime.UtcNow()
                                            End If

                                            file.IsCheckedOut = False
                                            file.CheckedOutByUserId = Nothing
                                            file.CheckedOutOnUTC = Nothing
                                            file.CheckedOutWorkSpaceId = Nothing

                                            file.IsDeleted = True
                                            file.DeletedByUserId = userId
                                            file.DeletedOnUTC = DateTime.UtcNow()

                                            service.SaveChanges()

                                            result.Data = True
                                            result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                            result.Status = Status.Success
                                        Else
                                            result.Data = False
                                            result.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                            result.Status = Status.AccessDenied
                                        End If
                                    Else
                                        result.Data = False
                                        result.Message = ServiceConstants.FILE_VERSION_MISMATCHED
                                        result.Status = Status.VersionNotLatest
                                    End If
                                Else
                                    Dim userName As String = String.Empty
                                    If file.CheckedOutByUserId.HasValue Then
                                        Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                        If userResult.Status = Status.Success Then
                                            userName = userResult.Data.UserName
                                        End If
                                    End If

                                    result.Data = False
                                    result.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                                    result.Status = Status.FileCheckedOutByDifferentUser
                                End If
                            Else
                                result.Data = False
                                result.Message = ServiceConstants.ATLEAST_FILEFOLDER_NOT_DELETED
                                result.Status = Status.ChildNotDeleted
                            End If

                        End If
                    Else
                        result.Data = True
                        result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        result.Status = Status.Success
                    End If
                Else
                    result.Data = False
                    result.Message = ServiceConstants.FILE_NOTFOUND
                    result.Status = Status.NotFound
                End If
            End Using
            Return result
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function


    Public Function SoftDeleteNoCheck(FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Try
            Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Dim userId As Guid = Me.User.UserId
            Using service = GetServerEntity()

                If Not IsFilePathValid(service, FileEntryId) Then
                    result.Data = False
                    result.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    result.Status = Status.PathNotValid
                    Return result
                End If

                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)
                Dim shareName As String = String.Empty
                If (file IsNot Nothing) Then
                    If (file.IsDeleted = False) Then

                        If (file.FileEntryTypeId = FileType.Folder) Then

                            Dim undelChildren = service.GetLatestFileVersionChildrenHierarchy(file.FileEntryId) 'GetLatestFileVersionChildrens(file.FileEntryId)

                            'Collect Folder's childern and store in staging table dbo.FileEntryDeleteGroupHierarchy
                            result = AddFileEntryDeleteGroupHierarchy(service, file, undelChildren.ToList())

                            'Mark all childern of the selected folder to Deleted and then folder itself.
                            DeleteFileVersionEntry(service, undelChildren, file.FileEntryId)

                            'Mark Folder itself delete
                            file.IsCheckedOut = False
                            file.CheckedOutByUserId = Nothing
                            file.CheckedOutOnUTC = Nothing
                            file.CheckedOutWorkSpaceId = Nothing

                            file.IsDeleted = True
                            file.DeletedByUserId = userId
                            file.DeletedOnUTC = DateTime.UtcNow()

                            Dim fileVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
                            If (fileVersion IsNot Nothing) Then
                                fileVersion.IsDeleted = True
                                fileVersion.DeletedByUserId = userId
                                fileVersion.DeletedOnUTC = DateTime.UtcNow()
                            End If

                            service.SaveChanges()

                            result.Data = True
                            result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            result.Status = Status.Success



                        Else

                            If (file.FileEntryTypeId = FileType.File) Then

                                'Collect Folder's childern and store in staging table dbo.FileEntryDeleteGroupHierarchy
                                result = AddFileEntryDeleteGroupHierarchy(service, file, Nothing)

                                Dim fileVersion = service.FileVersions.Where(Function(p) p.FileEntryId = file.FileEntryId And p.VersionNumber = file.CurrentVersionNumber).FirstOrDefault()

                                If (fileVersion IsNot Nothing) Then
                                    fileVersion.IsDeleted = True
                                    fileVersion.DeletedByUserId = userId
                                    fileVersion.DeletedOnUTC = DateTime.UtcNow()
                                End If

                                file.IsCheckedOut = False
                                file.CheckedOutByUserId = Nothing
                                file.CheckedOutOnUTC = Nothing
                                file.CheckedOutWorkSpaceId = Nothing

                                file.IsDeleted = True
                                file.DeletedByUserId = userId
                                file.DeletedOnUTC = DateTime.UtcNow()

                                service.SaveChanges()

                                result.Data = True
                                result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                result.Status = Status.Success
                            Else
                                result.Data = False
                                result.Message = ServiceConstants.ATLEAST_FILEFOLDER_NOT_DELETED
                                result.Status = Status.ChildNotDeleted
                            End If

                        End If
                    Else
                        result.Data = True
                        result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        result.Status = Status.Success
                    End If
                Else
                    result.Data = False
                    result.Message = ServiceConstants.FILE_NOTFOUND
                    result.Status = Status.NotFound
                End If
            End Using
            Return result
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function
    ''' <summary>
    ''' Hard Delete File
    ''' </summary>
    ''' <param name="FileEntryId">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HardDelete(FileEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.HardDelete
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Dim userId As Guid = Me.User.UserId

            Using service = GetServerEntity()

                'check if the user is admin or not
                If (Not IsUserAdmin(service, Me.User, FileEntryId)) Then
                    res.Data = False
                    res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                    res.Status = Status.AccessDenied
                    Return res
                End If

                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)

                Dim deletedChildern = service.GetFileEntryChildrenHierarchyInclDeleted(FileEntryId)

                'Get data from the recycle Group Hierachcies table
                Dim recycleInfo = (From fv In service.FileEntries
                                   Join a In service.FileEntryDeleteGroupHierarchies
                                          On fv.FileEntryId Equals a.FileEntryId
                                   Where a.FileEntryId = FileEntryId
                                   Select fv).FirstOrDefault()

                'Get date for all childern of Group Hierachcies table
                Dim recycleChildInfo = (From fv In service.FileEntries
                                        Join a In service.FileEntryDeleteGroupHierarchies
                                          On fv.FileEntryId Equals a.FileEntryId
                                        Join b In deletedChildern
                                            On fv.FileEntryId Equals b.FileEntryId
                                        Select fv)

                Dim permanentDeletedChildern = (From a In service.FileEntries
                                                Join b In deletedChildern
                                                    On a.FileEntryId Equals b.FileEntryId
                                                Where a.IsPermanentlyDeleted = True
                                                )

                'Do not delete from database, in case of folder and has entry in Recycle tables
                If ((recycleInfo IsNot Nothing AndAlso recycleInfo.FileEntryTypeId = FileType.Folder) AndAlso (permanentDeletedChildern Is Nothing _
                    OrElse (deletedChildern IsNot Nothing AndAlso deletedChildern.Count > 0))) Then
                    res = DeleteFromFileEntryDeleteGroupHierarchy(service, FileEntryId)
                    service.SaveChanges()
                    Return res

                ElseIf (file.FileEntryTypeId = FileType.Folder AndAlso (recycleChildInfo IsNot Nothing And recycleChildInfo.Count > 0)) Then
                    file.IsDeleted = True
                    file.DeletedByUserId = userId
                    file.DeletedOnUTC = DateTime.UtcNow()

                    For Each FEV In file.FileVersions
                        FEV.IsDeleted = True
                        FEV.DeletedByUserId = userId
                        FEV.DeletedOnUTC = DateTime.UtcNow()
                    Next

                    Dim childern = service.GetLatestFileVersionChildrenHierarchy(file.FileEntryId)
                    For Each child In childern
                        Dim childFile = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = child.FileEntryId)
                        childFile.IsDeleted = True
                        childFile.DeletedByUserId = userId
                        childFile.DeletedOnUTC = DateTime.UtcNow()

                        For Each FEV In childFile.FileVersions
                            FEV.IsDeleted = True
                            FEV.DeletedByUserId = userId
                            FEV.DeletedOnUTC = DateTime.UtcNow()
                        Next

                    Next

                    res = DeleteFromFileEntryDeleteGroupHierarchy(service, FileEntryId)
                    service.SaveChanges()

                    res.Data = True
                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                    res.Status = Status.Success

                    Return res

                End If


                Dim filehierarchy = service.GetFileEntryChildrenHierarchyInclDeleted(FileEntryId)

                If filehierarchy IsNot Nothing AndAlso filehierarchy.Count > 0 Then
                    Dim lstServerFileName As List(Of Guid) = New List(Of Guid)

                    Dim files = (From FE As FileEntry In service.FileEntries
                                 Join a In filehierarchy
                                 On a.FileEntryId Equals FE.FileEntryId
                                 Join FV As FileVersion In service.FileVersions
                                 On FV.FileEntryId Equals FE.FileEntryId
                                 Where (FE.IsPermanentlyDeleted = 0 And FV.VersionNumber = FE.CurrentVersionNumber)
                                 Select FE, FV.FileEntryRelativePath
                                ).Distinct()

                    Dim outfiles = files.OrderByDescending(Function(o) Len(o.FileEntryRelativePath)).Select(Function(p) p.FE)
                    Dim FileShareId As Integer = outfiles.FirstOrDefault().FileShareId
                    Dim shareName As String = String.Empty

                    For Each FE As FileEntry In outfiles

                        shareName = FE.FileShare.ShareContainerName

                        'Delete first from azure file, if done successfully
                        If (FE.FileEntryTypeId = FileType.Folder) Then
                            res = Util.Helper.DeleteFolder(FileEntryId:=FE.FileEntryId, ShareName:=shareName, IsDeleted:=FE.IsDeleted, FileShareId:=FE.FileShareId, user:=Me.User)
                            If (res.Status = Status.Error OrElse res.Status = Status.NotFound) Then
                                Return res
                            End If
                        Else
                            res = Util.Helper.DeleteFile(FileEntryId:=FE.FileEntryId, ShareName:=shareName, IsDeleted:=FE.IsDeleted, FileShareId:=FE.FileShareId, user:=Me.User)
                            If (res.Status = Status.Error) Then
                                Return res
                            End If
                        End If


                        'Manage Links
                        Dim inPrevLink = service.FileEntryLinks.FirstOrDefault(Function(p) p.PreviousFileEntryId = FE.FileEntryId)
                        Dim inNewLink = service.FileEntryLinks.FirstOrDefault(Function(p) p.FileEntryId = FE.FileEntryId)

                        If (inPrevLink IsNot Nothing) Then
                            If (inNewLink IsNot Nothing) Then
                                'file is linked both ways, in this case link the other 2 ends and remove one link record
                                inPrevLink.PreviousFileEntryId = inNewLink.PreviousFileEntryId
                                service.FileEntryLinks.Remove(inNewLink)
                            Else
                                'file just exists on one side; delete the link record
                                service.FileEntryLinks.Remove(inPrevLink)
                            End If
                        ElseIf (inNewLink IsNot Nothing) Then
                            'file just exists on one side; delete the link record
                            service.FileEntryLinks.Remove(inNewLink)
                        End If

                        FE.IsCheckedOut = False
                        FE.CheckedOutByUserId = Nothing
                        FE.CheckedOutOnUTC = Nothing
                        FE.CheckedOutWorkSpaceId = Nothing

                        FE.IsDeleted = True
                        FE.IsPermanentlyDeleted = True
                        FE.DeletedByUserId = userId
                        FE.DeletedOnUTC = DateTime.UtcNow()
                        FE.PermanentlyDeletedByUserId = userId
                        FE.PermanentlyDeletedOnUTC = DateTime.UtcNow()

                        For Each FEV In FE.FileVersions
                            FEV.IsDeleted = True
                            FEV.DeletedByUserId = userId
                            FEV.DeletedOnUTC = DateTime.UtcNow()

                            If (FEV.ServerFileName IsNot Nothing) Then
                                If (Not lstServerFileName.Exists(Function(p) p = FEV.ServerFileName)) Then
                                    lstServerFileName.Add(FEV.ServerFileName)
                                End If
                            End If
                        Next

                        ' Get all the filenames for conflicted files which re uploaded to server
                        For Each Conflict In FE.FileVersionConflicts
                            If (Conflict.FileVersionConflictRequest IsNot Nothing AndAlso Conflict.FileVersionConflictRequest.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded AndAlso (Not lstServerFileName.Exists(Function(p) p = Conflict.FileVersionConflictId))) Then
                                lstServerFileName.Add(Conflict.FileVersionConflictId)
                            End If
                        Next

                    Next

                    res = DeleteFromFileEntryDeleteGroupHierarchy(service, FileEntryId)
                    If (res.Data = False) Then
                        Return res
                    End If

                    service.SaveChanges()

                    Dim lstServerFileNameFinal = (From n In lstServerFileName
                                                  Group Join v In service.FileVersions.Where(Function(p) Not p.IsDeleted)
                                                 On n Equals v.ServerFileName Into ov = Group
                                                  From t1 In ov.DefaultIfEmpty()
                                                  Group Join c In service.FileVersionConflictRequests.Where(Function(p) p.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded AndAlso Not p.FileVersionConflict.IsResolved)
                                                 On n Equals c.FileVersionConflictId Into oc = Group
                                                  From t2 In oc.DefaultIfEmpty()
                                                  Where t1 Is Nothing AndAlso t2 Is Nothing
                                                  Select n).ToList()

                    'Delete the blobs from Azure storage
                    Dim deleteBlobResult As ResultInfo(Of Boolean, Status) = Util.Helper.DeleteBlobs(FileShareId, lstServerFileNameFinal, Me.User)

                    If (deleteBlobResult.Data) Then

                        res.Data = deleteBlobResult.Data
                        res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        res.Status = Status.Success
                    Else
                        res.Data = deleteBlobResult.Data
                        res.Message = deleteBlobResult.Message
                        res.Status = deleteBlobResult.Status
                    End If
                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILE_NOTFOUND
                    res.Status = Status.NotFound
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function


    ''' <summary>
    ''' Restore deleted files/folders
    ''' </summary>
    ''' <param name="FileEntryId">Id of the source to be restored</param>
    ''' <returns>ResultInfo Object</returns>
    Public Function UndoDelete(FileEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.UndoDelete
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.Error}
            Dim userId As Guid = Me.User.UserId
            Dim resultMessage As String = ServiceConstants.FOLDERFILE_RESTORED

            Using service = GetServerEntity()

                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)
                If (file Is Nothing) Then
                    res.Data = False
                    res.Message = ServiceConstants.FILE_NOTFOUND
                    res.Status = Status.NotFound
                    Return res
                End If

                If (Not IsUserAdmin(service, Me.User, FileEntryId)) Then
                    res.Data = False
                    res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                    res.Status = Status.AccessDenied
                    Return res
                End If

                If (file.IsPermanentlyDeleted) Then
                    res.Data = True
                    res.Message = "File is permanently deleted"
                    res.Status = Status.PermanentlyDeleted
                    Return res
                End If


                Dim latestFileVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = file.FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
                If (latestFileVersion Is Nothing) Then
                    res.Data = False
                    res.Message = "Latest File Version not found"
                    res.Status = Status.NotFound
                    Return res
                End If

                'Check Parent has entry in FileEntryDeleteGroupHierarchy
                Dim FileEntryDeleteGroupHierarchy = service.FileEntryDeleteGroupHierarchies.FirstOrDefault(Function(p) p.ParentFileEntryId = latestFileVersion.FileEntryId)
                If (Not IsFilePathValid(service, latestFileVersion.ParentFileEntryId) And FileEntryDeleteGroupHierarchy Is Nothing) Then
                    res.Data = False
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                    Return res
                End If

                Dim filechk = service.GetLatestFileVersionByPath(latestFileVersion.FileEntryRelativePath, file.FileShareId)

                If (filechk Is Nothing OrElse filechk.Count = 0 OrElse filechk.FirstOrDefault().FileEntryId = FileEntryId) Then

                    Dim files As IQueryable = Nothing
                    'files = GetFileEntryDeleteGroupHierarchies(service, FileEntryId)
                    Dim filehierarchy = service.GetFileEntryChildrenHierarchyInclDeleted(FileEntryId)

                    If (file.FileEntryTypeId = FileType.File) Then
                        files = (From FE As FileEntry In service.FileEntries
                                 Join a In filehierarchy
                                 On a.FileEntryId Equals FE.FileEntryId
                                 Join FV As FileVersion In service.FileVersions
                                     On FV.FileEntryId Equals FE.FileEntryId
                                 Where FE.CurrentVersionNumber = FV.VersionNumber And FE.IsDeleted = True And FV.IsDeleted = True And FE.IsPermanentlyDeleted = False
                                 Select FV
                                 )
                    Else
                        files = (From FE As FileEntry In service.FileEntries
                                 Join a In filehierarchy
                                 On a.FileEntryId Equals FE.FileEntryId
                                 Join FV As FileVersion In service.FileVersions
                                     On FV.FileEntryId Equals FE.FileEntryId
                                 Where FE.CurrentVersionNumber = FV.VersionNumber And FE.IsDeleted = True And FV.IsDeleted = True And FE.IsPermanentlyDeleted = False 'And Not service.FileEntryDeleteGroupHierarchies.Any(Function(p) p.FileEntryId = a.FileEntryId)
                                 Select FV
                                ).OrderBy(Function(p) Len(p.FileEntryRelativePath))

                    End If

                    Dim nonRestoreFolderList = New List(Of String)
                    If (file.FileEntryTypeId = FileType.Folder) Then
                        Dim recycleFolders = (From fe In service.FileEntries
                                              Join a In service.FileEntryDeleteGroupHierarchies
                                               On fe.FileEntryId Equals a.FileEntryId
                                              Join fv In service.FileVersions
                                                  On fe.FileEntryId Equals fv.FileEntryId
                                              Where fe.FileEntryTypeId = FileType.Folder And Not (a.FileEntryId = file.FileEntryId)
                                              Select fv)
                        For Each obj In recycleFolders
                            nonRestoreFolderList.Add(obj.FileEntryRelativePath)
                        Next
                    End If
                    Dim path = String.Empty
                    For Each FE As FileVersion In files

                        Dim recycleInfo = (From fv In service.FileEntries
                                           Join a In service.FileEntryDeleteGroupHierarchies.Where(Function(p) p.FileEntryId = FE.FileEntryId)
                                              On fv.FileEntryId Equals a.FileEntryId
                                           Select fv).FirstOrDefault()


                        If (FE.FileEntry.FileEntryTypeId = FileType.File) Then
                            path = Helper.GetFolderFullName(FE.FileEntryRelativePath)
                        Else
                            path = FE.FileEntryRelativePath
                        End If

                        If ((recycleInfo IsNot Nothing _
                                AndAlso Not (recycleInfo.FileEntryTypeId = FileType.Folder) _
                                AndAlso file.FileEntryTypeId = FileType.Folder) _
                            OrElse nonRestoreFolderList.Contains(path)) Then
                            Continue For
                        End If

                        latestFileVersion = FE
                        'undo delete from Azure file storage
                        Dim FileEntryVersionInfo = MapFromObject(latestFileVersion, file.FileShareId)
                        FileEntryVersionInfo.FileEntry.IsDeleted = file.IsDeleted

                        If (latestFileVersion.FileEntry.FileEntryTypeId = FileType.Folder) Then
                            'Restore Folder
                            Dim fileDirectory = Util.Helper.CreateAzureFileDirectory(StorageConnectionString:=Me.GetStorageAccount, ShareName:=file.FileShare.ShareContainerName, FolderName:=FileEntryVersionInfo.FileEntryRelativePath + "\")
                            resultMessage = String.Format(resultMessage, String.Format("Folder '{0}'", latestFileVersion.FileEntryName))

                            UndoDeleteFileVersionEntry(service, FE)

                        Else
                            'Restore File to Azure storage
                            Dim resResult = Me.CopyFromBlobToFile(FileEntryVersionsInfo:=FileEntryVersionInfo)
                            If (resResult.Status = Status.Error) Then
                                'res.Data = resResult.Data
                                'res.Message = resResult.Message
                                'res.Status = resResult.Status
                                'Return res
                                Return resResult
                            End If

                            UndoDeleteFileVersionEntry(service, FE)

                            resultMessage = String.Format(resultMessage, String.Format("File '{0}'", FE.FileEntryName))
                        End If

                        ' delete the associated record in Links table
                        Dim link = service.FileEntryLinks.FirstOrDefault(Function(p) p.PreviousFileEntryId = FileEntryId AndAlso Not p.IsDeleted) '

                        If link IsNot Nothing Then
                            service.FileEntryLinks.Remove(link)
                        End If

                        res.Data = True

                    Next

                    'Everything is restored now time to delete the entry from staging table.
                    res = DeleteFromFileEntryDeleteGroupHierarchy(service, FileEntryId)

                    If (res.Data = False) Then
                        Return res
                    End If

                    ' Undelete the file
                    file.IsDeleted = False
                    file.DeletedByUserId = Nothing
                    file.DeletedOnUTC = Nothing

                    res.Data = True
                    res.Message = resultMessage
                    res.Status = Status.Success

                    'Save all changes in one go
                    service.SaveChanges()

                Else
                    res.Data = False
                    res.Status = Status.AlreadyExists
                    res.Message = "File/Folder/FileShare Already Exists"

                End If

            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Rename the file
    ''' </summary>
    ''' <param name="FileEntryId">Entry Id of the File</param>
    ''' <param name="localVersionNumber">Local Version of the file</param>
    ''' <param name="newName">New name of the file</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Public Function RenameFile(FileEntryId As Guid, localVersionNumber As Integer, newName As String) As ResultInfo(Of Boolean, Status) Implements IFileRepository.RenameFile
        Try

            Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

            If retResult.Data = Guid.Empty Then
                Return BuildResponse(Of Boolean)(False, Status.None, "Server WorkSpaceId not found")
            End If
            Dim workSpaceId As Guid = retResult.Data

            Using Service = GetServerEntity()
                If IsFilePathValid(Service, FileEntryId) Then

                    Dim file = Service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)
                    If file IsNot Nothing Then

                        If (file.CurrentVersionNumber = localVersionNumber) Then

                            Dim permission = GetEffectivePermission(Service, Me.User, FileEntryId)
                            If (permission And PermissionType.Write) = PermissionType.Write Then

                                If (Not file.IsCheckedOut OrElse (file.CheckedOutByUserId = Me.User.UserId AndAlso file.CheckedOutWorkSpaceId = workSpaceId)) Then

                                    Dim latestVersion = file.FileVersions.FirstOrDefault(Function(p) p.VersionNumber = file.CurrentVersionNumber)
                                    If latestVersion IsNot Nothing Then

                                        Dim newRelativePath = newName
                                        If latestVersion.FileEntryRelativePath.LastIndexOf("\") <> -1 Then
                                            newRelativePath = String.Format("{0}\{1}", latestVersion.FileEntryRelativePath.Substring(0, latestVersion.FileEntryRelativePath.LastIndexOf("\")), newName)
                                        End If

                                        Dim newFile = Service.GetLatestFileVersionByPath(newRelativePath, file.FileShareId)

                                        If (newFile.Count = 0) Then

                                            If (file.FileEntryTypeId = FileType.File) Then
                                                'Case of File Rename
                                                If Not file.IsCheckedOut Then
                                                    file.IsCheckedOut = True
                                                    file.CheckedOutByUserId = Me.User.UserId
                                                    file.CheckedOutOnUTC = DateTime.UtcNow()
                                                    file.CheckedOutWorkSpaceId = workSpaceId

                                                    Service.SaveChanges()
                                                End If
                                                Dim lastIndex = newName.LastIndexOf(".")
                                                Dim filename
                                                Dim extn
                                                If lastIndex = -1 Then
                                                    filename = newName
                                                    extn = ""
                                                Else
                                                    filename = newName.Substring(0, lastIndex)
                                                    extn = newName.Substring(lastIndex)
                                                End If
                                                Dim newVersionNumber = latestVersion.VersionNumber + 1

                                                Dim newVersion As New FileVersion()
                                                file.FileVersions.Add(newVersion)
                                                With newVersion
                                                    .FileVersionId = Guid.NewGuid()
                                                    .FileEntryId = latestVersion.FileEntryId
                                                    .ParentFileEntryId = latestVersion.ParentFileEntryId
                                                    .PreviousFileVersionId = latestVersion.FileVersionId
                                                    .FileEntryExtension = extn
                                                    .FileEntryHash = latestVersion.FileEntryHash
                                                    .FileEntryName = filename
                                                    .FileEntryRelativePath = newRelativePath
                                                    .FileEntrySize = latestVersion.FileEntrySize
                                                    .IsDeleted = False
                                                    .ServerFileName = latestVersion.ServerFileName
                                                    .VersionNumber = newVersionNumber
                                                    .CreatedByUserId = Me.User.UserId
                                                    .CreatedOnUTC = DateTime.UtcNow()
                                                End With

                                                file.IsCheckedOut = False
                                                file.CheckedOutByUserId = Nothing
                                                file.CheckedOutOnUTC = Nothing
                                                file.CheckedOutWorkSpaceId = Nothing
                                                file.CurrentVersionNumber = newVersionNumber

                                                'As such no rename futionality is avaliable so we need to copy and delete file
                                                Dim res = Me.CopyAndDeleteFileFromFileShare(latestVersion.FileEntryId, MapFromObject(newVersion, file.FileShareId))

                                                If (res.Status = Status.Error) Then
                                                    Return BuildResponse(res.Data, res.Status, res.Message)
                                                End If

                                                Service.SaveChanges()

                                            Else
                                                'Case Of Folder Rename
                                                'Get all childrens
                                                Dim childrens = From f In Service.FileEntries
                                                                Join v In Service.FileVersions
                                                                On f.FileEntryId Equals v.FileEntryId And f.CurrentVersionNumber Equals v.VersionNumber
                                                                Where v.FileEntryRelativePath.StartsWith(latestVersion.FileEntryRelativePath) And f.FileEntryId <> latestVersion.FileEntryId
                                                                Select New With {.FileEntryId = f.FileEntryId, .IsCheckedOut = f.IsCheckedOut, .CheckedOutByUserId = f.CheckedOutByUserId, .CheckedOutWorkSpaceId = f.CheckedOutWorkSpaceId}

                                                'Check whether any child is checkedOut
                                                If Not (childrens.Any(Function(p) p.IsCheckedOut AndAlso (p.CheckedOutByUserId <> Me.User.UserId OrElse (p.CheckedOutByUserId = Me.User.UserId AndAlso p.CheckedOutWorkSpaceId <> workSpaceId)))) Then

                                                    'Check For Deny Permission
                                                    If (Me.User.UserAccount.UserTypeId <> Role.AccountAdmin AndAlso (permission And PermissionType.ShareAdmin) = 0) Then
                                                        'Check By User
                                                        Dim DenyPermissionCnt = (From c In childrens
                                                                                 Join p In Service.UserFileEntryPermissions.Where(Function(p) p.UserId = Me.User.UserId AndAlso Not p.IsDeleted)
                                                                             On c.FileEntryId Equals p.FileEntryId
                                                                                 Where (p.DeniedPermissions And PermissionType.Write) = PermissionType.Write
                                                                                 Select 1).Count()

                                                        'Check By UserGroup if not found by user
                                                        If (DenyPermissionCnt = 0) Then
                                                            DenyPermissionCnt = (From ug In Service.UserGroupAssignments.Where(Function(p) p.UserId = Me.User.UserId And Not p.IsDeleted)
                                                                                 Join g In Service.Groups.Where(Function(p) Not p.IsDeleted)
                                                                                 On ug.GroupId Equals g.GroupId
                                                                                 Join p In Service.GroupFileEntryPermissions.Where(Function(p) Not p.IsDeleted)
                                                                                On g.GroupId Equals p.GroupId
                                                                                 Join c In childrens
                                                                                On c.FileEntryId Equals p.FileEntryId
                                                                                 Where (p.DeniedPermissions And PermissionType.Write) = PermissionType.Write
                                                                                 Select 1).Count()

                                                        End If

                                                        If (DenyPermissionCnt > 0) Then
                                                            Return BuildResponse(False, Status.AccessDenied, "User Or Group has a Denied Write Permission on one or more subsequent file/Folder")
                                                        End If
                                                    End If
                                                    'All Checks passed. No rename the folder and all subsequent files and folders
                                                    'First CheckOut
                                                    Dim FileQuery = From c In childrens
                                                                    Join f In Service.FileEntries
                                                            On c.FileEntryId Equals f.FileEntryId
                                                                    Select f

                                                    file.IsCheckedOut = True
                                                    file.CheckedOutByUserId = Me.User.UserId
                                                    file.CheckedOutOnUTC = DateTime.UtcNow()
                                                    file.CheckedOutWorkSpaceId = workSpaceId

                                                    For Each FE In FileQuery
                                                        FE.IsCheckedOut = True
                                                        FE.CheckedOutByUserId = Me.User.UserId
                                                        FE.CheckedOutOnUTC = DateTime.UtcNow()
                                                        FE.CheckedOutWorkSpaceId = workSpaceId
                                                    Next

                                                    Service.SaveChanges()

                                                    'Now Create A new Version
                                                    Dim newVersionNumber = latestVersion.VersionNumber + 1
                                                    Dim oldRelativePath = latestVersion.FileEntryRelativePath

                                                    Dim newVersion As New FileVersion()
                                                    file.FileVersions.Add(newVersion)
                                                    With newVersion
                                                        .FileVersionId = Guid.NewGuid()
                                                        .FileEntryId = latestVersion.FileEntryId
                                                        .ParentFileEntryId = latestVersion.ParentFileEntryId
                                                        .PreviousFileVersionId = latestVersion.FileVersionId
                                                        .FileEntryExtension = String.Empty
                                                        .FileEntryHash = latestVersion.FileEntryHash
                                                        .FileEntryName = newName
                                                        .FileEntryRelativePath = newRelativePath
                                                        .FileEntrySize = latestVersion.FileEntrySize
                                                        .IsDeleted = False
                                                        .ServerFileName = latestVersion.ServerFileName
                                                        .VersionNumber = newVersionNumber
                                                        .CreatedByUserId = Me.User.UserId
                                                        .CreatedOnUTC = DateTime.UtcNow()
                                                    End With

                                                    For Each FE In FileQuery
                                                        Dim latestFileVersion = FE.FileVersions.Where(Function(p) p.VersionNumber = FE.CurrentVersionNumber).FirstOrDefault()

                                                        If (latestFileVersion IsNot Nothing) Then
                                                            latestFileVersion.FileEntryRelativePath = ReplaceFirst(latestFileVersion.FileEntryRelativePath, oldRelativePath, newRelativePath)
                                                            FE.IsCheckedOut = False
                                                            FE.CheckedOutByUserId = Nothing
                                                            FE.CheckedOutOnUTC = Nothing
                                                            FE.CheckedOutWorkSpaceId = Nothing
                                                        Else
                                                            Return BuildResponse(False, Status.NotFound, "Latest Version Not Found for one of the subsequent file/folder")
                                                        End If
                                                    Next

                                                    'Remove CheckOut lock
                                                    file.IsCheckedOut = False
                                                    file.CheckedOutByUserId = Nothing
                                                    file.CheckedOutOnUTC = Nothing
                                                    file.CheckedOutWorkSpaceId = Nothing
                                                    file.CurrentVersionNumber = newVersionNumber

                                                    Service.SaveChanges()

                                                Else
                                                    Return BuildResponse(False, Status.FileCheckedOutByDifferentUser, "At least one of the subsequent File is checked out by a different user/WorkSpace")
                                                End If
                                            End If
                                        Else
                                            Return BuildResponse(False, Status.AlreadyExists, ServiceConstants.FILEFOLDER_ALREADY_EXISTS)
                                        End If
                                    Else
                                        Return BuildResponse(False, Status.NotFound, "Latest Version Not Found")
                                    End If
                                Else
                                    Dim userName As String = "/WorkSpace"
                                    If file.CheckedOutByUserId <> Me.User.UserId Then
                                        If file.CheckedOutByUserId.HasValue Then
                                            Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                            If userResult.Status = Status.Success Then
                                                userName = userResult.Data.UserName
                                            End If
                                        End If
                                    End If

                                    Return BuildResponse(False, Status.FileCheckedOutByDifferentUser, String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName))
                                End If
                            Else
                                Return BuildResponse(False, Status.AccessDenied, ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION)
                            End If
                        Else
                            Return BuildResponse(False, Status.VersionNotLatest, ServiceConstants.FILE_VERSION_MISMATCHED)
                        End If
                    Else
                        Return BuildResponse(False, Status.NotFound, ServiceConstants.FILE_NOTFOUND)
                    End If
                Else
                    Return BuildResponse(False, Status.PathNotValid, ServiceConstants.FILEPATH_NOLONGERVALID)
                End If

                Return BuildResponse(True, Status.Success, ServiceConstants.OPERATION_SUCCESSFUL)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddFileLink(FileEntryId As Guid, linkedFileId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.AddFileLink
        Try
            Using service = GetServerEntity()
                Dim newObj = New FileEntryLink()
                newObj.FileEntryLinkId = Guid.NewGuid()
                newObj.FileEntryId = FileEntryId
                newObj.PreviousFileEntryId = linkedFileId
                newObj.IsDeleted = False
                newObj.CreatedByUserId = Me.User.UserId
                newObj.CreatedOnUTC = DateTime.UtcNow()

                service.FileEntryLinks.Add(newObj)
                service.SaveChanges()

                Return BuildResponse(True)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function RemoveFileLink(FileEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.RemoveFileLink
        Try
            Using service = GetServerEntity()

                Dim file = service.FileEntryLinks.FirstOrDefault(Function(p) p.PreviousFileEntryId = FileEntryId AndAlso Not p.IsDeleted)
                If file IsNot Nothing Then
                    file.IsDeleted = True
                    file.DeletedByUserId = Me.User.UserId
                    file.DeletedOnUTC = DateTime.UtcNow()
                    service.SaveChanges()
                End If

                Return BuildResponse(True)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AllFilesForLinking(FileEntryId As Guid, FileShareId As Integer) As ResultInfo(Of List(Of FileEntryLinkInfo), Status) Implements IFileRepository.AllFilesForLinking
        Try
            Dim lst As List(Of FileEntryLinkInfo) = New List(Of FileEntryLinkInfo)()

            Using service = GetServerEntity()
                Dim allFiles = service.FileEntries.Where(Function(p) p.FileShareId = FileShareId AndAlso Not p.IsDeleted AndAlso Not p.IsPermanentlyDeleted AndAlso p.FileEntryTypeId = FileType.File AndAlso p.FileEntryId <> FileEntryId)
                If (allFiles IsNot Nothing) Then
                    For Each item In allFiles
                        'If (service.FileEntryLinks.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.PreviousFileEntryId = item.FileEntryId) Is Nothing) Then
                        If (service.FileEntryLinks.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = item.FileEntryId) Is Nothing) Then
                            If (item.FileVersions IsNot Nothing AndAlso item.FileVersions.FirstOrDefault(Function(p) p.IsDeleted = False) IsNot Nothing) Then
                                Dim latestFV = item.FileVersions.OrderByDescending(Function(p) p.VersionNumber).Where(Function(p) p.IsDeleted = False)(0)
                                Dim t = New FileEntryLinkInfo With {
                                        .FileEntryId = item.FileEntryId,
                                        .LatestFileVersionFileEntryPathRelativeToShare = latestFV.FileEntryRelativePath
                                     }
                                lst.Add(t)
                            End If
                        End If
                    Next
                End If
            End Using
            Return BuildResponse(lst)
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryLinkInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Undo File Check Out
    ''' </summary>
    ''' <param name="FileEntryId">file entry Id of the file to be undo checkedout</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    ''' <remarks>File will be undocheckout either by the onwer of the file or by the Account admin</remarks>
    Public Function UndoCheckOut(FileEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.UndoCheckOut
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using service = GetServerEntity()

                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryId)
                If (file IsNot Nothing) Then
                    If (file.IsCheckedOut = True AndAlso (file.CheckedOutByUserId = userId OrElse Me.User.UserAccount.UserTypeId = Role.AccountAdmin)) Then
                        file.IsCheckedOut = False
                        file.CheckedOutByUserId = Nothing
                        file.CheckedOutOnUTC = Nothing
                        file.CheckedOutWorkSpaceId = Nothing
                        service.SaveChanges()
                        res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        res.Status = Status.Success
                        res.Data = True
                    Else
                        If (file.IsCheckedOut = False) Then
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success
                            res.Data = True
                        Else
                            Dim userName As String = String.Empty
                            If file.CheckedOutByUserId.HasValue Then
                                Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                If userResult.Status = Status.Success Then
                                    userName = userResult.Data.UserName
                                End If
                            End If

                            res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                            res.Status = Status.FileCheckedOutByDifferentUser
                        End If
                    End If
                Else
                    res.Message = ServiceConstants.FILE_NOTFOUND
                    res.Status = Status.NotFound
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Checkout file
    ''' </summary>
    ''' <param name="FileEntryId">File entry id of the file to be checked out</param>
    ''' <param name="localFileVersionNumber">local version of the file</param>
    ''' <param name="workSpaceId">Work space id of the file</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Public Function CheckOut(FileEntryId As Guid, localFileVersionNumber As Integer, workSpaceId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.CheckOut
        Try
            Try
                Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

                If retResult.Data <> Guid.Empty Then
                    workSpaceId = retResult.Data
                End If
            Catch
            End Try

            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using service = GetServerEntity()
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryId)
                If (file IsNot Nothing) Then
                    If (IsFilePathValid(service, FileEntryId)) Then

                        If (localFileVersionNumber = file.CurrentVersionNumber) Then

                            If (Not file.IsCheckedOut) Then

                                If (HasWritePermission(service, Me.User, file.FileEntryId)) Then
                                    file.IsCheckedOut = True
                                    file.CheckedOutByUserId = userId
                                    file.CheckedOutOnUTC = DateTime.UtcNow()
                                    file.CheckedOutWorkSpaceId = workSpaceId
                                    service.SaveChanges()

                                    res.Data = True
                                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                    res.Status = Status.Success
                                Else
                                    res.Data = False
                                    res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                    res.Status = Status.AccessDenied
                                End If
                            Else
                                If (file.IsCheckedOut AndAlso file.CheckedOutByUserId <> userId) Then
                                    Dim userName As String = String.Empty
                                    If file.CheckedOutByUserId.HasValue Then
                                        Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                        If userResult.Status = Status.Success Then
                                            userName = userResult.Data.UserName
                                        End If
                                    End If

                                    res.Data = False
                                    res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                                    res.Status = Status.FileCheckedOutByDifferentUser
                                Else
                                    If (file.IsCheckedOut AndAlso file.CheckedOutWorkSpaceId <> workSpaceId) Then
                                        res.Data = False
                                        res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, "/WorkSpace")
                                        res.Status = Status.FileCheckedOutByDifferentUser
                                    Else
                                        res.Data = True
                                        res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                        res.Status = Status.Success
                                    End If
                                End If
                            End If
                        Else
                            res.Data = False
                            res.Message = "File version mismatched"
                            res.Status = Status.VersionNotLatest
                        End If
                    Else
                        res.Data = False
                        res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                        res.Status = Status.PathNotValid
                    End If

                Else
                    res.Data = False
                    res.Message = "File not found to checkout"
                    res.Status = Status.NotFound
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Function CheckOutWithoutVersion(FileEntryId As Guid, workSpaceId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.CheckOutWithoutVersion
        Try
            Try
                If (workSpaceId = Guid.Empty) Then
                    Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

                    If retResult.Data <> Guid.Empty Then
                        workSpaceId = retResult.Data
                    End If
                End If

            Catch
            End Try
            Dim userId As Guid = Nothing
            If (Me.User IsNot Nothing) Then
                userId = Me.User.UserId
            End If

            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using service = GetServerEntity()
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryId)
                If (file IsNot Nothing) Then
                    If (IsFilePathValid(service, FileEntryId)) Then

                        If (Not file.IsCheckedOut) Then

                            If (Me.User.UserId = Guid.Empty OrElse HasWritePermission(service, Me.User, file.FileEntryId)) Then
                                file.IsCheckedOut = True
                                file.CheckedOutByUserId = userId
                                file.CheckedOutOnUTC = DateTime.UtcNow()
                                file.CheckedOutWorkSpaceId = workSpaceId
                                service.SaveChanges()

                                res.Data = True
                                res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                res.Status = Status.Success
                            Else
                                res.Data = False
                                res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                res.Status = Status.AccessDenied
                            End If
                        Else
                            If (file.IsCheckedOut AndAlso file.CheckedOutByUserId <> userId) Then
                                Dim userName As String = String.Empty
                                If file.CheckedOutByUserId.HasValue Then
                                    Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                    If userResult.Status = Status.Success Then
                                        userName = userResult.Data.UserName
                                    End If
                                End If

                                res.Data = False
                                res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                                res.Status = Status.FileCheckedOutByDifferentUser
                            Else
                                If (file.IsCheckedOut AndAlso file.CheckedOutWorkSpaceId <> workSpaceId) Then
                                    res.Data = False
                                    res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, "/WorkSpace")
                                    res.Status = Status.FileCheckedOutByDifferentUser
                                Else
                                    res.Data = True
                                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                    res.Status = Status.Success
                                End If
                            End If
                        End If

                    Else
                        res.Data = False
                        res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                        res.Status = Status.PathNotValid
                    End If

                Else
                    res.Data = False
                    res.Message = "File not found to checkout"
                    res.Status = Status.NotFound
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' CheckIn the file 
    ''' </summary>
    ''' <param name="FileEntryVersionsInfo">FileVersionInfo Object</param>
    ''' <param name="localFileVersionNumber">Local version of the file</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Public Function CheckIn(FileEntryVersionsInfo As Model.FileVersionInfo, localFileVersionNumber As Integer) As ResultInfo(Of Integer, Status) Implements IFileRepository.CheckIn
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Integer, Status) = New ResultInfo(Of Integer, Status)
            res.Data = -1

            Using Service = GetServerEntity()
                Dim file = Service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryVersionsInfo.FileEntryId)
                If (file IsNot Nothing) Then
                    If (IsFilePathValid(Service, FileEntryVersionsInfo.FileEntryId)) Then

                        If (file.CurrentVersionNumber = localFileVersionNumber) Then
                            If (file.IsCheckedOut = True AndAlso (userId = Guid.Empty OrElse file.CheckedOutByUserId = userId)) Then

                                If (userId = Guid.Empty OrElse HasWritePermission(Service, Me.User, file.FileEntryId)) Then

                                    If (Guid.Empty = FileEntryVersionsInfo.FileVersionId) Then
                                        'Dim currentVersionNumber = If(file.FileVersions.Max(Function(p) p.VersionNumber) = Nothing, 0, file.FileVersions.Max(Function(p) p.VersionNumber)) + 1
                                        Dim currentVersionNumber = localFileVersionNumber + 1
                                        FileEntryVersionsInfo.VersionNumber = currentVersionNumber
                                        FileEntryVersionsInfo.CreatedByUserId = userId
                                        FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()
                                        FileEntryVersionsInfo.IsDeleted = False
                                        Dim fileVersionRepository As FileVersionRepository = New FileVersionRepository()
                                        fileVersionRepository.fncUser = Me.fncUser
                                        file.IsCheckedOut = False
                                        file.CheckedOutByUserId = Nothing
                                        file.CheckedOutOnUTC = Nothing
                                        file.CheckedOutWorkSpaceId = Nothing
                                        file.CurrentVersionNumber = currentVersionNumber
                                        Service.FileVersions.Add(fileVersionRepository.MapFromObject(FileEntryVersionsInfo))
                                        res.Data = currentVersionNumber
                                    Else
                                        Dim fileVersion = Service.FileVersions.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileVersionId = FileEntryVersionsInfo.FileVersionId)
                                        If (fileVersion IsNot Nothing) Then
                                            ' TODO: Check for VersionNumber 
                                            fileVersion.VersionNumber = Convert.ToInt32(fileVersion.VersionNumber) + 1
                                            file.IsCheckedOut = False
                                            file.CheckedOutByUserId = Nothing
                                            file.CheckedOutOnUTC = Nothing
                                            file.CheckedOutWorkSpaceId = Nothing
                                            file.CurrentVersionNumber = fileVersion.VersionNumber
                                            res.Data = fileVersion.VersionNumber
                                        Else
                                            'Dim currentVersionNumber = If(file.FileVersions.Max(Function(p) p.VersionNumber) = Nothing, 0, Service.FileVersions.Max(Function(p) p.VersionNumber)) + 1
                                            Dim currentVersionNumber = localFileVersionNumber + 1
                                            FileEntryVersionsInfo.VersionNumber = currentVersionNumber
                                            FileEntryVersionsInfo.CreatedByUserId = userId
                                            FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()
                                            FileEntryVersionsInfo.IsDeleted = False
                                            Dim fileVersionRepository As FileVersionRepository = New FileVersionRepository()
                                            fileVersionRepository.fncUser = Me.fncUser
                                            file.IsCheckedOut = False
                                            file.CheckedOutByUserId = Nothing
                                            file.CheckedOutOnUTC = Nothing
                                            file.CheckedOutWorkSpaceId = Nothing
                                            file.CurrentVersionNumber = currentVersionNumber
                                            Service.FileVersions.Add(fileVersionRepository.MapFromObject(FileEntryVersionsInfo))
                                            res.Data = currentVersionNumber
                                        End If
                                    End If
                                    Service.SaveChanges()

                                    Dim msg As String = String.Empty
                                    'Set VersionId and FileNameAndExtension
                                    Dim rs = Service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = FileEntryVersionsInfo.FileEntryId AndAlso p.VersionNumber = res.Data)
                                    FileEntryVersionsInfo.FileEntryNameWithExtension = rs.FileEntryNameWithExtension
                                    FileEntryVersionsInfo.FileVersionId = rs.FileVersionId

                                    'Set Metadata values for the file on Azure
                                    Try
                                        Dim repository = New SyncRepository()
                                        repository.fncUser = Me.fncUser
                                        With Util.Helper.GetBlockBlobReference(FileShareId:=file.FileShareId, BlobName:=FileEntryVersionsInfo.ServerFileName, repository:=repository)
                                            .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyIsUploadFinished"), value:=Boolean.TrueString)
                                            .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyFileName"), value:=FileEntryVersionsInfo.FileEntryNameWithExtension.ToString())
                                            .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyVersionNumber"), value:=res.Data.ToString())
                                            .SetMetadata()
                                        End With
                                    Catch ex As Exception
                                        msg = String.Format("File Sucessfully Checked-in, but failed to set metadata on Azure with Error: {0}", ex.Message)
                                    End Try

                                    'no longer required as this will be taken care of by cloud service
                                    'Try
                                    '    'Update FileShare blobsize field               
                                    '    Dim ConfigRepo = New ConfigRepository()
                                    '    ConfigRepo.User = Me.User
                                    '    Dim result = ConfigRepo.UpdateShareBlobSize(file.FileShareId)

                                    '    If (Not result.Data) Then
                                    '        If msg = String.Empty Then
                                    '            msg = String.Format("File Sucessfully Checked-in, but failed to set FileShare blobsize in database with message: {0}", result.Message)
                                    '        Else
                                    '            msg += String.Format("Also failed to set FileShare blobsize in database with message: {0}", result.Message)
                                    '        End If
                                    '    Else
                                    '        If msg = String.Empty Then
                                    '            msg = ServiceConstants.OPERATION_SUCCESSFUL
                                    '        End If

                                    '    End If
                                    'Catch ex As Exception
                                    '    If msg = String.Empty Then
                                    '        msg = String.Format("File Sucessfully Checked-in, but failed to set FileShare blobsize in database with Error: {0}", ex.Message)
                                    '    Else
                                    '        msg += String.Format("Also failed to set FileShare blobsize in database with Error: {0}", ex.Message)
                                    '    End If
                                    'End Try

                                    res.Message = msg
                                    res.Status = Status.Success

                                Else
                                    res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                    res.Status = Status.AccessDenied
                                End If

                            Else
                                Dim userName As String = String.Empty
                                If file.CheckedOutByUserId.HasValue Then
                                    Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(file.CheckedOutByUserId.Value)

                                    If userResult.Status = Status.Success Then
                                        userName = userResult.Data.UserName
                                    End If
                                End If

                                res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                                res.Status = Status.FileCheckedOutByDifferentUser
                            End If

                        Else
                            res.Message = "File version mismatched"
                            res.Status = Status.VersionNotLatest
                        End If
                    Else
                        res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                        res.Status = Status.PathNotValid
                    End If
                Else
                    res.Status = Status.NotFound
                    res.Message = ServiceConstants.FILE_NOTFOUND
                End If

            End Using

            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Add file info in the database and keep the file checkout
    ''' </summary>
    ''' <param name="FileEntryInfo">An object contains file related information</param>
    ''' <param name="FileEntryVersionsInfo">An object contains file version related information</param>
    ''' <param name="workSpaceId">Work Space Id</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Public Function AddFileAndCheckout(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo, workSpaceId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.AddFileAndCheckout
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetServerEntity()
                If (IsFilePathValid(Service, FileEntryVersionsInfo.ParentFileEntryId)) Then

                    Dim obj = Service.GetLatestFileVersionByPath(FileEntryVersionsInfo.FileEntryRelativePath, FileEntryInfo.FileShareId).FirstOrDefault()

                    If (obj IsNot Nothing) Then

                        If (obj.FileEntryId = FileEntryInfo.FileEntryId AndAlso obj.VersionNumber = 0) Then

                            Return CheckOut(obj.FileEntryId, 0, workSpaceId)
                        Else
                            res.Data = False
                            res.Status = Status.AlreadyExists
                            res.Message = "File/Folder/FileShare Already Exists"

                        End If
                    Else

                        If (HasWritePermission(Service, Me.User, FileEntryVersionsInfo.ParentFileEntryId)) Then

                            FileEntryInfo.CurrentVersionNumber = 0
                            FileEntryInfo.IsCheckedOut = True
                            FileEntryInfo.CheckedOutOnUTC = DateTime.UtcNow()
                            FileEntryInfo.CheckedOutByUserId = userId
                            FileEntryInfo.CheckOutWorkSpaceId = workSpaceId
                            FileEntryInfo.IsDeleted = False
                            FileEntryInfo.DeletedByUserId = Nothing
                            FileEntryInfo.DeletedOnUTC = Nothing
                            Dim FileEntry = MapFromObject(FileEntryInfo)

                            Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                            fileVersionRepo.fncUser = Me.fncUser
                            FileEntryVersionsInfo.VersionNumber = 0
                            FileEntryVersionsInfo.IsDeleted = False
                            FileEntryVersionsInfo.DeletedByUserId = Nothing
                            FileEntryVersionsInfo.DeletedOnUTC = Nothing
                            FileEntryVersionsInfo.CreatedByUserId = userId
                            FileEntryVersionsInfo.CreatedOnUTC = Date.UtcNow()
                            FileEntry.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionsInfo))
                            Service.FileEntries.Add(FileEntry)

                            Service.SaveChanges()

                            res.Data = True
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success
                        Else
                            res.Data = False
                            res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                            res.Status = Status.AccessDenied
                        End If
                    End If
                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddAndCheckoutWeb(parentId As Guid, fileName As String, blobName As Guid, fileSize As Long) As ResultInfo(Of FileEntryInfo, Status) Implements IFileRepository.AddAndCheckoutWeb
        Try
            Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

            If retResult.Data = Guid.Empty Then
                Return BuildResponse(Of FileEntryInfo)(Nothing, Status.None, "Server WorkSpaceId not found")
            End If
            Dim workSpaceId As Guid = retResult.Data

            Using service = GetServerEntity()
                Dim folder = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = parentId)
                Dim userId As Guid = Me.User.UserId
                Dim result As ResultInfo(Of Boolean, Status)
                Dim FileEntryInfo As FileEntryInfo

                Dim relativePath = String.Empty
                If (folder.FileEntryTypeId = FileType.Share) Then
                    relativePath = fileName
                Else
                    Dim folderVersion = folder.FileVersions.FirstOrDefault(Function(p) p.VersionNumber = folder.CurrentVersionNumber)

                    If (folderVersion IsNot Nothing) Then
                        relativePath = System.IO.Path.Combine(folderVersion.FileEntryRelativePath, fileName)
                    Else
                        Throw New Exception("Parent Version Not found")
                    End If
                End If

                Dim existingFile = service.GetLatestFileVersionByPath(relativePath, folder.FileShareId).FirstOrDefault()

                If (existingFile Is Nothing) Then

                    FileEntryInfo = New FileEntryInfo With {
                        .FileEntryId = Guid.NewGuid,
                        .FileEntryTypeId = FileType.File,
                        .FileShareId = folder.FileShareId
                    }

                    Dim FileEntryVersionInfo As New Model.FileVersionInfo
                    With FileEntryVersionInfo
                        .FileEntryId = FileEntryInfo.FileEntryId
                        .FileVersionId = blobName
                        .ParentFileEntryId = parentId
                        .FileEntryRelativePath = relativePath
                        .FileEntrySize = fileSize
                        .FileEntryName = System.IO.Path.GetFileNameWithoutExtension(fileName)
                        .FileEntryExtension = System.IO.Path.GetExtension(fileName)
                        .ServerFileName = blobName
                        .CheckInSource = F_Enums.CheckInSource.Web
                    End With

                    result = AddFileAndCheckout(FileEntryInfo, FileEntryVersionInfo, workSpaceId)

                Else
                    FileEntryInfo = [Get](existingFile.FileEntryId).Data
                    result = CheckOut(existingFile.FileEntryId, existingFile.VersionNumber, workSpaceId)
                End If

                Return BuildResponse(FileEntryInfo, result.Status, result.Message)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of FileEntryInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' AddFolder will create a folder in Azure File Stogage
    ''' </summary>
    ''' <param name="FileEntryInfo">An object contains file related information</param>
    ''' <param name="FileEntryVersionsInfo">An object contains file version related information</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Public Function AddFolder(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status) Implements IFileRepository.AddFolder
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetServerEntity()
                If (IsFilePathValid(Service, FileEntryVersionsInfo.ParentFileEntryId)) Then

                    Dim resFun = Service.GetLatestFileVersionByPath(FileEntryVersionsInfo.FileEntryRelativePath, FileEntryInfo.FileShareId)
                    If (resFun IsNot Nothing And resFun.Count() > 0) Then
                        Dim folder = resFun.FirstOrDefault()

                        If (folder.FileEntryId = FileEntryInfo.FileEntryId AndAlso folder.VersionNumber = 1) Then
                            res.Data = True
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success

                        Else
                            res.Data = False
                            res.Status = Status.AlreadyExists
                            res.Message = "File/Folder/FileShare Already Exists"
                        End If
                    Else
                        If (HasWritePermission(Service, Me.User, FileEntryVersionsInfo.ParentFileEntryId)) Then

                            FileEntryInfo.CurrentVersionNumber = 1
                            FileEntryInfo.IsCheckedOut = False
                            FileEntryInfo.CheckedOutOnUTC = Nothing
                            FileEntryInfo.CheckedOutByUserId = Nothing
                            FileEntryInfo.CheckOutWorkSpaceId = Nothing
                            FileEntryInfo.IsDeleted = False
                            FileEntryInfo.DeletedByUserId = Nothing
                            FileEntryInfo.DeletedOnUTC = Nothing
                            Service.FileEntries.Add(MapFromObject(FileEntryInfo))

                            Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                            fileVersionRepo.fncUser = Me.fncUser
                            FileEntryVersionsInfo.VersionNumber = 1
                            FileEntryVersionsInfo.IsDeleted = False
                            FileEntryVersionsInfo.DeletedByUserId = Nothing
                            FileEntryVersionsInfo.DeletedOnUTC = Nothing
                            FileEntryVersionsInfo.CreatedByUserId = userId
                            FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()

                            Service.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionsInfo))

                            Try
                                Dim fileShare = Service.FileShares.FirstOrDefault(Function(p) p.FileShareId = FileEntryInfo.FileShareId AndAlso p.IsDeleted = False)
                                If (fileShare IsNot Nothing) Then
                                    Dim fileDirectory = Util.Helper.CreateAzureFileDirectory(StorageConnectionString:=Me.GetStorageAccount, ShareName:=fileShare.ShareContainerName, FolderName:=FileEntryVersionsInfo.FileEntryRelativePath + "\")
                                    Service.SaveChanges()
                                    res.Data = True
                                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                    res.Status = Status.Success
                                Else
                                    Throw New Exception(message:=String.Format(format:="Unable to create folder '{0}' of share '{1}' ", arg0:=FileEntryVersionsInfo.FileEntryName.ToString(), arg1:=FileEntryInfo.FileShareId.ToString()))
                                End If
                            Catch ex As Exception
                                res.Data = False
                                res.Message = ex.Message
                                res.Status = Status.Error
                            End Try
                        Else
                            res.Data = False
                            res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                            res.Status = Status.AccessDenied
                        End If
                    End If
                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddFolderNoCreateNoPermissionCheck(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetServerEntity()
                If (IsFilePathValid(Service, FileEntryVersionsInfo.ParentFileEntryId)) Then

                    Dim resFun = Service.GetLatestFileVersionByPath(FileEntryVersionsInfo.FileEntryRelativePath, FileEntryInfo.FileShareId)
                    If (resFun IsNot Nothing And resFun.Count() > 0) Then
                        Dim folder = resFun.FirstOrDefault()

                        If (folder.FileEntryId = FileEntryInfo.FileEntryId AndAlso folder.VersionNumber = 1) Then
                            res.Data = True
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success

                        Else
                            res.Data = False
                            res.Status = Status.AlreadyExists
                            res.Message = "File/Folder/FileShare Already Exists"
                        End If
                    Else
                        FileEntryInfo.CurrentVersionNumber = 1
                        FileEntryInfo.IsCheckedOut = False
                        FileEntryInfo.CheckedOutOnUTC = Nothing
                        FileEntryInfo.CheckedOutByUserId = Nothing
                        FileEntryInfo.CheckOutWorkSpaceId = Nothing
                        FileEntryInfo.IsDeleted = False
                        FileEntryInfo.DeletedByUserId = Nothing
                        FileEntryInfo.DeletedOnUTC = Nothing
                        Service.FileEntries.Add(MapFromObject(FileEntryInfo))

                        Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                        fileVersionRepo.fncUser = Me.fncUser
                        FileEntryVersionsInfo.VersionNumber = 1
                        FileEntryVersionsInfo.IsDeleted = False
                        FileEntryVersionsInfo.DeletedByUserId = Nothing
                        FileEntryVersionsInfo.DeletedOnUTC = Nothing
                        FileEntryVersionsInfo.FileEntryId = FileEntryInfo.FileEntryId

                        Service.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionsInfo))

                        Service.SaveChanges()
                        res.Data = True
                        res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        res.Status = Status.Success

                    End If
                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddFileCreateNoPermissionCheck(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetServerEntity()
                If (IsFilePathValid(Service, FileEntryVersionsInfo.ParentFileEntryId)) Then

                    Dim resFun = Service.GetLatestFileVersionByPath(FileEntryVersionsInfo.FileEntryRelativePath, FileEntryInfo.FileShareId)
                    If (resFun IsNot Nothing And resFun.Count() > 0) Then
                        Dim file = resFun.FirstOrDefault()

                        If (file.FileEntryId = FileEntryInfo.FileEntryId AndAlso file.VersionNumber = 1) Then
                            res.Data = True
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success

                        Else
                            res.Data = False
                            res.Status = Status.AlreadyExists
                            res.Message = "File/Folder/FileShare Already Exists"
                        End If
                    Else
                        FileEntryInfo.CurrentVersionNumber = 1
                        FileEntryInfo.IsCheckedOut = False
                        FileEntryInfo.CheckedOutOnUTC = Nothing
                        FileEntryInfo.CheckedOutByUserId = Nothing
                        FileEntryInfo.CheckOutWorkSpaceId = Nothing
                        FileEntryInfo.IsDeleted = False
                        FileEntryInfo.DeletedByUserId = Nothing
                        FileEntryInfo.DeletedOnUTC = Nothing
                        Service.FileEntries.Add(MapFromObject(FileEntryInfo))

                        Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                        fileVersionRepo.fncUser = Me.fncUser
                        FileEntryVersionsInfo.VersionNumber = 1
                        FileEntryVersionsInfo.IsDeleted = False
                        FileEntryVersionsInfo.DeletedByUserId = Nothing
                        FileEntryVersionsInfo.DeletedOnUTC = Nothing
                        FileEntryVersionsInfo.FileEntryId = FileEntryInfo.FileEntryId

                        Service.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionsInfo))

                        CopyFromFileToBlob(FileEntryVersionsInfo)

                        Service.SaveChanges()

                        res.Data = True
                        res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        res.Status = Status.Success

                    End If
                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddAndDelete(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status) Implements IFileRepository.AddAndDelete
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetServerEntity()
                Dim resFun = Service.GetLatestFileVersionByPath(FileEntryVersionsInfo.FileEntryRelativePath, FileEntryInfo.FileShareId)
                If (resFun IsNot Nothing And resFun.Count() > 0) Then
                    res.Data = False
                    res.Status = Status.AlreadyExists
                    res.Message = "File/Folder/FileShare Already Exists"
                Else

                    FileEntryInfo.CurrentVersionNumber = 1
                    FileEntryInfo.IsCheckedOut = False
                    FileEntryInfo.CheckedOutOnUTC = Nothing
                    FileEntryInfo.CheckedOutByUserId = Nothing
                    FileEntryInfo.CheckOutWorkSpaceId = Nothing
                    FileEntryInfo.IsDeleted = True
                    FileEntryInfo.DeletedByUserId = Me.User.UserId
                    FileEntryInfo.DeletedOnUTC = Date.UtcNow()
                    Service.FileEntries.Add(MapFromObject(FileEntryInfo))

                    Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                    fileVersionRepo.fncUser = Me.fncUser
                    FileEntryVersionsInfo.VersionNumber = 1
                    FileEntryVersionsInfo.IsDeleted = False
                    FileEntryVersionsInfo.DeletedByUserId = Nothing
                    FileEntryVersionsInfo.DeletedOnUTC = Nothing
                    FileEntryVersionsInfo.CreatedByUserId = userId
                    FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()

                    Service.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionsInfo))

                    Service.SaveChanges()

                    res.Data = True
                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                    res.Status = Status.Success
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function AddVersionWithoutUpload(FileEntryInfo As FileEntryInfo, FileEntryVersionInfo As Model.FileVersionInfo, links As List(Of FileEntryLinkInfo)) As ResultInfo(Of Integer, Status) Implements IFileRepository.AddVersionWithoutUpload
        Try
            Dim res As ResultInfo(Of Integer, Status) = New ResultInfo(Of Integer, Status)
            Dim userId As Guid = Me.User.UserId
            res.Data = -1
            Dim versionNumber As Integer = 1

            Using Service = GetServerEntity()
                If (IsFilePathValid(Service, FileEntryVersionInfo.ParentFileEntryId)) Then

                    Dim obj = Service.GetLatestFileVersionByPath(FileEntryVersionInfo.FileEntryRelativePath, FileEntryInfo.FileShareId).FirstOrDefault()

                    If (obj Is Nothing) Then

                        If (HasWritePermission(Service, Me.User, FileEntryVersionInfo.ParentFileEntryId)) Then

                            FileEntryInfo.CurrentVersionNumber = versionNumber
                            FileEntryInfo.IsCheckedOut = False
                            FileEntryInfo.CheckedOutOnUTC = Nothing
                            FileEntryInfo.CheckedOutByUserId = Nothing
                            FileEntryInfo.CheckOutWorkSpaceId = Nothing
                            FileEntryInfo.IsDeleted = False
                            FileEntryInfo.DeletedByUserId = Nothing
                            FileEntryInfo.DeletedOnUTC = Nothing
                            FileEntryInfo.IsPermanentlyDeleted = False
                            FileEntryInfo.PermanentlyDeletedByUserId = Nothing
                            FileEntryInfo.PermanentlyDeletedOnUTC = Nothing
                            Dim FileEntry = MapFromObject(FileEntryInfo)

                            Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository()
                            fileVersionRepo.fncUser = Me.fncUser
                            FileEntryVersionInfo.VersionNumber = versionNumber
                            FileEntryVersionInfo.IsDeleted = False
                            FileEntryVersionInfo.DeletedByUserId = Nothing
                            FileEntryVersionInfo.DeletedOnUTC = Nothing
                            FileEntryVersionInfo.CreatedByUserId = userId
                            FileEntryVersionInfo.CreatedOnUTC = Date.UtcNow()
                            FileEntry.FileVersions.Add(fileVersionRepo.MapFromObject(FileEntryVersionInfo))
                            Service.FileEntries.Add(FileEntry)

                            'Create Links and Check that all Linking files are deleted
                            For Each link As FileEntryLinkInfo In links

                                Dim linkfile = Service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = link.PreviousFileEntryId AndAlso p.IsDeleted = True)
                                If (linkfile IsNot Nothing) Then

                                    Dim dbLink = Service.FileEntryLinks.FirstOrDefault(Function(p) p.FileEntryLinkId = link.FileEntryLinkId)

                                    If (dbLink Is Nothing) Then
                                        dbLink = New FileEntryLink With
                                                 {
                                                    .FileEntryLinkId = link.FileEntryLinkId,
                                                    .FileEntryId = link.FileEntryId,
                                                    .PreviousFileEntryId = link.PreviousFileEntryId,
                                                    .CreatedByUserId = userId,
                                                    .CreatedOnUTC = Date.UtcNow()
                                                }
                                        Service.FileEntryLinks.Add(dbLink)
                                    ElseIf (dbLink.IsDeleted) Then
                                        res.Message = "One of the Links is deleted"
                                        res.Status = Status.LinkDeleted
                                        Return res
                                    End If

                                Else
                                    res.Message = "Linked File is not deleted"
                                    res.Status = Status.LinkedFileNotDeleted
                                    Return res
                                End If

                            Next

                            Service.SaveChanges()

                            res.Data = versionNumber
                            res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            res.Status = Status.Success

                        Else
                            res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                            res.Status = Status.AccessDenied
                        End If
                    Else
                        res.Status = Status.AlreadyExists
                        res.Message = "File/Folder/FileShare Already Exists"
                    End If
                Else
                    res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                    res.Status = Status.PathNotValid
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try
    End Function

    Public Overrides Function GetAll() As ResultInfo(Of List(Of FileEntryInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim result = service.FileEntries.Select(Function(s) New FileEntryInfo With {
                .FileEntryId = s.FileEntryId,
                .FileEntryTypeId = s.FileEntryTypeId,
                .CurrentVersionNumber = s.CurrentVersionNumber,
                .IsCheckedOut = s.IsCheckedOut
             }).ToList()
                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
        End Try
    End Function

    Public Overrides Function [Get](id As Guid) As ResultInfo(Of FileEntryInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = id)
                Dim obj = New FileEntryInfo
                If file IsNot Nothing Then
                    obj = MapToObject(file)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of FileEntryInfo)(ex)
        End Try
    End Function

    Public Overrides Function Add(data As FileEntryInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
                Dim file = MapFromObject(data)
                service.FileEntries.Add(file)
                service.SaveChanges()
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Overrides Function Update(id As Guid, data As FileEntryInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = id)
                If file IsNot Nothing Then
                    MapFromObject(data, file)
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Overrides Function Delete(id As Guid) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = id)
                If file IsNot Nothing Then
                    file.IsDeleted = True
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Private Function HasWritePermission(Service As FileMinisterEntities, user As RSCallerInfo, FileEntryId As Guid) As Boolean
        Dim permission = GetEffectivePermission(Service, user, FileEntryId)

        If ((PermissionType.Write And permission) = PermissionType.Write) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function IsFilePathValid(Service As FileMinisterEntities, FileEntryId As Guid) As Boolean

        Dim sql As String = "SELECT [dbo].[IsFileEntryPathValid] ({0})"
        Dim param = {FileEntryId.ToString()}

        Return Service.Database.SqlQuery(Of Boolean)(sql, param).FirstOrDefault()

    End Function

    Private Function ReplaceFirst(str As String, oldValue As String, newValue As String)
        Dim pos = str.IndexOf(oldValue)

        If (pos < 0) Then
            Return str
        End If

        Return (str.Substring(0, pos) + newValue + str.Substring(pos + oldValue.Length))
    End Function

    Public Shared Function GetEffectivePermission(service As FileMinisterEntities, user As RSCallerInfo, FileEntryId As Guid) As Byte
        Dim sql As String = "SELECT [dbo].[GetEffectiveUserPermissionsOnFileEntry] ({0}, {1})"
        Dim param = {user.UserId, FileEntryId.ToString()}

        Return service.Database.SqlQuery(Of Byte)(sql, param).FirstOrDefault()
    End Function

    Public Function GetEffectivePermission(user As RSCallerInfo, FileEntryId As Guid) As Byte Implements IFileRepository.GetEffectivePermission
        Using Service = GetServerEntity()
            Return GetEffectivePermission(Service, user, FileEntryId)
        End Using
    End Function

    Public Function [GetAllUserShares]() As ResultInfo(Of List(Of GetAllSharesForUser_Result), Status) Implements IFileRepository.GetAllUserShares
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId
                Dim result = service.GetAllSharesForUser(userId)
                Dim data = result.ToList()
                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of GetAllSharesForUser_Result))(ex)
        End Try
    End Function

    Public Function [GetAllChildren](id As Guid, eType As Integer) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.GetAllChildren
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId
                If (service.FileEntries.Any(Function(p) p.FileEntryId = id AndAlso p.IsDeleted)) Then
                    Return BuildResponse(New List(Of FileEntryInfo), Status.NotFound, "Folder Not Found")
                End If

                Dim result = service.GetLatestFileVersionChildrenWithPermission(id, Me.User.UserId).OrderBy(Function(p) p.FileEntryTypeId).ThenBy(Function(p) p.Name)
                If eType = 2 Then
                    result = result.Where(Function(x) x.FileEntryTypeId = eType)
                End If


                Dim data = MapToObject(result.ToList())
                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
        End Try
    End Function

    Public Function [GetAllChildrenHierarchy](id As Guid) As ResultInfo(Of List(Of FileEntryInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId
                If (service.FileEntries.Any(Function(p) p.FileEntryId = id AndAlso p.IsDeleted)) Then
                    Return BuildResponse(New List(Of FileEntryInfo), Status.NotFound, "Folder Not Found")
                End If

                Dim undelChildren = service.GetLatestFileVersionChildrenHierarchy(id)

                Dim files = (From FE As FileEntry In service.FileEntries
                             Join a In undelChildren
                     On a.FileEntryId Equals FE.FileEntryId
                             Join FV As FileVersion In service.FileVersions
                     On FV.FileEntryId Equals FE.FileEntryId
                             Where (FE.IsPermanentlyDeleted = 0 And FV.VersionNumber = FE.CurrentVersionNumber)
                             Select FE, FV.FileEntryRelativePath, a
            )
                Dim list As New List(Of FileEntryInfo)
                For Each s In files.ToList()
                    Dim t = New FileEntryInfo With {
                           .FileEntryId = s.FE.FileEntryId,
                           .FileEntryTypeId = s.FE.FileEntryTypeId,
                           .CurrentVersionNumber = s.a.VersionNumber,
                           .CheckedOutByUserId = s.FE.CheckedOutByUserId,
                           .CheckOutWorkSpaceId = s.FE.CheckedOutWorkSpaceId,
                           .FileShareId = s.FE.FileShareId,
                           .IsDeleted = s.FE.IsDeleted,
                           .IsCheckedOut = s.FE.IsCheckedOut,
                           .FileVersion = New Model.FileVersionInfo With {
                            .FileEntryName = s.a.FileSystemEntryName,
                            .FileEntryExtension = s.a.FileSystemEntryExtension,
                            .FileEntryNameWithExtension = s.a.FileSystemEntryNameWithExtension,
                            .FileEntryRelativePath = s.FileEntryRelativePath,
                            .CreatedByUserId = s.a.CreatedByUserId,
                            .CreatedOnUTC = s.a.CreatedOnUTC,
                            .ParentFileEntryId = s.a.ParentFileEntryId,
                            .FileVersionId = s.a.FileVersionId,
                            .FileEntrySize = s.a.FileSystemEntrySize
            }
                   }
                    list.Add(t)
                Next


                Return BuildResponse(list)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
        End Try
    End Function

    'todo: check this method as path already have file name
    Public Function GetLatestFileEntryVersionByPath(id As Guid, fileName As String) As ResultInfo(Of FileEntryInfo, Status) Implements IFileRepository.GetLatestFileEntryVersionByPath
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId
                Dim folder = [Get](id)
                Dim FileShareId As Integer = folder.Data.FileShareId
                Dim relativePath As Integer = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = id).FileEntryRelativePath
                relativePath = System.IO.Path.Combine(relativePath, fileName)
                Dim result = service.GetLatestFileVersionByPath(relativePath, FileShareId).FirstOrDefault()

                Dim data = New FileEntryInfo With {
                        .FileEntryId = result.FileEntryId,
                        .FileEntryTypeId = result.FileEntryTypeId,
                        .CurrentVersionNumber = result.VersionNumber,
                        .IsCheckedOut = result.IsCheckedOut
                    }

                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of FileEntryInfo)(ex)
        End Try
    End Function

    Public Function GetLatestFileEntryVersionByPath(relativePath As String, shareId As Short) As ResultInfo(Of FileEntryInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim result = service.GetLatestFileVersionByPath(relativePath, shareId).FirstOrDefault()

                Dim data = New FileEntryInfo With {
                        .FileEntryId = result.FileEntryId,
                        .FileEntryTypeId = result.FileEntryTypeId,
                        .CurrentVersionNumber = result.VersionNumber,
                        .IsCheckedOut = result.IsCheckedOut
                    }

                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of FileEntryInfo)(ex)
        End Try
    End Function

    Public Function CheckInWeb(FileEntryId As Guid, localFileVersionNumber As Integer, fileSize As Long, fileHash As String, blobName As Guid) As ResultInfo(Of Integer, Status) Implements IFileRepository.CheckInWeb
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId
                Dim s = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId And p.VersionNumber = localFileVersionNumber)

                Dim FileEntryVersionsInfo = New Model.FileVersionInfo With {
                                                       .FileVersionId = blobName,
                                                       .ParentFileEntryId = s.ParentFileEntryId,
                                                       .FileEntryId = s.FileEntryId,
                                                       .VersionNumber = s.VersionNumber,
                                                       .PreviousFileVersionId = s.FileVersionId,
                                                       .FileEntrySize = fileSize,
                                                       .FileEntryHash = fileHash,
                                                       .FileEntryName = s.FileEntryName,
                                                       .FileEntryRelativePath = s.FileEntryRelativePath,
                                                       .FileEntryExtension = s.FileEntryExtension,
                                                       .FileEntry = New FileEntryInfo With {.FileShareId = s.FileEntry.FileShareId},
                                                       .ServerFileName = blobName,
                                                       .CheckInSource = F_Enums.CheckInSource.Web
                                                    }

                Dim outResult = CopyFromBlobToFile(FileEntryVersionsInfo:=FileEntryVersionsInfo)
                If (outResult.Status = Status.Error) Then
                    Return New ResultInfo(Of Integer, Status) With {.Data = outResult.Data, .Message = outResult.Message, .Status = outResult.Status}
                End If

                Return CheckIn(FileEntryVersionsInfo, localFileVersionNumber)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try

    End Function

    Public Function AddFolderWeb(parentFolderId As Guid, folderName As String) As ResultInfo(Of Boolean, Status) Implements IFileRepository.AddFolderWeb
        Dim FileEntryInfo As New FileEntryInfo()
        FileEntryInfo.FileEntryId = Guid.NewGuid()
        FileEntryInfo.FileEntryTypeId = FileType.Folder

        Dim FileEntryVersionsInfo As New Model.FileVersionInfo()
        FileEntryVersionsInfo.FileVersionId = Guid.NewGuid()

        Using Service = GetServerEntity()
            Dim ParentFileEntry = Service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = parentFolderId)
            Dim ParentVersion = Service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = parentFolderId AndAlso p.VersionNumber = ParentFileEntry.CurrentVersionNumber)

            FileEntryInfo.FileShareId = ParentFileEntry.FileShareId

            FileEntryVersionsInfo.CheckInSource = F_Enums.CheckInSource.Web

            FileEntryVersionsInfo.FileEntryId = FileEntryInfo.FileEntryId
            FileEntryVersionsInfo.ParentFileEntryId = parentFolderId
            FileEntryVersionsInfo.FileEntryName = folderName
            If (ParentVersion Is Nothing) Then
                FileEntryVersionsInfo.FileEntryRelativePath = folderName
            Else
                FileEntryVersionsInfo.FileEntryRelativePath = System.IO.Path.Combine(ParentVersion.FileEntryRelativePath, folderName)
            End If


        End Using

        Return AddFolder(FileEntryInfo, FileEntryVersionsInfo)
    End Function

    Public Function [FileSearch](search As FileSearch) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.FileSearch
        Try
            Using service = GetServerEntity()
                Dim ParamStartFileId = New System.Data.SqlClient.SqlParameter("@StartFileId", SqlDbType.UniqueIdentifier)
                ParamStartFileId.Value = search.StartFileId

                Dim ParamFileName = New System.Data.SqlClient.SqlParameter("@FileName", SqlDbType.NVarChar)
                ParamFileName.Value = search.SearchText

                Dim ParamIsAdvancedSearch = New System.Data.SqlClient.SqlParameter("@IsAdvancedSearch", SqlDbType.Bit)
                ParamIsAdvancedSearch.Value = search.IsAdvancedSearch

                'Dim TagCount = search.Tags.Count()
                Dim searchResult = New List(Of FileEntryInfo)

                Dim result
                If search.Tags IsNot Nothing AndAlso search.Tags.Count > 0 Then
                    Dim Taglist = New DataTable()
                    Taglist.Columns.Add("TagName", GetType(String))
                    Taglist.Columns.Add("TagValue", GetType(String))
                    For Each Tag In search.Tags
                        Taglist.Rows.Add(Tag.TagName, Tag.TagValue)
                    Next
                    Dim ParamTaglist = New System.Data.SqlClient.SqlParameter("@TagList", SqlDbType.Structured)
                    ParamTaglist.Value = Taglist
                    ParamTaglist.TypeName = "dbo.TagList"
                    result = service.Database.SqlQuery(Of Model.FileSearchResult)("exec dbo.usp_SEL_FileSearch @Taglist,@StartFileId,@FileName,@IsAdvancedSearch", ParamTaglist, ParamStartFileId, ParamFileName, ParamIsAdvancedSearch)
                Else
                    result = service.Database.SqlQuery(Of Model.FileSearchResult)("exec dbo.usp_SEL_FileSearch @StartFileId = @StartFileId, @FileName=@FileName,@IsAdvancedSearch= @IsAdvancedSearch", ParamStartFileId, ParamFileName, ParamIsAdvancedSearch)
                End If

                result = result.ToList().OrderBy(Function(p) p.FileEntryTypeId).ThenBy(Function(p) p.FileEntryName)

                For Each file In result
                    Dim permssion = Util.Helper.GetEffectivePermission(file.FileEntryId, Me.User)
                    If Not (User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (permssion And PermissionType.Read) = PermissionType.Read) Then
                        Continue For
                    End If

                    Dim f As New FileEntryInfo()
                    With f
                        .FileEntryId = file.FileEntryId
                        .FileEntryTypeId = file.FileEntryTypeId
                        .FileShareId = search.FileShareId
                        .CheckedOutByUserId = file.CheckedOutByUserId
                        .CurrentVersionNumber = file.VersionNumber

                        .FileVersion = New Model.FileVersionInfo()
                        .FileVersion.DeltaOperation = New DeltaOperationInfo()
                        .FileVersion.DeltaOperation.FileEntryStatusId = file.FileEntryStatusId
                        .FileVersion.DeltaOperation.IsConflicted = file.IsConflicted
                        .FileVersion.FileEntryStatusDisplayName = file.FileEntryStatusDisplayName
                        .FileVersion.FileVersionId = file.FileVersionId
                        .FileVersion.FileEntryName = file.FileEntryName
                        .FileVersion.FileEntryExtension = file.FileEntryExtension
                        .FileVersion.FileEntryNameWithExtension = file.FileEntryNameWithExtension
                        .FileVersion.ServerFileName = file.ServerFileName
                        .FileVersion.FileEntrySize = file.FileEntrySize
                        .FileVersion.ParentFileEntryId = file.ParentFileEntryId
                        .FileVersion.PreviousFileVersionId = file.PreviousFileVersionId

                        .CanWrite = (User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (permssion And PermissionType.Write) = PermissionType.Write)

                        'computed relative path
                        .FileVersion.FileEntryRelativePath = search.SharePath + file.FileEntryRelativePath

                        .FileVersion.VersionNumber = file.VersionNumber
                        .FileVersion.PreviousVersionNumber = file.PrevVersionNumber

                        .FileVersion.CreatedByUserId = file.CreatedByUserId
                        .FileVersion.CreatedOnUTC = file.CreatedOnUTC
                    End With
                    searchResult.Add(f)
                Next
                Return BuildResponse(searchResult)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
        End Try
    End Function

    Public Function RequestConflictFileUpload(workSpaceId As Guid, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.RequestConflictFileUpload
        Try
            Using service = GetServerEntity()
                Dim conflict = service.FileVersionConflicts.FirstOrDefault(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                If (conflict IsNot Nothing AndAlso Not conflict.IsResolved) Then
                    If ((conflict.UserId = Me.User.UserId AndAlso conflict.WorkSpaceId = workSpaceId) OrElse (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (GetEffectivePermission(service, Me.User, FileEntryId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin)) Then
                        Dim conflictRequest = service.FileVersionConflictRequests.FirstOrDefault(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                        If (conflictRequest Is Nothing) Then
                            conflictRequest = New FileVersionConflictRequest
                            With conflictRequest
                                .FileVersionConflictId = FileVersionConflictId
                                .FileVersionConflictRequestStatusId = ConflictUploadStatus.Requested
                                .RequestedBy = Me.User.UserId
                                .RequestedAtUTC = DateTime.UtcNow
                            End With
                            service.FileVersionConflictRequests.Add(conflictRequest)
                            service.SaveChanges()
                        End If
                        Return BuildResponse(True)
                    Else
                        Return BuildResponse(False, Status.AccessDenied, "You do not have permission to do this")
                    End If
                Else
                    Return BuildResponse(False, Status.None, "Conflict already resolved")
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function GetConflictFilePendingUpload(workSpaceID As Guid) As ResultInfo(Of List(Of Guid), Status) Implements IFileRepository.GetConflictFilePendingUpload
        Try
            Try
                Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

                If retResult.Data <> Guid.Empty Then
                    workSpaceID = retResult.Data
                End If
            Catch
            End Try

            Using service = GetServerEntity()
                Return BuildResponse(service.FileVersionConflicts.Where(Function(p) Not p.IsResolved AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = workSpaceID AndAlso p.FileVersionConflictRequest.FileVersionConflictRequestStatusId = ConflictUploadStatus.Requested).Select(Function(p) p.FileVersionConflictId).ToList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of Guid))(ex)
        End Try
    End Function

    Public Function GetOtherUsersUnresolvedConflicts(FileEntryId As Guid) As ResultInfo(Of List(Of FileVersionConflictInfo), Status) Implements IFileRepository.GetOtherUsersUnresolvedConflicts
        Try
            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (GetEffectivePermission(service, Me.User, FileEntryId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim q = From fsec In service.FileVersionConflicts.Where(Function(p) Not p.IsResolved AndAlso p.FileEntryId = FileEntryId AndAlso p.UserId <> Me.User.UserId)
                            Group Join u In service.Users On fsec.UserId Equals u.UserId Into Group
                            From u In Group.DefaultIfEmpty()
                            Select New With {.FileVersionConflict = fsec, .User = u}

                    'Dim FileVersionConflicts = service.FileVersionConflicts.Where(Function(p) Not p.IsResolved AndAlso p.UserId <> Me.User.Id)
                    Dim FileVersionConflictsInfo As New List(Of FileVersionConflictInfo)
                    For Each d In q
                        Dim FileVersionConflict = d.FileVersionConflict

                        Dim FileVersionConflictInfo As New FileVersionConflictInfo
                        With FileVersionConflictInfo
                            .CreatedOnUTC = FileVersionConflict.CreatedOnUTC
                            .FileEntryId = FileVersionConflict.FileEntryId
                            .FileEntryNameAndExtension = FileVersionConflict.FileEntryNameAndExtension
                            .FileEntryPath = FileVersionConflict.FileEntryPath
                            .FileVersionConflictId = FileVersionConflict.FileVersionConflictId
                            .FileVersionConflictTypeId = FileVersionConflict.FileVersionConflictTypeId
                            .FileVersionId = FileVersionConflict.FileVersionId
                            .IsResolved = FileVersionConflict.IsResolved
                            .UserId = FileVersionConflict.UserId
                            .WorkSpaceId = FileVersionConflict.WorkSpaceId
                            .VersionNumber = FileVersionConflict.FileVersion.VersionNumber
                            If (d.User IsNot Nothing) Then
                                .UserName = d.User.UserName
                            End If
                            If (FileVersionConflict.FileVersionConflictRequest IsNot Nothing) Then
                                .FileEntryHash = FileVersionConflict.FileVersionConflictRequest.FileEntryHash
                                .FileEntrySize = FileVersionConflict.FileVersionConflictRequest.FileEntrySize
                                .FileVersionConflictRequestStatusId = FileVersionConflict.FileVersionConflictRequest.FileVersionConflictRequestStatusId


                                Dim m_ConflictUploadStatus As ConflictUploadStatus = CType(FileVersionConflictInfo.FileVersionConflictRequestStatusId, ConflictUploadStatus)
                                FileVersionConflictInfo.FileVersionConflictRequestStatus = m_ConflictUploadStatus.ToString()


                            End If

                            Dim m_FileVersionConflictType As F_Enums.FileVersionConflictType = CType(FileVersionConflictInfo.FileVersionConflictTypeId, F_Enums.FileVersionConflictType)
                            FileVersionConflictInfo.FileVersionConflictType = m_FileVersionConflictType.ToString()


                        End With

                        FileVersionConflictsInfo.Add(FileVersionConflictInfo)
                    Next
                    Return BuildResponse(FileVersionConflictsInfo, Status.Success, Nothing)
                Else
                    Return BuildResponse(New List(Of FileVersionConflictInfo), Status.AccessDenied, "You do not have permission to do this")
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileVersionConflictInfo))(ex)
        End Try
    End Function

    Public Function UpdateConflictUploadStatus(FileVersionConflictId As Guid, fileSize As Long, fileHash As String) As ResultInfo(Of Boolean, Status) Implements IFileRepository.UpdateConflictUploadStatus
        Try
            Dim msg As String = String.Empty
            Using service = GetServerEntity()
                Dim conflictStatus = service.FileVersionConflictRequests.FirstOrDefault(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                If (conflictStatus IsNot Nothing AndAlso conflictStatus.FileVersionConflictRequestStatusId <> ConflictUploadStatus.Uploaded) Then
                    conflictStatus.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded
                    conflictStatus.FileEntrySize = fileSize
                    conflictStatus.FileEntryHash = fileHash
                    service.SaveChanges()

                    Dim file = (From c As FileVersionConflict In service.FileVersionConflicts.Where(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                                Join f As FileEntry In service.FileEntries On c.FileEntryId Equals f.FileEntryId
                                Select f).FirstOrDefault()

                    If file IsNot Nothing Then
                        Try
                            'Update FileShare blobsize field               
                            Dim ConfigRepo = New ConfigRepository()
                            ConfigRepo.fncUser = Me.fncUser
                            Dim result = ConfigRepo.UpdateShareBlobSize(file.FileShareId)

                            If (Not result.Data) Then
                                msg = String.Format("Status Updated sucessfully. Failed to update FileShare BlobSize in database with message: {0}", result.Message)
                            Else
                                msg = ServiceConstants.OPERATION_SUCCESSFUL
                            End If
                        Catch ex As Exception
                            msg = String.Format("Status Updated sucessfully. Failed to update FileShare BlobSize in database with Error: {0}", ex.Message)
                        End Try
                    End If
                End If
                Return BuildResponse(True, Status.Success, msg)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function ResolveOthersConflictUsingOthers(FileShareId As Integer, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.ResolveOthersConflictUsingOthers
        Try
            Dim retResult = Util.Helper.GetWebWorkSpaceId(Me.User)

            If retResult.Data = Guid.Empty Then
                Return BuildResponse(Of Boolean)(False, Status.None, "Server WorkSpaceId not found")
            End If
            Dim workSpaceId As Guid = retResult.Data

            Using service = GetServerEntity()
                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (GetEffectivePermission(service, Me.User, FileEntryId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim conflict As FileVersionConflict = service.FileVersionConflicts.FirstOrDefault(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                    If (conflict IsNot Nothing AndAlso Not conflict.IsResolved) Then
                        Dim FileEntry = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId)
                        If (FileEntry Is Nothing) Then
                            Return BuildResponse(False, Status.Error, "Invalid File Entry")
                        End If

                        Select Case conflict.FileVersionConflictTypeId
                            Case F_Enums.FileVersionConflictType.Modified, F_Enums.FileVersionConflictType.ClientModifyServerRename, F_Enums.FileVersionConflictType.ServerDelete, F_Enums.FileVersionConflictType.InTheWay
                                If (conflict.FileVersionConflictRequest IsNot Nothing AndAlso conflict.FileVersionConflictRequest.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded) Then
                                    Dim localFileVersionNumber = FileEntry.CurrentVersionNumber
                                    Dim result = CheckOut(FileEntryId, localFileVersionNumber, workSpaceId)
                                    If (result.Data) Then
                                        Dim result1 = CheckInWeb(FileEntryId, localFileVersionNumber, conflict.FileVersionConflictRequest.FileEntrySize, conflict.FileVersionConflictRequest.FileEntryHash, FileVersionConflictId)
                                        If (result1.Status <> Status.Success) Then
                                            Return BuildResponse(False, result1.Status, result1.Message)
                                        End If
                                        If (conflict.FileVersionConflictTypeId = F_Enums.FileVersionConflictType.ServerDelete) Then
                                            FileEntry.DeletedByUserId = Nothing
                                            FileEntry.DeletedOnUTC = Nothing
                                            FileEntry.IsDeleted = False
                                            service.FileEntryLinks.RemoveRange(service.FileEntryLinks.Where(Function(p) p.PreviousFileEntryId = FileEntryId))
                                        End If
                                    Else
                                        Return result
                                    End If
                                Else
                                    Return BuildResponse(False, Status.NotFound, "Client Version not present on the server yet")
                                End If
                            Case F_Enums.FileVersionConflictType.ClientDelete
                                FileEntry.DeletedByUserId = Me.User.UserId
                                FileEntry.DeletedOnUTC = DateTime.UtcNow
                                FileEntry.IsDeleted = True
                        End Select

                        conflict.IsResolved = True
                        conflict.ResolvedByUserId = Me.User.UserId
                        conflict.ResolvedOnUTC = DateTime.UtcNow
                        conflict.ResolvedType = Enums.Constants.CLIENT_WIN_RESOLVETYPE
                        service.SaveChanges()
                    Else

                    End If

                Else
                    Return BuildResponse(False, Status.AccessDenied, "You do not have permission to do this")
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function ResolveOthersConflictUsingServer(FileShareId As Integer, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.ResolveOthersConflictUsingServer
        Try
            Using service = GetServerEntity()

                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (GetEffectivePermission(service, Me.User, FileEntryId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
                    Dim conflict = service.FileVersionConflicts.FirstOrDefault(Function(p) p.FileVersionConflictId = FileVersionConflictId)
                    If (conflict IsNot Nothing AndAlso Not conflict.IsResolved) Then
                        Select Case conflict.FileVersionConflictTypeId
                            Case F_Enums.FileVersionConflictType.Modified, F_Enums.FileVersionConflictType.ClientModifyServerRename, F_Enums.FileVersionConflictType.ServerDelete, F_Enums.FileVersionConflictType.InTheWay
                                'delete file of id FileVersionConflictId from azure
                                If (conflict.FileVersionConflictRequest IsNot Nothing AndAlso conflict.FileVersionConflictRequest.FileVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded) Then
                                    Dim lstServerFileName As New List(Of Guid)
                                    lstServerFileName.Add(FileVersionConflictId)
                                    Dim deleteBlobResult As ResultInfo(Of Boolean, Status) = Util.Helper.DeleteBlobs(FileShareId, lstServerFileName, Me.User)
                                End If
                            Case F_Enums.FileVersionConflictType.ClientDelete
                                service.FileEntryLinks.RemoveRange(service.FileEntryLinks.Where(Function(p) p.PreviousFileEntryId = FileEntryId))
                        End Select
                        conflict.IsResolved = True
                        conflict.ResolvedByUserId = Me.User.UserId
                        conflict.ResolvedOnUTC = DateTime.UtcNow
                        conflict.ResolvedType = Enums.Constants.SERVER_WIN_RESOLVETYPE
                        conflict.IsActionTaken = False
                        service.SaveChanges()
                    Else

                    End If

                Else
                    Return BuildResponse(False, Status.AccessDenied, "You do not have permission to do this")
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Check whether User is an admin or not
    ''' </summary>
    ''' <param name="service"></param>
    ''' <param name="userInfo"></param>
    ''' <param name="FileEntryId"></param>
    ''' <returns></returns>
    Private Function IsUserAdmin(service As FileMinisterEntities, userInfo As RSCallerInfo, FileEntryId As Guid) As Boolean
        If (userInfo.UserAccount.UserTypeId = Role.AccountAdmin OrElse (GetEffectivePermission(service, userInfo, FileEntryId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Move File from Source to Destination folder
    ''' </summary>
    ''' <param name="FileEntryId">Id of the Source File</param>
    ''' <param name="DestinationFileEntryId">Id of the destination folder</param>
    ''' <param name="IsReplaceExistingFile">True, if the user wants to replace existing file</param>
    ''' <param name="NewFileName">new name of the source file</param>
    ''' <returns></returns>
    Public Function MoveFile(FileEntryId As Guid, DestinationFileEntryId As Guid, IsReplaceExistingFile As Boolean, NewFileName As String) As ResultInfo(Of Boolean, Status) Implements IFileRepository.MoveFile
        Try
            Dim userId As Guid = Me.User.UserId
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using service = GetServerEntity()

                Dim fileEntryInfo As FileEntry = service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = FileEntryId)
                If (fileEntryInfo IsNot Nothing) Then

                    If (fileEntryInfo.IsCheckedOut = False OrElse (fileEntryInfo.IsCheckedOut = True AndAlso fileEntryInfo.CheckedOutByUserId = userId)) Then

                        If Not HasWritePermission(service, Me.User, FileEntryId) Then
                            res.Data = False
                            res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                            res.Status = Status.AccessDenied
                            Return BuildResponse(res.Data, res.Status, res.Message)
                        End If

                        'check destination folder exists or not
                        If (IsFilePathValid(service, DestinationFileEntryId)) Then

                            Dim objDestVersion = Nothing
                            Dim relativePath As String = String.Empty
                            Dim objCurrentfileVersion = fileEntryInfo.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = FileEntryId And p.VersionNumber = fileEntryInfo.CurrentVersionNumber)
                            Dim destObjectInfo = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = DestinationFileEntryId)

                            If (destObjectInfo.FileEntryTypeId = FileType.Share) Then
                                relativePath = IIf(String.IsNullOrWhiteSpace(NewFileName), objCurrentfileVersion.FileEntryNameWithExtension, NewFileName)
                            Else
                                objDestVersion = service.GetLatestFileVersion(DestinationFileEntryId).FirstOrDefault()
                                relativePath = System.IO.Path.Combine(objDestVersion.FileEntryRelativePath, IIf(String.IsNullOrWhiteSpace(NewFileName), objCurrentfileVersion.FileEntryNameWithExtension, NewFileName))
                            End If

                            'Check file is already exists or not
                            Dim obj = service.GetLatestFileVersionByPath(relativePath, fileEntryInfo.FileShareId).FirstOrDefault()
                            If (obj IsNot Nothing) AndAlso IsReplaceExistingFile = False Then
                                res.Data = False
                                res.Status = Status.AlreadyExists
                                res.Message = "File Already Exists"
                            Else
                                If (HasWritePermission(service, Me.User, DestinationFileEntryId)) Then

                                    Dim fileVersionRepo As FileVersionRepository = New FileVersionRepository() With {.fncUser = Me.fncUser}

                                    Dim currentNumber As Integer = fileEntryInfo.CurrentVersionNumber
                                    Dim versionNumber As Integer = System.Threading.Interlocked.Increment(currentNumber)
                                    Dim fileEntryName As String = IIf(String.IsNullOrWhiteSpace(NewFileName), objCurrentfileVersion.FileEntryName, System.IO.Path.GetFileNameWithoutExtension(NewFileName))
                                    Dim fileEntryExt As String = IIf(String.IsNullOrWhiteSpace(NewFileName), objCurrentfileVersion.FileEntryExtension, System.IO.Path.GetExtension(NewFileName))

                                    Dim fileEntryVersionsInfo As Model.FileVersionInfo = New Model.FileVersionInfo() With
                                        {
                                            .VersionNumber = versionNumber,
                                            .IsDeleted = False,
                                            .DeletedByUserId = Nothing,
                                            .DeletedOnUTC = Nothing,
                                            .CreatedByUserId = userId,
                                            .CreatedOnUTC = Date.UtcNow(),
                                            .ParentFileEntryId = DestinationFileEntryId,
                                            .FileEntryId = FileEntryId,
                                            .FileEntryRelativePath = relativePath,
                                            .FileEntryName = fileEntryName,
                                            .FileEntrySize = objCurrentfileVersion.FileEntrySize,
                                            .PreviousFileVersionId = objCurrentfileVersion.FileVersionId,
                                            .FileEntryExtension = fileEntryExt,
                                            .FileEntryHash = objCurrentfileVersion.FileEntryHash,
                                            .FileEntry = New FileEntryInfo With {.FileShareId = fileEntryInfo.FileShareId},
                                            .ServerFileName = objCurrentfileVersion.ServerFileName,
                                            .CheckInSource = F_Enums.CheckInSource.Web
                                         }

                                    'fileEntryInfo.CurrentVersionNumber = versionNumber
                                    service.FileVersions.Add(fileVersionRepo.MapFromObject(fileEntryVersionsInfo))

                                    'case when Is Replace Existing file is true, we first do soft delete and then move
                                    If (IsReplaceExistingFile = True) Then
                                        'delete existing file in case of replace
                                        res = Me.SoftDelete(obj.FileEntryId, obj.VersionNumber)
                                        If (res.Status = Status.Error OrElse res.Status = Status.ChildNotDeleted) Then Return res
                                    End If

                                    'Copy to destination and delete it from source
                                    'res = Me.CopyAndDeleteFileFromFileShare(objCurrentfileVersion.FileEntryId, MapFromObject(obj, fileEntryInfo.FileShareId))
                                    res = Me.CopyAndDeleteFileFromFileShare(objCurrentfileVersion.FileEntryId, MapFromObject(objCurrentfileVersion, fileEntryInfo.FileShareId))
                                    If (res.Status = Status.Error) Then
                                        Return BuildResponse(res.Data, res.Status, res.Message)
                                    End If

                                    fileEntryInfo.CurrentVersionNumber = versionNumber

                                    service.SaveChanges()

                                    res.Data = True
                                    res.Message = ServiceConstants.OPERATION_SUCCESSFUL
                                    res.Status = Status.Success

                                Else
                                    res.Data = False
                                    res.Message = ServiceConstants.NOTHAVE_APPROPRIATEPERMISSION
                                    res.Status = Status.AccessDenied
                                End If
                            End If
                        Else

                            res.Data = False
                            res.Message = ServiceConstants.FILEPATH_NOLONGERVALID
                            res.Status = Status.PathNotValid

                        End If

                    Else
                        Dim userName As String = String.Empty
                        If fileEntryInfo.CheckedOutByUserId.HasValue Then
                            Dim userResult = (New UserRepository() With {.fncUser = Me.fncUser}).GetUserById(fileEntryInfo.CheckedOutByUserId.Value)

                            If userResult.Status = Status.Success Then
                                userName = userResult.Data.UserName
                            End If
                        End If

                        res.Data = False
                        res.Message = String.Format(ServiceConstants.FILE_ALREADY_CHECKOUT, userName)
                        res.Status = Status.FileCheckedOutByDifferentUser
                    End If

                Else
                    res.Data = False
                    res.Message = ServiceConstants.FILE_NOTFOUND
                    res.Status = Status.NotFound
                End If
            End Using

            Return BuildResponse(res.Data, res.Status, res.Message)

        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function



    ''' <summary>
    ''' Get all Deleted Items (includes folders/files)
    ''' </summary>
    ''' <param name="FileEntryId">Id of the parent FileEntry</param>
    ''' <returns></returns>
    Public Function GetAllDeletedChildren(FileEntryId As Guid) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.GetAllDeletedChildren
        Try
            Using service = GetServerEntity()
                Dim userId As Guid = Me.User.UserId

                If (service.FileEntries.Any(Function(p) p.FileEntryId = FileEntryId AndAlso p.IsDeleted)) Then
                    Return BuildResponse(New List(Of FileEntryInfo), Status.NotFound, "Folder Not Found")
                End If

                Dim result = service.GetFileEntryChildrenHierarchyInclDeleted(FileEntryId) 'GetLatestFileVersionDeletedChildrenWithPermission(id, Me.User.UserId).OrderBy(Function(p) p.FileEntryTypeId).ThenBy(Function(p) p.Name)
                Dim newResultWithUser = (From b In result
                                         Join FE In service.FileEntries On b.FileEntryId Equals FE.FileEntryId
                                         Join a In service.FileVersions On a.FileEntryId Equals FE.FileEntryId
                                         Join GH In service.FileEntryDeleteGroupHierarchies On a.FileEntryId Equals GH.FileEntryId
                                         Join G In service.FileEntryDeleteGroups On G.FileEntryDeleteGroupId Equals GH.FileEntryDeleteGroupId
                                         Join u In service.Users On G.DeletedByUserId Equals u.UserId
                                         Group Join fel In service.FileEntryLinks.Where(Function(p) Not p.IsDeleted) On b.FileEntryId Equals fel.PreviousFileEntryId
                                         Into flist = Group
                                         From fel In flist.DefaultIfEmpty
                                         Where a.VersionNumber = FE.CurrentVersionNumber AndAlso fel Is Nothing
                                         Select New FileEntryInfo With {
                                             .FileEntryId = FE.FileEntryId,
                                             .FileEntryTypeId = FE.FileEntryTypeId,
                                             .CurrentVersionNumber = FE.CurrentVersionNumber,
                                             .FileShareId = FE.FileShareId,
                                             .DeletedByUserId = G.DeletedByUserId,
                                             .DeletedOnUTC = G.DeletedOnUTC,
                                             .DeletedByUserName = u.UserName,
                                             .FileVersion = New Model.FileVersionInfo With {
                                                 .FileEntryName = a.FileEntryName,
                                                 .FileEntryNameWithExtension = a.FileEntryNameWithExtension,
                                                 .FileEntryRelativePath = a.FileEntryRelativePath,
                                                 .DeletedByUserId = G.DeletedByUserId,
                                                 .CreatedByUserName = u.UserName,
                                                 .DeletedOnUTC = G.DeletedOnUTC
                                             }
                                         })

                Return BuildResponse(newResultWithUser.ToList())

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete data permanently from fileDeleteGroupHierarchy and fileDeleteGroup table
    ''' </summary>
    ''' <param name="service">DbContext</param>
    ''' <param name="FileEntryId">Id of the file to be deleted</param>
    ''' <returns>ResultInfo Object with Success or failure</returns>
    Function DeleteFromFileEntryDeleteGroupHierarchy(service As FileMinisterEntities, FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.Error}

        Try
            Dim fileDeleteGroupHierarchy = service.FileEntryDeleteGroupHierarchies.Where(Function(p) p.FileEntryId = FileEntryId).ToList()
            Dim userId As Guid = Me.User.UserId

            For Each file In fileDeleteGroupHierarchy
                Dim objGroupHierarchy = service.FileEntryDeleteGroupHierarchies.Where(Function(d) d.FileEntryId = file.FileEntryId)
                'Delete from child table
                If (objGroupHierarchy.SingleOrDefault() IsNot Nothing) Then
                    service.FileEntryDeleteGroupHierarchies.Remove(objGroupHierarchy.FirstOrDefault())
                End If
                'Delete from parent table
                Dim objDeleteGroup = service.FileEntryDeleteGroups.Where(Function(d) d.DeletedByUserId = userId And d.FileEntryDeleteGroupId = file.FileEntryDeleteGroupId)
                If (objDeleteGroup.SingleOrDefault() IsNot Nothing) Then
                    service.FileEntryDeleteGroups.Remove(objDeleteGroup.FirstOrDefault())
                End If
            Next

            result.Data = True
            result.Status = Status.Success
            result.Message = ServiceConstants.OPERATION_SUCCESSFUL

        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try

        Return result
    End Function

    ''' <summary>
    ''' AddFileEntryDeleteGroupHierarchy -  this method will add record in FileEntryDeleteGroup/FileEntryDeleteGroupHierarchy
    ''' </summary>
    ''' <param name="service">DbContext</param>
    ''' <param name="file">FileEntry Object</param>
    ''' <param name="undelChildren">List of Undeleted childern</param>
    ''' <returns>ResultInfo object</returns>
    Private Function AddFileEntryDeleteGroupHierarchy(service As FileMinisterEntities, file As FileEntry, undelChildren As List(Of GetLatestFileVersionChildrenHierarchy_Result)) As ResultInfo(Of Boolean, Status)
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.Error}
        Dim parentFileEntryId As Guid = Guid.Empty

        Try

            If (Not service.FileEntryDeleteGroupHierarchies.Any(Function(p) p.FileEntryId = file.FileEntryId)) Then

                'Add to dbo.FileEntryDeleteGroup
                Dim fileEntryDeleteGroup = service.FileEntryDeleteGroups.Add(New FileEntryDeleteGroup With {.FileEntryDeleteGroupId = Guid.NewGuid(), .DeletedByUserId = Me.User.UserId, .DeletedOnUTC = DateTime.UtcNow()})

                'Add to dbo.FileEntryDeleteGroupHierarchy
                If (undelChildren Is Nothing OrElse undelChildren.Count = 0) Then

                    undelChildren = New List(Of GetLatestFileVersionChildrenHierarchy_Result)()
                    undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = file.FileEntryId})
                    Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = file.FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
                    If (latestVersion IsNot Nothing) Then
                        parentFileEntryId = latestVersion.FileEntryId
                        service.FileEntryDeleteGroupHierarchies.AddRange(MapFromCollection(undelChildren, parentFileEntryId, fileEntryDeleteGroup))
                    End If
                Else
                    parentFileEntryId = file.FileEntryId
                    'Add parent record as well
                    Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = file.FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
                    If (latestVersion IsNot Nothing) Then
                        'undelChildren = New List(Of GetLatestFileVersionChildrenHierarchy_Result)()
                        undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = file.FileEntryId})
                        service.FileEntryDeleteGroupHierarchies.AddRange(MapFromCollection(undelChildren, latestVersion.FileEntryId, fileEntryDeleteGroup))
                    End If

                End If

                service.SaveChanges()
            End If

            result.Data = True
            result.Status = Status.Success

        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try

        Return result

    End Function


    'public Function AddFileEntryDeleteGroupHierarchy(service As FileMinisterEntities, file As FileEntry, undelChildren As List(Of GetLatestFileVersionChildrens_Result)) As ResultInfo(Of Boolean, Status)
    '    Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False, .Status = Status.Error}
    '    Dim parentFileEntryId As Guid = Guid.Empty


    '    Try
    '        If Not service.FileEntryDeleteGroupHierarchies.Any(Function(p) p.FileEntryId = file.FileEntryId) Then

    '            'Add to dbo.FileEntryDeleteGroup
    '            Dim fileEntryDeleteGroup = service.FileEntryDeleteGroups.Add(New FileEntryDeleteGroup With {.FileEntryDeleteGroupId = Guid.NewGuid(), .DeletedByUserId = Me.User.UserId, .DeletedOnUTC = DateTime.UtcNow()})

    '            'Add to dbo.FileEntryDeleteGroupHierarchy
    '            If (undelChildren Is Nothing OrElse undelChildren.Count = 0) Then
    '                undelChildren = New List(Of GetLatestFileVersionChildrens_Result)()
    '                undelChildren.Add(New GetLatestFileVersionChildrens_Result() With {.FileEntryId = file.FileEntryId})
    '                Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = file.FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)

    '                If (latestVersion IsNot Nothing) Then
    '                    parentFileEntryId = latestVersion.ParentFileEntryId
    '                    service.FileEntryDeleteGroupHierarchies.AddRange(MapFromCollection(undelChildren, parentFileEntryId, fileEntryDeleteGroup))
    '                End If

    '            Else
    '                parentFileEntryId = file.FileEntryId
    '                'Add parent record as well
    '                Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = file.FileEntryId AndAlso p.VersionNumber = file.CurrentVersionNumber)
    '                If (latestVersion IsNot Nothing) Then
    '                    'undelChildren = New List(Of GetLatestFileVersionChildrens_Result)()
    '                    undelChildren.Add(New GetLatestFileVersionChildrens_Result() With {.FileEntryId = file.FileEntryId})
    '                    service.FileEntryDeleteGroupHierarchies.AddRange(MapFromCollection(undelChildren, latestVersion.ParentFileEntryId, fileEntryDeleteGroup))
    '                End If
    '            End If
    '        End If

    '        result.Data = True
    '        result.Status = Status.Success

    '    Catch ex As Exception
    '        Return BuildExceptionResponse(Of Boolean)(ex)
    '    End Try

    '    Return result

    'End Function

    ''' <summary>
    ''' This method will mark isdeleted=true in FileEntry and corresponding FileVersion Table
    ''' </summary>
    ''' <param name="service">DbContext</param>
    ''' <param name="undelChildren">Collection of undeleted childern</param>
    ''' <param name="FileEntryId">Id of the file</param>
    Private Sub DeleteFileVersionEntry(service As FileMinisterEntities, undelChildren As IQueryable(Of GetLatestFileVersionChildrenHierarchy_Result), FileEntryId As Guid)

        Dim userId As Guid = Me.User.UserId
        If (undelChildren IsNot Nothing) Then

            'Create a CSV of the undeleted childern
            Dim fileEntryCsv = String.Join(",", undelChildren.Select(Function(p) p.FileEntryId).ToArray())

            'Get all FileVersions for the CSV created above
            Dim resultVersionList = service.FileVersions.Where(Function(p) fileEntryCsv.Contains(p.FileEntryId.ToString())).ToList() 'And p.ParentFileEntryId = FileEntryId).ToList()
            For Each fileVersion In resultVersionList

                Dim resultFileEntryList = service.FileEntries.Where(Function(p) p.FileEntryId = fileVersion.FileEntryId And p.CurrentVersionNumber = fileVersion.VersionNumber)
                Dim fileEntry = resultFileEntryList.FirstOrDefault()
                If (fileEntry IsNot Nothing) Then

                    fileEntry.IsDeleted = True
                    fileEntry.DeletedByUserId = userId
                    fileEntry.DeletedOnUTC = DateTime.UtcNow()

                    fileVersion.IsDeleted = True
                    fileVersion.DeletedByUserId = userId
                    fileVersion.DeletedOnUTC = DateTime.UtcNow()

                End If

            Next
        End If

    End Sub

    ''' <summary>
    ''' This method will mark isdeleted=true in FileEntry and corresponding FileVersion Table
    ''' </summary>
    ''' <param name="service">DbContext</param>
    ''' <param name="undelChildren">Collection of undeleted childern</param>
    ''' <param name="FileEntryId">Id of the file</param>
    Private Sub DeleteFileVersionEntry(service As FileMinisterEntities, undelChildren As IQueryable(Of GetLatestFileVersionChildrens_Result), FileEntryId As Guid)

        Dim userId As Guid = Me.User.UserId
        If (undelChildren IsNot Nothing) Then
            Dim fileEntryCsv = String.Join(",", undelChildren.Select(Function(p) p.FileEntryId).ToArray())

            Dim resultVersionList = service.FileVersions.Where(Function(p) fileEntryCsv.Contains(p.FileEntryId.ToString()) And p.ParentFileEntryId = FileEntryId).ToList()
            For Each fileVersion In resultVersionList

                Dim resultFileEntryList = service.FileEntries.Where(Function(p) p.FileEntryId = fileVersion.FileEntryId And p.CurrentVersionNumber = fileVersion.VersionNumber)
                Dim fileEntry = resultFileEntryList.FirstOrDefault()
                If (fileEntry IsNot Nothing) Then

                    fileEntry.IsDeleted = True
                    fileEntry.DeletedByUserId = userId
                    fileEntry.DeletedOnUTC = DateTime.UtcNow()

                    fileVersion.IsDeleted = True
                    fileVersion.DeletedByUserId = userId
                    fileVersion.DeletedOnUTC = DateTime.UtcNow()

                End If

            Next
        End If

    End Sub

    ''' <summary>
    ''' Un do the records in FileEntry and FileVersion table
    ''' </summary>
    ''' <param name="service">DbContext</param>
    ''' <param name="FileVersion">FileVersion Object</param>
    Private Sub UndoDeleteFileVersionEntry(service As FileMinisterEntities, FileVersion As FileVersion)

        Dim userId As Guid = Me.User.UserId

        Dim resultVersionList = service.FileVersions.Where(Function(p) p.FileEntryId = FileVersion.FileEntryId And p.VersionNumber = FileVersion.VersionNumber).ToList()
        For Each filever In resultVersionList

            Dim resultFileEntryList = service.FileEntries.Where(Function(p) p.FileEntryId = filever.FileEntryId And p.CurrentVersionNumber = filever.VersionNumber And p.IsPermanentlyDeleted = False)
            Dim file = resultFileEntryList.FirstOrDefault()
            If (file IsNot Nothing) Then

                filever.IsDeleted = False
                filever.DeletedByUserId = Nothing
                filever.DeletedOnUTC = Nothing

                file.IsDeleted = False
                file.DeletedByUserId = Nothing
                file.DeletedOnUTC = Nothing

            End If

        Next

        'Now Enable Parent Details as well
        Dim parentHierarchy = service.GetLatestFileVersionDeletedParentHierarchy(FileVersion.FileEntryId)
        For Each parent In parentHierarchy
            Dim fileEntry = service.FileEntries.Where(Function(p) p.FileEntryId = parent.ParentFileEntryId And p.IsDeleted = True And p.IsPermanentlyDeleted = False).FirstOrDefault()
            If (fileEntry IsNot Nothing) Then
                fileEntry.IsDeleted = False
                fileEntry.DeletedByUserId = Nothing
                fileEntry.DeletedOnUTC = Nothing

                Dim fileEntryVersion = service.FileVersions.Where(Function(p) p.FileEntryId = fileEntry.FileEntryId And p.IsDeleted = True).ToList()
                For Each version In fileEntryVersion
                    version.IsDeleted = False
                    version.DeletedByUserId = Nothing
                    version.DeletedOnUTC = Nothing
                Next
            End If
        Next

    End Sub

    Public Function AddVersionButRemainCheckedOut(FileEntryVersionsInfo As Model.FileVersionInfo, possibleHandles As List(Of FileHandleInfo)) As ResultInfo(Of Integer, Status)
        Try
            Dim res As ResultInfo(Of Integer, Status) = New ResultInfo(Of Integer, Status)
            res.Data = -1

            Using Service = GetServerEntity()
                Dim file = Service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryVersionsInfo.FileEntryId)
                If (file IsNot Nothing AndAlso file.IsCheckedOut) Then
                    Dim currentVersionNumber = file.CurrentVersionNumber + 1
                    FileEntryVersionsInfo.VersionNumber = currentVersionNumber
                    'FileEntryVersionsInfo.CreatedByUserId = Me.User.UserId
                    'FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()
                    FileEntryVersionsInfo.IsDeleted = False
                    Dim fileVersionRepository As FileVersionRepository = New FileVersionRepository()
                    fileVersionRepository.fncUser = Me.fncUser
                    file.CurrentVersionNumber = currentVersionNumber
                    Dim fv = Service.FileVersions.Add(fileVersionRepository.MapFromObject(FileEntryVersionsInfo))

                    CopyFromFileToBlob(FileEntryVersionsInfo)

                    Service.SaveChanges()
                    For Each possibleHandle In possibleHandles
                        Dim handleVersionFile As New FileVersionThruCloud
                        handleVersionFile.FileHandleId = possibleHandle.FileHandleId
                        handleVersionFile.FileVersionId = fv.FileVersionId
                        Service.FileVersionThruClouds.Add(handleVersionFile)
                    Next
                    Service.SaveChanges()
                    res.Data = currentVersionNumber
                Else
                    res.Status = Status.NotFound
                    res.Message = ServiceConstants.FILE_NOTFOUND
                End If

            End Using
            'todo: blob work
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try
    End Function

    Public Function AddVersionAndCheckIn(FileEntryVersionsInfo As Model.FileVersionInfo, possibleHandles As List(Of FileHandleInfo)) As ResultInfo(Of Integer, Status)
        Try
            Dim res As ResultInfo(Of Integer, Status) = New ResultInfo(Of Integer, Status)
            res.Data = -1

            Using Service = GetServerEntity()
                Dim file = Service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryVersionsInfo.FileEntryId)
                If (file IsNot Nothing AndAlso file.IsCheckedOut) Then
                    Dim currentVersionNumber = file.CurrentVersionNumber + 1
                    FileEntryVersionsInfo.VersionNumber = currentVersionNumber
                    'FileEntryVersionsInfo.CreatedByUserId = Me.User.UserId
                    'FileEntryVersionsInfo.CreatedOnUTC = DateTime.UtcNow()
                    FileEntryVersionsInfo.IsDeleted = False
                    Dim fileVersionRepository As FileVersionRepository = New FileVersionRepository()
                    fileVersionRepository.fncUser = Me.fncUser
                    file.CurrentVersionNumber = currentVersionNumber

                    file.IsCheckedOut = False
                    file.CheckedOutByUserId = Nothing
                    file.CheckedOutOnUTC = Nothing
                    file.CheckedOutWorkSpaceId = Nothing

                    Dim fv = Service.FileVersions.Add(fileVersionRepository.MapFromObject(FileEntryVersionsInfo))

                    CopyFromFileToBlob(FileEntryVersionsInfo)

                    Service.SaveChanges()
                    If (possibleHandles IsNot Nothing) Then
                        For Each possibleHandle In possibleHandles
                            Dim handleVersionFile As New FileVersionThruCloud
                            handleVersionFile.FileHandleId = possibleHandle.FileHandleId
                            handleVersionFile.FileVersionId = fv.FileVersionId
                            Service.FileVersionThruClouds.Add(handleVersionFile)
                        Next
                        Service.SaveChanges()
                    End If

                    res.Data = currentVersionNumber
                Else
                    res.Status = Status.NotFound
                    res.Message = ServiceConstants.FILE_NOTFOUND
                End If

            End Using
            'todo: blob work
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Integer)(ex)
        End Try
    End Function

    Sub Checkout(FileEntryId As Guid)
        Try
            Dim res As ResultInfo(Of Integer, Status) = New ResultInfo(Of Integer, Status)
            res.Data = -1

            Using Service = GetServerEntity()
                Dim dbFile = Service.FileEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileEntryId = FileEntryId)
                If (Not dbFile.IsCheckedOut) Then
                    dbFile.IsCheckedOut = True
                    dbFile.CheckedOutByUserId = Me.User.UserId
                    'dbFile.CheckedOutWorkSpaceId = Me.User.work
                    dbFile.CheckedOutOnUTC = DateTime.UtcNow
                    Service.SaveChanges()

                End If
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Function ResolveConflict(fileEntryId As Guid, mode As ConflictResolutonMode, Optional conflictFileName As String = Nothing) As ResultInfo(Of Boolean, Status)
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            res.Data = False

            Using Service = GetServerEntity()
                Dim conflict = Service.FileConflicts.FirstOrDefault(Function(p) p.FileEntryId = fileEntryId AndAlso p.ConflictResolvedTime Is Nothing)
                If (conflict IsNot Nothing) Then
                    conflict.ConflictResolvedTime = DateTime.UtcNow
                    conflict.ConflictResolutionModeId = CInt(mode)
                    conflict.ResolvedBy = Me.User.UserId

                    Dim files = Service.FileConflictFiles.Where(Function(f) f.FileConflictId = conflict.FileConflictId).ToList()
                    For Each file In files
                        If (conflictFileName IsNot Nothing AndAlso file.ConflictFileName = conflictFileName) Then
                            file.IsUsedForResolution = True
                        Else
                            file.IsUsedForResolution = False
                        End If
                        'todo: remove file from fileserver
                        'Helper.GetFileReference()
                    Next
                    Service.SaveChanges()
                    res.Data = True
                End If
            End Using

            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function GetListOfConflictedFiles(fileEntryId As Guid) As ResultInfo(Of List(Of String), Status)
        Try
            Dim res As ResultInfo(Of List(Of String), Status) = New ResultInfo(Of List(Of String), Status)
            res.Data = New List(Of String)

            Using Service = GetServerEntity()
                Dim conflict = Service.FileConflicts.FirstOrDefault(Function(p) p.FileEntryId = fileEntryId AndAlso p.ConflictResolvedTime Is Nothing)
                If (conflict IsNot Nothing) Then
                    res.Data = Service.FileConflictFiles.Where(Function(f) f.FileConflictId = conflict.FileConflictId).Select(Function(s) s.ConflictFileName).ToList()
                End If
            End Using

            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of String))(ex)
        End Try
    End Function

    Public Function GetAllConflictedFilesForShare(fileShareId As Short) As ResultInfo(Of List(Of String), Status)
        Try
            Dim res As ResultInfo(Of List(Of String), Status) = New ResultInfo(Of List(Of String), Status)
            res.Data = New List(Of String)

            Using Service = GetServerEntity()
                Dim conflicts = (From c In Service.FileConflicts
                                 Join e In Service.FileEntries
                                    On c.FileEntryId Equals e.FileEntryId
                                 Join f In Service.FileConflictFiles
                                         On c.FileConflictId Equals f.FileConflictId
                                 Where c.ConflictResolvedTime Is Nothing AndAlso e.FileShareId = fileShareId
                                 Select f.ConflictFilePath).ToList()
                res.Data = conflicts
            End Using

            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of String))(ex)
        End Try
    End Function


    Public Sub RemoveDeletedConflictedFiles(conflictedFilesRemoved As List(Of String))
        Using Service = GetServerEntity()
            Dim files = From f In Service.FileConflictFiles
                        Join c In conflictedFilesRemoved
                            On f.ConflictFilePath.ToLower() Equals c
                        Select f
            Service.FileConflictFiles.RemoveRange(files)
            Service.SaveChanges()

            Dim conflictsZeroFiles = From c In Service.FileConflicts
                                     Group Join f In Service.FileConflictFiles
                                                     On c.FileConflictId Equals f.FileConflictId Into Group
                                     From f In Group.DefaultIfEmpty()
                                     Where f Is Nothing AndAlso c.ConflictResolvedTime Is Nothing AndAlso DateAndTime.DateDiff(DateInterval.Minute, DateTime.Now, c.CreatedAt) > 5
                                     Select c
            Service.FileConflicts.RemoveRange(conflictsZeroFiles)
            Service.SaveChanges()
        End Using
    End Sub

    Public Function SetConflictFiles(fileEntryId As Guid, conflictFilesPath As List(Of String)) As ResultInfo(Of Boolean, Status)
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            res.Data = False

            Using Service = GetServerEntity()
                Dim conflict = Service.FileConflicts.FirstOrDefault(Function(p) p.FileEntryId = fileEntryId AndAlso p.ConflictResolvedTime Is Nothing)
                If (conflict IsNot Nothing) Then
                    Dim dbFiles = Service.FileConflictFiles.Where(Function(f) f.FileConflictId = conflict.FileConflictId).ToList()

                    Dim newFiles =
                        From c In conflictFilesPath
                        Group Join d In dbFiles On c Equals d.ConflictFilePath Into Group
                        From d In Group.DefaultIfEmpty() Where d Is Nothing


                    For Each file In newFiles
                        Dim fConflict As New FileConflictFile
                        fConflict.ConflictFileName = file.c.Substring(file.c.LastIndexOf("\") + 1)
                        fConflict.FileConflictId = conflict.FileConflictId
                        fConflict.CreatedAt = DateTime.UtcNow
                        fConflict.ConflictFilePath = file.c
                        Service.FileConflictFiles.Add(fConflict)
                    Next

                    'files removal happening through watcher
                    'Dim removedFiles =
                    '    From d In dbFiles
                    '    Group Join c In conflictFilesPath On c Equals d.ConflictFilePath Into Group
                    '    From c In Group.DefaultIfEmpty() Where c Is Nothing

                    'For Each file In removedFiles
                    '    Service.FileConflictFiles.Remove(file.d)
                    'Next
                    Service.SaveChanges()
                    res.Data = True
                Else
                    Dim newConflict As New FileConflict
                    newConflict.CreatedAt = DateTime.UtcNow
                    newConflict.FileEntryId = fileEntryId
                    newConflict = Service.FileConflicts.Add(newConflict)
                    Service.SaveChanges()

                    For Each filePath In conflictFilesPath
                        Dim fConflict As New FileConflictFile
                        fConflict.ConflictFileName = filePath.Substring(filePath.LastIndexOf("\") + 1)
                        fConflict.FileConflictId = newConflict.FileConflictId
                        fConflict.CreatedAt = DateTime.UtcNow
                        fConflict.ConflictFilePath = filePath
                        Service.FileConflictFiles.Add(fConflict)
                    Next
                    Service.SaveChanges()
                End If
            End Using

            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function
End Class
