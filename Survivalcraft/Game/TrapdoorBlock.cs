using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000113 RID: 275
	public abstract class TrapdoorBlock : Block, IElectricElementBlock
	{
		// Token: 0x0600053D RID: 1341 RVA: 0x0001D1BB File Offset: 0x0001B3BB
		public TrapdoorBlock(string modelName)
		{
			this.m_modelName = modelName;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001D1F0 File Offset: 0x0001B3F0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Trapdoor", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = TrapdoorBlock.GetRotation(i);
				bool open = TrapdoorBlock.GetOpen(i);
				bool upsideDown = TrapdoorBlock.GetUpsideDown(i);
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateTranslation(0f, -0.0625f, 0.4375f) * Matrix.CreateRotationX(open ? -1.5707964f : 0f) * Matrix.CreateTranslation(0f, 0.0625f, -0.4375f);
				matrix *= Matrix.CreateRotationZ(upsideDown ? 3.1415927f : 0f);
				matrix *= Matrix.CreateRotationY((float)rotation * 3.1415927f / 2f);
				matrix *= Matrix.CreateTranslation(new Vector3(0.5f, (float)(upsideDown ? 1 : 0), 0.5f));
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Trapdoor", true).MeshParts[0], boneAbsoluteTransform * matrix, false, false, false, false, Color.White);
				this.m_blockMeshesByData[i].GenerateSidesData();
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_blockMeshesByData[i].CalculateBoundingBox()
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Trapdoor", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001D3C8 File Offset: 0x0001B5C8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001D411 File Offset: 0x0001B611
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001D428 File Offset: 0x0001B628
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int rotation;
			bool upsideDown;
			if (raycastResult.CellFace.Face < 4)
			{
				rotation = raycastResult.CellFace.Face;
				upsideDown = (raycastResult.HitPoint(0f).Y - (float)raycastResult.CellFace.Y > 0.5f);
			}
			else
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				float num = Vector3.Dot(forward, Vector3.UnitZ);
				float num2 = Vector3.Dot(forward, Vector3.UnitX);
				float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
				float num4 = Vector3.Dot(forward, -Vector3.UnitX);
				rotation = ((num == MathUtils.Max(num, num2, num3, num4)) ? 2 : ((num2 == MathUtils.Max(num, num2, num3, num4)) ? 3 : ((num3 != MathUtils.Max(num, num2, num3, num4)) ? ((num4 == MathUtils.Max(num, num2, num3, num4)) ? 1 : 0) : 0)));
				upsideDown = (raycastResult.CellFace.Face == 5);
			}
			int data = TrapdoorBlock.SetOpen(TrapdoorBlock.SetRotation(TrapdoorBlock.SetUpsideDown(0, upsideDown), rotation), false);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001D580 File Offset: 0x0001B780
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001D5B0 File Offset: 0x0001B7B0
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new TrapDoorElectricElement(subsystemElectricity, new CellFace(x, y, z, TrapdoorBlock.GetMountingFace(data)));
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001D5DC File Offset: 0x0001B7DC
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (face == TrapdoorBlock.GetMountingFace(data))
			{
				int rotation = TrapdoorBlock.GetRotation(data);
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(4, (4 - rotation) % 4, connectorFace);
				ElectricConnectorDirection electricConnectorDirection = ElectricConnectorDirection.Top;
				if (connectorDirection.GetValueOrDefault() == electricConnectorDirection & connectorDirection != null)
				{
					return new ElectricConnectorType?(ElectricConnectorType.Input);
				}
			}
			return null;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001D635 File Offset: 0x0001B835
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001D63C File Offset: 0x0001B83C
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001D641 File Offset: 0x0001B841
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001D649 File Offset: 0x0001B849
		public static bool GetUpsideDown(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001D651 File Offset: 0x0001B851
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001D65B File Offset: 0x0001B85B
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001D668 File Offset: 0x0001B868
		public static int SetUpsideDown(int data, bool upsideDown)
		{
			if (!upsideDown)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001D675 File Offset: 0x0001B875
		public static int GetMountingFace(int data)
		{
			if (!TrapdoorBlock.GetUpsideDown(data))
			{
				return 4;
			}
			return 5;
		}

		// Token: 0x04000253 RID: 595
		public string m_modelName;

		// Token: 0x04000254 RID: 596
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000255 RID: 597
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x04000256 RID: 598
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];
	}
}
