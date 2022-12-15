using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000388 RID: 904
	public class GridPanelWidget : ContainerWidget
	{
		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x060019DE RID: 6622 RVA: 0x000CC991 File Offset: 0x000CAB91
		// (set) Token: 0x060019DF RID: 6623 RVA: 0x000CC9A0 File Offset: 0x000CABA0
		public int ColumnsCount
		{
			get
			{
				return this.m_columns.Count;
			}
			set
			{
				this.m_columns = new List<GridPanelWidget.Column>(this.m_columns.GetRange(0, MathUtils.Min(this.m_columns.Count, value)));
				while (this.m_columns.Count < value)
				{
					this.m_columns.Add(new GridPanelWidget.Column());
				}
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x060019E0 RID: 6624 RVA: 0x000CC9F5 File Offset: 0x000CABF5
		// (set) Token: 0x060019E1 RID: 6625 RVA: 0x000CCA04 File Offset: 0x000CAC04
		public int RowsCount
		{
			get
			{
				return this.m_rows.Count;
			}
			set
			{
				this.m_rows = new List<GridPanelWidget.Row>(this.m_rows.GetRange(0, MathUtils.Min(this.m_rows.Count, value)));
				while (this.m_rows.Count < value)
				{
					this.m_rows.Add(new GridPanelWidget.Row());
				}
			}
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x000CCA59 File Offset: 0x000CAC59
		public GridPanelWidget()
		{
			this.ColumnsCount = 1;
			this.RowsCount = 1;
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x000CCA90 File Offset: 0x000CAC90
		public Point2 GetWidgetCell(Widget widget)
		{
			Point2 result;
			this.m_cells.TryGetValue(widget, out result);
			return result;
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x000CCAAD File Offset: 0x000CACAD
		public void SetWidgetCell(Widget widget, Point2 cell)
		{
			this.m_cells[widget] = cell;
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x000CCABC File Offset: 0x000CACBC
		public static void SetCell(Widget widget, Point2 cell)
		{
			GridPanelWidget gridPanelWidget = widget.ParentWidget as GridPanelWidget;
			if (gridPanelWidget == null)
			{
				return;
			}
			gridPanelWidget.SetWidgetCell(widget, cell);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x000CCAD5 File Offset: 0x000CACD5
		public override void WidgetRemoved(Widget widget)
		{
			this.m_cells.Remove(widget);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x000CCAE4 File Offset: 0x000CACE4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			foreach (GridPanelWidget.Column column in this.m_columns)
			{
				column.ActualWidth = 0f;
			}
			foreach (GridPanelWidget.Row row in this.m_rows)
			{
				row.ActualHeight = 0f;
			}
			foreach (Widget widget in this.Children)
			{
				widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
				Point2 widgetCell = this.GetWidgetCell(widget);
				if (this.IsCellValid(widgetCell))
				{
					GridPanelWidget.Column column2 = this.m_columns[widgetCell.X];
					column2.ActualWidth = MathUtils.Max(column2.ActualWidth, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					GridPanelWidget.Row row2 = this.m_rows[widgetCell.Y];
					row2.ActualHeight = MathUtils.Max(row2.ActualHeight, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
				}
			}
			Vector2 zero = Vector2.Zero;
			foreach (GridPanelWidget.Column column3 in this.m_columns)
			{
				column3.Position = zero.X;
				zero.X += column3.ActualWidth;
			}
			foreach (GridPanelWidget.Row row3 in this.m_rows)
			{
				row3.Position = zero.Y;
				zero.Y += row3.ActualHeight;
			}
			base.DesiredSize = zero;
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x000CCD44 File Offset: 0x000CAF44
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				Point2 widgetCell = this.GetWidgetCell(widget);
				if (this.IsCellValid(widgetCell))
				{
					GridPanelWidget.Column column = this.m_columns[widgetCell.X];
					GridPanelWidget.Row row = this.m_rows[widgetCell.Y];
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(column.Position, row.Position), new Vector2(column.Position + column.ActualWidth, row.Position + row.ActualHeight), widget);
				}
				else
				{
					ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
				}
			}
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x000CCE18 File Offset: 0x000CB018
		public bool IsCellValid(Point2 cell)
		{
			return cell.X >= 0 && cell.X < this.m_columns.Count && cell.Y >= 0 && cell.Y < this.m_rows.Count;
		}

		// Token: 0x04001222 RID: 4642
		public List<GridPanelWidget.Column> m_columns = new List<GridPanelWidget.Column>();

		// Token: 0x04001223 RID: 4643
		public List<GridPanelWidget.Row> m_rows = new List<GridPanelWidget.Row>();

		// Token: 0x04001224 RID: 4644
		public Dictionary<Widget, Point2> m_cells = new Dictionary<Widget, Point2>();

		// Token: 0x02000520 RID: 1312
		public class Column
		{
			// Token: 0x040018EE RID: 6382
			public float Position;

			// Token: 0x040018EF RID: 6383
			public float ActualWidth;
		}

		// Token: 0x02000521 RID: 1313
		public class Row
		{
			// Token: 0x040018F0 RID: 6384
			public float Position;

			// Token: 0x040018F1 RID: 6385
			public float ActualHeight;
		}
	}
}
