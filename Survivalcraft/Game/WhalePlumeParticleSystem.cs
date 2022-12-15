using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000357 RID: 855
	public class WhalePlumeParticleSystem : ParticleSystem<WhalePlumeParticleSystem.Particle>
	{
		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06001816 RID: 6166 RVA: 0x000BE467 File Offset: 0x000BC667
		// (set) Token: 0x06001817 RID: 6167 RVA: 0x000BE46F File Offset: 0x000BC66F
		public bool IsStopped { get; set; }

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x000BE478 File Offset: 0x000BC678
		// (set) Token: 0x06001819 RID: 6169 RVA: 0x000BE480 File Offset: 0x000BC680
		public Vector3 Position { get; set; }

		// Token: 0x0600181A RID: 6170 RVA: 0x000BE489 File Offset: 0x000BC689
		public WhalePlumeParticleSystem(SubsystemTerrain terrain, float size, float duration) : base(100)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/WaterSplashParticle");
			base.TextureSlotsCount = 2;
			this.m_size = size;
			this.m_duration = duration;
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000BE4C4 File Offset: 0x000BC6C4
		public override bool Simulate(float dt)
		{
			this.m_time += dt;
			if (this.m_time < this.m_duration && !this.IsStopped)
			{
				this.m_toGenerate += 60f * dt;
			}
			else
			{
				this.m_toGenerate = 0f;
			}
			float s = MathUtils.Pow(0.001f, dt);
			float num = MathUtils.Lerp(4f, 10f, MathUtils.Saturate(2f * this.m_time / this.m_duration));
			Vector3 v = new Vector3(0f, 1f, 2f);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				WhalePlumeParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= s;
						particle.Velocity += v * dt;
						particle.TextureSlot = (int)MathUtils.Min(4f * particle.Time / particle.Duration * 1.2f, 3f);
						particle.Size = new Vector2(this.m_size) * MathUtils.Lerp(0.1f, 0.2f, particle.Time / particle.Duration);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v2 = 0.1f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(0f, 2f), this.m_random.Float(-1f, 1f));
					particle.Position = this.Position + v2;
					particle.Color = new Color(200, 220, 210);
					particle.Velocity = 1f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), num * this.m_random.Float(0.3f, 1f), this.m_random.Float(-1f, 1f));
					particle.Size = Vector2.Zero;
					particle.Time = 0f;
					particle.Duration = this.m_random.Float(1f, 3f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			return !flag && (this.m_time >= this.m_duration || this.IsStopped);
		}

		// Token: 0x04001103 RID: 4355
		public Game.Random m_random = new Game.Random();

		// Token: 0x04001104 RID: 4356
		public float m_time;

		// Token: 0x04001105 RID: 4357
		public float m_duration;

		// Token: 0x04001106 RID: 4358
		public float m_size;

		// Token: 0x04001107 RID: 4359
		public float m_toGenerate;

		// Token: 0x0200050A RID: 1290
		public class Particle : Game.Particle
		{
			// Token: 0x040018AC RID: 6316
			public Vector3 Velocity;

			// Token: 0x040018AD RID: 6317
			public float Time;

			// Token: 0x040018AE RID: 6318
			public float Duration;
		}
	}
}
