using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200013F RID: 319
	public class NewWorldScreen : Screen
	{
		// Token: 0x060005F1 RID: 1521 RVA: 0x0002214C File Offset: 0x0002034C
		public NewWorldScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/NewWorldScreen");
			base.LoadContents(this, node);
			this.m_nameTextBox = this.Children.Find<TextBoxWidget>("Name", true);
			this.m_seedTextBox = this.Children.Find<TextBoxWidget>("Seed", true);
			this.m_gameModeButton = this.Children.Find<ButtonWidget>("GameMode", true);
			this.m_startingPositionButton = this.Children.Find<ButtonWidget>("StartingPosition", true);
			this.m_worldOptionsButton = this.Children.Find<ButtonWidget>("WorldOptions", true);
			this.m_blankSeedLabel = this.Children.Find<LabelWidget>("BlankSeed", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_errorLabel = this.Children.Find<LabelWidget>("Error", true);
			this.m_playButton = this.Children.Find<ButtonWidget>("Play", true);
			this.m_nameTextBox.TextChanged += delegate(TextBoxWidget p)
			{
				this.m_worldSettings.Name = this.m_nameTextBox.Text;
			};
			this.m_seedTextBox.TextChanged += delegate(TextBoxWidget p)
			{
				this.m_worldSettings.Seed = this.m_seedTextBox.Text;
			};
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0002227C File Offset: 0x0002047C
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen.GetType() != typeof(WorldOptionsScreen))
			{
				this.m_worldSettings = new WorldSettings
				{
					Name = WorldsManager.NewWorldNames[this.m_random.Int(0, WorldsManager.NewWorldNames.Count - 1)],
					OriginalSerializationVersion = VersionsManager.SerializationVersion
				};
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000222E8 File Offset: 0x000204E8
		public override void Update()
		{
			if (this.m_gameModeButton.IsClicked)
			{
				ReadOnlyList<int> enumValues = EnumUtils.GetEnumValues(typeof(GameMode));
				this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				while (this.m_worldSettings.GameMode == GameMode.Adventure)
				{
					this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				}
			}
			if (this.m_startingPositionButton.IsClicked)
			{
				ReadOnlyList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(StartingPositionMode));
				this.m_worldSettings.StartingPositionMode = (StartingPositionMode)((enumValues2.IndexOf((int)this.m_worldSettings.StartingPositionMode) + 1) % enumValues2.Count);
			}
			bool flag = WorldsManager.ValidateWorldName(this.m_worldSettings.Name);
			this.m_nameTextBox.Text = this.m_worldSettings.Name;
			this.m_seedTextBox.Text = this.m_worldSettings.Seed;
			this.m_gameModeButton.Text = LanguageControl.Get("GameMode", this.m_worldSettings.GameMode.ToString());
			this.m_startingPositionButton.Text = LanguageControl.Get("StartingPositionMode", this.m_worldSettings.StartingPositionMode.ToString());
			this.m_playButton.IsVisible = flag;
			this.m_errorLabel.IsVisible = !flag;
			this.m_blankSeedLabel.IsVisible = (this.m_worldSettings.Seed.Length == 0 && !this.m_seedTextBox.HasFocus);
			this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			if (this.m_worldOptionsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("WorldOptions", new object[]
				{
					this.m_worldSettings,
					false
				});
			}
			if (this.m_playButton.IsClicked && WorldsManager.ValidateWorldName(this.m_nameTextBox.Text))
			{
				if (this.m_worldSettings.GameMode != GameMode.Creative)
				{
					this.m_worldSettings.ResetOptionsForNonCreativeMode();
				}
				WorldInfo worldInfo = WorldsManager.CreateWorld(this.m_worldSettings);
				string name = "GameLoading";
				object[] array = new object[2];
				array[0] = worldInfo;
				ScreensManager.SwitchScreen(name, array);
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
		}

		// Token: 0x040002CE RID: 718
		public TextBoxWidget m_nameTextBox;

		// Token: 0x040002CF RID: 719
		public TextBoxWidget m_seedTextBox;

		// Token: 0x040002D0 RID: 720
		public ButtonWidget m_gameModeButton;

		// Token: 0x040002D1 RID: 721
		public ButtonWidget m_startingPositionButton;

		// Token: 0x040002D2 RID: 722
		public ButtonWidget m_worldOptionsButton;

		// Token: 0x040002D3 RID: 723
		public LabelWidget m_blankSeedLabel;

		// Token: 0x040002D4 RID: 724
		public LabelWidget m_descriptionLabel;

		// Token: 0x040002D5 RID: 725
		public LabelWidget m_errorLabel;

		// Token: 0x040002D6 RID: 726
		public static string fName = "NewWorldScreen";

		// Token: 0x040002D7 RID: 727
		public ButtonWidget m_playButton;

		// Token: 0x040002D8 RID: 728
		public Random m_random = new Random();

		// Token: 0x040002D9 RID: 729
		public WorldSettings m_worldSettings;
	}
}
