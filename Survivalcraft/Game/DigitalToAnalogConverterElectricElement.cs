using System;

namespace Game
{
	// Token: 0x02000251 RID: 593
	public class DigitalToAnalogConverterElectricElement : RotateableElectricElement
	{
		// Token: 0x060011E5 RID: 4581 RVA: 0x0008A4C3 File Offset: 0x000886C3
		public DigitalToAnalogConverterElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x0008A4CD File Offset: 0x000886CD
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x0008A4D8 File Offset: 0x000886D8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = 0f;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input && ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						if (connectorDirection.Value == ElectricConnectorDirection.Top)
						{
							this.m_voltage += 0.06666667f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Right)
						{
							this.m_voltage += 0.13333334f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
						{
							this.m_voltage += 0.26666668f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Left)
						{
							this.m_voltage += 0.53333336f;
						}
					}
				}
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000C07 RID: 3079
		public float m_voltage;
	}
}
