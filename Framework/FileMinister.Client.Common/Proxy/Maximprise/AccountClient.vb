Imports System.Net.Http
Imports System.Text

Public Class AccountClient
    Inherits ServiceClient2
    Public Sub New()
        MyBase.New("api/Account")
    End Sub

    Private Shared serviceUrl As String = Nothing
    Public Async Function GetAuthProviderAsync(userName As String) As Task(Of ResultInfo(Of Users))
        Dim data As New Dictionary(Of String, String)
        data.Add("email", userName)
        data.Add("passwordHash", "")
        data.Add("accountName", "")
        Dim result = Await Me.PostAsync(Of IDictionary(Of String, String), Users)(data, "UserAuthProvider")
        Return result
    End Function

    Public Function SendOTP(id As String, email As String, accountName As String, phoneNumber As String, isTwoFactorAuthentication As Boolean) As ResultInfo(Of Users)
        Dim data As New Dictionary(Of String, String)
        data.Add("Id", id)
        data.Add("email", email)
        data.Add("accountName", accountName)
        data.Add("PhoneNumber", phoneNumber)
        data.Add("passwordHash", "")
        data.Add("IsTwoFactorAuthentication", isTwoFactorAuthentication)
        Dim result = Me.Post(Of IDictionary(Of String, String), Users)(data, "SendOTP")
        Return result
    End Function

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

    Public Async Function SendOTPAsync(user As Users) As Task(Of ResultInfo(Of Users))
        Dim dict = New Dictionary(Of String, Object)()
        dict.Add("email", user.Email)
        dict.Add("passwordHash", "")
        dict.Add("accountName", "")
        dict.Add("PhoneNumber", user.PhoneNumber)
        dict.Add("Id", user.UserId)
        dict.Add("IsTwoFactorAuthentication", user.IsTwoFactorAuthentication)

        Dim data = Await Me.PostAsync(Of Object, Users)(dict, "SendOTP")
        Return data
    End Function

    Public Async Function GetAccountsAsync() As Task(Of ResultInfo(Of List(Of ActiveUser)))
        Dim data = Await Me.GetAsync(Of List(Of ActiveUser))("accounts?moduleName=Cloud Sync")
        Return data
    End Function

    Public Overrides Function GetServiceUrl() As String
        If serviceUrl Is Nothing Then
            serviceUrl = RegistryHelper.GetRegistryValue("SOFTWARE\Risersoft\FileMinister", "ServiceUrl")
        End If
        Return serviceUrl
    End Function
End Class
