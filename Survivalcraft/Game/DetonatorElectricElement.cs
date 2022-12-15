using System;

namespace Game
{
	// Token: 0x0200024E RID: 590
	public class DetonatorElectricElement : MountedElectricElement
	{
		// Token: 0x060011D8 RID: 4568 RVA: 0x0008A0A0 File Offset: 0x000882A0
		public DetonatorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x0008A0AC File Offset: 0x000882AC
		public void Detonate()
		{
			CellFace cellFace = base.CellFaces[0];
			int value = Terrain.MakeBlockValue(147);
			base.SubsystemElectricity.Project.FindSubsystem<SubsystemExplosions>(true).TryExplodeBlock(cellFace.X, cellFace.Y, cellFace.Z, value);
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x0008A0FE File Offset: 0x000882FE
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				this.Detonate();
			}
			return false;
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0008A110 File Offset: 0x00088310
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Detonate();
		}
	}
}
