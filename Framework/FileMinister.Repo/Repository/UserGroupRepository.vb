Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class UserGroupRepository
    Inherits ServerRepositoryBase(Of UserGroupAssignmentsInfo, Guid)
    Implements IUserGroupRepository

    ''' <summary>
    ''' Delete Group by UserId
    ''' </summary>
    ''' <param name="userId">UserId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [DeleteByUser](userId As Guid) As ResultInfo(Of Boolean, Status) Implements IUserGroupRepository.DeleteByUser
        Try
            Using service = GetServerEntity()
                Dim uGAList = service.UserGroupAssignments.Where(Function(p) p.UserId = userId)
                If uGAList IsNot Nothing Then
                    For Each uGA In uGAList
                        service.UserGroupAssignments.Remove(uGA)
                    Next
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete Group by GroupId
    ''' </summary>
    ''' <param name="groupId">GroupId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [DeleteByGroup](groupId As Guid) As ResultInfo(Of Boolean, Status) Implements IUserGroupRepository.DeleteByGroup
        Try
            Using service = GetServerEntity()

                Dim uGAList = service.UserGroupAssignments.Where(Function(p) p.GroupId = groupId)
                If uGAList IsNot Nothing Then
                    For Each uGA In uGAList
                        service.UserGroupAssignments.Remove(uGA)
                    Next
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Add All Groups
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddAll(data As List(Of UserGroupAssignmentsInfo)) As ResultInfo(Of Boolean, Status) Implements IUserGroupRepository.AddAll
        Try
            Using service = GetServerEntity()
                For Each item In data
                    Dim uGA = New UserGroupAssignment()
                    uGA.UserGroupAssignmentId = Guid.NewGuid()
                    uGA.UserId = item.UserId
                    uGA.GroupId = item.GroupId
                    uGA.CreatedByUserId = Me.User.UserId
                    uGA.CreatedOnUTC = Date.UtcNow()
                    service.UserGroupAssignments.Add(uGA)
                Next
                service.SaveChanges()
            End Using
            Return BuildResponse(True)
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function GetAllGroups(searchText As String) As ResultInfo(Of List(Of Group), Status) Implements IUserGroupRepository.GetAllGroups
        Try
            Dim uGAList As List(Of Group)
            Using service = GetServerEntity()
                If (Not String.IsNullOrWhiteSpace(searchText)) Then
                    uGAList = service.Groups.Where(Function(p) p.GroupName.Contains(searchText)).ToList
                Else
                    uGAList = service.Groups.ToList()
                End If
                Return BuildResponse(uGAList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of Group))(ex)
        End Try
    End Function
End Class
