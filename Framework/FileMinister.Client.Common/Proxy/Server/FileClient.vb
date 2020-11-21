Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports FileMinister.Models.Sync
Public Class FileClient
    Inherits ServiceClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/file", user)
    End Sub

    Public Async Function [GetOtherUsersUnresolvedConflictsAsync](fileId As Guid) As Task(Of ResultInfo(Of List(Of FileVersionConflictInfo), Status))

        Return Await Me.GetAsync(Of List(Of FileVersionConflictInfo))(fileId.ToString() + "/GetOtherUsersUnresolvedConflicts")

        'Dim obj = New Dictionary(Of String, Object)
        'obj.Add("fileSystemEntryId", fileId)
        'Dim result = Me.Get(Of List(Of FileVersionConflictInfo))(endpoint:="GetOtherUsersUnresolvedConflicts", queryString:=obj)
        'Return result
    End Function

    Public Function [GetAllFilesForLinking](fileId As Guid, shareId As Int32) As ResultInfo(Of List(Of FileEntryLinkInfo), Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("shareId", shareId)
        Dim result = Me.Get(Of List(Of FileEntryLinkInfo))(endpoint:="AllFilesForLinking", queryString:=obj)
        Return result
    End Function

    Public Function [AddFileLink](fileSystemEntryId As Guid, linkedFileId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("linkedFileId", linkedFileId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="AddFileLink", data:=obj)
        Return result
    End Function

    Public Async Function RemoveFileLinkAsync(fileSystemEntryId As Guid) As Task(Of ResultInfo(Of Boolean, Status))
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Await Me.PostAsync(Of Dictionary(Of String, Object), Boolean)(endpoint:="RemoveFileLink", data:=obj)
        Return result
    End Function

    Public Function [CheckOut](fileSystemEntryId As Guid, localFileVersionNumber As Int32, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("localFileVersionNumber", localFileVersionNumber)
        obj.Add("workSpaceId", workSpaceId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="CheckOut", data:=obj)
        Return result
    End Function

    Public Function [CheckIn](FileVersionsInfo As FileVersionInfo, localFileVersionNumber As Int32) As ResultInfo(Of Integer, Status)
        Dim tuple As New Tuple(Of FileVersionInfo, Integer)(FileVersionsInfo, localFileVersionNumber)
        Dim result = Me.Post(Of Tuple(Of FileVersionInfo, Integer), Integer)(endpoint:="CheckIn", data:=tuple)
        Return result
    End Function

    Public Function [UndoCheckOut](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UndoCheckOut", data:=obj)
        Return result
    End Function

    Public Function [SoftDelete](fileSystemEntryId As Guid, localFileVersionNumber As Integer) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("localFileVersionNumber", localFileVersionNumber)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="SoftDelete", data:=obj)
        Return result
    End Function

    Public Function [HardDelete](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="HardDelete", data:=obj)
        Return result
    End Function

    Public Function [UndoDelete](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UndoDelete", data:=obj)
        Return result
    End Function

    Public Function [AddFileAndCheckOut](FileEntryInfo As FileEntryInfo, FileVersionsInfo As FileVersionInfo, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
        Dim tuple As New Tuple(Of FileEntryInfo, FileVersionInfo, Guid)(FileEntryInfo, FileVersionsInfo, workSpaceId)
        Dim result = Me.Post(Of Tuple(Of FileEntryInfo, FileVersionInfo, Guid), Boolean)(endpoint:="AddFileAndcheckout", data:=tuple)
        Return result
    End Function

    Public Function [AddFolder](FileEntryInfo As FileEntryInfo, FileVersionsInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Dim tuple As New Tuple(Of FileEntryInfo, FileVersionInfo)(FileEntryInfo, FileVersionsInfo)
        Dim result = Me.Post(Of Tuple(Of FileEntryInfo, FileVersionInfo), Boolean)(endpoint:="AddFolder", data:=tuple)
        Return result
    End Function

    Public Function [AddAndDelete](FileEntryInfo As FileEntryInfo, FileVersionsInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Dim tuple As New Tuple(Of FileEntryInfo, FileVersionInfo)(FileEntryInfo, FileVersionsInfo)
        Dim result = Me.Post(Of Tuple(Of FileEntryInfo, FileVersionInfo), Boolean)(endpoint:="AddAndDelete", data:=tuple)
        Return result
    End Function

    Public Function AddVersionWithoutUpload(FileEntryInfo As FileEntryInfo, FileVersionInfo As FileVersionInfo, links As List(Of FileEntryLinkInfo)) As ResultInfo(Of Integer, Status)
        Dim tuple As New Tuple(Of FileEntryInfo, FileVersionInfo, List(Of FileEntryLinkInfo))(FileEntryInfo, FileVersionInfo, links)
        Dim result = Me.Post(Of Tuple(Of FileEntryInfo, FileVersionInfo, List(Of FileEntryLinkInfo)), Integer)(endpoint:="AddVersionWithoutUpload", data:=tuple)
        Return result
    End Function

    Public Async Function ResolveOthersConflictUsingOthersAsync(shareId As Integer, fileSystemEntryId As Guid, FileVersionConflictId As Guid) As Task(Of ResultInfo(Of Boolean, Status))
        Dim tuple As New Tuple(Of Integer, Guid, Guid)(shareId, fileSystemEntryId, FileVersionConflictId)
        Dim result = Await Me.PostAsync(Of Tuple(Of Integer, Guid, Guid), Boolean)(endpoint:="ResolveOthersConflictUsingOthers", data:=tuple)
        Return result
    End Function

    Public Async Function ResolveOthersConflictUsingServerAsync(shareId As Integer, fileSystemEntryId As Guid, FileVersionConflictId As Guid) As Task(Of ResultInfo(Of Boolean, Status))
        Dim tuple As New Tuple(Of Integer, Guid, Guid)(shareId, fileSystemEntryId, FileVersionConflictId)
        Dim result = Await Me.PostAsync(Of Tuple(Of Integer, Guid, Guid), Boolean)(endpoint:="ResolveOthersConflictUsingServer", data:=tuple)
        Return result
    End Function

    Public Async Function RequestConflictFileUploadAsync(fileSystemEntryId As Guid, FileVersionConflictId As Guid) As Task(Of ResultInfo(Of Boolean, Status))
        Dim tuple As New Tuple(Of Guid, Guid)(fileSystemEntryId, FileVersionConflictId)
        Dim result = Await Me.PostAsync(Of Tuple(Of Guid, Guid), Boolean)(endpoint:="RequestConflictFileUpload", data:=tuple)
        Return result
    End Function

    Public Function GetConflictFilePendingUpload(workspaceId As Guid) As ResultInfo(Of List(Of Guid), Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("workspaceId", workspaceId)
        Dim result = Me.Get(Of List(Of Guid))(endpoint:="GetConflictFilePendingUpload", queryString:=obj)
        Return result
    End Function

    Public Function UpdateConflictUploadStatus(FileVersionConflictId As Guid, fileSize As Long, fileHash As String) As ResultInfo(Of Boolean, Status)
        Dim data As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
        data.Add("fileSize", fileSize)
        data.Add("FileVersionConflictId", FileVersionConflictId)
        data.Add("fileHash", fileHash)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UpdateConflictUploadStatus", data:=data)
        Return result
    End Function

End Class
