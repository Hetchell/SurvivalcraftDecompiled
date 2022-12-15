using System;

namespace Game
{
	// Token: 0x0200028F RID: 655
	public class HygrometerElectricElement : ElectricElement
	{
		// Token: 0x0600132F RID: 4911 RVA: 0x00096B79 File Offset: 0x00094D79
		public HygrometerElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x00096B83 File Offset: 0x00094D83
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x00096B8C File Offset: 0x00094D8C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			CellFace cellFace = base.CellFaces[0];
			int humidity = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetHumidity(cellFace.X, cellFace.Z);
			this.m_voltage = (float)humidity / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000D42 RID: 3394
		public float m_voltage;
	}
}
