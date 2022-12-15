using System;
using Engine;

namespace Game
{
	// Token: 0x02000312 RID: 786
	public class TerrainContentsGeneratorFlat : ITerrainContentsGenerator
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001694 RID: 5780 RVA: 0x000B2F4A File Offset: 0x000B114A
		public int OceanLevel
		{
			get
			{
				return this.m_worldSettings.TerrainLevel + this.m_worldSettings.SeaLevelOffset;
			}
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x000B2F64 File Offset: 0x000B1164
		public TerrainContentsGeneratorFlat(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			SubsystemGameInfo subsystemGameInfo = subsystemTerrain.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_worldSettings = subsystemGameInfo.WorldSettings;
			this.m_oceanCorner = ((string.CompareOrdinal(subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") < 0) ? (this.m_oceanCorner = new Vector2(2001f, 2001f)) : (this.m_oceanCorner = new Vector2(-199f, -199f)));
			this.m_islandSize = ((this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatIsland) ? new Vector2?(this.m_worldSettings.IslandSize) : null);
			this.m_shoreRoughnessAmplitude.X = MathUtils.Pow(this.m_worldSettings.ShoreRoughness, 2f) * ((this.m_islandSize != null) ? MathUtils.Min(4f * this.m_islandSize.Value.X, 400f) : 400f);
			this.m_shoreRoughnessAmplitude.Y = MathUtils.Pow(this.m_worldSettings.ShoreRoughness, 2f) * ((this.m_islandSize != null) ? MathUtils.Min(4f * this.m_islandSize.Value.Y, 400f) : 400f);
			this.m_shoreRoughnessFrequency = MathUtils.Lerp(0.5f, 1f, this.m_worldSettings.ShoreRoughness) * new Vector2(1f) / this.m_shoreRoughnessAmplitude;
			this.m_shoreRoughnessOctaves.X = (float)((int)MathUtils.Clamp(MathUtils.Log(1f / this.m_shoreRoughnessFrequency.X) / MathUtils.Log(2f) - 1f, 1f, 7f));
			this.m_shoreRoughnessOctaves.Y = (float)((int)MathUtils.Clamp(MathUtils.Log(1f / this.m_shoreRoughnessFrequency.Y) / MathUtils.Log(2f) - 1f, 1f, 7f));
			Game.Random random = new Game.Random(subsystemGameInfo.WorldSeed);
			this.m_shoreRoughnessOffset[0] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[1] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[2] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[3] = random.Float(-2000f, 2000f);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x000B31F8 File Offset: 0x000B13F8
		public Vector3 FindCoarseSpawnPosition()
		{
			for (int i = -400; i <= 400; i += 10)
			{
				for (int j = -400; j <= 400; j += 10)
				{
					Vector2 vector = this.m_oceanCorner + new Vector2((float)i, (float)j);
					float num = this.CalculateOceanShoreDistance(vector.X, vector.Y);
					if (num >= 1f && num <= 20f)
					{
						return new Vector3(vector.X, this.CalculateHeight(vector.X, vector.Y), vector.Y);
					}
				}
			}
			return new Vector3(this.m_oceanCorner.X, this.CalculateHeight(this.m_oceanCorner.X, this.m_oceanCorner.Y), this.m_oceanCorner.Y);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x000B32C4 File Offset: 0x000B14C4
		public void GenerateChunkContentsPass1(TerrainChunk chunk)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					chunk.SetTemperatureFast(i, j, this.CalculateTemperature((float)num, (float)num2));
					chunk.SetHumidityFast(i, j, this.CalculateHumidity((float)num, (float)num2));
					bool flag = this.CalculateOceanShoreDistance((float)num, (float)num2) >= 0f;
					int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
					for (int k = 0; k < 256; k++)
					{
						int value = Terrain.MakeBlockValue(0);
						if (flag)
						{
							if (k < 2)
							{
								value = Terrain.MakeBlockValue(1);
							}
							else if (k < this.m_worldSettings.TerrainLevel)
							{
								value = Terrain.MakeBlockValue((this.m_worldSettings.TerrainBlockIndex == 8) ? 2 : this.m_worldSettings.TerrainBlockIndex);
							}
							else if (k == this.m_worldSettings.TerrainLevel)
							{
								value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainBlockIndex);
							}
							else if (k <= this.OceanLevel)
							{
								value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainOceanBlockIndex);
							}
						}
						else if (k < 2)
						{
							value = Terrain.MakeBlockValue(1);
						}
						else if (k <= this.OceanLevel)
						{
							value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainOceanBlockIndex);
						}
						chunk.SetCellValueFast(num3 + k, value);
					}
				}
			}
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x000B343F File Offset: 0x000B163F
		public void GenerateChunkContentsPass2(TerrainChunk chunk)
		{
			this.UpdateFluidIsTop(chunk);
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x000B3448 File Offset: 0x000B1648
		public void GenerateChunkContentsPass3(TerrainChunk chunk)
		{
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x000B344A File Offset: 0x000B164A
		public void GenerateChunkContentsPass4(TerrainChunk chunk)
		{
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x000B344C File Offset: 0x000B164C
		public float CalculateOceanShoreDistance(float x, float z)
		{
			float x2 = 0f;
			float x3 = 0f;
			float y = 0f;
			float y2 = 0f;
			if (this.m_shoreRoughnessAmplitude.X > 0f)
			{
				x2 = this.m_shoreRoughnessAmplitude.X * SimplexNoise.OctavedNoise(z + this.m_shoreRoughnessOffset[0], this.m_shoreRoughnessFrequency.X, (int)this.m_shoreRoughnessOctaves.X, 2f, 0.6f, false);
				x3 = this.m_shoreRoughnessAmplitude.X * SimplexNoise.OctavedNoise(z + this.m_shoreRoughnessOffset[1], this.m_shoreRoughnessFrequency.X, (int)this.m_shoreRoughnessOctaves.X, 2f, 0.6f, false);
			}
			if (this.m_shoreRoughnessAmplitude.Y > 0f)
			{
				y = this.m_shoreRoughnessAmplitude.Y * SimplexNoise.OctavedNoise(x + this.m_shoreRoughnessOffset[2], this.m_shoreRoughnessFrequency.Y, (int)this.m_shoreRoughnessOctaves.Y, 2f, 0.6f, false);
				y2 = this.m_shoreRoughnessAmplitude.Y * SimplexNoise.OctavedNoise(x + this.m_shoreRoughnessOffset[3], this.m_shoreRoughnessFrequency.Y, (int)this.m_shoreRoughnessOctaves.Y, 2f, 0.6f, false);
			}
			Vector2 vector = this.m_oceanCorner + new Vector2(x2, y);
			Vector2 vector2 = this.m_oceanCorner + ((this.m_islandSize != null) ? this.m_islandSize.Value : new Vector2(float.MaxValue)) + new Vector2(x3, y2);
			return MathUtils.Min(x - vector.X, vector2.X - x, z - vector.Y, vector2.Y - z);
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x000B3607 File Offset: 0x000B1807
		public float CalculateHeight(float x, float z)
		{
			return (float)this.m_worldSettings.TerrainLevel;
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x000B3615 File Offset: 0x000B1815
		public int CalculateTemperature(float x, float z)
		{
			return MathUtils.Clamp(12 + (int)this.m_worldSettings.TemperatureOffset, 0, 15);
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x000B362E File Offset: 0x000B182E
		public int CalculateHumidity(float x, float z)
		{
			return MathUtils.Clamp(12 + (int)this.m_worldSettings.HumidityOffset, 0, 15);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x000B3647 File Offset: 0x000B1847
		public float CalculateMountainRangeFactor(float x, float z)
		{
			return 0f;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x000B3650 File Offset: 0x000B1850
		public void UpdateFluidIsTop(TerrainChunk chunk)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = TerrainChunk.CalculateCellIndex(i, 255, j);
					int num2 = 0;
					int k = 255;
					while (k >= 0)
					{
						int cellValueFast = chunk.GetCellValueFast(num);
						int num3 = Terrain.ExtractContents(cellValueFast);
						if (num3 != 0 && num3 != num2 && BlocksManager.Blocks[num3] is FluidBlock)
						{
							int data = Terrain.ExtractData(cellValueFast);
							chunk.SetCellValueFast(num, Terrain.MakeBlockValue(num3, 0, FluidBlock.SetIsTop(data, true)));
						}
						num2 = num3;
						k--;
						num--;
					}
				}
			}
		}

		// Token: 0x0400103C RID: 4156
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400103D RID: 4157
		public WorldSettings m_worldSettings;

		// Token: 0x0400103E RID: 4158
		public Vector2 m_oceanCorner;

		// Token: 0x0400103F RID: 4159
		public Vector2? m_islandSize;

		// Token: 0x04001040 RID: 4160
		public Vector2 m_shoreRoughnessFrequency;

		// Token: 0x04001041 RID: 4161
		public Vector2 m_shoreRoughnessAmplitude;

		// Token: 0x04001042 RID: 4162
		public Vector2 m_shoreRoughnessOctaves;

		// Token: 0x04001043 RID: 4163
		public float[] m_shoreRoughnessOffset = new float[4];
	}
}
