using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002F0 RID: 752
	public class ShapeshiftParticleSystem : ParticleSystem<ShapeshiftParticleSystem.Particle>
	{
		// Token: 0x17000358 RID: 856
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x000A3488 File Offset: 0x000A1688
		// (set) Token: 0x0600156B RID: 5483 RVA: 0x000A3490 File Offset: 0x000A1690
		public bool Stopped { get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x000A3499 File Offset: 0x000A1699
		// (set) Token: 0x0600156D RID: 5485 RVA: 0x000A34A1 File Offset: 0x000A16A1
		public Vector3 Position { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x0600156E RID: 5486 RVA: 0x000A34AA File Offset: 0x000A16AA
		// (set) Token: 0x0600156F RID: 5487 RVA: 0x000A34B2 File Offset: 0x000A16B2
		public BoundingBox BoundingBox { get; set; }

		// Token: 0x06001570 RID: 5488 RVA: 0x000A34BB File Offset: 0x000A16BB
		public ShapeshiftParticleSystem() : base(40)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/ShapeshiftParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000A34E8 File Offset: 0x000A16E8
		public override bool Simulate(float dt)
		{
			bool flag = false;
			this.m_generationSpeed = MathUtils.Min(this.m_generationSpeed + 15f * dt, 35f);
			this.m_toGenerate += this.m_generationSpeed * dt;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				ShapeshiftParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.FlipX = this.m_random.Bool();
						particle.FlipY = this.m_random.Bool();
						particle.TextureSlot = (int)MathUtils.Min(9.900001f * particle.Time / particle.Duration, 8f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.Stopped)
				{
					while (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position.X = this.m_random.Float(this.BoundingBox.Min.X, this.BoundingBox.Max.X);
						particle.Position.Y = this.m_random.Float(this.BoundingBox.Min.Y, this.BoundingBox.Max.Y);
						particle.Position.Z = this.m_random.Float(this.BoundingBox.Min.Z, this.BoundingBox.Max.Z);
						particle.Velocity = new Vector3(0f, this.m_random.Float(0.5f, 1.5f), 0f);
						particle.Color = Color.White;
						particle.Size = new Vector2(0.4f);
						particle.Time = 0f;
						particle.Duration = this.m_random.Float(0.75f, 1.5f);
						this.m_toGenerate -= 1f;
					}
				}
				else
				{
					this.m_toGenerate = 0f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			return this.Stopped && !flag;
		}

		// Token: 0x04000F31 RID: 3889
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F32 RID: 3890
		public float m_generationSpeed;

		// Token: 0x04000F33 RID: 3891
		public float m_toGenerate;

		// Token: 0x020004E0 RID: 1248
		public class Particle : Game.Particle
		{
			// Token: 0x040017DD RID: 6109
			public float Time;

			// Token: 0x040017DE RID: 6110
			public float Duration;

			// Token: 0x040017DF RID: 6111
			public Vector3 Velocity;
		}
	}
}
