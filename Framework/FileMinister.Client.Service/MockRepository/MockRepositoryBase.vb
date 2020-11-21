Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Namespace MockRepository
    Public Class MockRepositoryBase(Of T As BaseInfo, TId)
        Implements IRepositoryBase(Of T, TId, WorkSpaceInfo)

        Public Property WebController As clsWebControllerBase Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).WebController
        Public Property User As WorkSpaceInfo Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).User

        Public Overridable Function GetAll() As ResultInfo(Of List(Of T), Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).GetAll
            Throw New NotImplementedException()
        End Function

        Public Overridable Function [Get](id As TId) As ResultInfo(Of T, Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).Get
            Throw New NotImplementedException()
        End Function

        Public Overridable Function Add(data As T) As ResultInfo(Of Boolean, Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).Add
            Throw New NotImplementedException()
        End Function

        Public Overridable Function Update(di As TId, data As T) As ResultInfo(Of Boolean, Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).Update
            Throw New NotImplementedException()
        End Function

        Public Overridable Function Patch(data As T) As ResultInfo(Of Boolean, Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).Patch
            Throw New NotImplementedException()
        End Function

        Public Overridable Function Delete(id As TId) As ResultInfo(Of Boolean, Status) Implements IRepositoryBase(Of T, TId, WorkSpaceInfo).Delete
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace