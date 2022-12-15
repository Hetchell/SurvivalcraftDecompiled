using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000050 RID: 80
	public class DetonatorBlock : MountedElectricElementBlock
	{
		// Token: 0x06000189 RID: 393 RVA: 0x00009BB4 File Offset: 0x00007DB4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Detonator");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Detonator", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Detonator", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Detonator", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00009D4D File Offset: 0x00007F4D
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00009D58 File Offset: 0x00007F58
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00009D98 File Offset: 0x00007F98
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00009DC4 File Offset: 0x00007FC4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.125f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00009E30 File Offset: 0x00008030
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 4f * size, ref matrix, environmentData);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00009E4B File Offset: 0x0000804B
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DetonatorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00009E64 File Offset: 0x00008064
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x040000C7 RID: 199
		public const int Index = 147;

		// Token: 0x040000C8 RID: 200
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000C9 RID: 201
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x040000CA RID: 202
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
