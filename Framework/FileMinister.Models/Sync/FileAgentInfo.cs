
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
	public class FileAgentInfo : BaseInfo
	{
		public System.Guid FileAgentId { get; set; }
		public string AgentName { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public int CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<System.Guid> DeletedByUserId { get; set; }
		public ICollection<FileAgentShareInfo> AgentShares { get; set; }
		public ICollection<WorkSpaceInfo> WorkSpaces { get; set; }
		public string SecretKey { get; set; }
		public bool IsSelected { get; set; }
	}
}
