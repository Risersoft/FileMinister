Imports risersoft.shared.portable.Model
Imports System.IO
Imports log4net
Imports FileMinister.Client.Service.Repository
Imports risersoft.shared.messaging
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud

Public Class SyncRepository
    Inherits ClientRepositoryBase(Of LocalWorkSpaceInfo, Integer)
    Implements ISyncRepository

    'Private Shared ReadOnly localClientSyncLogger As ILog = LogManager.GetLogger(GetType(SyncRepository))
    Private Shared ReadOnly localClientSyncLogger As ILog = LogManager.GetLogger(name:="FileWatcherLogger")
    Public Function UpdateFileSystemCache(share As LocalConfigInfo, deltaSec As Integer) As ResultInfo(Of Boolean, Status) Implements ISyncRepository.UpdateFileSystemCache

        'Dim msg As New MessageInfo()
        'msg.MessageType = MessageType.Info
        'msg.Message = "File sync'ing started"
        'msg.User = Me.User
        'msg.Share = share
        'MessageClientProvider.Instance.BroadcastMessage(msg)




        'For folder
        'FileSystem	FileDb	FileDB.IsDel	Del.IsDel	View	Action
        '1.Exist	Exist	T		        T		    NE	    New Entry
        '2.Exist	Exist	F		        T		    NE	    New Entry
        '3.Exist	Exist	T		        F		    E	    Delete Folder in reverse order and if empty and mark Delta.isDeleted but not status
        '4.Exist	Exist	F		        F		    E	    ignore
        '5.Exist	NE					                NE	    New Entry
        '6.NE		Exist	T		        T		    NE	    Will not come
        '7.NE		Exist	F		        T		    NE	    Will not come
        '8.NE		Exist	T		        F		    E	    Set Delta.isDeleted
        '9.NE		Exist	F		        F		    E	    Set Delta.isDeleted and status = pending delete
        '10.NE		NE					                NE	    Will not come


        'For Files								
        'FileSystem	FileDB	FileDB.IsDel	Del.IsDel	View	Action
        '1.Exist	Exist	T		        T		    NE	    New Entry	
        '2.Exist	Exist	F		        T		    NE	    New Entry	
        '3.Exist	Exist	T		        F		    E	    match File.Lastwrite and Delta.timestamp	3a.Same: Delete File and mark Delta.isDeleted but not status
        '                                                                   						        3b.Not match: Create new file version and create Conflict
        '4.Exist	Exist	F		        F		    E	    Normal case as above but no conflict that is if local modified file then create new version but no conflict	
        '5.Exist	NE					                NE	    New Entry	
        '6.NE		Exist	T		        T		    NE	    Will not come	
        '7.NE		Exist	F		        T		    NE	    Will not come	
        '8.NE		Exist	T		        F		    E	    Set Delta.isDeleted	
        '9.NE		Exist	F		        F		    E	    Set Delta.isDeleted and status = pending delete	
        '10.NE		NE					                NE	    Will not come

        'Permission handling
        'if just read or no permission then delete all conflicts on that file for myself
        'FileSystem	View	Permission	Action
        'E	        E	    Null	    Non checked in version? Yes: Ignore or (change to ##, delete non version record and remove all delta record for that fileid)
        '                                                       No: Delete File and remove all Delta record for that file id
        'E	        E	    Read	    Ignore
        'E	        NE		            Check parent permission and if read or no permission, ignore
        'NE	        E	    Null	    delete null version record and Remove all delta record for that file id
        'NE	        E	    Read	    delete null version record and Remove all delta record for that file id
        'NE	        NE		            Will not come
        'All others continue
        Using service = GetClientEntity()

            'Dim domain As String = Nothing
            'Dim username As String = Nothing
            'Dim pwd As String = Nothing


            'If Not String.IsNullOrWhiteSpace(share.WindowsUser) Then
            '    Try
            '        Dim wuarr = share.WindowsUser.Split("\")
            '        domain = wuarr(0)
            '        username = wuarr(1)
            '        pwd = CommonUtils.Helper.Decrypt(share.Password)
            '    Catch ex As Exception
            '    End Try
            'End If


            'Using New Impersonator(wuarr(1), wuarr(0), CommonUtils.Helper.Decrypt(share.Password))


            Dim objShare = service.Shares.FirstOrDefault(Function(p) p.ShareId = share.FileShareId)
            If objShare Is Nothing Then
                Dim rInfo As New ResultInfo(Of Boolean, Status)
                rInfo.Status = Status.PathNotValid
                rInfo.Data = False
                Return rInfo
            ElseIf (objShare IsNot Nothing AndAlso objShare.IsDeleted) Then
                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.Tags.RemoveRange(service.Tags.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.GroupFileSystemEntryPermissionAssignments.RemoveRange(service.GroupFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.UserFileSystemEntryPermissionAssignments.RemoveRange(service.UserFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.FileSystemEntries.RemoveRange(service.FileSystemEntries.Where(Function(p) p.ShareId = share.FileShareId))
                service.Shares.Remove(objShare)
                service.SaveChanges()
                Using commonService = GetClientCommonEntity()
                    commonService.UserShares.RemoveRange(commonService.UserShares.Where(Function(p) p.ShareId = share.FileShareId AndAlso p.UserAccount.AccountId = Me.User.AccountId))
                    Dim agents = commonService.UserAgents.Where(Function(p) p.UserAccount.AccountId = Me.User.AccountId).Select(Function(p) p.AgentId).ToList
                    commonService.AgentShares.RemoveRange(commonService.AgentShares.Where(Function(p) p.ShareId = share.FileShareId AndAlso agents.Contains(p.AgentId)))
                    commonService.AccountShares.RemoveRange(commonService.AccountShares.Where(Function(p) p.ShareId = share.FileShareId AndAlso p.AccountId = Me.User.AccountId))
                    commonService.SaveChanges()
                End Using
                Dim rInfo As New ResultInfo(Of Boolean, Status)
                rInfo.Status = Status.PathNotValid
                rInfo.Data = False
                Return rInfo
            End If
            Dim shareRemoved = False
            Using CommonUtils.Helper.Impersonate(share)
                Dim d As New DirectoryInfo(share.SharePath)
                If Not d.Exists Then
                    shareRemoved = True
                End If
            End Using
            If shareRemoved Then
                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId))
                service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId AndAlso Not p.IsResolved))
                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntry.ShareId = share.FileShareId AndAlso p.VersionNumber Is Nothing))
                'service.Tags.RemoveRange(service.Tags.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.GroupFileSystemEntryPermissionAssignments.RemoveRange(service.GroupFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.UserFileSystemEntryPermissionAssignments.RemoveRange(service.UserFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntry.ShareId = share.ShareId))
                'service.FileSystemEntries.RemoveRange(service.FileSystemEntries.Where(Function(p) p.ShareId = share.ShareId))
                'service.Shares.Remove(objShare)
                service.SaveChanges()
                Using commonService = GetClientCommonEntity()
                    commonService.UserShares.RemoveRange(commonService.UserShares.Where(Function(p) p.ShareId = share.FileShareId AndAlso p.UserAccountId = Me.User.UserAccountId))
                    commonService.SaveChanges()
                End Using
                Dim rInfo As New ResultInfo(Of Boolean, Status)
                rInfo.Status = Status.PathNotValid
                rInfo.Data = False
                Return rInfo
            End If

            localClientSyncLogger.Info(String.Format("Sync'ing share '{0}' for user '{1}' and userAccount '{2}'", share.ShareName, share.User.UserId, share.UserAccountId))

            Dim shareTypeId = CType(FileType.Share, Short)
            Dim shareFileSystemEntryId = Nothing
            Dim shareFile = service.FileSystemEntries.FirstOrDefault(Function(p) p.ShareId = share.FileShareId AndAlso p.FileSystemEntryTypeId = shareTypeId)
            If shareFile IsNot Nothing Then
                shareFileSystemEntryId = shareFile.FileSystemEntryId
            End If

            Dim writeTypeId As Integer = PermissionType.Write

            'new folder in file system and thus db creation process
            Dim fileTypeDir = CType(FileType.Folder, Byte)
            Dim fileTypeFile = CType(FileType.File, Byte)


            Dim filterFoldersList = service.RelativePathsExcludedFromSyncs.Where(Function(p) p.ShareId = share.FileShareId).Select(Function(p) p.RelativePath).ToList()
            Dim filterFolders = filterFoldersList.Select(Function(p) Path.Combine(share.SharePath, p)).ToList()

            Dim permanentDeletedFiles = service.FileSystemEntries.Where(Function(p) p.ShareId = share.FileShareId AndAlso p.IsPermanentlyDeleted AndAlso p.FileSystemEntryTypeId = fileTypeFile AndAlso p.DeltaOperations.Any() AndAlso p.DeltaOperations.Count > 0).ToList()
            Dim fileRepository As New FileRepository
            fileRepository.User = Me.User
            For Each permanentDeletedFile In permanentDeletedFiles
                'Dim physicalFilePathRemove As New List(Of String)
                Dim latestVersions = permanentDeletedFile.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = permanentDeletedFile.FileSystemEntryId).OrderByDescending(Function(p) If(p.VersionNumber.HasValue, p.VersionNumber, 99999999)).ToList()

                If (filterFolders.Count > 0 AndAlso latestVersions IsNot Nothing AndAlso latestVersions.Count > 0) Then
                    Dim found = False
                    For Each filterFolder In filterFolders
                        If (latestVersions(0).FileSystemEntryRelativePath.StartsWith(filterFolder)) Then
                            found = True
                            Exit For
                        End If
                    Next
                    If found Then
                        Continue For
                    End If
                End If

                Using CommonUtils.Helper.Impersonate(share)
                    If (latestVersions IsNot Nothing AndAlso latestVersions.Count > 0) Then
                        For Each latestversion In latestVersions
                            If latestversion.DeltaOperations IsNot Nothing AndAlso latestversion.DeltaOperations.Count > 0 AndAlso Not String.IsNullOrWhiteSpace(latestversion.DeltaOperations(0).LocalFileSystemEntryName) Then
                                If File.Exists(latestversion.DeltaOperations(0).LocalFileSystemEntryName) Then
                                    Try
                                        File.Delete(latestversion.DeltaOperations(0).LocalFileSystemEntryName)
                                    Catch ex As Exception
                                        localClientSyncLogger.Info("Unable to delete file " + latestversion.DeltaOperations(0).LocalFileSystemEntryName, ex)
                                    End Try
                                End If
                                Exit For
                            End If
                        Next
                    End If
                End Using
                Dim conflictedDeltas = service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = permanentDeletedFile.FileSystemEntryId AndAlso p.IsConflicted).ToList()
                Using CommonUtils.Helper.Impersonate(share)
                    For Each conflictedDelta In conflictedDeltas
                        If (Not String.IsNullOrWhiteSpace(conflictedDelta.LocalFileSystemEntryName) AndAlso File.Exists(conflictedDelta.LocalFileSystemEntryName)) Then
                            Try
                                File.Delete(conflictedDelta.LocalFileSystemEntryName)
                            Catch ex As Exception
                                localClientSyncLogger.Info("Unable to delete file " + conflictedDelta.LocalFileSystemEntryName, ex)
                            End Try
                        End If
                    Next
                End Using

                'fileRepository.DeleteFile(permanentDeletedFile.FileSystemEntryId)
                fileRepository.DeleteDeltaOperationsForFile(permanentDeletedFile.FileSystemEntryId)
            Next

            Dim permanentDeletedFolders = service.FileSystemEntries.Where(Function(p) p.ShareId = share.FileShareId AndAlso p.IsPermanentlyDeleted AndAlso p.FileSystemEntryTypeId = fileTypeDir AndAlso p.DeltaOperations.Any() AndAlso p.DeltaOperations.Count > 0).ToList()
            For Each permanentDeletedFolder In permanentDeletedFolders
                'Dim physicalFilePathRemove As New List(Of String)
                Dim latestVersion = permanentDeletedFolder.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = permanentDeletedFolder.FileSystemEntryId).OrderByDescending(Function(p) If(p.VersionNumber.HasValue, p.VersionNumber, 99999999)).FirstOrDefault()
                Using CommonUtils.Helper.Impersonate(share)
                    If (latestVersion IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(latestVersion.FileSystemEntryRelativePath) AndAlso Directory.Exists(share.SharePath + "\" + latestVersion.FileSystemEntryRelativePath)) Then
                        Directory.Delete(share.SharePath + "\" + latestVersion.FileSystemEntryRelativePath, True)
                    End If
                End Using

                'fileRepository.DeleteFile(permanentDeletedFolder.FileSystemEntryId)
                fileRepository.DeleteDeltaOperationsForFile(permanentDeletedFolder.FileSystemEntryId)
            Next


            Dim dirs As DirectoryInfo()
            Dim files As IO.FileInfo()

            Dim view = service.GetAllLatestFileVersionsWithClientPresenceForShare(share.FileShareId, True, Me.User.UserId)

            Using CommonUtils.Helper.Impersonate(share)
                Dim d As New DirectoryInfo(share.SharePath)
                'If Not d.Exists Then
                '    Dim rInfo As New ResultInfo(Of Boolean)
                '    rInfo.Status = Status.PathNotValid
                '    rInfo.Data = False
                '    Return rInfo
                'End If

                dirs = d.GetDirectories("*", SearchOption.AllDirectories).ToList().Where(Function(p) filterFolders.All(Function(e) Not p.FullName.StartsWith(e))).ToArray()

                files = d.GetFiles("*", SearchOption.AllDirectories).ToList().Where(Function(p) filterFolders.All(Function(e) Not p.FullName.StartsWith(e))).ToArray()
            End Using

            Dim newQuery = From dir In dirs
                           Group Join v In view.Where(Function(p) p.FileSystemEntryTypeId = fileTypeDir).ToList()
                       On dir.FullName.ToLower() Equals (share.SharePath + "\" + v.FileSystemEntryRelativePath).ToLower() Into Group
                           From v In Group.DefaultIfEmpty()
                           Where v Is Nothing
                           Select dir
            For Each dd As DirectoryInfo In newQuery
                If (dd.Name.StartsWith("##") OrElse dd.FullName.Contains("\##")) Then
                    Continue For
                End If
                Try
                    Dim relativePath = dd.FullName.Remove(0, share.SharePath.Length + 1)
                    Dim parentRelativePath = Helper.GetParentRelativePath(relativePath)
                    Dim parentFileSystemEntryId As Guid
                    If parentRelativePath <> String.Empty Then
                        Dim parentFileSystemLatestVersion = service.GetLatestFileSystemEntryVersionByPath(parentRelativePath, share.FileShareId).FirstOrDefault()
                        If (parentFileSystemLatestVersion Is Nothing) Then
                            Continue For
                        End If
                        parentFileSystemEntryId = parentFileSystemLatestVersion.FileSystemEntryId
                    Else
                        parentFileSystemEntryId = shareFileSystemEntryId
                    End If

                    Dim obj As New UserRepository
                    obj.User = Me.User
                    If (obj.GetEffectivePermissionsOnFile(parentFileSystemEntryId, True).Data And writeTypeId) = 0 Then
                        Continue For
                    End If

                    Dim serverRecord = service.GetLatestFileSystemEntryVersionByPathAndClientPresenceFlag(relativePath, share.FileShareId, False).ToList()
                    'to handle special case that server file is moved but the flag is not updated
                    If (Not (serverRecord.Count > 0)) Then
                        Dim fileSystemEntry As New FileSystemEntry
                        With fileSystemEntry
                            .CurrentVersionNumber = 0
                            .FileSystemEntryId = Guid.NewGuid
                            .FileSystemEntryTypeId = FileType.Folder.ToString("D")
                            .ShareId = share.FileShareId
                        End With

                        Dim fileSystemEntryVersion As New FileSystemEntryVersion
                        fileSystemEntry.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                        With fileSystemEntryVersion
                            .CreatedByUserId = Me.User.UserId
                            .CreatedOnUTC = dd.CreationTimeUtc
                            .FileSystemEntryName = dd.Name
                            .FileSystemEntryRelativePath = relativePath
                            .FileSystemEntrySize = 0
                            .FileSystemEntryVersionId = Guid.NewGuid
                            .ParentFileSystemEntryId = parentFileSystemEntryId
                            .FileSystemEntryHash = 0
                        End With

                        Dim delta As New DeltaOperation
                        fileSystemEntry.DeltaOperations.Add(delta)
                        With delta
                            .DeltaOperationId = Guid.NewGuid
                            .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)
                            .FileSystemEntryVersion = fileSystemEntryVersion
                            .LocalCreatedOnUTC = dd.CreationTimeUtc
                            .LocalFileSystemEntryName = dd.FullName
                            .FileSystemEntry = fileSystemEntry
                        End With
                        service.FileSystemEntries.Add(fileSystemEntry)
                        service.SaveChanges()
                    End If

                Catch ex As Exception
                    localClientSyncLogger.Warn("Error in creating record in db for " + dd.FullName, ex)
                End Try
            Next

            'remove all null version records and my conflicts for file where I do not have write permission


            Dim arrIdsOfNonWritePermissionFiles = view.Where(Function(p) p.PermissionBitValue Is Nothing OrElse (p.PermissionBitValue And writeTypeId) = 0).Select(Function(p) p.FileSystemEntryId).ToList()
            localClientSyncLogger.Info("files with no permissions " + arrIdsOfNonWritePermissionFiles.Count.ToString() + arrIdsOfNonWritePermissionFiles.ToString())
            'localClientSyncLogger.Info("Deleteing versions for file due to permission ")
            service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId AndAlso arrIdsOfNonWritePermissionFiles.Contains(p.FileSystemEntryId)))
            For Each delta In service.DeltaOperations.Where(Function(p) arrIdsOfNonWritePermissionFiles.Contains(p.FileSystemEntryId) AndAlso p.IsConflicted)
                delta.IsConflicted = False
            Next
            service.DeltaOperations.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) arrIdsOfNonWritePermissionFiles.Contains(p.FileSystemEntryId) AndAlso p.VersionNumber Is Nothing).SelectMany(Function(p) p.DeltaOperations))
            service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) arrIdsOfNonWritePermissionFiles.Contains(p.FileSystemEntryId) AndAlso p.VersionNumber Is Nothing))
            service.SaveChanges()

            ' handle files to be marked as deleted

            Dim removeFileQuery = From v In view.Where(Function(p) p.FileSystemEntryTypeId = fileTypeFile).ToList()
                                  Group Join file In files
                    On file.FullName.ToLower() Equals (share.SharePath + "\" + v.FileSystemEntryRelativePath).ToLower() Into Group
                                  From file In Group.DefaultIfEmpty()
                                  Where file Is Nothing
                                  Select v

            Dim filesToBeDeleted = removeFileQuery.ToList()
            Dim countDeletedFile = filesToBeDeleted.Count
            For i As Integer = countDeletedFile - 1 To 0 Step -1
                Dim deletedFileVersionInfo = filesToBeDeleted(i)
                If deletedFileVersionInfo.PermissionBitValue Is Nothing OrElse (deletedFileVersionInfo.PermissionBitValue And PermissionType.Write) = 0 Then
                    service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = deletedFileVersionInfo.FileSystemEntryId))
                    'alreday done above
                    'service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = deletedFileVersionInfo.FileSystemEntryId AndAlso p.VersionNumber Is Nothing))
                    filesToBeDeleted.RemoveAt(i)
                End If
            Next
            service.SaveChanges()

            'For Each deletedFileVersionInfo In filesToBeDeleted
            '    If deletedFileVersionInfo.PermissionBitValue Is Nothing OrElse deletedFileVersionInfo.PermissionBitValue = PermissionType.Read Then
            '        service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = deletedFileVersionInfo.FileSystemEntryId))
            '        service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = deletedFileVersionInfo.FileSystemEntryId AndAlso p.VersionNumber Is Nothing))
            '    End If
            'Next

            Dim filesToBeDeletedIdArr = filesToBeDeleted.Where(Function(p) Not p.IsDeleted).Select(Function(p) p.FileSystemEntryVersionId).ToList()
            Dim fileDeletedAtBothPlacesIdArr = filesToBeDeleted.Where(Function(p) p.IsDeleted).Select(Function(p) p.FileSystemEntryId).ToList()

            Dim openFiles = OpenHandle.GetOpenFiles(share.SharePath)

            For Each file As IO.FileInfo In files
                Try
                    If (Not CommonUtils.Helper.IsValidFile(file.FullName)) OrElse (file.FullName.Contains("\##")) Then
                        Continue For
                    End If
                    Using CommonUtils.Helper.Impersonate(share)
                        If (Not System.IO.File.Exists(file.FullName)) Then
                            'file has been deleted by now so no new version
                            Continue For
                        End If
                    End Using
                    'find file version record in view by path
                    Dim isOpen = False
                    If (openFiles.Contains(file.FullName.ToLower())) Then
                        isOpen = True
                    End If
                    Dim relativePath = file.FullName.Remove(0, share.SharePath.Length + 1)
                    Dim fileSystemEntryVersionInfo = view.FirstOrDefault(Function(p) p.FileSystemEntryRelativePath.ToLower() = relativePath.ToLower())
                    If fileSystemEntryVersionInfo IsNot Nothing Then
                        'If (fileSystemEntryVersionInfo.IsPermanentlyDeleted) Then
                        '    Using CommonUtils.Helper.Impersonate(share)
                        '        If (file.Exists) Then
                        '            file.Delete()
                        '        End If
                        '    End Using
                        '    Dim fileRepository As New FileRepository
                        '    fileRepository.DeleteFile(fileSystemEntryVersionInfo.FileSystemEntryId)
                        '    Continue For
                        'End If
                        If (fileSystemEntryVersionInfo.PermissionBitValue Is Nothing OrElse fileSystemEntryVersionInfo.PermissionBitValue = 0) Then
                            If (Not fileSystemEntryVersionInfo.VersionNumber.HasValue OrElse fileSystemEntryVersionInfo.VersionNumber = 0) Then
                                Continue For
                            Else
                                Using CommonUtils.Helper.Impersonate(share)
                                    If (file.Exists) Then
                                        file.Delete()
                                    End If
                                End Using
                                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId))
                                Continue For
                            End If
                        ElseIf (fileSystemEntryVersionInfo.PermissionBitValue.Value And PermissionType.Write) = 0 Then 'OrElse fileSystemEntryVersionInfo.isPermanentlyDeleted Then
                            Continue For
                        End If

                        'if found, compare the Delta.timechange, if diff and delta check pass, check hash. 
                        'If diff, create new version else update localchange time in Delta table for that version
                        If (file.Length = 0 AndAlso fileSystemEntryVersionInfo.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.TempCreated, Byte)) Then
                            Continue For
                        End If

                        If fileSystemEntryVersionInfo.IsDeleted OrElse ((fileSystemEntryVersionInfo.LocalCreatedOnUTC Is Nothing OrElse Math.Abs(fileSystemEntryVersionInfo.LocalCreatedOnUTC.GetValueOrDefault().Subtract(file.LastWriteTimeUtc).TotalMilliseconds) > 1000) AndAlso
                        (file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow)) Then

                            Dim hash As String
                            If (fileSystemEntryVersionInfo.LocalCreatedOnUTC Is Nothing OrElse Math.Abs(fileSystemEntryVersionInfo.LocalCreatedOnUTC.GetValueOrDefault().Subtract(file.LastWriteTimeUtc).TotalMilliseconds) > 1000) Then
                                Using CommonUtils.Helper.Impersonate(share)
                                    hash = HashCalculator.hash_generator("md5", file.FullName)
                                End Using
                            Else
                                hash = fileSystemEntryVersionInfo.FileSystemEntryHash
                            End If
                            If (hash IsNot Nothing AndAlso hash = fileSystemEntryVersionInfo.FileSystemEntryHash AndAlso Not (fileSystemEntryVersionInfo.VersionNumber Is Nothing OrElse fileSystemEntryVersionInfo.VersionNumber = 0)) Then
                                If (fileSystemEntryVersionInfo.IsDeleted AndAlso Not fileSystemEntryVersionInfo.IsConflicted) Then
                                    'Delete file but do not mark status
                                    localClientSyncLogger.Info("Delete file without status for " + file.FullName)
                                    Using CommonUtils.Helper.Impersonate(share)
                                        If (file.Exists) Then
                                            file.Delete()
                                        End If
                                    End Using
                                    service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId))
                                    'Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    'deltaOperation.IsDeleted = True
                                    'deltaOperation.IsOpen = False
                                    'deltaOperation.DeletedByUserId = Me.User.Id
                                    'deltaOperation.DeletedOnUTC = Date.UtcNow
                                Else
                                    'update delta local time
                                    localClientSyncLogger.Info("Update file local  as no change but saved for " + file.FullName)
                                    Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    deltaOperation.LocalCreatedOnUTC = file.LastWriteTimeUtc
                                    deltaOperation.IsOpen = isOpen
                                    If (Not isOpen AndAlso deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte) AndAlso file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow AndAlso Not deltaOperation.IsConflicted) Then
                                        deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.PendingUpload, Byte)
                                    End If
                                End If
                                service.SaveChanges()
                            Else
                                'If ((fileSystemEntryVersionInfo.VersionNumber Is Nothing OrElse fileSystemEntryVersionInfo.VersionNumber = 0) AndAlso fileSystemEntryVersionInfo.IsDeleted AndAlso Not fileSystemEntryVersionInfo.IsConflicted) Then
                                If ((fileSystemEntryVersionInfo.VersionNumber Is Nothing OrElse fileSystemEntryVersionInfo.VersionNumber = 0) AndAlso fileSystemEntryVersionInfo.IsDeleted) Then
                                    'conflict
                                    'update exisiting version
                                    Dim fileVersionInfo = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    With fileVersionInfo
                                        .CreatedOnUTC = file.LastWriteTimeUtc
                                        .FileSystemEntrySize = file.Length
                                        .FileSystemEntryHash = hash
                                    End With

                                    'update delta local time
                                    Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    deltaOperation.LocalCreatedOnUTC = file.LastWriteTimeUtc
                                    deltaOperation.IsOpen = isOpen
                                    'reset the status to new modified
                                    If (Not fileSystemEntryVersionInfo.IsConflicted) Then
                                        deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)
                                    End If
                                    Dim createConflict = False
                                    Dim conflicts = service.FileSystemEntryVersionConflicts.OrderByDescending(Function(p) p.CreatedOnUTC).Where(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId AndAlso p.IsResolved = False).ToList()
                                    If (conflicts IsNot Nothing AndAlso conflicts.Count > 0) Then
                                        If Not (conflicts(0).FileSystemEntryVersionConflictTypeId = CByte(Enums.FileVersionConflictType.ServerDelete)) Then
                                            For Each conflict In conflicts
                                                conflict.IsResolved = True
                                                conflict.ResolvedByUserId = Me.User.UserId
                                                conflict.ResolvedOnUTC = DateTime.UtcNow
                                                conflict.ResolvedType = Enums.Constants.NEW_CONFLICT_RESOLVETYPE
                                            Next
                                            createConflict = True
                                        End If
                                    ElseIf fileSystemEntryVersionInfo.IsDeleted AndAlso Not deltaOperation.IsConflicted Then
                                        createConflict = True
                                    End If

                                    If createConflict Then
                                        'create conflict
                                        localClientSyncLogger.Warn("File Conflict of type server Delete conflict for " + file.FullName)
                                        deltaOperation.IsConflicted = True

                                        'remove any pending checkin other than this
                                        Dim versionsPendingCheckin = service.FileSystemEntryVersions.OrderBy(Function(p) p.CreatedOnUTC).Where(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId AndAlso (p.VersionNumber Is Nothing OrElse p.VersionNumber = 0))
                                        If (versionsPendingCheckin.Count > 1) Then
                                            fileVersionInfo.PreviousFileSystemEntryVersionId = versionsPendingCheckin(0).PreviousFileSystemEntryVersionId
                                            Dim versionsToBeDeleted As New List(Of FileSystemEntryVersion)
                                            For Each versionPendingCheckin In versionsPendingCheckin
                                                'delete file from temp if exist
                                                If (versionPendingCheckin.FileSystemEntryVersionId = deltaOperation.FileSystemEntryVersionId) Then
                                                Else
                                                    service.DeltaOperations.Remove(service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = versionPendingCheckin.FileSystemEntryVersionId))
                                                    versionsToBeDeleted.Add(versionPendingCheckin)
                                                End If
                                            Next
                                            'delete file version
                                            localClientSyncLogger.Info("~~~Deleteing versions for file " + file.FullName)
                                            service.FileSystemEntryVersions.RemoveRange(versionsToBeDeleted)
                                        End If

                                        Dim fileSystemEntryVersionConflict As New FileSystemEntryVersionConflict
                                        fileSystemEntryVersionConflict.CreatedOnUTC = DateTime.UtcNow
                                        fileSystemEntryVersionConflict.FileSystemEntryId = fileVersionInfo.FileSystemEntryId
                                        fileSystemEntryVersionConflict.FileSystemEntryNameAndExtension = file.Name
                                        fileSystemEntryVersionConflict.FileSystemEntryPath = file.FullName
                                        Dim fsevLatest = service.FileSystemEntryVersions.OrderByDescending(Function(p) p.VersionNumber).FirstOrDefault(Function(p) p.FileSystemEntryId = fileVersionInfo.FileSystemEntryId).FileSystemEntryVersionId
                                        fileSystemEntryVersionConflict.FileSystemEntryVersionId = fsevLatest

                                        fileSystemEntryVersionConflict.FileSystemEntryVersionConflictId = Guid.NewGuid
                                        fileSystemEntryVersionConflict.IsResolved = False
                                        fileSystemEntryVersionConflict.UserId = Me.User.UserId
                                        fileSystemEntryVersionConflict.WorkSpaceId = Me.User.WorkSpaceId
                                        fileSystemEntryVersionConflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ServerDelete
                                        service.FileSystemEntryVersionConflicts.Add(fileSystemEntryVersionConflict)

                                        Dim message As New MessageInfo()
                                        message.MessageType = MessageType.Action
                                        message.ActionType = ActionType.Conflict
                                        message.User = Me.User
                                        message.Share = share
                                        message.Data.Add("FileSystemEntryId", fileVersionInfo.FileSystemEntryId)
                                        MessageClientProvider.Instance.BroadcastMessage(message)

                                    End If
                                ElseIf (fileSystemEntryVersionInfo.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)) Then
                                    'update exisiting version
                                    localClientSyncLogger.Info("modified but changed existing version for " + file.FullName)
                                    Dim fileVersionInfo = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    With fileVersionInfo
                                        .CreatedOnUTC = file.LastWriteTimeUtc
                                        .FileSystemEntrySize = file.Length
                                        .FileSystemEntryHash = hash
                                    End With

                                    'update delta local time
                                    Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                    deltaOperation.LocalCreatedOnUTC = file.LastWriteTimeUtc
                                    deltaOperation.IsOpen = isOpen
                                    If (Not isOpen AndAlso deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte) AndAlso file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow AndAlso Not deltaOperation.IsConflicted) Then
                                        deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.PendingUpload, Byte)
                                    End If
                                Else
                                    'if file has come from server but no client presence due to not downloaded
                                    Dim serverRecord = service.GetLatestFileSystemEntryVersionByPathAndClientPresenceFlag(relativePath, share.FileShareId, False).ToList()
                                    'to handle special case that server file is moved but the flag is not updated
                                    If (Not (serverRecord.Count > 0 AndAlso serverRecord(0).FileSystemEntryHash = hash)) Then
                                        If (fileSystemEntryVersionInfo.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.MoveOrRename, Byte)) Then
                                            Continue For
                                        End If

                                        'create new version
                                        localClientSyncLogger.Info("modified and so creating new version for " + file.FullName)
                                        Dim fileSystemEntryVersion As New FileSystemEntryVersion
                                        With fileSystemEntryVersion
                                            .CreatedByUserId = Me.User.UserId
                                            .CreatedOnUTC = file.LastWriteTimeUtc
                                            .FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId
                                            .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(file.Name)
                                            .FileSystemEntryExtension = IO.Path.GetExtension(file.Name)
                                            .FileSystemEntryRelativePath = relativePath
                                            .FileSystemEntrySize = file.Length
                                            .FileSystemEntryVersionId = Guid.NewGuid
                                            .ParentFileSystemEntryId = fileSystemEntryVersionInfo.ParentFileSystemEntryId
                                            .PreviousFileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId
                                            .FileSystemEntryHash = hash
                                            .ServerFileSystemEntryName = .FileSystemEntryVersionId
                                        End With

                                        Dim delta As New DeltaOperation
                                        fileSystemEntryVersion.DeltaOperations.Add(delta)
                                        With delta
                                            .DeltaOperationId = Guid.NewGuid
                                            If (Not isOpen AndAlso .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte) AndAlso file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow) Then
                                                .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.PendingUpload, Byte)
                                            Else
                                                .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)
                                            End If

                                            .FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId
                                            .LocalCreatedOnUTC = file.LastWriteTimeUtc
                                            .LocalFileSystemEntryName = file.FullName
                                            .IsOpen = isOpen
                                        End With
                                        service.FileSystemEntryVersions.Add(fileSystemEntryVersion)

                                        Dim conflicts = service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId AndAlso p.IsResolved = False).ToList()
                                        For Each conflict In conflicts
                                            conflict.IsResolved = True
                                            conflict.ResolvedByUserId = Me.User.UserId
                                            conflict.ResolvedOnUTC = DateTime.UtcNow
                                            conflict.ResolvedType = Enums.Constants.NEW_CONFLICT_RESOLVETYPE
                                        Next

                                        If (fileSystemEntryVersionInfo.IsDeleted) Then
                                            'create conflict
                                            delta.IsConflicted = True
                                            Dim fileSystemEntryVersionConflict As New FileSystemEntryVersionConflict
                                            fileSystemEntryVersionConflict.CreatedOnUTC = DateTime.UtcNow
                                            fileSystemEntryVersionConflict.FileSystemEntryNameAndExtension = file.Name
                                            fileSystemEntryVersionConflict.FileSystemEntryPath = file.FullName
                                            fileSystemEntryVersionConflict.FileSystemEntryId = fileSystemEntryVersion.FileSystemEntryId
                                            Dim fsevLatest = service.FileSystemEntryVersions.OrderByDescending(Function(p) p.VersionNumber).FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId).FileSystemEntryVersionId
                                            fileSystemEntryVersionConflict.FileSystemEntryVersionId = fsevLatest
                                            fileSystemEntryVersionConflict.FileSystemEntryVersionConflictId = Guid.NewGuid
                                            fileSystemEntryVersionConflict.IsResolved = False
                                            fileSystemEntryVersionConflict.UserId = Me.User.UserId
                                            fileSystemEntryVersionConflict.WorkSpaceId = Me.User.WorkSpaceId
                                            fileSystemEntryVersionConflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ServerDelete
                                            service.FileSystemEntryVersionConflicts.Add(fileSystemEntryVersionConflict)

                                            Dim message As New MessageInfo()
                                            message.MessageType = MessageType.Action
                                            message.ActionType = ActionType.Conflict
                                            message.User = Me.User
                                            message.Share = share
                                            message.Data.Add("FileSystemEntryId", fileSystemEntryVersion.FileSystemEntryId)
                                            MessageClientProvider.Instance.BroadcastMessage(message)
                                        End If
                                    End If
                                End If
                                service.SaveChanges()
                            End If
                        Else
                            Dim deltaOperation As DeltaOperation = Nothing
                            If (isOpen AndAlso Not fileSystemEntryVersionInfo.IsOpen) OrElse
                        (Not isOpen AndAlso fileSystemEntryVersionInfo.IsOpen) Then
                                deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                deltaOperation.IsOpen = isOpen
                            End If
                            'file should not be open, nor conflicted, status in NewModified, is checkedout by me and time has elapsed, then only we should upload
                            If (Not isOpen AndAlso fileSystemEntryVersionInfo.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte) AndAlso file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow) Then
                                If deltaOperation Is Nothing Then
                                    deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId)
                                End If
                                If (Not deltaOperation.IsConflicted) Then
                                    Dim fse = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryVersionInfo.FileSystemEntryId)
                                    If (fse.CheckedOutByUserId = Me.User.UserId) Then
                                        localClientSyncLogger.Info("changed status to pending upload for " + file.FullName)
                                        deltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.PendingUpload, Byte)
                                    End If
                                End If
                            End If
                            service.SaveChanges()
                        End If
                    Else
                        'if not found, that means new file. Try to find a file that is marked as to be deleted in this run and compare hash. 
                        'if found, create new fileversion or should it be create new link with new file???
                        'if to be deleted file with same hash not found, check delta pass. if it pass, create new file.
                        'Using CommonUtils.Helper.Impersonate(share)
                        '    If (Not System.IO.File.Exists(file.FullName)) Then
                        '        'file has been deleted by now so no new version
                        '        Continue For
                        '    End If
                        'End Using
                        Dim parentRelativePath = Helper.GetParentRelativePath(relativePath)
                        Dim parentFileSystemEntryId As Guid
                        Dim parentFileSystemLatestVersion = view.FirstOrDefault(Function(p) p.FileSystemEntryRelativePath.ToLower() = relativePath.ToLower())
                        Dim parentPermission As Byte?
                        If (parentFileSystemLatestVersion Is Nothing) Then
                            If parentRelativePath <> String.Empty Then
                                Dim parentFileSystemLatestVersionInfo = service.GetLatestFileSystemEntryVersionByPath(parentRelativePath, share.FileShareId).FirstOrDefault()
                                parentFileSystemEntryId = parentFileSystemLatestVersionInfo.FileSystemEntryId
                            Else
                                parentFileSystemEntryId = shareFileSystemEntryId
                            End If
                            parentPermission = GetEffectivePermission(service, Me.User, parentFileSystemEntryId)
                        Else
                            parentPermission = parentFileSystemLatestVersion.PermissionBitValue
                            parentFileSystemEntryId = parentFileSystemLatestVersion.FileSystemEntryId
                        End If

                        If (parentPermission Is Nothing OrElse (parentPermission And PermissionType.Write) = 0) Then
                            Continue For
                        End If

                        Dim hash As String
                        Using CommonUtils.Helper.Impersonate(share)
                            hash = HashCalculator.hash_generator("md5", file.FullName)
                        End Using

                        'if file has come from server but no client presence due to not downloaded
                        Dim serverRecord = service.GetLatestFileSystemEntryVersionByPathAndClientPresenceFlag(relativePath, share.FileShareId, False).ToList()
                        If serverRecord.Count > 0 Then
                            'If (file.Length > 0 AndAlso Not serverRecord(0).IsConflicted) Then
                            '    Continue For
                            'End If

                            'If (hash <> serverRecord(0).FileSystemEntryHash AndAlso serverRecord(0).IsConflicted) Then
                            '    Dim newguid = Guid.NewGuid
                            '    Dim fileSystemEntryVersion As New FileSystemEntryVersion
                            '    With fileSystemEntryVersion
                            '        .CreatedByUserId = Me.User.Id
                            '        .CreatedOnUTC = file.CreationTimeUtc
                            '        .FileSystemEntryId = serverRecord(0).FileSystemEntryId
                            '        .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(file.Name)
                            '        .FileSystemEntryExtension = IO.Path.GetExtension(file.Name)
                            '        .FileSystemEntryRelativePath = relativePath
                            '        .FileSystemEntrySize = file.Length
                            '        .FileSystemEntryVersionId = newguid
                            '        .ParentFileSystemEntryId = parentFileSystemEntryId
                            '        .FileSystemEntryHash = hash
                            '        .ServerFileSystemEntryName = newguid
                            '    End With

                            '    Dim delta As New DeltaOperation
                            '    fileSystemEntryVersion.DeltaOperations.Add(delta)
                            '    With delta
                            '        .DeltaOperationId = Guid.NewGuid
                            '        .FileSystemEntryStatusId = CType(Enums.FileSystemEntryStatus.NewModified, Byte)
                            '        '.FileSystemEntryVersion = fileSystemEntryVersion
                            '        .LocalCreatedOnUTC = file.LastWriteTimeUtc
                            '        .LocalFileSystemEntryName = file.FullName
                            '        .IsOpen = isOpen
                            '        .FileSystemEntryId = serverRecord(0).FileSystemEntryId
                            '        .IsConflicted = True
                            '    End With
                            '    service.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                            '    service.SaveChanges()
                            'End If
                            Continue For
                        End If


                        Dim existingFile = filesToBeDeleted.FirstOrDefault(Function(p) p.FileSystemEntrySize > 0 AndAlso p.FileSystemEntrySize = file.Length AndAlso p.FileSystemEntryHash = hash)
                        If (existingFile IsNot Nothing AndAlso existingFile.VersionNumber > 0) Then
                            'found existing deleted file
                            Dim fileSystemEntry As New FileSystemEntry
                            Dim fileSystemNewEntryId = Guid.NewGuid
                            With fileSystemEntry
                                .FileSystemEntryId = fileSystemNewEntryId
                                .FileSystemEntryTypeId = FileType.File.ToString("D")
                                .ShareId = share.FileShareId
                                .CurrentVersionNumber = 0
                            End With

                            Dim fileSystemEntryVersion As New FileSystemEntryVersion
                            fileSystemEntry.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                            With fileSystemEntryVersion
                                .CreatedByUserId = Me.User.UserId
                                .CreatedOnUTC = file.CreationTimeUtc
                                .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(file.Name)
                                .FileSystemEntryExtension = IO.Path.GetExtension(file.Name)
                                .FileSystemEntryRelativePath = relativePath
                                .FileSystemEntrySize = file.Length
                                .FileSystemEntryVersionId = Guid.NewGuid
                                .ParentFileSystemEntryId = parentFileSystemEntryId
                                .FileSystemEntryHash = hash
                                .ServerFileSystemEntryName = existingFile.FileSystemEntryVersionId
                            End With

                            Dim delta As New DeltaOperation
                            fileSystemEntry.DeltaOperations.Add(delta)
                            With delta
                                .DeltaOperationId = Guid.NewGuid
                                .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.MoveOrRename, Byte)
                                .FileSystemEntryVersion = fileSystemEntryVersion
                                .LocalCreatedOnUTC = file.LastWriteTimeUtc
                                .LocalFileSystemEntryName = file.FullName
                                .IsOpen = isOpen
                            End With
                            service.FileSystemEntries.Add(fileSystemEntry)
                            Dim fileSystemEntryLink As New FileSystemEntryLink
                            With fileSystemEntryLink
                                .CreatedByUserId = Me.User.UserId
                                .CreatedOnUTC = Date.UtcNow
                                .FileSystemEntryId = fileSystemNewEntryId
                                .PreviousFileSystemEntryId = existingFile.FileSystemEntryId
                                .FileSystemEntryLinkId = Guid.NewGuid
                            End With
                            service.FileSystemEntryLinks.Add(fileSystemEntryLink)
                            For Each tag In service.Tags.Where(Function(p) (p.FileSystemEntryId = existingFile.FileSystemEntryId AndAlso Not p.IsDeleted)).ToList()
                                Dim tagNew As New Tag
                                tagNew.CreatedByUserId = tag.CreatedByUserId
                                tagNew.CreatedOnUTC = tag.CreatedOnUTC
                                tagNew.DisplayOrder = tag.DisplayOrder
                                tagNew.FileSystemEntryId = fileSystemNewEntryId
                                tagNew.TagName = tag.TagName
                                tagNew.TagId = Guid.NewGuid
                                tagNew.TagTypeId = tag.TagTypeId
                                tagNew.TagValue = tag.TagValue
                                service.Tags.Add(tagNew)
                            Next
                            localClientSyncLogger.Info("Created new file records " + fileSystemEntryVersion.FileSystemEntryVersionId.ToString() _
                                                   + " at path " + file.FullName + " with previous file " + existingFile.FileSystemEntryVersionId.ToString())
                        Else
                            'found new file
                            If (file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow) Then
                                Dim fileSystemEntry As New FileSystemEntry
                                With fileSystemEntry
                                    .CurrentVersionNumber = 0
                                    .FileSystemEntryId = Guid.NewGuid
                                    .FileSystemEntryTypeId = FileType.File.ToString("D")
                                    .ShareId = share.FileShareId
                                End With

                                Dim fileSystemEntryVersion As New FileSystemEntryVersion
                                fileSystemEntry.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                                With fileSystemEntryVersion
                                    .CreatedByUserId = Me.User.UserId
                                    .CreatedOnUTC = file.CreationTimeUtc
                                    .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(file.Name)
                                    .FileSystemEntryExtension = IO.Path.GetExtension(file.Name)
                                    .FileSystemEntryRelativePath = relativePath
                                    .FileSystemEntrySize = file.Length
                                    .FileSystemEntryVersionId = Guid.NewGuid
                                    .ParentFileSystemEntryId = parentFileSystemEntryId
                                    .FileSystemEntryHash = hash

                                    If (fileSystemEntryVersionInfo IsNot Nothing AndAlso fileSystemEntryVersionInfo.FileSystemEntryVersionId <> Guid.Empty) Then
                                        .PreviousFileSystemEntryVersionId = fileSystemEntryVersionInfo.FileSystemEntryVersionId
                                    End If

                                    .ServerFileSystemEntryName = .FileSystemEntryVersionId
                                End With

                                Dim delta As New DeltaOperation
                                fileSystemEntry.DeltaOperations.Add(delta)
                                With delta
                                    .DeltaOperationId = Guid.NewGuid
                                    .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)
                                    .FileSystemEntryVersion = fileSystemEntryVersion
                                    .LocalCreatedOnUTC = file.LastWriteTimeUtc
                                    .LocalFileSystemEntryName = file.FullName
                                    .IsOpen = isOpen
                                End With
                                service.FileSystemEntries.Add(fileSystemEntry)
                                localClientSyncLogger.Info("Created new file records " + fileSystemEntryVersion.FileSystemEntryVersionId.ToString() _
                                                   + " at path " + file.FullName)
                            End If
                        End If
                    End If
                    service.SaveChanges()
                Catch ex As Exception
                    localClientSyncLogger.Warn("Error in " + file.FullName, ex)
                End Try
            Next

            For Each delta In service.DeltaOperations.Include("FileSystemEntryVersion").Where(Function(p) filesToBeDeletedIdArr.Contains(p.FileSystemEntryVersionId))
                localClientSyncLogger.Info("File Deleted on local and now to be deleted from server " + delta.LocalFileSystemEntryName)
                If (delta.FileSystemEntryVersion.VersionNumber Is Nothing) Then
                    'If (delta.FileSystemEntry.CurrentVersionNumber = 0) Then
                    '    fileRepository.DeleteFile(delta.FileSystemEntryId)
                    'End If
                    If delta.FileSystemEntryVersion.PreviousFileSystemEntryVersionId Is Nothing AndAlso (delta.FileSystemEntryStatusId = CByte(Enums.FileEntryStatus.NewModified) OrElse delta.FileSystemEntryStatusId = CByte(Enums.FileEntryStatus.PendingUpload)) Then
                        'current version is not yet picked up for checking in and no previous version exist
                        fileRepository.DeleteFile(delta.FileSystemEntryId)
                    ElseIf (delta.FileSystemEntryVersion.PreviousFileSystemEntryVersionId IsNot Nothing) Then
                        'old version exist so remove this null version record
                        Dim versionId = delta.FileSystemEntryVersionId
                        service.FileSystemEntryVersionConflicts.RemoveRange(service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntryVersionId = versionId))
                        service.DeltaOperations.Remove(delta)
                        service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryVersionId = versionId))
                        'If delta.FileSystemEntryVersion.FileSystemEntryVersion1.VersionNumber IsNot Nothing AndAlso delta.FileSystemEntryVersion.FileSystemEntryVersion1.DeltaOperations.Count > 0 Then
                        '    Dim oldDelta = delta.FileSystemEntryVersion.FileSystemEntryVersion1.DeltaOperations(0)
                        '    With oldDelta
                        '        .FileSystemEntryStatusId = Enums.FileSystemEntryStatus.PendingFileSystemEntryDelete.ToString("D")
                        '        .IsDeleted = True
                        '        .DeletedByUserId = Me.User.Id
                        '        .DeletedOnUTC = Date.UtcNow
                        '    End With
                        'End If
                    End If
                Else
                    delta.FileSystemEntryStatusId = Enums.FileEntryStatus.PendingFileSystemEntryDelete.ToString("D")
                    delta.IsDeleted = True
                    delta.DeletedByUserId = Me.User.UserId
                    delta.DeletedOnUTC = Date.UtcNow
                End If
            Next

            service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) fileDeletedAtBothPlacesIdArr.Contains(p.FileSystemEntryId)))
            'For Each delta In service.DeltaOperations.Where(Function(p) fileDeletedAtBothPlacesIdArr.Contains(p.FileSystemEntryId))
            '    localClientSyncLogger.Info("File Deleted on local and server " + delta.LocalFileSystemEntryName)
            '    'delta.IsDeleted = True
            '    'delta.DeletedByUserId = Me.User.Id
            '    'delta.DeletedOnUTC = Date.UtcNow
            'Next

            service.SaveChanges()

            'if physical folder does not exist but record exist
            Dim removeFolderQuery = From v In view.Where(Function(p) p.FileSystemEntryTypeId = 2 AndAlso (p.PermissionBitValue And writeTypeId) = writeTypeId).ToList()
                                    Group Join dir In dirs
                    On dir.FullName.ToLower() Equals (share.SharePath + "\" + v.FileSystemEntryRelativePath).ToLower() Into Group
                                    From dir In Group.DefaultIfEmpty()
                                    Where dir Is Nothing
                                    Select v

            Dim foldersToBeDeleted = removeFolderQuery.ToList()
            Dim foldersToBeDeletedIdArr = foldersToBeDeleted.Where(Function(p) Not p.IsDeleted).Select(Function(p) p.FileSystemEntryVersionId).ToList()
            Dim folderDeletedAtBothPlacesIdArr = foldersToBeDeleted.Where(Function(p) p.IsDeleted).Select(Function(p) p.FileSystemEntryVersionId).ToList()

            For Each delta In service.DeltaOperations.Where(Function(p) foldersToBeDeletedIdArr.Contains(p.FileSystemEntryVersionId))
                delta.FileSystemEntryStatusId = Enums.FileEntryStatus.PendingFileSystemEntryDelete.ToString("D")
                delta.IsDeleted = True
                delta.DeletedByUserId = Me.User.UserId
                delta.DeletedOnUTC = Date.UtcNow
                localClientSyncLogger.Info("Folder Deleted on local and now to be dleeted from server " + delta.LocalFileSystemEntryName)
            Next

            service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) folderDeletedAtBothPlacesIdArr.Contains(p.FileSystemEntryId)))
            'For Each delta In service.DeltaOperations.Where(Function(p) folderDeletedAtBothPlacesIdArr.Contains(p.FileSystemEntryVersionId))
            '    localClientSyncLogger.Info("Folder Deleted on local and server " + delta.LocalFileSystemEntryName)
            '    delta.IsDeleted = True
            '    delta.DeletedByUserId = Me.User.Id
            '    delta.DeletedOnUTC = Date.UtcNow
            'Next

            service.SaveChanges()
            ''if physical folder exist and record exist with isPermanentDeleted true
            'Dim tobePermanentRemovedFolderQuery = From v In view.Where(Function(p) p.FileSystemEntryTypeId = 2 AndAlso p.IsPermanentlyDeleted).ToList()
            '                             Join dir In dirs
            '                         On dir.FullName.ToLower() Equals (share.SharePath + "\" + v.FileSystemEntryRelativePath).ToLower()
            '                             Select v
            '                             Order By v.FileSystemEntryRelativePath Descending
            'Dim tobePermamentRemovedFolder = tobePermanentRemovedFolderQuery.ToList()
            'For Each v In tobePermamentRemovedFolder
            '    Using CommonUtils.Helper.Impersonate(share)
            '        Directory.Delete(share.SharePath + "\" + v.FileSystemEntryRelativePath)
            '    End Using
            '    Dim fileRepository As New FileRepository
            '    fileRepository.DeleteFile(v.FileSystemEntryId)
            'Next
            'if physical folder exist and record exist with isDeleted true
            Dim tobeRemovedFolderQuery = From v In view.Where(Function(p) p.FileSystemEntryTypeId = 2 AndAlso p.IsDeleted AndAlso Not p.IsPermanentlyDeleted).ToList()
                                         Join dir In dirs
                                     On dir.FullName.ToLower() Equals (share.SharePath + "\" + v.FileSystemEntryRelativePath).ToLower()
                                         Select v
                                         Order By v.FileSystemEntryRelativePath Descending
            Dim tobeRemovedFolder = tobeRemovedFolderQuery.ToList()
            For Each v In tobeRemovedFolder
                localClientSyncLogger.Info("Delete Folder " + v.FileSystemEntryRelativePath)
                Try
                    Using CommonUtils.Helper.Impersonate(share)
                        Directory.Delete(share.SharePath + "\" + v.FileSystemEntryRelativePath)
                    End Using
                    'Dim delta = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = v.FileSystemEntryVersionId)
                    'delta.IsDeleted = True
                    'delta.DeletedByUserId = Me.User.Id
                    'delta.DeletedOnUTC = Date.UtcNow
                    service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = v.FileSystemEntryId))
                    service.SaveChanges()
                Catch ex As Exception
                    localClientSyncLogger.Warn("Error Deleting Folder " + v.FileSystemEntryRelativePath, ex)
                End Try
            Next

            'if the old linked file is undeleted on server but is marked as deleted here, change the new file status to 5 from 10 and set the server file name to new guid. also remove the prev file link
            Dim oldUndeletedLinkedFileQuery = service.DeltaOperations.Where(Function(pd) (Not pd.IsConflicted) AndAlso pd.FileSystemEntryStatusId <> 9 AndAlso Not pd.FileSystemEntry.IsDeleted AndAlso pd.FileSystemEntry.FileSystemEntryLinks1.Count > 0)
            Dim oldUndeletedLinkedFileIds = oldUndeletedLinkedFileQuery.Select(Function(pd) pd.FileSystemEntryId).ToList()

            Dim newFileQuery = service.DeltaOperations.Include("FileSystemEntryVersion").Where(Function(n) n.FileSystemEntryStatusId = 10 AndAlso Not n.FileSystemEntry.IsPermanentlyDeleted AndAlso (n.FileSystemEntry.FileSystemEntryLinks.Count() = 0 OrElse (n.FileSystemEntry.FileSystemEntryLinks.Count() > 0 AndAlso n.FileSystemEntry.FileSystemEntryLinks.Any(Function(t) oldUndeletedLinkedFileIds.Contains(t.PreviousFileSystemEntryId)))))
            For Each newFile In newFileQuery
                newFile.FileSystemEntryStatusId = Enums.FileEntryStatus.NewModified
                newFile.FileSystemEntryVersion.ServerFileSystemEntryName = Guid.NewGuid
            Next
            service.FileSystemEntryLinks.RemoveRange(oldUndeletedLinkedFileQuery.SelectMany(Function(p) p.FileSystemEntry.FileSystemEntryLinks1))

            service.SaveChanges()
            'Dim attr As FileAttributes = file.Attributes
            'If ((attr & FileAttribute.Directory) = FileAttribute.Directory) Then
            '    ' only left right join is needed
            'Else
            '    'find file version record in view by path
            '    'if found, compare the Delta.timechange, if diff and delta check pass, check hash. If diff, create new version else update localchange time in Delta table for that version
            '    'if not found, that means new file. Try to find a file that is marked as to be deleted in this run and compare hash. if found, create new fileversion or should it be create new link with new file???
            '    'if to be deleted file with same hash not found, check delta pass. if it pass, create new file.

            '    If (file.LastWriteTimeUtc.AddSeconds(deltaSec) < Date.UtcNow) Then
            '        'find file record in table and if the exact match on LastWriteTimeUtc is there or not
            '        'if not found new file
            '        'if found and match: ignore else get checksum and update checksum and status to U and create file/fileversion record

            '    End If
            'End If

            Dim resultInfo As New ResultInfo(Of Boolean, Status)
            resultInfo.Status = Status.Success
            resultInfo.Data = True
            Return resultInfo
        End Using
    End Function

    Private Function GetEffectivePermission(service As FileMinisterClientEntities, user As LocalWorkSpaceInfo, fileSystemEntryId As Guid) As Byte?
        Dim sql As String = "SELECT [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry] ({0}, {1})"
        Dim param = {user.UserId, fileSystemEntryId.ToString()}

        Return service.Database.SqlQuery(Of Byte?)(sql, param).FirstOrDefault()
    End Function

    Public Function ResetSyncTimestamp() As ResultInfo(Of Boolean, Status) Implements ISyncRepository.ResetSyncTimestamp
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Using service = GetClientEntity()
                service.Database.ExecuteSqlCommand("UPDATE [SynkFramework_scope_info] SET [scope_sync_knowledge] = NULL WHERE [sync_scope_name] IN ('FileMinisterFilteredScopeDownloadOnly','FileMinisterFilteredScopeDownloadAndUpload')")
                res.Data = True
                res.Status = Status.Success
                Return res
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function
End Class
