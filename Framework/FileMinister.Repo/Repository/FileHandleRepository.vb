Imports FileMinister.Repo
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Public Class FileHandleRepository
    Inherits ServerRepositoryBase(Of RSCallerInfo, Integer)
    Implements IFileHandleRepository

    Public Function GetOpenFileHandles(FileEntryId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status) Implements IFileHandleRepository.GetOpenFileHandles
        Try
            Dim result As List(Of FileHandleInfo) = Nothing
            Using service = GetServerEntity()
                Dim fileHandleList = service.FileHandles.Where(Function(f) f.FileEntryId = FileEntryId AndAlso f.ClosedAt Is Nothing)

                If Not fileHandleList Is Nothing AndAlso fileHandleList.Count > 0 Then
                    result = New List(Of FileHandleInfo)()

                    For Each handle As FileHandle In fileHandleList
                        result.Add(MapObject(handle))
                    Next
                End If

                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileHandleInfo))(ex)
        End Try
    End Function

    Public Function GetOpenFileHandlesForWorkspace(WorkspaceId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status) Implements IFileHandleRepository.GetOpenFileHandlesForWorkspace
        Try
            Dim result As List(Of FileHandleInfo) = Nothing
            Using service = GetServerEntity()
                Dim fileHandleList = service.FileHandles.Where(Function(f) f.WorkSpaceId = WorkspaceId AndAlso f.ClosedAt Is Nothing)
                Dim domainId = service.WorkSpaces.Where(Function(p) p.WorkSpaceId = WorkspaceId).Select(Function(p) p.UserDomainId).FirstOrDefault()

                If Not fileHandleList Is Nothing AndAlso fileHandleList.Count > 0 Then
                    result = New List(Of FileHandleInfo)()

                    For Each handle As FileHandle In fileHandleList
                        'Get User SID from UserMappings Table
                        Dim userSID = service.UserMappings.Where(Function(p) p.UserID = handle.UserId AndAlso p.UserDomainID = domainId).Select(Function(p) p.SID).FirstOrDefault()
                        Dim shareId = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = handle.FileEntryId).FileShareId

                        Dim obj = MapObject(handle)
                        obj.FileShareId = shareId

                        If Not String.IsNullOrWhiteSpace(userSID) Then
                            obj.UserSID = userSID
                        End If

                        result.Add(obj)
                    Next
                End If

                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileHandleInfo))(ex)
        End Try
    End Function

    Public Function OpenFileHandle(FileEntryId As Guid, RelativePath As String, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status) Implements IFileHandleRepository.OpenFileHandle
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Using service = GetServerEntity()
                If (service.FileHandles.Where(Function(f) f.FileEntryId = FileEntryId AndAlso f.WorkSpaceId = WorkspaceId AndAlso f.UserId = Me.User.UserAccount.UserId AndAlso f.ClosedAt Is Nothing).ToList().Count > 0) Then
                    res.Data = False
                    res.Message = ServiceConstants.FILE_ALREADY_OPENED
                Else
                    Dim fe = service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = FileEntryId AndAlso f.IsDeleted = False)
                    If (fe Is Nothing) Then
                        res.Data = False
                        res.Message = ServiceConstants.FILE_NOTFOUND
                    Else
                        Dim fh As New FileHandle
                        fh.FileEntryId = FileEntryId
                        fh.UserId = Me.User.UserId
                        fh.WorkSpaceId = WorkspaceId
                        fh.FileRelativePath = RelativePath
                        fh.OpenedAt = DateTime.Now
                        fh.ServerFileSizeAtOpen = ServerFileSize
                        fh.ServerFileTimeAtOpen = ServerFileTime

                        service.FileHandles.Add(fh)
                        If (fe.NumberHandles Is Nothing) Then
                            fe.NumberHandles = 0
                        End If
                        'checkout file if not checked out
                        If (fe.NumberHandles = 0 AndAlso Not fe.IsCheckedOut) Then
                            Dim fr As New FileRepository()
                            fr.fncUser = Me.fncUser
                            fr.CheckOutWithoutVersion(FileEntryId, WorkspaceId)
                        End If
                        'mark flag in fileentry
                        Dim fe1 = service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = FileEntryId)
                        If fe1.NumberHandles Is Nothing Then
                            fe.NumberHandles = 1
                        Else
                            fe.NumberHandles += 1
                        End If

                        service.SaveChanges()

                        res.Data = True
                    End If
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function OpenFileHandle(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status) Implements IFileHandleRepository.OpenFileHandle
        Dim fileRepo As New FileRepository()
        fileRepo.fncUser = Me.fncUser

        Dim result = fileRepo.GetLatestFileEntryVersionByPath(RelativePath, ShareId)

        If result.Status = Status.Success AndAlso result.Data IsNot Nothing Then
            Return OpenFileHandle(result.Data.FileEntryId, RelativePath, WorkspaceId, ServerFileSize, ServerFileTime)

        Else
            Dim res As New ResultInfo(Of Boolean, Status)()
            res.Status = Status.NotFound
            res.Data = False

            Return res
        End If

    End Function

    Public Function CloseFile(FileEntryId As Guid, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status) Implements IFileHandleRepository.CloseFile
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Using service = GetServerEntity()
                Dim fe = service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = FileEntryId AndAlso f.IsDeleted = False)
                If (fe Is Nothing) Then
                    res.Data = False
                    res.Message = ServiceConstants.FILE_NOTFOUND
                Else
                    Dim userId = If(Me.User IsNot Nothing, Me.User.UserId, Guid.Empty)
                    Dim openFileHandles = service.FileHandles.Where(Function(f) f.FileEntryId = FileEntryId _
                        AndAlso f.WorkSpaceId = WorkspaceId _
                        AndAlso ((f.UserId.HasValue AndAlso f.UserId = userId) OrElse ((userId = Guid.Empty) AndAlso (Not f.UserId.HasValue))) _
                        AndAlso f.ClosedAt Is Nothing).ToList()
                    If (openFileHandles.Count = 0) Then
                        res.Data = False
                        res.Message = ServiceConstants.FILE_NOT_OPEN
                    Else
                        For Each fh As FileHandle In openFileHandles
                            fh.ClosedAt = DateTime.Now
                            fh.ServerFileSizeAtClose = ServerFileSize
                            fh.ServerFileTimeAtClose = ServerFileTime

                            If (fh.ServerFileSizeAtOpen = fh.ServerFileSizeAtClose AndAlso fh.ServerFileTimeAtClose = fh.ServerFileTimeAtOpen) Then
                                If (fe.IsCheckedOut AndAlso fe.CheckedOutByUserId = userId AndAlso fe.CheckedOutWorkSpaceId = WorkspaceId) Then
                                    'if checked out by me
                                    If (fe.NumberHandles = 1) Then
                                        'undo checkout if checkout by same user and ws and no other file handle open and filesize and time at open/close matches
                                        fe.IsCheckedOut = False
                                        fe.CheckedOutByUserId = Nothing
                                        fe.CheckedOutOnUTC = Nothing
                                        fe.CheckedOutWorkSpaceId = Nothing
                                    Else
                                        Dim otherFileHandle = service.FileHandles.Where(Function(f) f.FileEntryId = FileEntryId AndAlso (f.WorkSpaceId <> WorkspaceId OrElse f.UserId <> Me.User.UserId) AndAlso f.ClosedAt Is Nothing).ToList().OrderBy(Function(o) o.OpenedAt).ToList()
                                        If (otherFileHandle.Count > 0) Then
                                            'checkout to user and workspace with oldest other open filehandle for that file
                                            fe.CheckedOutByUserId = otherFileHandle(0).UserId
                                            fe.CheckedOutWorkSpaceId = otherFileHandle(0).WorkSpaceId
                                            fe.CheckedOutOnUTC = DateTime.UtcNow
                                        End If
                                    End If
                                End If
                            Else
                                fh.ToBePrcessed = "Y"
                            End If
                            'mark flag in fileentry
                            service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = FileEntryId).NumberHandles -= 1
                        Next
                        service.SaveChanges()
                        res.Data = True
                    End If
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function CloseFile(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status) Implements IFileHandleRepository.CloseFile
        Dim fileRepo As New FileRepository()
        fileRepo.fncUser = Me.fncUser

        Dim result = fileRepo.GetLatestFileEntryVersionByPath(RelativePath, ShareId)

        If result.Status = Status.Success AndAlso result.Data IsNot Nothing Then
            Return CloseFile(result.Data.FileEntryId, WorkspaceId, ServerFileSize, ServerFileTime)

        Else
            Dim res As New ResultInfo(Of Boolean, Status)()
            res.Status = Status.NotFound
            res.Data = False

            Return res
        End If

    End Function

    Public Function GetToBeProcessedFileHandles(FileEntryId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status) Implements IFileHandleRepository.GetToBeProcessedFileHandles
        Try
            Dim result As List(Of FileHandleInfo) = Nothing
            Using service = GetServerEntity()
                Dim fileHandleList = service.FileHandles.Where(Function(f) f.FileEntryId = FileEntryId AndAlso f.ToBePrcessed = "Y")

                If Not fileHandleList Is Nothing AndAlso fileHandleList.Count > 0 Then
                    result = New List(Of FileHandleInfo)()

                    For Each handle As FileHandle In fileHandleList
                        result.Add(MapObject(handle))
                    Next
                End If

                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileHandleInfo))(ex)
        End Try
    End Function

    Public Function MarkProcessedFileHandles(FileHandleId As Int32, WorkspaceId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileHandleRepository.MarkProcessedFileHandles
        Try
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Using service = GetServerEntity()
                Dim fileHandle = service.FileHandles.FirstOrDefault(Function(f) f.FileHandleId = FileHandleId)
                If (fileHandle Is Nothing) Then
                    res.Data = False
                    res.Message = ServiceConstants.FILE_NOTFOUND
                Else
                    fileHandle.ToBePrcessed = False
                    Dim otherFileHandle = service.FileHandles.Where(Function(f) f.FileEntryId = fileHandle.FileEntryId AndAlso (f.WorkSpaceId <> WorkspaceId OrElse f.UserId <> Me.User.UserId) AndAlso f.ClosedAt Is Nothing).ToList().OrderBy(Function(o) o.OpenedAt).ToList()
                    If (otherFileHandle.Count > 0) Then
                        Dim fe = service.FileEntries.FirstOrDefault(Function(f) f.FileEntryId = fileHandle.FileEntryId AndAlso f.IsDeleted = False)
                        If (fe.IsCheckedOut AndAlso fe.CheckedOutByUserId = Me.User.UserId AndAlso fe.CheckedOutWorkSpaceId = WorkspaceId) Then
                            'checkout to user and workspace with oldest other open filehandle for that file
                            fe.CheckedOutByUserId = otherFileHandle(0).UserId
                            fe.CheckedOutWorkSpaceId = otherFileHandle(0).WorkSpaceId
                            fe.CheckedOutOnUTC = DateTime.UtcNow
                        End If
                    End If
                    service.SaveChanges()
                    res.Data = True
                End If
            End Using
            Return res
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Private Function MapObject(handle As FileHandle) As FileHandleInfo
        Return New FileHandleInfo With {
            .ClosedAt = handle.ClosedAt,
            .FileEntryId = handle.FileEntryId,
            .FileHandleId = handle.FileHandleId,
            .OpenedAt = handle.OpenedAt,
            .ServerFileSizeAtClose = handle.ServerFileSizeAtClose,
            .ServerFileSizeAtOpen = handle.ServerFileSizeAtOpen,
            .ServerFileTimeAtClose = handle.ServerFileTimeAtClose,
            .ServerFileTimeAtOpen = handle.ServerFileTimeAtOpen,
            .TenantID = handle.TenantID,
            .ToBePrcessed = handle.ToBePrcessed,
            .UserId = handle.UserId,
            .WorkSpaceId = handle.WorkSpaceId,
            .FileRelativePath = handle.FileRelativePath
            }
    End Function
End Class
