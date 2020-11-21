Imports System.Net.Http
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

Public Class LocalFileVersionClient
    Inherits LocalClient
    Public Sub New(Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New("api/FileVersion", user)
    End Sub

    Public Function DeleteFileByName(shareId As Short, filename As String) As ResultInfo(Of Boolean, Status)
        Using client = GetHttpClient("DELETE")
            Dim uri As String = ConstructUri()
            uri += "/" + shareId.ToString() + "/deleteFile/?filename=" + filename

            Dim response As HttpResponseMessage = client.DeleteAsync(uri).Result
            Return BuildResponse(Of Boolean)(response)
        End Using
    End Function

    Public Function [UpdateFileVersionStatus](FileVersionId As Guid, fileSystemEntryStatus As FileEntryStatus) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("FileVersionId", FileVersionId)
        obj.Add("fileSystemEntryStatus", Convert.ToByte(fileSystemEntryStatus))

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UpdateFileVersionStatus", data:=obj)
        Return result
    End Function
End Class
