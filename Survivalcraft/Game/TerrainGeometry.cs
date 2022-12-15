using System;

namespace Game
{
	// Token: 0x02000314 RID: 788
	public class TerrainGeometry
	{
		// Token: 0x04001049 RID: 4169
		public TerrainGeometrySubset SubsetOpaque;

		// Token: 0x0400104A RID: 4170
		public TerrainGeometrySubset SubsetAlphaTest;

		// Token: 0x0400104B RID: 4171
		public TerrainGeometrySubset SubsetTransparent;

		// Token: 0x0400104C RID: 4172
		public TerrainGeometrySubset[] OpaqueSubsetsByFace;

		// Token: 0x0400104D RID: 4173
		public TerrainGeometrySubset[] AlphaTestSubsetsByFace;

		// Token: 0x0400104E RID: 4174
		public TerrainGeometrySubset[] TransparentSubsetsByFace;
	}
}
