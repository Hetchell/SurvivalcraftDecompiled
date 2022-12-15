using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000232 RID: 562
	public class BusyDialog : Dialog
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600113F RID: 4415 RVA: 0x00086FD1 File Offset: 0x000851D1
		// (set) Token: 0x06001140 RID: 4416 RVA: 0x00086FDE File Offset: 0x000851DE
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

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x00087009 File Offset: 0x00085209
		// (set) Token: 0x06001142 RID: 4418 RVA: 0x00087016 File Offset: 0x00085216
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = (value ?? string.Empty);
				this.m_smallLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
			}
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x00087044 File Offset: 0x00085244
		public BusyDialog(string largeMessage, string smallMessage)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/BusyDialog");
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.SmallLabel", true);
			this.LargeMessage = largeMessage;
			this.SmallMessage = smallMessage;
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x000870A6 File Offset: 0x000852A6
		public override void Update()
		{
			if (base.Input.Back)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x04000B96 RID: 2966
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000B97 RID: 2967
		public LabelWidget m_smallLabelWidget;
	}
}
