using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000153 RID: 339
	public class SettingsUiScreen : Screen
	{
		// Token: 0x06000671 RID: 1649 RVA: 0x000280D8 File Offset: 0x000262D8
		public SettingsUiScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsUiScreen");
			base.LoadContents(this, node);
			this.m_windowModeContainer = this.Children.Find<ContainerWidget>("WindowModeContainer", true);
			this.m_languageButton = this.Children.Find<ButtonWidget>("LanguageButton", true);
			this.m_windowModeButton = this.Children.Find<ButtonWidget>("WindowModeButton", true);
			this.m_uiSizeButton = this.Children.Find<ButtonWidget>("UiSizeButton", true);
			this.m_upsideDownButton = this.Children.Find<ButtonWidget>("UpsideDownButton", true);
			this.m_hideMoveLookPadsButton = this.Children.Find<ButtonWidget>("HideMoveLookPads", true);
			this.m_showGuiInScreenshotsButton = this.Children.Find<ButtonWidget>("ShowGuiInScreenshotsButton", true);
			this.m_showLogoInScreenshotsButton = this.Children.Find<ButtonWidget>("ShowLogoInScreenshotsButton", true);
			this.m_screenshotSizeButton = this.Children.Find<ButtonWidget>("ScreenshotSizeButton", true);
			this.m_communityContentModeButton = this.Children.Find<ButtonWidget>("CommunityContentModeButton", true);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000281E4 File Offset: 0x000263E4
		public override void Enter(object[] parameters)
		{
			this.m_windowModeContainer.IsVisible = true;
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000281F4 File Offset: 0x000263F4
		public override void Update()
		{
			if (this.m_windowModeButton.IsClicked)
			{
				SettingsManager.WindowMode = (Game.WindowMode)((int)(SettingsManager.WindowMode + 1) % EnumUtils.GetEnumValues(typeof(Engine.WindowMode)).Count);
            }
			if (this.m_languageButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(SettingsUiScreen.fName, 1), LanguageControl.Get(SettingsUiScreen.fName, 2), LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						ModsManager.modSettings.languageType = (LanguageControl.LanguageType)(((int)ModsManager.modSettings.languageType + 1) % EnumUtils.GetEnumValues(typeof(LanguageControl.LanguageType)).Count);
						ModsManager.SaveSettings();
						Environment.Exit(0);
					}
				}));
			}
			if (this.m_uiSizeButton.IsClicked)
			{
				SettingsManager.GuiSize = (GuiSize)(((int)SettingsManager.GuiSize + 1) % EnumUtils.GetEnumValues(typeof(GuiSize)).Count);
			}
			if (this.m_upsideDownButton.IsClicked)
			{
				SettingsManager.UpsideDownLayout = !SettingsManager.UpsideDownLayout;
			}
			if (this.m_hideMoveLookPadsButton.IsClicked)
			{
				SettingsManager.HideMoveLookPads = !SettingsManager.HideMoveLookPads;
			}
			if (this.m_showGuiInScreenshotsButton.IsClicked)
			{
				SettingsManager.ShowGuiInScreenshots = !SettingsManager.ShowGuiInScreenshots;
			}
			if (this.m_showLogoInScreenshotsButton.IsClicked)
			{
				SettingsManager.ShowLogoInScreenshots = !SettingsManager.ShowLogoInScreenshots;
			}
			if (this.m_screenshotSizeButton.IsClicked)
			{
				SettingsManager.ScreenshotSize = (ScreenshotSize)(((int)SettingsManager.ScreenshotSize + 1) % EnumUtils.GetEnumValues(typeof(ScreenshotSize)).Count);
			}
			if (this.m_communityContentModeButton.IsClicked)
			{
				SettingsManager.CommunityContentMode = (CommunityContentMode)(((int)SettingsManager.CommunityContentMode + 1) % EnumUtils.GetEnumValues(typeof(CommunityContentMode)).Count);
			}
			this.m_windowModeButton.Text = LanguageControl.Get("WindowMode", SettingsManager.WindowMode.ToString());
			this.m_uiSizeButton.Text = LanguageControl.Get("GuiSize", SettingsManager.GuiSize.ToString());
			this.m_languageButton.Text = ModsManager.modSettings.languageType.ToString();
			this.m_upsideDownButton.Text = (SettingsManager.UpsideDownLayout ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
			this.m_hideMoveLookPadsButton.Text = (SettingsManager.HideMoveLookPads ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
			this.m_showGuiInScreenshotsButton.Text = (SettingsManager.ShowGuiInScreenshots ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
			this.m_showLogoInScreenshotsButton.Text = (SettingsManager.ShowLogoInScreenshots ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
			this.m_screenshotSizeButton.Text = LanguageControl.Get("ScreenshotSize", SettingsManager.ScreenshotSize.ToString());
			this.m_communityContentModeButton.Text = LanguageControl.Get("CommunityContentMode", SettingsManager.CommunityContentMode.ToString());
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000374 RID: 884
		public ContainerWidget m_windowModeContainer;

		// Token: 0x04000375 RID: 885
		public ButtonWidget m_windowModeButton;

		// Token: 0x04000376 RID: 886
		public ButtonWidget m_languageButton;

		// Token: 0x04000377 RID: 887
		public ButtonWidget m_uiSizeButton;

		// Token: 0x04000378 RID: 888
		public ButtonWidget m_upsideDownButton;

		// Token: 0x04000379 RID: 889
		public ButtonWidget m_hideMoveLookPadsButton;

		// Token: 0x0400037A RID: 890
		public ButtonWidget m_showGuiInScreenshotsButton;

		// Token: 0x0400037B RID: 891
		public ButtonWidget m_showLogoInScreenshotsButton;

		// Token: 0x0400037C RID: 892
		public ButtonWidget m_screenshotSizeButton;

		// Token: 0x0400037D RID: 893
		public ButtonWidget m_communityContentModeButton;

		// Token: 0x0400037E RID: 894
		public static string fName = "SettingsUiScreen";
	}
}
