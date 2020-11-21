Imports System.Net
Imports System.Net.Http
Imports risersoft.shared
Imports risersoft.shared.portable.Enums

Public MustInherit Class WebApiClientResultServer
    Inherits WebApiClientResultBase(Of Status)

    Protected Property AccessToken As String
    Protected Property AccountName As String



    Public Sub New(path As String)
        Me.path = path
        Me.BuildHeaders = Sub(client As HttpClient)
                              client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken)
                              client.DefaultRequestHeaders.Add("AccountName", AccountName)
                          End Sub
    End Sub


    Protected Overrides Function BuildResponse(Of TOutput)(response As HttpResponseMessage) As ResultInfo(Of TOutput, Status)
        Return MyBase.BuildResponse(Of TOutput)(response)
    End Function
    Protected Overrides Function GetStatus(IsError As Boolean) As Status
        If IsError Then Return Status.Error Else Return Status.Success
    End Function

End Class
