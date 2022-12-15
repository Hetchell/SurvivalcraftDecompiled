using System;
using Engine;

namespace Game
{
	// Token: 0x02000321 RID: 801
	public class ThermometerElectricElement : ElectricElement
	{
		// Token: 0x06001706 RID: 5894 RVA: 0x000B828D File Offset: 0x000B648D
		public ThermometerElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemMetersBlockBehavior = base.SubsystemElectricity.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x000B82AE File Offset: 0x000B64AE
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x000B82B8 File Offset: 0x000B64B8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			CellFace cellFace = base.CellFaces[0];
			this.m_voltage = MathUtils.Saturate((float)this.m_subsystemMetersBlockBehavior.GetThermometerReading(cellFace.X, cellFace.Y, cellFace.Z) / 15f);
			float num = 0.5f * (0.9f + 0.00020000001f * (float)(this.GetHashCode() % 1000));
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(num / 0.01f), 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x040010B0 RID: 4272
		public SubsystemMetersBlockBehavior m_subsystemMetersBlockBehavior;

		// Token: 0x040010B1 RID: 4273
		public float m_voltage;

		// Token: 0x040010B2 RID: 4274
		public const float m_pollingPeriod = 0.5f;
	}
}
