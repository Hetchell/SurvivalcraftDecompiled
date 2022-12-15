using System;

namespace Game
{
	// Token: 0x020002A9 RID: 681
	public static class MarketplaceManager
	{
		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600139C RID: 5020 RVA: 0x00098651 File Offset: 0x00096851
		// (set) Token: 0x0600139D RID: 5021 RVA: 0x0009866A File Offset: 0x0009686A
		public static bool IsTrialMode
		{
			get
			{
				if (!MarketplaceManager.m_isInitialized)
				{
					throw new InvalidOperationException("MarketplaceManager not initialized.");
				}
				return MarketplaceManager.m_isTrialMode;
			}
			set
			{
				MarketplaceManager.m_isTrialMode = value;
			}
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00098672 File Offset: 0x00096872
		public static void Initialize()
		{
			MarketplaceManager.m_isInitialized = true;
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x0009867A File Offset: 0x0009687A
		public static void ShowMarketplace()
		{
			AnalyticsManager.LogEvent("[MarketplaceManager] Show marketplace", Array.Empty<AnalyticsParameter>());
			WebBrowserManager.LaunchBrowser("http://play.google.com/store/apps/details?id=com.candyrufusgames.survivalcraft2");
		}

		// Token: 0x04000D74 RID: 3444
		public static bool m_isInitialized;

		// Token: 0x04000D75 RID: 3445
		public static bool m_isTrialMode;
	}
}
