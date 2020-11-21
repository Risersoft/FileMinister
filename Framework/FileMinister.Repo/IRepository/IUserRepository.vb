Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IUserRepository
    Inherits IRepositoryBase(Of RSCallerInfo, Integer, RSCallerInfo)

    Function GetAllUsers(searchText As String) As ResultInfo(Of List(Of User), Status)

    Function GetUserById(userId As Guid) As ResultInfo(Of User, Status)
    Function GetUserBySID(sId As String, domainId As Integer) As ResultInfo(Of UserInfo, Status)
    Function GetDomainUsers(domainId As Integer) As ResultInfo(Of List(Of UserInfo), Status)
    Function GetAccountAdmins() As ResultInfo(Of List(Of UserInfo), Status)
End Interface
