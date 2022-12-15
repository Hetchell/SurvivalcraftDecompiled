using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002F9 RID: 761
	public class SoundParticleSystem : ParticleSystem<SoundParticleSystem.Particle>
	{
		// Token: 0x0600159D RID: 5533 RVA: 0x000A5048 File Offset: 0x000A3248
		public SoundParticleSystem(SubsystemTerrain terrain, Vector3 position, Vector3 direction) : base(15)
		{
			this.m_position = position;
			this.m_direction = direction;
			base.Texture = ContentManager.Get<Texture2D>("Textures/SoundParticle");
			base.TextureSlotsCount = 2;
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000A5084 File Offset: 0x000A3284
		public void AddNote(Color color)
		{
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SoundParticleSystem.Particle particle = base.Particles[i];
				if (!base.Particles[i].IsActive)
				{
					particle.IsActive = true;
					particle.Position = this.m_position;
					particle.Color = Color.White;
					particle.Size = new Vector2(0.1f);
					particle.TimeToLive = this.m_random.Float(1f, 1.5f);
					particle.Velocity = 3f * (this.m_direction + this.m_random.Vector3(0.5f));
					particle.BaseColor = color;
					particle.TextureSlot = this.m_random.Int(0, base.TextureSlotsCount * base.TextureSlotsCount - 1);
					particle.BillboardingMode = ParticleBillboardingMode.Vertical;
					return;
				}
			}
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x000A5160 File Offset: 0x000A3360
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.02f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SoundParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						particle.Velocity += new Vector3(0f, 5f, 0f) * dt;
						particle.Velocity *= s;
						particle.Position += particle.Velocity * dt;
						particle.Color = particle.BaseColor * MathUtils.Saturate(2f * particle.TimeToLive);
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000F5C RID: 3932
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F5D RID: 3933
		public Vector3 m_position;

		// Token: 0x04000F5E RID: 3934
		public Vector3 m_direction;

		// Token: 0x020004E4 RID: 1252
		public class Particle : Game.Particle
		{
			// Token: 0x040017EA RID: 6122
			public float TimeToLive;

			// Token: 0x040017EB RID: 6123
			public Vector3 Velocity;

			// Token: 0x040017EC RID: 6124
			public Color BaseColor;
		}
	}
}
