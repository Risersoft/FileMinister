Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class TagRepository
    Inherits ServerRepositoryBase(Of TagInfo, Guid)
    Implements ITagRepository

    ''' <summary>
    ''' Get All Tags Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of TagInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim rep = service.Tags.Select(Function(s) New TagInfo With {
                .TagId = s.TagId,
                .FileEntryId = s.FileEntryId,
                .TagTypeId = s.TagTypeId,
                .TagName = s.TagName,
                .TagValue = s.TagValue,
                .DisplayOrder = s.DisplayOrder,
                .CreatedOnUTC = s.CreatedOnUTC,
                .CreatedByUserId = s.CreatedByUserId,
                .IsDeleted = s.IsDeleted
             }).Where(Function(p) p.IsDeleted = False).OrderBy(Function(p) p.DisplayOrder).ToList()
                Return BuildResponse(rep)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of TagInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get Details
    ''' </summary>
    ''' <param name="id">TagId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Guid) As ResultInfo(Of TagInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim tag = service.Tags.FirstOrDefault(Function(p) p.TagId = id)
                Dim obj As TagInfo = Nothing
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
    ''' Add Tag
    ''' </summary>
    ''' <param name="data">Tag Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As TagInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
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
    ''' Update Tag
    ''' </summary>
    ''' <param name="id">TagId</param>
    ''' <param name="data">Taqg Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update(id As Guid, data As TagInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
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
    ''' Delete Tag
    ''' </summary>
    ''' <param name="id">TagId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Delete(id As Guid) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
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
    ''' Map Tag Entity to Tag Model
    ''' </summary>
    ''' <param name="s">Tag Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As Tag) As TagInfo
        Dim t = New TagInfo With {
              .TagId = s.TagId,
                .FileEntryId = s.FileEntryId,
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
    ''' Map Tag Details to Tag Entity
    ''' </summary>
    ''' <param name="s">Tag Details</param>
    ''' <param name="t">Tag Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromObject(s As TagInfo, Optional t As Tag = Nothing) As Tag
        Dim tt = t
        If tt Is Nothing Then
            tt = New Tag()
        End If
        tt.TagId = s.TagId
        If s.FileEntryId <> Nothing Then
            tt.FileEntryId = s.FileEntryId
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
        If s.CreatedOnUTC = DateTime.MinValue Then
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

    ''' <summary>
    ''' Get All Tags
    ''' </summary>
    ''' <param name="fileId">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByFileId(fileId As Guid) As ResultInfo(Of List(Of TagInfo), Status) Implements ITagRepository.GetByFileId
        Try
            Using service = GetServerEntity()
                Dim res = service.Tags _
            .Where(Function(p) p.FileEntryId = fileId And p.IsDeleted = False) _
                .Select(Function(s) New TagInfo With {
                .TagId = s.TagId,
                .FileEntryId = s.FileEntryId,
                .TagTypeId = s.TagTypeId,
                .TagName = s.TagName,
                .TagValue = s.TagValue,
                .DisplayOrder = s.DisplayOrder,
                .CreatedOnUTC = s.CreatedOnUTC,
                .CreatedByUserId = s.CreatedByUserId,
                .IsDeleted = s.IsDeleted
             }).OrderBy(Function(p) p.DisplayOrder).ToList()
                Return BuildResponse(res)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of TagInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Manage Tags Action such as Add/Update/Remove
    ''' </summary>
    ''' <param name="data">Tag Data</param>
    ''' <returns>ResultInfo Object with Success/Failture</returns>
    Public Function ManageTags(data As Tuple(Of Guid, List(Of TagInfo), List(Of TagInfo), List(Of TagInfo))) As ResultInfo(Of Boolean, Status) Implements ITagRepository.ManageTags
        Try
            Using service = GetServerEntity()
                Dim fileId = data.Item1
                Dim addTagData = data.Item2
                Dim editTagData = data.Item3
                Dim remData = data.Item4

                If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin OrElse (FileRepository.GetEffectivePermission(service, Me.User, fileId) And PermissionType.ShareAdmin) = PermissionType.ShareAdmin OrElse HasTagPermission(Service:=service, user:=Me.User, FileEntryId:=fileId)) Then

                    'remove all tags for the supplied fileid
                    If (remData IsNot Nothing AndAlso remData.Count > 0) Then
                        RemoveTags(fileId, remData, service)
                    End If

                    'update all tags
                    If editTagData IsNot Nothing AndAlso editTagData.Count > 0 Then
                        'get tags info
                        Dim tagIds = String.Join(",", editTagData.Select(Function(p) p.TagId).ToArray())

                        'update tags
                        Dim tagsFromDb = service.Tags.Where(Function(p) p.FileEntryId = fileId And tagIds.Contains(p.TagId.ToString())).ToList()
                        If (tagsFromDb IsNot Nothing) Then
                            tagsFromDb.ForEach(Function(p) MapFromObject(editTagData.FirstOrDefault(Function(k) k.TagId = p.TagId), p))
                        End If
                    End If

                    'add tags
                    If addTagData IsNot Nothing AndAlso addTagData.Count > 0 Then
                        ' add multiple tags in one go
                        AddTagBatch(service, fileId, addTagData)
                    End If

                    service.SaveChanges()

                    Return BuildResponse(True)

                Else
                    Return BuildResponse(False, Status.AccessDenied)
                End If

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Private Sub AddTagBatch(service As FileMinisterEntities, fileId As Guid, addTagData As List(Of TagInfo))
        Const tagTypeId As Byte = 1 'TODO we need to think alrenate for this
        Dim lastDisplayOrder As Integer = 0
        If (service.Tags.Any(Function(p) p.FileEntryId = fileId And p.IsDeleted = False)) Then
            lastDisplayOrder = service.Tags.Where(Function(p) p.FileEntryId = fileId And p.IsDeleted = False).Max(Function(d) d.DisplayOrder)
        End If

        'now add tags either added or updated
        Dim newTagList = addTagData.ConvertAll(Function(s) New Tag With
                       {
                        .TagId = Guid.NewGuid(),
                        .CreatedOnUTC = DateTime.UtcNow(),
                        .CreatedByUserId = Me.User.UserId,
                        .FileEntryId = fileId,
                        .DisplayOrder = Threading.Interlocked.Increment(lastDisplayOrder),
                        .TagName = s.TagName,
                        .TagTypeId = tagTypeId,
                        .TagValue = s.TagValue
                       }
                       )
        service.Tags.AddRange(newTagList)
    End Sub

    ''' <summary>
    ''' Remove Tag based on File
    ''' </summary>
    ''' <param name="fileid">FileEntryId</param>
    ''' <param name="service">FileMinisterEntities</param>
    Private Sub RemoveTagsByFile(fileid As Guid, service As FileMinisterEntities)
        Dim tags = service.Tags.Where(Function(p) p.FileEntryId = fileid)
        MarkTagForDeletion(tags)
    End Sub

    ''' <summary>
    ''' Remove Tags based on File
    ''' </summary>
    ''' <param name="fileid">FileEntryId</param>
    ''' <param name="removedTags">List of tags to be removed</param>
    ''' <param name="service">FileMinisterEntities</param>
    Private Sub RemoveTags(fileid As Guid, removedTags As List(Of TagInfo), service As FileMinisterEntities)
        Dim tagIds = String.Join(",", removedTags.Select(Function(p) p.TagId).ToArray())
        Dim tags = service.Tags.Where(Function(p) p.FileEntryId = fileid And tagIds.Contains(p.TagId.ToString()))
        MarkTagForDeletion(tags)
    End Sub

    ''' <summary>
    ''' Mark Tag for Deletetion
    ''' </summary>
    ''' <param name="tags"></param>
    Private Sub MarkTagForDeletion(tags As IQueryable(Of Tag))
        If tags IsNot Nothing Then
            For Each objTag In tags

                objTag.DeletedByUserId = Me.User.UserId
                objTag.DeletedOnUTC = DateTime.UtcNow()
                objTag.IsDeleted = True

            Next
        End If
    End Sub

    ''' <summary>
    ''' Check Tag Permission for the user
    ''' </summary>
    ''' <param name="Service"></param>
    ''' <param name="user"></param>
    ''' <param name="FileEntryId"></param>
    ''' <returns></returns>
    Private Function HasTagPermission(Service As FileMinisterEntities, user As RSCallerInfo, FileEntryId As Guid) As Boolean
        Dim permission = FileRepository.GetEffectivePermission(Service, user, FileEntryId)

        'If ((PermissionType.Tag And permission) = PermissionType.Tag) Then
        '    Return True
        'Else
        '    Return False
        'End If

        Return ((PermissionType.Tag And permission) = PermissionType.Tag)

    End Function

End Class
