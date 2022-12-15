using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200021E RID: 542
	public abstract class BaseDelayGateElectricElement : RotateableElectricElement
	{
		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600109D RID: 4253
		public abstract int DelaySteps { get; }

		// Token: 0x0600109E RID: 4254 RVA: 0x0007E711 File Offset: 0x0007C911
		public BaseDelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x0007E726 File Offset: 0x0007C926
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0007E730 File Offset: 0x0007C930
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int delaySteps = this.DelaySteps;
			float num = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace);
					break;
				}
			}
			if (delaySteps > 0)
			{
				float voltage2;
				if (this.m_voltagesHistory.TryGetValue(base.SubsystemElectricity.CircuitStep, out voltage2))
				{
					this.m_voltage = voltage2;
					this.m_voltagesHistory.Remove(base.SubsystemElectricity.CircuitStep);
				}
				if (num != this.m_lastStoredVoltage)
				{
					this.m_lastStoredVoltage = num;
					if (this.m_voltagesHistory.Count < 300)
					{
						this.m_voltagesHistory[base.SubsystemElectricity.CircuitStep + this.DelaySteps] = num;
						base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + this.DelaySteps);
					}
				}
			}
			else
			{
				this.m_voltage = num;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000AD7 RID: 2775
		public float m_voltage;

		// Token: 0x04000AD8 RID: 2776
		public float m_lastStoredVoltage;

		// Token: 0x04000AD9 RID: 2777
		public Dictionary<int, float> m_voltagesHistory = new Dictionary<int, float>();
	}
}
