Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IUserGroupRepository
    Inherits IRepositoryBase(Of UserGroupAssignmentsInfo, Guid, RSCallerInfo)

    Function [DeleteByUser](userId As Guid) As ResultInfo(Of Boolean, Status)
    Function [DeleteByGroup](groupId As Guid) As ResultInfo(Of Boolean, Status)
    Function AddAll(data As List(Of UserGroupAssignmentsInfo)) As ResultInfo(Of Boolean, Status)

    Function GetAllGroups(searchText As String) As ResultInfo(Of List(Of Group), Status)

End Interface
