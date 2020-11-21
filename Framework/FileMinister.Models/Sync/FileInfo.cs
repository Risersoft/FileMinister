using FileMinister.Models.Enums;
using risersoft.shared.portable.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
namespace FileMinister.Models.Sync
{
    public class FileInfo
    {
        public Guid FileId { get; set; }
        public short ShareId { get; set; }
        public Guid ParentFileId { get; set; }
        public FileType FileType { get; set; }
        public int VersionNumber { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FullPath { get; set; }
        public DateTime CreateOnUTC { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedOnUTC { get; set; }
        public Guid DeletedByUserId { get; set; }
        public string FileNameWithExtension { get; set; }
        public bool IsLocalFile { get; set; }
        public string RelativePath { get; set; }
    }

}