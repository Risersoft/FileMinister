Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports Newtonsoft.Json
Imports risersoft.shared.portable

Public Class GroupFilePermissionClient
    Inherits ProxyBase

    Sub New(baseAddress As String, userEmail As String, accessToken As String)
        MyBase.New(baseAddress, "api/GroupFilePermission", userEmail, accessToken)
    End Sub

    Public Function GetGroupFilePermissionsForShare(shareId As Short) As ResultInfo(Of List(Of FilePermissionInfo), Status)
        Dim endpoint As String = String.Format("GetGroupFilePermissionsForShare/{0}", shareId.ToString())
        Return Me.Get(Of List(Of FilePermissionInfo))(endpoint:=endpoint)
    End Function

End Class
