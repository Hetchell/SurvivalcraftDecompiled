using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200028C RID: 652
	public class GunSmokeParticleSystem : ParticleSystem<GunSmokeParticleSystem.Particle>
	{
		// Token: 0x06001329 RID: 4905 RVA: 0x000964C8 File Offset: 0x000946C8
		public GunSmokeParticleSystem(SubsystemTerrain terrain, Vector3 position, Vector3 direction) : base(50)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/GunSmokeParticle");
			base.TextureSlotsCount = 3;
			this.m_position = position;
			this.m_direction = Vector3.Normalize(direction);
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
			float num5 = LightingManager.LightIntensityByLightValue[num4];
			this.m_color = new Color(num5, num5, num5);
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x000965DC File Offset: 0x000947DC
		public override bool Simulate(float dt)
		{
			this.m_time += dt;
			float num = MathUtils.Lerp(150f, 20f, MathUtils.Saturate(2f * this.m_time / 0.5f));
			float s = MathUtils.Pow(0.01f, dt);
			float s2 = MathUtils.Lerp(20f, 0f, MathUtils.Saturate(2f * this.m_time / 0.5f));
			Vector3 v = new Vector3(2f, 2f, 1f);
			if (this.m_time < 0.5f)
			{
				this.m_toGenerate += num * dt;
			}
			else
			{
				this.m_toGenerate = 0f;
			}
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				GunSmokeParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= s;
						particle.Velocity += v * dt;
						particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration, 8f);
						particle.Size = new Vector2(0.3f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v2 = this.m_random.Vector3(0f, 1f);
					particle.Position = this.m_position + 0.3f * v2;
					particle.Color = this.m_color;
					particle.Velocity = s2 * (this.m_direction + this.m_random.Vector3(0f, 0.1f)) + 2.5f * v2;
					particle.Size = Vector2.Zero;
					particle.Time = 0f;
					particle.Duration = this.m_random.Float(0.5f, 2f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			return !flag && this.m_time >= 0.5f;
		}

		// Token: 0x04000D38 RID: 3384
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D39 RID: 3385
		public float m_time;

		// Token: 0x04000D3A RID: 3386
		public float m_toGenerate;

		// Token: 0x04000D3B RID: 3387
		public Vector3 m_position;

		// Token: 0x04000D3C RID: 3388
		public Vector3 m_direction;

		// Token: 0x04000D3D RID: 3389
		public Color m_color;

		// Token: 0x020004AB RID: 1195
		public class Particle : Game.Particle
		{
			// Token: 0x0400174E RID: 5966
			public Vector3 Velocity;

			// Token: 0x0400174F RID: 5967
			public float Time;

			// Token: 0x04001750 RID: 5968
			public float Duration;
		}
	}
}
