using System;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200038F RID: 911
	public class MessageWidget : CanvasWidget
	{
		// Token: 0x06001A65 RID: 6757 RVA: 0x000CFD10 File Offset: 0x000CDF10
		public MessageWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/MessageWidget");
			base.LoadContents(this, node);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Label", true);
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x000CFD50 File Offset: 0x000CDF50
		public void DisplayMessage(string text, Color color, bool blinking)
		{
			this.m_message = text;
			this.m_messageStartTime = Time.RealTime;
			float duration;
			if (!blinking)
			{
				duration = 4f + MathUtils.Min(1f * (float)this.m_message.Count((char c) => c == '\n'), 4f);
			}
			else
			{
				duration = 6f;
			}
			this.m_duration = duration;
			this.m_color = color;
			this.m_blinking = blinking;
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x000CFDD0 File Offset: 0x000CDFD0
		public override void Update()
		{
			double realTime = Time.RealTime;
			if (!string.IsNullOrEmpty(this.m_message))
			{
				float num;
				if (this.m_blinking)
				{
					num = MathUtils.Saturate(1f * (float)(this.m_messageStartTime + (double)this.m_duration - realTime));
					if (realTime - this.m_messageStartTime < 0.417)
					{
						num *= MathUtils.Lerp(0.25f, 1f, 0.5f * (1f - MathUtils.Cos(37.699112f * (float)(realTime - this.m_messageStartTime))));
					}
				}
				else
				{
					num = MathUtils.Saturate(MathUtils.Min(3f * (float)(realTime - this.m_messageStartTime), 1f * (float)(this.m_messageStartTime + (double)this.m_duration - realTime)));
				}
				this.m_labelWidget.Color = this.m_color * num;
				this.m_labelWidget.IsVisible = true;
				this.m_labelWidget.Text = this.m_message;
				if (realTime - this.m_messageStartTime > (double)this.m_duration)
				{
					this.m_message = null;
					return;
				}
			}
			else
			{
				this.m_labelWidget.IsVisible = false;
			}
		}

		// Token: 0x04001269 RID: 4713
		public LabelWidget m_labelWidget;

		// Token: 0x0400126A RID: 4714
		public string m_message;

		// Token: 0x0400126B RID: 4715
		public double m_messageStartTime;

		// Token: 0x0400126C RID: 4716
		public float m_duration;

		// Token: 0x0400126D RID: 4717
		public Color m_color;

		// Token: 0x0400126E RID: 4718
		public bool m_blinking;
	}
}
