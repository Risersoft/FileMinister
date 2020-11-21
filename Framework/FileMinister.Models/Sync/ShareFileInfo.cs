using System;

namespace FileMinister.Models.Sync
{
    public class ShareFileInfo
    {
        public int FileShareId { get; set; }
        public System.Guid TenantID { get; set; }
        public string ShareName { get; set; }
        public string ShareContainerName { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> DeletedOnUTC { get; set; }
        public Nullable<System.Guid> DeletedByUserId { get; set; }
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
        public Nullable<DateTime> LastFullBackupTime { get; set; }
        public Nullable<DateTime> LastIncrBackupTime { get; set; }
        public Nullable<bool> UseSubDir { get; set; }
        public string BackupRootPathFull { get; set; }
        public string BackupRootPathIncr { get; set; }
        public Nullable<bool> IsBackup { get; set; }
        public Nullable<int> BackupShareID { get; set; }
        public string Description { get; set; }
        public int MultiUserVersionDiffMinutes { get; set; }
        public int SingleUserVersionDiffMinutes { get; set; }

        public Guid FileEntryId { get; set; }
    }
}
