using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200014F RID: 335
	public class SettingsControlsScreen : Screen
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x00026F2C File Offset: 0x0002512C
		public SettingsControlsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsControlsScreen");
			base.LoadContents(this, node);
			this.m_moveControlModeButton = this.Children.Find<ButtonWidget>("MoveControlMode", true);
			this.m_lookControlModeButton = this.Children.Find<ButtonWidget>("LookControlMode", true);
			this.m_leftHandedLayoutButton = this.Children.Find<ButtonWidget>("LeftHandedLayout", true);
			this.m_flipVerticalAxisButton = this.Children.Find<ButtonWidget>("FlipVerticalAxis", true);
			this.m_autoJumpButton = this.Children.Find<ButtonWidget>("AutoJump", true);
			this.m_horizontalCreativeFlightButton = this.Children.Find<ButtonWidget>("HorizontalCreativeFlight", true);
			this.m_horizontalCreativeFlightPanel = this.Children.Find<ContainerWidget>("HorizontalCreativeFlightPanel", true);
			this.m_moveSensitivitySlider = this.Children.Find<SliderWidget>("MoveSensitivitySlider", true);
			this.m_lookSensitivitySlider = this.Children.Find<SliderWidget>("LookSensitivitySlider", true);
			this.m_gamepadCursorSpeedSlider = this.Children.Find<SliderWidget>("GamepadCursorSpeedSlider", true);
			this.m_gamepadDeadZoneSlider = this.Children.Find<SliderWidget>("GamepadDeadZoneSlider", true);
			this.m_creativeDigTimeSlider = this.Children.Find<SliderWidget>("CreativeDigTimeSlider", true);
			this.m_creativeReachSlider = this.Children.Find<SliderWidget>("CreativeReachSlider", true);
			this.m_holdDurationSlider = this.Children.Find<SliderWidget>("HoldDurationSlider", true);
			this.m_dragDistanceSlider = this.Children.Find<SliderWidget>("DragDistanceSlider", true);
			this.m_horizontalCreativeFlightPanel.IsVisible = false;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x000270B8 File Offset: 0x000252B8
		public override void Update()
		{
			if (this.m_moveControlModeButton.IsClicked)
			{
				SettingsManager.MoveControlMode = (MoveControlMode)(((int)SettingsManager.MoveControlMode + 1) % EnumUtils.GetEnumValues(typeof(MoveControlMode)).Count);
			}
			if (this.m_lookControlModeButton.IsClicked)
			{
				SettingsManager.LookControlMode = (LookControlMode)(((int)SettingsManager.LookControlMode + 1) % EnumUtils.GetEnumValues(typeof(LookControlMode)).Count);
			}
			if (this.m_leftHandedLayoutButton.IsClicked)
			{
				SettingsManager.LeftHandedLayout = !SettingsManager.LeftHandedLayout;
			}
			if (this.m_flipVerticalAxisButton.IsClicked)
			{
				SettingsManager.FlipVerticalAxis = !SettingsManager.FlipVerticalAxis;
			}
			if (this.m_autoJumpButton.IsClicked)
			{
				SettingsManager.AutoJump = !SettingsManager.AutoJump;
			}
			if (this.m_horizontalCreativeFlightButton.IsClicked)
			{
				SettingsManager.HorizontalCreativeFlight = !SettingsManager.HorizontalCreativeFlight;
			}
			if (this.m_moveSensitivitySlider.IsSliding)
			{
				SettingsManager.MoveSensitivity = this.m_moveSensitivitySlider.Value;
			}
			if (this.m_lookSensitivitySlider.IsSliding)
			{
				SettingsManager.LookSensitivity = this.m_lookSensitivitySlider.Value;
			}
			if (this.m_gamepadCursorSpeedSlider.IsSliding)
			{
				SettingsManager.GamepadCursorSpeed = this.m_gamepadCursorSpeedSlider.Value;
			}
			if (this.m_gamepadDeadZoneSlider.IsSliding)
			{
				SettingsManager.GamepadDeadZone = this.m_gamepadDeadZoneSlider.Value;
			}
			if (this.m_creativeDigTimeSlider.IsSliding)
			{
				SettingsManager.CreativeDigTime = this.m_creativeDigTimeSlider.Value;
			}
			if (this.m_creativeReachSlider.IsSliding)
			{
				SettingsManager.CreativeReach = this.m_creativeReachSlider.Value;
			}
			if (this.m_holdDurationSlider.IsSliding)
			{
				SettingsManager.MinimumHoldDuration = this.m_holdDurationSlider.Value;
			}
			if (this.m_dragDistanceSlider.IsSliding)
			{
				SettingsManager.MinimumDragDistance = this.m_dragDistanceSlider.Value;
			}
			this.m_moveControlModeButton.Text = LanguageControl.Get("MoveControlMode", SettingsManager.MoveControlMode.ToString());
			this.m_lookControlModeButton.Text = LanguageControl.Get("LookControlMode", SettingsManager.LookControlMode.ToString());
			this.m_leftHandedLayoutButton.Text = (SettingsManager.LeftHandedLayout ? LanguageControl.Get("Usual", "on") : LanguageControl.Get("Usual", "off"));
			this.m_flipVerticalAxisButton.Text = (SettingsManager.FlipVerticalAxis ? LanguageControl.Get("Usual", "on") : LanguageControl.Get("Usual", "off"));
			this.m_autoJumpButton.Text = (SettingsManager.AutoJump ? LanguageControl.Get("Usual", "on") : LanguageControl.Get("Usual", "off"));
			this.m_horizontalCreativeFlightButton.Text = (SettingsManager.HorizontalCreativeFlight ? LanguageControl.Get("Usual", "on") : LanguageControl.Get("Usual", "off"));
			this.m_moveSensitivitySlider.Value = SettingsManager.MoveSensitivity;
			this.m_moveSensitivitySlider.Text = MathUtils.Round(SettingsManager.MoveSensitivity * 10f).ToString();
			this.m_lookSensitivitySlider.Value = SettingsManager.LookSensitivity;
			this.m_lookSensitivitySlider.Text = MathUtils.Round(SettingsManager.LookSensitivity * 10f).ToString();
			this.m_gamepadCursorSpeedSlider.Value = SettingsManager.GamepadCursorSpeed;
			this.m_gamepadCursorSpeedSlider.Text = string.Format("{0:0.0}x", SettingsManager.GamepadCursorSpeed);
			this.m_gamepadDeadZoneSlider.Value = SettingsManager.GamepadDeadZone;
			this.m_gamepadDeadZoneSlider.Text = string.Format("{0:0}%", SettingsManager.GamepadDeadZone * 100f);
			this.m_creativeDigTimeSlider.Value = SettingsManager.CreativeDigTime;
			this.m_creativeDigTimeSlider.Text = string.Format("{0}ms", MathUtils.Round(1000f * SettingsManager.CreativeDigTime));
			this.m_creativeReachSlider.Value = SettingsManager.CreativeReach;
			this.m_creativeReachSlider.Text = string.Format(LanguageControl.Get(SettingsControlsScreen.fName, 1), string.Format("{0:0.0} ", SettingsManager.CreativeReach));
			this.m_holdDurationSlider.Value = SettingsManager.MinimumHoldDuration;
			this.m_holdDurationSlider.Text = string.Format("{0}ms", MathUtils.Round(1000f * SettingsManager.MinimumHoldDuration));
			this.m_dragDistanceSlider.Value = SettingsManager.MinimumDragDistance;
			this.m_dragDistanceSlider.Text = string.Format("{0} ", MathUtils.Round(SettingsManager.MinimumDragDistance)) + LanguageControl.Get(SettingsControlsScreen.fName, 2);
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x0400034C RID: 844
		public ButtonWidget m_moveControlModeButton;

		// Token: 0x0400034D RID: 845
		public ButtonWidget m_lookControlModeButton;

		// Token: 0x0400034E RID: 846
		public ButtonWidget m_leftHandedLayoutButton;

		// Token: 0x0400034F RID: 847
		public ButtonWidget m_flipVerticalAxisButton;

		// Token: 0x04000350 RID: 848
		public ButtonWidget m_autoJumpButton;

		// Token: 0x04000351 RID: 849
		public ButtonWidget m_horizontalCreativeFlightButton;

		// Token: 0x04000352 RID: 850
		public ContainerWidget m_horizontalCreativeFlightPanel;

		// Token: 0x04000353 RID: 851
		public SliderWidget m_moveSensitivitySlider;

		// Token: 0x04000354 RID: 852
		public SliderWidget m_lookSensitivitySlider;

		// Token: 0x04000355 RID: 853
		public SliderWidget m_gamepadCursorSpeedSlider;

		// Token: 0x04000356 RID: 854
		public SliderWidget m_gamepadDeadZoneSlider;

		// Token: 0x04000357 RID: 855
		public SliderWidget m_creativeDigTimeSlider;

		// Token: 0x04000358 RID: 856
		public SliderWidget m_creativeReachSlider;

		// Token: 0x04000359 RID: 857
		public SliderWidget m_holdDurationSlider;

		// Token: 0x0400035A RID: 858
		public SliderWidget m_dragDistanceSlider;

		// Token: 0x0400035B RID: 859
		public static string fName = "SettingsControlsScreen";
	}
}
