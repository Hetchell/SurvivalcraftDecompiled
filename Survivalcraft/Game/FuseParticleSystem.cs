using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000282 RID: 642
	public class FuseParticleSystem : ParticleSystem<FuseParticleSystem.Particle>
	{
		// Token: 0x0600130A RID: 4874 RVA: 0x00094250 File Offset: 0x00092450
		public FuseParticleSystem(Vector3 position) : base(15)
		{
			this.m_position = position;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x00094284 File Offset: 0x00092484
		public override bool Simulate(float dt)
		{
			if (this.m_visible)
			{
				this.m_toGenerate += 15f * dt;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					FuseParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						particle.Time += dt;
						particle.TimeToLive -= dt;
						if (particle.TimeToLive > 0f)
						{
							FuseParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
							particle.Speed = MathUtils.Max(particle.Speed - 1.5f * dt, particle.TargetSpeed);
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 0.75f, 8f);
							particle.Size = new Vector2(0.07f * (1f + 2f * particle.Time));
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = this.m_position + 0.02f * new Vector3(0f, this.m_random.Float(-1f, 1f), 0f);
						particle.Color = Color.White;
						particle.TargetSpeed = this.m_random.Float(0.45f, 0.55f) * 0.4f;
						particle.Speed = this.m_random.Float(0.45f, 0.55f) * 2.5f;
						particle.Time = 0f;
						particle.Size = Vector2.Zero;
						particle.TimeToLive = this.m_random.Float(0.3f, 1f);
						particle.FlipX = (this.m_random.Int(0, 1) == 0);
						particle.FlipY = (this.m_random.Int(0, 1) == 0);
						this.m_toGenerate -= 1f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			}
			this.m_visible = false;
			return false;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x000944C8 File Offset: 0x000926C8
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.m_position - camera.ViewPosition, camera.ViewDirection);
			if (num > -0.5f && num <= 32f && Vector3.DistanceSquared(this.m_position, camera.ViewPosition) <= 1024f)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000D0E RID: 3342
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D0F RID: 3343
		public Vector3 m_position;

		// Token: 0x04000D10 RID: 3344
		public float m_toGenerate;

		// Token: 0x04000D11 RID: 3345
		public bool m_visible;

		// Token: 0x020004A7 RID: 1191
		public class Particle : Game.Particle
		{
			// Token: 0x0400173A RID: 5946
			public float Time;

			// Token: 0x0400173B RID: 5947
			public float TimeToLive;

			// Token: 0x0400173C RID: 5948
			public float Speed;

			// Token: 0x0400173D RID: 5949
			public float TargetSpeed;
		}
	}
}
