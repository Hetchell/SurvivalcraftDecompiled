using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000112 RID: 274
	public class TorchBlock : Block
	{
		// Token: 0x06000537 RID: 1335 RVA: 0x0001CC40 File Offset: 0x0001AE40
		public override void Initialize()
		{
			for (int i = 0; i < this.m_blockMeshesByVariant.Length; i++)
			{
				this.m_blockMeshesByVariant[i] = new BlockMesh();
			}
			Model model = ContentManager.Get<Model>("Models/Torch");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Torch", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Flame", true).ParentBone);
			Matrix m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(0.5f, 0.15f, -0.05f);
			this.m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(1.5707964f) * Matrix.CreateTranslation(-0.05f, 0.15f, 0.5f);
			this.m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(3.1415927f) * Matrix.CreateTranslation(0.5f, 0.15f, 1.05f);
			this.m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(4.712389f) * Matrix.CreateTranslation(1.05f, 0.15f, 0.5f);
			this.m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateTranslation(0.5f, 0f, 0.5f);
			this.m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, Color.White);
			for (int j = 0; j < 5; j++)
			{
				this.m_collisionBoxes[j] = new BoundingBox[]
				{
					this.m_blockMeshesByVariant[j].CalculateBoundingBox()
				};
			}
			base.Initialize();
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001D040 File Offset: 0x0001B240
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByVariant.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByVariant[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001D088 File Offset: 0x0001B288
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 3);
			}
			if (raycastResult.CellFace.Face == 4)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 4);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001D14C File Offset: 0x0001B34C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxes.Length)
			{
				return this.m_collisionBoxes[num];
			}
			return null;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001D175 File Offset: 0x0001B375
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400024F RID: 591
		public const int Index = 31;

		// Token: 0x04000250 RID: 592
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000251 RID: 593
		public BlockMesh[] m_blockMeshesByVariant = new BlockMesh[5];

		// Token: 0x04000252 RID: 594
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[5][];
	}
}
