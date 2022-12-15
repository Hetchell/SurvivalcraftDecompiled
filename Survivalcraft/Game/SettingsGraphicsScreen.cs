using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000150 RID: 336
	public class SettingsGraphicsScreen : Screen
	{
		// Token: 0x06000668 RID: 1640 RVA: 0x00027598 File Offset: 0x00025798
		public SettingsGraphicsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsGraphicsScreen");
			base.LoadContents(this, node);
			this.m_virtualRealityButton = this.Children.Find<BevelledButtonWidget>("VirtualRealityButton", true);
			this.m_brightnessSlider = this.Children.Find<SliderWidget>("BrightnessSlider", true);
			this.m_vrPanel = this.Children.Find<ContainerWidget>("VrPanel", true);
			this.m_vrPanel.IsVisible = false;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00027610 File Offset: 0x00025810
		public override void Update()
		{
			if (this.m_virtualRealityButton.IsClicked)
			{
				if (SettingsManager.UseVr)
				{
					SettingsManager.UseVr = false;
					VrManager.StopVr();
				}
				else
				{
					SettingsManager.UseVr = true;
					VrManager.StartVr();
				}
			}
			if (this.m_brightnessSlider.IsSliding)
			{
				SettingsManager.Brightness = this.m_brightnessSlider.Value;
			}
			this.m_virtualRealityButton.IsEnabled = VrManager.IsVrAvailable;
			this.m_virtualRealityButton.Text = (SettingsManager.UseVr ? "Enabled" : "Disabled");
			this.m_brightnessSlider.Value = SettingsManager.Brightness;
			this.m_brightnessSlider.Text = MathUtils.Round(SettingsManager.Brightness * 10f).ToString();
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x0400035C RID: 860
		public BevelledButtonWidget m_virtualRealityButton;

		// Token: 0x0400035D RID: 861
		public SliderWidget m_brightnessSlider;

		// Token: 0x0400035E RID: 862
		public ContainerWidget m_vrPanel;
	}
}
