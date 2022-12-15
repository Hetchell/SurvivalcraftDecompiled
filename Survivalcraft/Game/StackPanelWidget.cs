using System;
using Engine;

namespace Game
{
	// Token: 0x0200039C RID: 924
	public class StackPanelWidget : ContainerWidget
	{
		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001B05 RID: 6917 RVA: 0x000D3CC8 File Offset: 0x000D1EC8
		// (set) Token: 0x06001B06 RID: 6918 RVA: 0x000D3CD0 File Offset: 0x000D1ED0
		public LayoutDirection Direction { get; set; }

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001B07 RID: 6919 RVA: 0x000D3CD9 File Offset: 0x000D1ED9
		// (set) Token: 0x06001B08 RID: 6920 RVA: 0x000D3CE1 File Offset: 0x000D1EE1
		public bool IsInverted { get; set; }

		// Token: 0x06001B09 RID: 6921 RVA: 0x000D3CEC File Offset: 0x000D1EEC
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_fixedSize = 0f;
			this.m_fillCount = 0;
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					if (this.Direction == LayoutDirection.Horizontal)
					{
						if (widget.ParentDesiredSize.X != float.PositiveInfinity)
						{
							this.m_fixedSize += widget.ParentDesiredSize.X + 2f * widget.Margin.X;
							parentAvailableSize.X = MathUtils.Max(parentAvailableSize.X - (widget.ParentDesiredSize.X + 2f * widget.Margin.X), 0f);
						}
						else
						{
							this.m_fillCount++;
						}
						num = MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
					}
					else
					{
						if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
						{
							this.m_fixedSize += widget.ParentDesiredSize.Y + 2f * widget.Margin.Y;
							parentAvailableSize.Y = MathUtils.Max(parentAvailableSize.Y - (widget.ParentDesiredSize.Y + 2f * widget.Margin.Y), 0f);
						}
						else
						{
							this.m_fillCount++;
						}
						num = MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					}
				}
			}
			if (this.Direction == LayoutDirection.Horizontal)
			{
				if (this.m_fillCount == 0)
				{
					base.DesiredSize = new Vector2(this.m_fixedSize, num);
					return;
				}
				base.DesiredSize = new Vector2(float.PositiveInfinity, num);
				return;
			}
			else
			{
				if (this.m_fillCount == 0)
				{
					base.DesiredSize = new Vector2(num, this.m_fixedSize);
					return;
				}
				base.DesiredSize = new Vector2(num, float.PositiveInfinity);
				return;
			}
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x000D3F50 File Offset: 0x000D2150
		public override void ArrangeOverride()
		{
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.Direction == LayoutDirection.Horizontal)
					{
						float num2 = (widget.ParentDesiredSize.X == float.PositiveInfinity) ? ((this.m_fillCount > 0) ? (MathUtils.Max(base.ActualSize.X - this.m_fixedSize, 0f) / (float)this.m_fillCount) : 0f) : (widget.ParentDesiredSize.X + 2f * widget.Margin.X);
						Vector2 c;
						Vector2 c2;
						if (!this.IsInverted)
						{
							c = new Vector2(num, 0f);
							c2 = new Vector2(num + num2, base.ActualSize.Y);
						}
						else
						{
							c = new Vector2(base.ActualSize.X - (num + num2), 0f);
							c2 = new Vector2(base.ActualSize.X - num, base.ActualSize.Y);
						}
						ContainerWidget.ArrangeChildWidgetInCell(c, c2, widget);
						num += num2;
					}
					else
					{
						float num3 = (widget.ParentDesiredSize.Y == float.PositiveInfinity) ? ((this.m_fillCount > 0) ? (MathUtils.Max(base.ActualSize.Y - this.m_fixedSize, 0f) / (float)this.m_fillCount) : 0f) : (widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
						Vector2 c3;
						Vector2 c4;
						if (!this.IsInverted)
						{
							c3 = new Vector2(0f, num);
							c4 = new Vector2(base.ActualSize.X, num + num3);
						}
						else
						{
							c3 = new Vector2(0f, base.ActualSize.Y - (num + num3));
							c4 = new Vector2(base.ActualSize.X, base.ActualSize.Y - num);
						}
						ContainerWidget.ArrangeChildWidgetInCell(c3, c4, widget);
						num += num3;
					}
				}
			}
		}

		// Token: 0x040012CC RID: 4812
		public float m_fixedSize;

		// Token: 0x040012CD RID: 4813
		public int m_fillCount;
	}
}
