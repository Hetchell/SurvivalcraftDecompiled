using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200028E RID: 654
	public class HitValueParticleSystem : ParticleSystem<HitValueParticleSystem.Particle>
	{
		// Token: 0x0600132C RID: 4908 RVA: 0x000968B8 File Offset: 0x00094AB8
		public HitValueParticleSystem(Vector3 position, Vector3 velocity, Color color, string text) : base(1)
		{
			Game.Random random = new Game.Random();
			HitValueParticleSystem.Particle particle = base.Particles[0];
			particle.IsActive = true;
			particle.Position = position;
			particle.TimeToLive = 0.9f;
			particle.Velocity = velocity + random.Vector3(0.75f) * new Vector3(1f, 0f, 1f) + 0.5f * Vector3.UnitY;
			particle.BaseColor = color;
			particle.Text = text;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x00096948 File Offset: 0x00094B48
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				HitValueParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						particle.Velocity += new Vector3(0f, 0.5f, 0f) * dt;
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

		// Token: 0x0600132E RID: 4910 RVA: 0x00096A48 File Offset: 0x00094C48
		public override void Draw(Camera camera)
		{
			if (this.m_batch == null)
			{
				this.m_batch = this.SubsystemParticles.PrimitivesRenderer.FontBatch(ContentManager.Get<BitmapFont>("Fonts/Pericles"), 0, DepthStencilState.None, null, null, null);
			}
			Vector3 viewDirection = camera.ViewDirection;
			Vector3 vector = Vector3.Normalize(Vector3.Cross(viewDirection, Vector3.UnitY));
			Vector3 v = -Vector3.Normalize(Vector3.Cross(vector, viewDirection));
			for (int i = 0; i < base.Particles.Length; i++)
			{
				HitValueParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					float num = Vector3.Distance(camera.ViewPosition, particle.Position);
					float num2 = MathUtils.Saturate(3f * (num - 0.2f));
					float num3 = MathUtils.Saturate(0.2f * (20f - num));
					float num4 = num2 * num3;
					if (num4 > 0f)
					{
						float s = 0.006f * MathUtils.Sqrt(num);
						Color color = particle.Color * num4;
						this.m_batch.QueueText(particle.Text, particle.Position, vector * s, v * s, color, TextAnchor.HorizontalCenter, Vector2.Zero);
					}
				}
			}
		}

		// Token: 0x04000D41 RID: 3393
		public FontBatch3D m_batch;

		// Token: 0x020004AC RID: 1196
		public class Particle : Game.Particle
		{
			// Token: 0x04001751 RID: 5969
			public float TimeToLive;

			// Token: 0x04001752 RID: 5970
			public Vector3 Velocity;

			// Token: 0x04001753 RID: 5971
			public Color BaseColor;

			// Token: 0x04001754 RID: 5972
			public string Text;
		}
	}
}
