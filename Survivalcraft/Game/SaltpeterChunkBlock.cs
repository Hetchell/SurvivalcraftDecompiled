using System;
using Engine;

namespace Game
{
	// Token: 0x020000E3 RID: 227
	public class SaltpeterChunkBlock : ChunkBlock
	{
		// Token: 0x0600045D RID: 1117 RVA: 0x00017790 File Offset: 0x00015990
		public SaltpeterChunkBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(0f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x040001F2 RID: 498
		public const int Index = 102;
	}
}
