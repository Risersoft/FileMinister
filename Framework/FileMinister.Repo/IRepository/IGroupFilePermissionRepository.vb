Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IGroupFilePermissionRepository
    Inherits IRepositoryBase(Of UserFilePermissionInfo, Guid, RSCallerInfo)

    Function [GetPermissionByFileAndGroup](fileid As Guid, groupId As Guid) As ResultInfo(Of UserFilePermissionInfo, Status)
    Function [UpdatePermissionByFileAndGroup](fileid As Guid, groupId As Guid, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status)
    Function [DeletePermissionByFileAndGroup](fileid As Guid, groupId As Guid) As ResultInfo(Of Boolean, Status)
    Function GetGroupFilePermissionsForShare(shareId As Short) As ResultInfo(Of List(Of FilePermissionInfo), Status)

End Interface
