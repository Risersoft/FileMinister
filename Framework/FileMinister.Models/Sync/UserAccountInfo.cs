using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileMinister.Models.Sync
{
    public class UserAccountInfo
    {
        public int UserAccountId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public int RoleId { get; set; }
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public string WindowsUserSID { get; set; }
        public Guid WorkSpaceId { get; set; }
        public string OrganisationServiceURL { get; set; }
        public string CloudSyncServiceURL { get; set; }
        public string CloudSyncSyncServiceURL { get; set; }
        public string LocalDatabaseName { get; set; }
        public string AccessToken { get; set; }
        public System.DateTime LastLoggedInUTC { get; set; }
        public System.DateTime CreatedOnUTC { get; set; }
    }

}