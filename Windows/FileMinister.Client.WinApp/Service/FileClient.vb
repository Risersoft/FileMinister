
Imports CloudSync.Common.Model
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks

Namespace CloudSync.UI.Service
    Public Class FileClient
        Public Function GetFiles() As IList(Of FileSystemEntryInfo)
            
            Dim client As New HttpClient()
            Dim result = client.GetStringAsync("http://localhost/Mock/data/files.json").Result

            Dim files = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of FileSystemEntryInfo))(result)

            Return files
        End Function

        Public Function GetFiles(parentId As Guid) As IList(Of FileSystemEntryInfo)
            Dim files = GetFiles()
            Return files.Where(Function(p) p.ParentFileId = parentId).ToList()
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
