Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class WorkspaceRepository
    Inherits ServerRepositoryBase(Of RSCallerInfo, Integer)
    Implements IWorkspaceRepository

    Public Function GetAllWorkspaces() As ResultInfo(Of List(Of WorkSpace), Status) Implements IWorkspaceRepository.GetAllWorkspaces
        Try
            Dim workspaceList As List(Of WorkSpace)
            Using service = GetServerEntity()
                workspaceList = service.WorkSpaces.Where(Function(w) Not w.IsDeleted).ToList()
                Return BuildResponse(workspaceList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of WorkSpace))(ex)
        End Try
    End Function

    Public Function GetWorkspacesForShare(shareId As Short) As ResultInfo(Of List(Of WorkSpace), Status) Implements IWorkspaceRepository.GetWorkspacesForShare
        Try
            Dim workspaceList As List(Of WorkSpace)
            Using service = GetServerEntity()
                workspaceList = (From a In service.FileAgentShares
                                 Join f In service.FileAgents
                                     On a.FileAgentId Equals f.FileAgentId
                                 Join w In service.WorkSpaces
                                     On f.FileAgentId Equals w.FileAgentId
                                 Where Not w.IsDeleted AndAlso a.FileShareId = shareId AndAlso Not a.IsDeleted AndAlso Not f.IsDeleted
                                 Select w).ToList()
                Return BuildResponse(workspaceList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of WorkSpace))(ex)
        End Try
    End Function
End Class
