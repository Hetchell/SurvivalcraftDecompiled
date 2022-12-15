using System;
using Engine;

namespace Game
{
	// Token: 0x0200008A RID: 138
	public class IronFenceBlock : FenceBlock
	{
		// Token: 0x060002D5 RID: 725 RVA: 0x000107F8 File Offset: 0x0000E9F8
		public IronFenceBlock() : base("Models/IronFence", true, true, 58, new Color(192, 192, 192), new Color(80, 80, 80))
		{
		}

		// Token: 0x04000147 RID: 327
		public const int Index = 193;
	}
}
