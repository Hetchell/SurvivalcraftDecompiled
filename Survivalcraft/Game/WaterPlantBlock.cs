using System;
using Engine;

namespace Game
{
	// Token: 0x02000118 RID: 280
	public abstract class WaterPlantBlock : WaterBlock
	{
		// Token: 0x06000559 RID: 1369 RVA: 0x0001D94C File Offset: 0x0001BB4C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Point3 point = raycastResult.CellFace.Point + CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int cellValue = subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is WaterBlock)
			{
				return new BlockPlacementData
				{
					CellFace = raycastResult.CellFace,
					Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data)
				};
			}
			return default(BlockPlacementData);
		}
	}
}
