using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004E RID: 78
	public abstract class CubeBlock : Block
	{
		// Token: 0x06000183 RID: 387 RVA: 0x00009AB7 File Offset: 0x00007CB7
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00009AD2 File Offset: 0x00007CD2
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
}
