using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000284 RID: 644
	public static class GameManager
	{
		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x0009481C File Offset: 0x00092A1C
		public static Project Project
		{
			get
			{
				return GameManager.m_project;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x00094823 File Offset: 0x00092A23
		public static WorldInfo WorldInfo
		{
			get
			{
				return GameManager.m_worldInfo;
			}
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0009482C File Offset: 0x00092A2C
		public static void LoadProject(WorldInfo worldInfo, ContainerWidget gamesWidget)
		{
			GameManager.DisposeProject();
			WorldsManager.RepairWorldIfNeeded(worldInfo.DirectoryName);
			VersionsManager.UpgradeWorld(worldInfo.DirectoryName);
			using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				worldInfo.DirectoryName,
				"Project.xml"
			}), OpenFileMode.Read))
			{
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				ValuesDictionary valuesDictionary2 = new ValuesDictionary();
				valuesDictionary.SetValue<ValuesDictionary>("GameInfo", valuesDictionary2);
				valuesDictionary2.SetValue<string>("WorldDirectoryName", worldInfo.DirectoryName);
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary.SetValue<ValuesDictionary>("Views", valuesDictionary3);
				valuesDictionary3.SetValue<ContainerWidget>("GamesWidget", gamesWidget);
				XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
				ProjectData projectData = new ProjectData(DatabaseManager.GameDatabase, xelement, valuesDictionary, true);
				GameManager.m_project = new Project(DatabaseManager.GameDatabase, projectData);
				GameManager.m_subsystemUpdate = GameManager.m_project.FindSubsystem<SubsystemUpdate>(true);
			}
			GameManager.m_worldInfo = worldInfo;
			Log.Information("Loaded world, GameMode={0}, StartingPosition={1}, WorldName={2}, VisibilityRange={3}, Resolution={4}", new object[]
			{
				worldInfo.WorldSettings.GameMode,
				worldInfo.WorldSettings.StartingPositionMode,
				worldInfo.WorldSettings.Name,
				SettingsManager.VisibilityRange.ToString(),
				SettingsManager.ResolutionMode.ToString()
			});
			AnalyticsManager.LogEvent("[GameManager] Loaded world", new AnalyticsParameter[]
			{
				new AnalyticsParameter("GameMode", worldInfo.WorldSettings.GameMode.ToString()),
				new AnalyticsParameter("EnvironmentBehaviorMode", worldInfo.WorldSettings.EnvironmentBehaviorMode.ToString()),
				new AnalyticsParameter("TerrainGenerationMode", worldInfo.WorldSettings.TerrainGenerationMode.ToString()),
				new AnalyticsParameter("WorldDirectory", worldInfo.DirectoryName),
				new AnalyticsParameter("WorldName", worldInfo.WorldSettings.Name),
				new AnalyticsParameter("WorldSeedString", worldInfo.WorldSettings.Seed),
				new AnalyticsParameter("VisibilityRange", SettingsManager.VisibilityRange.ToString()),
				new AnalyticsParameter("Resolution", SettingsManager.ResolutionMode.ToString())
			});
			GC.Collect();
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x00094AA4 File Offset: 0x00092CA4
		public static void SaveProject(bool waitForCompletion, bool showErrorDialog)
		{
			if (GameManager.m_project != null)
			{
				double realTime = Time.RealTime;
				ProjectData projectData = GameManager.m_project.Save();
				GameManager.m_saveCompleted.WaitOne();
				GameManager.m_saveCompleted.Reset();
				SubsystemGameInfo subsystemGameInfo = GameManager.m_project.FindSubsystem<SubsystemGameInfo>(true);
				string projectFileName = Storage.CombinePaths(new string[]
				{
					subsystemGameInfo.DirectoryName,
					"Project.xml"
				});
				Task.Run(delegate()
				{
					try
					{
						WorldsManager.MakeQuickWorldBackup(subsystemGameInfo.DirectoryName);
						XElement xelement = new XElement("Project");
						projectData.Save(xelement);
						XmlUtils.SetAttributeValue(xelement, "Version", VersionsManager.SerializationVersion);
						Storage.CreateDirectory(subsystemGameInfo.DirectoryName);
						using (Stream stream = Storage.OpenFile(projectFileName, OpenFileMode.Create))
						{
							XmlUtils.SaveXmlToStream(xelement, stream, null, true);
						}
					}
					catch (Exception e)
					{
						if (showErrorDialog)
						{
							Dispatcher.Dispatch(delegate
							{
								DialogsManager.ShowDialog(null, new MessageDialog("Error saving game", e.Message, "OK", null, null));
							}, false);
						}
					}
					finally
					{
						GameManager.m_saveCompleted.Set();
					}
				});
				if (waitForCompletion)
				{
					GameManager.m_saveCompleted.WaitOne();
				}
				double realTime2 = Time.RealTime;
				Log.Verbose(string.Format("Saved project, {0}ms", MathUtils.Round((realTime2 - realTime) * 1000.0)));
			}
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x00094B7F File Offset: 0x00092D7F
		public static void UpdateProject()
		{
			if (GameManager.m_project != null)
			{
				GameManager.m_subsystemUpdate.Update();
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x00094B92 File Offset: 0x00092D92
		public static void DisposeProject()
		{
			if (GameManager.m_project != null)
			{
				GameManager.m_project.Dispose();
				GameManager.m_project = null;
				GameManager.m_subsystemUpdate = null;
				GameManager.m_worldInfo = null;
				GC.Collect();
			}
		}

		// Token: 0x04000D14 RID: 3348
		public static WorldInfo m_worldInfo;

		// Token: 0x04000D15 RID: 3349
		public static Project m_project;

		// Token: 0x04000D16 RID: 3350
		public static SubsystemUpdate m_subsystemUpdate;

		// Token: 0x04000D17 RID: 3351
		public static ManualResetEvent m_saveCompleted = new ManualResetEvent(true);
	}
}
