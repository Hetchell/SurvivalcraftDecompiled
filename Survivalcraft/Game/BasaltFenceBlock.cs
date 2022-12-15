using System;
using Engine;

namespace Game
{
	// Token: 0x02000016 RID: 22
	public class BasaltFenceBlock : FenceBlock
	{
		// Token: 0x060000BA RID: 186 RVA: 0x000066C5 File Offset: 0x000048C5
		public BasaltFenceBlock() : base("Models/StoneFence", false, false, 40, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000066F0 File Offset: 0x000048F0
		public override bool ShouldConnectTo(int value)
		{
			int num = Terrain.ExtractContents(value);
			return !BlocksManager.Blocks[num].IsTransparent || base.ShouldConnectTo(value);
		}

		// Token: 0x0400006B RID: 107
		public const int Index = 163;
	}
}
