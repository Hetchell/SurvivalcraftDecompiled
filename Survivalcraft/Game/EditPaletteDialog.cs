using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200025D RID: 605
	public class EditPaletteDialog : Dialog
	{
		// Token: 0x06001229 RID: 4649 RVA: 0x0008C660 File Offset: 0x0008A860
		public EditPaletteDialog(WorldPalette palette)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditPaletteDialog");
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ContainerWidget>("EditPaletteDialog.ListPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditPaletteDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditPaletteDialog.Cancel", true);
			for (int i = 0; i < 16; i++)
			{
				StackPanelWidget stackPanelWidget = new StackPanelWidget();
				stackPanelWidget.Direction = LayoutDirection.Horizontal;
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(32f, 60f),
					Children = 
					{
						new LabelWidget
						{
							Text = (i + 1).ToString() + ".",
							Color = Color.Gray,
							HorizontalAlignment = WidgetAlignment.Far,
							VerticalAlignment = WidgetAlignment.Center,
							Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
						}
					}
				});
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children = stackPanelWidget.Children;
				LinkWidget[] labels = this.m_labels;
				int num = i;
				LinkWidget linkWidget = new LinkWidget();
				linkWidget.Size = new Vector2(300f, -1f);
				linkWidget.VerticalAlignment = WidgetAlignment.Center;
				linkWidget.Font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
				LinkWidget widget = linkWidget;
				labels[num] = linkWidget;
				children.Add(widget);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children2 = stackPanelWidget.Children;
				BevelledButtonWidget[] rectangles = this.m_rectangles;
				int num2 = i;
				BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
				bevelledButtonWidget.Size = new Vector2(float.PositiveInfinity, 60f);
				bevelledButtonWidget.BevelSize = 1f;
				bevelledButtonWidget.AmbientLight = 1f;
				bevelledButtonWidget.DirectionalLight = 0.4f;
				bevelledButtonWidget.VerticalAlignment = WidgetAlignment.Center;
				BevelledButtonWidget widget2 = bevelledButtonWidget;
				rectangles[num2] = bevelledButtonWidget;
				children2.Add(widget2);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children3 = stackPanelWidget.Children;
				ButtonWidget[] resetButtons = this.m_resetButtons;
				int num3 = i;
				BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
				bevelledButtonWidget2.Size = new Vector2(160f, 60f);
				bevelledButtonWidget2.VerticalAlignment = WidgetAlignment.Center;
				bevelledButtonWidget2.Text = "Reset";
				ButtonWidget widget3 = bevelledButtonWidget2;
				resetButtons[num3] = bevelledButtonWidget2;
				children3.Add(widget3);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				StackPanelWidget widget4 = stackPanelWidget;
				this.m_listPanel.Children.Add(widget4);
			}
			this.m_palette = palette;
			this.m_tmpPalette = new WorldPalette();
			this.m_palette.CopyTo(this.m_tmpPalette);
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0008C93C File Offset: 0x0008AB3C
		public override void Update()
		{
			for (int k = 0; k < 16; k++)
			{
				this.m_labels[k].Text = this.m_tmpPalette.Names[k];
				this.m_rectangles[k].CenterColor = this.m_tmpPalette.Colors[k];
				this.m_resetButtons[k].IsEnabled = (this.m_tmpPalette.Colors[k] != WorldPalette.DefaultColors[k] || this.m_tmpPalette.Names[k] != LanguageControl.Get("WorldPalette", k));
			}
			for (int j = 0; j < 16; j++)
			{
				int i = j;
				if (this.m_labels[j].IsClicked)
				{
					DialogsManager.ShowDialog(this, new TextBoxDialog("Edit Color Name", this.m_labels[j].Text, 16, delegate(string s)
					{
						if (s != null)
						{
							if (WorldPalette.VerifyColorName(s))
							{
								this.m_tmpPalette.Names[i] = s;
								return;
							}
							DialogsManager.ShowDialog(this, new MessageDialog("Invalid name", null, "OK", null, null));
						}
					}));
				}
				if (this.m_rectangles[j].IsClicked)
				{
					DialogsManager.ShowDialog(this, new EditColorDialog(this.m_tmpPalette.Colors[j], delegate(Color? color)
					{
						if (color != null)
						{
							this.m_tmpPalette.Colors[i] = color.Value;
						}
					}));
				}
				if (this.m_resetButtons[j].IsClicked)
				{
					this.m_tmpPalette.Colors[j] = WorldPalette.DefaultColors[j];
					this.m_tmpPalette.Names[j] = LanguageControl.Get("WorldPalette", j);
				}
			}
			if (this.m_okButton.IsClicked)
			{
				this.m_tmpPalette.CopyTo(this.m_palette);
				this.Dismiss();
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss();
			}
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0008CB03 File Offset: 0x0008AD03
		public void Dismiss()
		{
			DialogsManager.HideDialog(this);
		}

		// Token: 0x04000C47 RID: 3143
		public ContainerWidget m_listPanel;

		// Token: 0x04000C48 RID: 3144
		public ButtonWidget m_okButton;

		// Token: 0x04000C49 RID: 3145
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C4A RID: 3146
		public LinkWidget[] m_labels = new LinkWidget[16];

		// Token: 0x04000C4B RID: 3147
		public BevelledButtonWidget[] m_rectangles = new BevelledButtonWidget[16];

		// Token: 0x04000C4C RID: 3148
		public ButtonWidget[] m_resetButtons = new ButtonWidget[16];

		// Token: 0x04000C4D RID: 3149
		public WorldPalette m_palette;

		// Token: 0x04000C4E RID: 3150
		public WorldPalette m_tmpPalette;
	}
}
