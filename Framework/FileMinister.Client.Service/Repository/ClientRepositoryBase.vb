Imports risersoft.shared.portable.Model
Imports Microsoft.Practices.Unity
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.web

Public Class ClientRepositoryBase(Of T As BaseInfo, TId)
    Inherits RepositoryBase(Of T, TId, LocalWorkSpaceInfo)

    Protected Const ConnectionStringTemplate As String = "metadata=res://*/DB.{0}.csdl|res://*/DB.{0}.ssdl|res://*/DB.{0}.msl;provider=System.Data.SqlClient;provider connection string='data source=(LocalDB)\MSSQLLocalDB;attachdbfilename={1};integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework'"
    Protected Const EntityNameClient As String = "FileMinisterClient"
    Dim entityNameCommonClient As String = "FileMinisterClientCommon"
    Protected Function GetClientCommonEntity() As FileMinisterClientCommonEntities
        Dim commonPath = Globals.myApp.objAppExtender.CommonAppDataPath
        Dim dbPath As String = System.IO.Path.Combine(commonPath, "FileMinisterClientCommon.mdf")
        Dim connStr = String.Format(ConnectionStringTemplate, entityNameCommonClient, dbPath)
        Return New FileMinisterClientCommonEntities(connStr)
    End Function

    Protected Function GetClientEntity() As FileMinisterClientEntities
        Dim cacheProvider = UserAccountCacheProvider.GetInstance()

        Dim repository = Helper.UnityContainer.Resolve(Of IUserRepository)()
        Dim key = ConstructKey(Me.User)
        Dim userAccount = cacheProvider.Get(key)
        If userAccount Is Nothing Then
            Dim res = repository.GetUserAccount(User)
            If res.Status = Status.Success Then
                userAccount = res.Data
            End If
            If userAccount IsNot Nothing Then
                cacheProvider.Add(key, userAccount)
            End If
        End If

        Dim dbPath As String = userAccount.LocalDatabaseName
        Dim connStr = String.Format(ConnectionStringTemplate, EntityNameClient, dbPath)
        Return New FileMinisterClientEntities(connStr)
    End Function

    Private Shared Function ConstructKey(user As LocalWorkSpaceInfo)
        Return user.UserId.ToString() + "::" + user.AccountId.ToString() + "::" + user.WindowsUserSId
    End Function

End Class
'https://connect.microsoft.com/SQLServer/feedback/details/539703/access-denied-attaching-a-database-when-permissions-are-inherited
'http://stackoverflow.com/questions/2330439/access-is-denied-when-attaching-a-database
'http://stackoverflow.com/questions/689355/metadataexception-unable-to-load-the-specified-metadata-resource