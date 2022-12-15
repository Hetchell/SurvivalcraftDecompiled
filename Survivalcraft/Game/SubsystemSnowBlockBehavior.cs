using System;

namespace Game
{
	// Token: 0x020001A8 RID: 424
	public class SubsystemSnowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x0004C9CF File Offset: 0x0004ABCF
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					61
				};
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0004C9DC File Offset: 0x0004ABDC
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (!SubsystemSnowBlockBehavior.CanSupportSnow(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z)))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0004CA0C File Offset: 0x0004AC0C
		public static bool CanSupportSnow(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return !block.IsTransparent || block is LeavesBlock;
		}
	}
}
