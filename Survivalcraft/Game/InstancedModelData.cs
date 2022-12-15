using System;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000297 RID: 663
	public class InstancedModelData
	{
		// Token: 0x04000D43 RID: 3395
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementSemantic.Normal),
			new VertexElement(24, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(32, VertexElementFormat.Single, VertexElementSemantic.Instance)
		});

		// Token: 0x04000D44 RID: 3396
		public VertexBuffer VertexBuffer;

		// Token: 0x04000D45 RID: 3397
		public IndexBuffer IndexBuffer;
	}
}
