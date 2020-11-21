Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports CloudSync.Common.Model

Namespace CloudSync.UI.Service
    Public Class ConfigurationClient
        Public Function GetConfiguration() As IList(Of ConfigurationInfo)
            Dim client As New HttpClient()
            Dim result = client.GetStringAsync("http://localhost/Mock/data/configuration.json").Result
            result = Regex.Replace(result, "(?<!\\)  # lookbehind: Check that previous character isn't a \" & vbCr & vbLf & " \\  # match a \" & vbCr & vbLf & " (?!\\)     # lookahead: Check that the following character isn't a \", "\\", RegexOptions.IgnorePatternWhitespace)
            Dim configuration = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of ConfigurationInfo))(result)

            Return configuration
        End Function
    End Class
End Namespace


