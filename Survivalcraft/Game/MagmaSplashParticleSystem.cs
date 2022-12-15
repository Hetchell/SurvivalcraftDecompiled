using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002A8 RID: 680
	public class MagmaSplashParticleSystem : ParticleSystem<MagmaSplashParticleSystem.Particle>
	{
		// Token: 0x0600139A RID: 5018 RVA: 0x00098154 File Offset: 0x00096354
		public MagmaSplashParticleSystem(SubsystemTerrain terrain, Vector3 position, bool large) : base(40)
		{
			this.m_subsystemTerrain = terrain;
			this.m_position = position;
			base.Texture = ContentManager.Get<Texture2D>("Textures/MagmaSplashParticle");
			base.TextureSlotsCount = 2;
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			int num4 = 0;
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num + 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num - 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 + 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 - 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 + 1));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 - 1));
			Color color = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			color *= s;
			color.A = byte.MaxValue;
			float num5 = large ? 1.5f : 1f;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				MagmaSplashParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				particle.Position = position;
				particle.Color = color;
				particle.Size = new Vector2(0.2f * num5);
				particle.TimeToLive = (particle.Duration = this.m_random.Float(0.5f, 3.5f));
				Vector3 v = 4f * this.m_random.Float(0.1f, 1f) * Vector3.Normalize(new Vector3(this.m_random.Float(-1f, 1f), 0f, this.m_random.Float(-1f, 1f)));
				particle.Velocity = num5 * (v + new Vector3(0f, this.m_random.Float(0f, 4f), 0f));
			}
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x00098388 File Offset: 0x00096588
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.015f, dt);
			this.m_time += dt;
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				MagmaSplashParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Position += particle.Velocity * dt;
					MagmaSplashParticleSystem.Particle particle2 = particle;
					particle2.Velocity.Y = particle2.Velocity.Y + -10f * dt;
					particle.Velocity *= s;
					particle.Color *= MathUtils.Saturate(particle.TimeToLive);
					particle.TimeToLive -= dt;
					particle.TextureSlot = (int)(3.99f * particle.TimeToLive / particle.Duration);
					particle.FlipX = (this.m_random.Sign() > 0);
					particle.FlipY = (this.m_random.Sign() > 0);
					if (particle.TimeToLive <= 0f || particle.Size.X <= 0f)
					{
						particle.IsActive = false;
					}
					else
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(particle.Position.X), Terrain.ToCell(particle.Position.Y), Terrain.ToCell(particle.Position.Z));
						int num = Terrain.ExtractContents(cellValue);
						if (num != 0)
						{
							Block block = BlocksManager.Blocks[num];
							if (block.IsCollidable)
							{
								particle.IsActive = true;
							}
							else if (block is MagmaBlock)
							{
								int level = FluidBlock.GetLevel(Terrain.ExtractData(cellValue));
								float levelHeight = ((MagmaBlock)block).GetLevelHeight(level);
								if (particle.Position.Y <= MathUtils.Floor(particle.Position.Y) + levelHeight)
								{
									particle.Velocity.Y = 0f;
									float num2 = Vector2.Distance(new Vector2(particle.Position.X, particle.Position.Z), new Vector2(this.m_position.X, this.m_position.Z));
									float num3 = 0.02f * MathUtils.Sin(2f * num2 + 10f * this.m_time);
									particle.Position.Y = MathUtils.Floor(particle.Position.Y) + levelHeight + num3;
									particle.TimeToLive -= 1f * dt;
									particle.Size -= new Vector2(0.04f * dt);
								}
							}
						}
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000D70 RID: 3440
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D71 RID: 3441
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000D72 RID: 3442
		public Vector3 m_position;

		// Token: 0x04000D73 RID: 3443
		public float m_time;

		// Token: 0x020004B3 RID: 1203
		public class Particle : Game.Particle
		{
			// Token: 0x0400176C RID: 5996
			public Vector3 Velocity;

			// Token: 0x0400176D RID: 5997
			public float TimeToLive;

			// Token: 0x0400176E RID: 5998
			public float Duration;
		}
	}
}
