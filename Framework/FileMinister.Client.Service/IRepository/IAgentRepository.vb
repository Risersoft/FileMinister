Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model


Public Interface IAgentRepository
    Inherits IRepositoryBase(Of FileAgentInfo, Guid, LocalWorkSpaceInfo)

    Function SyncShares(agentId As Guid) As ResultInfo(Of Boolean, Status)
End Interface
