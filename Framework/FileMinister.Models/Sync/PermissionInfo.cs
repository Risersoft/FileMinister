
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
	public class PermissionInfo : BaseInfo
	{

		public int PermissionId { get; set; }
		public string PermissionName { get; set; }

	}
}
