using System;

namespace FileMinister.Models.Sync
{
    public class FileHandleInfo
    {
        public int FileHandleId { get; set; }
        public System.Guid TenantID { get; set; }
        public string FileRelativePath { get; set; }
        public System.Guid FileEntryId { get; set; }
        public System.Guid WorkSpaceId { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public DateTime OpenedAt { get; set; }
        public Nullable<DateTime> ClosedAt { get; set; }
        public int ServerFileSizeAtOpen { get; set; }
        public DateTime ServerFileTimeAtOpen { get; set; }
        public Nullable<int> ServerFileSizeAtClose { get; set; }
        public Nullable<DateTime> ServerFileTimeAtClose { get; set; }
        public string ToBePrcessed { get; set; }

        public string UserSID { get; set; }

        public short FileShareId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals((FileHandleInfo)obj);
        }

        public override int GetHashCode()
        {
            return FileRelativePath.GetHashCode() * ((UserId.HasValue && UserId != Guid.Empty) ? UserId.GetHashCode() : 1) * (String.IsNullOrEmpty(UserSID) ? 1 : UserSID.GetHashCode());
        }

        public bool Equals(FileHandleInfo obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            //if (!base.Equals(obj))
            //    return false;

            return (this.FileRelativePath == obj.FileRelativePath && (((!this.UserId.HasValue || this.UserId == Guid.Empty) && (!obj.UserId.HasValue || obj.UserId == Guid.Empty)) || this.UserId == obj.UserId || this.UserSID == obj.UserSID));
        }
    }
}