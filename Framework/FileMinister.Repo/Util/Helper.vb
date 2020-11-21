Imports Microsoft.Practices.Unity
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob
Imports System.Net
Imports FileMinister.Models.Sync
Imports FileMinister.Models.Enums
Imports risersoft.shared.portable.Enums
Imports System.Threading.Tasks
Imports Microsoft.WindowsAzure.Storage.File
Imports System.Configuration
Imports Unity

Namespace Util
    Public Class Helper

        Public Shared Property UnityContainer As UnityContainer
        Public Shared ReadOnly SharedAccessTimeForDownloadInMinutes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="SharedAccessTimeForDownloadInMinutes"))
        Public Shared ReadOnly SharedAccessTimeForUploadInMinutes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="SharedAccessTimeForUploadInMinutes"))

        Public Shared Function CreateAzureStorageContainer(StorageConnectionString As String, ContainerName As String) As Boolean
            Dim CloudBlockBlob As CloudBlockBlob = Nothing

            Dim CloudBlobClient As CloudBlobClient = Nothing
            Try
                CloudBlobClient = CloudStorageAccount.Parse(connectionString:=StorageConnectionString).CreateCloudBlobClient()
            Catch Exception As Exception
                Throw New Exception(message:="Unable to initialize CloudBlobClient.", innerException:=Exception)
            End Try

            Dim CloudBlobContainer As CloudBlobContainer = Nothing

            If Not CloudBlobClient Is Nothing Then
                Try
                    Dim CloudBlobServiceProperties As [Shared].Protocol.ServiceProperties = CloudBlobClient.GetServiceProperties()

                    If CloudBlobServiceProperties.Cors.CorsRules.Count = 0 Then
                        CloudBlobServiceProperties.Cors.CorsRules.Add(item:=New [Shared].Protocol.CorsRule())

                        With CloudBlobServiceProperties.Cors.CorsRules(index:=0)
                            .AllowedHeaders.Add(item:="*")
                            .AllowedMethods = [Shared].Protocol.CorsHttpMethods.Get Or [Shared].Protocol.CorsHttpMethods.Head Or [Shared].Protocol.CorsHttpMethods.Post Or [Shared].Protocol.CorsHttpMethods.Put
                            .AllowedOrigins.Add(item:="*")
                            .ExposedHeaders.Add(item:="*")
                            .MaxAgeInSeconds = 1728000
                        End With

                        CloudBlobClient.SetServiceProperties(properties:=CloudBlobServiceProperties)
                    End If
                Catch Exception As Exception
                End Try

                Try
                    CloudBlobContainer = CloudBlobClient.GetContainerReference(containerName:=ContainerName)

                    Return CloudBlobContainer.CreateIfNotExists(accessType:=BlobContainerPublicAccessType.Off)
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to get reference for '{0}' container.", arg0:=ContainerName), innerException:=Exception)
                End Try
            End If

            Return False
        End Function

        Public Shared Function GetBlockBlobReference(FileShareId As Integer, BlobName As Guid, repository As ISyncRepository) As CloudBlockBlob
            Dim CloudBlockBlob As CloudBlockBlob = Nothing
            Dim StorageConnectionString As String = String.Empty
            Dim ContainerName As String = String.Empty

            repository.GetStorageConnectionStringAndContainerName(FileShareId:=FileShareId, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName)

            If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(ContainerName)) Then
                Return GetBlockBlobReference(StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, BlobName:=BlobName)
            Else
                Throw New Exception(message:=String.Format(format:="Fetched StorageConnectionString or ContainerName for FileShare with Id '{0}' is null.", arg0:=FileShareId))
            End If

            Return CloudBlockBlob
        End Function

        Public Shared Function GetBlockBlobReference(StorageConnectionString As String, ContainerName As String, BlobName As Guid) As CloudBlockBlob
            Dim CloudBlockBlob As CloudBlockBlob = Nothing

            Dim CloudBlobClient As CloudBlobClient = Nothing
            Try
                CloudBlobClient = CloudStorageAccount.Parse(connectionString:=StorageConnectionString).CreateCloudBlobClient()
            Catch Exception As Exception
                Throw New Exception(message:="Unable to initialize CloudBlobClient.", innerException:=Exception)
            End Try

            Dim CloudBlobContainer As CloudBlobContainer = Nothing

            If Not CloudBlobClient Is Nothing Then
                Try
                    Dim CloudBlobServiceProperties As [Shared].Protocol.ServiceProperties = CloudBlobClient.GetServiceProperties()

                    If CloudBlobServiceProperties.Cors.CorsRules.Count = 0 Then
                        CloudBlobServiceProperties.Cors.CorsRules.Add(item:=New [Shared].Protocol.CorsRule())

                        With CloudBlobServiceProperties.Cors.CorsRules(index:=0)
                            .AllowedHeaders.Add(item:="*")
                            .AllowedMethods = [Shared].Protocol.CorsHttpMethods.Get Or [Shared].Protocol.CorsHttpMethods.Head Or [Shared].Protocol.CorsHttpMethods.Post Or [Shared].Protocol.CorsHttpMethods.Put
                            .AllowedOrigins.Add(item:="*")
                            .ExposedHeaders.Add(item:="*")
                            .MaxAgeInSeconds = 1728000
                        End With

                        CloudBlobClient.SetServiceProperties(properties:=CloudBlobServiceProperties)
                    End If
                Catch Exception As Exception
                End Try

                Try
                    CloudBlobContainer = CloudBlobClient.GetContainerReference(containerName:=ContainerName)

                    CloudBlobContainer.CreateIfNotExists(accessType:=BlobContainerPublicAccessType.Off)
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to get reference for '{0}' container.", arg0:=ContainerName), innerException:=Exception)
                End Try
            End If

            If Not CloudBlobContainer Is Nothing Then
                Try
                    CloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(blobName:=BlobName.ToString())
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to get block blob reference for '{0}' blob of '{1}' container.", arg0:=BlobName.ToString(), arg1:=ContainerName), innerException:=Exception)
                End Try
            End If

            Return CloudBlockBlob
        End Function

        Public Shared Function DeleteBlobs(FileShareId As Integer, BlobNames As List(Of Guid), user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim result As New ResultInfo(Of Boolean, Status)() With {.Data = True}
            Dim repository = UnityContainer.Resolve(Of ISyncRepository)()
            repository.fncUser = Function() user

            For Each BlobName As Guid In BlobNames
                Try
                    Dim CloudBlockBlob As CloudBlockBlob = Util.Helper.GetBlockBlobReference(FileShareId:=FileShareId, BlobName:=BlobName, repository:=repository)

                    If Not CloudBlockBlob Is Nothing Then
                        Try
                            CloudBlockBlob.Delete()

                        Catch Exception As Exception
                            Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' blob of FileShare with Id '{1}'.", arg0:=BlobName.ToString(), arg1:=FileShareId), innerException:=Exception)
                        End Try
                    Else
                        Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOCKBLOBREFERENCE, arg0:=BlobName.ToString(), arg1:=FileShareId))
                    End If
                Catch Exception As Exception
                    result.Data = False
                    result.Message = Exception.Message
                End Try
            Next

            If Not result.Data Then
                result.Status = Status.NotFound
                'result.StatusCode = HttpStatusCode.NotFound
            End If

            Try
                'Update FileShare blobsize field               
                Dim ConfigRepo = UnityContainer.Resolve(Of IConfigRepository)()
                ConfigRepo.fncUser = Function() user
                result = ConfigRepo.UpdateShareBlobSize(FileShareId)

                If (Not result.Data) Then
                    result.Message = String.Format("Deleted the files from azure container. Failed to update FileShare BlobSize in database with message: {0}", result.Message)
                End If
            Catch ex As Exception
                result.Message = String.Format("Deleted the files from azure container. Failed to update FileShare BlobSize in database with Error: {0}", ex.Message)
            End Try


            Return result
        End Function

        ''' <summary>
        ''' Get Account Quota limit
        ''' </summary>
        ''' <param name="user">User Details</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAccountQuotaLimit(user As RSCallerInfo) As ResultInfo(Of Long, Status)
            Dim accountRepo = UnityContainer.Resolve(Of IAccountRepository)()
            accountRepo.fncUser = Function() user
            Dim appresult = accountRepo.GetApplicationSettingValue(Constants.APPLICATION_SETTING_ACCOUNT_QUOTA_LIMIT)

            Dim result = New ResultInfo(Of Long, Status)

            If String.IsNullOrEmpty(appresult.Data) OrElse Not Long.TryParse(appresult.Data, result.Data) Then
                result.Data = 0
                result.Status = Status.NotFound
                result.Message = "Correct value not set"
            Else
                result.Status = appresult.Status
                result.Message = appresult.Message
            End If

            Return result
        End Function

        ''' <summary>
        ''' Set Account Quota Limit
        ''' </summary>
        ''' <param name="quotaLimit">Account Quota Size</param>
        ''' <param name="user">User Details</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SetAccountQuotaLimit(quotaLimit As Long, user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim accountRepo = UnityContainer.Resolve(Of IAccountRepository)()
            accountRepo.fncUser = Function() user
            Return accountRepo.SetApplicationSettingValue(Constants.APPLICATION_SETTING_ID_ACCOUNT_QUOTA_LIMIT, Constants.APPLICATION_SETTING_ACCOUNT_QUOTA_LIMIT, quotaLimit.ToString())
        End Function

        Public Shared Function GetWebWorkSpaceId(user As RSCallerInfo) As ResultInfo(Of Guid, Status)
            Dim accountRepo = UnityContainer.Resolve(Of IAccountRepository)()
            accountRepo.fncUser = Function() user
            Dim result = accountRepo.GetApplicationSettingValue(Constants.APPLICATION_SETTING_WEB_WORK_SPACEID)

            Dim res As New ResultInfo(Of Guid, Status)
            Dim workSpaceId As Guid = Guid.Empty
            If Guid.TryParse(result.Data, workSpaceId) Then
                res.Data = workSpaceId
            Else
                workSpaceId = Guid.NewGuid()
                Dim setresult = accountRepo.SetApplicationSettingValue(Constants.APPLICATION_SETTING_ID_WEB_WORK_SPACEID, Constants.APPLICATION_SETTING_WEB_WORK_SPACEID, workSpaceId.ToString())

                If setresult.Data Then
                    res.Data = workSpaceId
                End If
            End If
            Return res
        End Function

        Public Shared Function GetEffectivePermission(fileId As Guid, user As RSCallerInfo) As Byte
            Dim fileRepo = UnityContainer.Resolve(Of IFileRepository)()
            fileRepo.fncUser = Function() user
            Return fileRepo.GetEffectivePermission(user, fileId)
        End Function

        Public Shared Function HasWritePermission(fileId As Guid, user As RSCallerInfo) As Boolean
            Return (user.UserAccount.UserTypeId = Role.AccountAdmin OrElse ((GetEffectivePermission(fileId, user) And PermissionType.Write) = PermissionType.Write))
        End Function

        Public Shared Function HasReadPermission(fileId As Guid, user As RSCallerInfo) As Boolean
            Return (user.UserAccount.UserTypeId = Role.AccountAdmin OrElse ((GetEffectivePermission(fileId, user) And PermissionType.Read) = PermissionType.Read))
        End Function

        Public Shared Function CheckQuota(filesize As Long, user As RSCallerInfo) As Boolean
            Dim accountRepo = UnityContainer.Resolve(Of IAccountRepository)()
            accountRepo.fncUser = Function() user

            Dim usedresult = accountRepo.GetUsedQuota()
            Dim usedQuota As Long = 0
            'Dim quotaLimit As Long = GetAccountQuotaLimit(user).Data

            'Convert GB into bytes
            Dim quotaLimit As Long = (Convert.ToInt64(user.Account.PurchasedQuota) + Convert.ToInt64(user.Account.AllocatedQuota)) * 1024 * 1024 * 1024

            If (usedresult IsNot Nothing) Then
                'usedQuota = usedresult.Data
                'Convert MB into bytes
                usedQuota = usedresult.Data * 1024 * 1024
            End If


            'If (filesize + usedQuota) > quotaLimit Then
            '    'check whether additional quota is purchased in Bytes
            '    Dim accountQuota As Integer = (user.Account.PurchasedQuota + user.Account.AllocatedQuota) * 1024 * 1024 * 1024

            '    If (accountQuota > 0) AndAlso accountQuota <> quotaLimit Then
            '        SetAccountQuotaLimit(accountQuota, user)

            '        If (filesize + usedQuota) > accountQuota Then
            '            Return False
            '        Else
            '            Return True
            '        End If
            '    Else
            '        Return False
            '    End If
            'Else
            '    Return True
            'End If
            Return Not ((filesize + usedQuota) > quotaLimit)
        End Function

        Public Shared Function GetAllUsers(searchText As String, user As RSCallerInfo) As ResultInfo(Of List(Of User), Status)
            Dim result = New ResultInfo(Of List(Of User), Status)()

            Dim userRepo = UnityContainer.Resolve(Of IUserRepository)()
            userRepo.fncUser = Function() user
            result = userRepo.GetAllUsers(searchText)

            Return result

        End Function

        Public Shared Function GetAllGroup(searchText As String, user As RSCallerInfo) As ResultInfo(Of List(Of Group), Status)
            Dim result = New ResultInfo(Of List(Of Group), Status)()

            Dim grpRepo = UnityContainer.Resolve(Of IUserGroupRepository)()
            grpRepo.fncUser = Function() user
            result = grpRepo.GetAllGroups(searchText)

            Return result

        End Function

        Friend Shared Function GetFileStorageShare(StorageConnectionString As String, ShareName As String) As File.CloudFileShare
            Dim cloudFile As File.CloudFile = Nothing

            Dim cloudFileClient As File.CloudFileClient = Nothing

            Try
                cloudFileClient = CloudStorageAccount.Parse(connectionString:=StorageConnectionString).CreateCloudFileClient()
            Catch Exception As Exception
                Throw New Exception(message:="Unable to initialize CloudFileClient.", innerException:=Exception)
            End Try

            Dim cloudFileShare As File.CloudFileShare = Nothing

            If Not cloudFileClient Is Nothing Then
                Try
                    Dim cloudFileServiceProperties As File.Protocol.FileServiceProperties = cloudFileClient.GetServiceProperties()

                    If cloudFileServiceProperties.Cors.CorsRules.Count = 0 Then
                        cloudFileServiceProperties.Cors.CorsRules.Add(item:=New [Shared].Protocol.CorsRule())

                        With cloudFileServiceProperties.Cors.CorsRules(index:=0)
                            .AllowedHeaders.Add(item:="*")
                            .AllowedMethods = [Shared].Protocol.CorsHttpMethods.Get Or [Shared].Protocol.CorsHttpMethods.Head Or [Shared].Protocol.CorsHttpMethods.Post Or [Shared].Protocol.CorsHttpMethods.Put
                            .AllowedOrigins.Add(item:="*")
                            .ExposedHeaders.Add(item:="*")
                            .MaxAgeInSeconds = 1728000
                        End With

                        cloudFileClient.SetServiceProperties(properties:=cloudFileServiceProperties)
                    End If
                Catch Exception As Exception
                End Try

                Try
                    cloudFileShare = cloudFileClient.GetShareReference(shareName:=ShareName)
                    cloudFileShare.CreateIfNotExists()
                Catch Exception As Exception
                    'Please make sure your storage account has storage file endpoint enabled and specified correctly in the app.config
                    Throw New Exception(message:=String.Format(format:="Unable to get reference for '{0}' share.", arg0:=ShareName), innerException:=Exception)
                End Try
            End If

            Return cloudFileShare
        End Function

        Friend Shared Function GetFileReference(FileEntryId As Guid, BlobName As Guid, FileShareId As Integer, repository As ISyncRepository) As File.CloudFile
            Dim CloudFile As File.CloudFile = Nothing
            Dim StorageConnectionString As String = String.Empty
            Dim FolderName As String = String.Empty
            Dim FileName As String = String.Empty
            Dim ContainerName As String = String.Empty

            repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FolderName:=FolderName, FileName:=FileName, FileShareId:=FileShareId)

            If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(FileName)) Then
                Return GetFileReference(StorageConnectionString:=StorageConnectionString, ShareName:=ContainerName, FolderName:=FolderName, FileName:=FileName)
            Else
                Throw New Exception(message:=String.Format(format:="Fetched StorageConnectionString or ContainerName for FileShare with Id '{0}' is null.", arg0:=FileEntryId))
            End If

            Return CloudFile
        End Function

        Friend Shared Function GetFileReference(FileEntryId As Guid, BlobName As Guid, repository As ISyncRepository) As File.CloudFile
            Dim CloudFile As File.CloudFile = Nothing
            Dim StorageConnectionString As String = String.Empty
            Dim FolderName As String = String.Empty
            Dim FileName As String = String.Empty
            Dim ContainerName As String = String.Empty

            repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FolderName:=FolderName, FileName:=FileName)

            If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(FolderName)) Then
                Return GetFileReference(StorageConnectionString:=StorageConnectionString, ShareName:=ContainerName, FolderName:=FolderName, FileName:=FileName)
            Else
                Throw New Exception(message:=String.Format(format:="Fetched StorageConnectionString or ContainerName for FileShare with Id '{0}' is null.", arg0:=FileEntryId))
            End If

            Return CloudFile
        End Function

        Friend Shared Function GetFileReference(FileEntryVerionInfo As FileVersionInfo, repository As ISyncRepository) As File.CloudFile
            Dim CloudFile As File.CloudFile = Nothing
            Dim StorageConnectionString As String = String.Empty
            Dim FolderName As String = String.Empty
            Dim ContainerName As String = String.Empty

            repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryVerionInfo.FileEntryId, IsDeleted:=FileEntryVerionInfo.FileEntry.IsDeleted, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FolderName:=FolderName, FileName:=FileEntryVerionInfo.FileEntryNameWithExtension, FileShareId:=FileEntryVerionInfo.FileEntry.FileShareId)
            If (String.IsNullOrWhiteSpace(FolderName)) Then FolderName = FileEntryVerionInfo.FileEntryRelativePath

            If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(FolderName)) Then
                Return GetFileReference(StorageConnectionString:=StorageConnectionString, ShareName:=ContainerName, FolderName:=FolderName, FileName:=FileEntryVerionInfo.FileEntryNameWithExtension)
            Else
                Throw New Exception(message:=String.Format(format:="Fetched StorageConnectionString or ContainerName for FileShare with Id '{0}' is null.", arg0:=FileEntryVerionInfo.FileEntryId))
            End If

            Return CloudFile
        End Function

        Public Shared Function GetFileReference(StorageConnectionString As String, ShareName As String, FolderName As String, FileName As String) As File.CloudFile
            Dim cloudFileShare As File.CloudFileShare = Nothing
            Dim cloudFile As File.CloudFile = Nothing
            Dim fileDirectory As File.CloudFileDirectory = Nothing

            fileDirectory = CreateAzureFileDirectory(StorageConnectionString:=StorageConnectionString, ShareName:=ShareName, FolderName:=FolderName)
            'cloudFileShare = GetFileStorageShare(StorageConnectionString, ShareName)

            If Not fileDirectory Is Nothing Then
                Try
                    'Dim rootDirectory As File.CloudFileDirectory = cloudFileShare.GetRootDirectoryReference()
                    'If (String.IsNullOrWhiteSpace(FolderName)) Then
                    '    'if there is no folder specified, so return a reference to the root directory.
                    '    fileDirectory = rootDirectory
                    'Else
                    '    'get the reference of the specified folder.
                    '    fileDirectory = rootDirectory.GetDirectoryReference(GetFolderFullName(FolderName))
                    '    fileDirectory.CreateIfNotExists()
                    'End If

                    cloudFile = fileDirectory.GetFileReference(fileName:=FileName)
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to get reference for '{0}' file of '{1}' folder of '{2}' share container.", arg0:=FileName.ToString(), arg1:=FolderName, arg2:=ShareName), innerException:=Exception)
                End Try
            End If

            Return cloudFile
        End Function

        Friend Shared Function SetFileMetaData(StorageConnectionString As String, ShareName As String, FolderName As String, FileEntryId As Guid, FileName As String, respository As ITagRepository) As ResultInfo(Of Boolean, Status)
            Dim cloudFile As File.CloudFile = Nothing
            Dim res As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status)
            Dim result = respository.GetByFileId(FileEntryId)
            If (result.Data IsNot Nothing) Then
                Dim tags As List(Of TagInfo) = result.Data
                cloudFile = GetFileReference(StorageConnectionString:=StorageConnectionString, ShareName:=ShareName, FolderName:=FolderName, FileName:=FileName)
                If (cloudFile IsNot Nothing) Then
                    For Each tag In tags
                        With cloudFile
                            .Metadata.Add(key:=tag.TagName, value:=tag.TagValue)
                            .SetMetadata()
                        End With
                    Next
                End If
            End If

            Return res

        End Function

        Friend Shared Function DeleteFolder(FileEntryId As Guid, ShareName As String, FileShareId As Integer, IsDeleted As Boolean, user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim result As New ResultInfo(Of Boolean, Status)() With {.Data = False}
            Dim repository = UnityContainer.Resolve(Of ISyncRepository)()
            repository.fncUser = Function() user

            Dim folderName As String = String.Empty
            Dim storageConnectionString As String = String.Empty
            Dim cloudFileShare As File.CloudFileShare
            Dim rootDirectory As File.CloudFileDirectory
            Dim fileDirectory As File.CloudFileDirectory
            Dim containerName As String = ShareName
            Dim fileName As String = String.Empty

            Try
                repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, IsDeleted:=IsDeleted, StorageConnectionString:=storageConnectionString, ContainerName:=containerName, FolderName:=folderName, FileName:=fileName, FileShareId:=FileShareId)

                cloudFileShare = Util.Helper.GetFileStorageShare(StorageConnectionString:=storageConnectionString, ShareName:=containerName)
                rootDirectory = cloudFileShare.GetRootDirectoryReference()

                folderName = GetFolderFullName(folderName)

                fileDirectory = rootDirectory.GetDirectoryReference(folderName)

                If Not fileDirectory Is Nothing Then

                    Try
                        If (fileDirectory.Exists()) Then

                            'DeleteAllFilesFromFolder(fileDirectory)

                            fileDirectory.Delete()
                            result.Data = True
                            result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                            result.Status = Status.Success
                            'Else
                            '    result.Data = False
                            '    result.Message = String.Format(format:="Unable to delete directory '{0}' of FileShare '{1}' as it does not exists.", arg0:=folderName.ToString(), arg1:=containerName)
                            '    result.Status = Status.NotFound
                        End If

                    Catch Exception As Exception
                        Dim response = DirectCast(Exception.InnerException, System.Net.WebException).Response
                        If (response IsNot Nothing) Then
                            Dim webResponse = DirectCast(response, System.Net.HttpWebResponse)
                            If (webResponse IsNot Nothing) Then
                                Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' directory of FileShare '{1}' as " + webResponse.StatusDescription, arg0:=folderName, arg1:=containerName), innerException:=Exception)
                            Else
                                Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' directory of FileShare '{1}'.", arg0:=folderName, arg1:=containerName), innerException:=Exception)
                            End If
                        Else
                            Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' directory of FileShare '{1}'.", arg0:=folderName, arg1:=containerName), innerException:=Exception)
                        End If

                    End Try
                Else
                    Throw New Exception(message:=String.Format(format:="Unable to get directory reference for '{0}' of FileShare '{1}'.", arg0:=folderName.ToString(), arg1:=containerName))
                End If

            Catch ex As Exception
                result.Data = False
                result.Message = ex.Message
                result.Status = Status.Error
            End Try

            Return result

        End Function

        Friend Shared Function DeleteFile(FileEntryId As Guid, ShareName As String, FileShareId As Integer, user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim result As New ResultInfo(Of Boolean, Status)() With {.Data = True}
            Dim repository = UnityContainer.Resolve(Of ISyncRepository)()
            repository.fncUser = Function() user

            Dim folderName As String = String.Empty
            Dim storageConnectionString As String = String.Empty
            Dim cloudFileShare As File.CloudFileShare
            Dim rootDirectory As File.CloudFileDirectory
            Dim fileDirectory As File.CloudFileDirectory
            Dim containerName As String = ShareName
            Dim fileName As String = String.Empty

            Try
                repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, StorageConnectionString:=storageConnectionString, ContainerName:=containerName, FolderName:=folderName, FileName:=fileName, FileShareId:=FileShareId)

                cloudFileShare = Util.Helper.GetFileStorageShare(StorageConnectionString:=storageConnectionString, ShareName:=containerName)
                rootDirectory = cloudFileShare.GetRootDirectoryReference()

                folderName = GetFolderFullName(folderName)
                If (String.IsNullOrEmpty(folderName)) Then
                    fileDirectory = cloudFileShare.GetRootDirectoryReference()
                Else
                    fileDirectory = rootDirectory.GetDirectoryReference(folderName)
                End If

                Dim cloudFile As File.CloudFile = fileDirectory.GetFileReference(System.IO.Path.GetFileNameWithoutExtension(fileName))

                If Not cloudFile Is Nothing Then
                    Try
                        If (cloudFile.Exists()) Then cloudFile.Delete()
                        result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        result.Status = Status.Success

                    Catch Exception As Exception
                        Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' file of FileShare '{1}'.", arg0:=fileName.ToString(), arg1:=containerName), innerException:=Exception)
                    End Try
                Else
                    Throw New Exception(message:=String.Format(format:="Unable to get file reference for '{0}' of FileShare '{1}'.", arg0:=fileName.ToString(), arg1:=containerName))
                End If

            Catch ex As Exception
                result.Data = False
                result.Message = ex.Message
                result.Status = Status.Error
            End Try

            Return result

        End Function

        Friend Shared Function DeleteFile(FileEntryId As Guid, ShareName As String, FileShareId As Integer, IsDeleted As Boolean, user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim result As New ResultInfo(Of Boolean, Status)() With {.Data = True}
            Dim repository = UnityContainer.Resolve(Of ISyncRepository)()
            repository.fncUser = Function() user

            Dim folderName As String = String.Empty
            Dim storageConnectionString As String = String.Empty
            Dim cloudFileShare As File.CloudFileShare
            Dim rootDirectory As File.CloudFileDirectory
            Dim fileDirectory As File.CloudFileDirectory
            Dim containerName As String = ShareName
            Dim fileName As String = String.Empty

            Try
                repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, IsDeleted:=IsDeleted, StorageConnectionString:=storageConnectionString, ContainerName:=containerName, FolderName:=folderName, FileName:=fileName, FileShareId:=FileShareId)

                cloudFileShare = Util.Helper.GetFileStorageShare(StorageConnectionString:=storageConnectionString, ShareName:=containerName)
                rootDirectory = cloudFileShare.GetRootDirectoryReference()

                folderName = GetFolderFullName(folderName)
                If (String.IsNullOrEmpty(folderName)) Then
                    fileDirectory = cloudFileShare.GetRootDirectoryReference()
                Else
                    fileDirectory = rootDirectory.GetDirectoryReference(folderName)
                End If
                Dim cloudFile As File.CloudFile = fileDirectory.GetFileReference(System.IO.Path.GetFileNameWithoutExtension(fileName))

                If Not cloudFile Is Nothing Then
                    Try
                        If (cloudFile.Exists()) Then
                            cloudFile.Delete()
                        End If
                        result.Message = ServiceConstants.OPERATION_SUCCESSFUL
                        result.Status = Status.Success

                    Catch Exception As Exception
                        Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' file of FileShare '{1}'.", arg0:=fileName.ToString(), arg1:=containerName), innerException:=Exception)
                    End Try
                Else
                    Throw New Exception(message:=String.Format(format:="Unable to get file reference for '{0}' of FileShare '{1}'.", arg0:=fileName.ToString(), arg1:=containerName))
                End If

            Catch ex As Exception
                result.Data = False
                result.Message = ex.Message
                result.Status = Status.Error
            End Try

            Return result

        End Function

        Public Shared Function GetFolderFullName(folderName As String) As String
            If (String.IsNullOrWhiteSpace(folderName)) Then Return folderName

            Dim name = System.IO.Path.GetDirectoryName(folderName)

            If (String.IsNullOrWhiteSpace(name)) Then Return folderName

            Dim dir = name.Split(System.IO.Path.DirectorySeparatorChar)
            name = String.Join("\", dir)
            Return name

        End Function

        Public Shared Function DeleteFiles(FileShareId As Integer, ShareNames As List(Of Guid), user As RSCallerInfo) As ResultInfo(Of Boolean, Status)
            Dim result As New ResultInfo(Of Boolean, Status)() With {.Data = True}
            Dim repository = UnityContainer.Resolve(Of ISyncRepository)()
            repository.fncUser = Function() user

            Dim fileRepository = UnityContainer.Resolve(Of IFileRepository)()
            fileRepository.fncUser = Function() user

            For Each ShareName As Guid In ShareNames
                Dim folders As ResultInfo(Of List(Of FileEntryInfo), Status) = fileRepository.GetAllChildren(ShareName, 2)
                For Each folder In folders.Data
                    If (folder.FileShareId = FileShareId) Then
                        Try
                            Dim CloudFile As File.CloudFile = Util.Helper.GetFileReference(FileEntryId:=folder.FileEntryId, BlobName:=ShareName, repository:=repository)
                            If Not CloudFile Is Nothing Then
                                Try
                                    CloudFile.Delete()
                                Catch Exception As Exception
                                    Throw New Exception(message:=String.Format(format:="Unable to delete '{0}' blob of FileShare with Id '{1}'.", arg0:=ShareName.ToString(), arg1:=FileShareId), innerException:=Exception)
                                End Try
                            Else
                                Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOCKBLOBREFERENCE, arg0:=ShareName.ToString(), arg1:=FileShareId))
                            End If
                        Catch Exception As Exception
                            result.Data = False
                            result.Message = Exception.Message
                        End Try
                    End If
                Next
            Next

            If Not result.Data Then
                result.Status = Status.NotFound
            End If

            Return result
        End Function

        Friend Shared Function GetSharedAccessSignatureUrl(CloudBlockBlob As CloudBlockBlob, SharedAccessSignatureType As SharedAccessSignatureType, BlobName As Guid, FileShareId As Integer, RSCallerInfo As RSCallerInfo, Optional fileSize As Long = 0, Optional FileNameForDownloading As String = "") As Uri
            Dim SharedAccessSignatureUrl As Uri = Nothing

            Try
                Dim UriBuilder As UriBuilder = New UriBuilder(uri:=CloudBlockBlob.Uri)

                Select Case SharedAccessSignatureType
                    Case SharedAccessSignatureType.Download
                        If CloudBlockBlob.Exists() Then
                            If Not String.IsNullOrWhiteSpace(FileNameForDownloading) Then
                                UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForDownloadInMinutes)}, headers:=New SharedAccessBlobHeaders() With {.ContentDisposition = String.Format(format:="attachment; filename={0}", arg0:=FileNameForDownloading)}).Substring(startIndex:=1)
                            Else
                                UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForDownloadInMinutes)}).Substring(startIndex:=1)
                            End If

                            SharedAccessSignatureUrl = UriBuilder.Uri
                        Else
                            Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' blob of FileShare with Id '{2}' as blob does not exists.", arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()))
                        End If
                    Case SharedAccessSignatureType.Upload
                        If Util.Helper.CheckQuota(fileSize, RSCallerInfo) Then
                            UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read Or SharedAccessBlobPermissions.Write, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForUploadInMinutes)}).Substring(startIndex:=1)

                            SharedAccessSignatureUrl = UriBuilder.Uri

                        Else
                            Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' blob of FileShare with Id '{2}'. Quota limit has exceeded", arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()))
                        End If

                End Select
            Catch Exception As Exception
                Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' blob of FileShare with Id '{2}'.", arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()), innerException:=Exception)
            End Try

            Return SharedAccessSignatureUrl
        End Function

        Public Shared Function GetSharedAccessSignatureUrlForFile(CloudFile As CloudFile, SharedAccessSignatureType As SharedAccessSignatureType, FileShareId As Integer, RSCallerInfo As RSCallerInfo, Optional fileSize As Long = 0, Optional FileNameForDownloading As String = "") As Uri
            Dim sharedAccessSignatureUrl As Uri = Nothing

            Try
                Dim UriBuilder As UriBuilder = New UriBuilder(uri:=CloudFile.Uri)

                Select Case SharedAccessSignatureType
                    Case SharedAccessSignatureType.Download
                        If CloudFile.Exists() Then
                            If Not String.IsNullOrWhiteSpace(FileNameForDownloading) Then
                                UriBuilder.Query = CloudFile.GetSharedAccessSignature(policy:=New SharedAccessFilePolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForDownloadInMinutes)}, headers:=New SharedAccessFileHeaders() With {.ContentDisposition = String.Format(format:="attachment; filename={0}", arg0:=FileNameForDownloading)}).Substring(startIndex:=1)
                            Else
                                UriBuilder.Query = CloudFile.GetSharedAccessSignature(policy:=New SharedAccessFilePolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForDownloadInMinutes)}).Substring(startIndex:=1)
                            End If

                            sharedAccessSignatureUrl = UriBuilder.Uri
                        Else
                            Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' file of FileShare with Id '{2}' as blob does not exists.", arg0:=SharedAccessSignatureType.ToString(), arg1:=CloudFile.Name, arg2:=FileShareId.ToString()))
                        End If
                    Case SharedAccessSignatureType.Upload
                        If Util.Helper.CheckQuota(fileSize, RSCallerInfo) Then
                            UriBuilder.Query = CloudFile.GetSharedAccessSignature(policy:=New SharedAccessFilePolicy() With {.Permissions = SharedAccessBlobPermissions.Read Or SharedAccessBlobPermissions.Write, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForUploadInMinutes)}).Substring(startIndex:=1)

                            sharedAccessSignatureUrl = UriBuilder.Uri

                        Else
                            Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' file of FileShare with Id '{2}'. Quota limit has exceeded", arg0:=SharedAccessSignatureType.ToString(), arg1:=CloudFile.Name, arg2:=FileShareId.ToString()))
                        End If

                End Select
            Catch Exception As Exception
                Throw New Exception(message:=String.Format(format:="Unable to create '{0}' type shared access signature for '{1}' file of FileShare with Id '{2}'.", arg0:=SharedAccessSignatureType.ToString(), arg1:=CloudFile.Name, arg2:=FileShareId.ToString()), innerException:=Exception)
            End Try

            Return sharedAccessSignatureUrl
        End Function

        Friend Shared Function ConvertBytesToMb(bytes As Long) As Long
            Return ((bytes / 1024)) / 1024
        End Function

        Public Shared Function CreateAzureFileDirectory(StorageConnectionString As String, ShareName As String, FolderName As String) As File.CloudFileDirectory
            Dim cloudFileShare As File.CloudFileShare = Nothing
            Dim cloudFile As File.CloudFile = Nothing
            Dim fileDirectory As File.CloudFileDirectory = Nothing
            cloudFileShare = GetFileStorageShare(StorageConnectionString, ShareName)

            Dim rootDirectory As File.CloudFileDirectory = Nothing

            If Not cloudFileShare Is Nothing Then
                Try
                    'Dim fName As String = GetFolderFullName(FolderName)
                    rootDirectory = cloudFileShare.GetRootDirectoryReference()
                    If (String.IsNullOrWhiteSpace(FolderName)) Then
                        'if there is no folder specified, so return a reference to the root directory.
                        fileDirectory = rootDirectory
                    Else
                        'get the reference of the specified folder.
                        Dim delimiter = New Char() {"\"}
                        Dim nestedFolderArray = FolderName.Split(delimiter)
                        For i As Integer = 0 To nestedFolderArray.Length - 1
                            If (Not String.IsNullOrWhiteSpace(nestedFolderArray(i))) Then
                                rootDirectory = rootDirectory.GetDirectoryReference(nestedFolderArray(i))
                                rootDirectory.CreateIfNotExists()
                            End If
                        Next

                        fileDirectory = rootDirectory

                        'fileDirectory = rootDirectory.GetDirectoryReference(fName)
                        'fileDirectory.CreateIfNotExists()
                    End If

                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to get reference for '{0}' folder of '{1}' share container.", arg0:=FolderName.ToString(), arg1:=ShareName), innerException:=Exception)
                End Try
            End If

            Return fileDirectory

        End Function

        'Private Shared Sub DeleteAllFilesFromFolder(ByVal dir As CloudFileDirectory)
        '    Dim continuationToken As FileContinuationToken = Nothing
        '    Dim resultSegment As FileResultSegment = Nothing
        '    Do
        '        resultSegment = dir.ListFilesAndDirectoriesSegmented(100, continuationToken, Nothing, Nothing)
        '        If resultSegment.Results.Count() > 0 Then
        '            For Each item In resultSegment.Results
        '                If item.[GetType]() = GetType(CloudFileDirectory) Then
        '                    Dim cloudFileDirectory = TryCast(item, CloudFileDirectory)
        '                    DeleteAllFilesFromFolder(cloudFileDirectory)
        '                ElseIf item.[GetType]() = GetType(CloudFile) Then
        '                    Dim cloudFile = TryCast(item, CloudFile)
        '                    cloudFile.DeleteIfExists()
        '                End If
        '            Next
        '        End If
        '    Loop While continuationToken IsNot Nothing
        'End Sub

        Public Shared Function GetParentRelativePath(fileSystemEntryOldRelativePath As String) As String
            Dim relativePath = fileSystemEntryOldRelativePath
            If relativePath.Length > 0 Then
                Dim lastIndex = fileSystemEntryOldRelativePath.LastIndexOf("\")
                If lastIndex >= 0 Then
                    relativePath = fileSystemEntryOldRelativePath.Substring(0, lastIndex)
                Else
                    relativePath = ""
                End If
            End If
            Return relativePath
        End Function
    End Class
End Namespace
