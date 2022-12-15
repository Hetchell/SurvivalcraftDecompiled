using System;
using Engine;

namespace Game
{
	// Token: 0x020000A5 RID: 165
	public class MagmaBlock : FluidBlock
	{
		// Token: 0x0600033B RID: 827 RVA: 0x0001290F File Offset: 0x00010B0F
		public MagmaBlock() : base(MagmaBlock.MaxLevel)
		{
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001291C File Offset: 0x00010B1C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return FluidBlock.GetIsTop(Terrain.ExtractData(value)) && face != 5;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00012934 File Offset: 0x00010B34
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			base.GenerateFluidTerrainVertices(generator, value, x, y, z, Color.White, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001295F File Offset: 0x00010B5F
		public override bool ShouldAvoid(int value)
		{
			return true;
		}

		// Token: 0x0400017A RID: 378
		public const int Index = 92;

		// Token: 0x0400017B RID: 379
		public new static int MaxLevel = 4;
	}
}
