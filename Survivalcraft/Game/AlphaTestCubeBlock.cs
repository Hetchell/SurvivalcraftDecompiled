using System;
using Engine;

namespace Game
{
	// Token: 0x0200000F RID: 15
	public abstract class AlphaTestCubeBlock : CubeBlock
	{
		// Token: 0x0600008E RID: 142 RVA: 0x000056D1 File Offset: 0x000038D1
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.AlphaTestSubsetsByFace);
		}
	}
}
