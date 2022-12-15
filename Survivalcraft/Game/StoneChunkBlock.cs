using System;
using Engine;

namespace Game
{
	// Token: 0x02000104 RID: 260
	public class StoneChunkBlock : ChunkBlock
	{
		// Token: 0x06000502 RID: 1282 RVA: 0x0001B6F0 File Offset: 0x000198F0
		public StoneChunkBlock() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(0f) * Matrix.CreateRotationZ(1f), Matrix.CreateScale(0.75f) * Matrix.CreateTranslation(0.1875f, 0.0625f, 0f), new Color(255, 255, 255), true)
		{
		}

		// Token: 0x04000237 RID: 567
		public const int Index = 79;
	}
}
