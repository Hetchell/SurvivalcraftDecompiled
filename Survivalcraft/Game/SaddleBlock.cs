using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000E2 RID: 226
	public class SaddleBlock : Block
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x00017684 File Offset: 0x00015884
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Saddle");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Saddle", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Saddle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.2f, 0f), false, false, false, false, new Color(224, 224, 224));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Saddle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.2f, 0f), false, true, false, false, new Color(96, 96, 96));
			base.Initialize();
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001775E File Offset: 0x0001595E
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00017760 File Offset: 0x00015960
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001F0 RID: 496
		public const int Index = 158;

		// Token: 0x040001F1 RID: 497
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
