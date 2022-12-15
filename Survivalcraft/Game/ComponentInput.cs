using System;
using System.Diagnostics;
using Engine;
using Engine.Input;
using GameEntitySystem;
using Survivalcraft.Game;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E9 RID: 489
	public class ComponentInput : Component, IUpdateable
	{

		public static int speed = 1;
		public static Boolean state = false;
		public static float step = 0;
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x00069BE9 File Offset: 0x00067DE9
		public PlayerInput PlayerInput
		{
			get
			{
				return this.m_playerInput;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x00069BF1 File Offset: 0x00067DF1
		// (set) Token: 0x06000DC5 RID: 3525 RVA: 0x00069BF9 File Offset: 0x00067DF9
		public bool IsControlledByTouch { get; set; } = true;

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x00069C02 File Offset: 0x00067E02
		public bool IsControlledByVr
		{
			get
			{
				return VrManager.IsVrStarted && (this.m_componentPlayer.GameWidget.Input.Devices & WidgetInputDevice.VrControllers) > WidgetInputDevice.None;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x00069C2B File Offset: 0x00067E2B
		// (set) Token: 0x06000DC8 RID: 3528 RVA: 0x00069C33 File Offset: 0x00067E33
		public IInventory SplitSourceInventory { get; set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x00069C3C File Offset: 0x00067E3C
		// (set) Token: 0x06000DCA RID: 3530 RVA: 0x00069C44 File Offset: 0x00067E44
		public int SplitSourceSlotIndex { get; set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000DCB RID: 3531 RVA: 0x00069C4D File Offset: 0x00067E4D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Input;
			}
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x00069C51 File Offset: 0x00067E51
		public void SetSplitSourceInventoryAndSlot(IInventory inventory, int slotIndex)
		{
			this.SplitSourceInventory = inventory;
			this.SplitSourceSlotIndex = slotIndex;
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x00069C64 File Offset: 0x00067E64
		public Ray3? CalculateVrHandRay()
		{
			if (VrManager.IsControllerPresent(VrController.Right))
			{
				Camera activeCamera = this.m_componentPlayer.GameWidget.ActiveCamera;
				Matrix m = VrManager.HmdMatrixInverted * Matrix.CreateWorld(activeCamera.ViewPosition, activeCamera.ViewDirection, activeCamera.ViewUp);
				Matrix matrix = VrManager.GetControllerMatrix(VrController.Right) * m;
				return new Ray3?(new Ray3(matrix.Translation + matrix.Forward * 0.078125f, matrix.Forward));
			}
			return null;
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x00069CF4 File Offset: 0x00067EF4
		public void Update(float dt)
		{
			this.m_playerInput = default(PlayerInput);
			this.UpdateInputFromMouseAndKeyboard(this.m_componentPlayer.GameWidget.Input);
			this.UpdateInputFromGamepad(this.m_componentPlayer.GameWidget.Input);
			this.UpdateInputFromVrControllers(this.m_componentPlayer.GameWidget.Input);
			this.UpdateInputFromWidgets(this.m_componentPlayer.GameWidget.Input);
			if (this.m_playerInput.Jump)
			{
				if (Time.RealTime - this.m_lastJumpTime < 0.3)
				{
					this.m_playerInput.ToggleCreativeFly = true;
					this.m_lastJumpTime = 0.0;
				}
				else
				{
					this.m_lastJumpTime = Time.RealTime;
				}
			}
			this.m_playerInput.CameraMove = this.m_playerInput.Move;
			this.m_playerInput.CameraSneakMove = this.m_playerInput.SneakMove;
			this.m_playerInput.CameraLook = this.m_playerInput.Look;
			if (!Window.IsActive || !this.m_componentPlayer.PlayerData.IsReadyForPlaying)
			{
				this.m_playerInput = default(PlayerInput);
			}
			else if (this.m_componentPlayer.ComponentHealth.Health <= 0f || this.m_componentPlayer.ComponentSleep.SleepFactor > 0f || !this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled)
			{
				this.m_playerInput = new PlayerInput
				{
					CameraMove = this.m_playerInput.CameraMove,
					CameraSneakMove = this.m_playerInput.CameraSneakMove,
					CameraLook = this.m_playerInput.CameraLook,
					TimeOfDay = this.m_playerInput.TimeOfDay,
					TakeScreenshot = this.m_playerInput.TakeScreenshot,
					KeyboardHelp = this.m_playerInput.KeyboardHelp
				};
			}
			else if (this.m_componentPlayer.GameWidget.ActiveCamera.UsesMovementControls)
			{
				this.m_playerInput.Move = Vector3.Zero;
				this.m_playerInput.SneakMove = Vector3.Zero;
				this.m_playerInput.Look = Vector2.Zero;
				this.m_playerInput.Jump = false;
				this.m_playerInput.ToggleSneak = false;
				this.m_playerInput.ToggleCreativeFly = false;
			}
			if (this.m_playerInput.Move.LengthSquared() > 1f)
			{
				//this.m_playerInput.Move = Vector3.Normalize(this.m_playerInput.Move);
			}
			if (this.m_playerInput.SneakMove.LengthSquared() > 1f)
			{
				this.m_playerInput.SneakMove = Vector3.Normalize(this.m_playerInput.SneakMove);
			}
			if (this.SplitSourceInventory != null && this.SplitSourceInventory.GetSlotCount(this.SplitSourceSlotIndex) == 0)
			{
				this.SetSplitSourceInventoryAndSlot(null, -1);
			}
		}

        // Token: 0x06000DCF RID: 3535 RVA: 0x00069FD1 File Offset: 0x000681D1
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x0006A00C File Offset: 0x0006820C
		public void UpdateInputFromMouseAndKeyboard(WidgetInput input)
		{
			//this.checker(input);
			//return;
			Vector3 viewPosition = this.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition;
			Vector3 viewDirection = this.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsMouseCursorVisible)
				{
					ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
					Vector2 value = viewWidget.WidgetToScreen(viewWidget.ActualSize / 2f);
					input.IsMouseCursorVisible = true;
					input.MousePosition = new Vector2?(value);
				}
			}
			else
			{
				input.IsMouseCursorVisible = false;
				Vector2 zero = Vector2.Zero;
				int num = 0;
				if (Window.IsActive && Time.FrameDuration > 0f)
				{
					Point2 mouseMovement = input.MouseMovement;
					int mouseWheelMovement = input.MouseWheelMovement;
					zero.X = 0.02f * (float)mouseMovement.X / Time.FrameDuration / 60f;
					zero.Y = -0.02f * (float)mouseMovement.Y / Time.FrameDuration / 60f;
					num = mouseWheelMovement / 120;
					if (mouseMovement != Point2.Zero)
					{
						this.IsControlledByTouch = false;
					}
				}
                if (input.IsKeyDownOnce(Key.UpArrow))
                {
                    speed++;
                    Debug.WriteLine("Speed is increased");
                }
                if (input.IsKeyDownOnce(Key.DownArrow))
                {
                    speed--;
                    Debug.WriteLine("Speed is decreased");
                }
                Debug.WriteLine("Speed is: " + speed);
				if (input.IsKeyDown(Key.LeftArrow))
				{
					state = true;
					step = ModifierHolder.steppedTravel;
				} else if (input.IsKeyDown(Key.RightArrow))
                {
                    state = true;
                    step = -ModifierHolder.steppedTravel;
                } else
				{
                    state = false;
                }
                Vector3 vector = default(Vector3) + Vector3.UnitX * (float)(input.IsKeyDown(Key.D) ? 1 : 0);
				vector += -Vector3.UnitZ * (float)(input.IsKeyDown(Key.S) ? 1 : 0);
				vector += Vector3.UnitZ * (float)(input.IsKeyDown(Key.W) ? 1 : 0);
				vector += -Vector3.UnitX * (float)(input.IsKeyDown(Key.A) ? 1 : 0);
				vector += Vector3.UnitY * (float)(input.IsKeyDown(Key.Space) ? 1 : 0);
				vector += -Vector3.UnitY * (float)(input.IsKeyDown(Key.Shift) ? 1 : 0);
				this.m_playerInput.Look = this.m_playerInput.Look + new Vector2(MathUtils.Clamp(zero.X, -15f, 15f), MathUtils.Clamp(zero.Y, -15f, 15f));
				this.m_playerInput.Move = this.m_playerInput.Move + vector;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + vector;
				this.m_playerInput.Jump = (this.m_playerInput.Jump | input.IsKeyDownOnce(Key.Space));
				this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory - num;
				this.m_playerInput.Dig = (input.IsMouseButtonDown(MouseButton.Left) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Dig);
				this.m_playerInput.Hit = (input.IsMouseButtonDownOnce(MouseButton.Left) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Hit);
				this.m_playerInput.Aim = (input.IsMouseButtonDown(MouseButton.Right) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Aim);
				this.m_playerInput.Interact = (input.IsMouseButtonDownOnce(MouseButton.Right) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Interact);
				this.m_playerInput.ToggleSneak = (this.m_playerInput.ToggleSneak | input.IsKeyDownOnce(Key.Shift));
				this.m_playerInput.ToggleMount = (this.m_playerInput.ToggleMount | input.IsKeyDownOnce(Key.R));
				this.m_playerInput.ToggleCreativeFly = (this.m_playerInput.ToggleCreativeFly | input.IsKeyDownOnce(Key.F));
				this.m_playerInput.PickBlockType = (input.IsMouseButtonDownOnce(MouseButton.Middle) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.PickBlockType);
			}
			if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsKeyDownOnce(Key.E));
				this.m_playerInput.ToggleClothing = (this.m_playerInput.ToggleClothing | input.IsKeyDownOnce(Key.C));
				this.m_playerInput.TakeScreenshot = (this.m_playerInput.TakeScreenshot | input.IsKeyDownOnce(Key.O));
				this.m_playerInput.SwitchCameraMode = (this.m_playerInput.SwitchCameraMode | input.IsKeyDownOnce(Key.F5));
				this.m_playerInput.TimeOfDay = (this.m_playerInput.TimeOfDay | input.IsKeyDownOnce(Key.T));
				this.m_playerInput.Lighting = (this.m_playerInput.Lighting | input.IsKeyDownOnce(Key.L));
				this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsKeyDownOnce(Key.Q));
				this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsKeyDownOnce(Key.G));
				this.m_playerInput.KeyboardHelp = (this.m_playerInput.KeyboardHelp | input.IsKeyDownOnce(Key.H));
				if (input.IsKeyDownOnce(Key.Number0))
				{
					this.m_playerInput.SelectInventorySlot = new int?(0);
				}
				if (input.IsKeyDownOnce(Key.Number1))
				{
					this.m_playerInput.SelectInventorySlot = new int?(1);
				}
				if (input.IsKeyDownOnce(Key.Number2))
				{
					this.m_playerInput.SelectInventorySlot = new int?(2);
				}
				if (input.IsKeyDownOnce(Key.Number3))
				{
					this.m_playerInput.SelectInventorySlot = new int?(3);
				}
				if (input.IsKeyDownOnce(Key.Number4))
				{
					this.m_playerInput.SelectInventorySlot = new int?(4);
				}
				if (input.IsKeyDownOnce(Key.Number5))
				{
					this.m_playerInput.SelectInventorySlot = new int?(5);
				}
				if (input.IsKeyDownOnce(Key.Number6))
				{
					this.m_playerInput.SelectInventorySlot = new int?(6);
				}
				if (input.IsKeyDownOnce(Key.Number7))
				{
					this.m_playerInput.SelectInventorySlot = new int?(7);
				}
				if (input.IsKeyDownOnce(Key.Number8))
				{
					this.m_playerInput.SelectInventorySlot = new int?(8);
				}
				if (input.IsKeyDownOnce(Key.Z))
				{
					this.m_playerInput.SelectInventorySlot = new int?(9);
				}
			}

			
		}

		public void checker(WidgetInput input)
		{
			Vector3 viewPosition = this.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition;
			Vector3 viewDirection = this.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsMouseCursorVisible)
				{
					ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
					Vector2 value = viewWidget.WidgetToScreen(viewWidget.ActualSize / 2f);
					input.IsMouseCursorVisible = true;
					input.MousePosition = new Vector2?(value);
				}
			}
			else
			{
				input.IsMouseCursorVisible = false;
				Vector2 zero = Vector2.Zero;
				int num = 0;
				if (Window.IsActive && Time.FrameDuration > 0f)
				{
					Point2 mouseMovement = input.MouseMovement;
					int mouseWheelMovement = input.MouseWheelMovement;
					zero.X = 0.02f * (float)mouseMovement.X / Time.FrameDuration / 60f;
					zero.Y = -0.02f * (float)mouseMovement.Y / Time.FrameDuration / 60f;
					num = mouseWheelMovement / 120;
					if (mouseMovement != Point2.Zero)
					{
						this.IsControlledByTouch = false;
					}
				}
				Vector3 vector = default(Vector3) + Vector3.UnitX * (float)(input.IsKeyDown(Key.C) ? 1 : 0);
				vector += -Vector3.UnitZ * (float)(input.IsKeyDown(Key.R) ? 1 : 0);
				vector += Vector3.UnitZ * (float)(input.IsKeyDown(Key.V) ? 1 : 0);
				vector += -Vector3.UnitX * (float)(input.IsKeyDown(Key.CapsLock) ? 1 : 0);
				vector += Vector3.UnitY * (float)(input.IsKeyDown(Key.Escape) ? 1 : 0);
				vector += -Vector3.UnitY * (float)(input.IsKeyDown(Key.Shift) ? 1 : 0);
				this.m_playerInput.Look = this.m_playerInput.Look + new Vector2(MathUtils.Clamp(zero.X, -15f, 15f), MathUtils.Clamp(zero.Y, -15f, 15f));
                if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
                {
                    this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsKeyDownOnce(Key.D));
                    this.m_playerInput.ToggleClothing = (this.m_playerInput.ToggleClothing | input.IsKeyDownOnce(Key.B));
                    this.m_playerInput.TakeScreenshot = (this.m_playerInput.TakeScreenshot | input.IsKeyDownOnce(Key.O));
                    this.m_playerInput.SwitchCameraMode = (this.m_playerInput.SwitchCameraMode | input.IsKeyDownOnce(Key.U));
                    this.m_playerInput.TimeOfDay = (this.m_playerInput.TimeOfDay | input.IsKeyDownOnce(Key.S));
                    this.m_playerInput.Lighting = (this.m_playerInput.Lighting | input.IsKeyDownOnce(Key.K));
                    this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsKeyDownOnce(Key.P));
                    this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsKeyDownOnce(Key.F));
                    this.m_playerInput.KeyboardHelp = (this.m_playerInput.KeyboardHelp | input.IsKeyDownOnce(Key.G));
                    if (input.IsKeyDownOnce(Key.Number0))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(0);
                    }
                    if (input.IsKeyDownOnce(Key.Number1))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(1);
                    }
                    if (input.IsKeyDownOnce(Key.Number2))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(2);
                    }
                    if (input.IsKeyDownOnce(Key.Number3))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(3);
                    }
                    if (input.IsKeyDownOnce(Key.Number4))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(4);
                    }
                    if (input.IsKeyDownOnce(Key.Number5))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(5);
                    }
                    if (input.IsKeyDownOnce(Key.Number6))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(6);
                    }
                    if (input.IsKeyDownOnce(Key.Number7))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(7);
                    }
                    if (input.IsKeyDownOnce(Key.Number8))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(8);
                    }
                    if (input.IsKeyDownOnce(Key.Z))
                    {
                        this.m_playerInput.SelectInventorySlot = new int?(9);
                    }
                }
            }
        }

        // Token: 0x06000DD1 RID: 3537 RVA: 0x0006A5DC File Offset: 0x000687DC
        public void UpdateInputFromGamepad(WidgetInput input)
		{
			Vector3 viewPosition = this.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition;
			Vector3 viewDirection = this.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsPadCursorVisible)
				{
					ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
					Vector2 padCursorPosition = viewWidget.WidgetToScreen(viewWidget.ActualSize / 2f);
					input.IsPadCursorVisible = true;
					input.PadCursorPosition = padCursorPosition;
				}
			}
			else
			{
				input.IsPadCursorVisible = false;
				Vector3 vector = Vector3.Zero;
				Vector2 padStickPosition = input.GetPadStickPosition(GamePadStick.Left, SettingsManager.GamepadDeadZone);
				Vector2 padStickPosition2 = input.GetPadStickPosition(GamePadStick.Right, SettingsManager.GamepadDeadZone);
				float padTriggerPosition = input.GetPadTriggerPosition(GamePadTrigger.Left, 0f);
				float padTriggerPosition2 = input.GetPadTriggerPosition(GamePadTrigger.Right, 0f);
				vector += new Vector3(2f * padStickPosition.X, 0f, 2f * padStickPosition.Y);
				vector += Vector3.UnitY * (float)(input.IsPadButtonDown(GamePadButton.A) ? 1 : 0);
				vector += -Vector3.UnitY * (float)(input.IsPadButtonDown(GamePadButton.RightShoulder) ? 1 : 0);
				this.m_playerInput.Move = this.m_playerInput.Move + vector;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + vector;
				this.m_playerInput.Look = this.m_playerInput.Look + 0.75f * padStickPosition2 * MathUtils.Pow(padStickPosition2.LengthSquared(), 0.25f);
				this.m_playerInput.Jump = (this.m_playerInput.Jump | input.IsPadButtonDownOnce(GamePadButton.A));
				this.m_playerInput.Dig = ((padTriggerPosition2 >= 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Dig);
				this.m_playerInput.Hit = ((padTriggerPosition2 >= 0.5f && this.m_lastRightTrigger < 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Hit);
				this.m_playerInput.Aim = ((padTriggerPosition >= 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Aim);
				this.m_playerInput.Interact = ((padTriggerPosition >= 0.5f && this.m_lastLeftTrigger < 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Interact);
				this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsPadButtonDownOnce(GamePadButton.B));
				this.m_playerInput.ToggleMount = (this.m_playerInput.ToggleMount | (input.IsPadButtonDownOnce(GamePadButton.LeftThumb) || input.IsPadButtonDownOnce(GamePadButton.DPadUp)));
				this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsPadButtonDownOnce(GamePadButton.LeftShoulder));
				this.m_playerInput.ToggleSneak = (this.m_playerInput.ToggleSneak | input.IsPadButtonDownOnce(GamePadButton.RightShoulder));
				this.m_playerInput.SwitchCameraMode = (this.m_playerInput.SwitchCameraMode | (input.IsPadButtonDownOnce(GamePadButton.RightThumb) || input.IsPadButtonDownOnce(GamePadButton.DPadDown)));
				if (input.IsPadButtonDownRepeat(GamePadButton.DPadLeft))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory - 1;
				}
				if (input.IsPadButtonDownRepeat(GamePadButton.DPadRight))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory + 1;
				}
				if (padStickPosition != Vector2.Zero || padStickPosition2 != Vector2.Zero)
				{
					this.IsControlledByTouch = false;
				}
				this.m_lastLeftTrigger = padTriggerPosition;
				this.m_lastRightTrigger = padTriggerPosition2;
			}
			if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsPadButtonDownOnce(GamePadButton.X));
				this.m_playerInput.ToggleClothing = (this.m_playerInput.ToggleClothing | input.IsPadButtonDownOnce(GamePadButton.Y));
				this.m_playerInput.GamepadHelp = (this.m_playerInput.GamepadHelp | input.IsPadButtonDownOnce(GamePadButton.Start));
			}
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x0006A9C4 File Offset: 0x00068BC4
		public void UpdateInputFromVrControllers(WidgetInput input)
		{
			if (!this.IsControlledByVr)
			{
				return;
			}
			this.IsControlledByTouch = false;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsVrCursorVisible)
				{
					input.IsVrCursorVisible = true;
				}
			}
			else
			{
				input.IsVrCursorVisible = false;
				float num = MathUtils.Pow(1.25f, 10f * (SettingsManager.MoveSensitivity - 0.5f));
				float num2 = MathUtils.Pow(1.25f, 10f * (SettingsManager.LookSensitivity - 0.5f));
				float num3 = MathUtils.Clamp(this.m_subsystemTime.GameTimeDelta, 0f, 0.1f);
				Vector2 v = Vector2.Normalize(this.m_componentPlayer.ComponentBody.Matrix.Right.XZ);
				Vector2 v2 = Vector2.Normalize(this.m_componentPlayer.ComponentBody.Matrix.Forward.XZ);
				Vector2 vrStickPosition = input.GetVrStickPosition(VrController.Left, 0.2f);
				Vector2 vrStickPosition2 = input.GetVrStickPosition(VrController.Right, 0.2f);
				Matrix m = VrManager.HmdMatrixInverted.OrientationMatrix * this.m_componentPlayer.ComponentCreatureModel.EyeRotation.ToMatrix();
				Vector2 xz = Vector3.TransformNormal(new Vector3(VrManager.WalkingVelocity.X, 0f, VrManager.WalkingVelocity.Y), m).XZ;
				Vector3 value = Vector3.TransformNormal(new Vector3(VrManager.HeadMove.X, 0f, VrManager.HeadMove.Y), m);
				Vector3 vector = Vector3.Zero;
				vector += 0.5f * new Vector3(Vector2.Dot(xz, v), 0f, Vector2.Dot(xz, v2));
				vector += new Vector3(2f * vrStickPosition.X, 2f * vrStickPosition2.Y, 2f * vrStickPosition.Y);
				this.m_playerInput.Move = this.m_playerInput.Move + vector;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + vector;
				this.m_playerInput.VrMove = new Vector3?(value);
				TouchInput? touchInput = VrManager.GetTouchInput(VrController.Left);
				if (touchInput != null && num3 > 0f)
				{
					if (touchInput.Value.InputType == TouchInputType.Move)
					{
						Vector2 move = touchInput.Value.Move;
						Vector2 vector2 = 10f * num / num3 * new Vector2(0.5f) * move * MathUtils.Pow(move.LengthSquared(), 0.175f);
						this.m_playerInput.SneakMove.X = this.m_playerInput.SneakMove.X + vector2.X;
						this.m_playerInput.SneakMove.Z = this.m_playerInput.SneakMove.Z + vector2.Y;
						this.m_playerInput.Move.X = this.m_playerInput.Move.X + ComponentInput.ProcessInputValue(touchInput.Value.TotalMoveLimited.X, 0.1f, 1f);
						this.m_playerInput.Move.Z = this.m_playerInput.Move.Z + ComponentInput.ProcessInputValue(touchInput.Value.TotalMoveLimited.Y, 0.1f, 1f);
					}
					else if (touchInput.Value.InputType == TouchInputType.Tap)
					{
						this.m_playerInput.Jump = true;
					}
				}
				this.m_playerInput.Look = this.m_playerInput.Look + 0.5f * vrStickPosition2 * MathUtils.Pow(vrStickPosition2.LengthSquared(), 0.25f);
				Vector3 hmdMatrixYpr = VrManager.HmdMatrixYpr;
				Vector3 hmdLastMatrixYpr = VrManager.HmdLastMatrixYpr;
				Vector3 vector3 = hmdMatrixYpr - hmdLastMatrixYpr;
				this.m_playerInput.VrLook = new Vector2?(new Vector2(vector3.X, hmdMatrixYpr.Y));
				TouchInput? touchInput2 = VrManager.GetTouchInput(VrController.Right);
				Vector2 zero = Vector2.Zero;
				if (touchInput2 != null)
				{
					if (touchInput2.Value.InputType == TouchInputType.Move)
					{
						zero.X = touchInput2.Value.Move.X;
						this.m_playerInput.Move.Y = this.m_playerInput.Move.Y + ComponentInput.ProcessInputValue(touchInput2.Value.TotalMoveLimited.Y, 0.1f, 1f);
					}
					else if (touchInput2.Value.InputType == TouchInputType.Tap)
					{
						this.m_playerInput.Jump = true;
					}
				}
				if (num3 > 0f)
				{
					this.m_vrSmoothLook = Vector2.Lerp(this.m_vrSmoothLook, zero, 14f * num3);
					this.m_playerInput.Look = this.m_playerInput.Look + num2 / num3 * new Vector2(0.25f) * this.m_vrSmoothLook * MathUtils.Pow(this.m_vrSmoothLook.LengthSquared(), 0.3f);
				}
				if (VrManager.IsControllerPresent(VrController.Right))
				{
					this.m_playerInput.Dig = (VrManager.IsButtonDown(VrController.Right, VrControllerButton.Trigger) ? this.CalculateVrHandRay() : this.m_playerInput.Dig);
					this.m_playerInput.Hit = (VrManager.IsButtonDownOnce(VrController.Right, VrControllerButton.Trigger) ? this.CalculateVrHandRay() : this.m_playerInput.Hit);
					this.m_playerInput.Aim = (VrManager.IsButtonDown(VrController.Left, VrControllerButton.Trigger) ? this.CalculateVrHandRay() : this.m_playerInput.Aim);
					this.m_playerInput.Interact = (VrManager.IsButtonDownOnce(VrController.Left, VrControllerButton.Trigger) ? this.CalculateVrHandRay() : this.m_playerInput.Interact);
				}
				this.m_playerInput.ToggleMount = (this.m_playerInput.ToggleMount | input.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadUp));
				this.m_playerInput.ToggleSneak = (this.m_playerInput.ToggleSneak | input.IsVrButtonDownOnce(VrController.Left, VrControllerButton.TouchpadDown));
				this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsVrButtonDownOnce(VrController.Left, VrControllerButton.Grip));
				this.m_playerInput.ToggleCreativeFly = (this.m_playerInput.ToggleCreativeFly | input.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadUp));
				if (input.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadLeft))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory - 1;
				}
				if (input.IsVrButtonDownOnce(VrController.Right, VrControllerButton.TouchpadRight))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory + 1;
				}
				this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Grip));
			}
			if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsVrButtonDownOnce(VrController.Right, VrControllerButton.Menu));
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0006B03C File Offset: 0x0006923C
		public void UpdateInputFromWidgets(WidgetInput input)
		{
			float num = MathUtils.Pow(1.25f, 10f * (SettingsManager.MoveSensitivity - 0.5f));
			float num2 = MathUtils.Pow(1.25f, 10f * (SettingsManager.LookSensitivity - 0.5f));
			float num3 = MathUtils.Clamp(this.m_subsystemTime.GameTimeDelta, 0f, 0.1f);
			ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
			this.m_componentGui.MoveWidget.Radius = 30f / num * this.m_componentGui.MoveWidget.GlobalScale;
			if (this.m_componentGui.ModalPanelWidget != null || this.m_subsystemTime.GameTimeFactor <= 0f || num3 <= 0f)
			{
				return;
			}
			Vector2 vector = new Vector2((float)(SettingsManager.LeftHandedLayout ? 96 : -96), -96f);
			vector = Vector2.TransformNormal(vector, input.Widget.GlobalTransform);
			if (this.m_componentGui.ViewWidget != null && this.m_componentGui.ViewWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				TouchInput value = this.m_componentGui.ViewWidget.TouchInput.Value;
				Camera activeCamera = this.m_componentPlayer.GameWidget.ActiveCamera;
				Vector3 viewPosition = activeCamera.ViewPosition;
				Vector3 viewDirection = activeCamera.ViewDirection;
				Vector3 direction = Vector3.Normalize(activeCamera.ScreenToWorld(new Vector3(value.Position, 1f), Matrix.Identity) - viewPosition);
				Vector3 direction2 = Vector3.Normalize(activeCamera.ScreenToWorld(new Vector3(value.Position + vector, 1f), Matrix.Identity) - viewPosition);
				if (value.InputType == TouchInputType.Tap)
				{
					if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						this.m_playerInput.Interact = new Ray3?(new Ray3(viewPosition, viewDirection));
						this.m_playerInput.Hit = new Ray3?(new Ray3(viewPosition, viewDirection));
					}
					else
					{
						this.m_playerInput.Interact = new Ray3?(new Ray3(viewPosition, direction));
						this.m_playerInput.Hit = new Ray3?(new Ray3(viewPosition, direction));
					}
				}
				else if (value.InputType == TouchInputType.Hold && value.DurationFrames > 1 && value.Duration > 0.2f)
				{
					if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, viewDirection));
						this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
					}
					else
					{
						this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, direction));
						this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
					}
					this.m_isViewHoldStarted = true;
				}
				else if (value.InputType == TouchInputType.Move)
				{
					if (SettingsManager.LookControlMode == LookControlMode.EntireScreen || SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						Vector2 v = Vector2.TransformNormal(value.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
						Vector2 v2 = num2 / num3 * new Vector2(0.0006f, -0.0006f) * v * MathUtils.Pow(v.LengthSquared(), 0.125f);
						this.m_playerInput.Look = this.m_playerInput.Look + v2;
					}
					if (this.m_isViewHoldStarted)
					{
						if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
						{
							this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, viewDirection));
							this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
						}
						else
						{
							this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, direction));
							this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
						}
					}
				}
			}
			else
			{
				this.m_isViewHoldStarted = false;
			}
			if (this.m_componentGui.MoveWidget != null && this.m_componentGui.MoveWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				float radius = this.m_componentGui.MoveWidget.Radius;
				TouchInput value2 = this.m_componentGui.MoveWidget.TouchInput.Value;
				if (value2.InputType == TouchInputType.Tap)
				{
					this.m_playerInput.Jump = true;
				}
				else if (value2.InputType == TouchInputType.Move || value2.InputType == TouchInputType.Hold)
				{
					Vector2 v3 = Vector2.TransformNormal(value2.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					Vector2 vector2 = num / num3 * new Vector2(0.003f, -0.003f) * v3 * MathUtils.Pow(v3.LengthSquared(), 0.175f);
					this.m_playerInput.SneakMove.X = this.m_playerInput.SneakMove.X + vector2.X;
					this.m_playerInput.SneakMove.Z = this.m_playerInput.SneakMove.Z + vector2.Y;
					Vector2 vector3 = Vector2.TransformNormal(value2.TotalMoveLimited, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					this.m_playerInput.Move.X = this.m_playerInput.Move.X + ComponentInput.ProcessInputValue(vector3.X * viewWidget.GlobalScale, 0.2f * radius, radius);
					this.m_playerInput.Move.Z = this.m_playerInput.Move.Z + ComponentInput.ProcessInputValue((0f - vector3.Y) * viewWidget.GlobalScale, 0.2f * radius, radius);
				}
			}
			if (this.m_componentGui.MoveRoseWidget != null)
			{
				if (this.m_componentGui.MoveRoseWidget.Direction != Vector3.Zero || this.m_componentGui.MoveRoseWidget.Jump)
				{
					this.IsControlledByTouch = true;
				}
				this.m_playerInput.Move = this.m_playerInput.Move + this.m_componentGui.MoveRoseWidget.Direction;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + this.m_componentGui.MoveRoseWidget.Direction;
				this.m_playerInput.Jump = (this.m_playerInput.Jump | this.m_componentGui.MoveRoseWidget.Jump);
			}
			if (this.m_componentGui.LookWidget != null && this.m_componentGui.LookWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				TouchInput value3 = this.m_componentGui.LookWidget.TouchInput.Value;
				if (value3.InputType == TouchInputType.Tap)
				{
					this.m_playerInput.Jump = true;
					return;
				}
				if (value3.InputType == TouchInputType.Move)
				{
					Vector2 v4 = Vector2.TransformNormal(value3.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					Vector2 v5 = num2 / num3 * new Vector2(0.0006f, -0.0006f) * v4 * MathUtils.Pow(v4.LengthSquared(), 0.125f);
					this.m_playerInput.Look = this.m_playerInput.Look + v5;
				}
			}
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x0006B75C File Offset: 0x0006995C
		public static float ProcessInputValue(float value, float deadZone, float saturationZone)
		{
			return MathUtils.Sign(value) * MathUtils.Clamp((MathUtils.Abs(value) - deadZone) / (saturationZone - deadZone), 0f, 1f);
		}

		// Token: 0x040008B0 RID: 2224
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008B1 RID: 2225
		public ComponentGui m_componentGui;

		// Token: 0x040008B2 RID: 2226
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040008B3 RID: 2227
		public PlayerInput m_playerInput;

		// Token: 0x040008B4 RID: 2228
		public bool m_isViewHoldStarted;

		// Token: 0x040008B5 RID: 2229
		public double m_lastJumpTime;

		// Token: 0x040008B6 RID: 2230
		public float m_lastLeftTrigger;

		// Token: 0x040008B7 RID: 2231
		public float m_lastRightTrigger;

		// Token: 0x040008B8 RID: 2232
		public Vector2 m_vrSmoothLook;
	}
}
