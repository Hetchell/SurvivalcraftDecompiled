using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Graphics;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000060 RID: 96
	public class EggBlock : Block
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000A99F File Offset: 0x00008B9F
		public ReadOnlyList<EggBlock.EggType> EggTypes
		{
			get
			{
				return new ReadOnlyList<EggBlock.EggType>(this.m_eggTypes);
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000A9AC File Offset: 0x00008BAC
		public override void Initialize()
		{
			List<EggBlock.EggType> list = new List<EggBlock.EggType>();
			DatabaseObjectType parameterSetType = DatabaseManager.GameDatabase.ParameterSetType;
			Guid eggParameterSetGuid = new Guid("300ff557-775f-4c7c-a88a-26655369f00b");
			IEnumerable<DatabaseObject> explicitNestingChildren = DatabaseManager.GameDatabase.Database.Root.GetExplicitNestingChildren(parameterSetType, false);
			Func<DatabaseObject, bool> predicate0 = null;
			Func<DatabaseObject, bool> predicate;
			if ((predicate = predicate0) == null)
			{
				predicate = (predicate0 = ((DatabaseObject o) => o.EffectiveInheritanceRoot.Guid == eggParameterSetGuid));
			}
			foreach (DatabaseObject databaseObject in explicitNestingChildren.Where(predicate))
			{
				int nestedValue = databaseObject.GetNestedValue<int>("EggTypeIndex");
				if (nestedValue >= 0)
				{
					string text = databaseObject.GetNestedValue<string>("DisplayName");
					if (text.StartsWith("[") && text.EndsWith("]"))
					{
						string[] array = text.Substring(1, text.Length - 2).Split(new string[]
						{
							":"
						}, StringSplitOptions.RemoveEmptyEntries);
						text = LanguageControl.GetDatabase("DisplayName", array[1]);
					}
					list.Add(new EggBlock.EggType
					{
						EggTypeIndex = nestedValue,
						ShowEgg = databaseObject.GetNestedValue<bool>("ShowEgg"),
						DisplayName = text,
						TemplateName = databaseObject.NestingParent.Name,
						NutritionalValue = databaseObject.GetNestedValue<float>("NutritionalValue"),
						Color = databaseObject.GetNestedValue<Color>("Color"),
						ScaleUV = databaseObject.GetNestedValue<Vector2>("ScaleUV"),
						SwapUV = databaseObject.GetNestedValue<bool>("SwapUV"),
						Scale = databaseObject.GetNestedValue<float>("Scale"),
						TextureSlot = databaseObject.GetNestedValue<int>("TextureSlot")
					});
				}
			}
			this.m_eggTypes.AddRange((from p in list
			orderby p.EggTypeIndex
			select p).ToArray<EggBlock.EggType>());
			Model model = ContentManager.Get<Model>("Models/Egg");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Egg", true).ParentBone);
			foreach (EggBlock.EggType eggType in this.m_eggTypes)
			{
				eggType.BlockMesh = new BlockMesh();
				eggType.BlockMesh.AppendModelMeshPart(model.FindMesh("Egg", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, eggType.Color);
				Matrix matrix = Matrix.Identity;
				if (eggType.SwapUV)
				{
					matrix.M11 = 0f;
					matrix.M12 = 1f;
					matrix.M21 = 1f;
					matrix.M22 = 0f;
				}
				matrix *= Matrix.CreateScale(0.0625f * eggType.ScaleUV.X, 0.0625f * eggType.ScaleUV.Y, 1f);
				matrix *= Matrix.CreateTranslation((float)(eggType.TextureSlot % 16) / 16f, (float)(eggType.TextureSlot / 16) / 16f, 0f);
				eggType.BlockMesh.TransformTextureCoordinates(matrix, -1);
			}
			base.Initialize();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000AD34 File Offset: 0x00008F34
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			EggBlock.EggType eggType = this.GetEggType(Terrain.ExtractData(value));
			int data = Terrain.ExtractData(value);
			bool isCooked = EggBlock.GetIsCooked(data);
			bool isLaid = EggBlock.GetIsLaid(data);
			if (isCooked)
			{
				return LanguageControl.Get(EggBlock.fName, 1) + eggType.DisplayName;
			}
			if (!isLaid)
			{
				return eggType.DisplayName;
			}
			return LanguageControl.Get(EggBlock.fName, 2) + eggType.DisplayName;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000AD9B File Offset: 0x00008F9B
		public override string GetCategory(int value)
		{
			return LanguageControl.Get("BlocksManager", "Spawner Eggs");
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000ADAC File Offset: 0x00008FAC
		public override string GetDescription(int value)
		{
			return LanguageControl.Get(EggBlock.fName, 3) + this.GetEggType(Terrain.ExtractData(value)).TemplateName;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000ADD0 File Offset: 0x00008FD0
		public override float GetNutritionalValue(int value)
		{
			EggBlock.EggType eggType = this.GetEggType(Terrain.ExtractData(value));
			if (!EggBlock.GetIsCooked(Terrain.ExtractData(value)))
			{
				return eggType.NutritionalValue;
			}
			return 1.5f * eggType.NutritionalValue;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000AE0A File Offset: 0x0000900A
		public override float GetSicknessProbability(int value)
		{
			if (!EggBlock.GetIsCooked(Terrain.ExtractData(value)))
			{
				return this.DefaultSicknessProbability;
			}
			return 0f;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000AE25 File Offset: 0x00009025
		public override int GetRotPeriod(int value)
		{
			if (this.GetNutritionalValue(value) > 0f)
			{
				return base.GetRotPeriod(value);
			}
			return 0;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000AE3E File Offset: 0x0000903E
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 16 & 1;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000AE4C File Offset: 0x0000904C
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num = ((num & -65537) | (damage & 1) << 16);
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000AE76 File Offset: 0x00009076
		public override int GetDamageDestructionValue(int value)
		{
			return 246;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000AE7D File Offset: 0x0000907D
		public override IEnumerable<int> GetCreativeValues()
		{
			foreach (EggBlock.EggType eggType in this.m_eggTypes)
			{
				if (eggType.ShowEgg)
				{
					yield return Terrain.MakeBlockValue(118, 0, EggBlock.SetEggType(0, eggType.EggTypeIndex));
					if (eggType.NutritionalValue > 0f)
					{
						yield return Terrain.MakeBlockValue(118, 0, EggBlock.SetIsCooked(EggBlock.SetEggType(0, eggType.EggTypeIndex), true));
					}
				}
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000AE8D File Offset: 0x0000908D
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			foreach (EggBlock.EggType eggType in ((EggBlock)BlocksManager.Blocks[118]).EggTypes)
			{
				if (eggType.NutritionalValue > 0f)
				{
					int num;
					for (int rot = 0; rot <= 1; rot = num)
					{
						CraftingRecipe craftingRecipe = new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = Terrain.MakeBlockValue(118, 0, EggBlock.SetEggType(EggBlock.SetIsCooked(0, true), eggType.EggTypeIndex)),
							RemainsCount = 1,
							RemainsValue = Terrain.MakeBlockValue(91),
							RequiredHeatLevel = 1f,
							Description = "Cook an egg to increase its nutritional value"
						};
						int data = EggBlock.SetEggType(EggBlock.SetIsLaid(0, true), eggType.EggTypeIndex);
						int value = this.SetDamage(Terrain.MakeBlockValue(118, 0, data), rot);
						craftingRecipe.Ingredients[0] = "egg:" + Terrain.ExtractData(value).ToString(CultureInfo.InvariantCulture);
						craftingRecipe.Ingredients[1] = "waterbucket";
						yield return craftingRecipe;
						num = rot + 1;
					}
				}
			}

		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000AE9D File Offset: 0x0000909D
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000AEA0 File Offset: 0x000090A0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			EggBlock.EggType eggType = this.GetEggType(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, eggType.BlockMesh, color, eggType.Scale * size, ref matrix, environmentData);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000AED8 File Offset: 0x000090D8
		public EggBlock.EggType GetEggType(int data)
		{
			int index = data >> 4 & 4095;
			return this.m_eggTypes[index];
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000AEFC File Offset: 0x000090FC
		public EggBlock.EggType GetEggTypeByCreatureTemplateName(string templateName)
		{
			return this.m_eggTypes.FirstOrDefault((EggBlock.EggType e) => e.TemplateName == templateName);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000AF2D File Offset: 0x0000912D
		public static bool GetIsCooked(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000AF35 File Offset: 0x00009135
		public static int SetIsCooked(int data, bool isCooked)
		{
			if (!isCooked)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000AF42 File Offset: 0x00009142
		public static bool GetIsLaid(int data)
		{
			return (data & 2) != 0;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000AF4A File Offset: 0x0000914A
		public static int SetIsLaid(int data, bool isLaid)
		{
			if (!isLaid)
			{
				return data & -3;
			}
			return data | 2;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000AF57 File Offset: 0x00009157
		public static int SetEggType(int data, int eggTypeIndex)
		{
			data &= -65521;
			data |= (eggTypeIndex & 4095) << 4;
			return data;
		}

		// Token: 0x040000DF RID: 223
		public new static string fName = "EggBlock";

		// Token: 0x040000E0 RID: 224
		public const int Index = 118;

		// Token: 0x040000E1 RID: 225
		public List<EggBlock.EggType> m_eggTypes = new List<EggBlock.EggType>();

		// Token: 0x020003C5 RID: 965
		public class EggType
		{
			// Token: 0x040013FF RID: 5119
			public int EggTypeIndex;

			// Token: 0x04001400 RID: 5120
			public bool ShowEgg;

			// Token: 0x04001401 RID: 5121
			public string DisplayName;

			// Token: 0x04001402 RID: 5122
			public string TemplateName;

			// Token: 0x04001403 RID: 5123
			public float NutritionalValue;

			// Token: 0x04001404 RID: 5124
			public int TextureSlot;

			// Token: 0x04001405 RID: 5125
			public Color Color;

			// Token: 0x04001406 RID: 5126
			public Vector2 ScaleUV;

			// Token: 0x04001407 RID: 5127
			public bool SwapUV;

			// Token: 0x04001408 RID: 5128
			public float Scale;

			// Token: 0x04001409 RID: 5129
			public BlockMesh BlockMesh;
		}
	}
}
