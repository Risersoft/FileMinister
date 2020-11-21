Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class UserFilePermissionClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/userfilepermission")
    End Sub

    Public Function [GetAllUsersAndGroups](fileId As Guid) As ResultInfo(Of List(Of UserGroupInfo), Status)
        ' Dim obj = New Dictionary(Of String, Object)
        ' obj.Add("fileId", fileId)
        Dim result = Me.Get(Of List(Of UserGroupInfo))(endpoint:=fileId.ToString() + "/UsersGroups")
        Return result
    End Function

    Public Function [GetPermissionByFileAndUser](fileId As Guid, userId As Guid) As ResultInfo(Of UserFilePermissionInfo, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("userId", userId)
        Dim result = Me.Get(Of UserFilePermissionInfo)(endpoint:="UPermission", queryString:=obj)
        Return result
    End Function

    Public Function [UpdatePermissionByFileAndUser](fileId As Guid, userId As Guid, permissionAllowed As Int32, permissionDenied As Int32) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("userId", userId)
        obj.Add("permissionAllowed", permissionAllowed)
        obj.Add("permissionDenied", permissionDenied)


        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="UpdateUserFilePermission", data:=obj)
        Return result
    End Function

    Public Function [DeletePermissionByFileAndUser](fileId As Guid, userId As Guid) As ResultInfo(Of Boolean, Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("fileId", fileId)
        obj.Add("userId", userId)

        Dim result = Me.Post(Of Dictionary(Of String, Object), Boolean)(endpoint:="DeleteUserFilePermission", data:=obj)
        Return result
    End Function

    Public Function [UpdateFilePermission](fileId As Guid, removedUserAndGroupList As List(Of UserGroupInfo), updatedUserAndGroupPermissions As List(Of UserFilePermissionInfo)) As ResultInfo(Of Boolean, Status)

        Dim data As New Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo))(fileId, removedUserAndGroupList, updatedUserAndGroupPermissions)

        'Dim obj = New Dictionary(Of String, Object)
        'obj.Add("fileId", fileId)
        'obj.Add("removedUserAndGroupList", removedUserAndGroupList)
        'obj.Add("updatedUserAndGroupPermissions", updatedUserAndGroupPermissions)

        Dim result = Me.Post(Of Tuple(Of Guid, List(Of UserGroupInfo), List(Of UserFilePermissionInfo)), Boolean)(endpoint:="UpdateFilePermission", data:=data)
        Return result
    End Function
End Class
