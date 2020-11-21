Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface ISyncRepository
    Inherits IRepositoryBase(Of LocalWorkSpaceInfo, Integer, LocalWorkSpaceInfo)

    Function UpdateFileSystemCache(share As LocalConfigInfo, deltaSec As Integer) As ResultInfo(Of Boolean, Status)

    Function ResetSyncTimestamp() As ResultInfo(Of Boolean, Status)

End Interface
