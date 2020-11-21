

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
	public class FileVersionConflictInfo : BaseInfo
	{
		public System.Guid FileVersionConflictId { get; set; }
		public System.Guid FileEntryId { get; set; }
		public System.Guid FileVersionId { get; set; }
		public byte FileVersionConflictTypeId { get; set; }
		public string FileVersionConflictType { get; set; }
		public Nullable<Guid> UserId { get; set; }
		public System.Guid WorkSpaceId { get; set; }
		public string FileEntryPath { get; set; }
		public string FileEntryNameAndExtension { get; set; }
		public bool IsResolved { get; set; }
		public Nullable<int> ResolvedByUserId { get; set; }
		public Nullable<System.DateTime> ResolvedOnUTC { get; set; }
		public string ResolvedType { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }


		public string UserName { get; set; }
		public int? VersionNumber { get; set; }

		// next three properites only from server db
		public string FileEntryHash { get; set; }
		public Nullable<long> FileEntrySize { get; set; }
		public byte FileVersionConflictRequestStatusId { get; set; }
		public string FileVersionConflictRequestStatus { get; set; }

		public bool IsClientUploadRequested { get; set; }
	}
}
