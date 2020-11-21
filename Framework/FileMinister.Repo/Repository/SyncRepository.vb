Imports risersoft.shared.agent
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Proxies

''' <summary>
''' Sync Repository
''' </summary>
''' <remarks></remarks>
Public Class SyncRepository
    Inherits ServerRepositoryBase(Of BaseInfo, Integer)
    Implements ISyncRepository

    Sub GetStorageConnectionStringAndContainerName(FileShareId As Integer, ByRef StorageConnectionString As String, ByRef ContainerName As String) Implements ISyncRepository.GetStorageConnectionStringAndContainerName
        Using service = GetServerEntity()
            Dim FileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = FileShareId AndAlso p.IsDeleted = False)

            If FileShare IsNot Nothing Then
                ContainerName = FileShare.ShareContainerName
            End If
        End Using
        StorageConnectionString = Me.GetStorageAccount
    End Sub

    Sub GetStorageConnectionStringAndContainerNameAndShareId(FileEntryId As Guid, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer) Implements ISyncRepository.GetStorageConnectionStringAndContainerNameAndShareId
        Dim BlobName As Guid = Nothing

        Using service = GetServerEntity()
            Dim query = From fse In service.FileEntries
                        Join s In service.FileShares On fse.FileShareId Equals s.FileShareId
                        Where fse.FileEntryId = FileEntryId AndAlso fse.IsDeleted = False AndAlso s.IsDeleted = False
                        Select s

            Dim FileShare = query.FirstOrDefault()

            If FileShare IsNot Nothing Then
                ContainerName = FileShare.ShareContainerName
                FileShareId = FileShare.FileShareId
            End If
        End Using
        StorageConnectionString = Me.GetStorageAccount
    End Sub

    Sub GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId As Guid, VersionNumber As Integer, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer, ByRef BlobName As Guid) Implements ISyncRepository.GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName
        Using service = GetServerEntity()
            Dim query = From fse In service.FileEntries
                        Join s In service.FileShares On fse.FileShareId Equals s.FileShareId
                        Join fsev In service.FileVersions On fsev.FileEntryId Equals fse.FileEntryId
                        Where fse.FileEntryId = FileEntryId AndAlso fse.IsDeleted = False AndAlso s.IsDeleted = False AndAlso fsev.IsDeleted = False AndAlso fsev.VersionNumber = VersionNumber
                        Select s, fsev

            Dim ss = query.FirstOrDefault()

            If ss IsNot Nothing Then
                ContainerName = ss.s.ShareContainerName
                BlobName = ss.fsev.ServerFileName
                FileShareId = ss.s.FileShareId
            End If
        End Using
        StorageConnectionString = Me.GetStorageAccount
    End Sub

    Public Function SyncServerData(users As List(Of UserAccountProxy), groups As List(Of GroupDefinitionProxy)) As ResultInfo(Of Boolean, Status) Implements ISyncRepository.SyncServerData
        Try
            Dim toBeDeletedGroupIds = ServiceAuthSyncProvider.SyncUsersGroups(Me.User, users, groups)
            Using service = GetServerEntity()
                For Each gfsepa In service.GroupFileEntryPermissions.Where(Function(p) toBeDeletedGroupIds.Contains(p.GroupId))
                    gfsepa.IsDeleted = True
                    gfsepa.DeletedByUserId = Me.User.UserId
                    gfsepa.DeletedOnUTC = DateTime.UtcNow
                Next

                service.SaveChanges()
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Trace.WriteLine("Sync users exception: " & ex.Message)
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' This method will return ConnectionString (Azure Blob) and File Entry Info such as fileName,FolderName
    ''' </summary>
    ''' <param name="FileEntryId"></param>
    ''' <param name="StorageConnectionString">Store the Connection string</param>
    ''' <param name="ContainerName">Store the container name</param>
    ''' <param name="FolderName">Store the folder name</param>
    ''' <param name="FileName">Store the file entry name</param>
    ''' <param name="FileShareId">Optional share id</param>
    Public Sub GetStorageConnectionStringAndFileEntryInfo(FileEntryId As Guid, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FolderName As String, ByRef FileName As String, Optional ByRef FileShareId As Integer = -1) Implements ISyncRepository.GetStorageConnectionStringAndFileEntryInfo

        Using service = GetServerEntity()

            Dim fileInfo = service.GetLatestFileVersion(FileEntryId).FirstOrDefault()

            Dim fileShare As FileShare = Nothing

            If (String.IsNullOrWhiteSpace(ContainerName)) Then
                If (FileShareId > 0) Then
                    Dim shareId = FileShareId ' creating local variable as ByRef variable cannot be used in lambda expression
                    fileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = shareId AndAlso p.IsDeleted = False)
                    If fileShare IsNot Nothing Then
                        ContainerName = fileShare.ShareContainerName
                    End If

                Else
                    Dim fileShareResult = service.GetFileEntryShare(FileEntryId).FirstOrDefault()
                    If fileShareResult IsNot Nothing Then
                        FileShareId = fileShareResult.FileShareId
                        ContainerName = fileShare.ShareName
                    End If

                End If
            End If


            If fileInfo IsNot Nothing Then
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = fileInfo.FileEntryId)
                If (file.FileEntryTypeId = FileType.File) Then
                    FileName = fileInfo.FileEntryNameWithExtension
                    Dim lastIndex = fileInfo.FileEntryRelativePath.LastIndexOf("\")
                    If (lastIndex > -1) Then
                        FolderName = fileInfo.FileEntryRelativePath.Substring(0, lastIndex)
                    Else
                        FolderName = ""
                    End If
                ElseIf (file.FileEntryTypeId = FileType.Folder) Then
                    FolderName = fileInfo.FileEntryRelativePath
                End If
            End If

        End Using

        StorageConnectionString = Me.GetStorageAccount

    End Sub

    ''' <summary>
    ''' This method will return ConnectionString (Azure Blob) and File Entry Info such as fileName,FolderName
    ''' </summary>
    ''' <param name="FileEntryId"></param>
    ''' <param name="IsDeleted"></param>
    ''' <param name="StorageConnectionString">Store the Connection string</param>
    ''' <param name="ContainerName">Store the container name</param>
    ''' <param name="FolderName">Store the folder name</param>
    ''' <param name="FileName">Store the file entry name</param>
    ''' <param name="FileShareId">Optional share id</param>
    Public Sub GetStorageConnectionStringAndFileEntryInfo(FileEntryId As Guid, IsDeleted As Boolean, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FolderName As String, ByRef FileName As String, Optional ByRef FileShareId As Integer = -1) Implements ISyncRepository.GetStorageConnectionStringAndFileEntryInfo

        Using service = GetServerEntity()

            Dim query = (From fe In service.FileEntries
                         Join fv In service.FileVersions On fe.FileEntryId Equals fv.FileEntryId
                         Where fe.FileEntryId = FileEntryId AndAlso fe.IsDeleted = IsDeleted 'AndAlso fv.IsDeleted = IsDeleted
                         Select fe, fv).OrderByDescending(Function(o) o.fv.VersionNumber)

            Dim FileInfo = query.FirstOrDefault()
            Dim fileShare As FileShare = Nothing

            If (FileShareId > 0) Then
                Dim shareId = FileShareId ' creating local variable as ByRef variable cannot be used in lambda expression
                fileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = shareId AndAlso p.IsDeleted = False)
                If fileShare IsNot Nothing Then
                    ContainerName = fileShare.ShareContainerName
                End If
            Else
                Dim fileShareResult = service.GetFileEntryShare(FileEntryId).FirstOrDefault()
                If fileShareResult IsNot Nothing Then
                    FileShareId = fileShareResult.FileShareId
                    ContainerName = fileShareResult.ShareContainerName
                End If
            End If

            'We will get Filename when we move the file to some other folder
            If FileInfo IsNot Nothing Then
                Dim file = service.FileEntries.FirstOrDefault(Function(p) p.FileEntryId = FileInfo.fe.FileEntryId)
                If (file.FileEntryTypeId = FileType.File AndAlso String.IsNullOrWhiteSpace(FileName)) Then
                    FileName = FileInfo.fv.FileEntryNameWithExtension
                    Dim lastIndex = FileInfo.fv.FileEntryRelativePath.LastIndexOf("\")
                    If (lastIndex > -1) Then
                        FolderName = FileInfo.fv.FileEntryRelativePath.Substring(0, lastIndex)
                    Else
                        FolderName = ""
                    End If
                ElseIf (file.FileEntryTypeId = FileType.Folder) Then
                    FolderName = FileInfo.fv.FileEntryRelativePath
                End If
            End If

        End Using

        StorageConnectionString = Me.GetStorageAccount

    End Sub

    ''' <summary>
    ''' This method will return the containername, blobname, File share Id stored in the database for specific file and also return the cloud storage connection string 
    ''' </summary>
    ''' <param name="FileEntryId">File Entry ID</param>
    ''' <param name="VersionNumber">Version number of the file</param>
    ''' <param name="IsDeleted">true or false if the file was deleted</param>
    ''' <param name="StorageConnectionString">Cloud Stogage connection string</param>
    ''' <param name="ContainerName">Name of the container</param>
    ''' <param name="FileShareId">File Share Id</param>
    ''' <param name="BlobName">Blob Name</param>
    Sub GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId As Guid, VersionNumber As Integer, IsDeleted As Boolean, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer, ByRef BlobName As Guid) Implements ISyncRepository.GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName
        Using service = GetServerEntity()
            Dim query = From fse In service.FileEntries
                        Join s In service.FileShares On fse.FileShareId Equals s.FileShareId
                        Join fsev In service.FileVersions On fsev.FileEntryId Equals fse.FileEntryId
                        Where fse.FileEntryId = FileEntryId AndAlso fse.IsDeleted = IsDeleted AndAlso fsev.VersionNumber = VersionNumber
                        Select s, fsev

            Dim ss = query.FirstOrDefault()

            If ss IsNot Nothing Then
                ContainerName = ss.s.ShareContainerName
                BlobName = ss.fsev.ServerFileName
                FileShareId = ss.s.FileShareId
            End If
        End Using
        StorageConnectionString = Me.GetStorageAccount
    End Sub

End Class
