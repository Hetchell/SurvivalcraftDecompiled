using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000236 RID: 566
	public class CancellableBusyDialog : Dialog
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x0600116A RID: 4458 RVA: 0x0008745F File Offset: 0x0008565F
		// (set) Token: 0x0600116B RID: 4459 RVA: 0x00087467 File Offset: 0x00085667
		public CancellableProgress Progress { get; set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x0600116C RID: 4460 RVA: 0x00087470 File Offset: 0x00085670
		// (set) Token: 0x0600116D RID: 4461 RVA: 0x0008747D File Offset: 0x0008567D
		public string LargeMessage
		{
			get
			{
				return this.m_largeLabelWidget.Text;
			}
			set
			{
				this.m_largeLabelWidget.Text = (value ?? string.Empty);
				this.m_largeLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x000874A8 File Offset: 0x000856A8
		// (set) Token: 0x0600116F RID: 4463 RVA: 0x000874B5 File Offset: 0x000856B5
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = (value ?? string.Empty);
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x000874CC File Offset: 0x000856CC
		// (set) Token: 0x06001171 RID: 4465 RVA: 0x000874D9 File Offset: 0x000856D9
		public bool IsCancelButtonEnabled
		{
			get
			{
				return this.m_cancelButtonWidget.IsEnabled;
			}
			set
			{
				this.m_cancelButtonWidget.IsEnabled = value;
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x000874E8 File Offset: 0x000856E8
		public CancellableBusyDialog(string largeMessage, bool autoHideOnCancel)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/CancellableBusyDialog");
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("CancellableBusyDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("CancellableBusyDialog.SmallLabel", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("CancellableBusyDialog.CancelButton", true);
			this.Progress = new CancellableProgress();
			this.m_autoHideOnCancel = autoHideOnCancel;
			this.LargeMessage = largeMessage;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0008756C File Offset: 0x0008576C
		public override void Update()
		{
			if (this.Progress.Completed > 0f && this.Progress.Total > 0f)
			{
				this.SmallMessage = string.Format("{0:0}%", this.Progress.Completed / this.Progress.Total * 100f);
			}
			else
			{
				this.SmallMessage = string.Empty;
			}
			if (this.m_cancelButtonWidget.IsClicked)
			{
				this.Progress.Cancel();
				if (this.m_autoHideOnCancel)
				{
					DialogsManager.HideDialog(this);
				}
			}
			if (base.Input.Cancel)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x04000B9E RID: 2974
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000B9F RID: 2975
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000BA0 RID: 2976
		public ButtonWidget m_cancelButtonWidget;

		// Token: 0x04000BA1 RID: 2977
		public bool m_autoHideOnCancel;
	}
}
