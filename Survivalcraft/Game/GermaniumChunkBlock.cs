using System;
using Engine;

namespace Game
{
	// Token: 0x02000072 RID: 114
	public class GermaniumChunkBlock : ChunkBlock
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0000F25C File Offset: 0x0000D45C
		public GermaniumChunkBlock() : base(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.875f, 0.25f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x0400011E RID: 286
		public const int Index = 149;
	}
}
