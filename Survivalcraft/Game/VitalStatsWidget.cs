using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020003A4 RID: 932
	public class VitalStatsWidget : CanvasWidget
	{
		// Token: 0x06001B85 RID: 7045 RVA: 0x000D65BC File Offset: 0x000D47BC
		public VitalStatsWidget(ComponentPlayer componentPlayer)
		{
			this.m_componentPlayer = componentPlayer;
			XElement node = ContentManager.Get<XElement>("Widgets/VitalStatsWidget");
			base.LoadContents(this, node);
			this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
			this.m_healthLink = this.Children.Find<LinkWidget>("HealthLink", true);
			this.m_healthValueBar = this.Children.Find<ValueBarWidget>("HealthValueBar", true);
			this.m_staminaLink = this.Children.Find<LinkWidget>("StaminaLink", true);
			this.m_staminaValueBar = this.Children.Find<ValueBarWidget>("StaminaValueBar", true);
			this.m_foodLink = this.Children.Find<LinkWidget>("FoodLink", true);
			this.m_foodValueBar = this.Children.Find<ValueBarWidget>("FoodValueBar", true);
			this.m_sleepLink = this.Children.Find<LinkWidget>("SleepLink", true);
			this.m_sleepValueBar = this.Children.Find<ValueBarWidget>("SleepValueBar", true);
			this.m_temperatureLink = this.Children.Find<LinkWidget>("TemperatureLink", true);
			this.m_temperatureValueBar = this.Children.Find<ValueBarWidget>("TemperatureValueBar", true);
			this.m_wetnessLink = this.Children.Find<LinkWidget>("WetnessLink", true);
			this.m_wetnessValueBar = this.Children.Find<ValueBarWidget>("WetnessValueBar", true);
			this.m_chokeButton = this.Children.Find<ButtonWidget>("ChokeButton", true);
			this.m_strengthLink = this.Children.Find<LinkWidget>("StrengthLink", true);
			this.m_strengthLabel = this.Children.Find<LabelWidget>("StrengthLabel", true);
			this.m_resilienceLink = this.Children.Find<LinkWidget>("ResilienceLink", true);
			this.m_resilienceLabel = this.Children.Find<LabelWidget>("ResilienceLabel", true);
			this.m_speedLink = this.Children.Find<LinkWidget>("SpeedLink", true);
			this.m_speedLabel = this.Children.Find<LabelWidget>("SpeedLabel", true);
			this.m_hungerLink = this.Children.Find<LinkWidget>("HungerLink", true);
			this.m_hungerLabel = this.Children.Find<LabelWidget>("HungerLabel", true);
			this.m_experienceLink = this.Children.Find<LinkWidget>("ExperienceLink", true);
			this.m_experienceValueBar = this.Children.Find<ValueBarWidget>("ExperienceValueBar", true);
			this.m_insulationLink = this.Children.Find<LinkWidget>("InsulationLink", true);
			this.m_insulationLabel = this.Children.Find<LabelWidget>("InsulationLabel", true);
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x000D6840 File Offset: 0x000D4A40
		public override void Update()
		{
			this.m_titleLabel.Text = string.Format("{0}, Level {1} {2}", this.m_componentPlayer.PlayerData.Name, MathUtils.Floor(this.m_componentPlayer.PlayerData.Level), this.m_componentPlayer.PlayerData.PlayerClass.ToString());
			this.m_healthValueBar.Value = this.m_componentPlayer.ComponentHealth.Health;
			this.m_staminaValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Stamina;
			this.m_foodValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Food;
			this.m_sleepValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Sleep;
			this.m_temperatureValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Temperature / 24f;
			this.m_wetnessValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Wetness;
			this.m_experienceValueBar.Value = this.m_componentPlayer.PlayerData.Level - MathUtils.Floor(this.m_componentPlayer.PlayerData.Level);
			this.m_strengthLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.StrengthFactor);
			this.m_resilienceLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.ResilienceFactor);
			this.m_speedLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.SpeedFactor);
			this.m_hungerLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.HungerFactor);
			this.m_insulationLabel.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00} clo", this.m_componentPlayer.ComponentClothing.Insulation);
			if (this.m_healthLink.IsClicked)
			{
				HelpTopic topic = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Health");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic.Title, topic.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_staminaLink.IsClicked)
			{
				HelpTopic topic2 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Stamina");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic2.Title, topic2.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_foodLink.IsClicked)
			{
				HelpTopic topic3 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Hunger");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic3.Title, topic3.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_sleepLink.IsClicked)
			{
				HelpTopic topic4 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Sleep");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic4.Title, topic4.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_temperatureLink.IsClicked)
			{
				HelpTopic topic5 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Temperature");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic5.Title, topic5.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_wetnessLink.IsClicked)
			{
				HelpTopic topic6 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Wetness");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic6.Title, topic6.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_strengthLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors = new List<ComponentLevel.Factor>();
				float total = this.m_componentPlayer.ComponentLevel.CalculateStrengthFactor(factors);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(VitalStatsWidget.fName, "Strength"), LanguageControl.GetContentWidgets(VitalStatsWidget.fName, 16), factors, total));
			}
			if (this.m_resilienceLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors2 = new List<ComponentLevel.Factor>();
				float total2 = this.m_componentPlayer.ComponentLevel.CalculateResilienceFactor(factors2);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(VitalStatsWidget.fName, "Resilience"), LanguageControl.GetContentWidgets(VitalStatsWidget.fName, 17), factors2, total2));
			}
			if (this.m_speedLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors3 = new List<ComponentLevel.Factor>();
				float total3 = this.m_componentPlayer.ComponentLevel.CalculateSpeedFactor(factors3);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(VitalStatsWidget.fName, "Speed"), LanguageControl.GetContentWidgets(VitalStatsWidget.fName, 18), factors3, total3));
			}
			if (this.m_hungerLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors4 = new List<ComponentLevel.Factor>();
				float total4 = this.m_componentPlayer.ComponentLevel.CalculateHungerFactor(factors4);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(VitalStatsWidget.fName, "Hunger"), LanguageControl.GetContentWidgets(VitalStatsWidget.fName, 19), factors4, total4));
			}
			if (this.m_experienceLink.IsClicked)
			{
				HelpTopic topic7 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Levels");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic7.Title, topic7.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_insulationLink.IsClicked)
			{
				HelpTopic topic8 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Clothing");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic8.Title, topic8.Text, LanguageControl.Get("Usual", "ok"), null, new Vector2(700f, 360f), null));
			}
			if (this.m_chokeButton.IsClicked)
			{
				this.m_componentPlayer.ComponentHealth.Injure(0.1f, null, true, LanguageControl.Get(VitalStatsWidget.fName, "Choked"));
			}
		}

		// Token: 0x04001309 RID: 4873
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400130A RID: 4874
		public ButtonWidget m_chokeButton;

		// Token: 0x0400130B RID: 4875
		public LabelWidget m_titleLabel;

		// Token: 0x0400130C RID: 4876
		public LinkWidget m_healthLink;

		// Token: 0x0400130D RID: 4877
		public ValueBarWidget m_healthValueBar;

		// Token: 0x0400130E RID: 4878
		public LinkWidget m_staminaLink;

		// Token: 0x0400130F RID: 4879
		public ValueBarWidget m_staminaValueBar;

		// Token: 0x04001310 RID: 4880
		public LinkWidget m_foodLink;

		// Token: 0x04001311 RID: 4881
		public ValueBarWidget m_foodValueBar;

		// Token: 0x04001312 RID: 4882
		public LinkWidget m_sleepLink;

		// Token: 0x04001313 RID: 4883
		public ValueBarWidget m_sleepValueBar;

		// Token: 0x04001314 RID: 4884
		public LinkWidget m_temperatureLink;

		// Token: 0x04001315 RID: 4885
		public ValueBarWidget m_temperatureValueBar;

		// Token: 0x04001316 RID: 4886
		public LinkWidget m_wetnessLink;

		// Token: 0x04001317 RID: 4887
		public ValueBarWidget m_wetnessValueBar;

		// Token: 0x04001318 RID: 4888
		public LinkWidget m_strengthLink;

		// Token: 0x04001319 RID: 4889
		public LabelWidget m_strengthLabel;

		// Token: 0x0400131A RID: 4890
		public LinkWidget m_resilienceLink;

		// Token: 0x0400131B RID: 4891
		public LabelWidget m_resilienceLabel;

		// Token: 0x0400131C RID: 4892
		public LinkWidget m_speedLink;

		// Token: 0x0400131D RID: 4893
		public LabelWidget m_speedLabel;

		// Token: 0x0400131E RID: 4894
		public LinkWidget m_hungerLink;

		// Token: 0x0400131F RID: 4895
		public LabelWidget m_hungerLabel;

		// Token: 0x04001320 RID: 4896
		public LinkWidget m_experienceLink;

		// Token: 0x04001321 RID: 4897
		public ValueBarWidget m_experienceValueBar;

		// Token: 0x04001322 RID: 4898
		public LinkWidget m_insulationLink;

		// Token: 0x04001323 RID: 4899
		public LabelWidget m_insulationLabel;

		// Token: 0x04001324 RID: 4900
		public static string fName = "VitalStatsWidget";
	}
}
