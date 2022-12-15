using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200031D RID: 797
	public class TextBoxDialog : Dialog
	{
		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x000B7D8C File Offset: 0x000B5F8C
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x000B7D94 File Offset: 0x000B5F94
		public bool AutoHide { get; set; }

		// Token: 0x060016FA RID: 5882 RVA: 0x000B7DA0 File Offset: 0x000B5FA0
		public TextBoxDialog(string title, string text, int maximumLength, Action<string> handler)
		{
			this.m_handler = handler;
			XElement node = ContentManager.Get<XElement>("Dialogs/TextBoxDialog");
			base.LoadContents(this, node);
			this.m_titleWidget = this.Children.Find<LabelWidget>("TextBoxDialog.Title", true);
			this.m_textBoxWidget = this.Children.Find<TextBoxWidget>("TextBoxDialog.TextBox", true);
			this.m_okButtonWidget = this.Children.Find<ButtonWidget>("TextBoxDialog.OkButton", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("TextBoxDialog.CancelButton", true);
			this.m_titleWidget.IsVisible = !string.IsNullOrEmpty(title);
			this.m_titleWidget.Text = (title ?? string.Empty);
			this.m_textBoxWidget.MaximumLength = maximumLength;
			this.m_textBoxWidget.Text = (text ?? string.Empty);
			this.m_textBoxWidget.HasFocus = true;
			this.m_textBoxWidget.Enter += delegate(TextBoxWidget p)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
			};
			this.AutoHide = true;
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x000B7EA0 File Offset: 0x000B60A0
		public override void Update()
		{
			if (base.Input.Cancel)
			{
				this.Dismiss(null);
				return;
			}
			if (base.Input.Ok)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
				return;
			}
			if (this.m_okButtonWidget.IsClicked)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
				return;
			}
			if (this.m_cancelButtonWidget.IsClicked)
			{
				this.Dismiss(null);
			}
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x000B7F14 File Offset: 0x000B6114
		public void Dismiss(string result)
		{
			if (this.AutoHide)
			{
				DialogsManager.HideDialog(this);
			}
			Action<string> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(result);
		}

		// Token: 0x040010A4 RID: 4260
		public Action<string> m_handler;

		// Token: 0x040010A5 RID: 4261
		public LabelWidget m_titleWidget;

		// Token: 0x040010A6 RID: 4262
		public TextBoxWidget m_textBoxWidget;

		// Token: 0x040010A7 RID: 4263
		public ButtonWidget m_okButtonWidget;

		// Token: 0x040010A8 RID: 4264
		public ButtonWidget m_cancelButtonWidget;
	}
}
