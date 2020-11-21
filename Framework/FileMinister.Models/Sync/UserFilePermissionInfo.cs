

using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using risersoft.shared.portable.Enums;
using risersoft.shared.portable.Model;
using FileMinister.Models.Enums;

namespace FileMinister.Models.Sync
{
	public class UserFilePermissionInfo : BaseInfo
	{

		//Public Property UserFilePermissionAssignmentId As System.Guid
		//Public Property UserId As Int64
		//Public Property FileId As System.Guid
		//Public Property AllowedPermissions As Integer
		//Public Property DeniedPermissions As Integer
		//Public Property IsDeleted As Boolean

		public int EffectiveAllow { get; set; }
		public int EffectiveDeny { get; set; }
		public int ExclusiveAllow { get; set; }
		public int ExclusiveDeny { get; set; }
		public Nullable<Guid>  Id { get; set; }
		public UserGroupType Type { get; set; }
        public string EffectivePermissionsSources { get; set; }

    }
}

