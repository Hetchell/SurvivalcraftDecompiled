using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000377 RID: 887
	public abstract class ContainerWidget : Widget
	{
		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x000C6CB0 File Offset: 0x000C4EB0
		public IEnumerable<Widget> AllChildren
		{
			get
			{
				foreach (Widget childWidget in this.Children)
				{
					yield return childWidget;
					ContainerWidget containerWidget = childWidget as ContainerWidget;
					if (containerWidget != null)
					{
						foreach (Widget widget in containerWidget.AllChildren)
						{
							yield return widget;
						}

					}

				}

			}
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x000C6CC0 File Offset: 0x000C4EC0
		public ContainerWidget()
		{
			this.Children = new WidgetsList(this);
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x000C6CD4 File Offset: 0x000C4ED4
		public override void UpdateCeases()
		{
			foreach (Widget widget in this.Children)
			{
				widget.UpdateCeases();
			}
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x000C6D24 File Offset: 0x000C4F24
		public virtual void WidgetAdded(Widget widget)
		{
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x000C6D26 File Offset: 0x000C4F26
		public virtual void WidgetRemoved(Widget widget)
		{
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000C6D28 File Offset: 0x000C4F28
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			foreach (Widget widget in this.Children)
			{
				widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
			}
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000C6D9C File Offset: 0x000C4F9C
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
			}
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x000C6DFC File Offset: 0x000C4FFC
		public static void ArrangeChildWidgetInCell(Vector2 c1, Vector2 c2, Widget widget)
		{
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			Vector2 vector = c2 - c1;
			Vector2 margin = widget.Margin;
			Vector2 parentDesiredSize = widget.ParentDesiredSize;
			if (float.IsPositiveInfinity(parentDesiredSize.X) || parentDesiredSize.X > vector.X - 2f * margin.X)
			{
				parentDesiredSize.X = MathUtils.Max(vector.X - 2f * margin.X, 0f);
			}
			if (float.IsPositiveInfinity(parentDesiredSize.Y) || parentDesiredSize.Y > vector.Y - 2f * margin.Y)
			{
				parentDesiredSize.Y = MathUtils.Max(vector.Y - 2f * margin.Y, 0f);
			}
			if (widget.HorizontalAlignment == WidgetAlignment.Near)
			{
				zero.X = c1.X + margin.X;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Center)
			{
				zero.X = c1.X + (vector.X - parentDesiredSize.X) / 2f;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Far)
			{
				zero.X = c2.X - parentDesiredSize.X - margin.X;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Stretch)
			{
				zero.X = c1.X + margin.X;
				zero2.X = MathUtils.Max(vector.X - 2f * margin.X, 0f);
			}
			if (widget.VerticalAlignment == WidgetAlignment.Near)
			{
				zero.Y = c1.Y + margin.Y;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Center)
			{
				zero.Y = c1.Y + (vector.Y - parentDesiredSize.Y) / 2f;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Far)
			{
				zero.Y = c2.Y - parentDesiredSize.Y - margin.Y;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Stretch)
			{
				zero.Y = c1.Y + margin.Y;
				zero2.Y = MathUtils.Max(vector.Y - 2f * margin.Y, 0f);
			}
			widget.Arrange(zero, zero2);
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x000C7090 File Offset: 0x000C5290
		public override void Dispose()
		{
			foreach (Widget widget in this.Children)
			{
				widget.Dispose();
			}
		}

		// Token: 0x040011C1 RID: 4545
		public readonly WidgetsList Children;
	}
}
