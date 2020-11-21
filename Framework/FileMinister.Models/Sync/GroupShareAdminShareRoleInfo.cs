
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
	public class GroupShareAdminShareRoleInfo : BaseInfo
	{
		public System.Guid GroupShareAdminRoleId { get; set; }
		public int GroupId { get; set; }
		public short ShareId { get; set; }
		public byte RoleId { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public int CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<int> DeletedByUserId { get; set; }
	}

}
