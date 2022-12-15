using System;
using Engine;

namespace Game
{
	// Token: 0x02000091 RID: 145
	public class IronOreChunkBlock : ChunkBlock
	{
		// Token: 0x060002DC RID: 732 RVA: 0x000108B4 File Offset: 0x0000EAB4
		public IronOreChunkBlock() : base(Matrix.CreateRotationX(0f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(136, 74, 36), false)
		{
		}

		// Token: 0x0400014E RID: 334
		public const int Index = 249;
	}
}
