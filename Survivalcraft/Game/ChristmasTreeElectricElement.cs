using System;

namespace Game
{
	// Token: 0x0200023B RID: 571
	public class ChristmasTreeElectricElement : ElectricElement
	{
		// Token: 0x0600119F RID: 4511 RVA: 0x000882B6 File Offset: 0x000864B6
		public ChristmasTreeElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_voltage = (float)(ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)) ? 1 : 0);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x000882EC File Offset: 0x000864EC
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float voltage = (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
			if (ElectricElement.IsSignalHigh(voltage) != ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			this.m_voltage = voltage;
			if (num >= 10)
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int data = ChristmasTreeBlock.SetLightState(Terrain.ExtractData(cellValue), ElectricElement.IsSignalHigh(this.m_voltage));
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			}
			else
			{
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
			}
			return false;
		}

		// Token: 0x04000BB2 RID: 2994
		public int m_lastChangeCircuitStep;

		// Token: 0x04000BB3 RID: 2995
		public float m_voltage;
	}
}
