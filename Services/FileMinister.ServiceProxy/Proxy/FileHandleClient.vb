Imports FileMinister.Models.Sync
Imports FileMinister.Models.Enums
Imports Newtonsoft.Json
Imports risersoft.shared.portable

Public Class FileHandleClient
    Inherits ProxyBase

    Sub New(baseAddress As String, userEmail As String, accessToken As String)
        MyBase.New(baseAddress, "api/FileHandle", userEmail, accessToken)
    End Sub

    Public Function GetOpenFileHandlesForWorkspace(workSpaceId As Guid) As ResultInfo(Of List(Of FileHandleInfo), Status)
        Dim endpoint As String = String.Format("{0}/GetOpenFileHandlesForWorkspace", workSpaceId.ToString())
        Return [Get](Of List(Of FileHandleInfo))(endpoint:=endpoint)
    End Function

    Public Function OpenFileHandle(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status)
        Dim dic As New Dictionary(Of String, Object)()
        dic.Add("RelativePath", RelativePath)
        dic.Add("ShareId", ShareId)
        dic.Add("WorkSpaceId", WorkspaceId)
        dic.Add("ServerFileSize", ServerFileSize)
        dic.Add("ServerFileTime", ServerFileTime)
        Return [Post](Of Dictionary(Of String, Object), Boolean)(dic, "OpenFileHandle")
    End Function

    Public Function CloseFile(RelativePath As String, ShareId As Short, WorkspaceId As Guid, ServerFileSize As Integer, ServerFileTime As Date) As ResultInfo(Of Boolean, Status)
        Dim dic As New Dictionary(Of String, Object)()
        dic.Add("RelativePath", RelativePath)
        dic.Add("ShareId", ShareId)
        dic.Add("WorkSpaceId", WorkspaceId)
        dic.Add("ServerFileSize", ServerFileSize)
        dic.Add("ServerFileTime", ServerFileTime)
        Return [Post](Of Dictionary(Of String, Object), Boolean)(dic, "CloseFile")
    End Function

End Class
