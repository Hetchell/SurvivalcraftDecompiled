using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000068 RID: 104
	public abstract class FlatBlock : Block
	{
		// Token: 0x06000227 RID: 551 RVA: 0x0000D510 File Offset: 0x0000B710
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000D512 File Offset: 0x0000B712
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
