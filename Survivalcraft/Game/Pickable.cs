using System;
using Engine;

namespace Game
{
	// Token: 0x020002C8 RID: 712
	public class Pickable : WorldItem
	{
		// Token: 0x04000DFE RID: 3582
		public int Count;

		// Token: 0x04000DFF RID: 3583
		public Vector3? FlyToPosition;

		// Token: 0x04000E00 RID: 3584
		public Matrix? StuckMatrix;

		// Token: 0x04000E01 RID: 3585
		public bool SplashGenerated = true;
	}
}
