using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CD RID: 205
	public class PumpkinSoupBucketBlock : BucketBlock
	{
		// Token: 0x0600041F RID: 1055 RVA: 0x00016608 File Offset: 0x00014808
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(200, 130, 35));
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00016738 File Offset: 0x00014938
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00016753 File Offset: 0x00014953
		public override int GetDamageDestructionValue(int value)
		{
			return 252;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0001675A File Offset: 0x0001495A
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int isDead = 0; isDead <= 1; isDead = num)
			{
				for (int rot = 0; rot <= 1; rot = num)
				{
					CraftingRecipe craftingRecipe = new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = 251,
						RequiredHeatLevel = 1f,
						Description = "烹饪南瓜粥"
					};
					int data = BasePumpkinBlock.SetIsDead(BasePumpkinBlock.SetSize(0, 7), isDead != 0);
					int value = this.SetDamage(Terrain.MakeBlockValue(131, 0, data), rot);
					craftingRecipe.Ingredients[0] = "pumpkin:" + Terrain.ExtractData(value).ToString(CultureInfo.InvariantCulture);
					craftingRecipe.Ingredients[1] = "waterbucket";
					yield return craftingRecipe;
					num = rot + 1;
				}
				num = isDead + 1;
			}
			yield break;
		}

		// Token: 0x040001D0 RID: 464
		public const int Index = 251;

		// Token: 0x040001D1 RID: 465
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
