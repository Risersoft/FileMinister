Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalUserClient
    Inherits LocalClient

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(user As LocalWorkSpaceInfo)
        MyBase.New("api/user", user)
    End Sub

    Public Async Function VerifyUserAsync(windowUserName As String) As Task(Of ResultInfo(Of LocalWorkSpaceInfo, Status))


        Dim result = Await Me.GetAsync(Of LocalWorkSpaceInfo)("verify/user?windowUserName=" + windowUserName)
        Return result
    End Function

    Public Function Log(userAccount As LocalWorkSpaceInfo) As ResultInfo(Of LocalWorkSpaceInfo, Status)
        Dim result = Me.Post(Of LocalWorkSpaceInfo, LocalWorkSpaceInfo)(userAccount, userAccount.UserId.ToString() + "/log")
        Return result
    End Function

    Public Function GetUserConfigured(userId As Guid, AccountName As String, windowUserName As String) As ResultInfo(Of LocalWorkSpaceInfo, Status)
        Dim queryString = New Dictionary(Of String, Object)()

        queryString.Add("windowUserName", windowUserName)
        queryString.Add("AccountName", AccountName)
        Dim uri As String = userId.ToString() + "/configured"
        Dim result = Me.[Get](Of LocalWorkSpaceInfo)(uri,, queryString)
        Return result
    End Function

    Public Function GetEffectivePermissionsOnFile(fileId As Guid, includeDeleted As Boolean) As ResultInfo(Of Byte, Status)
        Dim uri As String = fileId.ToString() + "/" + includeDeleted.ToString() + "/EffectivePermissions"
        Dim result = Me.[Get](Of Byte)(endpoint:=uri)
        Return result
    End Function

    Public Async Function LogOutAsync() As Task(Of ResultInfo(Of Boolean, Status))
        Dim result = Await Me.PostAsync(Of Object, Boolean)(Nothing, "LogOut")
        Return result
    End Function

End Class
