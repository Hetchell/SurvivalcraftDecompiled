using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200035A RID: 858
	public class WorldInfo
	{
		// Token: 0x0400110E RID: 4366
		public string DirectoryName = string.Empty;

		// Token: 0x0400110F RID: 4367
		public long Size;

		// Token: 0x04001110 RID: 4368
		public DateTime LastSaveTime;

		// Token: 0x04001111 RID: 4369
		public string SerializationVersion = string.Empty;

		// Token: 0x04001112 RID: 4370
		public WorldSettings WorldSettings = new WorldSettings();

		// Token: 0x04001113 RID: 4371
		public List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
	}
}
