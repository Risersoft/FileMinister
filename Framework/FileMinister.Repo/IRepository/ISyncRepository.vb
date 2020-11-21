Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Proxies

Public Interface ISyncRepository
    Inherits IRepositoryBase(Of BaseInfo, Integer, RSCallerInfo)

    Sub GetStorageConnectionStringAndContainerName(FileShareId As Integer, ByRef StorageConnectionString As String, ByRef ContainerName As String)
    Sub GetStorageConnectionStringAndContainerNameAndShareId(FileEntryId As Guid, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer)
    Sub GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId As Guid, VersionNumber As Integer, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer, ByRef BlobName As Guid)
    Function SyncServerData(users As List(Of UserAccountProxy), groups As List(Of GroupDefinitionProxy)) As ResultInfo(Of Boolean, Status)

    Sub GetStorageConnectionStringAndFileEntryInfo(FileEntryId As Guid, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FolderName As String, ByRef FileName As String, Optional ByRef FileShareId As Integer = -1)
    Sub GetStorageConnectionStringAndFileEntryInfo(FileEntryId As Guid, IsDeleted As Boolean, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FolderName As String, ByRef FileName As String, Optional ByRef FileShareId As Integer = -1)

    Sub GetStorageConnectionStringAndContainerNameAndShareIdAndBlobName(FileEntryId As Guid, VersionNumber As Integer, IsDeleted As Boolean, ByRef StorageConnectionString As String, ByRef ContainerName As String, ByRef FileShareId As Integer, ByRef BlobName As Guid)


End Interface
