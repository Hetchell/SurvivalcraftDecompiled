using System;
using Engine;

namespace Game
{
	// Token: 0x020002E4 RID: 740
	public class RealTimeClockElectricElement : RotateableElectricElement
	{
		// Token: 0x060014D8 RID: 5336 RVA: 0x000A1824 File Offset: 0x0009FA24
		public RealTimeClockElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemTimeOfDay = base.SubsystemElectricity.Project.FindSubsystem<SubsystemTimeOfDay>(true);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x000A184C File Offset: 0x0009FA4C
		public override float GetOutputVoltage(int face)
		{
			ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, base.Rotation, face);
			if (connectorDirection != null)
			{
				if (connectorDirection.Value == ElectricConnectorDirection.Top)
				{
					return (float)(this.GetClockValue() & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Right)
				{
					return (float)(this.GetClockValue() >> 4 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
				{
					return (float)(this.GetClockValue() >> 8 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Left)
				{
					return (float)(this.GetClockValue() >> 12 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.In)
				{
					return (float)(this.GetClockValue() >> 16 & 15) / 15f;
				}
			}
			return 0f;
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x000A191C File Offset: 0x0009FB1C
		public override bool Simulate()
		{
			double day = this.m_subsystemTimeOfDay.Day;
			int num = (int)(((MathUtils.Ceiling(day * 4096.0) + 0.5) / 4096.0 - day) * 1200.0 / 0.009999999776482582);
			int circuitStep = MathUtils.Max(base.SubsystemElectricity.FrameStartCircuitStep + num, base.SubsystemElectricity.CircuitStep + 1);
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, circuitStep);
			int clockValue = this.GetClockValue();
			if (clockValue != this.m_lastClockValue)
			{
				this.m_lastClockValue = clockValue;
				return true;
			}
			return false;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x000A19B8 File Offset: 0x0009FBB8
		public int GetClockValue()
		{
			return (int)(this.m_subsystemTimeOfDay.Day * 4096.0);
		}

		// Token: 0x04000ED6 RID: 3798
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000ED7 RID: 3799
		public int m_lastClockValue = -1;

		// Token: 0x04000ED8 RID: 3800
		public const int m_periodsPerDay = 4096;
	}
}
