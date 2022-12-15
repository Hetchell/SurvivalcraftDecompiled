using System;

namespace Game
{
	// Token: 0x02000270 RID: 624
	public class FenceGateElectricElement : ElectricElement
	{
		// Token: 0x0600127D RID: 4733 RVA: 0x0008E871 File Offset: 0x0008CA71
		public FenceGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x0008E894 File Offset: 0x0008CA94
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemFenceGateBlockBehavior>(true).OpenCloseGate(cellFace.X, cellFace.Y, cellFace.Z, !FenceGateBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000CB1 RID: 3249
		public int m_lastChangeCircuitStep;

		// Token: 0x04000CB2 RID: 3250
		public bool m_needsReset;

		// Token: 0x04000CB3 RID: 3251
		public float m_voltage;
	}
}
