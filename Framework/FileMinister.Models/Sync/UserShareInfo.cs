

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
	public class UserShareInfo : BaseInfo
	{

		public int UserAccountId { get; set; }
		public int ShareId { get; set; }
		public string SharePath { get; set; }
		public string WindowsUser { get; set; }
		public string Password { get; set; }


	}
}

