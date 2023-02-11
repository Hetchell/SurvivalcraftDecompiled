
using System;
using Game;
using Engine;
using System.Diagnostics;
using Survivalcraft.Game.NoiseModifier;
using Random = Game.Random;
using static Game.TerrainContentGeneratorVersion2_1_inactive;
using Survivalcraft.Game.ModificationHolder;
using static Game.TerrainChunkGeneratorProviderActive;

namespace Survivalcraft.Game
{
    internal class ModifierHolder
    {
        public static bool allowUnrestrictedTravel = ModificationsHolder.allowForUnrestrictedTravel;
        public static float steppedTravel = ModificationsHolder.steppedLevelTravel;
        public static bool allowMethodOverride = false;
        //instances
        private readonly SubsystemTerrain terrainInput;
        private readonly WorldSettings worldSettings;
        private readonly TerrainChunkGeneratorProviderActive activeChunkProvider;
        private readonly float[] holder;
        private double[] r;
        private double[] ar;
        private double[] br;
        private NoiseMain noiseMain;
        private NoiseMain noiseMin;
        private NoiseMain noiseMax;
        private int x_prev;
        private int y_prev;
        private Random rand;
        private readonly double[] s = new double[]
        {
            -101.1819239019000,
            -19.29182010183082,
            1.18192929109293,
            188.18182300,
            55.38192111,
            64.18192929192,
            69.19109102001,
            87.18181929281,
            -0.0007726161839119
            -0.0181832727173,
            15.1828332111111,
            1.1918918297129712,
            -133.131937918291212,
            -187.281281921093813,
            -54.19138128182192,
            31.39128913719814,
            16.1929139813081093,
            17.19029201933,
            8.0009999999
        };

        internal ModifierHolder(SubsystemTerrain terrainInput, TerrainChunkGeneratorProviderActive activeChunkProvider, float num, Random rand)
        {
            this.terrainInput = terrainInput;
            this.activeChunkProvider = activeChunkProvider;
            this.worldSettings = activeChunkProvider.m_worldSettings;
            this.holder = new float[]
            {
                0.04f * (float)Math.Pow(2, 0), //TGMinTurbulence0
                0.84f * (float)Math.Pow(2, 0), //TGTurbulenceZero1
                55f, //TGTurbulenceStrength2
                0.03f * (float)Math.Pow(2, 0), //TGTurbulenceFreq3
                16f, //TGTurbulenceOctaves4
                0.5f * 1, //TGTurbulencePersistence5
                55f, //TGDensityBias6
                MathUtils.Clamp(2f * num, 0f, 150f), //TGShoreFluctuations7
                MathUtils.Clamp(0.04f * num, 0.5f, 3f), //TGShoreFluctuationsScaling8
                0.0006f * (float)Math.Pow(Math.PI, 0), //TGMountainRangeFreq9
                0.006f *(float)Math.Pow(2, 0), //this.TGOceanSlope = 10
                0.004f * (float)Math.Pow(2, 0), //TGOceanSlopeVariation11
                0.01f * (float)Math.Pow(2, 0), //this.TGIslandsFrequency = 12
                0.32f - 0.31f * 0, //this.TGHillsPercentage = 13
                0.15f - 0.14f * 0, //this.TGMountainsPercentage = 14 --> This one, with lower value, squeeze mountain width
                0.014f * (float)Math.Pow(2, 0), //this.TGHillsFrequency = 15
                1f, //this.TGHillsOctaves = 16
                0.5f, //this.TGHillsPersistence = 17
                1f, //this.TGHeightBias = 18
                32f, //this.TGHillsStrength = 19
                220f, //this.TGMountainsStrength = 20
                1f, //this.TGRiversStrength = 21
            activeChunkProvider.OceanLevel //OceanLevel22
        };
            this.rand = rand;
            this.noiseMain = new NoiseMain(rand, (int)holder[4]);
            this.noiseMin = new NoiseMain(rand, 8);
            this.noiseMax = new NoiseMain(rand, 8);
        }

        public void GenerateTerrain(TerrainChunk chunkIn, bool type)
        {
            if (type)
            {
                this.GenerateTerrainMod(chunkIn, 0, 0, 16, 8);
                //this.GenerateTerrain(chunk, chunk.Origin.X, chunk.Origin.Y, 3, 3);
                //this.GenerateTerrain(chunk, 14, 27, 16, 5);
            }
            else
            {
                this.GenerateTerrainMod(chunkIn, 0, 8, 16, 16);
            }
        }

