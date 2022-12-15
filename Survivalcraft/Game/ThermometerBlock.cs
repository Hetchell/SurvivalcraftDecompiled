using System;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000111 RID: 273
	public class ThermometerBlock : Block, IElectricElementBlock
	{
		// Token: 0x0600052E RID: 1326 RVA: 0x0001C66C File Offset: 0x0001A86C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Thermometer");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fluid", true).ParentBone);
			this.m_caseMesh.AppendModelMeshPart(model.FindMesh("Case", true).MeshParts[0], boneAbsoluteTransform, false, false, true, false, Color.White);
			this.m_fluidMesh.AppendModelMeshPart(model.FindMesh("Fluid", true).MeshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				this.m_matricesByData[i] = Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)(i + 1) * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_caseMesh.CalculateBoundingBox(this.m_matricesByData[i])
				};
			}
			this.m_fluidBottomPosition = this.m_fluidMesh.Vertices.Min((BlockMeshVertex v) => v.Position.Y);
			base.Initialize();
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001C80C File Offset: 0x0001AA0C
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			return new ThermometerElectricElement(subsystemElectricity, new CellFace(x, y, z, num & 3));
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001C834 File Offset: 0x0001AA34
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 3) == face)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001C85C File Offset: 0x0001AA5C
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001C864 File Offset: 0x0001AA64
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001C890 File Offset: 0x0001AA90
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 3);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001C938 File Offset: 0x0001AB38
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_matricesByData.Length)
			{
				int num2 = (generator.SubsystemMetersBlockBehavior != null) ? generator.SubsystemMetersBlockBehavior.GetThermometerReading(x, y, z) : 8;
				float y2 = MathUtils.Lerp(1f, 4f, (float)num2 / 15f);
				Matrix matrix = this.m_matricesByData[num];
				Matrix value2 = Matrix.CreateTranslation(0f, 0f - this.m_fluidBottomPosition, 0f) * Matrix.CreateScale(1f, y2, 1f) * Matrix.CreateTranslation(0f, this.m_fluidBottomPosition, 0f) * matrix;
				generator.GenerateMeshVertices(this, x, y, z, this.m_caseMesh, Color.White, new Matrix?(matrix), geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_fluidMesh, Color.White, new Matrix?(value2), geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.2f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001CA58 File Offset: 0x0001AC58
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
				float x = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2, num3);
				float x2 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2, num3 + 1);
				float x3 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2 + 1, num3);
				float x4 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2 + 1, num3 + 1);
				float x5 = MathUtils.Lerp(x, x2, f2);
				float x6 = MathUtils.Lerp(x3, x4, f2);
				num = MathUtils.Lerp(x5, x6, f);
			}
			float y = MathUtils.Lerp(1f, 4f, num / 15f);
			Matrix m = Matrix.CreateScale(3f * size) * Matrix.CreateTranslation(0f, -0.15f, 0f) * matrix;
			Matrix matrix2 = Matrix.CreateTranslation(0f, 0f - this.m_fluidBottomPosition, 0f) * Matrix.CreateScale(1f, y, 1f) * Matrix.CreateTranslation(0f, this.m_fluidBottomPosition, 0f) * m;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_caseMesh, color, 1f, ref m, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_fluidMesh, color, 1f, ref matrix2, environmentData);
		}

		// Token: 0x04000249 RID: 585
		public const int Index = 120;

		// Token: 0x0400024A RID: 586
		public BlockMesh m_caseMesh = new BlockMesh();

		// Token: 0x0400024B RID: 587
		public BlockMesh m_fluidMesh = new BlockMesh();

		// Token: 0x0400024C RID: 588
		public Matrix[] m_matricesByData = new Matrix[4];

		// Token: 0x0400024D RID: 589
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];

		// Token: 0x0400024E RID: 590
		public float m_fluidBottomPosition;
	}
}
