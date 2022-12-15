using System;
using Engine;

namespace Game
{
	// Token: 0x020000DC RID: 220
	public class RottenFishBlock : FoodBlock
	{
		// Token: 0x06000444 RID: 1092 RVA: 0x0001708E File Offset: 0x0001528E
		public RottenFishBlock() : base("Models/Fish", Matrix.CreateTranslation(-0.125f, 0.125f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001E8 RID: 488
		public const int Index = 241;
	}
}
