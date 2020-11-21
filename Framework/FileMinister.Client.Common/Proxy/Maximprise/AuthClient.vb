Imports System.Net.Http
Imports System.Text
Imports CloudSync.Common

Public Class AuthClient
    Inherits ServiceClient2
    Public Sub New()
        MyBase.New("api")
    End Sub

    Public Async Function AuthenticateUserAsync(userName As String, password As String) As Task(Of ResultInfo(Of Users))
        Using client As New HttpClient()
            client.BaseAddress = New Uri(Me.BaseAddress)
            Dim postData = "grant_type=password&username=" + userName.Trim() + "&password=" + password.Trim()
            Dim request As New HttpRequestMessage(HttpMethod.Post, "/api/token")
            request.Headers.Add("User-Agent", "self")
            request.Content = New StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")
            Dim response = Await client.SendAsync(request)

            Dim result = BuildResponse(Of Users)(response)
            Return result
        End Using
        'Dim response = Await Me.Client.PostAsync("api/token", New StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"))
        'Return result
    End Function
End Class
