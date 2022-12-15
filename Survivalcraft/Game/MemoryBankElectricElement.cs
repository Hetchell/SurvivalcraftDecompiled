using System;
using Engine;

namespace Game
{
	// Token: 0x020002AC RID: 684
	public class MemoryBankElectricElement : RotateableElectricElement
	{
		// Token: 0x060013AC RID: 5036 RVA: 0x000989D4 File Offset: 0x00096BD4
		public MemoryBankElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemMemoryBankBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemMemoryBankBlockBehavior>(true);
			MemoryBankData blockData = this.m_subsystemMemoryBankBlockBehavior.GetBlockData(cellFace.Point);
			if (blockData != null)
			{
				this.m_voltage = (float)blockData.LastOutput / 15f;
			}
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x00098A24 File Offset: 0x00096C24
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x00098A2C File Offset: 0x00096C2C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
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
							num2 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								num3 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									int num4 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
									flag = (num4 >= 8);
									flag3 = (num4 > 0 && num4 < 8);
									flag2 = true;
								}
								else
								{
									electricConnectorDirection = connectorDirection;
									electricConnectorDirection2 = ElectricConnectorDirection.In;
									if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
									{
										num = electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace);
									}
								}
							}
						}
					}
				}
			}
			MemoryBankData memoryBankData = this.m_subsystemMemoryBankBlockBehavior.GetBlockData(base.CellFaces[0].Point);
			int address = num2 + (num3 << 4);
			if (flag2)
			{
				if (flag && this.m_clockAllowed)
				{
					this.m_clockAllowed = false;
					this.m_voltage = ((memoryBankData != null) ? ((float)memoryBankData.Read(address) / 15f) : 0f);
				}
				else if (flag3 && this.m_writeAllowed)
				{
					this.m_writeAllowed = false;
					if (memoryBankData == null)
					{
						memoryBankData = new MemoryBankData();
						this.m_subsystemMemoryBankBlockBehavior.SetBlockData(base.CellFaces[0].Point, memoryBankData);
					}
					memoryBankData.Write(address, (byte)MathUtils.Round(num * 15f));
				}
			}
			else
			{
				this.m_voltage = ((memoryBankData != null) ? ((float)memoryBankData.Read(address) / 15f) : 0f);
			}
			if (!flag)
			{
				this.m_clockAllowed = true;
			}
			if (!flag3)
			{
				this.m_writeAllowed = true;
			}
			if (memoryBankData != null)
			{
				memoryBankData.LastOutput = (byte)MathUtils.Round(this.m_voltage * 15f);
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000D79 RID: 3449
		public SubsystemMemoryBankBlockBehavior m_subsystemMemoryBankBlockBehavior;

		// Token: 0x04000D7A RID: 3450
		public float m_voltage;

		// Token: 0x04000D7B RID: 3451
		public bool m_writeAllowed;

		// Token: 0x04000D7C RID: 3452
		public bool m_clockAllowed;
	}
}
