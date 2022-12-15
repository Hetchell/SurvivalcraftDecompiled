using System;
using Engine;

namespace Game
{
	// Token: 0x020000D1 RID: 209
	public class RawBirdBlock : FoodBlock
	{
		// Token: 0x0600042C RID: 1068 RVA: 0x000169E7 File Offset: 0x00014BE7
		public RawBirdBlock() : base("Models/Bird", Matrix.Identity, new Color(224, 170, 164), 239)
		{
		}

		// Token: 0x040001D7 RID: 471
		public const int Index = 77;
	}
}
