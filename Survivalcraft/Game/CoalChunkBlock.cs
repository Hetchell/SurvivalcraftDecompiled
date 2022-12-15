using System;
using Engine;

namespace Game
{
	// Token: 0x02000036 RID: 54
	public class CoalChunkBlock : ChunkBlock
	{
		// Token: 0x0600014A RID: 330 RVA: 0x00008DBC File Offset: 0x00006FBC
		public CoalChunkBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.875f, 0.1875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x040000AA RID: 170
		public const int Index = 22;
	}
}
