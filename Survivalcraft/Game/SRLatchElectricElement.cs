using System;

namespace Game
{
	// Token: 0x020002FF RID: 767
	public class SRLatchElectricElement : RotateableElectricElement
	{
		// Token: 0x060015AB RID: 5547 RVA: 0x000A547C File Offset: 0x000A367C
		public SRLatchElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			float? num = subsystemElectricity.ReadPersistentVoltage(cellFace.Point);
			if (num != null)
			{
				this.m_voltage = num.Value;
			}
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x000A54CA File Offset: 0x000A36CA
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x000A54D4 File Offset: 0x000A36D4
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
						ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Right;
						if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
						{
							flag2 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								flag = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									flag3 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
									flag4 = true;
								}
							}
						}
					}
				}
			}
			if (flag4)
			{
				if (flag3 && this.m_clockAllowed)
				{
					this.m_clockAllowed = false;
					if (flag && flag2)
					{
						this.m_voltage = (float)((!ElectricElement.IsSignalHigh(this.m_voltage)) ? 1 : 0);
					}
					else if (flag)
					{
						this.m_voltage = 1f;
					}
					else if (flag2)
					{
						this.m_voltage = 0f;
					}
				}
			}
			else if (flag && this.m_setAllowed)
			{
				this.m_setAllowed = false;
				this.m_voltage = 1f;
			}
			else if (flag2 && this.m_resetAllowed)
			{
				this.m_resetAllowed = false;
				this.m_voltage = 0f;
			}
			if (!flag3)
			{
				this.m_clockAllowed = true;
			}
			if (!flag)
			{
				this.m_setAllowed = true;
			}
			if (!flag2)
			{
				this.m_resetAllowed = true;
			}
			if (this.m_voltage != voltage)
			{
				base.SubsystemElectricity.WritePersistentVoltage(base.CellFaces[0].Point, this.m_voltage);
				return true;
			}
			return false;
		}

		// Token: 0x04000F70 RID: 3952
		public bool m_setAllowed = true;

		// Token: 0x04000F71 RID: 3953
		public bool m_resetAllowed = true;

		// Token: 0x04000F72 RID: 3954
		public bool m_clockAllowed = true;

		// Token: 0x04000F73 RID: 3955
		public float m_voltage;
	}
}
