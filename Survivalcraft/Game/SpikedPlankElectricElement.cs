using System;

namespace Game
{
	// Token: 0x020002FE RID: 766
	public class SpikedPlankElectricElement : MountedElectricElement
	{
		// Token: 0x060015A9 RID: 5545 RVA: 0x000A5337 File Offset: 0x000A3537
		public SpikedPlankElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x000A535C File Offset: 0x000A355C
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemSpikesBlockBehavior>(true).RetractExtendSpikes(cellFace.X, cellFace.Y, cellFace.Z, !SpikedPlankBlock.GetSpikesState(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000F6D RID: 3949
		public int m_lastChangeCircuitStep;

		// Token: 0x04000F6E RID: 3950
		public bool m_needsReset;

		// Token: 0x04000F6F RID: 3951
		public float m_voltage;
	}
}
