using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000268 RID: 616
	public static class EnumUtils
	{
		// Token: 0x0600125C RID: 4700 RVA: 0x0008DEFC File Offset: 0x0008C0FC
		public static string GetEnumName(Type type, int value)
		{
			int num = EnumUtils.GetEnumValues(type).IndexOf(value);
			if (num >= 0)
			{
				return EnumUtils.GetEnumNames(type)[num];
			}
			return "<invalid enum>";
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x0008DF2C File Offset: 0x0008C12C
		public static ReadOnlyList<string> GetEnumNames(Type type)
		{
			return EnumUtils.Cache.Query(type).Names;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x0008DF3E File Offset: 0x0008C13E
		public static ReadOnlyList<int> GetEnumValues(Type type)
		{
			return EnumUtils.Cache.Query(type).Values;
		}

		// Token: 0x02000491 RID: 1169
		public struct NamesValues
		{
			// Token: 0x040016F2 RID: 5874
			public ReadOnlyList<string> Names;

			// Token: 0x040016F3 RID: 5875
			public ReadOnlyList<int> Values;
		}

		// Token: 0x02000492 RID: 1170
		public static class Cache
		{
			// Token: 0x06001F8A RID: 8074 RVA: 0x000E2020 File Offset: 0x000E0220
			public static EnumUtils.NamesValues Query(Type type)
			{
				Dictionary<Type, EnumUtils.NamesValues> namesValuesByType = EnumUtils.Cache.m_namesValuesByType;
				EnumUtils.NamesValues result;
				lock (namesValuesByType)
				{
					EnumUtils.NamesValues namesValues;
					if (!EnumUtils.Cache.m_namesValuesByType.TryGetValue(type, out namesValues))
					{
						namesValues = new EnumUtils.NamesValues
						{
							Names = new ReadOnlyList<string>(new List<string>(Enum.GetNames(type))),
							Values = new ReadOnlyList<int>(new List<int>(Enum.GetValues(type).Cast<int>()))
						};
						EnumUtils.Cache.m_namesValuesByType.Add(type, namesValues);
					}
					EnumUtils.NamesValues namesValues2 = namesValues;
					result = namesValues2;
				}
				return result;
			}

			// Token: 0x040016F4 RID: 5876
			public static Dictionary<Type, EnumUtils.NamesValues> m_namesValuesByType = new Dictionary<Type, EnumUtils.NamesValues>();
		}
	}
}
