using System;
using Engine;

namespace Game
{
	// Token: 0x02000022 RID: 34
	public class BreadBlock : FoodBlock
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000763B File Offset: 0x0000583B
		public BreadBlock() : base("Models/Bread", Matrix.Identity, Color.White, 242)
		{
		}

		// Token: 0x0400007F RID: 127
		public const int Index = 177;
	}
}
