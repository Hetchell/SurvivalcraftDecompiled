using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000136 RID: 310
	public class ExternalContentScreen : Screen
	{
		// Token: 0x060005BA RID: 1466 RVA: 0x000202C0 File Offset: 0x0001E4C0
		public ExternalContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ExternalContentScreen");
			base.LoadContents(this, node);
			this.m_directoryLabel = this.Children.Find<LabelWidget>("TopBar.Label", true);
			this.m_directoryList = this.Children.Find<ListPanelWidget>("DirectoryList", true);
			this.m_providerNameLabel = this.Children.Find<LabelWidget>("ProviderName", true);
			this.m_changeProviderButton = this.Children.Find<ButtonWidget>("ChangeProvider", true);
			this.m_loginLogoutButton = this.Children.Find<ButtonWidget>("LoginLogout", true);
			this.m_upDirectoryButton = this.Children.Find<ButtonWidget>("UpDirectory", true);
			this.m_actionButton = this.Children.Find<ButtonWidget>("Action", true);
			this.m_copyLinkButton = this.Children.Find<ButtonWidget>("CopyLink", true);
			this.m_directoryList.ItemWidgetFactory = delegate(object item)
			{
				ExternalContentEntry externalContentEntry = (ExternalContentEntry)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/ExternalContentItem");
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				string fileName = Storage.GetFileName(externalContentEntry.Path);
				string text = this.m_downloadedFiles.ContainsKey(externalContentEntry.Path) ? LanguageControl.Get(ExternalContentScreen.fName, 11) : string.Empty;
				string text2 = (externalContentEntry.Type != ExternalContentType.Directory) ? string.Format("{0} | {1} | {2:dd-MMM-yyyy HH:mm}{3}", new object[]
				{
					ExternalContentManager.GetEntryTypeDescription(externalContentEntry.Type),
					DataSizeFormatter.Format(externalContentEntry.Size),
					externalContentEntry.Time,
					text
				}) : ExternalContentManager.GetEntryTypeDescription(externalContentEntry.Type);
				containerWidget.Children.Find<RectangleWidget>("ExternalContentItem.Icon", true).Subtexture = ExternalContentManager.GetEntryTypeIcon(externalContentEntry.Type);
				containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Text", true).Text = fileName;
				containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Details", true).Text = text2;
				return containerWidget;
			};
			this.m_directoryList.ItemClicked += delegate(object item)
			{
				if (this.m_directoryList.SelectedItem == item)
				{
					ExternalContentEntry externalContentEntry = item as ExternalContentEntry;
					if (externalContentEntry != null && externalContentEntry.Type == ExternalContentType.Directory)
					{
						this.SetPath(externalContentEntry.Path);
					}
				}
			};
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x000203E2 File Offset: 0x0001E5E2
		public override void Enter(object[] parameters)
		{
			this.m_directoryList.ClearItems();
			this.SetPath(null);
			this.m_listDirty = true;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00020400 File Offset: 0x0001E600
		public override void Update()
		{
			if (this.m_listDirty)
			{
				this.m_listDirty = false;
				this.UpdateList();
			}
			ExternalContentEntry externalContentEntry = null;
			if (this.m_directoryList.SelectedIndex != null)
			{
				externalContentEntry = (this.m_directoryList.Items[this.m_directoryList.SelectedIndex.Value] as ExternalContentEntry);
			}
			if (externalContentEntry != null)
			{
				this.m_actionButton.IsVisible = true;
				if (externalContentEntry.Type == ExternalContentType.Directory)
				{
					this.m_actionButton.Text = LanguageControl.Get(ExternalContentScreen.fName, 1);
					this.m_actionButton.IsEnabled = true;
					this.m_copyLinkButton.IsEnabled = false;
				}
				else
				{
					this.m_actionButton.Text = LanguageControl.Get(ExternalContentScreen.fName, 2);
					if (ExternalContentManager.IsEntryTypeDownloadSupported(ExternalContentManager.ExtensionToType(Storage.GetExtension(externalContentEntry.Path).ToLower())))
					{
						this.m_actionButton.IsEnabled = true;
						this.m_copyLinkButton.IsEnabled = true;
					}
					else
					{
						this.m_actionButton.IsEnabled = false;
						this.m_copyLinkButton.IsEnabled = false;
					}
				}
			}
			else
			{
				this.m_actionButton.IsVisible = false;
				this.m_copyLinkButton.IsVisible = false;
			}
			this.m_directoryLabel.Text = (this.m_externalContentProvider.IsLoggedIn ? string.Format(LanguageControl.Get(ExternalContentScreen.fName, 3), this.m_path) : LanguageControl.Get(ExternalContentScreen.fName, 4));
			this.m_providerNameLabel.Text = this.m_externalContentProvider.DisplayName;
			this.m_upDirectoryButton.IsEnabled = (this.m_externalContentProvider.IsLoggedIn && this.m_path != "/");
			this.m_loginLogoutButton.Text = (this.m_externalContentProvider.IsLoggedIn ? LanguageControl.Get(ExternalContentScreen.fName, 5) : LanguageControl.Get(ExternalContentScreen.fName, 6));
			this.m_loginLogoutButton.IsVisible = this.m_externalContentProvider.RequiresLogin;
			this.m_copyLinkButton.IsVisible = this.m_externalContentProvider.SupportsLinks;
			this.m_copyLinkButton.IsEnabled = (externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type));
			if (this.m_changeProviderButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new SelectExternalContentProviderDialog(LanguageControl.Get(ExternalContentScreen.fName, 7), true, delegate(IExternalContentProvider provider)
				{
					this.m_externalContentProvider = provider;
					this.m_listDirty = true;
					this.SetPath(null);
				}));
			}
			if (this.m_upDirectoryButton.IsClicked)
			{
				string directoryName = Storage.GetDirectoryName(this.m_path);
				this.SetPath(directoryName);
			}
			if (this.m_actionButton.IsClicked && externalContentEntry != null)
			{
				if (externalContentEntry.Type == ExternalContentType.Directory)
				{
					this.SetPath(externalContentEntry.Path);
				}
				else
				{
					this.DownloadEntry(externalContentEntry);
				}
			}
			if (this.m_copyLinkButton.IsClicked && externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type))
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentScreen.fName, 8), false);
				DialogsManager.ShowDialog(null, busyDialog);
				this.m_externalContentProvider.Link(externalContentEntry.Path, busyDialog.Progress, delegate(string link)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new ExternalContentLinkDialog(link));
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
				});
			}
			if (this.m_loginLogoutButton.IsClicked)
			{
				if (this.m_externalContentProvider.IsLoggedIn)
				{
					this.m_externalContentProvider.Logout();
					this.SetPath(null);
					this.m_listDirty = true;
				}
				else
				{
					ExternalContentManager.ShowLoginUiIfNeeded(this.m_externalContentProvider, false, delegate
					{
						this.SetPath(null);
						this.m_listDirty = true;
					});
				}
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x000207B0 File Offset: 0x0001E9B0
		public void SetPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = Storage.GetSystemPath("android:SurvivalCraft2.2/files");
			}
			path = path.Replace("\\", "/");
			if (path != this.m_path)
			{
				this.m_path = path;
				this.m_listDirty = true;
			}
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00020800 File Offset: 0x0001EA00
		public void UpdateList()
		{
			this.m_directoryList.ClearItems();
			if (this.m_externalContentProvider != null && this.m_externalContentProvider.IsLoggedIn)
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentScreen.fName, 9), false);
				DialogsManager.ShowDialog(null, busyDialog);
				this.m_externalContentProvider.List(this.m_path, busyDialog.Progress, delegate(ExternalContentEntry entry)
				{
					DialogsManager.HideDialog(busyDialog);
					List<ExternalContentEntry> list = new List<ExternalContentEntry>((from e in entry.ChildEntries
					where ExternalContentScreen.EntryFilter(e)
					select e).Take(1000));
					this.m_directoryList.ClearItems();
					list.Sort(delegate(ExternalContentEntry e1, ExternalContentEntry e2)
					{
						if (e1.Type == ExternalContentType.Directory && e2.Type != ExternalContentType.Directory)
						{
							return -1;
						}
						if (e1.Type == ExternalContentType.Directory || e2.Type != ExternalContentType.Directory)
						{
							return string.Compare(e1.Path, e2.Path);
						}
						return 1;
					});
					foreach (ExternalContentEntry item in list)
					{
						this.m_directoryList.AddItem(item);
					}
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
				});
			}
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00020894 File Offset: 0x0001EA94
		public void DownloadEntry(ExternalContentEntry entry)
		{
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentScreen.fName, 10), false);
			DialogsManager.ShowDialog(null, busyDialog);
			this.m_externalContentProvider.Download(entry.Path, busyDialog.Progress, delegate(Stream stream)
			{
				busyDialog.LargeMessage = LanguageControl.Get(ExternalContentScreen.fName, 12);
				ExternalContentManager.ImportExternalContent(stream, entry.Type, Storage.GetFileName(entry.Path), delegate
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
				}, delegate(Exception error)
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
				});
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
			});
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x0002090B File Offset: 0x0001EB0B
		public static bool EntryFilter(ExternalContentEntry entry)
		{
			return entry.Type > ExternalContentType.Unknown;
		}

		// Token: 0x0400029E RID: 670
		public LabelWidget m_directoryLabel;

		// Token: 0x0400029F RID: 671
		public ListPanelWidget m_directoryList;

		// Token: 0x040002A0 RID: 672
		public LabelWidget m_providerNameLabel;

		// Token: 0x040002A1 RID: 673
		public ButtonWidget m_changeProviderButton;

		// Token: 0x040002A2 RID: 674
		public ButtonWidget m_loginLogoutButton;

		// Token: 0x040002A3 RID: 675
		public ButtonWidget m_upDirectoryButton;

		// Token: 0x040002A4 RID: 676
		public ButtonWidget m_actionButton;

		// Token: 0x040002A5 RID: 677
		public ButtonWidget m_copyLinkButton;

		// Token: 0x040002A6 RID: 678
		public string m_path;

		// Token: 0x040002A7 RID: 679
		public static string fName = "ExternalContentScreen";

		// Token: 0x040002A8 RID: 680
		public bool m_listDirty;

		// Token: 0x040002A9 RID: 681
		public Dictionary<string, bool> m_downloadedFiles = new Dictionary<string, bool>();

		// Token: 0x040002AA RID: 682
		public IExternalContentProvider m_externalContentProvider = ExternalContentManager.DefaultProvider;
	}
}
