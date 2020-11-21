Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class SyncClient
    Inherits ServiceClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/Sync", user)
    End Sub
    Public Function GetSharedAccessSignatureUrl(ShareId As Integer, BlobName As Guid, SharedAccessSignatureType As Integer, fileSystemEntryId As Guid, Optional filesize As Long = 0) As ResultInfo(Of Uri, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("ShareId", ShareId)
        obj.Add("BlobName", BlobName)
        obj.Add("SharedAccessSignatureType", SharedAccessSignatureType)
        obj.Add("FileSystemEntryId", fileSystemEntryId)
        obj.Add("FileSystemEntrySize", filesize)

        Dim result = Me.Get(Of Uri)(endpoint:="SasUrl", queryString:=obj)
        Return result
    End Function

    Public Function SyncServerData() As ResultInfo(Of Boolean, Status)
        Return Me.Post(Of Object, Boolean)(Nothing, "Server-Data")
    End Function

End Class
