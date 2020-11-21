Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class UserRepository
    Inherits ServerRepositoryBase(Of RSCallerInfo, Integer)
    Implements IUserRepository

    Public Function GetAllUsers(searchText As String) As ResultInfo(Of List(Of User), Status) Implements IUserRepository.GetAllUsers
        Try
            Dim userList As List(Of User)
            Using service = GetServerEntity()
                If (Not String.IsNullOrWhiteSpace(searchText)) Then
                    userList = service.Users.Where(Function(p) p.UserName.Contains(searchText) And p.UserId <> Me.User.UserAccount.UserId).ToList()
                Else
                    userList = service.Users.Where(Function(u) u.UserId <> Me.User.UserAccount.UserId).ToList()
                End If
                Return BuildResponse(userList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of User))(ex)
        End Try
    End Function

    Public Function GetUserById(userId As Guid) As ResultInfo(Of User, Status) Implements IUserRepository.GetUserById
        Dim user As User = Nothing

        If userId <> Guid.Empty Then
            Try
                Using service = GetServerEntity()
                    user = service.Users.FirstOrDefault(Function(p) p.UserId = userId)
                End Using

                If user.UserId = userId Then
                    Return BuildResponse(user, Status.Success)
                Else
                    Return BuildResponse(user, Status.NotFound)
                End If
            Catch Ex As Exception
                Return BuildExceptionResponse(Of User)(ex)
            End Try
        End If

        Return BuildResponse(user, Status.Error)
    End Function

    Public Function GetUserBySID(sId As String, domainId As Integer) As ResultInfo(Of UserInfo, Status) Implements IUserRepository.GetUserBySID
        Dim user As UserInfo = Nothing

        If Not String.IsNullOrWhiteSpace(sId) Then

            Try
                Using service = GetServerEntity()
                    Dim obj = (From um In service.UserMappings
                               Join u In service.Users
                                On um.UserID Equals u.UserId
                               Where um.SID = sId AndAlso um.UserDomainID = domainId AndAlso um.IsDeleted = False AndAlso u.IsDeleted = False
                               Select u).FirstOrDefault()

                    If obj IsNot Nothing Then
                        user = New UserInfo With {
                            .CreatedByUserId = obj.CreatedByUserId,
                            .CreatedOnUTC = obj.CreatedOnUTC,
                            .DefaultAppCode = obj.DefaultAppCode,
                            .DeletedByUserId = obj.DeletedByUserId,
                            .DeletedOnUTC = obj.DeletedOnUTC,
                            .FullName = obj.FullName,
                            .IsDeleted = obj.IsDeleted,
                            .LastLogin = obj.LastLogin,
                            .RoleId = obj.RoleId,
                            .TenantID = obj.TenantID,
                            .UserId = obj.UserId,
                            .UserName = obj.UserName
                        }

                        Return BuildResponse(user, Status.Success)
                    Else
                        Return BuildResponse(user, Status.NotFound)
                    End If
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of UserInfo)(ex)
            End Try

        End If
        Return BuildResponse(user, Status.Error)
    End Function

    Public Function GetDomainUsers(domainId As Integer) As ResultInfo(Of List(Of UserInfo), Status) Implements IUserRepository.GetDomainUsers
        Dim users As List(Of UserInfo) = Nothing

        Try
            Using service = GetServerEntity()
                Dim query = (From um In service.UserMappings
                             Join u In service.Users
                            On um.UserID Equals u.UserId
                             Where um.UserDomainID = domainId AndAlso um.IsDeleted = False AndAlso u.IsDeleted = False AndAlso um.SID IsNot Nothing AndAlso um.SID.Trim() <> String.Empty
                             Select u, um.SID)

                If query IsNot Nothing AndAlso query.Count > 0 Then
                    users = New List(Of UserInfo)()

                    For Each obj In query
                        users.Add(New UserInfo With {
                        .CreatedByUserId = obj.u.CreatedByUserId,
                        .CreatedOnUTC = obj.u.CreatedOnUTC,
                        .DefaultAppCode = obj.u.DefaultAppCode,
                        .DeletedByUserId = obj.u.DeletedByUserId,
                        .DeletedOnUTC = obj.u.DeletedOnUTC,
                        .FullName = obj.u.FullName,
                        .IsDeleted = obj.u.IsDeleted,
                        .LastLogin = obj.u.LastLogin,
                        .RoleId = obj.u.RoleId,
                        .TenantID = obj.u.TenantID,
                        .UserId = obj.u.UserId,
                        .UserName = obj.u.UserName,
                        .SID = obj.SID
                        })
                    Next

                    Return BuildResponse(users, Status.Success)
                Else
                    Return BuildResponse(users, Status.NotFound)
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of UserInfo))(ex)
        End Try
        Return BuildResponse(users, Status.Error)
    End Function

    Public Function GetAccountAdmins() As ResultInfo(Of List(Of UserInfo), Status) Implements IUserRepository.GetAccountAdmins
        Dim users As List(Of UserInfo) = Nothing

        Try
            Using service = GetServerEntity()
                Dim query = (From um In service.UserMappings
                             Join u In service.Users
                            On um.UserID Equals u.UserId
                             Where u.RoleId = Role.AccountAdmin AndAlso um.IsDeleted = False AndAlso u.IsDeleted = False AndAlso Not String.IsNullOrWhiteSpace(um.SID)
                             Select u, um.SID)

                If query IsNot Nothing AndAlso query.Count > 0 Then
                    users = New List(Of UserInfo)()

                    For Each obj In query
                        users.Add(New UserInfo With {
                        .CreatedByUserId = obj.u.CreatedByUserId,
                        .CreatedOnUTC = obj.u.CreatedOnUTC,
                        .DefaultAppCode = obj.u.DefaultAppCode,
                        .DeletedByUserId = obj.u.DeletedByUserId,
                        .DeletedOnUTC = obj.u.DeletedOnUTC,
                        .FullName = obj.u.FullName,
                        .IsDeleted = obj.u.IsDeleted,
                        .LastLogin = obj.u.LastLogin,
                        .RoleId = obj.u.RoleId,
                        .TenantID = obj.u.TenantID,
                        .UserId = obj.u.UserId,
                        .UserName = obj.u.UserName,
                        .SID = obj.SID
                        })
                    Next

                    Return BuildResponse(users, Status.Success)
                Else
                    Return BuildResponse(users, Status.NotFound)
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of UserInfo))(ex)
        End Try
        Return BuildResponse(users, Status.Error)

    End Function
End Class
