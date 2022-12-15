using System;

namespace Game
{
	// Token: 0x02000244 RID: 580
	public class CraftingRecipe
	{
		// Token: 0x04000BDF RID: 3039
		public const int MaxSize = 3;

		// Token: 0x04000BE0 RID: 3040
		public int ResultValue;

		// Token: 0x04000BE1 RID: 3041
		public int ResultCount;

		// Token: 0x04000BE2 RID: 3042
		public int RemainsValue;

		// Token: 0x04000BE3 RID: 3043
		public int RemainsCount;

		// Token: 0x04000BE4 RID: 3044
		public float RequiredHeatLevel;

		// Token: 0x04000BE5 RID: 3045
		public float RequiredPlayerLevel;

		// Token: 0x04000BE6 RID: 3046
		public string[] Ingredients = new string[9];

		// Token: 0x04000BE7 RID: 3047
		public string Description;

		// Token: 0x04000BE8 RID: 3048
		public string Message;
	}
}
