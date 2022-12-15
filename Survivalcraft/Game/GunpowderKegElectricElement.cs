using System;

namespace Game
{
	// Token: 0x0200028B RID: 651
	public class GunpowderKegElectricElement : ElectricElement
	{
		// Token: 0x06001327 RID: 4903 RVA: 0x0009646C File Offset: 0x0009466C
		public GunpowderKegElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x00096478 File Offset: 0x00094678
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.Project.FindSubsystem<SubsystemExplosivesBlockBehavior>(true).IgniteFuse(cellFace.X, cellFace.Y, cellFace.Z);
			}
			return false;
		}
	}
}
