using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Engine;

namespace Game
{
	// Token: 0x0200026E RID: 622
	public static class ExternalContentManager
	{
		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x0600126D RID: 4717 RVA: 0x0008E360 File Offset: 0x0008C560
		public static IExternalContentProvider DefaultProvider
		{
			get
			{
				if (ExternalContentManager.Providers.Count <= 0)
				{
					return null;
				}
				return ExternalContentManager.Providers[0];
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x0008E38D File Offset: 0x0008C58D
		public static ReadOnlyList<IExternalContentProvider> Providers
		{
			get
			{
				return new ReadOnlyList<IExternalContentProvider>(ExternalContentManager.m_providers);
			}
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0008E39C File Offset: 0x0008C59C
		public static void Initialize()
		{
			ExternalContentManager.m_providers = new List<IExternalContentProvider>();
			ExternalContentManager.m_providers.Add(new SPMBoxExternalContentProvider());
			ExternalContentManager.m_providers.Add(new DiskExternalContentProvider());
			ExternalContentManager.m_providers.Add(new DropboxExternalContentProvider());
			ExternalContentManager.m_providers.Add(new TransferShExternalContentProvider());
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0008E3F0 File Offset: 0x0008C5F0
		public static ExternalContentType ExtensionToType(string extension)
		{
			extension = extension.ToLower();
			Func<string, bool> predicate0 = null;
			foreach (object obj in Enum.GetValues(typeof(ExternalContentType)))
			{
				ExternalContentType externalContentType = (ExternalContentType)obj;
				IEnumerable<string> entryTypeExtensions = ExternalContentManager.GetEntryTypeExtensions(externalContentType);
				Func<string, bool> predicate;
				if ((predicate = predicate0) == null)
				{
					predicate = (predicate0 = ((string e) => e == extension));
				}
				if (entryTypeExtensions.FirstOrDefault(predicate) != null)
				{
					return externalContentType;
				}
			}
			return ExternalContentType.Unknown;
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x0008E4A4 File Offset: 0x0008C6A4
		public static IEnumerable<string> GetEntryTypeExtensions(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.World:
				yield return ".scworld";
				break;
			case ExternalContentType.BlocksTexture:
				yield return ".scbtex";
				yield return ".png";
				break;
			case ExternalContentType.CharacterSkin:
				yield return ".scskin";
				break;
			case ExternalContentType.FurniturePack:
				yield return ".scfpack";
				break;
			}
			yield break;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0008E4B4 File Offset: 0x0008C6B4
		public static Subtexture GetEntryTypeIcon(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.Directory:
				return ContentManager.Get<Subtexture>("Textures/Atlas/FolderIcon");
			case ExternalContentType.World:
				return ContentManager.Get<Subtexture>("Textures/Atlas/WorldIcon");
			case ExternalContentType.BlocksTexture:
				return ContentManager.Get<Subtexture>("Textures/Atlas/TexturePackIcon");
			case ExternalContentType.CharacterSkin:
				return ContentManager.Get<Subtexture>("Textures/Atlas/CharacterSkinIcon");
			case ExternalContentType.FurniturePack:
				return ContentManager.Get<Subtexture>("Textures/Atlas/FurnitureIcon");
			default:
				return ContentManager.Get<Subtexture>("Textures/Atlas/QuestionMarkIcon");
			}
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0008E520 File Offset: 0x0008C720
		public static string GetEntryTypeDescription(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.Directory:
				return LanguageControl.Get(ExternalContentManager.fName, "Directory");
			case ExternalContentType.World:
				return LanguageControl.Get(ExternalContentManager.fName, "World");
			case ExternalContentType.BlocksTexture:
				return LanguageControl.Get(ExternalContentManager.fName, "Blocks Texture");
			case ExternalContentType.CharacterSkin:
				return LanguageControl.Get(ExternalContentManager.fName, "Character Skin");
			case ExternalContentType.FurniturePack:
				return LanguageControl.Get(ExternalContentManager.fName, "Furniture Pack");
			default:
				return string.Empty;
			}
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0008E5A0 File Offset: 0x0008C7A0
		public static bool IsEntryTypeDownloadSupported(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.World:
				return true;
			case ExternalContentType.BlocksTexture:
				return true;
			case ExternalContentType.CharacterSkin:
				return true;
			case ExternalContentType.FurniturePack:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0008E5C5 File Offset: 0x0008C7C5
		public static bool DoesEntryTypeRequireName(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.BlocksTexture:
				return true;
			case ExternalContentType.CharacterSkin:
				return true;
			case ExternalContentType.FurniturePack:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0008E5E4 File Offset: 0x0008C7E4
		public static Exception VerifyExternalContentName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 1));
			}
			if (name.Length > 50)
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 2));
			}
			if (name[0] == ' ' || name[name.Length - 1] == ' ')
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 3));
			}
			return null;
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0008E654 File Offset: 0x0008C854
		public static void DeleteExternalContent(ExternalContentType type, string name)
		{
			switch (type)
			{
			case ExternalContentType.World:
				WorldsManager.DeleteWorld(name);
				return;
			case ExternalContentType.BlocksTexture:
				BlocksTexturesManager.DeleteBlocksTexture(name);
				return;
			case ExternalContentType.CharacterSkin:
				CharacterSkinsManager.DeleteCharacterSkin(name);
				return;
			case ExternalContentType.FurniturePack:
				FurniturePacksManager.DeleteFurniturePack(name);
				return;
			default:
				throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 4));
			}
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0008E6A7 File Offset: 0x0008C8A7
		public static void ImportExternalContent(Stream stream, ExternalContentType type, string name, Action<string> success, Action<Exception> failure)
		{
			Task.Run(delegate()
			{
				try
				{
					success(ExternalContentManager.ImportExternalContentSync(stream, type, name));
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			});
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0008E6E4 File Offset: 0x0008C8E4
		public static string ImportExternalContentSync(Stream stream, ExternalContentType type, string name)
		{
			switch (type)
			{
			case ExternalContentType.World:
				return WorldsManager.ImportWorld(stream);
			case ExternalContentType.BlocksTexture:
				return BlocksTexturesManager.ImportBlocksTexture(name, stream);
			case ExternalContentType.CharacterSkin:
				return CharacterSkinsManager.ImportCharacterSkin(name, stream);
			case ExternalContentType.FurniturePack:
				return FurniturePacksManager.ImportFurniturePack(name, stream);
			default:
				throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 4));
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0008E73C File Offset: 0x0008C93C
		public static void ShowLoginUiIfNeeded(IExternalContentProvider provider, bool showWarningDialog, Action handler)
		{
			if (provider.RequiresLogin && !provider.IsLoggedIn)
			{
				Action loginAction = delegate()
				{
					CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentManager.fName, 5), true);
					DialogsManager.ShowDialog(null, busyDialog);
					provider.Login(busyDialog.Progress, delegate
					{
						DialogsManager.HideDialog(busyDialog);
						Action handler3 = handler;
						if (handler3 == null)
						{
							return;
						}
						handler3();
					}, delegate(Exception error)
					{
						DialogsManager.HideDialog(busyDialog);
						if (error != null)
						{
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
						}
					});
				};
				if (showWarningDialog)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ExternalContentManager.fName, 6), string.Format(LanguageControl.Get(ExternalContentManager.fName, 7), provider.DisplayName), LanguageControl.Get(ExternalContentManager.fName, 8), LanguageControl.Get("Usual", "cancel"), delegate(MessageDialogButton b)
					{
						if (b == MessageDialogButton.Button1)
						{
							loginAction();
						}
					}));
					return;
				}
				loginAction();
				return;
			}
			else
			{
				Action handler2 = handler;
				if (handler2 == null)
				{
					return;
				}
				handler2();
				return;
			}
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x0008E820 File Offset: 0x0008CA20
		public static void ShowUploadUi(ExternalContentType type, string name)
		{
			DialogsManager.ShowDialog(null, new SelectExternalContentProviderDialog(LanguageControl.Get(ExternalContentManager.fName, 9), false, delegate(IExternalContentProvider provider)
			{
				try
				{
					if (provider != null)
					{
						ExternalContentManager.ShowLoginUiIfNeeded(provider, true, delegate
						{
							CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentManager.fName, 10), false);
							DialogsManager.ShowDialog(null, busyDialog);
							Task.Run(delegate()
							{
								bool needsDelete = false;
								string sourcePath = null;
								Stream stream = null;
								Action cleanup = delegate()
								{
									Utilities.Dispose<Stream>(ref stream);
									if (needsDelete && sourcePath != null)
									{
										try
										{
											Storage.DeleteFile(sourcePath);
										}
										catch
										{
										}
									}
								};
								try
								{
									string path;
									if (type == ExternalContentType.BlocksTexture)
									{
										sourcePath = BlocksTexturesManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else if (type == ExternalContentType.CharacterSkin)
									{
										sourcePath = CharacterSkinsManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else if (type == ExternalContentType.FurniturePack)
									{
										sourcePath = FurniturePacksManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else
									{
										if (type != ExternalContentType.World)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 12));
										}
										busyDialog.LargeMessage = LanguageControl.Get(ExternalContentManager.fName, 13);
										sourcePath = "android:SurvivalCraft2.2/WorldUpload.tmp";
										needsDelete = true;
										path = WorldsManager.GetWorldInfo(name).WorldSettings.Name + ".scworld";
										using (Stream stream2 = Storage.OpenFile(sourcePath, OpenFileMode.Create))
										{
											WorldsManager.ExportWorld(name, stream2);
										}
									}
									busyDialog.LargeMessage = LanguageControl.Get(ExternalContentManager.fName, 14);
									stream = Storage.OpenFile(sourcePath, OpenFileMode.Read);
									provider.Upload(path, stream, busyDialog.Progress, delegate(string link)
									{
										long length = stream.Length;
										cleanup();
										DialogsManager.HideDialog(busyDialog);
										if (string.IsNullOrEmpty(link))
										{
											DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "success"), string.Format(LanguageControl.Get(ExternalContentManager.fName, 15), DataSizeFormatter.Format(length)), LanguageControl.Get("Usual", "ok"), null, null));
											return;
										}
										DialogsManager.ShowDialog(null, new ExternalContentLinkDialog(link));
									}, delegate(Exception error)
									{
										cleanup();
										DialogsManager.HideDialog(busyDialog);
										DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
									});
								}
								catch (Exception ex2)
								{
									cleanup();
									DialogsManager.HideDialog(busyDialog);
									DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), ex2.Message, LanguageControl.Get("Usual", "ok"), null, null));
								}
							});
						});
					}
				}
				catch (Exception ex)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
				}
			}));
		}

		// Token: 0x04000CA8 RID: 3240
		public static List<IExternalContentProvider> m_providers;

		// Token: 0x04000CA9 RID: 3241
		public static string fName = "ExternalContentManager";
	}
}
