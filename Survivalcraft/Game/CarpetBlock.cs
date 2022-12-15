using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200002E RID: 46
	public class CarpetBlock : CubeBlock
	{
		// Token: 0x06000127 RID: 295 RVA: 0x000081B3 File Offset: 0x000063B3
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != 5;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000081BC File Offset: 0x000063BC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			Color fabricColor = SubsystemPalette.GetFabricColor(generator, new int?(CarpetBlock.GetColor(data)));
			generator.GenerateCubeVertices(this, value, x, y, z, 0.0625f, 0.0625f, 0.0625f, 0.0625f, fabricColor, fabricColor, fabricColor, fabricColor, fabricColor, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00008210 File Offset: 0x00006410
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			color *= SubsystemPalette.GetFabricColor(environmentData, new int?(CarpetBlock.GetColor(data)));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size, 0.0625f * size, size), ref matrix, color, color, environmentData);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000825C File Offset: 0x0000645C
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(208, 0, i);
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00008268 File Offset: 0x00006468
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int data = Terrain.ExtractData(oldValue);
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(208, 0, data),
					Count = 1
				});
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000082B8 File Offset: 0x000064B8
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int data = Terrain.ExtractData(value);
			Color fabricColor = SubsystemPalette.GetFabricColor(subsystemTerrain, new int?(CarpetBlock.GetColor(data)));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, fabricColor, this.DefaultTextureSlot);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000082F4 File Offset: 0x000064F4
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel, float playerLevel)
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
					if (a == BlocksManager.Blocks[208].CraftingId)
					{
						num3 = Terrain.MakeBlockValue(208, 0, (num4 != null) ? num4.Value : 0);
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
					int num5 = Terrain.ExtractData(num3);
					int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
					int damage = BlocksManager.Blocks[129].GetDamage(num);
					Block block = BlocksManager.Blocks[129];
					int num6 = PaintBucketBlock.CombineColors(num5, color);
					if (num6 != num5)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = Terrain.MakeBlockValue(208, 0, num6),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), damage + MathUtils.Max(block.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = "Dye carpet " + SubsystemPalette.GetName(subsystemTerrain, new int?(color), null),
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
				if (num2 != 0 && num3 != 0)
				{
					bool flag = Terrain.ExtractData(num3) != 0;
					int damage2 = BlocksManager.Blocks[128].GetDamage(num2);
					Block block2 = BlocksManager.Blocks[128];
					if (flag)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = Terrain.MakeBlockValue(208, 0, 0),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128, 0, 0), damage2 + MathUtils.Max(block2.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = "Undye carpet",
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
			}
			return null;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000085C8 File Offset: 0x000067C8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000085D0 File Offset: 0x000067D0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int value2 = Terrain.ExtractData(value);
			return SubsystemPalette.GetName(subsystemTerrain, new int?(value2), "地毯");
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000085F5 File Offset: 0x000067F5
		public static int GetColor(int data)
		{
			return data & 15;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000085FB File Offset: 0x000067FB
		public static int SetColor(int data, int color)
		{
			return (data & -16) | (color & 15);
		}

		// Token: 0x04000098 RID: 152
		public const int Index = 208;

		// Token: 0x04000099 RID: 153
		public BoundingBox[] m_collisionBoxes = new BoundingBox[]
		{
			new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.0625f, 1f))
		};
	}
}
