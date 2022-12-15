using System;
using System.Runtime.CompilerServices;
using Engine;

namespace Game
{
	// Token: 0x02000227 RID: 551
	public class BlockGeometryGenerator
	{
		// Token: 0x060010E7 RID: 4327 RVA: 0x0007FECC File Offset: 0x0007E0CC
		public BlockGeometryGenerator(Terrain terrain, SubsystemTerrain subsystemTerrain, SubsystemElectricity subsystemElectricity, SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior, SubsystemMetersBlockBehavior subsystemMetersBlockBehavior, SubsystemPalette subsystemPalette)
		{
			this.Terrain = terrain;
			this.SubsystemTerrain = subsystemTerrain;
			this.SubsystemElectricity = subsystemElectricity;
			this.SubsystemFurnitureBlockBehavior = subsystemFurnitureBlockBehavior;
			this.SubsystemMetersBlockBehavior = subsystemMetersBlockBehavior;
			this.SubsystemPalette = subsystemPalette;
			this.ResetCache();
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0007FF35 File Offset: 0x0007E135
		public void ResetCache()
		{
			this.m_cornerLightsPosition = new Point3(int.MaxValue);
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0007FF48 File Offset: 0x0007E148
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetupCornerVertex(float x, float y, float z, Color color, int light, int face, int textureSlot, int corner, ref TerrainVertex vertex)
		{
			float num = LightingManager.LightIntensityByLightValueAndFace[light + 16 * face];
			Color color2 = new Color((byte)((float)color.R * num), (byte)((float)color.G * num), (byte)((float)color.B * num), color.A);
			float tx = (BlockGeometryGenerator.m_textureCoordinates[corner].X + (float)(textureSlot % 16)) / 16f;
			float ty = (BlockGeometryGenerator.m_textureCoordinates[corner].Y + (float)(textureSlot / 16)) / 16f;
			BlockGeometryGenerator.SetupVertex(x, y, z, color2, tx, ty, ref vertex);
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x0007FFE0 File Offset: 0x0007E1E0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetupLitCornerVertex(float x, float y, float z, Color color, int textureSlot, int corner, ref TerrainVertex vertex)
		{
			float tx = (BlockGeometryGenerator.m_textureCoordinates[corner].X + (float)(textureSlot % 16)) / 16f;
			float ty = (BlockGeometryGenerator.m_textureCoordinates[corner].Y + (float)(textureSlot / 16)) / 16f;
			BlockGeometryGenerator.SetupVertex(x, y, z, color, tx, ty, ref vertex);
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00080038 File Offset: 0x0007E238
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetupVertex(float x, float y, float z, Color color, float tx, float ty, ref TerrainVertex vertex)
		{
			vertex.X = x;
			vertex.Y = y;
			vertex.Z = z;
			vertex.Tx = (short)(tx * 32767f);
			vertex.Ty = (short)(ty * 32767f);
			vertex.Color = color;
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x00080088 File Offset: 0x0007E288
		public void GenerateCrossfaceVertices(Block block, int value, int x, int y, int z, Color color, int textureSlot, TerrainGeometrySubset subset)
		{
			DynamicArray<TerrainVertex> vertices = subset.Vertices;
			DynamicArray<ushort> indices = subset.Indices;
			int num = Terrain.ExtractLight(value);
			float num2 = LightingManager.LightIntensityByLightValueAndFace[num + 64];
			Color color2 = new Color((byte)((float)color.R * num2), (byte)((float)color.G * num2), (byte)((float)color.B * num2), color.A);
			int count = vertices.Count;
			vertices.Count += 8;
			if ((x & 1) == 0)
			{
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color2, textureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color2, textureSlot, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color2, textureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color2, textureSlot, 3, ref vertices.Array[count + 3]);
			}
			else
			{
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color2, textureSlot, 1, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color2, textureSlot, 0, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color2, textureSlot, 3, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color2, textureSlot, 2, ref vertices.Array[count + 3]);
			}
			if ((z & 1) == 0)
			{
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color2, textureSlot, 0, ref vertices.Array[count + 4]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color2, textureSlot, 1, ref vertices.Array[count + 5]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color2, textureSlot, 2, ref vertices.Array[count + 6]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color2, textureSlot, 3, ref vertices.Array[count + 7]);
			}
			else
			{
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color2, textureSlot, 1, ref vertices.Array[count + 4]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color2, textureSlot, 0, ref vertices.Array[count + 5]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color2, textureSlot, 3, ref vertices.Array[count + 6]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color2, textureSlot, 2, ref vertices.Array[count + 7]);
			}
			int count2 = indices.Count;
			indices.Count += 24;
			indices.Array[count2] = (ushort)count;
			indices.Array[count2 + 1] = (ushort)(count + 1);
			indices.Array[count2 + 2] = (ushort)(count + 2);
			indices.Array[count2 + 3] = (ushort)(count + 2);
			indices.Array[count2 + 4] = (ushort)(count + 1);
			indices.Array[count2 + 5] = (ushort)count;
			indices.Array[count2 + 6] = (ushort)(count + 2);
			indices.Array[count2 + 7] = (ushort)(count + 3);
			indices.Array[count2 + 8] = (ushort)count;
			indices.Array[count2 + 9] = (ushort)count;
			indices.Array[count2 + 10] = (ushort)(count + 3);
			indices.Array[count2 + 11] = (ushort)(count + 2);
			indices.Array[count2 + 12] = (ushort)(count + 4);
			indices.Array[count2 + 13] = (ushort)(count + 5);
			indices.Array[count2 + 14] = (ushort)(count + 6);
			indices.Array[count2 + 15] = (ushort)(count + 6);
			indices.Array[count2 + 16] = (ushort)(count + 5);
			indices.Array[count2 + 17] = (ushort)(count + 4);
			indices.Array[count2 + 18] = (ushort)(count + 6);
			indices.Array[count2 + 19] = (ushort)(count + 7);
			indices.Array[count2 + 20] = (ushort)(count + 4);
			indices.Array[count2 + 21] = (ushort)(count + 4);
			indices.Array[count2 + 22] = (ushort)(count + 7);
			indices.Array[count2 + 23] = (ushort)(count + 6);
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x000804E0 File Offset: 0x0007E6E0
		public void GenerateCubeVertices(Block block, int value, int x, int y, int z, Color color, TerrainGeometrySubset[] subsetsByFace)
		{
			int blockIndex = block.BlockIndex;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z + 1);
			TerrainChunk chunkAtCell3 = this.Terrain.GetChunkAtCell(x + 1, z);
			TerrainChunk chunkAtCell4 = this.Terrain.GetChunkAtCell(x, z - 1);
			TerrainChunk chunkAtCell5 = this.Terrain.GetChunkAtCell(x - 1, z);
			int cellValueFast = chunkAtCell2.GetCellValueFast(x & 15, y, z + 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 0, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices = subsetsByFace[0].Vertices;
				DynamicArray<ushort> indices = subsetsByFace[0].Indices;
				int faceTextureSlot = block.GetFaceTextureSlot(0, value);
				int count = vertices.Count;
				vertices.Count += 4;
				this.SetupCubeVertexFace0(x, y, z + 1, 1f, 0, faceTextureSlot, color, ref vertices.Array[count]);
				this.SetupCubeVertexFace0(x + 1, y, z + 1, 1f, 1, faceTextureSlot, color, ref vertices.Array[count + 1]);
				this.SetupCubeVertexFace0(x + 1, y + 1, z + 1, 1f, 2, faceTextureSlot, color, ref vertices.Array[count + 2]);
				this.SetupCubeVertexFace0(x, y + 1, z + 1, 1f, 3, faceTextureSlot, color, ref vertices.Array[count + 3]);
				int count2 = indices.Count;
				indices.Count += 6;
				indices.Array[count2] = (ushort)count;
				indices.Array[count2 + 1] = (ushort)(count + 2);
				indices.Array[count2 + 2] = (ushort)(count + 1);
				indices.Array[count2 + 3] = (ushort)(count + 2);
				indices.Array[count2 + 4] = (ushort)count;
				indices.Array[count2 + 5] = (ushort)(count + 3);
			}
			cellValueFast = chunkAtCell3.GetCellValueFast(x + 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 1, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices2 = subsetsByFace[1].Vertices;
				DynamicArray<ushort> indices2 = subsetsByFace[1].Indices;
				int faceTextureSlot2 = block.GetFaceTextureSlot(1, value);
				int count3 = vertices2.Count;
				vertices2.Count += 4;
				this.SetupCubeVertexFace1(x + 1, y, z, 1f, 1, faceTextureSlot2, color, ref vertices2.Array[count3]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z, 1f, 2, faceTextureSlot2, color, ref vertices2.Array[count3 + 1]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z + 1, 1f, 3, faceTextureSlot2, color, ref vertices2.Array[count3 + 2]);
				this.SetupCubeVertexFace1(x + 1, y, z + 1, 1f, 0, faceTextureSlot2, color, ref vertices2.Array[count3 + 3]);
				int count4 = indices2.Count;
				indices2.Count += 6;
				indices2.Array[count4] = (ushort)count3;
				indices2.Array[count4 + 1] = (ushort)(count3 + 2);
				indices2.Array[count4 + 2] = (ushort)(count3 + 1);
				indices2.Array[count4 + 3] = (ushort)(count3 + 2);
				indices2.Array[count4 + 4] = (ushort)count3;
				indices2.Array[count4 + 5] = (ushort)(count3 + 3);
			}
			cellValueFast = chunkAtCell4.GetCellValueFast(x & 15, y, z - 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 2, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices3 = subsetsByFace[2].Vertices;
				DynamicArray<ushort> indices3 = subsetsByFace[2].Indices;
				int faceTextureSlot3 = block.GetFaceTextureSlot(2, value);
				int count5 = vertices3.Count;
				vertices3.Count += 4;
				this.SetupCubeVertexFace2(x, y, z, 1f, 1, faceTextureSlot3, color, ref vertices3.Array[count5]);
				this.SetupCubeVertexFace2(x + 1, y, z, 1f, 0, faceTextureSlot3, color, ref vertices3.Array[count5 + 1]);
				this.SetupCubeVertexFace2(x + 1, y + 1, z, 1f, 3, faceTextureSlot3, color, ref vertices3.Array[count5 + 2]);
				this.SetupCubeVertexFace2(x, y + 1, z, 1f, 2, faceTextureSlot3, color, ref vertices3.Array[count5 + 3]);
				int count6 = indices3.Count;
				indices3.Count += 6;
				indices3.Array[count6] = (ushort)count5;
				indices3.Array[count6 + 1] = (ushort)(count5 + 1);
				indices3.Array[count6 + 2] = (ushort)(count5 + 2);
				indices3.Array[count6 + 3] = (ushort)(count5 + 2);
				indices3.Array[count6 + 4] = (ushort)(count5 + 3);
				indices3.Array[count6 + 5] = (ushort)count5;
			}
			cellValueFast = chunkAtCell5.GetCellValueFast(x - 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 3, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices4 = subsetsByFace[3].Vertices;
				DynamicArray<ushort> indices4 = subsetsByFace[3].Indices;
				int faceTextureSlot4 = block.GetFaceTextureSlot(3, value);
				int count7 = vertices4.Count;
				vertices4.Count += 4;
				this.SetupCubeVertexFace3(x, y, z, 1f, 0, faceTextureSlot4, color, ref vertices4.Array[count7]);
				this.SetupCubeVertexFace3(x, y + 1, z, 1f, 3, faceTextureSlot4, color, ref vertices4.Array[count7 + 1]);
				this.SetupCubeVertexFace3(x, y + 1, z + 1, 1f, 2, faceTextureSlot4, color, ref vertices4.Array[count7 + 2]);
				this.SetupCubeVertexFace3(x, y, z + 1, 1f, 1, faceTextureSlot4, color, ref vertices4.Array[count7 + 3]);
				int count8 = indices4.Count;
				indices4.Count += 6;
				indices4.Array[count8] = (ushort)count7;
				indices4.Array[count8 + 1] = (ushort)(count7 + 1);
				indices4.Array[count8 + 2] = (ushort)(count7 + 2);
				indices4.Array[count8 + 3] = (ushort)(count7 + 2);
				indices4.Array[count8 + 4] = (ushort)(count7 + 3);
				indices4.Array[count8 + 5] = (ushort)count7;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y + 1, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 4, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices5 = subsetsByFace[4].Vertices;
				DynamicArray<ushort> indices5 = subsetsByFace[4].Indices;
				int faceTextureSlot5 = block.GetFaceTextureSlot(4, value);
				int count9 = vertices5.Count;
				vertices5.Count += 4;
				this.SetupCubeVertexFace4(x, y + 1, z, 1f, 3, faceTextureSlot5, color, ref vertices5.Array[count9]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z, 1f, 2, faceTextureSlot5, color, ref vertices5.Array[count9 + 1]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z + 1, 1f, 1, faceTextureSlot5, color, ref vertices5.Array[count9 + 2]);
				this.SetupCubeVertexFace4(x, y + 1, z + 1, 1f, 0, faceTextureSlot5, color, ref vertices5.Array[count9 + 3]);
				int count10 = indices5.Count;
				indices5.Count += 6;
				indices5.Array[count10] = (ushort)count9;
				indices5.Array[count10 + 1] = (ushort)(count9 + 1);
				indices5.Array[count10 + 2] = (ushort)(count9 + 2);
				indices5.Array[count10 + 3] = (ushort)(count9 + 2);
				indices5.Array[count10 + 4] = (ushort)(count9 + 3);
				indices5.Array[count10 + 5] = (ushort)count9;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y - 1, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 5, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices6 = subsetsByFace[5].Vertices;
				DynamicArray<ushort> indices6 = subsetsByFace[5].Indices;
				int faceTextureSlot6 = block.GetFaceTextureSlot(5, value);
				int count11 = vertices6.Count;
				vertices6.Count += 4;
				this.SetupCubeVertexFace5(x, y, z, 1f, 0, faceTextureSlot6, color, ref vertices6.Array[count11]);
				this.SetupCubeVertexFace5(x + 1, y, z, 1f, 1, faceTextureSlot6, color, ref vertices6.Array[count11 + 1]);
				this.SetupCubeVertexFace5(x + 1, y, z + 1, 1f, 2, faceTextureSlot6, color, ref vertices6.Array[count11 + 2]);
				this.SetupCubeVertexFace5(x, y, z + 1, 1f, 3, faceTextureSlot6, color, ref vertices6.Array[count11 + 3]);
				int count12 = indices6.Count;
				indices6.Count += 6;
				indices6.Array[count12] = (ushort)count11;
				indices6.Array[count12 + 1] = (ushort)(count11 + 2);
				indices6.Array[count12 + 2] = (ushort)(count11 + 1);
				indices6.Array[count12 + 3] = (ushort)(count11 + 2);
				indices6.Array[count12 + 4] = (ushort)count11;
				indices6.Array[count12 + 5] = (ushort)(count11 + 3);
			}
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00080E20 File Offset: 0x0007F020
		public void GenerateCubeVertices(Block block, int value, int x, int y, int z, float height11, float height21, float height22, float height12, Color sideColor, Color topColor11, Color topColor21, Color topColor22, Color topColor12, int overrideTopTextureSlot, TerrainGeometrySubset[] subsetsByFace)
		{
			int blockIndex = block.BlockIndex;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z + 1);
			TerrainChunk chunkAtCell3 = this.Terrain.GetChunkAtCell(x + 1, z);
			TerrainChunk chunkAtCell4 = this.Terrain.GetChunkAtCell(x, z - 1);
			TerrainChunk chunkAtCell5 = this.Terrain.GetChunkAtCell(x - 1, z);
			int cellValueFast = chunkAtCell2.GetCellValueFast(x & 15, y, z + 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 0, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices = subsetsByFace[0].Vertices;
				DynamicArray<ushort> indices = subsetsByFace[0].Indices;
				int faceTextureSlot = block.GetFaceTextureSlot(0, value);
				int count = vertices.Count;
				vertices.Count += 4;
				this.SetupCubeVertexFace0(x, y, z + 1, 1f, 0, faceTextureSlot, sideColor, ref vertices.Array[count]);
				this.SetupCubeVertexFace0(x + 1, y, z + 1, 1f, 1, faceTextureSlot, sideColor, ref vertices.Array[count + 1]);
				this.SetupCubeVertexFace0(x + 1, y + 1, z + 1, height22, 2, faceTextureSlot, sideColor, ref vertices.Array[count + 2]);
				this.SetupCubeVertexFace0(x, y + 1, z + 1, height12, 3, faceTextureSlot, sideColor, ref vertices.Array[count + 3]);
				int count2 = indices.Count;
				indices.Count += 6;
				indices.Array[count2] = (ushort)count;
				indices.Array[count2 + 1] = (ushort)(count + 2);
				indices.Array[count2 + 2] = (ushort)(count + 1);
				indices.Array[count2 + 3] = (ushort)(count + 2);
				indices.Array[count2 + 4] = (ushort)count;
				indices.Array[count2 + 5] = (ushort)(count + 3);
			}
			cellValueFast = chunkAtCell3.GetCellValueFast(x + 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 1, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices2 = subsetsByFace[1].Vertices;
				DynamicArray<ushort> indices2 = subsetsByFace[1].Indices;
				int faceTextureSlot2 = block.GetFaceTextureSlot(1, value);
				int count3 = vertices2.Count;
				vertices2.Count += 4;
				this.SetupCubeVertexFace1(x + 1, y, z, 1f, 1, faceTextureSlot2, sideColor, ref vertices2.Array[count3]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z, height21, 2, faceTextureSlot2, sideColor, ref vertices2.Array[count3 + 1]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z + 1, height22, 3, faceTextureSlot2, sideColor, ref vertices2.Array[count3 + 2]);
				this.SetupCubeVertexFace1(x + 1, y, z + 1, 1f, 0, faceTextureSlot2, sideColor, ref vertices2.Array[count3 + 3]);
				int count4 = indices2.Count;
				indices2.Count += 6;
				indices2.Array[count4] = (ushort)count3;
				indices2.Array[count4 + 1] = (ushort)(count3 + 2);
				indices2.Array[count4 + 2] = (ushort)(count3 + 1);
				indices2.Array[count4 + 3] = (ushort)(count3 + 2);
				indices2.Array[count4 + 4] = (ushort)count3;
				indices2.Array[count4 + 5] = (ushort)(count3 + 3);
			}
			cellValueFast = chunkAtCell4.GetCellValueFast(x & 15, y, z - 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 2, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices3 = subsetsByFace[2].Vertices;
				DynamicArray<ushort> indices3 = subsetsByFace[2].Indices;
				int faceTextureSlot3 = block.GetFaceTextureSlot(2, value);
				int count5 = vertices3.Count;
				vertices3.Count += 4;
				this.SetupCubeVertexFace2(x, y, z, 1f, 1, faceTextureSlot3, sideColor, ref vertices3.Array[count5]);
				this.SetupCubeVertexFace2(x + 1, y, z, 1f, 0, faceTextureSlot3, sideColor, ref vertices3.Array[count5 + 1]);
				this.SetupCubeVertexFace2(x + 1, y + 1, z, height21, 3, faceTextureSlot3, sideColor, ref vertices3.Array[count5 + 2]);
				this.SetupCubeVertexFace2(x, y + 1, z, height11, 2, faceTextureSlot3, sideColor, ref vertices3.Array[count5 + 3]);
				int count6 = indices3.Count;
				indices3.Count += 6;
				indices3.Array[count6] = (ushort)count5;
				indices3.Array[count6 + 1] = (ushort)(count5 + 1);
				indices3.Array[count6 + 2] = (ushort)(count5 + 2);
				indices3.Array[count6 + 3] = (ushort)(count5 + 2);
				indices3.Array[count6 + 4] = (ushort)(count5 + 3);
				indices3.Array[count6 + 5] = (ushort)count5;
			}
			cellValueFast = chunkAtCell5.GetCellValueFast(x - 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 3, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices4 = subsetsByFace[3].Vertices;
				DynamicArray<ushort> indices4 = subsetsByFace[3].Indices;
				int faceTextureSlot4 = block.GetFaceTextureSlot(3, value);
				int count7 = vertices4.Count;
				vertices4.Count += 4;
				this.SetupCubeVertexFace3(x, y, z, 1f, 0, faceTextureSlot4, sideColor, ref vertices4.Array[count7]);
				this.SetupCubeVertexFace3(x, y + 1, z, height11, 3, faceTextureSlot4, sideColor, ref vertices4.Array[count7 + 1]);
				this.SetupCubeVertexFace3(x, y + 1, z + 1, height12, 2, faceTextureSlot4, sideColor, ref vertices4.Array[count7 + 2]);
				this.SetupCubeVertexFace3(x, y, z + 1, 1f, 1, faceTextureSlot4, sideColor, ref vertices4.Array[count7 + 3]);
				int count8 = indices4.Count;
				indices4.Count += 6;
				indices4.Array[count8] = (ushort)count7;
				indices4.Array[count8 + 1] = (ushort)(count7 + 1);
				indices4.Array[count8 + 2] = (ushort)(count7 + 2);
				indices4.Array[count8 + 3] = (ushort)(count7 + 2);
				indices4.Array[count8 + 4] = (ushort)(count7 + 3);
				indices4.Array[count8 + 5] = (ushort)count7;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y + 1, z & 15);
			if ((Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 4, value, cellValueFast)) || height11 < 1f || height12 < 1f || height21 < 1f || height22 < 1f)
			{
				DynamicArray<TerrainVertex> vertices5 = subsetsByFace[4].Vertices;
				DynamicArray<ushort> indices5 = subsetsByFace[4].Indices;
				int textureSlot = (overrideTopTextureSlot >= 0) ? overrideTopTextureSlot : block.GetFaceTextureSlot(4, value);
				int count9 = vertices5.Count;
				vertices5.Count += 4;
				this.SetupCubeVertexFace4(x, y + 1, z, height11, 3, textureSlot, topColor11, ref vertices5.Array[count9]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z, height21, 2, textureSlot, topColor21, ref vertices5.Array[count9 + 1]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z + 1, height22, 1, textureSlot, topColor22, ref vertices5.Array[count9 + 2]);
				this.SetupCubeVertexFace4(x, y + 1, z + 1, height12, 0, textureSlot, topColor12, ref vertices5.Array[count9 + 3]);
				int count10 = indices5.Count;
				indices5.Count += 6;
				indices5.Array[count10] = (ushort)count9;
				indices5.Array[count10 + 1] = (ushort)(count9 + 1);
				indices5.Array[count10 + 2] = (ushort)(count9 + 2);
				indices5.Array[count10 + 3] = (ushort)(count9 + 2);
				indices5.Array[count10 + 4] = (ushort)(count9 + 3);
				indices5.Array[count10 + 5] = (ushort)count9;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y - 1, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 5, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices6 = subsetsByFace[5].Vertices;
				DynamicArray<ushort> indices6 = subsetsByFace[5].Indices;
				int faceTextureSlot5 = block.GetFaceTextureSlot(5, value);
				int count11 = vertices6.Count;
				vertices6.Count += 4;
				this.SetupCubeVertexFace5(x, y, z, 1f, 0, faceTextureSlot5, sideColor, ref vertices6.Array[count11]);
				this.SetupCubeVertexFace5(x + 1, y, z, 1f, 1, faceTextureSlot5, sideColor, ref vertices6.Array[count11 + 1]);
				this.SetupCubeVertexFace5(x + 1, y, z + 1, 1f, 2, faceTextureSlot5, sideColor, ref vertices6.Array[count11 + 2]);
				this.SetupCubeVertexFace5(x, y, z + 1, 1f, 3, faceTextureSlot5, sideColor, ref vertices6.Array[count11 + 3]);
				int count12 = indices6.Count;
				indices6.Count += 6;
				indices6.Array[count12] = (ushort)count11;
				indices6.Array[count12 + 1] = (ushort)(count11 + 2);
				indices6.Array[count12 + 2] = (ushort)(count11 + 1);
				indices6.Array[count12 + 3] = (ushort)(count11 + 2);
				indices6.Array[count12 + 4] = (ushort)count11;
				indices6.Array[count12 + 5] = (ushort)(count11 + 3);
			}
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x00081768 File Offset: 0x0007F968
		public void GenerateCubeVertices(Block block, int value, int x, int y, int z, int rotationX, int rotationY, int rotationZ, Color color, TerrainGeometrySubset[] subsetsByFace)
		{
			int blockIndex = block.BlockIndex;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z + 1);
			TerrainChunk chunkAtCell3 = this.Terrain.GetChunkAtCell(x + 1, z);
			TerrainChunk chunkAtCell4 = this.Terrain.GetChunkAtCell(x, z - 1);
			TerrainChunk chunkAtCell5 = this.Terrain.GetChunkAtCell(x - 1, z);
			int cellValueFast = chunkAtCell2.GetCellValueFast(x & 15, y, z + 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 0, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices = subsetsByFace[0].Vertices;
				DynamicArray<ushort> indices = subsetsByFace[0].Indices;
				int faceTextureSlot = block.GetFaceTextureSlot(0, value);
				int count = vertices.Count;
				vertices.Count += 4;
				this.SetupCubeVertexFace0(x, y, z + 1, 1f, rotationZ, faceTextureSlot, color, ref vertices.Array[count]);
				this.SetupCubeVertexFace0(x + 1, y, z + 1, 1f, 1 + rotationZ, faceTextureSlot, color, ref vertices.Array[count + 1]);
				this.SetupCubeVertexFace0(x + 1, y + 1, z + 1, 1f, 2 + rotationZ, faceTextureSlot, color, ref vertices.Array[count + 2]);
				this.SetupCubeVertexFace0(x, y + 1, z + 1, 1f, 3 + rotationZ, faceTextureSlot, color, ref vertices.Array[count + 3]);
				int count2 = indices.Count;
				indices.Count += 6;
				indices.Array[count2] = (ushort)count;
				indices.Array[count2 + 1] = (ushort)(count + 2);
				indices.Array[count2 + 2] = (ushort)(count + 1);
				indices.Array[count2 + 3] = (ushort)(count + 2);
				indices.Array[count2 + 4] = (ushort)count;
				indices.Array[count2 + 5] = (ushort)(count + 3);
			}
			cellValueFast = chunkAtCell3.GetCellValueFast(x + 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 1, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices2 = subsetsByFace[1].Vertices;
				DynamicArray<ushort> indices2 = subsetsByFace[1].Indices;
				int faceTextureSlot2 = block.GetFaceTextureSlot(1, value);
				int count3 = vertices2.Count;
				vertices2.Count += 4;
				this.SetupCubeVertexFace1(x + 1, y, z, 1f, 1 + rotationX, faceTextureSlot2, color, ref vertices2.Array[count3]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z, 1f, 2 + rotationX, faceTextureSlot2, color, ref vertices2.Array[count3 + 1]);
				this.SetupCubeVertexFace1(x + 1, y + 1, z + 1, 1f, 3 + rotationX, faceTextureSlot2, color, ref vertices2.Array[count3 + 2]);
				this.SetupCubeVertexFace1(x + 1, y, z + 1, 1f, rotationX, faceTextureSlot2, color, ref vertices2.Array[count3 + 3]);
				int count4 = indices2.Count;
				indices2.Count += 6;
				indices2.Array[count4] = (ushort)count3;
				indices2.Array[count4 + 1] = (ushort)(count3 + 2);
				indices2.Array[count4 + 2] = (ushort)(count3 + 1);
				indices2.Array[count4 + 3] = (ushort)(count3 + 2);
				indices2.Array[count4 + 4] = (ushort)count3;
				indices2.Array[count4 + 5] = (ushort)(count3 + 3);
			}
			cellValueFast = chunkAtCell4.GetCellValueFast(x & 15, y, z - 1 & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 2, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices3 = subsetsByFace[2].Vertices;
				DynamicArray<ushort> indices3 = subsetsByFace[2].Indices;
				int faceTextureSlot3 = block.GetFaceTextureSlot(2, value);
				int count5 = vertices3.Count;
				vertices3.Count += 4;
				this.SetupCubeVertexFace2(x, y, z, 1f, 1 + rotationZ, faceTextureSlot3, color, ref vertices3.Array[count5]);
				this.SetupCubeVertexFace2(x + 1, y, z, 1f, rotationZ, faceTextureSlot3, color, ref vertices3.Array[count5 + 1]);
				this.SetupCubeVertexFace2(x + 1, y + 1, z, 1f, 3 + rotationZ, faceTextureSlot3, color, ref vertices3.Array[count5 + 2]);
				this.SetupCubeVertexFace2(x, y + 1, z, 1f, 2 + rotationZ, faceTextureSlot3, color, ref vertices3.Array[count5 + 3]);
				int count6 = indices3.Count;
				indices3.Count += 6;
				indices3.Array[count6] = (ushort)count5;
				indices3.Array[count6 + 1] = (ushort)(count5 + 1);
				indices3.Array[count6 + 2] = (ushort)(count5 + 2);
				indices3.Array[count6 + 3] = (ushort)(count5 + 2);
				indices3.Array[count6 + 4] = (ushort)(count5 + 3);
				indices3.Array[count6 + 5] = (ushort)count5;
			}
			cellValueFast = chunkAtCell5.GetCellValueFast(x - 1 & 15, y, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 3, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices4 = subsetsByFace[3].Vertices;
				DynamicArray<ushort> indices4 = subsetsByFace[3].Indices;
				int faceTextureSlot4 = block.GetFaceTextureSlot(3, value);
				int count7 = vertices4.Count;
				vertices4.Count += 4;
				this.SetupCubeVertexFace3(x, y, z, 1f, rotationX, faceTextureSlot4, color, ref vertices4.Array[count7]);
				this.SetupCubeVertexFace3(x, y + 1, z, 1f, 3 + rotationX, faceTextureSlot4, color, ref vertices4.Array[count7 + 1]);
				this.SetupCubeVertexFace3(x, y + 1, z + 1, 1f, 2 + rotationX, faceTextureSlot4, color, ref vertices4.Array[count7 + 2]);
				this.SetupCubeVertexFace3(x, y, z + 1, 1f, 1 + rotationX, faceTextureSlot4, color, ref vertices4.Array[count7 + 3]);
				int count8 = indices4.Count;
				indices4.Count += 6;
				indices4.Array[count8] = (ushort)count7;
				indices4.Array[count8 + 1] = (ushort)(count7 + 1);
				indices4.Array[count8 + 2] = (ushort)(count7 + 2);
				indices4.Array[count8 + 3] = (ushort)(count7 + 2);
				indices4.Array[count8 + 4] = (ushort)(count7 + 3);
				indices4.Array[count8 + 5] = (ushort)count7;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y + 1, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 4, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices5 = subsetsByFace[4].Vertices;
				DynamicArray<ushort> indices5 = subsetsByFace[4].Indices;
				int faceTextureSlot5 = block.GetFaceTextureSlot(4, value);
				int count9 = vertices5.Count;
				vertices5.Count += 4;
				this.SetupCubeVertexFace4(x, y + 1, z, 1f, 3 + rotationY, faceTextureSlot5, color, ref vertices5.Array[count9]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z, 1f, 2 + rotationY, faceTextureSlot5, color, ref vertices5.Array[count9 + 1]);
				this.SetupCubeVertexFace4(x + 1, y + 1, z + 1, 1f, 1 + rotationY, faceTextureSlot5, color, ref vertices5.Array[count9 + 2]);
				this.SetupCubeVertexFace4(x, y + 1, z + 1, 1f, rotationY, faceTextureSlot5, color, ref vertices5.Array[count9 + 3]);
				int count10 = indices5.Count;
				indices5.Count += 6;
				indices5.Array[count10] = (ushort)count9;
				indices5.Array[count10 + 1] = (ushort)(count9 + 1);
				indices5.Array[count10 + 2] = (ushort)(count9 + 2);
				indices5.Array[count10 + 3] = (ushort)(count9 + 2);
				indices5.Array[count10 + 4] = (ushort)(count9 + 3);
				indices5.Array[count10 + 5] = (ushort)count9;
			}
			cellValueFast = chunkAtCell.GetCellValueFast(x & 15, y - 1, z & 15);
			if (Terrain.ExtractContents(cellValueFast) != blockIndex && block.ShouldGenerateFace(this.SubsystemTerrain, 5, value, cellValueFast))
			{
				DynamicArray<TerrainVertex> vertices6 = subsetsByFace[5].Vertices;
				DynamicArray<ushort> indices6 = subsetsByFace[5].Indices;
				int faceTextureSlot6 = block.GetFaceTextureSlot(5, value);
				int count11 = vertices6.Count;
				vertices6.Count += 4;
				this.SetupCubeVertexFace5(x, y, z, 1f, rotationY, faceTextureSlot6, color, ref vertices6.Array[count11]);
				this.SetupCubeVertexFace5(x + 1, y, z, 1f, 1 + rotationY, faceTextureSlot6, color, ref vertices6.Array[count11 + 1]);
				this.SetupCubeVertexFace5(x + 1, y, z + 1, 1f, 2 + rotationY, faceTextureSlot6, color, ref vertices6.Array[count11 + 2]);
				this.SetupCubeVertexFace5(x, y, z + 1, 1f, 3 + rotationY, faceTextureSlot6, color, ref vertices6.Array[count11 + 3]);
				int count12 = indices6.Count;
				indices6.Count += 6;
				indices6.Array[count12] = (ushort)count11;
				indices6.Array[count12 + 1] = (ushort)(count11 + 2);
				indices6.Array[count12 + 2] = (ushort)(count11 + 1);
				indices6.Array[count12 + 3] = (ushort)(count11 + 2);
				indices6.Array[count12 + 4] = (ushort)count11;
				indices6.Array[count12 + 5] = (ushort)(count11 + 3);
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x000820E4 File Offset: 0x000802E4
		public void GenerateMeshVertices(Block block, int x, int y, int z, BlockMesh blockMesh, Color color, Matrix? matrix, TerrainGeometrySubset subset)
		{
			DynamicArray<TerrainVertex> vertices = subset.Vertices;
			DynamicArray<ushort> indices = subset.Indices;
			int count = vertices.Count;
			int cellLightFast = this.Terrain.GetCellLightFast(x, y, z);
			float num = LightingManager.LightIntensityByLightValue[cellLightFast];
			vertices.Count += blockMesh.Vertices.Count;
			for (int i = 0; i < blockMesh.Vertices.Count; i++)
			{
				BlockMeshVertex blockMeshVertex = blockMesh.Vertices.Array[i];
				Vector3 vector = blockMeshVertex.Position;
				if (matrix != null)
				{
					vector = Vector3.Transform(blockMeshVertex.Position, matrix.Value);
				}
				Color color2;
				if (blockMeshVertex.IsEmissive)
				{
					color2 = new Color(color.R * blockMeshVertex.Color.R / byte.MaxValue, color.G * blockMeshVertex.Color.G / byte.MaxValue, color.B * blockMeshVertex.Color.B / byte.MaxValue);
				}
				else
				{
					float num2 = num / 255f;
					color2 = new Color((byte)((float)(color.R * blockMeshVertex.Color.R) * num2), (byte)((float)(color.G * blockMeshVertex.Color.G) * num2), (byte)((float)(color.B * blockMeshVertex.Color.B) * num2));
				}
				BlockGeometryGenerator.SetupVertex((float)x + vector.X, (float)y + vector.Y, (float)z + vector.Z, color2, blockMeshVertex.TextureCoordinates.X, blockMeshVertex.TextureCoordinates.Y, ref vertices.Array[count + i]);
			}
			if (blockMesh.Sides != null)
			{
				for (int j = 0; j < 6; j++)
				{
					Point3 point = CellFace.FaceToPoint3(j);
					int cellValueFastChunkExists = this.Terrain.GetCellValueFastChunkExists(x + point.X, y + point.Y, z + point.Z);
					this.m_visibleSides[j] = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFastChunkExists)].IsFaceTransparent(this.SubsystemTerrain, CellFace.OppositeFace(j), cellValueFastChunkExists);
				}
				for (int k = 0; k < blockMesh.Indices.Count / 3; k++)
				{
					int num3 = (int)((blockMesh.Sides == null) ? -1 : blockMesh.Sides.Array[k]);
					if (num3 < 0 || this.m_visibleSides[num3])
					{
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k] + count));
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k + 1] + count));
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k + 2] + count));
					}
				}
				return;
			}
			for (int l = 0; l < blockMesh.Indices.Count; l++)
			{
				indices.Add((ushort)((int)blockMesh.Indices.Array[l] + count));
			}
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x000823F4 File Offset: 0x000805F4
		public void GenerateShadedMeshVertices(Block block, int x, int y, int z, BlockMesh blockMesh, Color color, Matrix? matrix, int[] facesMap, TerrainGeometrySubset subset)
		{
			this.CalculateCornerLights(x, y, z);
			DynamicArray<TerrainVertex> vertices = subset.Vertices;
			DynamicArray<ushort> indices = subset.Indices;
			int count = vertices.Count;
			vertices.Count += blockMesh.Vertices.Count;
			for (int i = 0; i < blockMesh.Vertices.Count; i++)
			{
				BlockMeshVertex blockMeshVertex = blockMesh.Vertices.Array[i];
				Vector3 vector = blockMeshVertex.Position;
				if (matrix != null)
				{
					vector = Vector3.Transform(vector, matrix.Value);
				}
				Color color2;
				if (blockMeshVertex.IsEmissive)
				{
					color2 = new Color(color.R * blockMeshVertex.Color.R / byte.MaxValue, color.G * blockMeshVertex.Color.G / byte.MaxValue, color.B * blockMeshVertex.Color.B / byte.MaxValue);
				}
				else
				{
					int face = (facesMap != null) ? facesMap[(int)blockMeshVertex.Face] : ((int)blockMeshVertex.Face);
					float num = this.InterpolateCornerLights(face, vector) / 255f;
					color2 = new Color((byte)((float)(color.R * blockMeshVertex.Color.R) * num), (byte)((float)(color.G * blockMeshVertex.Color.G) * num), (byte)((float)(color.B * blockMeshVertex.Color.B) * num));
				}
				BlockGeometryGenerator.SetupVertex((float)x + vector.X, (float)y + vector.Y, (float)z + vector.Z, color2, blockMeshVertex.TextureCoordinates.X, blockMeshVertex.TextureCoordinates.Y, ref vertices.Array[count + i]);
			}
			if (blockMesh.Sides != null)
			{
				for (int j = 0; j < 6; j++)
				{
					Point3 point = CellFace.FaceToPoint3(j);
					int cellValueFastChunkExists = this.Terrain.GetCellValueFastChunkExists(x + point.X, y + point.Y, z + point.Z);
					this.m_visibleSides[j] = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFastChunkExists)].IsFaceTransparent(this.SubsystemTerrain, CellFace.OppositeFace(j), cellValueFastChunkExists);
				}
				for (int k = 0; k < blockMesh.Indices.Count / 3; k++)
				{
					int num2 = (int)((blockMesh.Sides == null) ? -1 : blockMesh.Sides.Array[k]);
					if (num2 < 0 || this.m_visibleSides[(facesMap != null) ? facesMap[num2] : num2])
					{
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k] + count));
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k + 1] + count));
						indices.Add((ushort)((int)blockMesh.Indices.Array[3 * k + 2] + count));
					}
				}
				return;
			}
			for (int l = 0; l < blockMesh.Indices.Count; l++)
			{
				indices.Add((ushort)((int)blockMesh.Indices.Array[l] + count));
			}
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00082714 File Offset: 0x00080914
		public void GenerateWireVertices(int value, int x, int y, int z, int mountingFace, float centerBoxSize, Vector2 centerOffset, TerrainGeometrySubset subset)
		{
			if (this.SubsystemElectricity == null)
			{
				return;
			}
			Color color = WireBlock.WireColor;
			int num = Terrain.ExtractContents(value);
			if (num == 133)
			{
				int? color2 = WireBlock.GetColor(Terrain.ExtractData(value));
				if (color2 != null)
				{
					color = SubsystemPalette.GetColor(this, color2);
				}
			}
			int num2 = Terrain.ExtractLight(value);
			float num3 = LightingManager.LightIntensityByLightValue[num2];
			Vector3 v = new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f) - 0.5f * CellFace.FaceToVector3(mountingFace);
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			Vector2 v2 = new Vector2(0.9376f, 0.0001f);
			Vector2 v3 = new Vector2(0.03125f, 0.0055078124f);
			Point3 point = CellFace.FaceToPoint3(mountingFace);
			int cellContents = this.Terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
			bool flag = cellContents == 2 || cellContents == 7 || cellContents == 8 || cellContents == 6 || cellContents == 62 || cellContents == 72;
			Vector3 v4 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Top));
			Vector3 vector2 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Left)) * centerOffset.X + v4 * centerOffset.Y;
			int num4 = 0;
			this.m_tmpConnectionPaths.Clear();
			this.SubsystemElectricity.GetAllConnectedNeighbors(x, y, z, mountingFace, this.m_tmpConnectionPaths);
			foreach (ElectricConnectionPath electricConnectionPath in this.m_tmpConnectionPaths)
			{
				if ((num4 & 1 << electricConnectionPath.ConnectorFace) == 0)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(mountingFace, 0, electricConnectionPath.ConnectorFace);
					ElectricConnectorDirection? electricConnectorDirection;
					ElectricConnectorDirection electricConnectorDirection2;
					if (centerOffset == Vector2.Zero)
					{
						electricConnectorDirection = connectorDirection;
						electricConnectorDirection2 = ElectricConnectorDirection.In;
						if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
						{
							continue;
						}
					}
					num4 |= 1 << electricConnectionPath.ConnectorFace;
					Color color3 = color;
					if (num != 133)
					{
						int cellValue = this.Terrain.GetCellValue(x + electricConnectionPath.NeighborOffsetX, y + electricConnectionPath.NeighborOffsetY, z + electricConnectionPath.NeighborOffsetZ);
						if (Terrain.ExtractContents(cellValue) == 133)
						{
							int? color4 = WireBlock.GetColor(Terrain.ExtractData(cellValue));
							if (color4 != null)
							{
								color3 = SubsystemPalette.GetColor(this, color4);
							}
						}
					}
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					Vector3 vector3 = (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)) ? CellFace.FaceToVector3(electricConnectionPath.ConnectorFace) : (-Vector3.Normalize(vector2));
					Vector3 vector4 = Vector3.Cross(vector, vector3);
					float s = (centerBoxSize >= 0f) ? MathUtils.Max(0.03125f, centerBoxSize / 2f) : (centerBoxSize / 2f);
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					float num5 = (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null) ? 0.03125f : 0.5f;
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					float num6 = (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null) ? 0f : ((electricConnectionPath.ConnectorFace == electricConnectionPath.NeighborFace) ? (num5 + 0.03125f) : ((electricConnectionPath.ConnectorFace != CellFace.OppositeFace(electricConnectionPath.NeighborFace)) ? num5 : (num5 - 0.03125f)));
					Vector3 vector5 = v - vector4 * 0.03125f + vector3 * s + vector2;
					Vector3 vector6 = v - vector4 * 0.03125f + vector3 * num5;
					Vector3 vector7 = v + vector4 * 0.03125f + vector3 * num5;
					Vector3 vector8 = v + vector4 * 0.03125f + vector3 * s + vector2;
					Vector3 vector9 = v + vector * 0.03125f + vector3 * (centerBoxSize / 2f) + vector2;
					Vector3 vector10 = v + vector * 0.03125f + vector3 * num6;
					if (flag && centerBoxSize == 0f)
					{
						Vector3 v5 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector5 + vector8), vector);
						vector5 += v5;
						vector8 += v5;
						vector9 += v5;
					}
					Vector2 vector11 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 0f);
					Vector2 vector12 = v2 + v3 * new Vector2(num5 * 2f, 0f);
					Vector2 vector13 = v2 + v3 * new Vector2(num5 * 2f, 1f);
					Vector2 vector14 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 1f);
					Vector2 vector15 = v2 + v3 * new Vector2(centerBoxSize, 0.5f);
					Vector2 vector16 = v2 + v3 * new Vector2(num6 * 2f, 0.5f);
					int num7 = Terrain.ExtractLight(this.Terrain.GetCellValue(x + electricConnectionPath.NeighborOffsetX, y + electricConnectionPath.NeighborOffsetY, z + electricConnectionPath.NeighborOffsetZ));
					float num8 = LightingManager.LightIntensityByLightValue[num7];
					float num9 = 0.5f * (num3 + num8);
					float num10 = LightingManager.CalculateLighting(-vector4);
					float num11 = LightingManager.CalculateLighting(vector4);
					float num12 = LightingManager.CalculateLighting(vector);
					float num13 = num10 * num3;
					float num14 = num10 * num9;
					float num15 = num11 * num9;
					float num16 = num11 * num3;
					float num17 = num12 * num3;
					float num18 = num12 * num9;
					Color color5 = new Color((byte)((float)color3.R * num13), (byte)((float)color3.G * num13), (byte)((float)color3.B * num13));
					Color color6 = new Color((byte)((float)color3.R * num14), (byte)((float)color3.G * num14), (byte)((float)color3.B * num14));
					Color color7 = new Color((byte)((float)color3.R * num15), (byte)((float)color3.G * num15), (byte)((float)color3.B * num15));
					Color color8 = new Color((byte)((float)color3.R * num16), (byte)((float)color3.G * num16), (byte)((float)color3.B * num16));
					Color color9 = new Color((byte)((float)color3.R * num17), (byte)((float)color3.G * num17), (byte)((float)color3.B * num17));
					Color color10 = new Color((byte)((float)color3.R * num18), (byte)((float)color3.G * num18), (byte)((float)color3.B * num18));
					int count = subset.Vertices.Count;
					subset.Vertices.Count += 6;
					TerrainVertex[] array = subset.Vertices.Array;
					BlockGeometryGenerator.SetupVertex(vector5.X, vector5.Y, vector5.Z, color5, vector11.X, vector11.Y, ref array[count]);
					BlockGeometryGenerator.SetupVertex(vector6.X, vector6.Y, vector6.Z, color6, vector12.X, vector12.Y, ref array[count + 1]);
					BlockGeometryGenerator.SetupVertex(vector7.X, vector7.Y, vector7.Z, color7, vector13.X, vector13.Y, ref array[count + 2]);
					BlockGeometryGenerator.SetupVertex(vector8.X, vector8.Y, vector8.Z, color8, vector14.X, vector14.Y, ref array[count + 3]);
					BlockGeometryGenerator.SetupVertex(vector9.X, vector9.Y, vector9.Z, color9, vector15.X, vector15.Y, ref array[count + 4]);
					BlockGeometryGenerator.SetupVertex(vector10.X, vector10.Y, vector10.Z, color10, vector16.X, vector16.Y, ref array[count + 5]);
					int count2 = subset.Indices.Count;
					DynamicArray<ushort> indices = subset.Indices;
					int count3 = indices.Count;
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					indices.Count = count3 + ((electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null) ? 15 : 12);
					ushort[] array2 = subset.Indices.Array;
					array2[count2] = (ushort)count;
					array2[count2 + 1] = (ushort)(count + 5);
					array2[count2 + 2] = (ushort)(count + 1);
					array2[count2 + 3] = (ushort)(count + 5);
					array2[count2 + 4] = (ushort)count;
					array2[count2 + 5] = (ushort)(count + 4);
					array2[count2 + 6] = (ushort)(count + 4);
					array2[count2 + 7] = (ushort)(count + 2);
					array2[count2 + 8] = (ushort)(count + 5);
					array2[count2 + 9] = (ushort)(count + 2);
					array2[count2 + 10] = (ushort)(count + 4);
					array2[count2 + 11] = (ushort)(count + 3);
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
					{
						array2[count2 + 12] = (ushort)(count + 2);
						array2[count2 + 13] = (ushort)(count + 1);
						array2[count2 + 14] = (ushort)(count + 5);
					}
				}
			}
			if (centerBoxSize != 0f || (num4 == 0 && num != 133))
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				if (i != mountingFace && i != CellFace.OppositeFace(mountingFace) && (num4 & 1 << i) == 0)
				{
					Vector3 vector17 = CellFace.FaceToVector3(i);
					Vector3 v6 = Vector3.Cross(vector, vector17);
					Vector3 vector18 = v - v6 * 0.03125f + vector17 * 0.03125f;
					Vector3 vector19 = v + v6 * 0.03125f + vector17 * 0.03125f;
					Vector3 vector20 = v + vector * 0.03125f;
					if (flag)
					{
						Vector3 v7 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector18 + vector19), vector);
						vector18 += v7;
						vector19 += v7;
						vector20 += v7;
					}
					Vector2 vector21 = v2 + v3 * new Vector2(0.0625f, 0f);
					Vector2 vector22 = v2 + v3 * new Vector2(0.0625f, 1f);
					Vector2 vector23 = v2 + v3 * new Vector2(0f, 0.5f);
					float num19 = LightingManager.CalculateLighting(vector17) * num3;
					float num20 = LightingManager.CalculateLighting(vector) * num3;
					Color color11 = new Color((byte)((float)color.R * num19), (byte)((float)color.G * num19), (byte)((float)color.B * num19));
					Color color12 = new Color((byte)((float)color.R * num20), (byte)((float)color.G * num20), (byte)((float)color.B * num20));
					int count4 = subset.Vertices.Count;
					subset.Vertices.Count += 3;
					TerrainVertex[] array3 = subset.Vertices.Array;
					BlockGeometryGenerator.SetupVertex(vector18.X, vector18.Y, vector18.Z, color11, vector21.X, vector21.Y, ref array3[count4]);
					BlockGeometryGenerator.SetupVertex(vector19.X, vector19.Y, vector19.Z, color11, vector22.X, vector22.Y, ref array3[count4 + 1]);
					BlockGeometryGenerator.SetupVertex(vector20.X, vector20.Y, vector20.Z, color12, vector23.X, vector23.Y, ref array3[count4 + 2]);
					int count5 = subset.Indices.Count;
					subset.Indices.Count += 3;
					ushort[] array4 = subset.Indices.Array;
					array4[count5] = (ushort)count4;
					array4[count5 + 1] = (ushort)(count4 + 2);
					array4[count5 + 2] = (ushort)(count4 + 1);
				}
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000833AC File Offset: 0x000815AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CalculateCubeVertexLight(int value, ref int light, ref int shadow)
		{
			int num = Terrain.ExtractContents(value);
			if (num == 0)
			{
				light = Math.Max(light, Terrain.ExtractLight(value));
				return;
			}
			light = Math.Max(light, Terrain.ExtractLight(value));
			shadow += BlocksManager.Blocks[num].GetShadowStrength(value);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x000833F4 File Offset: 0x000815F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CombineLightAndShadow(int light, int shadow)
		{
			return MathUtils.Max(light - MathUtils.Max(shadow / 7, 0), 0);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00083408 File Offset: 0x00081608
		public int CalculateVertexLightFace0(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x - 1, z);
			int num = TerrainChunk.CalculateCellIndex(x - 1 & 15, y, z & 15);
			int cellValueFast = chunkAtCell.GetCellValueFast(num - 1);
			int cellValueFast2 = chunkAtCell.GetCellValueFast(num);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast2, ref light, ref shadow);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z);
			int num2 = TerrainChunk.CalculateCellIndex(x & 15, y, z & 15);
			int cellValueFast3 = chunkAtCell2.GetCellValueFast(num2 - 1);
			int cellValueFast4 = chunkAtCell2.GetCellValueFast(num2);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast3, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast4, ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000834A8 File Offset: 0x000816A8
		public int CalculateVertexLightFace1(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z - 1);
			int num = TerrainChunk.CalculateCellIndex(x & 15, y, z - 1 & 15);
			int cellValueFast = chunkAtCell.GetCellValueFast(num - 1);
			int cellValueFast2 = chunkAtCell.GetCellValueFast(num);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast2, ref light, ref shadow);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z);
			int num2 = TerrainChunk.CalculateCellIndex(x & 15, y, z & 15);
			int cellValueFast3 = chunkAtCell2.GetCellValueFast(num2 - 1);
			int cellValueFast4 = chunkAtCell2.GetCellValueFast(num2);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast3, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast4, ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x00083548 File Offset: 0x00081748
		public int CalculateVertexLightFace2(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x - 1, z - 1);
			int num = TerrainChunk.CalculateCellIndex(x - 1 & 15, y, z - 1 & 15);
			int cellValueFast = chunkAtCell.GetCellValueFast(num - 1);
			int cellValueFast2 = chunkAtCell.GetCellValueFast(num);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast2, ref light, ref shadow);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x, z - 1);
			int num2 = TerrainChunk.CalculateCellIndex(x & 15, y, z - 1 & 15);
			int cellValueFast3 = chunkAtCell2.GetCellValueFast(num2 - 1);
			int cellValueFast4 = chunkAtCell2.GetCellValueFast(num2);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast3, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast4, ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x000835F0 File Offset: 0x000817F0
		public int CalculateVertexLightFace3(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x - 1, z - 1);
			int num = TerrainChunk.CalculateCellIndex(x - 1 & 15, y, z - 1 & 15);
			int cellValueFast = chunkAtCell.GetCellValueFast(num - 1);
			int cellValueFast2 = chunkAtCell.GetCellValueFast(num);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast2, ref light, ref shadow);
			TerrainChunk chunkAtCell2 = this.Terrain.GetChunkAtCell(x - 1, z);
			int num2 = TerrainChunk.CalculateCellIndex(x - 1 & 15, y, z & 15);
			int cellValueFast3 = chunkAtCell2.GetCellValueFast(num2 - 1);
			int cellValueFast4 = chunkAtCell2.GetCellValueFast(num2);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast3, ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(cellValueFast4, ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00083698 File Offset: 0x00081898
		public int CalculateVertexLightFace4(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x - 1, y, z - 1), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x, y, z - 1), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x - 1, y, z), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x, y, z), ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00083714 File Offset: 0x00081914
		public int CalculateVertexLightFace5(int x, int y, int z)
		{
			int light = 0;
			int shadow = 0;
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x - 1, y - 1, z - 1), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x, y - 1, z - 1), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x - 1, y - 1, z), ref light, ref shadow);
			BlockGeometryGenerator.CalculateCubeVertexLight(this.Terrain.GetCellValueFastChunkExists(x, y - 1, z), ref light, ref shadow);
			return BlockGeometryGenerator.CombineLightAndShadow(light, shadow);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00083798 File Offset: 0x00081998
		public void SetupCubeVertexFace0(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace0(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 0, textureSlot, corner, ref vertex);
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000837D0 File Offset: 0x000819D0
		public void SetupCubeVertexFace1(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace1(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 1, textureSlot, corner, ref vertex);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00083808 File Offset: 0x00081A08
		public void SetupCubeVertexFace2(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace2(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 2, textureSlot, corner, ref vertex);
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00083840 File Offset: 0x00081A40
		public void SetupCubeVertexFace3(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace3(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 3, textureSlot, corner, ref vertex);
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00083878 File Offset: 0x00081A78
		public void SetupCubeVertexFace4(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace4(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 4, textureSlot, corner, ref vertex);
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x000838B0 File Offset: 0x00081AB0
		public void SetupCubeVertexFace5(int x, int y, int z, float height, int corner, int textureSlot, Color color, ref TerrainVertex vertex)
		{
			float y2 = (float)y + height - 1f;
			int light = this.CalculateVertexLightFace5(x, y, z);
			BlockGeometryGenerator.SetupCornerVertex((float)x, y2, (float)z, color, light, 5, textureSlot, corner, ref vertex);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x000838E8 File Offset: 0x00081AE8
		public static Vector3 GetRandomWireOffset(Vector3 position, Vector3 normal)
		{
			int hashCode = Vector3.Round(2f * position).GetHashCode();
			return new Vector3
			{
				X = ((normal.X == 0f) ? ((float)(MathUtils.Hash((uint)hashCode) % 255U) / 255f - 0.5f) : 0f),
				Y = ((normal.Y == 0f) ? ((float)(MathUtils.Hash((uint)(hashCode + 1)) % 255U) / 255f - 0.5f) : 0f),
				Z = ((normal.Z == 0f) ? ((float)(MathUtils.Hash((uint)(hashCode + 2)) % 255U) / 255f - 0.5f) : 0f)
			};
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x000839C4 File Offset: 0x00081BC4
		public void CalculateCornerLights(int x, int y, int z)
		{
			if (!(this.m_cornerLightsPosition == new Point3(x, y, z)))
			{
				this.m_cornerLightsPosition = new Point3(x, y, z);
				this.m_cornerLightsByFace[0].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x, y, z)];
				this.m_cornerLightsByFace[0].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x, y, z + 1)];
				this.m_cornerLightsByFace[0].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x, y + 1, z)];
				this.m_cornerLightsByFace[0].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[0].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x + 1, y, z)];
				this.m_cornerLightsByFace[0].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[0].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[0].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace0(x + 1, y + 1, z + 1)];
				this.m_cornerLightsByFace[1].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x, y, z)];
				this.m_cornerLightsByFace[1].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x, y, z + 1)];
				this.m_cornerLightsByFace[1].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x, y + 1, z)];
				this.m_cornerLightsByFace[1].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[1].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x + 1, y, z)];
				this.m_cornerLightsByFace[1].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[1].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[1].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace1(x + 1, y + 1, z + 1)];
				this.m_cornerLightsByFace[2].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x, y, z)];
				this.m_cornerLightsByFace[2].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x, y, z + 1)];
				this.m_cornerLightsByFace[2].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x, y + 1, z)];
				this.m_cornerLightsByFace[2].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[2].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x + 1, y, z)];
				this.m_cornerLightsByFace[2].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[2].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[2].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace2(x + 1, y + 1, z + 1)];
				this.m_cornerLightsByFace[3].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x, y, z)];
				this.m_cornerLightsByFace[3].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x, y, z + 1)];
				this.m_cornerLightsByFace[3].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x, y + 1, z)];
				this.m_cornerLightsByFace[3].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[3].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x + 1, y, z)];
				this.m_cornerLightsByFace[3].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[3].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[3].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace3(x + 1, y + 1, z + 1)];
				this.m_cornerLightsByFace[4].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x, y, z)];
				this.m_cornerLightsByFace[4].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x, y, z + 1)];
				this.m_cornerLightsByFace[4].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x, y + 1, z)];
				this.m_cornerLightsByFace[4].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[4].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x + 1, y, z)];
				this.m_cornerLightsByFace[4].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[4].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[4].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace4(x + 1, y + 1, z + 1)];
				this.m_cornerLightsByFace[5].L000 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x, y, z)];
				this.m_cornerLightsByFace[5].L001 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x, y, z + 1)];
				this.m_cornerLightsByFace[5].L010 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x, y + 1, z)];
				this.m_cornerLightsByFace[5].L011 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x, y + 1, z + 1)];
				this.m_cornerLightsByFace[5].L100 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x + 1, y, z)];
				this.m_cornerLightsByFace[5].L101 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x + 1, y, z + 1)];
				this.m_cornerLightsByFace[5].L110 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x + 1, y + 1, z)];
				this.m_cornerLightsByFace[5].L111 = LightingManager.LightIntensityByLightValue[this.CalculateVertexLightFace5(x + 1, y + 1, z + 1)];
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00084088 File Offset: 0x00082288
		public float InterpolateCornerLights(int face, Vector3 position)
		{
			float x = position.X;
			float y = position.Y;
			float z = position.Z;
			float num = 1f - x;
			float num2 = 1f - y;
			float num3 = 1f - z;
			return this.m_cornerLightsByFace[face].L000 * num * num2 * num3 + this.m_cornerLightsByFace[face].L001 * num * num2 * z + this.m_cornerLightsByFace[face].L010 * num * y * num3 + this.m_cornerLightsByFace[face].L011 * num * y * z + this.m_cornerLightsByFace[face].L100 * x * num2 * num3 + this.m_cornerLightsByFace[face].L101 * x * num2 * z + this.m_cornerLightsByFace[face].L110 * x * y * num3 + this.m_cornerLightsByFace[face].L111 * x * y * z;
		}

		// Token: 0x04000B5B RID: 2907
		public static Vector2[] m_textureCoordinates = new Vector2[]
		{
			new Vector2(0.001f, 0.999f),
			new Vector2(0.999f, 0.999f),
			new Vector2(0.999f, 0.001f),
			new Vector2(0.001f, 0.001f),
			new Vector2(0.001f, 0.999f),
			new Vector2(0.999f, 0.999f),
			new Vector2(0.999f, 0.001f),
			new Vector2(0.001f, 0.001f)
		};

		// Token: 0x04000B5C RID: 2908
		public readonly Terrain Terrain;

		// Token: 0x04000B5D RID: 2909
		public readonly SubsystemTerrain SubsystemTerrain;

		// Token: 0x04000B5E RID: 2910
		public readonly SubsystemElectricity SubsystemElectricity;

		// Token: 0x04000B5F RID: 2911
		public readonly SubsystemFurnitureBlockBehavior SubsystemFurnitureBlockBehavior;

		// Token: 0x04000B60 RID: 2912
		public readonly SubsystemMetersBlockBehavior SubsystemMetersBlockBehavior;

		// Token: 0x04000B61 RID: 2913
		public readonly SubsystemPalette SubsystemPalette;

		// Token: 0x04000B62 RID: 2914
		public DynamicArray<ElectricConnectionPath> m_tmpConnectionPaths = new DynamicArray<ElectricConnectionPath>();

		// Token: 0x04000B63 RID: 2915
		public Point3 m_cornerLightsPosition;

		// Token: 0x04000B64 RID: 2916
		public BlockGeometryGenerator.CornerLights[] m_cornerLightsByFace = new BlockGeometryGenerator.CornerLights[6];

		// Token: 0x04000B65 RID: 2917
		public bool[] m_visibleSides = new bool[6];

		// Token: 0x02000472 RID: 1138
		public struct CornerLights
		{
			// Token: 0x0400168B RID: 5771
			public float L000;

			// Token: 0x0400168C RID: 5772
			public float L001;

			// Token: 0x0400168D RID: 5773
			public float L010;

			// Token: 0x0400168E RID: 5774
			public float L011;

			// Token: 0x0400168F RID: 5775
			public float L100;

			// Token: 0x04001690 RID: 5776
			public float L101;

			// Token: 0x04001691 RID: 5777
			public float L110;

			// Token: 0x04001692 RID: 5778
			public float L111;
		}
	}
}
