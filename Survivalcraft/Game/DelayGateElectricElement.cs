using System;

namespace Game
{
	// Token: 0x0200024D RID: 589
	public class DelayGateElectricElement : BaseDelayGateElectricElement
	{
		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060011D4 RID: 4564 RVA: 0x00089F90 File Offset: 0x00088190
		public override int DelaySteps
		{
			get
			{
				if (base.SubsystemElectricity.CircuitStep - this.m_lastDelayCalculationStep > 50)
				{
					this.m_delaySteps = null;
				}
				if (this.m_delaySteps == null)
				{
					int num = 0;
					DelayGateElectricElement.CountDelayPredecessors(this, ref num);
					this.m_delaySteps = new int?(DelayGateElectricElement.m_delaysByPredecessorsCount[num]);
					this.m_lastDelayCalculationStep = base.SubsystemElectricity.CircuitStep;
				}
				return this.m_delaySteps.Value;
			}
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0008A004 File Offset: 0x00088204
		public DelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0008A010 File Offset: 0x00088210
		public static void CountDelayPredecessors(DelayGateElectricElement delayGate, ref int count)
		{
			if (count < 2)
			{
				foreach (ElectricConnection electricConnection in delayGate.Connections)
				{
					if (electricConnection.ConnectorType == ElectricConnectorType.Input)
					{
						DelayGateElectricElement delayGateElectricElement = electricConnection.NeighborElectricElement as DelayGateElectricElement;
						if (delayGateElectricElement != null)
						{
							count++;
							DelayGateElectricElement.CountDelayPredecessors(delayGateElectricElement, ref count);
							break;
						}
					}
				}
			}
		}

		// Token: 0x04000C01 RID: 3073
		public int? m_delaySteps;

		// Token: 0x04000C02 RID: 3074
		public int m_lastDelayCalculationStep;

		// Token: 0x04000C03 RID: 3075
		public static int[] m_delaysByPredecessorsCount = new int[]
		{
			20,
			80,
			400
		};
	}
}
