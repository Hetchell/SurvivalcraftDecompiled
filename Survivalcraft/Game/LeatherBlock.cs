using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009E RID: 158
	public class LeatherBlock : Block
	{
		// Token: 0x06000302 RID: 770 RVA: 0x000117CC File Offset: 0x0000F9CC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leather");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Leather", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leather", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leather", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, true, false, false, new Color(128, 128, 160));
			base.Initialize();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x000118A0 File Offset: 0x0000FAA0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000304 RID: 772 RVA: 0x000118A2 File Offset: 0x0000FAA2
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000163 RID: 355
		public const int Index = 159;

		// Token: 0x04000164 RID: 356
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
