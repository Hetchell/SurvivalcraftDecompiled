using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000135 RID: 309
	public class ContentScreen : Screen
	{
		// Token: 0x060005B8 RID: 1464 RVA: 0x0002016C File Offset: 0x0001E36C
		public ContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ContentScreen");
			base.LoadContents(this, node);
			this.m_externalContentButton = this.Children.Find<ButtonWidget>("External", true);
			this.m_communityContentButton = this.Children.Find<ButtonWidget>("Community", true);
			this.m_linkButton = this.Children.Find<ButtonWidget>("Link", true);
			this.m_manageButton = this.Children.Find<ButtonWidget>("Manage", true);
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x000201F0 File Offset: 0x0001E3F0
		public override void Update()
		{
			this.m_communityContentButton.IsEnabled = (SettingsManager.CommunityContentMode > CommunityContentMode.Disabled);
			if (this.m_externalContentButton.IsClicked)
			{
				ScreensManager.SwitchScreen("ExternalContent", Array.Empty<object>());
			}
			if (this.m_communityContentButton.IsClicked)
			{
				ScreensManager.SwitchScreen("CommunityContent", Array.Empty<object>());
			}
			if (this.m_linkButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new DownloadContentFromLinkDialog());
			}
			if (this.m_manageButton.IsClicked)
			{
				ScreensManager.SwitchScreen("ManageContent", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x0400029A RID: 666
		public ButtonWidget m_externalContentButton;

		// Token: 0x0400029B RID: 667
		public ButtonWidget m_communityContentButton;

		// Token: 0x0400029C RID: 668
		public ButtonWidget m_linkButton;

		// Token: 0x0400029D RID: 669
		public ButtonWidget m_manageButton;
	}
}
