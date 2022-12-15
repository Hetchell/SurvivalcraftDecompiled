using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002DD RID: 733
	public class PukeParticleSystem : ParticleSystem<PukeParticleSystem.Particle>
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060014A9 RID: 5289 RVA: 0x000A0617 File Offset: 0x0009E817
		// (set) Token: 0x060014AA RID: 5290 RVA: 0x000A061F File Offset: 0x0009E81F
		public Vector3 Position { get; set; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060014AB RID: 5291 RVA: 0x000A0628 File Offset: 0x0009E828
		// (set) Token: 0x060014AC RID: 5292 RVA: 0x000A0630 File Offset: 0x0009E830
		public Vector3 Direction { get; set; }

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060014AD RID: 5293 RVA: 0x000A0639 File Offset: 0x0009E839
		// (set) Token: 0x060014AE RID: 5294 RVA: 0x000A0641 File Offset: 0x0009E841
		public bool IsStopped { get; set; }

		// Token: 0x060014AF RID: 5295 RVA: 0x000A064A File Offset: 0x0009E84A
		public PukeParticleSystem(SubsystemTerrain terrain) : base(80)
		{
			this.m_subsystemTerrain = terrain;
			base.Texture = ContentManager.Get<Texture2D>("Textures/PukeParticle");
			base.TextureSlotsCount = 3;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x000A0680 File Offset: 0x0009E880
		public override bool Simulate(float dt)
		{
			int num = Terrain.ToCell(this.Position.X);
			int num2 = Terrain.ToCell(this.Position.Y);
			int num3 = Terrain.ToCell(this.Position.Z);
			int num4 = 0;
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num + 1, num2, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num - 1, num2, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2 + 1, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2 - 1, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2, num3 + 1));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2, num3 - 1));
			Color c = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			c *= s;
			c.A = byte.MaxValue;
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s2 = MathUtils.Pow(0.03f, dt);
			this.m_duration += dt;
			if (this.m_duration > 3.5f)
			{
				this.IsStopped = true;
			}
			float num5 = MathUtils.Saturate(1.3f * SimplexNoise.Noise(3f * this.m_duration + (float)(this.GetHashCode() % 100)) - 0.3f);
			float num6 = 30f * num5;
			this.m_toGenerate += num6 * dt;
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				PukeParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						Vector3 position = particle.Position;
						Vector3 vector = position + particle.Velocity * dt;
						TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(position, vector, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable);
						if (terrainRaycastResult != null)
						{
							Plane plane = terrainRaycastResult.Value.CellFace.CalculatePlane();
							vector = position;
							if (plane.Normal.X != 0f)
							{
								particle.Velocity *= new Vector3(-0.05f, 0.05f, 0.05f);
							}
							if (plane.Normal.Y != 0f)
							{
								particle.Velocity *= new Vector3(0.05f, -0.05f, 0.05f);
							}
							if (plane.Normal.Z != 0f)
							{
								particle.Velocity *= new Vector3(0.05f, 0.05f, -0.05f);
							}
						}
						particle.Position = vector;
						PukeParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + -9.81f * dt;
						particle.Velocity *= s2;
						particle.Color *= MathUtils.Saturate(particle.TimeToLive);
						particle.TextureSlot = (int)(8.99f * MathUtils.Saturate(3f - particle.TimeToLive));
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					Vector3 v = this.m_random.Vector3(0f, 1f);
					particle.IsActive = true;
					particle.Position = this.Position + 0.05f * v;
					particle.Color = Color.MultiplyColorOnly(c, this.m_random.Float(0.7f, 1f));
					particle.Velocity = MathUtils.Lerp(1f, 2.5f, num5) * Vector3.Normalize(this.Direction + 0.25f * v);
					particle.TimeToLive = 3f;
					particle.Size = new Vector2(0.1f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x04000EB8 RID: 3768
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000EB9 RID: 3769
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000EBA RID: 3770
		public float m_duration;

		// Token: 0x04000EBB RID: 3771
		public float m_toGenerate;

		// Token: 0x020004D3 RID: 1235
		public class Particle : Game.Particle
		{
			// Token: 0x040017C0 RID: 6080
			public Vector3 Velocity;

			// Token: 0x040017C1 RID: 6081
			public float TimeToLive;
		}
	}
}
