using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x020001AD RID: 429
	public class SubsystemStairsBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000A82 RID: 2690 RVA: 0x0004E2AB File Offset: 0x0004C4AB
		public override int[] HandledBlocks
		{
			get
			{
				return this.m_handledBlocks;
			}
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0004E2B4 File Offset: 0x0004C4B4
		public SubsystemStairsBlockBehavior()
		{
			List<int> list = new List<int>();
			list.AddRange(from b in BlocksManager.Blocks
			where b is StairsBlock
			select b.BlockIndex);
			this.m_handledBlocks = list.ToArray();
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0004E32C File Offset: 0x0004C52C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			this.UpdateIsCorner(cellValue, x, y, z, true);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0004E358 File Offset: 0x0004C558
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.UpdateIsCorner(value, x, y, z, false);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0004E366 File Offset: 0x0004C566
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateIsCorner(value, x, y, z, true);
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0004E378 File Offset: 0x0004C578
		public void UpdateIsCorner(int value, int x, int y, int z, bool updateModificationCounter)
		{
			int value2 = Terrain.ExtractContents(value);
			if (!this.HandledBlocks.Contains(value2))
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			if (StairsBlock.GetCornerType(data) != StairsBlock.CornerType.None)
			{
				return;
			}
			int rotation = StairsBlock.GetRotation(data);
			bool isUpsideDown = StairsBlock.GetIsUpsideDown(data);
			Point3 point = StairsBlock.RotationToDirection(rotation);
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is StairsBlock)
			{
				int data2 = Terrain.ExtractData(cellValue);
				bool isUpsideDown2 = StairsBlock.GetIsUpsideDown(data2);
				StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(data2);
				int num2 = -1;
				if (isUpsideDown2 == isUpsideDown)
				{
					int rotation2 = StairsBlock.GetRotation(data2);
					if (rotation == 0 && rotation2 == 1 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 1;
					}
					if (rotation == 0 && rotation2 == 3 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 0;
					}
					if (rotation == 1 && rotation2 == 0 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 1;
					}
					if (rotation == 1 && rotation2 == 2 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 2;
					}
					if (rotation == 2 && rotation2 == 1 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 2;
					}
					if (rotation == 2 && rotation2 == 3 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 3;
					}
					if (rotation == 3 && rotation2 == 0 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 0;
					}
					if (rotation == 3 && rotation2 == 2 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 3;
					}
				}
				if (num2 >= 0)
				{
					int data3 = StairsBlock.SetRotation(StairsBlock.SetCornerType(data, StairsBlock.CornerType.OneQuarter), num2);
					int value3 = Terrain.ReplaceData(value, data3);
					base.SubsystemTerrain.ChangeCell(x, y, z, value3, updateModificationCounter);
				}
				return;
			}
			cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
			num = Terrain.ExtractContents(cellValue);
			if (!(BlocksManager.Blocks[num] is StairsBlock))
			{
				return;
			}
			int data4 = Terrain.ExtractData(cellValue);
			bool isUpsideDown3 = StairsBlock.GetIsUpsideDown(data4);
			StairsBlock.CornerType cornerType2 = StairsBlock.GetCornerType(data4);
			int num3 = -1;
			if (isUpsideDown3 == isUpsideDown)
			{
				int rotation3 = StairsBlock.GetRotation(data4);
				if (rotation == 0 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 1;
				}
				if (rotation == 0 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 0;
				}
				if (rotation == 0 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 1;
				}
				if (rotation == 0 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 0;
				}
				if (rotation == 1 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 1;
				}
				if (rotation == 1 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 2;
				}
				if (rotation == 1 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 2;
				}
				if (rotation == 1 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 1;
				}
				if (rotation == 2 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 2;
				}
				if (rotation == 2 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 3;
				}
				if (rotation == 2 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 3;
				}
				if (rotation == 2 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 2;
				}
				if (rotation == 3 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 0;
				}
				if (rotation == 3 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 3;
				}
				if (rotation == 3 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 3;
				}
				if (rotation == 3 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 0;
				}
			}
			if (num3 >= 0)
			{
				int data5 = StairsBlock.SetRotation(StairsBlock.SetCornerType(data, StairsBlock.CornerType.ThreeQuarters), num3);
				int value4 = Terrain.ReplaceData(value, data5);
				base.SubsystemTerrain.ChangeCell(x, y, z, value4, updateModificationCounter);
			}
		}

		// Token: 0x040005C4 RID: 1476
		public int[] m_handledBlocks;
	}
}
