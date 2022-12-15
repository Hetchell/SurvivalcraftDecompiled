using System;
using Engine;

namespace Game
{
	// Token: 0x02000074 RID: 116
	public class GermaniumOreChunkBlock : ChunkBlock
	{
		// Token: 0x06000286 RID: 646 RVA: 0x0000F2BC File Offset: 0x0000D4BC
		public GermaniumOreChunkBlock() : base(Matrix.CreateRotationX(-1f) * Matrix.CreateRotationZ(1f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(204, 181, 162), false)
		{
		}

		// Token: 0x04000120 RID: 288
		public const int Index = 250;
	}
}
