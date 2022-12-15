using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000114 RID: 276
	public class TreasureGeneratorBlock : Block
	{
		// Token: 0x0600054D RID: 1357 RVA: 0x0001D682 File Offset: 0x0001B882
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001D684 File Offset: 0x0001B884
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x04000257 RID: 599
		public const int Index = 190;
	}
}
