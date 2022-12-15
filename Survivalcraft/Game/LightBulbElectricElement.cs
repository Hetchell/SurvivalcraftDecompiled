using System;
using Engine;

namespace Game
{
	// Token: 0x020002A3 RID: 675
	public class LightBulbElectricElement : MountedElectricElement
	{
		// Token: 0x06001388 RID: 5000 RVA: 0x000978E8 File Offset: 0x00095AE8
		public LightBulbElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			int data = Terrain.ExtractData(value);
			this.m_intensity = LightbulbBlock.GetLightIntensity(data);
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x00097924 File Offset: 0x00095B24
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float num2 = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num2 = MathUtils.Max(num2, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			int intensity = this.m_intensity;
			this.m_intensity = MathUtils.Clamp((int)MathUtils.Round((num2 - 0.5f) * 30f), 0, 15);
			if (this.m_intensity != intensity)
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			if (num >= 10)
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int data = LightbulbBlock.SetLightIntensity(Terrain.ExtractData(cellValue), this.m_intensity);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			}
			else
			{
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
			}
			return false;
		}

		// Token: 0x04000D5F RID: 3423
		public int m_intensity;

		// Token: 0x04000D60 RID: 3424
		public int m_lastChangeCircuitStep;
	}
}
