using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200026C RID: 620
	public class ExternalContentEntry
	{
		// Token: 0x04000CA1 RID: 3233
		public ExternalContentType Type;

		// Token: 0x04000CA2 RID: 3234
		public string Path;

		// Token: 0x04000CA3 RID: 3235
		public long Size;

		// Token: 0x04000CA4 RID: 3236
		public DateTime Time;

		// Token: 0x04000CA5 RID: 3237
		public List<ExternalContentEntry> ChildEntries = new List<ExternalContentEntry>();
	}
}
