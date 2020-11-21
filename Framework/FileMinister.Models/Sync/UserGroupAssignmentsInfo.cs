

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
	public class UserGroupAssignmentsInfo : BaseInfo
	{

		public Guid UserGroupAssignmentId { get; set; }
		public Nullable<Guid> UserId { get; set; }
		public Nullable<Guid> GroupId { get; set; }

	}
}
