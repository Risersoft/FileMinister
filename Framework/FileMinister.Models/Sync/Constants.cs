
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace risersoft.shared.portable.Enums
{
	public class Constants
	{
		//Public Const AUTH_URI As String = "http://192.168.5.100:44301" '"https://maximpriseauthv1.azurewebsites.net""https://localhost:44301" ''

		public const string CLIENT_WIN_RESOLVETYPE = "C";
		public const string SERVER_WIN_RESOLVETYPE = "S";

		public const string NEW_CONFLICT_RESOLVETYPE = "N";

		public const string TEMP_FOLDER_NAME = "##Temp";
		public const string APPLICATION_SETTING_ACCOUNT_QUOTA_LIMIT = "AccountQuotaLimit";
		public const string APPLICATION_SETTING_WEB_WORK_SPACEID = "WebWorkSpaceId";
		public const int APPLICATION_SETTING_ID_ACCOUNT_QUOTA_LIMIT = 1;
		public const int APPLICATION_SETTING_ID_WEB_WORK_SPACEID = 2;
	}
}
