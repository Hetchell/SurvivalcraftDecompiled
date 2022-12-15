using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Engine;

namespace Game
{
	// Token: 0x0200030A RID: 778
	public class Terrain : IDisposable
	{
		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060015DC RID: 5596 RVA: 0x000A6676 File Offset: 0x000A4876
		public TerrainChunk[] AllocatedChunks
		{
			get
			{
				if (this.m_allocatedChunksArray == null)
				{
					this.m_allocatedChunksArray = this.m_allocatedChunks.ToArray<TerrainChunk>();
				}
				return this.m_allocatedChunksArray;
			}
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x000A6697 File Offset: 0x000A4897
		public Terrain()
		{
			this.m_allChunks = new Terrain.ChunksStorage();
			this.m_allocatedChunks = new HashSet<TerrainChunk>();
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x000A66B8 File Offset: 0x000A48B8
		public void Dispose()
		{
			foreach (TerrainChunk terrainChunk in this.m_allocatedChunks)
			{
				terrainChunk.Dispose();
			}
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x000A6708 File Offset: 0x000A4908
		public TerrainChunk GetNextChunk(int chunkX, int chunkZ)
		{
			TerrainChunk terrainChunk = this.GetChunkAtCoords(chunkX, chunkZ);
			if (terrainChunk != null)
			{
				return terrainChunk;
			}
			TerrainChunk[] allocatedChunks = this.AllocatedChunks;
			for (int i = 0; i < allocatedChunks.Length; i++)
			{
				if (Terrain.ComparePoints(allocatedChunks[i].Coords, new Point2(chunkX, chunkZ)) >= 0 && (terrainChunk == null || Terrain.ComparePoints(allocatedChunks[i].Coords, terrainChunk.Coords) < 0))
				{
					terrainChunk = allocatedChunks[i];
				}
			}
			if (terrainChunk == null)
			{
				for (int j = 0; j < allocatedChunks.Length; j++)
				{
					if (terrainChunk == null || Terrain.ComparePoints(allocatedChunks[j].Coords, terrainChunk.Coords) < 0)
					{
						terrainChunk = allocatedChunks[j];
					}
				}
			}
			return terrainChunk;
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x000A679B File Offset: 0x000A499B
		public TerrainChunk GetChunkAtCoords(int chunkX, int chunkZ)
		{
			return this.m_allChunks.Get(chunkX, chunkZ);
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x000A67AA File Offset: 0x000A49AA
		public TerrainChunk GetChunkAtCell(int x, int z)
		{
			return this.GetChunkAtCoords(x >> 4, z >> 4);
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x000A67B8 File Offset: 0x000A49B8
		public TerrainChunk AllocateChunk(int chunkX, int chunkZ)
		{
			if (this.GetChunkAtCoords(chunkX, chunkZ) != null)
			{
				throw new InvalidOperationException("Chunk already allocated.");
			}
			TerrainChunk terrainChunk = new TerrainChunk(this, chunkX, chunkZ);
			this.m_allocatedChunks.Add(terrainChunk);
			this.m_allChunks.Add(chunkX, chunkZ, terrainChunk);
			this.m_allocatedChunksArray = null;
			return terrainChunk;
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x000A6808 File Offset: 0x000A4A08
		public void FreeChunk(TerrainChunk chunk)
		{
			if (!this.m_allocatedChunks.Remove(chunk))
			{
				throw new InvalidOperationException("Chunk not allocated.");
			}
			this.m_allChunks.Remove(chunk.Coords.X, chunk.Coords.Y);
			this.m_allocatedChunksArray = null;
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x000A6856 File Offset: 0x000A4A56
		public static int ComparePoints(Point2 c1, Point2 c2)
		{
			if (c1.Y == c2.Y)
			{
				return c1.X - c2.X;
			}
			return c1.Y - c2.Y;
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x000A6881 File Offset: 0x000A4A81
		public static Point2 ToChunk(Vector2 p)
		{
			return Terrain.ToChunk(Terrain.ToCell(p.X), Terrain.ToCell(p.Y));
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x000A689E File Offset: 0x000A4A9E
		public static Point2 ToChunk(int x, int z)
		{
			return new Point2(x >> 4, z >> 4);
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x000A68AB File Offset: 0x000A4AAB
		public static int ToCell(float x)
		{
			return (int)MathUtils.Floor(x);
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x000A68B4 File Offset: 0x000A4AB4
		public static Point2 ToCell(float x, float y)
		{
			return new Point2((int)MathUtils.Floor(x), (int)MathUtils.Floor(y));
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x000A68C9 File Offset: 0x000A4AC9
		public static Point2 ToCell(Vector2 p)
		{
			return new Point2((int)MathUtils.Floor(p.X), (int)MathUtils.Floor(p.Y));
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x000A68E8 File Offset: 0x000A4AE8
		public static Point3 ToCell(float x, float y, float z)
		{
			return new Point3((int)MathUtils.Floor(x), (int)MathUtils.Floor(y), (int)MathUtils.Floor(z));
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x000A6904 File Offset: 0x000A4B04
		public static Point3 ToCell(Vector3 p)
		{
			return new Point3((int)MathUtils.Floor(p.X), (int)MathUtils.Floor(p.Y), (int)MathUtils.Floor(p.Z));
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x000A692F File Offset: 0x000A4B2F
		public bool IsCellValid(int x, int y, int z)
		{
			return y >= 0 && y < 256;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x000A693F File Offset: 0x000A4B3F
		public int GetCellValue(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellValueFast(x, y, z);
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x000A6957 File Offset: 0x000A4B57
		public int GetCellContents(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellContentsFast(x, y, z);
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x000A696F File Offset: 0x000A4B6F
		public int GetCellLight(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellLightFast(x, y, z);
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x000A6987 File Offset: 0x000A4B87
		public int GetCellValueFast(int x, int y, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.GetCellValueFast(x & 15, y, z & 15);
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x000A69A5 File Offset: 0x000A4BA5
		public int GetCellValueFastChunkExists(int x, int y, int z)
		{
			return this.GetChunkAtCell(x, z).GetCellValueFast(x & 15, y, z & 15);
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x000A69BD File Offset: 0x000A4BBD
		public int GetCellContentsFast(int x, int y, int z)
		{
			return Terrain.ExtractContents(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x000A69CD File Offset: 0x000A4BCD
		public int GetCellLightFast(int x, int y, int z)
		{
			return Terrain.ExtractLight(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x000A69DD File Offset: 0x000A4BDD
		public void SetCellValueFast(int x, int y, int z, int value)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return;
			}
			chunkAtCell.SetCellValueFast(x & 15, y, z & 15, value);
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x000A69FC File Offset: 0x000A4BFC
		public int CalculateTopmostCellHeight(int x, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.CalculateTopmostCellHeight(x & 15, z & 15);
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x000A6A19 File Offset: 0x000A4C19
		public int GetShaftValue(int x, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.GetShaftValueFast(x & 15, z & 15);
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x000A6A36 File Offset: 0x000A4C36
		public void SetShaftValue(int x, int z, int value)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return;
			}
			chunkAtCell.SetShaftValueFast(x & 15, z & 15, value);
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x000A6A53 File Offset: 0x000A4C53
		public int GetTemperature(int x, int z)
		{
			return Terrain.ExtractTemperature(this.GetShaftValue(x, z));
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x000A6A62 File Offset: 0x000A4C62
		public void SetTemperature(int x, int z, int temperature)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceTemperature(this.GetShaftValue(x, z), temperature));
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x000A6A7A File Offset: 0x000A4C7A
		public int GetHumidity(int x, int z)
		{
			return Terrain.ExtractHumidity(this.GetShaftValue(x, z));
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x000A6A89 File Offset: 0x000A4C89
		public void SetHumidity(int x, int z, int humidity)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceHumidity(this.GetShaftValue(x, z), humidity));
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x000A6AA1 File Offset: 0x000A4CA1
		public int GetTopHeight(int x, int z)
		{
			return Terrain.ExtractTopHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x000A6AB0 File Offset: 0x000A4CB0
		public void SetTopHeight(int x, int z, int topHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceTopHeight(this.GetShaftValue(x, z), topHeight));
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x000A6AC8 File Offset: 0x000A4CC8
		public int GetBottomHeight(int x, int z)
		{
			return Terrain.ExtractBottomHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x000A6AD7 File Offset: 0x000A4CD7
		public void SetBottomHeight(int x, int z, int bottomHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceBottomHeight(this.GetShaftValue(x, z), bottomHeight));
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x000A6AEF File Offset: 0x000A4CEF
		public int GetSunlightHeight(int x, int z)
		{
			return Terrain.ExtractSunlightHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000A6AFE File Offset: 0x000A4CFE
		public void SetSunlightHeight(int x, int z, int sunlightHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceSunlightHeight(this.GetShaftValue(x, z), sunlightHeight));
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x000A6B16 File Offset: 0x000A4D16
		public static int MakeBlockValue(int contents)
		{
			return contents & 1023;
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x000A6B1F File Offset: 0x000A4D1F
		public static int MakeBlockValue(int contents, int light, int data)
		{
			return (contents & 1023) | (light << 10 & 15360) | (data << 14 & -16384);
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x000A6B3E File Offset: 0x000A4D3E
		public static int ExtractContents(int value)
		{
			return value & 1023;
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x000A6B47 File Offset: 0x000A4D47
		public static int ExtractLight(int value)
		{
			return (value & 15360) >> 10;
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x000A6B53 File Offset: 0x000A4D53
		public static int ExtractData(int value)
		{
			return (value & -16384) >> 14;
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x000A6B5F File Offset: 0x000A4D5F
		public static int ExtractTopHeight(int value)
		{
			return value & 255;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x000A6B68 File Offset: 0x000A4D68
		public static int ExtractBottomHeight(int value)
		{
			return (value & 16711680) >> 16;
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x000A6B74 File Offset: 0x000A4D74
		public static int ExtractSunlightHeight(int value)
		{
			return (value & -16777216) >> 24;
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x000A6B80 File Offset: 0x000A4D80
		public static int ExtractHumidity(int value)
		{
			return (value & 61440) >> 12;
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x000A6B8C File Offset: 0x000A4D8C
		public static int ExtractTemperature(int value)
		{
			return (value & 3840) >> 8;
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000A6B97 File Offset: 0x000A4D97
		public static int ReplaceContents(int value, int contents)
		{
			return value ^ ((value ^ contents) & 1023);
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x000A6BA4 File Offset: 0x000A4DA4
		public static int ReplaceLight(int value, int light)
		{
			return value ^ ((value ^ light << 10) & 15360);
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x000A6BB4 File Offset: 0x000A4DB4
		public static int ReplaceData(int value, int data)
		{
			return value ^ ((value ^ data << 14) & -16384);
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x000A6BC4 File Offset: 0x000A4DC4
		public static int ReplaceTopHeight(int value, int topHeight)
		{
			return value ^ ((value ^ topHeight) & 255);
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x000A6BD1 File Offset: 0x000A4DD1
		public static int ReplaceBottomHeight(int value, int bottomHeight)
		{
			return value ^ ((value ^ bottomHeight << 16) & 16711680);
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x000A6BE1 File Offset: 0x000A4DE1
		public static int ReplaceSunlightHeight(int value, int sunlightHeight)
		{
			return value ^ ((value ^ sunlightHeight << 24) & -16777216);
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x000A6BF1 File Offset: 0x000A4DF1
		public static int ReplaceHumidity(int value, int humidity)
		{
			return value ^ ((value ^ humidity << 12) & 61440);
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x000A6C01 File Offset: 0x000A4E01
		public static int ReplaceTemperature(int value, int temperature)
		{
			return value ^ ((value ^ temperature << 8) & 3840);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x000A6C10 File Offset: 0x000A4E10
		public int GetSeasonalTemperature(int x, int z)
		{
			return this.GetTemperature(x, z) + this.SeasonTemperature;
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x000A6C21 File Offset: 0x000A4E21
		public int GetSeasonalTemperature(int shaftValue)
		{
			return Terrain.ExtractTemperature(shaftValue) + this.SeasonTemperature;
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x000A6C30 File Offset: 0x000A4E30
		public int GetSeasonalHumidity(int x, int z)
		{
			return this.GetHumidity(x, z) + this.SeasonHumidity;
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x000A6C41 File Offset: 0x000A4E41
		public int GetSeasonalHumidity(int shaftValue)
		{
			return Terrain.ExtractHumidity(shaftValue) + this.SeasonHumidity;
		}

		// Token: 0x04000F86 RID: 3974
		public const int ContentsMask = 1023;

		// Token: 0x04000F87 RID: 3975
		public const int LightMask = 15360;

		// Token: 0x04000F88 RID: 3976
		public const int LightShift = 10;

		// Token: 0x04000F89 RID: 3977
		public const int DataMask = -16384;

		// Token: 0x04000F8A RID: 3978
		public const int DataShift = 14;

		// Token: 0x04000F8B RID: 3979
		public const int TopHeightMask = 255;

		// Token: 0x04000F8C RID: 3980
		public const int TopHeightShift = 0;

		// Token: 0x04000F8D RID: 3981
		public const int TemperatureMask = 3840;

		// Token: 0x04000F8E RID: 3982
		public const int TemperatureShift = 8;

		// Token: 0x04000F8F RID: 3983
		public const int HumidityMask = 61440;

		// Token: 0x04000F90 RID: 3984
		public const int HumidityShift = 12;

		// Token: 0x04000F91 RID: 3985
		public const int BottomHeightMask = 16711680;

		// Token: 0x04000F92 RID: 3986
		public const int BottomHeightShift = 16;

		// Token: 0x04000F93 RID: 3987
		public const int SunlightHeightMask = -16777216;

		// Token: 0x04000F94 RID: 3988
		public const int SunlightHeightShift = 24;

		// Token: 0x04000F95 RID: 3989
		public Terrain.ChunksStorage m_allChunks;

		// Token: 0x04000F96 RID: 3990
		public HashSet<TerrainChunk> m_allocatedChunks;

		// Token: 0x04000F97 RID: 3991
		public TerrainChunk[] m_allocatedChunksArray;

		// Token: 0x04000F98 RID: 3992
		public int SeasonTemperature;

		// Token: 0x04000F99 RID: 3993
		public int SeasonHumidity;

		// Token: 0x020004E7 RID: 1255
		public class ChunksStorage
		{
			// Token: 0x0600205E RID: 8286 RVA: 0x000E4204 File Offset: 0x000E2404
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TerrainChunk Get(int x, int y)
			{
				int num = x + (y << 8) & 65535;
				for (;;)
				{
					TerrainChunk terrainChunk = this.m_array[num];
					if (terrainChunk == null)
					{
						break;
					}
					if (terrainChunk.Coords.X == x && terrainChunk.Coords.Y == y)
					{
						return terrainChunk;
					}
					num = (num + 1 & 65535);
				}
				return null;
			}

			// Token: 0x0600205F RID: 8287 RVA: 0x000E4254 File Offset: 0x000E2454
			public void Add(int x, int y, TerrainChunk chunk)
			{
				int num = x + (y << 8) & 65535;
				while (this.m_array[num] != null)
				{
					num = (num + 1 & 65535);
				}
				this.m_array[num] = chunk;
			}

			// Token: 0x06002060 RID: 8288 RVA: 0x000E428C File Offset: 0x000E248C
			public void Remove(int x, int y)
			{
				int num = x + (y << 8) & 65535;
				for (;;)
				{
					TerrainChunk terrainChunk = this.m_array[num];
					if (terrainChunk == null)
					{
						break;
					}
					if (terrainChunk.Coords.X == x && terrainChunk.Coords.Y == y)
					{
						goto IL_41;
					}
					num = (num + 1 & 65535);
				}
				return;
				IL_41:
				this.m_array[num] = null;
			}

			// Token: 0x040017F5 RID: 6133
			public const int Shift = 8;

			// Token: 0x040017F6 RID: 6134
			public const int Capacity = 65536;

			// Token: 0x040017F7 RID: 6135
			public const int CapacityMinusOne = 65535;

			// Token: 0x040017F8 RID: 6136
			public TerrainChunk[] m_array = new TerrainChunk[65536];
		}
	}
}
