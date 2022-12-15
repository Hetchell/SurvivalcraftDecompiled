using System;

namespace Game
{
	// Token: 0x020001B8 RID: 440
	public class SubsystemWaterPlantBlockBehavior : SubsystemInWaterBlockBehavior
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x000517FC File Offset: 0x0004F9FC
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00051804 File Offset: 0x0004FA04
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			int num = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z));
			int num2 = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z));
			if (num2 != 2 && num2 != 7 && num2 != 72 && num2 != num)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
