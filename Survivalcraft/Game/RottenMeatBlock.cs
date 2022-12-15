using System;
using Engine;

namespace Game
{
	// Token: 0x020000DD RID: 221
	public class RottenMeatBlock : FoodBlock
	{
		// Token: 0x06000445 RID: 1093 RVA: 0x000170B9 File Offset: 0x000152B9
		public RottenMeatBlock() : base("Models/Meat", Matrix.CreateTranslation(-0.0625f, 0f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001E9 RID: 489
		public const int Index = 240;
	}
}
