Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IUserRepository
    Inherits IRepositoryBase(Of LocalWorkSpaceInfo, Integer, LocalWorkSpaceInfo)

    Function GetByWindowUserSID(windowUserSID As String) As ResultInfo(Of LocalWorkSpaceInfo, Status)
    Function Log(user As LocalWorkSpaceInfo) As ResultInfo(Of LocalWorkSpaceInfo, Status)
    Function GetUserConfigured(userId As Guid, AccountName As String, windowUserName As String) As ResultInfo(Of LocalWorkSpaceInfo, Status)
    Function GetUserAccount(user As LocalWorkSpaceInfo) As ResultInfo(Of LocalWorkSpaceInfo, Status)
    Function GetEffectivePermissionsOnFile(fileId As Guid, includeDeleted As Boolean) As ResultInfo(Of Integer, Status)
    Function GetUserByUserAccountId() As ResultInfo(Of LocalWorkSpaceInfo, Status)
    Function LogOut() As ResultInfo(Of Boolean, Status)
End Interface
