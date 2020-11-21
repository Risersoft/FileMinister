Imports System.Net.Http
Imports System.Text
Imports FileMinister.Models.Sync
Imports Newtonsoft.Json
Imports risersoft.shared

Public MustInherit Class WebApiClientResultLocal
    Inherits WebApiClientResultBase(Of Status)

    Protected Property User As LocalWorkSpaceInfo



    Public Sub New(path As String, user As LocalWorkSpaceInfo)
        Me.path = path
        Me.User = user
        Me.BuildHeaders = Sub(client As HttpClient)
                              Me.BuildHeadersUser(client, Me.User)
                          End Sub

    End Sub

    Protected Friend Overridable Sub BuildHeadersUser(client As HttpClient, user As LocalWorkSpaceInfo)

        If (user IsNot Nothing AndAlso user.AccessToken IsNot Nothing) Then

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.AccessToken)

            Dim dic As New Dictionary(Of String, String)
            dic.Add("UserAccountId", user.UserAccountId.ToString)
            dic.Add("AccountId", user.AccountId.ToString)
            dic.Add("WindowsUser", user.WindowsUserSId)
            dic.Add("Role", user.RoleId.ToString("D"))
            dic.Add("WorkSpaceId", user.WorkSpaceId.ToString())

            Dim sdata = JsonConvert.SerializeObject(dic)

            Dim data = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sdata))

            client.DefaultRequestHeaders.Add("UserAccount", data)
        End If
    End Sub
    Protected Overrides Function GetStatus(IsError As Boolean) As Status
        If IsError Then Return Status.Error Else Return Status.Success
    End Function

End Class
