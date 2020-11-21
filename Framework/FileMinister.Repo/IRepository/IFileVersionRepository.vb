Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Imports Model = FileMinister.Models.Sync

Public Interface IFileVersionRepository
    Inherits IRepositoryBase(Of Model.FileVersionInfo, Guid, RSCallerInfo)
    Function [DeleteFileVersion](FileVersionId As Guid) As ResultInfo(Of Boolean, Status)
    Function [GetCheckoutFileDetails](userId As Guid, FileShareId As Integer) As ResultInfo(Of List(Of CheckinReportInfo), Status)
    Function [GetChekinsFileDetails](userId As Guid, FileShareId As Integer, fromdate As DateTime, toDate As DateTime) As ResultInfo(Of List(Of CheckinReportInfo), Status)
    Function [GetVersionHistory](id As Guid) As ResultInfo(Of List(Of CheckinReportInfo), Status)



End Interface
