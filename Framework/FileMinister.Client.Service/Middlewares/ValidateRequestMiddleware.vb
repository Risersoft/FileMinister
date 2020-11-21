Imports System.Net
Imports Microsoft.Owin
Imports risersoft.shared.cloud

Public Class ValidateRequestMiddleware
    Inherits OwinMiddleware
    Private Shared _PassPhrase As String = Nothing
    Private Shared _SyncObj As New Object()

    Public Sub New([next] As OwinMiddleware)
        MyBase.New([next])
    End Sub

    Public Overrides Async Function Invoke(context As IOwinContext) As Task

        If context.Request.Uri.Host <> "localhost" Then
            context.Response.StatusCode = HttpStatusCode.BadRequest
        Else
            Dim isValid As Boolean = False

            Dim hPassPhrase = context.Request.Headers.FirstOrDefault(Function(p) p.Key.ToLower() = "passphrase")
            If hPassPhrase.Value IsNot Nothing AndAlso hPassPhrase.Value.Length > 0 AndAlso Not String.IsNullOrWhiteSpace(hPassPhrase.Value(0)) Then
                Dim passPhrase = GetPassPhrase()
                isValid = (hPassPhrase.Value(0) = passPhrase)
            End If

            If isValid Then
                Await [Next].Invoke(context)
            Else
                context.Response.StatusCode = HttpStatusCode.BadRequest
            End If
        End If
    End Function

    Private Function GetPassPhrase() As String
        SyncLock _SyncObj
            If _PassPhrase Is Nothing Then
                Dim keyPath As String = "SOFTWARE\Risersoft\FileMinister"
                Dim keyName As String = "PassPhrase"
                _PassPhrase = RegistryHelper.GetRegistryValue(keyPath, keyName)
            End If
            Return _PassPhrase
        End SyncLock
    End Function
End Class
