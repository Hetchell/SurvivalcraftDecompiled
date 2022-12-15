using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000370 RID: 880
	public class CanvasWidget : ContainerWidget
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06001920 RID: 6432 RVA: 0x000C5DF9 File Offset: 0x000C3FF9
		// (set) Token: 0x06001921 RID: 6433 RVA: 0x000C5E01 File Offset: 0x000C4001
		public Vector2 Size { get; set; } = new Vector2(-1f);

		// Token: 0x06001922 RID: 6434 RVA: 0x000C5E0A File Offset: 0x000C400A
		public static void SetPosition(Widget widget, Vector2 position)
		{
			CanvasWidget canvasWidget = widget.ParentWidget as CanvasWidget;
			if (canvasWidget == null)
			{
				return;
			}
			canvasWidget.SetWidgetPosition(widget, new Vector2?(position));
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x000C5E28 File Offset: 0x000C4028
		public Vector2? GetWidgetPosition(Widget widget)
		{
			Vector2 value;
			if (this.m_positions.TryGetValue(widget, out value))
			{
				return new Vector2?(value);
			}
			return null;
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x000C5E55 File Offset: 0x000C4055
		public void SetWidgetPosition(Widget widget, Vector2? position)
		{
			if (position != null)
			{
				this.m_positions[widget] = position.Value;
				return;
			}
			this.m_positions.Remove(widget);
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x000C5E81 File Offset: 0x000C4081
		public override void WidgetRemoved(Widget widget)
		{
			this.m_positions.Remove(widget);
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x000C5E90 File Offset: 0x000C4090
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			Vector2 vector = Vector2.Zero;
			if (this.Size.X >= 0f)
			{
				parentAvailableSize.X = MathUtils.Min(parentAvailableSize.X, this.Size.X);
			}
			if (this.Size.Y >= 0f)
			{
				parentAvailableSize.Y = MathUtils.Min(parentAvailableSize.Y, this.Size.Y);
			}
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					Vector2? widgetPosition = this.GetWidgetPosition(widget);
					Vector2 vector2 = (widgetPosition != null) ? widgetPosition.Value : Vector2.Zero;
					widget.Measure(Vector2.Max(parentAvailableSize - vector2 - 2f * widget.Margin, Vector2.Zero));
					vector = new Vector2
					{
						X = MathUtils.Max(vector.X, vector2.X + widget.ParentDesiredSize.X + 2f * widget.Margin.X),
						Y = MathUtils.Max(vector.Y, vector2.Y + widget.ParentDesiredSize.Y + 2f * widget.Margin.Y)
					};
				}
			}
			if (this.Size.X >= 0f)
			{
				vector.X = this.Size.X;
			}
			if (this.Size.Y >= 0f)
			{
				vector.Y = this.Size.Y;
			}
			base.DesiredSize = vector;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x000C6068 File Offset: 0x000C4268
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					Vector2? widgetPosition = this.GetWidgetPosition(widget);
					if (widgetPosition != null)
					{
						Vector2 zero = Vector2.Zero;
						if (!float.IsPositiveInfinity(widget.ParentDesiredSize.X))
						{
							zero.X = widget.ParentDesiredSize.X;
						}
						else
						{
							zero.X = MathUtils.Max(base.ActualSize.X - widgetPosition.Value.X, 0f);
						}
						if (!float.IsPositiveInfinity(widget.ParentDesiredSize.Y))
						{
							zero.Y = widget.ParentDesiredSize.Y;
						}
						else
						{
							zero.Y = MathUtils.Max(base.ActualSize.Y - widgetPosition.Value.Y, 0f);
						}
						widget.Arrange(widgetPosition.Value, zero);
					}
					else
					{
						ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
					}
				}
			}
		}

		// Token: 0x0400119C RID: 4508
		public Dictionary<Widget, Vector2> m_positions = new Dictionary<Widget, Vector2>();
	}
}
