
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using risersoft.shared.portable.Model;

namespace FileMinister.Models.Sync
{
    [JsonObject()]

    public class ConfigInfo : BaseInfo
    {

        public int UserAccountId { get; set; }
        public Guid AccountId { get; set; }
        public WorkSpaceInfo WorkSpace { get; set; }
        public int FileShareId { get; set; }
        public string ShareName { get; set; }
        public string SharePath { get; set; }
        public string WindowsUser { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> LastSyncedUTC { get; set; }
        public System.DateTime CreatedOnUTC { get; set; }
        public Nullable<Guid> CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<Guid> DeletedByUserId { get; set; }
        public Nullable<System.DateTime> DeletedOnUTC { get; set; }
        public bool IsSelected { get; set; }
        public virtual ICollection<FileAgentShareInfo> AgentShares { get; set; }
        public string ShareType { get; set; }
        public Nullable<long> DiskSpaceMB { get; set; }
        public Nullable<bool> MaintainVersionHistory { get; set; }
        public Nullable<int> VersionDiffMinutes { get; set; }
        public Nullable<int> MaxVersionMonths { get; set; }
        public Nullable<int> MaxVersionFileSizeMB { get; set; }
        public string EmailTo { get; set; }
        public string Emailcc { get; set; }
        public string FullBackupPeriodType { get; set; }
        public Nullable<int> FullBackupPeriodValue { get; set; }
        public string IncrBackupPeriodType { get; set; }
        public Nullable<int> IncrBackupPeriodValue { get; set; }
        public Nullable<System.DateTime> LastFullBackupTime { get; set; }
        public Nullable<System.DateTime> LastIncrBackupTime { get; set; }
        public Nullable<bool> UseSubDir { get; set; }
        public string BackupRootPathFull { get; set; }
        public string BackupRootPathIncr { get; set; }
        public Nullable<bool> IsBackup { get; set; }
        public Nullable<int> BackupShareID { get; set; }
        public string Description { get; set; }

    }
    public class LocalConfigInfo : ConfigInfo
    {
        public LocalWorkSpaceInfo User { get; set; }

    }


}