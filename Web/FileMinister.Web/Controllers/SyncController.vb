Imports System.Web.Http
Imports FileMinister.Models.Sync
Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage.File
Imports System.Net
Imports FileMinister.Repo.Util
Imports risersoft.shared.portable.Enums
Imports FileMinister.Models.Enums
Imports risersoft.shared.portable.Model

''' <summary>
''' Sync Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/Sync")>
Public Class SyncController
    Inherits ServerApiController(Of BaseInfo, Integer, ISyncRepository)

    Private Shared ReadOnly SharedAccessTimeForDownloadInMinutes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="SharedAccessTimeForDownloadInMinutes"))
    Private Shared ReadOnly SharedAccessTimeForUploadInMinutes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="SharedAccessTimeForUploadInMinutes"))


    Public Sub New(repository As ISyncRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get Shared Access SignatureUrl
    ''' </summary>
    ''' <param name="FileShareId"></param>
    ''' <param name="BlobName"></param>
    ''' <param name="SharedAccessSignatureType"></param>
    ''' <param name="FileEntryId"></param>
    ''' <param name="FileEntrySize"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    <Route("SasUrl")>
    Public Function GetSharedAccessSignatureUrl(FileShareId As Integer, BlobName As Guid, SharedAccessSignatureType As SharedAccessSignatureType, FileEntryId As Guid, FileEntrySize As Long) As IHttpActionResult
        Dim result As New ResultInfo(Of Uri, Status)()

        Try
            If ((SharedAccessSignatureType = SharedAccessSignatureType.Download AndAlso Util.Helper.HasReadPermission(FileEntryId, Me.repository.User)) OrElse (SharedAccessSignatureType = SharedAccessSignatureType.Upload AndAlso Util.Helper.HasWritePermission(FileEntryId, Me.repository.User))) Then
                Dim SharedAccessSignatureUrl As Uri = Nothing
                Dim CloudBlockBlob As CloudBlockBlob = Util.Helper.GetBlockBlobReference(FileShareId:=FileShareId, BlobName:=BlobName, repository:=repository)

                If Not CloudBlockBlob Is Nothing Then
                    SharedAccessSignatureUrl = Me.GetSharedAccessSignatureUrl(CloudBlockBlob:=CloudBlockBlob, SharedAccessSignatureType:=SharedAccessSignatureType, BlobName:=BlobName, FileShareId:=FileShareId, fileSize:=FileEntrySize)
                    result.Data = SharedAccessSignatureUrl
                Else
                    Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOCKBLOBREFERENCE, arg0:=BlobName.ToString(), arg1:=FileShareId))
                End If
            Else
                result.Status = Status.AccessDenied
                result.Message = ServiceConstants.NOTHAVE_TASKPERMISSIONS
            End If
        Catch ex As Exception
            result.Status = Status.NotFound
            'result.StatusCode = HttpStatusCode.NotFound
            result.Message = ex.Message
        End Try

        Return Ok(result)
    End Function

    <HttpGet>
    <Route("{FileEntryId}/{VersionNumber}/{FileNameForDownloading}/SasUrlBlob")>
    Public Function GetSharedAccessSignatureUrlOfFileEntryForDownload(FileEntryId As Guid, VersionNumber As Integer, FileNameForDownloading As String) As IHttpActionResult
        Dim result As New ResultInfo(Of Uri, Status)()

        Try
            If (Util.Helper.HasReadPermission(FileEntryId, Me.repository.User)) Then
                Dim BlobName As Guid = Nothing
                Dim CloudBlockBlob As CloudBlockBlob = Nothing
                Dim FileShareId As Integer = -1
                Dim SharedAccessSignatureUrl As Uri = Nothing

                Me.GetBlockBlobReferenceAndBlobNameAndShareId(FileEntryId:=FileEntryId, VersionNumber:=VersionNumber, BlobName:=BlobName, CloudBlockBlob:=CloudBlockBlob, FileShareId:=FileShareId)

                If Not CloudBlockBlob Is Nothing Then
                    SharedAccessSignatureUrl = Me.GetSharedAccessSignatureUrl(CloudBlockBlob:=CloudBlockBlob, SharedAccessSignatureType:=SharedAccessSignatureType.Download, BlobName:=BlobName, FileShareId:=FileShareId, FileNameForDownloading:=FileNameForDownloading)
                    result.Data = SharedAccessSignatureUrl
                Else
                    Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOBREFERENCE, arg0:=FileEntryId.ToString(), arg1:=FileShareId.ToString()))
                End If
            Else
                result.Status = Status.AccessDenied
                result.Message = ServiceConstants.NOTHAVE_UPLOAD_FILEPERMISSIONS
            End If
        Catch ex As Exception
            result.Status = Status.NotFound
            'result.StatusCode = HttpStatusCode.NotFound
            result.Message = ex.Message
        End Try

        Return Ok(result)
    End Function

    <HttpGet>
    <Route("{FileEntryId}/{FileEntrySize}/SasUrlBlob")>
    Public Function GetSharedAccessSignatureUrlAndBlobNameOfFileEntryForUpload(FileEntryId As Guid, FileEntrySize As Long) As IHttpActionResult
        Dim result As New ResultInfo(Of Object, Status)()

        Try
            If (Util.Helper.HasWritePermission(FileEntryId, Me.repository.User)) Then
                Dim BlobName As Guid = Guid.NewGuid()
                Dim CloudBlockBlob As CloudBlockBlob = Nothing
                Dim FileShareId As Integer = -1
                Dim SharedAccessSignatureUrl As Uri = Nothing

                Me.GetBlockBlobReferenceAndShareId(FileEntryId:=FileEntryId, BlobName:=BlobName, CloudBlockBlob:=CloudBlockBlob, FileShareId:=FileShareId)

                If Not CloudBlockBlob Is Nothing Then
                    SharedAccessSignatureUrl = Me.GetSharedAccessSignatureUrl(CloudBlockBlob:=CloudBlockBlob, SharedAccessSignatureType:=SharedAccessSignatureType.Upload, BlobName:=BlobName, FileShareId:=FileShareId, fileSize:=FileEntrySize)
                Else
                    Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOBREFERENCE, arg0:=FileEntryId.ToString(), arg1:=FileShareId.ToString()))
                End If

                result.Data = New With {.SharedAccessSignatureUrl = SharedAccessSignatureUrl, .BlobName = BlobName}
                result.Status = Status.Success
            Else
                result.Status = Status.AccessDenied
                result.Message = ServiceConstants.NOTHAVE_UPLOAD_FILEPERMISSIONS
            End If

        Catch ex As Exception
            result.Status = Status.NotFound
            'result.StatusCode = HttpStatusCode.NotFound
            result.Message = ex.Message
        End Try

        Return Ok(result)
    End Function

    <HttpDelete>
    <Route("{FileShareId}")>
    Public Function DeleteBlobs(FileShareId As Integer, <FromBody> BlobNames As List(Of Guid)) As IHttpActionResult
        Dim result = Util.Helper.DeleteBlobs(FileShareId, BlobNames, Me.repository.User)
        Return Ok(result)
    End Function

    Private Function GetSharedAccessSignatureUrl(CloudBlockBlob As CloudBlockBlob, SharedAccessSignatureType As SharedAccessSignatureType, BlobName As Guid, FileShareId As Integer, Optional fileSize As Long = 0, Optional FileNameForDownloading As String = "") As Uri
        Dim SharedAccessSignatureUrl As Uri = Nothing

        Try
            Dim UriBuilder As UriBuilder = New UriBuilder(uri:=CloudBlockBlob.Uri)

            Dim sizeInMB = Math.Round(fileSize / (1024 * 1024), 2)
            sizeInMB = Math.Round(sizeInMB - 2) 'avg time is 5 mins for 2 MB
            If sizeInMB <= 0 Then
                sizeInMB = 0
            End If
            Dim accessExpiryMinutes = SharedAccessTimeForDownloadInMinutes + (SharedAccessTimeForDownloadInMinutes * sizeInMB)
            If accessExpiryMinutes > 60 Then
                accessExpiryMinutes = 60
            End If

            Select Case SharedAccessSignatureType
                Case SharedAccessSignatureType.Download
                    If CloudBlockBlob.Exists() Then
                        If Not String.IsNullOrWhiteSpace(FileNameForDownloading) Then
                            UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=accessExpiryMinutes)}, headers:=New SharedAccessBlobHeaders() With {.ContentDisposition = String.Format(format:="attachment; filename={0}", arg0:=FileNameForDownloading)}).Substring(startIndex:=1)
                        Else
                            UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=accessExpiryMinutes)}).Substring(startIndex:=1)
                        End If

                        SharedAccessSignatureUrl = UriBuilder.Uri
                    Else
                        Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_CREATE_SAS + " as blob does not exists.", arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()))
                    End If
                Case SharedAccessSignatureType.Upload
                    If Util.Helper.CheckQuota(fileSize, Me.repository.User) Then
                        UriBuilder.Query = CloudBlockBlob.GetSharedAccessSignature(policy:=New SharedAccessBlobPolicy() With {.Permissions = SharedAccessBlobPermissions.Read Or SharedAccessBlobPermissions.Write, .SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(minutes:=SharedAccessTimeForUploadInMinutes)}).Substring(startIndex:=1)

                        SharedAccessSignatureUrl = UriBuilder.Uri

                    Else
                        Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_CREATE_SAS + " Quota limit has exceeded", arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()))
                    End If

            End Select
        Catch Exception As Exception
            Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_CREATE_SAS, arg0:=SharedAccessSignatureType.ToString(), arg1:=BlobName.ToString(), arg2:=FileShareId.ToString()), innerException:=Exception)
        End Try

        Return SharedAccessSignatureUrl
    End Function

    Private Sub GetBlockBlobReferenceAndShareId(FileEntryId As Guid, BlobName As Guid, ByRef CloudBlockBlob As CloudBlockBlob, ByRef FileShareId As Integer)
        CloudBlockBlob = Nothing
        Dim StorageConnectionString As String = String.Empty
        Dim ContainerName As String = String.Empty

        repository.GetStorageConnectionStringAndContainerNameAndShareId(FileEntryId:=FileEntryId, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FileShareId:=FileShareId)

        If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(ContainerName)) Then
            CloudBlockBlob = Util.Helper.GetBlockBlobReference(StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, BlobName:=BlobName)
        Else
            Throw New Exception(message:=String.Format(format:=ServiceConstants.NULL_STORAGECONNECTION_OR_CONTAINERNAME, arg0:=FileEntryId.ToString()))
        End If
    End Sub

    Private Sub GetBlockBlobReferenceAndBlobNameAndShareId(FileEntryId As Guid, VersionNumber As Integer, ByRef BlobName As Guid, ByRef CloudBlockBlob As CloudBlockBlob, ByRef FileShareId As Integer)
        CloudBlockBlob = Nothing
        Dim StorageConnectionString As String = String.Empty
        Dim ContainerName As String = String.Empty

        repository.GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId:=FileEntryId, VersionNumber:=VersionNumber, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FileShareId:=FileShareId, BlobName:=BlobName)

        If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(ContainerName)) Then
            CloudBlockBlob = Util.Helper.GetBlockBlobReference(StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, BlobName:=BlobName)
        Else
            Throw New Exception(message:=String.Format(format:=ServiceConstants.NULL_STORAGECONNECTION_OR_CONTAINERNAME, arg0:=FileEntryId.ToString()))
        End If
    End Sub
    Private Sub GetBlockBlobReferenceAndBlobNameAndShareId(FileEntryId As Guid, VersionNumber As Integer, IsDeleted As Boolean, ByRef BlobName As Guid, ByRef CloudBlockBlob As CloudBlockBlob, ByRef FileShareId As Integer)
        CloudBlockBlob = Nothing
        Dim StorageConnectionString As String = String.Empty
        Dim ContainerName As String = String.Empty

        repository.GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId:=FileEntryId, VersionNumber:=VersionNumber, IsDeleted:=IsDeleted, StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, FileShareId:=FileShareId, BlobName:=BlobName)

        If (Not String.IsNullOrWhiteSpace(StorageConnectionString)) AndAlso (Not String.IsNullOrWhiteSpace(ContainerName)) Then
            CloudBlockBlob = Util.Helper.GetBlockBlobReference(StorageConnectionString:=StorageConnectionString, ContainerName:=ContainerName, BlobName:=BlobName)
        Else
            Throw New Exception(message:=String.Format(format:=ServiceConstants.NULL_STORAGECONNECTION_OR_CONTAINERNAME, arg0:=FileEntryId.ToString()))
        End If
    End Sub

    <HttpPost>
    <Route("Server-Data")>
    Public Function SyncServerData() As IHttpActionResult

        ServerDataSyncManager.Instance.SyncAccountData(repository.User)
        Return Ok(True)

    End Function

    <HttpGet>
    <Route("{FileEntryId}/{FileEntrySize}/SasUrlFile")>
    Public Function GetSharedAccessSignatureUrlAndShareNameOfFileEntryForUpload(FileEntryId As Guid, FileEntrySize As Long, BlobName As Guid) As IHttpActionResult
        Dim result As New ResultInfo(Of Object, Status)()
        Dim folderName As String = String.Empty
        Try
            If (Not Util.Helper.HasWritePermission(FileEntryId, Me.repository.User)) Then
                result.Status = Status.AccessDenied
                result.Message = ServiceConstants.NOTHAVE_UPLOAD_FILEPERMISSIONS
                Return Ok(result)
            End If


            Dim cloudFile As CloudFile = Nothing
            Dim fileShareId As Integer = -1
            Dim sharedAccessSignatureUrl As Uri = Nothing

            Me.GetFileReferenceAndShareId(FileEntryId:=FileEntryId, BlobName:=BlobName, FolderName:=folderName, CloudFile:=cloudFile, FileShareId:=fileShareId)

            If Not cloudFile Is Nothing Then
                sharedAccessSignatureUrl = Util.Helper.GetSharedAccessSignatureUrlForFile(CloudFile:=cloudFile, SharedAccessSignatureType:=SharedAccessSignatureType.Upload, FileShareId:=fileShareId, RSCallerInfo:=Me.repository.User, fileSize:=FileEntrySize)
            Else
                Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOBREFERENCE, arg0:=FileEntryId.ToString(), arg1:=fileShareId.ToString()))
            End If

            result.Data = New With {.SharedAccessSignatureUrl = sharedAccessSignatureUrl, .FolderName = folderName}
            result.Status = Status.Success

        Catch ex As Exception
            result.Status = Status.NotFound
            result.Message = ex.Message
        End Try

        Return Ok(result)
    End Function

    Private Sub GetFileReferenceAndShareId(FileEntryId As Guid, BlobName As Guid, ByRef FolderName As String, ByRef CloudFile As CloudFile, ByRef FileShareId As Integer)
        CloudFile = Nothing
        Dim storageConnectionString As String = String.Empty
        Dim shareName As Guid = BlobName
        Dim ContainerName As String = String.Empty
        Dim FileName As String = String.Empty

        repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, StorageConnectionString:=storageConnectionString, ContainerName:=ContainerName, FolderName:=FolderName, FileName:=FileName, FileShareId:=FileShareId)

        If (Not String.IsNullOrWhiteSpace(storageConnectionString)) AndAlso (Not (shareName = Guid.Empty)) Then
            CloudFile = Util.Helper.GetFileReference(StorageConnectionString:=storageConnectionString, ShareName:=ContainerName, FolderName:=FolderName, FileName:=FileName)
        Else
            Throw New Exception(message:=String.Format(format:=ServiceConstants.NULL_STORAGECONNECTION_OR_SHARENAME, arg0:=FileEntryId.ToString()))
        End If
    End Sub
    Private Sub GetFileReferenceAndShareId(FileEntryId As Guid, BlobName As Guid, IsDeleted As Boolean, ByRef FolderName As String, ByRef CloudFile As CloudFile, ByRef FileShareId As Integer)
        CloudFile = Nothing
        Dim storageConnectionString As String = String.Empty
        Dim shareName As Guid = BlobName
        Dim ContainerName As String = String.Empty
        Dim FileName As String = String.Empty

        repository.GetStorageConnectionStringAndFileEntryInfo(FileEntryId:=FileEntryId, IsDeleted:=IsDeleted, StorageConnectionString:=storageConnectionString, ContainerName:=ContainerName, FolderName:=FolderName, FileName:=FileName, FileShareId:=FileShareId)

        If (Not String.IsNullOrWhiteSpace(storageConnectionString)) AndAlso (Not (shareName = Guid.Empty)) Then
            CloudFile = Util.Helper.GetFileReference(StorageConnectionString:=storageConnectionString, ShareName:=ContainerName, FolderName:=FolderName, FileName:=FileName)
        Else
            Throw New Exception(message:=String.Format(format:=ServiceConstants.NULL_STORAGECONNECTION_OR_SHARENAME, arg0:=FileEntryId.ToString()))
        End If
    End Sub

    <HttpGet>
    <Route("{FileEntryId}/{VersionNumber}/{FileNameForDownloading}/{IsDeleted}/SasUrlBlob")>
    Public Function GetSharedAccessSignatureUrlOfFileEntryForDownloadSoftDeletedFile(FileEntryId As Guid, VersionNumber As Integer, FileNameForDownloading As String, IsDeleted As Boolean) As IHttpActionResult
        Dim result As New ResultInfo(Of Uri, Status)()

        If (IsDeleted = False) Then
            Return GetSharedAccessSignatureUrlOfFileEntryForDownload(FileEntryId, VersionNumber, FileNameForDownloading)
        End If

        Try
            If (Util.Helper.HasReadPermission(FileEntryId, Me.repository.User)) Then

                Dim BlobName As Guid = Nothing
                'Dim CloudFile As CloudFile = Nothing
                'Dim FolderName As String = String.Empty
                Dim CloudBlockBlob As CloudBlockBlob = Nothing
                Dim FileShareId As Integer = -1
                Dim SharedAccessSignatureUrl As Uri = Nothing

                Me.GetBlockBlobReferenceAndBlobNameAndShareId(FileEntryId:=FileEntryId, VersionNumber:=VersionNumber, IsDeleted:=IsDeleted, BlobName:=BlobName, CloudBlockBlob:=CloudBlockBlob, FileShareId:=FileShareId)

                If Not CloudBlockBlob Is Nothing Then
                    SharedAccessSignatureUrl = Me.GetSharedAccessSignatureUrl(CloudBlockBlob:=CloudBlockBlob, SharedAccessSignatureType:=SharedAccessSignatureType.Download, BlobName:=BlobName, FileShareId:=FileShareId, FileNameForDownloading:=FileNameForDownloading)
                    result.Data = SharedAccessSignatureUrl
                Else
                    Throw New Exception(message:=String.Format(format:=ServiceConstants.UNABLE_GET_BLOBREFERENCE, arg0:=FileEntryId.ToString(), arg1:=FileShareId.ToString()))
                End If
            Else
                result.Status = Status.AccessDenied
                result.Message = ServiceConstants.NOTHAVE_DOWNLOAD_FILEPERMISSION
            End If
        Catch ex As Exception
            result.Status = Status.NotFound
            result.Message = ex.Message
        End Try

        Return Ok(result)
    End Function



End Class
