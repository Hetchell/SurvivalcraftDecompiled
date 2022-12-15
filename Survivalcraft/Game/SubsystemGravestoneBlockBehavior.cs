using System;

namespace Game
{
	// Token: 0x02000185 RID: 389
	public class SubsystemGravestoneBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060008EB RID: 2283 RVA: 0x0003D9CA File Offset: 0x0003BBCA
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					189
				};
			}
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0003D9DC File Offset: 0x0003BBDC
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
