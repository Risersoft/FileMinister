Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IAccountRepository
    Inherits IRepositoryBase(Of BaseInfo, Integer, RSCallerInfo)

    Function GetUsedQuota() As ResultInfo(Of ULong, Status)

    Function GetBlobSize() As ResultInfo(Of ULong, Status)
    Function GetApplicationSettingValue(key As String) As ResultInfo(Of String, Status)
    Function SetApplicationSettingValue(id As Integer, key As String, value As String) As ResultInfo(Of Boolean, Status)
End Interface
