using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002AD RID: 685
	public class MessageDialog : Dialog
	{
		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x00098D2C File Offset: 0x00096F2C
		// (set) Token: 0x060013B0 RID: 5040 RVA: 0x00098D34 File Offset: 0x00096F34
		public bool AutoHide { get; set; }

		// Token: 0x060013B1 RID: 5041 RVA: 0x00098D40 File Offset: 0x00096F40
		public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Vector2 size, Action<MessageDialogButton> handler)
		{
			this.m_handler = handler;
			XElement node = ContentManager.Get<XElement>("Dialogs/MessageDialog");
			base.LoadContents(this, node);
			base.Size = new Vector2((size.X >= 0f) ? size.X : base.Size.X, (size.Y >= 0f) ? size.Y : base.Size.Y);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("MessageDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("MessageDialog.SmallLabel", true);
			this.m_button1Widget = this.Children.Find<ButtonWidget>("MessageDialog.Button1", true);
			this.m_button2Widget = this.Children.Find<ButtonWidget>("MessageDialog.Button2", true);
			this.m_largeLabelWidget.IsVisible = !string.IsNullOrEmpty(largeMessage);
			this.m_largeLabelWidget.Text = (largeMessage ?? string.Empty);
			this.m_smallLabelWidget.IsVisible = !string.IsNullOrEmpty(smallMessage);
			this.m_smallLabelWidget.Text = (smallMessage ?? string.Empty);
			this.m_button1Widget.IsVisible = !string.IsNullOrEmpty(button1Text);
			this.m_button1Widget.Text = (button1Text ?? string.Empty);
			this.m_button2Widget.IsVisible = !string.IsNullOrEmpty(button2Text);
			this.m_button2Widget.Text = (button2Text ?? string.Empty);
			if (!this.m_button1Widget.IsVisible && !this.m_button2Widget.IsVisible)
			{
				throw new InvalidOperationException("MessageDialog must have at least one button.");
			}
			this.AutoHide = true;
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x00098EEB File Offset: 0x000970EB
		public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Action<MessageDialogButton> handler) : this(largeMessage, smallMessage, button1Text, button2Text, new Vector2(-1f), handler)
		{
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x00098F04 File Offset: 0x00097104
		public override void Update()
		{
			if (base.Input.Cancel)
			{
				if (this.m_button2Widget.IsVisible)
				{
					this.Dismiss(MessageDialogButton.Button2);
					return;
				}
				this.Dismiss(MessageDialogButton.Button1);
				return;
			}
			else
			{
				if (base.Input.Ok || this.m_button1Widget.IsClicked)
				{
					this.Dismiss(MessageDialogButton.Button1);
					return;
				}
				if (this.m_button2Widget.IsClicked)
				{
					this.Dismiss(MessageDialogButton.Button2);
				}
				return;
			}
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x00098F71 File Offset: 0x00097171
		public void Dismiss(MessageDialogButton button)
		{
			if (this.AutoHide)
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_handler != null)
			{
				this.m_handler(button);
			}
		}

		// Token: 0x04000D7D RID: 3453
		public Action<MessageDialogButton> m_handler;

		// Token: 0x04000D7E RID: 3454
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000D7F RID: 3455
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000D80 RID: 3456
		public ButtonWidget m_button1Widget;

		// Token: 0x04000D81 RID: 3457
		public ButtonWidget m_button2Widget;
	}
}
