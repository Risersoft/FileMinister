Imports System.Net
Imports FileMinister.Repo
Imports risersoft.shared.portable.Model

Public Class ServiceClient
    Inherits WebApiClientResultServer

    Dim m_User As LocalWorkSpaceInfo

    Public Overrides ReadOnly Property BaseAddress As String
        Get
            Return m_User.CloudSyncServiceUrl '"http://localhost:50648"
        End Get
    End Property

    Public Sub New(path As String, Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New(path)
        m_User = user
        Me.AccessToken = m_User.AccessToken
        Me.AccountName = m_User.AccountName

    End Sub

    Public Function CheckForInternetConnection() As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead("http://www.google.com")
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

End Class
