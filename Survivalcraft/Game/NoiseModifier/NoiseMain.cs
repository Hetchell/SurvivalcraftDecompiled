using Engine;
using Game;
using OpenTK;
using Random = Game.Random;

namespace Survivalcraft.Game.NoiseModifier
{
    public class NoiseMain
    {
        private readonly Random rand;
        // Token: 0x04000F3C RID: 3900
        private int[][] m_grad3;
        private readonly int octaves;
        private ImprovedNoise[] noises;
        // Token: 0x04000F3D RID: 3901
        public int[] m_permutations;
        public NoiseMain(Random rand, int octaves)
        {
            this.rand = rand;
            this.octaves = octaves;
            this.m_permutations = new int[512];
            this.noises = new ImprovedNoise[octaves];
            for (int i = 0; i < 256; this.m_permutations[i] = i++) ;
            for (int l = 0; l < 256; ++l)
            {
                int j = rand.Int(256 - l) + l;
                (this.m_permutations[j], this.m_permutations[l]) = (this.m_permutations[l], this.m_permutations[j]);
                this.m_permutations[l + 256] = this.m_permutations[l];
            }
            int[][] array = new int[12][];
            int num = 0;
            int[] array2 = new int[3];
            array2[0] = 1;
            array2[1] = 1;
            array[num] = array2;
            int num2 = 1;
            int[] array3 = new int[3];
            array3[0] = -1;
            array3[1] = 1;
            array[num2] = array3;
            int num3 = 2;
            int[] array4 = new int[3];
            array4[0] = 1;
            array4[1] = -1;
            array[num3] = array4;
            int num4 = 3;
            int[] array5 = new int[3];
            array5[0] = -1;
            array5[1] = -1;
            array[num4] = array5;
            array[4] = new int[]
            {
                1,
                0,
                1
            };
            array[5] = new int[]
            {
                -1,
                0,
                1
            };
            array[6] = new int[]
            {
                1,
                0,
                -1
            };
            array[7] = new int[]
            {
                -1,
                0,
                -1
            };
            array[8] = new int[]
            {
                0,
                1,
                1
            };
            array[9] = new int[]
            {
                0,
                -1,
                1
            };
            array[10] = new int[]
            {
                0,
                1,
                -1
            };
            array[11] = new int[]
            {
                0,
                -1,
                -1
            };
            this.m_grad3 = array;
            for(int i = 0; i < octaves; i++)
            {
                this.noises[i] = new ImprovedNoise(rand);
            }
        }
        public static float Dot(int[] g, float x, float y)
        {
            return (float)g[0] * x + (float)g[1] * y;
        }

        // Token: 0x06001576 RID: 5494 RVA: 0x000A39D3 File Offset: 0x000A1BD3
        public static float Dot(int[] g, float x, float y, float z)
        {
            return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
        }

        // Token: 0x06001577 RID: 5495 RVA: 0x000A39E9 File Offset: 0x000A1BE9
        public static float Hash(int x)
        {
            x = (x << 13 ^ x);
            return (float)(x * (x * x * 15731 + 789221) + 1376312589 & int.MaxValue) / 2.1474836E+09f;
        }

        // Token: 0x06001578 RID: 5496 RVA: 0x000A3A18 File Offset: 0x000A1C18
        public float Noise(float x)
        {
            int num = (int)MathUtils.Floor(x);
            int x2 = (int)MathUtils.Ceiling(x);
            float num2 = x - (float)num;
            float num3 = NoiseMain.Hash(num);
            float num4 = NoiseMain.Hash(x2);
            return num3 + num2 * num2 * (3f - 2f * num2) * (num4 - num3);
        }

