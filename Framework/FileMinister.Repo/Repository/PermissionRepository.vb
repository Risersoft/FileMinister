Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

''' <summary>
''' Permission Repository
''' </summary>
''' <remarks></remarks>
Public Class PermissionRepository
    Inherits ServerRepositoryBase(Of PermissionInfo, Integer)
    Implements IPermissionRepository

    ''' <summary>
    ''' Get All Permission Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of PermissionInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim res = service.Permissions.Select(Function(s) New PermissionInfo With {
                .PermissionId = s.PermissionId,
                .PermissionName = s.PermissionName
             }).OrderBy(Function(p) p.PermissionId).ToList()
                Return BuildResponse(res)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of PermissionInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get Permission Details
    ''' </summary>
    ''' <param name="id">Permission Id</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Integer) As ResultInfo(Of PermissionInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim permission = service.Permissions.FirstOrDefault(Function(p) p.PermissionId = id)
                Dim obj = New PermissionInfo
                If permission IsNot Nothing Then
                    obj = MapToObject(permission)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of PermissionInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Map Permission Entity to Permission Model
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As Permission) As PermissionInfo
        Dim t = New PermissionInfo With {
                 .PermissionId = s.PermissionId,
                .PermissionName = s.PermissionName
             }
        Return t
    End Function


End Class
