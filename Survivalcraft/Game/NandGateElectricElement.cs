using System;
using Engine;

namespace Game
{
	// Token: 0x020002B9 RID: 697
	public class NandGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060013E8 RID: 5096 RVA: 0x0009A36A File Offset: 0x0009856A
		public NandGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0009A374 File Offset: 0x00098574
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0009A37C File Offset: 0x0009857C
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
			this.m_voltage = ((num == 2) ? ((float)(num2 & 15) / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x04000DBF RID: 3519
		public float m_voltage;
	}
}
