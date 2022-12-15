using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000067 RID: 103
	public class FireworksBlock : Block
	{
		// Token: 0x06000217 RID: 535 RVA: 0x0000D004 File Offset: 0x0000B204
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Fireworks");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fins", true).ParentBone);
			for (int i = 0; i < 64; i++)
			{
				int num = i / 8;
				int num2 = i % 8;
				Color color = FireworksBlock.FireworksColors[num2];
				color *= 0.75f;
				color.A = byte.MaxValue;
				Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(FireworksBlock.HeadNames[num], true).ParentBone);
				this.m_headBlockMeshes[i] = new BlockMesh();
				this.m_headBlockMeshes[i].AppendModelMeshPart(model.FindMesh(FireworksBlock.HeadNames[num], true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, color);
			}
			for (int j = 0; j < 2; j++)
			{
				float num3 = 0.5f + (float)j * 0.5f;
				Matrix m = Matrix.CreateScale(new Vector3(num3, 1f, num3));
				this.m_bodyBlockMeshes[j] = new BlockMesh();
				this.m_bodyBlockMeshes[j].AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, Color.White);
			}
			for (int k = 0; k < 2; k++)
			{
				this.m_finsBlockMeshes[k] = new BlockMesh();
				this.m_finsBlockMeshes[k].AppendModelMeshPart(model.FindMesh("Fins", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, (k == 0) ? Color.White : new Color(224, 0, 0));
			}
			base.Initialize();
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000D222 File Offset: 0x0000B422
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000D224 File Offset: 0x0000B424
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int color2 = FireworksBlock.GetColor(data);
			FireworksBlock.Shape shape = FireworksBlock.GetShape(data);
			int altitude = FireworksBlock.GetAltitude(data);
			bool flickering = FireworksBlock.GetFlickering(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_headBlockMeshes[(int)shape * 8 + color2], color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_bodyBlockMeshes[altitude], color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_finsBlockMeshes[flickering ? 1 : 0], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000D2B0 File Offset: 0x0000B4B0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = FireworksBlock.GetColor(data);
			FireworksBlock.Shape shape = FireworksBlock.GetShape(data);
			int altitude = FireworksBlock.GetAltitude(data);
			bool flickering = FireworksBlock.GetFlickering(data);
			string fireworks = LanguageControl.GetFireworks("Other", "1");
			object[] array = new object[4];
			array[0] = LanguageControl.GetFireworks("FireworksColorDisplayNames", color.ToString());
			array[1] = (flickering ? LanguageControl.GetFireworks("Other", "2") : null);
			int num = 2;
			string name = "ShapeDisplayNames";
			int num2 = (int)shape;
			array[num] = LanguageControl.GetFireworks(name, num2.ToString());
			array[3] = ((altitude == 0) ? LanguageControl.GetFireworks("Other", "3") : LanguageControl.GetFireworks("Other", "4"));
			return string.Format(fireworks, array);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000D361 File Offset: 0x0000B561
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				for (int altitude = 0; altitude < 2; altitude = num)
				{
					for (int flickering = 0; flickering < 2; flickering = num)
					{
						for (int shape = 0; shape < 8; shape = num)
						{
							yield return Terrain.MakeBlockValue(215, 0, FireworksBlock.SetColor(FireworksBlock.SetAltitude(FireworksBlock.SetShape(FireworksBlock.SetFlickering(0, flickering != 0), (FireworksBlock.Shape)shape), altitude), color));
							num = shape + 1;
						}
						num = flickering + 1;
					}
					num = altitude + 1;
				}
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000D36A File Offset: 0x0000B56A
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int shape = 0; shape < 8; shape = num)
			{
				for (int altitude = 0; altitude < 2; altitude = num)
				{
					for (int flickering = 0; flickering < 2; flickering = num)
					{
						for (int color = 0; color < 8; color = num)
						{
							CraftingRecipe craftingRecipe = new CraftingRecipe
							{
								ResultCount = 20,
								ResultValue = Terrain.MakeBlockValue(215, 0, FireworksBlock.SetColor(FireworksBlock.SetAltitude(FireworksBlock.SetShape(FireworksBlock.SetFlickering(0, flickering != 0), (FireworksBlock.Shape)shape), altitude), color)),
								RemainsCount = 1,
								RemainsValue = Terrain.MakeBlockValue(90),
								RequiredHeatLevel = 0f,
								Description = "制作烟花"
							};
							if (shape == 0)
							{
								craftingRecipe.Ingredients[0] = null;
								craftingRecipe.Ingredients[1] = "sulphurchunk";
								craftingRecipe.Ingredients[2] = null;
							}
							if (shape == 1)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "coalchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 2)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = null;
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 3)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "sulphurchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 4)
							{
								craftingRecipe.Ingredients[0] = "coalchunk";
								craftingRecipe.Ingredients[1] = "coalchunk";
								craftingRecipe.Ingredients[2] = "coalchunk";
							}
							if (shape == 5)
							{
								craftingRecipe.Ingredients[0] = null;
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = null;
							}
							if (shape == 6)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 7)
							{
								craftingRecipe.Ingredients[0] = "coalchunk";
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = "coalchunk";
							}
							if (flickering == 0)
							{
								craftingRecipe.Ingredients[3] = "canvas";
								craftingRecipe.Ingredients[5] = "canvas";
							}
							if (flickering == 1)
							{
								craftingRecipe.Ingredients[3] = "gunpowder";
								craftingRecipe.Ingredients[5] = "gunpowder";
							}
							if (altitude == 0)
							{
								craftingRecipe.Ingredients[6] = "gunpowder";
								craftingRecipe.Ingredients[7] = null;
								craftingRecipe.Ingredients[8] = "gunpowder";
							}
							if (altitude == 1)
							{
								craftingRecipe.Ingredients[6] = "gunpowder";
								craftingRecipe.Ingredients[7] = "gunpowder";
								craftingRecipe.Ingredients[8] = "gunpowder";
							}
							craftingRecipe.Ingredients[4] = "paintbucket:" + ((color != 7) ? color : 10).ToString(CultureInfo.InvariantCulture);
							yield return craftingRecipe;
							num = color + 1;
						}
						num = flickering + 1;
					}
					num = altitude + 1;
				}
				num = shape + 1;
			}
			yield break;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000D373 File Offset: 0x0000B573
		public static FireworksBlock.Shape GetShape(int data)
		{
			return (FireworksBlock.Shape)(data & 7);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000D378 File Offset: 0x0000B578
		public static int SetShape(int data, FireworksBlock.Shape shape)
		{
			return (data & -8) | (int)(shape & FireworksBlock.Shape.FlatTrails);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000D382 File Offset: 0x0000B582
		public static int GetAltitude(int data)
		{
			return data >> 3 & 1;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000D389 File Offset: 0x0000B589
		public static int SetAltitude(int data, int altitude)
		{
			return (data & -9) | (altitude & 1) << 3;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000D395 File Offset: 0x0000B595
		public static bool GetFlickering(int data)
		{
			return (data >> 4 & 1) != 0;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000D39F File Offset: 0x0000B59F
		public static int SetFlickering(int data, bool flickering)
		{
			return (data & -17) | (flickering ? 1 : 0) << 4;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000D3AF File Offset: 0x0000B5AF
		public static int GetColor(int data)
		{
			return data >> 5 & 7;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000D3B6 File Offset: 0x0000B5B6
		public static int SetColor(int data, int color)
		{
			return (data & -225) | (color & 7) << 5;
		}

		// Token: 0x040000FF RID: 255
		public const int Index = 215;

		// Token: 0x04000100 RID: 256
		public BlockMesh[] m_headBlockMeshes = new BlockMesh[64];

		// Token: 0x04000101 RID: 257
		public BlockMesh[] m_bodyBlockMeshes = new BlockMesh[2];

		// Token: 0x04000102 RID: 258
		public BlockMesh[] m_finsBlockMeshes = new BlockMesh[2];

		// Token: 0x04000103 RID: 259
		public static readonly string[] HeadNames = new string[]
		{
			"HeadConeSmall",
			"HeadConeLarge",
			"HeadCylinderSmall",
			"HeadCylinderLarge",
			"HeadSphere",
			"HeadDiamondSmall",
			"HeadDiamondLarge",
			"HeadCylinderFlat"
		};

		// Token: 0x04000104 RID: 260
		public static readonly Color[] FireworksColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(85, 255, 255),
			new Color(255, 85, 85),
			new Color(85, 85, 255),
			new Color(255, 255, 85),
			new Color(85, 255, 85),
			new Color(255, 170, 0),
			new Color(255, 85, 255)
		};

		// Token: 0x020003CD RID: 973
		public enum Shape
		{
			// Token: 0x04001427 RID: 5159
			SmallBurst,
			// Token: 0x04001428 RID: 5160
			LargeBurst,
			// Token: 0x04001429 RID: 5161
			Circle,
			// Token: 0x0400142A RID: 5162
			Disc,
			// Token: 0x0400142B RID: 5163
			Ball,
			// Token: 0x0400142C RID: 5164
			ShortTrails,
			// Token: 0x0400142D RID: 5165
			LongTrails,
			// Token: 0x0400142E RID: 5166
			FlatTrails
		}
	}
}
