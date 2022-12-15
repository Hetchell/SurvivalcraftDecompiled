using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C1 RID: 193
	public class PaintBucketBlock : BucketBlock
	{
		// Token: 0x0600039F RID: 927 RVA: 0x00014114 File Offset: 0x00012314
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBucketBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standalonePaintBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standalonePaintBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00014238 File Offset: 0x00012438
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int color2 = PaintBucketBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBucketBlockMesh, color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standalonePaintBlockMesh, color * SubsystemPalette.GetColor(environmentData, new int?(color2)), 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00014295 File Offset: 0x00012495
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(129, 0, PaintBucketBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0001429E File Offset: 0x0001249E
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			string[] additives = new string[]
			{
				BlocksManager.Blocks[43].CraftingId,
				BlocksManager.Blocks[24].CraftingId,
				BlocksManager.Blocks[103].CraftingId,
				BlocksManager.Blocks[22].CraftingId
			};
			int num2;
			for (int color = 0; color < 16; color = num2)
			{
				for (int additive = 0; additive < 4; additive = num2)
				{
					int num = PaintBucketBlock.CombineColors(color, 1 << additive);
					if (num != color)
					{
						CraftingRecipe craftingRecipe = new CraftingRecipe
						{
							Description = "制作 " + SubsystemPalette.GetName(null, new int?(num), null) + " 颜料",
							ResultValue = Terrain.MakeBlockValue(129, 0, num),
							ResultCount = 1,
							RequiredHeatLevel = 1f
						};
						craftingRecipe.Ingredients[0] = BlocksManager.Blocks[129].CraftingId + ":" + color.ToString(CultureInfo.InvariantCulture);
						craftingRecipe.Ingredients[1] = additives[additive];
						yield return craftingRecipe;
					}
					num2 = additive + 1;
				}
				num2 = color + 1;
			}
			yield break;
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x000142A8 File Offset: 0x000124A8
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = PaintBucketBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, new int?(color), "颜料桶");
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x000142D2 File Offset: 0x000124D2
		public override int GetDamageDestructionValue(int value)
		{
			return Terrain.MakeBlockValue(90);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x000142DB File Offset: 0x000124DB
		public static int GetColor(int data)
		{
			return data & 15;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x000142E1 File Offset: 0x000124E1
		public static int SetColor(int data, int color)
		{
			return (data & -16) | (color & 15);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000142EC File Offset: 0x000124EC
		public static Vector4 ColorToCmyk(int color)
		{
			float num = (float)(color & 1);
			int num2 = color >> 1 & 1;
			int num3 = color >> 2 & 1;
			int num4 = color >> 3 & 1;
			return new Vector4(num, (float)num2, (float)num3, (float)num4);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0001431C File Offset: 0x0001251C
		public static int CmykToColor(Vector4 cmyk)
		{
			if (cmyk.W <= 1f)
			{
				int num = (int)MathUtils.Round(MathUtils.Saturate(cmyk.X));
				int num2 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.Y));
				int num3 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.Z));
				int num4 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.W));
				return num | num2 << 1 | num3 << 2 | num4 << 3;
			}
			return 15;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001438C File Offset: 0x0001258C
		public static int CombineColors(int color1, int color2)
		{
			return PaintBucketBlock.CmykToColor(PaintBucketBlock.ColorToCmyk(color1) + PaintBucketBlock.ColorToCmyk(color2));
		}

		// Token: 0x040001A9 RID: 425
		public const int Index = 129;

		// Token: 0x040001AA RID: 426
		public BlockMesh m_standaloneBucketBlockMesh = new BlockMesh();

		// Token: 0x040001AB RID: 427
		public BlockMesh m_standalonePaintBlockMesh = new BlockMesh();
	}
}
