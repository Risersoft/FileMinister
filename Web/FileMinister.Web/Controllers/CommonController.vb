Imports System.Web.Http
Imports risersoft.shared.portable.Model
Imports FileMinister.Models.Sync
Imports FileMinister.Models.Enums
Imports risersoft.shared.cloud
Imports risersoft.shared.cloud.Providers
Imports risersoft.shared.portable.Proxies
Imports risersoft.shared
Imports risersoft.shared.portable
Imports risersoft.shared.web
Imports System.Security.Claims

<RoutePrefix("api/common")>
Public Class CommonController
    Inherits ServerApiController(Of BaseInfo, Integer, IAccountRepository)

    Public Sub New(repository As IAccountRepository)
        MyBase.New(repository)
    End Sub

    <HttpGet>
    <Route("usersgroups/{searchText}")>
    Public Function [GetAllUsersAndGroups](searchText As String) As IHttpActionResult

        Dim result = New ResultInfo(Of List(Of UserGroupInfo), Status)
        Dim convetedUserList As New List(Of UserGroupInfo)
        Dim convetedGroupList As New List(Of UserGroupInfo)

        'Get Users based on the searchtext
        Dim userResult = Util.Helper.GetAllUsers(searchText, Me.repository.User)

        If (userResult.Data IsNot Nothing) Then
            convetedUserList = userResult.Data.ConvertAll(Function(s) New UserGroupInfo With {.Id = s.UserId, .Name = s.UserName, .Type = UserGroupType.User})
        End If

        'Get Groups based on the searchtext
        Dim groupResult = Util.Helper.GetAllGroup(searchText, Me.repository.User)
        If (groupResult.Data IsNot Nothing) Then
            convetedGroupList = groupResult.Data.ConvertAll(Function(s) New UserGroupInfo With {.Id = s.GroupId, .Name = s.GroupName, .Type = UserGroupType.Group})
        End If

        'merge users and groups into UserGroupInfo
        convetedUserList.AddRange(convetedGroupList)
        result.Data = convetedUserList

        Return Ok(result)

    End Function
    Protected Friend Function GetAccountList() As List(Of UserAccountProxy)
        'Dim provider As IAuthProvider = New AuthProviderTrustedApp(Host)
        Dim provider = New AuthProviderTrustedApp
        Dim SiteApps As New List(Of String) From {"fmxp"}
        Dim AccountList = provider.GetAccountList(Me.repository.User.identity.UniqueName, "active")
        Dim ServedList = AccountList.Where(Function(x)
                                               Return myNav.CheckSiteServesAccount(x.AppList, SiteApps)
                                           End Function).ToList
        Return ServedList
    End Function

    <HttpGet>
    <Route("accounts")>
    Public Function GetAccounts() As IHttpActionResult
        Dim result = New ResultInfo(Of List(Of UserAccountProxy), Status)
        Try
            result.Data = Me.GetAccountList
        Catch ex As Exception
            Trace.WriteLine("Error while obtaining site applist:" & ex.Message)
        End Try
        Return Ok(result)
    End Function

    <HttpGet>
    <Route("account")>
    Public Function GetAccountDetails() As IHttpActionResult
        Dim result = New ResultInfo(Of UserAccountProxy, Status)
        Try
            Dim basehost As String = Me.Host.BaseHost
            Dim host As String = Me.Request.RequestUri.Host
            Dim tenantsubdomain As String = Replace(host, basehost, "",,, CompareMethod.Text).Trim(".").Trim
            Dim lst = Me.GetAccountList
            'Dim found = lst.Where(Function(x) myUtils.IsInList(x.AccountName, tenantsubdomain)).Count > 0
            Dim found = lst.FirstOrDefault(Function(x) myUtils.IsInList(x.AccountName, tenantsubdomain))
            If found IsNot Nothing Then result.Data = found 'tenantsubdomain

        Catch ex As Exception
            Trace.WriteLine("Error while obtaining site applist:" & ex.Message)
        End Try
        Return Ok(result)
    End Function

    <HttpGet>
    <Route("check")>
    Public Function CheckLogin() As IHttpActionResult
        Dim result = New ResultInfo(Of Boolean, Status)
        result.Data = True
        Return Ok(result)
    End Function

    <HttpGet>
    <Route("user")>
    Public Function GetUserDetails() As IHttpActionResult
        Dim result As RSUser = OwinHelper.GetRSUser(ClaimsPrincipal.Current.Identity)
        Return Ok(result)
    End Function
End Class
