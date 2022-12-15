using System;
using Engine;

namespace Game
{
	// Token: 0x020002BA RID: 698
	public class NorGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060013EB RID: 5099 RVA: 0x0009A438 File Offset: 0x00098638
		public NorGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0009A442 File Offset: 0x00098642
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0009A44C File Offset: 0x0009864C
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
			this.m_voltage = (float)(num & 15) / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000DC0 RID: 3520
		public float m_voltage;
	}
}
