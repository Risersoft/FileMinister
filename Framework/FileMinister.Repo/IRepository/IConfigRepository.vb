Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IConfigRepository
    Inherits IRepositoryBase(Of ConfigInfo, Short, RSCallerInfo)

    Function GetBlobSizeForShare(FileShareId As Short) As ResultInfo(Of Long, Status)

    Function UpdateShareBlobSize(FileShareId As Short) As ResultInfo(Of Boolean, Status)

    Function ShareMapDetails() As ResultInfo(Of List(Of AgentShareMappingInfo), Status)
    Function GetFileSizeForShare(FileShareId As Short) As ResultInfo(Of Long, Status)
End Interface
