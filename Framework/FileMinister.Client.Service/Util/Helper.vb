Imports Microsoft.Practices.Unity

Public Class Helper

    Public Shared Property UnityContainer As UnityContainer

    'Public Function IsUserShareAdmin(shareId As Int32) As Boolean
    '    Return True
    'End Function


    'Public Function GetConnectionString() As String
    '    Return "data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=C:\dbs\FileMinisterClient.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework"
    'End Function

    Public Shared Function GetParentRelativePath(fileSystemEntryOldRelativePath As String) As String
        Dim relativePath = fileSystemEntryOldRelativePath
        If relativePath.Length > 0 Then
            Dim lastIndex = fileSystemEntryOldRelativePath.LastIndexOf("\")
            If lastIndex >= 0 Then
                relativePath = fileSystemEntryOldRelativePath.Substring(0, lastIndex)
            Else
                relativePath = ""
            End If
        End If
            Return relativePath
    End Function

End Class
