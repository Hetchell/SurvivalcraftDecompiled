using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B1 RID: 177
	public class MilkBucketBlock : BucketBlock
	{
		// Token: 0x06000354 RID: 852 RVA: 0x00012EEC File Offset: 0x000110EC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00013010 File Offset: 0x00011210
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001302B File Offset: 0x0001122B
		public override int GetDamageDestructionValue(int value)
		{
			return 245;
		}

		// Token: 0x0400018B RID: 395
		public const int Index = 110;

		// Token: 0x0400018C RID: 396
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
