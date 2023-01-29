using System;
using System.Diagnostics;
using Engine;

namespace Game
{
	// Token: 0x020002DF RID: 735
	public class Random
	{
		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x000A0ED5 File Offset: 0x0009F0D5
		// (set) Token: 0x060014B5 RID: 5301 RVA: 0x000A0EE9 File Offset: 0x0009F0E9
		public ulong State
		{
			get
			{
				return (ulong)this.m_s0 + ((ulong)this.m_s1 << 32);
			}
			set
			{
				this.m_s0 = (uint)value;
				this.m_s1 = (uint)(value >> 32);
			}
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x000A0EFE File Offset: 0x0009F0FE
		public Random()
		{
			this.Seed();
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x000A0F0C File Offset: 0x0009F10C
		public Random(int seed)
		{
			this.Seed(seed);
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x000A0F1B File Offset: 0x0009F11B
		public void Seed()
		{
			this.Seed(Game.Random.m_counter++);
		}

		public int UniformInt(int min, int max) => (int)((long)min + (long)this.Int() * (long)(max - min + 1) / 2147483648L);

        // Token: 0x060014B9 RID: 5305 RVA: 0x000A0F30 File Offset: 0x0009F130
        public void Seed(int seed)
		{
			this.m_s0 = MathUtils.Hash((uint)seed);
			this.m_s1 = MathUtils.Hash((uint)(seed + 1));
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x000A0F4C File Offset: 0x0009F14C
		public int Sign()
		{
			return this.Int() % 2 * 2 - 1;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x000A0F5A File Offset: 0x0009F15A
		public bool Bool()
		{
			return (this.Int() & 1) != 0;
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x000A0F67 File Offset: 0x0009F167
		public bool Bool(float probability)
		{
			return (float)this.Int() / 2.147484E+09f < probability;
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x000A0F7C File Offset: 0x0009F17C
		public uint UInt()
		{
			uint s = this.m_s0;
			uint num = this.m_s1;
			num ^= s;
			this.m_s0 = (Game.Random.RotateLeft(s, 26) ^ num ^ num << 9);
			this.m_s1 = Game.Random.RotateLeft(num, 13);
			return Game.Random.RotateLeft(s * 2654435771U, 5) * 5U;
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x000A0FCD File Offset: 0x0009F1CD
		public int Int()
		{
			return (int)(this.UInt() & 2147483647U);
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x000A0FDB File Offset: 0x0009F1DB
		public int Int(int bound)
		{
			return (int)((long)this.Int() * (long)bound / 2147483648L);
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x000A0FEF File Offset: 0x0009F1EF
		public int Int(int min, int max)
		{
			return (int)((long)min + (long)this.Int() * (long)(max - min + 1) / 2147483648L);
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x000A100A File Offset: 0x0009F20A
		public float Float()
		{
			return (float)this.Int() / 2.147484E+09f;
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x000A1019 File Offset: 0x0009F219
		public float Float(float min, float max)
		{
			return min + this.Float() * (max - min);
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x000A1028 File Offset: 0x0009F228
		public float NormalFloat(float mean, float stddev)
		{
			float num = this.Float();
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

		// Token: 0x060014C4 RID: 5316 RVA: 0x000A112C File Offset: 0x0009F32C
		public Vector2 Vector2()
		{
			float num;
			float num2;
			float num3;
			float num4;
			float num5;
			do
			{
				num = 2f * this.Float() - 1f;
				num2 = 2f * this.Float() - 1f;
				num3 = num * num;
				num4 = num2 * num2;
				num5 = num3 + num4;
			}
			while (num5 >= 1f);
			float num6 = 1f / num5;
			return new Vector2((num3 - num4) * num6, 2f * num * num2 * num6);
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x000A1196 File Offset: 0x0009F396
		public Vector2 Vector2(float length)
		{
			return Engine.Vector2.Normalize(this.Vector2()) * length;
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x000A11A9 File Offset: 0x0009F3A9
		public Vector2 Vector2(float minLength, float maxLength)
		{
			return Engine.Vector2.Normalize(this.Vector2()) * this.Float(minLength, maxLength);
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x000A11C4 File Offset: 0x0009F3C4
		public Vector3 Vector3()
		{
			float num;
			float num2;
			float num3;
			do
			{
				num = 2f * this.Float() - 1f;
				num2 = 2f * this.Float() - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f);
			float num4 = MathUtils.Sqrt(1f - num3);
			return new Vector3(2f * num * num4, 2f * num2 * num4, 1f - 2f * num3);
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x000A1238 File Offset: 0x0009F438
		public Vector3 Vector3(float length)
		{
			return Engine.Vector3.Normalize(this.Vector3()) * length;
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x000A124B File Offset: 0x0009F44B
		public Vector3 Vector3(float minLength, float maxLength)
		{
			return Engine.Vector3.Normalize(this.Vector3()) * this.Float(minLength, maxLength);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x000A1265 File Offset: 0x0009F465
		public static uint RotateLeft(uint x, int k)
		{
			return x << k | x >> 32 - k;
		}

		// Token: 0x04000EC1 RID: 3777
		public static int m_counter = (int)(Stopwatch.GetTimestamp() + DateTime.Now.Ticks);

		// Token: 0x04000EC2 RID: 3778
		public uint m_s0;

		// Token: 0x04000EC3 RID: 3779
		public uint m_s1;
	}
}
