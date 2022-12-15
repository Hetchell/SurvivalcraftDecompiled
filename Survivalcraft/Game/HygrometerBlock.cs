using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007E RID: 126
	public class HygrometerBlock : Block, IElectricElementBlock
	{
		// Token: 0x060002B1 RID: 689 RVA: 0x000100F0 File Offset: 0x0000E2F0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Hygrometer");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case", true).ParentBone);
			Matrix matrix = this.m_pointerMatrix = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer", true).ParentBone);
			this.m_invPointerMatrix = Matrix.Invert(this.m_pointerMatrix);
			this.m_caseMesh.AppendModelMeshPart(model.FindMesh("Case", true).MeshParts[0], boneAbsoluteTransform, false, false, true, false, Color.White);
			this.m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer", true).MeshParts[0], matrix, false, false, false, false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				this.m_matricesByData[i] = Matrix.CreateScale(5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)(i + 1) * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_caseMesh.CalculateBoundingBox(this.m_matricesByData[i])
				};
			}
			base.Initialize();
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00010278 File Offset: 0x0000E478
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			return new HygrometerElectricElement(subsystemElectricity, new CellFace(x, y, z, num & 3));
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x000102A0 File Offset: 0x0000E4A0
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 3) == face)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x000102C8 File Offset: 0x0000E4C8
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x000102D0 File Offset: 0x0000E4D0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x000102FC File Offset: 0x0000E4FC
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 3);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x000103A4 File Offset: 0x0000E5A4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_matricesByData.Length)
			{
				int humidity = generator.Terrain.GetHumidity(x, z);
				float radians = MathUtils.Lerp(1.5f, -1.5f, (float)humidity / 15f);
				Matrix matrix = this.m_matricesByData[num];
				Matrix value2 = this.m_invPointerMatrix * Matrix.CreateRotationX(radians) * this.m_pointerMatrix * matrix;
				generator.GenerateMeshVertices(this, x, y, z, this.m_caseMesh, Color.White, new Matrix?(matrix), geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_pointerMesh, Color.White, new Matrix?(value2), geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00010488 File Offset: 0x0000E688
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float num = 8f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				int num2 = Terrain.ToCell(translation.X);
				int num3 = Terrain.ToCell(translation.Z);
				float f = translation.X - (float)num2;
				float f2 = translation.Z - (float)num3;
				float x = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2, num3);
				float x2 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2, num3 + 1);
				float x3 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2 + 1, num3);
				float x4 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2 + 1, num3 + 1);
				float x5 = MathUtils.Lerp(x, x2, f2);
				float x6 = MathUtils.Lerp(x3, x4, f2);
				num = MathUtils.Lerp(x5, x6, f);
			}
			float radians = MathUtils.Lerp(1.5f, -1.5f, num / 15f);
			Matrix m = Matrix.CreateScale(7f * size) * Matrix.CreateTranslation(0f, -0.1f, 0f) * matrix;
			Matrix matrix2 = this.m_invPointerMatrix * Matrix.CreateRotationX(radians) * this.m_pointerMatrix * m;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_caseMesh, color, 1f, ref m, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_pointerMesh, color, 1f, ref matrix2, environmentData);
		}

		// Token: 0x04000137 RID: 311
		public const int Index = 121;

		// Token: 0x04000138 RID: 312
		public BlockMesh m_caseMesh = new BlockMesh();

		// Token: 0x04000139 RID: 313
		public BlockMesh m_pointerMesh = new BlockMesh();

		// Token: 0x0400013A RID: 314
		public Matrix m_pointerMatrix;

		// Token: 0x0400013B RID: 315
		public Matrix m_invPointerMatrix;

		// Token: 0x0400013C RID: 316
		public Matrix[] m_matricesByData = new Matrix[4];

		// Token: 0x0400013D RID: 317
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];
	}
}
