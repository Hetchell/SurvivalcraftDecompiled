using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200001F RID: 31
	public class BombBlock : Block
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x000070A8 File Offset: 0x000052A8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bomb");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bomb", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bomb", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, new Color(0.3f, 0.3f, 0.3f));
			base.Initialize();
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00007134 File Offset: 0x00005334
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00007136 File Offset: 0x00005336
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400007B RID: 123
		public const int Index = 201;

		// Token: 0x0400007C RID: 124
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
