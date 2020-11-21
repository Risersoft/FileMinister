Imports System.Threading
Imports System.Web.Http
Imports risersoft.shared.portable.Enums
Imports FileMinister.Models.Sync
Imports FileMinister.Models.Enums

''' <summary>
''' Tag Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/tag")>
Public Class TagController
    Inherits ServerApiController(Of TagInfo, Guid, ITagRepository)

    Public Sub New(repository As ITagRepository)
        MyBase.New(repository)
    End Sub

    ''' <summary>
    ''' Get All Tags on File
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("file/{fileId}")>
    <HttpGet>
    Public Function GetByFileId(fileId As Guid) As IHttpActionResult
        Dim result = repository.GetByFileId(fileId)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Add, update and remove tags on a file
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Route("savetags")>
    <HttpPost>
    Public Function ManageTags(data As Dictionary(Of String, Object)) As IHttpActionResult

        Dim result As ResultInfo(Of Boolean, Status) = New ResultInfo(Of Boolean, Status) With {.Status = Status.Error}

        Dim fileId As Guid = New Guid(data("item1").ToString())
        Dim addedTagInfo As List(Of TagInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of TagInfo))(data("item2").ToString())
        Dim updatedTagInfo As List(Of TagInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of TagInfo))(data("item3").ToString())
        Dim removedTagInfo As List(Of TagInfo) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of TagInfo))(data("item4").ToString())

        Dim dataTuple As Tuple(Of Guid, List(Of TagInfo), List(Of TagInfo), List(Of TagInfo)) = Tuple.Create(fileId, addedTagInfo, updatedTagInfo, removedTagInfo)
        result = repository.ManageTags(dataTuple)

        Return Ok(result)

    End Function

End Class
