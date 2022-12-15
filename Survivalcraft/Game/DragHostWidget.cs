using System;
using Engine;

namespace Game
{
	// Token: 0x0200037E RID: 894
	public class DragHostWidget : ContainerWidget
	{
		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06001988 RID: 6536 RVA: 0x000C845F File Offset: 0x000C665F
		public bool IsDragInProgress
		{
			get
			{
				return this.m_dragWidget != null;
			}
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x000C846A File Offset: 0x000C666A
		public DragHostWidget()
		{
			this.IsHitTestVisible = false;
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x000C8479 File Offset: 0x000C6679
		public void BeginDrag(Widget dragWidget, object dragData, Action dragEndedHandler)
		{
			if (this.m_dragWidget == null)
			{
				this.m_dragWidget = dragWidget;
				this.m_dragData = dragData;
				this.m_dragEndedHandler = dragEndedHandler;
				this.Children.Add(this.m_dragWidget);
				this.UpdateDragPosition();
			}
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x000C84B0 File Offset: 0x000C66B0
		public void EndDrag()
		{
			if (this.m_dragWidget != null)
			{
				this.Children.Remove(this.m_dragWidget);
				this.m_dragWidget = null;
				this.m_dragData = null;
				if (this.m_dragEndedHandler != null)
				{
					this.m_dragEndedHandler();
					this.m_dragEndedHandler = null;
				}
			}
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x000C8500 File Offset: 0x000C6700
		public override void Update()
		{
			if (this.m_dragWidget != null)
			{
				this.UpdateDragPosition();
				IDragTargetWidget dragTargetWidget = base.HitTestGlobal(this.m_dragPosition, (Widget w) => w is IDragTargetWidget) as IDragTargetWidget;
				if (base.Input.Drag != null)
				{
					if (dragTargetWidget != null)
					{
						dragTargetWidget.DragOver(this.m_dragWidget, this.m_dragData);
						return;
					}
				}
				else
				{
					try
					{
						if (dragTargetWidget != null)
						{
							dragTargetWidget.DragDrop(this.m_dragWidget, this.m_dragData);
						}
					}
					finally
					{
						this.EndDrag();
					}
				}
			}
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x000C85AC File Offset: 0x000C67AC
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				Vector2 parentDesiredSize = widget.ParentDesiredSize;
				parentDesiredSize.X = MathUtils.Min(parentDesiredSize.X, base.ActualSize.X);
				parentDesiredSize.Y = MathUtils.Min(parentDesiredSize.Y, base.ActualSize.Y);
				widget.Arrange(base.ScreenToWidget(this.m_dragPosition) - 0.5f * parentDesiredSize, parentDesiredSize);
			}
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x000C865C File Offset: 0x000C685C
		public void UpdateDragPosition()
		{
			if (base.Input.Drag != null)
			{
				this.m_dragPosition = base.Input.Drag.Value;
				this.m_dragPosition.X = MathUtils.Clamp(this.m_dragPosition.X, base.GlobalBounds.Min.X, base.GlobalBounds.Max.X - 1f);
				this.m_dragPosition.Y = MathUtils.Clamp(this.m_dragPosition.Y, base.GlobalBounds.Min.Y, base.GlobalBounds.Max.Y - 1f);
			}
		}

		// Token: 0x040011EE RID: 4590
		public Widget m_dragWidget;

		// Token: 0x040011EF RID: 4591
		public object m_dragData;

		// Token: 0x040011F0 RID: 4592
		public Action m_dragEndedHandler;

		// Token: 0x040011F1 RID: 4593
		public Vector2 m_dragPosition;
	}
}
