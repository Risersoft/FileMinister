Imports risersoft.shared.portable.Enums

Public Class HistoryClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/fileversion")
    End Sub

    Public Function DeleteFileVersion(FileVersionId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("FileVersionId", FileVersionId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteFileVersion", data:=obj)
        Return result
    End Function

End Class
