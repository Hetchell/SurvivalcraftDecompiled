using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x0200037F RID: 895
	public class EditMemeryDialogB : Dialog
	{
		// Token: 0x0600198F RID: 6543 RVA: 0x000C871C File Offset: 0x000C691C
		public EditMemeryDialogB(MemoryBankData memoryBankData, Action onCancel)
		{
			this.memory = memoryBankData;
			this.Data.Clear();
			this.Data.AddRange(this.memory.Data);
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(600f, float.PositiveInfinity),
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
				Direction = LayoutDirection.Vertical
			};
			LabelWidget widget2 = new LabelWidget
			{
				Text = LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 0),
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f)
			};
			StackPanelWidget stackPanelWidget2 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(10f, 10f)
			};
			this.Children.Add(canvasWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(stackPanelWidget2);
			stackPanelWidget2.Children.Add(this.initData());
			stackPanelWidget2.Children.Add(this.initButton());
			this.MainView = stackPanelWidget;
			this.onCancel = onCancel;
			this.lastvalue = (int)this.memory.Read(0);
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x000C88D2 File Offset: 0x000C6AD2
		public byte Read(int address)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				return this.Data.Array[address];
			}
			return 0;
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x000C88F8 File Offset: 0x000C6AF8
		public void Write(int address, byte data)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				this.Data.Array[address] = data;
				return;
			}
			if (address >= 0 && address < 256 && data != 0)
			{
				this.Data.Count = MathUtils.Max(this.Data.Count, address + 1);
				this.Data.Array[address] = data;
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x000C8964 File Offset: 0x000C6B64
		public void LoadString(string data)
		{
			string[] array = data.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length >= 1)
			{
				string text = array[0];
				text = text.TrimEnd(new char[]
				{
					'0'
				});
				this.Data.Clear();
				for (int i = 0; i < MathUtils.Min(text.Length, 256); i++)
				{
					int num = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text[i]));
					if (num < 0)
					{
						num = 0;
					}
					this.Data.Add((byte)num);
				}
			}
			if (array.Length >= 2)
			{
				string text2 = array[1];
				int num2 = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text2[0]));
				if (num2 < 0)
				{
					num2 = 0;
				}
				this.LastOutput = (byte)num2;
			}
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000C8A25 File Offset: 0x000C6C25
		public string SaveString()
		{
			return this.SaveString(true);
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001994 RID: 6548 RVA: 0x000C8A2E File Offset: 0x000C6C2E
		// (set) Token: 0x06001995 RID: 6549 RVA: 0x000C8A36 File Offset: 0x000C6C36
		public byte LastOutput { get; set; }

		// Token: 0x06001996 RID: 6550 RVA: 0x000C8A40 File Offset: 0x000C6C40
		public string SaveString(bool saveLastOutput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = this.Data.Count;
			for (int i = 0; i < count; i++)
			{
				int index = MathUtils.Clamp((int)this.Data.Array[i], 0, 15);
				stringBuilder.Append(MemoryBankData.m_hexChars[index]);
			}
			if (saveLastOutput)
			{
				stringBuilder.Append(';');
				stringBuilder.Append(MemoryBankData.m_hexChars[MathUtils.Clamp((int)this.LastOutput, 0, 15)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x000C8AC4 File Offset: 0x000C6CC4
		public Widget initData()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(10f, 0f)
			};
			for (int i = 0; i < 17; i++)
			{
				StackPanelWidget stackPanelWidget2 = new StackPanelWidget
				{
					Direction = LayoutDirection.Horizontal
				};
				for (int j = 0; j < 17; j++)
				{
					int addr = (i - 1) * 16 + (j - 1);
					if (j > 0 && i > 0)
					{
						ClickTextWidget clickTextWidget = new ClickTextWidget(new Vector2(22f), string.Format("{0}", MemoryBankData.m_hexChars[(int)this.Read(addr)]), delegate()
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							this.clickpos = addr;
							this.isclick = true;
						}, false);
						this.list.Add(clickTextWidget);
						stackPanelWidget2.Children.Add(clickTextWidget);
					}
					else
					{
						int index;
						if (i == 0 && j > 0)
						{
							index = j - 1;
						}
						else
						{
							if (j != 0 || i <= 0)
							{
								ClickTextWidget widget = new ClickTextWidget(new Vector2(22f), "", null, false);
								stackPanelWidget2.Children.Add(widget);
								goto IL_181;
							}
							index = i - 1;
						}
						ClickTextWidget clickTextWidget2 = new ClickTextWidget(new Vector2(22f), MemoryBankData.m_hexChars[index].ToString(), delegate()
						{
						}, false);
						clickTextWidget2.labelWidget.Color = Color.DarkGray;
						stackPanelWidget2.Children.Add(clickTextWidget2);
					}
					IL_181:;
				}
				stackPanelWidget.Children.Add(stackPanelWidget2);
			}
			return stackPanelWidget;
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x000C8C78 File Offset: 0x000C6E78
		public Widget makeFuncButton(string txt, Action func)
		{
			return new ClickTextWidget(new Vector2(50f, 40f), txt, func, true)
			{
				Margin = new Vector2(5f, 2f),
				labelWidget = 
				{
					FontScale = 1f,
					Color = Color.Black
				}
			};
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x000C8CD4 File Offset: 0x000C6ED4
		public Widget initButton()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical
			};
			for (int i = 0; i < 6; i++)
			{
				StackPanelWidget stackPanelWidget2 = new StackPanelWidget
				{
					Direction = LayoutDirection.Horizontal
				};
				for (int j = 0; j < 3; j++)
				{
					int num = i * 3 + j;
					if (num < 15)
					{
						int pp = num + 1;
						stackPanelWidget2.Children.Add(this.makeFuncButton(string.Format("{0}", MemoryBankData.m_hexChars[pp]), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							if (!this.isSetPos)
							{
								this.Write(this.clickpos, (byte)pp);
								this.lastvalue = pp;
								this.clickpos++;
								if (this.clickpos >= 255)
								{
									this.clickpos = 0;
								}
								this.isclick = true;
								return;
							}
							if (this.setPosN == 0)
							{
								this.clickpos = 16 * pp;
							}
							else if (this.setPosN == 1)
							{
								this.clickpos += pp;
							}
							this.setPosN++;
							if (this.setPosN == 2)
							{
								if (this.clickpos > 255)
								{
									this.clickpos = 0;
								}
								this.setPosN = 0;
								this.isclick = true;
								this.isSetPos = false;
							}
						}));
					}
					else if (num == 15)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(string.Format("{0}", MemoryBankData.m_hexChars[0]), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							if (!this.isSetPos)
							{
								this.Write(this.clickpos, 0);
								this.lastvalue = 0;
								this.clickpos++;
								if (this.clickpos >= 255)
								{
									this.clickpos = 0;
								}
								this.isclick = true;
								return;
							}
							if (this.setPosN == 0)
							{
								this.clickpos = 0;
							}
							else if (this.setPosN == 1)
							{
								;
							}
							this.setPosN++;
							if (this.setPosN == 2)
							{
								if (this.clickpos > 255)
								{
									this.clickpos = 0;
								}
								this.setPosN = 0;
								this.isclick = true;
								this.isSetPos = false;
							}
						}));
					}
					else if (num == 16)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 1), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							for (int k = 0; k < this.Data.Count; k++)
							{
								this.Write(k, 0);
							}
							this.isclick = true;
						}));
					}
					else if (num == 17)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 2), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							DynamicArray<byte> dynamicArray = new DynamicArray<byte>();
							dynamicArray.AddRange(this.Data);
							dynamicArray.Count = 256;
							for (int k = 0; k < 16; k++)
							{
								for (int l = 0; l < 16; l++)
								{
									this.Write(k + l * 16, dynamicArray[k * 16 + l]);
								}
							}
							this.clickpos = 0;
							this.isclick = true;
						}));
					}
				}
				stackPanelWidget.Children.Add(stackPanelWidget2);
			}
			LabelWidget widget = new LabelWidget
			{
				FontScale = 0.8f,
				Text = LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 3),
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = Color.DarkGray
			};
			stackPanelWidget.Children.Add(widget);
			stackPanelWidget.Children.Add(this.makeTextBox(delegate(TextBoxWidget textBoxWidget)
			{
				this.LoadString(textBoxWidget.Text);
				this.isclick = true;
			}, this.memory.SaveString(false)));
			stackPanelWidget.Children.Add(this.makeButton(LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 4), delegate
			{
				for (int k = 0; k < this.Data.Count; k++)
				{
					this.memory.Write(k, this.Data[k]);
				}
				Action action = this.onCancel;
				if (action != null)
				{
					action();
				}
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				DialogsManager.HideDialog(this);
			}));
			stackPanelWidget.Children.Add(this.makeButton(LanguageControl.GetContentWidgets(EditMemeryDialogB.fName, 5), delegate
			{
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				DialogsManager.HideDialog(this);
				this.isclick = true;
			}));
			return stackPanelWidget;
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x000C8F08 File Offset: 0x000C7108
		public Widget makeTextBox(Action<TextBoxWidget> ac, string text = "")
		{
			CanvasWidget canvasWidget = new CanvasWidget();
			canvasWidget.HorizontalAlignment = WidgetAlignment.Center;
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = Color.Black,
				OutlineColor = Color.White,
				Size = new Vector2(120f, 30f)
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical
			};
			TextBoxWidget textBoxWidget = new TextBoxWidget
			{
				VerticalAlignment = WidgetAlignment.Center,
				Color = new Color(255, 255, 255),
				Margin = new Vector2(4f, 0f),
				Size = new Vector2(120f, 30f),
				MaximumLength = 256
			};
			textBoxWidget.FontScale = 0.7f;
			textBoxWidget.Text = text;
			textBoxWidget.TextChanged += ac;
			stackPanelWidget.Children.Add(textBoxWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			return canvasWidget;
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x000C8FF8 File Offset: 0x000C71F8
		public Widget makeButton(string txt, Action tas)
		{
			return new ClickTextWidget(new Vector2(120f, 30f), txt, tas, false)
			{
				rectangleWidget = 
				{
                    OutlineThickness = 2f,

                    OutlineColor = Color.White
				},
				BackGround = Color.Gray,
				Margin = new Vector2(0f, 3f),
				labelWidget = 
				{
					FontScale = 0.7f,
					Color = Color.Green
				}
			};
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x000C907C File Offset: 0x000C727C
		public override void Update()
		{
			int num = 0;
			if (this.isSetPos)
			{
				this.list[this.clickpos].rectangleWidget.OutlineColor = Color.Red;
				this.list[this.clickpos].rectangleWidget.OutlineThickness = 1f;
				return;
			}
			if (!this.isclick)
			{
				return;
			}
			foreach (ClickTextWidget clickTextWidget in this.list)
			{
				if (num == this.clickpos)
				{
					this.list[num].rectangleWidget.OutlineColor = Color.Yellow;
					this.list[num].rectangleWidget.OutlineThickness = 1f;
				}
				else
				{
					this.list[num].rectangleWidget.OutlineColor = Color.Transparent;
				}
				this.list[num].labelWidget.Text = string.Format("{0}", MemoryBankData.m_hexChars[(int)this.Read(num)]);
				this.list[num].IsDrawRequired = false;
				num++;
			}
			if (base.Input.Cancel || base.Input.Back)
			{
				DialogsManager.HideDialog(this);
			}
			this.isclick = false;
		}

		// Token: 0x040011F2 RID: 4594
		public MemoryBankData memory;

		// Token: 0x040011F3 RID: 4595
		public DynamicArray<byte> Data = new DynamicArray<byte>();

		// Token: 0x040011F4 RID: 4596
		public StackPanelWidget MainView;

		// Token: 0x040011F5 RID: 4597
		public Action onCancel;

		// Token: 0x040011F6 RID: 4598
		public int clickpos;

		// Token: 0x040011F7 RID: 4599
		public bool isSetPos;

		// Token: 0x040011F8 RID: 4600
		public int setPosN;

		// Token: 0x040011F9 RID: 4601
		public int lastvalue;

		// Token: 0x040011FA RID: 4602
		public bool isclick = true;

		// Token: 0x040011FB RID: 4603
		public static string fName = "EditMemeryDialogB";

		// Token: 0x040011FC RID: 4604
		public List<ClickTextWidget> list = new List<ClickTextWidget>();
	}
}
