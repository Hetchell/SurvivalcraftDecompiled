using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000273 RID: 627
	public class FireworksTrailParticleSystem : ParticleSystem<FireworksTrailParticleSystem.Particle>, ITrailParticleSystem
	{
		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06001286 RID: 4742 RVA: 0x0008F8E0 File Offset: 0x0008DAE0
		// (set) Token: 0x06001287 RID: 4743 RVA: 0x0008F8E8 File Offset: 0x0008DAE8
		public Vector3 Position { get; set; }

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06001288 RID: 4744 RVA: 0x0008F8F1 File Offset: 0x0008DAF1
		// (set) Token: 0x06001289 RID: 4745 RVA: 0x0008F8F9 File Offset: 0x0008DAF9
		public bool IsStopped { get; set; }

		// Token: 0x0600128A RID: 4746 RVA: 0x0008F902 File Offset: 0x0008DB02
		public FireworksTrailParticleSystem() : base(60)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x0008F930 File Offset: 0x0008DB30
		public override bool Simulate(float dt)
		{
			float num = 120f;
			this.m_toGenerate += num * dt;
			if (this.m_lastPosition == null)
			{
				this.m_lastPosition = new Vector3?(this.Position);
			}
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				FireworksTrailParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration, 8f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					particle.Position = Vector3.Lerp(this.m_lastPosition.Value, this.Position, this.m_random.Float(0f, 1f));
					particle.Color = Color.White;
					particle.Time = this.m_random.Float(0f, 0.75f);
					particle.Size = new Vector2(this.m_random.Float(0.12f, 0.16f));
					particle.Duration = 1f;
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			this.m_lastPosition = new Vector3?(this.Position);
			return this.IsStopped && !flag;
		}

		// Token: 0x04000CC0 RID: 3264
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000CC1 RID: 3265
		public float m_toGenerate;

		// Token: 0x04000CC2 RID: 3266
		public Vector3? m_lastPosition;

		// Token: 0x020004A0 RID: 1184
		public class Particle : Game.Particle
		{
			// Token: 0x0400171E RID: 5918
			public float Time;

			// Token: 0x0400171F RID: 5919
			public float Duration;
		}
	}
}
