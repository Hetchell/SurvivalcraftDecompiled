using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200035E RID: 862
	public static class WorldsManager
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x0600182E RID: 6190 RVA: 0x000BF33E File Offset: 0x000BD53E
		public static ReadOnlyList<string> NewWorldNames
		{
			get
			{
				return WorldsManager.m_newWorldNames;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x0600182F RID: 6191 RVA: 0x000BF345 File Offset: 0x000BD545
		public static ReadOnlyList<WorldInfo> WorldInfos
		{
			get
			{
				return new ReadOnlyList<WorldInfo>(WorldsManager.m_worldInfos);
			}
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001830 RID: 6192 RVA: 0x000BF354 File Offset: 0x000BD554
		// (remove) Token: 0x06001831 RID: 6193 RVA: 0x000BF388 File Offset: 0x000BD588
		public static event Action<string> WorldDeleted;

		// Token: 0x06001832 RID: 6194 RVA: 0x000BF3BC File Offset: 0x000BD5BC
		public static void Initialize()
		{
			Storage.CreateDirectory(WorldsManager.WorldsDirectoryName);
			string text = ContentManager.Get<string>("NewWorldNames");
			WorldsManager.m_newWorldNames = new ReadOnlyList<string>(text.Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries));
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x000BF400 File Offset: 0x000BD600
		public static string ImportWorld(Stream sourceStream)
		{
			if (MarketplaceManager.IsTrialMode)
			{
				throw new InvalidOperationException("Cannot import worlds in trial mode.");
			}
			if (WorldsManager.WorldInfos.Count >= 30)
			{
				throw new InvalidOperationException(string.Format("Too many worlds on device, maximum allowed is {0}. Delete some to free up space.", 30));
			}
			string unusedWorldDirectoryName = WorldsManager.GetUnusedWorldDirectoryName();
			Storage.CreateDirectory(unusedWorldDirectoryName);
			WorldsManager.UnpackWorld(unusedWorldDirectoryName, sourceStream, true);
			if (!WorldsManager.TestXmlFile(Storage.CombinePaths(new string[]
			{
				unusedWorldDirectoryName,
				"Project.xml"
			}), "Project"))
			{
				try
				{
					WorldsManager.DeleteWorld(unusedWorldDirectoryName);
				}
				catch
				{
				}
				throw new InvalidOperationException("Cannot import world because it does not contain valid world data.");
			}
			return unusedWorldDirectoryName;
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x000BF4A8 File Offset: 0x000BD6A8
		public static void ExportWorld(string directoryName, Stream targetStream)
		{
			WorldsManager.PackWorld(directoryName, targetStream, null, true);
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x000BF4B3 File Offset: 0x000BD6B3
		public static void DeleteWorld(string directoryName)
		{
			if (Storage.DirectoryExists(directoryName))
			{
				WorldsManager.DeleteWorldContents(directoryName, null);
				Storage.DeleteDirectory(directoryName);
			}
			Action<string> worldDeleted = WorldsManager.WorldDeleted;
			if (worldDeleted == null)
			{
				return;
			}
			worldDeleted(directoryName);
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x000BF4DC File Offset: 0x000BD6DC
		public static void RepairWorldIfNeeded(string directoryName)
		{
			try
			{
				string text = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.xml"
				});
				if (!WorldsManager.TestXmlFile(text, "Project"))
				{
					Log.Warning("Project file at \"" + text + "\" is corrupt or nonexistent. Will try copying data from the backup file. If that fails, will try making a recovery project file.");
					string text2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						"Project.bak"
					});
					if (WorldsManager.TestXmlFile(text2, "Project"))
					{
						Storage.CopyFile(text2, text);
					}
					else
					{
						string path = Storage.CombinePaths(new string[]
						{
							directoryName,
							"Chunks.dat"
						});
						if (!Storage.FileExists(path))
						{
							throw new InvalidOperationException("Recovery project file could not be generated because chunks file does not exist.");
						}
						XElement xelement = ContentManager.Get<XElement>("RecoveryProject");
						using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
						{
							int num;
							int num2;
							int num3;
							TerrainSerializer14.ReadTOCEntry(stream, out num, out num2, out num3);
							Vector3 vector = new Vector3((float)(16 * num), 255f, (float)(16 * num2));
							xelement.Element("Subsystems").Element("Values").Element("Value").Attribute("Value").SetValue(HumanReadableConverter.ConvertToString(vector));
						}
						using (Stream stream2 = Storage.OpenFile(text, OpenFileMode.Create))
						{
							XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new InvalidOperationException("The world files are corrupt and could not be repaired.");
			}
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x000BF68C File Offset: 0x000BD88C
		public static void MakeQuickWorldBackup(string directoryName)
		{
			string text = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			if (Storage.FileExists(text))
			{
				string destinationPath = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.bak"
				});
				Storage.CopyFile(text, destinationPath);
			}
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x000BF6D8 File Offset: 0x000BD8D8
		public static bool SnapshotExists(string directoryName, string snapshotName)
		{
			return Storage.FileExists(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName));
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x000BF6E8 File Offset: 0x000BD8E8
		public static void TakeWorldSnapshot(string directoryName, string snapshotName)
		{
			using (Stream stream = Storage.OpenFile(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName), OpenFileMode.Create))
			{
				WorldsManager.PackWorld(directoryName, stream, (string fn) => Path.GetExtension(fn).ToLower() != ".snapshot", false);
			}
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x000BF748 File Offset: 0x000BD948
		public static void RestoreWorldFromSnapshot(string directoryName, string snapshotName)
		{
			if (WorldsManager.SnapshotExists(directoryName, snapshotName))
			{
				WorldsManager.DeleteWorldContents(directoryName, (string fn) => Storage.GetExtension(fn).ToLower() != ".snapshot");
				using (Stream stream = Storage.OpenFile(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName), OpenFileMode.Read))
				{
					WorldsManager.UnpackWorld(directoryName, stream, false);
				}
			}
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000BF7B8 File Offset: 0x000BD9B8
		public static void DeleteWorldSnapshot(string directoryName, string snapshotName)
		{
			string path = WorldsManager.MakeSnapshotFilename(directoryName, snapshotName);
			if (Storage.FileExists(path))
			{
				Storage.DeleteFile(path);
			}
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x000BF7DC File Offset: 0x000BD9DC
		public static void UpdateWorldsList()
		{
			WorldsManager.m_worldInfos.Clear();
			foreach (string text in Storage.ListDirectoryNames(WorldsManager.WorldsDirectoryName))
			{
				WorldInfo worldInfo = WorldsManager.GetWorldInfo(Storage.CombinePaths(new string[]
				{
					WorldsManager.WorldsDirectoryName,
					text
				}));
				if (worldInfo != null)
				{
					WorldsManager.m_worldInfos.Add(worldInfo);
				}
			}
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x000BF85C File Offset: 0x000BDA5C
		public static bool ValidateWorldName(string name)
		{
			if (name.Length == 0 || name.Length > 14)
			{
				return false;
			}
			if (!char.IsLetterOrDigit(name[0]) || !char.IsLetterOrDigit(name[name.Length - 1]))
			{
				return false;
			}
			foreach (char c in name)
			{
				if (c > '\u007f')
				{
					return false;
				}
				if (!char.IsLetterOrDigit(c) && c != ' ')
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x000BF8D4 File Offset: 0x000BDAD4
		public static WorldInfo GetWorldInfo(string directoryName)
		{
			WorldInfo worldInfo = new WorldInfo();
			worldInfo.DirectoryName = directoryName;
			worldInfo.LastSaveTime = DateTime.MinValue;
			List<string> list = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, null, null);
			if (list.Count > 0)
			{
				foreach (string text in list)
				{
					DateTime fileLastWriteTime = Storage.GetFileLastWriteTime(text);
					if (fileLastWriteTime > worldInfo.LastSaveTime)
					{
						worldInfo.LastSaveTime = fileLastWriteTime;
					}
					try
					{
						worldInfo.Size += Storage.GetFileSize(text);
					}
					catch (Exception e)
					{
						Log.Error(ExceptionManager.MakeFullErrorMessage("Error getting size of file \"" + text + "\".", e));
					}
				}
				string text2 = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.xml"
				});
				try
				{
					if (Storage.FileExists(text2))
					{
						using (Stream stream = Storage.OpenFile(text2, OpenFileMode.Read))
						{
							XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
							worldInfo.SerializationVersion = XmlUtils.GetAttributeValue<string>(xelement, "Version", "1.0");
							VersionsManager.UpgradeProjectXml(xelement);
							XElement gameInfoNode = WorldsManager.GetGameInfoNode(xelement);
							ValuesDictionary valuesDictionary = new ValuesDictionary();
							valuesDictionary.ApplyOverrides(gameInfoNode);
							worldInfo.WorldSettings.Load(valuesDictionary);
							foreach (XContainer xcontainer in (from e in WorldsManager.GetPlayersNode(xelement).Elements()
							where XmlUtils.GetAttributeValue<string>(e, "Name") == "Players"
							select e).First<XElement>().Elements())
							{
								PlayerInfo playerInfo = new PlayerInfo();
								worldInfo.PlayerInfos.Add(playerInfo);
								XElement xelement2 = (from e in xcontainer.Elements()
								where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CharacterSkinName"
								select e).FirstOrDefault<XElement>();
								if (xelement2 != null)
								{
									playerInfo.CharacterSkinName = XmlUtils.GetAttributeValue<string>(xelement2, "Value", string.Empty);
								}
							}
							return worldInfo;
						}
					}
					return worldInfo;
				}
				catch (Exception e2)
				{
					Log.Error(ExceptionManager.MakeFullErrorMessage("Error getting data from project file \"" + text2 + "\".", e2));
					return worldInfo;
				}
			}
			return null;
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x000BFB8C File Offset: 0x000BDD8C
		public static WorldInfo CreateWorld(WorldSettings worldSettings)
		{
			string unusedWorldDirectoryName = WorldsManager.GetUnusedWorldDirectoryName();
			Storage.CreateDirectory(unusedWorldDirectoryName);
			if (!WorldsManager.ValidateWorldName(worldSettings.Name))
			{
				throw new InvalidOperationException("World name \"" + worldSettings.Name + "\" is invalid.");
			}
			int num;
			if (string.IsNullOrEmpty(worldSettings.Seed))
			{
				num = (int)((long)(Time.RealTime * 1000.0));
			}
			else if (worldSettings.Seed == "0")
			{
				num = 0;
			}
			else
			{
				num = 0;
				int num2 = 1;
				foreach (char c in worldSettings.Seed)
				{
					num += (int)c * num2;
					num2 += 29;
				}
			}
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			worldSettings.Save(valuesDictionary, false);
			valuesDictionary.SetValue<string>("WorldDirectoryName", unusedWorldDirectoryName);
			valuesDictionary.SetValue<int>("WorldSeed", num);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary2.SetValue<ValuesDictionary>("Players", new ValuesDictionary());
			DatabaseObject databaseObject = DatabaseManager.GameDatabase.Database.FindDatabaseObject("GameProject", DatabaseManager.GameDatabase.ProjectTemplateType, true);
			XElement xelement = new XElement("Project");
			XmlUtils.SetAttributeValue(xelement, "Guid", databaseObject.Guid);
			XmlUtils.SetAttributeValue(xelement, "Name", "GameProject");
			XmlUtils.SetAttributeValue(xelement, "Version", VersionsManager.SerializationVersion);
			XElement xelement2 = new XElement("Subsystems");
			xelement.Add(xelement2);
			XElement xelement3 = new XElement("Values");
			XmlUtils.SetAttributeValue(xelement3, "Name", "GameInfo");
			valuesDictionary.Save(xelement3);
			xelement2.Add(xelement3);
			XElement xelement4 = new XElement("Values");
			XmlUtils.SetAttributeValue(xelement4, "Name", "Players");
			valuesDictionary2.Save(xelement4);
			xelement2.Add(xelement4);
			using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				unusedWorldDirectoryName,
				"Project.xml"
			}), OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream, null, true);
			}
			return WorldsManager.GetWorldInfo(unusedWorldDirectoryName);
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x000BFDB4 File Offset: 0x000BDFB4
		public static void ChangeWorld(string directoryName, WorldSettings worldSettings)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			if (!Storage.FileExists(path))
			{
				return;
			}
			XElement xelement = null;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			XElement gameInfoNode = WorldsManager.GetGameInfoNode(xelement);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.ApplyOverrides(gameInfoNode);
			GameMode value = valuesDictionary.GetValue<GameMode>("GameMode");
			worldSettings.Save(valuesDictionary, true);
			gameInfoNode.RemoveNodes();
			valuesDictionary.Save(gameInfoNode);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
			if (worldSettings.GameMode != value)
			{
				if (worldSettings.GameMode == GameMode.Adventure)
				{
					WorldsManager.TakeWorldSnapshot(directoryName, "AdventureRestart");
					return;
				}
				WorldsManager.DeleteWorldSnapshot(directoryName, "AdventureRestart");
			}
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x000BFEA4 File Offset: 0x000BE0A4
		public static string GetUnusedWorldDirectoryName()
		{
			string text = Storage.CombinePaths(new string[]
			{
				WorldsManager.WorldsDirectoryName,
				"World"
			});
			for (int i = 0; i < 1000; i++)
			{
				string str = Storage.CombinePaths(new string[]
				{
					Storage.GetDirectoryName(text),
					Storage.GetFileNameWithoutExtension(text)
				});
				string extension = Storage.GetExtension(text);
				string text2 = str + ((i > 0) ? i.ToString() : string.Empty) + extension;
				if (!Storage.DirectoryExists(text2) && !Storage.FileExists(text2))
				{
					return text2;
				}
			}
			throw new InvalidOperationException("Out of filenames for root \"" + text + "\".");
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x000BFF44 File Offset: 0x000BE144
		public static void RecursiveEnumerateDirectory(string directoryName, List<string> files, List<string> directories, Func<string, bool> filesFilter)
		{
			try
			{
				foreach (string text in Storage.ListDirectoryNames(directoryName))
				{
					string text2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						text
					});
					WorldsManager.RecursiveEnumerateDirectory(text2, files, directories, filesFilter);
					if (directories != null)
					{
						directories.Add(text2);
					}
				}
				if (files != null)
				{
					foreach (string text3 in Storage.ListFileNames(directoryName))
					{
						string text4 = Storage.CombinePaths(new string[]
						{
							directoryName,
							text3
						});
						if (filesFilter == null || filesFilter(text4))
						{
							files.Add(text4);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error enumerating files/directories. Reason: " + ex.Message);
			}
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000C003C File Offset: 0x000BE23C
		public static XElement GetGameInfoNode(XElement projectNode)
		{
			XElement xelement = (from n in projectNode.Element("Subsystems").Elements("Values")
			where XmlUtils.GetAttributeValue<string>(n, "Name", string.Empty) == "GameInfo"
			select n).FirstOrDefault<XElement>();
			if (xelement != null)
			{
				return xelement;
			}
			throw new InvalidOperationException("GameInfo node not found in project.");
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000C00A4 File Offset: 0x000BE2A4
		public static XElement GetPlayersNode(XElement projectNode)
		{
			XElement xelement = (from n in projectNode.Element("Subsystems").Elements("Values")
			where XmlUtils.GetAttributeValue<string>(n, "Name", string.Empty) == "Players"
			select n).FirstOrDefault<XElement>();
			if (xelement != null)
			{
				return xelement;
			}
			throw new InvalidOperationException("Players node not found in project.");
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x000C010C File Offset: 0x000BE30C
		public static void PackWorld(string directoryName, Stream targetStream, Func<string, bool> filter, bool embedExternalContent)
		{
			WorldInfo worldInfo = WorldsManager.GetWorldInfo(directoryName);
			if (worldInfo == null)
			{
				throw new InvalidOperationException("Directory does not contain a world.");
			}
			List<string> list = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, null, filter);
			using (ZipArchive zipArchive = ZipArchive.Create(targetStream, true))
			{
				foreach (string path in list)
				{
					using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
					{
						string fileName = Storage.GetFileName(path);
						zipArchive.AddStream(fileName, stream);
					}
				}
				if (embedExternalContent)
				{
					if (!BlocksTexturesManager.IsBuiltIn(worldInfo.WorldSettings.BlocksTextureName))
					{
						try
						{
							using (Stream stream2 = Storage.OpenFile(BlocksTexturesManager.GetFileName(worldInfo.WorldSettings.BlocksTextureName), OpenFileMode.Read))
							{
								string filenameInZip = Storage.CombinePaths(new string[]
								{
									"EmbeddedContent",
									Storage.GetFileNameWithoutExtension(worldInfo.WorldSettings.BlocksTextureName) + ".scbtex"
								});
								zipArchive.AddStream(filenameInZip, stream2);
							}
						}
						catch (Exception ex)
						{
							Log.Warning("Failed to embed blocks texture \"" + worldInfo.WorldSettings.BlocksTextureName + "\". Reason: " + ex.Message);
						}
					}
					foreach (PlayerInfo playerInfo in worldInfo.PlayerInfos)
					{
						if (!CharacterSkinsManager.IsBuiltIn(playerInfo.CharacterSkinName))
						{
							try
							{
								using (Stream stream3 = Storage.OpenFile(CharacterSkinsManager.GetFileName(playerInfo.CharacterSkinName), OpenFileMode.Read))
								{
									string filenameInZip2 = Storage.CombinePaths(new string[]
									{
										"EmbeddedContent",
										Storage.GetFileNameWithoutExtension(playerInfo.CharacterSkinName) + ".scskin"
									});
									zipArchive.AddStream(filenameInZip2, stream3);
								}
							}
							catch (Exception ex2)
							{
								Log.Warning("Failed to embed character skin \"" + playerInfo.CharacterSkinName + "\". Reason: " + ex2.Message);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000C03E0 File Offset: 0x000BE5E0
		public static void UnpackWorld(string directoryName, Stream sourceStream, bool importEmbeddedExternalContent)
		{
			if (!Storage.DirectoryExists(directoryName))
			{
				throw new InvalidOperationException("Cannot import world into \"" + directoryName + "\" because this directory does not exist.");
			}
			using (ZipArchive zipArchive = ZipArchive.Open(sourceStream, true))
			{
				foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.ReadCentralDir())
				{
					string text = zipArchiveEntry.FilenameInZip.Replace('\\', '/');
					string extension = Storage.GetExtension(text);
					if (text.StartsWith("EmbeddedContent"))
					{
						try
						{
							if (importEmbeddedExternalContent)
							{
								MemoryStream memoryStream = new MemoryStream();
								zipArchive.ExtractFile(zipArchiveEntry, memoryStream);
								memoryStream.Position = 0L;
								ExternalContentType type = ExternalContentManager.ExtensionToType(extension);
								ExternalContentManager.ImportExternalContentSync(memoryStream, type, Storage.GetFileNameWithoutExtension(text));
							}
							continue;
						}
						catch (Exception ex)
						{
							Log.Warning("Failed to import embedded content \"" + text + "\". Reason: " + ex.Message);
							continue;
						}
					}
					using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileName(text)
					}), OpenFileMode.Create))
					{
						zipArchive.ExtractFile(zipArchiveEntry, stream);
					}
				}
			}
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x000C053C File Offset: 0x000BE73C
		public static void DeleteWorldContents(string directoryName, Func<string, bool> filter)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, list2, filter);
			foreach (string path in list)
			{
				Storage.DeleteFile(path);
			}
			foreach (string path2 in list2)
			{
				Storage.DeleteDirectory(path2);
			}
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000C05D8 File Offset: 0x000BE7D8
		public static string MakeSnapshotFilename(string directoryName, string snapshotName)
		{
			return Storage.CombinePaths(new string[]
			{
				directoryName,
				snapshotName + ".snapshot"
			});
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000C05F8 File Offset: 0x000BE7F8
		public static bool TestXmlFile(string fileName, string rootNodeName)
		{
			bool result;
			try
			{
				if (Storage.FileExists(fileName))
				{
					using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
					{
						XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, false);
						return xelement != null && xelement.Name == rootNodeName;
					}
				}
				result = false;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04001137 RID: 4407
		public static List<WorldInfo> m_worldInfos = new List<WorldInfo>();

		// Token: 0x04001138 RID: 4408
		public static ReadOnlyList<string> m_newWorldNames;

		// Token: 0x04001139 RID: 4409
		public static string WorldsDirectoryName = "app:/Worlds";

		// Token: 0x0400113A RID: 4410
		public const int MaxAllowedWorlds = 30;
	}
}
