using System;
using Engine;

namespace Game
{
	// Token: 0x020000D9 RID: 217
	public class RottenBreadBlock : FoodBlock
	{
		// Token: 0x06000441 RID: 1089 RVA: 0x0001700D File Offset: 0x0001520D
		public RottenBreadBlock() : base("Models/Bread", Matrix.CreateTranslation(-0.375f, -0.25f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001E5 RID: 485
		public const int Index = 242;
	}
}
