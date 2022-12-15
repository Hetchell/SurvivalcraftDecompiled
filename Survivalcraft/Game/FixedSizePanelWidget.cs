using System;
using Engine;

namespace Game
{
	// Token: 0x02000381 RID: 897
	public class FixedSizePanelWidget : ContainerWidget
	{
		// Token: 0x060019AA RID: 6570 RVA: 0x000C957C File Offset: 0x000C777C
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			Vector2 zero = Vector2.Zero;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					if (widget.ParentDesiredSize.X != float.PositiveInfinity)
					{
						zero.X = MathUtils.Max(zero.X, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					}
					if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
					{
						zero.Y = MathUtils.Max(zero.Y, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
					}
				}
			}
			base.DesiredSize = zero;
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x000C9690 File Offset: 0x000C7890
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
			}
		}
	}
}
