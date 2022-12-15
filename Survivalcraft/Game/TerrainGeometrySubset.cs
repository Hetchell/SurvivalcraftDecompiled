using System;
using Engine;

namespace Game
{
	// Token: 0x02000315 RID: 789
	public class TerrainGeometrySubset
	{
		// Token: 0x060016A2 RID: 5794 RVA: 0x000B3705 File Offset: 0x000B1905
		public TerrainGeometrySubset()
		{
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x000B3723 File Offset: 0x000B1923
		public TerrainGeometrySubset(DynamicArray<TerrainVertex> vertices, DynamicArray<ushort> indices)
		{
			this.Vertices = vertices;
			this.Indices = indices;
		}

		// Token: 0x0400104F RID: 4175
		public DynamicArray<TerrainVertex> Vertices = new DynamicArray<TerrainVertex>();

		// Token: 0x04001050 RID: 4176
		public DynamicArray<ushort> Indices = new DynamicArray<ushort>();
	}
}
