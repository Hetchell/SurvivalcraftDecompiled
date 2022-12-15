using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000272 RID: 626
	public class FireworksParticleSystem : ParticleSystem<FireworksParticleSystem.Particle>
	{
		// Token: 0x06001284 RID: 4740 RVA: 0x0008ECB8 File Offset: 0x0008CEB8
		public FireworksParticleSystem(Vector3 position, Color color, FireworksBlock.Shape shape, float flickering, float particleSize) : base(300)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireworksParticle");
			this.m_color = color;
			this.m_flickering = flickering;
			base.TextureSlotsCount = 2;
			if (shape == FireworksBlock.Shape.SmallBurst || shape == FireworksBlock.Shape.LargeBurst)
			{
				int num = (shape == FireworksBlock.Shape.SmallBurst) ? 100 : 200;
				while (this.m_nextParticle < num)
				{
					FireworksParticleSystem.Particle[] particles = base.Particles;
					int nextParticle = this.m_nextParticle;
					this.m_nextParticle = nextParticle + 1;
					FireworksParticleSystem.Particle particle = particles[nextParticle];
					particle.IsActive = true;
					particle.Position = position;
					particle.Size = new Vector2(0.2f * particleSize);
					particle.TimeToLive = ((shape == FireworksBlock.Shape.SmallBurst) ? this.m_random.Float(0.5f, 2f) : this.m_random.Float(1f, 3f));
					particle.Velocity = this.m_random.Vector3(0.5f, 1f);
					particle.Velocity *= (float)((shape == FireworksBlock.Shape.SmallBurst) ? 16 : 26) * particle.Velocity.LengthSquared();
					particle.TextureSlot = this.m_random.Int(0, 3);
					particle.FadeRate = this.m_random.Float(1f, 3f);
					particle.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
					particle.RotationSpeed = 0f;
				}
			}
			switch (shape)
			{
			case FireworksBlock.Shape.Circle:
			{
				float num2 = this.m_random.Float(0f, 6.2831855f);
				int num3 = 150;
				for (int i = 0; i < num3; i++)
				{
					float x = 6.2831855f * (float)i / (float)num3 + num2;
					Vector3 v = new Vector3(MathUtils.Sin(x) + 0.1f * this.m_random.Float(-1f, 1f), 0f, MathUtils.Cos(x) + 0.1f * this.m_random.Float(-1f, 1f));
					FireworksParticleSystem.Particle[] particles2 = base.Particles;
					int nextParticle = this.m_nextParticle;
					this.m_nextParticle = nextParticle + 1;
					Particle obj = particles2[nextParticle];
					obj.IsActive = true;
					obj.Position = position;
					obj.Size = new Vector2(0.2f * particleSize);
					obj.TimeToLive = this.m_random.Float(1f, 3f);
					obj.Velocity = 20f * v;
					obj.TextureSlot = this.m_random.Int(0, 3);
					obj.FadeRate = this.m_random.Float(1f, 3f);
					obj.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
					obj.RotationSpeed = 0f;
				}
				return;
			}
			case FireworksBlock.Shape.Disc:
			{
				float num4 = this.m_random.Float(0f, 6.2831855f);
				int num5 = 13;
				for (int j = 0; j <= num5; j++)
				{
					float num6 = (float)j / (float)num5;
					int num7 = (int)MathUtils.Round(num6 * 2f * (float)num5);
					for (int k = 0; k < num7; k++)
					{
						float x2 = 6.2831855f * (float)k / (float)num7 + num4;
						Vector3 v2 = new Vector3(num6 * MathUtils.Sin(x2) + 0.1f * this.m_random.Float(-1f, 1f), 0f, num6 * MathUtils.Cos(x2) + 0.1f * this.m_random.Float(-1f, 1f));
						FireworksParticleSystem.Particle[] particles3 = base.Particles;
						int nextParticle = this.m_nextParticle;
						this.m_nextParticle = nextParticle + 1;
						Particle obj2 = particles3[nextParticle];
						obj2.IsActive = true;
						obj2.Position = position;
						obj2.Size = new Vector2(0.2f * particleSize);
						obj2.TimeToLive = this.m_random.Float(1f, 3f);
						obj2.Velocity = 22f * v2;
						obj2.TextureSlot = this.m_random.Int(0, 3);
						obj2.FadeRate = this.m_random.Float(1f, 3f);
						obj2.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
						obj2.RotationSpeed = 0f;
					}
				}
				return;
			}
			case FireworksBlock.Shape.Ball:
			{
				float num8 = this.m_random.Float(0f, 6.2831855f);
				int num9 = 12;
				Vector3 v3 = default(Vector3);
				for (int l = 0; l <= num9; l++)
				{
					float x3 = 3.1415927f * (float)l / (float)num9;
					v3.Y = MathUtils.Cos(x3);
					float num10 = MathUtils.Sin(x3);
					int num11 = (int)MathUtils.Round(num10 * 2f * (float)num9);
					for (int m = 0; m < num11; m++)
					{
						float x4 = 6.2831855f * (float)m / (float)num11 + num8;
						v3.X = num10 * MathUtils.Sin(x4);
						v3.Z = num10 * MathUtils.Cos(x4);
						FireworksParticleSystem.Particle[] particles4 = base.Particles;
						int nextParticle = this.m_nextParticle;
						this.m_nextParticle = nextParticle + 1;
						Particle obj3 = particles4[nextParticle];
						obj3.IsActive = true;
						obj3.Position = position;
						obj3.Size = new Vector2(0.2f * particleSize);
						obj3.TimeToLive = this.m_random.Float(1f, 3f);
						obj3.Velocity = 20f * v3;
						obj3.TextureSlot = this.m_random.Int(0, 3);
						obj3.FadeRate = this.m_random.Float(1f, 3f);
						obj3.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
						obj3.RotationSpeed = 0f;
					}
				}
				return;
			}
			case FireworksBlock.Shape.ShortTrails:
			case FireworksBlock.Shape.LongTrails:
			{
				float num12 = this.m_random.Float(0f, 6.2831855f);
				int num13 = 3;
				Vector3 v4 = default(Vector3);
				for (int n = 0; n <= num13; n++)
				{
					float x5 = 3.1415927f * (float)n / (float)num13;
					float num14 = MathUtils.Sin(x5);
					int num15 = (int)MathUtils.Round(num14 * (float)((shape == FireworksBlock.Shape.ShortTrails) ? 3 : 2) * (float)num13);
					for (int num16 = 0; num16 < num15; num16++)
					{
						float x6 = 6.2831855f * (float)num16 / (float)num15 + num12;
						v4.X = num14 * MathUtils.Sin(x6) + 0.3f * this.m_random.Float(-1f, 1f);
						v4.Y = MathUtils.Cos(x5) + 0.3f * this.m_random.Float(-1f, 1f);
						v4.Z = num14 * MathUtils.Cos(x6) + 0.3f * this.m_random.Float(-1f, 1f);
						FireworksParticleSystem.Particle[] particles5 = base.Particles;
						int nextParticle = this.m_nextParticle;
						this.m_nextParticle = nextParticle + 1;
						Particle obj4 = particles5[nextParticle];
						obj4.IsActive = true;
						obj4.Position = position;
						obj4.Size = new Vector2(0.25f);
						obj4.TimeToLive = this.m_random.Float(0.5f, 2.5f);
						obj4.Velocity = ((shape == FireworksBlock.Shape.ShortTrails) ? (25f * v4) : (35f * v4));
						obj4.TextureSlot = this.m_random.Int(0, 3);
						obj4.FadeRate = this.m_random.Float(1f, 3f);
						obj4.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
						obj4.GenerationFrequency = ((shape == FireworksBlock.Shape.ShortTrails) ? 1.9f : 2.1f);
						obj4.RotationSpeed = this.m_random.Float(-40f, 40f);
					}
				}
				return;
			}
			case FireworksBlock.Shape.FlatTrails:
			{
				float num17 = this.m_random.Float(0f, 6.2831855f);
				int num18 = 13;
				for (int num19 = 0; num19 < num18; num19++)
				{
					float x7 = 6.2831855f * (float)num19 / (float)num18 + num17;
					Vector3 v5 = new Vector3(MathUtils.Sin(x7) + 0.1f * this.m_random.Float(-1f, 1f), 0f, MathUtils.Cos(x7) + 0.1f * this.m_random.Float(-1f, 1f));
					FireworksParticleSystem.Particle[] particles6 = base.Particles;
					int nextParticle = this.m_nextParticle;
					this.m_nextParticle = nextParticle + 1;
					Particle obj5 = particles6[nextParticle];
					obj5.IsActive = true;
					obj5.Position = position;
					obj5.Size = new Vector2(0.25f);
					obj5.TimeToLive = this.m_random.Float(0.5f, 2.5f);
					obj5.Velocity = 25f * v5;
					obj5.TextureSlot = this.m_random.Int(0, 3);
					obj5.FadeRate = this.m_random.Float(1f, 3f);
					obj5.BaseColor = this.m_color * this.m_random.Float(0.5f, 1f);
					obj5.GenerationFrequency = 2.5f;
					obj5.RotationSpeed = this.m_random.Float(-40f, 40f);
				}
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0008F654 File Offset: 0x0008D854
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float num = MathUtils.Pow(0.01f, dt);
			float num2 = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				FireworksParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						Vector3 position = particle.Position += particle.Velocity * dt;
						FireworksParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + -9.81f * dt;
						particle.Velocity *= (particle.HighDamping ? num : num2);
						particle.Color = particle.BaseColor * MathUtils.Min(particle.FadeRate * particle.TimeToLive, 1f);
						particle.Rotation += particle.RotationSpeed * dt;
						if (!particle.HighDamping && this.m_random.Float(0f, 1f) < this.m_flickering)
						{
							particle.Color = Color.Transparent;
						}
						if (this.m_random.Float(0f, 1f) < 20f * dt)
						{
							particle.TextureSlot = this.m_random.Int(0, 3);
						}
						if (particle.GenerationFrequency > 0f)
						{
							float num3 = particle.Velocity.Length();
							particle.GenerationAccumulator += particle.GenerationFrequency * num3 * dt;
							if (particle.GenerationAccumulator > 1f && this.m_nextParticle < base.Particles.Length)
							{
								particle.GenerationAccumulator -= 1f;
								FireworksParticleSystem.Particle[] particles = base.Particles;
								int nextParticle = this.m_nextParticle;
								this.m_nextParticle = nextParticle + 1;
								Particle obj = particles[nextParticle];
								obj.IsActive = true;
								obj.Position = position;
								obj.Size = new Vector2(0.2f);
								obj.TimeToLive = 1f;
								obj.TextureSlot = this.m_random.Int(0, 3);
								obj.FadeRate = 1f;
								obj.BaseColor = particle.BaseColor;
								obj.HighDamping = true;
								obj.RotationSpeed = 0f;
							}
						}
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000CBC RID: 3260
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000CBD RID: 3261
		public Color m_color;

		// Token: 0x04000CBE RID: 3262
		public float m_flickering;

		// Token: 0x04000CBF RID: 3263
		public int m_nextParticle;

		// Token: 0x0200049F RID: 1183
		public class Particle : Game.Particle
		{
			// Token: 0x04001716 RID: 5910
			public Vector3 Velocity;

			// Token: 0x04001717 RID: 5911
			public float TimeToLive;

			// Token: 0x04001718 RID: 5912
			public float FadeRate;

			// Token: 0x04001719 RID: 5913
			public Color BaseColor;

			// Token: 0x0400171A RID: 5914
			public float RotationSpeed;

			// Token: 0x0400171B RID: 5915
			public float GenerationFrequency;

			// Token: 0x0400171C RID: 5916
			public float GenerationAccumulator;

			// Token: 0x0400171D RID: 5917
			public bool HighDamping;
		}
	}
}
