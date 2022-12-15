using System;

namespace Game
{
	// Token: 0x02000218 RID: 536
	public struct AnalyticsParameter
	{
		// Token: 0x06001081 RID: 4225 RVA: 0x0007DFDF File Offset: 0x0007C1DF
		public AnalyticsParameter(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x04000ACB RID: 2763
		public string Name;

		// Token: 0x04000ACC RID: 2764
		public string Value;
	}
}
