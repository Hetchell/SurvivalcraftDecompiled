using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200039A RID: 922
	public class SliderWidget : CanvasWidget
	{
		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06001AE4 RID: 6884 RVA: 0x000D33AB File Offset: 0x000D15AB
		// (set) Token: 0x06001AE5 RID: 6885 RVA: 0x000D33B3 File Offset: 0x000D15B3
		public bool IsSliding { get; set; }

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x000D33BC File Offset: 0x000D15BC
		// (set) Token: 0x06001AE7 RID: 6887 RVA: 0x000D33C4 File Offset: 0x000D15C4
		public LayoutDirection LayoutDirection { get; set; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06001AE8 RID: 6888 RVA: 0x000D33CD File Offset: 0x000D15CD
		// (set) Token: 0x06001AE9 RID: 6889 RVA: 0x000D33D8 File Offset: 0x000D15D8
		public float MinValue
		{
			get
			{
				return this.m_minValue;
			}
			set
			{
				if (value != this.m_minValue)
				{
					this.m_minValue = value;
					this.MaxValue = MathUtils.Max(this.MinValue, this.MaxValue);
					this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
				}
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06001AEA RID: 6890 RVA: 0x000D3429 File Offset: 0x000D1629
		// (set) Token: 0x06001AEB RID: 6891 RVA: 0x000D3434 File Offset: 0x000D1634
		public float MaxValue
		{
			get
			{
				return this.m_maxValue;
			}
			set
			{
				if (value != this.m_maxValue)
				{
					this.m_maxValue = value;
					this.MinValue = MathUtils.Min(this.MinValue, this.MaxValue);
					this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
				}
			}
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001AEC RID: 6892 RVA: 0x000D3485 File Offset: 0x000D1685
		// (set) Token: 0x06001AED RID: 6893 RVA: 0x000D3490 File Offset: 0x000D1690
		public float Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (this.m_granularity > 0f)
				{
					this.m_value = MathUtils.Round(MathUtils.Clamp(value, this.MinValue, this.MaxValue) / this.m_granularity) * this.m_granularity;
					return;
				}
				this.m_value = MathUtils.Clamp(value, this.MinValue, this.MaxValue);
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001AEE RID: 6894 RVA: 0x000D34EE File Offset: 0x000D16EE
		// (set) Token: 0x06001AEF RID: 6895 RVA: 0x000D34F6 File Offset: 0x000D16F6
		public float Granularity
		{
			get
			{
				return this.m_granularity;
			}
			set
			{
				this.m_granularity = MathUtils.Max(value, 0f);
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x000D3509 File Offset: 0x000D1709
		// (set) Token: 0x06001AF1 RID: 6897 RVA: 0x000D3516 File Offset: 0x000D1716
		public string Text
		{
			get
			{
				return this.m_labelWidget.Text;
			}
			set
			{
				this.m_labelWidget.Text = value;
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001AF2 RID: 6898 RVA: 0x000D3524 File Offset: 0x000D1724
		// (set) Token: 0x06001AF3 RID: 6899 RVA: 0x000D3531 File Offset: 0x000D1731
		public BitmapFont Font
		{
			get
			{
				return this.m_labelWidget.Font;
			}
			set
			{
				this.m_labelWidget.Font = value;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001AF4 RID: 6900 RVA: 0x000D353F File Offset: 0x000D173F
		// (set) Token: 0x06001AF5 RID: 6901 RVA: 0x000D3547 File Offset: 0x000D1747
		public string SoundName { get; set; }

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x000D3550 File Offset: 0x000D1750
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x000D355D File Offset: 0x000D175D
		public bool IsLabelVisible
		{
			get
			{
				return this.m_labelCanvasWidget.IsVisible;
			}
			set
			{
				this.m_labelCanvasWidget.IsVisible = value;
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x000D356B File Offset: 0x000D176B
		// (set) Token: 0x06001AF9 RID: 6905 RVA: 0x000D357D File Offset: 0x000D177D
		public float LabelWidth
		{
			get
			{
				return this.m_labelCanvasWidget.Size.X;
			}
			set
			{
				this.m_labelCanvasWidget.Size = new Vector2(value, this.m_labelCanvasWidget.Size.Y);
			}
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x000D35A0 File Offset: 0x000D17A0
		public SliderWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/SliderContents");
			base.LoadChildren(this, node);
			this.m_canvasWidget = this.Children.Find<CanvasWidget>("Slider.Canvas", true);
			this.m_labelCanvasWidget = this.Children.Find<CanvasWidget>("Slider.LabelCanvas", true);
			this.m_tabWidget = this.Children.Find<Widget>("Slider.Tab", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Slider.Label", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x06001AFB RID: 6907 RVA: 0x000D3640 File Offset: 0x000D1840
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.MeasureOverride(parentAvailableSize);
			base.IsDrawRequired = true;
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x000D3650 File Offset: 0x000D1850
		public override void ArrangeOverride()
		{
			base.ArrangeOverride();
			float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_canvasWidget.ActualSize.X : this.m_canvasWidget.ActualSize.Y;
			float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_tabWidget.ActualSize.X : this.m_tabWidget.ActualSize.Y;
			float num3 = (this.MaxValue > this.MinValue) ? ((this.Value - this.MinValue) / (this.MaxValue - this.MinValue)) : 0f;
			if (this.LayoutDirection == LayoutDirection.Horizontal)
			{
				Vector2 zero = Vector2.Zero;
				zero.X = num3 * (num - num2);
				zero.Y = MathUtils.Max((base.ActualSize.Y - this.m_tabWidget.ActualSize.Y) / 2f, 0f);
				this.m_canvasWidget.SetWidgetPosition(this.m_tabWidget, new Vector2?(zero));
			}
			else
			{
				Vector2 zero2 = Vector2.Zero;
				zero2.X = MathUtils.Max(base.ActualSize.X - this.m_tabWidget.ActualSize.X, 0f) / 2f;
				zero2.Y = num3 * (num - num2);
				this.m_canvasWidget.SetWidgetPosition(this.m_tabWidget, new Vector2?(zero2));
			}
			base.ArrangeOverride();
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x000D37B4 File Offset: 0x000D19B4
		public override void Update()
		{
			float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_canvasWidget.ActualSize.X : this.m_canvasWidget.ActualSize.Y;
			float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_tabWidget.ActualSize.X : this.m_tabWidget.ActualSize.Y;
			if (base.Input.Tap != null && base.HitTestGlobal(base.Input.Tap.Value, null) == this.m_tabWidget)
			{
				this.m_dragStartPoint = new Vector2?(base.ScreenToWidget(base.Input.Press.Value));
			}
			if (base.Input.Press != null)
			{
				if (this.m_dragStartPoint != null)
				{
					Vector2 vector = base.ScreenToWidget(base.Input.Press.Value);
					float value = this.Value;
					if (this.LayoutDirection == LayoutDirection.Horizontal)
					{
						float f = (vector.X - num2 / 2f) / (num - num2);
						this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f);
					}
					else
					{
						float f2 = (vector.Y - num2 / 2f) / (num - num2);
						this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f2);
					}
					if (this.Value != value && this.m_granularity > 0f && !string.IsNullOrEmpty(this.SoundName))
					{
						AudioManager.PlaySound(this.SoundName, 1f, 0f, 0f);
					}
				}
			}
			else
			{
				this.m_dragStartPoint = null;
			}
			this.IsSliding = (this.m_dragStartPoint != null && base.IsEnabledGlobal && base.IsVisibleGlobal);
			if (this.m_dragStartPoint != null)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x040012B8 RID: 4792
		public CanvasWidget m_canvasWidget;

		// Token: 0x040012B9 RID: 4793
		public CanvasWidget m_labelCanvasWidget;

		// Token: 0x040012BA RID: 4794
		public Widget m_tabWidget;

		// Token: 0x040012BB RID: 4795
		public LabelWidget m_labelWidget;

		// Token: 0x040012BC RID: 4796
		public float m_minValue;

		// Token: 0x040012BD RID: 4797
		public float m_maxValue = 1f;

		// Token: 0x040012BE RID: 4798
		public float m_granularity = 0.1f;

		// Token: 0x040012BF RID: 4799
		public float m_value;

		// Token: 0x040012C0 RID: 4800
		public Vector2? m_dragStartPoint;
	}
}
