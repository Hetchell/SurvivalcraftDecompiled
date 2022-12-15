using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006C RID: 108
	public abstract class FoodBlock : Block
	{
		// Token: 0x06000243 RID: 579 RVA: 0x0000DAA4 File Offset: 0x0000BCA4
		public FoodBlock(string modelName, Matrix tcTransform, Color color, int rottenValue)
		{
			this.m_modelName = modelName;
			this.m_tcTransform = tcTransform;
			this.m_color = color;
			this.m_rottenValue = rottenValue;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000DAD4 File Offset: 0x0000BCD4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], boneAbsoluteTransform, false, false, false, false, this.m_color);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(this.m_tcTransform, -1);
			base.Initialize();
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000DB52 File Offset: 0x0000BD52
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000DB54 File Offset: 0x0000BD54
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000DB6F File Offset: 0x0000BD6F
		public override int GetDamageDestructionValue(int value)
		{
			return this.m_rottenValue;
		}

		// Token: 0x0400010A RID: 266
		public static int m_compostValue = Terrain.MakeBlockValue(168, 0, SoilBlock.SetHydration(SoilBlock.SetNitrogen(0, 1), false));

		// Token: 0x0400010B RID: 267
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400010C RID: 268
		public string m_modelName;

		// Token: 0x0400010D RID: 269
		public Matrix m_tcTransform;

		// Token: 0x0400010E RID: 270
		public Color m_color;

		// Token: 0x0400010F RID: 271
		public int m_rottenValue;
	}
}
