Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class TagRepository
    Inherits ClientRepositoryBase(Of TagInfo, Guid)
    Implements ITagRepository

    ''' <summary>
    ''' Get all tags
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of TagInfo), Status)
        Try
            Using service = GetClientEntity()
                Dim result = service.Tags _
                .Where(Function(p) p.IsDeleted = False).OrderBy(Function(p) p.DisplayOrder) _
                .Select(Function(s) New TagInfo With {
                    .TagId = s.TagId,
                    .FileEntryId = s.FileSystemEntryId,
                    .TagTypeId = s.TagTypeId,
                    .TagName = s.TagName,
                    .TagValue = s.TagValue,
                    .DisplayOrder = s.DisplayOrder,
                    .CreatedOnUTC = s.CreatedOnUTC,
                    .CreatedByUserId = s.CreatedByUserId,
                    .IsDeleted = s.IsDeleted
                 }).ToList()
                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of TagInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get tag by id
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Guid) As ResultInfo(Of TagInfo, Status)
        Try
            Using service = GetClientEntity()
                Dim tag = service.Tags.FirstOrDefault(Function(p) p.TagId = id)
                Dim obj = New TagInfo
                If tag IsNot Nothing Then
                    obj = MapToObject(tag)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of TagInfo)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Add a new tag
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As TagInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetClientEntity()
                Dim tag = MapFromObject(data)
                service.Tags.Add(tag)
                service.SaveChanges()
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Update existing tag
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update(id As Guid, data As TagInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetClientEntity()
                Dim tag = service.Tags.FirstOrDefault(Function(p) p.TagId = id)
                If tag IsNot Nothing Then
                    MapFromObject(data, tag)
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete a tag
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Delete(id As Guid) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetClientEntity()
                Dim tag = service.Tags.FirstOrDefault(Function(p) p.TagId = id)
                If tag IsNot Nothing Then
                    tag.IsDeleted = True
                    tag.DeletedByUserId = Me.User.UserId
                    tag.DeletedOnUTC = DateTime.UtcNow
                    service.SaveChanges()
                End If
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get all tags against a file
    ''' </summary>
    ''' <param name="fileId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [GetFileTags](fileId As Guid) As ResultInfo(Of List(Of TagInfo), Status) Implements ITagRepository.GetFileTags
        Try
            Using service = GetClientEntity()
                Dim tags = service.Tags.Where(Function(p) p.FileSystemEntryId = fileId And p.IsDeleted = False).Select(Function(s) New TagInfo With {
                    .TagId = s.TagId,
                    .FileEntryId = s.FileSystemEntryId,
                    .TagTypeId = s.TagTypeId,
                    .TagName = s.TagName,
                    .TagValue = s.TagValue,
                    .DisplayOrder = s.DisplayOrder,
                    .CreatedOnUTC = s.CreatedOnUTC,
                    .CreatedByUserId = s.CreatedByUserId,
                    .IsDeleted = s.IsDeleted
                 }).ToList()

                Return BuildResponse(tags)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of TagInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Map entity to model
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As Tag) As TagInfo
        Dim t = New TagInfo With {
              .TagId = s.TagId,
                .FileEntryId = s.FileSystemEntryId,
                .TagTypeId = s.TagTypeId,
                .TagName = s.TagName,
                .TagValue = s.TagValue,
                .DisplayOrder = s.DisplayOrder,
                .CreatedOnUTC = s.CreatedOnUTC,
                .CreatedByUserId = s.CreatedByUserId,
                .IsDeleted = s.IsDeleted
             }
        Return t
    End Function

    ''' <summary>
    ''' Map model to entity
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="t"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromObject(s As TagInfo, Optional t As Tag = Nothing) As Tag
        Dim tt = t
        If tt Is Nothing Then
            tt = New Tag()
        End If
        tt.TagId = s.TagId
        If s.FileEntryId <> Nothing Then
            tt.FileSystemEntryId = s.FileEntryId
        End If
        If s.TagTypeId <> Nothing Then
            tt.TagTypeId = s.TagTypeId
        End If
        If s.TagName <> Nothing Then
            tt.TagName = s.TagName
        End If
        If s.TagValue <> Nothing Then
            tt.TagValue = s.TagValue
        End If
        If s.DisplayOrder <> Nothing Then
            tt.DisplayOrder = s.DisplayOrder
        End If
        If s.CreatedOnUTC <> Nothing Then
            tt.CreatedOnUTC = DateTime.UtcNow
        End If
        If s.CreatedByUserId <> Nothing Then
            tt.CreatedByUserId = Me.User.UserId
        End If
        If s.IsDeleted <> Nothing Then
            tt.IsDeleted = s.IsDeleted
        End If
        Return tt
    End Function
End Class
