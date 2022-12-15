using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200013D RID: 317
	public class ModifyWorldScreen : Screen
	{
		// Token: 0x060005E7 RID: 1511 RVA: 0x00021A58 File Offset: 0x0001FC58
		public ModifyWorldScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ModifyWorldScreen");
			base.LoadContents(this, node);
			this.m_nameTextBox = this.Children.Find<TextBoxWidget>("Name", true);
			this.m_seedLabel = this.Children.Find<LabelWidget>("Seed", true);
			this.m_gameModeButton = this.Children.Find<ButtonWidget>("GameMode", true);
			this.m_worldOptionsButton = this.Children.Find<ButtonWidget>("WorldOptions", true);
			this.m_errorLabel = this.Children.Find<LabelWidget>("Error", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_applyButton = this.Children.Find<ButtonWidget>("Apply", true);
			this.m_deleteButton = this.Children.Find<ButtonWidget>("Delete", true);
			this.m_uploadButton = this.Children.Find<ButtonWidget>("Upload", true);
			this.m_nameTextBox.TextChanged += delegate(TextBoxWidget p)
			{
				this.m_worldSettings.Name = this.m_nameTextBox.Text;
			};
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00021B7C File Offset: 0x0001FD7C
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen.GetType() != typeof(WorldOptionsScreen))
			{
				this.m_directoryName = (string)parameters[0];
				this.m_worldSettings = (WorldSettings)parameters[1];
				this.m_originalWorldSettingsData.Clear();
				this.m_worldSettings.Save(this.m_originalWorldSettingsData, true);
			}
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00021BE0 File Offset: 0x0001FDE0
		public override void Update()
		{
			if (this.m_gameModeButton.IsClicked && this.m_worldSettings.GameMode != GameMode.Cruel)
			{
				ReadOnlyList<int> enumValues = EnumUtils.GetEnumValues(typeof(GameMode));
				do
				{
					this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				}
				while (this.m_worldSettings.GameMode == GameMode.Cruel);
				this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			}
			this.m_currentWorldSettingsData.Clear();
			this.m_worldSettings.Save(this.m_currentWorldSettingsData, true);
			bool flag = !ModifyWorldScreen.CompareValueDictionaries(this.m_originalWorldSettingsData, this.m_currentWorldSettingsData);
			bool flag2 = WorldsManager.ValidateWorldName(this.m_worldSettings.Name);
			this.m_nameTextBox.Text = this.m_worldSettings.Name;
			this.m_seedLabel.Text = this.m_worldSettings.Seed;
			this.m_gameModeButton.Text = LanguageControl.Get("GameMode", this.m_worldSettings.GameMode.ToString());
			this.m_gameModeButton.IsEnabled = (this.m_worldSettings.GameMode != GameMode.Cruel);
			this.m_errorLabel.IsVisible = !flag2;
			this.m_descriptionLabel.IsVisible = flag2;
			this.m_uploadButton.IsEnabled = (flag2 && !flag);
			this.m_applyButton.IsEnabled = (flag2 && flag);
			this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			if (this.m_worldOptionsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("WorldOptions", new object[]
				{
					this.m_worldSettings,
					true
				});
			}
			if (this.m_deleteButton.IsClicked)
			{
				MessageDialog dialog = null;
				dialog = new MessageDialog(LanguageControl.Get(ModifyWorldScreen.fName, 1), LanguageControl.Get(ModifyWorldScreen.fName, 2), LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						WorldsManager.DeleteWorld(this.m_directoryName);
						ScreensManager.SwitchScreen("Play", Array.Empty<object>());
						DialogsManager.HideDialog(dialog);
						return;
					}
					DialogsManager.HideDialog(dialog);
				});
				dialog.AutoHide = false;
				DialogsManager.ShowDialog(null, dialog);
			}
			if (this.m_uploadButton.IsClicked && flag2 && !flag)
			{
				ExternalContentManager.ShowUploadUi(ExternalContentType.World, this.m_directoryName);
			}
			if (this.m_applyButton.IsClicked && flag2 && flag)
			{
				if (this.m_worldSettings.GameMode != GameMode.Creative && this.m_worldSettings.GameMode != GameMode.Adventure)
				{
					this.m_worldSettings.ResetOptionsForNonCreativeMode();
				}
				WorldsManager.ChangeWorld(this.m_directoryName, this.m_worldSettings);
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				if (flag)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModifyWorldScreen.fName, 3), LanguageControl.Get(ModifyWorldScreen.fName, 4), LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton button)
					{
						if (button == MessageDialogButton.Button1)
						{
							ScreensManager.SwitchScreen("Play", Array.Empty<object>());
						}
					}));
					return;
				}
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00021F78 File Offset: 0x00020178
		public static bool CompareValueDictionaries(ValuesDictionary d1, ValuesDictionary d2)
		{
			if (d1.Count != d2.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, object> keyValuePair in d1)
			{
				object value = d2.GetValue<object>(keyValuePair.Key, null);
				ValuesDictionary valuesDictionary = value as ValuesDictionary;
				if (valuesDictionary != null)
				{
					ValuesDictionary valuesDictionary2 = keyValuePair.Value as ValuesDictionary;
					if (valuesDictionary2 == null || !ModifyWorldScreen.CompareValueDictionaries(valuesDictionary, valuesDictionary2))
					{
						return false;
					}
				}
				else if (!object.Equals(value, keyValuePair.Value))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040002C0 RID: 704
		public TextBoxWidget m_nameTextBox;

		// Token: 0x040002C1 RID: 705
		public LabelWidget m_seedLabel;

		// Token: 0x040002C2 RID: 706
		public ButtonWidget m_gameModeButton;

		// Token: 0x040002C3 RID: 707
		public ButtonWidget m_worldOptionsButton;

		// Token: 0x040002C4 RID: 708
		public LabelWidget m_errorLabel;

		// Token: 0x040002C5 RID: 709
		public LabelWidget m_descriptionLabel;

		// Token: 0x040002C6 RID: 710
		public ButtonWidget m_applyButton;

		// Token: 0x040002C7 RID: 711
		public ButtonWidget m_deleteButton;

		// Token: 0x040002C8 RID: 712
		public ButtonWidget m_uploadButton;

		// Token: 0x040002C9 RID: 713
		public string m_directoryName;

		// Token: 0x040002CA RID: 714
		public WorldSettings m_worldSettings;

		// Token: 0x040002CB RID: 715
		public ValuesDictionary m_currentWorldSettingsData = new ValuesDictionary();

		// Token: 0x040002CC RID: 716
		public ValuesDictionary m_originalWorldSettingsData = new ValuesDictionary();

		// Token: 0x040002CD RID: 717
		public static string fName = "ModifyWorldScreen";
	}
}
