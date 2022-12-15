using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002BD RID: 701
	public class OnFireParticleSystem : ParticleSystem<OnFireParticleSystem.Particle>
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0009A81C File Offset: 0x00098A1C
		// (set) Token: 0x060013F6 RID: 5110 RVA: 0x0009A824 File Offset: 0x00098A24
		public bool IsStopped { get; set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0009A82D File Offset: 0x00098A2D
		// (set) Token: 0x060013F8 RID: 5112 RVA: 0x0009A835 File Offset: 0x00098A35
		public Vector3 Position { get; set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0009A83E File Offset: 0x00098A3E
		// (set) Token: 0x060013FA RID: 5114 RVA: 0x0009A846 File Offset: 0x00098A46
		public float Radius { get; set; }

		// Token: 0x060013FB RID: 5115 RVA: 0x0009A84F File Offset: 0x00098A4F
		public OnFireParticleSystem() : base(25)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x0009A87C File Offset: 0x00098A7C
		public override bool Simulate(float dt)
		{
			bool flag = false;
			if (this.m_visible)
			{
				this.m_toGenerate += 20f * dt;
				float s = MathUtils.Pow(0.02f, dt);
				for (int i = 0; i < base.Particles.Length; i++)
				{
					OnFireParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						if (particle.Time <= particle.Duration)
						{
							particle.Position += particle.Velocity * dt;
							particle.Velocity *= s;
							OnFireParticleSystem.Particle particle2 = particle;
							particle2.Velocity.Y = particle2.Velocity.Y + 10f * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration * 1.2f, 8f);
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (!this.IsStopped)
					{
						if (this.m_toGenerate >= 1f)
						{
							particle.IsActive = true;
							Vector3 v = new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(0f, 1f), this.m_random.Float(-1f, 1f));
							particle.Position = this.Position + 0.75f * this.Radius * v;
							particle.Color = Color.White;
							particle.Velocity = 1.5f * v;
							particle.Size = new Vector2(0.5f);
							particle.Time = 0f;
							particle.Duration = this.m_random.Float(0.5f, 1.5f);
							particle.FlipX = this.m_random.Bool();
							particle.FlipY = this.m_random.Bool();
							this.m_toGenerate -= 1f;
						}
					}
					else
					{
						this.m_toGenerate = 0f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
				this.m_visible = false;
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x0009AACC File Offset: 0x00098CCC
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.Position - camera.ViewPosition, camera.ViewDirection);
			if (num > -5f && num <= 48f)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000DC6 RID: 3526
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000DC7 RID: 3527
		public float m_toGenerate;

		// Token: 0x04000DC8 RID: 3528
		public bool m_visible;

		// Token: 0x020004BB RID: 1211
		public class Particle : Game.Particle
		{
			// Token: 0x0400177E RID: 6014
			public float Time;

			// Token: 0x0400177F RID: 6015
			public float Duration;

			// Token: 0x04001780 RID: 6016
			public Vector3 Velocity;
		}
	}
}
