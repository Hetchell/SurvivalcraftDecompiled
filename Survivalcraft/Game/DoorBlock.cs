using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200005D RID: 93
	public abstract class DoorBlock : Block, IElectricElementBlock
	{
		// Token: 0x060001AF RID: 431 RVA: 0x0000A299 File Offset: 0x00008499
		public DoorBlock(string modelName, float pivotDistance)
		{
			this.m_modelName = modelName;
			this.m_pivotDistance = pivotDistance;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000A2D4 File Offset: 0x000084D4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Door", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = DoorBlock.GetRotation(i);
				bool open = DoorBlock.GetOpen(i);
				bool rightHanded = DoorBlock.GetRightHanded(i);
				float num = (float)((!rightHanded) ? 1 : -1);
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateScale(0f - num, 1f, 1f);
				matrix *= Matrix.CreateTranslation((0.5f - this.m_pivotDistance) * num, 0f, 0f) * Matrix.CreateRotationY(open ? (num * 3.1415927f / 2f) : 0f) * Matrix.CreateTranslation((0f - (0.5f - this.m_pivotDistance)) * num, 0f, 0f);
				matrix *= Matrix.CreateTranslation(0f, 0f, 0.5f - this.m_pivotDistance) * Matrix.CreateRotationY((float)rotation * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Door", true).MeshParts[0], boneAbsoluteTransform * matrix, false, !rightHanded, false, false, Color.White);
				BoundingBox boundingBox = this.m_blockMeshesByData[i].CalculateBoundingBox();
				boundingBox.Max.Y = 1f;
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					boundingBox
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Door", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000A4F8 File Offset: 0x000086F8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (DoorBlock.IsBottomPart(generator.Terrain, x, y, z) && num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetAlphaTest);
			}
			Vector2 centerOffset = DoorBlock.GetRightHanded(num) ? new Vector2(-0.45f, 0f) : new Vector2(0.45f, 0f);
			generator.GenerateWireVertices(value, x, y, z, DoorBlock.GetHingeFace(num), 0.01f, centerOffset, geometry.SubsetOpaque);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000A59B File Offset: 0x0000879B
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 0.75f * size, ref matrix, environmentData);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000A5B6 File Offset: 0x000087B6
		public override int GetShadowStrength(int value)
		{
			if (!DoorBlock.GetOpen(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return 4;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000A5D0 File Offset: 0x000087D0
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int rotation = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 1;
			}
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int num5 = raycastResult.CellFace.X + point.X;
			int y = raycastResult.CellFace.Y + point.Y;
			int num6 = raycastResult.CellFace.Z + point.Z;
			bool rightHanded = true;
			switch (rotation)
			{
			case 0:
				rightHanded = BlocksManager.Blocks[subsystemTerrain.Terrain.GetCellContents(num5 - 1, y, num6)].IsTransparent;
				break;
			case 1:
				rightHanded = BlocksManager.Blocks[subsystemTerrain.Terrain.GetCellContents(num5, y, num6 + 1)].IsTransparent;
				break;
			case 2:
				rightHanded = BlocksManager.Blocks[subsystemTerrain.Terrain.GetCellContents(num5 + 1, y, num6)].IsTransparent;
				break;
			case 3:
				rightHanded = BlocksManager.Blocks[subsystemTerrain.Terrain.GetCellContents(num5, y, num6 - 1)].IsTransparent;
				break;
			}
			int data = DoorBlock.SetRightHanded(DoorBlock.SetOpen(DoorBlock.SetRotation(0, rotation), false), rightHanded);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000A7BC File Offset: 0x000089BC
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000A7E5 File Offset: 0x000089E5
		public override bool ShouldAvoid(int value)
		{
			return !DoorBlock.GetOpen(Terrain.ExtractData(value));
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000A7F5 File Offset: 0x000089F5
		public override bool IsHeatBlocker(int value)
		{
			return !DoorBlock.GetOpen(Terrain.ExtractData(value));
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000A808 File Offset: 0x00008A08
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new DoorElectricElement(subsystemElectricity, new CellFace(x, y, z, DoorBlock.GetHingeFace(data)));
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A834 File Offset: 0x00008A34
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int hingeFace = DoorBlock.GetHingeFace(Terrain.ExtractData(value));
			if (face == hingeFace)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(hingeFace, 0, connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Right;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.Left;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						electricConnectorDirection = connectorDirection;
						electricConnectorDirection2 = ElectricConnectorDirection.In;
						if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
						{
							goto IL_69;
						}
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_69:
			return null;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000A8B4 File Offset: 0x00008AB4
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000A8BB File Offset: 0x00008ABB
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000A8C0 File Offset: 0x00008AC0
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000A8C8 File Offset: 0x00008AC8
		public static bool GetRightHanded(int data)
		{
			return (data & 8) == 0;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000A8D0 File Offset: 0x00008AD0
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000A8DA File Offset: 0x00008ADA
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000A8E7 File Offset: 0x00008AE7
		public static int SetRightHanded(int data, bool rightHanded)
		{
			if (rightHanded)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000A8F4 File Offset: 0x00008AF4
		public static bool IsTopPart(Terrain terrain, int x, int y, int z)
		{
			return BlocksManager.Blocks[terrain.GetCellContents(x, y - 1, z)] is DoorBlock;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000A90F File Offset: 0x00008B0F
		public static bool IsBottomPart(Terrain terrain, int x, int y, int z)
		{
			return BlocksManager.Blocks[terrain.GetCellContents(x, y + 1, z)] is DoorBlock;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000A92C File Offset: 0x00008B2C
		public static int GetHingeFace(int data)
		{
			int rotation = DoorBlock.GetRotation(data);
			int num = (rotation - 1 < 0) ? 3 : (rotation - 1);
			if (!DoorBlock.GetRightHanded(data))
			{
				num = CellFace.OppositeFace(num);
			}
			return num;
		}

		// Token: 0x040000D8 RID: 216
		public float m_pivotDistance;

		// Token: 0x040000D9 RID: 217
		public string m_modelName;

		// Token: 0x040000DA RID: 218
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000DB RID: 219
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x040000DC RID: 220
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];
	}
}
