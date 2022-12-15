using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000023 RID: 35
	public class BrickBlock : Block
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x00007658 File Offset: 0x00005858
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Brick");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Brick", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Brick", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.075f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000076D5 File Offset: 0x000058D5
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000076D7 File Offset: 0x000058D7
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2.5f * size, ref matrix, environmentData);
		}

		// Token: 0x04000080 RID: 128
		public const int Index = 74;

		// Token: 0x04000081 RID: 129
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
