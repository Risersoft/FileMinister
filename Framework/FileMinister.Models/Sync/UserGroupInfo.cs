

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
	public class UserGroupInfo : BaseInfo
	{

		public Guid Id { get; set; }
		public string Name { get; set; }
		public UserGroupType Type { get; set; }

	}

}
