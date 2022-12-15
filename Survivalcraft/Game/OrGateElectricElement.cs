using System;
using Engine;

namespace Game
{
	// Token: 0x020002BF RID: 703
	public class OrGateElectricElement : RotateableElectricElement
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x0009AE98 File Offset: 0x00099098
		public OrGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0009AEA2 File Offset: 0x000990A2
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0009AEAC File Offset: 0x000990AC
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

		// Token: 0x04000DCF RID: 3535
		public float m_voltage;
	}
}
