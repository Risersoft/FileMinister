Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model

Public Class ShareRepository
    Inherits ServerRepositoryBase(Of RSCallerInfo, Integer)
    Implements IShareRepository

    Public Function GetAllShares() As ResultInfo(Of List(Of FileShare), Status) Implements IShareRepository.GetAllShares
        Try
            Dim shareList As List(Of FileShare)
            Using service = GetServerEntity()
                shareList = service.FileShares.Where(Function(p) Not p.IsDeleted).ToList()
                Return BuildResponse(shareList)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileShare))(ex)
        End Try
    End Function

    Public Function GetShareByName(shareName As String) As ResultInfo(Of ShareFileInfo, Status) Implements IShareRepository.GetShareByName
        Dim share As ShareFileInfo = Nothing
        Dim fileShareType = CByte(FileType.Share)
        If Not String.IsNullOrWhiteSpace(shareName) Then
            Try
                Using service = GetServerEntity()
                    Dim s = (From fs In service.FileShares
                             Join fe In service.FileEntries On fe.FileShareId Equals fs.FileShareId
                             Where fs.ShareName = shareName AndAlso fs.IsDeleted = False AndAlso fe.FileEntryTypeId = fileShareType AndAlso fe.IsDeleted = False
                             Select fe.FileEntryId, fs).FirstOrDefault()

                    If (Not s Is Nothing) Then
                        share = New ShareFileInfo With {.BackupRootPathFull = s.fs.BackupRootPathFull,
                            .BackupRootPathIncr = s.fs.BackupRootPathIncr,
                            .BackupShareID = s.fs.BackupShareID,
                            .CreatedByUserId = s.fs.CreatedByUserId,
                            .CreatedOnUTC = s.fs.CreatedOnUTC,
                            .DeletedByUserId = s.fs.DeletedByUserId,
                            .DeletedOnUTC = s.fs.DeletedOnUTC,
                            .Description = s.fs.Description,
                            .DiskSpaceMB = s.fs.DiskSpaceMB,
                            .Emailcc = s.fs.Emailcc,
                            .EmailTo = s.fs.EmailTo,
                            .FileEntryId = s.FileEntryId,
                            .FileShareId = s.fs.FileShareId,
                            .FullBackupPeriodType = s.fs.FullBackupPeriodType,
                            .FullBackupPeriodValue = s.fs.FullBackupPeriodValue,
                            .IncrBackupPeriodType = s.fs.IncrBackupPeriodType,
                            .IncrBackupPeriodValue = s.fs.IncrBackupPeriodValue,
                            .IsBackup = s.fs.IsBackup,
                            .IsDeleted = s.fs.IsDeleted,
                            .LastFullBackupTime = s.fs.LastFullBackupTime,
                            .LastIncrBackupTime = s.fs.LastIncrBackupTime,
                            .MaintainVersionHistory = s.fs.MaintainVersionHistory,
                            .MaxVersionFileSizeMB = s.fs.MaxVersionFileSizeMB,
                            .MaxVersionMonths = s.fs.MaxVersionMonths,
                            .MultiUserVersionDiffMinutes = s.fs.MultiUserVersionDiffMinutes,
                            .ShareContainerName = s.fs.ShareContainerName,
                            .ShareName = s.fs.ShareName,
                            .ShareType = s.fs.ShareType,
                            .SingleUserVersionDiffMinutes = s.fs.SingleUserVersionDiffMinutes,
                            .TenantID = s.fs.TenantID,
                            .UseSubDir = s.fs.UseSubDir,
                            .VersionDiffMinutes = s.fs.VersionDiffMinutes
                            }
                    Else
                        Return BuildResponse(share, Status.NotFound)
                    End If
                End Using

                Return BuildResponse(share, Status.Success)
            Catch ex As Exception
                Return BuildExceptionResponse(Of ShareFileInfo)(ex)
            End Try
        End If

        Return BuildResponse(share, Status.Error)
    End Function

End Class
