using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200038C RID: 908
	public class LinkWidget : FixedSizePanelWidget
	{
		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000CED43 File Offset: 0x000CCF43
		// (set) Token: 0x06001A2E RID: 6702 RVA: 0x000CED50 File Offset: 0x000CCF50
		public Vector2 Size
		{
			get
			{
				return this.m_labelWidget.Size;
			}
			set
			{
				this.m_labelWidget.Size = value;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001A2F RID: 6703 RVA: 0x000CED5E File Offset: 0x000CCF5E
		public bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001A30 RID: 6704 RVA: 0x000CED6B File Offset: 0x000CCF6B
		public bool IsPressed
		{
			get
			{
				return this.m_clickableWidget.IsPressed;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001A31 RID: 6705 RVA: 0x000CED78 File Offset: 0x000CCF78
		// (set) Token: 0x06001A32 RID: 6706 RVA: 0x000CED85 File Offset: 0x000CCF85
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

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001A33 RID: 6707 RVA: 0x000CED93 File Offset: 0x000CCF93
		// (set) Token: 0x06001A34 RID: 6708 RVA: 0x000CEDA0 File Offset: 0x000CCFA0
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

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000CEDAE File Offset: 0x000CCFAE
		// (set) Token: 0x06001A36 RID: 6710 RVA: 0x000CEDBB File Offset: 0x000CCFBB
		public TextAnchor TextAnchor
		{
			get
			{
				return this.m_labelWidget.TextAnchor;
			}
			set
			{
				this.m_labelWidget.TextAnchor = value;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x000CEDC9 File Offset: 0x000CCFC9
		// (set) Token: 0x06001A38 RID: 6712 RVA: 0x000CEDD6 File Offset: 0x000CCFD6
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

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001A39 RID: 6713 RVA: 0x000CEDE4 File Offset: 0x000CCFE4
		// (set) Token: 0x06001A3A RID: 6714 RVA: 0x000CEDF1 File Offset: 0x000CCFF1
		public Color Color
		{
			get
			{
				return this.m_labelWidget.Color;
			}
			set
			{
				this.m_labelWidget.Color = value;
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001A3B RID: 6715 RVA: 0x000CEDFF File Offset: 0x000CCFFF
		// (set) Token: 0x06001A3C RID: 6716 RVA: 0x000CEE0C File Offset: 0x000CD00C
		public bool DropShadow
		{
			get
			{
				return this.m_labelWidget.DropShadow;
			}
			set
			{
				this.m_labelWidget.DropShadow = value;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001A3D RID: 6717 RVA: 0x000CEE1A File Offset: 0x000CD01A
		// (set) Token: 0x06001A3E RID: 6718 RVA: 0x000CEE22 File Offset: 0x000CD022
		public string Url { get; set; }

		// Token: 0x06001A3F RID: 6719 RVA: 0x000CEE2C File Offset: 0x000CD02C
		public LinkWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/LinkContents");
			base.LoadChildren(this, node);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x000CEE88 File Offset: 0x000CD088
		public override void Update()
		{
			if (!string.IsNullOrEmpty(this.Url) && this.IsClicked)
			{
				WebBrowserManager.LaunchBrowser(this.Url);
			}
		}

		// Token: 0x04001250 RID: 4688
		public LabelWidget m_labelWidget;

		// Token: 0x04001251 RID: 4689
		public ClickableWidget m_clickableWidget;
	}
}
