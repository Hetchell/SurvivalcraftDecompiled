using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000154 RID: 340
	public class TrialEndedScreen : Screen
	{
		// Token: 0x06000675 RID: 1653 RVA: 0x00028564 File Offset: 0x00026764
		public TrialEndedScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/TrialEndedScreen");
			base.LoadContents(this, node);
			this.m_buyButton = this.Children.Find<ButtonWidget>("Buy", false);
			this.m_quitButton = this.Children.Find<ButtonWidget>("Quit", false);
			this.m_newWorldButton = this.Children.Find<ButtonWidget>("NewWorld", false);
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x000285D0 File Offset: 0x000267D0
		public override void Update()
		{
			if (this.m_buyButton != null && this.m_buyButton.IsClicked)
			{
				AnalyticsManager.LogEvent("[TrialEndedScreen] Clicked buy button", Array.Empty<AnalyticsParameter>());
				MarketplaceManager.ShowMarketplace();
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if ((this.m_quitButton != null && this.m_quitButton.IsClicked) || base.Input.Back)
			{
				AnalyticsManager.LogEvent("[TrialEndedScreen] Clicked quit button", Array.Empty<AnalyticsParameter>());
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if (this.m_newWorldButton != null && this.m_newWorldButton.IsClicked)
			{
				AnalyticsManager.LogEvent("[TrialEndedScreen] Clicked newworld button", Array.Empty<AnalyticsParameter>());
				ScreensManager.SwitchScreen("NewWorld", Array.Empty<object>());
			}
		}

		// Token: 0x0400037F RID: 895
		public ButtonWidget m_buyButton;

		// Token: 0x04000380 RID: 896
		public ButtonWidget m_quitButton;

		// Token: 0x04000381 RID: 897
		public ButtonWidget m_newWorldButton;
	}
}
