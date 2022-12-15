using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002F5 RID: 757
	public class SmokeTrailParticleSystem : ParticleSystem<SmokeTrailParticleSystem.Particle>, ITrailParticleSystem
	{
		// Token: 0x1700035B RID: 859
		// (get) Token: 0x0600157F RID: 5503 RVA: 0x000A41FA File Offset: 0x000A23FA
		// (set) Token: 0x06001580 RID: 5504 RVA: 0x000A4202 File Offset: 0x000A2402
		public Vector3 Position { get; set; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001581 RID: 5505 RVA: 0x000A420B File Offset: 0x000A240B
		// (set) Token: 0x06001582 RID: 5506 RVA: 0x000A4213 File Offset: 0x000A2413
		public bool IsStopped { get; set; }

		// Token: 0x06001583 RID: 5507 RVA: 0x000A421C File Offset: 0x000A241C
		public SmokeTrailParticleSystem(int particlesCount, float size, float maxDuration, Color color) : base(particlesCount)
		{
			this.m_size = size;
			this.m_maxDuration = maxDuration;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
			this.m_textureSlotMultiplier = this.m_random.Float(1.1f, 1.9f);
			this.m_textureSlotOffset = (float)((this.m_random.Float(0f, 1f) < 0.33f) ? 3 : 0);
			this.m_color = color;
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x000A42AC File Offset: 0x000A24AC
		public override bool Simulate(float dt)
		{
			this.m_duration += dt;
			if (this.m_duration > this.m_maxDuration)
			{
				this.IsStopped = true;
			}
			float num = MathUtils.Clamp(50f / this.m_size, 10f, 40f);
			this.m_toGenerate += num * dt;
			float s = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SmokeTrailParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= s;
						SmokeTrailParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + 10f * dt;
						particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration * this.m_textureSlotMultiplier + this.m_textureSlotOffset, 8f);
						particle.Size = new Vector2(this.m_size * (0.15f + 0.8f * particle.Time / particle.Duration));
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v = new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
					particle.Position = this.Position + 0.025f * v;
					particle.Color = this.m_color;
					particle.Velocity = 0.2f * v;
					particle.Time = 0f;
					particle.Size = new Vector2(0.15f * this.m_size);
					particle.Duration = (float)base.Particles.Length / num * this.m_random.Float(0.8f, 1.05f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x04000F42 RID: 3906
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F43 RID: 3907
		public float m_toGenerate;

		// Token: 0x04000F44 RID: 3908
		public float m_textureSlotMultiplier;

		// Token: 0x04000F45 RID: 3909
		public float m_textureSlotOffset;

		// Token: 0x04000F46 RID: 3910
		public float m_duration;

		// Token: 0x04000F47 RID: 3911
		public float m_size;

		// Token: 0x04000F48 RID: 3912
		public float m_maxDuration;

		// Token: 0x04000F49 RID: 3913
		public Color m_color;

		// Token: 0x020004E1 RID: 1249
		public class Particle : Game.Particle
		{
			// Token: 0x040017E0 RID: 6112
			public float Time;

			// Token: 0x040017E1 RID: 6113
			public float Duration;

			// Token: 0x040017E2 RID: 6114
			public Vector3 Velocity;
		}
	}
}
