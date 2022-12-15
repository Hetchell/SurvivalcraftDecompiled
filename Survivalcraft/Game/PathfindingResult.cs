using System;
using Engine;

namespace Game
{
	// Token: 0x020002C5 RID: 709
	public class PathfindingResult
	{
		// Token: 0x04000DEA RID: 3562
		public volatile bool IsCompleted;

		// Token: 0x04000DEB RID: 3563
		public bool IsInProgress;

		// Token: 0x04000DEC RID: 3564
		public float PathCost;

		// Token: 0x04000DED RID: 3565
		public int PositionsChecked;

		// Token: 0x04000DEE RID: 3566
		public DynamicArray<Vector3> Path = new DynamicArray<Vector3>();
	}
}
