using System;
using Engine;

namespace Game
{
	// Token: 0x020000D2 RID: 210
	public class RawFishBlock : FoodBlock
	{
		// Token: 0x0600042D RID: 1069 RVA: 0x00016A12 File Offset: 0x00014C12
		public RawFishBlock() : base("Models/Fish", Matrix.Identity, Color.White, 241)
		{
		}

		// Token: 0x040001D8 RID: 472
		public const int Index = 161;
	}
}
