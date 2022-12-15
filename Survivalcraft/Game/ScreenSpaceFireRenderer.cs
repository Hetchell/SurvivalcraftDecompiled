using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200014C RID: 332
	public class ScreenSpaceFireRenderer
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x000266BC File Offset: 0x000248BC
		// (set) Token: 0x06000649 RID: 1609 RVA: 0x000266C4 File Offset: 0x000248C4
		public float ParticlesPerSecond { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x000266CD File Offset: 0x000248CD
		// (set) Token: 0x0600064B RID: 1611 RVA: 0x000266D5 File Offset: 0x000248D5
		public float ParticleSpeed { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x000266DE File Offset: 0x000248DE
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x000266E6 File Offset: 0x000248E6
		public float MinTimeToLive { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x000266EF File Offset: 0x000248EF
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x000266F7 File Offset: 0x000248F7
		public float MaxTimeToLive { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x00026700 File Offset: 0x00024900
		// (set) Token: 0x06000651 RID: 1617 RVA: 0x00026708 File Offset: 0x00024908
		public float ParticleSize { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x00026711 File Offset: 0x00024911
		// (set) Token: 0x06000653 RID: 1619 RVA: 0x00026719 File Offset: 0x00024919
		public float ParticleAnimationPeriod { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x00026722 File Offset: 0x00024922
		// (set) Token: 0x06000655 RID: 1621 RVA: 0x0002672A File Offset: 0x0002492A
		public float ParticleAnimationOffset { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x00026733 File Offset: 0x00024933
		// (set) Token: 0x06000657 RID: 1623 RVA: 0x0002673B File Offset: 0x0002493B
		public Vector2 Origin { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x00026744 File Offset: 0x00024944
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x0002674C File Offset: 0x0002494C
		public float Width { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x00026755 File Offset: 0x00024955
		// (set) Token: 0x0600065B RID: 1627 RVA: 0x0002675D File Offset: 0x0002495D
		public float CutoffPosition { get; set; }

		// Token: 0x0600065C RID: 1628 RVA: 0x00026768 File Offset: 0x00024968
		public ScreenSpaceFireRenderer(int particlesCount)
		{
			this.m_texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			for (int i = 0; i < particlesCount; i++)
			{
				this.m_particles.Add(new ScreenSpaceFireRenderer.Particle());
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x000267C0 File Offset: 0x000249C0
		public void Update(float dt)
		{
			this.m_toGenerate += this.ParticlesPerSecond * dt;
			foreach (ScreenSpaceFireRenderer.Particle particle in this.m_particles)
			{
				if (particle.Active)
				{
					ScreenSpaceFireRenderer.Particle particle2 = particle;
					particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
					particle.AnimationTime += dt;
					particle.TimeToLive -= dt;
					particle.TextureSlot = (int)MathUtils.Max(9f * particle.AnimationTime / this.ParticleAnimationPeriod, 0f);
					if (particle.TimeToLive <= 0f || particle.TextureSlot > 8 || particle.Position.Y < this.CutoffPosition)
					{
						particle.Active = false;
					}
				}
				else if (this.m_toGenerate >= 1f)
				{
					particle.Active = true;
					particle.Position = new Vector2(this.m_random.Float(this.Origin.X, this.Origin.X + this.Width), this.Origin.Y);
					particle.Size = new Vector2(this.ParticleSize);
					particle.Speed = (0f - this.m_random.Float(0.75f, 1.25f)) * this.ParticleSpeed;
					particle.AnimationTime = this.m_random.Float(0f, this.ParticleAnimationOffset);
					particle.TimeToLive = MathUtils.Lerp(this.MinTimeToLive, this.MaxTimeToLive, this.m_random.Float(0f, 1f));
					particle.FlipX = (this.m_random.Int(0, 1) == 0);
					particle.FlipY = (this.m_random.Int(0, 1) == 0);
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x000269F8 File Offset: 0x00024BF8
		public void Draw(PrimitivesRenderer2D primitivesRenderer, float depth, Matrix matrix, Color color)
		{
			TexturedBatch2D texturedBatch2D = primitivesRenderer.TexturedBatch(this.m_texture, false, 0, DepthStencilState.None, null, null, SamplerState.PointClamp);
			int count = texturedBatch2D.TriangleVertices.Count;
			foreach (ScreenSpaceFireRenderer.Particle particle in this.m_particles)
			{
				if (particle.Active)
				{
					this.DrawParticle(texturedBatch2D, particle, depth, color);
				}
			}
			texturedBatch2D.TransformTriangles(matrix, count, -1);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00026A88 File Offset: 0x00024C88
		public void DrawParticle(TexturedBatch2D batch, ScreenSpaceFireRenderer.Particle particle, float depth, Color color)
		{
			Vector2 corner = particle.Position - particle.Size / 2f;
			Vector2 corner2 = particle.Position + particle.Size / 2f;
			int textureSlot = particle.TextureSlot;
			Vector2 v = new Vector2((float)(textureSlot % 3), (float)(textureSlot / 3));
			float num = 0f;
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 1f;
			if (particle.FlipX)
			{
				num = 1f - num;
				num2 = 1f - num2;
			}
			if (particle.FlipY)
			{
				num3 = 1f - num3;
				num4 = 1f - num4;
			}
			Vector2 texCoord = (v + new Vector2(num, num3)) * 0.33333334f;
			Vector2 texCoord2 = (v + new Vector2(num2, num4)) * 0.33333334f;
			batch.QueueQuad(corner, corner2, depth, texCoord, texCoord2, color);
		}

		// Token: 0x04000334 RID: 820
		public List<ScreenSpaceFireRenderer.Particle> m_particles = new List<ScreenSpaceFireRenderer.Particle>();

		// Token: 0x04000335 RID: 821
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000336 RID: 822
		public float m_toGenerate;

		// Token: 0x04000337 RID: 823
		public Texture2D m_texture;

		// Token: 0x0200040C RID: 1036
		public class Particle
		{
			// Token: 0x04001521 RID: 5409
			public bool Active;

			// Token: 0x04001522 RID: 5410
			public Vector2 Position;

			// Token: 0x04001523 RID: 5411
			public Vector2 Size;

			// Token: 0x04001524 RID: 5412
			public float Speed;

			// Token: 0x04001525 RID: 5413
			public int TextureSlot;

			// Token: 0x04001526 RID: 5414
			public bool FlipX;

			// Token: 0x04001527 RID: 5415
			public bool FlipY;

			// Token: 0x04001528 RID: 5416
			public float AnimationTime;

			// Token: 0x04001529 RID: 5417
			public float TimeToLive;
		}
	}
}
