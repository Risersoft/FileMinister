

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace FileMinister.Models.Sync
{
	public class FileSearchResult
	{
		public byte FileEntryTypeId { get; set; }
		public System.Guid FileVersionId { get; set; }
		public Nullable<System.Guid> ParentFileEntryId { get; set; }
		public System.Guid FileEntryId { get; set; }
		public Nullable<int> VersionNumber { get; set; }
		public Nullable<System.Guid> PreviousFileVersionId { get; set; }
		public long FileEntrySize { get; set; }
		public string FileEntryName { get; set; }
		public string FileEntryExtension { get; set; }
		public string FileEntryRelativePath { get; set; }
		public string FileEntryOldRelativePath { get; set; }
		public string FileEntryHash { get; set; }
		public Nullable<System.Guid> ServerFileName { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public Nullable<Guid> CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<int> DeletedByUserId { get; set; }
		public string FileEntryNameWithExtension { get; set; }
		public string FileEntryStatusDisplayName { get; set; }
		public bool IsConflicted { get; set; }
		public Nullable<Guid> CheckedOutByUserId { get; set; }
		public Nullable<byte> FileEntryStatusId { get; set; }
		public string User { get; set; }
		public int showDelink { get; set; }
		public int? PrevVersionNumber { get; set; }
	}
}

