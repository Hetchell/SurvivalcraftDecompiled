using System;
using System.Diagnostics;
using Engine;

namespace Game
{
	// Token: 0x02000355 RID: 853
	public static class WebBrowserManager
	{
		// Token: 0x06001807 RID: 6151 RVA: 0x000BE104 File Offset: 0x000BC304
		public static void LaunchBrowser(string url)
		{
			AnalyticsManager.LogEvent("[WebBrowserManager] Launching browser", new AnalyticsParameter[]
			{
				new AnalyticsParameter("Url", url)
			});
			if (!url.Contains("://"))
			{
				url = "http://" + url;
			}
			try
			{
				Process.Start(url);
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error launching web browser with URL \"{0}\". Reason: {1}", new object[]
				{
					url,
					ex.Message
				}));
			}
		}
	}
}
