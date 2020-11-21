
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using risersoft.shared.portable.Enums;
using risersoft.shared.portable.Model;
using FileMinister.Models.Enums;

namespace FileMinister.Models.Sync
{
	public class DeltaOperationInfo : BaseInfo
	{

		public System.Guid DeltaOperationId { get; set; }
		public System.Guid FileEntryId { get; set; }
		public System.Guid FileVersionId { get; set; }
		public Nullable<byte> FileEntryStatusId { get; set; }
		public string LocalFileEntryName { get; set; }
		public string LocalFileEntryExtension { get; set; }
		public Nullable<System.DateTime> LocalCreatedOnUTC { get; set; }
		public bool? IsOpen { get; set; }
		public bool? IsConflicted { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<int> DeletedByUserId { get; set; }
		public string LocalFileEntryNameWithExtension { get; set; }

		public virtual FileEntryInfo FileEntry { get; set; }
		public virtual FileEntryStatus FileEntryStatus { get; set; }
		public virtual FileVersionInfo FileEntryVersion { get; set; }
	}
}
