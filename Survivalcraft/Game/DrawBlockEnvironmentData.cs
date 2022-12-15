using System;
using Engine;

namespace Game
{
	// Token: 0x02000258 RID: 600
	public class DrawBlockEnvironmentData
	{
		// Token: 0x06001216 RID: 4630 RVA: 0x0008B6D1 File Offset: 0x000898D1
		public DrawBlockEnvironmentData()
		{
			this.InWorldMatrix = Matrix.Identity;
			this.Humidity = 15;
			this.Temperature = 8;
			this.Light = 15;
		}

		// Token: 0x04000C1F RID: 3103
		public SubsystemTerrain SubsystemTerrain;

		// Token: 0x04000C20 RID: 3104
		public Matrix InWorldMatrix;

		// Token: 0x04000C21 RID: 3105
		public Matrix? ViewProjectionMatrix;

		// Token: 0x04000C22 RID: 3106
		public Vector3? BillboardDirection;

		// Token: 0x04000C23 RID: 3107
		public int Humidity;

		// Token: 0x04000C24 RID: 3108
		public int Temperature;

		// Token: 0x04000C25 RID: 3109
		public int Light;
	}
}
