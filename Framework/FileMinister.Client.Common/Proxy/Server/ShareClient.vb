Imports FileMinister.Models.Sync
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class ShareClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/Config")
    End Sub
    Public Function ShareMapDetails() As ResultInfo(Of List(Of AgentShareMappingInfo), Status)
        Dim result = Me.Get(Of List(Of AgentShareMappingInfo))(endpoint:="ShareMapDetails")
        Return result
    End Function


End Class
