using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000053 RID: 83
	public class DiamondChunkBlock : Block
	{
		// Token: 0x06000194 RID: 404 RVA: 0x00009EE4 File Offset: 0x000080E4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Diamond");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Diamond", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Diamond", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00009F61 File Offset: 0x00008161
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00009F63 File Offset: 0x00008163
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000CD RID: 205
		public const int Index = 111;

		// Token: 0x040000CE RID: 206
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
