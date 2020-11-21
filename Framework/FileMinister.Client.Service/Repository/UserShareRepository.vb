Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Namespace Repository
    Public Class UserShareRepository
        Inherits ClientRepositoryBase(Of UserShareInfo, Int32)
        Implements IUserShareRepository

        ''' <summary>
        ''' Get UserShare By share id and user's account id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Get](id As Int32) As ResultInfo(Of UserShareInfo, Status)
            Try
                Using service = GetClientCommonEntity()
                    Dim file = service.UserShares.FirstOrDefault(Function(p) p.ShareId = id AndAlso p.UserAccountId = Me.User.UserAccountId)
                    Dim obj = New UserShareInfo
                    If file IsNot Nothing Then
                        obj.UserAccountId = file.UserAccountId
                        obj.[ShareId] = file.[ShareId]
                        obj.[SharePath] = file.[SharePath]
                        obj.WindowsUser = file.[WindowsUser]
                        obj.Password = file.Password
                    End If
                    Return BuildResponse(obj)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of UserShareInfo)(ex)
            End Try
        End Function
    End Class
End Namespace
