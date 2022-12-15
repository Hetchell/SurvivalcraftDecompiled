using System;
using Engine;

namespace Game
{
	// Token: 0x0200022D RID: 557
	public struct BodyRaycastResult
	{
		// Token: 0x0600112A RID: 4394 RVA: 0x00086012 File Offset: 0x00084212
		public Vector3 HitPoint()
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance;
		}

		// Token: 0x04000B74 RID: 2932
		public Ray3 Ray;

		// Token: 0x04000B75 RID: 2933
		public ComponentBody ComponentBody;

		// Token: 0x04000B76 RID: 2934
		public float Distance;
	}
}
