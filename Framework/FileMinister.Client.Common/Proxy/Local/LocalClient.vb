Imports System.Net.Http
Imports System.Security.Claims
Imports System.Text
Imports FileMinister.Models.Sync
Imports FileMinister.Repo
Imports Newtonsoft.Json
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalClient
    Inherits WebApiClientResultLocal


    Private Shared _PassPhrase As String = Nothing
    Private Shared _SyncObj As New Object()

    Public Overrides ReadOnly Property BaseAddress As String
        Get
            Return LOCAL_SERVER_URI
        End Get
    End Property

    Public Sub New(path As String, Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New(path, user)
    End Sub
    Protected Overrides Sub BuildHeadersUser(client As HttpClient, user As LocalWorkSpaceInfo)
        If user IsNot Nothing Then

            Dim token As String = Nothing

            Dim dic As New Dictionary(Of String, String)

            dic.Add(ClaimTypes.NameIdentifier, user.UserId.ToString)
            If Not String.IsNullOrWhiteSpace(user.FullName) Then
                dic.Add(ClaimTypes.GivenName, user.FullName)
            End If
            If Not String.IsNullOrWhiteSpace(user.Email) Then
                dic.Add(ClaimTypes.Email, user.Email)
            End If
            If Not String.IsNullOrWhiteSpace(user.AccessToken) Then
                dic.Add("Token", user.AccessToken)
            End If
            Dim sdata = JsonConvert.SerializeObject(dic)

            token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sdata))

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token)

            Dim dic2 As New Dictionary(Of String, String)
            dic2.Add("UserAccountId", user.UserAccountId)
            dic2.Add("AccountId", user.AccountId.ToString)
            dic2.Add("WindowsUser", user.WindowsUserSId)
            dic2.Add("Role", user.RoleId.ToString("D"))
            dic2.Add("WorkSpaceId", user.WorkSpaceId.ToString())

            Dim sdata2 = JsonConvert.SerializeObject(dic2)

            Dim data2 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sdata2))

            client.DefaultRequestHeaders.Add("UserAccount", data2)
        End If

        SyncLock _SyncObj
            LoadPassPhrase()
            If _PassPhrase IsNot Nothing Then
                client.DefaultRequestHeaders.Add("PassPhrase", _PassPhrase)
            End If
        End SyncLock

    End Sub

    Private Sub LoadPassPhrase()
        If _PassPhrase Is Nothing Then
            Dim keyPath As String = "SOFTWARE\Risersoft\FileMinister"
            Dim keyName As String = "PassPhrase"
            _PassPhrase = RegistryHelper.GetRegistryValue(keyPath, keyName)

            'If _PassPhrase Is Nothing Then
            '    Dim registry = RegistryHelper.CreateSubKey("SOFTWARE", "FileMinister")
            '    If registry IsNot Nothing Then
            '        Dim passPhrase = Guid.NewGuid.ToString()
            '        registry.SetValue("PassPhrase", passPhrase)
            '        _PassPhrase = passPhrase
            '    End If
            'End If

        End If
    End Sub

End Class
