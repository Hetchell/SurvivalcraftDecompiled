using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200026D RID: 621
	public class ExternalContentLinkDialog : Dialog
	{
		// Token: 0x0600126B RID: 4715 RVA: 0x0008E2D8 File Offset: 0x0008C4D8
		public ExternalContentLinkDialog(string link)
		{
			ClipboardManager.ClipboardString = link;
			XElement node = ContentManager.Get<XElement>("Dialogs/ExternalContentLinkDialog");
			base.LoadContents(this, node);
			this.m_textBoxWidget = this.Children.Find<TextBoxWidget>("ExternalContentLinkDialog.TextBox", true);
			this.m_okButtonWidget = this.Children.Find<ButtonWidget>("ExternalContentLinkDialog.OkButton", true);
			this.m_textBoxWidget.Text = link;
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0008E33E File Offset: 0x0008C53E
		public override void Update()
		{
			if (base.Input.Cancel || this.m_okButtonWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000CA6 RID: 3238
		public TextBoxWidget m_textBoxWidget;

		// Token: 0x04000CA7 RID: 3239
		public ButtonWidget m_okButtonWidget;
	}
}
