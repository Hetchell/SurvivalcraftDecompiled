using System;

namespace Game
{
	// Token: 0x0200030E RID: 782
	public class TerrainChunkSliceGeometry : TerrainGeometry
	{
		// Token: 0x06001648 RID: 5704 RVA: 0x000A7A90 File Offset: 0x000A5C90
		public TerrainChunkSliceGeometry()
		{
			this.Subsets = new TerrainGeometrySubset[7];
			for (int i = 0; i < this.Subsets.Length; i++)
			{
				this.Subsets[i] = new TerrainGeometrySubset();
			}
			this.SubsetOpaque = this.Subsets[4];
			this.SubsetAlphaTest = this.Subsets[5];
			this.SubsetTransparent = this.Subsets[6];
			this.OpaqueSubsetsByFace = new TerrainGeometrySubset[]
			{
				this.Subsets[0],
				this.Subsets[1],
				this.Subsets[2],
				this.Subsets[3],
				this.Subsets[4],
				this.Subsets[4]
			};
			this.AlphaTestSubsetsByFace = new TerrainGeometrySubset[]
			{
				this.Subsets[5],
				this.Subsets[5],
				this.Subsets[5],
				this.Subsets[5],
				this.Subsets[5],
				this.Subsets[5]
			};
			this.TransparentSubsetsByFace = new TerrainGeometrySubset[]
			{
				this.Subsets[6],
				this.Subsets[6],
				this.Subsets[6],
				this.Subsets[6],
				this.Subsets[6],
				this.Subsets[6]
			};
		}

		// Token: 0x04000FBD RID: 4029
		public const int OpaqueFace0Index = 0;

		// Token: 0x04000FBE RID: 4030
		public const int OpaqueFace1Index = 1;

		// Token: 0x04000FBF RID: 4031
		public const int OpaqueFace2Index = 2;

		// Token: 0x04000FC0 RID: 4032
		public const int OpaqueFace3Index = 3;

		// Token: 0x04000FC1 RID: 4033
		public const int OpaqueIndex = 4;

		// Token: 0x04000FC2 RID: 4034
		public const int AlphaTestIndex = 5;

		// Token: 0x04000FC3 RID: 4035
		public const int TransparentIndex = 6;

		// Token: 0x04000FC4 RID: 4036
		public TerrainGeometrySubset[] Subsets = new TerrainGeometrySubset[7];

		// Token: 0x04000FC5 RID: 4037
		public int ContentsHash;
	}
}
