Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class AgentClient
    Inherits ServiceClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/agent", user)
    End Sub

    Public Function ValidateAgentWithMAC(agentName As String, macAddresses As List(Of String), secretKey As String) As ResultInfo(Of AgentMacAddressInfo, Status)
        'Dim dic As New Dictionary(Of String, Object)()
        'dic.Add("agentName", agentName)
        'dic.Add("macAddress", macAddresses)
        'dic.Add("secretKey", secretKey)

        Dim data As New Tuple(Of String, String, List(Of String))(agentName, secretKey, macAddresses)
        Dim result = Me.Post(Of Tuple(Of String, String, List(Of String)), AgentMacAddressInfo)(data, "validate")
        Return result
    End Function

    Public Function GetSharesByAgentId(agentId As Guid) As ResultInfo(Of List(Of ConfigInfo), Status)
        Dim result = Me.[Get](Of List(Of ConfigInfo))(agentId.ToString() & "/shares")
        Return result
    End Function

    Public Function AgentMapDetails() As ResultInfo(Of List(Of AgentShareMappingInfo), Status)
        Dim result = Me.Get(Of List(Of AgentShareMappingInfo))(endpoint:="AgentMapDetails")
        Return result
    End Function

End Class
