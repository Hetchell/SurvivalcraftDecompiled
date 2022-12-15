using System;
using Engine;

namespace Game
{
	// Token: 0x0200005E RID: 94
	public class DoughBlock : FoodBlock
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000A95D File Offset: 0x00008B5D
		public DoughBlock() : base("Models/Bread", Matrix.CreateTranslation(0.5625f, -0.875f, 0f), new Color(241, 231, 214), 247)
		{
		}

		// Token: 0x040000DD RID: 221
		public const int Index = 176;
	}
}
