Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalFileClient
    Inherits LocalClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/file", user)
    End Sub

    Public Function [ResolveConflictUsingTheirs](fileSystemEntryId As Guid, shareId As Integer, lastFullPath As String, lastFileName As String) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("shareId", shareId)
        obj.Add("lastFullPath", lastFullPath)
        obj.Add("lastFileName", lastFileName)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="ResolveConflictUsingTheirs", data:=obj)
        Return result
    End Function

    Public Function [ResolveConflictUsingMine](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="ResolveConflictUsingMine", data:=obj)
        Return result
    End Function

    Public Function [SoftDelete](fileSystemEntryId As Guid, isForce As Boolean) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("isForce", isForce)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="SoftDelete", data:=obj)
        Return result
    End Function

    Public Function [HardDelete](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="HardDelete", data:=obj)
        Return result
    End Function

    Public Function [DeleteFile](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteFile", data:=obj)
        Return result
    End Function

    Function DeleteFiles(baseRelativePath As String, shareId As Integer) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("baseRelativePath", baseRelativePath)
        obj.Add("shareId", shareId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteFiles", data:=obj)
        Return result
    End Function

    Public Function GetFiles() As ResultInfo(Of List(Of FileEntryInfo), Status)
        Return Me.Get(Of List(Of FileEntryInfo))()
    End Function

    Public Async Function GetByShareIdAsync(shareId As Integer) As Task(Of ResultInfo(Of List(Of FileEntryInfo), Status))
        Dim result = Await Me.GetAsync(Of List(Of FileEntryInfo))("Share/" + shareId.ToString())
        Return result
    End Function

    Public Function GetFile(fileSystemEntryId As Guid) As ResultInfo(Of FileEntryInfo, Status)
        Return Me.Get(Of FileEntryInfo)(fileSystemEntryId.ToString())
    End Function

    Public Async Function GetFilesAsync() As Task(Of ResultInfo(Of List(Of FileEntryInfo), Status))
        Dim result = Await Me.GetAsync(Of List(Of FileEntryInfo))()
        Return result
    End Function

    Public Async Function GetFilesByParentIdAsync(parentId As Guid) As Task(Of ResultInfo(Of List(Of FileEntryInfo), Status))
        Dim result = Await Me.GetAsync(Of List(Of FileEntryInfo))(parentId.ToString() + "/children")
        Return result
    End Function

    Public Function [FileSearch](search As FileSearch) As ResultInfo(Of List(Of FileEntryInfo), Status)
        Dim result = Me.Post(Of FileSearch, List(Of FileEntryInfo))(endpoint:="FileSearch", data:=search)
        Return result
    End Function

    Public Function [CheckOut](fileSystemEntryId As Guid, workSpaceId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        obj.Add("workSpaceId", workSpaceId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="CheckOut", data:=obj)
        Return result
    End Function

    Public Function [CheckIn](FileVersionId As Guid, versionNumber As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("FileVersionId", FileVersionId)
        obj.Add("versionNumber", versionNumber)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="CheckIn", data:=obj)
        Return result
    End Function

    Public Function UndoCheckOut(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UndoCheckOut", data:=obj)
        Return result
    End Function

    Public Function GetFileLinks(fileSystemEntryId As Guid) As ResultInfo(Of List(Of FileEntryLinkInfo), Status)
        Return Me.Get(Of List(Of FileEntryLinkInfo))(fileSystemEntryId.ToString() + "/Links")
    End Function

    Public Function DeleteDeltaOperationsForFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteDeltaOperationsForFile", data:=obj)
        Return result
    End Function

    Public Function UndoDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UndoDelete", data:=obj)
        Return result
    End Function

    Public Function GetMyConflict(fileSystemEntryId As Guid) As ResultInfo(Of FileVersionConflictInfo, Status)
        Return Me.Get(Of FileVersionConflictInfo)("MyConflict/" + fileSystemEntryId.ToString())
    End Function

    Public Function [IsFilePreviouslyLinked](fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileSystemEntryId", fileSystemEntryId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="IsFilePreviouslyLinked", data:=obj)
        Return result
    End Function
End Class
