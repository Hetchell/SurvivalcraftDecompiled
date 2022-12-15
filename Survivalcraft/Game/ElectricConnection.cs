using System;

namespace Game
{
	// Token: 0x02000261 RID: 609
	public class ElectricConnection
	{
		// Token: 0x04000C7B RID: 3195
		public CellFace CellFace;

		// Token: 0x04000C7C RID: 3196
		public int ConnectorFace;

		// Token: 0x04000C7D RID: 3197
		public ElectricConnectorType ConnectorType;

		// Token: 0x04000C7E RID: 3198
		public ElectricElement NeighborElectricElement;

		// Token: 0x04000C7F RID: 3199
		public CellFace NeighborCellFace;

		// Token: 0x04000C80 RID: 3200
		public int NeighborConnectorFace;

		// Token: 0x04000C81 RID: 3201
		public ElectricConnectorType NeighborConnectorType;
	}
}
