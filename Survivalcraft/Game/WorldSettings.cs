using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200035D RID: 861
	public class WorldSettings
	{
		// Token: 0x0600182A RID: 6186 RVA: 0x000BEE78 File Offset: 0x000BD078
		public void ResetOptionsForNonCreativeMode()
		{
			if (this.TerrainGenerationMode == TerrainGenerationMode.FlatContinent)
			{
				this.TerrainGenerationMode = TerrainGenerationMode.Continent;
			}
			if (this.TerrainGenerationMode == TerrainGenerationMode.FlatIsland)
			{
				this.TerrainGenerationMode = TerrainGenerationMode.Island;
			}
			this.EnvironmentBehaviorMode = EnvironmentBehaviorMode.Living;
			this.TimeOfDayMode = TimeOfDayMode.Changing;
			this.AreWeatherEffectsEnabled = true;
			this.IsAdventureRespawnAllowed = true;
			this.AreAdventureSurvivalMechanicsEnabled = true;
			this.TerrainLevel = 64;
			this.ShoreRoughness = 0.5f;
			this.TerrainBlockIndex = 8;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000BEEE4 File Offset: 0x000BD0E4
		public void Load(ValuesDictionary valuesDictionary)
		{
			this.Name = valuesDictionary.GetValue<string>("WorldName");
			this.OriginalSerializationVersion = valuesDictionary.GetValue<string>("OriginalSerializationVersion", string.Empty);
			this.Seed = valuesDictionary.GetValue<string>("WorldSeedString", string.Empty);
			this.GameMode = valuesDictionary.GetValue<GameMode>("GameMode", GameMode.Challenging);
			this.EnvironmentBehaviorMode = valuesDictionary.GetValue<EnvironmentBehaviorMode>("EnvironmentBehaviorMode", EnvironmentBehaviorMode.Living);
			this.TimeOfDayMode = valuesDictionary.GetValue<TimeOfDayMode>("TimeOfDayMode", TimeOfDayMode.Changing);
			this.StartingPositionMode = valuesDictionary.GetValue<StartingPositionMode>("StartingPositionMode", StartingPositionMode.Easy);
			this.AreWeatherEffectsEnabled = valuesDictionary.GetValue<bool>("AreWeatherEffectsEnabled", true);
			this.IsAdventureRespawnAllowed = valuesDictionary.GetValue<bool>("IsAdventureRespawnAllowed", true);
			this.AreAdventureSurvivalMechanicsEnabled = valuesDictionary.GetValue<bool>("AreAdventureSurvivalMechanicsEnabled", true);
			this.AreSupernaturalCreaturesEnabled = valuesDictionary.GetValue<bool>("AreSupernaturalCreaturesEnabled", true);
			this.IsFriendlyFireEnabled = valuesDictionary.GetValue<bool>("IsFriendlyFireEnabled", true);
			this.TerrainGenerationMode = valuesDictionary.GetValue<TerrainGenerationMode>("TerrainGenerationMode", TerrainGenerationMode.Continent);
			this.IslandSize = valuesDictionary.GetValue<Vector2>("IslandSize", new Vector2(200f, 200f));
			this.TerrainLevel = valuesDictionary.GetValue<int>("TerrainLevel", 64);
			this.ShoreRoughness = valuesDictionary.GetValue<float>("ShoreRoughness", 0f);
			this.TerrainBlockIndex = valuesDictionary.GetValue<int>("TerrainBlockIndex", 8);
			this.TerrainOceanBlockIndex = valuesDictionary.GetValue<int>("TerrainOceanBlockIndex", 18);
			this.TemperatureOffset = valuesDictionary.GetValue<float>("TemperatureOffset", 0f);
			this.HumidityOffset = valuesDictionary.GetValue<float>("HumidityOffset", 0f);
			this.SeaLevelOffset = valuesDictionary.GetValue<int>("SeaLevelOffset", 0);
			this.BiomeSize = valuesDictionary.GetValue<float>("BiomeSize", 1f);
			this.BlocksTextureName = valuesDictionary.GetValue<string>("BlockTextureName", string.Empty);
			this.Palette = new WorldPalette(valuesDictionary.GetValue<ValuesDictionary>("Palette", new ValuesDictionary()));
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000BF0D8 File Offset: 0x000BD2D8
		public void Save(ValuesDictionary valuesDictionary, bool liveModifiableParametersOnly)
		{
			valuesDictionary.SetValue<string>("WorldName", this.Name);
			valuesDictionary.SetValue<string>("OriginalSerializationVersion", this.OriginalSerializationVersion);
			valuesDictionary.SetValue<GameMode>("GameMode", this.GameMode);
			valuesDictionary.SetValue<EnvironmentBehaviorMode>("EnvironmentBehaviorMode", this.EnvironmentBehaviorMode);
			valuesDictionary.SetValue<TimeOfDayMode>("TimeOfDayMode", this.TimeOfDayMode);
			valuesDictionary.SetValue<bool>("AreWeatherEffectsEnabled", this.AreWeatherEffectsEnabled);
			valuesDictionary.SetValue<bool>("IsAdventureRespawnAllowed", this.IsAdventureRespawnAllowed);
			valuesDictionary.SetValue<bool>("AreAdventureSurvivalMechanicsEnabled", this.AreAdventureSurvivalMechanicsEnabled);
			valuesDictionary.SetValue<bool>("AreSupernaturalCreaturesEnabled", this.AreSupernaturalCreaturesEnabled);
			valuesDictionary.SetValue<bool>("IsFriendlyFireEnabled", this.IsFriendlyFireEnabled);
			if (!liveModifiableParametersOnly)
			{
				valuesDictionary.SetValue<string>("WorldSeedString", this.Seed);
				valuesDictionary.SetValue<TerrainGenerationMode>("TerrainGenerationMode", this.TerrainGenerationMode);
				valuesDictionary.SetValue<Vector2>("IslandSize", this.IslandSize);
				valuesDictionary.SetValue<int>("TerrainLevel", this.TerrainLevel);
				valuesDictionary.SetValue<float>("ShoreRoughness", this.ShoreRoughness);
				valuesDictionary.SetValue<int>("TerrainBlockIndex", this.TerrainBlockIndex);
				valuesDictionary.SetValue<int>("TerrainOceanBlockIndex", this.TerrainOceanBlockIndex);
				valuesDictionary.SetValue<float>("TemperatureOffset", this.TemperatureOffset);
				valuesDictionary.SetValue<float>("HumidityOffset", this.HumidityOffset);
				valuesDictionary.SetValue<int>("SeaLevelOffset", this.SeaLevelOffset);
				valuesDictionary.SetValue<float>("BiomeSize", this.BiomeSize);
				valuesDictionary.SetValue<StartingPositionMode>("StartingPositionMode", this.StartingPositionMode);
			}
			valuesDictionary.SetValue<string>("BlockTextureName", this.BlocksTextureName);
			valuesDictionary.SetValue<ValuesDictionary>("Palette", this.Palette.Save());
		}

		// Token: 0x0400111F RID: 4383
		public string Name = string.Empty;

		// Token: 0x04001120 RID: 4384
		public string OriginalSerializationVersion = string.Empty;

		// Token: 0x04001121 RID: 4385
		public string Seed = string.Empty;

		// Token: 0x04001122 RID: 4386
		public GameMode GameMode = GameMode.Challenging;

		// Token: 0x04001123 RID: 4387
		public EnvironmentBehaviorMode EnvironmentBehaviorMode;

		// Token: 0x04001124 RID: 4388
		public TimeOfDayMode TimeOfDayMode;

		// Token: 0x04001125 RID: 4389
		public StartingPositionMode StartingPositionMode;

		// Token: 0x04001126 RID: 4390
		public bool AreWeatherEffectsEnabled = true;

		// Token: 0x04001127 RID: 4391
		public bool IsAdventureRespawnAllowed = true;

		// Token: 0x04001128 RID: 4392
		public bool AreAdventureSurvivalMechanicsEnabled = true;

		// Token: 0x04001129 RID: 4393
		public bool AreSupernaturalCreaturesEnabled = true;

		// Token: 0x0400112A RID: 4394
		public bool IsFriendlyFireEnabled = true;

		// Token: 0x0400112B RID: 4395
		public TerrainGenerationMode TerrainGenerationMode;

		// Token: 0x0400112C RID: 4396
		public Vector2 IslandSize = new Vector2(400f, 400f);

		// Token: 0x0400112D RID: 4397
		public float BiomeSize = 1f;

		// Token: 0x0400112E RID: 4398
		public int TerrainLevel = 64;

		// Token: 0x0400112F RID: 4399
		public float ShoreRoughness = 0.5f;

		// Token: 0x04001130 RID: 4400
		public int TerrainBlockIndex = 8;

		// Token: 0x04001131 RID: 4401
		public int TerrainOceanBlockIndex = 18;

		// Token: 0x04001132 RID: 4402
		public float TemperatureOffset;

		// Token: 0x04001133 RID: 4403
		public float HumidityOffset;

		// Token: 0x04001134 RID: 4404
		public int SeaLevelOffset;

		// Token: 0x04001135 RID: 4405
		public string BlocksTextureName = string.Empty;

		// Token: 0x04001136 RID: 4406
		public WorldPalette Palette = new WorldPalette();
	}
}
