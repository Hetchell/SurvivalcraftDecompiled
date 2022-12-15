using System;
using Engine;

namespace Game
{
	// Token: 0x02000106 RID: 262
	public class StoneFenceBlock : FenceBlock
	{
		// Token: 0x06000507 RID: 1287 RVA: 0x0001B8E9 File Offset: 0x00019AE9
		public StoneFenceBlock() : base("Models/StoneFence", false, false, 24, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001B914 File Offset: 0x00019B14
		public override bool ShouldConnectTo(int value)
		{
			int num = Terrain.ExtractContents(value);
			return !BlocksManager.Blocks[num].IsTransparent || base.ShouldConnectTo(value);
		}

		// Token: 0x0400023A RID: 570
		public const int Index = 202;
	}
}
