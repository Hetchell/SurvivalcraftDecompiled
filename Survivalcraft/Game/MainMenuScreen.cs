using System;
using System.Xml.Linq;
using Engine;
using Engine.Input;
using Survivalcraft.Game.ModificationHolder;

namespace Game
{
	// Token: 0x0200013C RID: 316
	public class MainMenuScreen : Screen
	{
		// Token: 0x060005E3 RID: 1507 RVA: 0x0002178C File Offset: 0x0001F98C
		public MainMenuScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/MainMenuScreen");
			//ModificationsHolder.nodeForMainScreen = node;
			base.LoadContents(this, node);
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x000217BD File Offset: 0x0001F9BD
		public override void Enter(object[] parameters)
		{
			this.Children.Find<MotdWidget>(true).Restart();
			if (SettingsManager.IsolatedStorageMigrationCounter < 3)
			{
				SettingsManager.IsolatedStorageMigrationCounter++;
				VersionConverter126To127.MigrateDataFromIsolatedStorageWithDialog();
			}
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x000217E9 File Offset: 0x0001F9E9
		public override void Leave()
		{
			Keyboard.BackButtonQuitsApp = false;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x000217F4 File Offset: 0x0001F9F4
		public override void Update()
		{
			Keyboard.BackButtonQuitsApp = !MarketplaceManager.IsTrialMode;
			if (string.IsNullOrEmpty(this.m_versionString) || MarketplaceManager.IsTrialMode != this.m_versionStringTrial)
			{
				this.m_versionString = string.Format("Version {0}{1}", VersionsManager.Version, MarketplaceManager.IsTrialMode ? " (Day One)" : string.Empty);
				this.m_versionStringTrial = MarketplaceManager.IsTrialMode;
			}
			this.Children.Find("Buy", true).IsVisible = MarketplaceManager.IsTrialMode;
			this.Children.Find<LabelWidget>("Version", true).Text = this.m_versionString;
			RectangleWidget rectangleWidget = this.Children.Find<RectangleWidget>("Logo", true);
			float num = 1f + 0.02f * MathUtils.Sin(1.5f * (float)MathUtils.Remainder(Time.FrameStartTime, 10000.0));
			rectangleWidget.RenderTransform = Matrix.CreateTranslation((0f - rectangleWidget.ActualSize.X) / 2f, (0f - rectangleWidget.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(rectangleWidget.ActualSize.X / 2f, rectangleWidget.ActualSize.Y / 2f, 0f);
			if (this.Children.Find<ButtonWidget>("Play", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Help", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Content", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Settings", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Settings", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Buy", true).IsClicked)
			{
				AnalyticsManager.LogEvent("[MainMenuScreen] Clicked buy button", Array.Empty<AnalyticsParameter>());
				MarketplaceManager.ShowMarketplace();
			}
			if ((base.Input.Back && !Keyboard.BackButtonQuitsApp) || base.Input.IsKeyDownOnce(Key.Enter))
			{
				if (MarketplaceManager.IsTrialMode)
				{
					ScreensManager.SwitchScreen("Nag", Array.Empty<object>());
					return;
				}
				Window.Close();
			}
		}

		// Token: 0x040002BE RID: 702
		public string m_versionString = string.Empty;

		// Token: 0x040002BF RID: 703
		public bool m_versionStringTrial;
	}
}