        // Token: 0x06001579 RID: 5497 RVA: 0x000A3A60 File Offset: 0x000A1C60
        public float Noise(float x, float y)
        {
            float num = (x + y) * 0.36602542f;
            int num2 = (int)MathUtils.Floor(x + num);
            int num3 = (int)MathUtils.Floor(y + num);
            float num4 = (float)(num2 + num3) * 0.21132487f;
            float num5 = (float)num2 - num4;
            float num6 = (float)num3 - num4;
            float num7 = x - num5;
            float num8 = y - num6;
            int num9;
            int num10;
            if (num7 > num8)
            {
                num9 = 1;
                num10 = 0;
            }
            else
            {
                num9 = 0;
                num10 = 1;
            }
            float num11 = num7 - (float)num9 + 0.21132487f;
            float num12 = num8 - (float)num10 + 0.21132487f;
            float num13 = num7 - 1f + 0.42264974f;
            float num14 = num8 - 1f + 0.42264974f;
            int num15 = num2 & 255;
            int num16 = num3 & 255;
            int num17 = this.m_permutations[num15 + this.m_permutations[num16]] % 12;
            int num18 = this.m_permutations[num15 + num9 + this.m_permutations[num16 + num10]] % 12;
            int num19 = this.m_permutations[num15 + 1 + this.m_permutations[num16 + 1]] % 12;
            float num20 = 0.5f - num7 * num7 - num8 * num8;
            float num21;
            if (num20 < 0f)
            {
                num21 = 0f;
            }
            else
            {
                num20 *= num20;
                num21 = num20 * num20 * NoiseMain.Dot(this.m_grad3[num17], num7, num8);
            }
            float num22 = 0.5f - num11 * num11 - num12 * num12;
            float num23;
            if (num22 < 0f)
            {
                num23 = 0f;
            }
            else
            {
                num22 *= num22;
                num23 = num22 * num22 * NoiseMain.Dot(this.m_grad3[num18], num11, num12);
            }
            float num24 = 0.5f - num13 * num13 - num14 * num14;
            float num25;
            if (num24 < 0f)
            {
                num25 = 0f;
            }
            else
            {
                num24 *= num24;
                num25 = num24 * num24 * NoiseMain.Dot(this.m_grad3[num19], num13, num14);
            }
            return 35f * (num21 + num23 + num25) + 0.5f;
        }

        // Token: 0x0600157A RID: 5498 RVA: 0x000A3C3C File Offset: 0x000A1E3C
        public float Noise(float x, float y, float z)
        {
            float num = (x + y + z) * 0.33333334f;
            int num2 = (int)MathUtils.Floor(x + num);
            int num3 = (int)MathUtils.Floor(y + num);
            int num4 = (int)MathUtils.Floor(z + num);
            float num5 = (float)(num2 + num3 + num4) * 0.16666667f;
            float num6 = (float)num2 - num5;
            float num7 = (float)num3 - num5;
            float num8 = (float)num4 - num5;
            float num9 = x - num6;
            float num10 = y - num7;
            float num11 = z - num8;
            int num12;
            int num13;
            int num14;
            int num15;
            int num16;
            int num17;
            if (num9 >= num10)
            {
                if (num10 >= num11)
                {
                    num12 = 1;
                    num13 = 0;
                    num14 = 0;
                    num15 = 1;
                    num16 = 1;
                    num17 = 0;
                }
                else if (num9 >= num11)
                {
                    num12 = 1;
                    num13 = 0;
                    num14 = 0;
                    num15 = 1;
                    num16 = 0;
                    num17 = 1;
                }
                else
                {
                    num12 = 0;
                    num13 = 0;
                    num14 = 1;
                    num15 = 1;
                    num16 = 0;
                    num17 = 1;
                }
            }
            else if (num10 < num11)
            {
                num12 = 0;
                num13 = 0;
                num14 = 1;
                num15 = 0;
                num16 = 1;
                num17 = 1;
            }
            else if (num9 < num11)
            {
                num12 = 0;
                num13 = 1;
                num14 = 0;
                num15 = 0;
                num16 = 1;
                num17 = 1;
            }
            else
            {
                num12 = 0;
                num13 = 1;
                num14 = 0;
                num15 = 1;
                num16 = 1;
                num17 = 0;
            }
            float num18 = num9 - (float)num12 + 0.16666667f;
            float num19 = num10 - (float)num13 + 0.16666667f;
            float num20 = num11 - (float)num14 + 0.16666667f;
            float num21 = num9 - (float)num15 + 0.33333334f;
            float num22 = num10 - (float)num16 + 0.33333334f;
            float num23 = num11 - (float)num17 + 0.33333334f;
            float num24 = num9 - 1f + 0.5f;
            float num25 = num10 - 1f + 0.5f;
            float num26 = num11 - 1f + 0.5f;
            int num27 = num2 & 255;
            int num28 = num3 & 255;
            int num29 = num4 & 255;
            int num30 = this.m_permutations[num27 + this.m_permutations[num28 + this.m_permutations[num29]]] % 12;
            int num31 = this.m_permutations[num27 + num12 + this.m_permutations[num28 + num13 + this.m_permutations[num29 + num14]]] % 12;
            int num32 = this.m_permutations[num27 + num15 + this.m_permutations[num28 + num16 + this.m_permutations[num29 + num17]]] % 12;
            int num33 = this.m_permutations[num27 + 1 + this.m_permutations[num28 + 1 + this.m_permutations[num29 + 1]]] % 12;
            float num34 = 0.6f - num9 * num9 - num10 * num10 - num11 * num11;
            float num35;
            if (num34 < 0f)
            {
                num35 = 0f;
            }
            else
            {
                num34 *= num34;
                num35 = num34 * num34 * NoiseMain.Dot(this.m_grad3[num30], num9, num10, num11);
            }
            float num36 = 0.6f - num18 * num18 - num19 * num19 - num20 * num20;
            float num37;
            if (num36 < 0f)
            {
                num37 = 0f;
            }
            else
            {
                num36 *= num36;
                num37 = num36 * num36 * NoiseMain.Dot(this.m_grad3[num31], num18, num19, num20);
            }
            float num38 = 0.6f - num21 * num21 - num22 * num22 - num23 * num23;
            float num39;
            if (num38 < 0f)
            {
                num39 = 0f;
            }
            else
            {
                num38 *= num38;
                num39 = num38 * num38 * NoiseMain.Dot(this.m_grad3[num32], num21, num22, num23);
            }
            float num40 = 0.6f - num24 * num24 - num25 * num25 - num26 * num26;
            float num41;
            if (num40 < 0f)
            {
                num41 = 0f;
            }
            else
            {
                num40 *= num40;
                num41 = num40 * num40 * NoiseMain.Dot(this.m_grad3[num33], num24, num25, num26);
            }
            return 16f * (num35 + num37 + num39 + num41) + 0.5f;
        }

