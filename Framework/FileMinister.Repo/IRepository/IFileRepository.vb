Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports Model = FileMinister.Models.Sync

Public Interface IFileRepository
    Inherits IRepositoryBase(Of FileEntryInfo, Guid, RSCallerInfo)

    Function UndoCheckOut(FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
    Function AllFilesForLinking(FileEntryId As Guid, FileShareId As Integer) As ResultInfo(Of List(Of FileEntryLinkInfo), Status)
    Function AddFileLink(FileEntryId As Guid, linkedFileId As Guid) As ResultInfo(Of Boolean, Status)
    Function RemoveFileLink(FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
    Function AddFolder(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
    Function AddFileAndCheckout(FileEntryInfo As FileEntryInfo, FileEntryVersionsInfo As Model.FileVersionInfo, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
    Function AddAndDelete(FileEntryInfo As FileEntryInfo, FileEntryVersionInfo As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
    Function CheckIn(FileEntryVersionsInfo As Model.FileVersionInfo, localFileVersionNumber As Integer) As ResultInfo(Of Integer, Status)
    Function CheckOut(FileEntryId As Guid, localFileVersionNumber As Integer, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
    Function CheckOutWithoutVersion(FileEntryId As Guid, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
    Function AddVersionWithoutUpload(FileEntryInfo As FileEntryInfo, FileEntryVersionInfo As Model.FileVersionInfo, links As List(Of FileEntryLinkInfo)) As ResultInfo(Of Integer, Status)
    Function SoftDelete(FileEntryId As Guid, localFileVersionNumber As Integer) As ResultInfo(Of Boolean, Status)
    Function HardDelete(FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
    Function UndoDelete(FileEntryId As Guid) As ResultInfo(Of Boolean, Status)
    Function GetAllUserShares() As ResultInfo(Of List(Of GetAllSharesForUser_Result), Status)
    Function GetAllChildren(id As Guid, eType As Integer) As ResultInfo(Of List(Of FileEntryInfo), Status)
    Function GetLatestFileEntryVersionByPath(id As Guid, fileName As String) As ResultInfo(Of FileEntryInfo, Status)
    Function AddAndCheckoutWeb(id As Guid, fileName As String, blobName As Guid, fileSize As Long) As ResultInfo(Of FileEntryInfo, Status)
    Function CheckInWeb(FileEntryId As Guid, localFileVersionNumber As Integer, fileSize As Long, fileHash As String, blobName As Guid) As ResultInfo(Of Integer, Status)
    Function AddFolderWeb(parentFolderId As Guid, FolderName As String) As ResultInfo(Of Boolean, Status)
    Function [FileSearch](search As FileSearch) As ResultInfo(Of List(Of FileEntryInfo), Status)

    Function RequestConflictFileUpload(workSpaceId As Guid, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status)
    Function GetConflictFilePendingUpload(workSpaceId As Guid) As ResultInfo(Of List(Of Guid), Status)
    Function GetOtherUsersUnresolvedConflicts(FileEntryId As Guid) As ResultInfo(Of List(Of FileVersionConflictInfo), Status)
    Function UpdateConflictUploadStatus(FileVersionConflictId As Guid, fileSize As Long, fileHash As String) As ResultInfo(Of Boolean, Status)
    Function ResolveOthersConflictUsingOthers(FileShareId As Integer, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status)
    Function ResolveOthersConflictUsingServer(FileShareId As Integer, FileEntryId As Guid, FileVersionConflictId As Guid) As ResultInfo(Of Boolean, Status)
    Function GetEffectivePermission(user As RSCallerInfo, FileEntryId As Guid) As Byte
    Function RenameFile(FileEntryId As Guid, versionNumber As Integer, newName As String) As ResultInfo(Of Boolean, Status)
    Function MoveFile(FileEntryId As Guid, DestinationFileEntryId As Guid, IsReplaceExistingFile As Boolean, NewFileName As String) As ResultInfo(Of Boolean, Status)
    Function GetAllDeletedChildren(id As Guid) As ResultInfo(Of List(Of FileEntryInfo), Status)

End Interface
