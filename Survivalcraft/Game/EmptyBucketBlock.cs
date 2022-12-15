using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000061 RID: 97
	public class EmptyBucketBlock : BucketBlock
	{
		// Token: 0x060001DE RID: 478 RVA: 0x0000AF90 File Offset: 0x00009190
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/EmptyBucket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000B021 File Offset: 0x00009221
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000E2 RID: 226
		public const int Index = 90;

		// Token: 0x040000E3 RID: 227
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
