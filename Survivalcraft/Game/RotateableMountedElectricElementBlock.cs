using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000D7 RID: 215
	public abstract class RotateableMountedElectricElementBlock : MountedElectricElementBlock
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x00016BF4 File Offset: 0x00014DF4
		public RotateableMountedElectricElementBlock(string modelName, string meshName, float centerBoxSize)
		{
			this.m_modelName = modelName;
			this.m_meshName = meshName;
			this.m_centerBoxSize = centerBoxSize;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00016C44 File Offset: 0x00014E44
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(this.m_meshName, true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				float radians;
				bool flag;
				if (i < 4)
				{
					radians = (float)i * 3.1415927f / 2f;
					flag = false;
				}
				else if (i == 4)
				{
					radians = -1.5707964f;
					flag = true;
				}
				else
				{
					radians = 1.5707964f;
					flag = true;
				}
				for (int j = 0; j < 4; j++)
				{
					float radians2 = (float)(-(float)j) * 3.1415927f / 2f;
					int num = (i << 2) + j;
					Matrix m = Matrix.CreateRotationX(1.5707964f) * Matrix.CreateRotationZ(radians2) * Matrix.CreateTranslation(0f, 0f, -0.5f) * (flag ? Matrix.CreateRotationX(radians) : Matrix.CreateRotationY(radians)) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
					this.m_blockMeshes[num] = new BlockMesh();
					this.m_blockMeshes[num].AppendModelMeshPart(model.FindMesh(this.m_meshName, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_collisionBoxes[num] = new BoundingBox[]
					{
						this.m_blockMeshes[num].CalculateBoundingBox()
					};
				}
			}
			Matrix m2 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(this.m_meshName, true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00016E0B File Offset: 0x0001500B
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) >> 2 & 7;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00016E17 File Offset: 0x00015017
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00016E34 File Offset: 0x00015034
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 31;
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[num], Color.White, null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), this.m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00016E9C File Offset: 0x0001509C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int rotation = 0;
			if (raycastResult.CellFace.Face >= 4)
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				float num = Vector3.Dot(forward, Vector3.UnitZ);
				float num2 = Vector3.Dot(forward, Vector3.UnitX);
				float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
				float num4 = Vector3.Dot(forward, -Vector3.UnitX);
				if (num == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 2;
				}
				else if (num2 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 1;
				}
				else if (num3 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 0;
				}
				else if (num4 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 3;
				}
			}
			int num5 = Terrain.ExtractData(value);
			num5 &= -29;
			num5 |= raycastResult.CellFace.Face << 2;
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, RotateableMountedElectricElementBlock.SetRotation(num5, rotation)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00016FB4 File Offset: 0x000151B4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value) & 31;
			return this.m_collisionBoxes[num];
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00016FD3 File Offset: 0x000151D3
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00016FD8 File Offset: 0x000151D8
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x040001DE RID: 478
		public string m_modelName;

		// Token: 0x040001DF RID: 479
		public string m_meshName;

		// Token: 0x040001E0 RID: 480
		public float m_centerBoxSize;

		// Token: 0x040001E1 RID: 481
		public BlockMesh[] m_blockMeshes = new BlockMesh[24];

		// Token: 0x040001E2 RID: 482
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040001E3 RID: 483
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];
	}
}
