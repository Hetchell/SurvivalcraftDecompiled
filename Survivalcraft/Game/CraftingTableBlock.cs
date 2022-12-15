using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004B RID: 75
	public class CraftingTableBlock : Block
	{
		// Token: 0x06000171 RID: 369 RVA: 0x000094E0 File Offset: 0x000076E0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/CraftingTable");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CraftingTable", true).ParentBone);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000095A8 File Offset: 0x000077A8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, null, geometry.SubsetOpaque);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000095DD File Offset: 0x000077DD
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x040000C1 RID: 193
		public const int Index = 27;

		// Token: 0x040000C2 RID: 194
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x040000C3 RID: 195
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
