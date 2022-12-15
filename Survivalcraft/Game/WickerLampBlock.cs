using System;

namespace Game
{
	// Token: 0x0200011B RID: 283
	public class WickerLampBlock : AlphaTestCubeBlock
	{
		// Token: 0x06000560 RID: 1376 RVA: 0x0001DB06 File Offset: 0x0001BD06
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 5)
			{
				return this.DefaultTextureSlot;
			}
			return 4;
		}

		// Token: 0x04000260 RID: 608
		public const int Index = 17;
	}
}
