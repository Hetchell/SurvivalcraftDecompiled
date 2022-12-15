using System;

namespace Game
{
	// Token: 0x02000303 RID: 771
	public static class StringsManager
	{
		// Token: 0x060015BA RID: 5562 RVA: 0x000A594F File Offset: 0x000A3B4F
		public static string GetString(string name)
		{
			return LanguageControl.Get("Strings", name);
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x000A595C File Offset: 0x000A3B5C
		public static void LoadStrings()
		{
		}
	}
}
