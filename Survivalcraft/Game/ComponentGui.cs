using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E4 RID: 484
	public class ComponentGui : Component, IUpdateable
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000D55 RID: 3413 RVA: 0x000651CA File Offset: 0x000633CA
		// (set) Token: 0x06000D56 RID: 3414 RVA: 0x000651D2 File Offset: 0x000633D2
		public ContainerWidget ControlsContainerWidget { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000D57 RID: 3415 RVA: 0x000651DB File Offset: 0x000633DB
		// (set) Token: 0x06000D58 RID: 3416 RVA: 0x000651E3 File Offset: 0x000633E3
		public TouchInputWidget ViewWidget { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000D59 RID: 3417 RVA: 0x000651EC File Offset: 0x000633EC
		// (set) Token: 0x06000D5A RID: 3418 RVA: 0x000651F4 File Offset: 0x000633F4
		public TouchInputWidget MoveWidget { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000D5B RID: 3419 RVA: 0x000651FD File Offset: 0x000633FD
		// (set) Token: 0x06000D5C RID: 3420 RVA: 0x00065205 File Offset: 0x00063405
		public MoveRoseWidget MoveRoseWidget { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000D5D RID: 3421 RVA: 0x0006520E File Offset: 0x0006340E
		// (set) Token: 0x06000D5E RID: 3422 RVA: 0x00065216 File Offset: 0x00063416
		public TouchInputWidget LookWidget { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x0006521F File Offset: 0x0006341F
		// (set) Token: 0x06000D60 RID: 3424 RVA: 0x00065227 File Offset: 0x00063427
		public ShortInventoryWidget ShortInventoryWidget { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x00065230 File Offset: 0x00063430
		// (set) Token: 0x06000D62 RID: 3426 RVA: 0x00065238 File Offset: 0x00063438
		public ValueBarWidget HealthBarWidget { get; set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x00065241 File Offset: 0x00063441
		// (set) Token: 0x06000D64 RID: 3428 RVA: 0x00065249 File Offset: 0x00063449
		public ValueBarWidget FoodBarWidget { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000D65 RID: 3429 RVA: 0x00065252 File Offset: 0x00063452
		// (set) Token: 0x06000D66 RID: 3430 RVA: 0x0006525A File Offset: 0x0006345A
		public ValueBarWidget TemperatureBarWidget { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000D67 RID: 3431 RVA: 0x00065263 File Offset: 0x00063463
		// (set) Token: 0x06000D68 RID: 3432 RVA: 0x0006526B File Offset: 0x0006346B
		public LabelWidget LevelLabelWidget { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x00065274 File Offset: 0x00063474
		// (set) Token: 0x06000D6A RID: 3434 RVA: 0x0006529C File Offset: 0x0006349C
		public Widget ModalPanelWidget
		{
			get
			{
				if (this.m_modalPanelContainerWidget.Children.Count <= 0)
				{
					return null;
				}
				return this.m_modalPanelContainerWidget.Children[0];
			}
			set
			{
				if (value != this.ModalPanelWidget)
				{
					if (this.m_modalPanelAnimationData != null)
					{
						this.EndModalPanelAnimation();
					}
					this.m_modalPanelAnimationData = new ComponentGui.ModalPanelAnimationData
					{
						OldWidget = this.ModalPanelWidget,
						NewWidget = value
					};
					if (value != null)
					{
						value.HorizontalAlignment = WidgetAlignment.Center;
						this.m_modalPanelContainerWidget.Children.Insert(0, value);
					}
					this.UpdateModalPanelAnimation();
					this.m_componentPlayer.GameWidget.Input.Clear();
					this.m_componentPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(null, -1);
				}
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000D6B RID: 3435 RVA: 0x00065327 File Offset: 0x00063527
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x0006532A File Offset: 0x0006352A
		public void DisplayLargeMessage(string largeText, string smallText, float duration, float delay)
		{
			this.m_message = new ComponentGui.Message
			{
				LargeText = largeText,
				SmallText = smallText,
				Duration = duration,
				StartTime = Time.RealTime + (double)delay
			};
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x0006535B File Offset: 0x0006355B
		public void DisplaySmallMessage(string text, Color color, bool blinking, bool playNotificationSound)
		{
			this.m_messageWidget.DisplayMessage(text, color, blinking);
			if (playNotificationSound)
			{
				this.m_subsystemAudio.PlaySound("Audio/UI/Message", 1f, 0f, 0f, 0f);
			}
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x00065394 File Offset: 0x00063594
		public bool IsGameMenuDialogVisible()
		{
			foreach (Dialog dialog in DialogsManager.Dialogs)
			{
				if (dialog.ParentWidget == this.m_componentPlayer.GuiWidget && dialog is GameMenuDialog)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x00065404 File Offset: 0x00063604
		public void Update(float dt)
		{
			this.HandleInput();
			this.UpdateWidgets();
		}

        // Token: 0x06000D70 RID: 3440 RVA: 0x00065414 File Offset: 0x00063614
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			ContainerWidget guiWidget = this.m_componentPlayer.GuiWidget;
			this.m_backButtonWidget = guiWidget.Children.Find<ButtonWidget>("BackButton", true);
			this.m_inventoryButtonWidget = guiWidget.Children.Find<ButtonWidget>("InventoryButton", true);
			this.m_clothingButtonWidget = guiWidget.Children.Find<ButtonWidget>("ClothingButton", true);
			this.m_moreButtonWidget = guiWidget.Children.Find<ButtonWidget>("MoreButton", true);
			this.m_moreContentsWidget = guiWidget.Children.Find<Widget>("MoreContents", true);
			this.m_helpButtonWidget = guiWidget.Children.Find<ButtonWidget>("HelpButton", true);
			this.m_photoButtonWidget = guiWidget.Children.Find<ButtonWidget>("PhotoButton", true);
			this.m_lightningButtonWidget = guiWidget.Children.Find<ButtonWidget>("LightningButton", true);
			this.m_timeOfDayButtonWidget = guiWidget.Children.Find<ButtonWidget>("TimeOfDayButton", true);
			this.m_cameraButtonWidget = guiWidget.Children.Find<ButtonWidget>("CameraButton", true);
			this.m_creativeFlyButtonWidget = guiWidget.Children.Find<ButtonWidget>("CreativeFlyButton", true);
			this.m_sneakButtonWidget = guiWidget.Children.Find<ButtonWidget>("SneakButton", true);
			this.m_mountButtonWidget = guiWidget.Children.Find<ButtonWidget>("MountButton", true);
			this.m_editItemButton = guiWidget.Children.Find<ButtonWidget>("EditItemButton", true);
			this.MoveWidget = guiWidget.Children.Find<TouchInputWidget>("Move", true);
			this.MoveRoseWidget = guiWidget.Children.Find<MoveRoseWidget>("MoveRose", true);
			this.LookWidget = guiWidget.Children.Find<TouchInputWidget>("Look", true);
			this.ViewWidget = this.m_componentPlayer.ViewWidget;
			this.HealthBarWidget = guiWidget.Children.Find<ValueBarWidget>("HealthBar", true);
			this.FoodBarWidget = guiWidget.Children.Find<ValueBarWidget>("FoodBar", true);
			this.TemperatureBarWidget = guiWidget.Children.Find<ValueBarWidget>("TemperatureBar", true);
			this.LevelLabelWidget = guiWidget.Children.Find<LabelWidget>("LevelLabel", true);
			this.m_modalPanelContainerWidget = guiWidget.Children.Find<ContainerWidget>("ModalPanelContainer", true);
			this.ControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("ControlsContainer", true);
			this.m_leftControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("LeftControlsContainer", true);
			this.m_rightControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("RightControlsContainer", true);
			this.m_moveContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveContainer", true);
			this.m_lookContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookContainer", true);
			this.m_moveRectangleWidget = guiWidget.Children.Find<RectangleWidget>("MoveRectangle", true);
			this.m_lookRectangleWidget = guiWidget.Children.Find<RectangleWidget>("LookRectangle", true);
			this.m_moveRectangleContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveRectangleContainer", true);
			this.m_lookRectangleContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookRectangleContainer", true);
			this.m_moveRectangleWidget = guiWidget.Children.Find<RectangleWidget>("MoveRectangle", true);
			this.m_lookRectangleWidget = guiWidget.Children.Find<RectangleWidget>("LookRectangle", true);
			this.m_movePadContainerWidget = guiWidget.Children.Find<ContainerWidget>("MovePadContainer", true);
			this.m_lookPadContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookPadContainer", true);
			this.m_moveButtonsContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveButtonsContainer", true);
			this.ShortInventoryWidget = guiWidget.Children.Find<ShortInventoryWidget>("ShortInventory", true);
			this.m_largeMessageWidget = guiWidget.Children.Find<ContainerWidget>("LargeMessage", true);
			this.m_messageWidget = guiWidget.Children.Find<MessageWidget>("Message", true);
			this.m_keyboardHelpMessageShown = valuesDictionary.GetValue<bool>("KeyboardHelpMessageShown");
			this.m_gamepadHelpMessageShown = valuesDictionary.GetValue<bool>("GamepadHelpMessageShown");
		}

        // Token: 0x06000D71 RID: 3441 RVA: 0x0006584D File Offset: 0x00063A4D
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("KeyboardHelpMessageShown", this.m_keyboardHelpMessageShown);
			valuesDictionary.SetValue<bool>("GamepadHelpMessageShown", this.m_gamepadHelpMessageShown);
		}

        // Token: 0x06000D72 RID: 3442 RVA: 0x00065871 File Offset: 0x00063A71
        public override void OnEntityAdded()
		{
			this.ShortInventoryWidget.AssignComponents(this.m_componentPlayer.ComponentMiner.Inventory);
		}

        // Token: 0x06000D73 RID: 3443 RVA: 0x0006588E File Offset: 0x00063A8E
        public override void OnEntityRemoved()
		{
			this.ShortInventoryWidget.AssignComponents(null);
			this.m_message = null;
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x000658A3 File Offset: 0x00063AA3
		public override void Dispose()
		{
			this.ModalPanelWidget = null;
			this.m_keyboardHelpDialog = null;
			if (this.ShortInventoryWidget != null)
			{
				this.ShortInventoryWidget.AssignComponents(null);
			}
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x000658C8 File Offset: 0x00063AC8
		public void UpdateSidePanelsAnimation()
		{
			float num = MathUtils.Min(Time.FrameDuration, 0.1f);
			bool flag = this.ModalPanelWidget != null && (this.m_modalPanelAnimationData == null || this.m_modalPanelAnimationData.NewWidget != null);
			float num2 = (float)((!this.m_componentPlayer.ComponentInput.IsControlledByTouch && !flag) ? 1 : 0);
			float x = num2 - this.m_sidePanelsFactor;
			if (MathUtils.Abs(x) > 0.01f)
			{
				this.m_sidePanelsFactor += MathUtils.Clamp(12f * MathUtils.PowSign(x, 0.75f) * num, 0f - MathUtils.Abs(x), MathUtils.Abs(x));
			}
			else
			{
				this.m_sidePanelsFactor = num2;
			}
			this.m_leftControlsContainerWidget.RenderTransform = Matrix.CreateTranslation(this.m_leftControlsContainerWidget.ActualSize.X * (0f - this.m_sidePanelsFactor), 0f, 0f);
			this.m_rightControlsContainerWidget.RenderTransform = Matrix.CreateTranslation(this.m_rightControlsContainerWidget.ActualSize.X * this.m_sidePanelsFactor, 0f, 0f);
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x000659E4 File Offset: 0x00063BE4
		public void UpdateModalPanelAnimation()
		{
			this.m_modalPanelAnimationData.Factor += 6f * MathUtils.Min(Time.FrameDuration, 0.1f);
			if (this.m_modalPanelAnimationData.Factor < 1f)
			{
				float factor = this.m_modalPanelAnimationData.Factor;
				float num = 0.5f + 0.5f * MathUtils.Pow(1f - factor, 0.1f);
				float num2 = 0.5f + 0.5f * MathUtils.Pow(factor, 0.1f);
				float s = 1f - factor;
				float s2 = factor;
				if (this.m_modalPanelAnimationData.OldWidget != null)
				{
					Vector2 actualSize = this.m_modalPanelAnimationData.OldWidget.ActualSize;
					this.m_modalPanelAnimationData.OldWidget.ColorTransform = Color.White * s;
					this.m_modalPanelAnimationData.OldWidget.RenderTransform = Matrix.CreateTranslation((0f - actualSize.X) / 2f, (0f - actualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(actualSize.X / 2f, actualSize.Y / 2f, 0f);
				}
				if (this.m_modalPanelAnimationData.NewWidget != null)
				{
					Vector2 actualSize2 = this.m_modalPanelAnimationData.NewWidget.ActualSize;
					this.m_modalPanelAnimationData.NewWidget.ColorTransform = Color.White * s2;
					this.m_modalPanelAnimationData.NewWidget.RenderTransform = Matrix.CreateTranslation((0f - actualSize2.X) / 2f, (0f - actualSize2.Y) / 2f, 0f) * Matrix.CreateScale(num2, num2, 1f) * Matrix.CreateTranslation(actualSize2.X / 2f, actualSize2.Y / 2f, 0f);
					return;
				}
			}
			else
			{
				this.EndModalPanelAnimation();
			}
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x00065BF0 File Offset: 0x00063DF0
		public void EndModalPanelAnimation()
		{
			if (this.m_modalPanelAnimationData.OldWidget != null)
			{
				this.m_modalPanelContainerWidget.Children.Remove(this.m_modalPanelAnimationData.OldWidget);
			}
			if (this.m_modalPanelAnimationData.NewWidget != null)
			{
				this.m_modalPanelAnimationData.NewWidget.ColorTransform = Color.White;
				this.m_modalPanelAnimationData.NewWidget.RenderTransform = Matrix.Identity;
			}
			this.m_modalPanelAnimationData = null;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00065C64 File Offset: 0x00063E64
		public void UpdateWidgets()
		{
			ComponentRider componentRider = this.m_componentPlayer.ComponentRider;
			ComponentSleep componentSleep = this.m_componentPlayer.ComponentSleep;
			ComponentInput componentInput = this.m_componentPlayer.ComponentInput;
			WorldSettings worldSettings = this.m_subsystemGameInfo.WorldSettings;
			GameMode gameMode = worldSettings.GameMode;
			this.UpdateSidePanelsAnimation();
			if (this.m_modalPanelAnimationData != null)
			{
				this.UpdateModalPanelAnimation();
			}
			if (this.m_message != null)
			{
				double realTime = Time.RealTime;
				this.m_largeMessageWidget.IsVisible = true;
				LabelWidget labelWidget = this.m_largeMessageWidget.Children.Find<LabelWidget>("LargeLabel", true);
				LabelWidget labelWidget2 = this.m_largeMessageWidget.Children.Find<LabelWidget>("SmallLabel", true);
				labelWidget.Text = this.m_message.LargeText;
				labelWidget2.Text = this.m_message.SmallText;
				labelWidget.IsVisible = !string.IsNullOrEmpty(this.m_message.LargeText);
				labelWidget2.IsVisible = !string.IsNullOrEmpty(this.m_message.SmallText);
				float num = (float)MathUtils.Min(MathUtils.Saturate(2.0 * (realTime - this.m_message.StartTime)), MathUtils.Saturate(2.0 * (this.m_message.StartTime + (double)this.m_message.Duration - realTime)));
				labelWidget.Color = new Color(num, num, num, num);
				labelWidget2.Color = new Color(num, num, num, num);
				if (Time.RealTime > this.m_message.StartTime + (double)this.m_message.Duration)
				{
					this.m_message = null;
				}
			}
			else
			{
				this.m_largeMessageWidget.IsVisible = false;
			}
			this.ControlsContainerWidget.IsVisible = (this.m_componentPlayer.PlayerData.IsReadyForPlaying && this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled && componentSleep.SleepFactor <= 0f);
			this.m_moveRectangleContainerWidget.IsVisible = (!SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch);
			this.m_lookRectangleContainerWidget.IsVisible = (!SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch && (SettingsManager.LookControlMode != LookControlMode.EntireScreen || SettingsManager.MoveControlMode != MoveControlMode.Buttons));
			this.m_lookPadContainerWidget.IsVisible = (SettingsManager.LookControlMode != LookControlMode.SplitTouch);
			this.MoveRoseWidget.IsVisible = componentInput.IsControlledByTouch;
			this.m_moreContentsWidget.IsVisible = this.m_moreButtonWidget.IsChecked;
			this.HealthBarWidget.IsVisible = (gameMode > GameMode.Creative);
			this.FoodBarWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.TemperatureBarWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.LevelLabelWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.m_creativeFlyButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_timeOfDayButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_lightningButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_moveButtonsContainerWidget.IsVisible = (SettingsManager.MoveControlMode == MoveControlMode.Buttons);
			this.m_movePadContainerWidget.IsVisible = (SettingsManager.MoveControlMode == MoveControlMode.Pad);
			if (SettingsManager.LeftHandedLayout)
			{
				this.m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				this.m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				this.m_moveRectangleWidget.FlipHorizontal = true;
				this.m_lookRectangleWidget.FlipHorizontal = false;
			}
			else
			{
				this.m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				this.m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				this.m_moveRectangleWidget.FlipHorizontal = false;
				this.m_lookRectangleWidget.FlipHorizontal = true;
			}
			this.m_sneakButtonWidget.IsChecked = this.m_componentPlayer.ComponentBody.IsSneaking;
			this.m_creativeFlyButtonWidget.IsChecked = this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
			this.m_inventoryButtonWidget.IsChecked = this.IsInventoryVisible();
			this.m_clothingButtonWidget.IsChecked = this.IsClothingVisible();
			if (this.IsActiveSlotEditable() || this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell != null)
			{
				this.m_sneakButtonWidget.IsVisible = false;
				this.m_mountButtonWidget.IsVisible = false;
				this.m_editItemButton.IsVisible = true;
			}
			else if (componentRider != null && componentRider.Mount != null)
			{
				this.m_sneakButtonWidget.IsVisible = false;
				this.m_mountButtonWidget.IsChecked = true;
				this.m_mountButtonWidget.IsVisible = true;
				this.m_editItemButton.IsVisible = false;
			}
			else
			{
				this.m_mountButtonWidget.IsChecked = false;
				if (componentRider != null && Time.FrameStartTime - this.m_lastMountableCreatureSearchTime > 0.5)
				{
					this.m_lastMountableCreatureSearchTime = Time.FrameStartTime;
					if (componentRider.FindNearestMount() != null)
					{
						this.m_sneakButtonWidget.IsVisible = false;
						this.m_mountButtonWidget.IsVisible = true;
						this.m_editItemButton.IsVisible = false;
					}
					else
					{
						this.m_sneakButtonWidget.IsVisible = true;
						this.m_mountButtonWidget.IsVisible = false;
						this.m_editItemButton.IsVisible = false;
					}
				}
			}
			if (!this.m_componentPlayer.IsAddedToProject || this.m_componentPlayer.ComponentHealth.Health == 0f || componentSleep.IsSleeping || this.m_componentPlayer.ComponentSickness.IsPuking)
			{
				this.ModalPanelWidget = null;
			}
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				this.m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = new Color(166, 175, 103);
				return;
			}
			if (this.m_componentPlayer.ComponentFlu.HasFlu)
			{
				this.m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = new Color(0, 48, 255);
				return;
			}
			this.m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = new Color(224, 24, 0);
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x00066230 File Offset: 0x00064430
		public void HandleInput()
		{
			WidgetInput input = this.m_componentPlayer.GameWidget.Input;
			PlayerInput playerInput = this.m_componentPlayer.ComponentInput.PlayerInput;
			ComponentRider componentRider = this.m_componentPlayer.ComponentRider;
			if (this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled)
			{
				if (!this.m_keyboardHelpMessageShown && (this.m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Time.PeriodicEvent(7.0, 0.0))
				{
					this.m_keyboardHelpMessageShown = true;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 1), Color.White, true, true);
				}
				else if (!this.m_gamepadHelpMessageShown && (this.m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != WidgetInputDevice.None && Time.PeriodicEvent(7.0, 0.0))
				{
					this.m_gamepadHelpMessageShown = true;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 2), Color.White, true, true);
				}
			}
			if (playerInput.KeyboardHelp)
			{
				if (this.m_keyboardHelpDialog == null)
				{
					this.m_keyboardHelpDialog = new KeyboardHelpDialog();
				}
				if (this.m_keyboardHelpDialog.ParentWidget != null)
				{
					DialogsManager.HideDialog(this.m_keyboardHelpDialog);
				}
				else
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, this.m_keyboardHelpDialog);
				}
			}
			if (playerInput.GamepadHelp)
			{
				if (this.m_gamepadHelpDialog == null)
				{
					this.m_gamepadHelpDialog = new GamepadHelpDialog();
				}
				if (this.m_gamepadHelpDialog.ParentWidget != null)
				{
					DialogsManager.HideDialog(this.m_gamepadHelpDialog);
				}
				else
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, this.m_gamepadHelpDialog);
				}
			}
			if (this.m_helpButtonWidget.IsClicked)
			{
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
			if (playerInput.ToggleInventory || this.m_inventoryButtonWidget.IsClicked)
			{
				if (this.IsInventoryVisible())
				{
					this.ModalPanelWidget = null;
				}
				else if (this.m_componentPlayer.ComponentMiner.Inventory is ComponentCreativeInventory)
				{
					this.ModalPanelWidget = new CreativeInventoryWidget(this.m_componentPlayer.Entity);
				}
				else
				{
					this.ModalPanelWidget = new FullInventoryWidget(this.m_componentPlayer.ComponentMiner.Inventory, this.m_componentPlayer.Entity.FindComponent<ComponentCraftingTable>(true));
				}
			}
			if (playerInput.ToggleClothing || this.m_clothingButtonWidget.IsClicked)
			{
				if (this.IsClothingVisible())
				{
					this.ModalPanelWidget = null;
				}
				else
				{
					this.ModalPanelWidget = new ClothingWidget(this.m_componentPlayer);
				}
			}
			if (this.m_sneakButtonWidget.IsClicked || playerInput.ToggleSneak)
			{
				bool isSneaking = this.m_componentPlayer.ComponentBody.IsSneaking;
				this.m_componentPlayer.ComponentBody.IsSneaking = !isSneaking;
				if (this.m_componentPlayer.ComponentBody.IsSneaking != isSneaking)
				{
					if (this.m_componentPlayer.ComponentBody.IsSneaking)
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 3), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 4), Color.White, false, false);
					}
				}
			}
			if (componentRider != null && (this.m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
				{
					componentRider.StartDismounting();
				}
				else
				{
					ComponentMount componentMount = componentRider.FindNearestMount();
					if (componentMount != null)
					{
						componentRider.StartMounting(componentMount);
					}
				}
				if (componentRider.Mount != null != flag)
				{
					if (componentRider.Mount != null)
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 5), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 6), Color.White, false, false);
					}
				}
			}
			if ((this.m_editItemButton.IsClicked || playerInput.EditItem) && this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell != null)
			{
				Point3 value = this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell.Value;
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(value.X, value.Y, value.Z);
				int contents = Terrain.ExtractContents(cellValue);
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(contents);
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					if (blockBehaviors[i].OnEditBlock(value.X, value.Y, value.Z, cellValue, this.m_componentPlayer))
					{
						break;
					}
				}
			}
			else if ((this.m_editItemButton.IsClicked || playerInput.EditItem) && this.IsActiveSlotEditable())
			{
				IInventory inventory = this.m_componentPlayer.ComponentMiner.Inventory;
				if (inventory != null)
				{
					int activeSlotIndex = inventory.ActiveSlotIndex;
					int num = Terrain.ExtractContents(inventory.GetSlotValue(activeSlotIndex));
					if (BlocksManager.Blocks[num].IsEditable)
					{
						SubsystemBlockBehavior[] blockBehaviors2 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num);
						int num2 = 0;
						while (num2 < blockBehaviors2.Length && !blockBehaviors2[num2].OnEditInventoryItem(inventory, activeSlotIndex, this.m_componentPlayer))
						{
							num2++;
						}
					}
				}
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_creativeFlyButtonWidget.IsClicked || playerInput.ToggleCreativeFly) && componentRider.Mount == null)
			{
				bool isCreativeFlyEnabled = this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
				this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled = !isCreativeFlyEnabled;
				if (this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled != isCreativeFlyEnabled)
				{
					if (this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled)
					{
						this.m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 7), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 8), Color.White, false, false);
					}
				}
			}
			if (!this.m_componentPlayer.ComponentInput.IsControlledByVr && (this.m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode))
			{
				GameWidget gameWidget = this.m_componentPlayer.GameWidget;
				if (gameWidget.ActiveCamera.GetType() == typeof(FppCamera))
				{
					gameWidget.ActiveCamera = gameWidget.FindCamera<TppCamera>(true);
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 9), Color.White, false, false);
				}
				else if (gameWidget.ActiveCamera.GetType() == typeof(TppCamera))
				{
					gameWidget.ActiveCamera = gameWidget.FindCamera<OrbitCamera>(true);
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 10), Color.White, false, false);
				}
				else if (gameWidget.ActiveCamera.GetType() == typeof(OrbitCamera))
				{
					gameWidget.ActiveCamera = gameWidget.FindCamera<FixedCamera>(true);
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 11), Color.White, false, false);
				}
				else
				{
					gameWidget.ActiveCamera = gameWidget.FindCamera<FppCamera>(true);
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 12), Color.White, false, false);
				}
			}
			if (this.m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto(delegate
				{
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 13), Color.White, false, false);
				}, delegate
				{
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 14), Color.White, false, false);
				});
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_lightningButtonWidget.IsClicked || playerInput.Lighting))
			{
				Matrix matrix = Matrix.CreateFromQuaternion(this.m_componentPlayer.ComponentCreatureModel.EyeRotation);
				base.Project.FindSubsystem<SubsystemWeather>(true).ManualLightingStrike(this.m_componentPlayer.ComponentCreatureModel.EyePosition, matrix.Forward);
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_timeOfDayButtonWidget.IsClicked || playerInput.TimeOfDay))
			{
				float num3 = MathUtils.Remainder(0.25f, 1f);
				float num4 = MathUtils.Remainder(0.5f, 1f);
				float num5 = MathUtils.Remainder(0.75f, 1f);
				float num6 = MathUtils.Remainder(1f, 1f);
				float num7 = MathUtils.Remainder(num3 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num8 = MathUtils.Remainder(num4 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num9 = MathUtils.Remainder(num5 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num10 = MathUtils.Remainder(num6 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num11 = MathUtils.Min(num7, num8, num9, num10);
				if (num7 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num7;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 15), Color.White, false, false);
				}
				else if (num8 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num8;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 16), Color.White, false, false);
				}
				else if (num9 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num9;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 17), Color.White, false, false);
				}
				else if (num10 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num10;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 18), Color.White, false, false);
				}
			}
			if (this.ModalPanelWidget != null)
			{
				if (input.Cancel || input.Back || this.m_backButtonWidget.IsClicked)
				{
					this.ModalPanelWidget = null;
					return;
				}
			}
			else if (input.Back || this.m_backButtonWidget.IsClicked)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new GameMenuDialog(this.m_componentPlayer));
			}
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00066BD7 File Offset: 0x00064DD7
		public bool IsClothingVisible()
		{
			return this.ModalPanelWidget is ClothingWidget;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00066BE7 File Offset: 0x00064DE7
		public bool IsInventoryVisible()
		{
			return this.ModalPanelWidget != null && !this.IsClothingVisible();
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00066BFC File Offset: 0x00064DFC
		public bool IsActiveSlotEditable()
		{
			IInventory inventory = this.m_componentPlayer.ComponentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				int num = Terrain.ExtractContents(inventory.GetSlotValue(activeSlotIndex));
				if (BlocksManager.Blocks[num].IsEditable)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000824 RID: 2084
		public static string fName = "ComponentGui";

		// Token: 0x04000825 RID: 2085
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000826 RID: 2086
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000827 RID: 2087
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000828 RID: 2088
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000829 RID: 2089
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x0400082A RID: 2090
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400082B RID: 2091
		public ContainerWidget m_leftControlsContainerWidget;

		// Token: 0x0400082C RID: 2092
		public ContainerWidget m_rightControlsContainerWidget;

		// Token: 0x0400082D RID: 2093
		public ContainerWidget m_moveContainerWidget;

		// Token: 0x0400082E RID: 2094
		public ContainerWidget m_lookContainerWidget;

		// Token: 0x0400082F RID: 2095
		public RectangleWidget m_moveRectangleWidget;

		// Token: 0x04000830 RID: 2096
		public RectangleWidget m_lookRectangleWidget;

		// Token: 0x04000831 RID: 2097
		public ContainerWidget m_moveRectangleContainerWidget;

		// Token: 0x04000832 RID: 2098
		public ContainerWidget m_lookRectangleContainerWidget;

		// Token: 0x04000833 RID: 2099
		public ContainerWidget m_movePadContainerWidget;

		// Token: 0x04000834 RID: 2100
		public ContainerWidget m_lookPadContainerWidget;

		// Token: 0x04000835 RID: 2101
		public ContainerWidget m_moveButtonsContainerWidget;

		// Token: 0x04000836 RID: 2102
		public ContainerWidget m_modalPanelContainerWidget;

		// Token: 0x04000837 RID: 2103
		public ContainerWidget m_largeMessageWidget;

		// Token: 0x04000838 RID: 2104
		public MessageWidget m_messageWidget;

		// Token: 0x04000839 RID: 2105
		public ButtonWidget m_backButtonWidget;

		// Token: 0x0400083A RID: 2106
		public ButtonWidget m_inventoryButtonWidget;

		// Token: 0x0400083B RID: 2107
		public ButtonWidget m_clothingButtonWidget;

		// Token: 0x0400083C RID: 2108
		public ButtonWidget m_moreButtonWidget;

		// Token: 0x0400083D RID: 2109
		public Widget m_moreContentsWidget;

		// Token: 0x0400083E RID: 2110
		public ButtonWidget m_lightningButtonWidget;

		// Token: 0x0400083F RID: 2111
		public ButtonWidget m_photoButtonWidget;

		// Token: 0x04000840 RID: 2112
		public ButtonWidget m_helpButtonWidget;

		// Token: 0x04000841 RID: 2113
		public ButtonWidget m_timeOfDayButtonWidget;

		// Token: 0x04000842 RID: 2114
		public ButtonWidget m_cameraButtonWidget;

		// Token: 0x04000843 RID: 2115
		public ButtonWidget m_creativeFlyButtonWidget;

		// Token: 0x04000844 RID: 2116
		public ButtonWidget m_sneakButtonWidget;

		// Token: 0x04000845 RID: 2117
		public ButtonWidget m_mountButtonWidget;

		// Token: 0x04000846 RID: 2118
		public ButtonWidget m_editItemButton;

		// Token: 0x04000847 RID: 2119
		public float m_sidePanelsFactor;

		// Token: 0x04000848 RID: 2120
		public ComponentGui.ModalPanelAnimationData m_modalPanelAnimationData;

		// Token: 0x04000849 RID: 2121
		public ComponentGui.Message m_message;

		// Token: 0x0400084A RID: 2122
		public KeyboardHelpDialog m_keyboardHelpDialog;

		// Token: 0x0400084B RID: 2123
		public GamepadHelpDialog m_gamepadHelpDialog;

		// Token: 0x0400084C RID: 2124
		public double m_lastMountableCreatureSearchTime;

		// Token: 0x0400084D RID: 2125
		public bool m_keyboardHelpMessageShown;

		// Token: 0x0400084E RID: 2126
		public bool m_gamepadHelpMessageShown;

		// Token: 0x02000457 RID: 1111
		public class ModalPanelAnimationData
		{
			// Token: 0x04001645 RID: 5701
			public Widget NewWidget;

			// Token: 0x04001646 RID: 5702
			public Widget OldWidget;

			// Token: 0x04001647 RID: 5703
			public float Factor;
		}

		// Token: 0x02000458 RID: 1112
		public class Message
		{
			// Token: 0x04001648 RID: 5704
			public string LargeText;

			// Token: 0x04001649 RID: 5705
			public string SmallText;

			// Token: 0x0400164A RID: 5706
			public double StartTime;

			// Token: 0x0400164B RID: 5707
			public float Duration;
		}
	}
}
