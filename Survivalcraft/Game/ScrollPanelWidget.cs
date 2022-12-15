using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000398 RID: 920
	public class ScrollPanelWidget : ContainerWidget
	{
		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x000D29C2 File Offset: 0x000D0BC2
		// (set) Token: 0x06001AD4 RID: 6868 RVA: 0x000D29CA File Offset: 0x000D0BCA
		public virtual LayoutDirection Direction { get; set; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06001AD5 RID: 6869 RVA: 0x000D29D3 File Offset: 0x000D0BD3
		// (set) Token: 0x06001AD6 RID: 6870 RVA: 0x000D29DB File Offset: 0x000D0BDB
		public virtual float ScrollPosition { get; set; }

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x000D29E4 File Offset: 0x000D0BE4
		// (set) Token: 0x06001AD8 RID: 6872 RVA: 0x000D29EC File Offset: 0x000D0BEC
		public virtual float ScrollSpeed { get; set; }

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000D29F5 File Offset: 0x000D0BF5
		public ScrollPanelWidget()
		{
			base.ClampToBounds = true;
			this.StartInitialScroll();
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000D2A0A File Offset: 0x000D0C0A
		public void StartInitialScroll()
		{
			this.ScrollPosition = 12f;
			this.ScrollSpeed = -70f;
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x000D2A24 File Offset: 0x000D0C24
		public virtual float CalculateScrollAreaLength()
		{
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					num = ((this.Direction != LayoutDirection.Horizontal) ? MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y) : MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X));
				}
			}
			return num;
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x000D2AD0 File Offset: 0x000D0CD0
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.Direction == LayoutDirection.Horizontal)
					{
						widget.Measure(new Vector2(float.MaxValue, MathUtils.Max(parentAvailableSize.Y - 2f * widget.Margin.Y, 0f)));
					}
					else
					{
						widget.Measure(new Vector2(MathUtils.Max(parentAvailableSize.X - 2f * widget.Margin.X, 0f), float.MaxValue));
					}
				}
			}
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x000D2B9C File Offset: 0x000D0D9C
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				Vector2 zero = Vector2.Zero;
				Vector2 actualSize = base.ActualSize;
				if (this.Direction == LayoutDirection.Horizontal)
				{
					zero.X -= this.ScrollPosition;
					actualSize.X = zero.X + widget.ParentDesiredSize.X;
				}
				else
				{
					zero.Y -= this.ScrollPosition;
					actualSize.Y = zero.Y + widget.ParentDesiredSize.Y;
				}
				ContainerWidget.ArrangeChildWidgetInCell(zero, actualSize, widget);
			}
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x000D2C60 File Offset: 0x000D0E60
		public override void Update()
		{
			float num = 50f;
			this.m_scrollAreaLength = this.CalculateScrollAreaLength();
			this.m_scrollBarAlpha = MathUtils.Max(this.m_scrollBarAlpha - 2f * Time.FrameDuration, 0f);
			if (base.Input.Tap != null && this.HitTestPanel(base.Input.Tap.Value))
			{
				this.m_lastDragPosition = new Vector2?(base.ScreenToWidget(base.Input.Tap.Value));
			}
			if (this.m_lastDragPosition != null)
			{
				if (base.Input.Press != null)
				{
					Vector2 vector = base.ScreenToWidget(base.Input.Press.Value);
					Vector2 vector2 = vector - this.m_lastDragPosition.Value;
					float num2;
					if (this.Direction == LayoutDirection.Horizontal)
					{
						this.ScrollPosition += 0f - vector2.X;
						num2 = vector2.X / Time.FrameDuration;
					}
					else
					{
						this.ScrollPosition += 0f - vector2.Y;
						num2 = vector2.Y / Time.FrameDuration;
					}
					float num3 = (MathUtils.Abs(num2) < MathUtils.Abs(this.m_dragSpeed)) ? 20f : 16f;
					this.m_dragSpeed += MathUtils.Saturate(num3 * Time.FrameDuration) * (num2 - this.m_dragSpeed);
					this.m_scrollBarAlpha = 4f;
					this.m_lastDragPosition = new Vector2?(vector);
					this.ScrollSpeed = 0f;
				}
				else
				{
					this.ScrollSpeed = 0f - this.m_dragSpeed;
					this.m_dragSpeed = 0f;
					this.m_lastDragPosition = null;
				}
			}
			if (this.ScrollSpeed != 0f)
			{
				this.ScrollSpeed *= MathUtils.Pow(0.33f, Time.FrameDuration);
				if (MathUtils.Abs(this.ScrollSpeed) < 40f)
				{
					this.ScrollSpeed = 0f;
				}
				this.ScrollPosition += this.ScrollSpeed * Time.FrameDuration;
				this.m_scrollBarAlpha = 3f;
			}
			if (base.Input.Scroll != null && this.HitTestPanel(base.Input.Scroll.Value.XY))
			{
				this.ScrollPosition -= 40f * base.Input.Scroll.Value.Z;
				this.ScrollSpeed = 0f;
				num = 0f;
				this.m_scrollBarAlpha = 3f;
			}
			float num4 = MathUtils.Max(this.m_scrollAreaLength - base.ActualSize.Y, 0f);
			if (this.ScrollPosition < 0f)
			{
				if (this.m_lastDragPosition == null)
				{
					this.ScrollPosition = MathUtils.Min(this.ScrollPosition + 6f * Time.FrameDuration * (0f - this.ScrollPosition + 5f), 0f);
				}
				this.ScrollPosition = MathUtils.Max(this.ScrollPosition, 0f - num);
				this.ScrollSpeed = 0f;
			}
			if (this.ScrollPosition > num4)
			{
				if (this.m_lastDragPosition == null)
				{
					this.ScrollPosition = MathUtils.Max(this.ScrollPosition + 6f * Time.FrameDuration * (num4 - this.ScrollPosition - 5f), num4);
				}
				this.ScrollPosition = MathUtils.Min(this.ScrollPosition, num4 + num);
				this.ScrollSpeed = 0f;
			}
			if (this.m_lastDragPosition != null && (base.Input.Drag != null || base.Input.Hold != null))
			{
				base.Input.Clear();
			}
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x000D3060 File Offset: 0x000D1260
		public override void Draw(Widget.DrawContext dc)
		{
			Color color = new Color(128, 128, 128) * base.GlobalColorTransform * MathUtils.Saturate(this.m_scrollBarAlpha);
			if (color.A > 0 && this.m_scrollAreaLength > 0f)
			{
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
				int count = flatBatch2D.TriangleVertices.Count;
				if (this.Direction == LayoutDirection.Horizontal)
				{
					float scrollPosition = this.ScrollPosition;
					float x = base.ActualSize.X;
					Vector2 corner = new Vector2(scrollPosition / this.m_scrollAreaLength * x, base.ActualSize.Y - 5f);
					Vector2 corner2 = new Vector2((scrollPosition + x) / this.m_scrollAreaLength * x, base.ActualSize.Y - 1f);
					flatBatch2D.QueueQuad(corner, corner2, 0f, color);
				}
				else
				{
					float scrollPosition2 = this.ScrollPosition;
					float y = base.ActualSize.Y;
					Vector2 corner3 = new Vector2(base.ActualSize.X - 5f, scrollPosition2 / this.m_scrollAreaLength * y);
					Vector2 corner4 = new Vector2(base.ActualSize.X - 1f, (scrollPosition2 + y) / this.m_scrollAreaLength * y);
					flatBatch2D.QueueQuad(corner3, corner4, 0f, color);
				}
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x000D31D0 File Offset: 0x000D13D0
		public bool HitTestPanel(Vector2 position)
		{
			bool found = false;
			base.HitTestGlobal(position, delegate(Widget widget)
			{
				found = (widget.IsChildWidgetOf(this) || widget == this);
				return true;
			});
			return found;
		}

		// Token: 0x040012AF RID: 4783
		public Vector2? m_lastDragPosition;

		// Token: 0x040012B0 RID: 4784
		public float m_dragSpeed;

		// Token: 0x040012B1 RID: 4785
		public float m_scrollBarAlpha;

		// Token: 0x040012B2 RID: 4786
		public float m_scrollAreaLength;
	}
}
