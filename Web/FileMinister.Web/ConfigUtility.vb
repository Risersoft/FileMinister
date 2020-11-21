Public Class ConfigUtility

    Public Shared Function ReadSetting(key As String) As String
        Dim result As String = String.Empty
        Try
            Dim appSettings = ConfigurationManager.AppSettings
            result = appSettings(key)
            If IsNothing(result) Then
                result = ""
            End If
        Catch e As ConfigurationErrorsException
            result = ""
        End Try

        Return result

    End Function
End Class
