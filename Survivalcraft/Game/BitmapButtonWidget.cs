using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200036B RID: 875
	public class BitmapButtonWidget : ButtonWidget
	{
		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x060018E2 RID: 6370 RVA: 0x000C54FA File Offset: 0x000C36FA
		public override bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x060018E3 RID: 6371 RVA: 0x000C5507 File Offset: 0x000C3707
		// (set) Token: 0x060018E4 RID: 6372 RVA: 0x000C5514 File Offset: 0x000C3714
		public override bool IsChecked
		{
			get
			{
				return this.m_clickableWidget.IsChecked;
			}
			set
			{
				this.m_clickableWidget.IsChecked = value;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x060018E5 RID: 6373 RVA: 0x000C5522 File Offset: 0x000C3722
		// (set) Token: 0x060018E6 RID: 6374 RVA: 0x000C552F File Offset: 0x000C372F
		public override bool IsAutoCheckingEnabled
		{
			get
			{
				return this.m_clickableWidget.IsAutoCheckingEnabled;
			}
			set
			{
				this.m_clickableWidget.IsAutoCheckingEnabled = value;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x060018E7 RID: 6375 RVA: 0x000C553D File Offset: 0x000C373D
		// (set) Token: 0x060018E8 RID: 6376 RVA: 0x000C554A File Offset: 0x000C374A
		public override string Text
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

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060018E9 RID: 6377 RVA: 0x000C5558 File Offset: 0x000C3758
		// (set) Token: 0x060018EA RID: 6378 RVA: 0x000C5565 File Offset: 0x000C3765
		public override BitmapFont Font
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

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x060018EB RID: 6379 RVA: 0x000C5573 File Offset: 0x000C3773
		// (set) Token: 0x060018EC RID: 6380 RVA: 0x000C557B File Offset: 0x000C377B
		public Subtexture NormalSubtexture { get; set; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x060018ED RID: 6381 RVA: 0x000C5584 File Offset: 0x000C3784
		// (set) Token: 0x060018EE RID: 6382 RVA: 0x000C558C File Offset: 0x000C378C
		public Subtexture ClickedSubtexture { get; set; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x060018EF RID: 6383 RVA: 0x000C5595 File Offset: 0x000C3795
		// (set) Token: 0x060018F0 RID: 6384 RVA: 0x000C559D File Offset: 0x000C379D
		public override Color Color { get; set; }

		// Token: 0x060018F1 RID: 6385 RVA: 0x000C55A8 File Offset: 0x000C37A8
		public BitmapButtonWidget()
		{
			this.Color = Color.White;
			XElement node = ContentManager.Get<XElement>("Widgets/BitmapButtonContents");
			base.LoadChildren(this, node);
			this.m_rectangleWidget = this.Children.Find<RectangleWidget>("Button.Rectangle", true);
			this.m_imageWidget = this.Children.Find<RectangleWidget>("Button.Image", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Button.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Button.Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x000C5640 File Offset: 0x000C3840
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_imageWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			if (this.m_clickableWidget.IsPressed || this.IsChecked)
			{
				this.m_rectangleWidget.Subtexture = this.ClickedSubtexture;
			}
			else
			{
				this.m_rectangleWidget.Subtexture = this.NormalSubtexture;
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x04001180 RID: 4480
		public RectangleWidget m_rectangleWidget;

		// Token: 0x04001181 RID: 4481
		public RectangleWidget m_imageWidget;

		// Token: 0x04001182 RID: 4482
		public LabelWidget m_labelWidget;

		// Token: 0x04001183 RID: 4483
		public ClickableWidget m_clickableWidget;
	}
}
