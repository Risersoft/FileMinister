Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalTagClient
    Inherits LocalClient
    Public Sub New()
        MyBase.New("api/tag")
    End Sub

    Public Function GetFileTags(fileId As Guid) As ResultInfo(Of List(Of TagInfo), Status)
        Dim result = Me.Get(Of List(Of TagInfo))(endpoint:=fileId.ToString + "/GetFileTags")
        Return result
    End Function

End Class
