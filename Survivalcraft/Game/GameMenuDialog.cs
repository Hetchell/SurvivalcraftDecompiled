using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000285 RID: 645
	public class GameMenuDialog : Dialog
	{
		// Token: 0x06001318 RID: 4888 RVA: 0x00094BCC File Offset: 0x00092DCC
		public GameMenuDialog(ComponentPlayer componentPlayer)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/GameMenuDialog");
			base.LoadContents(this, node);
			this.m_statsPanel = this.Children.Find<StackPanelWidget>("StatsPanel", true);
			this.m_componentPlayer = componentPlayer;
			this.m_adventureRestartExists = WorldsManager.SnapshotExists(GameManager.WorldInfo.DirectoryName, "AdventureRestart");
			if (!GameMenuDialog.m_increaseDetailDialogShown && PerformanceManager.LongTermAverageFrameTime != null && PerformanceManager.LongTermAverageFrameTime.Value * 1000f < 25f && (SettingsManager.VisibilityRange <= 64 || SettingsManager.ResolutionMode == ResolutionMode.Low))
			{
				GameMenuDialog.m_increaseDetailDialogShown = true;
				DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get(GameMenuDialog.fName, 1), LanguageControl.Get(GameMenuDialog.fName, 2), LanguageControl.Get("Usual", "ok"), null, null));
				AnalyticsManager.LogEvent("[GameMenuScreen] IncreaseDetailDialog Shown", Array.Empty<AnalyticsParameter>());
			}
			if (!GameMenuDialog.m_decreaseDetailDialogShown && PerformanceManager.LongTermAverageFrameTime != null && PerformanceManager.LongTermAverageFrameTime.Value * 1000f > 50f && (SettingsManager.VisibilityRange >= 64 || SettingsManager.ResolutionMode == ResolutionMode.High))
			{
				GameMenuDialog.m_decreaseDetailDialogShown = true;
				DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get(GameMenuDialog.fName, 3), LanguageControl.Get(GameMenuDialog.fName, 4), LanguageControl.Get("Usual", "ok"), null, null));
				AnalyticsManager.LogEvent("[GameMenuScreen] DecreaseDetailDialog Shown", Array.Empty<AnalyticsParameter>());
			}
			this.m_statsPanel.Children.Clear();
			Project project = componentPlayer.Project;
			PlayerData playerData = componentPlayer.PlayerData;
			PlayerStats playerStats = componentPlayer.PlayerStats;
			SubsystemGameInfo subsystemGameInfo = project.FindSubsystem<SubsystemGameInfo>(true);
			SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior = project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			BitmapFont font2 = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			Color white = Color.White;
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Center
			};
			this.m_statsPanel.Children.Add(stackPanelWidget);
			stackPanelWidget.Children.Add(new LabelWidget
			{
				Text = LanguageControl.Get(GameMenuDialog.fName, 5),
				Font = font,
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = white
			});
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 6), LanguageControl.Get("GameMode", subsystemGameInfo.WorldSettings.GameMode.ToString()) + ", " + LanguageControl.Get("EnvironmentBehaviorMode", subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode.ToString()), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 7), StringsManager.GetString("TerrainGenerationMode." + subsystemGameInfo.WorldSettings.TerrainGenerationMode.ToString() + ".Name"), "");
			string seed = subsystemGameInfo.WorldSettings.Seed;
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 8), (!string.IsNullOrEmpty(seed)) ? seed : LanguageControl.Get(GameMenuDialog.fName, 9), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 10), WorldOptionsScreen.FormatOffset((float)subsystemGameInfo.WorldSettings.SeaLevelOffset), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 11), WorldOptionsScreen.FormatOffset(subsystemGameInfo.WorldSettings.TemperatureOffset), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 12), WorldOptionsScreen.FormatOffset(subsystemGameInfo.WorldSettings.HumidityOffset), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 13), subsystemGameInfo.WorldSettings.BiomeSize.ToString() + "x", "");
			int num = 0;
			for (int i = 0; i < 65535; i++)
			{
				if (subsystemFurnitureBlockBehavior.GetDesign(i) != null)
				{
					num++;
				}
			}
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 14), string.Format("{0}/{1}", num, 65535), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 15), string.IsNullOrEmpty(subsystemGameInfo.WorldSettings.OriginalSerializationVersion) ? LanguageControl.Get(GameMenuDialog.fName, 16) : subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "");
			stackPanelWidget.Children.Add(new LabelWidget
			{
				Text = LanguageControl.Get(GameMenuDialog.fName, 17),
				Font = font,
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = white
			});
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 18), playerData.Name, "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 19), playerData.PlayerClass.ToString(), "");
			string value = (playerData.FirstSpawnTime >= 0.0) ? (((subsystemGameInfo.TotalElapsedGameTime - playerData.FirstSpawnTime) / 1200.0).ToString("N1") + LanguageControl.Get(GameMenuDialog.fName, 20)) : LanguageControl.Get(GameMenuDialog.fName, 21);
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 22), value, "");
			string value2 = (playerData.LastSpawnTime >= 0.0) ? (((subsystemGameInfo.TotalElapsedGameTime - playerData.LastSpawnTime) / 1200.0).ToString("N1") + LanguageControl.Get(GameMenuDialog.fName, 23)) : LanguageControl.Get(GameMenuDialog.fName, 24);
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 25), value2, "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 26), MathUtils.Max(playerData.SpawnsCount - 1, 0).ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 27), "");
			this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 28), string.Format(LanguageControl.Get(GameMenuDialog.fName, 29), ((int)MathUtils.Floor(playerStats.HighestLevel)).ToString("N0")), "");
			if (componentPlayer != null)
			{
				Vector3 position = componentPlayer.ComponentBody.Position;
				if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
				{
					this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 30), string.Format(LanguageControl.Get(GameMenuDialog.fName, 31), string.Format("{0:0}", position.X), string.Format("{0:0}", position.Z), string.Format("{0:0}", position.Y)), "");
				}
				else
				{
					this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 30), string.Format(LanguageControl.Get(GameMenuDialog.fName, 32), LanguageControl.Get("GameMode", subsystemGameInfo.WorldSettings.GameMode.ToString())), "");
				}
			}
			if (string.CompareOrdinal(subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "1.29") > 0)
			{
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 33),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 34), playerStats.PlayerKills.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 35), playerStats.LandCreatureKills.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 36), playerStats.WaterCreatureKills.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 37), playerStats.AirCreatureKills.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 38), playerStats.MeleeAttacks.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 39), playerStats.MeleeHits.ToString("N0"), string.Format("({0:0}%)", (playerStats.MeleeHits == 0L) ? 0.0 : ((double)playerStats.MeleeHits / (double)playerStats.MeleeAttacks * 100.0)));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 40), playerStats.RangedAttacks.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 41), playerStats.RangedHits.ToString("N0"), string.Format("({0:0}%)", (playerStats.RangedHits == 0L) ? 0.0 : ((double)playerStats.RangedHits / (double)playerStats.RangedAttacks * 100.0)));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 42), playerStats.HitsReceived.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 43),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 44), playerStats.BlocksDug.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 45), playerStats.BlocksPlaced.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 46), playerStats.BlocksInteracted.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 47), playerStats.ItemsCrafted.ToString("N0"), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 48), playerStats.FurnitureItemsMade.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 49),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 50), GameMenuDialog.FormatDistance(playerStats.DistanceTravelled), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 51), GameMenuDialog.FormatDistance(playerStats.DistanceWalked), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceWalked / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 52), GameMenuDialog.FormatDistance(playerStats.DistanceFallen), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceFallen / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 53), GameMenuDialog.FormatDistance(playerStats.DistanceClimbed), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceClimbed / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 54), GameMenuDialog.FormatDistance(playerStats.DistanceFlown), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceFlown / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 55), GameMenuDialog.FormatDistance(playerStats.DistanceSwam), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceSwam / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 56), GameMenuDialog.FormatDistance(playerStats.DistanceRidden), string.Format("({0:0.0}%)", (playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceRidden / playerStats.DistanceTravelled * 100.0) : 0.0));
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 57), GameMenuDialog.FormatDistance(playerStats.LowestAltitude), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 58), GameMenuDialog.FormatDistance(playerStats.HighestAltitude), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 59), playerStats.DeepestDive.ToString("N1") + "m", "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 60), playerStats.Jumps.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 61),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 62), (playerStats.TotalHealthLost * 100.0).ToString("N0") + "%", "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 63), playerStats.FoodItemsEaten.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 64), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 65), playerStats.TimesWentToSleep.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 66), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 67), (playerStats.TimeSlept / 1200.0).ToString("N1") + LanguageControl.Get(GameMenuDialog.fName, 68), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 69), playerStats.TimesWasSick.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 66), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 70), playerStats.TimesPuked.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 66), "");
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 71), playerStats.TimesHadFlu.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 66), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 72),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 73), playerStats.StruckByLightning.ToString("N0") + LanguageControl.Get(GameMenuDialog.fName, 66), "");
				GameMode easiestModeUsed = playerStats.EasiestModeUsed;
				this.AddStat(stackPanelWidget, LanguageControl.Get(GameMenuDialog.fName, 74), LanguageControl.Get("GameMode", easiestModeUsed.ToString()), "");
				if (playerStats.DeathRecords.Count <= 0)
				{
					return;
				}
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = LanguageControl.Get(GameMenuDialog.fName, 75),
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				using (ReadOnlyList<PlayerStats.DeathRecord>.Enumerator enumerator = playerStats.DeathRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerStats.DeathRecord deathRecord = enumerator.Current;
						float num2 = (float)MathUtils.Remainder(deathRecord.Day, 1.0);
						string arg = (num2 >= 0.2f && num2 < 0.8f) ? ((num2 < 0.7f) ? ((num2 < 0.5f) ? LanguageControl.Get(GameMenuDialog.fName, 76) : LanguageControl.Get(GameMenuDialog.fName, 77)) : LanguageControl.Get(GameMenuDialog.fName, 78)) : LanguageControl.Get(GameMenuDialog.fName, 79);
						this.AddStat(stackPanelWidget, string.Format(LanguageControl.Get(GameMenuDialog.fName, 80), MathUtils.Floor(deathRecord.Day) + 1.0, arg), "", deathRecord.Cause);
					}
					return;
				}
			}
			stackPanelWidget.Children.Add(new LabelWidget
			{
				Text = LanguageControl.Get(GameMenuDialog.fName, 81),
				WordWrap = true,
				Font = font2,
				HorizontalAlignment = WidgetAlignment.Center,
				TextAnchor = TextAnchor.HorizontalCenter,
				Margin = new Vector2(20f, 10f),
				Color = white
			});
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x00095E68 File Offset: 0x00094068
		public override void Update()
		{
			if (this.Children.Find<ButtonWidget>("More", true).IsClicked)
			{
				List<Tuple<string, Action>> list = new List<Tuple<string, Action>>();
				if (this.m_adventureRestartExists && GameManager.WorldInfo.WorldSettings.GameMode == GameMode.Adventure)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 82), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get(GameMenuDialog.fName, 83), LanguageControl.Get(GameMenuDialog.fName, 84), LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton result)
						{
							if (result == MessageDialogButton.Button1)
							{
								ScreensManager.SwitchScreen("GameLoading", new object[]
								{
									GameManager.WorldInfo,
									"AdventureRestart"
								});
							}
						}));
					}));
				}
				if (this.GetRateableItems().FirstOrDefault<ActiveExternalContentInfo>() != null && UserManager.ActiveUser != null)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 85), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new ListSelectionDialog(LanguageControl.Get(GameMenuDialog.fName, 86), this.GetRateableItems(), 60f, (object o) => ((ActiveExternalContentInfo)o).DisplayName, delegate(object o)
						{
							ActiveExternalContentInfo activeExternalContentInfo = (ActiveExternalContentInfo)o;
							DialogsManager.ShowDialog(base.ParentWidget, new RateCommunityContentDialog(activeExternalContentInfo.Address, activeExternalContentInfo.DisplayName, UserManager.ActiveUser.UniqueId));
						}));
					}));
				}
				list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 87), delegate()
				{
					ScreensManager.SwitchScreen("Players", new object[]
					{
						this.m_componentPlayer.Project.FindSubsystem<SubsystemPlayers>(true)
					});
				}));
				list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 88), delegate()
				{
					ScreensManager.SwitchScreen("Settings", Array.Empty<object>());
				}));
				list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 89), delegate()
				{
					ScreensManager.SwitchScreen("Help", Array.Empty<object>());
				}));
				if ((base.Input.Devices & (WidgetInputDevice.Keyboard | WidgetInputDevice.Mouse)) != WidgetInputDevice.None)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 90), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new KeyboardHelpDialog());
					}));
				}
				if ((base.Input.Devices & WidgetInputDevice.Gamepads) != WidgetInputDevice.None)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(GameMenuDialog.fName, 91), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new GamepadHelpDialog());
					}));
				}
				ListSelectionDialog dialog = new ListSelectionDialog(LanguageControl.Get(GameMenuDialog.fName, 92), list, 60f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
				{
					((Tuple<string, Action>)t).Item2();
				});
				DialogsManager.ShowDialog(base.ParentWidget, dialog);
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("Resume", true).IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
			if (this.Children.Find<ButtonWidget>("Quit", true).IsClicked)
			{
				DialogsManager.HideDialog(this);
				GameManager.SaveProject(true, true);
				GameManager.DisposeProject();
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x000960D1 File Offset: 0x000942D1
		public IEnumerable<ActiveExternalContentInfo> GetRateableItems()
		{
			if (GameManager.Project != null && UserManager.ActiveUser != null)
			{
				SubsystemGameInfo subsystemGameInfo = GameManager.Project.FindSubsystem<SubsystemGameInfo>(true);
				foreach (ActiveExternalContentInfo activeExternalContentInfo in subsystemGameInfo.GetActiveExternalContent())
				{
					if (!CommunityContentManager.IsContentRated(activeExternalContentInfo.Address, UserManager.ActiveUser.UniqueId))
					{
						yield return activeExternalContentInfo;
					}
				}
			}
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x000960DA File Offset: 0x000942DA
		public static string FormatDistance(double value)
		{
			if (value < 1000.0)
			{
				return string.Format("{0:0}m", value);
			}
			return string.Format("{0:N2}km", value / 1000.0);
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x00096114 File Offset: 0x00094314
		public void AddStat(ContainerWidget containerWidget, string title, string value1, string value2 = "")
		{
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			Color white = Color.White;
			Color gray = Color.Gray;
			containerWidget.Children.Add(new UniformSpacingPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Center,
				Children = 
				{
					new LabelWidget
					{
						Text = title + ":",
						HorizontalAlignment = WidgetAlignment.Far,
						Font = font,
						Color = gray,
						Margin = new Vector2(5f, 1f)
					},
					new StackPanelWidget
					{
						Direction = LayoutDirection.Horizontal,
						HorizontalAlignment = WidgetAlignment.Near,
						Children = 
						{
							new LabelWidget
							{
								Text = value1,
								Font = font,
								Color = white,
								Margin = new Vector2(5f, 1f)
							},
							new LabelWidget
							{
								Text = value2,
								Font = font,
								Color = gray,
								Margin = new Vector2(5f, 1f)
							}
						}
					}
				}
			});
		}

		// Token: 0x04000D18 RID: 3352
		public static bool m_increaseDetailDialogShown;

		// Token: 0x04000D19 RID: 3353
		public static bool m_decreaseDetailDialogShown;

		// Token: 0x04000D1A RID: 3354
		public bool m_adventureRestartExists;

		// Token: 0x04000D1B RID: 3355
		public StackPanelWidget m_statsPanel;

		// Token: 0x04000D1C RID: 3356
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000D1D RID: 3357
		public static string fName = "GameMenuDialog";
	}
}
