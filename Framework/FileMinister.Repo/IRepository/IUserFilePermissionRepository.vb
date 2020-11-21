Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IUserFilePermissionRepository
    Inherits IRepositoryBase(Of UserFilePermissionInfo, Guid, RSCallerInfo)

    Function [GetAllUsersAndGroups](fileid As Guid) As ResultInfo(Of List(Of UserGroupInfo), Status)
    Function [GetPermissionByFileAndUser](fileid As Guid, userId As Guid) As ResultInfo(Of UserFilePermissionInfo, Status)
    Function [UpdatePermissionByFileAndUser](fileid As Guid, userId As Guid, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status)
    Function [DeletePermissionByFileAndUser](fileid As Guid, userId As Guid) As ResultInfo(Of Boolean, Status)

    Function UpdateFilePermission(data As Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo))) As ResultInfo(Of Boolean, Status)
    Function GetUserFilePermissionsForShare(shareId As Short) As ResultInfo(Of List(Of FilePermissionInfo), Status)
End Interface
