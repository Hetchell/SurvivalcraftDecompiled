using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Game
{
	// Token: 0x02000360 RID: 864
	public class ZipArchive : IDisposable
	{
		// Token: 0x0600184E RID: 6222 RVA: 0x000C0790 File Offset: 0x000BE990
		static ZipArchive()
		{
			for (int i = 0; i < ZipArchive.CrcTable.Length; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1U) == 0U) ? (num >> 1) : (3988292384U ^ num >> 1));
				}
				ZipArchive.CrcTable[i] = num;
			}
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x000C0806 File Offset: 0x000BEA06
		public static ZipArchive Create(Stream stream, bool keepStreamOpen = false)
		{
			return new ZipArchive
			{
				Comment = string.Empty,
				ZipFileStream = stream,
				ReadOnly = false,
				KeepStreamOpen = keepStreamOpen
			};
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x000C0830 File Offset: 0x000BEA30
		public static ZipArchive Open(Stream stream, bool keepStreamOpen = false)
		{
			ZipArchive zipArchive = new ZipArchive();
			zipArchive.ZipFileStream = stream;
			zipArchive.ReadOnly = true;
			zipArchive.KeepStreamOpen = keepStreamOpen;
			if (zipArchive.ReadFileInfo())
			{
				return zipArchive;
			}
			throw new InvalidDataException();
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x000C0868 File Offset: 0x000BEA68
		public void AddStream(string filenameInZip, Stream source)
		{
			if (this.ReadOnly)
			{
				throw new InvalidOperationException("Writing is not allowed");
			}
			ZipArchiveEntry zipArchiveEntry = new ZipArchiveEntry();
			zipArchiveEntry.Method = ZipArchive.Compression.Deflate;
			zipArchiveEntry.FilenameInZip = this.NormalizedFilename(filenameInZip);
			zipArchiveEntry.Comment = string.Empty;
			zipArchiveEntry.Crc32 = 0U;
			zipArchiveEntry.HeaderOffset = (uint)this.ZipFileStream.Position;
			zipArchiveEntry.ModifyTime = DateTime.Now;
			this.WriteLocalHeader(zipArchiveEntry);
			zipArchiveEntry.FileOffset = (uint)this.ZipFileStream.Position;
			this.Store(zipArchiveEntry, source);
			this.UpdateCrcAndSizes(zipArchiveEntry);
			this.Files.Add(zipArchiveEntry);
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x000C0908 File Offset: 0x000BEB08
		public void Close()
		{
			if (!this.ReadOnly)
			{
				uint offset = (uint)this.ZipFileStream.Position;
				uint num = 0U;
				if (this.CentralDirImage != null)
				{
					this.ZipFileStream.Write(this.CentralDirImage, 0, this.CentralDirImage.Length);
				}
				for (int i = 0; i < this.Files.Count; i++)
				{
					long position = this.ZipFileStream.Position;
					this.WriteCentralDirRecord(this.Files[i]);
					num += (uint)((int)(this.ZipFileStream.Position - position));
				}
				if (this.CentralDirImage != null)
				{
					this.WriteEndRecord(num + (uint)this.CentralDirImage.Length, offset);
				}
				else
				{
					this.WriteEndRecord(num, offset);
				}
			}
			if (this.ZipFileStream != null && !this.KeepStreamOpen)
			{
				this.ZipFileStream.Flush();
				this.ZipFileStream.Dispose();
			}
			this.ZipFileStream = null;
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x000C09E8 File Offset: 0x000BEBE8
		public List<ZipArchiveEntry> ReadCentralDir()
		{
			if (this.CentralDirImage == null)
			{
				throw new InvalidOperationException("Central directory currently does not exist");
			}
			List<ZipArchiveEntry> list = new List<ZipArchiveEntry>();
			int num = 0;
			while (num < this.CentralDirImage.Length && BitConverter.ToUInt32(this.CentralDirImage, num) == 33639248U)
			{
				ushort method = BitConverter.ToUInt16(this.CentralDirImage, num + 10);
				uint dt = BitConverter.ToUInt32(this.CentralDirImage, num + 12);
				uint crc = BitConverter.ToUInt32(this.CentralDirImage, num + 16);
				uint compressedSize = BitConverter.ToUInt32(this.CentralDirImage, num + 20);
				uint fileSize = BitConverter.ToUInt32(this.CentralDirImage, num + 24);
				ushort num2 = BitConverter.ToUInt16(this.CentralDirImage, num + 28);
				ushort num3 = BitConverter.ToUInt16(this.CentralDirImage, num + 30);
				ushort num4 = BitConverter.ToUInt16(this.CentralDirImage, num + 32);
				uint headerOffset = BitConverter.ToUInt32(this.CentralDirImage, num + 42);
				uint headerSize = (uint)(46 + num2 + num3 + num4);
				Encoding utf = Encoding.UTF8;
				ZipArchiveEntry zipArchiveEntry = new ZipArchiveEntry();
				zipArchiveEntry.Method = (ZipArchive.Compression)method;
				zipArchiveEntry.FilenameInZip = this.NormalizedFilename(utf.GetString(this.CentralDirImage, num + 46, (int)num2));
				zipArchiveEntry.FileOffset = this.GetFileOffset(headerOffset);
				zipArchiveEntry.FileSize = fileSize;
				zipArchiveEntry.CompressedSize = compressedSize;
				zipArchiveEntry.HeaderOffset = headerOffset;
				zipArchiveEntry.HeaderSize = headerSize;
				zipArchiveEntry.Crc32 = crc;
				zipArchiveEntry.ModifyTime = this.DosTimeToDateTime(dt);
				if (num4 > 0)
				{
					zipArchiveEntry.Comment = utf.GetString(this.CentralDirImage, num + 46 + (int)num2 + (int)num3, (int)num4);
				}
				list.Add(zipArchiveEntry);
				num += (int)(46 + num2 + num3 + num4);
			}
			return list;
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x000C0B9C File Offset: 0x000BED9C
		public void ExtractFile(ZipArchiveEntry zfe, Stream stream)
		{
			if (!stream.CanWrite)
			{
				throw new InvalidOperationException("Stream cannot be written");
			}
			byte[] array = new byte[4];
			this.ZipFileStream.Seek((long)((ulong)zfe.HeaderOffset), SeekOrigin.Begin);
			this.ZipFileStream.Read(array, 0, 4);
			if (BitConverter.ToUInt32(array, 0) != 67324752U)
			{
				throw new InvalidOperationException("Unsupported zip archive.");
			}
			Stream stream2;
			if (zfe.Method == ZipArchive.Compression.Store)
			{
				stream2 = this.ZipFileStream;
			}
			else
			{
				if (zfe.Method != ZipArchive.Compression.Deflate)
				{
					throw new InvalidOperationException("Unsupported zip archive.");
				}
				stream2 = new DeflateStream(this.ZipFileStream, CompressionMode.Decompress, true);
			}
			byte[] array2 = new byte[16384];
			this.ZipFileStream.Seek((long)((ulong)zfe.FileOffset), SeekOrigin.Begin);
			int num2;
			for (uint num = zfe.FileSize; num != 0U; num -= (uint)num2)
			{
				num2 = stream2.Read(array2, 0, (int)Math.Min((long)((ulong)num), (long)array2.Length));
				stream.Write(array2, 0, num2);
			}
			stream.Flush();
			if (zfe.Method == ZipArchive.Compression.Deflate)
			{
				stream2.Dispose();
			}
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x000C0C98 File Offset: 0x000BEE98
		public uint GetFileOffset(uint _headerOffset)
		{
			byte[] array = new byte[2];
			this.ZipFileStream.Seek((long)((ulong)(_headerOffset + 26U)), SeekOrigin.Begin);
			this.ZipFileStream.Read(array, 0, 2);
			ushort num = BitConverter.ToUInt16(array, 0);
			this.ZipFileStream.Read(array, 0, 2);
			ushort num2 = BitConverter.ToUInt16(array, 0);
			return (uint)((long)(30 + num + num2) + (long)((ulong)_headerOffset));
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x000C0CF8 File Offset: 0x000BEEF8
		public void WriteLocalHeader(ZipArchiveEntry _zfe)
		{
			long position = this.ZipFileStream.Position;
			byte[] bytes = Encoding.UTF8.GetBytes(_zfe.FilenameInZip);
			this.ZipFileStream.Write(new byte[]
			{
				80,
				75,
				3,
				4,
				20,
				0
			}, 0, 6);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(this.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			this.ZipFileStream.Write(new byte[12], 0, 12);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
			_zfe.HeaderSize = (uint)(this.ZipFileStream.Position - position);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x000C0DFC File Offset: 0x000BEFFC
		public void WriteCentralDirRecord(ZipArchiveEntry _zfe)
		{
			Encoding utf = Encoding.UTF8;
			byte[] bytes = utf.GetBytes(_zfe.FilenameInZip);
			byte[] bytes2 = utf.GetBytes(_zfe.Comment);
			this.ZipFileStream.Write(new byte[]
			{
				80,
				75,
				1,
				2,
				23,
				11,
				20,
				0
			}, 0, 8);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(this.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(33024), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
			this.ZipFileStream.Write(bytes2, 0, bytes2.Length);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x000C0FB0 File Offset: 0x000BF1B0
		public void WriteEndRecord(uint _size, uint _offset)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(this.Comment);
			this.ZipFileStream.Write(new byte[]
			{
				80,
				75,
				5,
				6,
				0,
				0,
				0,
				0
			}, 0, 8);
			this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x000C1084 File Offset: 0x000BF284
		public void Store(ZipArchiveEntry _zfe, Stream _source)
		{
			byte[] array = new byte[16384];
			uint num = 0U;
			long position = this.ZipFileStream.Position;
			long position2 = _source.Position;
			Stream stream = (_zfe.Method != ZipArchive.Compression.Store) ? new DeflateStream(this.ZipFileStream, CompressionMode.Compress, true) : this.ZipFileStream;
			_zfe.Crc32 = uint.MaxValue;
			int num2;
			do
			{
				num2 = _source.Read(array, 0, array.Length);
				num += (uint)num2;
				if (num2 > 0)
				{
					stream.Write(array, 0, num2);
					uint num3 = 0U;
					while ((ulong)num3 < (ulong)((long)num2))
					{
						_zfe.Crc32 = (ZipArchive.CrcTable[(int)((_zfe.Crc32 ^ (uint)array[(int)num3]) & 255U)] ^ _zfe.Crc32 >> 8);
						num3 += 1U;
					}
				}
			}
			while (num2 == array.Length);
			stream.Flush();
			if (_zfe.Method == ZipArchive.Compression.Deflate)
			{
				stream.Dispose();
			}
			_zfe.Crc32 ^= uint.MaxValue;
			_zfe.FileSize = num;
			_zfe.CompressedSize = (uint)(this.ZipFileStream.Position - position);
			if (_zfe.Method == ZipArchive.Compression.Deflate && !this.ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
			{
				_zfe.Method = ZipArchive.Compression.Store;
				this.ZipFileStream.Position = position;
				this.ZipFileStream.SetLength(position);
				_source.Position = position2;
				this.Store(_zfe, _source);
			}
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x000C11D0 File Offset: 0x000BF3D0
		public uint DateTimeToDosTime(DateTime _dt)
		{
			return (uint)(_dt.Second / 2 | _dt.Minute << 5 | _dt.Hour << 11 | _dt.Day << 16 | _dt.Month << 21 | _dt.Year - 1980 << 25);
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x000C1222 File Offset: 0x000BF422
		public DateTime DosTimeToDateTime(uint _dt)
		{
			return new DateTime((int)((_dt >> 25) + 1980U), (int)(_dt >> 21 & 15U), (int)(_dt >> 16 & 31U), (int)(_dt >> 11 & 31U), (int)(_dt >> 5 & 63U), (int)((_dt & 31U) * 2U));
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x000C1254 File Offset: 0x000BF454
		public void UpdateCrcAndSizes(ZipArchiveEntry _zfe)
		{
			long position = this.ZipFileStream.Position;
			this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 8U));
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 14U));
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			this.ZipFileStream.Position = position;
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x000C1304 File Offset: 0x000BF504
		public string NormalizedFilename(string _filename)
		{
			string text = _filename.Replace('\\', '/');
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Remove(0, num + 1);
			}
			return text.Trim(new char[]
			{
				'/'
			});
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000C1348 File Offset: 0x000BF548
		public bool ReadFileInfo()
		{
			if (this.ZipFileStream.Length < 22L)
			{
				return false;
			}
			try
			{
				this.ZipFileStream.Seek(-17L, SeekOrigin.End);
				BinaryReader binaryReader = new BinaryReader(this.ZipFileStream);
				for (;;)
				{
					this.ZipFileStream.Seek(-5L, SeekOrigin.Current);
					if (binaryReader.ReadUInt32() == 101010256U)
					{
						break;
					}
					if (this.ZipFileStream.Position <= 0L)
					{
						goto Block_6;
					}
				}
				this.ZipFileStream.Seek(6L, SeekOrigin.Current);
				ushort existingFiles = binaryReader.ReadUInt16();
				int num = binaryReader.ReadInt32();
				uint num2 = binaryReader.ReadUInt32();
				ushort num3 = binaryReader.ReadUInt16();
				if (this.ZipFileStream.Position + (long)((ulong)num3) != this.ZipFileStream.Length)
				{
					return false;
				}
				this.ExistingFiles = existingFiles;
				this.CentralDirImage = new byte[num];
				this.ZipFileStream.Seek((long)((ulong)num2), SeekOrigin.Begin);
				this.ZipFileStream.Read(this.CentralDirImage, 0, num);
				this.ZipFileStream.Seek((long)((ulong)num2), SeekOrigin.Begin);
				return true;
				Block_6:;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x000C1468 File Offset: 0x000BF668
		public void Dispose()
		{
			this.Close();
		}

		// Token: 0x0400113D RID: 4413
		public bool ForceDeflating;

		// Token: 0x0400113E RID: 4414
		public bool KeepStreamOpen;

		// Token: 0x0400113F RID: 4415
		public List<ZipArchiveEntry> Files = new List<ZipArchiveEntry>();

		// Token: 0x04001140 RID: 4416
		public Stream ZipFileStream;

		// Token: 0x04001141 RID: 4417
		public string Comment = "";

		// Token: 0x04001142 RID: 4418
		public byte[] CentralDirImage;

		// Token: 0x04001143 RID: 4419
		public ushort ExistingFiles;

		// Token: 0x04001144 RID: 4420
		public bool ReadOnly;

		// Token: 0x04001145 RID: 4421
		public static uint[] CrcTable = new uint[256];

		// Token: 0x0200050D RID: 1293
		public enum Compression : ushort
		{
			// Token: 0x040018BA RID: 6330
			Store,
			// Token: 0x040018BB RID: 6331
			Deflate = 8
		}
	}
}
