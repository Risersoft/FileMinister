Imports FileMinister.Client.Common.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IConfigRepository
    Inherits IRepositoryBase(Of ConfigInfo, Integer, LocalWorkSpaceInfo)

    Function GetAccountShares() As ResultInfo(Of List(Of LocalConfigInfo), Status)

    'Function AllSharesMapped(userId As Integer, agentId As Guid) As Boolean

    'Function IsShareMapped(agentId As Guid) As Boolean

    Function ShareMappedSummary() As ResultInfo(Of ShareSummaryInfo, Status)

    Function GetShares() As ResultInfo(Of List(Of ConfigInfo), Status)

    Function AddAll(data As List(Of ConfigInfo)) As ResultInfo(Of Boolean, Status)

    Function AllShareByAccount() As ResultInfo(Of List(Of ConfigInfo), Status)

    Function DeleteShareMapping(shareId As Integer) As ResultInfo(Of Boolean, Status)

    Function DeleteShare(shareId As Integer) As ResultInfo(Of Boolean, Status)

    Function IsAnyPathExists(paths As List(Of String), accountId As Integer) As ResultInfo(Of Boolean, Status)

End Interface
