Imports Newtonsoft.Json
Imports System.Net.Http
Imports FileMinister.Common.Model
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

Public MustInherit Class ServiceClient
    Inherits WebApiClientResultServer

    Public Sub New(path As String, user As RSCallerInfo)
        MyBase.New(path, user.AccessToken, user.AccountName)
    End Sub

    Protected Overrides Function BuildResponse(Of TOutput)(response As HttpResponseMessage) As ResultInfo(Of TOutput)
        Dim resultInfo As New ResultInfo(Of TOutput)
        resultInfo.StatusCode = response.StatusCode.ToString("D")
        If resultInfo.StatusCode = 200 Then
            Dim result = JsonConvert.DeserializeObject(Of TOutput)(response.Content.ReadAsStringAsync().Result)
            resultInfo.Data = result
            resultInfo.Status = Status.Success
        End If

        Return resultInfo
    End Function
End Class
