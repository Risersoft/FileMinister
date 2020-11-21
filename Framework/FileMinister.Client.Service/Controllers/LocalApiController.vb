Imports System.Text
Imports risersoft.shared.portable.Model
Imports Newtonsoft.Json
Imports risersoft.shared.portable.Enums
Imports System.Security.Claims
Imports System.Web.Http.Controllers

Public Class LocalApiController(Of T As BaseInfo, TId, TRepo As IRepositoryBase(Of T, TId, LocalWorkSpaceInfo))
    Inherits ApiControllerRepoBase(Of T, TId, LocalWorkSpaceInfo, TRepo)

    Public Sub New(repository As TRepo)
        MyBase.New(repository)
    End Sub
    Protected Overrides Sub Initialize(controllerContext As HttpControllerContext)

        If repository.User IsNot Nothing Then
            Dim user = repository.User

            If user.AccessToken Is Nothing Then
                Dim authorization = controllerContext.Request.Headers.FirstOrDefault(Function(p) p.Key = "Authorization")
                If authorization.Value IsNot Nothing Then
                    Dim accessToken = authorization.Value.First()
                    If accessToken IsNot Nothing Then
                        user.AccessToken = accessToken.Replace("Bearer ", "").Trim()
                    End If
                End If
            End If
            If controllerContext.Request.Headers.Contains("UserAccount") Then
                Dim value = controllerContext.Request.Headers.GetValues("UserAccount").First()
                If value IsNot Nothing AndAlso value <> String.Empty Then
                    Dim str = value
                    Dim bytes = Convert.FromBase64String(str)
                    Dim data = ASCIIEncoding.ASCII.GetString(bytes)
                    Dim dic = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(data)

                    If dic.ContainsKey("UserAccountId") Then
                        user.UserAccountId = dic("UserAccountId")
                    End If
                    If dic.ContainsKey("AccountId") Then
                        Dim accountId As Guid
                        If Guid.TryParse(dic("AccountId"), accountId) Then
                            user.WorkSpaceId = accountId
                        End If
                    End If
                    If dic.ContainsKey("WindowsUser") Then
                        user.WindowsUserSId = dic("WindowsUser")
                    End If
                    If dic.ContainsKey("WorkSpaceId") Then
                        Dim workSpaceId As Guid
                        If Guid.TryParse(dic("WorkSpaceId"), workSpaceId) Then
                            user.WorkSpaceId = workSpaceId
                        End If
                    End If
                    If dic.ContainsKey("Role") Then
                        user.RoleId = CType(dic("Role"), Role)
                    End If
                End If
            End If
        End If

        MyBase.Initialize(controllerContext)
    End Sub

    Protected Overrides Function GetUserData() As LocalWorkSpaceInfo
        Dim user As LocalWorkSpaceInfo = Nothing

        If Me.User IsNot Nothing AndAlso Me.User.Identity IsNot Nothing AndAlso GetType(ClaimsIdentity) Is Me.User.Identity.GetType() Then
            Dim identity As ClaimsIdentity = Me.User.Identity
            Dim dic = identity.Claims.ToDictionary(Function(c) c.Type, Function(v) v.Value)
            user = New LocalWorkSpaceInfo()
            If (dic.ContainsKey(ClaimTypes.NameIdentifier) = True) Then
                user.UserId = New Guid(dic(ClaimTypes.NameIdentifier).ToString)
            End If
            If (dic.ContainsKey(ClaimTypes.GivenName) = True) Then
                user.FullName = dic(ClaimTypes.GivenName)
            End If
            If (dic.ContainsKey(ClaimTypes.Email) = True) Then
                user.Email = dic(ClaimTypes.Email)
            End If
            If dic.ContainsKey(ClaimTypes.Role) Then
                user.RoleId = CType(dic(ClaimTypes.Role), Role)
            End If

            If dic.ContainsKey("Token") Then
                user.AccessToken = dic("Token")
            End If

        End If
        Return user
    End Function

End Class
