Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class GroupFilePermissionClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/groupfilepermission")
    End Sub


    Public Function [GetPermissionByFileAndGroup](fileId As Guid, groupId As Guid) As ResultInfo(Of UserFilePermissionInfo, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("groupId", groupId)
        Dim result = Me.Get(Of UserFilePermissionInfo)(endpoint:="GPermission", queryString:=obj)
        Return result
    End Function


    Public Function [UpdatePermissionByFileAndGroup](fileId As Guid, groupId As Int32, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("groupId", groupId)
        obj.Add("permissionAllowed", permissionAllowed)
        obj.Add("permissionDenied", permissionDenied)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UpdateGroupFilePermission", data:=obj)
        Return result

        'Dim result = Me.Get(Of String)(endpoint:="UpdateGroupFilePermission", queryString:=obj)
        'Return result
    End Function

    Public Function [DeletePermissionByFileAndGroup](fileId As Guid, groupId As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("groupId", groupId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteGroupFilePermission", data:=obj)
        Return result

        'Dim result = Me.Get(Of String)(endpoint:="DeleteGroupFilePermission", queryString:=obj)
        'Return result
    End Function


End Class
