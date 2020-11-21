Imports System.ComponentModel.DataAnnotations

Public Class AccessTokenInfo

    <Display(Name:="access_token")>
    Public Property AccessToken As String

    <Display(Name:="token_type")>
    Public Property TokenType As String

    <Display(Name:="expires_in")>
    Public Property ExpiresIn As Integer

    <Display(Name:="refresh_token")>
    Public Property RefreshToken As String

End Class



