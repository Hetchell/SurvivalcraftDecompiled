using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200003B RID: 59
	public class CompassBlock : Block
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00008E38 File Offset: 0x00007038
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Compass");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer", true).ParentBone);
			this.m_caseMesh.AppendModelMeshPart(model.FindMesh("Case", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.01f, 0f), false, false, true, false, Color.White);
			this.m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.01f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00008F14 File Offset: 0x00007114
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00008F18 File Offset: 0x00007118
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float radians = 0f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 forward = environmentData.InWorldMatrix.Forward;
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				Vector3 v = environmentData.SubsystemTerrain.Project.FindSubsystem<SubsystemMagnetBlockBehavior>(true).FindNearestCompassTarget(translation);
				Vector3 vector = translation - v;
				Vector2 v2 = new Vector2(forward.X, forward.Z);
				radians = Vector2.Angle(new Vector2(vector.X, vector.Z), v2);
			}
			Matrix matrix2 = matrix;
			Matrix matrix3 = Matrix.CreateRotationY(radians) * matrix;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_caseMesh, color, size * 6f, ref matrix2, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_pointerMesh, color, size * 6f, ref matrix3, environmentData);
		}

		// Token: 0x040000AF RID: 175
		public const int Index = 117;

		// Token: 0x040000B0 RID: 176
		public BlockMesh m_caseMesh = new BlockMesh();

		// Token: 0x040000B1 RID: 177
		public BlockMesh m_pointerMesh = new BlockMesh();
	}
}
