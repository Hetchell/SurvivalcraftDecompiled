using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200014E RID: 334
	public class SettingsCompatibilityScreen : Screen
	{
		// Token: 0x06000662 RID: 1634 RVA: 0x00026CBC File Offset: 0x00024EBC
		public SettingsCompatibilityScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsCompatibilityScreen");
			base.LoadContents(this, node);
			this.m_singlethreadedTerrainUpdateButton = this.Children.Find<ButtonWidget>("SinglethreadedTerrainUpdateButton", true);
			this.m_useAudioTrackCachingButton = this.Children.Find<ButtonWidget>("UseAudioTrackCachingButton", true);
			this.m_disableAudioTrackCachingContainer = this.Children.Find<ContainerWidget>("DisableAudioTrackCachingContainer", true);
			this.m_useReducedZRangeButton = this.Children.Find<ButtonWidget>("UseReducedZRangeButton", true);
			this.m_useReducedZRangeContainer = this.Children.Find<ContainerWidget>("UseReducedZRangeContainer", true);
			this.m_viewGameLogButton = this.Children.Find<ButtonWidget>("ViewGameLogButton", true);
			this.m_resetDefaultsButton = this.Children.Find<ButtonWidget>("ResetDefaultsButton", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_useAudioTrackCachingButton.IsVisible = false;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00026DA6 File Offset: 0x00024FA6
		public override void Enter(object[] parameters)
		{
			this.m_descriptionLabel.Text = string.Empty;
			this.m_disableAudioTrackCachingContainer.IsVisible = false;
			this.m_useReducedZRangeContainer.IsVisible = false;
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x00026DD0 File Offset: 0x00024FD0
		public override void Update()
		{
			if (this.m_singlethreadedTerrainUpdateButton.IsClicked)
			{
				SettingsManager.MultithreadedTerrainUpdate = !SettingsManager.MultithreadedTerrainUpdate;
				this.m_descriptionLabel.Text = StringsManager.GetString("Settings.Compatibility.SinglethreadedTerrainUpdate.Description");
			}
			if (this.m_useReducedZRangeButton.IsClicked)
			{
				SettingsManager.UseReducedZRange = !SettingsManager.UseReducedZRange;
				this.m_descriptionLabel.Text = StringsManager.GetString("Settings.Compatibility.UseReducedZRange.Description");
			}
			if (this.m_viewGameLogButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new ViewGameLogDialog());
			}
			if (this.m_resetDefaultsButton.IsClicked)
			{
				SettingsManager.MultithreadedTerrainUpdate = true;
				SettingsManager.UseReducedZRange = false;
			}
			this.m_singlethreadedTerrainUpdateButton.Text = (SettingsManager.MultithreadedTerrainUpdate ? LanguageControl.Get("Usual", "off") : LanguageControl.Get("Usual", "on"));
			this.m_useReducedZRangeButton.Text = (SettingsManager.UseReducedZRange ? LanguageControl.Get("Usual", "on") : LanguageControl.Get("Usual", "off"));
			this.m_resetDefaultsButton.IsEnabled = (!SettingsManager.MultithreadedTerrainUpdate || SettingsManager.UseReducedZRange);
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000344 RID: 836
		public ButtonWidget m_singlethreadedTerrainUpdateButton;

		// Token: 0x04000345 RID: 837
		public ButtonWidget m_useAudioTrackCachingButton;

		// Token: 0x04000346 RID: 838
		public ContainerWidget m_disableAudioTrackCachingContainer;

		// Token: 0x04000347 RID: 839
		public ButtonWidget m_useReducedZRangeButton;

		// Token: 0x04000348 RID: 840
		public ContainerWidget m_useReducedZRangeContainer;

		// Token: 0x04000349 RID: 841
		public ButtonWidget m_viewGameLogButton;

		// Token: 0x0400034A RID: 842
		public ButtonWidget m_resetDefaultsButton;

		// Token: 0x0400034B RID: 843
		public LabelWidget m_descriptionLabel;
	}
}
