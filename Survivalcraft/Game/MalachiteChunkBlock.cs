using System;
using Engine;

namespace Game
{
	// Token: 0x020000A9 RID: 169
	public class MalachiteChunkBlock : ChunkBlock
	{
		// Token: 0x0600034A RID: 842 RVA: 0x00012D58 File Offset: 0x00010F58
		public MalachiteChunkBlock() : base(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(0.1875f, 0.6875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x04000183 RID: 387
		public const int Index = 43;
	}
}
