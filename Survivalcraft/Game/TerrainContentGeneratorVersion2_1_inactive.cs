using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine;

namespace Game
{
	// Token: 0x02000310 RID: 784
	/*
	 * This is used in version 2.1.
	 */
	public class TerrainContentGeneratorVersion2_1_inactive : ITerrainContentsGenerator
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001649 RID: 5705 RVA: 0x000A7BEF File Offset: 0x000A5DEF
		public int OceanLevel
		{
			get
			{
				return 64 + this.m_worldSettings.SeaLevelOffset;
			}
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x000A7C00 File Offset: 0x000A5E00
		static TerrainContentGeneratorVersion2_1_inactive()
		{
			TerrainContentGeneratorVersion2_1_inactive.CreateBrushes();
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x000A7CBC File Offset: 0x000A5EBC
		public TerrainContentGeneratorVersion2_1_inactive(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_subsystemBottomSuckerBlockBehavior = subsystemTerrain.Project.FindSubsystem<SubsystemBottomSuckerBlockBehavior>(true);
			SubsystemGameInfo subsystemGameInfo = subsystemTerrain.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_worldSettings = subsystemGameInfo.WorldSettings;
			this.m_seed = subsystemGameInfo.WorldSeed;
			this.m_islandSize = ((this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island) ? new Vector2?(this.m_worldSettings.IslandSize) : null);
			TerrainContentGeneratorVersion2_1_inactive.OldRandom oldRandom = new TerrainContentGeneratorVersion2_1_inactive.OldRandom(100 + this.m_seed);
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed);
			if (string.IsNullOrEmpty(subsystemGameInfo.WorldSettings.OriginalSerializationVersion))
			{
				this.m_oceanCorner = new Vector2(oldRandom.UniformFloat(2000f, 4000f), oldRandom.UniformFloat(2000f, 4000f));
				this.m_temperatureOffset = new Vector2(1000f, 1000f);
				this.m_humidityOffset = new Vector2(0f, 0f);
				this.m_mountainsOffset = new Vector2(0f, 0f);
				this.m_riversOffset = new Vector2(0f, 0f);
				this.TGNewBiomeNoise = false;
				this.TGBiomeScaling = 1f;
				this.TGShoreFluctuations = 100f;
				this.TGShoreFluctuationsScaling = 1f;
				this.TGOceanSlope = 0.015f;
				this.TGOceanSlopeVariation = 0f;
				this.TGIslandsFrequency = 0.017f;
				this.TGDensityBias = 57f;
				this.TGHeightBias = 1f;
				this.TGRiversStrength = 0f;
				this.TGMountainsStrength = 56f;
				this.TGMountainsPeriod = 0.0014f;
				this.TGMountainsPercentage = 0.15f;
				this.TGHillsStrength = 13f;
				this.TGTurbulenceStrength = 13f;
				this.TGTurbulenceTopOffset = 3f;
				this.TGTurbulencePower = 0.5f;
				TerrainContentGeneratorVersion2_1_inactive.TGSurfaceMultiplier = 1f;
				this.TGWater = true;
				this.TGExtras = true;
				this.TGCavesAndPockets = true;
				return;
			}
			if (string.CompareOrdinal(subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") < 0)
			{
				this.m_oceanCorner = new Vector2(oldRandom.UniformFloat(2000f, 4000f), oldRandom.UniformFloat(2000f, 4000f));
				this.m_temperatureOffset = new Vector2(1000f, 1000f);
				this.m_humidityOffset = new Vector2(0f, 0f);
				this.m_mountainsOffset = new Vector2(0f, 0f);
				this.m_riversOffset = new Vector2(0f, 0f);
				this.TGNewBiomeNoise = false;
				this.TGBiomeScaling = 1f;
				this.TGShoreFluctuations = 100f;
				this.TGShoreFluctuationsScaling = 1f;
				this.TGOceanSlope = 0.015f;
				this.TGOceanSlopeVariation = 0f;
				this.TGIslandsFrequency = 0.017f;
				this.TGDensityBias = 57f;
				this.TGHeightBias = 1f;
				this.TGRiversStrength = 0f;
				this.TGMountainsStrength = 50f;
				this.TGMountainsPeriod = 0.0014f;
				this.TGMountainsPercentage = 0.15f;
				this.TGHillsStrength = 10f;
				this.TGTurbulenceStrength = 24f;
				this.TGTurbulenceTopOffset = 0f;
				this.TGTurbulencePower = 0.3f;
				TerrainContentGeneratorVersion2_1_inactive.TGSurfaceMultiplier = 1f;
				this.TGWater = true;
				this.TGExtras = true;
				this.TGCavesAndPockets = true;
				return;
			}
			float num = (this.m_islandSize != null) ? MathUtils.Min(this.m_islandSize.Value.X, this.m_islandSize.Value.Y) : float.MaxValue;
			this.m_oceanCorner = new Vector2(random.UniformFloat(-100f, -100f), random.UniformFloat(-100f, -100f));
			this.m_temperatureOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
			this.m_humidityOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
			this.m_mountainsOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
			this.m_riversOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
			this.TGNewBiomeNoise = true;
			this.TGBiomeScaling = 1.5f * this.m_worldSettings.BiomeSize;
			this.TGShoreFluctuations = MathUtils.Clamp(2f * num, 0f, 150f);
			this.TGShoreFluctuationsScaling = MathUtils.Clamp(0.04f * num, 0.5f, 3f);
			this.TGOceanSlope = 0.006f;
			this.TGOceanSlopeVariation = 0.004f;
			this.TGIslandsFrequency = 0.01f;
			this.TGDensityBias = 55f;
			this.TGHeightBias = 1f;
			this.TGRiversStrength = 1f;
			this.TGMountainsStrength = 85f;
			this.TGMountainsPeriod = 0.0015f;
			this.TGMountainsPercentage = 0.15f;
			this.TGHillsStrength = 8f;
			this.TGTurbulenceStrength = 35f;
			this.TGTurbulenceTopOffset = 0f;
			this.TGTurbulencePower = 0.3f;
			TerrainContentGeneratorVersion2_1_inactive.TGSurfaceMultiplier = 2f;
			this.TGWater = true;
			this.TGExtras = true;
			this.TGCavesAndPockets = true;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x000A824C File Offset: 0x000A644C
		public Vector3 FindCoarseSpawnPosition()
		{
			Vector2 zero = Vector2.Zero;
			float num = float.MinValue;
			for (int i = 0; i < 800; i += 2)
			{
				for (int j = 4; j <= 8; j += 2)
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
			return new Vector3(zero.X, this.CalculateHeight(zero.X, zero.Y), zero.Y);
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x000A8326 File Offset: 0x000A6526
		public void GenerateChunkContentsPass1(TerrainChunk chunk)
		{
			this.GenerateSurfaceParameters(chunk, 0, 0, 16, 8);
			this.GenerateTerrain(chunk, 0, 0, 16, 8);
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x000A8340 File Offset: 0x000A6540
		public void GenerateChunkContentsPass2(TerrainChunk chunk)
		{
			this.GenerateSurfaceParameters(chunk, 0, 8, 16, 16);
			this.GenerateTerrain(chunk, 0, 8, 16, 16);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x000A835C File Offset: 0x000A655C
		public void GenerateChunkContentsPass3(TerrainChunk chunk)
		{
			this.GenerateCaves(chunk);
			this.GeneratePockets(chunk);
			this.GenerateMinerals(chunk);
			this.GenerateSurface(chunk);
			this.PropagateFluidsDownwards(chunk);
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x000A8384 File Offset: 0x000A6584
		public void GenerateChunkContentsPass4(TerrainChunk chunk)
		{
			this.GenerateGrassAndPlants(chunk);
			this.GenerateTreesAndLogs(chunk);
			this.GenerateCacti(chunk);
			this.GeneratePumpkins(chunk);
			this.GenerateKelp(chunk);
			this.GenerateSeagrass(chunk);
			this.GenerateBottomSuckers(chunk);
			this.GenerateTraps(chunk);
			this.GenerateIvy(chunk);
			this.GenerateGraves(chunk);
			this.GenerateSnowAndIce(chunk);
			this.GenerateBedrockAndAir(chunk);
			this.UpdateFluidIsTop(chunk);
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000A83EC File Offset: 0x000A65EC
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

		// Token: 0x06001652 RID: 5714 RVA: 0x000A8488 File Offset: 0x000A6688
		public float CalculateMountainRangeFactor(float x, float z)
		{
			return 1f - MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.m_mountainsOffset.X, z + this.m_mountainsOffset.Y, this.TGMountainsPeriod / this.TGBiomeScaling, 3, 1.91f, 0.75f, false) - 1f);
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x000A84E4 File Offset: 0x000A66E4
		public float CalculateHeight(float x, float z)
		{
			float num = this.TGOceanSlope + this.TGOceanSlopeVariation * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.m_mountainsOffset.X, z + this.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
			float num2 = this.CalculateOceanShoreDistance(x, z);
			float num3 = MathUtils.Saturate(1f - 0.05f * MathUtils.Abs(num2));
			float num4 = MathUtils.Saturate(MathUtils.Sin(this.TGIslandsFrequency * num2));
			float num5 = MathUtils.Saturate(MathUtils.Saturate((0f - num) * num2) - 0.85f * num4);
			float num6 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - num2 - 10f)) - num4);
			float num7 = this.CalculateMountainRangeFactor(x, z);
			float f = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.001f / this.TGBiomeScaling, 2, 1.97f, 0.8f, false);
			float f2 = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.0017f / this.TGBiomeScaling, 2, 1.93f, 0.7f, false);
			float num8 = (1f - num6) * (1f - num3) * MathUtils.Saturate((num7 - 0.6f) / 0.4f);
			float num9 = (1f - num6) * MathUtils.Saturate((num7 - (1f - this.TGMountainsPercentage)) / this.TGMountainsPercentage);
			float num10 = 2f * SimplexNoise.OctavedNoise(x, z, 0.02f, 3, 1.93f, 0.8f, false) - 1f;
			float num11 = 1.5f * SimplexNoise.OctavedNoise(x, z, 0.004f, 4, 1.98f, 0.9f, false) - 0.5f;
			float num12 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * num9 + 0.5f * num8 + MathUtils.Saturate(1f - num2 / 30f)));
			float x2 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(num9 + 0.5f * num8));
			float num13 = MathUtils.Saturate(1.5f - num12 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.m_riversOffset.X, z + this.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
			float num14 = -50f * num5 + this.TGHeightBias;
			float num15 = MathUtils.Lerp(0f, 8f, f);
			float num16 = MathUtils.Lerp(0f, -6f, f2);
			float num17 = this.TGHillsStrength * num8 * num10;
			float num18 = this.TGMountainsStrength * num9 * num11;
			float f3 = this.TGRiversStrength * num13;
			float num19 = num14 + num15 + num16 + num18 + num17;
			float num20 = MathUtils.Min(MathUtils.Lerp(num19, x2, f3), num19);
			return MathUtils.Clamp(64f + num20, 10f, 251f);
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x000A87F0 File Offset: 0x000A69F0
		public int CalculateTemperature(float x, float z)
		{
			if (this.TGNewBiomeNoise)
			{
				return MathUtils.Clamp((int)(MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.m_temperatureOffset.X, z + this.m_temperatureOffset.Y, 0.0015f / this.TGBiomeScaling, 5, 2f, 0.7f, false) - 1.6f + this.m_worldSettings.TemperatureOffset / 16f) * 16f), 0, 15);
			}
			return MathUtils.Clamp((int)((MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.m_temperatureOffset.X, z + this.m_temperatureOffset.Y, 0.0006f / this.TGBiomeScaling, 4, 1.93f, 1f, false) - 1.6f) + this.m_worldSettings.TemperatureOffset / 16f) * 16f), 0, 15);
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x000A88D8 File Offset: 0x000A6AD8
		public int CalculateHumidity(float x, float z)
		{
			if (this.TGNewBiomeNoise)
			{
				return MathUtils.Clamp((int)(MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.m_humidityOffset.X, z + this.m_humidityOffset.Y, 0.0012f / this.TGBiomeScaling, 5, 2f, 0.7f, false) - 1.2f + this.m_worldSettings.HumidityOffset / 16f) * 16f), 0, 15);
			}
			return MathUtils.Clamp((int)((MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.m_humidityOffset.X, z + this.m_humidityOffset.Y, 0.0008f / this.TGBiomeScaling, 5, 1.97f, 1f, false) - 1.5f) + this.m_worldSettings.HumidityOffset / 16f) * 16f), 0, 15);
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x000A89C0 File Offset: 0x000A6BC0
		public float CalculateOceanShoreX(float z)
		{
			return this.m_oceanCorner.X + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(z, 0f, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f, false);
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x000A8A04 File Offset: 0x000A6C04
		public float CalculateOceanShoreZ(float x)
		{
			return this.m_oceanCorner.Y + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(0f, x, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f, false);
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x000A8A48 File Offset: 0x000A6C48
		public float ScoreSpawnPosition(int x, int z)
		{
			float num = 0f;
			float num2 = this.CalculateOceanShoreDistance((float)x, (float)z);
			float num3 = this.CalculateMountainRangeFactor((float)x, (float)z);
			int num4 = this.CalculateHumidity((float)x, (float)z);
			int num5 = this.CalculateTemperature((float)x, (float)z);
			if (num2 < 0f)
			{
				num -= 1f;
			}
			if (num2 > 10f)
			{
				num -= 1f;
			}
			if (num3 > 0.66f)
			{
				num -= 0.5f;
			}
			if (num4 < 10)
			{
				num -= 1f;
			}
			if (num5 < 2)
			{
				num -= 0.5f;
			}
			float x2 = this.CalculateHeight((float)x, (float)z);
			float x3 = this.CalculateHeight((float)(x - 5), (float)(z - 5));
			float x4 = this.CalculateHeight((float)(x - 5), (float)(z + 5));
			float x5 = this.CalculateHeight((float)(x + 5), (float)(z - 5));
			float x6 = this.CalculateHeight((float)(x + 5), (float)(z + 5));
			float num6 = MathUtils.Min(x2, MathUtils.Min(x3, x4, x5, x6));
			float num7 = MathUtils.Max(x2, MathUtils.Max(x3, x4, x5, x6));
			if (num6 < 64f || num7 > 75f)
			{
				num -= 1f;
			}
			return num;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x000A8B5C File Offset: 0x000A6D5C
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

		// Token: 0x0600165A RID: 5722 RVA: 0x000A8BCC File Offset: 0x000A6DCC
		public void GenerateTerrain(TerrainChunk chunk, int x1, int z1, int x2, int z2)
		{
			int num = x2 - x1;
			int num2 = z2 - z1;
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int num3 = chunk.Origin.X + x1;
			int num4 = chunk.Origin.Y + z1;
			TerrainContentGeneratorVersion2_1_inactive.Grid2d grid2d = new TerrainContentGeneratorVersion2_1_inactive.Grid2d(num, num2);
			TerrainContentGeneratorVersion2_1_inactive.Grid2d grid2d2 = new TerrainContentGeneratorVersion2_1_inactive.Grid2d(num, num2);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					grid2d.Set(j, i, this.CalculateOceanShoreDistance((float)(j + num3), (float)(i + num4)));
					grid2d2.Set(j, i, this.CalculateMountainRangeFactor((float)(j + num3), (float)(i + num4)));
				}
			}
			TerrainContentGeneratorVersion2_1_inactive.Grid3d grid3d = new TerrainContentGeneratorVersion2_1_inactive.Grid3d(num / 4 + 1, 33, num2 / 4 + 1);
			for (int k = 0; k < grid3d.SizeX; k++)
			{
				for (int l = 0; l < grid3d.SizeZ; l++)
				{
					int num5 = k * 4 + num3;
					int num6 = l * 4 + num4;
					float num7 = this.CalculateHeight((float)num5, (float)num6);
					float num8 = this.CalculateMountainRangeFactor((float)num5, (float)num6);
					float num9 = MathUtils.Saturate(0.9f * (num8 - 0.8f) / 0.2f + 0.1f);
					for (int m = 0; m < grid3d.SizeY; m++)
					{
						int num10 = m * 8;
						float num11 = num7 - this.TGTurbulenceTopOffset;
						float num12 = MathUtils.Lerp(0f, this.TGTurbulenceStrength * num9, MathUtils.Saturate((num11 - (float)num10) * 0.2f)) * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise((float)num5, (float)(num10 + 1000), (float)num6, 0.008f, 3, 2f, 0.75f, false) - 1f, this.TGTurbulencePower);
						float num13 = (float)num10 + num12;
						float num14 = num7 - num13;
						num14 += MathUtils.Max(4f * (this.TGDensityBias - (float)num10), 0f);
						grid3d.Set(k, m, l, num14);
					}
				}
			}
			int oceanLevel = this.OceanLevel;
			for (int n = 0; n < grid3d.SizeX - 1; n++)
			{
				for (int num15 = 0; num15 < grid3d.SizeZ - 1; num15++)
				{
					for (int num16 = 0; num16 < grid3d.SizeY - 1; num16++)
					{
						float num17;
						float num18;
						float num19;
						float num20;
						float num21;
						float num22;
						float num23;
						float num24;
						grid3d.Get8(n, num16, num15, out num17, out num18, out num19, out num20, out num21, out num22, out num23, out num24);
						float num25 = (num18 - num17) / 4f;
						float num26 = (num20 - num19) / 4f;
						float num27 = (num22 - num21) / 4f;
						float num28 = (num24 - num23) / 4f;
						float num29 = num17;
						float num30 = num19;
						float num31 = num21;
						float num32 = num23;
						for (int num33 = 0; num33 < 4; num33++)
						{
							float num34 = (num31 - num29) / 4f;
							float num35 = (num32 - num30) / 4f;
							float num36 = num29;
							float num37 = num30;
							for (int num38 = 0; num38 < 4; num38++)
							{
								float num39 = (num37 - num36) / 8f;
								float num40 = num36;
								int num41 = num33 + n * 4;
								int num42 = num38 + num15 * 4;
								int x3 = x1 + num41;
								int z3 = z1 + num42;
								float x4 = grid2d.Get(num41, num42);
								float num43 = grid2d2.Get(num41, num42);
								int temperatureFast = chunk.GetTemperatureFast(x3, z3);
								int humidityFast = chunk.GetHumidityFast(x3, z3);
								float f = num43 - 0.01f * (float)humidityFast;
								float num44 = MathUtils.Lerp(100f, 0f, f);
								float num45 = MathUtils.Lerp(300f, 30f, f);
								bool flag = (temperatureFast > 8 && humidityFast < 8 && num43 < 0.95f) || (MathUtils.Abs(x4) < 12f && num43 < 0.9f);
								int num46 = TerrainChunk.CalculateCellIndex(x3, 0, z3);
								for (int num47 = 0; num47 < 8; num47++)
								{
									int num48 = num47 + num16 * 8;
									int value = 0;
									if (num40 < 0f)
									{
										if (num48 <= oceanLevel)
										{
											value = 18;
										}
									}
									else
									{
										value = ((!flag) ? ((num40 >= num45) ? 67 : 3) : ((num40 >= num44) ? ((num40 >= num45) ? 67 : 3) : 4));
									}
									chunk.SetCellValueFast(num46 + num48, value);
									num40 += num39;
								}
								num36 += num34;
								num37 += num35;
							}
                            //num29 += num25;
                            //num30 += num26;
                            //num31 += num27;
                            //num32 += num28;
                            num29 += MathUtils.Sin(num25);
							num30 += num26;
							num31 += num27;
							num32 += num28;
						}
					}
				}
			}
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x000A9050 File Offset: 0x000A7250
		public void GenerateSurface(TerrainChunk chunk)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + chunk.Coords.X + 101 * chunk.Coords.Y);
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
							float f = MathUtils.Saturate(MathUtils.Saturate((num5 - 0.9f) / 0.1f) - MathUtils.Saturate(((float)humidity - 3f) / 12f) + TerrainContentGeneratorVersion2_1_inactive.TGSurfaceMultiplier * MathUtils.Saturate(((float)k - 85f) * 0.05f));
							int min = (int)MathUtils.Lerp(4f, 0f, f);
							int max = (int)MathUtils.Lerp(7f, 0f, f);
							int num6 = MathUtils.Min(random.UniformInt(min, max), k);
							int contents;
							if (num4 == 4)
							{
								contents = ((temperature > 4 && temperature < 7) ? 6 : 7);
							}
							else
							{
								int num7 = temperature / 4;
								int num8 = (k + 1 < 255) ? chunk.GetCellContentsFast(i, k + 1, j) : 0;
								contents = (((k < 66 || k == 84 + num7 || k == 103 + num7) && humidity == 9 && temperature % 6 == 1) ? 66 : ((num8 != 18 || humidity <= 8 || humidity % 2 != 0 || temperature % 3 != 0) ? 2 : 72));
							}
							int num9 = TerrainChunk.CalculateCellIndex(i, k + 1, j);
							for (int l = num9 - num6; l < num9; l++)
							{
								if (Terrain.ExtractContents(chunk.GetCellValueFast(l)) != 0)
								{
									int value = Terrain.ReplaceContents(0, contents);
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

		// Token: 0x0600165C RID: 5724 RVA: 0x000A9294 File Offset: 0x000A7494
		public void GenerateMinerals(TerrainChunk chunk)
		{
			if (!this.TGCavesAndPockets)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 1; j <= y + 1; j++)
				{
					TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + i + 119 * j);
					int num = random.UniformInt(0, 10);
					for (int k = 0; k < num; k++)
					{
						random.UniformInt(0, 1);
					}
					float num2 = this.CalculateMountainRangeFactor((float)(i * 16), (float)(j * 16));
					int num3 = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)i, (float)j, 0.33f, 1, 1f, 1f, false));
					for (int l = 0; l < num3; l++)
					{
						int x2 = i * 16 + random.UniformInt(0, 15);
						int y2 = random.UniformInt(5, 80);
						int z = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_coalBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_coalBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z, 3);
					}
					int num4 = (int)(6f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 1211), (float)(j + 396), 0.33f, 1, 1f, 1f, false));
					for (int m = 0; m < num4; m++)
					{
						int x3 = i * 16 + random.UniformInt(0, 15);
						int y3 = random.UniformInt(20, 65);
						int z2 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_copperBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_copperBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z2, 3);
					}
					int num5 = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 713), (float)(j + 211), 0.33f, 1, 1f, 1f, false));
					for (int n = 0; n < num5; n++)
					{
						int x4 = i * 16 + random.UniformInt(0, 15);
						int y4 = random.UniformInt(2, 40);
						int z3 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_ironBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_ironBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z3, 67);
					}
					int num6 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 915), (float)(j + 272), 0.33f, 1, 1f, 1f, false));
					for (int num7 = 0; num7 < num6; num7++)
					{
						int x5 = i * 16 + random.UniformInt(0, 15);
						int y5 = random.UniformInt(50, 70);
						int z4 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_saltpeterBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_saltpeterBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z4, 4);
					}
					int num8 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 711), (float)(j + 1194), 0.33f, 1, 1f, 1f, false));
					for (int num9 = 0; num9 < num8; num9++)
					{
						int x6 = i * 16 + random.UniformInt(0, 15);
						int y6 = random.UniformInt(2, 40);
						int z5 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_sulphurBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_sulphurBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z5, 67);
					}
					int num10 = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 432), (float)(j + 907), 0.33f, 1, 1f, 1f, false));
					for (int num11 = 0; num11 < num10; num11++)
					{
						int x7 = i * 16 + random.UniformInt(0, 15);
						int y7 = random.UniformInt(2, 15);
						int z6 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_diamondBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_diamondBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z6, 67);
					}
					int num12 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 799), (float)(j + 131), 0.33f, 1, 1f, 1f, false));
					for (int num13 = 0; num13 < num12; num13++)
					{
						int x8 = i * 16 + random.UniformInt(0, 15);
						int y8 = random.UniformInt(2, 50);
						int z7 = j * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_germaniumBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_germaniumBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z7, 67);
					}
				}
			}
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x000A97A4 File Offset: 0x000A79A4
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
					TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + num + 71 * num2);
					int num3 = random.UniformInt(0, 10);
					for (int k = 0; k < num3; k++)
					{
						random.UniformInt(0, 1);
					}
					float num4 = this.CalculateMountainRangeFactor((float)(num * 16), (float)(num2 * 16));
					for (int l = 0; l < 3; l++)
					{
						int x = num * 16 + random.UniformInt(0, 15);
						int y = random.UniformInt(50, 100);
						int z = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_dirtPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_dirtPocketBrushes.Count - 1)].PaintFastSelective(chunk, x, y, z, 3);
					}
					for (int m = 0; m < 10; m++)
					{
						int x2 = num * 16 + random.UniformInt(0, 15);
						int y2 = random.UniformInt(20, 80);
						int z2 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_gravelPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_gravelPocketBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z2, 3);
					}
					for (int n = 0; n < 2; n++)
					{
						int x3 = num * 16 + random.UniformInt(0, 15);
						int y3 = random.UniformInt(20, 120);
						int z3 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_limestonePocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_limestonePocketBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z3, 3);
					}
					for (int num5 = 0; num5 < 1; num5++)
					{
						int x4 = num * 16 + random.UniformInt(0, 15);
						int y4 = random.UniformInt(50, 70);
						int z4 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_clayPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_clayPocketBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z4, 3);
					}
					for (int num6 = 0; num6 < 6; num6++)
					{
						int x5 = num * 16 + random.UniformInt(0, 15);
						int y5 = random.UniformInt(40, 80);
						int z5 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_sandPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_sandPocketBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z5, 4);
					}
					for (int num7 = 0; num7 < 4; num7++)
					{
						int x6 = num * 16 + random.UniformInt(0, 15);
						int y6 = random.UniformInt(40, 60);
						int z6 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_basaltPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z6, 4);
					}
					for (int num8 = 0; num8 < 3; num8++)
					{
						int x7 = num * 16 + random.UniformInt(0, 15);
						int y7 = random.UniformInt(20, 40);
						int z7 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_basaltPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z7, 3);
					}
					for (int num9 = 0; num9 < 6; num9++)
					{
						int x8 = num * 16 + random.UniformInt(0, 15);
						int y8 = random.UniformInt(4, 50);
						int z8 = num2 * 16 + random.UniformInt(0, 15);
						TerrainContentGeneratorVersion2_1_inactive.m_granitePocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_granitePocketBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z8, 67);
					}
					if (random.Bool(0.02f + 0.01f * num4))
					{
						int num10 = num * 16;
						int num11 = random.UniformInt(40, 60);
						int num12 = num2 * 16;
						int num13 = random.UniformInt(1, 3);
						for (int num14 = 0; num14 < num13; num14++)
						{
							Vector2 vector = random.Vector2(7f, false);
							int num15 = 8 + (int)MathUtils.Round(vector.X);
							int num16 = 0;
							int num17 = 8 + (int)MathUtils.Round(vector.Y);
							TerrainContentGeneratorVersion2_1_inactive.m_waterPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_waterPocketBrushes.Count - 1)].PaintFast(chunk, num10 + num15, num11 + num16, num12 + num17);
						}
					}
					if (random.Bool(0.06f + 0.05f * num4))
					{
						int num18 = num * 16;
						int num19 = random.UniformInt(15, 42);
						int num20 = num2 * 16;
						int num21 = random.UniformInt(1, 2);
						for (int num22 = 0; num22 < num21; num22++)
						{
							Vector2 vector2 = random.Vector2(7f, false);
							int num23 = 8 + (int)MathUtils.Round(vector2.X);
							int num24 = random.UniformInt(0, 1);
							int num25 = 8 + (int)MathUtils.Round(vector2.Y);
							TerrainContentGeneratorVersion2_1_inactive.m_magmaPocketBrushes[random.UniformInt(0, TerrainContentGeneratorVersion2_1_inactive.m_magmaPocketBrushes.Count - 1)].PaintFast(chunk, num18 + num23, num19 + num24, num20 + num25);
						}
					}
				}
			}
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x000A9D0C File Offset: 0x000A7F0C
		public void GenerateCaves(TerrainChunk chunk)
		{
			if (!this.TGCavesAndPockets)
			{
				return;
			}
			List<TerrainContentGeneratorVersion2_1_inactive.CavePoint> list = new List<TerrainContentGeneratorVersion2_1_inactive.CavePoint>();
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			for (int i = x - 2; i <= x + 2; i++)
			{
				for (int j = y - 2; j <= y + 2; j++)
				{
					list.Clear();
					TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + i + 9973 * j);
					int num = i * 16 + random.UniformInt(0, 15);
					int num2 = j * 16 + random.UniformInt(0, 15);
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
							list.Add(new TerrainContentGeneratorVersion2_1_inactive.CavePoint
							{
								Position = position,
								Direction = vector,
								BrushType = 0,
								Length = random.UniformInt(80, 240)
							});
						}
						int num6 = i * 16 + 8;
						int num7 = j * 16 + 8;
						int k = 0;
						while (k < list.Count)
						{
							TerrainContentGeneratorVersion2_1_inactive.CavePoint cavePoint = list[k];
							List<TerrainBrush> list2 = TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType[cavePoint.BrushType];
							list2[random.UniformInt(0, list2.Count - 1)].PaintFastAvoidWater(chunk, Terrain.ToCell(cavePoint.Position.X), Terrain.ToCell(cavePoint.Position.Y), Terrain.ToCell(cavePoint.Position.Z));
							cavePoint.Position += 2f * cavePoint.Direction;
							cavePoint.StepsTaken += 2;
							float num8 = cavePoint.Position.X - (float)num6;
							float num9 = cavePoint.Position.Z - (float)num7;
							if (random.Bool(0.5f))
							{
								Vector3 vector2 = Vector3.Normalize(random.Vector3(1f, true));
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
								cavePoint.Direction = Vector3.Normalize(random.Vector3(1f, true) * new Vector3(1f, 0.33f, 1f));
							}
							if (cavePoint.StepsTaken > 20 && random.Bool(0.05f))
							{
								cavePoint.Direction.Y = 0f;
								cavePoint.BrushType = MathUtils.Min(cavePoint.BrushType + 2, TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType.Count - 1);
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
								cavePoint.BrushType = (int)(MathUtils.Pow(random.UniformFloat(0f, 0.999f), 7f) * (float)TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType.Count);
							}
							if (random.Bool(0.06f) && list.Count < 12 && cavePoint.StepsTaken > 20 && cavePoint.Position.Y < 58f)
							{
								list.Add(new TerrainContentGeneratorVersion2_1_inactive.CavePoint
								{
									Position = cavePoint.Position,
									Direction = Vector3.Normalize(random.UniformVector3(1f, 1f, false) * new Vector3(1f, 0.33f, 1f)),
									BrushType = (int)(MathUtils.Pow(random.UniformFloat(0f, 0.999f), 7f) * (float)TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType.Count),
									Length = random.UniformInt(40, 180)
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

		// Token: 0x0600165F RID: 5727 RVA: 0x000AA388 File Offset: 0x000A8588
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
					if (num4 > 0.95f)
					{
						num5 = 1 + (random.Bool(0.25f) ? 1 : 0);
					}
					else if (num4 > 0.5f)
					{
						num5 = (random.Bool(0.25f) ? 1 : 0);
					}
					int num6 = 0;
					int num7 = 0;
					while (num7 < 8 && num6 < num5)
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
									TreeType? treeType = PlantsManager.GenerateRandomTreeType(random, num3 + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num10), humidity, num10, 1f);
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
												if (random.Bool(0.3f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 + point2.X, num10, num15 + point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 + point2.X, num10, num15 + point2.Z, treeLeavesValue);
												}
												if (random.Bool(0.05f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 + point2.X, num10, num15 + point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 + point2.X, num10, num15 + point2.Z, value);
												}
												if (random.Bool(0.3f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 - point2.X, num10, num15 - point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 - point2.X, num10, num15 - point2.Z, treeLeavesValue);
												}
												if (random.Bool(0.05f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14 - point2.X, num10, num15 - point2.Z)].IsCollidable)
												{
													terrain.SetCellValueFast(num14 - point2.X, num10, num15 - point2.Z, value);
												}
												if (random.Bool(0.1f) && !BlocksManager.Blocks[terrain.GetCellContentsFast(num14, num10 + 1, num15)].IsCollidable)
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

		// Token: 0x06001660 RID: 5728 RVA: 0x000AAA84 File Offset: 0x000A8C84
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

		// Token: 0x06001661 RID: 5729 RVA: 0x000AAB28 File Offset: 0x000A8D28
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

		// Token: 0x06001662 RID: 5730 RVA: 0x000AAC14 File Offset: 0x000A8E14
		public void GenerateBottomSuckers(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + chunk.Coords.X + 2210 * chunk.Coords.Y);
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
									int face = random.UniformInt(0, 5);
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
											float num7 = random.UniformFloat(0f, 1f);
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
												int face2 = random.UniformInt(0, 3);
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

		// Token: 0x06001663 RID: 5731 RVA: 0x000AAEA8 File Offset: 0x000A90A8
		public void GenerateCacti(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + x + 1991 * y);
			if (!random.Bool(0.5f))
			{
				return;
			}
			int num = random.UniformInt(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.UniformInt(3, 12);
				int num3 = random.UniformInt(3, 12);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast < 6 && temperatureFast > 8)
				{
					for (int j = 0; j < 8; j++)
					{
						int num4 = num2 + random.UniformInt(-2, 2);
						int num5 = num3 + random.UniformInt(-2, 2);
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

		// Token: 0x06001664 RID: 5732 RVA: 0x000AB02C File Offset: 0x000A922C
		public void GeneratePumpkins(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + x + 1495 * y);
			if (!random.Bool(0.2f))
			{
				return;
			}
			int num = random.UniformInt(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.UniformInt(1, 14);
				int num3 = random.UniformInt(1, 14);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast >= 10 && temperatureFast > 6)
				{
					for (int j = 0; j < 5; j++)
					{
						int x2 = num2 + random.UniformInt(-1, 1);
						int z = num3 + random.UniformInt(-1, 1);
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

		// Token: 0x06001665 RID: 5733 RVA: 0x000AB174 File Offset: 0x000A9374
		public void GenerateKelp(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(0);
			float num = 0f;
			for (int i = 0; i < 9; i++)
			{
				int num2 = i % 3 - 1;
				int num3 = i / 3 - 1;
				random.Reset(this.m_seed + (x + num2) + 850 * (y + num3));
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
			random.Reset(this.m_seed + x + 850 * y);
			int num4 = random.UniformInt(0, MathUtils.Max((int)(256f * num), 1));
			for (int j = 0; j < num4; j++)
			{
				int num5 = random.UniformInt(2, 13);
				int num6 = random.UniformInt(2, 13);
				int num7 = num5 + chunk.Origin.X;
				int num8 = num6 + chunk.Origin.Y;
				int num9 = random.UniformInt(10, 26);
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
						int x2 = num5 + random.UniformInt(-2, 2);
						int z = num6 + random.UniformInt(-2, 2);
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
										int num13 = flag ? random.UniformInt(num11 - 2, num11 - 1) : random.UniformInt(num11 - 1, num11);
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

		// Token: 0x06001666 RID: 5734 RVA: 0x000AB3B8 File Offset: 0x000A95B8
		public void GenerateSeagrass(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + x + 378 * y);
			for (int i = 0; i < 6; i++)
			{
				int num = random.UniformInt(1, 14);
				int num2 = random.UniformInt(1, 14);
				int num3 = chunk.Origin.X + num;
				int num4 = chunk.Origin.Y + num2;
				bool flag = this.CalculateOceanShoreDistance((float)num3, (float)num4) < 10f;
				int num5 = random.UniformInt(1, 3);
				for (int j = 0; j < num5; j++)
				{
					int x2 = num + random.UniformInt(-1, 1);
					int z = num2 + random.UniformInt(-1, 1);
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

		// Token: 0x06001667 RID: 5735 RVA: 0x000AB55C File Offset: 0x000A975C
		public void GenerateIvy(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + chunk.Coords.X + 2191 * chunk.Coords.Y);
			int num = random.UniformInt(0, MathUtils.Max(12, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.UniformInt(4, 11);
				int num3 = random.UniformInt(4, 11);
				int humidityFast = chunk.GetHumidityFast(num2, num3);
				int temperatureFast = chunk.GetTemperatureFast(num2, num3);
				if (humidityFast > 10 && temperatureFast > 10)
				{
					int num4 = chunk.CalculateTopmostCellHeight(num2, num3);
					int j = 0;
					while (j < 100)
					{
						int num5 = num2 + random.UniformInt(-3, 3);
						int num6 = MathUtils.Clamp(num4 + random.UniformInt(-10, 1), 1, 255);
						int num7 = num3 + random.UniformInt(-3, 3);
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
						int num9 = random.UniformInt(0, 3);
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

		// Token: 0x06001668 RID: 5736 RVA: 0x000AB7C8 File Offset: 0x000A99C8
		public void GenerateTraps(TerrainChunk chunk)
		{
            if (!this.TGExtras)
                return;
            int x1 = chunk.Coords.X;
            int y1 = chunk.Coords.Y;
            Terrain terrain = this.m_subsystemTerrain.Terrain;
            TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(this.m_seed + x1 + 2113 * y1);
            if (!random.Bool(0.15f) || (double)this.CalculateOceanShoreDistance((float)chunk.Origin.X, (float)chunk.Origin.Y) <= 50.0)
                return;
            int num1 = random.UniformInt(0, MathUtils.Max(2, 1));
        label_31:
            for (int index = 0; index < num1; ++index)
            {
                int num2 = random.UniformInt(2, 5);
                int num3 = random.UniformInt(2, 5);
                int num4 = random.UniformInt(1, 16 - num2 - 2);
                int num5 = random.UniformInt(1, 16 - num3 - 2);
                bool flag = (double)random.UniformFloat(0.0f, 1f) < 0.5;
                int num6 = random.UniformInt(3, 5);
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
                                    int data = SpikedPlankBlock.SetSpikesState(0, (double)random.UniformFloat(0.0f, 1f) < 0.330000013113022);
                                    chunk.SetCellValueFast(x3, nullable1.Value - num6 + 1, z, Terrain.MakeBlockValue(86, 0, data));
                                }
                            }
                        }
                    }
                }
            }
        }

		// Token: 0x06001669 RID: 5737 RVA: 0x000ABADC File Offset: 0x000A9CDC
		public void GenerateGraves(TerrainChunk chunk)
		{
			if (!this.TGExtras)
			{
				return;
			}
			int x = chunk.Coords.X;
			int y = chunk.Coords.Y;
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random((int)MathUtils.Hash((uint)(this.m_seed + x + 10323 * y)));
			if (random.UniformFloat(0f, 1f) >= 0.033f || this.CalculateOceanShoreDistance((float)chunk.Origin.X, (float)chunk.Origin.Y) <= 10f)
			{
				return;
			}
			int num = random.UniformInt(0, MathUtils.Max(1, 1));
			for (int i = 0; i < num; i++)
			{
				int num2 = random.UniformInt(6, 9);
				int num3 = random.UniformInt(6, 9);
				int num4 = random.Bool(0.2f) ? random.UniformInt(6, 20) : random.UniformInt(1, 5);
				bool flag = random.Bool(0.5f);
				for (int j = 0; j < num4; j++)
				{
					int num5 = num2 + random.UniformInt(-4, 4);
					int num6 = num3 + random.UniformInt(-4, 4);
					int num7 = chunk.CalculateTopmostCellHeight(num5, num6);
					if (num7 >= 10 && num7 <= 246)
					{
						int num8 = random.UniformInt(0, 3);
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
							int num15 = random.UniformInt(0, 7);
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
								else if (random.UniformFloat(0f, 1f) < 0.5f)
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
							float num17 = random.UniformFloat(0f, 1f);
							float num18 = random.UniformFloat(0f, 1f);
							int num19 = random.UniformInt(-1, 0);
							int num20 = random.UniformInt(1, 2);
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

		// Token: 0x0600166A RID: 5738 RVA: 0x000AC1BC File Offset: 0x000AA3BC
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

		// Token: 0x0600166B RID: 5739 RVA: 0x000AC334 File Offset: 0x000AA534
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

		// Token: 0x0600166C RID: 5740 RVA: 0x000AC3B0 File Offset: 0x000AA5B0
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

		// Token: 0x0600166D RID: 5741 RVA: 0x000AC454 File Offset: 0x000AA654
		public static void CreateBrushes()
		{
			TerrainContentGeneratorVersion2_1_inactive.Random random = new TerrainContentGeneratorVersion2_1_inactive.Random(17);
			for (int i = 0; i < 16; i++)
			{
				TerrainBrush terrainBrush = new TerrainBrush();
				int num = random.UniformInt(4, 12);
				for (int j = 0; j < num; j++)
				{
					Vector3 v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num2 = random.UniformInt(3, 8);
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
				TerrainContentGeneratorVersion2_1_inactive.m_coalBrushes.Add(terrainBrush);
			}
			for (int l = 0; l < 16; l++)
			{
				TerrainBrush terrainBrush2 = new TerrainBrush();
				int num3 = random.UniformInt(3, 7);
				for (int m = 0; m < num3; m++)
				{
					Vector3 v2 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num4 = random.UniformInt(3, 6);
					Vector3 vector2 = Vector3.Zero;
					for (int n = 0; n < num4; n++)
					{
						terrainBrush2.AddBox((int)MathUtils.Floor(vector2.X), (int)MathUtils.Floor(vector2.Y), (int)MathUtils.Floor(vector2.Z), 1, 1, 1, 39);
						vector2 += v2;
					}
				}
				terrainBrush2.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_ironBrushes.Add(terrainBrush2);
			}
			for (int num5 = 0; num5 < 16; num5++)
			{
				TerrainBrush terrainBrush3 = new TerrainBrush();
				int num6 = random.UniformInt(4, 10);
				for (int num7 = 0; num7 < num6; num7++)
				{
					Vector3 v3 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-2f, 2f), random.UniformFloat(-1f, 1f)));
					int num8 = random.UniformInt(3, 6);
					Vector3 vector3 = Vector3.Zero;
					for (int num9 = 0; num9 < num8; num9++)
					{
						terrainBrush3.AddBox((int)MathUtils.Floor(vector3.X), (int)MathUtils.Floor(vector3.Y), (int)MathUtils.Floor(vector3.Z), 1, 1, 1, 41);
						vector3 += v3;
					}
				}
				terrainBrush3.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_copperBrushes.Add(terrainBrush3);
			}
			for (int num10 = 0; num10 < 16; num10++)
			{
				TerrainBrush terrainBrush4 = new TerrainBrush();
				int num11 = random.UniformInt(8, 16);
				for (int num12 = 0; num12 < num11; num12++)
				{
					Vector3 v4 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.25f, 0.25f), random.UniformFloat(-1f, 1f)));
					int num13 = random.UniformInt(4, 8);
					Vector3 vector4 = Vector3.Zero;
					for (int num14 = 0; num14 < num13; num14++)
					{
						terrainBrush4.AddBox((int)MathUtils.Floor(vector4.X), (int)MathUtils.Floor(vector4.Y), (int)MathUtils.Floor(vector4.Z), 1, 1, 1, 100);
						vector4 += v4;
					}
				}
				terrainBrush4.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_saltpeterBrushes.Add(terrainBrush4);
			}
			for (int num15 = 0; num15 < 16; num15++)
			{
				TerrainBrush terrainBrush5 = new TerrainBrush();
				int num16 = random.UniformInt(4, 10);
				for (int num17 = 0; num17 < num16; num17++)
				{
					Vector3 v5 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num18 = random.UniformInt(3, 6);
					Vector3 vector5 = Vector3.Zero;
					for (int num19 = 0; num19 < num18; num19++)
					{
						terrainBrush5.AddBox((int)MathUtils.Floor(vector5.X), (int)MathUtils.Floor(vector5.Y), (int)MathUtils.Floor(vector5.Z), 1, 1, 1, 101);
						vector5 += v5;
					}
				}
				terrainBrush5.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_sulphurBrushes.Add(terrainBrush5);
			}
			for (int num20 = 0; num20 < 16; num20++)
			{
				TerrainBrush terrainBrush6 = new TerrainBrush();
				int num21 = random.UniformInt(2, 6);
				for (int num22 = 0; num22 < num21; num22++)
				{
					Vector3 v6 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num23 = random.UniformInt(3, 6);
					Vector3 vector6 = Vector3.Zero;
					for (int num24 = 0; num24 < num23; num24++)
					{
						terrainBrush6.AddBox((int)MathUtils.Floor(vector6.X), (int)MathUtils.Floor(vector6.Y), (int)MathUtils.Floor(vector6.Z), 1, 1, 1, 112);
						vector6 += v6;
					}
				}
				terrainBrush6.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_diamondBrushes.Add(terrainBrush6);
			}
			for (int num25 = 0; num25 < 16; num25++)
			{
				TerrainBrush terrainBrush7 = new TerrainBrush();
				int num26 = random.UniformInt(4, 10);
				for (int num27 = 0; num27 < num26; num27++)
				{
					Vector3 v7 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num28 = random.UniformInt(3, 6);
					Vector3 vector7 = Vector3.Zero;
					for (int num29 = 0; num29 < num28; num29++)
					{
						terrainBrush7.AddBox((int)MathUtils.Floor(vector7.X), (int)MathUtils.Floor(vector7.Y), (int)MathUtils.Floor(vector7.Z), 1, 1, 1, 148);
						vector7 += v7;
					}
				}
				terrainBrush7.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_germaniumBrushes.Add(terrainBrush7);
			}
			for (int num30 = 0; num30 < 16; num30++)
			{
				TerrainBrush terrainBrush8 = new TerrainBrush();
				int num31 = random.UniformInt(16, 32);
				for (int num32 = 0; num32 < num31; num32++)
				{
					Vector3 v8 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.75f, 0.75f), random.UniformFloat(-1f, 1f)));
					int num33 = random.UniformInt(6, 12);
					Vector3 vector8 = Vector3.Zero;
					for (int num34 = 0; num34 < num33; num34++)
					{
						terrainBrush8.AddBox((int)MathUtils.Floor(vector8.X), (int)MathUtils.Floor(vector8.Y), (int)MathUtils.Floor(vector8.Z), 1, 1, 1, 2);
						vector8 += v8;
					}
				}
				terrainBrush8.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_dirtPocketBrushes.Add(terrainBrush8);
			}
			for (int num35 = 0; num35 < 16; num35++)
			{
				TerrainBrush terrainBrush9 = new TerrainBrush();
				int num36 = random.UniformInt(16, 32);
				for (int num37 = 0; num37 < num36; num37++)
				{
					Vector3 v9 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.75f, 0.75f), random.UniformFloat(-1f, 1f)));
					int num38 = random.UniformInt(6, 12);
					Vector3 vector9 = Vector3.Zero;
					for (int num39 = 0; num39 < num38; num39++)
					{
						terrainBrush9.AddBox((int)MathUtils.Floor(vector9.X), (int)MathUtils.Floor(vector9.Y), (int)MathUtils.Floor(vector9.Z), 1, 1, 1, 6);
						vector9 += v9;
					}
				}
				terrainBrush9.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_gravelPocketBrushes.Add(terrainBrush9);
			}
			for (int num40 = 0; num40 < 16; num40++)
			{
				TerrainBrush terrainBrush10 = new TerrainBrush();
				int num41 = random.UniformInt(16, 32);
				for (int num42 = 0; num42 < num41; num42++)
				{
					Vector3 v10 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.75f, 0.75f), random.UniformFloat(-1f, 1f)));
					int num43 = random.UniformInt(6, 12);
					Vector3 vector10 = Vector3.Zero;
					for (int num44 = 0; num44 < num43; num44++)
					{
						terrainBrush10.AddBox((int)MathUtils.Floor(vector10.X), (int)MathUtils.Floor(vector10.Y), (int)MathUtils.Floor(vector10.Z), 1, 1, 1, 66);
						vector10 += v10;
					}
				}
				terrainBrush10.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_limestonePocketBrushes.Add(terrainBrush10);
			}
			for (int num45 = 0; num45 < 16; num45++)
			{
				TerrainBrush terrainBrush11 = new TerrainBrush();
				int num46 = random.UniformInt(16, 32);
				for (int num47 = 0; num47 < num46; num47++)
				{
					Vector3 v11 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.1f, 0.1f), random.UniformFloat(-1f, 1f)));
					int num48 = random.UniformInt(6, 12);
					Vector3 vector11 = Vector3.Zero;
					for (int num49 = 0; num49 < num48; num49++)
					{
						terrainBrush11.AddBox((int)MathUtils.Floor(vector11.X), (int)MathUtils.Floor(vector11.Y), (int)MathUtils.Floor(vector11.Z), 1, 1, 1, 72);
						vector11 += v11;
					}
				}
				terrainBrush11.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_clayPocketBrushes.Add(terrainBrush11);
			}
			for (int num50 = 0; num50 < 16; num50++)
			{
				TerrainBrush terrainBrush12 = new TerrainBrush();
				int num51 = random.UniformInt(16, 32);
				for (int num52 = 0; num52 < num51; num52++)
				{
					Vector3 v12 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.75f, 0.75f), random.UniformFloat(-1f, 1f)));
					int num53 = random.UniformInt(6, 12);
					Vector3 vector12 = Vector3.Zero;
					for (int num54 = 0; num54 < num53; num54++)
					{
						terrainBrush12.AddBox((int)MathUtils.Floor(vector12.X), (int)MathUtils.Floor(vector12.Y), (int)MathUtils.Floor(vector12.Z), 1, 1, 1, 7);
						vector12 += v12;
					}
				}
				terrainBrush12.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_sandPocketBrushes.Add(terrainBrush12);
			}
			for (int num55 = 0; num55 < 16; num55++)
			{
				TerrainBrush terrainBrush13 = new TerrainBrush();
				int num56 = random.UniformInt(16, 32);
				for (int num57 = 0; num57 < num56; num57++)
				{
					Vector3 v13 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.75f, 0.75f), random.UniformFloat(-1f, 1f)));
					int num58 = random.UniformInt(6, 12);
					Vector3 vector13 = Vector3.Zero;
					for (int num59 = 0; num59 < num58; num59++)
					{
						terrainBrush13.AddBox((int)MathUtils.Floor(vector13.X), (int)MathUtils.Floor(vector13.Y), (int)MathUtils.Floor(vector13.Z), 1, 1, 1, 67);
						vector13 += v13;
					}
				}
				terrainBrush13.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_basaltPocketBrushes.Add(terrainBrush13);
			}
			for (int num60 = 0; num60 < 16; num60++)
			{
				TerrainBrush terrainBrush14 = new TerrainBrush();
				int num61 = random.UniformInt(16, 32);
				for (int num62 = 0; num62 < num61; num62++)
				{
					Vector3 v14 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					int num63 = random.UniformInt(5, 10);
					Vector3 vector14 = Vector3.Zero;
					for (int num64 = 0; num64 < num63; num64++)
					{
						terrainBrush14.AddBox((int)MathUtils.Floor(vector14.X), (int)MathUtils.Floor(vector14.Y), (int)MathUtils.Floor(vector14.Z), 1, 1, 1, 3);
						vector14 += v14;
					}
				}
				terrainBrush14.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_granitePocketBrushes.Add(terrainBrush14);
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
				bool circular = num65 % 4 >= 2;
				int num69 = (num65 % 4 == 1) ? (num66 * num66) : (2 * num66 * num66);
				for (int num70 = 0; num70 < num69; num70++)
				{
					Vector2 vector15 = random.UniformVector2(0f, (float)num66, circular);
					float num71 = vector15.Length();
					int num72 = random.UniformInt(3, 4);
					int sizeY = 1 + (int)MathUtils.Lerp(MathUtils.Max((float)(num66 / 3), 2.5f) * num68, 0f, num71 / (float)num66) + random.UniformInt(0, 1);
					terrainBrush15.AddBox((int)MathUtils.Floor(vector15.X), 0, (int)MathUtils.Floor(vector15.Y), num72, sizeY, num72, 0);
					terrainBrush15.AddBox((int)MathUtils.Floor(vector15.X), -num67, (int)MathUtils.Floor(vector15.Y), num72, num67, num72, 18);
				}
				terrainBrush15.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_waterPocketBrushes.Add(terrainBrush15);
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
				bool circular2 = num73 % 4 >= 2;
				int num77 = (num73 % 4 == 1) ? (num74 * num74) : (2 * num74 * num74);
				for (int num78 = 0; num78 < num77; num78++)
				{
					Vector2 vector16 = random.UniformVector2(0f, (float)num74, circular2);
					float num79 = vector16.Length();
					int num80 = random.UniformInt(3, 4);
					int sizeY2 = 1 + (int)MathUtils.Lerp(MathUtils.Max((float)(num74 / 3), 2.5f) * num76, 0f, num79 / (float)num74) + random.UniformInt(0, 1);
					int num81 = 1 + (int)MathUtils.Lerp((float)num75, 0f, num79 / (float)num74) + random.UniformInt(0, 1);
					terrainBrush16.AddBox((int)MathUtils.Floor(vector16.X), 0, (int)MathUtils.Floor(vector16.Y), num80, sizeY2, num80, 0);
					terrainBrush16.AddBox((int)MathUtils.Floor(vector16.X), -num81, (int)MathUtils.Floor(vector16.Y), num80, num81, num80, 92);
				}
				terrainBrush16.Compile();
				TerrainContentGeneratorVersion2_1_inactive.m_magmaPocketBrushes.Add(terrainBrush16);
			}
			for (int num82 = 0; num82 < 7; num82++)
			{
				TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType.Add(new List<TerrainBrush>());
				for (int num83 = 0; num83 < 3; num83++)
				{
					TerrainBrush terrainBrush17 = new TerrainBrush();
					int num84 = 6 + 4 * num82;
					int max = 3 + num82 / 3;
					int max2 = 9 + num82;
					for (int num85 = 0; num85 < num84; num85++)
					{
						int num86 = random.UniformInt(2, max);
						int num87 = random.UniformInt(8, max2) - 2 * num86;
						Vector3 v15 = 0.5f * new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(0f, 1f), random.UniformFloat(-1f, 1f));
						Vector3 vector17 = Vector3.Zero;
						for (int num88 = 0; num88 < num87; num88++)
						{
							terrainBrush17.AddBox((int)MathUtils.Floor(vector17.X) - num86 / 2, (int)MathUtils.Floor(vector17.Y) - num86 / 2, (int)MathUtils.Floor(vector17.Z) - num86 / 2, num86, num86, num86, 0);
							vector17 += v15;
						}
					}
					terrainBrush17.Compile();
					TerrainContentGeneratorVersion2_1_inactive.m_caveBrushesByType[num82].Add(terrainBrush17);
				}
			}
		}

		// Token: 0x04000FD1 RID: 4049
		public static List<TerrainBrush> m_coalBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD2 RID: 4050
		public static List<TerrainBrush> m_ironBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD3 RID: 4051
		public static List<TerrainBrush> m_copperBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD4 RID: 4052
		public static List<TerrainBrush> m_saltpeterBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD5 RID: 4053
		public static List<TerrainBrush> m_sulphurBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD6 RID: 4054
		public static List<TerrainBrush> m_diamondBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD7 RID: 4055
		public static List<TerrainBrush> m_germaniumBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD8 RID: 4056
		public static List<TerrainBrush> m_dirtPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FD9 RID: 4057
		public static List<TerrainBrush> m_gravelPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDA RID: 4058
		public static List<TerrainBrush> m_limestonePocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDB RID: 4059
		public static List<TerrainBrush> m_sandPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDC RID: 4060
		public static List<TerrainBrush> m_basaltPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDD RID: 4061
		public static List<TerrainBrush> m_granitePocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDE RID: 4062
		public static List<TerrainBrush> m_clayPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FDF RID: 4063
		public static List<TerrainBrush> m_waterPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FE0 RID: 4064
		public static List<TerrainBrush> m_magmaPocketBrushes = new List<TerrainBrush>();

		// Token: 0x04000FE1 RID: 4065
		public static List<List<TerrainBrush>> m_caveBrushesByType = new List<List<TerrainBrush>>();

		// Token: 0x04000FE2 RID: 4066
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000FE3 RID: 4067
		public SubsystemBottomSuckerBlockBehavior m_subsystemBottomSuckerBlockBehavior;

		// Token: 0x04000FE4 RID: 4068
		public WorldSettings m_worldSettings;

		// Token: 0x04000FE5 RID: 4069
		public int m_seed;

		// Token: 0x04000FE6 RID: 4070
		public Vector2? m_islandSize;

		// Token: 0x04000FE7 RID: 4071
		public Vector2 m_oceanCorner;

		// Token: 0x04000FE8 RID: 4072
		public Vector2 m_temperatureOffset;

		// Token: 0x04000FE9 RID: 4073
		public Vector2 m_humidityOffset;

		// Token: 0x04000FEA RID: 4074
		public Vector2 m_mountainsOffset;

		// Token: 0x04000FEB RID: 4075
		public Vector2 m_riversOffset;

		// Token: 0x04000FEC RID: 4076
		public bool TGNewBiomeNoise;

		// Token: 0x04000FED RID: 4077
		public float TGBiomeScaling;

		// Token: 0x04000FEE RID: 4078
		public float TGShoreFluctuations;

		// Token: 0x04000FEF RID: 4079
		public float TGShoreFluctuationsScaling;

		// Token: 0x04000FF0 RID: 4080
		public float TGOceanSlope;

		// Token: 0x04000FF1 RID: 4081
		public float TGOceanSlopeVariation;

		// Token: 0x04000FF2 RID: 4082
		public float TGIslandsFrequency;

		// Token: 0x04000FF3 RID: 4083
		public float TGDensityBias;

		// Token: 0x04000FF4 RID: 4084
		public float TGHeightBias;

		// Token: 0x04000FF5 RID: 4085
		public float TGHillsStrength;

		// Token: 0x04000FF6 RID: 4086
		public float TGMountainsStrength;

		// Token: 0x04000FF7 RID: 4087
		public float TGMountainsPeriod;

		// Token: 0x04000FF8 RID: 4088
		public float TGMountainsPercentage;

		// Token: 0x04000FF9 RID: 4089
		public float TGRiversStrength;

		// Token: 0x04000FFA RID: 4090
		public float TGTurbulenceStrength;

		// Token: 0x04000FFB RID: 4091
		public float TGTurbulenceTopOffset;

		// Token: 0x04000FFC RID: 4092
		public float TGTurbulencePower;

		// Token: 0x04000FFD RID: 4093
		public static float TGSurfaceMultiplier;

		// Token: 0x04000FFE RID: 4094
		public bool TGWater;

		// Token: 0x04000FFF RID: 4095
		public bool TGExtras;

		// Token: 0x04001000 RID: 4096
		public bool TGCavesAndPockets;

		// Token: 0x020004EC RID: 1260
		public class Random
		{
			// Token: 0x0600206F RID: 8303 RVA: 0x000E4487 File Offset: 0x000E2687
			public Random() : this(997 * TerrainContentGeneratorVersion2_1_inactive.Random.m_counter++)
			{
			}

			// Token: 0x06002070 RID: 8304 RVA: 0x000E44A2 File Offset: 0x000E26A2
			public Random(int seed)
			{
				this.Reset(seed);
			}

			// Token: 0x06002071 RID: 8305 RVA: 0x000E44B1 File Offset: 0x000E26B1
			public void Reset(int seed)
			{
				this.m_seed = (ulong)((long)seed ^ 25214903917L);
			}

			// Token: 0x06002072 RID: 8306 RVA: 0x000E44C5 File Offset: 0x000E26C5
			public int Sign()
			{
				if (this.Int() % 2 != 0)
				{
					return 1;
				}
				return -1;
			}

			// Token: 0x06002073 RID: 8307 RVA: 0x000E44D4 File Offset: 0x000E26D4
			public bool Bool()
			{
				return this.Int() % 2 == 0;
			}

			// Token: 0x06002074 RID: 8308 RVA: 0x000E44E1 File Offset: 0x000E26E1
			public bool Bool(float probability)
			{
				return (float)this.Int() / 2.147484E+09f < probability;
			}

			// Token: 0x06002075 RID: 8309 RVA: 0x000E44F3 File Offset: 0x000E26F3
			public int Int()
			{
				this.m_seed = (this.m_seed * 25214903917UL + 11UL & 281474976710655UL);
				return (int)(this.m_seed >> 17);
			}

			// Token: 0x06002076 RID: 8310 RVA: 0x000E4523 File Offset: 0x000E2723
			public int UniformInt(int min, int max)
			{
				return (int)((long)min + (long)this.Int() * (long)(max - min + 1) / 2147483647L);
			}

			// Token: 0x06002077 RID: 8311 RVA: 0x000E4540 File Offset: 0x000E2740
			public float UniformFloat(float min, float max)
			{
				float num = (float)this.Int() / 2.147484E+09f;
				return min + num * (max - min);
			}

			// Token: 0x06002078 RID: 8312 RVA: 0x000E4564 File Offset: 0x000E2764
			public float NormalFloat(float mean, float stddev)
			{
				float num = this.UniformFloat(0f, 1f);
				if ((double)num < 0.5)
				{
					float num2 = MathUtils.Sqrt(-2f * MathUtils.Log(num));
					float num3 = 0.32223243f + num2 * (1f + num2 * (0.3422421f + num2 * (0.020423122f + num2 * 4.536422E-05f)));
					float num4 = 0.09934846f + num2 * (0.58858156f + num2 * (0.5311035f + num2 * (0.10353775f + num2 * 0.00385607f)));
					return mean + stddev * (num3 / num4 - num2);
				}
				float num5 = MathUtils.Sqrt(-2f * MathUtils.Log(1f - num));
				float num6 = 0.32223243f + num5 * (1f + num5 * (0.3422421f + num5 * (0.020423122f + num5 * 4.536422E-05f)));
				float num7 = 0.09934846f + num5 * (0.58858156f + num5 * (0.5311035f + num5 * (0.10353775f + num5 * 0.00385607f)));
				return mean - stddev * (num6 / num7 - num5);
			}

			// Token: 0x06002079 RID: 8313 RVA: 0x000E4674 File Offset: 0x000E2874
			public Vector2 Vector2(float length, bool circular = false)
			{
				Vector2 v;
				float num;
				do
				{
					v = new Vector2(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f));
					num = v.LengthSquared();
				}
				while (circular && num > 1f);
				return v * (length / MathUtils.Sqrt(num));
			}

			// Token: 0x0600207A RID: 8314 RVA: 0x000E46CC File Offset: 0x000E28CC
			public Vector2 UniformVector2(float minLength, float maxLength, bool circular = false)
			{
				Vector2 v;
				float num;
				do
				{
					v = new Vector2(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f));
					num = v.LengthSquared();
				}
				while (circular && num > 1f);
				return v * (this.UniformFloat(minLength, maxLength) / MathUtils.Sqrt(num));
			}

			// Token: 0x0600207B RID: 8315 RVA: 0x000E4728 File Offset: 0x000E2928
			public Vector3 Vector3(float length, bool spherical = false)
			{
				Vector3 v;
				float num;
				do
				{
					v = new Vector3(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f));
					num = v.LengthSquared();
				}
				while (spherical && num > 1f);
				return v * (length / MathUtils.Sqrt(num));
			}

			// Token: 0x0600207C RID: 8316 RVA: 0x000E4790 File Offset: 0x000E2990
			public Vector3 UniformVector3(float minLength, float maxLength, bool spherical = false)
			{
				Vector3 v;
				float num;
				do
				{
					v = new Vector3(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f));
					num = v.LengthSquared();
				}
				while (spherical && num > 1f);
				return v * (this.UniformFloat(minLength, maxLength) / MathUtils.Sqrt(num));
			}

			// Token: 0x04001807 RID: 6151
			public static int m_counter = (int)Stopwatch.GetTimestamp();

			// Token: 0x04001808 RID: 6152
			public ulong m_seed;

			// Token: 0x04001809 RID: 6153
			public const ulong m_multiplier = 25214903917UL;

			// Token: 0x0400180A RID: 6154
			public const ulong m_addend = 11UL;

			// Token: 0x0400180B RID: 6155
			public const ulong m_mask = 281474976710655UL;

			// Token: 0x0400180C RID: 6156
			public static readonly TerrainContentGeneratorVersion2_1_inactive.Random GlobalRandom = new TerrainContentGeneratorVersion2_1_inactive.Random(0);
		}

		// Token: 0x020004ED RID: 1261
		public class OldRandom
		{
			// Token: 0x0600207E RID: 8318 RVA: 0x000E4814 File Offset: 0x000E2A14
			public OldRandom()
			{
				this.m_random = new TerrainContentGeneratorVersion2_1_inactive.OldRandom.InternalRandom(997 * TerrainContentGeneratorVersion2_1_inactive.OldRandom.m_seed++);
			}

			// Token: 0x0600207F RID: 8319 RVA: 0x000E483A File Offset: 0x000E2A3A
			public OldRandom(int seed)
			{
				this.m_random = new TerrainContentGeneratorVersion2_1_inactive.OldRandom.InternalRandom(seed);
			}

			// Token: 0x06002080 RID: 8320 RVA: 0x000E484E File Offset: 0x000E2A4E
			public int Sign()
			{
				if (this.m_random.Next() % 2 != 0)
				{
					return 1;
				}
				return -1;
			}

			// Token: 0x06002081 RID: 8321 RVA: 0x000E4862 File Offset: 0x000E2A62
			public bool Bool()
			{
				return this.m_random.Next() % 2 == 0;
			}

			// Token: 0x06002082 RID: 8322 RVA: 0x000E4874 File Offset: 0x000E2A74
			public int UniformInt(int min, int max)
			{
				return this.m_random.Next(min, max + 1);
			}

			// Token: 0x06002083 RID: 8323 RVA: 0x000E4885 File Offset: 0x000E2A85
			public float UniformFloat(float min, float max)
			{
				return (float)this.m_random.NextDouble() * (max - min) + min;
			}

			// Token: 0x06002084 RID: 8324 RVA: 0x000E489C File Offset: 0x000E2A9C
			public float NormalFloat(float mean, float stddev)
			{
				float num = this.UniformFloat(0f, 1f);
				if ((double)num < 0.5)
				{
					float num2 = MathUtils.Sqrt(-2f * MathUtils.Log(num));
					float num3 = 0.32223243f + num2 * (1f + num2 * (0.3422421f + num2 * (0.020423122f + num2 * 4.536422E-05f)));
					float num4 = 0.09934846f + num2 * (0.58858156f + num2 * (0.5311035f + num2 * (0.10353775f + num2 * 0.00385607f)));
					return mean + stddev * (num3 / num4 - num2);
				}
				float num5 = MathUtils.Sqrt(-2f * MathUtils.Log(1f - num));
				float num6 = 0.32223243f + num5 * (1f + num5 * (0.3422421f + num5 * (0.020423122f + num5 * 4.536422E-05f)));
				float num7 = 0.09934846f + num5 * (0.58858156f + num5 * (0.5311035f + num5 * (0.10353775f + num5 * 0.00385607f)));
				return mean - stddev * (num6 / num7 - num5);
			}

			// Token: 0x06002085 RID: 8325 RVA: 0x000E49AA File Offset: 0x000E2BAA
			public Vector2 Vector2(float length)
			{
				return Engine.Vector2.Normalize(new Vector2(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f))) * length;
			}

			// Token: 0x06002086 RID: 8326 RVA: 0x000E49DC File Offset: 0x000E2BDC
			public Vector2 UniformVector2(float minLength, float maxLength)
			{
				return Engine.Vector2.Normalize(new Vector2(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f))) * this.UniformFloat(minLength, maxLength);
			}

			// Token: 0x06002087 RID: 8327 RVA: 0x000E4A18 File Offset: 0x000E2C18
			public Vector3 Vector3(float length)
			{
				return Engine.Vector3.Normalize(new Vector3(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f))) * length;
			}

			// Token: 0x06002088 RID: 8328 RVA: 0x000E4A68 File Offset: 0x000E2C68
			public Vector3 UniformVector3(float minLength, float maxLength)
			{
				return Engine.Vector3.Normalize(new Vector3(this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f), this.UniformFloat(-1f, 1f))) * this.UniformFloat(minLength, maxLength);
			}

			// Token: 0x0400180D RID: 6157
			public TerrainContentGeneratorVersion2_1_inactive.OldRandom.InternalRandom m_random;

			// Token: 0x0400180E RID: 6158
			public static int m_seed = (int)Stopwatch.GetTimestamp();

			// Token: 0x0400180F RID: 6159
			public static readonly TerrainContentGeneratorVersion2_1_inactive.OldRandom GlobalRandom = new TerrainContentGeneratorVersion2_1_inactive.OldRandom(0);

			// Token: 0x0200053F RID: 1343
			public class InternalRandom
			{
				// Token: 0x06002172 RID: 8562 RVA: 0x000E72CC File Offset: 0x000E54CC
				public InternalRandom(int seed)
				{
					this.m_seedArray = new int[56];
					int num = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed);
					int num2 = 161803398 - num;
					this.m_seedArray[55] = num2;
					int num3 = 1;
					for (int i = 1; i < 55; i++)
					{
						int num4 = 21 * i % 55;
						this.m_seedArray[num4] = num3;
						num3 = num2 - num3;
						if (num3 < 0)
						{
							num3 += int.MaxValue;
						}
						num2 = this.m_seedArray[num4];
					}
					for (int j = 1; j < 5; j++)
					{
						for (int k = 1; k < 56; k++)
						{
							this.m_seedArray[k] -= this.m_seedArray[1 + (k + 30) % 55];
							if (this.m_seedArray[k] < 0)
							{
								this.m_seedArray[k] += int.MaxValue;
							}
						}
					}
					this.m_inext = 0;
					this.m_inextp = 21;
				}

				// Token: 0x06002173 RID: 8563 RVA: 0x000E73C4 File Offset: 0x000E55C4
				public double GetSampleForLargeRange()
				{
					int num = this.InternalSample();
					if (this.InternalSample() % 2 == 0)
					{
						num = -num;
					}
					return ((double)num + 2147483646.0) / 4294967293.0;
				}

				// Token: 0x06002174 RID: 8564 RVA: 0x000E73FC File Offset: 0x000E55FC
				public int InternalSample()
				{
					int num = this.m_inext;
					int num2 = this.m_inextp;
					if (++num >= 56)
					{
						num = 1;
					}
					if (++num2 >= 56)
					{
						num2 = 1;
					}
					int num3 = this.m_seedArray[num] - this.m_seedArray[num2];
					if (num3 == 2147483647)
					{
						num3--;
					}
					if (num3 < 0)
					{
						num3 += int.MaxValue;
					}
					this.m_seedArray[num] = num3;
					this.m_inext = num;
					this.m_inextp = num2;
					return num3;
				}

				// Token: 0x06002175 RID: 8565 RVA: 0x000E746F File Offset: 0x000E566F
				public int Next()
				{
					return this.InternalSample();
				}

				// Token: 0x06002176 RID: 8566 RVA: 0x000E7477 File Offset: 0x000E5677
				public int Next(int maxValue)
				{
					if (maxValue < 0)
					{
						throw new ArgumentOutOfRangeException("maxValue");
					}
					return (int)(this.Sample() * (double)maxValue);
				}

				// Token: 0x06002177 RID: 8567 RVA: 0x000E7494 File Offset: 0x000E5694
				public int Next(int minValue, int maxValue)
				{
					if (minValue > maxValue)
					{
						throw new ArgumentOutOfRangeException("minValue");
					}
					long num = (long)(maxValue - minValue);
					if (num <= 2147483647L)
					{
						return (int)(this.Sample() * (double)num) + minValue;
					}
					return (int)((long)(this.GetSampleForLargeRange() * (double)num)) + minValue;
				}

				// Token: 0x06002178 RID: 8568 RVA: 0x000E74D8 File Offset: 0x000E56D8
				public void NextBytes(byte[] buffer)
				{
					if (buffer == null)
					{
						throw new ArgumentNullException("buffer");
					}
					for (int i = 0; i < buffer.Length; i++)
					{
						buffer[i] = (byte)(this.InternalSample() % 256);
					}
				}

				// Token: 0x06002179 RID: 8569 RVA: 0x000E7511 File Offset: 0x000E5711
				public double NextDouble()
				{
					return this.Sample();
				}

				// Token: 0x0600217A RID: 8570 RVA: 0x000E7519 File Offset: 0x000E5719
				public double Sample()
				{
					return (double)this.InternalSample() * 4.656612875245797E-10;
				}

				// Token: 0x04001910 RID: 6416
				public int m_inext;

				// Token: 0x04001911 RID: 6417
				public int m_inextp;

				// Token: 0x04001912 RID: 6418
				public int[] m_seedArray;
			}
		}

		// Token: 0x020004EE RID: 1262
		public class CavePoint
		{
			// Token: 0x04001810 RID: 6160
			public Vector3 Position;

			// Token: 0x04001811 RID: 6161
			public Vector3 Direction;

			// Token: 0x04001812 RID: 6162
			public int BrushType;

			// Token: 0x04001813 RID: 6163
			public int Length;

			// Token: 0x04001814 RID: 6164
			public int StepsTaken;
		}

		// Token: 0x020004EF RID: 1263
		public class Grid2d
		{
			// Token: 0x17000560 RID: 1376
			// (get) Token: 0x0600208B RID: 8331 RVA: 0x000E4ADC File Offset: 0x000E2CDC
			public int SizeX
			{
				get
				{
					return this.m_sizeX;
				}
			}

			// Token: 0x17000561 RID: 1377
			// (get) Token: 0x0600208C RID: 8332 RVA: 0x000E4AE4 File Offset: 0x000E2CE4
			public int SizeY
			{
				get
				{
					return this.m_sizeY;
				}
			}

			// Token: 0x0600208D RID: 8333 RVA: 0x000E4AEC File Offset: 0x000E2CEC
			public Grid2d(int sizeX, int sizeY)
			{
				this.m_sizeX = sizeX;
				this.m_sizeY = sizeY;
				this.m_data = new float[this.m_sizeX * this.m_sizeY];
			}

			// Token: 0x0600208E RID: 8334 RVA: 0x000E4B1A File Offset: 0x000E2D1A
			public float Get(int x, int y)
			{
				return this.m_data[x + y * this.m_sizeX];
			}

			// Token: 0x0600208F RID: 8335 RVA: 0x000E4B2D File Offset: 0x000E2D2D
			public void Set(int x, int y, float value)
			{
				this.m_data[x + y * this.m_sizeX] = value;
			}

			// Token: 0x06002090 RID: 8336 RVA: 0x000E4B44 File Offset: 0x000E2D44
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

			// Token: 0x04001815 RID: 6165
			public int m_sizeX;

			// Token: 0x04001816 RID: 6166
			public int m_sizeY;

			// Token: 0x04001817 RID: 6167
			public float[] m_data;
		}

		// Token: 0x020004F0 RID: 1264
		public class Grid3d
		{
			// Token: 0x17000562 RID: 1378
			// (get) Token: 0x06002091 RID: 8337 RVA: 0x000E4BE6 File Offset: 0x000E2DE6
			public int SizeX
			{
				get
				{
					return this.m_sizeX;
				}
			}

			// Token: 0x17000563 RID: 1379
			// (get) Token: 0x06002092 RID: 8338 RVA: 0x000E4BEE File Offset: 0x000E2DEE
			public int SizeY
			{
				get
				{
					return this.m_sizeY;
				}
			}

			// Token: 0x17000564 RID: 1380
			// (get) Token: 0x06002093 RID: 8339 RVA: 0x000E4BF6 File Offset: 0x000E2DF6
			public int SizeZ
			{
				get
				{
					return this.m_sizeZ;
				}
			}

			// Token: 0x06002094 RID: 8340 RVA: 0x000E4C00 File Offset: 0x000E2E00
			public Grid3d(int sizeX, int sizeY, int sizeZ)
			{
				this.m_sizeX = sizeX;
				this.m_sizeY = sizeY;
				this.m_sizeZ = sizeZ;
				this.m_sizeXY = this.m_sizeX * this.m_sizeY;
				this.m_data = new float[this.m_sizeX * this.m_sizeY * this.m_sizeZ];
			}

			// Token: 0x06002095 RID: 8341 RVA: 0x000E4C5C File Offset: 0x000E2E5C
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

			// Token: 0x06002096 RID: 8342 RVA: 0x000E4D15 File Offset: 0x000E2F15
			public float Get(int x, int y, int z)
			{
				return this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY];
			}

			// Token: 0x06002097 RID: 8343 RVA: 0x000E4D31 File Offset: 0x000E2F31
			public void Set(int x, int y, int z, float value)
			{
				this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY] = value;
			}

			// Token: 0x06002098 RID: 8344 RVA: 0x000E4D50 File Offset: 0x000E2F50
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

			// Token: 0x04001818 RID: 6168
			public int m_sizeX;

			// Token: 0x04001819 RID: 6169
			public int m_sizeY;

			// Token: 0x0400181A RID: 6170
			public int m_sizeZ;

			// Token: 0x0400181B RID: 6171
			public int m_sizeXY;

			// Token: 0x0400181C RID: 6172
			public float[] m_data;
		}
	}
}
