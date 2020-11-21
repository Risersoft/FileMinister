using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using risersoft.shared.portable.Model;
using System.Runtime.Serialization;
using risersoft.shared.portable.Enums;

namespace FileMinister.Models.Sync
{
    [DataContract]
    public class LocalWorkSpaceInfo : BaseInfo
    {
        //known as UserInfo in CloudSyncProject
        public string MacAddresses { get; set; }

        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserAccountId { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string WindowsUserSId { get; set; }
        public Guid WorkSpaceId { get; set; }
        public Role RoleId { get; set; }
        public string AccessToken { get; set; }
        public string OrganisationServiceURL { get; set; }
        public string CloudSyncServiceUrl { get; set; }
        public string CloudSyncSyncServiceUrl { get; set; }
        public string LocalDatabaseName { get; set; }
        public System.DateTime LastLoggedInUTC { get; set; }
        public System.DateTime CreatedOnUTC { get; set; }
        public Guid SelectedAgentId { get; set; }
        public string SelectedAgentName { get; set; }
        public bool IsDeleted { get; set; }

        public LocalWorkSpaceInfo()
        {
           
        }
    }
    [DataContract]
    public class WorkSpaceInfo : BaseInfo
    {
        //known as UserInfo in CloudSyncProject
        public string MacAddresses { get; set; }
        public Guid WorkSpaceId { get; set; }
        public System.DateTime LastLoggedInUTC { get; set; }
        public System.DateTime CreatedOnUTC { get; set; }
        public Guid SelectedAgentId { get; set; }
        public string SelectedAgentName { get; set; }
        public bool IsDeleted { get; set; }
        public int UserDomainId { get; set; }
        public string ServerName { get; set; }
        public string IndexServerName { get; set; }
        public string IndexServerUserName { get; set; }
        public string IndexServerPassword { get; set; }
        public string UserDomain { get; set; }

        public WorkSpaceInfo()
        {

        }
    }



}
