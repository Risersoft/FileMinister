
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace FileMinister.Models.Sync
{
	public class CheckinReportInfo
	{
        public Guid FileEntryId { get; set; }
        public string FileEntryName { get; set; }
        public string FileEntryNameWithExtension { get; set; }
        public string Share { get; set; }
		public string FilePath { get; set; }
		public string User { get; set; }
		public DateTime Time { get; set; }
		public int Version { get; set; }

        public FileEntryLinkInfo FileEntryLinkInfo { get; set; }
    }

}
