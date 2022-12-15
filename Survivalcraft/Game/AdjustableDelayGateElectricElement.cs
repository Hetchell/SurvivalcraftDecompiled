using System;

namespace Game
{
	// Token: 0x02000213 RID: 531
	public class AdjustableDelayGateElectricElement : BaseDelayGateElectricElement
	{
		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06001075 RID: 4213 RVA: 0x0007DBB2 File Offset: 0x0007BDB2
		public override int DelaySteps
		{
			get
			{
				return this.m_delaySteps;
			}
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0007DBBC File Offset: 0x0007BDBC
		public AdjustableDelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			int data = Terrain.ExtractData(subsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			this.m_delaySteps = AdjustableDelayGateBlock.GetDelay(data);
		}

		// Token: 0x04000AC5 RID: 2757
		public int m_delaySteps;
	}
}
