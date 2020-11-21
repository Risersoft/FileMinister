Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalSyncClient
    Inherits LocalClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/sync", user)
    End Sub

    Public Function UpdateFileSystemCache(share As ConfigInfo, deltaSec As Integer) As ResultInfo(Of Boolean, Status)
        Me.RequestTimeout = TimeSpan.FromMinutes(10)
        Dim dic As New Dictionary(Of String, Object)()
        Dim result = Me.Post(Of ConfigInfo, Boolean)(share, deltaSec.ToString() + "/Cache")
        Return result
    End Function

    Public Async Function ResetSyncTimestampAsync() As Task(Of ResultInfo(Of Boolean, Status))
        Dim result = Await Me.PostAsync(Of Object, Boolean)(Nothing, "/ResetSyncTimestamp")
        Return result
    End Function

End Class
