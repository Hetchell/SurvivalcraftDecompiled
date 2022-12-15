using System;
using Engine;

namespace Game
{
	// Token: 0x02000316 RID: 790
	public struct TerrainRaycastResult
	{
		// Token: 0x060016A4 RID: 5796 RVA: 0x000B3750 File Offset: 0x000B1950
		public Vector3 HitPoint(float offsetFromSurface = 0f)
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance + CellFace.FaceToVector3(this.CellFace.Face) * offsetFromSurface;
		}

		// Token: 0x04001051 RID: 4177
		public Ray3 Ray;

		// Token: 0x04001052 RID: 4178
		public int Value;

		// Token: 0x04001053 RID: 4179
		public CellFace CellFace;

		// Token: 0x04001054 RID: 4180
		public int CollisionBoxIndex;

		// Token: 0x04001055 RID: 4181
		public float Distance;
	}
}
