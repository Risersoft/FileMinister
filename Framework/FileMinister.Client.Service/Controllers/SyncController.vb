Imports System.Web.Http
Imports risersoft.shared.portable.Model

<RoutePrefix("api/sync")>
Public Class SyncController
    Inherits LocalApiController(Of LocalWorkSpaceInfo, Integer, ISyncRepository)

    Public Sub New(repository As ISyncRepository)
        MyBase.New(repository)
    End Sub

    <Route("{deltaSec}/Cache")>
    <HttpPost>
    Public Function UpdateFileSystemCache(share As ConfigInfo, deltaSec As Integer) As IHttpActionResult
        Dim result = repository.UpdateFileSystemCache(share, deltaSec)
        Return Ok(result)
    End Function

    <Route("ResetSyncTimestamp")>
    <HttpPost>
    Public Function ResetSyncTimestamp() As IHttpActionResult
        Dim result = repository.ResetSyncTimestamp()
        Return Ok(result)
    End Function

End Class
