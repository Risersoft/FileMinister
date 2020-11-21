Imports System.Web.Http
Imports risersoft.shared.portable.Enums
Imports FileMinister.Models.Sync

Public Class PermissionController
    Inherits ServerApiController(Of PermissionInfo, Integer, IPermissionRepository)


    Public Sub New(repository As IPermissionRepository)
        MyBase.New(repository)
    End Sub


End Class
