using System;

namespace Game
{
	// Token: 0x0200032C RID: 812
	public class UserInfo
	{
		// Token: 0x06001728 RID: 5928 RVA: 0x000B8C8F File Offset: 0x000B6E8F
		public UserInfo(string uniqueId, string displayName)
		{
			this.UniqueId = uniqueId;
			this.DisplayName = displayName;
		}

		// Token: 0x040010DF RID: 4319
		public readonly string UniqueId;

		// Token: 0x040010E0 RID: 4320
		public readonly string DisplayName;
	}
}
