using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000318 RID: 792
	public class TerrainSerializer129 : IDisposable
	{
		// Token: 0x060016B4 RID: 5812 RVA: 0x000B478C File Offset: 0x000B298C
		public TerrainSerializer129(Terrain terrain, string directoryName)
		{
			this.m_terrain = terrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer129.WriteTOCEntry(stream, 0, 0, -1);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer129.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num < 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = 786444L + 132112L * (long)num;
			}
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x000B4868 File Offset: 0x000B2A68
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x000B4871 File Offset: 0x000B2A71
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x000B4893 File Offset: 0x000B2A93
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x000B48A0 File Offset: 0x000B2AA0
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer129.ReadInt(stream);
			int num2 = TerrainSerializer129.ReadInt(stream);
			TerrainSerializer129.ReadInt(stream);
			TerrainSerializer129.ReadInt(stream);
			if (num != -559038737 || num2 != -2)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x000B48DF File Offset: 0x000B2ADF
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer129.WriteInt(stream, -559038737);
			TerrainSerializer129.WriteInt(stream, -2);
			TerrainSerializer129.WriteInt(stream, cx);
			TerrainSerializer129.WriteInt(stream, cz);
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x000B4902 File Offset: 0x000B2B02
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int index)
		{
			cx = TerrainSerializer129.ReadInt(stream);
			cz = TerrainSerializer129.ReadInt(stream);
			index = TerrainSerializer129.ReadInt(stream);
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x000B491C File Offset: 0x000B2B1C
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int index)
		{
			TerrainSerializer129.WriteInt(stream, cx);
			TerrainSerializer129.WriteInt(stream, cz);
			TerrainSerializer129.WriteInt(stream, index);
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x000B4934 File Offset: 0x000B2B34
		public unsafe bool LoadChunkBlocks(TerrainChunk chunk)
		{
			bool flag = false;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			bool result;
			try
			{
				long offset;
				if (!this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out offset))
				{
					result = flag;
				}
				else
				{
					double realTime = Time.RealTime;
					this.m_stream.Seek(offset, SeekOrigin.Begin);
					TerrainSerializer129.ReadChunkHeader(this.m_stream);
					this.m_stream.Read(this.m_buffer, 0, 131072);
					try
					{
						fixed (byte* ptr = &this.m_buffer[0])
						{
							int* ptr2 = (int*)ptr;
							for (int i = 0; i < 16; i++)
							{
								for (int j = 0; j < 16; j++)
								{
									int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
									int k = 0;
									while (k < 128)
									{
										chunk.SetCellValueFast(num3, *ptr2);
										k++;
										num3++;
										ptr2++;
									}
								}
							}
						}
					}
					finally
					{
						byte* ptr = null;
					}
					this.m_stream.Read(this.m_buffer, 0, 1024);
					try
					{
						fixed (byte* ptr = &this.m_buffer[0])
						{
							int* ptr3 = (int*)ptr;
							for (int l = 0; l < 16; l++)
							{
								for (int m = 0; m < 16; m++)
								{
									this.m_terrain.SetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y, *ptr3);
									ptr3++;
								}
							}
						}
					}
					finally
					{
						byte* ptr = null;
					}
					flag = true;
					double realTime2 = Time.RealTime;
					result = flag;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error loading data for chunk ({0},{1}).", num, num2), e));
				result = flag;
			}
			return result;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x000B4B30 File Offset: 0x000B2D30
		public unsafe void SaveChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				bool flag = false;
				long length;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out length))
				{
					this.m_stream.Seek(length, SeekOrigin.Begin);
				}
				else
				{
					flag = true;
					length = this.m_stream.Length;
					this.m_stream.Seek(length, SeekOrigin.Begin);
				}
				TerrainSerializer129.WriteChunkHeader(this.m_stream, num, num2);
				try
				{
					fixed (byte* ptr = &this.m_buffer[0])
					{
						int* ptr2 = (int*)ptr;
						for (int i = 0; i < 16; i++)
						{
							for (int j = 0; j < 16; j++)
							{
								int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
								int k = 0;
								while (k < 128)
								{
									*ptr2 = chunk.GetCellValueFast(num3);
									k++;
									num3++;
									ptr2++;
								}
							}
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				this.m_stream.Write(this.m_buffer, 0, 131072);
				try
				{
					fixed (byte* ptr = &this.m_buffer[0])
					{
						int* ptr3 = (int*)ptr;
						for (int l = 0; l < 16; l++)
						{
							for (int m = 0; m < 16; m++)
							{
								*ptr3 = this.m_terrain.GetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y);
								ptr3++;
							}
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				this.m_stream.Write(this.m_buffer, 0, 1024);
				if (flag)
				{
					this.m_stream.Flush();
					int num4 = this.m_chunkOffsets.Count % 65536 * 3 * 4;
					this.m_stream.Seek((long)num4, SeekOrigin.Begin);
					TerrainSerializer129.WriteInt(this.m_stream, num);
					TerrainSerializer129.WriteInt(this.m_stream, num2);
					TerrainSerializer129.WriteInt(this.m_stream, this.m_chunkOffsets.Count);
					this.m_chunkOffsets[new Point2(num, num2)] = length;
				}
				this.m_stream.Flush();
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error writing data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x000B4DB8 File Offset: 0x000B2FB8
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x000B4DDD File Offset: 0x000B2FDD
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}

		// Token: 0x04001064 RID: 4196
		public const int MaxChunks = 65536;

		// Token: 0x04001065 RID: 4197
		public const int TocEntryBytesCount = 12;

		// Token: 0x04001066 RID: 4198
		public const int TocBytesCount = 786444;

		// Token: 0x04001067 RID: 4199
		public const int ChunkSizeX = 16;

		// Token: 0x04001068 RID: 4200
		public const int ChunkSizeY = 128;

		// Token: 0x04001069 RID: 4201
		public const int ChunkSizeZ = 16;

		// Token: 0x0400106A RID: 4202
		public const int ChunkBitsX = 4;

		// Token: 0x0400106B RID: 4203
		public const int ChunkBitsZ = 4;

		// Token: 0x0400106C RID: 4204
		public const int ChunkBytesCount = 132112;

		// Token: 0x0400106D RID: 4205
		public const string ChunksFileName = "Chunks32.dat";

		// Token: 0x0400106E RID: 4206
		public Terrain m_terrain;

		// Token: 0x0400106F RID: 4207
		public byte[] m_buffer = new byte[131072];

		// Token: 0x04001070 RID: 4208
		public Dictionary<Point2, long> m_chunkOffsets = new Dictionary<Point2, long>();

		// Token: 0x04001071 RID: 4209
		public Stream m_stream;
	}
}
