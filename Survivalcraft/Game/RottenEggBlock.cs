using System;
using Engine;

namespace Game
{
	// Token: 0x020000DB RID: 219
	public class RottenEggBlock : FoodBlock
	{
		// Token: 0x06000443 RID: 1091 RVA: 0x00017072 File Offset: 0x00015272
		public RottenEggBlock() : base("Models/RottenEgg", Matrix.Identity, Color.White, 246)
		{
		}

		// Token: 0x040001E7 RID: 487
		public const int Index = 246;
	}
}
