using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000359 RID: 857
	public class WireDomainElectricElement : ElectricElement
	{
		// Token: 0x0600181C RID: 6172 RVA: 0x000BE811 File Offset: 0x000BCA11
		public WireDomainElectricElement(SubsystemElectricity subsystemElectricity, IEnumerable<CellFace> cellFaces) : base(subsystemElectricity, cellFaces)
		{
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x000BE81B File Offset: 0x000BCA1B
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x000BE824 File Offset: 0x000BCA24
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num |= (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
				}
			}
			this.m_voltage = (float)num / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x000BE8C4 File Offset: 0x000BCAC4
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (!(BlocksManager.Blocks[num] is WireBlock))
			{
				return;
			}
			int wireFacesBitmask = WireBlock.GetWireFacesBitmask(cellValue);
			int num2 = wireFacesBitmask;
			if (WireBlock.WireExistsOnFace(cellValue, cellFace.Face))
			{
				Point3 point = CellFace.FaceToPoint3(cellFace.Face);
				int cellValue2 = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X - point.X, cellFace.Y - point.Y, cellFace.Z - point.Z);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
				if (!block.IsCollidable || block.IsTransparent)
				{
					num2 &= ~(1 << cellFace.Face);
				}
			}
			if (num2 == 0)
			{
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
				return;
			}
			if (num2 != wireFacesBitmask)
			{
				int newValue = WireBlock.SetWireFacesBitmask(cellValue, num2);
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, newValue, false, false);
			}
		}

		// Token: 0x0400110D RID: 4365
		public float m_voltage;
	}
}
