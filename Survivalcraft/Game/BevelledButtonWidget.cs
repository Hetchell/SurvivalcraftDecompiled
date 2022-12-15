using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000369 RID: 873
	public class BevelledButtonWidget : ButtonWidget
	{
		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x060018AF RID: 6319 RVA: 0x000C48F9 File Offset: 0x000C2AF9
		// (set) Token: 0x060018B0 RID: 6320 RVA: 0x000C4906 File Offset: 0x000C2B06
		public float FontScale
		{
			get
			{
				return this.m_labelWidget.FontScale;
			}
			set
			{
				this.m_labelWidget.FontScale = value;
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x060018B1 RID: 6321 RVA: 0x000C4914 File Offset: 0x000C2B14
		public override bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x060018B2 RID: 6322 RVA: 0x000C4921 File Offset: 0x000C2B21
		// (set) Token: 0x060018B3 RID: 6323 RVA: 0x000C492E File Offset: 0x000C2B2E
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

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060018B4 RID: 6324 RVA: 0x000C493C File Offset: 0x000C2B3C
		// (set) Token: 0x060018B5 RID: 6325 RVA: 0x000C4949 File Offset: 0x000C2B49
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

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x000C4957 File Offset: 0x000C2B57
		// (set) Token: 0x060018B7 RID: 6327 RVA: 0x000C4964 File Offset: 0x000C2B64
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

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060018B8 RID: 6328 RVA: 0x000C4972 File Offset: 0x000C2B72
		// (set) Token: 0x060018B9 RID: 6329 RVA: 0x000C497F File Offset: 0x000C2B7F
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

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060018BA RID: 6330 RVA: 0x000C498D File Offset: 0x000C2B8D
		// (set) Token: 0x060018BB RID: 6331 RVA: 0x000C499A File Offset: 0x000C2B9A
		public Subtexture Subtexture
		{
			get
			{
				return this.m_imageWidget.Subtexture;
			}
			set
			{
				this.m_imageWidget.Subtexture = value;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060018BC RID: 6332 RVA: 0x000C49A8 File Offset: 0x000C2BA8
		// (set) Token: 0x060018BD RID: 6333 RVA: 0x000C49B0 File Offset: 0x000C2BB0
		public override Color Color { get; set; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060018BE RID: 6334 RVA: 0x000C49B9 File Offset: 0x000C2BB9
		// (set) Token: 0x060018BF RID: 6335 RVA: 0x000C49C6 File Offset: 0x000C2BC6
		public Color BevelColor
		{
			get
			{
				return this.m_rectangleWidget.BevelColor;
			}
			set
			{
				this.m_rectangleWidget.BevelColor = value;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060018C0 RID: 6336 RVA: 0x000C49D4 File Offset: 0x000C2BD4
		// (set) Token: 0x060018C1 RID: 6337 RVA: 0x000C49E1 File Offset: 0x000C2BE1
		public Color CenterColor
		{
			get
			{
				return this.m_rectangleWidget.CenterColor;
			}
			set
			{
				this.m_rectangleWidget.CenterColor = value;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x000C49EF File Offset: 0x000C2BEF
		// (set) Token: 0x060018C3 RID: 6339 RVA: 0x000C49FC File Offset: 0x000C2BFC
		public float AmbientLight
		{
			get
			{
				return this.m_rectangleWidget.AmbientLight;
			}
			set
			{
				this.m_rectangleWidget.AmbientLight = value;
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060018C4 RID: 6340 RVA: 0x000C4A0A File Offset: 0x000C2C0A
		// (set) Token: 0x060018C5 RID: 6341 RVA: 0x000C4A17 File Offset: 0x000C2C17
		public float DirectionalLight
		{
			get
			{
				return this.m_rectangleWidget.DirectionalLight;
			}
			set
			{
				this.m_rectangleWidget.DirectionalLight = value;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060018C6 RID: 6342 RVA: 0x000C4A25 File Offset: 0x000C2C25
		// (set) Token: 0x060018C7 RID: 6343 RVA: 0x000C4A2D File Offset: 0x000C2C2D
		public float BevelSize { get; set; }

		// Token: 0x060018C8 RID: 6344 RVA: 0x000C4A38 File Offset: 0x000C2C38
		public BevelledButtonWidget()
		{
			this.Color = Color.White;
			this.BevelSize = 2f;
			XElement node = ContentManager.Get<XElement>("Widgets/BevelledButtonContents");
			base.LoadChildren(this, node);
			this.m_rectangleWidget = this.Children.Find<BevelledRectangleWidget>("BevelledButton.Rectangle", true);
			this.m_imageWidget = this.Children.Find<RectangleWidget>("BevelledButton.Image", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("BevelledButton.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("BevelledButton.Clickable", true);
			this.m_labelWidget.VerticalAlignment = WidgetAlignment.Center;
			base.LoadProperties(this, node);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x000C4AE4 File Offset: 0x000C2CE4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_imageWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			if (this.m_clickableWidget.IsPressed || this.IsChecked)
			{
				this.m_rectangleWidget.BevelSize = -0.5f * this.BevelSize;
			}
			else
			{
				this.m_rectangleWidget.BevelSize = this.BevelSize;
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x04001170 RID: 4464
		public BevelledRectangleWidget m_rectangleWidget;

		// Token: 0x04001171 RID: 4465
		public RectangleWidget m_imageWidget;

		// Token: 0x04001172 RID: 4466
		public LabelWidget m_labelWidget;

		// Token: 0x04001173 RID: 4467
		public ClickableWidget m_clickableWidget;
	}
}
