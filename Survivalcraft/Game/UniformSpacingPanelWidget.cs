using System;
using Engine;

namespace Game
{
	// Token: 0x020003A1 RID: 929
	public class UniformSpacingPanelWidget : ContainerWidget
	{
		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001B55 RID: 6997 RVA: 0x000D5587 File Offset: 0x000D3787
		// (set) Token: 0x06001B56 RID: 6998 RVA: 0x000D558F File Offset: 0x000D378F
		public LayoutDirection Direction
		{
			get
			{
				return this.m_direction;
			}
			set
			{
				this.m_direction = value;
			}
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x000D5598 File Offset: 0x000D3798
		public override void ArrangeOverride()
		{
			Vector2 zero = Vector2.Zero;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.m_direction == LayoutDirection.Horizontal)
					{
						float num = (this.m_count > 0) ? (base.ActualSize.X / (float)this.m_count) : 0f;
						ContainerWidget.ArrangeChildWidgetInCell(zero, new Vector2(zero.X + num, zero.Y + base.ActualSize.Y), widget);
						zero.X += num;
					}
					else
					{
						float num2 = (this.m_count > 0) ? (base.ActualSize.Y / (float)this.m_count) : 0f;
						ContainerWidget.ArrangeChildWidgetInCell(zero, new Vector2(zero.X + base.ActualSize.X, zero.Y + num2), widget);
						zero.Y += num2;
					}
				}
			}
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x000D56B4 File Offset: 0x000D38B4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_count = 0;
			using (WidgetsList.Enumerator enumerator = this.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsVisible)
					{
						this.m_count++;
					}
				}
			}
			parentAvailableSize = ((this.m_direction != LayoutDirection.Horizontal) ? Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X, parentAvailableSize.Y / (float)this.m_count)) : Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X / (float)this.m_count, parentAvailableSize.Y)));
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					num = ((this.m_direction != LayoutDirection.Horizontal) ? MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X) : MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y));
				}
			}
			if (this.m_direction == LayoutDirection.Horizontal)
			{
				base.DesiredSize = new Vector2(float.PositiveInfinity, num);
				return;
			}
			base.DesiredSize = new Vector2(num, float.PositiveInfinity);
		}

		// Token: 0x040012F6 RID: 4854
		public LayoutDirection m_direction;

		// Token: 0x040012F7 RID: 4855
		public int m_count;
	}
}
