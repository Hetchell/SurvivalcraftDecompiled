using System;
using Engine;

namespace Game
{
	// Token: 0x0200035F RID: 863
	public class XorGateElectricElement : RotateableElectricElement
	{
		// Token: 0x0600184B RID: 6219 RVA: 0x000C067E File Offset: 0x000BE87E
		public XorGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x000C0688 File Offset: 0x000BE888
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x000C0690 File Offset: 0x000BE890
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int? num = null;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					int num2 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					num = ((num == null) ? new int?(num2) : (num ^= num2));
				}
			}
			this.m_voltage = ((num != null) ? ((float)num.Value / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x0400113C RID: 4412
		public float m_voltage;
	}
}
