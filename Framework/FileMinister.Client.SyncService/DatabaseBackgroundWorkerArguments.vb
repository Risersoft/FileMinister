Public Class DatabaseBackgroundWorkerArguments
    Public Property UserAccountId As Integer
    Public Property AccountId As Guid
    Public Property AccountName As String
    Public Property FileMinisterDatabaseSyncServiceURL As String
    Public Property CloudSyncServiceURL As String
    Public Property LocalDatabaseName As String
    Public Property AccessToken As String
    Public Property UserId As Guid
    Public Property UserMappedShareIds As String
    Public Property WorkSpaceId As Guid
    Public Property ShareCredentials As System.Collections.Generic.Dictionary(Of Integer, ShareCredentials)
    Public Property WindowsUserSID As String
End Class