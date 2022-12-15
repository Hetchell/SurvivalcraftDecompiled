using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000126 RID: 294
	public class WoodenClubBlock : Block
	{
		// Token: 0x0600058F RID: 1423 RVA: 0x0001E574 File Offset: 0x0001C774
		public override void Initialize()
		{
			int num = 47;
			Model model = ContentManager.Get<Model>("Models/WoodenClub");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Handle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			base.Initialize();
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x0001E629 File Offset: 0x0001C829
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x0001E62B File Offset: 0x0001C82B
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000270 RID: 624
		public const int Index = 122;

		// Token: 0x04000271 RID: 625
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
