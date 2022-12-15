using System;
using System.Collections.Generic;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000197 RID: 407
	public class SubsystemParticles : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x00042638 File Offset: 0x00040838
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000980 RID: 2432 RVA: 0x0004263B File Offset: 0x0004083B
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00042643 File Offset: 0x00040843
		public void AddParticleSystem(ParticleSystemBase particleSystem)
		{
			if (particleSystem.SubsystemParticles == null)
			{
				this.m_particleSystems.Add(particleSystem, true);
				particleSystem.SubsystemParticles = this;
				particleSystem.OnAdded();
				return;
			}
			throw new InvalidOperationException("Particle system is already added.");
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00042672 File Offset: 0x00040872
		public void RemoveParticleSystem(ParticleSystemBase particleSystem)
		{
			if (particleSystem.SubsystemParticles == this)
			{
				particleSystem.OnRemoved();
				this.m_particleSystems.Remove(particleSystem);
				particleSystem.SubsystemParticles = null;
				return;
			}
			throw new InvalidOperationException("Particle system is not added.");
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x000426A2 File Offset: 0x000408A2
		public bool ContainsParticleSystem(ParticleSystemBase particleSystem)
		{
			return particleSystem.SubsystemParticles == this;
		}

        // Token: 0x06000984 RID: 2436 RVA: 0x000426AD File Offset: 0x000408AD
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x000426C4 File Offset: 0x000408C4
		public void Update(float dt)
		{
			if (this.ParticleSystemsSimulate)
			{
				this.m_endedParticleSystems.Clear();
				foreach (ParticleSystemBase particleSystemBase in this.m_particleSystems.Keys)
				{
					if (particleSystemBase.Simulate(this.m_subsystemTime.GameTimeDelta))
					{
						this.m_endedParticleSystems.Add(particleSystemBase);
					}
				}
				foreach (ParticleSystemBase particleSystem in this.m_endedParticleSystems)
				{
					this.RemoveParticleSystem(particleSystem);
				}
			}
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0004278C File Offset: 0x0004098C
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.ParticleSystemsDraw)
			{
				foreach (ParticleSystemBase particleSystemBase in this.m_particleSystems.Keys)
				{
					particleSystemBase.Draw(camera);
				}
				this.PrimitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

		// Token: 0x040004FF RID: 1279
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000500 RID: 1280
		public Dictionary<ParticleSystemBase, bool> m_particleSystems = new Dictionary<ParticleSystemBase, bool>();

		// Token: 0x04000501 RID: 1281
		public PrimitivesRenderer3D PrimitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000502 RID: 1282
		public bool ParticleSystemsDraw = true;

		// Token: 0x04000503 RID: 1283
		public bool ParticleSystemsSimulate = true;

		// Token: 0x04000504 RID: 1284
		public int[] m_drawOrders = new int[]
		{
			300
		};

		// Token: 0x04000505 RID: 1285
		public List<ParticleSystemBase> m_endedParticleSystems = new List<ParticleSystemBase>();
	}
}
