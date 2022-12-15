using System;
using Engine;

namespace Game
{
	// Token: 0x02000224 RID: 548
	public class BlockDebrisParticleSystem : ParticleSystem<BlockDebrisParticleSystem.Particle>
	{
		// Token: 0x060010E5 RID: 4325 RVA: 0x0007FA44 File Offset: 0x0007DC44
		public BlockDebrisParticleSystem(SubsystemTerrain terrain, Vector3 position, float strength, float scale, Color color, int textureSlot) : base((int)(50f * strength))
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
			base.TextureSlotsCount = 32;
			float s = LightingManager.LightIntensityByLightValue[num4];
			color *= s;
			color.A = byte.MaxValue;
			float num5 = MathUtils.Sqrt(strength);
			for (int i = 0; i < base.Particles.Length; i++)
			{
				BlockDebrisParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				Vector3 vector = new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Position = position + strength * 0.45f * vector;
				particle.Color = Color.MultiplyColorOnly(color, this.m_random.Float(0.7f, 1f));
				particle.Size = num5 * scale * new Vector2(this.m_random.Float(0.05f, 0.06f));
				particle.TimeToLive = num5 * this.m_random.Float(1f, 3f);
				particle.Velocity = num5 * 2f * (vector + new Vector3(this.m_random.Float(-0.2f, 0.2f), 0.6f, this.m_random.Float(-0.2f, 0.2f)));
				particle.TextureSlot = textureSlot % 16 * 2 + this.m_random.Int(0, 1) + 32 * (textureSlot / 16 * 2 + this.m_random.Int(0, 1));
			}
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x0007FCE0 File Offset: 0x0007DEE0
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				BlockDebrisParticleSystem.Particle particle = base.Particles[i];
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
								particle.Velocity *= new Vector3(-0.25f, 0.25f, 0.25f);
							}
							if (plane.Normal.Y != 0f)
							{
								particle.Velocity *= new Vector3(0.25f, -0.25f, 0.25f);
							}
							if (plane.Normal.Z != 0f)
							{
								particle.Velocity *= new Vector3(0.25f, 0.25f, -0.25f);
							}
						}
						particle.Position = vector;
						BlockDebrisParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + -9.81f * dt;
						particle.Velocity *= s;
						particle.Color *= MathUtils.Saturate(particle.TimeToLive);
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000B52 RID: 2898
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B53 RID: 2899
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x02000470 RID: 1136
		public class Particle : Game.Particle
		{
			// Token: 0x04001687 RID: 5767
			public Vector3 Velocity;

			// Token: 0x04001688 RID: 5768
			public float TimeToLive;
		}
	}
}
