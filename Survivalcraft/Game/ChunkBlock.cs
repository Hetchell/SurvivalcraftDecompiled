using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000033 RID: 51
	public abstract class ChunkBlock : Block
	{
		// Token: 0x06000144 RID: 324 RVA: 0x00008CBE File Offset: 0x00006EBE
		public ChunkBlock(Matrix transform, Matrix tcTransform, Color color, bool smooth)
		{
			this.m_transform = transform;
			this.m_tcTransform = tcTransform;
			this.m_color = color;
			this.m_smooth = smooth;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00008CF0 File Offset: 0x00006EF0
		public override void Initialize()
		{
			Model model = this.m_smooth ? ContentManager.Get<Model>("Models/ChunkSmooth") : ContentManager.Get<Model>("Models/Chunk");
			Matrix matrix = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * this.m_transform;
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], matrix, false, false, false, false, this.m_color);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(this.m_tcTransform, -1);
			base.Initialize();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00008D8C File Offset: 0x00006F8C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00008D8E File Offset: 0x00006F8E
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000A3 RID: 163
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000A4 RID: 164
		public Matrix m_transform;

		// Token: 0x040000A5 RID: 165
		public Matrix m_tcTransform;

		// Token: 0x040000A6 RID: 166
		public Color m_color;

		// Token: 0x040000A7 RID: 167
		public bool m_smooth;
	}
}
