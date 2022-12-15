using System;
using System.Globalization;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020002CE RID: 718
	public class PlayerData : IDisposable
	{
		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001431 RID: 5169 RVA: 0x0009CEC8 File Offset: 0x0009B0C8
		// (set) Token: 0x06001432 RID: 5170 RVA: 0x0009CED0 File Offset: 0x0009B0D0
		public int PlayerIndex { get; set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001433 RID: 5171 RVA: 0x0009CED9 File Offset: 0x0009B0D9
		// (set) Token: 0x06001434 RID: 5172 RVA: 0x0009CEE1 File Offset: 0x0009B0E1
		public SubsystemGameWidgets SubsystemGameWidgets { get; set; }

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001435 RID: 5173 RVA: 0x0009CEEA File Offset: 0x0009B0EA
		// (set) Token: 0x06001436 RID: 5174 RVA: 0x0009CEF2 File Offset: 0x0009B0F2
		public SubsystemPlayers SubsystemPlayers { get; set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001437 RID: 5175 RVA: 0x0009CEFB File Offset: 0x0009B0FB
		// (set) Token: 0x06001438 RID: 5176 RVA: 0x0009CF03 File Offset: 0x0009B103
		public ComponentPlayer ComponentPlayer { get; set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001439 RID: 5177 RVA: 0x0009CF0C File Offset: 0x0009B10C
		public GameWidget GameWidget
		{
			get
			{
				if (this.m_gameWidget == null)
				{
					foreach (GameWidget gameWidget in this.SubsystemGameWidgets.GameWidgets)
					{
						if (gameWidget.PlayerData == this)
						{
							this.m_gameWidget = gameWidget;
							break;
						}
					}
					if (this.m_gameWidget == null)
					{
						throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 11));
					}
				}
				return this.m_gameWidget;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x0009CF9C File Offset: 0x0009B19C
		// (set) Token: 0x0600143B RID: 5179 RVA: 0x0009CFA4 File Offset: 0x0009B1A4
		public Vector3 SpawnPosition { get; set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600143C RID: 5180 RVA: 0x0009CFAD File Offset: 0x0009B1AD
		// (set) Token: 0x0600143D RID: 5181 RVA: 0x0009CFB5 File Offset: 0x0009B1B5
		public double FirstSpawnTime { get; set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x0009CFBE File Offset: 0x0009B1BE
		// (set) Token: 0x0600143F RID: 5183 RVA: 0x0009CFC6 File Offset: 0x0009B1C6
		public double LastSpawnTime { get; set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06001440 RID: 5184 RVA: 0x0009CFCF File Offset: 0x0009B1CF
		// (set) Token: 0x06001441 RID: 5185 RVA: 0x0009CFD7 File Offset: 0x0009B1D7
		public int SpawnsCount { get; set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x0009CFE0 File Offset: 0x0009B1E0
		// (set) Token: 0x06001443 RID: 5187 RVA: 0x0009CFE8 File Offset: 0x0009B1E8
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				if (value != this.m_name)
				{
					this.m_name = value;
					this.IsDefaultName = false;
				}
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x0009D006 File Offset: 0x0009B206
		// (set) Token: 0x06001445 RID: 5189 RVA: 0x0009D00E File Offset: 0x0009B20E
		public bool IsDefaultName { get; set; }

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x0009D017 File Offset: 0x0009B217
		// (set) Token: 0x06001447 RID: 5191 RVA: 0x0009D020 File Offset: 0x0009B220
		public PlayerClass PlayerClass
		{
			get
			{
				return this.m_playerClass;
			}
			set
			{
				if (this.SubsystemPlayers.PlayersData.Contains(this))
				{
					throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 1));
				}
				this.m_playerClass = value;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x0009D05B File Offset: 0x0009B25B
		// (set) Token: 0x06001449 RID: 5193 RVA: 0x0009D063 File Offset: 0x0009B263
		public float Level { get; set; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x0009D06C File Offset: 0x0009B26C
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x0009D074 File Offset: 0x0009B274
		public string CharacterSkinName { get; set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x0009D07D File Offset: 0x0009B27D
		// (set) Token: 0x0600144D RID: 5197 RVA: 0x0009D085 File Offset: 0x0009B285
		public WidgetInputDevice InputDevice { get; set; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x0009D08E File Offset: 0x0009B28E
		public bool IsReadyForPlaying
		{
			get
			{
				return this.m_stateMachine.CurrentState == "Playing" || this.m_stateMachine.CurrentState == "PlayerDead";
			}
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0009D0C0 File Offset: 0x0009B2C0
		public PlayerData(Project project)
		{
			this.m_project = project;
			this.SubsystemPlayers = project.FindSubsystem<SubsystemPlayers>(true);
			this.SubsystemGameWidgets = project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = project.FindSubsystem<SubsystemSky>(true);
			this.m_playerClass = PlayerClass.Male;
			this.Level = 1f;
			this.FirstSpawnTime = -1.0;
			this.LastSpawnTime = -1.0;
			this.RandomizeCharacterSkin();
			this.ResetName();
			this.InputDevice = WidgetInputDevice.None;
			this.m_stateMachine.AddState("FirstUpdate", null, delegate
			{
				if (this.ComponentPlayer != null)
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 4), this.Name, MathUtils.Floor(this.Level)), null, 0f, true);
					this.m_stateMachine.TransitionTo("WaitForTerrain");
					return;
				}
				this.m_stateMachine.TransitionTo("PrepareSpawn");
			}, null);
			this.m_stateMachine.AddState("PrepareSpawn", delegate
			{
				if (this.SpawnPosition == Vector3.Zero)
				{
					if (this.SubsystemPlayers.GlobalSpawnPosition == Vector3.Zero)
					{
						PlayerData playerData = this.SubsystemPlayers.PlayersData.FirstOrDefault((PlayerData pd) => pd.SpawnPosition != Vector3.Zero);
						if (playerData != null)
						{
							if (playerData.ComponentPlayer != null)
							{
								this.SpawnPosition = playerData.ComponentPlayer.ComponentBody.Position;
								this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
							}
							else
							{
								this.SpawnPosition = playerData.SpawnPosition;
								this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
							}
						}
						else
						{
							this.SpawnPosition = this.m_subsystemTerrain.TerrainContentsGenerator.FindCoarseSpawnPosition();
							this.m_spawnMode = PlayerData.SpawnMode.InitialIntro;
						}
						this.SubsystemPlayers.GlobalSpawnPosition = this.SpawnPosition;
					}
					else
					{
						this.SpawnPosition = this.SubsystemPlayers.GlobalSpawnPosition;
						this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
					}
				}
				else
				{
					this.m_spawnMode = PlayerData.SpawnMode.Respawn;
				}
				if (this.m_spawnMode == PlayerData.SpawnMode.Respawn)
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 2), this.Name, MathUtils.Floor(this.Level)), LanguageControl.Get(PlayerData.fName, 3), 0f, true);
				}
				else
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 4), this.Name, MathUtils.Floor(this.Level)), null, 0f, true);
				}
				this.m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(this.PlayerIndex, this.SpawnPosition.XZ, 0f, 64f);
				this.m_terrainWaitStartTime = Time.FrameStartTime;
			}, delegate
			{
				if (Time.PeriodicEvent(0.1, 0.0))
				{
					float updateProgress = this.m_subsystemTerrain.TerrainUpdater.GetUpdateProgress(this.PlayerIndex, 0f, 64f);
					this.UpdateSpawnDialog(null, null, 0.5f * updateProgress, false);
					if (updateProgress >= 1f || Time.FrameStartTime - this.m_terrainWaitStartTime >= 15.0)
					{
						switch (this.m_spawnMode)
						{
						case PlayerData.SpawnMode.InitialIntro:
							this.SpawnPosition = this.FindIntroSpawnPosition(this.SpawnPosition.XZ);
							break;
						case PlayerData.SpawnMode.InitialNoIntro:
							this.SpawnPosition = this.FindNoIntroSpawnPosition(this.SpawnPosition, false);
							break;
						case PlayerData.SpawnMode.Respawn:
							this.SpawnPosition = this.FindNoIntroSpawnPosition(this.SpawnPosition, true);
							break;
						default:
							throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 5));
						}
						this.m_stateMachine.TransitionTo("WaitForTerrain");
					}
				}
			}, null);
			this.m_stateMachine.AddState("WaitForTerrain", delegate
			{
				this.m_terrainWaitStartTime = Time.FrameStartTime;
				Vector2 center = (this.ComponentPlayer != null) ? this.ComponentPlayer.ComponentBody.Position.XZ : this.SpawnPosition.XZ;
				this.m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(this.PlayerIndex, center, MathUtils.Min(this.m_subsystemSky.VisibilityRange, 64f), 0f);
			}, delegate
			{
				if (Time.PeriodicEvent(0.1, 0.0))
				{
					float updateProgress = this.m_subsystemTerrain.TerrainUpdater.GetUpdateProgress(this.PlayerIndex, MathUtils.Min(this.m_subsystemSky.VisibilityRange, 64f), 0f);
					this.UpdateSpawnDialog(null, null, 0.5f + 0.5f * updateProgress, false);
					if ((updateProgress >= 1f && Time.FrameStartTime - this.m_terrainWaitStartTime > 2.0) || Time.FrameStartTime - this.m_terrainWaitStartTime >= 15.0)
					{
						if (this.ComponentPlayer == null)
						{
							this.SpawnPlayer(this.SpawnPosition, this.m_spawnMode);
						}
						this.m_stateMachine.TransitionTo("Playing");
					}
				}
			}, null);
			this.m_stateMachine.AddState("Playing", delegate
			{
				this.HideSpawnDialog();
			}, delegate
			{
				if (this.ComponentPlayer == null)
				{
					this.m_stateMachine.TransitionTo("PrepareSpawn");
					return;
				}
				if (this.m_playerDeathTime != null)
				{
					this.m_stateMachine.TransitionTo("PlayerDead");
					return;
				}
				if (this.ComponentPlayer.ComponentHealth.Health <= 0f)
				{
					this.m_playerDeathTime = new double?(Time.RealTime);
				}
			}, null);
			this.m_stateMachine.AddState("PlayerDead", delegate
			{
				this.GameWidget.ActiveCamera = this.GameWidget.FindCamera<DeathCamera>(true);
				if (this.ComponentPlayer != null)
				{
					string text = this.ComponentPlayer.ComponentHealth.CauseOfDeath;
					if (string.IsNullOrEmpty(text))
					{
						text = LanguageControl.Get(PlayerData.fName, 12);
					}
					string arg = string.Format(LanguageControl.Get(PlayerData.fName, 13), text);
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					{
						this.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 7), arg, LanguageControl.Get("GameMode", this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())), 30f, 1.5f);
					}
					else if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !this.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					{
						this.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 8), arg), 30f, 1.5f);
					}
					else
					{
						this.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 9), arg), 30f, 1.5f);
					}
				}
				this.Level = MathUtils.Max(MathUtils.Floor(this.Level / 2f), 1f);
			}, delegate
			{
				if (this.ComponentPlayer == null)
				{
					this.m_stateMachine.TransitionTo("PrepareSpawn");
					return;
				}
				if (Time.RealTime - this.m_playerDeathTime.Value > 1.5 && !DialogsManager.HasDialogs(this.ComponentPlayer.GuiWidget) && this.ComponentPlayer.GameWidget.Input.Any)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					{
						DialogsManager.ShowDialog(this.ComponentPlayer.GuiWidget, new GameMenuDialog(this.ComponentPlayer));
						return;
					}
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !this.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					{
						ScreensManager.SwitchScreen("GameLoading", new object[]
						{
							GameManager.WorldInfo,
							"AdventureRestart"
						});
						return;
					}
					this.m_project.RemoveEntity(this.ComponentPlayer.Entity, true);
				}
			}, null);
			this.m_stateMachine.TransitionTo("FirstUpdate");
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0009D23B File Offset: 0x0009B43B
		public void Dispose()
		{
			this.HideSpawnDialog();
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0009D244 File Offset: 0x0009B444
		public void RandomizeCharacterSkin()
		{
			Game.Random random = new Game.Random();
			CharacterSkinsManager.UpdateCharacterSkinsList();
			string[] array = CharacterSkinsManager.CharacterSkinsNames.Where(delegate(string n)
			{
				if (CharacterSkinsManager.IsBuiltIn(n))
				{
					PlayerClass? playerClass = CharacterSkinsManager.GetPlayerClass(n);
					PlayerClass playerClass2 = this.m_playerClass;
					return playerClass.GetValueOrDefault() == playerClass2 & playerClass != null;
				}
				return false;
			}).ToArray<string>();
			string[] second = (from pd in this.SubsystemPlayers.PlayersData
			select pd.CharacterSkinName).ToArray<string>();
			string[] array2 = array.Except(second).ToArray<string>();
			if (array2.Length != 0)
			{
				this.CharacterSkinName = array2[random.Int(0, array2.Length - 1)];
				return;
			}
			this.CharacterSkinName = array[random.Int(0, array.Length - 1)];
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0009D2F1 File Offset: 0x0009B4F1
		public void ResetName()
		{
			this.m_name = CharacterSkinsManager.GetDisplayName(this.CharacterSkinName);
			this.IsDefaultName = true;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0009D30B File Offset: 0x0009B50B
		public static bool VerifyName(string name)
		{
			return name.Length >= 2;
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0009D319 File Offset: 0x0009B519
		public void Update()
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0009D328 File Offset: 0x0009B528
		public void Load(ValuesDictionary valuesDictionary)
		{
			this.SpawnPosition = valuesDictionary.GetValue<Vector3>("SpawnPosition", Vector3.Zero);
			this.FirstSpawnTime = valuesDictionary.GetValue<double>("FirstSpawnTime", 0.0);
			this.LastSpawnTime = valuesDictionary.GetValue<double>("LastSpawnTime", 0.0);
			this.SpawnsCount = valuesDictionary.GetValue<int>("SpawnsCount", 0);
			this.Name = valuesDictionary.GetValue<string>("Name", "Walter");
			this.PlayerClass = valuesDictionary.GetValue<PlayerClass>("PlayerClass", PlayerClass.Male);
			this.Level = valuesDictionary.GetValue<float>("Level", 1f);
			this.CharacterSkinName = valuesDictionary.GetValue<string>("CharacterSkinName", CharacterSkinsManager.CharacterSkinsNames[0]);
			this.InputDevice = valuesDictionary.GetValue<WidgetInputDevice>("InputDevice", this.InputDevice);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0009D408 File Offset: 0x0009B608
		public void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<Vector3>("SpawnPosition", this.SpawnPosition);
			valuesDictionary.SetValue<double>("FirstSpawnTime", this.FirstSpawnTime);
			valuesDictionary.SetValue<double>("LastSpawnTime", this.LastSpawnTime);
			valuesDictionary.SetValue<int>("SpawnsCount", this.SpawnsCount);
			valuesDictionary.SetValue<string>("Name", this.Name);
			valuesDictionary.SetValue<PlayerClass>("PlayerClass", this.PlayerClass);
			valuesDictionary.SetValue<float>("Level", this.Level);
			valuesDictionary.SetValue<string>("CharacterSkinName", this.CharacterSkinName);
			valuesDictionary.SetValue<WidgetInputDevice>("InputDevice", this.InputDevice);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0009D4B0 File Offset: 0x0009B6B0
		public void OnEntityAdded(Entity entity)
		{
			ComponentPlayer componentPlayer = entity.FindComponent<ComponentPlayer>();
			if (componentPlayer != null && componentPlayer.PlayerData == this)
			{
				if (this.ComponentPlayer != null)
				{
					throw new InvalidOperationException(string.Format(LanguageControl.Get(PlayerData.fName, 10), this.PlayerIndex));
				}
				this.ComponentPlayer = componentPlayer;
				this.GameWidget.ActiveCamera = this.GameWidget.FindCamera<FppCamera>(true);
				this.GameWidget.Target = componentPlayer;
				if (this.FirstSpawnTime < 0.0)
				{
					this.FirstSpawnTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
				}
			}
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0009D546 File Offset: 0x0009B746
		public void OnEntityRemoved(Entity entity)
		{
			if (this.ComponentPlayer != null && entity == this.ComponentPlayer.Entity)
			{
				this.ComponentPlayer = null;
				this.m_playerDeathTime = null;
			}
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0009D574 File Offset: 0x0009B774
		public Vector3 FindIntroSpawnPosition(Vector2 desiredSpawnPosition)
		{
			Vector2 zero = Vector2.Zero;
			float num = float.MinValue;
			for (int i = -30; i <= 30; i += 2)
			{
				for (int j = -30; j <= 30; j += 2)
				{
					int num2 = Terrain.ToCell(desiredSpawnPosition.X) + i;
					int num3 = Terrain.ToCell(desiredSpawnPosition.Y) + j;
					float num4 = this.ScoreIntroSpawnPosition(desiredSpawnPosition, num2, num3);
					if (num4 > num)
					{
						num = num4;
						zero = new Vector2((float)num2, (float)num3);
					}
				}
			}
			float num5 = (float)(this.m_subsystemTerrain.Terrain.CalculateTopmostCellHeight(Terrain.ToCell(zero.X), Terrain.ToCell(zero.Y)) + 1);
			return new Vector3(zero.X + 0.5f, num5 + 0.01f, zero.Y + 0.5f);
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x0009D63C File Offset: 0x0009B83C
		public Vector3 FindNoIntroSpawnPosition(Vector3 desiredSpawnPosition, bool respawn)
		{
			Vector3 zero = Vector3.Zero;
			float num = float.MinValue;
			for (int i = -8; i <= 8; i++)
			{
				for (int j = -8; j <= 8; j++)
				{
					for (int k = -8; k <= 8; k++)
					{
						int num2 = Terrain.ToCell(desiredSpawnPosition.X) + i;
						int num3 = Terrain.ToCell(desiredSpawnPosition.Y) + j;
						int num4 = Terrain.ToCell(desiredSpawnPosition.Z) + k;
						float num5 = this.ScoreNoIntroSpawnPosition(desiredSpawnPosition, num2, num3, num4);
						if (num5 > num)
						{
							num = num5;
							zero = new Vector3((float)num2, (float)num3, (float)num4);
						}
					}
				}
			}
			return new Vector3(zero.X + 0.5f, zero.Y + 0.01f, zero.Z + 0.5f);
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0009D700 File Offset: 0x0009B900
		public float ScoreIntroSpawnPosition(Vector2 desiredSpawnPosition, int x, int z)
		{
			float num = -0.01f * Vector2.Distance(new Vector2((float)x, (float)z), desiredSpawnPosition);
			int num2 = this.m_subsystemTerrain.Terrain.CalculateTopmostCellHeight(x, z);
			if (num2 < 64 || num2 > 85)
			{
				num -= 5f;
			}
			if (this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) < 8)
			{
				num -= 5f;
			}
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(x, num2, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				num -= 5f;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					if (this.m_subsystemTerrain.Terrain.GetCellContents(i, num2 + 2, j) != 0)
					{
						num -= 1f;
					}
				}
			}
			Vector2 vector = ComponentIntro.FindOceanDirection(this.m_subsystemTerrain.TerrainContentsGenerator, new Vector2((float)x, (float)z));
			Vector3 vector2 = new Vector3((float)x, (float)num2 + 1.5f, (float)z);
			for (int k = -1; k <= 1; k++)
			{
				Vector3 end = vector2 + new Vector3(30f * vector.X, 5f * (float)k, 30f * vector.Y);
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector2, end, false, true, (int value, float distance) => Terrain.ExtractContents(value) != 0);
				if (terrainRaycastResult != null)
				{
					CellFace cellFace = terrainRaycastResult.Value.CellFace;
					int cellContents2 = this.m_subsystemTerrain.Terrain.GetCellContents(cellFace.X, cellFace.Y, cellFace.Z);
					if (cellContents2 != 18 && cellContents2 != 0)
					{
						num -= 2f;
					}
				}
			}
			return num;
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0009D8D0 File Offset: 0x0009BAD0
		public float ScoreNoIntroSpawnPosition(Vector3 desiredSpawnPosition, int x, int y, int z)
		{
			float num = -0.01f * Vector3.Distance(new Vector3((float)x, (float)y, (float)z), desiredSpawnPosition);
			if (y < 1 || y >= 255)
			{
				num -= 100f;
			}
			Block block = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(x, y - 1, z)];
			Block block2 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(x, y, z)];
			Block block3 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(x, y + 1, z)];
			if (block.IsTransparent)
			{
				num -= 10f;
			}
			if (!block.IsCollidable)
			{
				num -= 10f;
			}
			if (block2.IsCollidable)
			{
				num -= 10f;
			}
			if (block3.IsCollidable)
			{
				num -= 10f;
			}
			foreach (PlayerData playerData in this.SubsystemPlayers.PlayersData)
			{
				if (playerData != this && Vector3.DistanceSquared(playerData.SpawnPosition, new Vector3((float)x, (float)y, (float)z)) < (float)MathUtils.Sqr(2))
				{
					num -= 1f;
				}
			}
			return num;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0009DA18 File Offset: 0x0009BC18
		public bool CheckIsPointInWater(Point3 p)
		{
			bool result = true;
			for (int i = p.X - 1; i < p.X + 1; i++)
			{
				for (int j = p.Z - 1; j < p.Z + 1; j++)
				{
					for (int k = p.Y; k > 0; k--)
					{
						int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(p.X, k, p.Z);
						Block block = BlocksManager.Blocks[cellContents];
						if (block.IsCollidable)
						{
							return false;
						}
						if (block is WaterBlock)
						{
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0009DAAC File Offset: 0x0009BCAC
		public void SpawnPlayer(Vector3 position, PlayerData.SpawnMode spawnMode)
		{
			ComponentMount componentMount = null;
			if (spawnMode != PlayerData.SpawnMode.Respawn && this.CheckIsPointInWater(Terrain.ToCell(position)))
			{
				Entity entity = DatabaseManager.CreateEntity(this.m_project, "Boat", true);
				entity.FindComponent<ComponentBody>(true).Position = position;
				entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.DegToRad(45f));
				componentMount = entity.FindComponent<ComponentMount>(true);
				this.m_project.AddEntity(entity);
				position.Y += entity.FindComponent<ComponentBody>(true).BoxSize.Y;
			}
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			if (spawnMode != PlayerData.SpawnMode.Respawn)
			{
				if (this.PlayerClass == PlayerClass.Female)
				{
					if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("2"))
					{
						text = "";
						text2 = PlayerData.MakeClothingValue(37, 2);
						text3 = PlayerData.MakeClothingValue(16, 14);
						text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(27, 0);
					}
					else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("3"))
					{
						text = PlayerData.MakeClothingValue(31, 0);
						text2 = PlayerData.MakeClothingValue(13, 7) + ";" + PlayerData.MakeClothingValue(5, 0);
						text3 = PlayerData.MakeClothingValue(17, 15);
						text4 = PlayerData.MakeClothingValue(29, 0);
					}
					else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("4"))
					{
						text = PlayerData.MakeClothingValue(30, 7);
						text2 = PlayerData.MakeClothingValue(14, 6);
						text3 = PlayerData.MakeClothingValue(25, 7);
						text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
					}
					else
					{
						text = PlayerData.MakeClothingValue(30, 12);
						text2 = PlayerData.MakeClothingValue(37, 3) + ";" + PlayerData.MakeClothingValue(1, 3);
						text3 = PlayerData.MakeClothingValue(0, 12);
						text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
					}
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("2"))
				{
					text = "";
					text2 = PlayerData.MakeClothingValue(13, 0) + ";" + PlayerData.MakeClothingValue(5, 0);
					text3 = PlayerData.MakeClothingValue(25, 8);
					text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("3"))
				{
					text = PlayerData.MakeClothingValue(32, 0);
					text2 = PlayerData.MakeClothingValue(37, 5);
					text3 = PlayerData.MakeClothingValue(0, 15);
					text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("4"))
				{
					text = PlayerData.MakeClothingValue(31, 0);
					text2 = PlayerData.MakeClothingValue(15, 14);
					text3 = PlayerData.MakeClothingValue(0, 0);
					text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
				}
				else
				{
					text = PlayerData.MakeClothingValue(32, 0);
					text2 = PlayerData.MakeClothingValue(37, 0) + ";" + PlayerData.MakeClothingValue(1, 9);
					text3 = PlayerData.MakeClothingValue(0, 12);
					text4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
				}
			}
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			string text5 = "Player";
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary2.SetValue<int>("PlayerIndex", this.PlayerIndex);
			valuesDictionary.SetValue<ValuesDictionary>(text5, valuesDictionary2);
			string text6 = "Intro";
			ValuesDictionary valuesDictionary3 = new ValuesDictionary();
			valuesDictionary3.SetValue<bool>("PlayIntro", spawnMode == PlayerData.SpawnMode.InitialIntro);
			valuesDictionary.SetValue<ValuesDictionary>(text6, valuesDictionary3);
			string text7 = "Clothing";
			ValuesDictionary valuesDictionary4 = new ValuesDictionary();
			string text8 = "Clothes";
			ValuesDictionary valuesDictionary5 = new ValuesDictionary();
			valuesDictionary5.SetValue<string>("Feet", text4);
			valuesDictionary5.SetValue<string>("Legs", text3);
			valuesDictionary5.SetValue<string>("Torso", text2);
			valuesDictionary5.SetValue<string>("Head", text);
			valuesDictionary4.SetValue<ValuesDictionary>(text8, valuesDictionary5);
			valuesDictionary.SetValue<ValuesDictionary>(text7, valuesDictionary4);
			ValuesDictionary overrides = valuesDictionary;
			Vector2 v = ComponentIntro.FindOceanDirection(this.m_subsystemTerrain.TerrainContentsGenerator, position.XZ);
			string entityTemplateName = (this.PlayerClass == PlayerClass.Male) ? "MalePlayer" : "FemalePlayer";
			Entity entity2 = DatabaseManager.CreateEntity(this.m_project, entityTemplateName, overrides, true);
			entity2.FindComponent<ComponentBody>(true).Position = position;
			entity2.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Vector2.Angle(v, -Vector2.UnitY));
			this.m_project.AddEntity(entity2);
			if (componentMount != null)
			{
				entity2.FindComponent<ComponentRider>(true).StartMounting(componentMount);
			}
			this.LastSpawnTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			int spawnsCount = this.SpawnsCount + 1;
			this.SpawnsCount = spawnsCount;
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0009DF80 File Offset: 0x0009C180
		public string GetEntityTemplateName()
		{
			if (this.PlayerClass != PlayerClass.Male)
			{
				return "FemalePlayer";
			}
			return "MalePlayer";
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0009DF98 File Offset: 0x0009C198
		public void UpdateSpawnDialog(string largeMessage, string smallMessage, float progress, bool resetProgress)
		{
			if (resetProgress)
			{
				this.m_progress = 0f;
			}
			this.m_progress = MathUtils.Max(progress, this.m_progress);
			if (this.m_spawnDialog == null)
			{
				this.m_spawnDialog = new SpawnDialog();
				DialogsManager.ShowDialog(this.GameWidget.GuiWidget, this.m_spawnDialog);
			}
			if (largeMessage != null)
			{
				this.m_spawnDialog.LargeMessage = largeMessage;
			}
			if (smallMessage != null)
			{
				this.m_spawnDialog.SmallMessage = smallMessage;
			}
			this.m_spawnDialog.Progress = this.m_progress;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0009E01E File Offset: 0x0009C21E
		public void HideSpawnDialog()
		{
			if (this.m_spawnDialog != null)
			{
				DialogsManager.HideDialog(this.m_spawnDialog);
				this.m_spawnDialog = null;
			}
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x0009E03C File Offset: 0x0009C23C
		public static string MakeClothingValue(int index, int color)
		{
			return Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, color), index)).ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x04000E13 RID: 3603
		public static string fName = "PlayerData";

		// Token: 0x04000E14 RID: 3604
		public Project m_project;

		// Token: 0x04000E15 RID: 3605
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000E16 RID: 3606
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000E17 RID: 3607
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000E18 RID: 3608
		public GameWidget m_gameWidget;

		// Token: 0x04000E19 RID: 3609
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000E1A RID: 3610
		public PlayerClass m_playerClass;

		// Token: 0x04000E1B RID: 3611
		public string m_name;

		// Token: 0x04000E1C RID: 3612
		public PlayerData.SpawnMode m_spawnMode;

		// Token: 0x04000E1D RID: 3613
		public double? m_playerDeathTime;

		// Token: 0x04000E1E RID: 3614
		public double m_terrainWaitStartTime;

		// Token: 0x04000E1F RID: 3615
		public SpawnDialog m_spawnDialog;

		// Token: 0x04000E20 RID: 3616
		public float m_progress;

		// Token: 0x020004C8 RID: 1224
		public enum SpawnMode
		{
			// Token: 0x0400179A RID: 6042
			InitialIntro,
			// Token: 0x0400179B RID: 6043
			InitialNoIntro,
			// Token: 0x0400179C RID: 6044
			Respawn
		}
	}
}
