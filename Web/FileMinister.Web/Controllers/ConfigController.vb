Imports FileMinister.Models.Sync
Imports System.Web.Http

''' <summary>
''' Config Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/Config")>
Public Class ConfigController
    Inherits ServerApiController(Of ConfigInfo, Short, IConfigRepository)

    Public Sub New(repository As IConfigRepository)
        MyBase.New(repository)
    End Sub

    ' <Route("GetAllShares")>
    '<HttpGet>
    ' Public Function GetAllShares() As IHttpActionResult
    '     Dim result = repository.GetAll()
    '     Return Ok(result)
    ' End Function

    ''' <summary>
    ''' Get All FileShare FileAgent Map Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("ShareMapDetails")>
    <HttpGet>
    Public Function ShareMapDetails() As IHttpActionResult
        Dim result = repository.ShareMapDetails()
        Return Ok(result)
    End Function
End Class
