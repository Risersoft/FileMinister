Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.shared.cloud.Repository
Imports risersoft.shared.cloud.IRepository
Imports risersoft.shared.agent
''' <summary>
''' Base Server Repository
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="TId"></typeparam>
''' <remarks></remarks>
Public Class ServerRepositoryBase(Of T As BaseInfo, TId)
    Inherits RepositoryBase(Of T, TId, RSCallerInfo)
    Protected Function GetServerEntity() As FileMinisterEntities
        Dim strConn = AgentAuthProvider.CalculateConnectionString(Me.User, "FileMinister", "File")
        Return New FileMinisterEntities(strConn.ConnectionString, Me.User.UserAccount.AccountId)
    End Function
    Protected Function GetStorageAccount() As String
        Dim strConn As String = Me.User.Account.StorageAccount
        Return strConn
    End Function
End Class
Public Class RepositoryBase(Of TInfo As BaseInfo, TId, TUser)
    Inherits RepositoryBase(Of TInfo, TId, TInfo, Boolean, TUser, Status)

    Protected Overrides Function GetStatus(IsError As Boolean) As Status
        If IsError Then Return Status.Error Else Return Status.Success
    End Function
End Class
Public Interface IRepositoryBase(Of TInfo As BaseInfo, TId, TUser)
    Inherits IRepositoryBase(Of TInfo, TId, TInfo, Boolean, TUser, Status)
End Interface
