using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x0200031A RID: 794
	public class TerrainSerializer22 : IDisposable
	{
		// Token: 0x060016CC RID: 5836 RVA: 0x000B54B0 File Offset: 0x000B36B0
		public TerrainSerializer22(Terrain terrain, string directoryName)
		{
			this.m_terrain = terrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32h.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer22.WriteTOCEntry(stream, 0, 0, -1);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer22.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num < 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = 786444L + 263184L * (long)num;
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000B558C File Offset: 0x000B378C
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x000B5595 File Offset: 0x000B3795
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x000B55B7 File Offset: 0x000B37B7
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x000B55C4 File Offset: 0x000B37C4
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer22.ReadInt(stream);
			int num2 = TerrainSerializer22.ReadInt(stream);
			TerrainSerializer22.ReadInt(stream);
			TerrainSerializer22.ReadInt(stream);
			if (num != -559038737 || num2 != -2)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x000B5603 File Offset: 0x000B3803
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer22.WriteInt(stream, -559038737);
			TerrainSerializer22.WriteInt(stream, -2);
			TerrainSerializer22.WriteInt(stream, cx);
			TerrainSerializer22.WriteInt(stream, cz);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000B5626 File Offset: 0x000B3826
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int index)
		{
			cx = TerrainSerializer22.ReadInt(stream);
			cz = TerrainSerializer22.ReadInt(stream);
			index = TerrainSerializer22.ReadInt(stream);
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x000B5640 File Offset: 0x000B3840
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int index)
		{
			TerrainSerializer22.WriteInt(stream, cx);
			TerrainSerializer22.WriteInt(stream, cz);
			TerrainSerializer22.WriteInt(stream, index);
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x000B5658 File Offset: 0x000B3858
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
					TerrainSerializer22.ReadChunkHeader(this.m_stream);
					this.m_stream.Read(this.m_buffer, 0, 262144);
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
									while (k < 256)
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

		// Token: 0x060016D5 RID: 5845 RVA: 0x000B5854 File Offset: 0x000B3A54
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
				TerrainSerializer22.WriteChunkHeader(this.m_stream, num, num2);
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
								while (k < 256)
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
				this.m_stream.Write(this.m_buffer, 0, 262144);
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
					TerrainSerializer22.WriteInt(this.m_stream, num);
					TerrainSerializer22.WriteInt(this.m_stream, num2);
					TerrainSerializer22.WriteInt(this.m_stream, this.m_chunkOffsets.Count);
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

		// Token: 0x060016D6 RID: 5846 RVA: 0x000B5ADC File Offset: 0x000B3CDC
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x000B5B01 File Offset: 0x000B3D01
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}

		// Token: 0x04001078 RID: 4216
		public const int MaxChunks = 65536;

		// Token: 0x04001079 RID: 4217
		public const int TocEntryBytesCount = 12;

		// Token: 0x0400107A RID: 4218
		public const int TocBytesCount = 786444;

		// Token: 0x0400107B RID: 4219
		public const int ChunkSizeX = 16;

		// Token: 0x0400107C RID: 4220
		public const int ChunkSizeY = 256;

		// Token: 0x0400107D RID: 4221
		public const int ChunkSizeZ = 16;

		// Token: 0x0400107E RID: 4222
		public const int ChunkBitsX = 4;

		// Token: 0x0400107F RID: 4223
		public const int ChunkBitsZ = 4;

		// Token: 0x04001080 RID: 4224
		public const int ChunkBytesCount = 263184;

		// Token: 0x04001081 RID: 4225
		public const string ChunksFileName = "Chunks32h.dat";

		// Token: 0x04001082 RID: 4226
		public Terrain m_terrain;

		// Token: 0x04001083 RID: 4227
		public byte[] m_buffer = new byte[262144];

		// Token: 0x04001084 RID: 4228
		public Dictionary<Point2, long> m_chunkOffsets = new Dictionary<Point2, long>();

		// Token: 0x04001085 RID: 4229
		public Stream m_stream;
	}
}
