using System;
using Engine;

namespace Game
{
	// Token: 0x02000219 RID: 537
	public class AndGateElectricElement : RotateableElectricElement
	{
		// Token: 0x06001082 RID: 4226 RVA: 0x0007DFEF File Offset: 0x0007C1EF
		public AndGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0007DFF9 File Offset: 0x0007C1F9
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0007E004 File Offset: 0x0007C204
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			int num2 = 15;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num2 &= (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					num++;
				}
			}
			this.m_voltage = ((num == 2) ? ((float)num2 / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x04000ACD RID: 2765
		public float m_voltage;
	}
}
