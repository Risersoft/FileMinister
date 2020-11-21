Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface ITagRepository
    Inherits IRepositoryBase(Of TagInfo, Guid, LocalWorkSpaceInfo)

    Function [GetFileTags](fileid As Guid) As ResultInfo(Of List(Of TagInfo), Status)
End Interface
