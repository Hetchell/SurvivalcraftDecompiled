using System;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x020003A7 RID: 935
	public class WidgetInput
	{
		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x000D8059 File Offset: 0x000D6259
		// (set) Token: 0x06001BDA RID: 7130 RVA: 0x000D8061 File Offset: 0x000D6261
		public bool Any { get; set; }

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001BDB RID: 7131 RVA: 0x000D806A File Offset: 0x000D626A
		// (set) Token: 0x06001BDC RID: 7132 RVA: 0x000D8072 File Offset: 0x000D6272
		public bool Ok { get; set; }

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06001BDD RID: 7133 RVA: 0x000D807B File Offset: 0x000D627B
		// (set) Token: 0x06001BDE RID: 7134 RVA: 0x000D8083 File Offset: 0x000D6283
		public bool Cancel { get; set; }

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06001BDF RID: 7135 RVA: 0x000D808C File Offset: 0x000D628C
		// (set) Token: 0x06001BE0 RID: 7136 RVA: 0x000D8094 File Offset: 0x000D6294
		public bool Back { get; set; }

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x000D809D File Offset: 0x000D629D
		// (set) Token: 0x06001BE2 RID: 7138 RVA: 0x000D80A5 File Offset: 0x000D62A5
		public bool Left { get; set; }

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06001BE3 RID: 7139 RVA: 0x000D80AE File Offset: 0x000D62AE
		// (set) Token: 0x06001BE4 RID: 7140 RVA: 0x000D80B6 File Offset: 0x000D62B6
		public bool Right { get; set; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06001BE5 RID: 7141 RVA: 0x000D80BF File Offset: 0x000D62BF
		// (set) Token: 0x06001BE6 RID: 7142 RVA: 0x000D80C7 File Offset: 0x000D62C7
		public bool Up { get; set; }

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x000D80D0 File Offset: 0x000D62D0
		// (set) Token: 0x06001BE8 RID: 7144 RVA: 0x000D80D8 File Offset: 0x000D62D8
		public bool Down { get; set; }

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06001BE9 RID: 7145 RVA: 0x000D80E1 File Offset: 0x000D62E1
		// (set) Token: 0x06001BEA RID: 7146 RVA: 0x000D80E9 File Offset: 0x000D62E9
		public Vector2? Press { get; set; }

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06001BEB RID: 7147 RVA: 0x000D80F2 File Offset: 0x000D62F2
		// (set) Token: 0x06001BEC RID: 7148 RVA: 0x000D80FA File Offset: 0x000D62FA
		public Vector2? Tap { get; set; }

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001BED RID: 7149 RVA: 0x000D8103 File Offset: 0x000D6303
		// (set) Token: 0x06001BEE RID: 7150 RVA: 0x000D810B File Offset: 0x000D630B
		public Segment2? Click { get; set; }

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06001BEF RID: 7151 RVA: 0x000D8114 File Offset: 0x000D6314
		// (set) Token: 0x06001BF0 RID: 7152 RVA: 0x000D811C File Offset: 0x000D631C
		public Segment2? SpecialClick { get; set; }

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06001BF1 RID: 7153 RVA: 0x000D8125 File Offset: 0x000D6325
		// (set) Token: 0x06001BF2 RID: 7154 RVA: 0x000D812D File Offset: 0x000D632D
		public Vector2? Drag { get; set; }

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06001BF3 RID: 7155 RVA: 0x000D8136 File Offset: 0x000D6336
		// (set) Token: 0x06001BF4 RID: 7156 RVA: 0x000D813E File Offset: 0x000D633E
		public DragMode DragMode { get; set; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x000D8147 File Offset: 0x000D6347
		// (set) Token: 0x06001BF6 RID: 7158 RVA: 0x000D814F File Offset: 0x000D634F
		public Vector2? Hold { get; set; }

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x000D8158 File Offset: 0x000D6358
		// (set) Token: 0x06001BF8 RID: 7160 RVA: 0x000D8160 File Offset: 0x000D6360
		public float HoldTime { get; set; }

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x000D8169 File Offset: 0x000D6369
		// (set) Token: 0x06001BFA RID: 7162 RVA: 0x000D8171 File Offset: 0x000D6371
		public Vector3? Scroll { get; set; }

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06001BFB RID: 7163 RVA: 0x000D817C File Offset: 0x000D637C
		public Key? LastKey
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
				{
					return null;
				}
				return Keyboard.LastKey;
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06001BFC RID: 7164 RVA: 0x000D81AC File Offset: 0x000D63AC
		public char? LastChar
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
				{
					return null;
				}
				return Keyboard.LastChar;
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x000D81DA File Offset: 0x000D63DA
		// (set) Token: 0x06001BFE RID: 7166 RVA: 0x000D81E2 File Offset: 0x000D63E2
		public bool UseSoftMouseCursor
		{
			get
			{
				return this.m_useSoftMouseCursor;
			}
			set
			{
				this.m_useSoftMouseCursor = value;
			}
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06001BFF RID: 7167 RVA: 0x000D81EB File Offset: 0x000D63EB
		// (set) Token: 0x06001C00 RID: 7168 RVA: 0x000D81FF File Offset: 0x000D63FF
		public bool IsMouseCursorVisible
		{
			get
			{
				return (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && this.m_isMouseCursorVisible;
			}
			set
			{
				this.m_isMouseCursorVisible = value;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x000D8208 File Offset: 0x000D6408
		// (set) Token: 0x06001C02 RID: 7170 RVA: 0x000D8278 File Offset: 0x000D6478
		public Vector2? MousePosition
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Mouse) == WidgetInputDevice.None)
				{
					return null;
				}
				if (this.m_useSoftMouseCursor)
				{
					return new Vector2?(this.m_softMouseCursorPosition);
				}
				if (Mouse.MousePosition == null)
				{
					return null;
				}
				return new Vector2?(new Vector2(Mouse.MousePosition.Value));
			}
			set
			{
				if ((this.Devices & WidgetInputDevice.Mouse) == WidgetInputDevice.None || value == null)
				{
					return;
				}
				if (this.m_useSoftMouseCursor)
				{
					Vector2 vector;
					Vector2 max;
					if (this.Widget != null)
					{
						vector = this.Widget.GlobalBounds.Min;
						max = this.Widget.GlobalBounds.Max;
					}
					else
					{
						vector = Vector2.Zero;
						max = new Vector2(Window.Size);
					}
					this.m_softMouseCursorPosition = new Vector2(MathUtils.Clamp(value.Value.X, vector.X, max.X - 1f), MathUtils.Clamp(value.Value.Y, vector.Y, max.Y - 1f));
					return;
				}
				Mouse.SetMousePosition((int)value.Value.X, (int)value.Value.Y);
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x000D8353 File Offset: 0x000D6553
		public Point2 MouseMovement
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					return Mouse.MouseMovement;
				}
				return Point2.Zero;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x000D8372 File Offset: 0x000D6572
		public int MouseWheelMovement
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					return Mouse.MouseWheelMovement;
				}
				return 0;
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06001C05 RID: 7173 RVA: 0x000D8390 File Offset: 0x000D6590
		// (set) Token: 0x06001C06 RID: 7174 RVA: 0x000D83F4 File Offset: 0x000D65F4
		public bool IsPadCursorVisible
		{
			get
			{
				return this.m_isPadCursorVisible && (((this.Devices & WidgetInputDevice.GamePad1) != WidgetInputDevice.None && GamePad.IsConnected(0)) || ((this.Devices & WidgetInputDevice.GamePad2) != WidgetInputDevice.None && GamePad.IsConnected(1)) || ((this.Devices & WidgetInputDevice.GamePad3) != WidgetInputDevice.None && GamePad.IsConnected(2)) || ((this.Devices & WidgetInputDevice.GamePad4) != WidgetInputDevice.None && GamePad.IsConnected(3)));
			}
			set
			{
				this.m_isPadCursorVisible = value;
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06001C07 RID: 7175 RVA: 0x000D83FD File Offset: 0x000D65FD
		// (set) Token: 0x06001C08 RID: 7176 RVA: 0x000D8408 File Offset: 0x000D6608
		public Vector2 PadCursorPosition
		{
			get
			{
				return this.m_padCursorPosition;
			}
			set
			{
				Vector2 vector;
				Vector2 max;
				if (this.Widget != null)
				{
					vector = this.Widget.GlobalBounds.Min;
					max = this.Widget.GlobalBounds.Max;
				}
				else
				{
					vector = Vector2.Zero;
					max = new Vector2(Window.Size);
				}
				value.X = MathUtils.Clamp(value.X, vector.X, max.X - 1f);
				value.Y = MathUtils.Clamp(value.Y, vector.Y, max.Y - 1f);
				this.m_padCursorPosition = value;
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001C09 RID: 7177 RVA: 0x000D84A2 File Offset: 0x000D66A2
		public ReadOnlyList<TouchLocation> TouchLocations
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
				{
					return Touch.TouchLocations;
				}
				return ReadOnlyList<TouchLocation>.Empty;
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001C0A RID: 7178 RVA: 0x000D84C1 File Offset: 0x000D66C1
		// (set) Token: 0x06001C0B RID: 7179 RVA: 0x000D84C9 File Offset: 0x000D66C9
		public Matrix? VrQuadMatrix { get; set; }

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06001C0C RID: 7180 RVA: 0x000D84D2 File Offset: 0x000D66D2
		// (set) Token: 0x06001C0D RID: 7181 RVA: 0x000D84F3 File Offset: 0x000D66F3
		public bool IsVrCursorVisible
		{
			get
			{
				return this.m_isVrCursorVisible && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None && VrManager.IsVrStarted;
			}
			set
			{
				this.m_isVrCursorVisible = value;
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06001C0E RID: 7182 RVA: 0x000D84FC File Offset: 0x000D66FC
		// (set) Token: 0x06001C0F RID: 7183 RVA: 0x000D8504 File Offset: 0x000D6704
		public Vector2? VrCursorPosition { get; set; }

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06001C10 RID: 7184 RVA: 0x000D850D File Offset: 0x000D670D
		public static WidgetInput EmptyInput { get; } = new WidgetInput(WidgetInputDevice.None);

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06001C11 RID: 7185 RVA: 0x000D8514 File Offset: 0x000D6714
		public Widget Widget
		{
			get
			{
				return this.m_widget;
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06001C12 RID: 7186 RVA: 0x000D851C File Offset: 0x000D671C
		// (set) Token: 0x06001C13 RID: 7187 RVA: 0x000D8524 File Offset: 0x000D6724
		public WidgetInputDevice Devices { get; set; }

		// Token: 0x06001C14 RID: 7188 RVA: 0x000D852D File Offset: 0x000D672D
		public bool IsKeyDown(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDown(key);
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x000D8549 File Offset: 0x000D6749
		public bool IsKeyDownOnce(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownOnce(key);
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x000D8565 File Offset: 0x000D6765
		public bool IsKeyDownRepeat(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownRepeat(key);
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x000D8584 File Offset: 0x000D6784
		public void EnterText(ContainerWidget parentWidget, string title, string text, int maxLength, Action<string> handler)
		{
			Keyboard.ShowKeyboard(title, string.Empty, text, false, delegate(string s)
			{
				if (s.Length > maxLength)
				{
					s = s.Substring(0, maxLength);
				}
				handler(s);
			}, delegate
			{
				handler(null);
			});
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x000D85CC File Offset: 0x000D67CC
		public bool IsMouseButtonDown(MouseButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDown(button);
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x000D85E8 File Offset: 0x000D67E8
		public bool IsMouseButtonDownOnce(MouseButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDownOnce(button);
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x000D8604 File Offset: 0x000D6804
		public Vector2 GetPadStickPosition(GamePadStick stick, float deadZone = 0f)
		{
			if (this.m_isCleared)
			{
				return Vector2.Zero;
			}
			Vector2 vector = Vector2.Zero;
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
				{
					vector += GamePad.GetStickPosition(i, stick, deadZone);
				}
			}
			if (vector.LengthSquared() <= 1f)
			{
				return vector;
			}
			return Vector2.Normalize(vector);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x000D8668 File Offset: 0x000D6868
		public float GetPadTriggerPosition(GamePadTrigger trigger, float deadZone = 0f)
		{
			if (this.m_isCleared)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
				{
					num += GamePad.GetTriggerPosition(i, trigger, deadZone);
				}
			}
			return MathUtils.Min(num, 1f);
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x000D86BC File Offset: 0x000D68BC
		public bool IsPadButtonDown(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDown(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x000D86FC File Offset: 0x000D68FC
		public bool IsPadButtonDownOnce(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownOnce(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x000D873C File Offset: 0x000D693C
		public bool IsPadButtonDownRepeat(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownRepeat(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000D877A File Offset: 0x000D697A
		public Vector2 GetVrStickPosition(VrController controller, float deadZone = 0f)
		{
			if (!this.m_isCleared && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None)
			{
				return VrManager.GetStickPosition(controller, deadZone);
			}
			return Vector2.Zero;
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x000D87A0 File Offset: 0x000D69A0
		public Vector2? GetVrTouchpadPosition(VrController controller, float deadZone = 0f)
		{
			if (!this.m_isCleared && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None)
			{
				return VrManager.GetTouchpadPosition(controller, deadZone);
			}
			return null;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x000D87D4 File Offset: 0x000D69D4
		public float GetVrTriggerPosition(VrController controller, float deadZone = 0f)
		{
			if (!this.m_isCleared && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None)
			{
				return VrManager.GetTriggerPosition(controller, deadZone);
			}
			return 0f;
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000D87F9 File Offset: 0x000D69F9
		public bool IsVrButtonDown(VrController controller, VrControllerButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None && VrManager.IsButtonDown(controller, button);
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x000D881A File Offset: 0x000D6A1A
		public bool IsVrButtonDownOnce(VrController controller, VrControllerButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None && VrManager.IsButtonDownOnce(controller, button);
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000D883B File Offset: 0x000D6A3B
		public WidgetInput(WidgetInputDevice devices = WidgetInputDevice.All)
		{
			this.Devices = devices;
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x000D8860 File Offset: 0x000D6A60
		public void Clear()
		{
			this.m_isCleared = true;
			this.m_mouseDownPoint = null;
			this.m_mouseDragInProgress = false;
			this.m_touchCleared = true;
			this.m_padDownPoint = null;
			this.m_padDragInProgress = false;
			this.m_vrDownPoint = null;
			this.m_vrDragInProgress = false;
			this.ClearInput();
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x000D88BC File Offset: 0x000D6ABC
		public void Update()
		{
			this.m_isCleared = false;
			this.ClearInput();
			if (Window.IsActive)
			{
				if ((this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None)
				{
					this.UpdateInputFromKeyboard();
				}
				if ((this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					this.UpdateInputFromMouse();
				}
				if ((this.Devices & WidgetInputDevice.Gamepads) != WidgetInputDevice.None)
				{
					this.UpdateInputFromGamepads();
				}
				if ((this.Devices & WidgetInputDevice.VrControllers) != WidgetInputDevice.None && VrManager.IsVrStarted)
				{
					this.UpdateInputFromVrControllers();
				}
				if ((this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
				{
					this.UpdateInputFromTouch();
				}
			}
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x000D893C File Offset: 0x000D6B3C
		public void Draw(Widget.DrawContext dc)
		{
			if (this.IsMouseCursorVisible && this.UseSoftMouseCursor && this.MousePosition != null)
			{
				Texture2D texture2D = this.m_mouseDragInProgress ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDrag") : ((this.m_mouseDownPoint == null) ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursor") : ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDown"));
				TexturedBatch2D texturedBatch2D = dc.CursorPrimitivesRenderer2D.TexturedBatch(texture2D, false, 0, null, null, null, null);
				Vector2 corner2;
				Vector2 corner = (corner2 = Vector2.Transform(this.MousePosition.Value, this.Widget.InvertedGlobalTransform)) + new Vector2((float)texture2D.Width, (float)texture2D.Height) * 0.8f;
				int count = texturedBatch2D.TriangleVertices.Count;
				texturedBatch2D.QueueQuad(corner2, corner, 0f, Vector2.Zero, Vector2.One, Color.White);
				texturedBatch2D.TransformTriangles(this.Widget.GlobalTransform, count, -1);
			}
			if (this.IsPadCursorVisible)
			{
				Texture2D texture2D2 = this.m_padDragInProgress ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDrag") : ((this.m_padDownPoint == null) ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursor") : ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDown"));
				TexturedBatch2D texturedBatch2D2 = dc.CursorPrimitivesRenderer2D.TexturedBatch(texture2D2, false, 0, null, null, null, null);
				Vector2 corner4;
				Vector2 corner3 = (corner4 = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform)) + new Vector2((float)texture2D2.Width, (float)texture2D2.Height) * 0.8f;
				int count2 = texturedBatch2D2.TriangleVertices.Count;
				texturedBatch2D2.QueueQuad(corner4, corner3, 0f, Vector2.Zero, Vector2.One, Color.White);
				texturedBatch2D2.TransformTriangles(this.Widget.GlobalTransform, count2, -1);
			}
			if (this.VrCursorPosition != null)
			{
				dc.CursorPrimitivesRenderer2D.FlatBatch(0, null, null, null).QueueDisc(this.VrCursorPosition.Value, new Vector2(10f, 10f), 0f, Color.White, 32, 0f, 6.2831855f);
			}
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x000D8B68 File Offset: 0x000D6D68
		public void ClearInput()
		{
			this.Any = false;
			this.Ok = false;
			this.Cancel = false;
			this.Back = false;
			this.Left = false;
			this.Right = false;
			this.Up = false;
			this.Down = false;
			this.Press = null;
			this.Tap = null;
			this.Click = null;
			this.SpecialClick = null;
			this.Drag = null;
			this.DragMode = DragMode.AllItems;
			this.Hold = null;
			this.HoldTime = 0f;
			this.Scroll = null;
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x000D8C28 File Offset: 0x000D6E28
		public void UpdateInputFromKeyboard()
		{
			if (this.LastKey != null)
			{
				Key? lastKey = this.LastKey;
				Key key = Key.Enter;
				if (!(lastKey.GetValueOrDefault() == key & lastKey != null))
				{
					this.Any = true;
				}
			}
			if (this.IsKeyDownOnce(Key.Escape))
			{
				this.Back = true;
				this.Cancel = true;
			}
			if (this.IsKeyDownRepeat(Key.F12))
			{
				this.Left = true;
			}
			if (this.IsKeyDownRepeat(Key.LeftArrow))
			{
				this.Right = true;
			}
			if (this.IsKeyDownRepeat(Key.RightArrow))
			{
				this.Up = true;
			}
			if (this.IsKeyDownRepeat(Key.UpArrow))
			{
				this.Down = true;
			}
			this.Back |= Keyboard.IsKeyDownOnce(Key.Back);
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x000D8CDC File Offset: 0x000D6EDC
		public void UpdateInputFromMouse()
		{
			if (this.IsMouseButtonDownOnce(MouseButton.Left))
			{
				this.Any = true;
			}
			if (this.IsMouseCursorVisible && this.MousePosition != null)
			{
				Vector2 value = this.MousePosition.Value;
				if (this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right))
				{
					this.Press = new Vector2?(value);
				}
				if (this.IsMouseButtonDownOnce(MouseButton.Left) || this.IsMouseButtonDownOnce(MouseButton.Right))
				{
					this.Tap = new Vector2?(value);
					this.m_mouseDownPoint = new Vector2?(value);
					this.m_mouseDownButton = ((!this.IsMouseButtonDownOnce(MouseButton.Left)) ? MouseButton.Right : MouseButton.Left);
					this.m_mouseDragTime = Time.FrameStartTime;
				}
				if (!this.IsMouseButtonDown(MouseButton.Left) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Left)
				{
					if (this.IsKeyDown(Key.Shift))
					{
						this.SpecialClick = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
					}
					else
					{
						this.Click = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
					}
				}
				if (!this.IsMouseButtonDown(MouseButton.Right) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Right)
				{
					this.SpecialClick = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
				}
				if (this.MouseWheelMovement != 0)
				{
					this.Scroll = new Vector3?(new Vector3(value, (float)this.MouseWheelMovement / 120f));
				}
				if (this.m_mouseHoldInProgress && this.m_mouseDownPoint != null)
				{
					this.Hold = new Vector2?(this.m_mouseDownPoint.Value);
					this.HoldTime = (float)(Time.FrameStartTime - this.m_mouseDragTime);
				}
				if (this.m_mouseDragInProgress)
				{
					this.Drag = new Vector2?(value);
				}
				else if ((this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right)) && this.m_mouseDownPoint != null)
				{
					if (Vector2.Distance(this.m_mouseDownPoint.Value, value) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
					{
						this.m_mouseDragInProgress = true;
						this.DragMode = ((!this.IsMouseButtonDown(MouseButton.Left)) ? DragMode.SingleItem : DragMode.AllItems);
						this.Drag = new Vector2?(this.m_mouseDownPoint.Value);
					}
					else if (Time.FrameStartTime - this.m_mouseDragTime > (double)SettingsManager.MinimumHoldDuration)
					{
						this.m_mouseHoldInProgress = true;
					}
				}
			}
			if (!this.IsMouseButtonDown(MouseButton.Left) && !this.IsMouseButtonDown(MouseButton.Right))
			{
				this.m_mouseDragInProgress = false;
				this.m_mouseHoldInProgress = false;
				this.m_mouseDownPoint = null;
			}
			if (this.m_useSoftMouseCursor && this.IsMouseCursorVisible)
			{
				this.MousePosition = new Vector2?((this.MousePosition ?? Vector2.Zero) + new Vector2(this.MouseMovement));
			}
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x000D8FAC File Offset: 0x000D71AC
		public void UpdateInputFromGamepads()
		{
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadLeft))
			{
				this.Left = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadRight))
			{
				this.Right = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
			{
				this.Up = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
			{
				this.Down = true;
			}
			if (this.IsPadCursorVisible)
			{
				if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
				{
					this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, 1f));
				}
				if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
				{
					this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, -1f));
				}
				if (this.IsPadButtonDown(GamePadButton.A))
				{
					this.Press = new Vector2?(this.PadCursorPosition);
				}
				if (this.IsPadButtonDownOnce(GamePadButton.A))
				{
					this.Ok = true;
					this.Tap = new Vector2?(this.PadCursorPosition);
					this.m_padDownPoint = new Vector2?(this.PadCursorPosition);
					this.m_padDragTime = Time.FrameStartTime;
				}
				if (!this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
				{
					if (this.GetPadTriggerPosition(GamePadTrigger.Left, 0f) > 0.5f)
					{
						this.SpecialClick = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
					}
					else
					{
						this.Click = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
					}
				}
			}
			if (this.IsPadButtonDownOnce(GamePadButton.A) || this.IsPadButtonDownOnce(GamePadButton.B) || this.IsPadButtonDownOnce(GamePadButton.X) || this.IsPadButtonDownOnce(GamePadButton.Y))
			{
				this.Any = true;
			}
			if (!this.IsPadButtonDown(GamePadButton.A))
			{
				this.m_padDragInProgress = false;
				this.m_padDownPoint = null;
			}
			if (this.IsPadButtonDownOnce(GamePadButton.B))
			{
				this.Cancel = true;
			}
			if (this.IsPadButtonDownOnce(GamePadButton.Back))
			{
				this.Back = true;
			}
			if (this.m_padDragInProgress)
			{
				this.Drag = new Vector2?(this.PadCursorPosition);
			}
			else if (this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
			{
				if (Vector2.Distance(this.m_padDownPoint.Value, this.PadCursorPosition) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
				{
					this.m_padDragInProgress = true;
					this.Drag = new Vector2?(this.m_padDownPoint.Value);
					this.DragMode = DragMode.AllItems;
				}
				else if (Time.FrameStartTime - this.m_padDragTime > (double)SettingsManager.MinimumHoldDuration)
				{
					this.Hold = new Vector2?(this.m_padDownPoint.Value);
					this.HoldTime = (float)(Time.FrameStartTime - this.m_padDragTime);
				}
			}
			if (this.IsPadCursorVisible)
			{
				Vector2 vector = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform);
				Vector2 padStickPosition = this.GetPadStickPosition(GamePadStick.Left, SettingsManager.GamepadDeadZone);
				Vector2 vector2 = new Vector2(padStickPosition.X, 0f - padStickPosition.Y);
				vector2 = 1200f * SettingsManager.GamepadCursorSpeed * vector2.LengthSquared() * Vector2.Normalize(vector2) * Time.FrameDuration;
				vector += vector2;
				this.PadCursorPosition = Vector2.Transform(vector, this.Widget.GlobalTransform);
			}
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x000D92D4 File Offset: 0x000D74D4
		public void UpdateInputFromTouch()
		{
			foreach (TouchLocation touchLocation in this.TouchLocations)
			{
				if (touchLocation.State == TouchLocationState.Pressed)
				{
					if (this.Widget.HitTest(touchLocation.Position))
					{
						this.Any = true;
						this.Tap = new Vector2?(touchLocation.Position);
						this.Press = new Vector2?(touchLocation.Position);
						this.m_touchStartPoint = touchLocation.Position;
						this.m_touchId = new int?(touchLocation.Id);
						this.m_touchCleared = false;
						this.m_touchStartTime = Time.FrameStartTime;
						this.m_touchDragInProgress = false;
						this.m_touchHoldInProgress = false;
					}
				}
				else if (touchLocation.State == TouchLocationState.Moved)
				{
					int? touchId = this.m_touchId;
					int id = touchLocation.Id;
					if (touchId.GetValueOrDefault() == id & touchId != null)
					{
						this.Press = new Vector2?(touchLocation.Position);
						if (!this.m_touchCleared)
						{
							if (this.m_touchDragInProgress)
							{
								this.Drag = new Vector2?(touchLocation.Position);
							}
							else if (Vector2.Distance(touchLocation.Position, this.m_touchStartPoint) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
							{
								this.m_touchDragInProgress = true;
								this.Drag = new Vector2?(this.m_touchStartPoint);
							}
							if (!this.m_touchDragInProgress)
							{
								if (this.m_touchHoldInProgress)
								{
									this.Hold = new Vector2?(this.m_touchStartPoint);
									this.HoldTime = (float)(Time.FrameStartTime - this.m_touchStartTime);
								}
								else if (Time.FrameStartTime - this.m_touchStartTime > (double)SettingsManager.MinimumHoldDuration)
								{
									this.m_touchHoldInProgress = true;
								}
							}
						}
					}
				}
				else if (touchLocation.State == TouchLocationState.Released)
				{
					int? touchId = this.m_touchId;
					int id = touchLocation.Id;
					if (touchId.GetValueOrDefault() == id & touchId != null)
					{
						if (!this.m_touchCleared)
						{
							this.Click = new Segment2?(new Segment2(this.m_touchStartPoint, touchLocation.Position));
						}
						this.m_touchId = null;
						this.m_touchCleared = false;
						this.m_touchDragInProgress = false;
						this.m_touchHoldInProgress = false;
					}
				}
			}
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000D9534 File Offset: 0x000D7734
		public void UpdateInputFromVrControllers()
		{
			this.VrCursorPosition = null;
			if (this.VrQuadMatrix != null)
			{
				Matrix value = this.VrQuadMatrix.Value;
				Matrix controllerMatrix = VrManager.GetControllerMatrix(VrController.Right);
				Plane plane = new Plane(value.Translation, value.Translation + value.Right, value.Translation + value.Up);
				Ray3 ray = new Ray3(controllerMatrix.Translation, controllerMatrix.Forward);
				float? num = ray.Intersection(plane);
				if (num != null)
				{
					Vector3 v = ray.Position + num.Value * ray.Direction - value.Translation;
					float x = Vector3.Dot(v, Vector3.Normalize(value.Right)) / value.Right.Length() * this.Widget.ActualSize.X;
					float y = (1f - Vector3.Dot(v, Vector3.Normalize(value.Up)) / value.Up.Length()) * this.Widget.ActualSize.Y;
					this.VrCursorPosition = new Vector2?(Vector2.Transform(new Vector2(x, y), this.Widget.GlobalTransform));
				}
			}
			if (this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadLeft))
			{
				this.Left = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadRight))
			{
				this.Right = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadUp))
			{
				this.Up = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadDown))
			{
				this.Down = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadLeft))
			{
				this.Left = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadRight))
			{
				this.Right = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadUp))
			{
				this.Up = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadDown))
			{
				this.Down = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Grip))
			{
				this.Back = true;
				this.Cancel = true;
			}
			if (this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.Touchpad) || this.IsVrButtonDownOnce(VrController.Left, VrControllerButton.Trigger) || this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Touchpad) || this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Trigger))
			{
				this.Any = true;
			}
			if (this.IsVrCursorVisible && this.VrCursorPosition != null)
			{
				if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadUp))
				{
					this.Scroll = new Vector3?(new Vector3(this.VrCursorPosition.Value, 1f));
				}
				if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadDown))
				{
					this.Scroll = new Vector3?(new Vector3(this.VrCursorPosition.Value, -1f));
				}
				if (this.IsVrButtonDown(VrController.Right, VrControllerButton.Trigger))
				{
					this.Press = new Vector2?(this.VrCursorPosition.Value);
				}
				if (this.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Trigger))
				{
					this.Ok = true;
					this.Tap = new Vector2?(this.VrCursorPosition.Value);
					this.m_vrDownPoint = new Vector2?(this.VrCursorPosition.Value);
					this.m_vrDragTime = Time.FrameStartTime;
				}
				if (!this.IsVrButtonDown(VrController.Right, VrControllerButton.Trigger) && this.m_vrDownPoint != null)
				{
					if (this.GetVrTriggerPosition(VrController.Left, 0f) > 0.5f)
					{
						this.SpecialClick = new Segment2?(new Segment2(this.m_vrDownPoint.Value, this.VrCursorPosition.Value));
					}
					else
					{
						this.Click = new Segment2?(new Segment2(this.m_vrDownPoint.Value, this.VrCursorPosition.Value));
					}
				}
			}
			if (!this.IsVrButtonDown(VrController.Right, VrControllerButton.Trigger))
			{
				this.m_vrDragInProgress = false;
				this.m_vrDownPoint = null;
			}
			if (this.m_vrDragInProgress && this.VrCursorPosition != null)
			{
				this.Drag = this.VrCursorPosition;
				return;
			}
			if (this.IsVrButtonDown(VrController.Right, VrControllerButton.Trigger) && this.m_vrDownPoint != null)
			{
				if (Vector2.Distance(this.m_vrDownPoint.Value, this.VrCursorPosition.Value) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
				{
					this.m_vrDragInProgress = true;
					this.Drag = new Vector2?(this.m_vrDownPoint.Value);
					this.DragMode = DragMode.AllItems;
					return;
				}
				if (Time.FrameStartTime - this.m_vrDragTime > (double)SettingsManager.MinimumHoldDuration)
				{
					this.Hold = new Vector2?(this.m_vrDownPoint.Value);
					this.HoldTime = (float)(Time.FrameStartTime - this.m_vrDragTime);
				}
			}
		}

		// Token: 0x0400134C RID: 4940
		public bool m_isCleared;

		// Token: 0x0400134D RID: 4941
		public Widget m_widget;

		// Token: 0x0400134E RID: 4942
		public Vector2 m_softMouseCursorPosition;

		// Token: 0x0400134F RID: 4943
		public Vector2? m_mouseDownPoint;

		// Token: 0x04001350 RID: 4944
		public MouseButton m_mouseDownButton;

		// Token: 0x04001351 RID: 4945
		public double m_mouseDragTime;

		// Token: 0x04001352 RID: 4946
		public bool m_mouseDragInProgress;

		// Token: 0x04001353 RID: 4947
		public bool m_mouseHoldInProgress;

		// Token: 0x04001354 RID: 4948
		public bool m_isMouseCursorVisible = true;

		// Token: 0x04001355 RID: 4949
		public bool m_useSoftMouseCursor;

		// Token: 0x04001356 RID: 4950
		public int? m_touchId;

		// Token: 0x04001357 RID: 4951
		public bool m_touchCleared;

		// Token: 0x04001358 RID: 4952
		public Vector2 m_touchStartPoint;

		// Token: 0x04001359 RID: 4953
		public double m_touchStartTime;

		// Token: 0x0400135A RID: 4954
		public bool m_touchDragInProgress;

		// Token: 0x0400135B RID: 4955
		public bool m_touchHoldInProgress;

		// Token: 0x0400135C RID: 4956
		public Vector2 m_padCursorPosition;

		// Token: 0x0400135D RID: 4957
		public Vector2? m_padDownPoint;

		// Token: 0x0400135E RID: 4958
		public double m_padDragTime;

		// Token: 0x0400135F RID: 4959
		public bool m_padDragInProgress;

		// Token: 0x04001360 RID: 4960
		public bool m_isPadCursorVisible = true;

		// Token: 0x04001361 RID: 4961
		public Vector2? m_vrDownPoint;

		// Token: 0x04001362 RID: 4962
		public double m_vrDragTime;

		// Token: 0x04001363 RID: 4963
		public bool m_vrDragInProgress;

		// Token: 0x04001364 RID: 4964
		public bool m_isVrCursorVisible = true;
	}
}
