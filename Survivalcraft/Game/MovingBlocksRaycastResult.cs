using System;
using Engine;

namespace Game
{
	// Token: 0x020000B7 RID: 183
	public struct MovingBlocksRaycastResult
	{
		// Token: 0x06000368 RID: 872 RVA: 0x00013386 File Offset: 0x00011586
		public Vector3 HitPoint()
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance;
		}

		// Token: 0x04000195 RID: 405
		public Ray3 Ray;

		// Token: 0x04000196 RID: 406
		public IMovingBlockSet MovingBlockSet;

		// Token: 0x04000197 RID: 407
		public float Distance;
	}
}
