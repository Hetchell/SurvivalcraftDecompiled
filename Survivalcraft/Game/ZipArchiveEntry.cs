using System;

namespace Game
{
	// Token: 0x02000361 RID: 865
	public class ZipArchiveEntry
	{
		// Token: 0x06001861 RID: 6241 RVA: 0x000C1470 File Offset: 0x000BF670
		public override string ToString()
		{
			return this.FilenameInZip;
		}

		// Token: 0x04001146 RID: 4422
		public ZipArchive.Compression Method;

		// Token: 0x04001147 RID: 4423
		public string FilenameInZip;

		// Token: 0x04001148 RID: 4424
		public uint FileSize;

		// Token: 0x04001149 RID: 4425
		public uint CompressedSize;

		// Token: 0x0400114A RID: 4426
		public uint HeaderOffset;

		// Token: 0x0400114B RID: 4427
		public uint FileOffset;

		// Token: 0x0400114C RID: 4428
		public uint HeaderSize;

		// Token: 0x0400114D RID: 4429
		public uint Crc32;

		// Token: 0x0400114E RID: 4430
		public DateTime ModifyTime;

		// Token: 0x0400114F RID: 4431
		public string Comment;

		// Token: 0x04001150 RID: 4432
		public bool EncodeUTF8;
	}
}
