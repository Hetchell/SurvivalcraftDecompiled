using System;

namespace Game
{
	// Token: 0x02000216 RID: 534
	public static class AnalyticsManager
	{
		// Token: 0x17000259 RID: 601
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x0007DD88 File Offset: 0x0007BF88
		public static string AnalyticsVersion
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0007DD8F File Offset: 0x0007BF8F
		public static void Initialize()
		{
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0007DD91 File Offset: 0x0007BF91
		public static void LogError(string message, Exception error)
		{
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0007DD93 File Offset: 0x0007BF93
		public static void LogEvent(string eventName, params AnalyticsParameter[] parameters)
		{
		}
	}
}
