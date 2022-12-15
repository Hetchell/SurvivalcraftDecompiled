using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200026B RID: 619
	public class ExplosionParticleSystem : ParticleSystem<ExplosionParticleSystem.Particle>
	{
		// Token: 0x06001266 RID: 4710 RVA: 0x0008DFF0 File Offset: 0x0008C1F0
		public ExplosionParticleSystem() : base(1000)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
			this.m_inactiveParticles.AddRange(base.Particles);
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x0008E054 File Offset: 0x0008C254
		public void SetExplosionCell(Point3 point, float strength)
		{
			ExplosionParticleSystem.Particle particle;
			if (!this.m_particlesByPoint.TryGetValue(point, out particle))
			{
				if (this.m_inactiveParticles.Count > 0)
				{
					particle = this.m_inactiveParticles[this.m_inactiveParticles.Count - 1];
					this.m_inactiveParticles.RemoveAt(this.m_inactiveParticles.Count - 1);
				}
				else
				{
					for (int i = 0; i < 5; i++)
					{
						int num = this.m_random.Int(0, base.Particles.Length - 1);
						if (strength > base.Particles[num].Strength)
						{
							particle = base.Particles[num];
						}
					}
				}
				if (particle != null)
				{
					this.m_particlesByPoint.Add(point, particle);
				}
			}
			if (particle != null)
			{
				particle.IsActive = true;
				particle.Position = new Vector3((float)point.X, (float)point.Y, (float)point.Z) + new Vector3(0.5f) + 0.2f * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Size = new Vector2(this.m_random.Float(0.6f, 0.9f));
				particle.Strength = strength;
				particle.Color = Color.White;
				this.m_isEmpty = false;
			}
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x0008E1CC File Offset: 0x0008C3CC
		public override bool Simulate(float dt)
		{
			if (!this.m_isEmpty)
			{
				this.m_isEmpty = true;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					ExplosionParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						this.m_isEmpty = false;
						particle.Strength -= dt / 1.5f;
						if (particle.Strength > 0f)
						{
							particle.TextureSlot = (int)MathUtils.Min(9f * (1f - particle.Strength) * 0.6f, 8f);
							ExplosionParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + 2f * MathUtils.Max(1f - particle.Strength - 0.25f, 0f) * dt;
						}
						else
						{
							particle.IsActive = false;
							this.m_inactiveParticles.Add(particle);
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x0008E2B2 File Offset: 0x0008C4B2
		public override void Draw(Camera camera)
		{
			if (!this.m_isEmpty)
			{
				base.Draw(camera);
			}
		}

		// Token: 0x04000C9C RID: 3228
		public Dictionary<Point3, ExplosionParticleSystem.Particle> m_particlesByPoint = new Dictionary<Point3, ExplosionParticleSystem.Particle>();

		// Token: 0x04000C9D RID: 3229
		public List<ExplosionParticleSystem.Particle> m_inactiveParticles = new List<ExplosionParticleSystem.Particle>();

		// Token: 0x04000C9E RID: 3230
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000C9F RID: 3231
		public const float m_duration = 1.5f;

		// Token: 0x04000CA0 RID: 3232
		public bool m_isEmpty;

		// Token: 0x02000493 RID: 1171
		public class Particle : Game.Particle
		{
			// Token: 0x040016F5 RID: 5877
			public float Strength;
		}
	}
}
