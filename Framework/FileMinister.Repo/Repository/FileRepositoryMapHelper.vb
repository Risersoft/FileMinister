Imports FileMinister.Models.Sync

Partial Public Class FileRepository

    Private Function MapToObject(s As FileEntry) As FileEntryInfo
        Dim t = New FileEntryInfo With {
                .FileEntryId = s.FileEntryId,
                .FileEntryTypeId = s.FileEntryTypeId,
                .CurrentVersionNumber = s.CurrentVersionNumber,
                .IsCheckedOut = s.IsCheckedOut,
                .FileShareId = s.FileShareId
        }
        Return t
    End Function

    Private Function MapFromObject(s As FileEntryInfo, Optional t As FileEntry = Nothing) As FileEntry
        Dim tt = t
        If tt Is Nothing Then
            tt = New FileEntry()
        End If

        If s.FileEntryId = Guid.Empty Then
            tt.FileEntryId = Guid.NewGuid()
        Else
            tt.FileEntryId = s.FileEntryId
        End If

        tt.FileEntryTypeId = s.FileEntryTypeId
        tt.CurrentVersionNumber = s.CurrentVersionNumber
        tt.IsCheckedOut = s.IsCheckedOut
        tt.FileShareId = s.FileShareId
        tt.CheckedOutByUserId = s.CheckedOutByUserId
        tt.CheckedOutOnUTC = s.CheckedOutOnUTC
        tt.CheckedOutWorkSpaceId = s.CheckOutWorkSpaceId
        tt.IsDeleted = s.IsDeleted
        tt.DeletedByUserId = s.DeletedByUserId
        tt.DeletedOnUTC = s.DeletedOnUTC
        Return tt
    End Function

    Private Function MapFromObject(source As GetLatestFileVersion_Result, FileShareId As Integer) As FileVersionInfo

        Dim fileVersionInfo As FileVersionInfo = New FileVersionInfo() With {
            .FileVersionId = source.FileVersionId,
            .ParentFileEntryId = source.ParentFileEntryId,
            .FileEntryId = source.FileEntryId,
            .VersionNumber = source.VersionNumber,
            .PreviousFileVersionId = source.PreviousFileVersionId,
            .FileEntrySize = source.FileEntrySize,
            .FileEntryHash = source.FileEntryHash,
            .FileEntryName = source.FileEntryName,
            .FileEntryRelativePath = source.FileEntryRelativePath,
            .FileEntryExtension = source.FileEntryExtension,
            .FileEntry = New FileEntryInfo With {.FileShareId = FileShareId},
            .ServerFileName = source.ServerFileName
            }

        Return fileVersionInfo
    End Function

    ''' <summary>
    ''' Map data from one collection to other
    ''' </summary>
    ''' <param name="source">List of GetLatestFileVersionChildrens_Result object</param>
    ''' <param name="ParentFileEntryId">ParentFileEntryId of the file</param>
    ''' <param name="fileEntryDeleteGroup">An Object that has file entry delete group infortmation</param>
    ''' <returns>Return a list of FileEntryDeleteGroupHierarchy</returns>
    Public Function MapFromCollection(source As List(Of GetLatestFileVersionChildrens_Result), ParentFileEntryId As Guid, fileEntryDeleteGroup As FileEntryDeleteGroup) As List(Of FileEntryDeleteGroupHierarchy)
        Dim deleteGroupHierarchy As List(Of FileEntryDeleteGroupHierarchy) = New List(Of FileEntryDeleteGroupHierarchy)()

        For Each obj In source
            Dim fileEntryDeleteGroupHierarchy As FileEntryDeleteGroupHierarchy = New FileEntryDeleteGroupHierarchy() With {
                    .FileEntryDeleteGroupHierarchyId = Guid.NewGuid(),
                    .FileEntryId = obj.FileEntryId,
                    .ParentFileEntryId = ParentFileEntryId,
                    .FileEntryDeleteGroupId = fileEntryDeleteGroup.FileEntryDeleteGroupId
                }
            deleteGroupHierarchy.Add(fileEntryDeleteGroupHierarchy)
        Next
        Return deleteGroupHierarchy

    End Function

    ''' <summary>
    ''' Map data from one collection to other
    ''' </summary>
    ''' <param name="source">List of GetLatestFileVersionChildrenHierarchy_Result object</param>
    ''' <param name="ParentFileEntryId">ParentFileEntryId of the file</param>
    ''' <param name="fileEntryDeleteGroup">An Object that has file entry delete group infortmation</param>
    ''' <returns>Return a list of FileEntryDeleteGroupHierarchy</returns>
    Public Function MapFromCollection(source As List(Of GetLatestFileVersionChildrenHierarchy_Result), ParentFileEntryId As Guid, fileEntryDeleteGroup As FileEntryDeleteGroup) As List(Of FileEntryDeleteGroupHierarchy)
        Dim deleteGroupHierarchy As List(Of FileEntryDeleteGroupHierarchy) = New List(Of FileEntryDeleteGroupHierarchy)()

        For Each obj In source
            Dim fileEntryDeleteGroupHierarchy As FileEntryDeleteGroupHierarchy = New FileEntryDeleteGroupHierarchy() With {
                    .FileEntryDeleteGroupHierarchyId = Guid.NewGuid(),
                    .FileEntryId = obj.FileEntryId,
                    .ParentFileEntryId = ParentFileEntryId,
                    .FileEntryDeleteGroupId = fileEntryDeleteGroup.FileEntryDeleteGroupId
                }
            deleteGroupHierarchy.Add(fileEntryDeleteGroupHierarchy)
        Next
        Return deleteGroupHierarchy

    End Function

    ''' <summary>
    ''' Create FileVersionInfo object from FileVersion 
    ''' </summary>
    ''' <param name="source">A file version object</param>
    ''' <param name="FileShareId">File Share Id</param>
    ''' <returns>A new FileVersionInfo Object</returns>
    Private Function MapFromObject(source As FileVersion, FileShareId As Integer) As FileVersionInfo

        Dim fileVersionInfo As FileVersionInfo = New FileVersionInfo() With {
            .FileVersionId = source.FileVersionId,
            .ParentFileEntryId = source.ParentFileEntryId,
            .FileEntryId = source.FileEntryId,
            .VersionNumber = source.VersionNumber,
            .PreviousFileVersionId = source.PreviousFileVersionId,
            .FileEntrySize = source.FileEntrySize,
            .FileEntryHash = source.FileEntryHash,
            .FileEntryName = source.FileEntryName,
            .FileEntryRelativePath = source.FileEntryRelativePath,
            .FileEntryExtension = source.FileEntryExtension,
            .FileEntry = New FileEntryInfo With {.FileShareId = FileShareId},
            .ServerFileName = source.ServerFileName
            }

        Return fileVersionInfo
    End Function

    Private Function MapToObject(sList As List(Of GetLatestFileVersionChildrenWithPermission_Result)) As List(Of FileEntryInfo)
        Dim list As New List(Of FileEntryInfo)
        For Each s As GetLatestFileVersionChildrenWithPermission_Result In sList
            Dim t = New FileEntryInfo With {
                           .FileEntryId = s.FileEntryId,
                           .FileEntryTypeId = s.FileEntryTypeId,
                           .CurrentVersionNumber = s.VersionNumber,
                           .CheckedOutByUserId = s.CheckedOutByUserId,
                           .CheckedOutByUserName = s.CheckedOutByUserName,
                           .CanWrite = s.CanWrite.Value,
                           .FileShareId = s.FileShareId,
                            .FileVersion = New FileVersionInfo With {
                            .FileEntryName = s.Name,
                            .FileEntryNameWithExtension = s.Name,
                            .FileEntryRelativePath = s.FileEntryRelativePath,
                            .CreatedByUserId = s.CreatedByUserId,
                            .CreatedByUserName = s.UserName,
                            .CreatedOnUTC = s.CreatedOnUTC
            }
                   }
            list.Add(t)
        Next
        '.IsCheckedOut = s.IsCheckedOut
        Return list
    End Function

End Class
