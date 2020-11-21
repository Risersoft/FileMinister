Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports Newtonsoft.Json
Imports risersoft.shared.portable

Public Class FileAgentClient
    Inherits ProxyBase

    Sub New(baseAddress As String, userEmail As String, accessToken As String)
        MyBase.New(baseAddress, "api/FileAgent", userEmail, accessToken)
    End Sub

    Public Function ValidateAgentWithMAC(agentId As Guid, secretKey As String, macAddresses As List(Of String)) As ResultInfo(Of WorkSpaceInfo, Status)
        Dim data As New Dictionary(Of String, Object)()
        data.Add("AgentId", agentId)
        data.Add("SecretKey", secretKey)
        data.Add("MacAddresses", macAddresses)
        Return Me.Post(Of Dictionary(Of String, Object), WorkSpaceInfo)(data, "validate")
    End Function

    Public Function GetLocalShares(WorkSpaceId As Guid) As ResultInfo(Of List(Of LocalShareInfo), Status)
        Dim endpoint As String = String.Format("{0}/localshares", WorkSpaceId.ToString())
        Return Me.Get(Of List(Of LocalShareInfo))(endpoint:=endpoint)
    End Function

End Class
