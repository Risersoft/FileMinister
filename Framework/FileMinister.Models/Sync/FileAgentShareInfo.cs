
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace FileMinister.Models.Sync
{
	public class FileAgentShareInfo
	{
		public int FileAgentShareId { get; set; }
		public System.Guid FileAgentId { get; set; }
		public short FileShareId { get; set; }
		public string ShareName { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public Nullable<Guid> CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<Guid> DeletedByUserId { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
	}
}