        // Token: 0x0600157B RID: 5499 RVA: 0x000A3FB4 File Offset: 0x000A21B4
        public float OctavedNoise(float x, float frequency, int octaves, float frequencyStep, float amplitudeStep, bool ridged = false)
        {
            float num = 0f;
            float num2 = 0f;
            float num3 = 1f;
            for (int i = 0; i < octaves; i++)
            {
                num += num3 * this.Noise(x * frequency);
                num2 += num3;
                frequency *= frequencyStep;
                num3 *= amplitudeStep;
            }
            if (!ridged)
            {
                return num / num2;
            }
            return 1f - MathUtils.Abs(2f * num / num2 - 1f);
        }

        // Token: 0x0600157C RID: 5500 RVA: 0x000A401C File Offset: 0x000A221C
        public float OctavedNoise(float x, float y, float frequency, int octaves, float frequencyStep, float amplitudeStep, bool ridged = false)
        {
            float num = 0f;
            float num2 = 0f;
            float num3 = 1f;
            for (int i = 0; i < octaves; i++)
            {
                num += num3 * this.Noise(x * frequency, y * frequency);
                num2 += num3;
                frequency *= frequencyStep;
                num3 *= amplitudeStep;
            }
            if (!ridged)
            {
                return num / num2;
            }
            return 1f - MathUtils.Abs(2f * num / num2 - 1f);
        }

        // Token: 0x0600157D RID: 5501 RVA: 0x000A4088 File Offset: 0x000A2288
        public float OctavedNoise(float x, float y, float z, float frequency, int octaves, float frequencyStep, float amplitudeStep, bool ridged = false)
        {
            float num = 0f;
            float num2 = 0f;
            float num3 = 1f;
            for (int i = 0; i < octaves; i++)
            {
                num += num3 * this.Noise(x * frequency, y * frequency, z * frequency);
                num2 += num3;
                frequency *= frequencyStep;
                num3 *= amplitudeStep;
            }
            if (!ridged)
            {
                return num / num2;
            }
            return 1f - MathUtils.Abs(2f * num / num2 - 1f);
        }

        public double[] UseImprovedNoiseGenerateNoiseOctaves(double[] noiseArray, int xOffset, int yOffset, int zOffset, int xSize, int ySize, int zSize, double xScale, double yScale, double zScale)
        {
            if (noiseArray == null)
            {
                noiseArray = new double[xSize * ySize * zSize];
            }
            else
            {
                for (int i = 0; i < noiseArray.Length; ++i)
                {
                    noiseArray[i] = 0.0D;
                }
            }

            double d3 = 1.0D;

            for (int j = 0; j < this.octaves; ++j)
            {
                double d0 = (double)xOffset * d3 * xScale;
                double d1 = (double)yOffset * d3 * yScale;
                double d2 = (double)zOffset * d3 * zScale;
                this.noises[j].populateNoiseArray(noiseArray, d0, d1, d2, xSize, ySize, zSize, xScale * d3, yScale * d3, zScale * d3, d3);
                d3 /= 2.0D;
            }

            return noiseArray;
        }
    }

}
