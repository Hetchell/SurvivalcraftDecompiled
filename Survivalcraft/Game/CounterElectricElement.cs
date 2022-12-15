using System;
using Engine;

namespace Game
{
	// Token: 0x02000243 RID: 579
	public class CounterElectricElement : RotateableElectricElement
	{
		// Token: 0x060011BC RID: 4540 RVA: 0x00089034 File Offset: 0x00087234
		public CounterElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			float? num = subsystemElectricity.ReadPersistentVoltage(cellFace.Point);
			if (num != null)
			{
				this.m_counter = (int)MathUtils.Round(MathUtils.Abs(num.Value) * 15f);
				this.m_overflow = (num.Value < 0f);
			}
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x000890A8 File Offset: 0x000872A8
		public override float GetOutputVoltage(int face)
		{
			ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, base.Rotation, face);
			if (connectorDirection != null)
			{
				if (connectorDirection.Value == ElectricConnectorDirection.Top)
				{
					return (float)this.m_counter / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
				{
					return (float)(this.m_overflow ? 1 : 0);
				}
			}
			return 0f;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00089114 File Offset: 0x00087314
		public override bool Simulate()
		{
			int counter = this.m_counter;
			bool overflow = this.m_overflow;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
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
							flag = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								flag2 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.In;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									flag3 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
								}
							}
						}
					}
				}
			}
			if (flag && this.m_plusAllowed)
			{
				this.m_plusAllowed = false;
				if (this.m_counter < 15)
				{
					this.m_counter++;
					this.m_overflow = false;
				}
				else
				{
					this.m_counter = 0;
					this.m_overflow = true;
				}
			}
			else if (flag2 && this.m_minusAllowed)
			{
				this.m_minusAllowed = false;
				if (this.m_counter > 0)
				{
					this.m_counter--;
					this.m_overflow = false;
				}
				else
				{
					this.m_counter = 15;
					this.m_overflow = true;
				}
			}
			else if (flag3 && this.m_resetAllowed)
			{
				this.m_counter = 0;
				this.m_overflow = false;
			}
			if (!flag)
			{
				this.m_plusAllowed = true;
			}
			if (!flag2)
			{
				this.m_minusAllowed = true;
			}
			if (!flag3)
			{
				this.m_resetAllowed = true;
			}
			if (this.m_counter != counter || this.m_overflow != overflow)
			{
				base.SubsystemElectricity.WritePersistentVoltage(base.CellFaces[0].Point, (float)this.m_counter / 15f * (float)((!this.m_overflow) ? 1 : -1));
				return true;
			}
			return false;
		}

		// Token: 0x04000BDA RID: 3034
		public bool m_plusAllowed = true;

		// Token: 0x04000BDB RID: 3035
		public bool m_minusAllowed = true;

		// Token: 0x04000BDC RID: 3036
		public bool m_resetAllowed = true;

		// Token: 0x04000BDD RID: 3037
		public int m_counter;

		// Token: 0x04000BDE RID: 3038
		public bool m_overflow;
	}
}
