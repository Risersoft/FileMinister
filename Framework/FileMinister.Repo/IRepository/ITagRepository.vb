Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface ITagRepository
    Inherits IRepositoryBase(Of TagInfo, Guid, RSCallerInfo)

    Function GetByFileId(fileId As Guid) As ResultInfo(Of List(Of TagInfo), Status)

    Function ManageTags(data As Tuple(Of Guid, List(Of TagInfo), List(Of TagInfo), List(Of TagInfo))) As ResultInfo(Of Boolean, Status)

End Interface
