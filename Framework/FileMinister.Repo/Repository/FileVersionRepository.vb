Imports risersoft.shared.portable.Model
Imports System.Data.SqlClient
Imports risersoft.shared.portable.Enums
Imports Model = FileMinister.Models.Sync

Public Class FileVersionRepository
    Inherits ServerRepositoryBase(Of Model.FileVersionInfo, Guid)
    Implements IFileVersionRepository

    ''' <summary>
    ''' Get File Version Details
    ''' </summary>
    ''' <param name="id">File version Id</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Guid) As ResultInfo(Of Model.FileVersionInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim file = service.FileVersions.FirstOrDefault(Function(p) p.FileVersionId = id)
                Dim obj = New Model.FileVersionInfo
                If file IsNot Nothing Then
                    obj = MapToObject(file)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Model.FileVersionInfo)(ex)
        End Try
    End Function


    ''' <summary>
    ''' Add File Version
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As Model.FileVersionInfo) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetServerEntity()
                Dim file = MapFromObject(data)
                service.FileVersions.Add(file)
                service.SaveChanges()
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Map File Version Details to File Version Entity
    ''' </summary>
    ''' <param name="s">File Version Model</param>
    ''' <param name="t">File Version Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MapFromObject(s As Model.FileVersionInfo, Optional t As FileVersion = Nothing) As FileVersion
        Dim tt = t
        If tt Is Nothing Then
            tt = New FileVersion()
        End If
        If (s.FileVersionId = Guid.Empty) Then
            s.FileVersionId = Guid.NewGuid()
        End If
        tt.[FileVersionId] = s.FileVersionId

        tt.CheckInSourceId = CInt(s.CheckInSource)
        tt.[ParentFileEntryId] = s.ParentFileEntryId
        tt.[FileEntryId] = s.[FileEntryId]
        tt.[VersionNumber] = s.[VersionNumber]
        tt.[PreviousFileVersionId] = s.PreviousFileVersionId
        tt.[FileEntrySize] = s.FileEntrySize
        tt.[FileEntryName] = s.FileEntryName
        tt.[FileEntryExtension] = s.FileEntryExtension
        tt.FileEntryRelativePath = s.FileEntryRelativePath
        tt.FileEntryHash = s.FileEntryHash

        If (s.ServerFileName = Guid.Empty) Then
            s.ServerFileName = Guid.NewGuid()
        End If
        tt.[ServerFileName] = s.ServerFileName

        tt.IsDeleted = s.IsDeleted
        tt.DeletedByUserId = s.DeletedByUserId
        tt.DeletedOnUTC = s.DeletedOnUTC
        tt.CreatedByUserId = Me.User.UserId
        If (s.CreatedOnUTC = DateTime.MinValue) Then
            tt.CreatedOnUTC = DateTime.UtcNow
        Else
            tt.CreatedOnUTC = s.CreatedOnUTC
        End If

        Return tt
    End Function

    ''' <summary>
    ''' Map File Version Entity to File Version Model
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As FileVersion) As Model.FileVersionInfo
        Dim t = New Model.FileVersionInfo With {
                .FileVersionId = s.FileVersionId,
                .ParentFileEntryId = s.ParentFileEntryId,
                .FileEntryId = s.FileEntryId,
                .VersionNumber = s.VersionNumber,
                .PreviousFileVersionId = s.PreviousFileVersionId,
                .FileEntrySize = s.FileEntrySize,
                .FileEntryName = s.FileEntryName,
                .FileEntryExtension = s.FileEntryExtension
             }
        Return t
    End Function

    ''' <summary>
    ''' Delete File Version
    ''' </summary>
    ''' <param name="FileVersionId">File Version Id</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [DeleteFileVersion](FileVersionId As Guid) As ResultInfo(Of Boolean, Status) Implements IFileVersionRepository.DeleteFileVersion
        Try

            Using service = GetServerEntity()
                Dim obj = New Model.FileVersionInfo
                Dim fileVersion = service.FileVersions.FirstOrDefault(Function(p) p.FileVersionId = FileVersionId)
                If fileVersion IsNot Nothing Then
                    Dim deleteBlobResult As ResultInfo(Of Boolean, Status) = Nothing
                    If (service.FileVersions.Where(Function(p) p.ServerFileName = fileVersion.ServerFileName AndAlso Not p.IsDeleted AndAlso p.PreviousFileVersionId <> FileVersionId).ToList().Count = 0) Then
                        'DELETE FILE FROM AZURE SERVER 
                        Dim rep As SyncRepository = New SyncRepository()
                        rep.fncUser = Me.fncUser
                        'Dim syncController As SyncController = New SyncController(rep)
                        Dim lstServerFileName As New List(Of Guid)
                        lstServerFileName.Add(fileVersion.ServerFileName)
                        'TODO: To be moved to 
                        'deleteBlobResult = syncController.DeleteBlobs(fileVersion.FileEntry.FileShareId, lstServerFileName)
                    End If

                    If (deleteBlobResult Is Nothing OrElse (deleteBlobResult IsNot Nothing AndAlso deleteBlobResult.Data)) Then
                        fileVersion.DeletedByUserId = Me.User.UserId
                        fileVersion.DeletedOnUTC = Date.UtcNow()
                        fileVersion.IsDeleted = True

                        service.SaveChanges()
                        Return BuildResponse(True)
                    End If
                End If
                Return BuildResponse(False)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get All File Version
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of Model.FileVersionInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim data = service.FileVersions.Select(Function(s) New Model.FileVersionInfo With {
                    .FileEntryId = s.FileEntryId,
                    .FileVersionId = s.FileVersionId,
                    .ParentFileEntryId = s.ParentFileEntryId,
                    .FileEntryName = s.FileEntryName
                 }).ToList()

                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of Model.FileVersionInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get Checkin File Details
    ''' </summary>
    ''' <param name="userId">UserId</param>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <param name="fromDate">FromDate</param>
    ''' <param name="toDate">ToDate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetChekinsFileDetails(userId As Guid, FileShareId As Integer, fromdate As DateTime, toDate As DateTime) As ResultInfo(Of List(Of CheckinReportInfo), Status) Implements IFileVersionRepository.GetChekinsFileDetails
        Try
            Using service = GetServerEntity()

                Dim ParamUserId = New SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
                ParamUserId.Value = userId

                Dim ParamShareId = New SqlParameter("@FileShareId", SqlDbType.Int)
                ParamShareId.Value = FileShareId

                Dim ParamFromdate = New SqlParameter("@Fromdate", SqlDbType.DateTime)
                ParamFromdate.Value = fromdate

                Dim ParamTodate = New SqlParameter("@Todate", SqlDbType.DateTime)
                ParamTodate.Value = toDate

                Dim result = service.Database.SqlQuery(Of Model.CheckinReportInfo)("exec dbo.usp_SEL_GetCheckinFiles @UserId,@FileShareId,@Fromdate,@Todate", ParamUserId, ParamShareId, ParamFromdate, ParamTodate)

                Dim data = result.ToList()
                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of CheckinReportInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get Checkout File Details
    ''' </summary>
    ''' <param name="userId">UserId</param>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckoutFileDetails(userId As Guid, FileShareId As Integer) As ResultInfo(Of List(Of CheckinReportInfo), Status) Implements IFileVersionRepository.GetCheckoutFileDetails
        Try
            Using service = GetServerEntity()

                Dim ParamUserId = New SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
                ParamUserId.Value = userId

                Dim ParamShareId = New SqlParameter("@FileShareId", SqlDbType.Int)
                ParamShareId.Value = FileShareId

                Dim result = service.Database.SqlQuery(Of Model.CheckinReportInfo)("exec dbo.usp_SEL_GetCheckedoutFiles @UserId,@FileShareId", ParamUserId, ParamShareId)

                Dim data = result.ToList()
                data = data.GroupBy(Function(p) p.FilePath).Select(Function(qrp) qrp.First).ToList()

                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of CheckinReportInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get File Version History
    ''' </summary>
    ''' <param name="id">FileId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [GetVersionHistory](id As Guid) As ResultInfo(Of List(Of CheckinReportInfo), Status) Implements IFileVersionRepository.GetVersionHistory
        Try
            Using service = GetServerEntity()
                Dim result = From FSV In service.FileVersions
                             Group Join FSL In service.FileEntryLinks.Where(Function(p) Not p.IsDeleted AndAlso p.FileEntryId = id)
                             On FSV.FileEntryId Equals FSL.FileEntryId
                             Into R1 = Group
                             From S In R1.DefaultIfEmpty
                             Group Join FE In service.FileEntries
                             On S.PreviousFileEntryId Equals FE.FileEntryId
                             Into R2 = Group
                             From P In R2.DefaultIfEmpty
                             Group Join PFV In service.FileVersions
                             On P.FileEntryId Equals PFV.FileEntryId And P.CurrentVersionNumber Equals PFV.VersionNumber
                             Into R3 = Group
                             From Q In R3.DefaultIfEmpty
                             Group Join U In service.Users
                                 On U.UserId Equals FSV.CreatedByUserId
                             Into R4 = Group
                             From T In R4.DefaultIfEmpty
                             Where FSV.FileEntryId = id
                             Order By FSV.VersionNumber Descending
                             Select New With {
                                 .FSV = FSV,
                                 .FSL = R1.FirstOrDefault,
                                 .R3 = R3.FirstOrDefault,
                                 .R4 = R4.FirstOrDefault
                                 }

                Dim result1 = New List(Of CheckinReportInfo)

                For Each r In result
                    result1.Add(New CheckinReportInfo With {
                        .FilePath = r.FSV.FileEntryRelativePath,
                        .Time = r.FSV.CreatedOnUTC.ToString(),
                        .User = If(r.R4 Is Nothing, String.Empty, r.R4.UserName),
                        .Version = r.FSV.VersionNumber.ToString(),
                        .FileEntryId = r.FSV.FileEntryId,
                        .FileEntryName = r.FSV.FileEntryName,
                        .FileEntryNameWithExtension = r.FSV.FileEntryNameWithExtension,
                        .FileEntryLinkInfo = New FileEntryLinkInfo With {
                                    .PreviousFileEntryId = If(r.FSL Is Nothing, Nothing, If(r.FSL.PreviousFileEntryId = Guid.Empty, Nothing, r.FSL.PreviousFileEntryId)),
                                    .LatestFileVersionFileEntryPathRelativeToShare = If(r.R3 Is Nothing, String.Empty, r.R3.FileEntryRelativePath)
                                    }
                                })
                Next

                Return BuildResponse(result1.ToList())
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of CheckinReportInfo))(ex)
        End Try
    End Function


End Class
