using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FF RID: 255
	public class StickBlock : Block
	{
		// Token: 0x060004F7 RID: 1271 RVA: 0x0001B498 File Offset: 0x00019698
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Stick");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Stick", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Stick", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001B515 File Offset: 0x00019715
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001B517 File Offset: 0x00019717
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000230 RID: 560
		public const int Index = 23;

		// Token: 0x04000231 RID: 561
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
