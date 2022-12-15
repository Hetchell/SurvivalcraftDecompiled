using System;
using Engine;

namespace Game
{
	// Token: 0x0200010B RID: 267
	public class SulphurChunkBlock : ChunkBlock
	{
		// Token: 0x0600050D RID: 1293 RVA: 0x0001B968 File Offset: 0x00019B68
		public SulphurChunkBlock() : base(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(1f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(255, 255, 140), true)
		{
		}

		// Token: 0x0400023F RID: 575
		public const int Index = 103;
	}
}
