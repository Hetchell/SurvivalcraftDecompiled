
using System;
using Game;
using Engine;

namespace Survivalcraft.Game
{
    internal class ModifierHolder
    {
        public static bool allowUnrestrictedTravel = true;
        public static float steppedTravel = 5.0f;
        //instances
        private readonly SubsystemTerrain terrainInput;
        private readonly WorldSettings worldSettings;
        private readonly TerrainChunkGeneratorProviderActive activeChunkProvider;
        private readonly float[] holder;
        private int x_prev;
        private int y_prev;

        internal ModifierHolder(SubsystemTerrain terrainInput, TerrainChunkGeneratorProviderActive activeChunkProvider, float num)
        {
            this.terrainInput = terrainInput;
            this.activeChunkProvider = activeChunkProvider;
            this.worldSettings = activeChunkProvider.m_worldSettings;
            this.holder = new float[]
            {
                0.04f * (float)Math.Pow(2, 0), //TGMinTurbulence
                0.84f * (float)Math.Pow(2, 0), //TGTurbulenceZero
                55f, //TGTurbulenceStrength
                0.03f * (float)Math.Pow(2, 0), //TGTurbulenceFreq
                8f, //TGTurbulenceOctaves
                0.5f, //TGTurbulencePersistence
                55f, //TGDensityBias
                MathUtils.Clamp(2f * num, 0f, 150f), //TGShoreFluctuations
                MathUtils.Clamp(0.04f * num, 0.5f, 3f), //TGShoreFluctuationsScaling
                0.0006f * (float)Math.Pow(Math.PI, 0), //TGMountainRangeFreq
                0.006f *(float)Math.Pow(2, 0), //this.TGOceanSlope = 
                0.004f * (float)Math.Pow(2, 0), //TGOceanSlopeVariation
                0.01f * (float)Math.Pow(2, 0), //this.TGIslandsFrequency = 
                0.32f, //this.TGHillsPercentage = 
                0.15f, //this.TGMountainsPercentage = 
                0.014f * (float)Math.Pow(2, 0), //this.TGHillsFrequency = 
                1f, //this.TGHillsOctaves = 
                0.5f, //this.TGHillsPersistence = 
                1f, //this.TGHeightBias = 
                32f, //this.TGHillsStrength = 
                220f, //this.TGMountainsStrength = 
                1f, //this.TGRiversStrength = 
            activeChunkProvider.OceanLevel //OceanLevel
        };
        }

        public void GenerateTerrain(TerrainChunk chunkIn, bool type)
        {
            if (type)
            {
                this.GenerateTerrain(chunkIn, 0, 0, 16, 8);
                //this.GenerateTerrain(chunk, chunk.Origin.X, chunk.Origin.Y, 3, 3);
                //this.GenerateTerrain(chunk, 14, 27, 16, 5);
            }
            else
            {
                this.GenerateTerrain(chunkIn, 0, 8, 16, 16);
            }
        }

