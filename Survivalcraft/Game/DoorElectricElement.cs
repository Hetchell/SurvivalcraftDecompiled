using System;

namespace Game
{
	// Token: 0x02000254 RID: 596
	public class DoorElectricElement : ElectricElement
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x0008A922 File Offset: 0x00088B22
		public DoorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0008A944 File Offset: 0x00088B44
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemDoorBlockBehavior>(true).OpenCloseDoor(cellFace.X, cellFace.Y, cellFace.Z, !DoorBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000C0B RID: 3083
		public int m_lastChangeCircuitStep;

		// Token: 0x04000C0C RID: 3084
		public bool m_needsReset;

		// Token: 0x04000C0D RID: 3085
		public float m_voltage;
	}
}
