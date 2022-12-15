using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002FB RID: 763
	public class SpawnDialog : Dialog
	{
		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060015A1 RID: 5537 RVA: 0x000A5273 File Offset: 0x000A3473
		// (set) Token: 0x060015A2 RID: 5538 RVA: 0x000A5280 File Offset: 0x000A3480
		public string LargeMessage
		{
			get
			{
				return this.m_largeLabelWidget.Text;
			}
			set
			{
				this.m_largeLabelWidget.Text = value;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060015A3 RID: 5539 RVA: 0x000A528E File Offset: 0x000A348E
		// (set) Token: 0x060015A4 RID: 5540 RVA: 0x000A529B File Offset: 0x000A349B
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = value;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x000A52A9 File Offset: 0x000A34A9
		// (set) Token: 0x060015A6 RID: 5542 RVA: 0x000A52B6 File Offset: 0x000A34B6
		public float Progress
		{
			get
			{
				return this.m_progressWidget.Value;
			}
			set
			{
				this.m_progressWidget.Value = value;
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x000A52C4 File Offset: 0x000A34C4
		public SpawnDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/SpawnDialog");
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("SpawnDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("SpawnDialog.SmallLabel", true);
			this.m_progressWidget = this.Children.Find<ValueBarWidget>("SpawnDialog.Progress", true);
		}

		// Token: 0x04000F63 RID: 3939
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000F64 RID: 3940
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000F65 RID: 3941
		public ValueBarWidget m_progressWidget;
	}
}
