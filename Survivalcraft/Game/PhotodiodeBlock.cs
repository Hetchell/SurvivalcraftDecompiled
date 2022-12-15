using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C4 RID: 196
	public class PhotodiodeBlock : MountedElectricElementBlock
	{
		// Token: 0x060003BD RID: 957 RVA: 0x00014788 File Offset: 0x00012988
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Photodiode");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Photodiode", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Photodiode", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Photodiode", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00014921 File Offset: 0x00012B21
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0001492C File Offset: 0x00012B2C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001496C File Offset: 0x00012B6C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00014998 File Offset: 0x00012B98
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00014A04 File Offset: 0x00012C04
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00014A1F File Offset: 0x00012C1F
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PhotodiodeElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00014A38 File Offset: 0x00012C38
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x040001AF RID: 431
		public const int Index = 151;

		// Token: 0x040001B0 RID: 432
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040001B1 RID: 433
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x040001B2 RID: 434
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
