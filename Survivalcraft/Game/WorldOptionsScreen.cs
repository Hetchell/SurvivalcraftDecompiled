using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000155 RID: 341
	public class WorldOptionsScreen : Screen
	{
		// Token: 0x06000677 RID: 1655 RVA: 0x00028688 File Offset: 0x00026888
		public WorldOptionsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/WorldOptionsScreen");
			base.LoadContents(this, node);
			this.m_creativeModePanel = this.Children.Find<Widget>("CreativeModePanel", true);
			this.m_newWorldOnlyPanel = this.Children.Find<Widget>("NewWorldOnlyPanel", true);
			this.m_continentTerrainPanel = this.Children.Find<Widget>("ContinentTerrainPanel", true);
			this.m_islandTerrainPanel = this.Children.Find<Widget>("IslandTerrainPanel", true);
			this.m_islandSizeNS = this.Children.Find<SliderWidget>("IslandSizeNS", true);
			this.m_islandSizeEW = this.Children.Find<SliderWidget>("IslandSizeEW", true);
			this.m_flatTerrainPanel = this.Children.Find<Widget>("FlatTerrainPanel", true);
			this.m_blocksTextureIcon = this.Children.Find<RectangleWidget>("BlocksTextureIcon", true);
			this.m_blocksTextureLabel = this.Children.Find<LabelWidget>("BlocksTextureLabel", true);
			this.m_blocksTextureDetails = this.Children.Find<LabelWidget>("BlocksTextureDetails", true);
			this.m_blocksTextureButton = this.Children.Find<ButtonWidget>("BlocksTextureButton", true);
			this.m_seaLevelOffsetSlider = this.Children.Find<SliderWidget>("SeaLevelOffset", true);
			this.m_temperatureOffsetSlider = this.Children.Find<SliderWidget>("TemperatureOffset", true);
			this.m_humidityOffsetSlider = this.Children.Find<SliderWidget>("HumidityOffset", true);
			this.m_biomeSizeSlider = this.Children.Find<SliderWidget>("BiomeSize", true);
			this.m_paletteButton = this.Children.Find<ButtonWidget>("Palette", true);
			this.m_supernaturalCreaturesButton = this.Children.Find<ButtonWidget>("SupernaturalCreatures", true);
			this.m_friendlyFireButton = this.Children.Find<ButtonWidget>("FriendlyFire", true);
			this.m_environmentBehaviorButton = this.Children.Find<ButtonWidget>("EnvironmentBehavior", true);
			this.m_timeOfDayButton = this.Children.Find<ButtonWidget>("TimeOfDay", true);
			this.m_weatherEffectsButton = this.Children.Find<ButtonWidget>("WeatherEffects", true);
			this.m_adventureRespawnButton = this.Children.Find<ButtonWidget>("AdventureRespawn", true);
			this.m_adventureSurvivalMechanicsButton = this.Children.Find<ButtonWidget>("AdventureSurvivalMechanics", true);
			this.m_terrainGenerationButton = this.Children.Find<ButtonWidget>("TerrainGeneration", true);
			this.m_flatTerrainLevelSlider = this.Children.Find<SliderWidget>("FlatTerrainLevel", true);
			this.m_flatTerrainShoreRoughnessSlider = this.Children.Find<SliderWidget>("FlatTerrainShoreRoughness", true);
			this.m_flatTerrainBlock = this.Children.Find<BlockIconWidget>("FlatTerrainBlock", true);
			this.m_flatTerrainBlockLabel = this.Children.Find<LabelWidget>("FlatTerrainBlockLabel", true);
			this.m_flatTerrainBlockButton = this.Children.Find<ButtonWidget>("FlatTerrainBlockButton", true);
			this.m_flatTerrainMagmaOceanCheckbox = this.Children.Find<CheckboxWidget>("MagmaOcean", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_islandSizeEW.MinValue = 0f;
			this.m_islandSizeEW.MaxValue = (float)(WorldOptionsScreen.m_islandSizes.Length - 1);
			this.m_islandSizeEW.Granularity = 1f;
			this.m_islandSizeNS.MinValue = 0f;
			this.m_islandSizeNS.MaxValue = (float)(WorldOptionsScreen.m_islandSizes.Length - 1);
			this.m_islandSizeNS.Granularity = 1f;
			this.m_biomeSizeSlider.MinValue = 0f;
			this.m_biomeSizeSlider.MaxValue = (float)(WorldOptionsScreen.m_biomeSizes.Length - 1);
			this.m_biomeSizeSlider.Granularity = 1f;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00028A21 File Offset: 0x00026C21
		public static string FormatOffset(float value)
		{
			if (value != 0f)
			{
				return ((value >= 0f) ? "+" : "") + value.ToString();
			}
			return LanguageControl.Get(WorldOptionsScreen.fName, 6);
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00028A58 File Offset: 0x00026C58
		public override void Enter(object[] parameters)
		{
			this.m_worldSettings = (WorldSettings)parameters[0];
			this.m_isExistingWorld = (bool)parameters[1];
			this.m_descriptionLabel.Text = StringsManager.GetString("EnvironmentBehaviorMode." + this.m_worldSettings.EnvironmentBehaviorMode.ToString() + ".Description");
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00028AB6 File Offset: 0x00026CB6
		public override void Leave()
		{
			this.m_blockTexturesCache.Clear();
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x00028AC4 File Offset: 0x00026CC4
		public override void Update()
		{
			if (this.m_terrainGenerationButton.IsClicked && !this.m_isExistingWorld)
			{
				ReadOnlyList<int> enumValues = EnumUtils.GetEnumValues(typeof(TerrainGenerationMode));
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(WorldOptionsScreen.fName, 1), enumValues, 56f, (object e) => StringsManager.GetString("TerrainGenerationMode." + ((TerrainGenerationMode)e).ToString() + ".Name"), delegate(object e)
				{
					if (this.m_worldSettings.GameMode != GameMode.Creative && ((TerrainGenerationMode)e == TerrainGenerationMode.FlatContinent || (TerrainGenerationMode)e == TerrainGenerationMode.FlatIsland))
					{
						DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(WorldOptionsScreen.fName, 4), LanguageControl.Get(WorldOptionsScreen.fName, 5), LanguageControl.Get("Usual", "ok"), null, null));
						return;
					}
					this.m_worldSettings.TerrainGenerationMode = (TerrainGenerationMode)e;
					this.m_descriptionLabel.Text = StringsManager.GetString("TerrainGenerationMode." + this.m_worldSettings.TerrainGenerationMode.ToString() + ".Description");
				}));
			}
			if (this.m_islandSizeEW.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.IslandSize.X = WorldOptionsScreen.m_islandSizes[MathUtils.Clamp((int)this.m_islandSizeEW.Value, 0, WorldOptionsScreen.m_islandSizes.Length - 1)];
			}
			if (this.m_islandSizeNS.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.IslandSize.Y = WorldOptionsScreen.m_islandSizes[MathUtils.Clamp((int)this.m_islandSizeNS.Value, 0, WorldOptionsScreen.m_islandSizes.Length - 1)];
			}
			if (this.m_flatTerrainLevelSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.TerrainLevel = MathUtils.Clamp((int)this.m_flatTerrainLevelSlider.Value / (int)this.m_flatTerrainLevelSlider.Granularity * (int)this.m_flatTerrainLevelSlider.Granularity, 2, 252);
				this.m_descriptionLabel.Text = StringsManager.GetString("FlatTerrainLevel.Description");
			}
			if (this.m_flatTerrainShoreRoughnessSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.ShoreRoughness = this.m_flatTerrainShoreRoughnessSlider.Value;
				this.m_descriptionLabel.Text = StringsManager.GetString("FlatTerrainShoreRoughness.Description");
			}
			if (this.m_flatTerrainBlockButton.IsClicked && !this.m_isExistingWorld)
			{
				int[] items = new int[]
				{
					8,
					2,
					7,
					3,
					67,
					66,
					4,
					5,
					26,
					73,
					21,
					46,
					47,
					15,
					62,
					68,
					126,
					71,
					1
				};
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(WorldOptionsScreen.fName, 2), items, 72f, delegate(object index)
				{
					XElement node = ContentManager.Get<XElement>("Widgets/SelectBlockItem");
					ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(null, node, null);
					containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
					containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));
					return containerWidget;
				}, delegate(object index)
				{
					this.m_worldSettings.TerrainBlockIndex = (int)index;
				}));
			}
			if (this.m_flatTerrainMagmaOceanCheckbox.IsClicked)
			{
				this.m_worldSettings.TerrainOceanBlockIndex = ((this.m_worldSettings.TerrainOceanBlockIndex == 18) ? 92 : 18);
				this.m_descriptionLabel.Text = StringsManager.GetString("FlatTerrainMagmaOcean.Description");
			}
			if (this.m_seaLevelOffsetSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.SeaLevelOffset = (int)this.m_seaLevelOffsetSlider.Value;
				this.m_descriptionLabel.Text = StringsManager.GetString("SeaLevelOffset.Description");
			}
			if (this.m_temperatureOffsetSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.TemperatureOffset = this.m_temperatureOffsetSlider.Value;
				this.m_descriptionLabel.Text = StringsManager.GetString("TemperatureOffset.Description");
			}
			if (this.m_humidityOffsetSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.HumidityOffset = this.m_humidityOffsetSlider.Value;
				this.m_descriptionLabel.Text = StringsManager.GetString("HumidityOffset.Description");
			}
			if (this.m_biomeSizeSlider.IsSliding && !this.m_isExistingWorld)
			{
				this.m_worldSettings.BiomeSize = WorldOptionsScreen.m_biomeSizes[MathUtils.Clamp((int)this.m_biomeSizeSlider.Value, 0, WorldOptionsScreen.m_biomeSizes.Length - 1)];
				this.m_descriptionLabel.Text = StringsManager.GetString("BiomeSize.Description");
			}
			if (this.m_blocksTextureButton.IsClicked)
			{
				BlocksTexturesManager.UpdateBlocksTexturesList();
				ListSelectionDialog dialog = new ListSelectionDialog(LanguageControl.Get(WorldOptionsScreen.fName, 3), BlocksTexturesManager.BlockTexturesNames, 64f, delegate(object item)
				{
					XElement node = ContentManager.Get<XElement>("Widgets/BlocksTextureItem");
					ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node, null);
					Texture2D texture2 = this.m_blockTexturesCache.GetTexture((string)item);
					containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Text", true).Text = BlocksTexturesManager.GetDisplayName((string)item);
					containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Details", true).Text = string.Format("{0}x{1}", texture2.Width, texture2.Height);
					containerWidget.Children.Find<RectangleWidget>("BlocksTextureItem.Icon", true).Subtexture = new Subtexture(texture2, Vector2.Zero, Vector2.One);
					return containerWidget;
				}, delegate(object item)
				{
					this.m_worldSettings.BlocksTextureName = (string)item;
				});
				DialogsManager.ShowDialog(null, dialog);
				this.m_descriptionLabel.Text = StringsManager.GetString("BlocksTexture.Description");
			}
			if (this.m_paletteButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new EditPaletteDialog(this.m_worldSettings.Palette));
			}
			if (this.m_supernaturalCreaturesButton.IsClicked)
			{
				this.m_worldSettings.AreSupernaturalCreaturesEnabled = !this.m_worldSettings.AreSupernaturalCreaturesEnabled;
				this.m_descriptionLabel.Text = StringsManager.GetString("SupernaturalCreatures." + this.m_worldSettings.AreSupernaturalCreaturesEnabled.ToString());
			}
			if (this.m_friendlyFireButton.IsClicked)
			{
				this.m_worldSettings.IsFriendlyFireEnabled = !this.m_worldSettings.IsFriendlyFireEnabled;
				this.m_descriptionLabel.Text = StringsManager.GetString("FriendlyFire." + this.m_worldSettings.IsFriendlyFireEnabled.ToString());
			}
			if (this.m_environmentBehaviorButton.IsClicked)
			{
				ReadOnlyList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(EnvironmentBehaviorMode));
				this.m_worldSettings.EnvironmentBehaviorMode = (EnvironmentBehaviorMode)((enumValues2.IndexOf((int)this.m_worldSettings.EnvironmentBehaviorMode) + 1) % enumValues2.Count);
				this.m_descriptionLabel.Text = StringsManager.GetString("EnvironmentBehaviorMode." + this.m_worldSettings.EnvironmentBehaviorMode.ToString() + ".Description");
			}
			if (this.m_timeOfDayButton.IsClicked)
			{
				ReadOnlyList<int> enumValues3 = EnumUtils.GetEnumValues(typeof(TimeOfDayMode));
				this.m_worldSettings.TimeOfDayMode = (TimeOfDayMode)((enumValues3.IndexOf((int)this.m_worldSettings.TimeOfDayMode) + 1) % enumValues3.Count);
				this.m_descriptionLabel.Text = StringsManager.GetString("TimeOfDayMode." + this.m_worldSettings.TimeOfDayMode.ToString() + ".Description");
			}
			if (this.m_weatherEffectsButton.IsClicked)
			{
				this.m_worldSettings.AreWeatherEffectsEnabled = !this.m_worldSettings.AreWeatherEffectsEnabled;
				this.m_descriptionLabel.Text = StringsManager.GetString("WeatherMode." + this.m_worldSettings.AreWeatherEffectsEnabled.ToString());
			}
			if (this.m_adventureRespawnButton.IsClicked)
			{
				this.m_worldSettings.IsAdventureRespawnAllowed = !this.m_worldSettings.IsAdventureRespawnAllowed;
				this.m_descriptionLabel.Text = StringsManager.GetString("AdventureRespawnMode." + this.m_worldSettings.IsAdventureRespawnAllowed.ToString());
			}
			if (this.m_adventureSurvivalMechanicsButton.IsClicked)
			{
				this.m_worldSettings.AreAdventureSurvivalMechanicsEnabled = !this.m_worldSettings.AreAdventureSurvivalMechanicsEnabled;
				this.m_descriptionLabel.Text = StringsManager.GetString("AdventureSurvivalMechanics." + this.m_worldSettings.AreAdventureSurvivalMechanicsEnabled.ToString());
			}
			this.m_creativeModePanel.IsVisible = (this.m_worldSettings.GameMode == GameMode.Creative);
			this.m_newWorldOnlyPanel.IsVisible = !this.m_isExistingWorld;
			this.m_continentTerrainPanel.IsVisible = (this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Continent || this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatContinent);
			this.m_islandTerrainPanel.IsVisible = (this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island || this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatIsland);
			this.m_flatTerrainPanel.IsVisible = (this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatContinent || this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatIsland);
			this.m_terrainGenerationButton.Text = StringsManager.GetString("TerrainGenerationMode." + this.m_worldSettings.TerrainGenerationMode.ToString() + ".Name");
			this.m_islandSizeEW.Value = (float)WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_islandSizes, this.m_worldSettings.IslandSize.X);
			this.m_islandSizeEW.Text = this.m_worldSettings.IslandSize.X.ToString();
			this.m_islandSizeNS.Value = (float)WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_islandSizes, this.m_worldSettings.IslandSize.Y);
			this.m_islandSizeNS.Text = this.m_worldSettings.IslandSize.Y.ToString();
			this.m_flatTerrainLevelSlider.Value = (float)this.m_worldSettings.TerrainLevel;
			this.m_flatTerrainLevelSlider.Text = this.m_worldSettings.TerrainLevel.ToString();
			this.m_flatTerrainShoreRoughnessSlider.Value = this.m_worldSettings.ShoreRoughness;
			this.m_flatTerrainShoreRoughnessSlider.Text = string.Format("{0:0}%", this.m_worldSettings.ShoreRoughness * 100f);
			this.m_flatTerrainBlock.Contents = this.m_worldSettings.TerrainBlockIndex;
			this.m_flatTerrainMagmaOceanCheckbox.IsChecked = (this.m_worldSettings.TerrainOceanBlockIndex == 92);
			string text = (BlocksManager.Blocks[this.m_worldSettings.TerrainBlockIndex] != null) ? BlocksManager.Blocks[this.m_worldSettings.TerrainBlockIndex].GetDisplayName(null, Terrain.MakeBlockValue(this.m_worldSettings.TerrainBlockIndex)) : string.Empty;
			this.m_flatTerrainBlockLabel.Text = ((text.Length > 10) ? (text.Substring(0, 10) + "...") : text);
			Texture2D texture = this.m_blockTexturesCache.GetTexture(this.m_worldSettings.BlocksTextureName);
			this.m_blocksTextureIcon.Subtexture = new Subtexture(texture, Vector2.Zero, Vector2.One);
			this.m_blocksTextureLabel.Text = BlocksTexturesManager.GetDisplayName(this.m_worldSettings.BlocksTextureName);
			this.m_blocksTextureDetails.Text = string.Format("{0}x{1}", texture.Width, texture.Height);
			this.m_seaLevelOffsetSlider.Value = (float)this.m_worldSettings.SeaLevelOffset;
			this.m_seaLevelOffsetSlider.Text = WorldOptionsScreen.FormatOffset((float)this.m_worldSettings.SeaLevelOffset);
			this.m_temperatureOffsetSlider.Value = this.m_worldSettings.TemperatureOffset;
			this.m_temperatureOffsetSlider.Text = WorldOptionsScreen.FormatOffset(this.m_worldSettings.TemperatureOffset);
			this.m_humidityOffsetSlider.Value = this.m_worldSettings.HumidityOffset;
			this.m_humidityOffsetSlider.Text = WorldOptionsScreen.FormatOffset(this.m_worldSettings.HumidityOffset);
			this.m_biomeSizeSlider.Value = (float)WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_biomeSizes, this.m_worldSettings.BiomeSize);
			this.m_biomeSizeSlider.Text = this.m_worldSettings.BiomeSize.ToString() + "x";
			this.m_environmentBehaviorButton.Text = LanguageControl.Get("EnvironmentBehaviorMode", this.m_worldSettings.EnvironmentBehaviorMode.ToString());
			this.m_timeOfDayButton.Text = LanguageControl.Get("TimeOfDayMode", this.m_worldSettings.TimeOfDayMode.ToString());
			this.m_weatherEffectsButton.Text = (this.m_worldSettings.AreWeatherEffectsEnabled ? LanguageControl.Get("Usual", "enable") : LanguageControl.Get("Usual", "disable"));
			this.m_adventureRespawnButton.Text = (this.m_worldSettings.IsAdventureRespawnAllowed ? LanguageControl.Get("Usual", "allowed") : LanguageControl.Get("Usual", "not allowed"));
			this.m_adventureSurvivalMechanicsButton.Text = (this.m_worldSettings.AreAdventureSurvivalMechanicsEnabled ? LanguageControl.Get("Usual", "enable") : LanguageControl.Get("Usual", "disable"));
			this.m_supernaturalCreaturesButton.Text = (this.m_worldSettings.AreSupernaturalCreaturesEnabled ? LanguageControl.Get("Usual", "enable") : LanguageControl.Get("Usual", "disable"));
			this.m_friendlyFireButton.Text = (this.m_worldSettings.IsFriendlyFireEnabled ? LanguageControl.Get("Usual", "allowed") : LanguageControl.Get("Usual", "not allowed"));
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x000296C4 File Offset: 0x000278C4
		public static int FindNearestIndex(IList<float> list, float v)
		{
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (MathUtils.Abs(list[i] - v) < MathUtils.Abs(list[num] - v))
				{
					num = i;
				}
			}
			return num;
		}

		// Token: 0x04000382 RID: 898
		public static string fName = "WorldOptionsScreen";

		// Token: 0x04000383 RID: 899
		public Widget m_newWorldOnlyPanel;

		// Token: 0x04000384 RID: 900
		public ButtonWidget m_terrainGenerationButton;

		// Token: 0x04000385 RID: 901
		public Widget m_continentTerrainPanel;

		// Token: 0x04000386 RID: 902
		public Widget m_islandTerrainPanel;

		// Token: 0x04000387 RID: 903
		public SliderWidget m_islandSizeEW;

		// Token: 0x04000388 RID: 904
		public SliderWidget m_islandSizeNS;

		// Token: 0x04000389 RID: 905
		public Widget m_flatTerrainPanel;

		// Token: 0x0400038A RID: 906
		public SliderWidget m_flatTerrainLevelSlider;

		// Token: 0x0400038B RID: 907
		public SliderWidget m_flatTerrainShoreRoughnessSlider;

		// Token: 0x0400038C RID: 908
		public BlockIconWidget m_flatTerrainBlock;

		// Token: 0x0400038D RID: 909
		public LabelWidget m_flatTerrainBlockLabel;

		// Token: 0x0400038E RID: 910
		public ButtonWidget m_flatTerrainBlockButton;

		// Token: 0x0400038F RID: 911
		public CheckboxWidget m_flatTerrainMagmaOceanCheckbox;

		// Token: 0x04000390 RID: 912
		public SliderWidget m_seaLevelOffsetSlider;

		// Token: 0x04000391 RID: 913
		public SliderWidget m_temperatureOffsetSlider;

		// Token: 0x04000392 RID: 914
		public SliderWidget m_humidityOffsetSlider;

		// Token: 0x04000393 RID: 915
		public SliderWidget m_biomeSizeSlider;

		// Token: 0x04000394 RID: 916
		public RectangleWidget m_blocksTextureIcon;

		// Token: 0x04000395 RID: 917
		public LabelWidget m_blocksTextureLabel;

		// Token: 0x04000396 RID: 918
		public LabelWidget m_blocksTextureDetails;

		// Token: 0x04000397 RID: 919
		public ButtonWidget m_blocksTextureButton;

		// Token: 0x04000398 RID: 920
		public ButtonWidget m_paletteButton;

		// Token: 0x04000399 RID: 921
		public ButtonWidget m_supernaturalCreaturesButton;

		// Token: 0x0400039A RID: 922
		public ButtonWidget m_friendlyFireButton;

		// Token: 0x0400039B RID: 923
		public Widget m_creativeModePanel;

		// Token: 0x0400039C RID: 924
		public ButtonWidget m_environmentBehaviorButton;

		// Token: 0x0400039D RID: 925
		public ButtonWidget m_timeOfDayButton;

		// Token: 0x0400039E RID: 926
		public ButtonWidget m_weatherEffectsButton;

		// Token: 0x0400039F RID: 927
		public ButtonWidget m_adventureRespawnButton;

		// Token: 0x040003A0 RID: 928
		public ButtonWidget m_adventureSurvivalMechanicsButton;

		// Token: 0x040003A1 RID: 929
		public LabelWidget m_descriptionLabel;

		// Token: 0x040003A2 RID: 930
		public WorldSettings m_worldSettings;

		// Token: 0x040003A3 RID: 931
		public bool m_isExistingWorld;

		// Token: 0x040003A4 RID: 932
		public BlocksTexturesCache m_blockTexturesCache = new BlocksTexturesCache();

		// Token: 0x040003A5 RID: 933
		public static float[] m_islandSizes = new float[]
		{
			30f,
			40f,
			50f,
			60f,
			80f,
			100f,
			120f,
			150f,
			200f,
			250f,
			300f,
			400f,
			500f,
			600f,
			800f,
			1000f,
			1200f,
			1500f,
			2000f,
			2500f
		};

		// Token: 0x040003A6 RID: 934
		public static float[] m_biomeSizes = new float[]
		{
			0.25f,
			0.33f,
			0.5f,
			0.75f,
			1f,
			1.5f,
			2f,
			3f,
			4f
		};
	}
}
