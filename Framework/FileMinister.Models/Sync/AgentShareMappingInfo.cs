
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
	public class AgentShareMappingInfo : BaseInfo
	{
		public System.Guid FileAgentId { get; set; }
		public string AgentName { get; set; }
		public int ShareCount { get; set; }
		public short FileShareId { get; set; }
		public string ShareName { get; set; }
		public int AgentCount { get; set; }
	}
}


