using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200030D RID: 781
	public class TerrainChunkGeometry : IDisposable
	{
		// Token: 0x06001644 RID: 5700 RVA: 0x000A798C File Offset: 0x000A5B8C
		public TerrainChunkGeometry()
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i] = new TerrainChunkSliceGeometry();
			}
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000A79D8 File Offset: 0x000A5BD8
		public void Dispose()
		{
			foreach (TerrainChunkGeometry.Buffer buffer in this.Buffers)
			{
				buffer.Dispose();
			}
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x000A7A28 File Offset: 0x000A5C28
		public void InvalidateSliceContentsHashes()
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i].ContentsHash = 0;
			}
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x000A7A58 File Offset: 0x000A5C58
		public void CopySliceContentsHashes(TerrainChunk chunk)
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i].ContentsHash = chunk.SliceContentsHashes[i];
			}
		}

		// Token: 0x04000FBA RID: 4026
		public const int SubsetsCount = 7;

		// Token: 0x04000FBB RID: 4027
		public TerrainChunkSliceGeometry[] Slices = new TerrainChunkSliceGeometry[16];

		// Token: 0x04000FBC RID: 4028
		public DynamicArray<TerrainChunkGeometry.Buffer> Buffers = new DynamicArray<TerrainChunkGeometry.Buffer>();

		// Token: 0x020004EB RID: 1259
		public class Buffer : IDisposable
		{
			// Token: 0x0600206D RID: 8301 RVA: 0x000E444F File Offset: 0x000E264F
			public void Dispose()
			{
				Utilities.Dispose<VertexBuffer>(ref this.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.IndexBuffer);
			}

			// Token: 0x04001803 RID: 6147
			public VertexBuffer VertexBuffer;

			// Token: 0x04001804 RID: 6148
			public IndexBuffer IndexBuffer;

			// Token: 0x04001805 RID: 6149
			public int[] SubsetIndexBufferStarts = new int[7];

			// Token: 0x04001806 RID: 6150
			public int[] SubsetIndexBufferEnds = new int[7];
		}
	}
}
