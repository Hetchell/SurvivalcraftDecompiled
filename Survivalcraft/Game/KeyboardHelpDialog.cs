using System;
using System.Xml.Linq;
using Engine.Input;

namespace Game
{
	// Token: 0x0200029E RID: 670
	public class KeyboardHelpDialog : Dialog
	{
		// Token: 0x0600137D RID: 4989 RVA: 0x000970B8 File Offset: 0x000952B8
		public KeyboardHelpDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/KeyboardHelpDialog");
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
			this.m_helpButton = this.Children.Find<ButtonWidget>("HelpButton", true);
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0009710C File Offset: 0x0009530C
		public override void Update()
		{
			this.m_helpButton.IsVisible = !(ScreensManager.CurrentScreen is HelpScreen);
			if (this.m_okButton.IsClicked || base.Input.Cancel || base.Input.IsKeyDownOnce(Key.G))
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_helpButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
		}

		// Token: 0x04000D4E RID: 3406
		public ButtonWidget m_okButton;

		// Token: 0x04000D4F RID: 3407
		public ButtonWidget m_helpButton;
	}
}
