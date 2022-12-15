using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002E2 RID: 738
	public class RateCommunityContentDialog : Dialog
	{
		// Token: 0x060014D6 RID: 5334 RVA: 0x000A1654 File Offset: 0x0009F854
		public RateCommunityContentDialog(string address, string displayName, string userId)
		{
			this.m_address = address;
			this.m_displayName = displayName;
			this.m_userId = userId;
			XElement node = ContentManager.Get<XElement>("Dialogs/RateCommunityContentDialog");
			base.LoadContents(this, node);
			this.m_nameLabel = this.Children.Find<LabelWidget>("RateCommunityContentDialog.Name", true);
			this.m_starRating = this.Children.Find<StarRatingWidget>("RateCommunityContentDialog.StarRating", true);
			this.m_rateButton = this.Children.Find<ButtonWidget>("RateCommunityContentDialog.Rate", true);
			this.m_reportLink = this.Children.Find<LinkWidget>("RateCommunityContentDialog.Report", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("RateCommunityContentDialog.Cancel", true);
			this.m_nameLabel.Text = displayName;
			this.m_rateButton.IsEnabled = false;
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x000A171C File Offset: 0x0009F91C
		public override void Update()
		{
			this.m_rateButton.IsEnabled = (this.m_starRating.Rating != 0f);
			if (this.m_rateButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("Sending Rating", false);
				DialogsManager.ShowDialog(base.ParentWidget, busyDialog);
				CommunityContentManager.Rate(this.m_address, this.m_userId, (int)this.m_starRating.Rating, busyDialog.Progress, delegate
				{
					DialogsManager.HideDialog(busyDialog);
				}, delegate
				{
					DialogsManager.HideDialog(busyDialog);
				});
			}
			if (this.m_reportLink.IsClicked && UserManager.ActiveUser != null)
			{
				DialogsManager.HideDialog(this);
				DialogsManager.ShowDialog(base.ParentWidget, new ReportCommunityContentDialog(this.m_address, this.m_displayName, UserManager.ActiveUser.UniqueId));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000ECA RID: 3786
		public string m_address;

		// Token: 0x04000ECB RID: 3787
		public string m_displayName;

		// Token: 0x04000ECC RID: 3788
		public string m_userId;

		// Token: 0x04000ECD RID: 3789
		public LabelWidget m_nameLabel;

		// Token: 0x04000ECE RID: 3790
		public StarRatingWidget m_starRating;

		// Token: 0x04000ECF RID: 3791
		public ButtonWidget m_rateButton;

		// Token: 0x04000ED0 RID: 3792
		public LinkWidget m_reportLink;

		// Token: 0x04000ED1 RID: 3793
		public ButtonWidget m_cancelButton;
	}
}
