using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006D RID: 109
	public class FourLedBlock : MountedElectricElementBlock
	{
		// Token: 0x06000249 RID: 585 RVA: 0x0000DB98 File Offset: 0x0000BD98
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds").FindMesh("FourLed", true);
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

		// Token: 0x0600024A RID: 586 RVA: 0x0000DD1F File Offset: 0x0000BF1F
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = LanguageControl.Get(base.GetType().Name, 1)
				};
				craftingRecipe.Ingredients[0] = "glass";
				craftingRecipe.Ingredients[1] = "glass";
				craftingRecipe.Ingredients[2] = "glass";
				craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
				craftingRecipe.Ingredients[6] = "copperingot";
				craftingRecipe.Ingredients[7] = "copperingot";
				craftingRecipe.Ingredients[8] = "copperingot";
				yield return craftingRecipe;
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000DD30 File Offset: 0x0000BF30
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000DD55 File Offset: 0x0000BF55
		public override int GetFace(int value)
		{
			return FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000DD64 File Offset: 0x0000BF64
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = FourLedBlock.GetColor(data);
			return LanguageControl.Get("LedBlock", color) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000DDB5 File Offset: 0x0000BFB5
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000DDC0 File Offset: 0x0000BFC0
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = FourLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000DE0C File Offset: 0x0000C00C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = FourLedBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000DE5C File Offset: 0x0000C05C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000DE8C File Offset: 0x0000C08C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000DEF7 File Offset: 0x0000C0F7
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000DF12 File Offset: 0x0000C112
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new FourLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000DF2C File Offset: 0x0000C12C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000DF68 File Offset: 0x0000C168
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000DF6F File Offset: 0x0000C16F
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000DF7B File Offset: 0x0000C17B
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000DF80 File Offset: 0x0000C180
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x04000110 RID: 272
		public const int Index = 182;

		// Token: 0x04000111 RID: 273
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x04000112 RID: 274
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];

		// Token: 0x04000113 RID: 275
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
	}
}
