
using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using risersoft.shared.portable.Model;

namespace FileMinister.Models.Sync
{
	public class FileEntryInfo : BaseInfo
	{

		public System.Guid FileEntryId { get; set; }
		public byte FileEntryTypeId { get; set; }
		public short FileShareId { get; set; }
		public int? CurrentVersionNumber { get; set; }
		public bool IsCheckedOut { get; set; }
		public Nullable<Guid> CheckedOutByUserId { get; set; }
		public string CheckedOutByUserName { get; set; }
		public Nullable<System.DateTime> CheckedOutOnUTC { get; set; }
		public Nullable<Guid> CheckOutWorkSpaceId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<Guid> DeletedByUserId { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public string DeletedByUserName { get; set; }
		public bool IsPermanentlyDeleted { get; set; }
		public Nullable<System.DateTime> PermanentlyDeletedOnUTC { get; set; }
		public Nullable<Guid> PermanentlyDeletedByUserId { get; set; }
		public FileVersionInfo FileVersion { get; set; }
		public bool IsLocalFile { get; set; }
		public bool IsPhysicalFile { get; set; }
		public bool IsOtherConflicts { get; set; }
		public bool CanWrite { get; set; }
	}

	public class FileEntryLinkInfo : BaseInfo
	{

		public System.Guid FileEntryLinkId { get; set; }
		public System.Guid PreviousFileEntryId { get; set; }
		public System.Guid FileEntryId { get; set; }
		public string LatestFileVersionFileEntryPathRelativeToShare { get; set; }

	}

}

