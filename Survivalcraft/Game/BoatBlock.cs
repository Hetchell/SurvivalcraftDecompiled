using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200001E RID: 30
	public class BoatBlock : Block
	{
		// Token: 0x060000DF RID: 223 RVA: 0x00006F9C File Offset: 0x0000519C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/BoatItem");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Boat", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Boat", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, false, false, false, new Color(96, 96, 96));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Boat", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, true, false, false, new Color(255, 255, 255));
			base.Initialize();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00007076 File Offset: 0x00005276
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007078 File Offset: 0x00005278
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x04000079 RID: 121
		public const int Index = 178;

		// Token: 0x0400007A RID: 122
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
