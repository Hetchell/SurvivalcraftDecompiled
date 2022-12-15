using System;
using Engine;
using Engine.Media;
using Engine.Serialization;

namespace Game
{
	// Token: 0x0200025B RID: 603
	public class EditColorDialog : Dialog
	{
		// Token: 0x0600121F RID: 4639 RVA: 0x0008BAD0 File Offset: 0x00089CD0
		public EditColorDialog(Color color, Action<Color?> handler)
		{
			WidgetsList children = this.Children;
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(660f, 420f),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Children = 
				{
					new RectangleWidget
					{
						FillColor = new Color(0, 0, 0, 255),
						OutlineColor = new Color(128, 128, 128, 128),
						OutlineThickness = 2f
					}
				}
			};
			WidgetsList children2 = canvasWidget.Children;
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				Margin = new Vector2(15f),
				HorizontalAlignment = WidgetAlignment.Center,
				Children = 
				{
					new LabelWidget
					{
						Text = "Edit Color",
						HorizontalAlignment = WidgetAlignment.Center
					},
					new CanvasWidget
					{
						Size = new Vector2(0f, float.PositiveInfinity)
					}
				}
			};
			WidgetsList children3 = stackPanelWidget.Children;
			StackPanelWidget stackPanelWidget2 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal
			};
			WidgetsList children4 = stackPanelWidget2.Children;
			StackPanelWidget stackPanelWidget3 = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				VerticalAlignment = WidgetAlignment.Center
			};
			WidgetsList children5 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget4 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = "Red:",
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center,
						Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children6 = stackPanelWidget4.Children;
			SliderWidget sliderWidget = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			SliderWidget widget = sliderWidget;
			this.m_sliderR = sliderWidget;
			children6.Add(widget);
			children5.Add(stackPanelWidget4);
			WidgetsList children7 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget5 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = "Green:",
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center,
						Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children8 = stackPanelWidget5.Children;
			SliderWidget sliderWidget2 = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			widget = sliderWidget2;
			this.m_sliderG = sliderWidget2;
			children8.Add(widget);
			children7.Add(stackPanelWidget5);
			WidgetsList children9 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget6 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = "Blue:",
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center,
						Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children10 = stackPanelWidget6.Children;
			SliderWidget sliderWidget3 = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			widget = sliderWidget3;
			this.m_sliderB = sliderWidget3;
			children10.Add(widget);
			children9.Add(stackPanelWidget6);
			children4.Add(stackPanelWidget3);
			stackPanelWidget2.Children.Add(new CanvasWidget
			{
				Size = new Vector2(20f, 0f)
			});
			WidgetsList children11 = stackPanelWidget2.Children;
			CanvasWidget canvasWidget2 = new CanvasWidget();
			WidgetsList children12 = canvasWidget2.Children;
			BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget
			{
				Size = new Vector2(200f, 240f),
				AmbientLight = 1f,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			BevelledButtonWidget widget2 = bevelledButtonWidget;
			this.m_rectangle = bevelledButtonWidget;
			children12.Add(widget2);
			WidgetsList children13 = canvasWidget2.Children;
			LabelWidget labelWidget = new LabelWidget
			{
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
			};
			LabelWidget widget3 = labelWidget;
			this.m_label = labelWidget;
			children13.Add(widget3);
			children11.Add(canvasWidget2);
			children3.Add(stackPanelWidget2);
			stackPanelWidget.Children.Add(new CanvasWidget
			{
				Size = new Vector2(0f, float.PositiveInfinity)
			});
			WidgetsList children14 = stackPanelWidget.Children;
			StackPanelWidget stackPanelWidget7 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Center
			};
			WidgetsList children15 = stackPanelWidget7.Children;
			BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Text = "OK"
			};
			ButtonWidget widget4 = bevelledButtonWidget2;
			this.m_okButton = bevelledButtonWidget2;
			children15.Add(widget4);
			stackPanelWidget7.Children.Add(new CanvasWidget
			{
				Size = new Vector2(50f, 0f)
			});
			WidgetsList children16 = stackPanelWidget7.Children;
			BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Text = "Cancel"
			};
			widget4 = bevelledButtonWidget3;
			this.m_cancelButton = bevelledButtonWidget3;
			children16.Add(widget4);
			children14.Add(stackPanelWidget7);
			children2.Add(stackPanelWidget);
			children.Add(canvasWidget);
			this.m_handler = handler;
			this.m_color = color;
			this.UpdateControls();
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0008C0E8 File Offset: 0x0008A2E8
		public override void Update()
		{
			if (this.m_rectangle.IsClicked)
			{
				DialogsManager.ShowDialog(this, new TextBoxDialog("Enter Color", this.GetColorString(), 20, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							this.m_color.RGB = HumanReadableConverter.ConvertFromString<Color>(s);
						}
						catch
						{
							DialogsManager.ShowDialog(this, new MessageDialog("Invalid Color", "Use R,G,B or #HEX notation, e.g. 255,92,13 or #FF5C0D", "OK", null, null));
						}
					}
				}));
			}
			if (this.m_sliderR.IsSliding)
			{
				this.m_color.R = (byte)this.m_sliderR.Value;
			}
			if (this.m_sliderG.IsSliding)
			{
				this.m_color.G = (byte)this.m_sliderG.Value;
			}
			if (this.m_sliderB.IsSliding)
			{
				this.m_color.B = (byte)this.m_sliderB.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new Color?(this.m_color));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0008C1E0 File Offset: 0x0008A3E0
		public void UpdateControls()
		{
			this.m_rectangle.CenterColor = this.m_color;
			this.m_sliderR.Value = (float)this.m_color.R;
			this.m_sliderG.Value = (float)this.m_color.G;
			this.m_sliderB.Value = (float)this.m_color.B;
			this.m_label.Text = this.GetColorString();
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0008C254 File Offset: 0x0008A454
		public string GetColorString()
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", this.m_color.R, this.m_color.G, this.m_color.B);
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0008C290 File Offset: 0x0008A490
		public void Dismiss(Color? result)
		{
			DialogsManager.HideDialog(this);
			Action<Color?> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(result);
		}

		// Token: 0x04000C33 RID: 3123
		public BevelledButtonWidget m_rectangle;

		// Token: 0x04000C34 RID: 3124
		public SliderWidget m_sliderR;

		// Token: 0x04000C35 RID: 3125
		public SliderWidget m_sliderG;

		// Token: 0x04000C36 RID: 3126
		public SliderWidget m_sliderB;

		// Token: 0x04000C37 RID: 3127
		public LabelWidget m_label;

		// Token: 0x04000C38 RID: 3128
		public ButtonWidget m_okButton;

		// Token: 0x04000C39 RID: 3129
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C3A RID: 3130
		public Action<Color?> m_handler;

		// Token: 0x04000C3B RID: 3131
		public Color m_color;
	}
}
