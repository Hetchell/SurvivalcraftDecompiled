using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000187 RID: 391
	public class SubsystemImpactExplosivesBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x0003DA96 File Offset: 0x0003BC96
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x0003DAA0 File Offset: 0x0003BCA0
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			return this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(worldItem.Position.X), Terrain.ToCell(worldItem.Position.Y), Terrain.ToCell(worldItem.Position.Z), worldItem.Value);
		}

        // Token: 0x060008F4 RID: 2292 RVA: 0x0003DAEE File Offset: 0x0003BCEE
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
		}

		// Token: 0x040004B2 RID: 1202
		public SubsystemExplosions m_subsystemExplosions;
	}
}
