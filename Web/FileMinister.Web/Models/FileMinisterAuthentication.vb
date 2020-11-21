Imports System.Collections
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json

Public Class FileMinisterAuthentication
    Private ReadOnly _clientId As String
    Private ReadOnly _clientSecret As String
    Private ReadOnly _authorityUrl As String
    Private ReadOnly _localUrl As String
    Private _tokenInfo As AccessTokenInfo

    Public Sub New(ByVal clientId As String, ByVal clientSecret As String, clientCode As String, ByVal authorityUrl As String, ByVal localUrl As String)
        MyBase.New
        Me._clientId = clientId
        Me._clientSecret = clientSecret
        Me._authorityUrl = authorityUrl
        Me._localUrl = localUrl & "/account"
        Me._tokenInfo = Me.HttpPost(authorityUrl, clientCode)
    End Sub

    Public Sub New(ByVal clientId As String, ByVal clientSecret As String, ByVal authorityUrl As String)
        MyBase.New
        Me._clientId = clientId
        Me._clientSecret = clientSecret
        Me._authorityUrl = authorityUrl
    End Sub

    Public Function GetAccessToken(clientCode As String) As AccessTokenInfo
        Me._tokenInfo = Me.HttpPost(_authorityUrl, clientCode)
        Return Me._tokenInfo
    End Function

    Public Function GetAccessToken() As AccessTokenInfo
        Return Me._tokenInfo
    End Function

    Public Function RefreshAccessToken(ByVal refreshToken As String) As AccessTokenInfo
        Return Me.RefreshTokenPost(_authorityUrl, refreshToken)
    End Function

    Private Function HttpPost(ByVal authorityUri As String, ByVal clientCode As String) As AccessTokenInfo
        'Prepare OAuth request 
        Dim url As String = authorityUri + "/OAuth/Token"
        Dim redirectURI As String = _localUrl

        'Build up the body for the token request
        Dim postData = New Dictionary(Of String, String)()
        postData.Add("grant_type", "authorization_code")
        postData.Add("code", clientCode)
        postData.Add("client_id", _clientId)
        postData.Add("client_secret", _clientSecret)
        postData.Add("redirect_uri", redirectURI)
        GetAccessTokenInfo(url, postData)
        Return Me._tokenInfo

    End Function

    Private Shared Function GetJSonData(response As HttpResponseMessage) As Dictionary(Of String, String)
        Dim jsonString As String = response.Content.ReadAsStringAsync().Result
        Dim responseData As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(jsonString)
        Return responseData
    End Function

    Private Function RefreshTokenPost(ByVal authorityUri As String, ByVal refreshToken As String) As AccessTokenInfo
        'Prepare OAuth request 
        Dim url As String = authorityUri + "/OAuth/Token"
        Dim redirectURI As String = _localUrl

        'Build up the body for the token request
        Dim postData = New Dictionary(Of String, String)()
        postData.Add("grant_type", "refresh_token")
        postData.Add("refresh_token", refreshToken)
        postData.Add("client_id", _clientId)
        postData.Add("client_secret", _clientSecret)
        postData.Add("redirect_uri", redirectURI)

        GetAccessTokenInfo(url, postData)
        Return Me._tokenInfo

    End Function

    Private Sub GetAccessTokenInfo(url As String, postData As Dictionary(Of String, String))
        Dim Content As FormUrlEncodedContent = New FormUrlEncodedContent(postData)

        'Post to the Server And parse the response.
        Dim client As HttpClient = New HttpClient()
        Dim response As HttpResponseMessage = client.PostAsync(url, Content).Result

        _tokenInfo = New AccessTokenInfo()

        'return the Access Token.
        If (response.StatusCode = HttpStatusCode.OK) Then
            Try
                Dim jsonString As String = response.Content.ReadAsStringAsync().Result
                Dim responseData As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(jsonString)

                _tokenInfo.AccessToken = responseData("access_token")
                _tokenInfo.RefreshToken = responseData("refresh_token")
                _tokenInfo.ExpiresIn = Convert.ToInt32(responseData("expires_in"))
                _tokenInfo.TokenType = responseData("token_type")

            Catch ex As Exception
                _tokenInfo.AccessToken = String.Empty
            End Try

        End If


    End Sub
End Class
