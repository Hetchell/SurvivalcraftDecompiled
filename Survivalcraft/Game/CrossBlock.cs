using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004C RID: 76
	public abstract class CrossBlock : Block
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00009610 File Offset: 0x00007810
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000963E File Offset: 0x0000783E
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
