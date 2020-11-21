Imports System.Net
Imports risersoft.shared.portable.Enums

Public Class LocalAgentClient
    Inherits LocalClient
    Public Sub New()
        MyBase.New("api/agent")
    End Sub

    Public Async Function SyncSharesAsync(agentId As Guid) As Task(Of ResultInfo(Of Boolean, Status))
        Dim result = Await Me.PostAsync(Of Object, Boolean)(Nothing, agentId.ToString() + "/sync-share")
        Return result
    End Function

    Public Function SyncShares() As ResultInfo(Of Boolean, Status)
        Dim result = Me.Post(Of Object, Boolean)(Nothing, Guid.Empty.ToString + "/sync-share")
        Return result
    End Function
End Class
