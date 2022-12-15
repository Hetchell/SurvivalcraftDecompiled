using System;
using Engine;

namespace Game
{
	// Token: 0x0200003E RID: 62
	public class CookedMeatBlock : FoodBlock
	{
		// Token: 0x06000155 RID: 341 RVA: 0x0000905C File Offset: 0x0000725C
		public CookedMeatBlock() : base("Models/Meat", Matrix.Identity, new Color(155, 122, 51), 240)
		{
		}

		// Token: 0x040000B4 RID: 180
		public const int Index = 89;
	}
}
