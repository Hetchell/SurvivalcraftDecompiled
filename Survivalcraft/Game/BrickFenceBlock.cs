using System;
using Engine;

namespace Game
{
	// Token: 0x02000024 RID: 36
	public class BrickFenceBlock : FenceBlock
	{
		// Token: 0x060000FD RID: 253 RVA: 0x00007705 File Offset: 0x00005905
		public BrickFenceBlock() : base("Models/StoneFence", false, false, 39, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00007730 File Offset: 0x00005930
		public override bool ShouldConnectTo(int value)
		{
			int num = Terrain.ExtractContents(value);
			return !BlocksManager.Blocks[num].IsTransparent || base.ShouldConnectTo(value);
		}

		// Token: 0x04000082 RID: 130
		public const int Index = 164;
	}
}
