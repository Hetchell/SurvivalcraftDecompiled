using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200000E RID: 14
	public class AirBlock : Block
	{
		// Token: 0x0600008B RID: 139 RVA: 0x000056C5 File Offset: 0x000038C5
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000056C7 File Offset: 0x000038C7
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0400004C RID: 76
		public const int Index = 0;
	}
}
