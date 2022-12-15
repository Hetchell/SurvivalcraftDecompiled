using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000163 RID: 355
	public class SubsystemBombBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060006FE RID: 1790 RVA: 0x0002C40E File Offset: 0x0002A60E
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x0002C416 File Offset: 0x0002A616
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

        // Token: 0x06000700 RID: 1792 RVA: 0x0002C41C File Offset: 0x0002A61C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			foreach (Projectile projectile2 in this.m_subsystemProjectiles.Projectiles)
			{
				this.ScanProjectile(projectile2);
			}
			this.m_subsystemProjectiles.ProjectileAdded += delegate(Projectile projectile)
			{
				this.ScanProjectile(projectile);
			};
			this.m_subsystemProjectiles.ProjectileRemoved += delegate(Projectile projectile)
			{
				this.m_projectiles.Remove(projectile);
			};
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0002C508 File Offset: 0x0002A708
		public void ScanProjectile(Projectile projectile)
		{
			if (!this.m_projectiles.ContainsKey(projectile))
			{
				int num = Terrain.ExtractContents(projectile.Value);
				if (this.m_subsystemBlockBehaviors.GetBlockBehaviors(num).Contains(this))
				{
					this.m_projectiles.Add(projectile, true);
					projectile.ProjectileStoppedAction = ProjectileStoppedAction.DoNothing;
					Color color = (num == 228) ? new Color(255, 140, 192) : Color.White;
					this.m_subsystemProjectiles.AddTrail(projectile, new Vector3(0f, 0.25f, 0.1f), new SmokeTrailParticleSystem(20, 0.33f, float.MaxValue, color));
				}
			}
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0002C5B0 File Offset: 0x0002A7B0
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(0.1, 0.0))
			{
				foreach (Projectile projectile in this.m_projectiles.Keys)
				{
					if (this.m_subsystemGameInfo.TotalElapsedGameTime - projectile.CreationTime > 5.0)
					{
						this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y), Terrain.ToCell(projectile.Position.Z), projectile.Value);
						projectile.ToRemove = true;
					}
				}
			}
		}

		// Token: 0x040003E0 RID: 992
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040003E1 RID: 993
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003E2 RID: 994
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x040003E3 RID: 995
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x040003E4 RID: 996
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040003E5 RID: 997
		public Dictionary<Projectile, bool> m_projectiles = new Dictionary<Projectile, bool>();
	}
}
