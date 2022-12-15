using System;
using Engine;

namespace Game
{
	// Token: 0x02000215 RID: 533
	public class AnalogToDigitalConverterElectricElement : RotateableElectricElement
	{
		// Token: 0x06001077 RID: 4215 RVA: 0x0007DC05 File Offset: 0x0007BE05
		public AnalogToDigitalConverterElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0007DC10 File Offset: 0x0007BE10
		public override float GetOutputVoltage(int face)
		{
			ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, base.Rotation, face);
			if (connectorDirection != null)
			{
				if (connectorDirection.Value == ElectricConnectorDirection.Top)
				{
					return (float)(((this.m_bits & 1) != 0) ? 1 : 0);
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Right)
				{
					return (float)(((this.m_bits & 2) != 0) ? 1 : 0);
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
				{
					return (float)(((this.m_bits & 4) != 0) ? 1 : 0);
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Left)
				{
					return (float)(((this.m_bits & 8) != 0) ? 1 : 0);
				}
			}
			return 0f;
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x0007DCB4 File Offset: 0x0007BEB4
		public override bool Simulate()
		{
			int bits = this.m_bits;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null && connectorDirection.Value == ElectricConnectorDirection.In)
					{
						float outputVoltage = electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace);
						this.m_bits = (int)MathUtils.Round(outputVoltage * 15f);
					}
				}
			}
			return this.m_bits != bits;
		}

		// Token: 0x04000ACA RID: 2762
		public int m_bits;
	}
}
