using System;
using Engine;

namespace Game
{
	// Token: 0x020002C0 RID: 704
	public class PaintParticleSystem : ParticleSystem<PaintParticleSystem.Particle>
	{
		// Token: 0x06001406 RID: 5126 RVA: 0x0009AF4C File Offset: 0x0009914C
		public PaintParticleSystem(SubsystemTerrain terrain, Vector3 position, Vector3 normal, Color color) : base(20)
		{
			this.m_subsystemTerrain = terrain;
			base.Texture = terrain.Project.FindSubsystem<SubsystemBlocksTexture>(true).BlocksTexture;
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
			base.TextureSlotsCount = 16;
			float s = LightingManager.LightIntensityByLightValue[num4];
			this.m_color = color * s;
			this.m_color.A = color.A;
			Vector3 vector = Vector3.Normalize(Vector3.Cross(normal, new Vector3(0.37f, 0.15f, 0.17f)));
			Vector3 v = Vector3.Normalize(Vector3.Cross(normal, vector));
			for (int i = 0; i < base.Particles.Length; i++)
			{
				PaintParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				Vector2 vector2 = new Vector2(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Position = position + 0.4f * (vector2.X * vector + vector2.Y * v) + 0.03f * normal;
				particle.Color = this.m_color;
				particle.Size = new Vector2(this.m_random.Float(0.025f, 0.035f));
				particle.TimeToLive = this.m_random.Float(0.5f, 1.5f);
				particle.Velocity = 1f * (vector2.X * vector + vector2.Y * v) + this.m_random.Float(-3f, 0.5f) * normal;
				particle.TextureSlot = 15;
				particle.Alpha = this.m_random.Float(0.3f, 0.6f);
			}
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0009B1FC File Offset: 0x000993FC
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float num = MathUtils.Pow(0.2f, dt);
			float num2 = MathUtils.Pow(1E-07f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				PaintParticleSystem.Particle particle = base.Particles[i];
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
							particle.Velocity = Vector3.Zero;
							particle.Position = terrainRaycastResult.Value.HitPoint(0.03f);
							particle.HighDampingFactor = this.m_random.Float(0.5f, 1f);
							if (terrainRaycastResult.Value.CellFace.Face >= 4)
							{
								particle.NoGravity = true;
							}
						}
						else
						{
							particle.Position = vector;
						}
						if (!particle.NoGravity)
						{
							PaintParticleSystem.Particle particle2 = particle;
							particle2.Velocity.Y = particle2.Velocity.Y + -9.81f * dt;
						}
						particle.Velocity *= ((particle.HighDampingFactor > 0f) ? (num2 * particle.HighDampingFactor) : num);
						particle.Color = this.m_color * MathUtils.Saturate(1.5f * particle.TimeToLive * particle.Alpha);
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000DD0 RID: 3536
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000DD1 RID: 3537
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000DD2 RID: 3538
		public Color m_color;

		// Token: 0x020004BD RID: 1213
		public class Particle : Game.Particle
		{
			// Token: 0x04001783 RID: 6019
			public Vector3 Velocity;

			// Token: 0x04001784 RID: 6020
			public float TimeToLive;

			// Token: 0x04001785 RID: 6021
			public float HighDampingFactor;

			// Token: 0x04001786 RID: 6022
			public bool NoGravity;

			// Token: 0x04001787 RID: 6023
			public float Alpha;
		}
	}
}
