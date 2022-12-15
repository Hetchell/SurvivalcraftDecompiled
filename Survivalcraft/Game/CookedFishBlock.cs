using System;
using Engine;

namespace Game
{
	// Token: 0x0200003D RID: 61
	public class CookedFishBlock : FoodBlock
	{
		// Token: 0x06000154 RID: 340 RVA: 0x00009037 File Offset: 0x00007237
		public CookedFishBlock() : base("Models/Fish", Matrix.Identity, new Color(160, 80, 40), 241)
		{
		}

		// Token: 0x040000B3 RID: 179
		public const int Index = 162;
	}
}
