
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
	public class AgentMacAddressInfo : BaseInfo
	{

		public System.Guid AgentMacAddressId { get; set; }
		public System.Guid FileAgentId { get; set; }
		public string AgentName { get; set; }
		public string AgentMacAddress { get; set; }
		public System.DateTime CreatedOnUTC { get; set; }
		public int CreatedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public Nullable<System.DateTime> DeletedOnUTC { get; set; }
		public Nullable<System.Guid> DeletedByUserId { get; set; }
	}
}
