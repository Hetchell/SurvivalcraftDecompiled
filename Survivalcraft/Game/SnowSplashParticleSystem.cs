using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002F6 RID: 758
	public class SnowSplashParticleSystem : ParticleSystem<SnowSplashParticleSystem.Particle>
	{
		// Token: 0x06001585 RID: 5509 RVA: 0x000A455E File Offset: 0x000A275E
		public SnowSplashParticleSystem() : base(100)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/SnowParticle");
			base.TextureSlotsCount = 4;
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x000A458C File Offset: 0x000A278C
		public void AddSplash(int value, Vector3 position, Vector2 size, Color color, int textureSlot)
		{
			int i = 0;
			while (i < base.Particles.Length)
			{
				SnowSplashParticleSystem.Particle particle = base.Particles[i];
				if (!particle.IsActive)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					particle.IsActive = true;
					particle.Position = position;
					particle.BaseColor = color;
					particle.BillboardingMode = ParticleBillboardingMode.Horizontal;
					particle.Size = size;
					particle.TextureSlot = textureSlot;
					if (block is WaterBlock)
					{
						((WaterBlock)block).GetLevelHeight(FluidBlock.GetLevel(Terrain.ExtractData(value)));
						particle.TimeToLive = this.m_random.Float(0.2f, 0.3f);
						particle.FadeFactor = 1f;
						break;
					}
					if (block.IsCollidable || block is SnowBlock)
					{
						particle.TimeToLive = this.m_random.Float(0.8f, 1.2f);
						particle.FadeFactor = 1f;
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

		// Token: 0x06001587 RID: 5511 RVA: 0x000A4688 File Offset: 0x000A2888
		public override bool Simulate(float dt)
		{
			if (this.m_isActive)
			{
				dt = MathUtils.Clamp(dt, 0f, 0.1f);
				bool flag = false;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					SnowSplashParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						particle.Color = particle.BaseColor * MathUtils.Saturate(particle.FadeFactor * particle.TimeToLive);
						particle.TimeToLive -= dt;
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

		// Token: 0x04000F4C RID: 3916
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F4D RID: 3917
		public bool m_isActive;

		// Token: 0x020004E2 RID: 1250
		public class Particle : Game.Particle
		{
			// Token: 0x040017E3 RID: 6115
			public float TimeToLive;

			// Token: 0x040017E4 RID: 6116
			public Color BaseColor;

			// Token: 0x040017E5 RID: 6117
			public float FadeFactor;
		}
	}
}
