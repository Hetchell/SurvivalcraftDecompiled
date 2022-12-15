using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000231 RID: 561
	public class BurntDebrisParticleSystem : ParticleSystem<BurntDebrisParticleSystem.Particle>
	{
		// Token: 0x0600113C RID: 4412 RVA: 0x00086B6C File Offset: 0x00084D6C
		public BurntDebrisParticleSystem(SubsystemTerrain terrain, int x, int y, int z) : this(terrain, new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f))
		{
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00086B94 File Offset: 0x00084D94
		public BurntDebrisParticleSystem(SubsystemTerrain terrain, Vector3 position) : base(15)
		{
			this.m_subsystemTerrain = terrain;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			int num4 = 0;
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num + 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num - 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 + 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 - 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 + 1));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 - 1));
			base.TextureSlotsCount = 3;
			Color color = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			color *= s;
			color.A = byte.MaxValue;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				BurntDebrisParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				particle.Position = position + 0.5f * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Color = color;
				particle.Size = new Vector2(0.5f);
				particle.TimeToLive = this.m_random.Float(0.75f, 2f);
				particle.Velocity = new Vector3(3f * this.m_random.Float(-1f, 1f), 2f * this.m_random.Float(-1f, 1f), 3f * this.m_random.Float(-1f, 1f));
				particle.TextureSlot = 8;
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00086DC8 File Offset: 0x00084FC8
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.04f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				BurntDebrisParticleSystem.Particle particle = base.Particles[i];
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
								particle.Velocity *= new Vector3(-0.1f, 0.1f, 0.1f);
							}
							if (plane.Normal.Y != 0f)
							{
								particle.Velocity *= new Vector3(0.1f, -0.1f, 0.1f);
							}
							if (plane.Normal.Z != 0f)
							{
								particle.Velocity *= new Vector3(0.1f, 0.1f, -0.1f);
							}
						}
						particle.Position = vector;
						BurntDebrisParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + -10f * dt;
						particle.Velocity *= s;
						particle.Color *= MathUtils.Saturate(particle.TimeToLive);
						particle.TextureSlot = (int)(8.99f * MathUtils.Saturate(2f - particle.TimeToLive));
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000B94 RID: 2964
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B95 RID: 2965
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x02000477 RID: 1143
		public class Particle : Game.Particle
		{
			// Token: 0x0400169E RID: 5790
			public Vector3 Velocity;

			// Token: 0x0400169F RID: 5791
			public float TimeToLive;
		}
	}
}
