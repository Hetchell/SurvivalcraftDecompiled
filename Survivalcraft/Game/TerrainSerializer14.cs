using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000319 RID: 793
	public class TerrainSerializer14 : IDisposable
	{
		// Token: 0x060016C0 RID: 5824 RVA: 0x000B4E08 File Offset: 0x000B3008
		public TerrainSerializer14(SubsystemTerrain subsystemTerrain, string directoryName)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer14.WriteTOCEntry(stream, 0, 0, 0);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer14.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num == 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = num;
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x000B4ED4 File Offset: 0x000B30D4
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x000B4EDD File Offset: 0x000B30DD
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x000B4EFF File Offset: 0x000B30FF
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x000B4F0C File Offset: 0x000B310C
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer14.ReadInt(stream);
			int num2 = TerrainSerializer14.ReadInt(stream);
			TerrainSerializer14.ReadInt(stream);
			TerrainSerializer14.ReadInt(stream);
			if (num != -559038737 || num2 != -1)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x000B4F4A File Offset: 0x000B314A
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer14.WriteInt(stream, -559038737);
			TerrainSerializer14.WriteInt(stream, -1);
			TerrainSerializer14.WriteInt(stream, cx);
			TerrainSerializer14.WriteInt(stream, cz);
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000B4F6C File Offset: 0x000B316C
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int offset)
		{
			cx = TerrainSerializer14.ReadInt(stream);
			cz = TerrainSerializer14.ReadInt(stream);
			offset = TerrainSerializer14.ReadInt(stream);
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000B4F86 File Offset: 0x000B3186
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int offset)
		{
			TerrainSerializer14.WriteInt(stream, cx);
			TerrainSerializer14.WriteInt(stream, cz);
			TerrainSerializer14.WriteInt(stream, offset);
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x000B4FA0 File Offset: 0x000B31A0
		public bool LoadChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			bool result = false;
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				int num3;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out num3))
				{
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
					TerrainSerializer14.ReadChunkHeader(this.m_stream);
					int num4 = 0;
					this.m_stream.Read(this.m_buffer, 0, 131072);
					for (int i = 0; i < 16; i++)
					{
						for (int j = 0; j < 16; j++)
						{
							int num5 = TerrainChunk.CalculateCellIndex(i, 0, j);
							for (int k = 0; k < 256; k++)
							{
								int num6 = (int)this.m_buffer[num4++];
								num6 |= (int)this.m_buffer[num4++] << 8;
								chunk.SetCellValueFast(num5++, num6);
							}
						}
					}
					num4 = 0;
					this.m_stream.Read(this.m_buffer, 0, 1024);
					for (int l = 0; l < 16; l++)
					{
						for (int m = 0; m < 16; m++)
						{
							int num7 = (int)this.m_buffer[num4++];
							num7 |= (int)this.m_buffer[num4++] << 8;
							num7 |= (int)this.m_buffer[num4++] << 16;
							num7 |= (int)this.m_buffer[num4++] << 24;
							terrain.SetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y, num7);
						}
					}
					result = true;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error loading data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
			return result;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x000B51AC File Offset: 0x000B33AC
		public void SaveChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				bool flag = false;
				int num3;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out num3))
				{
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
				}
				else
				{
					flag = true;
					num3 = (int)this.m_stream.Length;
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
				}
				TerrainSerializer14.WriteChunkHeader(this.m_stream, num, num2);
				int num4 = 0;
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						int num5 = TerrainChunk.CalculateCellIndex(i, 0, j);
						for (int k = 0; k < 256; k++)
						{
							int cellValueFast = chunk.GetCellValueFast(num5++);
							this.m_buffer[num4++] = (byte)cellValueFast;
							this.m_buffer[num4++] = (byte)(cellValueFast >> 8);
						}
					}
				}
				this.m_stream.Write(this.m_buffer, 0, 131072);
				num4 = 0;
				for (int l = 0; l < 16; l++)
				{
					for (int m = 0; m < 16; m++)
					{
						int shaftValue = terrain.GetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y);
						this.m_buffer[num4++] = (byte)shaftValue;
						this.m_buffer[num4++] = (byte)(shaftValue >> 8);
						this.m_buffer[num4++] = (byte)(shaftValue >> 16);
						this.m_buffer[num4++] = (byte)(shaftValue >> 24);
					}
				}
				this.m_stream.Write(this.m_buffer, 0, 1024);
				if (flag)
				{
					this.m_stream.Flush();
					int num6 = this.m_chunkOffsets.Count % 65536 * 3 * 4;
					this.m_stream.Seek((long)num6, SeekOrigin.Begin);
					TerrainSerializer14.WriteInt(this.m_stream, num);
					TerrainSerializer14.WriteInt(this.m_stream, num2);
					TerrainSerializer14.WriteInt(this.m_stream, num3);
					this.m_chunkOffsets[new Point2(num, num2)] = num3;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error writing data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x000B5438 File Offset: 0x000B3638
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x000B5460 File Offset: 0x000B3660
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)(value & 255));
			stream.WriteByte((byte)(value >> 8 & 255));
			stream.WriteByte((byte)(value >> 16 & 255));
			stream.WriteByte((byte)(value >> 24 & 255));
		}

		// Token: 0x04001072 RID: 4210
		public const int MaxChunks = 65536;

		// Token: 0x04001073 RID: 4211
		public const string ChunksFileName = "Chunks.dat";

		// Token: 0x04001074 RID: 4212
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001075 RID: 4213
		public byte[] m_buffer = new byte[131072];

		// Token: 0x04001076 RID: 4214
		public Dictionary<Point2, int> m_chunkOffsets = new Dictionary<Point2, int>();

		// Token: 0x04001077 RID: 4215
		public Stream m_stream;
	}
}
