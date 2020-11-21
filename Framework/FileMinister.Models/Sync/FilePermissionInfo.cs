using FileMinister.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMinister.Models.Sync
{
    public class FilePermissionInfo
    {
        public Guid FileEntryId { get; set; }
        public byte FileEntryTypeId { get; set; }
        public short FileShareId { get; set; }
        public string RelativePath { get; set; }
        public int ExclusiveAllow { get; set; }
        public int ExclusiveDeny { get; set; }
        public Guid UserGroupId { get; set; }
        public UserGroupType Type { get; set; }
        public string SID { get; set; }
    }
}
