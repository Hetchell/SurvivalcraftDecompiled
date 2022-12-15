using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200014D RID: 333
	public class SettingsAudioScreen : Screen
	{
		// Token: 0x06000660 RID: 1632 RVA: 0x00026B78 File Offset: 0x00024D78
		public SettingsAudioScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsAudioScreen");
			base.LoadContents(this, node);
			this.m_soundsVolumeSlider = this.Children.Find<SliderWidget>("SoundsVolumeSlider", true);
			this.m_musicVolumeSlider = this.Children.Find<SliderWidget>("MusicVolumeSlider", true);
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x00026BCC File Offset: 0x00024DCC
		public override void Update()
		{
			if (this.m_soundsVolumeSlider.IsSliding)
			{
				SettingsManager.SoundsVolume = this.m_soundsVolumeSlider.Value;
			}
			if (this.m_musicVolumeSlider.IsSliding)
			{
				SettingsManager.MusicVolume = this.m_musicVolumeSlider.Value;
			}
			this.m_soundsVolumeSlider.Value = SettingsManager.SoundsVolume;
			this.m_soundsVolumeSlider.Text = MathUtils.Round(SettingsManager.SoundsVolume * 10f).ToString();
			this.m_musicVolumeSlider.Value = SettingsManager.MusicVolume;
			this.m_musicVolumeSlider.Text = MathUtils.Round(SettingsManager.MusicVolume * 10f).ToString();
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000342 RID: 834
		public SliderWidget m_soundsVolumeSlider;

		// Token: 0x04000343 RID: 835
		public SliderWidget m_musicVolumeSlider;
	}
}
