using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000BF RID: 191
	public class OneLedBlock : MountedElectricElementBlock
	{
		// Token: 0x0600038E RID: 910 RVA: 0x00013CA4 File Offset: 0x00011EA4
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds").FindMesh("OneLed", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByFace[i] = new BlockMesh();
				this.m_blockMeshesByFace[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByFace[i] = new BoundingBox[]
				{
					this.m_blockMeshesByFace[i].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00013E2B File Offset: 0x0001202B
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			CraftingRecipe craftingRecipe = new CraftingRecipe
			{
				ResultCount = 4,
				ResultValue = Terrain.MakeBlockValue(253, 0, 0),
				RequiredHeatLevel = 0f,
				Description = LanguageControl.Get(base.GetType().Name, 1)
			};
			craftingRecipe.Ingredients[0] = "glass";
			craftingRecipe.Ingredients[1] = "glass";
			craftingRecipe.Ingredients[2] = "glass";
			craftingRecipe.Ingredients[4] = "wire";
			craftingRecipe.Ingredients[6] = "copperingot";
			craftingRecipe.Ingredients[7] = "copperingot";
			craftingRecipe.Ingredients[8] = "copperingot";
			yield return craftingRecipe;
			yield break;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00013E3C File Offset: 0x0001203C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00013E61 File Offset: 0x00012061
		public override int GetFace(int value)
		{
			return OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00013E70 File Offset: 0x00012070
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = OneLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00013EBC File Offset: 0x000120BC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(253, 0, 0),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00013EF8 File Offset: 0x000120F8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00013F28 File Offset: 0x00012128
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00013F93 File Offset: 0x00012193
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00013FAE File Offset: 0x000121AE
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new OneLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00013FC8 File Offset: 0x000121C8
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00014004 File Offset: 0x00012204
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00014009 File Offset: 0x00012209
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x040001A4 RID: 420
		public const int Index = 253;

		// Token: 0x040001A5 RID: 421
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x040001A6 RID: 422
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];

		// Token: 0x040001A7 RID: 423
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
	}
}
