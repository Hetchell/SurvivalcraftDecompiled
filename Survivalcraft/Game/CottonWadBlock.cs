using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000049 RID: 73
	public class CottonWadBlock : FlatBlock
	{
		// Token: 0x0600016C RID: 364 RVA: 0x000093CB File Offset: 0x000075CB
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x040000BF RID: 191
		public const int Index = 205;
	}
}
