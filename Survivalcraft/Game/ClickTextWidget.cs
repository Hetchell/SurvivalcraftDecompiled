using System;
using Engine;

namespace Game
{
	// Token: 0x02000375 RID: 885
	public class ClickTextWidget : CanvasWidget
	{
		// Token: 0x0600195D RID: 6493 RVA: 0x000C6850 File Offset: 0x000C4A50
		public ClickTextWidget(Vector2 vector2, string text, Action click, bool box = false)
		{
			base.Size = vector2;
			this.HorizontalAlignment = WidgetAlignment.Center;
			this.VerticalAlignment = WidgetAlignment.Center;
			this.labelWidget = new LabelWidget
			{
				Text = text,
				FontScale = 0.8f,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			if (click == null)
			{
				this.Children.Add(this.labelWidget);
				return;
			}
			this.rectangleWidget = new RectangleWidget
			{
				OutlineThickness = 0f
			};
			if (box)
			{
				this.BackGround = Color.Gray;
				this.rectangleWidget.FillColor = this.BackGround;
				this.rectangleWidget.OutlineColor = Color.Transparent;
				this.rectangleWidget.OutlineThickness = 1f;
			}
			this.Children.Add(this.rectangleWidget);
			this.Children.Add(this.labelWidget);
			this.click = click;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x000C6950 File Offset: 0x000C4B50
		public override void Update()
		{
			if (base.Input.Click != null && (this.HitTest(base.Input.Click.Value.Start) || this.HitTest(base.Input.Click.Value.End)))
			{
				Action action = this.click;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x040011B5 RID: 4533
		public LabelWidget labelWidget;

		// Token: 0x040011B6 RID: 4534
		public Action click;

		// Token: 0x040011B7 RID: 4535
		public RectangleWidget rectangleWidget;

		// Token: 0x040011B8 RID: 4536
		public Color BackGround = Color.Transparent;

		// Token: 0x040011B9 RID: 4537
		public Color pressColor = Color.Red;
	}
}
