using System;
using Engine;

namespace Game
{
	// Token: 0x0200007B RID: 123
	public class GunpowderBlock : ChunkBlock
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x0000FC78 File Offset: 0x0000DE78
		public GunpowderBlock() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(3f), Matrix.CreateScale(1f) * Matrix.CreateTranslation(0.0625f, 0.875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x0400012D RID: 301
		public const int Index = 109;
	}
}
