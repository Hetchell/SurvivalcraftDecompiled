using System;
using Engine;

namespace Game
{
	// Token: 0x02000220 RID: 544
	public class BatteryElectricElement : ElectricElement
	{
		// Token: 0x060010B2 RID: 4274 RVA: 0x0007EFEB File Offset: 0x0007D1EB
		public BatteryElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x0007EFF8 File Offset: 0x0007D1F8
		public override float GetOutputVoltage(int face)
		{
			Point3 point = base.CellFaces[0].Point;
			return (float)BatteryBlock.GetVoltageLevel(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z))) / 15f;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0007F058 File Offset: 0x0007D258
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y - 1, cellFace.Z);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			if (!block.IsCollidable || block.IsTransparent)
			{
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
			}
		}
	}
}
