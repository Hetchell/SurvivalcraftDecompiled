using System;
using Engine;

namespace Game
{
	// Token: 0x020000D3 RID: 211
	public class RawMeatBlock : FoodBlock
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x00016A2E File Offset: 0x00014C2E
		public RawMeatBlock() : base("Models/Meat", Matrix.Identity, Color.White, 240)
		{
		}

		// Token: 0x040001D9 RID: 473
		public const int Index = 88;
	}
}
