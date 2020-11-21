Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable
Imports risersoft.shared.portable.Enums

Public Interface IFileVersionRepository
    Inherits IRepositoryBase(Of FileVersionInfo, Guid, LocalWorkSpaceInfo)

    Function GetAllFileVersions(fileid As Guid) As ResultInfo(Of List(Of FileVersionInfo), Status)
    Function DeleteFileVersion(Id As Guid) As ResultInfo(Of Boolean, Status)
    Function DeleteFileByFilename(shareId As Integer, filename As String) As ResultInfo(Of Boolean, Status)
    Function UpdateFileVersionStatus(fileSystemEntryVersionId As Guid, fileSystemEntryStatus As Enums.FileEntryStatus) As ResultInfo(Of Boolean, Status)

End Interface
