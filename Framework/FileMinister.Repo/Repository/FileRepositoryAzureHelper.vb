Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage.File
Imports Model = FileMinister.Models.Sync
Imports System.Configuration

Partial Public Class FileRepository
    ''' <summary>
    ''' Copy and Delete file from Azure File Storage
    ''' </summary>
    ''' <param name="SourceFileEntryId">Id of the Source File</param>
    ''' <param name="FileEntryVersionsInfo">File Version info</param>
    ''' <returns>ResultInfo Object with Success or Failure</returns>
    Private Function CopyAndDeleteFileFromFileShare(SourceFileEntryId As Guid, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)() With {.Data = False}
        Try
            Dim result = Me.CopyFromBlobToFile(FileEntryVersionsInfo:=FileEntryVersionsInfo)
            If (result.Status = Status.Error) Then
                'res.Data = False
                'res.Message = result.Message
                'res.Status = result.Status
                'Return res
                Return result
            End If

            Dim shareId As Short
            If (FileEntryVersionsInfo.FileEntry Is Nothing) Then
                shareId = FileEntryVersionsInfo.FileShareId
            Else
                shareId = FileEntryVersionsInfo.FileEntry.FileShareId
            End If

            'Delete from azure file, if done successfully, in that case do the soft delete from db and then continue
            res = Util.Helper.DeleteFile(FileEntryId:=SourceFileEntryId, ShareName:=String.Empty, FileShareId:=shareId, user:=Me.User)
            Return res

        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Copy file from Blob storage to File Storage (Azure)
    ''' </summary>
    ''' <param name="FileEntryVersionsInfo"></param>
    ''' <returns></returns>
    Private Function CopyFromBlobToFile(FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status) 'ResultInfo(Of Integer, Status)

        Dim repository As New SyncRepository()
        repository.fncUser = Me.fncUser
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False}
        Dim cloudFile As CloudFile = Nothing
        Dim cloudBlockBlob As CloudBlockBlob = Nothing
        Dim sharedAccessSignatureUrl As Uri = Nothing
        Dim startCopyId As String = String.Empty
        Dim msg As String = String.Empty

        Try
            Dim shareId As Short
            If (FileEntryVersionsInfo.FileEntry Is Nothing) Then
                shareId = FileEntryVersionsInfo.FileShareId
            Else
                shareId = FileEntryVersionsInfo.FileEntry.FileShareId
            End If

            cloudBlockBlob = Util.Helper.GetBlockBlobReference(FileShareId:=shareId, BlobName:=FileEntryVersionsInfo.ServerFileName, repository:=repository)
            sharedAccessSignatureUrl = Util.Helper.GetSharedAccessSignatureUrl(CloudBlockBlob:=cloudBlockBlob, SharedAccessSignatureType:=SharedAccessSignatureType.Upload, BlobName:=FileEntryVersionsInfo.ServerFileName, FileShareId:=shareId, RSCallerInfo:=repository.User)

            'Get File Reference
            cloudFile = Util.Helper.GetFileReference(FileEntryVersionsInfo, repository:=repository)

            'copy only if blob exists
            If (cloudBlockBlob.Exists()) Then
                'Copy a blob to file
                'startCopyId = cloudFile.StartCopy(sharedAccessSignatureUrl)
                cloudFile.StartCopyAsync(sharedAccessSignatureUrl).Wait()
                cloudFile.FetchAttributes()
                While (cloudFile.CopyState.Status = CopyStatus.Pending)
                    Threading.Thread.Sleep(50)
                    cloudFile.FetchAttributes()
                End While

                Try
                    repository.fncUser = Me.fncUser
                    With cloudFile
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyIsUploadFinished"), value:=Boolean.TrueString)
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyFileName"), value:=IIf(FileEntryVersionsInfo.FileEntryNameWithExtension Is Nothing, FileEntryVersionsInfo.FileEntryName + FileEntryVersionsInfo.FileEntryExtension, FileEntryVersionsInfo.FileEntryNameWithExtension))
                        Dim currentVersionNumber = FileEntryVersionsInfo.VersionNumber + 1
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyVersionNumber"), value:=currentVersionNumber.ToString())
                        .SetMetadata()
                    End With
                Catch ex As Exception
                    msg = String.Format(" but file Sucessfully Checked-in, but failed to set metadata on Azure with Error: {0}", ex.Message)
                End Try

            End If

            result.Data = True
            result.Message = ServiceConstants.OPERATION_SUCCESSFUL + msg
            result.Status = Status.Success

        Catch ex As Exception

            If ((cloudFile IsNot Nothing) AndAlso cloudFile.CopyState IsNot Nothing AndAlso cloudFile.CopyState.Status = CopyStatus.Pending) Then
                cloudFile.AbortCopy(startCopyId)
            End If
            result.Data = False
            result.Message = ex.Message
            result.Status = Status.Error

        End Try

        Return result

    End Function

    ''' <summary>
    ''' Copy file from Blob storage to File Storage (Azure)
    ''' </summary>
    ''' <param name="FileEntryVersionsInfo"></param>
    ''' <returns></returns>
    Private Function CopyFromFileToBlob(FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status) 'ResultInfo(Of Integer, Status)

        Dim repository As New SyncRepository()
        repository.fncUser = Me.fncUser
        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Data = False}
        Dim cloudFile As CloudFile = Nothing
        Dim cloudBlockBlob As CloudBlockBlob = Nothing
        Dim sharedAccessSignatureUrl As Uri = Nothing
        Dim startCopyId As String = String.Empty
        Dim msg As String = String.Empty

        Try

            Dim shareId As Short
            If (FileEntryVersionsInfo.FileEntry Is Nothing) Then
                shareId = FileEntryVersionsInfo.FileShareId
            Else
                shareId = FileEntryVersionsInfo.FileEntry.FileShareId
            End If

            Dim StorageConnectionString As String = String.Empty
            Dim ContainerName As String = String.Empty
            repository.GetStorageConnectionStringAndContainerName(shareId, StorageConnectionString, ContainerName)

            cloudBlockBlob = Util.Helper.GetBlockBlobReference(StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, BlobName:=FileEntryVersionsInfo.ServerFileName)

            'Get File Reference
            cloudFile = Util.Helper.GetFileReference(StorageConnectionString:=StorageConnectionString, ShareName:=ContainerName, FolderName:=Util.Helper.GetParentRelativePath(FileEntryVersionsInfo.FileEntryRelativePath), FileName:=FileEntryVersionsInfo.FileEntryNameWithExtension)
            'cloudFile = Util.Helper.GetFileReference(FileEntryVersionsInfo, repository:=repository)
            sharedAccessSignatureUrl = Util.Helper.GetSharedAccessSignatureUrlForFile(CloudFile:=cloudFile, SharedAccessSignatureType:=SharedAccessSignatureType.Download, FileShareId:=shareId, RSCallerInfo:=repository.User)

            'copy only if blob exists
            If (cloudFile.Exists()) Then
                'Copy a blob to file
                'startCopyId = cloudFile.StartCopy(sharedAccessSignatureUrl)
                cloudBlockBlob.StartCopyAsync(sharedAccessSignatureUrl).Wait()
                cloudBlockBlob.FetchAttributes()
                While (cloudBlockBlob.CopyState.Status = CopyStatus.Pending)
                    Threading.Thread.Sleep(50)
                    cloudBlockBlob.FetchAttributes()
                End While

                Try
                    With cloudBlockBlob
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyIsUploadFinished"), value:=Boolean.TrueString)
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyFileName"), value:=IIf(FileEntryVersionsInfo.FileEntryNameWithExtension Is Nothing, FileEntryVersionsInfo.FileEntryName + FileEntryVersionsInfo.FileEntryExtension, FileEntryVersionsInfo.FileEntryNameWithExtension))
                        Dim currentVersionNumber = FileEntryVersionsInfo.VersionNumber
                        .Metadata.Add(key:=ConfigurationManager.AppSettings(name:="MetadataKeyVersionNumber"), value:=currentVersionNumber.ToString())
                        .SetMetadata()
                    End With
                Catch ex As Exception
                    msg = String.Format(" but file Sucessfully Checked-in, but failed to set metadata on Azure with Error: {0}", ex.Message)
                End Try

            End If

            result.Data = True
            result.Message = ServiceConstants.OPERATION_SUCCESSFUL + msg
            result.Status = Status.Success

        Catch ex As Exception

            If ((cloudFile IsNot Nothing) AndAlso cloudFile.CopyState IsNot Nothing AndAlso cloudFile.CopyState.Status = CopyStatus.Pending) Then
                cloudFile.AbortCopy(startCopyId)
            End If
            result.Data = False
            result.Message = ex.Message
            result.Status = Status.Error

        End Try

        Return result

    End Function

    ''' <summary>
    ''' Delete Folders and files from Azure File Storage
    ''' </summary>
    ''' <param name="Service">FileMinisterEntities</param>
    ''' <param name="FileEntry">An object that has file entry information</param>
    ''' <returns>ResultInfo Object with Success or failure</returns>
    Private Function DeleteFolderFileFromAzureFileStorage(Service As FileMinisterEntities, FileEntry As FileEntry)

        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
        Dim shareName As String = String.Empty

        'get undeleted childern
        Dim undelChildren = Service.GetLatestFileVersionChildrenHierarchy(FileEntry.FileEntryId)

        Dim files = (From FE As FileEntry In Service.FileEntries
                     Join a In undelChildren
                     On a.FileEntryId Equals FE.FileEntryId
                     Join FV As FileVersion In Service.FileVersions
                     On FV.FileEntryId Equals FE.FileEntryId
                     Where (FE.IsPermanentlyDeleted = 0 And FV.VersionNumber = FE.CurrentVersionNumber)
                     Select FE, FV.FileEntryRelativePath
            )

        Dim outfiles = files.OrderByDescending(Function(o) Len(o.FileEntryRelativePath)).Select(Function(p) p.FE)

        If (outfiles.FirstOrDefault() IsNot Nothing) Then

            'Dim FileShareId As Integer = outfiles.FirstOrDefault().FileShareId

            For Each FE As FileEntry In outfiles

                If (FE.FileShare IsNot Nothing) Then shareName = FE.FileShare.ShareContainerName
                'Delete first from azure file, if done successfully
                If (FE.FileEntryTypeId = FileType.Folder) Then 'Folder
                    result = Util.Helper.DeleteFolder(FileEntryId:=FE.FileEntryId, ShareName:=shareName, IsDeleted:=FE.IsDeleted, FileShareId:=FE.FileShareId, user:=Me.User)
                    If (result.Status = Status.Error OrElse result.Status = Status.NotFound) Then
                        Return result
                    End If
                Else
                    result = Util.Helper.DeleteFile(FileEntryId:=FE.FileEntryId, ShareName:=shareName, IsDeleted:=FE.IsDeleted, FileShareId:=FE.FileShareId, user:=Me.User)
                    If (result.Status = Status.Error) Then
                        Return result
                    End If
                End If
            Next

        End If

        'Delete selected folder
        If (FileEntry.FileEntryTypeId = FileType.Folder) Then
            result = Util.Helper.DeleteFolder(FileEntryId:=FileEntry.FileEntryId, ShareName:=shareName, IsDeleted:=FileEntry.IsDeleted, FileShareId:=FileEntry.FileShareId, user:=Me.User)
            If (result.Status = Status.Error OrElse result.Status = Status.NotFound) Then
                Return result
            End If
        End If

        result.Data = True
        result.Message = ServiceConstants.OPERATION_SUCCESSFUL
        result.Status = Status.Success

        Return result

    End Function

End Class
