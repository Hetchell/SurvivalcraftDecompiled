using System;
using Engine;

namespace Game
{
	// Token: 0x020000DA RID: 218
	public class RottenDoughBlock : FoodBlock
	{
		// Token: 0x06000442 RID: 1090 RVA: 0x00017038 File Offset: 0x00015238
		public RottenDoughBlock() : base("Models/Bread", Matrix.CreateTranslation(-0.375f, -0.25f, 0f), new Color(192, 255, 212), FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001E6 RID: 486
		public const int Index = 247;
	}
}
