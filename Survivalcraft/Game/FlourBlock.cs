using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000069 RID: 105
	public class FlourBlock : FlatBlock
	{
		// Token: 0x0600022A RID: 554 RVA: 0x0000D52C File Offset: 0x0000B72C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * new Color(248, 255, 232), false, environmentData);
		}

		// Token: 0x04000105 RID: 261
		public const int Index = 175;
	}
}
