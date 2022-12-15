using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000110 RID: 272
	public class TargetBlock : MountedElectricElementBlock
	{
		// Token: 0x06000524 RID: 1316 RVA: 0x0001BF64 File Offset: 0x0001A164
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = TargetBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= 0 && mountingFace < 4)
			{
				return this.m_boundingBoxes[mountingFace];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001BF98 File Offset: 0x0001A198
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			BlockPlacementData result = default(BlockPlacementData);
			if (raycastResult.CellFace.Face < 4)
			{
				result.CellFace = raycastResult.CellFace;
				result.Value = Terrain.MakeBlockValue(199, 0, TargetBlock.SetMountingFace(0, raycastResult.CellFace.Face));
			}
			return result;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001BFF0 File Offset: 0x0001A1F0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			TerrainGeometrySubset subsetAlphaTest = geometry.SubsetAlphaTest;
			DynamicArray<TerrainVertex> vertices = subsetAlphaTest.Vertices;
			DynamicArray<ushort> indices = subsetAlphaTest.Indices;
			int count = vertices.Count;
			int data = Terrain.ExtractData(value);
			int num = Terrain.ExtractLight(value);
			int mountingFace = TargetBlock.GetMountingFace(data);
			float s = LightingManager.LightIntensityByLightValueAndFace[num + 16 * mountingFace];
			Color color = Color.White * s;
			switch (mountingFace)
			{
			case 0:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 3]);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				return;
			case 1:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 3]);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				return;
			case 2:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 3]);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)count);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)(count + 2));
				return;
			case 3:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 3]);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 1));
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 2));
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)count);
				indices.Add((ushort)count);
				indices.Add((ushort)(count + 3));
				indices.Add((ushort)(count + 2));
				return;
			default:
				return;
			}
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001C4D3 File Offset: 0x0001A6D3
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001C4E5 File Offset: 0x0001A6E5
		public static int GetMountingFace(int data)
		{
			return data & 3;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001C4EA File Offset: 0x0001A6EA
		public static int SetMountingFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001C4F4 File Offset: 0x0001A6F4
		public override int GetFace(int value)
		{
			return TargetBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001C501 File Offset: 0x0001A701
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new TargetElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001C51C File Offset: 0x0001A71C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x04000247 RID: 583
		public const int Index = 199;

		// Token: 0x04000248 RID: 584
		public BoundingBox[][] m_boundingBoxes = new BoundingBox[][]
		{
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.0625f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.0625f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0.9375f), new Vector3(1f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0.9375f, 0f, 0f), new Vector3(1f, 1f, 1f))
			}
		};
	}
}
