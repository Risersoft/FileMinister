using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMinister.Models.Sync
{
    public class UserInfo
    {
        public System.Guid UserId { get; set; }
        public System.Guid TenantID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> DeletedOnUTC { get; set; }
        public Nullable<System.Guid> DeletedByUserId { get; set; }
        public Nullable<DateTime> LastLogin { get; set; }
        public string DefaultAppCode { get; set; }
        public string SID { get; set; }
    }
}
