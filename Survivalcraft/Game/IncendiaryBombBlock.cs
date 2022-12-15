using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000083 RID: 131
	public class IncendiaryBombBlock : Block
	{
		// Token: 0x060002C7 RID: 711 RVA: 0x00010648 File Offset: 0x0000E848
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bomb");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bomb", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bomb", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, new Color(0.4f, 0.2f, 0.2f));
			base.Initialize();
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x000106D4 File Offset: 0x0000E8D4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x000106D6 File Offset: 0x0000E8D6
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400013F RID: 319
		public const int Index = 228;

		// Token: 0x04000140 RID: 320
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
