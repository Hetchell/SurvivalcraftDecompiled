using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000E0 RID: 224
	public class RottenPumpkinSoupBucketBlock : BucketBlock
	{
		// Token: 0x0600044A RID: 1098 RVA: 0x00017240 File Offset: 0x00015440
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(255, 160, 64));
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.625f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00017370 File Offset: 0x00015570
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001ED RID: 493
		public const int Index = 252;

		// Token: 0x040001EE RID: 494
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
