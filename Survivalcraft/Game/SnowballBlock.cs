using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F4 RID: 244
	public class SnowballBlock : Block
	{
		// Token: 0x060004A7 RID: 1191 RVA: 0x00018E90 File Offset: 0x00017090
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Snowball");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Snowball", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Snowball", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00018F0D File Offset: 0x0001710D
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00018F0F File Offset: 0x0001710F
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2.5f * size, ref matrix, environmentData);
		}

		// Token: 0x04000212 RID: 530
		public const int Index = 85;

		// Token: 0x04000213 RID: 531
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
