Imports FileMinister.Repo
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Proxies

Public Class OrganizationClient
    Inherits ServiceClient2
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/user", user)
    End Sub


    Public Function GetMyAccountGroupUser(searchText As String, accountName As String) As ResultInfo(Of List(Of Dictionary(Of String, Object)), Status)
        Dim queryString = New Dictionary(Of String, Object)()

        queryString.Add("AccountName", accountName)
        queryString.Add("searchText", searchText)

        Dim data = Me.[Get](Of List(Of Dictionary(Of String, Object)))(endpoint:="Search", queryString:=queryString)
        Return data
    End Function

    Public Function GetAllAccountUser(accountname As String) As ResultInfo(Of List(Of UserAccountProxy), Status)
        Dim queryString = New Dictionary(Of String, Object)()
        queryString.Add("AccountName", accountname)
        Dim data = Me.[Get](Of List(Of UserAccountProxy))(endpoint:="AccountUsers", queryString:=queryString)
        Return data
    End Function


    Public Function GetAccountEmail(userId As Guid) As ResultInfo(Of LocalWorkSpaceInfo, Status)
        Dim queryString = New Dictionary(Of String, Object)()
        queryString.Add("userId", userId)
        Dim result = Me.[Get](Of String)(endpoint:="GetAccountEmail", queryString:=queryString)

        Dim returnResult = New ResultInfo(Of LocalWorkSpaceInfo, Status)()
        returnResult.Status = result.Status
        returnResult.Message = result.Message
        If result.Status = Status.Success AndAlso result.Data IsNot Nothing Then
            Dim user = New LocalWorkSpaceInfo()
            Dim email = result.Data
            user.Email = email
            returnResult.Data = user
        End If
        Return returnResult
    End Function

    Public Overrides Function GetServiceUrl() As String
        Dim user = Me.m_User
        Dim url = user.OrganisationServiceURL
        Return url
    End Function
End Class
