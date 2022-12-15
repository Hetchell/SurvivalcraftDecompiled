using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000119 RID: 281
	public class WhistleBlock : Block
	{
		// Token: 0x0600055B RID: 1371 RVA: 0x0001D9F4 File Offset: 0x0001BBF4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Whistle");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Whistle", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), false, false, false, false, new Color(255, 255, 255));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), false, true, false, false, new Color(64, 64, 64));
			base.Initialize();
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001DACE File Offset: 0x0001BCCE
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001DAD0 File Offset: 0x0001BCD0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 9f * size, ref matrix, environmentData);
		}

		// Token: 0x0400025D RID: 605
		public const int Index = 160;

		// Token: 0x0400025E RID: 606
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
