using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200032E RID: 814
	public abstract class VersionConverter
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x0600172E RID: 5934
		public abstract string SourceVersion { get; }

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x0600172F RID: 5935
		public abstract string TargetVersion { get; }

		// Token: 0x06001730 RID: 5936
		public abstract void ConvertProjectXml(XElement projectNode);

		// Token: 0x06001731 RID: 5937
		public abstract void ConvertWorld(string directoryName);
	}
}
