using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200002B RID: 43
	public class CactusBlock : Block
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00007C64 File Offset: 0x00005E64
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Cactus");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Cactus", true).ParentBone);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("Cactus", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Cactus", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007D2C File Offset: 0x00005F2C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetAlphaTest);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007D60 File Offset: 0x00005F60
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007D75 File Offset: 0x00005F75
		public override bool ShouldAvoid(int value)
		{
			return true;
		}

		// Token: 0x04000090 RID: 144
		public const int Index = 127;

		// Token: 0x04000091 RID: 145
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000092 RID: 146
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
