using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002DC RID: 732
	public class PublishCommunityLinkDialog : Dialog
	{
		// Token: 0x060014A3 RID: 5283 RVA: 0x000A032C File Offset: 0x0009E52C
		public PublishCommunityLinkDialog(string user, string address, string name)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/PublishCommunityLinkDialog");
			base.LoadContents(this, node);
			this.m_linkTextBoxWidget = this.Children.Find<TextBoxWidget>("PublishCommunityLinkDialog.Link", true);
			this.m_nameTextBoxWidget = this.Children.Find<TextBoxWidget>("PublishCommunityLinkDialog.Name", true);
			this.m_typeIconWidget = this.Children.Find<RectangleWidget>("PublishCommunityLinkDialog.TypeIcon", true);
			this.m_typeLabelWidget = this.Children.Find<LabelWidget>("PublishCommunityLinkDialog.Type", true);
			this.m_changeTypeButtonWidget = this.Children.Find<ButtonWidget>("PublishCommunityLinkDialog.ChangeType", true);
			this.m_publishButtonWidget = this.Children.Find<ButtonWidget>("PublishCommunityLinkDialog.Publish", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("PublishCommunityLinkDialog.Cancel", true);
			this.m_linkTextBoxWidget.TextChanged += delegate(TextBoxWidget p)
			{
				this.m_nameTextBoxWidget.Text = Storage.GetFileNameWithoutExtension(PublishCommunityLinkDialog.GetFilenameFromLink(this.m_linkTextBoxWidget.Text));
			};
			if (!string.IsNullOrEmpty(address))
			{
				this.m_linkTextBoxWidget.Text = address;
			}
			if (!string.IsNullOrEmpty(name))
			{
				this.m_nameTextBoxWidget.Text = name;
			}
			this.m_user = user;
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x000A0440 File Offset: 0x0009E640
		public override void Update()
		{
			string text = this.m_linkTextBoxWidget.Text.Trim();
			string text2 = this.m_nameTextBoxWidget.Text.Trim();
			this.m_typeLabelWidget.Text = ExternalContentManager.GetEntryTypeDescription(this.m_type);
			this.m_typeIconWidget.Subtexture = ExternalContentManager.GetEntryTypeIcon(this.m_type);
			this.m_publishButtonWidget.IsEnabled = (text.Length > 0 && text2.Length > 0);
			if (this.m_changeTypeButtonWidget.IsClicked)
			{
				DialogsManager.ShowDialog(base.ParentWidget, new SelectExternalContentTypeDialog("Select Content Type", delegate(ExternalContentType item)
				{
					this.m_type = item;
				}));
				return;
			}
			if (base.Input.Cancel || this.m_cancelButtonWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
				return;
			}
			if (this.m_publishButtonWidget.IsClicked)
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("Publishing", false);
				DialogsManager.ShowDialog(base.ParentWidget, busyDialog);
				CommunityContentManager.Publish(text, text2, this.m_type, this.m_user, busyDialog.Progress, delegate
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(this.ParentWidget, new MessageDialog("Link Published Successfully", "It should start appearing in the listings after it is moderated. Please keep the file accessible through this link, so that other community members can download it.", "OK", null, delegate(MessageDialogButton p)
					{
						DialogsManager.HideDialog(this);
					}));
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(this.ParentWidget, new MessageDialog("Error", error.Message, "OK", null, null));
				});
			}
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x000A0580 File Offset: 0x0009E780
		public static string GetFilenameFromLink(string address)
		{
			string result;
			try
			{
				string text = address;
				int num = text.IndexOf('&');
				if (num > 0)
				{
					text = text.Remove(num);
				}
				int num2 = text.IndexOf('?');
				if (num2 > 0)
				{
					text = text.Remove(num2);
				}
				text = Uri.UnescapeDataString(text);
				result = Storage.GetFileName(text);
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x04000EAF RID: 3759
		public TextBoxWidget m_linkTextBoxWidget;

		// Token: 0x04000EB0 RID: 3760
		public TextBoxWidget m_nameTextBoxWidget;

		// Token: 0x04000EB1 RID: 3761
		public RectangleWidget m_typeIconWidget;

		// Token: 0x04000EB2 RID: 3762
		public LabelWidget m_typeLabelWidget;

		// Token: 0x04000EB3 RID: 3763
		public ButtonWidget m_changeTypeButtonWidget;

		// Token: 0x04000EB4 RID: 3764
		public ButtonWidget m_publishButtonWidget;

		// Token: 0x04000EB5 RID: 3765
		public ButtonWidget m_cancelButtonWidget;

		// Token: 0x04000EB6 RID: 3766
		public string m_user;

		// Token: 0x04000EB7 RID: 3767
		public ExternalContentType m_type = ExternalContentType.BlocksTexture;
	}
}
