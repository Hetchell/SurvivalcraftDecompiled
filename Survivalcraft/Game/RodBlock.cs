using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000D6 RID: 214
	public class RodBlock : Block
	{
		// Token: 0x06000433 RID: 1075 RVA: 0x00016B44 File Offset: 0x00014D44
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Rod");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronRod", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00016BC1 File Offset: 0x00014DC1
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00016BC3 File Offset: 0x00014DC3
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001DC RID: 476
		public const int Index = 195;

		// Token: 0x040001DD RID: 477
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
