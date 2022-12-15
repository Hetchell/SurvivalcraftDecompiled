using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000271 RID: 625
	public class FireParticleSystem : ParticleSystem<FireParticleSystem.Particle>
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x0600127F RID: 4735 RVA: 0x0008E9B4 File Offset: 0x0008CBB4
		// (set) Token: 0x06001280 RID: 4736 RVA: 0x0008E9BC File Offset: 0x0008CBBC
		public bool IsStopped { get; set; }

		// Token: 0x06001281 RID: 4737 RVA: 0x0008E9C8 File Offset: 0x0008CBC8
		public FireParticleSystem(Vector3 position, float size, float maxVisibilityDistance) : base(10)
		{
			this.m_position = position;
			this.m_size = size;
			this.m_maxVisibilityDistance = maxVisibilityDistance;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0008EA14 File Offset: 0x0008CC14
		public override bool Simulate(float dt)
		{
			this.m_age += dt;
			bool flag = false;
			if (this.m_visible || this.m_age < 2f)
			{
				this.m_toGenerate += (this.IsStopped ? 0f : (5f * dt));
				for (int i = 0; i < base.Particles.Length; i++)
				{
					FireParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						particle.TimeToLive -= dt;
						if (particle.TimeToLive > 0f)
						{
							FireParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 8f);
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = this.m_position + 0.25f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), 0f, this.m_random.Float(-1f, 1f));
						particle.Color = Color.White;
						particle.Size = new Vector2(this.m_size);
						particle.Speed = this.m_random.Float(0.45f, 0.55f) * this.m_size / 0.15f;
						particle.Time = 0f;
						particle.TimeToLive = this.m_random.Float(0.5f, 2f);
						particle.FlipX = (this.m_random.Int(0, 1) == 0);
						particle.FlipY = (this.m_random.Int(0, 1) == 0);
						this.m_toGenerate -= 1f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			}
			this.m_visible = false;
			return this.IsStopped && !flag;
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0008EC4C File Offset: 0x0008CE4C
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.m_position - camera.ViewPosition, camera.ViewDirection);
			if (num > -0.5f && num <= this.m_maxVisibilityDistance && Vector3.DistanceSquared(this.m_position, camera.ViewPosition) <= this.m_maxVisibilityDistance * this.m_maxVisibilityDistance)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000CB4 RID: 3252
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000CB5 RID: 3253
		public Vector3 m_position;

		// Token: 0x04000CB6 RID: 3254
		public float m_size;

		// Token: 0x04000CB7 RID: 3255
		public float m_toGenerate;

		// Token: 0x04000CB8 RID: 3256
		public bool m_visible;

		// Token: 0x04000CB9 RID: 3257
		public float m_maxVisibilityDistance;

		// Token: 0x04000CBA RID: 3258
		public float m_age;

		// Token: 0x0200049E RID: 1182
		public class Particle : Game.Particle
		{
			// Token: 0x04001713 RID: 5907
			public float Time;

			// Token: 0x04001714 RID: 5908
			public float TimeToLive;

			// Token: 0x04001715 RID: 5909
			public float Speed;
		}
	}
}
