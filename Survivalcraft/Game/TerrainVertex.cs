using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200031C RID: 796
	public struct TerrainVertex
	{
		// Token: 0x0400109D RID: 4253
		public float X;

		// Token: 0x0400109E RID: 4254
		public float Y;

		// Token: 0x0400109F RID: 4255
		public float Z;

		// Token: 0x040010A0 RID: 4256
		public short Tx;

		// Token: 0x040010A1 RID: 4257
		public short Ty;

		// Token: 0x040010A2 RID: 4258
		public Color Color;

		// Token: 0x040010A3 RID: 4259
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.NormalizedShort2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(16, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});
	}
}
