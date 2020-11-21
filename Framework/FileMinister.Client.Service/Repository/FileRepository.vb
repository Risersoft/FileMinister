Imports risersoft.shared.portable.Model
Imports FileMinister.Client.Service.IRepository
Imports System.Data.SqlClient
Imports System.IO
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud
Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync

Namespace Repository
    Public Class FileRepository
        Inherits ClientRepositoryBase(Of FileEntryInfo, Guid)
        Implements IFileRepository

        ''' <summary>
        ''' Resolve a conflict on a file using server 
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <param name="shareId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ResolveConflictUsingTheirs(fileSystemEntryId As Guid, shareId As Integer) As ResultInfo(Of Boolean, Status) Implements IFileRepository.ResolveConflictUsingTheirs
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Try
                Dim sharePath As String
                Dim share
                Using servicecommon = GetClientCommonEntity()
                    share = servicecommon.UserShares.FirstOrDefault(Function(p) p.ShareId = shareId AndAlso p.UserAccountId = Me.User.UserAccountId)
                    sharePath = share.SharePath
                End Using
                Using service = GetClientEntity()
                    Dim shareCredentials = New ConfigInfo() With {.WindowsUser = share.WindowsUser, .Password = share.Password}
                    Dim conflict = service.FileSystemEntryVersionConflicts.OrderByDescending(Function(p) p.CreatedOnUTC).FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso p.IsResolved = False AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId)
                    If (conflict IsNot Nothing) Then
                        Dim fsevLatest = service.FileSystemEntryVersions.OrderByDescending(Function(p) p.VersionNumber).FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)
                        Dim deltaConflict = service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso p.IsConflicted)
                        Dim nullDelta As New List(Of Service.DeltaOperation)
                        Select Case conflict.FileSystemEntryVersionConflictTypeId
                            Case Enums.FileVersionConflictType.Modified, Enums.FileVersionConflictType.ClientModifyServerRename, Enums.FileVersionConflictType.ClientDelete, Enums.FileVersionConflictType.InTheWay

                                For Each delta As DeltaOperation In deltaConflict
                                    Using CommonUtils.Helper.Impersonate(shareCredentials)
                                        If delta.FileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId Then
                                            If (File.Exists(delta.LocalFileSystemEntryName)) Then
                                                Dim destPath = Path.Combine(sharePath, delta.FileSystemEntryVersion.FileSystemEntryRelativePath)
                                                'If (File.Exists(destPath)) Then
                                                '    File.Delete(destPath)
                                                'End If
                                                File.Copy(delta.LocalFileSystemEntryName, destPath, True)

                                                File.Delete(delta.LocalFileSystemEntryName)
                                            Else
                                                'conflicted file has been deleted so downloading again from server
                                                service.DeltaOperations.RemoveRange(service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))
                                                nullDelta = Nothing
                                                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso (p.VersionNumber Is Nothing OrElse p.VersionNumber = 0)))
                                                Exit For
                                            End If

                                        ElseIf (Not (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0)) AndAlso File.Exists(delta.LocalFileSystemEntryName) Then
                                            File.Delete(delta.LocalFileSystemEntryName)
                                        ElseIf (conflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ClientModifyServerRename AndAlso (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0)) AndAlso File.Exists(delta.LocalFileSystemEntryName) Then
                                            File.Delete(delta.LocalFileSystemEntryName)
                                        End If
                                    End Using
                                    If (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0) Then
                                        nullDelta.Add(delta)
                                    Else
                                        delta.IsConflicted = False
                                        delta.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                        delta.LocalFileSystemEntryName = Path.Combine(sharePath, delta.FileSystemEntryVersion.FileSystemEntryRelativePath)
                                    End If
                                Next
                            Case Enums.FileVersionConflictType.ServerDelete
                                For Each delta As DeltaOperation In deltaConflict
                                    Using CommonUtils.Helper.Impersonate(shareCredentials)
                                        If (File.Exists(delta.LocalFileSystemEntryName)) Then
                                            File.Delete(delta.LocalFileSystemEntryName)
                                        End If
                                    End Using
                                    If (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0) Then
                                        nullDelta.Add(delta)
                                    Else
                                        delta.IsConflicted = False
                                        delta.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                        delta.LocalFileSystemEntryName = delta.FileSystemEntryVersion.FileSystemEntryName
                                    End If
                                Next
                        End Select
                        conflict.FileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId
                        'removed null versions and delta records
                        If (nullDelta IsNot Nothing) Then
                            For Each delta In nullDelta
                                Dim entryVersionId = delta.FileSystemEntryVersionId
                                service.DeltaOperations.Remove(delta)
                                service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryVersionId = entryVersionId))
                            Next
                        End If

                        If conflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ClientDelete Then
                            Dim links = service.FileSystemEntryLinks.FirstOrDefault(Function(p) p.PreviousFileSystemEntryId = fileSystemEntryId)
                            If (links IsNot Nothing) Then
                                Dim moved = CType(Enums.FileEntryStatus.MoveOrRename, Byte)
                                Dim deltaMoved = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryId = links.FileSystemEntryId AndAlso p.FileSystemEntryStatusId = moved)
                                If (deltaMoved IsNot Nothing) Then
                                    deltaMoved.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                    service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = deltaMoved.FileSystemEntryVersionId).ServerFileSystemEntryName = Guid.NewGuid
                                End If
                                service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))
                            End If
                        End If

                        'Dim fsevNull = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso (p.VersionNumber Is Nothing OrElse p.VersionNumber = 0))
                        'service.DeltaOperations.RemoveRange(service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fsevNull.FileSystemEntryVersionId)

                        conflict.IsResolved = True
                        conflict.ResolvedByUserId = Me.User.UserId
                        conflict.ResolvedOnUTC = DateTime.UtcNow
                        conflict.ResolvedType = Enums.Constants.SERVER_WIN_RESOLVETYPE

                        service.SaveChanges()

                    End If
                    res.Data = True
                    res.Message = "Operation successfull"
                    res.Status = Status.Success
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
            Return res
        End Function

        ''' <summary>
        ''' Resolve a conflict on a file using client 
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ResolveConflictUsingMine(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.ResolveConflictUsingMine
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Try
                Using service = GetClientEntity()
                    Dim conflict = service.FileSystemEntryVersionConflicts.OrderByDescending(Function(p) p.CreatedOnUTC).FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso p.IsResolved = False AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId)
                    If (conflict IsNot Nothing) Then

                        Dim fsevLatest = service.FileSystemEntryVersions.OrderByDescending(Function(p) p.VersionNumber).FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)
                        ' Get Share Path
                        Dim sharePath As String
                        Dim share
                        Using servicecommon = GetClientCommonEntity()
                            share = servicecommon.UserShares.FirstOrDefault(Function(p) p.ShareId = fsevLatest.FileSystemEntry.ShareId AndAlso p.UserAccountId = Me.User.UserAccountId)
                            sharePath = share.SharePath
                        End Using
                        Dim shareCredentials = New ConfigInfo() With {.WindowsUser = share.WindowsUser, .Password = share.Password}

                        If (conflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.InTheWay) Then
                            Dim fileInfo = New IO.FileInfo(Path.Combine(sharePath, fsevLatest.FileSystemEntryRelativePath))
                            Using CommonUtils.Helper.Impersonate(shareCredentials)
                                If (fileInfo.Exists) Then
                                    Dim nullVersion = service.FileSystemEntryVersions.OrderByDescending(Function(p) p.CreatedOnUTC).FirstOrDefault(Function(p) p.VersionNumber Is Nothing AndAlso p.FileSystemEntryId = fileSystemEntryId)
                                    If (nullVersion Is Nothing) Then
                                        Dim hash = HashCalculator.hash_generator("md5", fileInfo.FullName)
                                        Dim newguid = Guid.NewGuid
                                        Dim fileSystemEntryVersion As New FileSystemEntryVersion
                                        With fileSystemEntryVersion
                                            .CreatedByUserId = Me.User.UserId
                                            .CreatedOnUTC = fileInfo.CreationTimeUtc
                                            .FileSystemEntryId = fileSystemEntryId
                                            .FileSystemEntryName = IO.Path.GetFileNameWithoutExtension(fileInfo.Name)
                                            .FileSystemEntryExtension = IO.Path.GetExtension(fileInfo.Name)
                                            .FileSystemEntryRelativePath = fsevLatest.FileSystemEntryRelativePath
                                            .FileSystemEntryNameWithExtension = fsevLatest.FileSystemEntryNameWithExtension
                                            .FileSystemEntrySize = fileInfo.Length
                                            .FileSystemEntryVersionId = newguid
                                            .ParentFileSystemEntryId = fsevLatest.ParentFileSystemEntryId
                                            .PreviousFileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId
                                            .FileSystemEntryHash = hash
                                            .ServerFileSystemEntryName = newguid
                                        End With

                                        Dim delta As New DeltaOperation
                                        fileSystemEntryVersion.DeltaOperations.Add(delta)
                                        With delta
                                            .DeltaOperationId = Guid.NewGuid
                                            .FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NewModified, Byte)
                                            '.FileSystemEntryVersion = fileSystemEntryVersion
                                            .LocalCreatedOnUTC = fileInfo.LastWriteTimeUtc
                                            .LocalFileSystemEntryName = fileInfo.FullName
                                            .FileSystemEntryId = fileSystemEntryId
                                            .IsConflicted = False
                                        End With
                                        service.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                                        service.SaveChanges()
                                    End If
                                End If
                            End Using
                        End If

                        Dim deltaConflict = service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso p.IsConflicted)
                        Select Case conflict.FileSystemEntryVersionConflictTypeId
                            Case Enums.FileVersionConflictType.Modified, Enums.FileVersionConflictType.ClientModifyServerRename, Enums.FileVersionConflictType.InTheWay, Enums.FileVersionConflictType.ServerDelete
                                'Dim nullDelta As New List(Of Service.DeltaOperation)
                                For Each delta As DeltaOperation In deltaConflict
                                    If (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0) Then
                                        'nullDelta.Add(delta)
                                        If (conflict.FileSystemEntryVersionConflictTypeId <> Enums.FileVersionConflictType.InTheWay) Then
                                            delta.FileSystemEntryVersion.PreviousFileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId
                                            Using CommonUtils.Helper.Impersonate(shareCredentials)
                                                If (conflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ClientModifyServerRename AndAlso File.Exists(delta.LocalFileSystemEntryName)) Then
                                                    File.Move(delta.LocalFileSystemEntryName, Path.Combine(sharePath, fsevLatest.FileSystemEntryRelativePath))
                                                End If
                                            End Using
                                            delta.FileSystemEntryVersion.FileSystemEntryRelativePath = fsevLatest.FileSystemEntryRelativePath
                                            delta.FileSystemEntryVersion.FileSystemEntryName = fsevLatest.FileSystemEntryName
                                            delta.FileSystemEntryVersion.FileSystemEntryExtension = fsevLatest.FileSystemEntryExtension
                                            delta.FileSystemEntryVersion.FileSystemEntryNameWithExtension = fsevLatest.FileSystemEntryNameWithExtension
                                        End If
                                    Else
                                        Using CommonUtils.Helper.Impersonate(shareCredentials)
                                            If (File.Exists(delta.LocalFileSystemEntryName)) Then
                                                File.Delete(delta.LocalFileSystemEntryName)
                                            End If
                                        End Using
                                        delta.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                    End If
                                    delta.IsConflicted = False
                                    delta.LocalFileSystemEntryName = Path.Combine(sharePath, fsevLatest.FileSystemEntryRelativePath)
                                Next
                                'nullDelta(0).FileSystemEntryVersion.PreviousFileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId
                                If (conflict.FileSystemEntryVersionConflictTypeId = Enums.FileVersionConflictType.ServerDelete) Then
                                    Dim fileSystemEntry = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)
                                    fileSystemEntry.IsDeleted = False
                                    fileSystemEntry.DeletedByUserId = Nothing
                                    fileSystemEntry.DeletedOnUTC = Nothing
                                    service.FileSystemEntryLinks.RemoveRange(service.FileSystemEntryLinks.Where(Function(p) p.PreviousFileSystemEntryId = fileSystemEntryId))
                                End If
                            Case Enums.FileVersionConflictType.ClientDelete
                                For Each delta As DeltaOperation In deltaConflict
                                    If (delta.FileSystemEntryVersion.VersionNumber Is Nothing OrElse delta.FileSystemEntryVersion.VersionNumber = 0) Then
                                        'nullDelta.Add(delta)
                                        service.DeltaOperations.Remove(delta)
                                        service.FileSystemEntryVersions.RemoveRange(service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryVersionId = delta.FileSystemEntryVersionId))
                                        Using CommonUtils.Helper.Impersonate(shareCredentials)
                                            If (File.Exists(delta.LocalFileSystemEntryName)) Then
                                                File.Delete(delta.LocalFileSystemEntryName)
                                            End If
                                        End Using
                                    Else
                                        Using CommonUtils.Helper.Impersonate(shareCredentials)
                                            If (File.Exists(delta.LocalFileSystemEntryName)) Then
                                                File.Delete(delta.LocalFileSystemEntryName)
                                            End If
                                        End Using
                                        If (delta.FileSystemEntryVersionId = fsevLatest.FileSystemEntryVersionId) Then
                                            delta.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.PendingFileSystemEntryDelete, Byte)
                                            delta.IsDeleted = True
                                            delta.DeletedByUserId = Me.User.UserId
                                            delta.DeletedOnUTC = DateTime.UtcNow
                                        Else
                                            delta.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                        End If

                                    End If
                                    delta.IsConflicted = False
                                    delta.LocalFileSystemEntryName = Path.Combine(sharePath, delta.FileSystemEntryVersion.FileSystemEntryRelativePath)
                                Next

                        End Select


                        conflict.IsResolved = True
                        conflict.ResolvedByUserId = Me.User.UserId
                        conflict.ResolvedOnUTC = DateTime.UtcNow
                        conflict.ResolvedType = Enums.Constants.CLIENT_WIN_RESOLVETYPE
                        service.SaveChanges()
                    End If

                    res.Data = True
                    res.Message = "Operation successfull"
                    res.Status = Status.Success


                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Soft Delete a file/folder - mark IsDeleted true in delta operation of each version and remove version physically
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <param name="isForce"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SoftDelete(fileSystemEntryId As Guid, isForce As Boolean) As ResultInfo(Of Boolean, Status) Implements IFileRepository.SoftDelete
            'TODO - NEED TO DISCUSS ISFORCE LOGIC
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
                Dim userId As Guid = Me.User.UserId
                Using service = GetClientEntity()

                    Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)
                    If (file IsNot Nothing) Then
                        Using servicecommon = GetClientCommonEntity()

                            Dim share = servicecommon.UserShares.FirstOrDefault(Function(p) p.ShareId = file.ShareId AndAlso p.UserAccountId = Me.User.UserAccountId)
                            If (share IsNot Nothing) Then

                                Dim shareCredentials = New ConfigInfo() With {.WindowsUser = share.WindowsUser, .Password = share.Password}

                                Dim filehierarchy = service.GetLatestFileSystemEntryVersionsChildrenHierarchy(fileSystemEntryId)

                                If (filehierarchy IsNot Nothing And filehierarchy.Count > 0) Then

                                    If (isForce = False) Then
                                        Dim checkedoutfiles = filehierarchy.Where(Function(p) p.IsCheckedOut = True)
                                        If (checkedoutfiles IsNot Nothing And checkedoutfiles.Count > 0) Then
                                            res.Data = False
                                            res.Message = "Some files are in checked out mode. Do you still want to delete?"
                                            res.Status = Status.FileCheckedOut
                                            Return res
                                        End If
                                    End If


                                    'Dim DeleteStatus = service.FileSystemEntryStatuses.First(Function(p) p.FileSystemEntryStatusName = "Pending FileSystemEntry Delete")


                                    For Each item In filehierarchy
                                        Dim fileInLoop = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = item.FileSystemEntryId)

                                        'fileInLoop.IsCheckedOut = False
                                        'fileInLoop.CheckedOutByUserId = Nothing
                                        'fileInLoop.CheckedOutOnUTC = Nothing

                                        'fileInLoop.IsDeleted = True
                                        'fileInLoop.IsPermanentlyDeleted = True
                                        'fileInLoop.DeletedByUserId = userId
                                        'fileInLoop.DeletedOnUTC = DateTime.UtcNow()

                                        Dim allFileVersionsInLoop = service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = item.FileSystemEntryId)
                                        For Each itemVersion In allFileVersionsInLoop
                                            'itemVersion.IsDeleted = True
                                            'itemVersion.DeletedByUserId = userId
                                            'itemVersion.DeletedOnUTC = DateTime.UtcNow()

                                            For Each deltaop In itemVersion.DeltaOperations
                                                deltaop.IsDeleted = True
                                                deltaop.DeletedByUserId = Me.User.UserId
                                                deltaop.DeletedOnUTC = DateTime.UtcNow
                                                'If (DeleteStatus IsNot Nothing) Then
                                                deltaop.FileSystemEntryStatusId = Enums.FileEntryStatus.PendingFileSystemEntryDelete
                                                'End If
                                            Next

                                            If (file.FileSystemEntryType.FileSystemEntryTypeId = FileType.File) Then
                                                Dim filepath = share.SharePath + "\" + itemVersion.FileSystemEntryRelativePath
                                                Using CommonUtils.Helper.Impersonate(shareCredentials)
                                                    If (System.IO.File.Exists(filepath) = True) Then
                                                        System.IO.File.Delete(filepath)
                                                    End If
                                                End Using
                                            Else
                                                If (file.FileSystemEntryType.FileSystemEntryTypeId = FileType.Folder) Then
                                                    Dim filepath = share.SharePath + "\" + itemVersion.FileSystemEntryRelativePath
                                                    Using CommonUtils.Helper.Impersonate(shareCredentials)
                                                        If (System.IO.Directory.Exists(filepath) = True) Then
                                                            System.IO.Directory.Delete(filepath, True)
                                                        Else
                                                            If (System.IO.File.Exists(filepath) = True) Then
                                                                System.IO.File.Delete(filepath)
                                                            End If
                                                        End If
                                                    End Using
                                                End If
                                            End If
                                        Next
                                    Next

                                    service.SaveChanges()

                                    res.Data = True
                                    res.Message = "Operation successfull"
                                    res.Status = Status.Success
                                Else
                                    res.Data = False
                                    res.Message = "File not found"
                                    res.Status = Status.NotFound
                                End If

                            Else
                                res.Data = False
                                res.Message = "Share not found"
                                res.Status = Status.NotFound
                            End If
                        End Using
                    Else
                        res.Data = False
                        res.Message = "File not found"
                        res.Status = Status.NotFound
                    End If
                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Hard Delete a file/folder - get the hierarchy, delete/update links if exists, undo any check out, mark IsDeleted and IsPermanentlyDeleted on a file
        ''' Remove records from delta operation against each version of each file
        ''' For each version against each file mark IsDeleted and remove version physically
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HardDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.HardDelete
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
                Dim userId As Guid = Me.User.UserId
                Using service = GetClientEntity()

                    Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)
                    If (file IsNot Nothing) Then
                        Using servicecommon = GetClientCommonEntity()
                            Dim share = servicecommon.UserShares.FirstOrDefault(Function(p) p.ShareId = file.ShareId AndAlso p.UserAccountId = Me.User.UserAccountId)
                            If (share IsNot Nothing) Then

                                Dim shareCredentials = New ConfigInfo() With {.WindowsUser = share.WindowsUser, .Password = share.Password}

                                Dim filehierarchy = service.GetFileSystemEntryChildrenHierarchyInclDeleted(fileSystemEntryId)
                                If (filehierarchy IsNot Nothing And filehierarchy.Count > 0) Then

                                    Dim files = From FE In service.FileSystemEntries.Include("FileSystemEntryVersions").Include("DeltaOperations")
                                                Join a In filehierarchy
                                                On a.FileSystemEntryId Equals FE.FileSystemEntryId
                                                Select FE

                                    For Each FE In files

                                        'Manage Links
                                        Dim inPrevLink = service.FileSystemEntryLinks.FirstOrDefault(Function(p) p.PreviousFileSystemEntryId = FE.FileSystemEntryId)
                                        Dim inNewLink = service.FileSystemEntryLinks.FirstOrDefault(Function(p) p.FileSystemEntryId = FE.FileSystemEntryId)

                                        If (inPrevLink IsNot Nothing) Then
                                            If (inNewLink IsNot Nothing) Then
                                                'file is linked both ways, in this case link the other 2 ends and remove one link record
                                                inPrevLink.PreviousFileSystemEntryId = inNewLink.PreviousFileSystemEntryId
                                                service.FileSystemEntryLinks.Remove(inNewLink)
                                            Else
                                                'file just exists on one side; delete the link record
                                                service.FileSystemEntryLinks.Remove(inPrevLink)
                                            End If
                                        ElseIf (inNewLink IsNot Nothing) Then
                                            'file just exists on one side; delete the link record
                                            service.FileSystemEntryLinks.Remove(inNewLink)
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

                                        service.DeltaOperations.RemoveRange(FE.DeltaOperations)

                                        For Each FEV In FE.FileSystemEntryVersions
                                            FEV.IsDeleted = True
                                            FEV.DeletedByUserId = userId
                                            FEV.DeletedOnUTC = DateTime.UtcNow()

                                            If (FE.FileSystemEntryType.FileSystemEntryTypeId = FileType.File) Then
                                                Dim filepath = Path.Combine(share.SharePath, FEV.FileSystemEntryRelativePath)
                                                Using CommonUtils.Helper.Impersonate(shareCredentials)
                                                    If (System.IO.File.Exists(filepath) = True) Then
                                                        System.IO.File.Delete(filepath)
                                                    End If
                                                End Using
                                            Else
                                                If (FE.FileSystemEntryType.FileSystemEntryTypeId = FileType.Folder) Then
                                                    Dim filepath = Path.Combine(share.SharePath, FEV.FileSystemEntryRelativePath)
                                                    Using CommonUtils.Helper.Impersonate(shareCredentials)
                                                        If (System.IO.Directory.Exists(filepath) = True) Then
                                                            System.IO.Directory.Delete(filepath, True)
                                                        Else
                                                            If (System.IO.File.Exists(filepath) = True) Then
                                                                System.IO.File.Delete(filepath)
                                                            End If
                                                        End If
                                                    End Using
                                                End If
                                            End If
                                        Next
                                    Next

                                    service.SaveChanges()

                                    res.Data = True
                                    res.Message = "Operation successfull"
                                    res.Status = Status.Success
                                Else
                                    res.Data = False
                                    res.Message = "File not found"
                                    res.Status = Status.NotFound
                                End If
                            Else
                                res.Data = False
                                res.Message = "Share Details not found"
                                res.Status = Status.NotFound
                            End If

                        End Using
                    Else
                        res.Data = False
                        res.Message = "File not found"
                        res.Status = Status.NotFound
                    End If
                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get the file against a path and then get the hierarchy of that file and remove all related records using DeleteFile() function
        ''' </summary>
        ''' <param name="baseRelativePath"></param>
        ''' <param name="shareId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteFiles(baseRelativePath As String, shareId As Integer) As ResultInfo(Of Boolean, Status) Implements IFileRepository.DeleteFiles
            Try
                Dim fileSystemEntryIds As List(Of Guid?) = Nothing
                Using Service = GetClientEntity()
                    Dim obj = Service.GetLatestFileSystemEntryVersionByPath(baseRelativePath, shareId).FirstOrDefault()

                    If (obj IsNot Nothing) Then
                        fileSystemEntryIds = Service.GetFileSystemEntryChildrenHierarchyInclDeleted(obj.FileSystemEntryId).OrderByDescending(Function(p) p.Level).Select(Function(p) p.FileSystemEntryId).ToList()
                    End If
                End Using

                Dim res As ResultInfo(Of Boolean, Status)
                If fileSystemEntryIds IsNot Nothing Then
                    For Each fileSystemEntryId In fileSystemEntryIds
                        res = DeleteFile(fileSystemEntryId)
                        If (Not res.Data) Then
                            Return res
                        End If
                    Next
                End If
                res = New ResultInfo(Of Boolean, Status)
                res.Data = True
                res.Message = "Operation Successful"
                res.Status = Status.Success
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Remove records from DeltaOperations, FileSystemEntryVersionConflicts, FileSystemEntryVersions, Tags, GroupFileSystemEntryPermissionAssignments,
        ''' UserFileSystemEntryPermissionAssignments, FileSystemEntryLinks, FileSystemEntries against a file
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.DeleteFile
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

                Using Service = GetClientEntity()
                    'Delete Delta Records
                    Service.DeltaOperations.RemoveRange(Service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete Conflicts records
                    Service.FileSystemEntryVersionConflicts.RemoveRange(Service.FileSystemEntryVersionConflicts.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete Version Records
                    Service.FileSystemEntryVersions.RemoveRange(Service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete Tag Records
                    Service.Tags.RemoveRange(Service.Tags.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete Group Permission Assignments Records
                    Service.GroupFileSystemEntryPermissionAssignments.RemoveRange(Service.GroupFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete User Permission Assignments Records
                    Service.UserFileSystemEntryPermissionAssignments.RemoveRange(Service.UserFileSystemEntryPermissionAssignments.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    'Delete FileLink Records record
                    Service.FileSystemEntryLinks.RemoveRange(Service.FileSystemEntryLinks.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId OrElse p.PreviousFileSystemEntryId = fileSystemEntryId))

                    'Delete File record
                    Service.FileSystemEntries.RemoveRange(Service.FileSystemEntries.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                    Service.SaveChanges()

                    res.Data = True
                    res.Message = "Operation Successful"
                    res.Status = Status.Success

                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' For each version of a file(deleted), insert either new delta operation (if not exist) or update existing one with isDeletd false 
        ''' and update FileSystemEntryStatusId according to max version number
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UndoDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.UndoDelete
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

                Using Service = GetClientEntity()
                    Dim file = Service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)

                    If file IsNot Nothing Then
                        If Not file.IsPermanentlyDeleted Then
                            'Get All versions for the file Also get the Max Version
                            Dim versions = Service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso p.VersionNumber IsNot Nothing).Select(Function(p) p).ToList()
                            Dim maxVersion As Integer = versions.Max(Function(p) p.VersionNumber)

                            If (file.FileSystemEntryTypeId = FileType.File) Then

                                'Create Delta Records for all the versions with latest version in status in Downloading
                                For Each version In versions

                                    Dim delta As DeltaOperation = Nothing
                                    If version.DeltaOperations Is Nothing OrElse version.DeltaOperations.Count = 0 Then
                                        version.DeltaOperations = New List(Of DeltaOperation)

                                        delta = New DeltaOperation()
                                        version.DeltaOperations.Add(delta)
                                        With delta
                                            .DeltaOperationId = Guid.NewGuid()
                                            .FileSystemEntryId = fileSystemEntryId
                                            .FileSystemEntryVersionId = version.FileSystemEntryVersionId
                                            .IsConflicted = False
                                            .IsDeleted = False
                                            .IsOpen = False
                                        End With

                                    Else
                                        delta = version.DeltaOperations.FirstOrDefault()
                                        delta.IsOpen = False
                                        delta.IsDeleted = False
                                        delta.IsConflicted = False
                                        delta.DeletedByUserId = Nothing
                                        delta.DeletedOnUTC = Nothing
                                        delta.LocalCreatedOnUTC = Nothing
                                        delta.LocalFileSystemEntryExtension = Nothing
                                        delta.LocalFileSystemEntryName = Nothing
                                    End If

                                    If (version.VersionNumber = maxVersion) Then
                                        delta.FileSystemEntryStatusId = Enums.FileEntryStatus.Downloading
                                    Else
                                        delta.FileSystemEntryStatusId = Enums.FileEntryStatus.NoDownloadRequired
                                    End If

                                Next

                            End If

                            'Set Isdeleted flag to false in FileSystemEntry Table
                            file.IsDeleted = False
                            file.DeletedByUserId = Nothing
                            file.DeletedOnUTC = Nothing

                            Service.SaveChanges()

                            res.Data = True
                            res.Message = "Operation Sucessful"
                            res.Status = Status.Success

                        Else
                            res.Data = False
                            res.Message = "File Permanently Deleted"
                            res.Status = Status.PermanentlyDeleted
                        End If
                    Else
                        res.Data = False
                        res.Message = "File Not Found"
                        res.Status = Status.NotFound
                    End If

                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        Public Overrides Function GetAll() As ResultInfo(Of List(Of FileEntryInfo), Status)
            Return GetFolders()
        End Function

        ''' <summary>
        ''' Get all folders records against a share
        ''' </summary>
        ''' <param name="shareId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByShareId(shareId As Integer) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.GetByShareId
            Return GetFolders(shareId)
        End Function

        ''' <summary>
        ''' Get all folders records either against a share or all shares
        ''' </summary>
        ''' <param name="shareId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFolders(Optional shareId As Integer? = Nothing) As ResultInfo(Of List(Of FileEntryInfo), Status)
            Try
                Using service = GetClientEntity()
                    Dim q = service.FileSystemEntries.Where(Function(p) p.IsDeleted = False AndAlso (p.FileSystemEntryTypeId = 1 Or p.FileSystemEntryTypeId = 2))

                    If shareId.HasValue Then
                        q = q.Where(Function(p) p.ShareId = shareId.Value)
                    End If

                    Dim files = q.Select(Function(p) New With {Key .File = p, Key .FileVersion = p.FileSystemEntryVersions.OrderByDescending(Function(fv) fv.VersionNumber AndAlso fv.IsDeleted = False).FirstOrDefault()}).ToList()

                    Dim fileProxies As New List(Of FileEntryInfo)()
                    For Each file In files
                        Dim f As New FileEntryInfo()
                        With f
                            .FileEntryId = file.File.FileSystemEntryId
                            .FileEntryTypeId = file.File.FileSystemEntryTypeId
                            .FileShareId = file.File.ShareId
                            .IsCheckedOut = file.File.IsCheckedOut
                            .CheckedOutByUserId = file.File.CheckedOutByUserId
                            .CheckedOutOnUTC = file.File.CheckedOutOnUTC
                            .CheckOutWorkSpaceId = file.File.CheckedOutWorkSpaceId
                            .CurrentVersionNumber = file.File.CurrentVersionNumber


                            If (file.FileVersion IsNot Nothing) Then
                                .FileVersion = New FileVersionInfo()
                                .FileVersion.FileVersionId = file.FileVersion.FileSystemEntryVersionId
                                .FileVersion.ParentFileEntryId = file.FileVersion.ParentFileSystemEntryId
                                .FileVersion.FileEntryId = file.FileVersion.FileSystemEntryId
                                .FileVersion.VersionNumber = file.FileVersion.VersionNumber
                                .FileVersion.FileEntryName = file.FileVersion.FileSystemEntryName
                                .FileVersion.FileEntryExtension = file.FileVersion.FileSystemEntryExtension
                                .FileVersion.FileEntryNameWithExtension = file.FileVersion.FileSystemEntryNameWithExtension
                                .FileVersion.FileEntrySize = file.FileVersion.FileSystemEntrySize
                                .FileVersion.ServerFileName = file.FileVersion.ServerFileSystemEntryName
                                .FileVersion.CreatedOnUTC = file.FileVersion.CreatedOnUTC

                            End If

                        End With
                        fileProxies.Add(f)
                    Next
                    Return BuildResponse(fileProxies)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get file by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Get](id As Guid) As ResultInfo(Of FileEntryInfo, Status)
            Try
                Using service = GetClientEntity()
                    Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = id)
                    Dim obj = New FileEntryInfo
                    If file IsNot Nothing Then

                        obj.FileEntryId = file.FileSystemEntryId
                        obj.FileShareId = file.ShareId
                        Dim ltstFileVersion = file.FileSystemEntryVersions.Where(Function(p) p.VersionNumber IsNot Nothing AndAlso p.VersionNumber > 0).OrderByDescending(Function(p) p.VersionNumber).FirstOrDefault()
                        obj.IsCheckedOut = file.IsCheckedOut
                        obj.CheckedOutByUserId = file.CheckedOutByUserId
                        obj.CheckedOutOnUTC = file.CheckedOutOnUTC
                        obj.CheckOutWorkSpaceId = file.CheckedOutWorkSpaceId
                        obj.IsDeleted = file.IsDeleted
                        obj.IsPermanentlyDeleted = file.IsPermanentlyDeleted
                        obj.CurrentVersionNumber = file.CurrentVersionNumber

                        If (ltstFileVersion IsNot Nothing) Then
                            obj.FileVersion = New FileVersionInfo()
                            obj.FileVersion.FileEntryRelativePath = ltstFileVersion.FileSystemEntryRelativePath
                            obj.FileVersion.FileVersionId = ltstFileVersion.FileSystemEntryVersionId
                            obj.FileVersion.FileEntryName = ltstFileVersion.FileSystemEntryName
                            obj.FileVersion.FileEntryExtension = ltstFileVersion.FileSystemEntryExtension
                            obj.FileVersion.FileEntryNameWithExtension = ltstFileVersion.FileSystemEntryNameWithExtension
                            obj.FileVersion.ServerFileName = ltstFileVersion.ServerFileSystemEntryName
                            obj.FileVersion.FileEntrySize = ltstFileVersion.FileSystemEntrySize
                            obj.FileVersion.ParentFileEntryId = ltstFileVersion.ParentFileSystemEntryId
                            obj.FileVersion.PreviousFileVersionId = ltstFileVersion.PreviousFileSystemEntryVersionId
                            obj.FileVersion.VersionNumber = ltstFileVersion.VersionNumber
                            obj.FileVersion.CreatedByUserId = ltstFileVersion.CreatedByUserId
                            obj.FileVersion.CreatedOnUTC = ltstFileVersion.CreatedOnUTC
                        End If
                    End If
                    Return BuildResponse(obj)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of FileEntryInfo)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get all child records against a file id
        ''' </summary>
        ''' <param name="parentId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByParentId(parentId As Guid) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.GetByParentId
            Try
                Using service = GetClientEntity()

                    Dim fileProxies As New List(Of FileEntryInfo)()

                    'Dim files = service.FileSystemEntryVersions.Where(Function(p) p.ParentFileSystemEntryId = parentId).ToList()
                    'For Each file In files
                    '    Dim f As New FileSystemEntryInfo()
                    '    With f
                    '        .FileSystemEntryId = file.FileSystemEntryId
                    '        .FileSystemEntryName = file.FileSystemEntryName
                    '        .FileSystemEntryNameWithExtension = file.FileSystemEntryNameWithExtension
                    '        .ShareId = file.FileSystemEntry.ShareId
                    '        .FileSystemEntryTypeId = file.FileSystemEntry.FileSystemEntryTypeId
                    '        .ParentFileId = file.ParentFileSystemEntryId
                    '    End With
                    '    fileProxies.Add(f)
                    'Next

                    Dim files = service.GetLatestFileSystemEntryVersionsChildrens(parentId, Me.User.UserId).ToList()

                    For Each file In files
                        Dim f As New FileEntryInfo()
                        With f
                            .FileEntryId = file.FileSystemEntryId
                            .FileShareId = file.ShareId
                            .FileEntryTypeId = file.FileSystemEntryTypeId
                            .IsCheckedOut = file.IsCheckedOut
                            .CheckedOutByUserId = file.CheckedOutByUserId
                            .CheckedOutOnUTC = file.CheckedOutOnUTC
                            .CheckedOutByUserName = file.CheckedInUserName
                            .IsDeleted = file.IsDeleted
                            .DeletedByUserName = file.DeletedByUserName
                            .CurrentVersionNumber = file.VersionNumber
                            .IsPermanentlyDeleted = file.IsPermanentlyDeleted
                            .FileVersion = New FileVersionInfo()
                            .FileVersion.DeltaOperation = New DeltaOperationInfo()
                            .FileVersion.DeltaOperation.FileEntryStatusId = file.FileSystemEntryStatusId
                            .FileVersion.DeltaOperation.IsConflicted = file.IsConflicted
                            .FileVersion.DeltaOperation.IsOpen = file.IsOpen
                            .FileVersion.FileEntryStatusDisplayName = file.FileSystemEntryStatusDisplayName
                            .FileVersion.FileVersionId = file.FileSystemEntryVersionId
                            .FileVersion.FileEntryName = file.FileSystemEntryName
                            .FileVersion.FileEntryExtension = file.FileSystemEntryExtension
                            .FileVersion.FileEntryNameWithExtension = file.Name
                            .FileVersion.ServerFileName = file.ServerFileSystemEntryName
                            .FileVersion.FileEntrySize = file.FileSystemEntrySize
                            .FileVersion.ParentFileEntryId = file.ParentFileSystemEntryId
                            .FileVersion.PreviousFileVersionId = file.PreviousFileSystemEntryVersionId
                            .FileVersion.PreviousVersionNumber = file.PrevVersionNumber
                            .FileVersion.VersionNumber = file.VersionNumber

                            .FileVersion.CreatedByUserId = file.CreatedByUserId
                            .FileVersion.CreatedOnUTC = file.CreatedOnUTC
                            .IsOtherConflicts = file.IsOtherConflicts
                        End With
                        fileProxies.Add(f)
                    Next

                    Return BuildResponse(fileProxies)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of List(Of FileEntryInfo))(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get all the files based on search text and tag value
        ''' </summary>
        ''' <param name="search"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [FileSearch](search As FileSearch) As ResultInfo(Of List(Of FileEntryInfo), Status) Implements IFileRepository.FileSearch
            Try
                Using service = GetClientEntity()

                    Dim Taglist = New DataTable()
                    Taglist.Columns.Add("TagName", GetType(String))
                    Taglist.Columns.Add("TagValue", GetType(String))

                    If search.Tags IsNot Nothing Then
                        For Each Tag In search.Tags
                            Taglist.Rows.Add(Tag.TagName, Tag.TagValue)
                        Next
                    End If

                    Dim ParamTaglist = New SqlParameter("@TagList", SqlDbType.Structured)
                    ParamTaglist.Value = Taglist
                    ParamTaglist.TypeName = "dbo.TagList"

                    Dim ParamStartFileId = New SqlParameter("@StartFileId", SqlDbType.UniqueIdentifier)
                    ParamStartFileId.Value = search.StartFileId

                    Dim ParamFileName = New SqlParameter("@FileName", SqlDbType.NVarChar)
                    ParamFileName.Value = search.SearchText

                    Dim ParamIsAdvancedSearch = New SqlParameter("@IsAdvancedSearch", SqlDbType.Bit)
                    ParamIsAdvancedSearch.Value = search.IsAdvancedSearch

                    'Dim TagCount = search.Tags.Count()
                    Dim searchResult = New List(Of FileEntryInfo)

                    Dim result = service.Database.SqlQuery(Of risersoft.shared.portable.Model.FileSearchResult)("exec dbo.usp_SEL_FileSearch @Taglist,@StartFileId,@FileName,@IsAdvancedSearch", ParamTaglist, ParamStartFileId, ParamFileName, ParamIsAdvancedSearch).ToList()



                    For Each file In result
                        Dim f As New FileEntryInfo()
                        With f
                            .FileEntryId = file.FileEntryId
                            .FileEntryTypeId = file.FileEntryTypeId
                            .FileShareId = search.FileShareId
                            .CheckedOutByUserId = file.CheckedOutByUserId

                            .FileVersion = New FileVersionInfo()
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

                            'computed relative path
                            .FileVersion.FileEntryRelativePath = search.SharePath + "\" + file.FileEntryRelativePath

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

        ''' <summary>
        ''' set checked out flag true and checked out user on a file
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <param name="workSpaceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckOut(fileSystemEntryId As Guid, workSpaceId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.CheckOut

            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
                Dim userId As Guid = Me.User.UserId
                Using service = GetClientEntity()
                    Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)

                    If (file IsNot Nothing) Then

                        If Not file.IsCheckedOut Then
                            file.IsCheckedOut = True
                            file.CheckedOutByUserId = userId
                            file.CheckedOutOnUTC = DateTime.UtcNow()
                            file.CheckedOutWorkSpaceId = workSpaceId

                            service.SaveChanges()

                            res.Data = True
                            res.Message = "Operation successfull"
                            res.Status = Status.Success

                        Else
                            'File Already Checked out
                            If file.CheckedOutByUserId = userId Then
                                res.Data = True
                                res.Message = "File Already CheckedOut by Same User"
                                res.Status = Status.Success
                            Else
                                res.Data = False
                                res.Message = "File checked out by a different user"
                                res.Status = Status.FileCheckedOutByDifferentUser
                            End If
                        End If

                    Else
                        res.Data = False
                        res.Message = "File not found"
                        res.Status = Status.NotFound

                    End If
                End Using

                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' set checked out flag false and set checkedout user null, workspace null, version against a file version and then update delta operation of that version
        ''' </summary>
        ''' <param name="fileSystemEntryVersionId"></param>
        ''' <param name="versionNumber"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckIn(fileSystemEntryVersionId As Guid, versionNumber As Int32) As ResultInfo(Of Boolean, Status) Implements IFileRepository.CheckIn
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
                Dim userId As Guid = Me.User.UserId
                Using service = GetClientEntity()
                    Dim fileVersion = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionId)

                    If fileVersion IsNot Nothing Then
                        Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileVersion.FileSystemEntryId)

                        If (file IsNot Nothing) Then
                            file.IsCheckedOut = False
                            file.CheckedOutByUserId = Nothing
                            file.CheckedOutOnUTC = Nothing
                            file.CheckedOutWorkSpaceId = Nothing
                            file.CurrentVersionNumber = versionNumber

                            fileVersion.VersionNumber = versionNumber
                            fileVersion.CreatedByUserId = userId
                            fileVersion.CreatedOnUTC = DateTime.UtcNow

                            Dim oDeltaOperation As DeltaOperation = Nothing

                            If (fileVersion.DeltaOperations IsNot Nothing AndAlso fileVersion.DeltaOperations.Count > 0) Then
                                oDeltaOperation = fileVersion.DeltaOperations.FirstOrDefault()
                                oDeltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                oDeltaOperation.IsOpen = False

                            Else

                                fileVersion.DeltaOperations = New List(Of DeltaOperation)

                                oDeltaOperation = New DeltaOperation()
                                oDeltaOperation.DeltaOperationId = New Guid()
                                oDeltaOperation.FileSystemEntryVersionId = fileVersion.FileSystemEntryVersionId
                                oDeltaOperation.FileSystemEntryId = fileVersion.FileSystemEntryId
                                oDeltaOperation.FileSystemEntryStatusId = CType(Enums.FileEntryStatus.NoActionRequired, Byte)
                                oDeltaOperation.IsOpen = False
                                oDeltaOperation.IsDeleted = False

                                fileVersion.DeltaOperations.Add(oDeltaOperation)
                            End If

                            service.SaveChanges()

                            res.Data = True
                            res.Message = "Operation successfull"
                            res.Status = Status.Success
                        Else
                            res.Data = False
                            res.Message = "File not found"
                            res.Status = Status.NotFound

                        End If
                    Else
                        res.Data = False
                        res.Message = "File Version not found"
                        res.Status = Status.NotFound

                    End If

                End Using

                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' set checked out flag false and set checkedout user and workspace null
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UndoCheckOut(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.UndoCheckOut
            Try
                Dim userId As Guid = Me.User.UserId
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

                Using service = GetClientEntity()
                    Dim file = service.FileSystemEntries.FirstOrDefault(Function(p) p.IsDeleted = False AndAlso p.FileSystemEntryId = fileSystemEntryId)
                    If (file IsNot Nothing) Then
                        If (file.IsCheckedOut = True AndAlso (file.CheckedOutByUserId = userId OrElse Me.User.RoleId = Role.AccountAdmin)) Then
                            file.IsCheckedOut = False
                            file.CheckedOutByUserId = Nothing
                            file.CheckedOutOnUTC = Nothing
                            file.CheckedOutWorkSpaceId = Nothing
                            service.SaveChanges()
                            res.Message = "Operation successfull"
                            res.Status = Status.Success
                            res.Data = True
                        Else
                            If (file.IsCheckedOut = False) Then
                                res.Message = "Operation successfull"
                                res.Status = Status.Success
                                res.Data = True
                            Else
                                res.Message = "File checked out by a different user"
                                res.Status = Status.FileCheckedOutByDifferentUser
                            End If
                        End If
                    Else
                        res.Message = "File not found"
                        res.Status = Status.NotFound
                    End If
                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get all links against a file 
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFileLinks(fileSystemEntryId As Guid) As ResultInfo(Of List(Of FileEntryLinkInfo), Status) Implements IFileRepository.GetFileLinks
            Try
                Dim Links As List(Of FileEntryLinkInfo) = Nothing
                Dim res As ResultInfo(Of List(Of FileEntryLinkInfo), Status) = New ResultInfo(Of List(Of FileEntryLinkInfo), Status)

                Using Service = GetClientEntity()
                    Dim dbLinks = Service.GetAllFileSystemEntryLinks(fileSystemEntryId)

                    If (dbLinks IsNot Nothing AndAlso dbLinks.Count > 1) Then
                        Links = New List(Of FileEntryLinkInfo)()

                        For Each Dblink In dbLinks.Where(Function(p) p.LinkLevel > 0).Select(Function(p) p).OrderBy(Function(p) p.LinkLevel)
                            Links.Add(New FileEntryLinkInfo With {.FileEntryLinkId = Dblink.FileSystemEntryLinkId, .FileEntryId = Dblink.FileSystemEntryId, .PreviousFileEntryId = Dblink.PreviousFileSystemEntryId})
                        Next
                    End If
                End Using

                res.Data = Links
                res.Status = Status.None

                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of List(Of FileEntryLinkInfo))(ex)
            End Try
        End Function

        ''' <summary>
        ''' Remove all records from delta operation against a file
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDeltaOperationsForFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.DeleteDeltaOperationsForFile
            Try
                Dim res = New ResultInfo(Of Boolean, Status)

                Using Service = GetClientEntity()
                    Dim file = Service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId)

                    If (file IsNot Nothing) Then
                        'Delete Delta Opertions
                        Service.DeltaOperations.RemoveRange(Service.DeltaOperations.Where(Function(p) p.FileSystemEntryId = fileSystemEntryId))

                        'Mark file as deleted
                        file.IsDeleted = True
                        file.IsCheckedOut = False
                        file.DeletedByUserId = Me.User.UserId
                        file.DeletedOnUTC = DateTime.UtcNow()
                        file.CheckedOutByUserId = Nothing
                        file.CheckedOutOnUTC = Nothing
                        file.CheckedOutWorkSpaceId = Nothing

                        Service.SaveChanges()

                        res.Data = True
                        res.Message = "Operation Sucessful"
                        res.Status = Status.Success
                    Else
                        res.Data = False
                        res.Message = "File Not Found"
                        res.Status = Status.NotFound
                    End If
                End Using

                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get an unresolved Conflict on a file by a user and workspace
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMyConflict(fileSystemEntryId As Guid) As ResultInfo(Of FileVersionConflictInfo, Status) Implements IFileRepository.GetMyConflict
            Try
                Using service = GetClientEntity()
                    Dim fileSystemEntryVersionConflict = service.FileSystemEntryVersionConflicts.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemEntryId AndAlso Not p.IsResolved AndAlso p.UserId = Me.User.UserId AndAlso p.WorkSpaceId = Me.User.WorkSpaceId)
                    If (fileSystemEntryVersionConflict IsNot Nothing) Then
                        Dim FileVersionConflictInfo As New FileVersionConflictInfo
                        With FileVersionConflictInfo
                            .CreatedOnUTC = fileSystemEntryVersionConflict.CreatedOnUTC
                            .FileEntryId = fileSystemEntryVersionConflict.FileSystemEntryId
                            .FileEntryNameAndExtension = fileSystemEntryVersionConflict.FileSystemEntryNameAndExtension
                            .FileEntryPath = fileSystemEntryVersionConflict.FileSystemEntryPath
                            .FileVersionConflictId = fileSystemEntryVersionConflict.FileSystemEntryVersionConflictId
                            .FileVersionConflictTypeId = fileSystemEntryVersionConflict.FileSystemEntryVersionConflictTypeId
                            .FileVersionId = fileSystemEntryVersionConflict.FileSystemEntryVersionId
                            .IsResolved = fileSystemEntryVersionConflict.IsResolved
                            .UserId = fileSystemEntryVersionConflict.UserId
                            .WorkSpaceId = fileSystemEntryVersionConflict.WorkSpaceId
                            .IsClientUploadRequested = fileSystemEntryVersionConflict.IsUploadRequested
                        End With
                        Return BuildResponse(FileVersionConflictInfo, Status.Success, Nothing)
                    Else
                        Return BuildResponse(Of FileVersionConflictInfo)(Nothing, Status.NotFound, "No unresolved conflict")
                    End If

                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of FileVersionConflictInfo)(ex)
            End Try
        End Function

        ''' <summary>
        ''' Check whether file is previously linked with any other file
        ''' </summary>
        ''' <param name="fileSystemEntryId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsFilePreviouslyLinked(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileRepository.IsFilePreviouslyLinked
            Try
                Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
                res.Data = False
                Using service = GetClientEntity()
                    If (service.FileSystemEntryLinks.FirstOrDefault(Function(p) Not p.IsDeleted AndAlso p.FileSystemEntryId = fileSystemEntryId) Is Nothing) Then
                        res.Data = False
                    Else
                        res.Data = True
                    End If
                End Using
                Return res
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        End Function

    End Class
End Namespace