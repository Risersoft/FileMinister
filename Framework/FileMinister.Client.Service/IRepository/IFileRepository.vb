Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Namespace IRepository
    Public Interface IFileRepository
        Inherits IRepositoryBase(Of FileEntryInfo, Guid, LocalWorkSpaceInfo)

        Function GetByShareId(shareId As Integer) As ResultInfo(Of List(Of FileEntryInfo), Status)
        Function GetByParentId(parentId As Guid) As ResultInfo(Of List(Of FileEntryInfo), Status)
        Function [FileSearch](search As FileSearch) As ResultInfo(Of List(Of FileEntryInfo), Status)
        Function HardDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function SoftDelete(fileSystemEntryId As Guid, isForce As Boolean) As ResultInfo(Of Boolean, Status)
        Function GetMyConflict(fileSystemEntryId As Guid) As ResultInfo(Of FileVersionConflictInfo, Status)
        Function ResolveConflictUsingTheirs(fileSystemEntryId As Guid, shareId As Integer) As ResultInfo(Of Boolean, Status)
        Function ResolveConflictUsingMine(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function CheckOut(fileSystemEntryId As Guid, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
        Function CheckIn(fileSystemEntryVersionId As Guid, versionNumber As Int32) As ResultInfo(Of Boolean, Status)
        Function UndoCheckOut(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function GetFileLinks(fileSystemEntryId As Guid) As ResultInfo(Of List(Of FileEntryLinkInfo), Status)
        Function DeleteFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function DeleteFiles(baseRelativePath As String, shareId As Integer) As ResultInfo(Of Boolean, Status)
        Function DeleteDeltaOperationsForFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function UndoDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Function IsFilePreviouslyLinked(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)

    End Interface
End Namespace