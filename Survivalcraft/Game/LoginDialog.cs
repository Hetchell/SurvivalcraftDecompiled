using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x0200038E RID: 910
	public class LoginDialog : Dialog
	{
		// Token: 0x06001A61 RID: 6753 RVA: 0x000CF8C0 File Offset: 0x000CDAC0
		public LoginDialog()
		{
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(600f, 240f),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = new Color(0, 0, 0, 255),
				OutlineColor = new Color(128, 128, 128, 128),
				OutlineThickness = 2f
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(10f, 10f)
			};
			this.Children.Add(canvasWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			this.MainView = stackPanelWidget;
			this.MainView.Children.Add(this.tip);
			this.MainView.Children.Add(this.makeTextBox("用户名:"));
			this.MainView.Children.Add(this.makeTextBox("密  码:"));
			this.MainView.Children.Add(this.makeButton());
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x000CFA2C File Offset: 0x000CDC2C
		public Widget makeTextBox(string title)
		{
			CanvasWidget canvasWidget = new CanvasWidget();
			canvasWidget.Margin = new Vector2(10f, 0f);
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = Color.Black,
				OutlineColor = Color.White,
				Size = new Vector2(float.PositiveInfinity, 80f)
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal
			};
			LabelWidget widget2 = new LabelWidget
			{
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Text = title,
				Margin = new Vector2(1f, 1f)
			};
			TextBoxWidget widget3 = new TextBoxWidget
			{
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Stretch,
				Color = new Color(255, 255, 255),
				Margin = new Vector2(4f, 0f),
				Size = new Vector2(float.PositiveInfinity, 80f)
			};
			if (title == "用户名:")
			{
				this.txa = widget3;
			}
			if (title == "密  码:")
			{
				this.txb = widget3;
			}
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(widget3);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			return canvasWidget;
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x000CFB74 File Offset: 0x000CDD74
		public Widget makeButton()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget();
			stackPanelWidget.Direction = LayoutDirection.Horizontal;
			BevelledButtonWidget widget = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "登陆"
			};
			BevelledButtonWidget widget2 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "注册"
			};
			BevelledButtonWidget widget3 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "取消"
			};
			stackPanelWidget.Children.Add(widget);
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(widget3);
			this.btna = widget;
			this.btnb = widget2;
			this.btnc = widget3;
			return stackPanelWidget;
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x000CFC78 File Offset: 0x000CDE78
		public override void Update()
		{
			if (this.btna.IsClicked)
			{
				WebManager.Post("https://m.schub.top/com/api/login", new Dictionary<string, string>
				{
					{
						"user",
						this.txa.Text
					},
					{
						"pass",
						this.txb.Text
					}
				}, null, new MemoryStream(), new CancellableProgress(), this.succ, this.fail);
			}
			if (this.btnb.IsClicked)
			{
				WebBrowserManager.LaunchBrowser("https://m.schub.top/com/reg");
			}
			if (this.btnc.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04001260 RID: 4704
		public Action<byte[]> succ;

		// Token: 0x04001261 RID: 4705
		public Action<Exception> fail;

		// Token: 0x04001262 RID: 4706
		public StackPanelWidget MainView;

		// Token: 0x04001263 RID: 4707
		public BevelledButtonWidget btna;

		// Token: 0x04001264 RID: 4708
		public BevelledButtonWidget btnb;

		// Token: 0x04001265 RID: 4709
		public BevelledButtonWidget btnc;

		// Token: 0x04001266 RID: 4710
		public TextBoxWidget txa;

		// Token: 0x04001267 RID: 4711
		public TextBoxWidget txb;

		// Token: 0x04001268 RID: 4712
		public LabelWidget tip = new LabelWidget
		{
			HorizontalAlignment = WidgetAlignment.Near,
			VerticalAlignment = WidgetAlignment.Near,
			Margin = new Vector2(1f, 1f)
		};
	}
}
