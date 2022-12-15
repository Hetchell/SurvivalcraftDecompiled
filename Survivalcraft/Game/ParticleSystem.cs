using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002C3 RID: 707
	public class ParticleSystem<T> : ParticleSystemBase where T : Particle, new()
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06001409 RID: 5129 RVA: 0x0009B3D9 File Offset: 0x000995D9
		public T[] Particles
		{
			get
			{
				return this.m_particles;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x0600140A RID: 5130 RVA: 0x0009B3E1 File Offset: 0x000995E1
		// (set) Token: 0x0600140B RID: 5131 RVA: 0x0009B3E9 File Offset: 0x000995E9
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
			set
			{
				if (value != this.m_texture)
				{
					this.m_texture = value;
					this.AdditiveBatch = null;
					this.AlphaBlendedBatch = null;
				}
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x0009B409 File Offset: 0x00099609
		// (set) Token: 0x0600140D RID: 5133 RVA: 0x0009B411 File Offset: 0x00099611
		public int TextureSlotsCount { get; set; }

		// Token: 0x0600140E RID: 5134 RVA: 0x0009B41C File Offset: 0x0009961C
		public ParticleSystem(int particlesCount)
		{
			this.m_particles = new T[particlesCount];
			for (int i = 0; i < this.m_particles.Length; i++)
			{
				this.m_particles[i] = Activator.CreateInstance<T>();
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0009B484 File Offset: 0x00099684
		public override void Draw(Camera camera)
		{
			if (this.AdditiveBatch == null || this.AlphaBlendedBatch == null)
			{
				this.AdditiveBatch = this.SubsystemParticles.PrimitivesRenderer.TexturedBatch(this.m_texture, true, 0, DepthStencilState.DepthRead, null, BlendState.Additive, SamplerState.PointClamp);
				this.AlphaBlendedBatch = this.SubsystemParticles.PrimitivesRenderer.TexturedBatch(this.m_texture, true, 0, DepthStencilState.Default, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			}
			this.m_front[0] = camera.ViewDirection;
			this.m_right[0] = Vector3.Normalize(Vector3.Cross(this.m_front[0], Vector3.UnitY));
			this.m_up[0] = Vector3.Normalize(Vector3.Cross(this.m_right[0], this.m_front[0]));
			this.m_front[1] = camera.ViewDirection;
			this.m_right[1] = Vector3.Normalize(Vector3.Cross(this.m_front[1], Vector3.UnitY));
			this.m_up[1] = Vector3.UnitY;
			this.m_front[2] = Vector3.UnitY;
			this.m_right[2] = Vector3.UnitX;
			this.m_up[2] = Vector3.UnitZ;
			float s = 1f / (float)this.TextureSlotsCount;
			for (int i = 0; i < this.m_particles.Length; i++)
			{
				Particle particle = this.m_particles[i];
				if (particle.IsActive)
				{
					Vector3 position = particle.Position;
					Vector2 size = particle.Size;
					float rotation = particle.Rotation;
					int textureSlot = particle.TextureSlot;
					int billboardingMode = (int)particle.BillboardingMode;
					Vector3 p;
					Vector3 p2;
					Vector3 p3;
					Vector3 p4;
					if (rotation != 0f)
					{
						Vector3 vector = (this.m_front[billboardingMode].X * this.m_front[billboardingMode].X > this.m_front[billboardingMode].Z * this.m_front[billboardingMode].Z) ? new Vector3(0f, MathUtils.Cos(rotation), MathUtils.Sin(rotation)) : new Vector3(MathUtils.Sin(rotation), MathUtils.Cos(rotation), 0f);
						Vector3 vector2 = Vector3.Normalize(Vector3.Cross(this.m_front[(int)particle.BillboardingMode], vector));
						vector = Vector3.Normalize(Vector3.Cross(this.m_front[(int)particle.BillboardingMode], vector2));
						vector2 *= size.Y;
						vector *= size.X;
						p = position + (-vector2 - vector);
						p2 = position + (vector2 - vector);
						p3 = position + (vector2 + vector);
						p4 = position + (-vector2 + vector);
					}
					else
					{
						Vector3 vector3 = this.m_right[billboardingMode] * size.X;
						Vector3 v = this.m_up[billboardingMode] * size.Y;
						p = position + (-vector3 - v);
						p2 = position + (vector3 - v);
						p3 = position + (vector3 + v);
						p4 = position + (-vector3 + v);
					}
					TexturedBatch3D texturedBatch3D = particle.UseAdditiveBlending ? this.AdditiveBatch : this.AlphaBlendedBatch;
					Vector2 v2 = new Vector2((float)(textureSlot % this.TextureSlotsCount), (float)(textureSlot / this.TextureSlotsCount));
					float num = 0f;
					float num2 = 1f;
					float num3 = 1f;
					float num4 = 0f;
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
					Vector2 texCoord = (v2 + new Vector2(num, num3)) * s;
					Vector2 texCoord2 = (v2 + new Vector2(num2, num3)) * s;
					Vector2 texCoord3 = (v2 + new Vector2(num2, num4)) * s;
					Vector2 texCoord4 = (v2 + new Vector2(num, num4)) * s;
					texturedBatch3D.QueueQuad(p, p2, p3, p4, texCoord, texCoord2, texCoord3, texCoord4, particle.Color);
				}
			}
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0009B906 File Offset: 0x00099B06
		public override bool Simulate(float dt)
		{
			return false;
		}

		// Token: 0x04000DE1 RID: 3553
		public T[] m_particles;

		// Token: 0x04000DE2 RID: 3554
		public Texture2D m_texture;

		// Token: 0x04000DE3 RID: 3555
		public Vector3[] m_front = new Vector3[3];

		// Token: 0x04000DE4 RID: 3556
		public Vector3[] m_right = new Vector3[3];

		// Token: 0x04000DE5 RID: 3557
		public Vector3[] m_up = new Vector3[3];

		// Token: 0x04000DE6 RID: 3558
		public TexturedBatch3D AdditiveBatch;

		// Token: 0x04000DE7 RID: 3559
		public TexturedBatch3D AlphaBlendedBatch;
	}
}
