Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Interface IFileHandleRepository
    Inherits IRepositoryBase(Of RSCallerInfo, Integer, RSCallerInfo)

    Function GetOpenFileHandles(FileEntryId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status)
    Function GetOpenFileHandlesForWorkspace(WorkspaceId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status)
    Function OpenFileHandle(FileEntryId As Guid, RelativePath As String, WorkspaceId As Guid, ServerFileSize As Int32, ServerFileTime As DateTime) As ResultInfo(Of Boolean, Status)
    Function OpenFileHandle(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Int32, ServerFileTime As DateTime) As ResultInfo(Of Boolean, Status)
    Function CloseFile(FileEntryId As Guid, WorkspaceId As Guid, ServerFileSize As Int32, ServerFileTime As DateTime) As ResultInfo(Of Boolean, Status)
    Function CloseFile(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Int32, ServerFileTime As DateTime) As ResultInfo(Of Boolean, Status)
    Function GetToBeProcessedFileHandles(FileEntryId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status)
    Function MarkProcessedFileHandles(FileHandleId As Int32, WorkspaceId As Guid) As ResultInfo(Of Boolean, Status)
End Interface
