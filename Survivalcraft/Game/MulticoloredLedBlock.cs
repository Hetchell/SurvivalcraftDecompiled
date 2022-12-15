using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B8 RID: 184
	public class MulticoloredLedBlock : MountedElectricElementBlock
	{
		// Token: 0x06000369 RID: 873 RVA: 0x000133B0 File Offset: 0x000115B0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leds");
			ModelMesh modelMesh = model.FindMesh("Led", true);
			ModelMesh modelMesh2 = model.FindMesh("LedBulb", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(modelMesh2.ParentBone);
			Matrix m = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, new Color(48, 48, 48));
			for (int i = 0; i < 6; i++)
			{
				int num = MulticoloredLedBlock.SetMountingFace(0, i);
				Matrix m2 = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
				this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m2, false, false, false, false, new Color(48, 48, 48));
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x000135CE File Offset: 0x000117CE
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			CraftingRecipe craftingRecipe = new CraftingRecipe
			{
				ResultCount = 4,
				ResultValue = Terrain.MakeBlockValue(254, 0, 0),
				RequiredHeatLevel = 0f,
				Description = LanguageControl.Get(base.GetType().Name, 1)
			};
			craftingRecipe.Ingredients[1] = "glass";
			craftingRecipe.Ingredients[4] = "wire";
			craftingRecipe.Ingredients[6] = "copperingot";
			craftingRecipe.Ingredients[7] = "copperingot";
			craftingRecipe.Ingredients[8] = "copperingot";
			yield return craftingRecipe;
			yield break;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x000135DE File Offset: 0x000117DE
		public override int GetFace(int value)
		{
			return MulticoloredLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600036C RID: 876 RVA: 0x000135EB File Offset: 0x000117EB
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(254, 0, 0);
			yield break;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x000135F4 File Offset: 0x000117F4
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = MulticoloredLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00013640 File Offset: 0x00011840
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001366C File Offset: 0x0001186C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000370 RID: 880 RVA: 0x000136D8 File Offset: 0x000118D8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x000136F3 File Offset: 0x000118F3
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MulticoloredLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0001370C File Offset: 0x0001190C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00013748 File Offset: 0x00011948
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001374D File Offset: 0x0001194D
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x04000198 RID: 408
		public const int Index = 254;

		// Token: 0x04000199 RID: 409
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x0400019A RID: 410
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x0400019B RID: 411
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
