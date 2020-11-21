Imports System.Web.Http
Imports risersoft.shared.portable.Model

Namespace Controllers
    <RoutePrefix("api/Tag")>
    Public Class TagController
        Inherits LocalApiController(Of TagInfo, Guid, ITagRepository)

        Public Sub New(repository As ITagRepository)
            MyBase.New(repository)
        End Sub

        ''' <summary>
        ''' Get all tags against a file
        ''' </summary>
        ''' <param name="fileId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <HttpGet>
            <Route("{fileId}/GetFileTags")>
        Public Function GetFileTags(fileId As String) As IHttpActionResult
            Dim result = repository.GetFileTags(New Guid(fileId))
            Return Ok(result)
        End Function

    End Class
End Namespace

