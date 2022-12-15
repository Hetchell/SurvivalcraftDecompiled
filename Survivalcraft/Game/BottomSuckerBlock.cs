using System;
using Engine;

namespace Game
{
	// Token: 0x02000020 RID: 32
	public abstract class BottomSuckerBlock : WaterBlock
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x00007164 File Offset: 0x00005364
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Point3 point = raycastResult.CellFace.Point + CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int cellValue = subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			Block block = BlocksManager.Blocks[num];
			int face = Time.FrameIndex % 4;
			if (block is WaterBlock)
			{
				return new BlockPlacementData
				{
					CellFace = raycastResult.CellFace,
					Value = Terrain.MakeBlockValue(this.BlockIndex, 0, BottomSuckerBlock.SetSubvariant(BottomSuckerBlock.SetFace(data, raycastResult.CellFace.Face), face))
				};
			}
			return default(BlockPlacementData);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00007222 File Offset: 0x00005422
		public static int GetFace(int data)
		{
			return data >> 8 & 7;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007229 File Offset: 0x00005429
		public static int SetFace(int data, int face)
		{
			return (data & -1793) | (face & 7) << 8;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007238 File Offset: 0x00005438
		public static int GetSubvariant(int data)
		{
			return data >> 11 & 3;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007240 File Offset: 0x00005440
		public static int SetSubvariant(int data, int face)
		{
			return (data & -6145) | (face & 3) << 11;
		}
	}
}
