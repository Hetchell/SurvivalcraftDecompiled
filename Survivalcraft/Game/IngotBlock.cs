using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000084 RID: 132
	public abstract class IngotBlock : Block
	{
		// Token: 0x060002CB RID: 715 RVA: 0x00010704 File Offset: 0x0000E904
		public IngotBlock(string meshName)
		{
			this.m_meshName = meshName;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00010720 File Offset: 0x0000E920
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Ingots");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(this.m_meshName, true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(this.m_meshName, true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0001079F File Offset: 0x0000E99F
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000107A1 File Offset: 0x0000E9A1
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000141 RID: 321
		public string m_meshName;

		// Token: 0x04000142 RID: 322
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
