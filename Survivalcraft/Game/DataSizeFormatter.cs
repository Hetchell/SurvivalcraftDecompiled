using System;
using Engine;

namespace Game
{
	// Token: 0x02000247 RID: 583
	public static class DataSizeFormatter
	{
		// Token: 0x060011C2 RID: 4546 RVA: 0x00089830 File Offset: 0x00087A30
		public static string Format(long bytes)
		{
			if (bytes < 1024L)
			{
				return string.Format("{0}B", bytes);
			}
			if (bytes < 1048576L)
			{
				float num = (float)bytes / 1024f;
				return string.Format(DataSizeFormatter.PrepareFormatString(num, "kB"), num);
			}
			if (bytes < 1073741824L)
			{
				float num2 = (float)bytes / 1024f / 1024f;
				return string.Format(DataSizeFormatter.PrepareFormatString(num2, "MB"), num2);
			}
			float num3 = (float)bytes / 1024f / 1024f / 1024f;
			return string.Format(DataSizeFormatter.PrepareFormatString(num3, "GB"), num3);
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x000898DC File Offset: 0x00087ADC
		public static string PrepareFormatString(float value, string unit)
		{
			int num = (int)(MathUtils.Log10(value) + 1f);
			return "{0:F" + MathUtils.Max(3 - num, 0).ToString() + "}" + unit;
		}
	}
}
