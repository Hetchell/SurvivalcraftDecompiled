using System;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;

namespace Game
{
	// Token: 0x0200039F RID: 927
	public class TextBoxWidget : Widget
	{
		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001B28 RID: 6952 RVA: 0x000D46F2 File Offset: 0x000D28F2
		// (set) Token: 0x06001B29 RID: 6953 RVA: 0x000D4712 File Offset: 0x000D2912
		public Vector2 Size
		{
			get
			{
				if (this.m_size == null)
				{
					return Vector2.Zero;
				}
				return this.m_size.Value;
			}
			set
			{
				this.m_size = new Vector2?(value);
			}
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001B2A RID: 6954 RVA: 0x000D4720 File Offset: 0x000D2920
		// (set) Token: 0x06001B2B RID: 6955 RVA: 0x000D4728 File Offset: 0x000D2928
		public string Title { get; set; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06001B2C RID: 6956 RVA: 0x000D4731 File Offset: 0x000D2931
		// (set) Token: 0x06001B2D RID: 6957 RVA: 0x000D4739 File Offset: 0x000D2939
		public string Description { get; set; }

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001B2E RID: 6958 RVA: 0x000D4742 File Offset: 0x000D2942
		// (set) Token: 0x06001B2F RID: 6959 RVA: 0x000D474C File Offset: 0x000D294C
		public string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				string text = (value.Length > this.MaximumLength) ? value.Substring(0, this.MaximumLength) : value;
				if (text != this.m_text)
				{
					this.m_text = text;
					this.CaretPosition = this.CaretPosition;
					Action<TextBoxWidget> textChanged = this.TextChanged;
					if (textChanged == null)
					{
						return;
					}
					textChanged(this);
				}
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06001B30 RID: 6960 RVA: 0x000D47AA File Offset: 0x000D29AA
		// (set) Token: 0x06001B31 RID: 6961 RVA: 0x000D47B2 File Offset: 0x000D29B2
		public int MaximumLength
		{
			get
			{
				return this.m_maximumLength;
			}
			set
			{
				this.m_maximumLength = MathUtils.Max(value, 0);
				if (this.Text.Length > this.m_maximumLength)
				{
					this.Text = this.Text.Substring(0, this.m_maximumLength);
				}
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001B32 RID: 6962 RVA: 0x000D47EC File Offset: 0x000D29EC
		// (set) Token: 0x06001B33 RID: 6963 RVA: 0x000D47F4 File Offset: 0x000D29F4
		public bool OverwriteMode { get; set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001B34 RID: 6964 RVA: 0x000D47FD File Offset: 0x000D29FD
		// (set) Token: 0x06001B35 RID: 6965 RVA: 0x000D4808 File Offset: 0x000D2A08
		public bool HasFocus
		{
			get
			{
				return this.m_hasFocus;
			}
			set
			{
				if (value != this.m_hasFocus)
				{
					this.m_hasFocus = value;
					if (value)
					{
						this.CaretPosition = this.m_text.Length;
						Keyboard.ShowKeyboard(this.Title, this.Description, this.Text, false, delegate(string text)
						{
							this.Text = text;
						}, null);
						return;
					}
					Action<TextBoxWidget> focusLost = this.FocusLost;
					if (focusLost == null)
					{
						return;
					}
					focusLost(this);
				}
			}
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001B36 RID: 6966 RVA: 0x000D4870 File Offset: 0x000D2A70
		// (set) Token: 0x06001B37 RID: 6967 RVA: 0x000D4878 File Offset: 0x000D2A78
		public BitmapFont Font
		{
			get
			{
				return this.m_font;
			}
			set
			{
				this.m_font = value;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001B38 RID: 6968 RVA: 0x000D4881 File Offset: 0x000D2A81
		// (set) Token: 0x06001B39 RID: 6969 RVA: 0x000D4889 File Offset: 0x000D2A89
		public float FontScale { get; set; }

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06001B3A RID: 6970 RVA: 0x000D4892 File Offset: 0x000D2A92
		// (set) Token: 0x06001B3B RID: 6971 RVA: 0x000D489A File Offset: 0x000D2A9A
		public Vector2 FontSpacing { get; set; }

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06001B3C RID: 6972 RVA: 0x000D48A3 File Offset: 0x000D2AA3
		// (set) Token: 0x06001B3D RID: 6973 RVA: 0x000D48AB File Offset: 0x000D2AAB
		public Color Color { get; set; }

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001B3E RID: 6974 RVA: 0x000D48B4 File Offset: 0x000D2AB4
		// (set) Token: 0x06001B3F RID: 6975 RVA: 0x000D48BC File Offset: 0x000D2ABC
		public bool TextureLinearFilter { get; set; }

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001B40 RID: 6976 RVA: 0x000D48C5 File Offset: 0x000D2AC5
		// (set) Token: 0x06001B41 RID: 6977 RVA: 0x000D48CD File Offset: 0x000D2ACD
		public int CaretPosition
		{
			get
			{
				return this.m_caretPosition;
			}
			set
			{
				this.m_caretPosition = MathUtils.Clamp(value, 0, this.Text.Length);
				this.m_focusStartTime = Time.RealTime;
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001B42 RID: 6978 RVA: 0x000D48F4 File Offset: 0x000D2AF4
		// (remove) Token: 0x06001B43 RID: 6979 RVA: 0x000D492C File Offset: 0x000D2B2C
		public event Action<TextBoxWidget> TextChanged;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001B44 RID: 6980 RVA: 0x000D4964 File Offset: 0x000D2B64
		// (remove) Token: 0x06001B45 RID: 6981 RVA: 0x000D499C File Offset: 0x000D2B9C
		public event Action<TextBoxWidget> Enter;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001B46 RID: 6982 RVA: 0x000D49D4 File Offset: 0x000D2BD4
		// (remove) Token: 0x06001B47 RID: 6983 RVA: 0x000D4A0C File Offset: 0x000D2C0C
		public event Action<TextBoxWidget> Escape;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001B48 RID: 6984 RVA: 0x000D4A44 File Offset: 0x000D2C44
		// (remove) Token: 0x06001B49 RID: 6985 RVA: 0x000D4A7C File Offset: 0x000D2C7C
		public event Action<TextBoxWidget> FocusLost;

		// Token: 0x06001B4A RID: 6986 RVA: 0x000D4AB4 File Offset: 0x000D2CB4
		public TextBoxWidget()
		{
			base.ClampToBounds = true;
			this.Color = Color.White;
			this.TextureLinearFilter = true;
			this.Font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			this.FontScale = 1f;
			this.Title = string.Empty;
			this.Description = string.Empty;
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x000D4B24 File Offset: 0x000D2D24
		public override void Update()
		{
			if (this.m_hasFocus)
			{
				if (base.Input.LastChar != null && !base.Input.IsKeyDown(Key.Control) && !char.IsControl(base.Input.LastChar.Value))
				{
					this.EnterText(new string(base.Input.LastChar.Value, 1));
					base.Input.Clear();
				}
				if (base.Input.LastKey != null)
				{
					bool flag = false;
					Key value = base.Input.LastKey.Value;
					if (value == Key.U && base.Input.IsKeyDown(Key.Control))
					{
						this.EnterText(ClipboardManager.ClipboardString);
						flag = true;
					}
					else if (value == Key.Tab && this.CaretPosition > 0)
					{
						int caretPosition = this.CaretPosition;
						this.CaretPosition = caretPosition - 1;
						this.Text = this.Text.Remove(this.CaretPosition, 1);
						flag = true;
					}
					else
					{
						switch (value)
						{
						case Key.F12:
						{
							int caretPosition = this.CaretPosition;
							this.CaretPosition = caretPosition - 1;
							flag = true;
							break;
						}
						case Key.LeftArrow:
						{
							int caretPosition = this.CaretPosition;
							this.CaretPosition = caretPosition + 1;
							flag = true;
							break;
						}
						case Key.RightArrow:
						case Key.UpArrow:
							break;
						case Key.DownArrow:
						{
							flag = true;
							this.HasFocus = false;
							Action<TextBoxWidget> enter = this.Enter;
							if (enter != null)
							{
								enter(this);
							}
							break;
						}
						case Key.Enter:
						{
							flag = true;
							this.HasFocus = false;
							Action<TextBoxWidget> escape = this.Escape;
							if (escape != null)
							{
								escape(this);
							}
							break;
						}
						default:
							switch (value)
							{
							case Key.Insert:
								if (this.CaretPosition < this.m_text.Length)
								{
									this.Text = this.Text.Remove(this.CaretPosition, 1);
									flag = true;
								}
								break;
							case Key.PageDown:
								this.CaretPosition = 0;
								flag = true;
								break;
							case Key.Home:
								this.CaretPosition = this.m_text.Length;
								flag = true;
								break;
							}
							break;
						}
					}
					if (flag)
					{
						base.Input.Clear();
					}
				}
			}
			if (base.Input.Click != null)
			{
				this.HasFocus = (base.HitTestGlobal(base.Input.Click.Value.Start, null) == this && base.HitTestGlobal(base.Input.Click.Value.End, null) == this);
			}
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x000D4DA4 File Offset: 0x000D2FA4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			if (this.m_size != null)
			{
				base.DesiredSize = this.m_size.Value;
				return;
			}
			if (this.Text.Length == 0)
			{
				base.DesiredSize = this.Font.MeasureText(" ", new Vector2(this.FontScale), this.FontSpacing);
			}
			else
			{
				base.DesiredSize = this.Font.MeasureText(this.Text, new Vector2(this.FontScale), this.FontSpacing);
			}
			base.DesiredSize += new Vector2(1f * this.FontScale * this.Font.Scale, 0f);
		}

		// Token: 0x06001B4D RID: 6989 RVA: 0x000D4E68 File Offset: 0x000D3068
		public override void Draw(Widget.DrawContext dc)
		{
			Color color = this.Color * base.GlobalColorTransform;
			if (!string.IsNullOrEmpty(this.m_text))
			{
				Vector2 position = new Vector2(0f - this.m_scroll, base.ActualSize.Y / 2f);
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp;
				FontBatch2D fontBatch2D = dc.PrimitivesRenderer2D.FontBatch(this.Font, 1, DepthStencilState.None, null, null, samplerState);
				int count = fontBatch2D.TriangleVertices.Count;
				fontBatch2D.QueueText(this.Text, position, 0f, color, TextAnchor.VerticalCenter, new Vector2(this.FontScale), this.FontSpacing, 0f);
				fontBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
			if (!this.m_hasFocus || MathUtils.Remainder(Time.RealTime - this.m_focusStartTime, 0.5) >= 0.25)
			{
				return;
			}
			float num = this.Font.CalculateCharacterPosition(this.Text, this.CaretPosition, new Vector2(this.FontScale), this.FontSpacing);
			Vector2 vector = new Vector2(0f, base.ActualSize.Y / 2f) + new Vector2(num - this.m_scroll, 0f);
			if (this.m_hasFocus)
			{
				if (vector.X < 0f)
				{
					this.m_scroll = MathUtils.Max(this.m_scroll + vector.X, 0f);
				}
				if (vector.X > base.ActualSize.X)
				{
					this.m_scroll += vector.X - base.ActualSize.X + 1f;
				}
			}
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, null);
			int count2 = flatBatch2D.TriangleVertices.Count;
			flatBatch2D.QueueQuad(vector - new Vector2(0f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), vector + new Vector2(1f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), 0f, color);
			flatBatch2D.TransformTriangles(base.GlobalTransform, count2, -1);
		}

		// Token: 0x06001B4E RID: 6990 RVA: 0x000D50C8 File Offset: 0x000D32C8
		public void EnterText(string s)
		{
			if (this.OverwriteMode)
			{
				if (this.CaretPosition + s.Length <= this.MaximumLength)
				{
					if (this.CaretPosition < this.m_text.Length)
					{
						string text = this.Text;
						text = text.Remove(this.CaretPosition, s.Length);
						string text2 = this.Text = text.Insert(this.CaretPosition, s);
					}
					else
					{
						this.Text = this.m_text + s;
					}
					this.CaretPosition += s.Length;
					return;
				}
			}
			else if (this.m_text.Length + s.Length <= this.MaximumLength)
			{
				if (this.CaretPosition < this.m_text.Length)
				{
					this.Text = this.Text.Insert(this.CaretPosition, s);
				}
				else
				{
					this.Text = this.m_text + s;
				}
				this.CaretPosition += s.Length;
			}
		}

		// Token: 0x040012DA RID: 4826
		public BitmapFont m_font;

		// Token: 0x040012DB RID: 4827
		public string m_text = string.Empty;

		// Token: 0x040012DC RID: 4828
		public int m_maximumLength = 32;

		// Token: 0x040012DD RID: 4829
		public bool m_hasFocus;

		// Token: 0x040012DE RID: 4830
		public int m_caretPosition;

		// Token: 0x040012DF RID: 4831
		public double m_focusStartTime;

		// Token: 0x040012E0 RID: 4832
		public float m_scroll;

		// Token: 0x040012E1 RID: 4833
		public Vector2? m_size;
	}
}
