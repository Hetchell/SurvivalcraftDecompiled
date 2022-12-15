using System;
using Engine;

namespace Game
{
	// Token: 0x020000D8 RID: 216
	public class RottenBirdBlock : FoodBlock
	{
		// Token: 0x06000440 RID: 1088 RVA: 0x00016FE2 File Offset: 0x000151E2
		public RottenBirdBlock() : base("Models/Bird", Matrix.CreateTranslation(-0.9375f, 0.4375f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001E4 RID: 484
		public const int Index = 239;
	}
}
