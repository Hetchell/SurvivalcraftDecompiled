using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200032D RID: 813
	public static class UserManager
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x000B8CA5 File Offset: 0x000B6EA5
		// (set) Token: 0x0600172A RID: 5930 RVA: 0x000B8CBF File Offset: 0x000B6EBF
		public static UserInfo ActiveUser
		{
			get
			{
				return UserManager.GetUser(SettingsManager.UserId) ?? UserManager.GetUsers().FirstOrDefault<UserInfo>();
			}
			set
			{
				SettingsManager.UserId = ((value != null) ? value.UniqueId : string.Empty);
			}
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x000B8CD8 File Offset: 0x000B6ED8
		static UserManager()
		{
			string text;
			try
			{
				string path = "app:/UserId.dat";
				if (!Storage.FileExists(path))
				{
					text = Guid.NewGuid().ToString();
					Storage.WriteAllText(path, text);
				}
				else
				{
					text = Storage.ReadAllText(path);
				}
			}
			catch (Exception)
			{
				text = Guid.NewGuid().ToString();
			}
			UserManager.m_users.Add(new UserInfo(text.ToString(), "Windows User"));
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x000B8D64 File Offset: 0x000B6F64
		public static IEnumerable<UserInfo> GetUsers()
		{
			return new ReadOnlyList<UserInfo>(UserManager.m_users);
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x000B8D78 File Offset: 0x000B6F78
		public static UserInfo GetUser(string uniqueId)
		{
			return UserManager.GetUsers().FirstOrDefault((UserInfo u) => u.UniqueId == uniqueId);
		}

		// Token: 0x040010E1 RID: 4321
		public static List<UserInfo> m_users = new List<UserInfo>();
	}
}
