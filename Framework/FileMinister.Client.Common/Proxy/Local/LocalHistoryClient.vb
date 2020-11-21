Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports FileMinister.Models.Sync
Public Class LocalHistoryClient
    Inherits LocalClient
    Public Sub New()
        MyBase.New("api/fileversion")
    End Sub

    Public Function [GetAllFileVersions](fileId As Guid) As ResultInfo(Of List(Of FileVersionInfo), Status)
        Dim result = Me.Get(Of List(Of FileVersionInfo))(endpoint:=fileId.ToString() + "/FileVersions")
        Return result
    End Function

    Public Function [DeleteFileVersion](Id As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("Id", Id)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteFileVersion", data:=obj)
        Return result
    End Function

End Class
