using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200038B RID: 907
	public class LabelWidget : Widget
	{
		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001A0E RID: 6670 RVA: 0x000CE10B File Offset: 0x000CC30B
		// (set) Token: 0x06001A0F RID: 6671 RVA: 0x000CE113 File Offset: 0x000CC313
		public Vector2 Size { get; set; } = new Vector2(-1f);

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001A10 RID: 6672 RVA: 0x000CE11C File Offset: 0x000CC31C
		// (set) Token: 0x06001A11 RID: 6673 RVA: 0x000CE124 File Offset: 0x000CC324
		public string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (this.m_text != value && value != null)
				{
					if (value.StartsWith("[") && value.EndsWith("]"))
					{
						string[] array = value.Substring(1, value.Length - 2).Split(new char[]
						{
							':'
						});
						if (array.Length > 1)
						{
							this.m_text = LanguageControl.GetContentWidgets(array[0], array[1]);
						}
						else
						{
							this.m_text = value;
						}
					}
					else
					{
						this.m_text = LanguageControl.Get("Usual", value);
					}
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x000CE1BB File Offset: 0x000CC3BB
		// (set) Token: 0x06001A13 RID: 6675 RVA: 0x000CE1C3 File Offset: 0x000CC3C3
		public TextAnchor TextAnchor { get; set; }

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001A14 RID: 6676 RVA: 0x000CE1CC File Offset: 0x000CC3CC
		// (set) Token: 0x06001A15 RID: 6677 RVA: 0x000CE1D4 File Offset: 0x000CC3D4
		public TextOrientation TextOrientation
		{
			get
			{
				return this.m_textOrientation;
			}
			set
			{
				if (value != this.m_textOrientation)
				{
					this.m_textOrientation = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001A16 RID: 6678 RVA: 0x000CE1F2 File Offset: 0x000CC3F2
		// (set) Token: 0x06001A17 RID: 6679 RVA: 0x000CE1FA File Offset: 0x000CC3FA
		public BitmapFont Font
		{
			get
			{
				return this.m_font;
			}
			set
			{
				if (value != this.m_font)
				{
					this.m_font = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001A18 RID: 6680 RVA: 0x000CE218 File Offset: 0x000CC418
		// (set) Token: 0x06001A19 RID: 6681 RVA: 0x000CE220 File Offset: 0x000CC420
		public float FontScale
		{
			get
			{
				return this.m_fontScale;
			}
			set
			{
				if (value != this.m_fontScale)
				{
					this.m_fontScale = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001A1A RID: 6682 RVA: 0x000CE23E File Offset: 0x000CC43E
		// (set) Token: 0x06001A1B RID: 6683 RVA: 0x000CE246 File Offset: 0x000CC446
		public Vector2 FontSpacing
		{
			get
			{
				return this.m_fontSpacing;
			}
			set
			{
				if (value != this.m_fontSpacing)
				{
					this.m_fontSpacing = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001A1C RID: 6684 RVA: 0x000CE269 File Offset: 0x000CC469
		// (set) Token: 0x06001A1D RID: 6685 RVA: 0x000CE271 File Offset: 0x000CC471
		public bool WordWrap
		{
			get
			{
				return this.m_wordWrap;
			}
			set
			{
				if (value != this.m_wordWrap)
				{
					this.m_wordWrap = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001A1E RID: 6686 RVA: 0x000CE28F File Offset: 0x000CC48F
		// (set) Token: 0x06001A1F RID: 6687 RVA: 0x000CE297 File Offset: 0x000CC497
		public bool Ellipsis
		{
			get
			{
				return this.m_ellipsis;
			}
			set
			{
				if (value != this.m_ellipsis)
				{
					this.m_ellipsis = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001A20 RID: 6688 RVA: 0x000CE2B5 File Offset: 0x000CC4B5
		// (set) Token: 0x06001A21 RID: 6689 RVA: 0x000CE2BD File Offset: 0x000CC4BD
		public int MaxLines
		{
			get
			{
				return this.m_maxLines;
			}
			set
			{
				if (value != this.m_maxLines)
				{
					this.m_maxLines = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001A22 RID: 6690 RVA: 0x000CE2DB File Offset: 0x000CC4DB
		// (set) Token: 0x06001A23 RID: 6691 RVA: 0x000CE2E3 File Offset: 0x000CC4E3
		public Color Color { get; set; }

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001A24 RID: 6692 RVA: 0x000CE2EC File Offset: 0x000CC4EC
		// (set) Token: 0x06001A25 RID: 6693 RVA: 0x000CE2F4 File Offset: 0x000CC4F4
		public bool DropShadow { get; set; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001A26 RID: 6694 RVA: 0x000CE2FD File Offset: 0x000CC4FD
		// (set) Token: 0x06001A27 RID: 6695 RVA: 0x000CE305 File Offset: 0x000CC505
		public bool TextureLinearFilter { get; set; }

		// Token: 0x06001A28 RID: 6696 RVA: 0x000CE310 File Offset: 0x000CC510
		public LabelWidget()
		{
			this.IsHitTestVisible = false;
			this.Font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			this.Text = string.Empty;
			this.FontScale = 1f;
			this.Color = Color.White;
			this.TextureLinearFilter = true;
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x000CE388 File Offset: 0x000CC588
		public override void Draw(Widget.DrawContext dc)
		{
			if (!string.IsNullOrEmpty(this.Text) && this.Color.A != 0)
			{
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp;
				FontBatch2D fontBatch2D = dc.PrimitivesRenderer2D.FontBatch(this.Font, 1, DepthStencilState.None, null, null, samplerState);
				int count = fontBatch2D.TriangleVertices.Count;
				float num = 0f;
				if ((this.TextAnchor & TextAnchor.VerticalCenter) != TextAnchor.Default)
				{
					float num2 = this.Font.GlyphHeight * this.FontScale * this.Font.Scale + (float)(this.m_lines.Count - 1) * ((this.Font.GlyphHeight + this.Font.Spacing.Y) * this.FontScale * this.Font.Scale + this.FontSpacing.Y);
					num = (base.ActualSize.Y - num2) / 2f;
				}
				else if ((this.TextAnchor & TextAnchor.Bottom) != TextAnchor.Default)
				{
					float num3 = this.Font.GlyphHeight * this.FontScale * this.Font.Scale + (float)(this.m_lines.Count - 1) * ((this.Font.GlyphHeight + this.Font.Spacing.Y) * this.FontScale * this.Font.Scale + this.FontSpacing.Y);
					num = base.ActualSize.Y - num3;
				}
				TextAnchor anchor = this.TextAnchor & ~(TextAnchor.VerticalCenter | TextAnchor.Bottom);
				Color color = this.Color * base.GlobalColorTransform;
				float num4 = this.CalculateLineHeight();
				foreach (string text in this.m_lines)
				{
					float x = 0f;
					if ((this.TextAnchor & TextAnchor.HorizontalCenter) != TextAnchor.Default)
					{
						x = base.ActualSize.X / 2f;
					}
					else if ((this.TextAnchor & TextAnchor.Right) != TextAnchor.Default)
					{
						x = base.ActualSize.X;
					}
					bool flag = true;
					Vector2 zero = Vector2.Zero;
					float angle = 0f;
					if (this.TextOrientation == TextOrientation.Horizontal)
					{
						zero = new Vector2(x, num);
						angle = 0f;
						Rectangle scissorRectangle = Display.ScissorRectangle;
						flag = true;
					}
					else if (this.TextOrientation == TextOrientation.VerticalLeft)
					{
						zero = new Vector2(x, base.ActualSize.Y + num);
						angle = MathUtils.DegToRad(-90f);
						flag = true;
					}
					if (flag)
					{
						if (this.DropShadow)
						{
							fontBatch2D.QueueText(text, zero + 1f * new Vector2(this.FontScale), 0f, new Color(0, 0, 0, (int)color.A), anchor, new Vector2(this.FontScale), this.FontSpacing, angle);
						}
						fontBatch2D.QueueText(text, zero, 0f, color, anchor, new Vector2(this.FontScale), this.FontSpacing, angle);
					}
					num += num4;
				}
				fontBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x000CE6C4 File Offset: 0x000CC8C4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (!string.IsNullOrEmpty(this.Text) && this.Color.A > 0);
			if (this.TextOrientation == TextOrientation.Horizontal)
			{
				this.UpdateLines(parentAvailableSize.X, parentAvailableSize.Y);
				base.DesiredSize = new Vector2((this.Size.X < 0f) ? this.m_linesSize.Value.X : this.Size.X, (this.Size.Y < 0f) ? this.m_linesSize.Value.Y : this.Size.Y);
				return;
			}
			if (this.TextOrientation == TextOrientation.VerticalLeft)
			{
				this.UpdateLines(parentAvailableSize.Y, parentAvailableSize.X);
				base.DesiredSize = new Vector2((this.Size.X < 0f) ? this.m_linesSize.Value.Y : this.Size.X, (this.Size.Y < 0f) ? this.m_linesSize.Value.X : this.Size.Y);
			}
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x000CE800 File Offset: 0x000CCA00
		public float CalculateLineHeight()
		{
			return (this.Font.GlyphHeight + this.Font.Spacing.Y + this.FontSpacing.Y) * this.FontScale * this.Font.Scale;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x000CE840 File Offset: 0x000CCA40
		public void UpdateLines(float availableWidth, float availableHeight)
		{
			if (this.m_linesAvailableHeight != null)
			{
				float? linesAvailableHeight = this.m_linesAvailableHeight;
				if ((linesAvailableHeight.GetValueOrDefault() == availableHeight & linesAvailableHeight != null) && this.m_linesAvailableWidth != null && this.m_linesSize != null)
				{
					float num = MathUtils.Min(this.m_linesSize.Value.X, this.m_linesAvailableWidth.Value) - 0.1f;
					float num2 = MathUtils.Max(this.m_linesSize.Value.X, this.m_linesAvailableWidth.Value) + 0.1f;
					if (availableWidth >= num && availableWidth <= num2)
					{
						return;
					}
				}
			}
			availableWidth += 0.1f;
			this.m_lines.Clear();
			string[] array = (this.Text ?? string.Empty).Split(new string[]
			{
				"\n"
			}, StringSplitOptions.None);
			string text = "...";
			float x = this.Font.MeasureText(text, new Vector2(this.FontScale), this.FontSpacing).X;
			if (this.WordWrap)
			{
				int num3 = (int)MathUtils.Min(MathUtils.Floor(availableHeight / this.CalculateLineHeight()), (float)this.MaxLines);
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i].TrimEnd(Array.Empty<char>());
					if (text2.Length == 0)
					{
						this.m_lines.Add(string.Empty);
					}
					else
					{
						while (text2.Length > 0)
						{
							int num4;
							bool flag;
							if (this.Ellipsis && this.m_lines.Count + 1 >= num3)
							{
								num4 = this.Font.FitText(MathUtils.Max(availableWidth - x, 0f), text2, 0, text2.Length, this.FontScale, this.FontSpacing.X);
								flag = true;
							}
							else
							{
								num4 = this.Font.FitText(availableWidth, text2, 0, text2.Length, this.FontScale, this.FontSpacing.X);
								num4 = MathUtils.Max(num4, 1);
								flag = false;
								if (num4 < text2.Length)
								{
									int num5 = num4;
									int num6 = num5 - 2;
									while (num6 >= 0 && !char.IsWhiteSpace(text2[num6]) && !char.IsPunctuation(text2[num6]))
									{
										num6--;
									}
									if (num6 < 0)
									{
										num6 = num5 - 1;
									}
									num4 = num6 + 1;
								}
							}
							string text3;
							if (num4 == text2.Length)
							{
								text3 = text2;
								text2 = string.Empty;
							}
							else
							{
								text3 = text2.Substring(0, num4).TrimEnd(Array.Empty<char>());
								if (flag)
								{
									text3 += text;
								}
								text2 = text2.Substring(num4, text2.Length - num4).TrimStart(Array.Empty<char>());
							}
							this.m_lines.Add(text3);
							if (flag && this.m_lines.Count > this.MaxLines)
							{
								this.m_lines = this.m_lines.Take(this.MaxLines).ToList<string>();
							}
						}
					}
				}
			}
			else if (this.Ellipsis)
			{
				for (int j = 0; j < array.Length; j++)
				{
					string text4 = array[j].TrimEnd(Array.Empty<char>());
					int num7 = this.Font.FitText(MathUtils.Max(availableWidth - x, 0f), text4, 0, text4.Length, this.FontScale, this.FontSpacing.X);
					if (num7 < text4.Length)
					{
						this.m_lines.Add(text4.Substring(0, num7).TrimEnd(Array.Empty<char>()) + text);
					}
					else
					{
						this.m_lines.Add(text4);
					}
				}
			}
			else
			{
				this.m_lines.AddRange(array);
			}
			if (this.m_lines.Count > this.MaxLines)
			{
				this.m_lines = this.m_lines.Take(this.MaxLines).ToList<string>();
			}
			Vector2 zero = Vector2.Zero;
			for (int k = 0; k < this.m_lines.Count; k++)
			{
				Vector2 vector = this.Font.MeasureText(this.m_lines[k], new Vector2(this.FontScale), this.FontSpacing);
				zero.X = MathUtils.Max(zero.X, vector.X);
				if (k < this.m_lines.Count - 1)
				{
					zero.Y += (this.Font.GlyphHeight + this.Font.Spacing.Y + this.FontSpacing.Y) * this.FontScale * this.Font.Scale;
				}
				else
				{
					zero.Y += this.Font.GlyphHeight * this.FontScale * this.Font.Scale;
				}
			}
			this.m_linesSize = new Vector2?(zero);
			this.m_linesAvailableWidth = new float?(availableWidth);
			this.m_linesAvailableHeight = new float?(availableHeight);
		}

		// Token: 0x0400123F RID: 4671
		public string m_text;

		// Token: 0x04001240 RID: 4672
		public TextOrientation m_textOrientation;

		// Token: 0x04001241 RID: 4673
		public BitmapFont m_font;

		// Token: 0x04001242 RID: 4674
		public Vector2 m_fontSpacing;

		// Token: 0x04001243 RID: 4675
		public float m_fontScale;

		// Token: 0x04001244 RID: 4676
		public int m_maxLines = int.MaxValue;

		// Token: 0x04001245 RID: 4677
		public bool m_wordWrap;

		// Token: 0x04001246 RID: 4678
		public bool m_ellipsis;

		// Token: 0x04001247 RID: 4679
		public List<string> m_lines = new List<string>();

		// Token: 0x04001248 RID: 4680
		public Vector2? m_linesSize;

		// Token: 0x04001249 RID: 4681
		public float? m_linesAvailableWidth;

		// Token: 0x0400124A RID: 4682
		public float? m_linesAvailableHeight;
	}
}
