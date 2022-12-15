using System;
using System.Xml.Linq;
using Engine.Input;

namespace Game
{
	// Token: 0x02000287 RID: 647
	public class GamepadHelpDialog : Dialog
	{
		// Token: 0x06001324 RID: 4900 RVA: 0x00096398 File Offset: 0x00094598
		public GamepadHelpDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/GamepadHelpDialog");
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
			this.m_helpButton = this.Children.Find<ButtonWidget>("HelpButton", true);
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x000963EC File Offset: 0x000945EC
		public override void Update()
		{
			this.m_helpButton.IsVisible = !(ScreensManager.CurrentScreen is HelpScreen);
			if (this.m_okButton.IsClicked || base.Input.Cancel || base.Input.IsPadButtonDownOnce(GamePadButton.Start))
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_helpButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
		}

		// Token: 0x04000D24 RID: 3364
		public ButtonWidget m_okButton;

		// Token: 0x04000D25 RID: 3365
		public ButtonWidget m_helpButton;
	}
}
