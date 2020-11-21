Imports System.Text.RegularExpressions
Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports FileMinister.Repo
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage.File
Imports risersoft.shared.cloud
Imports risersoft.shared
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Proxies
Imports risersoft.shared.DotnetFx

Public Class CloudFileShareWatcher
    'todo: fill this
    Private ignoreFileList As New List(Of String)
    Private ignoreFolderList As String() = {".SystemShareInformation"}

    Public Sub New()
    End Sub

    Public Sub SyncAll()
        Dim accountList = AgentAuthProvider.GenerateAccountList(myApp)
        For Each acc In accountList
            If Not String.IsNullOrWhiteSpace(acc.Account.StorageAccount) Then
                Dim dbConnectionString = ProductUtils.CalculateConnectionStringModule("FileMinister", "File", acc.Account.AccountModuleList)
                If Not String.IsNullOrWhiteSpace(dbConnectionString) Then
                    Dim caller As New RSCallerInfo
                    caller.Account = acc.Account
                    caller.UserAccount = New UserAccountProxy With {.AccountId = acc.Account.Id}
                    SyncAccount(acc.Account.StorageAccount, caller, dbConnectionString)
                End If
            End If
        Next
    End Sub

    Private Sub SyncAccount(storageAccConnectionString As String, caller As RSCallerInfo, dbConnectionString As String)
        Dim storageAccount As CloudStorageAccount = CloudStorageAccount.Parse(storageAccConnectionString)
        ' Create a CloudFileClient object for credentialed access to Azure Files.
        Dim fileClient As CloudFileClient = storageAccount.CreateCloudFileClient
        Dim cloudShareList As IEnumerable(Of CloudFileShare) = fileClient.ListShares
        For Each share As CloudFileShare In cloudShareList
            Me.SyncShare(share, caller, dbConnectionString)
        Next
    End Sub

    Private Sub SyncShare(ByVal share As CloudFileShare, caller As RSCallerInfo, dbConnectionString As String)
        ' Ensure that the share exists.
        If share.Exists Then
            ' Get a reference to the root directory for the share.
            Dim rootDir As CloudFileDirectory = share.GetRootDirectoryReference
            Dim dirs As List(Of CloudDirExtension) = New List(Of CloudDirExtension)
            Dim files As List(Of CloudFileExtension) = New List(Of CloudFileExtension)
            Me.enumerateFiles(rootDir, dirs, files, rootDir.Uri.LocalPath)
            'Using service As New FileMinisterEntities(dbConnectionString, caller.Account.Id)
            'getShareByName
            Dim fileTypeDir = CType(FileType.Folder, Byte)
            Dim fileTypeFile = CType(FileType.File, Byte)

            Dim shareRep As New ShareRepository
            shareRep.User = caller

            Dim dbShare = shareRep.GetShareByName(share.Name)
            If (dbShare.Status = Status.Success) Then
                Dim dbRootFolderEntryId = dbShare.Data.FileEntryId
                Dim fileRep As New FileRepository
                fileRep.User = caller
                'Dim childFolders = fileRep.GetAllChildren(dbRootFolder.FileEntryId, FileType.Folder)
                'Dim childFiles = fileRep.GetAllChildren(dbRootFolder.FileEntryId, FileType.File)
                Dim childs = fileRep.GetAllChildrenHierarchy(dbRootFolderEntryId)
                If (childs.Status = Status.Success) Then

                    Dim conflictedFilesInDb = fileRep.GetAllConflictedFilesForShare(dbShare.Data.FileShareId).Data
                    If (conflictedFilesInDb.Count > 0) Then
                        'removing all conflicted files in db but no longer present in cloud file
                        Dim conflictedFilesRemoved = (From c In conflictedFilesInDb
                                                      Group Join file In files
                                                     On c.ToLower() Equals file.RelativePath Into Group
                                                      From file In Group.DefaultIfEmpty()
                                                      Where file Is Nothing
                                                      Select c).ToList()
                        If conflictedFilesRemoved.Count > 0 Then
                            fileRep.RemoveDeletedConflictedFiles(conflictedFilesRemoved)
                        End If

                        'get only non conflicted files
                        Dim filesConflictedAndExistsInBothPlaces = (From c In conflictedFilesInDb
                                                                    Join file In files
                                                     On c.ToLower() Equals file.RelativePath
                                                                    Select file).ToList()
                        If (filesConflictedAndExistsInBothPlaces.Count > 0) Then
                            files = files.Except(filesConflictedAndExistsInBothPlaces).ToList()
                        End If
                    End If


                    'get servernames for share
                    Dim workspaceRep As New WorkspaceRepository
                    workspaceRep.User = caller
                    Dim ws = workspaceRep.GetWorkspacesForShare(dbShare.Data.FileShareId)
                    Dim servers = ws.Data.Select(Function(w) w.ServerName).ToList()

                    'handle new folders in db and not in db
                    Dim newFolderQuery = From dir In dirs
                                         Group Join v In childs.Data.Where(Function(p) p.FileEntryTypeId = fileTypeDir).ToList()
                   On dir.RelativePath Equals (v.FileVersion.FileEntryRelativePath).ToLower() Into Group
                                         From v In Group.DefaultIfEmpty()
                                         Where v Is Nothing
                                         Select dir
                                         Order By dir.RelativePath.Length

                    For Each dd As CloudDirExtension In newFolderQuery
                        'to be ignored folders already ignored
                        'create new folders
                        'get relative path, get server file entry for the same and then add new file folder entry in db
                        Dim relativePath = dd.RelativePath
                        Dim parentRelativePath = Util.Helper.GetParentRelativePath(relativePath)
                        Dim parentFileSystemEntryId As Guid
                        If parentRelativePath <> String.Empty Then
                            Dim parentFileSystemLatestVersion = fileRep.GetLatestFileEntryVersionByPath(parentRelativePath, dbShare.Data.FileShareId)
                            If (parentFileSystemLatestVersion.Status <> Status.Success) Then
                                Continue For
                            End If
                            parentFileSystemEntryId = parentFileSystemLatestVersion.Data.FileEntryId
                        Else
                            parentFileSystemEntryId = dbRootFolderEntryId
                        End If
                        Dim fileSystemEntry As New FileEntryInfo
                        With fileSystemEntry
                            .FileEntryId = Guid.NewGuid
                            .FileEntryTypeId = FileType.Folder.ToString("D")
                            .FileShareId = dbShare.Data.FileShareId
                        End With

                        Dim fileSystemEntryVersion As New FileVersionInfo

                        With fileSystemEntryVersion
                            .CreatedOnUTC = dd.CloudDir.Properties.LastModified.GetValueOrDefault().DateTime
                            .FileEntryName = dd.CloudDir.Name
                            .FileEntryRelativePath = relativePath
                            .FileEntrySize = 0
                            .FileVersionId = Guid.NewGuid
                            .ParentFileEntryId = parentFileSystemEntryId
                            .FileEntryHash = 0
                            '.CheckInSource = Models.Enums.CheckInSource.Cloud
                        End With
                        'fileRep.AddFolderNoCreateNoPermissionCheck(fileSystemEntry, fileSystemEntryVersion)
                    Next

                    'handle folders to be marked as deleted
                    Dim removeFolderQuery = From v In childs.Data.Where(Function(p) p.FileEntryTypeId = fileTypeDir).ToList()
                                            Group Join dir In dirs
                                On dir.RelativePath Equals (v.FileVersion.FileEntryRelativePath).ToLower() Into Group
                                            From dir In Group.DefaultIfEmpty()
                                            Where dir Is Nothing
                                            Select v
                                            Order By v.FileVersion.FileEntryRelativePath.Length

                    Dim foldersToBeDeleted = removeFolderQuery.ToList()
                    Dim countDeletedFolders = foldersToBeDeleted.Count


                    ' handle files to be marked as deleted
                    Dim removeFileQuery = From v In childs.Data.Where(Function(p) p.FileEntryTypeId = fileTypeFile).ToList()
                                          Group Join file In files
                                On file.RelativePath Equals (v.FileVersion.FileEntryRelativePath).ToLower() Into Group
                                          From file In Group.DefaultIfEmpty()
                                          Where file Is Nothing
                                          Select v
                                          Order By v.FileVersion.FileEntryRelativePath.Length

                    Dim filesToBeDeleted = removeFileQuery.ToList()

                    For i As Integer = 0 To countDeletedFolders - 1 Step 1
                        Dim folderToBeDeleted = foldersToBeDeleted(i)
                        Dim otherInternalFolders = foldersToBeDeleted.Where(Function(f) f.FileVersion.FileEntryRelativePath.StartsWith(folderToBeDeleted.FileVersion.FileEntryRelativePath) AndAlso f.FileEntryId <> folderToBeDeleted.FileEntryId).ToList()
                        Dim otherInternalFilesTobeDeleted = filesToBeDeleted.Where(Function(f) f.FileVersion.FileEntryRelativePath.StartsWith(folderToBeDeleted.FileVersion.FileEntryRelativePath)).ToList()
                        'Dim undelChildren As New List(Of GetLatestFileVersionChildrenHierarchy_Result)()
                        'For Each f In otherInternalFolders
                        '    'undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = f.FileEntryId})
                        '    foldersToBeDeleted.Remove(f)
                        '    i = i + 1
                        'Next
                        'For Each f In otherInternalFilesTobeDeleted
                        '    'undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = f.FileEntryId})
                        '    filesToBeDeleted.Remove(f)
                        'Next

                        'fileRep.SoftDeleteNoCheck(folderToBeDeleted.FileEntryId)
                        'Add parent record as well
                        'undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = folderToBeDeleted.FileEntryId})
                        'delete as one group
                        'If (Not service.FileEntryDeleteGroupHierarchies.Any(Function(p) p.FileEntryId = folderToBeDeleted.FileEntryId)) Then
                        '    'Add to dbo.FileEntryDeleteGroup
                        '    Dim fileEntryDeleteGroup = service.FileEntryDeleteGroups.Add(New FileEntryDeleteGroup With {.FileEntryDeleteGroupId = Guid.NewGuid(), .DeletedOnUTC = DateTime.UtcNow()})
                        '    Dim parentFileEntryId = folderToBeDeleted.FileEntryId
                        '    Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = folderToBeDeleted.FileEntryId AndAlso p.VersionNumber = folderToBeDeleted.CurrentVersionNumber)
                        '    If (latestVersion IsNot Nothing) Then
                        '        service.FileEntryDeleteGroupHierarchies.AddRange(fileRep.MapFromCollection(undelChildren, latestVersion.ParentFileEntryId, fileEntryDeleteGroup))
                        '    End If
                        'End If

                        'Get all FileVersions for the CSV created above
                        'Dim resultVersionList = From d In undelChildren
                        '                        Join version In service.FileVersions
                        '                     On d.FileEntryId Equals (version.FileEntryId) Select version
                        'For Each version In resultVersionList

                        '    Dim resultFileEntryList = service.FileEntries.Where(Function(p) p.FileEntryId = version.FileEntryId And p.CurrentVersionNumber = version.VersionNumber)
                        '    Dim fileEntry = resultFileEntryList.FirstOrDefault()
                        '    If (fileEntry IsNot Nothing) Then

                        '        fileEntry.IsDeleted = True
                        '        fileEntry.DeletedOnUTC = DateTime.UtcNow()

                        '        version.IsDeleted = True
                        '        version.DeletedOnUTC = DateTime.UtcNow()
                        '    End If
                        'Next

                        ''Mark Folder itself delete
                        'folderToBeDeleted.IsCheckedOut = False
                        'folderToBeDeleted.CheckedOutByUserId = Nothing
                        'folderToBeDeleted.CheckedOutOnUTC = Nothing
                        'folderToBeDeleted.CheckOutWorkSpaceId = Nothing

                        'service.SaveChanges()
                    Next

                    'delete remaining files
                    Dim countDeletedFile = filesToBeDeleted.Count
                    For i As Integer = countDeletedFile - 1 To 0 Step -1
                        Dim deletedFileVersionInfo = filesToBeDeleted(i)
                        fileRep.SoftDeleteNoCheck(deletedFileVersionInfo.FileEntryId)
                        'If (deletedFileVersionInfo.IsCheckedOut = False) Then


                        '    If Not service.FileEntryDeleteGroupHierarchies.Any(Function(p) p.FileEntryId = deletedFileVersionInfo.FileEntryId) Then

                        '        'Add to dbo.FileEntryDeleteGroup
                        '        Dim fileEntryDeleteGroup = service.FileEntryDeleteGroups.Add(New FileEntryDeleteGroup With {.FileEntryDeleteGroupId = Guid.NewGuid(), .DeletedOnUTC = DateTime.UtcNow()})

                        '        'Add to dbo.FileEntryDeleteGroupHierarchy
                        '        Dim undelChildren As New List(Of GetLatestFileVersionChildrenHierarchy_Result)()
                        '        undelChildren.Add(New GetLatestFileVersionChildrenHierarchy_Result() With {.FileEntryId = deletedFileVersionInfo.FileEntryId})
                        '        Dim latestVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileEntryId = deletedFileVersionInfo.FileEntryId AndAlso p.VersionNumber = deletedFileVersionInfo.CurrentVersionNumber)

                        '        If (latestVersion IsNot Nothing) Then
                        '            Dim parentFileEntryId = latestVersion.ParentFileEntryId
                        '            service.FileEntryDeleteGroupHierarchies.AddRange(fileRep.MapFromCollection(undelChildren, parentFileEntryId, fileEntryDeleteGroup))
                        '        End If

                        '        Dim fileVersion = service.FileVersions.Where(Function(p) p.FileEntryId = deletedFileVersionInfo.FileEntryId And p.VersionNumber = deletedFileVersionInfo.CurrentVersionNumber).FirstOrDefault()

                        '        If (fileVersion IsNot Nothing) Then
                        '            fileVersion.IsDeleted = True
                        '            fileVersion.DeletedOnUTC = DateTime.UtcNow()
                        '        End If
                        '        Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = deletedFileVersionInfo.FileEntryId)
                        '        file.IsCheckedOut = False
                        '        file.CheckedOutByUserId = Nothing
                        '        file.CheckedOutOnUTC = Nothing
                        '        file.CheckedOutWorkSpaceId = Nothing

                        '        file.IsDeleted = True
                        '        file.DeletedOnUTC = DateTime.UtcNow()

                        '        service.SaveChanges()

                        '    End If

                        '    filesToBeDeleted.RemoveAt(i)
                        'End If

                    Next
                    'service.SaveChanges()

                    'new files
                    'todo: wait for some time to checkin. check if this can be done as part of new version only
                    'Dim addedFiles = (From file In files
                    '                  Group Join v In childs.Data.Where(Function(p) p.FileEntryTypeId = fileTypeFile).ToList()
                    '                On file.RelativePath Equals (v.FileVersion.FileEntryRelativePath).ToLower() Into Group
                    '                  From v In Group.DefaultIfEmpty()
                    '                  Where v Is Nothing
                    '                  Select file
                    '                  Order By file.RelativePath.Length).ToList()
                    'For Each dd As CloudFileExtension In addedFiles
                    '    'to be ignored folders already ignored
                    '    'create new files
                    '    Dim relativePath = dd.RelativePath
                    '    Dim parentRelativePath = GetParentRelativePath(relativePath)
                    '    Dim parentFileSystemEntryId As Guid
                    '    If parentRelativePath <> String.Empty Then
                    '        Dim parentFileSystemLatestVersion = fileRep.GetLatestFileEntryVersionByPath(parentRelativePath, dbShare.Data.FileShareId)
                    '        If (parentFileSystemLatestVersion.Status <> Status.Success) Then
                    '            Continue For
                    '        End If
                    '        parentFileSystemEntryId = parentFileSystemLatestVersion.Data.FileEntryId
                    '    Else
                    '        parentFileSystemEntryId = dbRootFolderEntryId
                    '    End If
                    '    Dim fileSystemEntry As New FileEntryInfo
                    '    With fileSystemEntry
                    '        .FileEntryId = Guid.NewGuid
                    '        .FileEntryTypeId = FileType.File.ToString("D")
                    '        .FileShareId = dbShare.Data.FileShareId
                    '    End With

                    '    Dim fileSystemEntryVersion As New FileVersionInfo

                    '    With fileSystemEntryVersion
                    '        .CreatedOnUTC = dd.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime
                    '        .FileEntryId = fileSystemEntry.FileEntryId
                    '        .FileEntryName = getFileNameWithoutExtension(dd.CloudFile.Name)
                    '        .FileEntryExtension = getFileExtension(dd.CloudFile.Name)
                    '        .FileEntryRelativePath = relativePath
                    '        .FileEntrySize = dd.CloudFile.Properties.Length
                    '        .FileVersionId = Guid.NewGuid
                    '        .ParentFileEntryId = parentFileSystemEntryId
                    '        .FileEntryHash = 0
                    '    End With
                    '    fileRep.AddFileCreateNoPermissionCheck(fileSystemEntry, fileSystemEntryVersion)
                    '    'todo: getAllConflictedFilesForFile(dd, addedFiles, servers)
                    'Next

                    Dim allConflictedFiles As New List(Of CloudFileExtension)

                    'get all files/folders from DB with latest version
                    'dirs - dbFolders -> New/Moved/Renamed
                    'dbFolders - dirs -> Deleted/Moved/Renamed
                    Dim fhrep As New FileHandleRepository
                    fhrep.User = caller

                    For Each file As CloudFileExtension In files
                        'ignore conflicted file
                        If (allConflictedFiles.Contains(file)) Then
                            Continue For
                        End If

                        'check for dir sync and file added/missing
                        'if file is presnt on both sides i.e. db and cloud
                        If (file.CloudFile.CopyState Is Nothing OrElse file.CloudFile.CopyState.Status = CopyStatus.Success) Then
                            'todo: gte from db
                            Dim dbFile = childs.Data.FirstOrDefault(Function(e) e.FileVersion.FileEntryRelativePath.ToLower() = file.RelativePath AndAlso e.FileEntryTypeId = FileType.File)
                            Dim fileEntryId As Guid?
                            If dbFile Is Nothing Then
                                'to be ignored folders already ignored
                                'create new files
                                Dim relativePath = file.RelativePath
                                'Dim parentRelativePath = Util.Helper.GetParentRelativePath(relativePath)
                                Dim parentFileSystemEntryId As Guid
                                'If parentRelativePath <> String.Empty Then
                                '    Dim parentFileSystemLatestVersion = fileRep.GetLatestFileEntryVersionByPath(parentRelativePath, dbShare.Data.FileShareId)
                                '    If (parentFileSystemLatestVersion.Status <> Status.Success) Then
                                '        Continue For
                                '    End If
                                '    parentFileSystemEntryId = parentFileSystemLatestVersion.Data.FileEntryId
                                'Else
                                '    parentFileSystemEntryId = dbRootFolderEntryId
                                'End If
                                Dim fileSystemEntry As New FileEntryInfo
                                With fileSystemEntry
                                    .FileEntryId = Guid.NewGuid
                                    .FileEntryTypeId = FileType.File.ToString("D")
                                    .FileShareId = dbShare.Data.FileShareId
                                End With

                                fileEntryId = fileSystemEntry.FileEntryId

                                Dim fileSystemEntryVersion As New FileVersionInfo

                                With fileSystemEntryVersion
                                    .CreatedOnUTC = file.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime
                                    .FileEntryId = fileSystemEntry.FileEntryId
                                    .FileEntryName = getFileNameWithoutExtension(file.CloudFile.Name)
                                    .FileEntryExtension = getFileExtension(file.CloudFile.Name)
                                    .FileEntryNameWithExtension = file.CloudFile.Name
                                    .FileEntryRelativePath = relativePath
                                    .FileEntrySize = file.CloudFile.Properties.Length
                                    .FileVersionId = Guid.NewGuid
                                    .ParentFileEntryId = parentFileSystemEntryId
                                    .FileEntryHash = 0
                                    '.CheckInSource = Models.Enums.CheckInSource.Cloud
                                    .FileEntry = New FileEntryInfo() With {
                                           .FileEntryId = fileSystemEntry.FileEntryId,
                                           .FileEntryTypeId = fileTypeFile,
                                           .IsDeleted = False,
                                           .FileShareId = dbShare.Data.FileShareId
                                           }
                                End With
                                'fileRep.AddFileCreateNoPermissionCheck(fileSystemEntry, fileSystemEntryVersion)
                            ElseIf (dbFile.FileVersion.FileEntrySize <> file.CloudFile.Properties.Length OrElse (file.CloudFile.Properties.LastModified.HasValue AndAlso dbFile.FileVersion.CreatedOnUTC < file.CloudFile.Properties.LastModified)) Then
                                fileEntryId = dbFile.FileEntryId
                                'file changed
                                If (dbFile.IsCheckedOut) Then
                                    Dim tobeProcessedFileHandles = fhrep.GetToBeProcessedFileHandles(dbFile.FileEntryId).Data
                                    'not needed as we may have already marked checkout for another user
                                    'If (tobeProcessedFileHandles IsNot Nothing AndAlso tobeProcessedFileHandles.Count > 0 AndAlso tobeProcessedFileHandles.FirstOrDefault(Function(h) h.UserId = dbFile.CheckedOutByUserId AndAlso h.WorkSpaceId = dbFile.CheckOutWorkSpaceId) IsNot Nothing) Then
                                    If (tobeProcessedFileHandles IsNot Nothing AndAlso tobeProcessedFileHandles.Count > 0) Then
                                        'checkin
                                        Dim fv As New FileMinister.Models.Sync.FileVersionInfo
                                        fv.FileEntryId = dbFile.FileEntryId
                                        fv.ParentFileEntryId = dbFile.FileVersion.ParentFileEntryId
                                        fv.PreviousFileVersionId = dbFile.FileVersion.FileVersionId
                                        fv.FileEntrySize = file.CloudFile.Properties.Length
                                        fv.FileEntryName = dbFile.FileVersion.FileEntryName
                                        fv.FileEntryExtension = dbFile.FileVersion.FileEntryExtension
                                        fv.FileEntryNameWithExtension = dbFile.FileVersion.FileEntryNameWithExtension
                                        fv.FileEntryRelativePath = dbFile.FileVersion.FileEntryRelativePath
                                        fv.FileShareId = dbFile.FileShareId
                                        fv.CreatedOnUTC = file.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime
                                        'fv.CheckInSource = Models.Enums.CheckInSource.Cloud
                                        fv.FileEntry = New FileEntryInfo() With {
                                            .FileEntryId = dbFile.FileEntryId,
                                            .FileEntryTypeId = dbFile.FileEntryTypeId,
                                            .IsDeleted = dbFile.IsDeleted,
                                            .FileShareId = dbFile.FileShareId
                                            }
                                        If (tobeProcessedFileHandles.Count = 1) Then
                                            fv.CreatedByUserId = tobeProcessedFileHandles(0).UserId
                                        End If

                                        'fileRep.AddVersionAndCheckIn(fv, tobeProcessedFileHandles)

                                        'For Each handle In tobeProcessedFileHandles
                                        '    fhrep.MarkProcessedFileHandles(handle.FileHandleId, dbFile.CheckOutWorkSpaceId)
                                        'Next
                                    Else
                                        Dim openFileHandles = fhrep.GetOpenFileHandles(dbFile.FileEntryId).Data
                                        Dim fv As New FileVersionInfo
                                        fv.FileEntryId = dbFile.FileEntryId
                                        fv.ParentFileEntryId = dbFile.FileVersion.ParentFileEntryId
                                        fv.PreviousFileVersionId = dbFile.FileVersion.FileVersionId
                                        fv.FileEntrySize = file.CloudFile.Properties.Length
                                        fv.FileEntryName = dbFile.FileVersion.FileEntryName
                                        fv.FileEntryExtension = dbFile.FileVersion.FileEntryExtension
                                        fv.FileEntryNameWithExtension = dbFile.FileVersion.FileEntryNameWithExtension
                                        fv.FileEntryRelativePath = dbFile.FileVersion.FileEntryRelativePath
                                        fv.FileShareId = dbFile.FileShareId
                                        fv.CreatedOnUTC = file.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime
                                        'fv.CheckInSource = Models.Enums.CheckInSource.Cloud
                                        fv.FileEntry = New FileEntryInfo() With {
                                            .FileEntryId = dbFile.FileEntryId,
                                            .FileEntryTypeId = dbFile.FileEntryTypeId,
                                            .IsDeleted = dbFile.IsDeleted,
                                            .FileShareId = dbFile.FileShareId
                                            }
                                        'If (openFileHandles IsNot Nothing AndAlso openFileHandles.Count > 0) Then
                                        '    If (openFileHandles.Count > 1 AndAlso dbFile.IsCheckedOut AndAlso file.CloudFile.Properties.LastModified.GetValueOrDefault().Subtract(dbFile.FileVersion.CreatedOnUTC).TotalMinutes > dbShare.Data.MultiUserVersionDiffMinutes) Then
                                        '        'create new version but remain checked out
                                        '        fileRep.AddVersionButRemainCheckedOut(fv, openFileHandles)
                                        '    ElseIf (openFileHandles.Count = 1 AndAlso dbFile.IsCheckedOut AndAlso DateTime.UtcNow.Subtract(file.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime).TotalMinutes > dbShare.Data.SingleUserVersionDiffMinutes AndAlso dbFile.CheckedOutByUserId = openFileHandles(0).UserId AndAlso dbFile.CheckOutWorkSpaceId = openFileHandles(0).WorkSpaceId) Then
                                        '        'create new version but remain check out
                                        '        fv.CreatedByUserId = openFileHandles(0).UserId
                                        '        fileRep.AddVersionButRemainCheckedOut(fv, openFileHandles)
                                        '    End If
                                        'Else
                                        '    fileRep.AddVersionAndCheckIn(fv, Nothing)
                                        'End If
                                    End If
                                Else
                                    'file not checked out but changed
                                    If (DateTime.UtcNow.Subtract(dbFile.FileVersion.CreatedOnUTC).TotalMinutes > dbShare.Data.VersionDiffMinutes) Then
                                        'checkout and checkin again
                                        'fileRep.Checkout(dbFile.FileEntryId)

                                        Dim fv As New FileVersionInfo
                                        fv.FileEntryId = dbFile.FileEntryId

                                        fv.ParentFileEntryId = dbFile.FileVersion.ParentFileEntryId
                                        fv.PreviousFileVersionId = dbFile.FileVersion.FileVersionId
                                        fv.FileEntrySize = file.CloudFile.Properties.Length
                                        fv.FileEntryName = dbFile.FileVersion.FileEntryName
                                        fv.FileEntryExtension = dbFile.FileVersion.FileEntryExtension
                                        fv.FileEntryNameWithExtension = dbFile.FileVersion.FileEntryNameWithExtension
                                        fv.FileEntryRelativePath = dbFile.FileVersion.FileEntryRelativePath
                                        fv.FileShareId = dbFile.FileShareId
                                        fv.CreatedOnUTC = file.CloudFile.Properties.LastModified.GetValueOrDefault().DateTime
                                        'fv.CheckInSource = Models.Enums.CheckInSource.Cloud
                                        fv.FileEntry = New FileEntryInfo() With {
                                            .FileEntryId = dbFile.FileEntryId,
                                            .FileEntryTypeId = dbFile.FileEntryTypeId,
                                            .IsDeleted = dbFile.IsDeleted,
                                            .FileShareId = dbFile.FileShareId
                                            }
                                        'fileRep.AddVersionAndCheckIn(fv, Nothing)
                                    End If
                                End If
                            End If

                            If (fileEntryId.HasValue) Then
                                'find conflict file if any and add to all conflicted files
                                Dim conflictedFiles = getAllConflictedFilesForFile(file, files, servers)
                                If (conflictedFiles.Count > 0) Then
                                    allConflictedFiles.AddRange(conflictedFiles)
                                    'fileRep.SetConflictFiles(fileEntryId, conflictedFiles.Select(Function(f) f.RelativePath).ToList())
                                End If
                            End If
                        End If
                    Next
                End If

                Dim configRepo As New ConfigRepository
                configRepo.User = caller
                configRepo.UpdateShareBlobSize(dbShare.Data.FileShareId)
            End If
            'End Using
        End If

    End Sub

    Private Sub enumerateFiles(ByVal dir As CloudFileDirectory, ByVal dirs As List(Of CloudDirExtension), ByVal files As List(Of CloudFileExtension), basePath As String)
        Dim fileEnum As IEnumerable(Of IListFileItem) = dir.ListFilesAndDirectories
        For Each file As IListFileItem In fileEnum
            If (file.GetType = GetType(CloudFileDirectory)) Then
                If (Not ignoreFolderList.Contains(CType(file, CloudFileDirectory).Name)) Then
                    dirs.Add(New CloudDirExtension With {.CloudDir = file,
                             .RelativePath = file.Uri.LocalPath.Replace(basePath, "").Replace("/", "\").Substring(1).ToLower()
                             })
                    Me.enumerateFiles(CType(file, CloudFileDirectory), dirs, files, basePath)
                End If
            Else
                files.Add(New CloudFileExtension With {.CloudFile = file,
                             .RelativePath = file.Uri.LocalPath.Replace(basePath, "").Replace("/", "\").Substring(1).ToLower()
                             })
            End If

        Next
    End Sub

    Private Function getAllConflictedFilesForFile(ByVal file As CloudFileExtension, ByVal files As List(Of CloudFileExtension), ByVal serverNames As List(Of String)) As List(Of CloudFileExtension)
        'todo: check further for servername <FileNameWithoutExtension>-<MachineName>[-#].<ext>
        Dim fileNameWithoutExtension As String = Me.getFileNameWithoutExtension(file.CloudFile.Name)
        Dim extension = Me.getFileExtension(file.CloudFile.Name)

        Dim initialFiltered = files.Where(Function(x As CloudFileExtension) (x.CloudFile.Parent.StorageUri = file.CloudFile.Parent.StorageUri _
                        AndAlso x.CloudFile.Name <> file.CloudFile.Name AndAlso x.CloudFile.Name.StartsWith(fileNameWithoutExtension + "-") AndAlso x.CloudFile.Name.EndsWith(extension))).ToList()
        If (initialFiltered.Count > 0) Then
            Dim servers = String.Join("|", serverNames)
            Dim regExpression = String.Format("^{0}(-({1})(-\d+)?)+{2}$", fileNameWithoutExtension, servers, extension)
            Dim regex As New Regex(regExpression)
            initialFiltered = initialFiltered.Where(Function(x) regex.IsMatch(x.CloudFile.Name)).ToList()
        End If

        Return initialFiltered
    End Function

    Private Function getFileExtension(ByVal name As String) As String
        Dim index As Integer = name.LastIndexOf(Microsoft.VisualBasic.ChrW(46))
        If (index >= 0) Then
            Return name.Substring((index))
        Else
            Return String.Empty
        End If

    End Function

    Private Function getFileNameWithoutExtension(ByVal name As String) As String
        Dim index As Integer = name.LastIndexOf(Microsoft.VisualBasic.ChrW(46))
        If (index >= 0) Then
            Return name.Substring(0, index)
        Else
            Return name
        End If

    End Function

End Class
