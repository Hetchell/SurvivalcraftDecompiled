using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine;
using Survivalcraft.Game;

namespace Game
{
	
	// Token: 0x02000311 RID: 785
	public class TerrainChunkGeneratorProviderActive : ITerrainContentsGenerator
	{

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x000AD68E File Offset: 0x000AB88E
		private ModifierHolder modifyTerrain;
        public int OceanLevel
		{
			get
			{
				return 64 + this.m_worldSettings.SeaLevelOffset;
			}
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x000AD6A0 File Offset: 0x000AB8A0
		static TerrainChunkGeneratorProviderActive()
		{
			TerrainChunkGeneratorProviderActive.CreateBrushes();
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x000AD75C File Offset: 0x000AB95C
		public TerrainChunkGeneratorProviderActive(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_subsystemBottomSuckerBlockBehavior = subsystemTerrain.Project.FindSubsystem<SubsystemBottomSuckerBlockBehavior>(true);
			SubsystemGameInfo subsystemGameInfo = subsystemTerrain.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_worldSettings = subsystemGameInfo.WorldSettings;
			this.m_seed = subsystemGameInfo.WorldSeed;
			this.m_islandSize = ((this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island) ? new Vector2?(this.m_worldSettings.IslandSize) : null);
			Game.Random random = new Game.Random(this.m_seed);
			float num = (this.m_islandSize != null) ? MathUtils.Min(this.m_islandSize.Value.X, this.m_islandSize.Value.Y) : float.MaxValue;
			this.m_oceanCorner = new Vector2(-200f, -200f);
			this.m_temperatureOffset = new Vector2(random.Float(-3000f, 3000f), random.Float(-3000f, 3000f));
			this.m_humidityOffset = new Vector2(random.Float(-3000f, 3000f), random.Float(-3000f, 3000f));
			this.m_mountainsOffset = new Vector2(random.Float(-3000f, 3000f), random.Float(-3000f, 3000f));
			this.m_riversOffset = new Vector2(random.Float(-3000f, 3000f), random.Float(-3000f, 3000f));
			this.TGBiomeScaling = ((this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island) ? 1f : 1.5f) * this.m_worldSettings.BiomeSize;
			this.TGShoreFluctuations = MathUtils.Clamp(2f * num, 0f, 150f);
			this.TGShoreFluctuationsScaling = MathUtils.Clamp(0.04f * num, 0.5f, 3f);
			this.TGOceanSlope = 0.006f *(float)Math.Pow(2, 0);
			this.TGOceanSlopeVariation = 0.004f * (float)Math.Pow(2, 0);
			this.TGIslandsFrequency = 0.01f * (float)Math.Pow(2, 0);
			this.TGDensityBias = 55f;
			this.TGHeightBias = 1f;
			this.TGRiversStrength = 1f;
			this.TGMountainsStrength = 220f;
			this.TGMountainRangeFreq = 0.0006f * (float)Math.Pow(Math.PI, 0);
			this.TGMountainsPercentage = 0.15f;
			TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq = 0.003f * (float)Math.Pow(2, 0);
			TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves = 3;
			TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence = 0.53f;
			this.TGHillsPercentage = 0.32f;
			this.TGHillsStrength = 32f;
			this.TGHillsOctaves = 1;
			this.TGHillsFrequency = 0.014f * (float)Math.Pow(2, 0);
			this.TGHillsPersistence = 0.5f;
			this.TGTurbulenceStrength = 55f;
			this.TGTurbulenceFreq = 0.03f * (float)Math.Pow(2, 0);
			this.TGTurbulenceOctaves = 8;
			this.TGTurbulencePersistence = 0.5f;
			this.TGMinTurbulence = 0.04f * (float)Math.Pow(2, 0);
			this.TGTurbulenceZero = 0.84f * (float)Math.Pow(2, 0);
			TerrainChunkGeneratorProviderActive.TGSurfaceMultiplier = 2f;// * 1728183;
			this.TGWater = true;
			this.TGExtras = true;
			this.TGCavesAndPockets = true;
			this.modifyTerrain = new ModifierHolder(subsystemTerrain, this, num, random);
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x000ADA54 File Offset: 0x000ABC54
		public Vector3 FindCoarseSpawnPosition()
		{
			Vector2 zero = Vector2.Zero;
			float num = float.MinValue;
			for (int i = 0; i < 800; i += 5)
			{
				for (int j = 0; j <= 12; j += 4)
				{
					for (int k = 0; k < 2; k++)
					{
						float num2;
						float x;
						if (k == 0)
						{
							num2 = this.m_oceanCorner.Y + (float)i;
							x = this.CalculateOceanShoreX(num2) + (float)j;
						}
						else
						{
							x = this.m_oceanCorner.X + (float)i;
							num2 = this.CalculateOceanShoreZ(x) + (float)j;
						}
						float num3 = this.ScoreSpawnPosition(Terrain.ToCell(x), Terrain.ToCell(num2));
						if (num3 > num)
						{
							zero = new Vector2(x, num2);
							num = num3;
						}
					}
				}
			}
			int u = 1371930;
			//return new Vector3(zero.X + 0, this.CalculateHeight(zero.X, zero.Y), zero.Y);
			return new Vector3(0, 300, 0);
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x000ADB2F File Offset: 0x000ABD2F
		public void GenerateChunkContentsPass1(TerrainChunk chunk)
		{
			this.GenerateSurfaceParameters(chunk, 0, 0, 16, 8);
            //this.modifyTerrain.GenerateTerrain(chunk, true);
            //this.GenerateTerrain(chunk, chunk.Origin.X, chunk.Origin.Y, 3, 3);
            //this.GenerateTerrain(chunk, 14, 27, 16, 5);
            this.GenerateTerrain(chunk, 0, 0, 16, 8);
        }

		// Token: 0x06001673 RID: 5747 RVA: 0x000ADB49 File Offset: 0x000ABD49
		public void GenerateChunkContentsPass2(TerrainChunk chunk)
		{
			this.GenerateSurfaceParameters(chunk, 0, 8, 16, 16);
			this.modifyTerrain.GenerateTerrain(chunk, false);
            //this.GenerateTerrain(chunk, 0, 8, 16, 16);
        }

		// Token: 0x06001674 RID: 5748 RVA: 0x000ADB65 File Offset: 0x000ABD65
		public void GenerateChunkContentsPass3(TerrainChunk chunk)
		{
			this.GenerateCaves(chunk);
			this.GeneratePockets(chunk);
			this.GenerateMinerals(chunk);
			Action<TerrainChunk> generateMinerals = TerrainChunkGeneratorProviderActive.GenerateMinerals2;
			if (generateMinerals != null)
			{
				generateMinerals(chunk);
			}
			this.GenerateSurface(chunk);
			this.PropagateFluidsDownwards(chunk);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x000ADB9C File Offset: 0x000ABD9C
		public void GenerateChunkContentsPass4(TerrainChunk chunk)
		{
			this.GenerateGrassAndPlants(chunk);
			this.GenerateTreesAndLogs(chunk);
			this.GenerateCacti(chunk);
			this.GeneratePumpkins(chunk);
			this.GenerateKelp(chunk);
			this.GenerateSeagrass(chunk);
			this.GenerateBottomSuckers(chunk);
			this.GenerateTraps(chunk); //possible crash causing method. 
			//this.GenerateIvy(chunk);
			this.GenerateGraves(chunk);
			this.GenerateSnowAndIce(chunk);
			this.GenerateBedrockAndAir(chunk);
			this.UpdateFluidIsTop(chunk);
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x000ADC04 File Offset: 0x000ABE04
		public float CalculateOceanShoreDistance(float x, float z)
		{
			if (this.m_islandSize != null)
			{
				float num = this.CalculateOceanShoreX(z);
				float num2 = this.CalculateOceanShoreZ(x);
				float num3 = this.CalculateOceanShoreX(z + 1000f) + this.m_islandSize.Value.X;
				float num4 = this.CalculateOceanShoreZ(x + 1000f) + this.m_islandSize.Value.Y;
				return MathUtils.Min(x - num, z - num2, num3 - x, num4 - z);
			}
			float num5 = this.CalculateOceanShoreX(z);
			float num6 = this.CalculateOceanShoreZ(x);
			return MathUtils.Min(x - num5, z - num6);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x000ADC9D File Offset: 0x000ABE9D
		public float CalculateMountainRangeFactor(float x, float z)
		{
			return SimplexNoise.OctavedNoise(x + this.m_mountainsOffset.X, z + this.m_mountainsOffset.Y, this.TGMountainRangeFreq / this.TGBiomeScaling, 3, 1.91f, 0.75f, true);
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x000ADCD8 File Offset: 0x000ABED8
		public float CalculateHeight(float x, float z)
		{
			float num = this.TGOceanSlope + this.TGOceanSlopeVariation * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.m_mountainsOffset.X, z + this.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
			float num2 = this.CalculateOceanShoreDistance(x, z);
			float num3 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(num2));
			float num4 = MathUtils.Saturate(MathUtils.Sin(this.TGIslandsFrequency * num2));
			float num5 = MathUtils.Saturate(MathUtils.Saturate((0f - num) * num2) - 0.85f * num4);
			float num6 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - num2 - 10f)) - num4);
			float v = this.CalculateMountainRangeFactor(x, z);
			float f = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.001f / this.TGBiomeScaling, 2, 2f, 0.5f, false);
			float f2 = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.0017f / this.TGBiomeScaling, 2, 4f, 0.7f, false);
			float num7 = (1f - num6) * (1f - num3) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.TGHillsPercentage, 1f - this.TGMountainsPercentage);
			float num8 = (1f - num6) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.TGMountainsPercentage, 1f);
			float num9 = 1f * SimplexNoise.OctavedNoise(x, z, this.TGHillsFrequency, this.TGHillsOctaves, 1.93f, this.TGHillsPersistence, false);
			float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f);
			float num10 = 1.5f * SimplexNoise.OctavedNoise(x, z, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
			float num11 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * num8 + 0.5f * num7 + MathUtils.Saturate(1f - num2 / 30f)));
			float x2 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(num8 + 0.5f * num7));
			float num12 = MathUtils.Saturate(1.5f - num11 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.m_riversOffset.X, z + this.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
			float num13 = -50f * num5 + this.TGHeightBias;
			float num14 = MathUtils.Lerp(0f, 8f, f);
			float num15 = MathUtils.Lerp(0f, -6f, f2);
			float num16 = this.TGHillsStrength * num7 * num9;
			float num17 = this.TGMountainsStrength * num8 * num10;
			float f3 = this.TGRiversStrength * num12;
			float num18 = num13 + num14 + num15 + num17 + num16;
			float num19 = MathUtils.Min(MathUtils.Lerp(num18, x2, f3), num18);
			return MathUtils.Clamp(64f + num19, 10f, 251f);
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x000AE00C File Offset: 0x000AC20C
		public int CalculateTemperature(float x, float z)
		{
			return MathUtils.Clamp((int)(MathUtils.Saturate(3f * SimplexNoise.OctavedNoise(x + this.m_temperatureOffset.X, z + this.m_temperatureOffset.Y, 0.0015f / this.TGBiomeScaling, 5, 2f, 0.6f, false) - 1.1f + this.m_worldSettings.TemperatureOffset / 16f) * 16f), 0, 15);
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x000AE084 File Offset: 0x000AC284
		public int CalculateHumidity(float x, float z)
		{
			return MathUtils.Clamp((int)(MathUtils.Saturate(3f * SimplexNoise.OctavedNoise(x + this.m_humidityOffset.X, z + this.m_humidityOffset.Y, 0.0012f / this.TGBiomeScaling, 5, 2f, 0.6f, false) - 0.9f + this.m_worldSettings.HumidityOffset / 16f) * 16f), 0, 15);
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x000AE0FA File Offset: 0x000AC2FA
		public static float Squish(float v, float zero, float one)
		{
			return MathUtils.Saturate((v - zero) / (one - zero));
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x000AE108 File Offset: 0x000AC308
		public float CalculateOceanShoreX(float z)
		{
			return this.m_oceanCorner.X + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(z, 0f, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f, false);
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x000AE14C File Offset: 0x000AC34C
		public float CalculateOceanShoreZ(float x)
		{
			return this.m_oceanCorner.Y + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(0f, x, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f, false);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x000AE190 File Offset: 0x000AC390
		public float ScoreSpawnPosition(int x, int z)
		{
			int num = this.CalculateTemperature((float)x, (float)z);
			int num2 = this.CalculateHumidity((float)x, (float)z);
			float num3 = this.CalculateMountainRangeFactor((float)x, (float)z);
			float num4 = this.CalculateHeight((float)x, (float)z);
			float x2 = this.CalculateHeight((float)(x - 5), (float)(z - 5));
			float x3 = this.CalculateHeight((float)(x - 5), (float)(z + 5));
			float x4 = this.CalculateHeight((float)(x + 5), (float)(z - 5));
			float x5 = this.CalculateHeight((float)(x + 5), (float)(z + 5));
			float num5 = MathUtils.Min(num4, MathUtils.Max(x2, x3, x4, x5));
			float num6 = MathUtils.Max(num4, MathUtils.Max(x2, x3, x4, x5));
			float num7 = num6 - num5;
			float num8 = 0f;
			if (MathUtils.Max(MathUtils.Abs(x), MathUtils.Abs(z)) > 400)
			{
				num8 -= 0.25f;
			}
			if (num4 < 65f)
			{
				num8 -= 1f;
			}
			if (num4 < 66f)
			{
				num8 -= 0.5f;
			}
			if (num3 > 0.9f)
			{
				num8 -= 1f;
			}
			if (num3 > 0.85f)
			{
				num8 -= 0.5f;
			}
			if (num3 > 0.8f)
			{
				num8 -= 0.25f;
			}
			StartingPositionMode startingPositionMode = this.m_subsystemTerrain.SubsystemGameInfo.WorldSettings.StartingPositionMode;
			if (startingPositionMode != StartingPositionMode.Easy)
			{
				if (startingPositionMode != StartingPositionMode.Medium)
				{
					if (num > 0)
					{
						num8 -= 0.5f;
					}
					if (num > 2)
					{
						num8 -= 1f;
					}
					if (num2 > 6)
					{
						num8 -= 0.5f;
					}
					if (num2 > 8)
					{
						num8 -= 1f;
					}
					if (num6 > 85f)
					{
						num8 -= 1f;
					}
					if (num7 > 15f)
					{
						num8 -= 1f;
					}
					if (num7 < 5f)
					{
						num8 -= 0.5f;
					}
				}
				else
				{
					if (num < 3)
					{
						num8 -= 0.5f;
					}
					if (num > 6)
					{
						num8 -= 1f;
					}
					if (num2 > 3 && num2 < 8)
					{
						num8 -= 0.5f;
					}
					if (num2 > 10)
					{
						num8 -= 0.5f;
					}
					if (num6 > 80f)
					{
						num8 -= 1f;
					}
					if (num7 > 10f)
					{
						num8 -= 1f;
					}
				}
			}
			else
			{
				if (num < 9)
				{
					num8 -= 0.5f;
				}
				if (num < 7)
				{
					num8 -= 1f;
				}
				if (num2 > 2 && num2 < 10)
				{
					num8 -= 0.5f;
				}
				if (num6 > 75f)
				{
					num8 -= 1f;
				}
				if (num7 > 5f)
				{
					num8 -= 1f;
				}
			}
			return num8;
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x000AE410 File Offset: 0x000AC610
		public void GenerateSurfaceParameters(TerrainChunk chunk, int x1, int z1, int x2, int z2)
		{
			for (int i = x1; i < x2; i++)
			{
				for (int j = z1; j < z2; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					int temperature = this.CalculateTemperature((float)num, (float)num2);
					int humidity = this.CalculateHumidity((float)num, (float)num2);
					chunk.SetTemperatureFast(i, j, temperature);
					chunk.SetHumidityFast(i, j, humidity);
				}
			}
		}

        // Token: 0x06001680 RID: 5760 RVA: 0x000AE480 File Offset: 0x000AC680
		//This is hijacked by ModifierHolder.
        public void GenerateTerrain(TerrainChunk chunk, int x1, int z1, int x2, int z2)
        {
            int num = x2 - x1;
            int num2 = z2 - z1;
            Terrain terrain = this.m_subsystemTerrain.Terrain;
            int num3 = chunk.Origin.X + x1;
            int num4 = chunk.Origin.Y + z1;
            TerrainChunkGeneratorProviderActive.Grid2d grid2d = new TerrainChunkGeneratorProviderActive.Grid2d(num, num2);
            TerrainChunkGeneratorProviderActive.Grid2d grid2d2 = new TerrainChunkGeneratorProviderActive.Grid2d(num, num2);
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    grid2d.Set(j, i, this.CalculateOceanShoreDistance((float)(j + num3), (float)(i + num4)));
                    grid2d2.Set(j, i, this.CalculateMountainRangeFactor((float)(j + num3), (float)(i + num4)));
                }
            }
            TerrainChunkGeneratorProviderActive.Grid3d grid3d = new TerrainChunkGeneratorProviderActive.Grid3d(num / 4 + 1, 33, num2 / 4 + 1);
            for (int k = 0; k < grid3d.SizeX; k++)
            {
                for (int l = 0; l < grid3d.SizeZ; l++)
                {
                    int num5 = k * 4 + num3;
                    int num6 = l * 4 + num4;
                    float num7 = this.CalculateHeight((float)num5, (float)num6);
                    float v = this.CalculateMountainRangeFactor((float)num5, (float)num6);
                    float num8 = MathUtils.Lerp(this.TGMinTurbulence, 1f, TerrainChunkGeneratorProviderActive.Squish(v, this.TGTurbulenceZero, 1f));
                    for (int m = 0; m < grid3d.SizeY; m++)
                    {
                        int num9 = m * 8;
                        float num10 = this.TGTurbulenceStrength * num8 * MathUtils.Saturate(num7 - (float)num9) * (2f * SimplexNoise.OctavedNoise((float)num5, (float)num9, (float)num6, this.TGTurbulenceFreq, this.TGTurbulenceOctaves, 4f, this.TGTurbulencePersistence, false) - 1f);
                        float num11 = (float)num9 + num10;
                        float num12 = num7 - num11;
                        num12 += MathUtils.Max(4f * (this.TGDensityBias - (float)num9), 0f);
                        grid3d.Set(k, m, l, num12);
                    }
                }
            }
            int oceanLevel = this.OceanLevel;
            for (int n = 0; n < grid3d.SizeX - 1; n++)
            {
                for (int num13 = 0; num13 < grid3d.SizeZ - 1; num13++)
                {
                    for (int num14 = 0; num14 < grid3d.SizeY - 1; num14++)
                    {
                        float num15;
                        float num16;
                        float num17;
                        float num18;
                        float num19;
                        float num20;
                        float num21;
                        float num22;
                        grid3d.Get8(n, num14, num13, out num15, out num16, out num17, out num18, out num19, out num20, out num21, out num22);
                        float num23 = (num16 - num15) / 4f;
                        float num24 = (num18 - num17) / 4f;
                        float num25 = (num20 - num19) / 4f;
                        float num26 = (num22 - num21) / 4f;
                        float num27 = num15;
                        float num28 = num17;
                        float num29 = num19;
                        float num30 = num21;
                        for (int num31 = 0; num31 < 4; num31++)
                        {
                            float num32 = (num29 - num27) / 4f;
                            float num33 = (num30 - num28) / 4f;
                            float num34 = num27;
                            float num35 = num28;
                            for (int num36 = 0; num36 < 4; num36++)
                            {
                                float num37 = (num35 - num34) / 8f;
                                float num38 = num34;
                                int num39 = num31 + n * 4;
                                int num40 = num36 + num13 * 4;
                                int x3 = x1 + num39;
                                int z3 = z1 + num40;
                                float x4 = grid2d.Get(num39, num40);
                                float num41 = grid2d2.Get(num39, num40);
                                int temperatureFast = chunk.GetTemperatureFast(x3, z3);
                                int humidityFast = chunk.GetHumidityFast(x3, z3);
                                float f = num41 - 0.01f * (float)humidityFast;
                                float num42 = MathUtils.Lerp(100f, 0f, f);
                                float num43 = MathUtils.Lerp(300f, 30f, f);
                                bool flag = (temperatureFast > 8 && humidityFast < 8 && num41 < 0.97f) || (MathUtils.Abs(x4) < 16f && num41 < 0.97f);
                                int num44 = TerrainChunk.CalculateCellIndex(x3, 0, z3);
                                for (int num45 = 0; num45 < 8; num45++)
                                {
                                    int num46 = num45 + num14 * 8;
                                    int value = 0;
                                    if (num38 < 0f)
                                    {
                                        if (num46 <= oceanLevel)
                                        {
                                            value = 18;
                                        }
                                    }
                                    else
                                    {
                                        value = ((!flag) ? ((num38 >= num43) ? 67 : 3) : ((num38 >= num42) ? ((num38 >= num43) ? 67 : 3) : 4));
                                    }
                                    chunk.SetCellValueFast(num44 + num46, value);
                                    num38 += num37;
                                }
                                num34 += num32;
                                num35 += num33;
                            }
                            num27 += num23;
                            num28 += num24;
                            num29 += num25;
                            num30 += num26;
                        }
                    }
                }
            }
        }

        // Token: 0x06001681 RID: 5761 RVA: 0x000AE8E4 File Offset: 0x000ACAE4
        public void GenerateSurface(TerrainChunk chunk)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			Game.Random random = new Game.Random(this.m_seed + chunk.Coords.X + 101 * chunk.Coords.Y);
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					int num3 = TerrainChunk.CalculateCellIndex(i, 254, j);
					int k = 254;
					while (k >= 0)
					{
						int num4 = Terrain.ExtractContents(chunk.GetCellValueFast(num3));
						if (!BlocksManager.Blocks[num4].IsTransparent)
						{
							float num5 = this.CalculateMountainRangeFactor((float)num, (float)num2);
							int temperature = terrain.GetTemperature(num, num2);
							int humidity = terrain.GetHumidity(num, num2);
							int num6;
							if (num4 == 4)
							{
								num6 = ((temperature > 4 && temperature < 7) ? 6 : 7);
							}
							else
							{
								int num7 = temperature / 4;
								int num8 = (k + 1 < 255) ? chunk.GetCellContentsFast(i, k + 1, j) : 0;
								num6 = ((k > 120 && SubsystemWeather.IsPlaceFrozen(temperature, k)) ? 62 : (((k < 66 || k == 84 + num7 || k == 103 + num7) && humidity == 9 && temperature % 6 == 1) ? 66 : ((num8 != 18 || humidity <= 8 || humidity % 2 != 0 || temperature % 3 != 0) ? 2 : 72)));
							}
							int num9;
							if (num6 == 62)
							{
								num9 = (int)MathUtils.Clamp(1f * (float)(-(float)temperature), 1f, 7f);
							}
							else
							{
								float num10 = MathUtils.Saturate(((float)k - 100f) * 0.05f);
								float f = MathUtils.Saturate(MathUtils.Saturate((num5 - 0.9f) / 0.1f) - MathUtils.Saturate(((float)humidity - 3f) / 12f) + TerrainChunkGeneratorProviderActive.TGSurfaceMultiplier * num10);
								int min = (int)MathUtils.Lerp(4f, 0f, f);
								int max = (int)MathUtils.Lerp(7f, 0f, f);
								num9 = MathUtils.Min(random.Int(min, max), k);
							}
							int num11 = TerrainChunk.CalculateCellIndex(i, k + 1, j);
							for (int l = num11 - num9; l < num11; l++)
							{
								if (Terrain.ExtractContents(chunk.GetCellValueFast(l)) != 0)
								{
									int value = Terrain.ReplaceContents(0, num6);
									chunk.SetCellValueFast(l, value);
								}
							}
							break;
						}
						k--;
						num3--;
					}
				}
			}
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x000AEB6C File Offset: 0x000ACD6C
		public void GenerateMinerals(TerrainChunk chunk)
		{
			if (!this.TGCavesAndPockets)
			{
				return;
			}
			if (TerrainChunkGeneratorProviderActive.GenerateMinerals1 != null)
			{
				TerrainChunkGeneratorProviderActive.GenerateMinerals1(chunk);
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 1; j <= y + 1; j++)
				{
					Game.Random random = new Game.Random(this.m_seed + i + 119 * j);
					int num = random.Int(0, 10);
					for (int k = 0; k < num; k++)
					{
						random.Int(0, 1);
					}
					float num2 = this.CalculateMountainRangeFactor((float)(i * 16), (float)(j * 16));
					int num3 = (int)(5f + 3f * num2 * SimplexNoise.OctavedNoise((float)i, (float)j, 0.33f, 1, 1f, 1f, false));
					for (int l = 0; l < num3; l++)
					{
						int x2 = i * 16 + random.Int(0, 15);
						int y2 = random.Int(5, 200);
						int z = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_coalBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_coalBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z, 3);
					}
					int num4 = (int)(6f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 1211), (float)(j + 396), 0.33f, 1, 1f, 1f, false));
					for (int m = 0; m < num4; m++)
					{
						int x3 = i * 16 + random.Int(0, 15);
						int y3 = random.Int(20, 65);
						int z2 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_copperBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_copperBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z2, 3);
					}
					int num5 = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 713), (float)(j + 211), 0.33f, 1, 1f, 1f, false));
					for (int n = 0; n < num5; n++)
					{
						int x4 = i * 16 + random.Int(0, 15);
						int y4 = random.Int(2, 40);
						int z3 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_ironBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_ironBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z3, 67);
					}
					int num6 = (int)(3f + 3f * num2 * SimplexNoise.OctavedNoise((float)(i + 915), (float)(j + 272), 0.33f, 1, 1f, 1f, false));
					for (int num7 = 0; num7 < num6; num7++)
					{
						int x5 = i * 16 + random.Int(0, 15);
						int y5 = random.Int(50, 90);
						int z4 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_saltpeterBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_saltpeterBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z4, 4);
					}
					int num8 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 711), (float)(j + 1194), 0.33f, 1, 1f, 1f, false));
					for (int num9 = 0; num9 < num8; num9++)
					{
						int x6 = i * 16 + random.Int(0, 15);
						int y6 = random.Int(2, 40);
						int z5 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_sulphurBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_sulphurBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z5, 67);
					}
					int num10 = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 432), (float)(j + 907), 0.33f, 1, 1f, 1f, false));
					for (int num11 = 0; num11 < num10; num11++)
					{
						int x7 = i * 16 + random.Int(0, 15);
						int y7 = random.Int(2, 15);
						int z6 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_diamondBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_diamondBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z6, 67);
					}
					int num12 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 799), (float)(j + 131), 0.33f, 1, 1f, 1f, false));
					for (int num13 = 0; num13 < num12; num13++)
					{
						int x8 = i * 16 + random.Int(0, 15);
						int y8 = random.Int(2, 50);
						int z7 = j * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_germaniumBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_germaniumBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z7, 67);
					}
				}
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x000AF094 File Offset: 0x000AD294
		public void GeneratePockets(TerrainChunk chunk)
		{
			if (!this.TGCavesAndPockets)
			{
				return;
			}
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int num = i + chunk.Coords.X;
					int num2 = j + chunk.Coords.Y;
					Game.Random random = new Game.Random(this.m_seed + num + 71 * num2);
					int num3 = random.Int(0, 10);
					for (int k = 0; k < num3; k++)
					{
						random.Int(0, 1);
					}
					float num4 = this.CalculateMountainRangeFactor((float)(num * 16), (float)(num2 * 16));
					for (int l = 0; l < 3; l++)
					{
						int x = num * 16 + random.Int(0, 15);
						int y = random.Int(50, 100);
						int z = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_dirtPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_dirtPocketBrushes.Count - 1)].PaintFastSelective(chunk, x, y, z, 3);
					}
					for (int m = 0; m < 10; m++)
					{
						int x2 = num * 16 + random.Int(0, 15);
						int y2 = random.Int(20, 80);
						int z2 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_gravelPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_gravelPocketBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z2, 3);
					}
					for (int n = 0; n < 2; n++)
					{
						int x3 = num * 16 + random.Int(0, 15);
						int y3 = random.Int(20, 120);
						int z3 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_limestonePocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_limestonePocketBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z3, 3);
					}
					for (int num5 = 0; num5 < 1; num5++)
					{
						int x4 = num * 16 + random.Int(0, 15);
						int y4 = random.Int(50, 70);
						int z4 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_clayPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_clayPocketBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z4, 3);
					}
					for (int num6 = 0; num6 < 6; num6++)
					{
						int x5 = num * 16 + random.Int(0, 15);
						int y5 = random.Int(40, 80);
						int z5 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_sandPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_sandPocketBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z5, 4);
					}
					for (int num7 = 0; num7 < 4; num7++)
					{
						int x6 = num * 16 + random.Int(0, 15);
						int y6 = random.Int(40, 60);
						int z6 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_basaltPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z6, 4);
					}
					for (int num8 = 0; num8 < 3; num8++)
					{
						int x7 = num * 16 + random.Int(0, 15);
						int y7 = random.Int(20, 40);
						int z7 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_basaltPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z7, 3);
					}
					for (int num9 = 0; num9 < 6; num9++)
					{
						int x8 = num * 16 + random.Int(0, 15);
						int y8 = random.Int(4, 50);
						int z8 = num2 * 16 + random.Int(0, 15);
						TerrainChunkGeneratorProviderActive.m_granitePocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_granitePocketBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z8, 67);
					}
					if (random.Bool(0.02f + 0.01f * num4))
					{
						int num10 = num * 16;
						int num11 = random.Int(40, 60);
						int num12 = num2 * 16;
						int num13 = random.Int(1, 3);
						for (int num14 = 0; num14 < num13; num14++)
						{
							Vector2 vector = random.Vector2(7f);
							int num15 = 8 + (int)MathUtils.Round(vector.X);
							int num16 = 0;
							int num17 = 8 + (int)MathUtils.Round(vector.Y);
							TerrainChunkGeneratorProviderActive.m_waterPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_waterPocketBrushes.Count - 1)].PaintFast(chunk, num10 + num15, num11 + num16, num12 + num17);
						}
					}
					if (random.Bool(0.06f + 0.05f * num4))
					{
						int num18 = num * 16;
						int num19 = random.Int(15, 42);
						int num20 = num2 * 16;
						int num21 = random.Int(1, 2);
						for (int num22 = 0; num22 < num21; num22++)
						{
							Vector2 vector2 = random.Vector2(7f);
							int num23 = 8 + (int)MathUtils.Round(vector2.X);
							int num24 = random.Int(0, 1);
							int num25 = 8 + (int)MathUtils.Round(vector2.Y);
							TerrainChunkGeneratorProviderActive.m_magmaPocketBrushes[random.Int(0, TerrainChunkGeneratorProviderActive.m_magmaPocketBrushes.Count - 1)].PaintFast(chunk, num18 + num23, num19 + num24, num20 + num25);
						}
					}
				}
			}
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x000AF5FC File Offset: 0x000AD7FC
		public void GenerateCaves(TerrainChunk chunk)
		{
			if (!this.TGCavesAndPockets)
			{
				return;
			}
			List<TerrainChunkGeneratorProviderActive.CavePoint> list = new List<TerrainChunkGeneratorProviderActive.CavePoint>();
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			for (int i = x - 2; i <= x + 2; i++)
			{
				for (int j = y - 2; j <= y + 2; j++)
				{
					list.Clear();
					Game.Random random = new Game.Random(this.m_seed + i + 9973 * j);
					int num = i * 16 + random.Int(0, 15);
					int num2 = j * 16 + random.Int(0, 15);
					float probability = 0.5f;
					if (random.Bool(probability))
					{
						int num3 = (int)this.CalculateHeight((float)num, (float)num2);
						int num4 = (int)this.CalculateHeight((float)(num + 3), (float)num2);
						int num5 = (int)this.CalculateHeight((float)num, (float)(num2 + 3));
						Vector3 position = new Vector3((float)num, (float)(num3 - 1), (float)num2);
						Vector3 v = new Vector3(3f, (float)(num4 - num3), 0f);
						Vector3 v2 = new Vector3(0f, (float)(num5 - num3), 3f);
						Vector3 vector = Vector3.Normalize(Vector3.Cross(v, v2));
						if (vector.Y > -0.6f)
						{
							list.Add(new TerrainChunkGeneratorProviderActive.CavePoint
							{
								Position = position,
								Direction = vector,
								BrushType = 0,
								Length = random.Int(80, 240)
							});
						}
						int num6 = i * 16 + 8;
						int num7 = j * 16 + 8;
						int k = 0;
						while (k < list.Count)
						{
							TerrainChunkGeneratorProviderActive.CavePoint cavePoint = list[k];
							List<TerrainBrush> list2 = TerrainChunkGeneratorProviderActive.m_caveBrushesByType[cavePoint.BrushType];
							list2[random.Int(0, list2.Count - 1)].PaintFastAvoidWater(chunk, Terrain.ToCell(cavePoint.Position.X), Terrain.ToCell(cavePoint.Position.Y), Terrain.ToCell(cavePoint.Position.Z));
							cavePoint.Position += 2f * cavePoint.Direction;
							cavePoint.StepsTaken += 2;
							float num8 = cavePoint.Position.X - (float)num6;
							float num9 = cavePoint.Position.Z - (float)num7;
							if (random.Bool(0.5f))
							{
								Vector3 vector2 = Vector3.Normalize(random.Vector3(1f));
								if ((num8 < -25.5f && vector2.X < 0f) || (num8 > 25.5f && vector2.X > 0f))
								{
									vector2.X = 0f - vector2.X;
								}
								if ((num9 < -25.5f && vector2.Z < 0f) || (num9 > 25.5f && vector2.Z > 0f))
								{
									vector2.Z = 0f - vector2.Z;
								}
								if ((cavePoint.Direction.Y < -0.5f && vector2.Y < -10f) || (cavePoint.Direction.Y > 0.1f && vector2.Y > 0f))
								{
									vector2.Y = 0f - vector2.Y;
								}
								cavePoint.Direction = Vector3.Normalize(cavePoint.Direction + 0.5f * vector2);
							}
							if (cavePoint.StepsTaken > 20 && random.Bool(0.06f))
							{
								cavePoint.Direction = Vector3.Normalize(random.Vector3(1f) * new Vector3(1f, 0.33f, 1f));
							}
							if (cavePoint.StepsTaken > 20 && random.Bool(0.05f))
							{
								cavePoint.Direction.Y = 0f;
								cavePoint.BrushType = MathUtils.Min(cavePoint.BrushType + 2, TerrainChunkGeneratorProviderActive.m_caveBrushesByType.Count - 1);
							}
							if (cavePoint.StepsTaken > 30 && random.Bool(0.03f))
							{
								cavePoint.Direction.X = 0f;
								cavePoint.Direction.Y = -1f;
								cavePoint.Direction.Z = 0f;
							}
							if (cavePoint.StepsTaken > 30 && cavePoint.Position.Y < 30f && random.Bool(0.02f))
							{
								cavePoint.Direction.X = 0f;
								cavePoint.Direction.Y = 1f;
								cavePoint.Direction.Z = 0f;
							}
							if (random.Bool(0.33f))
							{
								cavePoint.BrushType = (int)(MathUtils.Pow(random.Float(0f, 0.999f), 7f) * (float)TerrainChunkGeneratorProviderActive.m_caveBrushesByType.Count);
							}
							if (random.Bool(0.06f) && list.Count < 12 && cavePoint.StepsTaken > 20 && cavePoint.Position.Y < 58f)
							{
								list.Add(new TerrainChunkGeneratorProviderActive.CavePoint
								{
									Position = cavePoint.Position,
									Direction = Vector3.Normalize(random.Vector3(1f, 1f) * new Vector3(1f, 0.33f, 1f)),
									BrushType = (int)(MathUtils.Pow(random.Float(0f, 0.999f), 7f) * (float)TerrainChunkGeneratorProviderActive.m_caveBrushesByType.Count),
									Length = random.Int(40, 180)
								});
							}
							if (cavePoint.StepsTaken >= cavePoint.Length || MathUtils.Abs(num8) > 34f || MathUtils.Abs(num9) > 34f || cavePoint.Position.Y < 5f || cavePoint.Position.Y > 246f)
							{
								k++;
							}
							else if (cavePoint.StepsTaken % 20 == 0)
							{
								float num10 = this.CalculateHeight(cavePoint.Position.X, cavePoint.Position.Z);
								if (cavePoint.Position.Y > num10 + 1f)
								{
									k++;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x000AFC74 File Offset: 0x000ADE74
		public void GenerateTreesAndLogs(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int x = chunk.Origin.X;
			int num = x + 16;
			int y = chunk.Origin.Y;
			int num2 = y + 16;
			int x2 = chunk.Coords.X;
			int y2 = chunk.Coords.Y;
			for (int i = x2; i <= x2; i++)
			{
				for (int j = y2; j <= y2; j++)
				{
					Game.Random random = new Game.Random(this.m_seed + i + 3943 * j);
					int humidity = this.CalculateHumidity((float)(i * 16), (float)(j * 16));
					int num3 = this.CalculateTemperature((float)(i * 16), (float)(j * 16));
					float num4 = MathUtils.Saturate((SimplexNoise.OctavedNoise((float)i, (float)j, 0.1f, 2, 2f, 0.5f, false) - 0.25f) / 0.2f + (random.Bool(0.25f) ? 0.5f : 0f));
					int num5 = 0;
					if (num4 > 0.9f)
					{
						num5 = random.Int(1, 2);
					}
					else if (num4 > 0.6f)
					{
						num5 = random.Int(0, 1);
					}
					int num6 = 0;
					int num7 = 0;
					while (num7 < 12 && num6 < num5)
					{
						int num8 = i * 16 + random.Int(0, 15);
						int num9 = j * 16 + random.Int(0, 15);
						int num10 = terrain.CalculateTopmostCellHeight(num8, num9);
						if (num10 >= 66)
						{
							int cellContentsFast = terrain.GetCellContentsFast(num8, num10, num9);
							if (cellContentsFast == 2 || cellContentsFast == 8)
							{
								num10++;
								int num11 = random.Int(3, 7);
								Point3 point = CellFace.FaceToPoint3(random.Int(0, 3));
								if (point.X < 0 && num8 - num11 + 1 < 0)
								{
									point.X *= -1;
								}
								if (point.X > 0 && num8 + num11 - 1 > 15)
								{
									point.X *= -1;
								}
								if (point.Z < 0 && num9 - num11 + 1 < 0)
								{
									point.Z *= -1;
								}
								if (point.Z > 0 && num9 + num11 - 1 > 15)
								{
									point.Z *= -1;
								}
								bool flag = true;
								bool flag2 = false;
								bool flag3 = false;
								for (int k = 0; k < num11; k++)
								{
									int num12 = num8 + point.X * k;
									int num13 = num9 + point.Z * k;
									if (num12 < x + 1 || num12 >= num - 1 || num13 < y + 1 || num13 >= num2 - 1)
									{
										flag = false;
										break;
									}
									if (BlocksManager.Blocks[terrain.GetCellContentsFast(num12, num10, num13)].IsCollidable)
									{
										flag = false;
										break;
									}
									if (BlocksManager.Blocks[terrain.GetCellContentsFast(num12, num10 - 1, num13)].IsCollidable)
									{
										if (k <= MathUtils.Max(num11 / 2, 0))
										{
											flag2 = true;
										}
										if (k >= MathUtils.Min(num11 / 2 + 1, num11 - 1))
										{
											flag3 = true;
										}
									}
								}
								if (flag && flag2 && flag3)
								{
									Point3 point2 = (point.X != 0) ? new Point3(0, 0, 1) : new Point3(1, 0, 0);
									TreeType? treeType = PlantsManager.GenerateRandomTreeType(random, num3 + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num10), humidity, num10, 2f);
									if (treeType != null)
									{
										int value = PlantsManager.GetTreeTrunkValue(treeType.Value);
										value = Terrain.ReplaceData(value, WoodBlock.SetCutFace(Terrain.ExtractData(value), (point.X != 0) ? 1 : 0));
										int treeLeavesValue = PlantsManager.GetTreeLeavesValue(treeType.Value);
										for (int l = 0; l < num11; l++)
										{
											int num14 = num8 + point.X * l;
											int num15 = num9 + point.Z * l;
											terrain.SetCellValueFast(num14, num10, num15, value);
											if (l > num11 / 2)
											{
												if (random.Bool(0.5f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 + point2.X, num10, num15 + point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 + point2.X, num10, num15 + point2.Z, treeLeavesValue);
												}
												if (random.Bool(0.05f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 + point2.X, num10, num15 + point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 + point2.X, num10, num15 + point2.Z, value);
												}
												if (random.Bool(0.5f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 - point2.X, num10, num15 - point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 - point2.X, num10, num15 - point2.Z, treeLeavesValue);
												}
												if (random.Bool(0.05f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 - point2.X, num10, num15 - point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 - point2.X, num10, num15 - point2.Z, value);
												}
												if (random.Bool(0.5f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14, num10 + 1, num15)].IsCollidable)
												{
													terrain.SetCellValueFast(num14, num10 + 1, num15, treeLeavesValue);
												}
											}
										}
									}
									num6++;
								}
							}
						}
						num7++;
					}
					int num16 = (int)(5f * num4);
					int num17 = 0;
					int num18 = 0;
					while (num18 < 32 && num17 < num16)
					{
						int num19 = i * 16 + random.Int(2, 13);
						int num20 = j * 16 + random.Int(2, 13);
						int num21 = terrain.CalculateTopmostCellHeight(num19, num20);
						if (num21 >= 66)
						{
							int cellContentsFast2 = terrain.GetCellContentsFast(num19, num21, num20);
							if (cellContentsFast2 == 2 || cellContentsFast2 == 8)
							{
								num21++;
								if (!BlocksManager.Blocks[terrain.GetCellContentsFast(num19 + 1, num21, num20)].IsCollidable && !BlocksManager.Blocks[terrain.GetCellContentsFast(num19 - 1, num21, num20)].IsCollidable && !BlocksManager.Blocks[terrain.GetCellContentsFast(num19, num21, num20 + 1)].IsCollidable && !BlocksManager.Blocks[terrain.GetCellContentsFast(num19, num21, num20 - 1)].IsCollidable)
								{
									TreeType? treeType2 = PlantsManager.GenerateRandomTreeType(random, num3 + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num21), humidity, num21, 1f);
									if (treeType2 != null)
									{
										ReadOnlyList<TerrainBrush> treeBrushes = PlantsManager.GetTreeBrushes(treeType2.Value);
										treeBrushes[random.Int(treeBrushes.Count)].PaintFast(chunk, num19, num21, num20);
									}
									num17++;
								}
							}
						}
						num18++;
					}
				}
			}
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x000B035C File Offset: 0x000AE55C
		public void GenerateBedrockAndAir(TerrainChunk chunk)
		{
			int value = Terrain.MakeBlockValue(1);
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					float num3 = (float)(2 + (int)(4f * SimplexNoise.OctavedNoise((float)num, (float)num2, 0.1f, 1, 1f, 1f, false)));
					int num4 = 0;
					while ((float)num4 < num3)
					{
						chunk.SetCellValueFast(i, num4, j, value);
						num4++;
					}
					chunk.SetCellValueFast(i, 255, j, 0);
				}
			}
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x000B0400 File Offset: 0x000AE600
		public void GenerateGrassAndPlants(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			Game.Random random = new Game.Random(this.m_seed + chunk.Coords.X + 3943 * chunk.Coords.Y);
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int k = 254;
					while (k >= 0)
					{
						int cellValueFast = chunk.GetCellValueFast(i, k, j);
						int num = Terrain.ExtractContents(cellValueFast);
						if (num != 0)
						{
							if (BlocksManager.Blocks[num] is FluidBlock)
							{
								break;
							}
							int temperatureFast = chunk.GetTemperatureFast(i, j);
							int humidityFast = chunk.GetHumidityFast(i, j);
							int num2 = PlantsManager.GenerateRandomPlantValue(random, cellValueFast, temperatureFast, humidityFast, k + 1);
							if (num2 != 0)
							{
								chunk.SetCellValueFast(i, k + 1, j, num2);
							}
							if (num == 2)
							{
								chunk.SetCellValueFast(i, k, j, Terrain.MakeBlockValue(8, 0, 0));
								break;
							}
							break;
						}
						else
						{
							k--;
						}
					}
				}
			}
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x000B04EC File Offset: 0x000AE6EC
		public void GenerateBottomSuckers(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			Game.Random random = new Game.Random(this.m_seed + chunk.Coords.X + 2210 * chunk.Coords.Y);
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					if (random.Bool(0.2f))
					{
						int num = chunk.Origin.X + i;
						int num2 = chunk.Origin.Y + j;
						int temperatureFast = chunk.GetTemperatureFast(i, j);
						if (this.CalculateOceanShoreDistance((float)num, (float)num2) <= 10f)
						{
							int num3 = 0;
							for (int k = 254; k >= 0; k--)
							{
								if (Terrain.ExtractContents(chunk.GetCellValueFast(i, k, j)) == 18)
								{
									num3++;
									int face = random.Int(0, 5);
									Point3 point = CellFace.FaceToPoint3(face);
									if (i + point.X >= 0 && i + point.X < 16 && k + point.Y >= 0 && k + point.Y < 254 && j + point.Z >= 0 && j + point.Z < 16)
									{
										int cellValueFast = chunk.GetCellValueFast(i + point.X, k + point.Y, j + point.Z);
										if (this.m_subsystemBottomSuckerBlockBehavior.IsSupport(cellValueFast, CellFace.OppositeFace(face)))
										{
											int num4 = 0;
											float num5 = 0.6f;
											float num6 = 0.4f;
											if (temperatureFast < 8)
											{
												num5 = 0.9f;
												num6 = 0.1f;
											}
											if (num3 > 6)
											{
												num5 *= 0.25f;
											}
											if (num3 > 12)
											{
												num6 *= 0.5f;
											}
											if (num3 < 4)
											{
												num6 *= 0.5f;
											}
											if (k < 45)
											{
												num5 *= 0.1f;
												num6 *= 0.1f;
											}
											float num7 = random.Float(0f, 1f);
											num7 -= num5;
											if (num4 == 0 && num7 < 0f)
											{
												num4 = 226;
											}
											num7 -= num6;
											if (num4 == 0 && num7 < 0f)
											{
												num4 = 229;
											}
											if (num4 != 0)
											{
												int face2 = random.Int(0, 3);
												int data = BottomSuckerBlock.SetFace(BottomSuckerBlock.SetSubvariant(0, face2), CellFace.OppositeFace(face));
												int value = Terrain.MakeBlockValue(num4, 0, data);
												chunk.SetCellValueFast(i, k, j, value);
											}
										}
									}
								}
								else
								{
									num3 = 0;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x000B0780 File Offset: 0x000AE980
		public void GenerateCacti(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			Game.Random random = new Game.Random(this.m_seed + x + 1991 * y);
			if (!random.Bool(0.5f))
			{
				return;
			}
			int num = random.Int(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.Int(3, 12);
				int num3 = random.Int(3, 12);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast < 6 && temperatureFast > 8)
				{
					for (int j = 0; j < 8; j++)
					{
						int num4 = num2 + random.Int(-2, 2);
						int num5 = num3 + random.Int(-2, 2);
						int k = 251;
						while (k >= 0)
						{
							int num6 = Terrain.ExtractContents(chunk.GetCellValueFast(num4, k, num5));
							if (num6 != 0)
							{
								if (num6 == 7)
								{
									int num7 = k + 1;
									while (num7 <= k + 3 && chunk.GetCellContentsFast(num4 + 1, num7, num5) == 0 && chunk.GetCellContentsFast(num4 - 1, num7, num5) == 0 && chunk.GetCellContentsFast(num4, num7, num5 + 1) == 0)
									{
										if (chunk.GetCellContentsFast(num4, num7, num5 - 1) != 0)
										{
											break;
										}
										chunk.SetCellValueFast(num4, num7, num5, Terrain.MakeBlockValue(127));
										num7++;
									}
									break;
								}
								break;
							}
							else
							{
								k--;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x000B0904 File Offset: 0x000AEB04
		public void GeneratePumpkins(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			Game.Random random = new Game.Random(this.m_seed + x + 1495 * y);
			if (!random.Bool(0.2f))
			{
				return;
			}
			int num = random.Int(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.Int(1, 14);
				int num3 = random.Int(1, 14);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast >= 10 && temperatureFast > 6)
				{
					for (int j = 0; j < 5; j++)
					{
						int x2 = num2 + random.Int(-1, 1);
						int z = num3 + random.Int(-1, 1);
						int k = 254;
						while (k >= 0)
						{
							int num4 = Terrain.ExtractContents(chunk.GetCellValueFast(x2, k, z));
							if (num4 != 0)
							{
								if (num4 == 8)
								{
									chunk.SetCellValueFast(x2, k + 1, z, random.Bool(0.25f) ? Terrain.MakeBlockValue(244) : Terrain.MakeBlockValue(131));
									break;
								}
								break;
							}
							else
							{
								k--;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x000B0A4C File Offset: 0x000AEC4C
		public void GenerateKelp(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			Game.Random random = new Game.Random(0);
			float num = 0f;
			for (int i = 0; i < 9; i++)
			{
				int num2 = i % 3 - 1;
				int num3 = i / 3 - 1;
				random.Seed(this.m_seed + (x + num2) + 850 * (y + num3));
				if (random.Bool(0.2f))
				{
					num = MathUtils.Max(num, 0.025f);
					if (i == 4)
					{
						num = MathUtils.Max(num, 0.1f);
					}
				}
			}
			if (num == 0f)
			{
				return;
			}
			random.Seed(this.m_seed + x + 850 * y);
			int num4 = random.Int(0, MathUtils.Max((int)(256f * num), 1));
			for (int j = 0; j < num4; j++)
			{
				int num5 = random.Int(2, 13);
				int num6 = random.Int(2, 13);
				int num7 = num5 + chunk.Origin.X;
				int num8 = num6 + chunk.Origin.Y;
				int num9 = random.Int(10, 26);
				int num10 = 6;
				bool flag = true;
				if (this.CalculateOceanShoreDistance((float)num7, (float)num8) > 5f)
				{
					num10 = 4;
					flag = false;
				}
				if (num9 > 0)
				{
					for (int k = 0; k < num9; k++)
					{
						int x2 = num5 + random.Int(-2, 2);
						int z = num6 + random.Int(-2, 2);
						int num11 = 0;
						for (int l = 254; l >= 0; l--)
						{
							int num12 = Terrain.ExtractContents(chunk.GetCellValueFast(x2, l, z));
							Block block = BlocksManager.Blocks[num12];
							if (num12 != 0)
							{
								if (!(block is WaterBlock))
								{
									if ((num12 == 2 || num12 == 7 || num12 == 72) && num11 >= 2)
									{
										int num13 = flag ? random.Int(num11 - 2, num11 - 1) : random.Int(num11 - 1, num11);
										for (int m = 0; m < num13; m++)
										{
											chunk.SetCellValueFast(x2, l + 1 + m, z, Terrain.MakeBlockValue(232));
										}
										break;
									}
									break;
								}
								else
								{
									num11++;
									if (num11 > num10)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x000B0C90 File Offset: 0x000AEE90
		public void GenerateSeagrass(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			Game.Random random = new Game.Random(this.m_seed + x + 378 * y);
			for (int i = 0; i < 6; i++)
			{
				int num = random.Int(1, 14);
				int num2 = random.Int(1, 14);
				int num3 = chunk.Origin.X + num;
				int num4 = chunk.Origin.Y + num2;
				bool flag = this.CalculateOceanShoreDistance((float)num3, (float)num4) < 10f;
				int num5 = random.Int(1, 3);
				for (int j = 0; j < num5; j++)
				{
					int x2 = num + random.Int(-1, 1);
					int z = num2 + random.Int(-1, 1);
					int num6 = 0;
					for (int k = 254; k >= 0; k--)
					{
						int num7 = Terrain.ExtractContents(chunk.GetCellValueFast(x2, k, z));
						if (num7 != 0)
						{
							if (num7 == 18)
							{
								num6++;
								if (num6 > 16)
								{
									break;
								}
							}
							else
							{
								if (num6 > 1 && (num7 == 2 || num7 == 7 || num7 == 72 || num7 == 3))
								{
									int num8 = (!random.Bool(0.1f)) ? 1 : 2;
									num8 = (flag ? MathUtils.Min(num8, num6 - 1) : MathUtils.Min(num8, num6));
									for (int l = 0; l < num8; l++)
									{
										chunk.SetCellValueFast(x2, k + 1 + l, z, Terrain.MakeBlockValue(233));
									}
									break;
								}
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x000B0E34 File Offset: 0x000AF034
		public void GenerateIvy(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			Game.Random random = new Game.Random(this.m_seed + chunk.Coords.X + 2191 * chunk.Coords.Y);
			int num = random.Int(0, MathUtils.Max(12, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.Int(4, 11);
				int num3 = random.Int(4, 11);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast >= 10 && temperatureFast >= 10)
				{
					int num4 = chunk.CalculateTopmostCellHeight(num2, num3);
					int j = 0;
					while (j < 100)
					{
						int num5 = num2 + random.Int(-3, 3);
						int num6 = MathUtils.Clamp(num4 + random.Int(-12, 1), 1, 255);
						int num7 = num3 + random.Int(-3, 3);
						int num8 = Terrain.ExtractContents(chunk.GetCellValueFast(num5, num6, num7));
						if (num8 <= 9)
						{
							if (num8 - 2 <= 1 || num8 - 8 <= 1)
							{
								goto IL_107;
							}
						}
						else if (num8 == 12 || num8 - 66 <= 1)
						{
							goto IL_107;
						}
						IL_242:
						j++;
						continue;
						IL_107:
						int num9 = random.Int(0, 3);
						int k = 0;
						while (k < 4)
						{
							int face = (k + num9) % 4;
							Point3 point = CellFace.FaceToPoint3(face);
							if (chunk.GetCellContentsFast(num5 + point.X, num6, num7 + point.Z) == 0)
							{
								int l = num6 - 1;
								while (l >= 1 && chunk.GetCellContentsFast(num5 + point.X, l, num7 + point.Z) == 0 && chunk.GetCellContentsFast(num5, l, num7) != 0)
								{
									l--;
								}
								if (chunk.GetCellContentsFast(num5 + point.X, l, num7 + point.Z) == 0)
								{
									l++;
									int value = Terrain.MakeBlockValue(197, 0, IvyBlock.SetFace(0, CellFace.OppositeFace(face)));
									while (l >= 1)
									{
										if (chunk.GetCellContentsFast(num5 + point.X, l, num7 + point.Z) != 0)
										{
											break;
										}
										chunk.SetCellValueFast(num5 + point.X, l, num7 + point.Z, value);
										if (IvyBlock.IsGrowthStopCell(num5 + point.X, l, num7 + point.Z))
										{
											break;
										}
										l--;
									}
									break;
								}
								break;
							}
							else
							{
								k++;
							}
						}
						goto IL_242;
					}
				}
			}
		}

        // Token: 0x0600168E RID: 5774 RVA: 0x000B10A0 File Offset: 0x000AF2A0

        public void GenerateTraps(TerrainChunk chunk)
        {
            if (!this.TGExtras)
            {
                return;
            }
            int x = chunk.Coords.X;
            int y = chunk.Coords.Y;
            Terrain terrain = this.m_subsystemTerrain.Terrain;
            Random random = new Random(this.m_seed + x + 2113 * y);
            if (!random.Bool(0.15f) || this.CalculateOceanShoreDistance((float)chunk.Origin.X, (float)chunk.Origin.Y) <= 50f)
            {
                return;
            }
            int num = random.Int(0, MathUtils.Max(2, 1));
            int i = 0;
        IL_2FD:
            while (i < num)
            {
                int num2 = random.Int(2, 5);
                int num3 = random.Int(2, 5);
                int num4 = random.Int(1, 16 - num2 - 2);
                int num5 = random.Int(1, 16 - num3 - 2);
                bool flag = random.Float(0f, 1f) < 0.5f;
                int num6 = random.Int(3, 5);
                int? num7 = null;
                int j = num4 - 1;
                int? num10;
                int num11;
                while (j < num4 + num2 + 1)
                {
                    for (int k = num5 - 1; k < num5 + num3 + 1; k++)
                    {
                        int num8 = chunk.CalculateTopmostCellHeight(j, k);
                        int num9 = MathUtils.Max(num8 - 20, 5);
                        while (num8 >= num9 && chunk.GetCellContentsFast(j, num8, k) != 8)
                        {
                            num8--;
                        }
                        if (num7 != null)
                        {
                            num10 = num7;
                            num11 = num8;
                            if (!(num10.GetValueOrDefault() == num11 & num10 != null))
                            {
                                i++;
								goto IL_2FD;
                            }
                        }
                        num7 = new int?(num8);
                        if (chunk.GetCellContentsFast(j, num8, k) != 8)
                        {
                            i++;
							goto IL_2FD;
                        }
                    }
                    j++;
                    continue;
                }
                if (num7 == null)
                {
					i++;
					goto IL_2FD;
                }
                num10 = num7 - num6;
                num11 = 5;
                if (!(num10.GetValueOrDefault() < num11 & num10 != null))
                {
                    for (int l = num4; l < num4 + num2; l++)
                    {
                        for (int m = num5; m < num5 + num3; m++)
                        {
                            int num12 = num7.Value - 1;
                            for (; ; )
                            {
                                int num13 = num12;
                                int? num14 = num7;
                                num11 = num6;
                                num10 = ((num14 != null) ? new int?(num14.GetValueOrDefault() - num11 + 1) : null);
                                if (!(num13 >= num10.GetValueOrDefault() & num10 != null))
                                {
                                    break;
                                }
                                chunk.SetCellValueFast(l, num12, m, Terrain.MakeBlockValue(0));
                                num12--;
                            }
                            chunk.SetCellValueFast(l, num7.Value, m, Terrain.MakeBlockValue(87));
                            if (flag)
                            {
                                int data = SpikedPlankBlock.SetSpikesState(0, random.Float(0f, 1f) < 0.33f);
                                chunk.SetCellValueFast(l, num7.Value - num6 + 1, m, Terrain.MakeBlockValue(86, 0, data));
                            }
                        }
                    }
                    i++;
					goto IL_2FD;
                }
                i++;
				goto IL_2FD;
            }
        }
        public void GenerateTrapsOuter(TerrainChunk chunk)
        {
            if (!this.TGExtras)
                return;
            int x1 = chunk.Coords.X;
            int y1 = chunk.Coords.Y;
            Terrain terrain = this.m_subsystemTerrain.Terrain;
            Random random = new Random(this.m_seed + x1 + 2113 * y1);
            if (!random.Bool(0.15f) || (double)this.CalculateOceanShoreDistance((float)chunk.Origin.X, (float)chunk.Origin.Y) <= 50.0)
                return;
            int num1 = random.Int(0, MathUtils.Max(2, 1));
        label_31:
            for (int index = 0; index < num1; ++index)
            {
                int num2 = random.Int(2, 5);
                int num3 = random.Int(2, 5);
                int num4 = random.Int(1, 16 - num2 - 2);
                int num5 = random.Int(1, 16 - num3 - 2);
                bool flag = (double)random.Float(0.0f, 1f) < 0.5;
                int num6 = random.Int(3, 5);
                int? nullable1 = new int?();
                int? nullable2;
                for (int x2 = num4 - 1; x2 < num4 + num2 + 1; ++x2)
                {
                    for (int z = num5 - 1; z < num5 + num3 + 1; ++z)
                    {
                        int topmostCellHeight = chunk.CalculateTopmostCellHeight(x2, z);
                        int num7 = MathUtils.Max(topmostCellHeight - 20, 5);
                        while (topmostCellHeight >= num7 && chunk.GetCellContentsFast(x2, topmostCellHeight, z) != 8)
                            --topmostCellHeight;
                        if (nullable1.HasValue)
                        {
                            nullable2 = nullable1;
                            int num8 = topmostCellHeight;
                            if (!(nullable2.GetValueOrDefault() == num8 & nullable2.HasValue))
                                goto label_31;
                        }
                        nullable1 = new int?(topmostCellHeight);
                        if (chunk.GetCellContentsFast(x2, topmostCellHeight, z) != 8)
                            goto label_31;
                    }
                }
                if (nullable1.HasValue)
                {
                    int? nullable3 = nullable1;
                    int num9 = num6;
                    nullable2 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - num9) : new int?();
                    int num10 = 5;
                    if (!(nullable2.GetValueOrDefault() < num10 & nullable2.HasValue))
                    {
                        for (int x3 = num4; x3 < num4 + num2; ++x3)
                        {
                            for (int z = num5; z < num5 + num3; ++z)
                            {
                                int y2 = nullable1.Value - 1;
                                while (true)
                                {
                                    int num11 = y2;
                                    nullable3 = nullable1;
                                    int num12 = num6;
                                    nullable2 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - num12 + 1) : new int?();
                                    int valueOrDefault = nullable2.GetValueOrDefault();
                                    if (num11 >= valueOrDefault & nullable2.HasValue)
                                    {
                                        chunk.SetCellValueFast(x3, y2, z, Terrain.MakeBlockValue(0));
                                        --y2;
                                    }
                                    else
                                        break;
                                }
                                chunk.SetCellValueFast(x3, nullable1.Value, z, Terrain.MakeBlockValue(87));
                                if (flag)
                                {
                                    int data = SpikedPlankBlock.SetSpikesState(0, (double)random.Float(0.0f, 1f) < 0.330000013113022);
                                    chunk.SetCellValueFast(x3, nullable1.Value - num6 + 1, z, Terrain.MakeBlockValue(86, 0, data));
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600168F RID: 5775 RVA: 0x000B13B4 File Offset: 0x000AF5B4
        public void GenerateGraves(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			Game.Random random = new Game.Random((int)MathUtils.Hash((uint)(this.m_seed + x + 10323 * y)));
			if (random.Float(0f, 1f) >= 0.033f || this.CalculateOceanShoreDistance((float)chunk.Origin.X, (float)chunk.Origin.Y) <= 10f)
			{
				return;
			}
			int num = random.Int(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.Int(6, 9);
				int num3 = random.Int(6, 9);
				int num4 = random.Bool(0.2f) ? random.Int(6, 20) : random.Int(1, 5);
				bool flag = random.Bool(0.5f);
				for (int j = 0; j < num4; j++)
				{
					int num5 = num2 + random.Int(-4, 4);
					int num6 = num3 + random.Int(-4, 4);
					int num7 = chunk.CalculateTopmostCellHeight(num5, num6);
					if (num7 >= 10 && num7 <= 246)
					{
						int num8 = random.Int(0, 3);
						int k = 0;
						IL_6AD:
						while (k < 4)
						{
							int num9 = (k + num8) % 4;
							Point3 point = CellFace.FaceToPoint3(num9);
							Point3 point2 = new Point3(-point.Z, point.Y, point.X);
							int num10 = (point.X < 0) ? (num5 - 2) : (num5 - 1);
							int num11 = (point.X > 0) ? (num5 + 2) : (num5 + 1);
							int num12 = (point.Z < 0) ? (num6 - 2) : (num6 - 1);
							int num13 = (point.Z > 0) ? (num6 + 2) : (num6 + 1);
							for (int l = num10; l <= num11; l++)
							{
								for (int m = num7 - 2; m <= num7 + 2; m++)
								{
									int n = num12;
									while (n <= num13)
									{
										int num14 = Terrain.ExtractContents(chunk.GetCellValueFast(l, m, n));
										Block block = BlocksManager.Blocks[num14];
										if (m > num7)
										{
											if (block.IsCollidable)
											{
												goto IL_6A7;
											}
										}
										else if (num14 != 8 && num14 != 2 && num14 != 7 && num14 != 3 && num14 != 4)
										{
											goto IL_6A7;
										}
										n++;
										continue;
										IL_6A7:
										k++;
										goto IL_6AD;
									}
								}
							}
							int num15 = random.Int(0, 7);
							int data = GravestoneBlock.SetVariant(GravestoneBlock.SetRotation(0, num9 % 2), num15);
							int? num16 = null;
							int contents = 217;
							int contents2 = 136;
							if (num15 >= 4 && !flag)
							{
								int cellContentsFast = chunk.GetCellContentsFast(num5, num7, num6);
								if (cellContentsFast == 7 || cellContentsFast == 4)
								{
									num16 = new int?(Terrain.MakeBlockValue(4));
									contents = 51;
									contents2 = 52;
								}
								else if (random.Float(0f, 1f) < 0.5f)
								{
									num16 = new int?(Terrain.MakeBlockValue(3));
									contents = 217;
									contents2 = 136;
								}
								else
								{
									num16 = new int?(Terrain.MakeBlockValue(67));
									contents = 96;
									contents2 = 95;
								}
							}
							bool flag2 = num16 != null && random.Bool(0.33f);
							float num17 = random.Float(0f, 1f);
							float num18 = random.Float(0f, 1f);
							int num19 = random.Int(-1, 0);
							int num20 = random.Int(1, 2);
							int num21 = flag2 ? (num7 + 2) : (num7 + 1);
							chunk.SetCellValueFast(num5, num21, num6, Terrain.MakeBlockValue(189, 0, data));
							for (int num22 = num19; num22 <= num20; num22++)
							{
								int num23 = num5 + point.X * num22;
								int num24 = num6 + point.Z * num22;
								if (num22 == 0 || num22 == 1)
								{
									chunk.SetCellValueFast(num23, num21 - 2, num24, Terrain.MakeBlockValue(190));
									if (num16 != null)
									{
										chunk.SetCellValueFast(num23, num21 - 1, num24, num16.Value);
										if (num22 == 1)
										{
											int num25 = 0;
											if (num18 < 0.2f)
											{
												num25 = Terrain.MakeBlockValue(20);
											}
											else if (num18 < 0.3f)
											{
												num25 = Terrain.MakeBlockValue(24);
											}
											else if (num18 < 0.4f)
											{
												num25 = Terrain.MakeBlockValue(25);
											}
											else if (num18 < 0.5f)
											{
												num25 = Terrain.MakeBlockValue(31, 0, 4);
											}
											else if (num18 < 0.6f)
											{
												num25 = Terrain.MakeBlockValue(132, 0, CellFace.OppositeFace(num9));
											}
											if (num25 != 0)
											{
												chunk.SetCellValueFast(num23, num21, num24, num25);
											}
										}
									}
								}
								if (flag2)
								{
									if (num17 < 0.3f)
									{
										int value = Terrain.MakeBlockValue(contents, 0, StairsBlock.SetRotation(0, CellFace.Point3ToFace(point2, 5)));
										int value2 = Terrain.MakeBlockValue(contents, 0, StairsBlock.SetRotation(0, CellFace.OppositeFace(CellFace.Point3ToFace(point2, 5))));
										chunk.SetCellValueFast(num23 + point2.X, num21 - 1, num24 + point2.Z, value);
										chunk.SetCellValueFast(num23 - point2.X, num21 - 1, num24 - point2.Z, value2);
										if (num22 == -1)
										{
											int value3 = Terrain.MakeBlockValue(contents, 0, StairsBlock.SetRotation(0, CellFace.OppositeFace(CellFace.Point3ToFace(point, 5))));
											chunk.SetCellValueFast(num23, num21 - 1, num24, value3);
										}
										if (num22 == 2)
										{
											int value4 = Terrain.MakeBlockValue(contents, 0, StairsBlock.SetRotation(0, CellFace.Point3ToFace(point, 5)));
											chunk.SetCellValueFast(num23, num21 - 1, num24, value4);
										}
									}
									else if (num17 < 0.4f)
									{
										chunk.SetCellValueFast(num23 + point2.X, num21 - 1, num24 + point2.Z, Terrain.MakeBlockValue(contents2));
										chunk.SetCellValueFast(num23 - point2.X, num21 - 1, num24 - point2.Z, Terrain.MakeBlockValue(contents2));
										if (num22 == -1)
										{
											chunk.SetCellValueFast(num23, num21 - 1, num24, Terrain.MakeBlockValue(contents2));
										}
										if (num22 == 2)
										{
											chunk.SetCellValueFast(num23, num21 - 1, num24, Terrain.MakeBlockValue(contents2));
										}
									}
									else if (num17 < 0.6f)
									{
										if (num22 == 0 || num22 == 1)
										{
											chunk.SetCellValueFast(num23 + point2.X, num21 - 1, num24 + point2.Z, Terrain.MakeBlockValue(31, 0, CellFace.Point3ToFace(point2, 5)));
											chunk.SetCellValueFast(num23 - point2.X, num21 - 1, num24 - point2.Z, Terrain.MakeBlockValue(31, 0, CellFace.OppositeFace(CellFace.Point3ToFace(point2, 5))));
										}
										if (num22 == -1)
										{
											chunk.SetCellValueFast(num23, num21 - 1, num24, Terrain.MakeBlockValue(31, 0, CellFace.OppositeFace(num9)));
										}
										if (num22 == 2)
										{
											chunk.SetCellValueFast(num23, num21 - 1, num24, Terrain.MakeBlockValue(31, 0, num9));
										}
									}
								}
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x000B1A94 File Offset: 0x000AFC94
		public void GenerateSnowAndIce(TerrainChunk chunk)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					int k = 254;
					while (k >= 0)
					{
						int cellValueFast = chunk.GetCellValueFast(i, k, j);
						int num3 = Terrain.ExtractContents(cellValueFast);
						if (num3 != 0)
						{
							if (!SubsystemWeather.IsPlaceFrozen(chunk.GetTemperatureFast(i, j), k))
							{
								break;
							}
							if (BlocksManager.Blocks[num3] is WaterBlock)
							{
								if (this.CalculateOceanShoreDistance((float)num, (float)num2) > -20f)
								{
									float num4 = (float)(1 + (int)(2f * MathUtils.Sqr(SimplexNoise.OctavedNoise((float)num, (float)num2, 0.2f, 1, 2f, 1f, false))));
									int num5 = 0;
									while ((float)num5 < num4)
									{
										if (k - num5 > 0)
										{
											if (!(BlocksManager.Blocks[chunk.GetCellContentsFast(i, k - num5, j)] is WaterBlock))
											{
												break;
											}
											chunk.SetCellValueFast(i, k - num5, j, 62);
										}
										num5++;
									}
									if (SubsystemWeather.ShaftHasSnowOnIce(num, num2))
									{
										chunk.SetCellValueFast(i, k + 1, j, 61);
									}
								}
							}
							else if (SubsystemSnowBlockBehavior.CanSupportSnow(cellValueFast))
							{
								chunk.SetCellValueFast(i, k + 1, j, 61);
							}
							if (num3 == 8)
							{
								chunk.SetCellValueFast(i, k, j, Terrain.MakeBlockValue(8, 0, 1));
								break;
							}
							break;
						}
						else
						{
							k--;
						}
					}
				}
			}
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x000B1C0C File Offset: 0x000AFE0C
		public void PropagateFluidsDownwards(TerrainChunk chunk)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = TerrainChunk.CalculateCellIndex(i, 255, j);
					int num2 = 0;
					int k = 255;
					while (k >= 0)
					{
						int num3 = Terrain.ExtractContents(chunk.GetCellValueFast(num));
						if (num3 == 0 && num2 != 0 && BlocksManager.FluidBlocks[num2] != null)
						{
							chunk.SetCellValueFast(num, num2);
							num3 = num2;
						}
						num2 = num3;
						k--;
						num--;
					}
				}
			}
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x000B1C88 File Offset: 0x000AFE88
		public void UpdateFluidIsTop(TerrainChunk chunk)
		{
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
						if (num3 != num2 && BlocksManager.FluidBlocks[num3] != null && BlocksManager.FluidBlocks[num2] == null)
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

		// Token: 0x06001693 RID: 5779 RVA: 0x000B1D2C File Offset: 0x000AFF2C
		public static void CreateBrushes()
		{
			Game.Random random = new Game.Random(17);
			for (int i = 0; i < 16; i++)
			{
				TerrainBrush terrainBrush = new TerrainBrush();
				int num = random.Int(4, 12);
				for (int j = 0; j < num; j++)
				{
					Vector3 v = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num2 = random.Int(3, 8);
					Vector3 vector = Vector3.Zero;
					for (int k = 0; k < num2; k++)
					{
						terrainBrush.AddBox((int)MathUtils.Floor(vector.X), (int)MathUtils.Floor(vector.Y), (int)MathUtils.Floor(vector.Z), 1, 1, 1, 16);
						vector += v;
					}
				}
				if (i == 0)
				{
					terrainBrush.AddCell(0, 0, 0, 150);
				}
				terrainBrush.Compile();
				TerrainChunkGeneratorProviderActive.m_coalBrushes.Add(terrainBrush);
			}
			for (int l = 0; l < 16; l++)
			{
				TerrainBrush terrainBrush2 = new TerrainBrush();
				int num3 = random.Int(3, 7);
				for (int m = 0; m < num3; m++)
				{
					Vector3 v2 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num4 = random.Int(3, 6);
					Vector3 vector2 = Vector3.Zero;
					for (int n = 0; n < num4; n++)
					{
						terrainBrush2.AddBox((int)MathUtils.Floor(vector2.X), (int)MathUtils.Floor(vector2.Y), (int)MathUtils.Floor(vector2.Z), 1, 1, 1, 39);
						vector2 += v2;
					}
				}
				terrainBrush2.Compile();
				TerrainChunkGeneratorProviderActive.m_ironBrushes.Add(terrainBrush2);
			}
			for (int num5 = 0; num5 < 16; num5++)
			{
				TerrainBrush terrainBrush3 = new TerrainBrush();
				int num6 = random.Int(4, 10);
				for (int num7 = 0; num7 < num6; num7++)
				{
					Vector3 v3 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-2f, 2f), random.Float(-1f, 1f)));
					int num8 = random.Int(3, 6);
					Vector3 vector3 = Vector3.Zero;
					for (int num9 = 0; num9 < num8; num9++)
					{
						terrainBrush3.AddBox((int)MathUtils.Floor(vector3.X), (int)MathUtils.Floor(vector3.Y), (int)MathUtils.Floor(vector3.Z), 1, 1, 1, 41);
						vector3 += v3;
					}
				}
				terrainBrush3.Compile();
				TerrainChunkGeneratorProviderActive.m_copperBrushes.Add(terrainBrush3);
			}
			for (int num10 = 0; num10 < 16; num10++)
			{
				TerrainBrush terrainBrush4 = new TerrainBrush();
				int num11 = random.Int(8, 16);
				for (int num12 = 0; num12 < num11; num12++)
				{
					Vector3 v4 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.25f, 0.25f), random.Float(-1f, 1f)));
					int num13 = random.Int(4, 8);
					Vector3 vector4 = Vector3.Zero;
					for (int num14 = 0; num14 < num13; num14++)
					{
						terrainBrush4.AddBox((int)MathUtils.Floor(vector4.X), (int)MathUtils.Floor(vector4.Y), (int)MathUtils.Floor(vector4.Z), 1, 1, 1, 100);
						vector4 += v4;
					}
				}
				terrainBrush4.Compile();
				TerrainChunkGeneratorProviderActive.m_saltpeterBrushes.Add(terrainBrush4);
			}
			for (int num15 = 0; num15 < 16; num15++)
			{
				TerrainBrush terrainBrush5 = new TerrainBrush();
				int num16 = random.Int(4, 10);
				for (int num17 = 0; num17 < num16; num17++)
				{
					Vector3 v5 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num18 = random.Int(3, 6);
					Vector3 vector5 = Vector3.Zero;
					for (int num19 = 0; num19 < num18; num19++)
					{
						terrainBrush5.AddBox((int)MathUtils.Floor(vector5.X), (int)MathUtils.Floor(vector5.Y), (int)MathUtils.Floor(vector5.Z), 1, 1, 1, 101);
						vector5 += v5;
					}
				}
				terrainBrush5.Compile();
				TerrainChunkGeneratorProviderActive.m_sulphurBrushes.Add(terrainBrush5);
			}
			for (int num20 = 0; num20 < 16; num20++)
			{
				TerrainBrush terrainBrush6 = new TerrainBrush();
				int num21 = random.Int(2, 6);
				for (int num22 = 0; num22 < num21; num22++)
				{
					Vector3 v6 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num23 = random.Int(3, 6);
					Vector3 vector6 = Vector3.Zero;
					for (int num24 = 0; num24 < num23; num24++)
					{
						terrainBrush6.AddBox((int)MathUtils.Floor(vector6.X), (int)MathUtils.Floor(vector6.Y), (int)MathUtils.Floor(vector6.Z), 1, 1, 1, 112);
						vector6 += v6;
					}
				}
				terrainBrush6.Compile();
				TerrainChunkGeneratorProviderActive.m_diamondBrushes.Add(terrainBrush6);
			}
			for (int num25 = 0; num25 < 16; num25++)
			{
				TerrainBrush terrainBrush7 = new TerrainBrush();
				int num26 = random.Int(4, 10);
				for (int num27 = 0; num27 < num26; num27++)
				{
					Vector3 v7 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num28 = random.Int(3, 6);
					Vector3 vector7 = Vector3.Zero;
					for (int num29 = 0; num29 < num28; num29++)
					{
						terrainBrush7.AddBox((int)MathUtils.Floor(vector7.X), (int)MathUtils.Floor(vector7.Y), (int)MathUtils.Floor(vector7.Z), 1, 1, 1, 148);
						vector7 += v7;
					}
				}
				terrainBrush7.Compile();
				TerrainChunkGeneratorProviderActive.m_germaniumBrushes.Add(terrainBrush7);
			}
			for (int num30 = 0; num30 < 16; num30++)
			{
				TerrainBrush terrainBrush8 = new TerrainBrush();
				int num31 = random.Int(16, 32);
				for (int num32 = 0; num32 < num31; num32++)
				{
					Vector3 v8 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.75f, 0.75f), random.Float(-1f, 1f)));
					int num33 = random.Int(6, 12);
					Vector3 vector8 = Vector3.Zero;
					for (int num34 = 0; num34 < num33; num34++)
					{
						terrainBrush8.AddBox((int)MathUtils.Floor(vector8.X), (int)MathUtils.Floor(vector8.Y), (int)MathUtils.Floor(vector8.Z), 1, 1, 1, 2);
						vector8 += v8;
					}
				}
				terrainBrush8.Compile();
				TerrainChunkGeneratorProviderActive.m_dirtPocketBrushes.Add(terrainBrush8);
			}
			for (int num35 = 0; num35 < 16; num35++)
			{
				TerrainBrush terrainBrush9 = new TerrainBrush();
				int num36 = random.Int(16, 32);
				for (int num37 = 0; num37 < num36; num37++)
				{
					Vector3 v9 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.75f, 0.75f), random.Float(-1f, 1f)));
					int num38 = random.Int(6, 12);
					Vector3 vector9 = Vector3.Zero;
					for (int num39 = 0; num39 < num38; num39++)
					{
						terrainBrush9.AddBox((int)MathUtils.Floor(vector9.X), (int)MathUtils.Floor(vector9.Y), (int)MathUtils.Floor(vector9.Z), 1, 1, 1, 6);
						vector9 += v9;
					}
				}
				terrainBrush9.Compile();
				TerrainChunkGeneratorProviderActive.m_gravelPocketBrushes.Add(terrainBrush9);
			}
			for (int num40 = 0; num40 < 16; num40++)
			{
				TerrainBrush terrainBrush10 = new TerrainBrush();
				int num41 = random.Int(16, 32);
				for (int num42 = 0; num42 < num41; num42++)
				{
					Vector3 v10 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.75f, 0.75f), random.Float(-1f, 1f)));
					int num43 = random.Int(6, 12);
					Vector3 vector10 = Vector3.Zero;
					for (int num44 = 0; num44 < num43; num44++)
					{
						terrainBrush10.AddBox((int)MathUtils.Floor(vector10.X), (int)MathUtils.Floor(vector10.Y), (int)MathUtils.Floor(vector10.Z), 1, 1, 1, 66);
						vector10 += v10;
					}
				}
				terrainBrush10.Compile();
				TerrainChunkGeneratorProviderActive.m_limestonePocketBrushes.Add(terrainBrush10);
			}
			for (int num45 = 0; num45 < 16; num45++)
			{
				TerrainBrush terrainBrush11 = new TerrainBrush();
				int num46 = random.Int(16, 32);
				for (int num47 = 0; num47 < num46; num47++)
				{
					Vector3 v11 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.1f, 0.1f), random.Float(-1f, 1f)));
					int num48 = random.Int(6, 12);
					Vector3 vector11 = Vector3.Zero;
					for (int num49 = 0; num49 < num48; num49++)
					{
						terrainBrush11.AddBox((int)MathUtils.Floor(vector11.X), (int)MathUtils.Floor(vector11.Y), (int)MathUtils.Floor(vector11.Z), 1, 1, 1, 72);
						vector11 += v11;
					}
				}
				terrainBrush11.Compile();
				TerrainChunkGeneratorProviderActive.m_clayPocketBrushes.Add(terrainBrush11);
			}
			for (int num50 = 0; num50 < 16; num50++)
			{
				TerrainBrush terrainBrush12 = new TerrainBrush();
				int num51 = random.Int(16, 32);
				for (int num52 = 0; num52 < num51; num52++)
				{
					Vector3 v12 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.75f, 0.75f), random.Float(-1f, 1f)));
					int num53 = random.Int(6, 12);
					Vector3 vector12 = Vector3.Zero;
					for (int num54 = 0; num54 < num53; num54++)
					{
						terrainBrush12.AddBox((int)MathUtils.Floor(vector12.X), (int)MathUtils.Floor(vector12.Y), (int)MathUtils.Floor(vector12.Z), 1, 1, 1, 7);
						vector12 += v12;
					}
				}
				terrainBrush12.Compile();
				TerrainChunkGeneratorProviderActive.m_sandPocketBrushes.Add(terrainBrush12);
			}
			for (int num55 = 0; num55 < 16; num55++)
			{
				TerrainBrush terrainBrush13 = new TerrainBrush();
				int num56 = random.Int(16, 32);
				for (int num57 = 0; num57 < num56; num57++)
				{
					Vector3 v13 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-0.75f, 0.75f), random.Float(-1f, 1f)));
					int num58 = random.Int(6, 12);
					Vector3 vector13 = Vector3.Zero;
					for (int num59 = 0; num59 < num58; num59++)
					{
						terrainBrush13.AddBox((int)MathUtils.Floor(vector13.X), (int)MathUtils.Floor(vector13.Y), (int)MathUtils.Floor(vector13.Z), 1, 1, 1, 67);
						vector13 += v13;
					}
				}
				terrainBrush13.Compile();
				TerrainChunkGeneratorProviderActive.m_basaltPocketBrushes.Add(terrainBrush13);
			}
			for (int num60 = 0; num60 < 16; num60++)
			{
				TerrainBrush terrainBrush14 = new TerrainBrush();
				int num61 = random.Int(16, 32);
				for (int num62 = 0; num62 < num61; num62++)
				{
					Vector3 v14 = 0.5f * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f)));
					int num63 = random.Int(5, 10);
					Vector3 vector14 = Vector3.Zero;
					for (int num64 = 0; num64 < num63; num64++)
					{
						terrainBrush14.AddBox((int)MathUtils.Floor(vector14.X), (int)MathUtils.Floor(vector14.Y), (int)MathUtils.Floor(vector14.Z), 1, 1, 1, 3);
						vector14 += v14;
					}
				}
				terrainBrush14.Compile();
				TerrainChunkGeneratorProviderActive.m_granitePocketBrushes.Add(terrainBrush14);
			}
			int[] array = new int[]
			{
				4,
				6,
				8
			};
			for (int num65 = 0; num65 < 4 * array.Length; num65++)
			{
				TerrainBrush terrainBrush15 = new TerrainBrush();
				int num66 = array[num65 / 4];
				int num67 = num65 % 2 + 1;
				float num68 = (num65 % 4 == 2) ? 0.5f : 1f;
				int num69 = (num65 % 4 == 1) ? (num66 * num66) : (2 * num66 * num66);
				for (int num70 = 0; num70 < num69; num70++)
				{
					Vector2 vector15 = random.Vector2(0f, (float)num66);
					float num71 = vector15.Length();
					int num72 = random.Int(3, 4);
					int sizeY = 1 + (int)MathUtils.Lerp(MathUtils.Max((float)(num66 / 3), 2.5f) * num68, 0f, num71 / (float)num66) + random.Int(0, 1);
					terrainBrush15.AddBox((int)MathUtils.Floor(vector15.X), 0, (int)MathUtils.Floor(vector15.Y), num72, sizeY, num72, 0);
					terrainBrush15.AddBox((int)MathUtils.Floor(vector15.X), -num67, (int)MathUtils.Floor(vector15.Y), num72, num67, num72, 18);
				}
				terrainBrush15.Compile();
				TerrainChunkGeneratorProviderActive.m_waterPocketBrushes.Add(terrainBrush15);
			}
			int[] array2 = new int[]
			{
				8,
				12,
				14,
				16
			};
			for (int num73 = 0; num73 < 4 * array2.Length; num73++)
			{
				TerrainBrush terrainBrush16 = new TerrainBrush();
				int num74 = array2[num73 / 4];
				int num75 = num74 + 2;
				float num76 = (num73 % 4 == 2) ? 0.5f : 1f;
				int num77 = (num73 % 4 == 1) ? (num74 * num74) : (2 * num74 * num74);
				for (int num78 = 0; num78 < num77; num78++)
				{
					Vector2 vector16 = random.Vector2(0f, (float)num74);
					float num79 = vector16.Length();
					int num80 = random.Int(3, 4);
					int sizeY2 = 1 + (int)MathUtils.Lerp(MathUtils.Max((float)(num74 / 3), 2.5f) * num76, 0f, num79 / (float)num74) + random.Int(0, 1);
					int num81 = 1 + (int)MathUtils.Lerp((float)num75, 0f, num79 / (float)num74) + random.Int(0, 1);
					terrainBrush16.AddBox((int)MathUtils.Floor(vector16.X), 0, (int)MathUtils.Floor(vector16.Y), num80, sizeY2, num80, 0);
					terrainBrush16.AddBox((int)MathUtils.Floor(vector16.X), -num81, (int)MathUtils.Floor(vector16.Y), num80, num81, num80, 92);
				}
				terrainBrush16.Compile();
				TerrainChunkGeneratorProviderActive.m_magmaPocketBrushes.Add(terrainBrush16);
			}
			for (int num82 = 0; num82 < 7; num82++)
			{
				TerrainChunkGeneratorProviderActive.m_caveBrushesByType.Add(new List<TerrainBrush>());
				for (int num83 = 0; num83 < 3; num83++)
				{
					TerrainBrush terrainBrush17 = new TerrainBrush();
					int num84 = 6 + 4 * num82;
					int max = 3 + num82 / 3;
					int max2 = 9 + num82;
					for (int num85 = 0; num85 < num84; num85++)
					{
						int num86 = random.Int(2, max);
						int num87 = random.Int(8, max2) - 2 * num86;
						Vector3 v15 = 0.5f * new Vector3(random.Float(-1f, 1f), random.Float(0f, 1f), random.Float(-1f, 1f));
						Vector3 vector17 = Vector3.Zero;
						for (int num88 = 0; num88 < num87; num88++)
						{
							terrainBrush17.AddBox((int)MathUtils.Floor(vector17.X) - num86 / 2, (int)MathUtils.Floor(vector17.Y) - num86 / 2, (int)MathUtils.Floor(vector17.Z) - num86 / 2, num86, num86, num86, 0);
							vector17 += v15;
						}
					}
					terrainBrush17.Compile();
					TerrainChunkGeneratorProviderActive.m_caveBrushesByType[num82].Add(terrainBrush17);
				}
			}
		}

		// Token: 0x04001001 RID: 4097
		public static List<TerrainBrush> m_coalBrushes = new List<TerrainBrush>();

		// Token: 0x04001002 RID: 4098
		public static List<TerrainBrush> m_ironBrushes = new List<TerrainBrush>();

		// Token: 0x04001003 RID: 4099
		public static List<TerrainBrush> m_copperBrushes = new List<TerrainBrush>();

		// Token: 0x04001004 RID: 4100
		public static List<TerrainBrush> m_saltpeterBrushes = new List<TerrainBrush>();

		// Token: 0x04001005 RID: 4101
		public static List<TerrainBrush> m_sulphurBrushes = new List<TerrainBrush>();

		// Token: 0x04001006 RID: 4102
		public static List<TerrainBrush> m_diamondBrushes = new List<TerrainBrush>();

		// Token: 0x04001007 RID: 4103
		public static List<TerrainBrush> m_germaniumBrushes = new List<TerrainBrush>();

		// Token: 0x04001008 RID: 4104
		public static List<TerrainBrush> m_dirtPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04001009 RID: 4105
		public static List<TerrainBrush> m_gravelPocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100A RID: 4106
		public static List<TerrainBrush> m_limestonePocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100B RID: 4107
		public static List<TerrainBrush> m_sandPocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100C RID: 4108
		public static List<TerrainBrush> m_basaltPocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100D RID: 4109
		public static List<TerrainBrush> m_granitePocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100E RID: 4110
		public static List<TerrainBrush> m_clayPocketBrushes = new List<TerrainBrush>();

		// Token: 0x0400100F RID: 4111
		public static List<TerrainBrush> m_waterPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04001010 RID: 4112
		public static List<TerrainBrush> m_magmaPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04001011 RID: 4113
		public static Action<TerrainChunk> GenerateMinerals1;

		// Token: 0x04001012 RID: 4114
		public static Action<TerrainChunk> GenerateMinerals2;

		// Token: 0x04001013 RID: 4115
		public static List<List<TerrainBrush>> m_caveBrushesByType = new List<List<TerrainBrush>>();

		// Token: 0x04001014 RID: 4116
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001015 RID: 4117
		public SubsystemBottomSuckerBlockBehavior m_subsystemBottomSuckerBlockBehavior;

		// Token: 0x04001016 RID: 4118
		public WorldSettings m_worldSettings;

		// Token: 0x04001017 RID: 4119
		public int m_seed;

		// Token: 0x04001018 RID: 4120
		public Vector2? m_islandSize;

		// Token: 0x04001019 RID: 4121
		public Vector2 m_oceanCorner;

		// Token: 0x0400101A RID: 4122
		public Vector2 m_temperatureOffset;

		// Token: 0x0400101B RID: 4123
		public Vector2 m_humidityOffset;

		// Token: 0x0400101C RID: 4124
		public Vector2 m_mountainsOffset;

		// Token: 0x0400101D RID: 4125
		public Vector2 m_riversOffset;

		// Token: 0x0400101E RID: 4126
		public float TGBiomeScaling;

		// Token: 0x0400101F RID: 4127
		public float TGShoreFluctuations;

		// Token: 0x04001020 RID: 4128
		public float TGShoreFluctuationsScaling;

		// Token: 0x04001021 RID: 4129
		public float TGOceanSlope;

		// Token: 0x04001022 RID: 4130
		public float TGOceanSlopeVariation;

		// Token: 0x04001023 RID: 4131
		public float TGIslandsFrequency;

		// Token: 0x04001024 RID: 4132
		public float TGDensityBias;

		// Token: 0x04001025 RID: 4133
		public float TGHeightBias;

		// Token: 0x04001026 RID: 4134
		public float TGHillsPercentage;

		// Token: 0x04001027 RID: 4135
		public float TGHillsStrength;

		// Token: 0x04001028 RID: 4136
		public int TGHillsOctaves;

		// Token: 0x04001029 RID: 4137
		public float TGHillsFrequency;

		// Token: 0x0400102A RID: 4138
		public float TGHillsPersistence;

		// Token: 0x0400102B RID: 4139
		public float TGMountainsStrength;

		// Token: 0x0400102C RID: 4140
		public float TGMountainRangeFreq;

		// Token: 0x0400102D RID: 4141
		public float TGMountainsPercentage;

		// Token: 0x0400102E RID: 4142
		public static float TGMountainsDetailFreq;

		// Token: 0x0400102F RID: 4143
		public static int TGMountainsDetailOctaves;

		// Token: 0x04001030 RID: 4144
		public static float TGMountainsDetailPersistence;

		// Token: 0x04001031 RID: 4145
		public float TGRiversStrength;

		// Token: 0x04001032 RID: 4146
		public float TGTurbulenceStrength;

		// Token: 0x04001033 RID: 4147
		public float TGTurbulenceFreq;

		// Token: 0x04001034 RID: 4148
		public int TGTurbulenceOctaves;

		// Token: 0x04001035 RID: 4149
		public float TGTurbulencePersistence;

		// Token: 0x04001036 RID: 4150
		public float TGMinTurbulence;

		// Token: 0x04001037 RID: 4151
		public float TGTurbulenceZero;

		// Token: 0x04001038 RID: 4152
		public static float TGSurfaceMultiplier;

		// Token: 0x04001039 RID: 4153
		public bool TGWater;

		// Token: 0x0400103A RID: 4154
		public bool TGExtras;

		// Token: 0x0400103B RID: 4155
		public bool TGCavesAndPockets;

		// Token: 0x020004F1 RID: 1265

		public class CavePoint
		{
			// Token: 0x0400181D RID: 6173
			public Vector3 Position;

			// Token: 0x0400181E RID: 6174
			public Vector3 Direction;

			// Token: 0x0400181F RID: 6175
			public int BrushType;

			// Token: 0x04001820 RID: 6176
			public int Length;

			// Token: 0x04001821 RID: 6177
			public int StepsTaken;
		}

		// Token: 0x020004F2 RID: 1266
		public class Grid2d
		{
			// Token: 0x17000565 RID: 1381
			// (get) Token: 0x0600209A RID: 8346 RVA: 0x000E4F17 File Offset: 0x000E3117
			public int SizeX
			{
				get
				{
					return this.m_sizeX;
				}
			}

			// Token: 0x17000566 RID: 1382
			// (get) Token: 0x0600209B RID: 8347 RVA: 0x000E4F1F File Offset: 0x000E311F
			public int SizeY
			{
				get
				{
					return this.m_sizeY;
				}
			}

			// Token: 0x0600209C RID: 8348 RVA: 0x000E4F27 File Offset: 0x000E3127
			public Grid2d(int sizeX, int sizeY)
			{
				sizeY = Math.Abs(sizeY);
				sizeX = Math.Abs(sizeX);
				this.m_sizeX = sizeX;
				this.m_sizeY = sizeY;
				this.m_data = new float[this.m_sizeX * this.m_sizeY];
			}

			// Token: 0x0600209D RID: 8349 RVA: 0x000E4F55 File Offset: 0x000E3155
			public float Get(int x, int y)
			{
				return this.m_data[x + y * this.m_sizeX];
			}

			// Token: 0x0600209E RID: 8350 RVA: 0x000E4F68 File Offset: 0x000E3168
			public void Set(int x, int y, float value)
			{
				this.m_data[x + y * this.m_sizeX] = value;
			}

			// Token: 0x0600209F RID: 8351 RVA: 0x000E4F7C File Offset: 0x000E317C
			public float Sample(float x, float y)
			{
				int num = (int)MathUtils.Floor(x);
				int num2 = (int)MathUtils.Floor(y);
				int num3 = (int)MathUtils.Ceiling(x);
				int num4 = (int)MathUtils.Ceiling(y);
				float f = x - (float)num;
				float f2 = y - (float)num2;
				float x2 = this.m_data[num + num2 * this.m_sizeX];
				float x3 = this.m_data[num3 + num2 * this.m_sizeX];
				float x4 = this.m_data[num + num4 * this.m_sizeX];
				float x5 = this.m_data[num3 + num4 * this.m_sizeX];
				float x6 = MathUtils.Lerp(x2, x3, f);
				float x7 = MathUtils.Lerp(x4, x5, f);
				return MathUtils.Lerp(x6, x7, f2);
			}

			// Token: 0x04001822 RID: 6178
			public int m_sizeX;

			// Token: 0x04001823 RID: 6179
			public int m_sizeY;

			// Token: 0x04001824 RID: 6180
			public float[] m_data;
		}

		// Token: 0x020004F3 RID: 1267
		public class Grid3d
		{
			// Token: 0x17000567 RID: 1383
			// (get) Token: 0x060020A0 RID: 8352 RVA: 0x000E501E File Offset: 0x000E321E
			public int SizeX
			{
				get
				{
					return this.m_sizeX;
				}
			}

			// Token: 0x17000568 RID: 1384
			// (get) Token: 0x060020A1 RID: 8353 RVA: 0x000E5026 File Offset: 0x000E3226
			public int SizeY
			{
				get
				{
					return this.m_sizeY;
				}
			}

			// Token: 0x17000569 RID: 1385
			// (get) Token: 0x060020A2 RID: 8354 RVA: 0x000E502E File Offset: 0x000E322E
			public int SizeZ
			{
				get
				{
					return this.m_sizeZ;
				}
			}

			// Token: 0x060020A3 RID: 8355 RVA: 0x000E5038 File Offset: 0x000E3238
			public Grid3d(int sizeX, int sizeY, int sizeZ)
			{
				sizeX = Math.Abs(sizeX);
                sizeY = Math.Abs(sizeY);
                sizeZ = Math.Abs(sizeZ);
                this.m_sizeX = sizeX;
				this.m_sizeY = sizeY;
				this.m_sizeZ = sizeZ;
				this.m_sizeXY = this.m_sizeX * this.m_sizeY;
				this.m_data = new float[this.m_sizeX * this.m_sizeY * this.m_sizeZ];
			}

			// Token: 0x060020A4 RID: 8356 RVA: 0x000E5094 File Offset: 0x000E3294
			public void Get8(int x, int y, int z, out float v111, out float v211, out float v121, out float v221, out float v112, out float v212, out float v122, out float v222)
			{
				int num = x + y * this.m_sizeX + z * this.m_sizeXY;
				v111 = this.m_data[num];
				v211 = this.m_data[num + 1];
				v121 = this.m_data[num + this.m_sizeX];
				v221 = this.m_data[num + 1 + this.m_sizeX];
				v112 = this.m_data[num + this.m_sizeXY];
				v212 = this.m_data[num + 1 + this.m_sizeXY];
				v122 = this.m_data[num + this.m_sizeX + this.m_sizeXY];
				v222 = this.m_data[num + 1 + this.m_sizeX + this.m_sizeXY];
			}

			// Token: 0x060020A5 RID: 8357 RVA: 0x000E514D File Offset: 0x000E334D
			public float Get(int x, int y, int z)
			{
				return this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY];
			}

			// Token: 0x060020A6 RID: 8358 RVA: 0x000E5169 File Offset: 0x000E3369
			public void Set(int x, int y, int z, float value)
			{
				this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY] = value;
			}

			// Token: 0x060020A7 RID: 8359 RVA: 0x000E5188 File Offset: 0x000E3388
			public float Sample(float x, float y, float z)
			{
				int num = (int)MathUtils.Floor(x);
				int num2 = (int)MathUtils.Ceiling(x);
				int num3 = (int)MathUtils.Floor(y);
				int num4 = (int)MathUtils.Ceiling(y);
				int num5 = (int)MathUtils.Floor(z);
				int num6 = (int)MathUtils.Ceiling(z);
				float f = x - (float)num;
				float f2 = y - (float)num3;
				float f3 = z - (float)num5;
				float x2 = this.m_data[num + num3 * this.m_sizeX + num5 * this.m_sizeX * this.m_sizeY];
				float x3 = this.m_data[num2 + num3 * this.m_sizeX + num5 * this.m_sizeX * this.m_sizeY];
				float x4 = this.m_data[num + num4 * this.m_sizeX + num5 * this.m_sizeX * this.m_sizeY];
				float x5 = this.m_data[num2 + num4 * this.m_sizeX + num5 * this.m_sizeX * this.m_sizeY];
				float x6 = this.m_data[num + num3 * this.m_sizeX + num6 * this.m_sizeX * this.m_sizeY];
				float x7 = this.m_data[num2 + num3 * this.m_sizeX + num6 * this.m_sizeX * this.m_sizeY];
				float x8 = this.m_data[num + num4 * this.m_sizeX + num6 * this.m_sizeX * this.m_sizeY];
				float x9 = this.m_data[num2 + num4 * this.m_sizeX + num6 * this.m_sizeX * this.m_sizeY];
				float x10 = MathUtils.Lerp(x2, x3, f);
				float x11 = MathUtils.Lerp(x4, x5, f);
				float x12 = MathUtils.Lerp(x6, x7, f);
				float x13 = MathUtils.Lerp(x8, x9, f);
				float x14 = MathUtils.Lerp(x10, x11, f2);
				float x15 = MathUtils.Lerp(x12, x13, f2);
				return MathUtils.Lerp(x14, x15, f3);
			}

			// Token: 0x04001825 RID: 6181
			public int m_sizeX;

			// Token: 0x04001826 RID: 6182
			public int m_sizeY;

			// Token: 0x04001827 RID: 6183
			public int m_sizeZ;

			// Token: 0x04001828 RID: 6184
			public int m_sizeXY;

			// Token: 0x04001829 RID: 6185
			public float[] m_data;
		}
	}
}
