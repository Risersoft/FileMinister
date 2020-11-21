Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json.Linq

Public Class ServiceTokenHelper
    Private Property ClientId As String
    Private Property ClientSecret As String
    Private Property LoginUrl As String
    Private Property ExpiryDateTime As DateTime
    Private Property RefreshToken As String
    Private Property AccessToken As String
    Friend ReadOnly Property ServerAccessToken As String
        Get
            Return Me.AccessToken
        End Get
    End Property

    Sub New(ByVal authorityUrl As String, ByVal clientId As String, ByVal clientSecret As String)
        Me.ClientId = clientId
        Me.ClientSecret = clientSecret
        Me.LoginUrl = authorityUrl
    End Sub

    Friend Sub ValidateAndRefreshToken()
        If DateTime.Now.AddMinutes(5) > ExpiryDateTime Then
            SetTokens()
        End If
    End Sub

    Friend Sub SetTokens()
        Dim dic As New Dictionary(Of String, String)()
        dic.Add("client_id", Me.ClientId)
        dic.Add("client_secret", Me.ClientSecret)
        dic.Add("grant_type", "client_credentials")

        PostAndSet(dic)
    End Sub

    Private Sub PostAndSet(dic As Dictionary(Of String, String))
        Dim curDate As DateTime = DateTime.Now
        Using client = GetHttpClient()
            Dim requestContent = New FormUrlEncodedContent(dic)
            Dim result = client.PostAsync("oAuth/Token", requestContent).Result

            If result.IsSuccessStatusCode AndAlso result.Content IsNot Nothing Then
                Dim json As String = result.Content.ReadAsStringAsync().Result
                Dim obj As JObject = JObject.Parse(json)
                Me.AccessToken = obj.Value(Of String)("access_token")
                Me.RefreshToken = obj.Value(Of String)("refresh_token")
                Dim expInterval As Integer = obj.Value(Of Integer)("expires_in")
                Me.ExpiryDateTime = curDate.AddSeconds(expInterval)
            Else
                Throw New Exception("Server Access Token could not be fetched. StatusCode " & result.StatusCode.ToString())
            End If

        End Using
    End Sub

    Private Function GetHttpClient() As HttpClient
        Dim client = New HttpClient()
        client.BaseAddress = New Uri(Me.LoginUrl)
        client.DefaultRequestHeaders.Accept.Clear()
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"))

        Return client
    End Function
End Class
