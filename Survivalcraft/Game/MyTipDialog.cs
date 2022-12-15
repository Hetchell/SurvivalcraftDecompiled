using System;

namespace Game
{
	// Token: 0x02000367 RID: 871
	public class MyTipDialog : Dialog
	{
		// Token: 0x0600189F RID: 6303 RVA: 0x000C41C8 File Offset: 0x000C23C8
		public MyTipDialog(string text, string cancel)
		{
			this.Children.Add(this.canvasWidget);
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Direction = LayoutDirection.Vertical
			};
			this.bevelledButtonWidget.Text = cancel;
			this.labelWidget.Text = text;
			stackPanelWidget.Children.Add(this.labelWidget);
			this.canvasWidget.Children.Add(stackPanelWidget);
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x000C4262 File Offset: 0x000C2462
		public override void Update()
		{
			if (this.bevelledButtonWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04001165 RID: 4453
		public BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();

		// Token: 0x04001166 RID: 4454
		public LabelWidget labelWidget = new LabelWidget();

		// Token: 0x04001167 RID: 4455
		public CanvasWidget canvasWidget = new CanvasWidget();
	}
}
