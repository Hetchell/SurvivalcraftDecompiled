using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200038D RID: 909
	public class ListPanelWidget : ScrollPanelWidget
	{
		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x000CEEAA File Offset: 0x000CD0AA
		// (set) Token: 0x06001A42 RID: 6722 RVA: 0x000CEEB2 File Offset: 0x000CD0B2
		public Func<object, Widget> ItemWidgetFactory { get; set; }

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x000CEEBB File Offset: 0x000CD0BB
		// (set) Token: 0x06001A44 RID: 6724 RVA: 0x000CEEC3 File Offset: 0x000CD0C3
		public override LayoutDirection Direction
		{
			get
			{
				return base.Direction;
			}
			set
			{
				if (value != this.Direction)
				{
					base.Direction = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001A45 RID: 6725 RVA: 0x000CEEDC File Offset: 0x000CD0DC
		// (set) Token: 0x06001A46 RID: 6726 RVA: 0x000CEEE4 File Offset: 0x000CD0E4
		public override float ScrollPosition
		{
			get
			{
				return base.ScrollPosition;
			}
			set
			{
				if (value != this.ScrollPosition)
				{
					base.ScrollPosition = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001A47 RID: 6727 RVA: 0x000CEEFD File Offset: 0x000CD0FD
		// (set) Token: 0x06001A48 RID: 6728 RVA: 0x000CEF05 File Offset: 0x000CD105
		public float ItemSize
		{
			get
			{
				return this.m_itemSize;
			}
			set
			{
				if (value != this.m_itemSize)
				{
					this.m_itemSize = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x000CEF1E File Offset: 0x000CD11E
		// (set) Token: 0x06001A4A RID: 6730 RVA: 0x000CEF28 File Offset: 0x000CD128
		public int? SelectedIndex
		{
			get
			{
				return this.m_selectedItemIndex;
			}
			set
			{
				if (value != null && (value.Value < 0 || value.Value >= this.m_items.Count))
				{
					value = null;
				}
				int? num = value;
				int? selectedItemIndex = this.m_selectedItemIndex;
				if (!(num.GetValueOrDefault() == selectedItemIndex.GetValueOrDefault() & num != null == (selectedItemIndex != null)))
				{
					this.m_selectedItemIndex = value;
					if (this.SelectionChanged != null)
					{
						this.SelectionChanged();
					}
				}
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001A4B RID: 6731 RVA: 0x000CEFAA File Offset: 0x000CD1AA
		// (set) Token: 0x06001A4C RID: 6732 RVA: 0x000CEFD4 File Offset: 0x000CD1D4
		public object SelectedItem
		{
			get
			{
				if (this.m_selectedItemIndex == null)
				{
					return null;
				}
				return this.m_items[this.m_selectedItemIndex.Value];
			}
			set
			{
				int num = this.m_items.IndexOf(value);
				this.SelectedIndex = ((num >= 0) ? new int?(num) : null);
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001A4D RID: 6733 RVA: 0x000CF009 File Offset: 0x000CD209
		public ReadOnlyList<object> Items
		{
			get
			{
				return new ReadOnlyList<object>(this.m_items);
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001A4E RID: 6734 RVA: 0x000CF016 File Offset: 0x000CD216
		// (set) Token: 0x06001A4F RID: 6735 RVA: 0x000CF01E File Offset: 0x000CD21E
		public Color SelectionColor { get; set; }

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001A50 RID: 6736 RVA: 0x000CF028 File Offset: 0x000CD228
		// (remove) Token: 0x06001A51 RID: 6737 RVA: 0x000CF060 File Offset: 0x000CD260
		public event Action<object> ItemClicked;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001A52 RID: 6738 RVA: 0x000CF098 File Offset: 0x000CD298
		// (remove) Token: 0x06001A53 RID: 6739 RVA: 0x000CF0D0 File Offset: 0x000CD2D0
		public event Action SelectionChanged;

		// Token: 0x06001A54 RID: 6740 RVA: 0x000CF108 File Offset: 0x000CD308
		public ListPanelWidget()
		{
			this.SelectionColor = Color.Gray;
			this.ItemWidgetFactory = ((object item) => new LabelWidget
			{
				Text = ((item != null) ? item.ToString() : string.Empty),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			});
			this.ItemSize = 48f;
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x000CF17C File Offset: 0x000CD37C
		public void AddItem(object item)
		{
			this.m_items.Add(item);
			this.m_widgetsDirty = true;
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x000CF194 File Offset: 0x000CD394
		public void RemoveItem(object item)
		{
			int num = this.m_items.IndexOf(item);
			if (num >= 0)
			{
				this.RemoveItemAt(num);
			}
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x000CF1BC File Offset: 0x000CD3BC
		public void RemoveItemAt(int index)
		{
			object obj = this.m_items[index];
			this.m_items.RemoveAt(index);
			this.m_widgetsByIndex.Clear();
			this.m_widgetsDirty = true;
			int? selectedIndex = this.SelectedIndex;
			if (index == selectedIndex.GetValueOrDefault() & selectedIndex != null)
			{
				this.SelectedIndex = null;
			}
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x000CF220 File Offset: 0x000CD420
		public void ClearItems()
		{
			this.m_items.Clear();
			this.m_widgetsByIndex.Clear();
			this.m_widgetsDirty = true;
			this.SelectedIndex = null;
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x000CF25C File Offset: 0x000CD45C
		public override float CalculateScrollAreaLength()
		{
			return (float)this.Items.Count * this.ItemSize;
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x000CF280 File Offset: 0x000CD480
		public void ScrollToItem(object item)
		{
			int num = this.m_items.IndexOf(item);
			if (num >= 0)
			{
				float num2 = (float)num * this.ItemSize;
				float num3 = (this.Direction == LayoutDirection.Horizontal) ? base.ActualSize.X : base.ActualSize.Y;
				if (num2 < this.ScrollPosition)
				{
					this.ScrollPosition = num2;
					return;
				}
				if (num2 > this.ScrollPosition + num3 - this.ItemSize)
				{
					this.ScrollPosition = num2 - num3 + this.ItemSize;
				}
			}
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x000CF2FC File Offset: 0x000CD4FC
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.Direction == LayoutDirection.Horizontal)
					{
						widget.Measure(new Vector2(this.ItemSize, MathUtils.Max(parentAvailableSize.Y - 2f * widget.Margin.Y, 0f)));
					}
					else
					{
						widget.Measure(new Vector2(MathUtils.Max(parentAvailableSize.X - 2f * widget.Margin.X, 0f), this.ItemSize));
					}
				}
			}
			if (this.m_widgetsDirty)
			{
				this.m_widgetsDirty = false;
				this.CreateListWidgets((this.Direction == LayoutDirection.Horizontal) ? base.ActualSize.X : base.ActualSize.Y);
			}
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x000CF404 File Offset: 0x000CD604
		public override void ArrangeOverride()
		{
			if (base.ActualSize != this.lastActualSize)
			{
				this.m_widgetsDirty = true;
			}
			this.lastActualSize = base.ActualSize;
			int num = this.m_firstVisibleIndex;
			foreach (Widget widget in this.Children)
			{
				if (this.Direction == LayoutDirection.Horizontal)
				{
					Vector2 vector = new Vector2((float)num * this.ItemSize - this.ScrollPosition, 0f);
					ContainerWidget.ArrangeChildWidgetInCell(vector, vector + new Vector2(this.ItemSize, base.ActualSize.Y), widget);
				}
				else
				{
					Vector2 vector2 = new Vector2(0f, (float)num * this.ItemSize - this.ScrollPosition);
					ContainerWidget.ArrangeChildWidgetInCell(vector2, vector2 + new Vector2(base.ActualSize.X, this.ItemSize), widget);
				}
				num++;
			}
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x000CF50C File Offset: 0x000CD70C
		public override void Update()
		{
			bool flag = this.ScrollSpeed != 0f;
			base.Update();
			if (base.Input.Tap != null && base.HitTestPanel(base.Input.Tap.Value))
			{
				this.m_clickAllowed = !flag;
			}
			if (base.Input.Click != null && this.m_clickAllowed && base.HitTestPanel(base.Input.Click.Value.Start) && base.HitTestPanel(base.Input.Click.Value.End))
			{
				int num = this.PositionToItemIndex(base.Input.Click.Value.End);
				if (this.ItemClicked != null && num >= 0 && num < this.m_items.Count)
				{
					this.ItemClicked(this.Items[num]);
				}
				this.SelectedIndex = new int?(num);
				if (this.SelectedIndex != null)
				{
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				}
			}
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x000CF660 File Offset: 0x000CD860
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.SelectedIndex != null && this.SelectedIndex.Value >= this.m_firstVisibleIndex && this.SelectedIndex.Value <= this.m_lastVisibleIndex)
			{
				Vector2 vector = (this.Direction == LayoutDirection.Horizontal) ? new Vector2((float)this.SelectedIndex.Value * this.ItemSize - this.ScrollPosition, 0f) : new Vector2(0f, (float)this.SelectedIndex.Value * this.ItemSize - this.ScrollPosition);
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
				int count = flatBatch2D.TriangleVertices.Count;
				Vector2 v = (this.Direction == LayoutDirection.Horizontal) ? new Vector2(this.ItemSize, base.ActualSize.Y) : new Vector2(base.ActualSize.X, this.ItemSize);
				flatBatch2D.QueueQuad(vector, vector + v, 0f, this.SelectionColor * base.GlobalColorTransform);
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
			base.Draw(dc);
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x000CF79C File Offset: 0x000CD99C
		public int PositionToItemIndex(Vector2 position)
		{
			Vector2 vector = base.ScreenToWidget(position);
			if (this.Direction == LayoutDirection.Horizontal)
			{
				return (int)((vector.X + this.ScrollPosition) / this.ItemSize);
			}
			return (int)((vector.Y + this.ScrollPosition) / this.ItemSize);
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x000CF7E4 File Offset: 0x000CD9E4
		public void CreateListWidgets(float size)
		{
			this.Children.Clear();
			if (this.m_items.Count <= 0)
			{
				return;
			}
			int x = (int)MathUtils.Floor(this.ScrollPosition / this.ItemSize);
			int x2 = (int)MathUtils.Floor((this.ScrollPosition + size) / this.ItemSize);
			this.m_firstVisibleIndex = MathUtils.Max(x, 0);
			this.m_lastVisibleIndex = MathUtils.Min(x2, this.m_items.Count - 1);
			for (int i = this.m_firstVisibleIndex; i <= this.m_lastVisibleIndex; i++)
			{
				object obj = this.m_items[i];
				Widget widget;
				if (!this.m_widgetsByIndex.TryGetValue(i, out widget))
				{
					widget = this.ItemWidgetFactory(obj);
					widget.Tag = obj;
					this.m_widgetsByIndex.Add(i, widget);
				}
				this.Children.Add(widget);
			}
		}

		// Token: 0x04001253 RID: 4691
		public List<object> m_items = new List<object>();

		// Token: 0x04001254 RID: 4692
		public int? m_selectedItemIndex;

		// Token: 0x04001255 RID: 4693
		public Dictionary<int, Widget> m_widgetsByIndex = new Dictionary<int, Widget>();

		// Token: 0x04001256 RID: 4694
		public int m_firstVisibleIndex;

		// Token: 0x04001257 RID: 4695
		public int m_lastVisibleIndex;

		// Token: 0x04001258 RID: 4696
		public float m_itemSize;

		// Token: 0x04001259 RID: 4697
		public bool m_widgetsDirty;

		// Token: 0x0400125A RID: 4698
		public bool m_clickAllowed;

		// Token: 0x0400125B RID: 4699
		public Vector2 lastActualSize = new Vector2(-1f);
	}
}
