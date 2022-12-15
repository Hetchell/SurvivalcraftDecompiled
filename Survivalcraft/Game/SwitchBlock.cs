using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200010D RID: 269
	public class SwitchBlock : MountedElectricElementBlock
	{
		// Token: 0x0600050F RID: 1295 RVA: 0x0001B9C8 File Offset: 0x00019BC8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Switch");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Lever", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int num = i << 1 | j;
					Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					Matrix m2 = Matrix.CreateRotationX((j == 0) ? MathUtils.DegToRad(30f) : MathUtils.DegToRad(-30f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Lever", true).MeshParts[0], boneAbsoluteTransform2 * m2 * m, false, false, false, false, Color.White);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
			}
			Matrix m3 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m3, false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Lever", true).MeshParts[0], boneAbsoluteTransform2 * m3, false, false, false, false, Color.White);
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001BC33 File Offset: 0x00019E33
		public static bool GetLeverState(int value)
		{
			return (Terrain.ExtractData(value) & 1) != 0;
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001BC40 File Offset: 0x00019E40
		public static int SetLeverState(int value, bool state)
		{
			return Terrain.ReplaceData(value, state ? (Terrain.ExtractData(value) | 1) : (Terrain.ExtractData(value) & -2));
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001BC5E File Offset: 0x00019E5E
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) >> 1 & 7;
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001BC6C File Offset: 0x00019E6C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face << 1),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001BCAC File Offset: 0x00019EAC
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001BCD8 File Offset: 0x00019ED8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001BD44 File Offset: 0x00019F44
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001BD5F File Offset: 0x00019F5F
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SwitchElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)), value);
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001BD7C File Offset: 0x00019F7C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x04000241 RID: 577
		public const int Index = 141;

		// Token: 0x04000242 RID: 578
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000243 RID: 579
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[12];

		// Token: 0x04000244 RID: 580
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[12][];
	}
}
