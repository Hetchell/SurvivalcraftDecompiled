using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000066 RID: 102
	public class FireBlock : Block
	{
		// Token: 0x06000212 RID: 530 RVA: 0x0000C744 File Offset: 0x0000A944
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			TerrainGeometrySubset[] alphaTestSubsetsByFace = geometry.AlphaTestSubsetsByFace;
			int data = Terrain.ExtractData(value);
			int value2 = (y + 1 < 256) ? generator.Terrain.GetCellValueFast(x, y + 1, z) : 0;
			int num = Terrain.ExtractContents(value2);
			int data2 = Terrain.ExtractData(value2);
			int value3 = (y + 2 < 256) ? generator.Terrain.GetCellValueFast(x, y + 2, z) : 0;
			int num2 = Terrain.ExtractContents(value3);
			int data3 = Terrain.ExtractData(value3);
			if (FireBlock.HasFireOnFace(data, 0))
			{
				int value4 = (y + 1 < 256) ? generator.Terrain.GetCellValueFast(x, y + 1, z + 1) : 0;
				int num3 = Terrain.ExtractContents(value4);
				int data4 = Terrain.ExtractData(value4);
				int value5 = (y + 2 < 256) ? generator.Terrain.GetCellValueFast(x, y + 2, z + 1) : 0;
				int num4 = Terrain.ExtractContents(value5);
				int data5 = Terrain.ExtractData(value5);
				int num5 = this.DefaultTextureSlot;
				if ((num == 104 && FireBlock.HasFireOnFace(data2, 0)) || (num3 == 104 && FireBlock.HasFireOnFace(data4, 2)))
				{
					num5 += 16;
					if ((num2 == 104 && FireBlock.HasFireOnFace(data3, 0)) || (num4 == 104 && FireBlock.HasFireOnFace(data5, 2)))
					{
						num5 += 16;
					}
				}
				DynamicArray<TerrainVertex> vertices = alphaTestSubsetsByFace[0].Vertices;
				DynamicArray<ushort> indices = alphaTestSubsetsByFace[0].Indices;
				int count = vertices.Count;
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), Color.White, num5, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), Color.White, num5, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), Color.White, num5, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), Color.White, num5, 3, ref vertices.Array[count + 3]);
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
			}
			if (FireBlock.HasFireOnFace(data, 1))
			{
				int value6 = (y + 1 < 256) ? generator.Terrain.GetCellValueFast(x + 1, y + 1, z) : 0;
				int num6 = Terrain.ExtractContents(value6);
				int data6 = Terrain.ExtractData(value6);
				int value7 = (y + 2 < 256) ? generator.Terrain.GetCellValueFast(x + 1, y + 2, z) : 0;
				int num7 = Terrain.ExtractContents(value7);
				int data7 = Terrain.ExtractData(value7);
				int num8 = this.DefaultTextureSlot;
				if ((num == 104 && FireBlock.HasFireOnFace(data2, 1)) || (num6 == 104 && FireBlock.HasFireOnFace(data6, 3)))
				{
					num8 += 16;
					if ((num2 == 104 && FireBlock.HasFireOnFace(data3, 1)) || (num7 == 104 && FireBlock.HasFireOnFace(data7, 3)))
					{
						num8 += 16;
					}
				}
				DynamicArray<TerrainVertex> vertices2 = alphaTestSubsetsByFace[1].Vertices;
				DynamicArray<ushort> indices2 = alphaTestSubsetsByFace[1].Indices;
				int count2 = vertices2.Count;
				vertices2.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, Color.White, num8, 0, ref vertices2.Array[count2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, Color.White, num8, 3, ref vertices2.Array[count2 + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), Color.White, num8, 2, ref vertices2.Array[count2 + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), Color.White, num8, 1, ref vertices2.Array[count2 + 3]);
				indices2.Add((ushort)count2);
				indices2.Add((ushort)(count2 + 1));
				indices2.Add((ushort)(count2 + 2));
				indices2.Add((ushort)(count2 + 2));
				indices2.Add((ushort)(count2 + 1));
				indices2.Add((ushort)count2);
				indices2.Add((ushort)(count2 + 2));
				indices2.Add((ushort)(count2 + 3));
				indices2.Add((ushort)count2);
				indices2.Add((ushort)count2);
				indices2.Add((ushort)(count2 + 3));
				indices2.Add((ushort)(count2 + 2));
			}
			if (FireBlock.HasFireOnFace(data, 2))
			{
				int value8 = (y + 1 < 256) ? generator.Terrain.GetCellValueFast(x, y + 1, z - 1) : 0;
				int num9 = Terrain.ExtractContents(value8);
				int data8 = Terrain.ExtractData(value8);
				int value9 = (y + 2 < 256) ? generator.Terrain.GetCellValueFast(x, y + 2, z - 1) : 0;
				int num10 = Terrain.ExtractContents(value9);
				int data9 = Terrain.ExtractData(value9);
				int num11 = this.DefaultTextureSlot;
				if ((num == 104 && FireBlock.HasFireOnFace(data2, 2)) || (num9 == 104 && FireBlock.HasFireOnFace(data8, 0)))
				{
					num11 += 16;
					if ((num2 == 104 && FireBlock.HasFireOnFace(data3, 2)) || (num10 == 104 && FireBlock.HasFireOnFace(data9, 0)))
					{
						num11 += 16;
					}
				}
				DynamicArray<TerrainVertex> vertices3 = alphaTestSubsetsByFace[2].Vertices;
				DynamicArray<ushort> indices3 = alphaTestSubsetsByFace[2].Indices;
				int count3 = vertices3.Count;
				vertices3.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, Color.White, num11, 0, ref vertices3.Array[count3]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, Color.White, num11, 1, ref vertices3.Array[count3 + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, Color.White, num11, 2, ref vertices3.Array[count3 + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, Color.White, num11, 3, ref vertices3.Array[count3 + 3]);
				indices3.Add((ushort)count3);
				indices3.Add((ushort)(count3 + 2));
				indices3.Add((ushort)(count3 + 1));
				indices3.Add((ushort)(count3 + 1));
				indices3.Add((ushort)(count3 + 2));
				indices3.Add((ushort)count3);
				indices3.Add((ushort)(count3 + 2));
				indices3.Add((ushort)count3);
				indices3.Add((ushort)(count3 + 3));
				indices3.Add((ushort)(count3 + 3));
				indices3.Add((ushort)count3);
				indices3.Add((ushort)(count3 + 2));
			}
			if (!FireBlock.HasFireOnFace(data, 3))
			{
				return;
			}
			int value10 = (y + 1 < 256) ? generator.Terrain.GetCellValueFast(x - 1, y + 1, z) : 0;
			int num12 = Terrain.ExtractContents(value10);
			int data10 = Terrain.ExtractData(value10);
			int value11 = (y + 2 < 256) ? generator.Terrain.GetCellValueFast(x - 1, y + 2, z) : 0;
			int num13 = Terrain.ExtractContents(value11);
			int data11 = Terrain.ExtractData(value11);
			int num14 = this.DefaultTextureSlot;
			if ((num == 104 && FireBlock.HasFireOnFace(data2, 3)) || (num12 == 104 && FireBlock.HasFireOnFace(data10, 1)))
			{
				num14 += 16;
				if ((num2 == 104 && FireBlock.HasFireOnFace(data3, 3)) || (num13 == 104 && FireBlock.HasFireOnFace(data11, 1)))
				{
					num14 += 16;
				}
			}
			DynamicArray<TerrainVertex> vertices4 = alphaTestSubsetsByFace[3].Vertices;
			DynamicArray<ushort> indices4 = alphaTestSubsetsByFace[3].Indices;
			int count4 = vertices4.Count;
			vertices4.Count += 4;
			BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, Color.White, num14, 0, ref vertices4.Array[count4]);
			BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, Color.White, num14, 3, ref vertices4.Array[count4 + 1]);
			BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), Color.White, num14, 2, ref vertices4.Array[count4 + 2]);
			BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), Color.White, num14, 1, ref vertices4.Array[count4 + 3]);
			indices4.Add((ushort)count4);
			indices4.Add((ushort)(count4 + 2));
			indices4.Add((ushort)(count4 + 1));
			indices4.Add((ushort)(count4 + 1));
			indices4.Add((ushort)(count4 + 2));
			indices4.Add((ushort)count4);
			indices4.Add((ushort)(count4 + 2));
			indices4.Add((ushort)count4);
			indices4.Add((ushort)(count4 + 3));
			indices4.Add((ushort)(count4 + 3));
			indices4.Add((ushort)count4);
			indices4.Add((ushort)(count4 + 2));
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000CFE0 File Offset: 0x0000B1E0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000CFE2 File Offset: 0x0000B1E2
		public override bool ShouldAvoid(int value)
		{
			return true;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000CFE5 File Offset: 0x0000B1E5
		public static bool HasFireOnFace(int data, int face)
		{
			return data == 0 || (data & 1 << face) != 0;
		}

		// Token: 0x040000FE RID: 254
		public const int Index = 104;
	}
}
