using System;
using Engine;

namespace Game
{
	// Token: 0x02000116 RID: 278
	public class WaterBlock : FluidBlock
	{
		// Token: 0x06000553 RID: 1363 RVA: 0x0001D786 File Offset: 0x0001B986
		public WaterBlock() : base(WaterBlock.MaxLevel)
		{
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001D794 File Offset: 0x0001B994
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color sideColor;
			Color color = sideColor = BlockColorsMap.WaterColorsMap.Lookup(generator.Terrain, x, y, z);
			sideColor.A = byte.MaxValue;
			Color topColor = color;
			topColor.A = 0;
			base.GenerateFluidTerrainVertices(generator, value, x, y, z, sideColor, topColor, geometry.TransparentSubsetsByFace);
		}

		// Token: 0x04000259 RID: 601
		public const int Index = 18;

		// Token: 0x0400025A RID: 602
		public new static int MaxLevel = 7;
	}
}
