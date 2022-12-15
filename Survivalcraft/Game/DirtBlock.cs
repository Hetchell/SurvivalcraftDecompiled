using System;
using Engine;

namespace Game
{
	// Token: 0x0200005B RID: 91
	public class DirtBlock : CubeBlock
	{
		// Token: 0x060001A1 RID: 417 RVA: 0x0000A0DA File Offset: 0x000082DA
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x040000D6 RID: 214
		public const int Index = 2;
	}
}
