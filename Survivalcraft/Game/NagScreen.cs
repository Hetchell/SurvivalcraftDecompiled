using System;
using System.Xml.Linq;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x0200013E RID: 318
	public class NagScreen : Screen
	{
		// Token: 0x060005ED RID: 1517 RVA: 0x00022040 File Offset: 0x00020240
		public NagScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/NagScreen");
			base.LoadContents(this, node);
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00022068 File Offset: 0x00020268
		public override void Enter(object[] parameters)
		{
			Keyboard.BackButtonQuitsApp = true;
			this.Children.Find<Widget>("Quit", true).IsVisible = true;
			this.Children.Find<Widget>("QuitLabel_Wp81", true).IsVisible = false;
			this.Children.Find<Widget>("QuitLabel_Win81", true).IsVisible = false;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x000220C0 File Offset: 0x000202C0
		public override void Leave()
		{
			Keyboard.BackButtonQuitsApp = false;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x000220C8 File Offset: 0x000202C8
		public override void Update()
		{
			if (this.Children.Find<ButtonWidget>("Buy", true).IsClicked)
			{
				AnalyticsManager.LogEvent("[NagScreen] Clicked buy button", Array.Empty<AnalyticsParameter>());
				MarketplaceManager.ShowMarketplace();
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Quit", true).IsClicked || base.Input.Back)
			{
				AnalyticsManager.LogEvent("[NagScreen] Clicked quit button", Array.Empty<AnalyticsParameter>());
				Window.Close();
			}
		}
	}
}
