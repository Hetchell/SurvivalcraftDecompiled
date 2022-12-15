using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009A RID: 154
	public abstract class LadderBlock : Block
	{
		// Token: 0x060002F6 RID: 758 RVA: 0x000114E5 File Offset: 0x0000F6E5
		public LadderBlock(string modelName, float offset)
		{
			this.m_modelName = modelName;
			this.m_offset = offset;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00011520 File Offset: 0x0000F720
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ladder", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix m = Matrix.CreateTranslation(0f, 0f, 0f - (0.5f - this.m_offset)) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Ladder", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_blockMeshesByData[i].GenerateSidesData();
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_blockMeshesByData[i].CalculateBoundingBox()
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Ladder", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00011678 File Offset: 0x0000F878
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x000116C0 File Offset: 0x0000F8C0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x000116D8 File Offset: 0x0000F8D8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = ((raycastResult.CellFace.Face < 4) ? Terrain.MakeBlockValue(this.BlockIndex, 0, LadderBlock.SetFace(0, raycastResult.CellFace.Face)) : 0),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00011734 File Offset: 0x0000F934
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00011764 File Offset: 0x0000F964
		public static int GetFace(int data)
		{
			return data & 3;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00011769 File Offset: 0x0000F969
		public static int SetFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x0400015B RID: 347
		public string m_modelName;

		// Token: 0x0400015C RID: 348
		public float m_offset;

		// Token: 0x0400015D RID: 349
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400015E RID: 350
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x0400015F RID: 351
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];
	}
}
