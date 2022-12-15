using System;

namespace Game
{
	// Token: 0x02000262 RID: 610
	public class ElectricConnectionPath
	{
		// Token: 0x0600123A RID: 4666 RVA: 0x0008D893 File Offset: 0x0008BA93
		public ElectricConnectionPath(int neighborOffsetX, int neighborOffsetY, int neighborOffsetZ, int neighborFace, int connectorFace, int neighborConnectorFace)
		{
			this.NeighborOffsetX = neighborOffsetX;
			this.NeighborOffsetY = neighborOffsetY;
			this.NeighborOffsetZ = neighborOffsetZ;
			this.NeighborFace = neighborFace;
			this.ConnectorFace = connectorFace;
			this.NeighborConnectorFace = neighborConnectorFace;
		}

		// Token: 0x04000C82 RID: 3202
		public readonly int NeighborOffsetX;

		// Token: 0x04000C83 RID: 3203
		public readonly int NeighborOffsetY;

		// Token: 0x04000C84 RID: 3204
		public readonly int NeighborOffsetZ;

		// Token: 0x04000C85 RID: 3205
		public readonly int NeighborFace;

		// Token: 0x04000C86 RID: 3206
		public readonly int ConnectorFace;

		// Token: 0x04000C87 RID: 3207
		public readonly int NeighborConnectorFace;
	}
}
