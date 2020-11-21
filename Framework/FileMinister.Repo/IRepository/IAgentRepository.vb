Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IAgentRepository
    Inherits IRepositoryBase(Of FileAgentInfo, Guid, RSCallerInfo)

    Function ValidateAgentWithMAC(agentId As Guid, secretKey As String, macAddresses As List(Of String)) As ResultInfo(Of WorkSpaceInfo, Status)
    Function GetSharesByAgentId(FileAgentId As Guid) As ResultInfo(Of List(Of ConfigInfo), Status)
    Function AgentMapDetails() As ResultInfo(Of List(Of AgentShareMappingInfo), Status)
    Function GetLocalShares(workSpaceId As Guid) As ResultInfo(Of List(Of LocalShareInfo), Status)
End Interface
