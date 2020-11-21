using FileMinister.Models.Enums;
using risersoft.shared.portable.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
namespace FileMinister.Models.Sync
{
    public class FileVersionInfo : BaseInfo
    {
        public System.Guid FileVersionId { get; set; }
        //Public Property Version As Integer
        //Public Property ModifiedOn As Date
        //Public Property UserId As Integer
        //Public Property Path As String
        //Public Property User As String
        //Public Property showDelink As Integer
        //Public Property PrevVersionNumber As Integer?
        public System.Guid ParentFileEntryId { get; set; }
        public System.Guid FileEntryId { get; set; }
        public int? PreviousVersionNumber { get; set; }
        public Nullable<int> VersionNumber { get; set; }
        public Nullable<System.Guid> PreviousFileVersionId { get; set; }
        public long FileEntrySize { get; set; }
        public string FileEntryName { get; set; }
        public string FileEntryExtension { get; set; }
        public string FileEntryRelativePath { get; set; }
        public string FileEntryOldRelativePath { get; set; }
        public string FileEntryHash { get; set; }
        public System.Guid ServerFileName { get; set; }
        public System.DateTime CreatedOnUTC { get; set; }
        public Nullable<Guid> CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOnUTC { get; set; }
        public Nullable<Guid> DeletedByUserId { get; set; }
        public string FileEntryNameWithExtension { get; set; }
        public string FileEntryStatusDisplayName { get; set; }
        public FileEntryInfo FileEntry { get; set; }
        public DeltaOperationInfo DeltaOperation { get; set; }
        public string User { get; set; }
        public int showDelink { get; set; }
        public string CreatedByUserName { get; set; }
        public int FileShareId { get; set; }
        public CheckInSource CheckInSource { get; set; }
    }
}

