using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000142 RID: 322
	public class PlayScreen : Screen
	{
		// Token: 0x0600060C RID: 1548 RVA: 0x0002352C File Offset: 0x0002172C
		public PlayScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/PlayScreen");
			base.LoadContents(this, node);
			this.m_worldsListWidget = this.Children.Find<ListPanelWidget>("WorldsList", true);
			ListPanelWidget worldsListWidget = this.m_worldsListWidget;
			worldsListWidget.ItemWidgetFactory = (Func<object, Widget>)Delegate.Combine(worldsListWidget.ItemWidgetFactory, new Func<object, Widget>(delegate(object item)
			{
				WorldInfo worldInfo = (WorldInfo)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/SavedWorldItem");
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				LabelWidget labelWidget = containerWidget.Children.Find<LabelWidget>("WorldItem.Name", true);
				LabelWidget labelWidget2 = containerWidget.Children.Find<LabelWidget>("WorldItem.Details", true);
				containerWidget.Tag = worldInfo;
				labelWidget.Text = worldInfo.WorldSettings.Name;
				labelWidget2.Text = string.Format("{0} | {1:dd MMM yyyy HH:mm} | {2} | {3} | {4}", new object[]
				{
					DataSizeFormatter.Format(worldInfo.Size),
					worldInfo.LastSaveTime.ToLocalTime(),
					(worldInfo.PlayerInfos.Count > 1) ? string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 9), worldInfo.PlayerInfos.Count) : string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 10), 1),
					LanguageControl.Get("GameMode", worldInfo.WorldSettings.GameMode.ToString()),
					LanguageControl.Get("EnvironmentBehaviorMode", worldInfo.WorldSettings.EnvironmentBehaviorMode.ToString())
				});
				if (worldInfo.SerializationVersion != VersionsManager.SerializationVersion)
				{
					labelWidget2.Text = labelWidget2.Text + " | " + (string.IsNullOrEmpty(worldInfo.SerializationVersion) ? LanguageControl.GetContentWidgets("Usual", "Unknown") : ("(" + worldInfo.SerializationVersion + ")"));
				}
				return containerWidget;
			}));
			this.m_worldsListWidget.ScrollPosition = 0f;
			this.m_worldsListWidget.ScrollSpeed = 0f;
			this.m_worldsListWidget.ItemClicked += delegate(object item)
			{
				if (item != null && this.m_worldsListWidget.SelectedItem == item)
				{
					this.Play(item);
				}
			};
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x000235C8 File Offset: 0x000217C8
		public static void SaveLoadedModInfo(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			stream.SetLength(0L);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(1);
			binaryWriter.Write(ModsManager.quickAddModsFileList.Count);
			foreach (FileEntry fileEntry in ModsManager.quickAddModsFileList)
			{
				binaryWriter.Write(fileEntry.Filename);
			}
			binaryWriter.Close();
			stream.Close();
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0002365C File Offset: 0x0002185C
		public override void Enter(object[] parameters)
		{
			BusyDialog dialog = new BusyDialog(LanguageControl.GetContentWidgets(PlayScreen.fName, 5), null);
			DialogsManager.ShowDialog(null, dialog);
			Task.Run(delegate()
			{
				WorldInfo selectedItem = (WorldInfo)this.m_worldsListWidget.SelectedItem;
				WorldsManager.UpdateWorldsList();
				List<WorldInfo> worldInfos = new List<WorldInfo>(WorldsManager.WorldInfos);
				worldInfos.Sort((WorldInfo w1, WorldInfo w2) => DateTime.Compare(w2.LastSaveTime, w1.LastSaveTime));
				Dispatcher.Dispatch(delegate
				{
					this.m_worldsListWidget.ClearItems();
					foreach (WorldInfo item in worldInfos)
					{
						this.m_worldsListWidget.AddItem(item);
					}
					if (selectedItem != null)
					{
						this.m_worldsListWidget.SelectedItem = worldInfos.FirstOrDefault((WorldInfo wi) => wi.DirectoryName == selectedItem.DirectoryName);
					}
					DialogsManager.HideDialog(dialog);
				}, false);
			});
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x000236AC File Offset: 0x000218AC
		public override void Update()
		{
			if (this.m_worldsListWidget.SelectedItem != null && WorldsManager.WorldInfos.IndexOf((WorldInfo)this.m_worldsListWidget.SelectedItem) < 0)
			{
				this.m_worldsListWidget.SelectedItem = null;
			}
			this.Children.Find<LabelWidget>("TopBar.Label", true).Text = string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 6), this.m_worldsListWidget.Items.Count);
			this.Children.Find("Play", true).IsEnabled = (this.m_worldsListWidget.SelectedItem != null);
			this.Children.Find("Properties", true).IsEnabled = (this.m_worldsListWidget.SelectedItem != null);
			if (this.Children.Find<ButtonWidget>("Play", true).IsClicked && this.m_worldsListWidget.SelectedItem != null)
			{
				//Console.Beep();
				this.Play(this.m_worldsListWidget.SelectedItem);
			}
			if (this.Children.Find<ButtonWidget>("NewWorld", true).IsClicked)
			{
				//Console.Beep();
				if (WorldsManager.WorldInfos.Count >= PlayScreen.MaxWorlds)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.GetContentWidgets(PlayScreen.fName, 7), string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 8), PlayScreen.MaxWorlds), LanguageControl.GetContentWidgets("Usual", "ok"), null, null));
				}
				else
				{
					ScreensManager.SwitchScreen("NewWorld", Array.Empty<object>());
					this.m_worldsListWidget.SelectedItem = null;
				}
			}
			if (this.Children.Find<ButtonWidget>("Properties", true).IsClicked && this.m_worldsListWidget.SelectedItem != null)
			{
				WorldInfo worldInfo = (WorldInfo)this.m_worldsListWidget.SelectedItem;
				ScreensManager.SwitchScreen("ModifyWorld", new object[]
				{
					worldInfo.DirectoryName,
					worldInfo.WorldSettings
				});
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
				this.m_worldsListWidget.SelectedItem = null;
			}
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x000238E0 File Offset: 0x00021AE0
		public void Play(object item)
		{
			string text = "";
			DialogsManager.HideAllDialogs();
			WorldInfo worldInfo = (WorldInfo)item;
			List<string> list = new List<string>();
			string moddatapath = Storage.CombinePaths(new string[]
			{
				worldInfo.DirectoryName,
				"mod.dat"
			});
			Stream stream1;
			if (!Storage.FileExists(moddatapath))
			{
				stream1 = Storage.OpenFile(moddatapath, OpenFileMode.CreateOrOpen);
			}
			else
			{
				stream1 = Storage.OpenFile(moddatapath, OpenFileMode.ReadWrite);
			}
			if (stream1.Length == 0L)
			{
				PlayScreen.SaveLoadedModInfo(stream1);
			}
			else
			{
				BinaryReader binaryReader = new BinaryReader(stream1);
				if (binaryReader.BaseStream.Length > 0L && binaryReader.ReadInt32() != PlayScreen.ModChunkVer)
				{
					PlayScreen.SaveLoadedModInfo(stream1);
				}
				else
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						string fileitem = binaryReader.ReadString();
						if ((from p in ModsManager.quickAddModsFileList
						where p.Filename == fileitem
						select p).FirstOrDefault<FileEntry>() == null)
						{
							list.Add(fileitem);
							text = text + fileitem + "\n";
						}
					}
				}
			}
			if (list.Count > 0)
			{
				MessageDialog dialog = new MessageDialog(LanguageControl.GetContentWidgets(PlayScreen.fName, 11), text, LanguageControl.GetContentWidgets(PlayScreen.fName, 12), LanguageControl.GetContentWidgets(PlayScreen.fName, 13), delegate(MessageDialogButton btnText)
				{
					if (btnText == MessageDialogButton.Button1)
					{
						stream1.Close();
						stream1 = Storage.OpenFile(moddatapath, OpenFileMode.ReadWrite);
						PlayScreen.SaveLoadedModInfo(stream1);
						string name2 = "GameLoading";
						object[] array2 = new object[2];
						array2[0] = item;
						ScreensManager.SwitchScreen(name2, array2);
					}
					DialogsManager.HideAllDialogs();
				});
				DialogsManager.ShowDialog(this, dialog);
			}
			else
			{
				PlayScreen.SaveLoadedModInfo(stream1);
				string name = "GameLoading";
				object[] array = new object[2];
				array[0] = item;
				ScreensManager.SwitchScreen(name, array);
			}
			this.m_worldsListWidget.SelectedItem = null;
		}

		// Token: 0x040002F1 RID: 753
		public ListPanelWidget m_worldsListWidget;

		// Token: 0x040002F2 RID: 754
		public static int ModChunkVer = 1;

		// Token: 0x040002F3 RID: 755
		public static int MaxWorlds = 300;

		// Token: 0x040002F4 RID: 756
		public static string fName = "PlayScreen";
	}
}
