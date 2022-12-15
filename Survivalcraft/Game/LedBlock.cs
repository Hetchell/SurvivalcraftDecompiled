using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000A0 RID: 160
	public class LedBlock : MountedElectricElementBlock
	{
		// Token: 0x0600030B RID: 779 RVA: 0x00011A0C File Offset: 0x0000FC0C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leds");
			ModelMesh modelMesh = model.FindMesh("Led", true);
			ModelMesh modelMesh2 = model.FindMesh("LedBulb", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(modelMesh2.ParentBone);
			for (int i = 0; i < 8; i++)
			{
				Color color = LedBlock.LedColors[i];
				color *= 0.5f;
				color.A = byte.MaxValue;
				Matrix m = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
				this.m_standaloneBlockMeshesByColor[i] = new BlockMesh();
				this.m_standaloneBlockMeshesByColor[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_standaloneBlockMeshesByColor[i].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, color);
				for (int j = 0; j < 6; j++)
				{
					int num = LedBlock.SetMountingFace(LedBlock.SetColor(0, i), j);
					Matrix m2 = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m2, false, false, false, false, color);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00011C66 File Offset: 0x0000FE66
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = LanguageControl.Get(base.GetType().Name, 1)
				};
				craftingRecipe.Ingredients[1] = "glass";
				craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
				craftingRecipe.Ingredients[6] = "copperingot";
				craftingRecipe.Ingredients[7] = "copperingot";
				craftingRecipe.Ingredients[8] = "copperingot";
				yield return craftingRecipe;
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00011C76 File Offset: 0x0000FE76
		public override int GetFace(int value)
		{
			return LedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00011C84 File Offset: 0x0000FE84
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = LedBlock.GetColor(data);
			return LanguageControl.Get("LedBlock", color) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00011CD5 File Offset: 0x0000FED5
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00011CE0 File Offset: 0x0000FEE0
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = LedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00011D2C File Offset: 0x0000FF2C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = LedBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00011D7C File Offset: 0x0000FF7C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00011DA8 File Offset: 0x0000FFA8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00011E14 File Offset: 0x00010014
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int color2 = LedBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesByColor[color2], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00011E48 File Offset: 0x00010048
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new LedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00011E64 File Offset: 0x00010064
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00011EA0 File Offset: 0x000100A0
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00011EA5 File Offset: 0x000100A5
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00011EAF File Offset: 0x000100AF
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00011EB6 File Offset: 0x000100B6
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x04000167 RID: 359
		public const int Index = 152;

		// Token: 0x04000168 RID: 360
		public static readonly Color[] LedColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(0, 255, 255),
			new Color(255, 0, 0),
			new Color(0, 0, 255),
			new Color(255, 240, 0),
			new Color(0, 255, 0),
			new Color(255, 120, 0),
			new Color(255, 0, 255)
		};

		// Token: 0x04000169 RID: 361
		public BlockMesh[] m_standaloneBlockMeshesByColor = new BlockMesh[8];

		// Token: 0x0400016A RID: 362
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[64];

		// Token: 0x0400016B RID: 363
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[64][];
	}
}
