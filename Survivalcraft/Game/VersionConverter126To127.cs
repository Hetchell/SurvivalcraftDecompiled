using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000340 RID: 832
	public class VersionConverter126To127 : VersionConverter
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x000BA160 File Offset: 0x000B8360
		public override string SourceVersion
		{
			get
			{
				return "1.26";
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x000BA167 File Offset: 0x000B8367
		public override string TargetVersion
		{
			get
			{
				return "1.27";
			}
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000BA16E File Offset: 0x000B836E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ConvertTypesToEngine(projectNode);
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x000BA188 File Offset: 0x000B8388
		public override void ConvertWorld(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x000BA20C File Offset: 0x000B840C
		public static void MigrateDataFromIsolatedStorageWithDialog()
		{
			try
			{
				if (Storage.DirectoryExists("app:/.config/.isolated-storage"))
				{
					Log.Information("1.26 data found, starting migration to 1.27.");
					BusyDialog dialog = new BusyDialog("Please wait", "Migrating 1.26 data to 1.27 format...");
					DialogsManager.ShowDialog(null, dialog);
					Task.Run(delegate()
					{
						string largeMessage = string.Empty;
						string text = string.Empty;
						try
						{
							int num = VersionConverter126To127.MigrateFolder("app:/.config/.isolated-storage", "data:");
							largeMessage = "Migration Successful";
							text = string.Format("{0} file(s) were migrated from 1.26 to 1.27.", num);
							AnalyticsManager.LogEvent("[Migration to 1.27]", new AnalyticsParameter[]
							{
								new AnalyticsParameter("Details", text)
							});
						}
						catch (Exception ex2)
						{
							largeMessage = "Migration Failed";
							text = ex2.Message;
							Log.Error("Migration to 1.27 failed, reason: {0}", new object[]
							{
								ex2.Message
							});
							AnalyticsManager.LogError("Migration to 1.27 failed", ex2);
						}
						DialogsManager.HideDialog(dialog);
						DialogsManager.ShowDialog(null, new MessageDialog(largeMessage, text, "OK", null, null));
						Dispatcher.Dispatch(delegate
						{
							SettingsManager.LoadSettings();
						}, false);
					});
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to migrate data. Reason: {0}", new object[]
				{
					ex.Message
				});
				AnalyticsManager.LogError("Migration to 1.27 failed", ex);
			}
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x000BA2A4 File Offset: 0x000B84A4
		public void ConvertTypesToEngine(XElement node)
		{
			foreach (XElement xelement in node.DescendantsAndSelf("Value"))
			{
				XAttribute xattribute = xelement.Attribute("Type");
				if (xattribute != null)
				{
					if (xattribute.Value == "Microsoft.Xna.Framework.Vector2")
					{
						xattribute.Value = "Engine.Vector2";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Vector3")
					{
						xattribute.Value = "Engine.Vector3";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Vector4")
					{
						xattribute.Value = "Engine.Vector4";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Quaternion")
					{
						xattribute.Value = "Engine.Quaternion";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Matrix")
					{
						xattribute.Value = "Engine.Matrix";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Point")
					{
						xattribute.Value = "Engine.Point2";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Color")
					{
						xattribute.Value = "Engine.Color";
					}
					else if (xattribute.Value == "Game.Point3")
					{
						xattribute.Value = "Engine.Point3";
					}
				}
			}
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000BA41C File Offset: 0x000B861C
		public static int MigrateFolder(string sourceFolderName, string targetFolderName)
		{
			int num = 0;
			Storage.CreateDirectory(targetFolderName);
			foreach (string text in Storage.ListDirectoryNames(sourceFolderName))
			{
				num += VersionConverter126To127.MigrateFolder(Storage.CombinePaths(new string[]
				{
					sourceFolderName,
					text
				}), Storage.CombinePaths(new string[]
				{
					targetFolderName,
					text
				}));
			}
			foreach (string text2 in Storage.ListFileNames(sourceFolderName))
			{
				VersionConverter126To127.MigrateFile(Storage.CombinePaths(new string[]
				{
					sourceFolderName,
					text2
				}), targetFolderName);
				num++;
			}
			Storage.DeleteDirectory(sourceFolderName);
			Log.Information("Migrated {0}", new object[]
			{
				sourceFolderName
			});
			return num;
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000BA508 File Offset: 0x000B8708
		public static void MigrateFile(string sourceFileName, string targetFolderName)
		{
			Storage.CopyFile(sourceFileName, Storage.CombinePaths(new string[]
			{
				targetFolderName,
				Storage.GetFileName(sourceFileName)
			}));
			Storage.DeleteFile(sourceFileName);
			Log.Information("Migrated {0}", new object[]
			{
				sourceFileName
			});
		}
	}
}
