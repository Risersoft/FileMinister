Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable
Imports risersoft.shared.portable.Enums

Public Class FileVersionRepository
    Inherits ClientRepositoryBase(Of FileVersionInfo, Guid)
    Implements IFileVersionRepository

    ''' <summary>
    ''' Get File Version by id
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Guid) As ResultInfo(Of FileVersionInfo, Status)
        Using service = GetClientEntity()
            Dim data = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = id)
            Dim result As New ResultInfo(Of FileVersionInfo, Status)()
            If data IsNot Nothing Then
                result.Status = Status.Success
                result.Data = MapToObject(data)
            Else
                result.Status = Status.Success
            End If
            Return result
        End Using
    End Function

    ''' <summary>
    ''' Add/Update FileSystem Version with DelatOperations
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetClientEntity()
                ' check if file already exists
                Dim fileSystemLatestVersion = service.GetLatestFileSystemEntryVersionByPath(data.FileEntryOldRelativePath, data.FileEntry.FileShareId).FirstOrDefault()
                Dim shareTypeId = CType(FileType.Share, Short)
                Dim relativePath = Helper.GetParentRelativePath(data.FileEntryOldRelativePath)

                If fileSystemLatestVersion Is Nothing Then 'file not found so it's a new file

                    Dim parentFileSystemEntryId As Guid? = Nothing

                    'if relative path is \ then treat file entry for share as parent
                    'else get the actual parent
                    If relativePath = "" Then
                        Dim shareFile = service.FileSystemEntries.FirstOrDefault(Function(p) p.ShareId = data.FileEntry.FileShareId AndAlso p.FileSystemEntryTypeId = shareTypeId)
                        If shareFile IsNot Nothing Then
                            parentFileSystemEntryId = shareFile.FileSystemEntryId
                        End If
                    Else
                        Dim parent = service.GetLatestFileSystemEntryVersionByPath(relativePath, data.FileEntry.FileShareId).FirstOrDefault()
                        If parent IsNot Nothing Then
                            parentFileSystemEntryId = parent.FileSystemEntryId
                        End If
                    End If

                    If parentFileSystemEntryId.HasValue = False Then
                        Throw New Exception("Parent not found")
                    End If

                    Dim fileSystemEntry As New FileSystemEntry()
                    Dim fileSystemEntryVersion As New FileSystemEntryVersion()
                    Dim deltaOperation As New DeltaOperation()

                    fileSystemEntry.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                    fileSystemEntryVersion.DeltaOperations.Add(deltaOperation)

                    With fileSystemEntry
                        .FileSystemEntryId = Guid.NewGuid()
                        .FileSystemEntryTypeId = data.FileEntry.FileEntryTypeId
                        .ShareId = data.FileEntry.FileShareId
                        .CurrentVersionNumber = 0
                    End With

                    Dim fileSystemEntryVersionId = Guid.NewGuid()
                    With fileSystemEntryVersion
                        .FileSystemEntryVersionId = fileSystemEntryVersionId
                        .ParentFileSystemEntryId = parentFileSystemEntryId.Value
                        .FileSystemEntryExtension = data.FileEntryExtension
                        .FileSystemEntryHash = data.FileEntryHash
                        .FileSystemEntryName = data.FileEntryName
                        '.FileSystemEntryNameWithExtension = data.FileSystemEntryNameWithExtension
                        .FileSystemEntryRelativePath = data.FileEntryRelativePath
                        .FileSystemEntrySize = data.FileEntrySize
                        .ServerFileSystemEntryName = fileSystemEntryVersionId
                        .CreatedByUserId = Me.User.UserId
                        .CreatedOnUTC = DateTime.UtcNow
                    End With

                    With deltaOperation
                        .DeltaOperationId = Guid.NewGuid()
                        .FileSystemEntryStatusId = data.DeltaOperation.FileEntryStatusId
                        .LocalCreatedOnUTC = DateTime.UtcNow
                        .LocalFileSystemEntryExtension = data.DeltaOperation.LocalFileEntryExtension
                        .LocalFileSystemEntryName = data.DeltaOperation.LocalFileEntryName
                        .FileSystemEntry = fileSystemEntry
                    End With

                    service.FileSystemEntries.Add(fileSystemEntry)
                Else 'file found
                    'if version number is null or file upload is not started yet
                    'then update the existing version and delta object
                    ' with file names as it may be a case of file rename
                    If fileSystemLatestVersion.VersionNumber.HasValue = False Or fileSystemLatestVersion.VersionNumber = 0 Or fileSystemLatestVersion.FileSystemEntryStatusId = Enums.FileEntryStatus.NewModified Or fileSystemLatestVersion.FileSystemEntryStatusId = Enums.FileEntryStatus.PendingUpload Then
                        Dim fileSystemEntryVersion = service.FileSystemEntryVersions.Include("FileSystemEntry").FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemLatestVersion.FileSystemEntryVersionId)

                        With fileSystemEntryVersion
                            .FileSystemEntryExtension = data.FileEntryExtension
                            .FileSystemEntryHash = data.FileEntryHash
                            .FileSystemEntryName = data.FileEntryName
                            .FileSystemEntryNameWithExtension = data.FileEntryNameWithExtension
                            .FileSystemEntryRelativePath = data.FileEntryRelativePath
                            .FileSystemEntrySize = data.FileEntrySize
                            .CreatedByUserId = Me.User.UserId
                            .CreatedOnUTC = DateTime.UtcNow
                        End With

                        Dim deltaOperation = fileSystemEntryVersion.DeltaOperations.FirstOrDefault()

                        With deltaOperation
                            .LocalCreatedOnUTC = DateTime.UtcNow
                            .LocalFileSystemEntryExtension = data.DeltaOperation.LocalFileEntryExtension
                            .LocalFileSystemEntryName = data.DeltaOperation.LocalFileEntryName
                        End With

                    Else 'file upload has aleady been started, so need to insert new records for version and delta
                        Dim parentFileSystemEntryId As Guid? = Nothing

                        If relativePath = "\" Then
                            Dim shareFile = service.FileSystemEntries.FirstOrDefault(Function(p) p.ShareId = data.FileEntry.FileShareId AndAlso p.FileSystemEntryTypeId = shareTypeId)
                            If shareFile IsNot Nothing Then
                                parentFileSystemEntryId = shareFile.FileSystemEntryId
                            End If
                        Else
                            Dim parent = service.GetLatestFileSystemEntryVersionByPath(relativePath, data.FileEntry.FileShareId).FirstOrDefault()
                            If parent IsNot Nothing Then
                                parentFileSystemEntryId = parent.FileSystemEntryId
                            End If
                        End If

                        If parentFileSystemEntryId.HasValue = False Then
                            Throw New Exception("Parent not found")
                        End If

                        Dim fileSystemEntry = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemLatestVersion.FileSystemEntryId)
                        Dim fileSystemEntryVersion As New FileSystemEntryVersion()
                        Dim deltaOperation As New DeltaOperation()

                        fileSystemEntry.FileSystemEntryVersions.Add(fileSystemEntryVersion)
                        fileSystemEntryVersion.DeltaOperations.Add(deltaOperation)

                        Dim fileSystemEntryVersionId = Guid.NewGuid()
                        With fileSystemEntryVersion
                            .FileSystemEntryVersionId = fileSystemEntryVersionId
                            .PreviousFileSystemEntryVersionId = fileSystemLatestVersion.FileSystemEntryVersionId
                            .ParentFileSystemEntryId = parentFileSystemEntryId.Value
                            .FileSystemEntryExtension = data.FileEntryExtension
                            .FileSystemEntryHash = data.FileEntryHash
                            .FileSystemEntryName = data.FileEntryName
                            .FileSystemEntryNameWithExtension = data.FileEntryNameWithExtension
                            .FileSystemEntryRelativePath = data.FileEntryRelativePath
                            .FileSystemEntrySize = data.FileEntrySize
                            .ServerFileSystemEntryName = fileSystemEntryVersionId
                            .CreatedByUserId = Me.User.UserId
                            .CreatedOnUTC = DateTime.UtcNow
                        End With

                        With deltaOperation
                            .DeltaOperationId = Guid.NewGuid()
                            .FileSystemEntryStatusId = data.DeltaOperation.FileEntryStatusId
                            .LocalCreatedOnUTC = DateTime.UtcNow
                            .LocalFileSystemEntryExtension = data.DeltaOperation.LocalFileEntryExtension
                            .LocalFileSystemEntryName = data.DeltaOperation.LocalFileEntryName
                            .FileSystemEntry = fileSystemEntry
                        End With
                    End If
                End If

                service.SaveChanges()

                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all file version against a file
    ''' </summary>
    ''' <param name="fileid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [GetAllFileVersions](fileid As Guid) As ResultInfo(Of List(Of FileVersionInfo), Status) Implements IFileVersionRepository.GetAllFileVersions
        Try
            Using service = GetClientEntity()

                Dim lstGuid As List(Of Guid) = New List(Of Guid)
                lstGuid.Add(fileid)

                Dim sharePath As String = ""

                Dim objListFileVersions = New List(Of FileVersionInfo)

                Dim shareId = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileid).ShareId
                Using commonService = GetClientCommonEntity()
                    sharePath = commonService.UserShares.FirstOrDefault(Function(p) p.ShareId = shareId AndAlso p.UserAccountId = Me.User.UserAccountId).SharePath
                End Using

                Dim resultObjLinksVersionJoin = From a In service.GetAllFileSystemEntryLinks(fileid)
                                                Join FV In service.FileSystemEntryVersions
                                                On a.PreviousFileSystemEntryId Equals FV.FileSystemEntryId
                                                Join c In service.Users
                                                On FV.CreatedByUserId Equals c.UserId
                                                Where FV.IsDeleted = False AndAlso FV.VersionNumber IsNot Nothing
                                                Select FV, c.UserName
                                                Order By FV.FileSystemEntryId, FV.VersionNumber Descending

                For Each lnkVersion In resultObjLinksVersionJoin.ToList()
                    Dim obj = New FileVersionInfo
                    obj.FileVersionId = lnkVersion.FV.FileSystemEntryVersionId
                    obj.FileEntryId = lnkVersion.FV.FileSystemEntryId
                    obj.VersionNumber = lnkVersion.FV.VersionNumber
                    obj.FileEntryRelativePath = System.IO.Path.Combine(sharePath + "\" + lnkVersion.FV.FileSystemEntryRelativePath)
                    obj.CreatedByUserId = lnkVersion.FV.CreatedByUserId
                    obj.CreatedOnUTC = lnkVersion.FV.CreatedOnUTC.ToLocalTime()
                    obj.User = lnkVersion.UserName
                    obj.FileShareId = shareId
                    obj.ServerFileName = lnkVersion.FV.ServerFileSystemEntryName

                    If (lstGuid.Exists(Function(p) p.ToString() = obj.FileEntryId.ToString())) Then
                        obj.showDelink = 0
                    Else
                        obj.showDelink = 1
                        lstGuid.Add(obj.FileEntryId)
                    End If
                    objListFileVersions.Add(obj)
                Next


                'Dim resultObjLinks = service.GetAllFileSystemEntryLinks(fileid)
                'For Each lnk In resultObjLinks
                '    Dim resultObj = service.FileSystemEntryVersions.Where(Function(p) p.FileSystemEntryId = lnk.PreviousFileSystemEntryId AndAlso p.IsDeleted = False).OrderByDescending(Function(x) x.VersionNumber).ToList()
                '    For Each element In resultObj
                '        Dim obj = New FileVersionInfo
                '        obj.FileSystemEntryVersionId = element.FileSystemEntryVersionId
                '        obj.FileSystemEntryId = element.FileSystemEntryId
                '        obj.VersionNumber = element.VersionNumber
                '        ''obj.Path = element.FileSystemEntryPathRelativeToShare
                '        obj.CreatedByUserId = element.CreatedByUserId
                '        obj.CreatedOnUTC = element.CreatedOnUTC.ToLocalTime()
                '        If (lstGuid.Exists(Function(p) p.ToString() = obj.FileSystemEntryId.ToString())) Then
                '            obj.showDelink = 0
                '        Else
                '            obj.showDelink = 1
                '            lstGuid.Add(obj.FileSystemEntryId)
                '        End If
                '        objListFileVersions.Add(obj)
                '    Next
                'Next
                Return BuildResponse(objListFileVersions)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileVersionInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Set IsDeleted true against a file version
    ''' </summary>
    ''' <param name="Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFileVersion(Id As Guid) As ResultInfo(Of Boolean, Status) Implements IFileVersionRepository.DeleteFileVersion
        Try
            Using service = GetClientEntity()
                Dim obj = New FileVersionInfo
                Dim userfilepermission = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = Id)
                If userfilepermission IsNot Nothing Then
                    userfilepermission.DeletedByUserId = Me.User.UserId
                    userfilepermission.DeletedOnUTC = DateTime.UtcNow()
                    userfilepermission.IsDeleted = True
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete File by file path
    ''' </summary>
    ''' <param name="shareId"></param>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFileByFilename(shareId As Integer, filename As String) As ResultInfo(Of Boolean, Status) Implements IFileVersionRepository.DeleteFileByFilename
        Try
            Using service = GetClientEntity()
                Dim fileSystemLatestVersion = service.GetLatestFileSystemEntryVersionByPath(filename, shareId).FirstOrDefault()
                If fileSystemLatestVersion IsNot Nothing Then

                    If fileSystemLatestVersion.VersionNumber.HasValue = False Then

                        If service.FileSystemEntryVersions.Count(Function(p) p.FileSystemEntryId = fileSystemLatestVersion.FileSystemEntryId) > 1 Then
                            Dim fileSystemEntryVersion = service.FileSystemEntryVersions.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemLatestVersion.FileSystemEntryVersionId)
                            Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemLatestVersion.FileSystemEntryVersionId)

                            service.FileSystemEntryVersions.Remove(fileSystemEntryVersion)
                            service.DeltaOperations.Remove(deltaOperation)
                        Else
                            Dim fileSystemEntry = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemLatestVersion.FileSystemEntryId)

                            service.DeltaOperations.RemoveRange(fileSystemEntry.DeltaOperations)
                            service.FileSystemEntryVersions.RemoveRange(fileSystemEntry.FileSystemEntryVersions)
                            service.FileSystemEntries.Remove(fileSystemEntry)
                        End If

                    Else
                        Dim fileSystemEntry = service.FileSystemEntries.FirstOrDefault(Function(p) p.FileSystemEntryId = fileSystemLatestVersion.FileSystemEntryId)

                        fileSystemEntry.DeletedByUserId = Me.User.UserId
                        fileSystemEntry.DeletedOnUTC = DateTime.UtcNow
                        fileSystemEntry.IsDeleted = True

                        Dim deltaOperation = service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemLatestVersion.FileSystemEntryVersionId)
                        deltaOperation.FileSystemEntryStatusId = Enums.FileEntryStatus.PendingFileSystemEntryDelete.ToString("D")
                        deltaOperation.DeletedByUserId = Me.User.UserId
                        deltaOperation.DeletedOnUTC = DateTime.UtcNow
                        deltaOperation.IsDeleted = True

                    End If
                    service.SaveChanges()
                    Return BuildResponse(True)
                Else
                    Return BuildResponse(False)
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Update File Status in DelatOperation
    ''' </summary>
    ''' <param name="fileSystemEntryVersionId"></param>
    ''' <param name="fileSystemEntryStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateFileVersionStatus(fileSystemEntryVersionId As Guid, fsEntryStatus As Enums.FileEntryStatus) As ResultInfo(Of Boolean, Status) Implements IFileVersionRepository.UpdateFileVersionStatus
        Try
            Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)

            Using Service = GetClientEntity()

                Dim delta = Service.DeltaOperations.FirstOrDefault(Function(p) p.FileSystemEntryVersionId = fileSystemEntryVersionId)

                If delta IsNot Nothing Then
                    If ((Not delta.IsOpen AndAlso fsEntryStatus = Enums.FileEntryStatus.Uploading) Or fsEntryStatus <> Enums.FileEntryStatus.Uploading) Then
                        delta.FileSystemEntryStatusId = Convert.ToByte(fsEntryStatus)

                        Service.SaveChanges()

                        result.Data = True
                        result.Message = "Operation Successful"
                        result.Status = Enums.Status.Success

                    Else
                        result.Data = False
                        result.Message = "File is Open"
                        result.Status = Enums.Status.FileOpen
                    End If
                Else
                    result.Data = False
                    result.Message = "Record Not Found"
                    result.Status = Status.NotFound
                End If

            End Using
            Return result
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Map entity to model
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As FileSystemEntryVersion) As FileVersionInfo
        Dim t = New FileVersionInfo()
        With t
            .FileEntryId = s.FileSystemEntryId
            .FileEntryName = s.FileSystemEntryName
            .FileEntryNameWithExtension = s.FileSystemEntryNameWithExtension
            .FileEntryRelativePath = s.FileSystemEntryRelativePath
            .FileEntrySize = s.FileSystemEntrySize
            .FileVersionId = s.FileSystemEntryVersionId
            .ParentFileEntryId = s.ParentFileSystemEntryId
            .PreviousFileVersionId = s.PreviousFileSystemEntryVersionId
            .VersionNumber = s.VersionNumber
        End With
        Return t
    End Function
End Class
