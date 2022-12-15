using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000152 RID: 338
	public class SettingsScreen : Screen
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x00027F10 File Offset: 0x00026110
		public SettingsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsScreen");
			base.LoadContents(this, node);
			this.m_performanceButton = this.Children.Find<ButtonWidget>("Performance", true);
			this.m_graphicsButton = this.Children.Find<ButtonWidget>("Graphics", true);
			this.m_uiButton = this.Children.Find<ButtonWidget>("Ui", true);
			this.m_compatibilityButton = this.Children.Find<ButtonWidget>("Compatibility", true);
			this.m_audioButton = this.Children.Find<ButtonWidget>("Audio", true);
			this.m_controlsButton = this.Children.Find<ButtonWidget>("Controls", true);
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00027FC0 File Offset: 0x000261C0
		public override void Enter(object[] parameters)
		{
			if (this.m_previousScreen == null)
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00027FD8 File Offset: 0x000261D8
		public override void Update()
		{
			if (this.m_performanceButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsPerformance", Array.Empty<object>());
			}
			if (this.m_graphicsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsGraphics", Array.Empty<object>());
			}
			if (this.m_uiButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsUi", Array.Empty<object>());
			}
			if (this.m_compatibilityButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsCompatibility", Array.Empty<object>());
			}
			if (this.m_audioButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsAudio", Array.Empty<object>());
			}
			if (this.m_controlsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsControls", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
				this.m_previousScreen = null;
			}
		}

		// Token: 0x0400036D RID: 877
		public Screen m_previousScreen;

		// Token: 0x0400036E RID: 878
		public ButtonWidget m_performanceButton;

		// Token: 0x0400036F RID: 879
		public ButtonWidget m_graphicsButton;

		// Token: 0x04000370 RID: 880
		public ButtonWidget m_uiButton;

		// Token: 0x04000371 RID: 881
		public ButtonWidget m_compatibilityButton;

		// Token: 0x04000372 RID: 882
		public ButtonWidget m_audioButton;

		// Token: 0x04000373 RID: 883
		public ButtonWidget m_controlsButton;
	}
}
