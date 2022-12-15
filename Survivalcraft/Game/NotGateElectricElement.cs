using System;
using Engine;

namespace Game
{
	// Token: 0x020002BB RID: 699
	public class NotGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060013EE RID: 5102 RVA: 0x0009A4F0 File Offset: 0x000986F0
		public NotGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0009A4FA File Offset: 0x000986FA
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x0009A504 File Offset: 0x00098704
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					break;
				}
			}
			this.m_voltage = (float)(num & 15) / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000DC1 RID: 3521
		public float m_voltage;
	}
}
