using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020002FA RID: 762
	public class SpawnChunk
	{
		// Token: 0x04000F5F RID: 3935
		public Point2 Point;

		// Token: 0x04000F60 RID: 3936
		public bool IsSpawned;

		// Token: 0x04000F61 RID: 3937
		public double? LastVisitedTime;

		// Token: 0x04000F62 RID: 3938
		public List<SpawnEntityData> SpawnsData = new List<SpawnEntityData>();
	}
}
