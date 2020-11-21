Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports Newtonsoft.Json
Imports risersoft.shared.portable

Public Class UserClient
    Inherits ProxyBase

    Sub New(baseAddress As String, userEmail As String, accessToken As String)
        MyBase.New(baseAddress, "api/user", userEmail, accessToken)
    End Sub

    Public Function GetUserBySID(sId As String, domainId As Integer) As ResultInfo(Of UserInfo, Status)
        Dim endpoint As String = String.Format("GetUserBySID/{0}/{1}", domainId.ToString(), sId)
        Return Me.Get(Of UserInfo)(endpoint:=endpoint)
    End Function

    Public Function GetDomainUsers(domainId As Integer) As ResultInfo(Of List(Of UserInfo), Status)
        Dim endpoint As String = String.Format("{0}/Users", domainId.ToString())
        Return Me.Get(Of List(Of UserInfo))(endpoint:=endpoint)
    End Function

    Public Function GetAccountAdmins() As ResultInfo(Of List(Of UserInfo), Status)
        Return Me.Get(Of List(Of UserInfo))("GetAccountAdmins")
    End Function

End Class
