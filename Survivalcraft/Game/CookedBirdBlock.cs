using System;
using Engine;

namespace Game
{
	// Token: 0x0200003C RID: 60
	public class CookedBirdBlock : FoodBlock
	{
		// Token: 0x06000153 RID: 339 RVA: 0x00009012 File Offset: 0x00007212
		public CookedBirdBlock() : base("Models/Bird", Matrix.Identity, new Color(150, 69, 15), 239)
		{
		}

		// Token: 0x040000B2 RID: 178
		public const int Index = 78;
	}
}