        //original
        public void GenerateTerrainOr(TerrainChunk chunk, int x1, int z1, int x2, int z2)
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
                    grid2d.Set(j, i, this.activeChunkProvider.CalculateOceanShoreDistance((float)(j + num3), (float)(i + num4)));
                    grid2d2.Set(j, i, this.activeChunkProvider.CalculateMountainRangeFactor((float)(j + num3), (float)(i + num4)));
                }
            }
            TerrainChunkGeneratorProviderActive.Grid3d grid3d = new TerrainChunkGeneratorProviderActive.Grid3d(num / 4 + 1, 33, num2 / 4 + 1);
            for (int k = 0; k < grid3d.SizeX; k++)
            {
                for (int l = 0; l < grid3d.SizeZ; l++)
                {
                    int num5 = k * 4 + num3;
                    int num6 = l * 4 + num4;
                    float num7 = this.activeChunkProvider.CalculateHeight((float)num5, (float)num6);
                    float v = this.activeChunkProvider.CalculateMountainRangeFactor((float)num5, (float)num6);
                    float num8 = MathUtils.Lerp(this.activeChunkProvider.TGMinTurbulence, 1f, TerrainChunkGeneratorProviderActive.Squish(v, this.activeChunkProvider.TGTurbulenceZero, 1f));
                    for (int m = 0; m < grid3d.SizeY; m++)
                    {
                        int num9 = m * 8;
                        float num10 = this.activeChunkProvider.TGTurbulenceStrength * num8 * MathUtils.Saturate(num7 - (float)num9) * (2f * SimplexNoise.OctavedNoise((float)num5, (float)num9, (float)num6, this.activeChunkProvider.TGTurbulenceFreq, this.activeChunkProvider.TGTurbulenceOctaves, 4f, this.activeChunkProvider.TGTurbulencePersistence, false) - 1f);
                        float num11 = (float)num9 + num10;
                        float num12 = num7 - num11;
                        num12 += MathUtils.Max(4f * (this.activeChunkProvider.TGDensityBias - (float)num9), 0f);
                        grid3d.Set(k, m, l, num12);
                    }
                }
            }
            int oceanLevel = this.activeChunkProvider.OceanLevel;
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
                        float num23 = (num16 - num15) / 4f; //(num16 - num15) / 4f
                        float num24 = (num18 - num17) / 1f; //(num18 - num17) / 4f
                        float num25 = (num20 - num19) / 4f; //(num20 - num19) / 4f
                        float num26 = (num22 - num21) / 1f; //(num22 - num21) / 4f
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

        private void GenerateTerrainMod(TerrainChunk chunk, int x1, int z1, int x2, int z2)
        {
            int I = 0;
            float a0 = 684.412f;
            float a1 = a0;
            float a2 = a0 * 2.0f;
            int i0 = x2 - x1;
            int i1 = z2 - z1;
            Terrain terrain = this.terrainInput.Terrain;
            int k0 = chunk.Origin.X + x1;
            int k1 = chunk.Origin.Y + z1;
            Grid2d grid2d = new Grid2d(i0, 2 * i1);
            Grid2d grid2d2 = new Grid2d(i0, i1);
            for (int i = 0; i < i1; i++)
            {
                for (int j = 0; j < i0; j++)
                {
                    //float f0;
                    //float x0 = (float)(j + k0);
                    //float z0 = (float)(i + k1);
                    //if (this.activeChunkProvider.m_islandSize != null)
                    //{
                    //    float f1 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                    //    float f2 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                    //    float f3 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0 + 1000f, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.X;
                    //    float f4 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0 + 1000f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.Y;
                    //    f0 = MathUtils.Min(x0 - f1, z0 - f2, f3 - x0, f4 - z0);
                    //}
                    //else
                    //{
                    //    float f5 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                    //    float f6 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                    //    f0 = MathUtils.Min(x0 - f5, z0 - f6);
                    //}
                    //f0 = allowMethodOverride ? this.CalculateOceanShoreDistance(x0, z0) : f0 * 1;
                    //grid2d.Set(j, i, f0);
                    //f0 = !allowMethodOverride ? this.noiseMain.OctavedNoise(
                    //    x0 + this.activeChunkProvider.m_mountainsOffset.X,
                    //    z0 + this.activeChunkProvider.m_mountainsOffset.Y,
                    //    holder[9] / this.activeChunkProvider.TGBiomeScaling,
                    //    3,
                    //    1.91f,
                    //    0.75f,
                    //    true
                    //) : this.CalculateMountainRangeFactor(x0, z0);
                    //grid2d2.Set(j, i, f0);
                    grid2d.Set(j, i, this.activeChunkProvider.CalculateOceanShoreDistance((float)(j + i1), (float)(i + i0)));
                    grid2d2.Set(j, i, this.activeChunkProvider.CalculateMountainRangeFactor((float)(j + i1), (float)(i + i0)));
                }
            }
            Grid3d grid3d = new Grid3d(i0 / 4 + 1, 33, i1 / 4 + 1);
            //this.r = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.r, k0, 0, k1, i0 / 4 + 1, 33, i1 / 4 + 1, a0  * Math.Pow(2, 10), a1 , a2);
            //this.ar = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.ar, k0, 0, k1, i0 / 4 + 1, 33, i1 / 4 + 1, a0 * Math.Pow(2, 10), a1, a2);
            //this.br = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.br, k0, 0, k1, i0 / 4 + 1, 33, i1 / 4 + 1, a0 * Math.Pow(2, 10), a1, a2);
            this.r = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.r, k0, 0, k1, 5, 33, 5, a0 * Math.Pow(2, 15), a1, a2);
            this.ar = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.ar, k0, 0, k1, 5, 33, 5, a0 * Math.Pow(2, 15), a1, a2);
            this.br = this.noiseMain.UseImprovedNoiseGenerateNoiseOctaves(this.br, k0, 0, k1, 5, 33, 5, a0 * Math.Pow(2, 15), a1, a2);
            for (int k = 0; k < grid3d.SizeX; k++)
            {
                for (int l = 0; l < grid3d.SizeZ; l++)
                {
                    int x = k * 4 + k0;
                    int y = l * 4 + k1;
                    if (x / 100 != this.x_prev / 100 || y / 100 != this.y_prev / 100)
                    {
                        //Debug.WriteLine("x -> chunk coordinate shiftscale: " + x, "TerrainChunkProviderActive");
                        //Debug.WriteLine("y -> chunk coordinate shiftscale: " + y, "TerrainChunkProviderActive");
                    }
                    this.y_prev = y;
                    this.x_prev = x;
                    ////float num = this.activeChunkProvider.TGOceanSlope + this.activeChunkProvider.TGOceanSlopeVariation * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, z + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
                    //float f_0 = holder[10] + holder[11] * MathUtils.PowSign(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, y + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
                    ////float num2 = this.CalculateOceanShoreDistance(x, z);
                    //float f_1 = this.CalculateOceanShoreDistance(x, y);
                    ////float num3 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(num2));
                    //float f_2 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(f_1));
                    ////float num4 = MathUtils.Saturate(MathUtils.Sin(this.activeChunkProvider.TGIslandsFrequency * num2));
                    //float f_3 = MathUtils.Saturate(MathUtils.Sin(holder[12] * f_1));
                    ////float num5 = MathUtils.Saturate(MathUtils.Saturate((0f - num) * num2) - 0.85f * num4);
                    //float f_4 = MathUtils.Saturate(MathUtils.Saturate((0f - f_0) * f_1) - 0.85f * f_3);
                    ////float num6 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - num2 - 10f)) - num4);
                    //float f_5 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - f_1 - 10f)) - f_3);
                    ////float v = this.CalculateMountainRangeFactor(x, z);
                    //float f_6 = this.CalculateMountainRangeFactor(x, y);
                    ////float f = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
                    //float f_7 = (1f - f_2) * this.noiseMain.OctavedNoise(x, y, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
                    ////float f2 = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
                    //float f_8 = (1f - f_2) * this.noiseMain.OctavedNoise(x, y, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
                    ////float num7 = (1f - num6) * (1f - num3) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.activeChunkProvider.TGHillsPercentage, 1f - this.activeChunkProvider.TGMountainsPercentage);
                    //float f_9 = (1f - f_5) * (1f - f_2) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[13], 1f - holder[14]);
                    ////float num8 = (1f - num6) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.activeChunkProvider.TGMountainsPercentage, 1f);
                    //float f_10 = (1f - f_5) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[14], 1f);
                    ////float num9 = 1f * SimplexNoise.OctavedNoise(x, z, this.activeChunkProvider.TGHillsFrequency, this.activeChunkProvider.TGHillsOctaves, 1.93f, this.activeChunkProvider.TGHillsPersistence, false);
                    //float f_11 = 1f * this.noiseMain.OctavedNoise(x, y, holder[15], (int)holder[16], 1.93f, holder[17], false);
                    ////float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f);
                    //float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f_7);
                    ////float num10 = 1.5f * SimplexNoise.OctavedNoise(x, z, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
                    //float f_12 = 1.5f * this.noiseMain.OctavedNoise(x, y, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
                    ////float num11 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * num8 + 0.5f * num7 + MathUtils.Saturate(1f - num2 / 30f)));
                    //float f_13 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * f_10 + 0.5f * f_9 + MathUtils.Saturate(1f - f_1 / 30f)));
                    ////float x2 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(num8 + 0.5f * num7));
                    //float f_14 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(f_10 + 0.5f * f_9));
                    ////float num12 = MathUtils.Saturate(1.5f - num11 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, z + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
                    //float f_15 = MathUtils.Saturate(1.5f - f_13 * MathUtils.Abs(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, y + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
                    ////float num13 = -50f * num5 + this.activeChunkProvider.TGHeightBias;
                    //float f_16 = -50f * f_4 + holder[18];
                    ////float num14 = MathUtils.Lerp(0f, 8f, f);
                    //float f_17 = MathUtils.Lerp(0f, 8f, f_7);
                    ////float num15 = MathUtils.Lerp(0f, -6f, f2);
                    //float f_18 = MathUtils.Lerp(0f, -6f, f_8);
                    ////float num16 = this.activeChunkProvider.TGHillsStrength * num7 * num9;
                    //float f_19 = holder[19] * f_9 * f_11;
                    ////float num17 = this.activeChunkProvider.TGMountainsStrength * num8 * num10;
                    //float f_20 = holder[20] * f_10 * f_12;
                    ////float f3 = this.activeChunkProvider.TGRiversStrength * num12;
                    //float f_21 = holder[21] * f_15;
                    ////float num18 = num13 + num14 + num15 + num17 + num16;
                    //float f_22 = f_16 + f_17 + f_18 + f_20 + f_19;
                    ////float num19 = MathUtils.Min(MathUtils.Lerp(num18, x2, f3), num18);
                    //float f_23 = MathUtils.Min(MathUtils.Lerp(f_22, f_14, f_21), f_22);
                    //float f0 = MathUtils.Clamp(64f + f_23, 10f, 251f);
                    //float v;
                    //f0 = allowMethodOverride ? this.CalculateHeight(x, y) : f0 * 1;
                    ////num7 = this.activeChunkProvider.CalculateHeight(x, y);
                    //v = this.CalculateMountainRangeFactor((float)x, (float)y);
                    float f0 = this.activeChunkProvider.CalculateHeight((float)x, (float)y) + 1;
                    float v = this.activeChunkProvider.CalculateMountainRangeFactor((float)x, (float)y);
                    //float f1 = MathUtils.Lerp(holder[0], 1f, TerrainChunkGeneratorProviderActive.Squish(v, holder[1], 1f));
                    float f1 = MathUtils.Lerp(
                        this.activeChunkProvider.TGMinTurbulence, 
                        1f, 
                        TerrainChunkGeneratorProviderActive.Squish(
                            v, 
                            this.activeChunkProvider.TGTurbulenceZero, 
                            1f
                            )
                        );
                    for (int m = 0; m < grid3d.SizeY; m++)
                    {
                        //int f2 = m * 8;
                        //double d0 = this.ar[I] / 512.0D;
                        //double d1 = this.br[I] / 512.0D;
                        //double d2 = (this.r[I] / 10.0D + 1.0D) / 2.0D;
                        //double d3;
                        //if (d2 < 0.0D)
                        //{
                        //   d3 = d0;
                        //}
                        //else
                        //{
                        //    d3 = d2 > 1.0D ? d1 : d0 + (d1 - d0) * d2;
                        //}
                        //float f3 = holder[2] * f1 * MathUtils.Saturate(f0 - (float)f2) * (2f * this.noiseMain.OctavedNoise((float)x, (float)f2, (float)y, holder[3], (int)holder[4], 4f, holder[5], false) - 1f);
                        ////f3 -= holder[2] * f1 * MathUtils.Saturate(f0 - (float)f2) * (2f * (float)r[I] - 1f);
                        ////f3 = (float)d3 * f1;
                        //float f4 = (float)f2 + f3;
                        //float f5 = f0 - f4;
                        ////f5 = (float)this.r[I] / 800000;
                        ////f5 *= 318.7f;
                        //f5 += MathUtils.Max(4f * (holder[6] - (float)f2), 0f);
                        ////f5 += f2;
                        ////f5 = MathUtils.Sin(x);
                        ////f5 = (float)d3;
                        ////f5 = f3 * 14.0f;
                        ////f5 = (float)MathUtils.Clamp((k0 + k1) * rand.Float(), -200f, 200f);
                        ////f5 += (float)this.s[I % this.s.Length];
                        ////f5 = rand.Float(-200.0f, 200.0f);
                        ////f5 += (m + 1.0f) / (m + 2.33333333333f) * (float)Math.Pow(-1, m);
                        //grid3d.Set(k, m, l, f5);
                        //I++;
                        int num9 = m * 8;
                        float num10 = this.activeChunkProvider.TGTurbulenceStrength * f1 * MathUtils.Saturate(
                            f0 - (float)num9
                            ) * (2f * SimplexNoise.OctavedNoise(
                                (float)x, 
                                (float)num9, 
                                (float)y, 
                                this.activeChunkProvider.TGTurbulenceFreq, 
                                this.activeChunkProvider.TGTurbulenceOctaves, 
                                4f, 
                                this.activeChunkProvider.TGTurbulencePersistence, 
                                false
                                ) - 1f
                                );
                        float num11 = (float)num9 + num10;
                        float num12 = f0 - num11 + 18 * 0;
                        num12 += MathUtils.Max(4f * (this.activeChunkProvider.TGDensityBias - (float)num9), 0f);
                        //Console.WriteLine("Value of num12 is " + num12);
                        grid3d.Set(k, m, l, (float)(num12));
                        //grid3d.Set(I++, num12);
                    }
                }
            }
            this.setBlock(new Grid2d[] { grid2d, grid2d2 }, grid3d, chunk, x1, z1, x2, z2);
        }

 
        
        public class Grid2d
        {

            public int m_sizeX;

            public int m_sizeY;

            public float[] m_data;

            public int SizeX
            {
                get
                {
                    return this.m_sizeX;
                }
            }

            
            public int SizeY
            {
                get
                {
                    return this.m_sizeY;
                }
            }

           
            public Grid2d(int sizeX, int sizeY)
            {
                sizeY = Math.Abs(sizeY);
                sizeX = Math.Abs(sizeX);
                this.m_sizeX = sizeX;
                this.m_sizeY = sizeY;
                this.m_data = new float[this.m_sizeX * this.m_sizeY];
            }

            public float Get(int x, int y)
            {
                return this.m_data[x + y * this.m_sizeX];
            }

            public void Set(int x, int y, float value)
            {
                this.m_data[x + y * this.m_sizeX] = value;
            }

            public float Sample(float x, float y)
            {
                int i = (int)MathUtils.Floor(x);
                int j = (int)MathUtils.Floor(y);
                int k = (int)MathUtils.Ceiling(x);
                int l = (int)MathUtils.Ceiling(y);
                float f = x - (float)i;
                float f2 = y - (float)j;
                float x2 = this.m_data[i + j * this.m_sizeX];
                float x3 = this.m_data[i + j * this.m_sizeX];
                float x4 = this.m_data[i + l * this.m_sizeX];
                float x5 = this.m_data[i + l * this.m_sizeX];
                float x6 = MathUtils.Lerp(x2, x3, f);
                float x7 = MathUtils.Lerp(x4, x5, f);
                return MathUtils.Lerp(x6, x7, f2);
            }

        }

        public class Grid3d
        {

            public int m_sizeX;

            public int m_sizeY;

            public int m_sizeZ;

            public int m_sizeXY;

            public float[] m_data;
            public int SizeX
            {
                get
                {
                    return this.m_sizeX;
                }
            }

            public int SizeY
            {
                get
                {
                    return this.m_sizeY;
                }
            }

            public int SizeZ
            {
                get
                {
                    return this.m_sizeZ;
                }
            }

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

            public void Get8(int x, int y, int z, out float v111, out float v211, out float v121, out float v221, out float v112, out float v212, out float v122, out float v222)
            {
                int i = x + y * this.m_sizeX + z * this.m_sizeXY;
                v111 = this.m_data[i];
                v211 = this.m_data[i + 1];
                v121 = this.m_data[i + this.m_sizeX];
                v221 = this.m_data[i + 1 + this.m_sizeX];
                v112 = this.m_data[i + this.m_sizeXY];
                v212 = this.m_data[i + 1 + this.m_sizeXY];
                v122 = this.m_data[i + this.m_sizeX + this.m_sizeXY];
                v222 = this.m_data[i + 1 + this.m_sizeX + this.m_sizeXY];
            }

            public float Get(int x, int y, int z)
            {
                return this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY];
            }

            public void Set(int index, float value)
            {
                this.m_data[index] = value;
            }
            public void Set(int x, int y, int z, float value)
            {
                this.m_data[x + y * this.m_sizeX + z * this.m_sizeXY] = value;
            }

            private float Sample(float x, float y, float z)
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

        }

        private float CalculateOceanShoreDistance(float x0, float z0)
        {
            float f0 = 0;
            if (this.activeChunkProvider.m_islandSize != null)
            {
                float f1 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f2 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f3 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0 + 1000f, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.X;
                float f4 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0 + 1000f, 0.005f / holder[8], 4, 1.95f, 1f, false) + this.activeChunkProvider.m_islandSize.Value.Y;
                f0 += MathUtils.Min(x0 - f1, z0 - f2, f3 - x0, f4 - z0);
            }
            else
            {
                float f5 = this.activeChunkProvider.m_oceanCorner.X + holder[7] * this.noiseMain.OctavedNoise(z0, 0f, 0.005f / holder[8], 4, 1.95f, 1f, false);
                float f6 = this.activeChunkProvider.m_oceanCorner.Y + holder[7] * this.noiseMain.OctavedNoise(0f, x0, 0.005f / holder[8], 4, 1.95f, 1f, false);
                f0 += MathUtils.Min(x0 - f5, z0 - f6);
            }
            //f0 = 3.3f;
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

        private float CalculateMountainRangeFactor(float x, float z)
        {
            float f = this.noiseMain.OctavedNoise(
                x + this.activeChunkProvider.m_mountainsOffset.X,
                z + this.activeChunkProvider.m_mountainsOffset.Y,
                holder[9] / this.activeChunkProvider.TGBiomeScaling + (float)Math.Pow(3, 35) * 0,
                3,
                1.91f,
                0.75f,
                true);
            //f = 54.5f;
            //Debug.WriteLine("CalculateMountainRangeFactor output : " + f);
            //f = 0.87777777f;
            return f;
            //return MathUtils.Clamp(f, 0.0f, 1.0f);
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

        public float CalculateHeight(float a, float b)
        {
            float f = this.CalculateHeightCriticalMethodApparently(a, b, 1.0f, false);
            //f = this.activeChunkProvider.CalculateHeight(a, b);
            //f = 0.33334f;
            return f ;
        }

        public float CalculateHeightCriticalMethodApparently(float x, float z, float y, bool allowMethodOverrideInternal)
        {
            float f_0 = holder[10] + holder[11] * MathUtils.PowSign(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, z + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
            float f_1 = allowMethodOverrideInternal ? this.CalculateOceanShoreDistance(x, z) : this.activeChunkProvider.CalculateOceanShoreDistance(x, z);
            float f_2 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(f_1));
            float f_3 = MathUtils.Saturate(MathUtils.Sin(holder[12] * f_1));
            float f_4 = MathUtils.Saturate(MathUtils.Saturate((0f - f_0) * f_1) - 0.85f * f_3);
            float f_5 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - f_1 - 10f)) - f_3);
            float f_6 = allowMethodOverrideInternal ? this.CalculateMountainRangeFactor(x, z) : this.activeChunkProvider.CalculateMountainRangeFactor(x, z);
            float f_7 = (1f - f_2) * this.noiseMain.OctavedNoise(x, z, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
            float f_8 = (1f - f_2) * this.noiseMain.OctavedNoise(x, z, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
            float f_9 = (1f - f_5) * (1f - f_2) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[13], 1f - holder[14]);
            float f_10 = (1f - f_5) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[14], 1f);
            float f_11 = 1f * this.noiseMain.OctavedNoise(x, z, holder[15], (int)holder[16], 1.93f, holder[17], false);
            float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f_7);
            float f_12 = 1.5f * this.noiseMain.OctavedNoise(x, z, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
            float f_13 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * f_10 + 0.5f * f_9 + MathUtils.Saturate(1f - f_1 / 30f)));
            float f_14 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(f_10 + 0.5f * f_9));
            float f_15 = MathUtils.Saturate(1.5f - f_13 * MathUtils.Abs(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, z + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
            float f_16 = -50f * f_4 + holder[18];
            float f_17 = MathUtils.Lerp(0f, 8f, f_7);
            float f_18 = MathUtils.Lerp(0f, -6f, f_8);
            float f_19 = holder[19] * f_9 * f_11;
            float f_20 = holder[20] * f_10 * f_12;
            float f_21 = holder[21] * f_15;
            float f_22 = f_16 + f_17 + f_18 + f_20 + f_19;
            float f_23 = MathUtils.Min(MathUtils.Lerp(f_22, f_14, f_21), f_22) * y;
            return MathUtils.Clamp(64f + f_23, 10f, 251f);
        }

        //For reference only. Direct reference to analog of TerrainChunkGeneratorProviderActive.CalculateHeight in ModifiedHolder. 
        private float a(float x, float z)
        {
            //float num = this.activeChunkProvider.TGOceanSlope + this.activeChunkProvider.TGOceanSlopeVariation * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, z + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
            float f_0 = holder[10] + holder[11] * MathUtils.PowSign(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_mountainsOffset.X, z + this.activeChunkProvider.m_mountainsOffset.Y, 0.01f, 1, 2f, 0.5f, false) - 1f, 0.5f);
            //float num2 = this.CalculateOceanShoreDistance(x, z);
            float f_1 = this.CalculateOceanShoreDistance(x, z);
            //float num3 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(num2));
            float f_2 = MathUtils.Saturate(2f - 0.05f * MathUtils.Abs(f_1));
            //float num4 = MathUtils.Saturate(MathUtils.Sin(this.activeChunkProvider.TGIslandsFrequency * num2));
            float f_3 = MathUtils.Saturate(MathUtils.Sin(holder[12] * f_1));
            //float num5 = MathUtils.Saturate(MathUtils.Saturate((0f - num) * num2) - 0.85f * num4);
            float f_4 = MathUtils.Saturate(MathUtils.Saturate((0f - f_0) * f_1) - 0.85f * f_3);
            //float num6 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - num2 - 10f)) - num4);
            float f_5 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (0f - f_1 - 10f)) - f_3);
            //float v = this.CalculateMountainRangeFactor(x, z);
            float f_6 = this.CalculateMountainRangeFactor(x, z);
            //float f = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
            float f_7 = (1f - f_2) * this.noiseMain.OctavedNoise(x, z, 0.001f / this.activeChunkProvider.TGBiomeScaling, 2, 2f, 0.5f, false);
            //float f2 = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
            float f_8 = (1f - f_2) * this.noiseMain.OctavedNoise(x, z, 0.0017f / this.activeChunkProvider.TGBiomeScaling, 2, 4f, 0.7f, false);
            //float num7 = (1f - num6) * (1f - num3) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.activeChunkProvider.TGHillsPercentage, 1f - this.activeChunkProvider.TGMountainsPercentage);
            float f_9 = (1f - f_5) * (1f - f_2) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[13], 1f - holder[14]);
            //float num8 = (1f - num6) * TerrainChunkGeneratorProviderActive.Squish(v, 1f - this.activeChunkProvider.TGMountainsPercentage, 1f);
            float f_10 = (1f - f_5) * TerrainChunkGeneratorProviderActive.Squish(f_6, 1f - holder[14], 1f);
            //float num9 = 1f * SimplexNoise.OctavedNoise(x, z, this.activeChunkProvider.TGHillsFrequency, this.activeChunkProvider.TGHillsOctaves, 1.93f, this.activeChunkProvider.TGHillsPersistence, false);
            float f_11 = 1f * this.noiseMain.OctavedNoise(x, z, holder[15], (int)holder[16], 1.93f, holder[17], false);
            //float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f);
            float amplitudeStep = MathUtils.Lerp(0.75f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, 1.33f * TerrainChunkGeneratorProviderActive.TGMountainsDetailPersistence, f_7);
            //float num10 = 1.5f * SimplexNoise.OctavedNoise(x, z, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
            float f_12 = 1.5f * this.noiseMain.OctavedNoise(x, z, TerrainChunkGeneratorProviderActive.TGMountainsDetailFreq, TerrainChunkGeneratorProviderActive.TGMountainsDetailOctaves, 1.98f, amplitudeStep, false) - 0.5f;
            //float num11 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * num8 + 0.5f * num7 + MathUtils.Saturate(1f - num2 / 30f)));
            float f_13 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * f_10 + 0.5f * f_9 + MathUtils.Saturate(1f - f_1 / 30f)));
            //float x2 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(num8 + 0.5f * num7));
            float f_14 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(f_10 + 0.5f * f_9));
            //float num12 = MathUtils.Saturate(1.5f - num11 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, z + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
            float f_15 = MathUtils.Saturate(1.5f - f_13 * MathUtils.Abs(2f * this.noiseMain.OctavedNoise(x + this.activeChunkProvider.m_riversOffset.X, z + this.activeChunkProvider.m_riversOffset.Y, 0.001f, 4, 2f, 0.5f, false) - 1f));
            //float num13 = -50f * num5 + this.activeChunkProvider.TGHeightBias;
            float f_16 = -50f * f_4 + holder[18];
            //float num14 = MathUtils.Lerp(0f, 8f, f);
            float f_17 = MathUtils.Lerp(0f, 8f, f_7);
            //float num15 = MathUtils.Lerp(0f, -6f, f2);
            float f_18 = MathUtils.Lerp(0f, -6f, f_8);
            //float num16 = this.activeChunkProvider.TGHillsStrength * num7 * num9;
            float f_19 = holder[19] * f_9 * f_11;
            //float num17 = this.activeChunkProvider.TGMountainsStrength * num8 * num10;
            float f_20 = holder[20] * f_10 * f_12;
            //float f3 = this.activeChunkProvider.TGRiversStrength * num12;
            float f_21 = holder[21] * f_15;
            //float num18 = num13 + num14 + num15 + num17 + num16;
            float f_22 = f_16 + f_17 + f_18 + f_20 + f_19;
            //float num19 = MathUtils.Min(MathUtils.Lerp(num18, x2, f3), num18);
            float f_23 = MathUtils.Min(MathUtils.Lerp(f_22, f_14, f_21), f_22);
            return MathUtils.Clamp(64f + f_23, 10f, 251f);

        }

        private void setBlock(Grid2d[] grid2DCache, Grid3d grid3DCache, TerrainChunk chunk, params int[] par)
        {
            //Sequence is safe below. 
            int u = (int)holder[holder.Length - 1] * 1 + this.worldSettings.SeaLevelOffset * 0;
            //u = 15;
            int oceanLevel = u;
            for (int i = 0; i < grid3DCache.SizeX - 1; i++)
            {
                for (int j = 0; j < grid3DCache.SizeZ - 1; j++)
                {
                    for (int k = 0; k < grid3DCache.SizeY - 1; k++)
                    {
                        grid3DCache.Get8(i, k, j, out float r0, out float r1, out float r2, out float r3, out float r4, out float r5, out float r6, out float r7);
                        double d0 = (r1 - r0) / 4f;
                        double d1 = (r3 - r2) / 4f;
                        double d2 = (r5 - r4) / 4f;
                        double d3 = (r7 - r6) / 4f;
                        double d4 = r0;
                        double d5 = r2;
                        double d6 = r4;
                        double d7 = r6;
                        for (int a = 0; a < 4; a++)//4
                        {
                            double d8 = (d6 - d4) / 4f;
                            double d9 = (d7 - d5) / 4f;
                            double d10 = d4;
                            double d11 = d5;
                            for (int b = 0; b < 4; b++)
                            {
                                double d12 = (d11 - d10) / 8f;
                                double d13 = d10;
                                //int x = a + i * 4;
                                //int y = b + j * 4;
                                //int x3 = x1 + a + i * 4;
                                //int z3 = z1 + b + j * 4;
                                float x4 = grid2DCache[0].Get(a + i * 4, b + j * 4);
                                float x5 = grid2DCache[1].Get(a + i * 4, b + j * 4);
                                int temperatureFast = chunk.GetTemperatureFast(par[0] + a + i * 4, par[1] + b + j * 4);
                                int humidityFast = chunk.GetHumidityFast(par[0] + a + i * 4, par[1] + b + j * 4);
                                float f = x5 - 0.01f * (float)humidityFast;
                                float f0 = MathUtils.Lerp(100f, 0f, f);
                                float f1 = MathUtils.Lerp(300f, 30f, f);
                                bool Desertificationflag = (temperatureFast > 8 && humidityFast < 8 && x5 < 0.97f) || (MathUtils.Abs(x4) < 16f && x5 < 0.97f);
                                Desertificationflag = false;
                                for (int c = 0; c < 8; c++)
                                {
                                    byte blockID = 0;
                                    if (d13 < 0f)
                                    {
                                        if (c + k * 8 <= oceanLevel)
                                        {
                                            blockID = BlocksManager.getID(block => block is MagmaBlock);
                                        }
                                    }
                                    else
                                    {
                                        byte graniteBlockId = BlocksManager.getID(block => block is GraniteBlock);
                                        byte sandstoneBlockId = BlocksManager.getID(block => block is SandstoneBlock);
                                        byte basaltBlockId = BlocksManager.getID(block => block is GlassBlock);
                                        if (!Desertificationflag)
                                        {
                                            if (d13 >= f1)
                                            {
                                                blockID = basaltBlockId;
                                            }
                                            else
                                            {
                                                blockID = graniteBlockId;
                                            }
                                        }
                                        else
                                        {
                                            if (d13 >= f0)
                                            {
                                                if (d13 >= f1)
                                                {
                                                    blockID = basaltBlockId;
                                                }
                                                else
                                                {
                                                    blockID = graniteBlockId;
                                                }
                                            }
                                            else
                                            {
                                                blockID = sandstoneBlockId;
                                            }
                                        }
                                    }
                                    chunk.SetCellValueFast(par[0] + a + i * 4, c + k * 8, par[1] + b + j * 4, blockID);
                                    d13 += d12;
                                }
                                d10 += d8;
                                d11 += d9;
                            }
                            d4 += d0;
                            d5 += d1;
                            d6 += d2;
                            d7 += d3;
                        }
                    }
                }
            }
        }

    }

    


}
