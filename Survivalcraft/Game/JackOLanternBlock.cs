using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000098 RID: 152
	public class JackOLanternBlock : Block
	{
		// Token: 0x060002EB RID: 747 RVA: 0x000110A8 File Offset: 0x0000F2A8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pumpkins");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("JackOLantern", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				float radians = (float)i * 3.1415927f / 2f;
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, new Color(232, 232, 232));
				blockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), true, true, false, false, Color.White);
				this.m_blockMeshesByData[i] = blockMesh;
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.23f, 0f), false, false, false, false, new Color(232, 232, 232));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.23f, 0f), true, true, false, false, Color.White);
			this.m_collisionBoxes[0] = this.m_blockMeshesByData[0].CalculateBoundingBox();
			base.Initialize();
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00011278 File Offset: 0x0000F478
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 132), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00011352 File Offset: 0x0000F552
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0001135C File Offset: 0x0000F55C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000113A4 File Offset: 0x0000F5A4
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000156 RID: 342
		public const int Index = 132;

		// Token: 0x04000157 RID: 343
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x04000158 RID: 344
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000159 RID: 345
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
