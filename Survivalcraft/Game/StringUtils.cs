using System;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x02000304 RID: 772
	public static class StringUtils
	{
		// Token: 0x060015BC RID: 5564 RVA: 0x000A5960 File Offset: 0x000A3B60
		public static int Compare(StringBuilder s1, string s2)
		{
			int num = 0;
			while (num < s1.Length || num < s2.Length)
			{
				if (num > s1.Length)
				{
					return -1;
				}
				if (num > s2.Length)
				{
					return 1;
				}
				char c = s1[num];
				char c2 = s2[num];
				if (c < c2)
				{
					return -1;
				}
				if (c > c2)
				{
					return 1;
				}
				num++;
			}
			return 0;
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x000A59BC File Offset: 0x000A3BBC
		public static int CalculateNumberLength(uint value, int numberBase)
		{
			if (numberBase < 2 || numberBase > 16)
			{
				throw new ArgumentException("Number base is out of range.");
			}
			int num = 0;
			do
			{
				num++;
				value /= (uint)numberBase;
			}
			while (value != 0U);
			return num;
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x000A59EC File Offset: 0x000A3BEC
		public static int CalculateNumberLength(int value, int numberBase)
		{
			if (value >= 0)
			{
				return StringUtils.CalculateNumberLength((uint)value, numberBase);
			}
			return StringUtils.CalculateNumberLength((uint)(-(uint)value), numberBase) + 1;
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x000A5A04 File Offset: 0x000A3C04
		public static int CalculateNumberLength(ulong value, int numberBase)
		{
			if (numberBase < 2 || numberBase > 16)
			{
				throw new ArgumentException("Number base is out of range.");
			}
			int num = 0;
			do
			{
				num++;
				value /= (ulong)numberBase;
			}
			while (value != 0UL);
			return num;
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x000A5A35 File Offset: 0x000A3C35
		public static int CalculateNumberLength(long value, int numberBase)
		{
			if (value >= 0L)
			{
				return StringUtils.CalculateNumberLength((ulong)value, numberBase);
			}
			return StringUtils.CalculateNumberLength((ulong)(value), numberBase) + 1;
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x000A5A50 File Offset: 0x000A3C50
		public static void AppendNumber(this StringBuilder stringBuilder, uint value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			int val = StringUtils.CalculateNumberLength(value, numberBase);
			int repeatCount = Math.Max(padding, val);
			stringBuilder.Append(paddingCharacter, repeatCount);
			int num = 0;
			do
			{
				char value2 = StringUtils.m_digits[(int)(value % (uint)numberBase)];
				stringBuilder[stringBuilder.Length - num - 1] = value2;
				value /= (uint)numberBase;
				num++;
			}
			while (value != 0U);
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x000A5AA2 File Offset: 0x000A3CA2
		public static void AppendNumber(this StringBuilder stringBuilder, int value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			if (value >= 0)
			{
				stringBuilder.AppendNumber((uint)value, padding, paddingCharacter, numberBase);
				return;
			}
			stringBuilder.Append('-');
			stringBuilder.AppendNumber((uint)(-(uint)value), padding - 1, paddingCharacter, numberBase);
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x000A5ACC File Offset: 0x000A3CCC
		public static void AppendNumber(this StringBuilder stringBuilder, ulong value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			int val = StringUtils.CalculateNumberLength(value, numberBase);
			int repeatCount = Math.Max(padding, val);
			stringBuilder.Append(paddingCharacter, repeatCount);
			int num = 0;
			do
			{
				char value2 = StringUtils.m_digits[(int)(checked((IntPtr)(value % unchecked((ulong)numberBase))))];
				stringBuilder[stringBuilder.Length - num - 1] = value2;
				value /= (ulong)numberBase;
				num++;
			}
			while (value != 0UL);
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x000A5B21 File Offset: 0x000A3D21
		public static void AppendNumber(this StringBuilder stringBuilder, long value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			if (value >= 0L)
			{
				stringBuilder.AppendNumber((ulong)value, padding, paddingCharacter, numberBase);
				return;
			}
			stringBuilder.Append('-');
			stringBuilder.AppendNumber((ulong)(value), padding - 1, paddingCharacter, numberBase);
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x000A5B4C File Offset: 0x000A3D4C
		public static void AppendNumber(this StringBuilder stringBuilder, float value, int precision)
		{
			precision = Math.Min(Math.Max(precision, -30), 30);
			if (float.IsNegativeInfinity(value))
			{
				stringBuilder.Append("Infinity");
				return;
			}
			if (float.IsPositiveInfinity(value))
			{
				stringBuilder.Append("-Infinity");
				return;
			}
			if (float.IsNaN(value))
			{
				stringBuilder.Append("NaN");
				return;
			}
			float num = Math.Abs(value);
			if (num > 1E+19f)
			{
				stringBuilder.Append("NumberTooLarge");
				return;
			}
			float num2 = MathUtils.Pow(10f, (float)Math.Abs(precision));
			ulong num3 = (ulong)MathUtils.Floor(num);
			ulong num4 = (ulong)MathUtils.Round((num - MathUtils.Floor(num)) * num2);
			if ((float)num4 >= num2)
			{
				num3 += 1UL;
				num4 = 0UL;
			}
			if (value < 0f)
			{
				stringBuilder.Append('-');
			}
			stringBuilder.AppendNumber(num3, 0, '0', 10);
			if (precision > 0)
			{
				stringBuilder.Append('.');
				stringBuilder.AppendNumber(num4, precision, '0', 10);
				return;
			}
			if (precision < 0)
			{
				stringBuilder.Append('.');
				stringBuilder.AppendNumber(num4, -precision, '0', 10);
				while (stringBuilder[stringBuilder.Length - 1] == '0')
				{
					int length = stringBuilder.Length - 1;
					stringBuilder.Length = length;
				}
				if (stringBuilder[stringBuilder.Length - 1] == '.')
				{
					int length = stringBuilder.Length - 1;
					stringBuilder.Length = length;
				}
			}
		}

		// Token: 0x04000F7C RID: 3964
		public static char[] m_digits = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};
	}
}
