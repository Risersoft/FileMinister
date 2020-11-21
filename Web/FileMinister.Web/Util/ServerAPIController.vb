Imports FileMinister.Models.Enums
Imports FileMinister.Models.Sync
Imports risersoft.shared.portable.Model
Imports risersoft.shared.web.Controllers
Public Class ServerApiController(Of T As BaseInfo, TId, TRepo As IRepositoryBase(Of T, TId, RSCallerInfo))
    Inherits ServerApiController(Of T, TId, T, Boolean, Status, TRepo)
    Public Sub New(repository As TRepo)
        MyBase.New(repository)
    End Sub
End Class

Public MustInherit Class ApiControllerRepoBase(Of TOutput As BaseInfo, TInputId, TUser, TRepo As IRepositoryBase(Of TOutput, TInputId, TUser))
    Inherits ApiControllerRepoBase(Of TOutput, TInputId, TOutput, Boolean, TUser, Status, TRepo)


    Public Sub New(repository As TRepo)
        MyBase.New(repository)
    End Sub
End Class