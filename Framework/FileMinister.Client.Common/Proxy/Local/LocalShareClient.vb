Imports FileMinister.Client.Common.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class LocalShareClient
    Inherits LocalClient

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(user As LocalWorkSpaceInfo)
        MyBase.New("api/Share", user)
    End Sub

    Public Function GetAccountShares() As ResultInfo(Of IList(Of ConfigInfo), Status)
        Dim result = Me.Get(Of IList(Of ConfigInfo))("Account/Shares")
        Return result
    End Function

    'Public Function AllSharedMapped(userId As Integer, agentId As Guid) As ResultInfo(Of Boolean)
    '    Dim result = Me.Get(Of Boolean)(endpoint:=userId.ToString() + "/" + agentId.ToString() + "/AllSharesMapped")
    '    Return result
    'End Function

    'Public Function IsShareMapped(agentId As Guid) As ResultInfo(Of Boolean)
    '    Dim result = Me.Get(Of Boolean)(agentId.ToString() + "/SharesMapped")
    '    Return result
    'End Function

    Public Async Function GetSharesAsync() As Task(Of ResultInfo(Of List(Of ConfigInfo), Status))
        Dim result = Await Me.GetAsync(Of List(Of ConfigInfo))(endpoint:="GetShares")
        Return result
    End Function

    Public Function AddAll(lst As List(Of ConfigInfo)) As ResultInfo(Of Boolean, Status)
        Dim result = Me.Post(Of List(Of ConfigInfo), Boolean)(endpoint:="AddAll", data:=lst)
        Return result
    End Function

    Public Function GetAllShareByAccount() As ResultInfo(Of List(Of ConfigInfo), Status)
        Dim result = Me.Get(Of List(Of ConfigInfo))(endpoint:="GetAllShareByAccount")
        Return result
    End Function

    Public Function DeleteShare(shareId As Integer) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("shareId", shareId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteShare", data:=obj)
        Return result
    End Function

    Public Function DeleteShareMapping(shareId As Integer) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("shareId", shareId)
        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteShareMapping", data:=obj)
        Return result
    End Function

    Public Function ShareMappedSummary() As ResultInfo(Of ShareSummaryInfo, Status)
        Dim result = Me.Get(Of ShareSummaryInfo)("MappedSummary")
        Return result
    End Function

    Public Function IsAnyPathExists(paths As List(Of String), accountId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("paths", paths)
        obj.Add("accountId", accountId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="IsAnyPathExists", data:=obj)
        Return result
    End Function

End Class
