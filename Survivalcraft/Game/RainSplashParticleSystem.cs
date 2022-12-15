using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002DE RID: 734
	public class RainSplashParticleSystem : ParticleSystem<RainSplashParticleSystem.Particle>
	{
		// Token: 0x060014B1 RID: 5297 RVA: 0x000A0B2D File Offset: 0x0009ED2D
		public RainSplashParticleSystem() : base(150)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/RainSplashParticle");
			base.TextureSlotsCount = 1;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x000A0B5C File Offset: 0x0009ED5C
		public void AddSplash(int value, Vector3 position, Color color)
		{
			int i = 0;
			while (i < base.Particles.Length)
			{
				RainSplashParticleSystem.Particle particle = base.Particles[i];
				if (!particle.IsActive)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					particle.IsActive = true;
					particle.Position = position;
					particle.BaseColor = color;
					if (block is WaterBlock)
					{
						RainSplashParticleSystem.Particle particle2 = particle;
						particle2.Position.Y = particle2.Position.Y + 0.05f;
						particle.BaseSize1 = 0.02f;
						particle.BaseSize2 = 0.09f;
						particle.Duration = (particle.TimeToLive = this.m_random.Float(0.3f, 0.5f));
						particle.Velocity = Vector3.Zero;
						particle.Gravity = 0f;
						particle.BillboardingMode = ParticleBillboardingMode.Horizontal;
						particle.FadeFactor = 1.6f;
						break;
					}
					if (block.IsCollidable)
					{
						particle.BaseSize1 = 0.03f;
						particle.BaseSize2 = 0.08f;
						particle.Duration = (particle.TimeToLive = this.m_random.Float(0.25f, 0.3f));
						particle.Velocity = this.m_random.Float(0.7f, 0.9f) * Vector3.UnitY;
						particle.Gravity = -10f;
						particle.BillboardingMode = ParticleBillboardingMode.Camera;
						particle.FadeFactor = 2.8f;
						break;
					}
					if (this.m_random.Bool(0.33f))
					{
						particle.BaseSize1 = this.m_random.Float(0.015f, 0.025f);
						particle.BaseSize2 = particle.BaseSize1;
						particle.Duration = (particle.TimeToLive = this.m_random.Float(0.25f, 0.3f));
						particle.Velocity = this.m_random.Vector3(0f, 1.5f) * new Vector3(1f, 0f, 1f);
						particle.Gravity = -10f;
						particle.BillboardingMode = ParticleBillboardingMode.Camera;
						particle.FadeFactor = 2.8f;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.m_isActive = true;
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x000A0D84 File Offset: 0x0009EF84
		public override bool Simulate(float dt)
		{
			if (this.m_isActive)
			{
				dt = MathUtils.Clamp(dt, 0f, 0.1f);
				float s = MathUtils.Pow(0.0005f, dt);
				bool flag = false;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					RainSplashParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						particle.Position += particle.Velocity * dt;
						RainSplashParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + particle.Gravity * dt;
						particle.Velocity *= s;
						particle.Size = new Vector2(MathUtils.Lerp(particle.BaseSize1, particle.BaseSize2, (particle.Duration - particle.TimeToLive) / particle.Duration));
						particle.Color = particle.BaseColor * MathUtils.Saturate(particle.FadeFactor * particle.TimeToLive);
						particle.TimeToLive -= dt;
						particle.FlipX = this.m_random.Bool();
						particle.FlipY = this.m_random.Bool();
						if (particle.TimeToLive <= 0f)
						{
							particle.IsActive = false;
						}
						else
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					this.m_isActive = false;
				}
			}
			return false;
		}

		// Token: 0x04000EBF RID: 3775
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000EC0 RID: 3776
		public bool m_isActive;

		// Token: 0x020004D5 RID: 1237
		public class Particle : Game.Particle
		{
			// Token: 0x040017C4 RID: 6084
			public Vector3 Velocity;

			// Token: 0x040017C5 RID: 6085
			public float Duration;

			// Token: 0x040017C6 RID: 6086
			public float TimeToLive;

			// Token: 0x040017C7 RID: 6087
			public Color BaseColor;

			// Token: 0x040017C8 RID: 6088
			public float BaseSize1;

			// Token: 0x040017C9 RID: 6089
			public float BaseSize2;

			// Token: 0x040017CA RID: 6090
			public float Gravity;

			// Token: 0x040017CB RID: 6091
			public float FadeFactor;
		}
	}
}
