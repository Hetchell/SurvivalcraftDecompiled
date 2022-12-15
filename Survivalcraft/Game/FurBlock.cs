using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006F RID: 111
	public class FurBlock : Block
	{
		// Token: 0x0600025C RID: 604 RVA: 0x0000DFB4 File Offset: 0x0000C1B4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Fur");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fur", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Fur", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Fur", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, true, false, false, new Color(128, 128, 160));
			base.Initialize();
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000E088 File Offset: 0x0000C288
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000E08A File Offset: 0x0000C28A
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000115 RID: 277
		public const int Index = 207;

		// Token: 0x04000116 RID: 278
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
