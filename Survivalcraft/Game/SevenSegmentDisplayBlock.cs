using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EE RID: 238
	public class SevenSegmentDisplayBlock : MountedElectricElementBlock
	{
		// Token: 0x0600047A RID: 1146 RVA: 0x00018188 File Offset: 0x00016388
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Gates").FindMesh("SevenSegmentDisplay", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 4; i++)
			{
				Matrix m = Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
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

		// Token: 0x0600047B RID: 1147 RVA: 0x000182CC File Offset: 0x000164CC
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = "用铜玻璃和颜料来制作7段显示器"
				};
				craftingRecipe.Ingredients[0] = "glass";
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

		// Token: 0x0600047C RID: 1148 RVA: 0x000182D8 File Offset: 0x000164D8
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x000182FD File Offset: 0x000164FD
		public override int GetFace(int value)
		{
			return SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001830C File Offset: 0x0001650C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = SevenSegmentDisplayBlock.GetColor(data);
			return LanguageControl.Get("WorldPalette", color) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001835D File Offset: 0x0001655D
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00018368 File Offset: 0x00016568
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			if (raycastResult.CellFace.Face < 4)
			{
				int data = SevenSegmentDisplayBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
				int value2 = Terrain.ReplaceData(value, data);
				return new BlockPlacementData
				{
					Value = value2,
					CellFace = raycastResult.CellFace
				};
			}
			return default(BlockPlacementData);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x000183D0 File Offset: 0x000165D0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = SevenSegmentDisplayBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00018420 File Offset: 0x00016620
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00018450 File Offset: 0x00016650
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x000184BB File Offset: 0x000166BB
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x000184D6 File Offset: 0x000166D6
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SevenSegmentDisplayElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000184F0 File Offset: 0x000166F0
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001852C File Offset: 0x0001672C
		public static int GetMountingFace(int data)
		{
			return data & 3;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00018531 File Offset: 0x00016731
		public static int SetMountingFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001853B File Offset: 0x0001673B
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00018542 File Offset: 0x00016742
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x04000202 RID: 514
		public const int Index = 185;

		// Token: 0x04000203 RID: 515
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x04000204 RID: 516
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[4];

		// Token: 0x04000205 RID: 517
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[4][];
	}
}
