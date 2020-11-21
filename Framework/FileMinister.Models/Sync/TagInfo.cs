
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
	public class TagInfo : BaseInfo
	{

		public System.Guid TagId { get; set; }
		public System.Guid FileEntryId { get; set; }
		public byte TagTypeId { get; set; }
		public string TagName { get; set; }
		public string TagValue { get; set; }
		public byte DisplayOrder { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public Nullable<Guid> CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<System.Guid> DeletedByUserId { get; set; }

		//Public Overridable Property File As File
		//Public Overridable Property TagType As TagType
		//Public Overridable Property User As User
		//Public Overridable Property User1 As User

	}
}


