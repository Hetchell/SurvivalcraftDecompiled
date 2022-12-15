using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000364 RID: 868
	public static class CraftingRecipesManager
	{
		// Token: 0x170003CE RID: 974
		// (get) Token: 0x0600187D RID: 6269 RVA: 0x000C2B47 File Offset: 0x000C0D47
		public static ReadOnlyList<CraftingRecipe> Recipes
		{
			get
			{
				return new ReadOnlyList<CraftingRecipe>(CraftingRecipesManager.m_recipes);
			}
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x000C2B54 File Offset: 0x000C0D54
		public static void Initialize()
		{
			if (CraftingRecipesManager.Initialize1 != null)
			{
				CraftingRecipesManager.Initialize1();
				return;
			}
			XElement xelement = ContentManager.Get<XElement>("CraftingRecipes");
			ModsManager.CombineXml(xelement, ModsManager.GetEntries(".cr"), "Description", "Result", "Recipes");
			foreach (XElement xelement2 in xelement.Descendants("Recipe"))
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe();
				string attributeValue = XmlUtils.GetAttributeValue<string>(xelement2, "Result");
				string name = string.IsNullOrEmpty(attributeValue) ? " " : attributeValue;
				string text = XmlUtils.GetAttributeValue<string>(xelement2, "Message", null);
				string text2 = XmlUtils.GetAttributeValue<string>(xelement2, "Description");
				if (text2.StartsWith("[") && text2.EndsWith("]"))
				{
					text2 = LanguageControl.GetBlock(name, "CRDescription:" + text2.Substring(1, text2.Length - 2));
				}
				if (text != null && text.StartsWith("[") && text.EndsWith("]"))
				{
					text = LanguageControl.GetBlock(name, "CRMessage:" + text.Substring(1, text.Length - 2));
				}
				craftingRecipe.ResultValue = CraftingRecipesManager.DecodeResult(attributeValue);
				craftingRecipe.ResultCount = XmlUtils.GetAttributeValue<int>(xelement2, "ResultCount");
				string attributeValue2 = XmlUtils.GetAttributeValue<string>(xelement2, "Remains", string.Empty);
				if (!string.IsNullOrEmpty(attributeValue2))
				{
					craftingRecipe.RemainsValue = CraftingRecipesManager.DecodeResult(attributeValue2);
					craftingRecipe.RemainsCount = XmlUtils.GetAttributeValue<int>(xelement2, "RemainsCount");
				}
				craftingRecipe.RequiredHeatLevel = XmlUtils.GetAttributeValue<float>(xelement2, "RequiredHeatLevel");
				craftingRecipe.RequiredPlayerLevel = XmlUtils.GetAttributeValue<float>(xelement2, "RequiredPlayerLevel", 1f);
				craftingRecipe.Description = text2;
				craftingRecipe.Message = text;
				if (craftingRecipe.ResultCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.ResultValue)].MaxStacking)
				{
					throw new InvalidOperationException("In recipe for \"" + attributeValue + "\" ResultCount is larger than max stacking of result block.");
				}
				if (craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].MaxStacking)
				{
					throw new InvalidOperationException("In Recipe for \"" + attributeValue2 + "\" RemainsCount is larger than max stacking of remains block.");
				}
				Dictionary<char, string> dictionary = new Dictionary<char, string>();
				foreach (XAttribute xattribute in from a in xelement2.Attributes()
				where a.Name.LocalName.Length == 1 && char.IsLower(a.Name.LocalName[0])
				select a)
				{
					string craftingId;
					int? num;
					CraftingRecipesManager.DecodeIngredient(xattribute.Value, out craftingId, out num);
					if (BlocksManager.FindBlocksByCraftingId(craftingId).Length == 0)
					{
						throw new InvalidOperationException("Block with craftingId \"" + xattribute.Value + "\" not found.");
					}
					if (num != null && (num.Value < 0 || num.Value > 262143))
					{
						throw new InvalidOperationException("Data in recipe ingredient \"" + xattribute.Value + "\" must be between 0 and 0x3FFFF.");
					}
					dictionary.Add(xattribute.Name.LocalName[0], xattribute.Value);
				}
				string[] array = xelement2.Value.Trim().Split(new string[]
				{
					"\n"
				}, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = array[i].IndexOf('"');
					int num3 = array[i].LastIndexOf('"');
					if (num2 < 0 || num3 < 0 || num3 <= num2)
					{
						throw new InvalidOperationException("Invalid recipe line.");
					}
					string text3 = array[i].Substring(num2 + 1, num3 - num2 - 1);
					for (int j = 0; j < text3.Length; j++)
					{
						char c = text3[j];
						if (char.IsLower(c))
						{
							string text4 = dictionary[c];
							craftingRecipe.Ingredients[j + i * 3] = text4;
						}
					}
				}
				CraftingRecipesManager.m_recipes.Add(craftingRecipe);
			}
			foreach (Block block in BlocksManager.Blocks)
			{
				CraftingRecipesManager.m_recipes.AddRange(block.GetProceduralCraftingRecipes());
			}
			Action initialized = CraftingRecipesManager.Initialized;
			if (initialized != null)
			{
				initialized();
			}
			CraftingRecipesManager.m_recipes.Sort(delegate(CraftingRecipe r1, CraftingRecipe r2)
			{
				int y = r1.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				int x = r2.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				return Comparer<int>.Default.Compare(x, y);
			});
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x000C3000 File Offset: 0x000C1200
		public static CraftingRecipe FindMatchingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			CraftingRecipe craftingRecipe = null;
			Block[] blocks = BlocksManager.Blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				CraftingRecipe adHocCraftingRecipe = blocks[i].GetAdHocCraftingRecipe(terrain, ingredients, heatLevel, playerLevel);
				if (adHocCraftingRecipe != null && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					craftingRecipe = adHocCraftingRecipe;
					break;
				}
			}
			if (craftingRecipe == null)
			{
				foreach (CraftingRecipe craftingRecipe2 in CraftingRecipesManager.Recipes)
				{
					if (CraftingRecipesManager.MatchRecipe(craftingRecipe2.Ingredients, ingredients))
					{
						craftingRecipe = craftingRecipe2;
						break;
					}
				}
			}
			if (craftingRecipe != null)
			{
				if (heatLevel < craftingRecipe.RequiredHeatLevel)
				{
					CraftingRecipe craftingRecipe3;
					if (heatLevel > 0f)
					{
						(craftingRecipe3 = new CraftingRecipe()).Message = LanguageControl.Get(CraftingRecipesManager.fName, 1);
					}
					else
					{
						(craftingRecipe3 = new CraftingRecipe()).Message = LanguageControl.Get(CraftingRecipesManager.fName, 0);
					}
					craftingRecipe = craftingRecipe3;
				}
				else if (playerLevel < craftingRecipe.RequiredPlayerLevel)
				{
					CraftingRecipe craftingRecipe4;
					if (craftingRecipe.RequiredHeatLevel > 0f)
					{
						(craftingRecipe4 = new CraftingRecipe()).Message = string.Format(LanguageControl.Get(CraftingRecipesManager.fName, 3), craftingRecipe.RequiredPlayerLevel);
					}
					else
					{
						(craftingRecipe4 = new CraftingRecipe()).Message = string.Format(LanguageControl.Get(CraftingRecipesManager.fName, 2), craftingRecipe.RequiredPlayerLevel);
					}
					craftingRecipe = craftingRecipe4;
				}
			}
			return craftingRecipe;
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000C3150 File Offset: 0x000C1350
		public static int DecodeResult(string result)
		{
			if (!string.IsNullOrEmpty(result))
			{
				string[] array = result.Split(new char[]
				{
					':'
				}, StringSplitOptions.None);
				Block block = BlocksManager.FindBlockByTypeName(array[0], true);
				int data = (array.Length >= 2) ? int.Parse(array[1], CultureInfo.InvariantCulture) : 0;
				return Terrain.MakeBlockValue(block.BlockIndex, 0, data);
			}
			return 0;
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x000C31A8 File Offset: 0x000C13A8
		public static void DecodeIngredient(string ingredient, out string craftingId, out int? data)
		{
			string[] array = ingredient.Split(new char[]
			{
				':'
			}, StringSplitOptions.None);
			craftingId = array[0];
			data = ((array.Length >= 2) ? new int?(int.Parse(array[1], CultureInfo.InvariantCulture)) : null);
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x000C31F8 File Offset: 0x000C13F8
		public static bool MatchRecipe(string[] requiredIngredients, string[] actualIngredients)
		{
			string[] array = new string[9];
			for (int i = 0; i < 2; i++)
			{
				for (int j = -3; j <= 3; j++)
				{
					for (int k = -3; k <= 3; k++)
					{
						bool flip = i != 0;
						if (CraftingRecipesManager.TransformRecipe(array, requiredIngredients, k, j, flip))
						{
							bool flag = true;
							for (int l = 0; l < 9; l++)
							{
								if (!CraftingRecipesManager.CompareIngredients(array[l], actualIngredients[l]))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000C3278 File Offset: 0x000C1478
		public static bool TransformRecipe(string[] transformedIngredients, string[] ingredients, int shiftX, int shiftY, bool flip)
		{
			for (int i = 0; i < 9; i++)
			{
				transformedIngredients[i] = null;
			}
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					int num = (flip ? (3 - k - 1) : k) + shiftX;
					int num2 = j + shiftY;
					string text = ingredients[k + j * 3];
					if (num >= 0 && num2 >= 0 && num < 3 && num2 < 3)
					{
						transformedIngredients[num + num2 * 3] = text;
					}
					else if (!string.IsNullOrEmpty(text))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x000C32F8 File Offset: 0x000C14F8
		public static bool CompareIngredients(string requiredIngredient, string actualIngredient)
		{
			if (requiredIngredient == null)
			{
				return actualIngredient == null;
			}
			if (actualIngredient == null)
			{
				return requiredIngredient == null;
			}
			string a;
			int? num;
			CraftingRecipesManager.DecodeIngredient(requiredIngredient, out a, out num);
			string b;
			int? num2;
			CraftingRecipesManager.DecodeIngredient(actualIngredient, out b, out num2);
			if (num2 == null)
			{
				throw new InvalidOperationException("Actual ingredient data not specified.");
			}
			return a == b && (num == null || num.Value == num2.Value);
		}

		// Token: 0x04001159 RID: 4441
		public static List<CraftingRecipe> m_recipes = new List<CraftingRecipe>();

		// Token: 0x0400115A RID: 4442
		public static Action Initialize1;

		// Token: 0x0400115B RID: 4443
		public static Action Initialized;

		// Token: 0x0400115C RID: 4444
		public static string fName = "CraftingRecipesManager";
	}
}
