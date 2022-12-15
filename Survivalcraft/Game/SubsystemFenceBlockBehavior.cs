using System;

namespace Game
{
	// Token: 0x02000179 RID: 377
	public class SubsystemFenceBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x000384B9 File Offset: 0x000366B9
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x000384C4 File Offset: 0x000366C4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			this.UpdateVariant(cellValue, x, y, z);
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x000384EF File Offset: 0x000366EF
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateVariant(value, x, y, z);
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00038500 File Offset: 0x00036700
		public void UpdateVariant(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractContents(value);
			FenceBlock fenceBlock = BlocksManager.Blocks[num] as FenceBlock;
			if (fenceBlock != null)
			{
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + 1, y, z);
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x - 1, y, z);
				int cellValue3 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z + 1);
				int cellValue4 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z - 1);
				int num2 = 0;
				if (fenceBlock.ShouldConnectTo(cellValue))
				{
					num2++;
				}
				if (fenceBlock.ShouldConnectTo(cellValue2))
				{
					num2 += 2;
				}
				if (fenceBlock.ShouldConnectTo(cellValue3))
				{
					num2 += 4;
				}
				if (fenceBlock.ShouldConnectTo(cellValue4))
				{
					num2 += 8;
				}
				int data = Terrain.ExtractData(value);
				int value2 = Terrain.ReplaceData(value, FenceBlock.SetVariant(data, num2));
				base.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
			}
		}
	}
}
