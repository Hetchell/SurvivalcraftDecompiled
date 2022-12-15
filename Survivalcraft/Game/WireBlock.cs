using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200011D RID: 285
	public class WireBlock : Block, IElectricWireElementBlock, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x0001DB24 File Offset: 0x0001BD24
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Wire");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Wire", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Wire", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
			for (int i = 0; i < 6; i++)
			{
				Vector3 vector = CellFace.FaceToVector3(i);
				Vector3 v = new Vector3(0.5f, 0.5f, 0.5f) - 0.5f * vector;
				Vector3 v2;
				Vector3 v3;
				if (vector.X != 0f)
				{
					v2 = new Vector3(0f, 1f, 0f);
					v3 = new Vector3(0f, 0f, 1f);
				}
				else if (vector.Y != 0f)
				{
					v2 = new Vector3(1f, 0f, 0f);
					v3 = new Vector3(0f, 0f, 1f);
				}
				else
				{
					v2 = new Vector3(1f, 0f, 0f);
					v3 = new Vector3(0f, 1f, 0f);
				}
				Vector3 v4 = v - 0.5f * v2 - 0.5f * v3;
				Vector3 v5 = v + 0.5f * v2 + 0.5f * v3 + 0.05f * vector;
				this.m_collisionBoxesByFace[i] = new BoundingBox(Vector3.Min(v4, v5), Vector3.Max(v4, v5));
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001DD1B File Offset: 0x0001BF1B
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return null;
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001DD20 File Offset: 0x0001BF20
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (!WireBlock.WireExistsOnFace(value, face))
			{
				return null;
			}
			return new ElectricConnectorType?(ElectricConnectorType.InputOutput);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0001DD48 File Offset: 0x0001BF48
		public int GetConnectionMask(int value)
		{
			int? color = WireBlock.GetColor(Terrain.ExtractData(value));
			if (color == null)
			{
				return int.MaxValue;
			}
			return 1 << color.Value;
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001DD7C File Offset: 0x0001BF7C
		public int GetConnectedWireFacesMask(int value, int face)
		{
			int num = 0;
			if (WireBlock.WireExistsOnFace(value, face))
			{
				int num2 = CellFace.OppositeFace(face);
				bool flag = false;
				for (int i = 0; i < 6; i++)
				{
					if (i == face)
					{
						num |= 1 << i;
					}
					else if (i != num2 && WireBlock.WireExistsOnFace(value, i))
					{
						num |= 1 << i;
						flag = true;
					}
				}
				if (flag && WireBlock.WireExistsOnFace(value, num2))
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0001DDE8 File Offset: 0x0001BFE8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			BoundingBox[] array = new BoundingBox[6];
			for (int i = 0; i < 6; i++)
			{
				if (WireBlock.WireExistsOnFace(value, i))
				{
					array[i] = this.m_collisionBoxesByFace[i];
				}
				else
				{
					array[i] = default(BoundingBox);
				}
			}
			return array;
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001DE34 File Offset: 0x0001C034
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			for (int i = 0; i < 6; i++)
			{
				if (WireBlock.WireExistsOnFace(value, i))
				{
					generator.GenerateWireVertices(value, x, y, z, i, 0f, Vector2.Zero, geometry.SubsetOpaque);
				}
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0001DE74 File Offset: 0x0001C074
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = this.GetPaintColor(value);
			Color color2 = (paintColor != null) ? (color * SubsystemPalette.GetColor(environmentData, paintColor)) : (1.25f * WireBlock.WireColor * color);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color2, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001DED4 File Offset: 0x0001C0D4
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int cellValue = subsystemTerrain.Terrain.GetCellValue(raycastResult.CellFace.X + point.X, raycastResult.CellFace.Y + point.Y, raycastResult.CellFace.Z + point.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int wireFacesBitmask = WireBlock.GetWireFacesBitmask(cellValue);
			int num2 = wireFacesBitmask | 1 << raycastResult.CellFace.Face;
			if (num2 != wireFacesBitmask || !(block is WireBlock))
			{
				return new BlockPlacementData
				{
					Value = WireBlock.SetWireFacesBitmask(value, num2),
					CellFace = raycastResult.CellFace
				};
			}
			return default(BlockPlacementData);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0001DFA0 File Offset: 0x0001C1A0
		public override BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			int num = WireBlock.GetWireFacesBitmask(value);
			num &= ~(1 << raycastResult.CollisionBoxIndex);
			return new BlockPlacementData
			{
				Value = WireBlock.SetWireFacesBitmask(value, num),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0001DFEC File Offset: 0x0001C1EC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int? paintColor = this.GetPaintColor(oldValue);
			for (int i = 0; i < 6; i++)
			{
				if (WireBlock.WireExistsOnFace(oldValue, i) && !WireBlock.WireExistsOnFace(newValue, i))
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, paintColor)),
						Count = 1
					});
				}
			}
			showDebris = (dropValues.Count > 0);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001E05D File Offset: 0x0001C25D
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(133);
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(0)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(8)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(15)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(11)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(12)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(13)));
			yield return Terrain.MakeBlockValue(133, 0, WireBlock.SetColor(0, new int?(14)));
			yield break;
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001E068 File Offset: 0x0001C268
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? paintColor = this.GetPaintColor(value);
			return SubsystemPalette.GetName(subsystemTerrain, paintColor, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x0001E08C File Offset: 0x0001C28C
		public int? GetPaintColor(int value)
		{
			return WireBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0001E09C File Offset: 0x0001C29C
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, WireBlock.SetColor(data, color));
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0001E0BD File Offset: 0x0001C2BD
		public static bool WireExistsOnFace(int value, int face)
		{
			return (WireBlock.GetWireFacesBitmask(value) & 1 << face) != 0;
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x0001E0CF File Offset: 0x0001C2CF
		public static int GetWireFacesBitmask(int value)
		{
			if (Terrain.ExtractContents(value) == 133)
			{
				return Terrain.ExtractData(value) & 63;
			}
			return 0;
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0001E0EC File Offset: 0x0001C2EC
		public static int SetWireFacesBitmask(int value, int bitmask)
		{
			int num = Terrain.ExtractData(value);
			num &= -64;
			num |= (bitmask & 63);
			return Terrain.ReplaceData(Terrain.ReplaceContents(value, 133), num);
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x0001E120 File Offset: 0x0001C320
		public static int? GetColor(int data)
		{
			if ((data & 64) != 0)
			{
				return new int?(data >> 7 & 15);
			}
			return null;
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x0001E148 File Offset: 0x0001C348
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -1985) | 64 | (color.Value & 15) << 7;
			}
			return data & -1985;
		}

		// Token: 0x04000262 RID: 610
		public const int Index = 133;

		// Token: 0x04000263 RID: 611
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000264 RID: 612
		public BoundingBox[] m_collisionBoxesByFace = new BoundingBox[6];

		// Token: 0x04000265 RID: 613
		public static readonly Color WireColor = new Color(79, 36, 21);
	}
}
