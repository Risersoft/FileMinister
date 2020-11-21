Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IWorkspaceRepository
    Inherits IRepositoryBase(Of RSCallerInfo, Integer, RSCallerInfo)

    Function GetAllWorkspaces() As ResultInfo(Of List(Of WorkSpace), Status)

    Function GetWorkspacesForShare(shareId As Short) As ResultInfo(Of List(Of WorkSpace), Status)
End Interface
