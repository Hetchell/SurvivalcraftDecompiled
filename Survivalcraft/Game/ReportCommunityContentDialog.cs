using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002E5 RID: 741
	public class ReportCommunityContentDialog : Dialog
	{
		// Token: 0x060014DC RID: 5340 RVA: 0x000A19D0 File Offset: 0x0009FBD0
		public ReportCommunityContentDialog(string address, string displayName, string userId)
		{
			this.m_address = address;
			this.m_userId = userId;
			XElement node = ContentManager.Get<XElement>("Dialogs/ReportCommunityContentDialog");
			base.LoadContents(this, node);
			this.m_nameLabel = this.Children.Find<LabelWidget>("ReportCommunityContentDialog.Name", true);
			this.m_container = this.Children.Find<ContainerWidget>("ReportCommunityContentDialog.Container", true);
			this.m_reportButton = this.Children.Find<ButtonWidget>("ReportCommunityContentDialog.Report", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("ReportCommunityContentDialog.Cancel", true);
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Cruelty",
				Tag = "cruelty"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Dating",
				Tag = "dating"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Drugs / Alcohol",
				Tag = "drugs"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Hate Speech",
				Tag = "hate"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Plagiarism",
				Tag = "plagiarism"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Racism",
				Tag = "racism"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Sex / Nudity",
				Tag = "sex"
			});
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Excessive Swearing",
				Tag = "swearing"
			});
			Random random = new Random();
			this.m_reasonWidgetsList.RandomShuffle((int max) => random.Int(0, max - 1));
			this.m_reasonWidgetsList.Add(new CheckboxWidget
			{
				Text = "Other",
				Tag = "other"
			});
			foreach (CheckboxWidget widget in this.m_reasonWidgetsList)
			{
				this.m_container.Children.Add(widget);
			}
			this.m_nameLabel.Text = displayName;
			this.m_reportButton.IsEnabled = false;
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x000A1C54 File Offset: 0x0009FE54
		public override void Update()
		{
			this.m_reportButton.IsEnabled = (this.m_reasonWidgetsList.Count((CheckboxWidget w) => w.IsChecked) == 1);
			if (this.m_reportButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog("Are you sure?", "Reporting offensive content is a serious matter. Please make sure you checked the right box. Do not report content which is not offensive.", "Proceed", "Cancel", delegate(MessageDialogButton b)
				{
					if (b == MessageDialogButton.Button1)
					{
						string report = string.Empty;
						foreach (CheckboxWidget checkboxWidget in this.m_reasonWidgetsList)
						{
							if (checkboxWidget.IsChecked)
							{
								report = (string)checkboxWidget.Tag;
								break;
							}
						}
						CancellableBusyDialog busyDialog = new CancellableBusyDialog("Sending Report", false);
						DialogsManager.ShowDialog(base.ParentWidget, busyDialog);
						CommunityContentManager.Report(this.m_address, this.m_userId, report, busyDialog.Progress, delegate
						{
							DialogsManager.HideDialog(busyDialog);
						}, delegate
						{
							DialogsManager.HideDialog(busyDialog);
						});
					}
				}));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000ED9 RID: 3801
		public string m_address;

		// Token: 0x04000EDA RID: 3802
		public string m_userId;

		// Token: 0x04000EDB RID: 3803
		public LabelWidget m_nameLabel;

		// Token: 0x04000EDC RID: 3804
		public ContainerWidget m_container;

		// Token: 0x04000EDD RID: 3805
		public ButtonWidget m_reportButton;

		// Token: 0x04000EDE RID: 3806
		public ButtonWidget m_cancelButton;

		// Token: 0x04000EDF RID: 3807
		public List<CheckboxWidget> m_reasonWidgetsList = new List<CheckboxWidget>();
	}
}
