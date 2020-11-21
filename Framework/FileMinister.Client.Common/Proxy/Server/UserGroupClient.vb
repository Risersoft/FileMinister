Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class UserGroupClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/usergroup")
    End Sub

    Public Function [DeleteByGroup](groupId As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("groupId", groupId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteByGroup", data:=obj)
        Return result
    End Function

    Public Function [DeleteByUser](userId As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("userId", userId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteByUser", data:=obj)
        Return result
    End Function

    Public Function AddAll(lst As List(Of UserGroupAssignmentsInfo)) As ResultInfo(Of Boolean, Status)
        Dim result = Me.Post(Of List(Of UserGroupAssignmentsInfo), Boolean)(endpoint:="AddAll", data:=lst)
        Return result
    End Function

End Class
