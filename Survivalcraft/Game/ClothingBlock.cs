using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000366 RID: 870
	public class ClothingBlock : Block
	{
		// Token: 0x0600188E RID: 6286 RVA: 0x000C3594 File Offset: 0x000C1794
		public override void Initialize()
		{
			if (ClothingBlock.Initialize1 != null)
			{
				ClothingBlock.Initialize1();
				return;
			}
			int num = 0;
			List<ClothingData> list = new List<ClothingData>();
			foreach (XElement node in ModsManager.CombineXml(ContentManager.Get<XElement>("Clothes"), ModsManager.GetEntries(".clo"), "Index", "Clothes", null).Elements())
			{
				int attributeValue = XmlUtils.GetAttributeValue<int>(node, "Index");
				string text = LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, attributeValue), "Description");
				string text2 = LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, attributeValue), "DisplayName");
				if (string.IsNullOrEmpty(text))
				{
					text = XmlUtils.GetAttributeValue<string>(node, "Description");
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = XmlUtils.GetAttributeValue<string>(node, "DisplayName");
				}
				ClothingData item = new ClothingData
				{
					Index = attributeValue,
					DisplayIndex = num,
					DisplayName = text2,
					Slot = XmlUtils.GetAttributeValue<ClothingSlot>(node, "Slot"),
					ArmorProtection = XmlUtils.GetAttributeValue<float>(node, "ArmorProtection"),
					Sturdiness = XmlUtils.GetAttributeValue<float>(node, "Sturdiness"),
					Insulation = XmlUtils.GetAttributeValue<float>(node, "Insulation"),
					MovementSpeedFactor = XmlUtils.GetAttributeValue<float>(node, "MovementSpeedFactor"),
					SteedMovementSpeedFactor = XmlUtils.GetAttributeValue<float>(node, "SteedMovementSpeedFactor"),
					DensityModifier = XmlUtils.GetAttributeValue<float>(node, "DensityModifier"),
					IsOuter = XmlUtils.GetAttributeValue<bool>(node, "IsOuter"),
					CanBeDyed = XmlUtils.GetAttributeValue<bool>(node, "CanBeDyed"),
					Layer = XmlUtils.GetAttributeValue<int>(node, "Layer"),
					PlayerLevelRequired = XmlUtils.GetAttributeValue<int>(node, "PlayerLevelRequired"),
					Texture = ContentManager.Get<Texture2D>(XmlUtils.GetAttributeValue<string>(node, "TextureName")),
					ImpactSoundsFolder = XmlUtils.GetAttributeValue<string>(node, "ImpactSoundsFolder"),
					Description = text
				};
				num++;
				list.Add(item);
			}
			ClothingBlock.m_clothingData = new ClothingData[list.Count];
			num = 0;
			foreach (ClothingData clothingData in from p in list
			orderby p.Index
			select p)
			{
				clothingData.Index = num;
				ClothingBlock.m_clothingData[num] = clothingData;
				num++;
			}
			Model playerModel = CharacterSkinsManager.GetPlayerModel(PlayerClass.Male);
			Matrix[] array = new Matrix[playerModel.Bones.Count];
			playerModel.CopyAbsoluteBoneTransformsTo(array);
			int index = playerModel.FindBone("Hand1", true).Index;
			int index2 = playerModel.FindBone("Hand2", true).Index;
			array[index] = Matrix.CreateRotationY(0.1f) * array[index];
			array[index2] = Matrix.CreateRotationY(-0.1f) * array[index2];
			this.m_innerMesh = new BlockMesh();
			foreach (ModelMesh modelMesh in playerModel.Meshes)
			{
				Matrix matrix = array[modelMesh.ParentBone.Index];
				foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
				{
					Color color = Color.White * 0.8f;
					color.A = byte.MaxValue;
					this.m_innerMesh.AppendModelMeshPart(meshPart, matrix, false, false, false, false, Color.White);
					this.m_innerMesh.AppendModelMeshPart(meshPart, matrix, false, true, false, true, color);
				}
			}
			Model outerClothingModel = CharacterSkinsManager.GetOuterClothingModel(PlayerClass.Male);
			Matrix[] array2 = new Matrix[outerClothingModel.Bones.Count];
			outerClothingModel.CopyAbsoluteBoneTransformsTo(array2);
			int index3 = outerClothingModel.FindBone("Leg1", true).Index;
			int index4 = outerClothingModel.FindBone("Leg2", true).Index;
			array2[index3] = Matrix.CreateTranslation(-0.02f, 0f, 0f) * array2[index3];
			array2[index4] = Matrix.CreateTranslation(0.02f, 0f, 0f) * array2[index4];
			this.m_outerMesh = new BlockMesh();
			foreach (ModelMesh modelMesh2 in outerClothingModel.Meshes)
			{
				Matrix matrix2 = array2[modelMesh2.ParentBone.Index];
				foreach (ModelMeshPart meshPart2 in modelMesh2.MeshParts)
				{
					Color color2 = Color.White * 0.8f;
					color2.A = byte.MaxValue;
					this.m_outerMesh.AppendModelMeshPart(meshPart2, matrix2, false, false, false, false, Color.White);
					this.m_outerMesh.AppendModelMeshPart(meshPart2, matrix2, false, true, false, true, color2);
				}
			}
			base.Initialize();
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000C3B44 File Offset: 0x000C1D44
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			ClothingData clothingData = ClothingBlock.GetClothingData(data);
			int clothingColor = ClothingBlock.GetClothingColor(data);
			string displayName = clothingData.DisplayName;
			if (clothingColor != 0)
			{
				return SubsystemPalette.GetName(subsystemTerrain, new int?(clothingColor), displayName);
			}
			return displayName;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000C3B80 File Offset: 0x000C1D80
		public override string GetDescription(int value)
		{
			ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
			string text = LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, clothingData.Index), "Description");
			if (string.IsNullOrEmpty(text))
			{
				text = clothingData.Description;
			}
			return text;
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x000C3BD4 File Offset: 0x000C1DD4
		public override string GetCategory(int value)
		{
			if (ClothingBlock.GetClothingColor(Terrain.ExtractData(value)) == 0)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Dyed");
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000C3BFA File Offset: 0x000C1DFA
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 15;
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x000C3C08 File Offset: 0x000C1E08
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num = ((num & -3841) | (damage & 15) << 8);
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x000C3C32 File Offset: 0x000C1E32
		public override IEnumerable<int> GetCreativeValues()
		{
			IEnumerable<ClothingData> enumerable = from cd in ClothingBlock.m_clothingData
			orderby cd.DisplayIndex
			select cd;
			foreach (ClothingData clothingData in enumerable)
			{
				int colorsCount = (!clothingData.CanBeDyed) ? 1 : 16;
				int num;
				for (int color = 0; color < colorsCount; color = num)
				{
					int data = ClothingBlock.SetClothingColor(ClothingBlock.SetClothingIndex(0, clothingData.Index), color);
					yield return Terrain.MakeBlockValue(203, 0, data);
					num = color + 1;
				}

			}

		}

		// Token: 0x06001895 RID: 6293 RVA: 0x000C3C3C File Offset: 0x000C1E3C
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			if (heatLevel < 1f)
			{
				return null;
			}
			List<string> list = (from i in ingredients
			where !string.IsNullOrEmpty(i)
			select i).ToList<string>();
			if (list.Count == 2)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (string ingredient in list)
				{
					string a;
					int? num4;
					CraftingRecipesManager.DecodeIngredient(ingredient, out a, out num4);
					if (a == BlocksManager.Blocks[203].CraftingId)
					{
						num3 = Terrain.MakeBlockValue(203, 0, (num4 != null) ? num4.Value : 0);
					}
					else if (a == BlocksManager.Blocks[129].CraftingId)
					{
						num = Terrain.MakeBlockValue(129, 0, (num4 != null) ? num4.Value : 0);
					}
					else if (a == BlocksManager.Blocks[128].CraftingId)
					{
						num2 = Terrain.MakeBlockValue(128, 0, (num4 != null) ? num4.Value : 0);
					}
				}
				if (num != 0 && num3 != 0)
				{
					int data = Terrain.ExtractData(num3);
					int clothingColor = ClothingBlock.GetClothingColor(data);
					int clothingIndex = ClothingBlock.GetClothingIndex(data);
					bool canBeDyed = ClothingBlock.GetClothingData(data).CanBeDyed;
					int damage = BlocksManager.Blocks[203].GetDamage(num3);
					int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
					int damage2 = BlocksManager.Blocks[129].GetDamage(num);
					Block block = BlocksManager.Blocks[129];
					Block block2 = BlocksManager.Blocks[203];
					if (!canBeDyed)
					{
						return null;
					}
					int num5 = PaintBucketBlock.CombineColors(clothingColor, color);
					if (num5 != clothingColor)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block2.SetDamage(Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, num5), clothingIndex)), damage),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), damage2 + MathUtils.Max(block.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = LanguageControl.Get("BlocksManager", "Dyed") + " " + SubsystemPalette.GetName(terrain, new int?(color), null),
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
				if (num2 != 0 && num3 != 0)
				{
					int data2 = Terrain.ExtractData(num3);
					int clothingColor2 = ClothingBlock.GetClothingColor(data2);
					int clothingIndex2 = ClothingBlock.GetClothingIndex(data2);
					bool canBeDyed2 = ClothingBlock.GetClothingData(data2).CanBeDyed;
					int damage3 = BlocksManager.Blocks[203].GetDamage(num3);
					int damage4 = BlocksManager.Blocks[128].GetDamage(num2);
					Block block3 = BlocksManager.Blocks[128];
					Block block4 = BlocksManager.Blocks[203];
					if (!canBeDyed2)
					{
						return null;
					}
					if (clothingColor2 != 0)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block4.SetDamage(Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, 0), clothingIndex2)), damage3),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128, 0, 0), damage4 + MathUtils.Max(block3.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = LanguageControl.Get("BlocksManager", "Not Dyed") + " " + LanguageControl.Get("BlocksManager", "Clothes"),
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
			}
			return null;
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x000C3FF0 File Offset: 0x000C21F0
		public static int GetClothingIndex(int data)
		{
			return data & 255;
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x000C3FF9 File Offset: 0x000C21F9
		public static int SetClothingIndex(int data, int clothingIndex)
		{
			return (data & -256) | (clothingIndex & 255);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x000C400C File Offset: 0x000C220C
		public static ClothingData GetClothingData(int data)
		{
			int num = ClothingBlock.GetClothingIndex(data);
			if (num >= ClothingBlock.m_clothingData.Length)
			{
				num = 0;
			}
			return ClothingBlock.m_clothingData[num];
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x000C4033 File Offset: 0x000C2233
		public static int GetClothingColor(int data)
		{
			return data >> 12 & 15;
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x000C403C File Offset: 0x000C223C
		public static int SetClothingColor(int data, int color)
		{
			return (data & -61441) | (color & 15) << 12;
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x000C404D File Offset: 0x000C224D
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x000C4050 File Offset: 0x000C2250
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int clothingColor = ClothingBlock.GetClothingColor(data);
			ClothingData clothingData = ClothingBlock.GetClothingData(data);
			Matrix matrix2 = ClothingBlock.m_slotTransforms[(int)clothingData.Slot] * Matrix.CreateScale(size) * matrix;
			if (clothingData.IsOuter)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_outerMesh, clothingData.Texture, color * SubsystemPalette.GetFabricColor(environmentData, new int?(clothingColor)), 1f, ref matrix2, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_innerMesh, clothingData.Texture, color * SubsystemPalette.GetFabricColor(environmentData, new int?(clothingColor)), 1f, ref matrix2, environmentData);
		}

		// Token: 0x0400115F RID: 4447
		public const int Index = 203;

		// Token: 0x04001160 RID: 4448
		public static ClothingData[] m_clothingData;

		// Token: 0x04001161 RID: 4449
		public BlockMesh m_innerMesh;

		// Token: 0x04001162 RID: 4450
		public BlockMesh m_outerMesh;

		// Token: 0x04001163 RID: 4451
		public static Action Initialize1;

		// Token: 0x04001164 RID: 4452
		public static Matrix[] m_slotTransforms = new Matrix[]
		{
			Matrix.CreateTranslation(0f, -1.5f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -1.1f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -0.1f, 0f) * Matrix.CreateScale(2.7f)
		};
	}
}
