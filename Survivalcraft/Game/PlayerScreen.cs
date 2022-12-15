using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine.Graphics;
using Engine.Input;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000140 RID: 320
	public class PlayerScreen : Screen
	{
		// Token: 0x060005F7 RID: 1527 RVA: 0x000225C4 File Offset: 0x000207C4
		public PlayerScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/PlayerScreen");
			base.LoadContents(this, node);
			this.m_playerModel = this.Children.Find<PlayerModelWidget>("Model", true);
			this.m_playerClassButton = this.Children.Find<ButtonWidget>("PlayerClassButton", true);
			this.m_nameTextBox = this.Children.Find<TextBoxWidget>("Name", true);
			this.m_characterSkinLabel = this.Children.Find<LabelWidget>("CharacterSkinLabel", true);
			this.m_characterSkinButton = this.Children.Find<ButtonWidget>("CharacterSkinButton", true);
			this.m_controlsLabel = this.Children.Find<LabelWidget>("ControlsLabel", true);
			this.m_controlsButton = this.Children.Find<ButtonWidget>("ControlsButton", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("DescriptionLabel", true);
			this.m_addButton = this.Children.Find<ButtonWidget>("AddButton", true);
			this.m_addAnotherButton = this.Children.Find<ButtonWidget>("AddAnotherButton", true);
			this.m_deleteButton = this.Children.Find<ButtonWidget>("DeleteButton", true);
			this.m_playButton = this.Children.Find<ButtonWidget>("PlayButton", true);
			this.m_characterSkinsCache = new CharacterSkinsCache();
			this.m_playerModel.CharacterSkinsCache = this.m_characterSkinsCache;
			this.m_nameTextBox.FocusLost += delegate(TextBoxWidget p)
			{
				if (this.VerifyName())
				{
					this.m_playerData.Name = this.m_nameTextBox.Text.Trim();
					return;
				}
				this.m_nameWasInvalid = true;
			};
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00022748 File Offset: 0x00020948
		public override void Enter(object[] parameters)
		{
			this.m_mode = (PlayerScreen.Mode)parameters[0];
			if (this.m_mode == PlayerScreen.Mode.Edit)
			{
				this.m_playerData = (PlayerData)parameters[1];
			}
			else
			{
				this.m_playerData = new PlayerData((Project)parameters[1]);
			}
			if (this.m_mode == PlayerScreen.Mode.Initial)
			{
				this.m_playerClassButton.IsEnabled = true;
				this.m_addButton.IsVisible = false;
				this.m_deleteButton.IsVisible = false;
				this.m_playButton.IsVisible = true;
				this.m_addAnotherButton.IsVisible = (this.m_playerData.SubsystemPlayers.PlayersData.Count < 3);
				return;
			}
			if (this.m_mode == PlayerScreen.Mode.Add)
			{
				this.m_playerClassButton.IsEnabled = true;
				this.m_addButton.IsVisible = true;
				this.m_deleteButton.IsVisible = false;
				this.m_playButton.IsVisible = false;
				this.m_addAnotherButton.IsVisible = false;
				return;
			}
			if (this.m_mode == PlayerScreen.Mode.Edit)
			{
				this.m_playerClassButton.IsEnabled = false;
				this.m_addButton.IsVisible = false;
				this.m_deleteButton.IsVisible = (this.m_playerData.SubsystemPlayers.PlayersData.Count > 1);
				this.m_playButton.IsVisible = false;
				this.m_addAnotherButton.IsVisible = false;
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00022893 File Offset: 0x00020A93
		public override void Leave()
		{
			this.m_characterSkinsCache.Clear();
			this.m_playerData = null;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000228A8 File Offset: 0x00020AA8
		public override void Update()
		{
			this.m_characterSkinsCache.GetTexture(this.m_playerData.CharacterSkinName);
			this.m_playerModel.PlayerClass = this.m_playerData.PlayerClass;
			this.m_playerModel.CharacterSkinName = this.m_playerData.CharacterSkinName;
			this.m_playerClassButton.Text = this.m_playerData.PlayerClass.ToString();
			if (!this.m_nameTextBox.HasFocus)
			{
				this.m_nameTextBox.Text = this.m_playerData.Name;
			}
			this.m_characterSkinLabel.Text = CharacterSkinsManager.GetDisplayName(this.m_playerData.CharacterSkinName);
			this.m_controlsLabel.Text = PlayerScreen.GetDeviceDisplayName((from id in this.m_inputDevices
			where (id & this.m_playerData.InputDevice) > WidgetInputDevice.None
			select id).FirstOrDefault<WidgetInputDevice>());
			string text = DatabaseManager.FindValuesDictionaryForComponent(DatabaseManager.FindEntityValuesDictionary(this.m_playerData.GetEntityTemplateName(), true), typeof(ComponentCreature)).GetValue<string>("Description");
			if (text.StartsWith("[") && text.EndsWith("]"))
			{
				string[] array = text.Substring(1, text.Length - 2).Split(new string[]
				{
					":"
				}, StringSplitOptions.RemoveEmptyEntries);
				text = LanguageControl.GetDatabase("Description", array[1]);
			}
			this.m_descriptionLabel.Text = text;
			if (this.m_playerClassButton.IsClicked)
			{
				this.m_playerData.PlayerClass = ((this.m_playerData.PlayerClass == PlayerClass.Male) ? PlayerClass.Female : PlayerClass.Male);
				this.m_playerData.RandomizeCharacterSkin();
				if (this.m_playerData.IsDefaultName)
				{
					this.m_playerData.ResetName();
				}
			}
			if (this.m_characterSkinButton.IsClicked)
			{
				CharacterSkinsManager.UpdateCharacterSkinsList();
				IEnumerable<string> items = CharacterSkinsManager.CharacterSkinsNames.Where(delegate(string n)
				{
					PlayerClass? playerClass = CharacterSkinsManager.GetPlayerClass(n);
					PlayerClass playerClass2 = this.m_playerData.PlayerClass;
					return (playerClass.GetValueOrDefault() == playerClass2 & playerClass != null) || CharacterSkinsManager.GetPlayerClass(n) == null;
				});
				ListSelectionDialog dialog = new ListSelectionDialog(LanguageControl.Get(PlayerScreen.fName, 1), items, 64f, delegate(object item)
				{
					XElement node = ContentManager.Get<XElement>("Widgets/CharacterSkinItem");
					ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node, null);
					Texture2D texture = this.m_characterSkinsCache.GetTexture((string)item);
					containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Text", true).Text = CharacterSkinsManager.GetDisplayName((string)item);
					containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Details", true).Text = string.Format("{0}x{1}", texture.Width, texture.Height);
					PlayerModelWidget playerModelWidget = containerWidget.Children.Find<PlayerModelWidget>("CharacterSkinItem.Model", true);
					playerModelWidget.PlayerClass = this.m_playerData.PlayerClass;
					playerModelWidget.CharacterSkinTexture = texture;
					return containerWidget;
				}, delegate(object item)
				{
					this.m_playerData.CharacterSkinName = (string)item;
					if (this.m_playerData.IsDefaultName)
					{
						this.m_playerData.ResetName();
					}
				});
				DialogsManager.ShowDialog(null, dialog);
			}
			if (this.m_controlsButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(PlayerScreen.fName, 2), this.m_inputDevices, 56f, (object d) => PlayerScreen.GetDeviceDisplayName((WidgetInputDevice)d), delegate(object d)
				{
					WidgetInputDevice widgetInputDevice = (WidgetInputDevice)d;
					this.m_playerData.InputDevice = widgetInputDevice;
					foreach (PlayerData playerData in this.m_playerData.SubsystemPlayers.PlayersData)
					{
						if (playerData != this.m_playerData && (playerData.InputDevice & widgetInputDevice) != WidgetInputDevice.None)
						{
							playerData.InputDevice &= ~widgetInputDevice;
						}
					}
				}));
			}
			if (this.m_addButton.IsClicked && this.VerifyName())
			{
				this.m_playerData.SubsystemPlayers.AddPlayerData(this.m_playerData);
				ScreensManager.SwitchScreen("Players", new object[]
				{
					this.m_playerData.SubsystemPlayers
				});
			}
			if (this.m_deleteButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "warning"), LanguageControl.Get(PlayerScreen.fName, 3), LanguageControl.Get("Usual", "ok"), LanguageControl.Get("Usual", "cancel"), delegate(MessageDialogButton b)
				{
					if (b == MessageDialogButton.Button1)
					{
						this.m_playerData.SubsystemPlayers.RemovePlayerData(this.m_playerData);
						ScreensManager.SwitchScreen("Players", new object[]
						{
							this.m_playerData.SubsystemPlayers
						});
					}
				}));
			}
			if (this.m_playButton.IsClicked && this.VerifyName())
			{
				this.m_playerData.SubsystemPlayers.AddPlayerData(this.m_playerData);
				ScreensManager.SwitchScreen("Game", Array.Empty<object>());
			}
			if (this.m_addAnotherButton.IsClicked && this.VerifyName())
			{
				this.m_playerData.SubsystemPlayers.AddPlayerData(this.m_playerData);
				ScreensManager.SwitchScreen("Player", new object[]
				{
					PlayerScreen.Mode.Initial,
					this.m_playerData.SubsystemPlayers.Project
				});
			}
			if ((base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked) && this.VerifyName())
			{
				if (this.m_mode == PlayerScreen.Mode.Initial)
				{
					GameManager.SaveProject(true, true);
					GameManager.DisposeProject();
					ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
				}
				else if (this.m_mode == PlayerScreen.Mode.Add || this.m_mode == PlayerScreen.Mode.Edit)
				{
					ScreensManager.SwitchScreen("Players", new object[]
					{
						this.m_playerData.SubsystemPlayers
					});
				}
			}
			this.m_nameWasInvalid = false;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00022CE0 File Offset: 0x00020EE0
		public static string GetDeviceDisplayName(WidgetInputDevice device)
		{
			if (device <= WidgetInputDevice.GamePad2)
			{
				if (device == (WidgetInputDevice.Keyboard | WidgetInputDevice.Mouse))
				{
					return LanguageControl.Get(PlayerScreen.fName, 4);
				}
				if (device == WidgetInputDevice.GamePad1)
				{
					return LanguageControl.Get(PlayerScreen.fName, 5) + (GamePad.IsConnected(0) ? "" : LanguageControl.Get(PlayerScreen.fName, 9));
				}
				if (device == WidgetInputDevice.GamePad2)
				{
					return LanguageControl.Get(PlayerScreen.fName, 6) + (GamePad.IsConnected(1) ? "" : LanguageControl.Get(PlayerScreen.fName, 9));
				}
			}
			else
			{
				if (device == WidgetInputDevice.GamePad3)
				{
					return LanguageControl.Get(PlayerScreen.fName, 7) + (GamePad.IsConnected(2) ? "" : LanguageControl.Get(PlayerScreen.fName, 9));
				}
				if (device == WidgetInputDevice.GamePad4)
				{
					return LanguageControl.Get(PlayerScreen.fName, 8) + (GamePad.IsConnected(3) ? "" : LanguageControl.Get(PlayerScreen.fName, 9));
				}
				if (device == WidgetInputDevice.VrControllers)
				{
					return LanguageControl.Get(PlayerScreen.fName, 11) + (VrManager.IsVrAvailable ? "" : LanguageControl.Get(PlayerScreen.fName, 9));
				}
			}
			return LanguageControl.Get(PlayerScreen.fName, 10);
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00022E18 File Offset: 0x00021018
		public bool VerifyName()
		{
			if (this.m_nameWasInvalid)
			{
				return false;
			}
			if (PlayerData.VerifyName(this.m_nameTextBox.Text.Trim()))
			{
				return true;
			}
			DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), LanguageControl.Get(PlayerScreen.fName, 12), LanguageControl.Get("Usual", "ok"), null, null));
			return false;
		}

		// Token: 0x040002DA RID: 730
		public PlayerData m_playerData;

		// Token: 0x040002DB RID: 731
		public PlayerScreen.Mode m_mode;

		// Token: 0x040002DC RID: 732
		public CharacterSkinsCache m_characterSkinsCache;

		// Token: 0x040002DD RID: 733
		public bool m_nameWasInvalid;

		// Token: 0x040002DE RID: 734
		public PlayerModelWidget m_playerModel;

		// Token: 0x040002DF RID: 735
		public ButtonWidget m_playerClassButton;

		// Token: 0x040002E0 RID: 736
		public TextBoxWidget m_nameTextBox;

		// Token: 0x040002E1 RID: 737
		public LabelWidget m_characterSkinLabel;

		// Token: 0x040002E2 RID: 738
		public ButtonWidget m_characterSkinButton;

		// Token: 0x040002E3 RID: 739
		public LabelWidget m_controlsLabel;

		// Token: 0x040002E4 RID: 740
		public ButtonWidget m_controlsButton;

		// Token: 0x040002E5 RID: 741
		public LabelWidget m_descriptionLabel;

		// Token: 0x040002E6 RID: 742
		public ButtonWidget m_addButton;

		// Token: 0x040002E7 RID: 743
		public ButtonWidget m_addAnotherButton;

		// Token: 0x040002E8 RID: 744
		public ButtonWidget m_deleteButton;

		// Token: 0x040002E9 RID: 745
		public ButtonWidget m_playButton;

		// Token: 0x040002EA RID: 746
		public static string fName = "PlayerScreen";

		// Token: 0x040002EB RID: 747
		public WidgetInputDevice[] m_inputDevices = new WidgetInputDevice[]
		{
			WidgetInputDevice.None,
			WidgetInputDevice.GamePad1,
			WidgetInputDevice.GamePad2,
			WidgetInputDevice.GamePad3,
			WidgetInputDevice.GamePad4
		};

		// Token: 0x020003FE RID: 1022
		public enum Mode
		{
			// Token: 0x040014E7 RID: 5351
			Initial,
			// Token: 0x040014E8 RID: 5352
			Add,
			// Token: 0x040014E9 RID: 5353
			Edit
		}
	}
}
