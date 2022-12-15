using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000371 RID: 881
	public class CheckboxWidget : CanvasWidget
	{
		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x000C61C3 File Offset: 0x000C43C3
		public bool IsPressed
		{
			get
			{
				return this.m_clickableWidget.IsPressed;
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x0600192A RID: 6442 RVA: 0x000C61D0 File Offset: 0x000C43D0
		public bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x0600192B RID: 6443 RVA: 0x000C61DD File Offset: 0x000C43DD
		public bool IsTapped
		{
			get
			{
				return this.m_clickableWidget.IsTapped;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x0600192C RID: 6444 RVA: 0x000C61EA File Offset: 0x000C43EA
		// (set) Token: 0x0600192D RID: 6445 RVA: 0x000C61F2 File Offset: 0x000C43F2
		public bool IsChecked { get; set; }

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x0600192E RID: 6446 RVA: 0x000C61FB File Offset: 0x000C43FB
		// (set) Token: 0x0600192F RID: 6447 RVA: 0x000C6203 File Offset: 0x000C4403
		public bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001930 RID: 6448 RVA: 0x000C620C File Offset: 0x000C440C
		// (set) Token: 0x06001931 RID: 6449 RVA: 0x000C6219 File Offset: 0x000C4419
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

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06001932 RID: 6450 RVA: 0x000C6227 File Offset: 0x000C4427
		// (set) Token: 0x06001933 RID: 6451 RVA: 0x000C6234 File Offset: 0x000C4434
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

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001934 RID: 6452 RVA: 0x000C6242 File Offset: 0x000C4442
		// (set) Token: 0x06001935 RID: 6453 RVA: 0x000C624F File Offset: 0x000C444F
		public Subtexture TickSubtexture
		{
			get
			{
				return this.m_tickWidget.Subtexture;
			}
			set
			{
				this.m_tickWidget.Subtexture = value;
			}
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001936 RID: 6454 RVA: 0x000C625D File Offset: 0x000C445D
		// (set) Token: 0x06001937 RID: 6455 RVA: 0x000C6265 File Offset: 0x000C4465
		public Color Color { get; set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001938 RID: 6456 RVA: 0x000C626E File Offset: 0x000C446E
		// (set) Token: 0x06001939 RID: 6457 RVA: 0x000C627B File Offset: 0x000C447B
		public Vector2 CheckboxSize
		{
			get
			{
				return this.m_canvasWidget.Size;
			}
			set
			{
				this.m_canvasWidget.Size = value;
			}
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x000C628C File Offset: 0x000C448C
		public CheckboxWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CheckboxContents");
			base.LoadChildren(this, node);
			this.m_canvasWidget = this.Children.Find<CanvasWidget>("Checkbox.Canvas", true);
			this.m_rectangleWidget = this.Children.Find<RectangleWidget>("Checkbox.Rectangle", true);
			this.m_tickWidget = this.Children.Find<RectangleWidget>("Checkbox.Tick", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Checkbox.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Checkbox.Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x000C632D File Offset: 0x000C452D
		public override void Update()
		{
			if (this.IsClicked && this.IsAutoCheckingEnabled)
			{
				this.IsChecked = !this.IsChecked;
			}
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x000C6350 File Offset: 0x000C4550
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_rectangleWidget.FillColor = new Color(0, 0, 0, 128);
			this.m_rectangleWidget.OutlineColor = (isEnabledGlobal ? new Color(128, 128, 128) : new Color(112, 112, 112));
			this.m_tickWidget.IsVisible = this.IsChecked;
			this.m_tickWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_tickWidget.OutlineColor = Color.Transparent;
			this.m_tickWidget.Subtexture = this.TickSubtexture;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x0400119E RID: 4510
		public CanvasWidget m_canvasWidget;

		// Token: 0x0400119F RID: 4511
		public RectangleWidget m_rectangleWidget;

		// Token: 0x040011A0 RID: 4512
		public RectangleWidget m_tickWidget;

		// Token: 0x040011A1 RID: 4513
		public LabelWidget m_labelWidget;

		// Token: 0x040011A2 RID: 4514
		public ClickableWidget m_clickableWidget;
	}
}
