using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000159 RID: 345
	public class SubsystemArrowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x0002AC63 File Offset: 0x00028E63
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0002AC6C File Offset: 0x00028E6C
		public override void OnFiredAsProjectile(Projectile projectile)
		{
			if (ArrowBlock.GetArrowType(Terrain.ExtractData(projectile.Value)) == ArrowBlock.ArrowType.FireArrow)
			{
				this.m_subsystemProjectiles.AddTrail(projectile, Vector3.Zero, new SmokeTrailParticleSystem(20, 0.5f, float.MaxValue, Color.White));
				projectile.ProjectileStoppedAction = ProjectileStoppedAction.Disappear;
				projectile.IsIncendiary = true;
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0002ACC4 File Offset: 0x00028EC4
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(worldItem.Value));
			if (worldItem.Velocity.Length() > 10f)
			{
				float num = 0.1f;
				if (arrowType == ArrowBlock.ArrowType.FireArrow)
				{
					num = 0.5f;
				}
				if (arrowType == ArrowBlock.ArrowType.WoodenArrow)
				{
					num = 0.2f;
				}
				if (arrowType == ArrowBlock.ArrowType.DiamondArrow)
				{
					num = 0f;
				}
				if (arrowType == ArrowBlock.ArrowType.IronBolt)
				{
					num = 0.05f;
				}
				if (arrowType == ArrowBlock.ArrowType.DiamondBolt)
				{
					num = 0f;
				}
				if (this.m_random.Float(0f, 1f) < num)
				{
					return true;
				}
			}
			return false;
		}

        // Token: 0x060006A0 RID: 1696 RVA: 0x0002AD46 File Offset: 0x00028F46
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
		}

		// Token: 0x040003BE RID: 958
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040003BF RID: 959
		public Game.Random m_random = new Game.Random();
	}
}
