using System;

namespace Game
{
	// Token: 0x02000327 RID: 807
	public class TrapDoorElectricElement : ElectricElement
	{
		// Token: 0x0600171C RID: 5916 RVA: 0x000B871E File Offset: 0x000B691E
		public TrapDoorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x000B8740 File Offset: 0x000B6940
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float voltage = (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
			if (ElectricElement.IsSignalHigh(voltage) != ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			this.m_voltage = voltage;
			if (!ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_needsReset = false;
			}
			if (!this.m_needsReset)
			{
				if (num >= 10)
				{
					if (ElectricElement.IsSignalHigh(this.m_voltage))
					{
						CellFace cellFace = base.CellFaces[0];
						int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemTrapdoorBlockBehavior>(true).OpenCloseTrapdoor(cellFace.X, cellFace.Y, cellFace.Z, !TrapdoorBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x040010C5 RID: 4293
		public int m_lastChangeCircuitStep;

		// Token: 0x040010C6 RID: 4294
		public bool m_needsReset;

		// Token: 0x040010C7 RID: 4295
		public float m_voltage;
	}
}
