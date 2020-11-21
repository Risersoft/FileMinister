

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
	public class FileSearch : BaseInfo
	{
		public System.Guid StartFileId { get; set; }
		public short FileShareId { get; set; }
		public string SharePath { get; set; }
		public string SearchText { get; set; }
		//Public Property Tags As Dictionary(Of String, String) = New Dictionary(Of String, String)
		public IList<FileTag> Tags { get; set; }
		public bool IsAdvancedSearch { get; set; }
	}

	public class FileTag : BaseInfo
	{
		public string TagName { get; set; }
		public string TagValue { get; set; }
	}

}

