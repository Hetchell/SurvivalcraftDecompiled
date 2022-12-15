using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C6 RID: 198
	public class PigmentBlock : FlatBlock
	{
		// Token: 0x060003CA RID: 970 RVA: 0x00014C3C File Offset: 0x00012E3C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int value2 = Terrain.ExtractData(value);
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * SubsystemPalette.GetColor(environmentData, new int?(value2)), false, environmentData);
		}

		// Token: 0x040001B6 RID: 438
		public const int Index = 130;
	}
}
