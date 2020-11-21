Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IShareRepository
    Inherits IRepositoryBase(Of RSCallerInfo, Integer, RSCallerInfo)

    Function GetAllShares() As ResultInfo(Of List(Of FileShare), Status)
    Function GetShareByName(shareName As String) As ResultInfo(Of ShareFileInfo, Status)

End Interface