        private void GenerateTerrain(TerrainChunk chunk, int x1, int z1, int x2, int z2)
        {
            int num = x2 - x1;
            int num2 = z2 - z1;
            Terrain terrain = this.terrainInput.Terrain;
            int num3 = chunk.Origin.X + x1;
            int num4 = chunk.Origin.Y + z1;
            TerrainChunkGeneratorProviderActive.Grid2d grid2d = new TerrainChunkGeneratorProviderActive.Grid2d(num, num2);
            TerrainChunkGeneratorProviderActive.Grid2d grid2d2 = new TerrainChunkGeneratorProviderActive.Grid2d(num, num2);
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    float f0;
                    float x0 = (float)(j + num3);
                    float z0 = (float)(i + num4);
                    if (this.activeChunkProvider.m_islandSize != null)
                    {
                        float f1 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                        float f2 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                        float f3 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0 + 1000f, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.X;
                        float f4 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0 + 1000f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.Y;
                        f0 = MathUtils.Min(x0 - f1, z0 - f2, f3 - x0, f4 - z0);
                    }
                    else
                    {
                        float f5 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                        float f6 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                        f0 = MathUtils.Min(x0 - f5, z0 - f6);
                    }
                    //f0 = this.activeChunkProvider.CalculateOceanShoreDistance(x0, z0);
                    grid2d.Set(j, i, f0);
                    f0 = SimplexNoise.OctavedNoise(
                        x0 + this.activeChunkProvider.m_mountainsOffset.X,
                        z0 + this.activeChunkProvider.m_mountainsOffset.Y,
                        holder[9] / this.activeChunkProvider.TGBiomeScaling,
                        3,
                        1.91f,
                        0.75f,
                        true
                    );
                    grid2d2.Set(j, i, f0);
                }
            }
            TerrainChunkGeneratorProviderActive.Grid3d grid3d = new TerrainChunkGeneratorProviderActive.Grid3d(num / 4 + 1, 33, num2 / 4 + 1);
            for (int k = 0; k < grid3d.SizeX; k++)
            {
                for (int l = 0; l < grid3d.SizeZ; l++)
                {
                    int x = k * 4 + num3;
                    int y = l * 4 + num4;
                    if (x / 100 != this.x_prev / 100 || y / 100 != this.y_prev / 100)
                    {
                        //Debug.WriteLine("x -> chunk coordinate shiftscale: " + x, "TerrainChunkProviderActive");
                        //Debug.WriteLine("y -> chunk coordinate shiftscale: " + y, "TerrainChunkProviderActive");
                    }
                    this.y_prev = y;
                    this.x_prev = x;
                    float f_0 = holder[10] + holder[11] * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, y + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
                    float f_1 = this.CalculateOceanShoreDistance(x, y);
                    float f_2 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(f_1));
                    float f_3 = MathUtils.Saturate(MathUtils.Sin(holder[12] * f_1));
                    float f_4 = MathUtils.Saturate(MathUtils.Saturate((0f - f_0) * f_1) - 0.85f * f_3);
                    float f_5 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - f_1 - 10f)) - f_3);
                    float v = this.CalculateMountainRangeFactor(x, y);
                    float f = (1f - f_2) * SimplexNoise.OctavedNoise(x, y, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
                    float f2 = (1f - f_2) * SimplexNoise.OctavedNoise(x, y, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
                    float f_6 = (1f - f_5) * (1f - f_2) * this.Squish(v, 1f - holder[13], 1f - holder[14]);
                    float f_7 = (1f - f_5) * this.Squish(v, 1f - holder[14], 1f);
                    float f_8 = 1f * SimplexNoise.OctavedNoise(x, y, holder[15], (int)holder[16], 1.93f, holder[17], false);
                    float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f);
                    float f_9 = 1.5f * SimplexNoise.OctavedNoise(x, y, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
                    float f_10 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * f_7 + 0.5f * f_6 + MathUtils.Saturate(1f - f_1 / 30f)));
                    float f_19 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(f_7 + 0.5f * f_6));
                    float f_11 = MathUtils.Saturate(1.5f - f_10 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, y + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
                    float f_12 = -50f * f_4 + holder[18];
                    float f_13 = MathUtils.Lerp(0f, 8f, f);
                    float f_14 = MathUtils.Lerp(0f, -6f, f2);
                    float f_15 = holder[19] * f_6 * f_8;
                    float f_16 = holder[20] * f_7 * f_9;
                    float f3 = holder[21] * f_11;
                    float f_17 = f_12 + f_13 + f_14 + f_16 + f_15;
                    float f_18 = MathUtils.Min(MathUtils.Lerp(f_17, f_19, f3), f_17);
                    float num7 = MathUtils.Clamp(64f + f_18, 10f, 251f);
                    //float num7 = this.activeChunkProvider.CalculateHeight(x, y);
                    v = this.activeChunkProvider.CalculateMountainRangeFactor((float)x, (float)y);
                    float num8 = MathUtils.Lerp(holder[0], 1f, TerrainChunkGeneratorProviderActive.Squish(v, holder[1], 1f));
                    for (int m = 0; m < grid3d.SizeY; m++)
                    {
                        int num9 = m * 8;
                        float num10 = holder[2] * num8 * MathUtils.Saturate(num7 - (float)num9) * (2f * SimplexNoise.OctavedNoise((float)x, (float)num9, (float)y, holder[3], (int)holder[4], 4f, holder[5], false) - 1f);
                        float num11 = (float)num9 + num10;
                        float num12 = num7 - num11;
                        //num12 *= 318.7f;
                        num12 += MathUtils.Max(4f * (holder[6] - (float)num9), 0f);
                        grid3d.Set(k, m, l, num12);
                    }
                }
            }
            int oceanLevel = (int)holder[holder.Length - 1] * 1 + this.worldSettings.SeaLevelOffset * 0;
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

        private float CalculateMountainRangeFactor(float x, float z)
        {
            return SimplexNoise.OctavedNoise(
                x + this.activeChunkProvider.m_mountainsOffset.X, 
                z + this.activeChunkProvider.m_mountainsOffset.Y, 
                holder[9] / this.activeChunkProvider.TGBiomeScaling, 
                3, 
                1.91f, 
                0.75f, 
                true);
        }

        private float CalculateOceanShoreDistance(float x0, float z0)
        {
            float f0 = 0;
            if (this.activeChunkProvider.m_islandSize != null)
            {
                float f1 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f2 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f3 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0 + 1000f, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.X;
                float f4 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0 + 1000f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.Y;
                f0 += MathUtils.Min(x0 - f1, z0 - f2, f3 - x0, f4 - z0);
            }
            else
            {
                float f5 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * SimplexNoise.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f6 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * SimplexNoise.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                f0 += MathUtils.Min(x0 - f5, z0 - f6);
            }
            return f0;
            //if (this.activeChunkProvider.m_islandSize != null)
            //{
            //    float num = this.activeChunkProvider.CalculateOceanShoreX(z0);
            //    float num2 = this.activeChunkProvider.CalculateOceanShoreZ(x0);
            //    float num3 = this.activeChunkProvider.CalculateOceanShoreX(z0 + 1000f) + this.activeChunkProvider.m_islandSize.Value.X;
            //    float num4 = this.activeChunkProvider.CalculateOceanShoreZ(x0 + 1000f) + this.activeChunkProvider.m_islandSize.Value.Y;
            //    return MathUtils.Min(x0 - num, z0 - num2, num3 - x0, num4 - z0);
            //}
            //float num5 = this.activeChunkProvider.CalculateOceanShoreX(z0);
            //float num6 = this.activeChunkProvider.CalculateOceanShoreZ(x0);
            //return MathUtils.Min(x0 - num5, z0 - num6);
        }

        public float Squish(float v, float r, float t)
        {
            float x = (v - r) / (t - r);
            if (!(x < 0f))
            {
                if (!(x > 1f))
                {
                    return x;
                }

                return 1f;
            }

            return 0f;
        }
    }
}
