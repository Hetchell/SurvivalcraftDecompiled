using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Game;

// Token: 0x02000002 RID: 2
public class ManageContentScreen : Screen
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public ManageContentScreen()
	{
		XElement node = ContentManager.Get<XElement>("Screens/ManageContentScreen");
		base.LoadContents(this, node);
		this.m_contentList = this.Children.Find<ListPanelWidget>("ContentList", true);
		this.m_deleteButton = this.Children.Find<ButtonWidget>("DeleteButton", true);
		this.m_uploadButton = this.Children.Find<ButtonWidget>("UploadButton", true);
		this.m_changeFilterButton = this.Children.Find<ButtonWidget>("ChangeFilter", true);
		this.m_filterLabel = this.Children.Find<LabelWidget>("Filter", true);
		this.m_contentList.ItemWidgetFactory = delegate(object obj)
		{
			ManageContentScreen.ListItem listItem = (ManageContentScreen.ListItem)obj;
			ContainerWidget containerWidget;
			if (listItem.Type == ExternalContentType.BlocksTexture)
			{
				XElement node2 = ContentManager.Get<XElement>("Widgets/BlocksTextureItem");
				containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				RectangleWidget rectangleWidget = containerWidget.Children.Find<RectangleWidget>("BlocksTextureItem.Icon", true);
				LabelWidget labelWidget = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Text", true);
				LabelWidget labelWidget2 = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Details", true);
				Texture2D texture = this.m_blocksTexturesCache.GetTexture(listItem.Name);
				BlocksTexturesManager.GetCreationDate(listItem.Name);
				rectangleWidget.Subtexture = new Subtexture(texture, Vector2.Zero, Vector2.One);
				labelWidget.Text = listItem.DisplayName;
				labelWidget2.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 1), texture.Width, texture.Height);
				if (!listItem.IsBuiltIn)
				{
					LabelWidget labelWidget3 = labelWidget2;
					labelWidget3.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
					if (listItem.UseCount > 0)
					{
						LabelWidget labelWidget4 = labelWidget2;
						labelWidget4.Text += string.Format(LanguageControl.Get(ManageContentScreen.fName, 2), listItem.UseCount);
					}
				}
			}
			else
			{
				if (listItem.Type != ExternalContentType.CharacterSkin)
				{
					if (listItem.Type == ExternalContentType.FurniturePack)
					{
						XElement node3 = ContentManager.Get<XElement>("Widgets/FurniturePackItem");
						containerWidget = (ContainerWidget)Widget.LoadWidget(this, node3, null);
						LabelWidget labelWidget5 = containerWidget.Children.Find<LabelWidget>("FurniturePackItem.Text", true);
						LabelWidget labelWidget6 = containerWidget.Children.Find<LabelWidget>("FurniturePackItem.Details", true);
						labelWidget5.Text = listItem.DisplayName;
						try
						{
							List<FurnitureDesign> designs = FurniturePacksManager.LoadFurniturePack(null, listItem.Name);
							labelWidget6.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 3), FurnitureDesign.ListChains(designs).Count);
							if (string.IsNullOrEmpty(listItem.Name))
							{
								return containerWidget;
							}
							LabelWidget labelWidget7 = labelWidget6;
							labelWidget7.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
							return containerWidget;
						}
						catch (Exception ex)
						{
							labelWidget6.Text = labelWidget6.Text + LanguageControl.Get("Usual", "error") + ex.Message;
							return containerWidget;
						}
					}
					throw new InvalidOperationException(LanguageControl.Get(ManageContentScreen.fName, 10));
				}
				XElement node4 = ContentManager.Get<XElement>("Widgets/CharacterSkinItem");
				containerWidget = (ContainerWidget)Widget.LoadWidget(this, node4, null);
				PlayerModelWidget playerModelWidget = containerWidget.Children.Find<PlayerModelWidget>("CharacterSkinItem.Model", true);
				LabelWidget labelWidget8 = containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Text", true);
				LabelWidget labelWidget9 = containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Details", true);
				Texture2D texture2 = this.m_characterSkinsCache.GetTexture(listItem.Name);
				playerModelWidget.PlayerClass = PlayerClass.Male;
				playerModelWidget.CharacterSkinTexture = texture2;
				labelWidget8.Text = listItem.DisplayName;
				labelWidget9.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 4), texture2.Width, texture2.Height);
				if (!listItem.IsBuiltIn)
				{
					LabelWidget labelWidget10 = labelWidget9;
					labelWidget10.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
					if (listItem.UseCount > 0)
					{
						LabelWidget labelWidget11 = labelWidget9;
						labelWidget11.Text += string.Format(LanguageControl.Get(ManageContentScreen.fName, 2), listItem.UseCount);
					}
				}
			}
			return containerWidget;
		};
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002116 File Offset: 0x00000316
	public override void Enter(object[] parameters)
	{
		this.UpdateList();
	}

	// Token: 0x06000003 RID: 3 RVA: 0x0000211E File Offset: 0x0000031E
	public override void Leave()
	{
		this.m_blocksTexturesCache.Clear();
		this.m_characterSkinsCache.Clear();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002138 File Offset: 0x00000338
	public override void Update()
	{
		ManageContentScreen.ListItem selectedItem = (ManageContentScreen.ListItem)this.m_contentList.SelectedItem;
		this.m_deleteButton.IsEnabled = (selectedItem != null && !selectedItem.IsBuiltIn);
		this.m_uploadButton.IsEnabled = (selectedItem != null && !selectedItem.IsBuiltIn);
		this.m_filterLabel.Text = ManageContentScreen.GetFilterDisplayName(this.m_filter);
		if (this.m_deleteButton.IsClicked)
		{
			string smallMessage = (selectedItem.UseCount <= 0) ? string.Format(LanguageControl.Get(ManageContentScreen.fName, 5), selectedItem.DisplayName) : string.Format(LanguageControl.Get(ManageContentScreen.fName, 6), selectedItem.DisplayName, selectedItem.UseCount);
			DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ManageContentScreen.fName, 9), smallMessage, LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton button)
			{
				if (button == MessageDialogButton.Button1)
				{
					ExternalContentManager.DeleteExternalContent(selectedItem.Type, selectedItem.Name);
					this.UpdateList();
				}
			}));
		}
		if (this.m_uploadButton.IsClicked)
		{
			ExternalContentManager.ShowUploadUi(selectedItem.Type, selectedItem.Name);
		}
		if (this.m_changeFilterButton.IsClicked)
		{
			List<ExternalContentType> list = new List<ExternalContentType>();
			list.Add(ExternalContentType.Unknown);
			list.Add(ExternalContentType.BlocksTexture);
			list.Add(ExternalContentType.CharacterSkin);
			list.Add(ExternalContentType.FurniturePack);
			DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(ManageContentScreen.fName, 7), list, 60f, (object item) => ManageContentScreen.GetFilterDisplayName((ExternalContentType)item), delegate(object item)
			{
				if ((ExternalContentType)item != this.m_filter)
				{
					this.m_filter = (ExternalContentType)item;
					this.UpdateList();
				}
			}));
		}
		if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
		{
			ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
		}
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002350 File Offset: 0x00000550
	public void UpdateList()
	{
		WorldsManager.UpdateWorldsList();
		List<ManageContentScreen.ListItem> list = new List<ManageContentScreen.ListItem>();
		if (this.m_filter == ExternalContentType.BlocksTexture || this.m_filter == ExternalContentType.Unknown)
		{
			BlocksTexturesManager.UpdateBlocksTexturesList();
			foreach (string name2 in BlocksTexturesManager.BlockTexturesNames)
			{
				list.Add(new ManageContentScreen.ListItem
				{
					Name = name2,
					IsBuiltIn = BlocksTexturesManager.IsBuiltIn(name2),
					Type = ExternalContentType.BlocksTexture,
					DisplayName = BlocksTexturesManager.GetDisplayName(name2),
					CreationTime = BlocksTexturesManager.GetCreationDate(name2),
					UseCount = WorldsManager.WorldInfos.Count((WorldInfo wi) => wi.WorldSettings.BlocksTextureName == name2)
				});
			}
		}
		if (this.m_filter == ExternalContentType.CharacterSkin || this.m_filter == ExternalContentType.Unknown)
		{
			CharacterSkinsManager.UpdateCharacterSkinsList();
			using (ReadOnlyList<string>.Enumerator enumerator = CharacterSkinsManager.CharacterSkinsNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string name = enumerator.Current;
					Func<PlayerInfo, bool> predicate0 = null;
					list.Add(new ManageContentScreen.ListItem
					{
						Name = name,
						IsBuiltIn = CharacterSkinsManager.IsBuiltIn(name),
						Type = ExternalContentType.CharacterSkin,
						DisplayName = CharacterSkinsManager.GetDisplayName(name),
						CreationTime = CharacterSkinsManager.GetCreationDate(name),
						UseCount = WorldsManager.WorldInfos.Count(delegate(WorldInfo wi)
						{
							IEnumerable<PlayerInfo> playerInfos = wi.PlayerInfos;
							Func<PlayerInfo, bool> predicate;
							if ((predicate = predicate0) == null)
							{
								predicate = (predicate0 = ((PlayerInfo pi) => pi.CharacterSkinName == name));
							}
							return playerInfos.Any(predicate);
						})
					});
				}
			}
		}
		if (this.m_filter == ExternalContentType.FurniturePack || this.m_filter == ExternalContentType.Unknown)
		{
			FurniturePacksManager.UpdateFurniturePacksList();
			foreach (string name2 in FurniturePacksManager.FurniturePackNames)
			{
				list.Add(new ManageContentScreen.ListItem
				{
					Name = name2,
					IsBuiltIn = false,
					Type = ExternalContentType.FurniturePack,
					DisplayName = FurniturePacksManager.GetDisplayName(name2),
					CreationTime = FurniturePacksManager.GetCreationDate(name2)
				});
			}
		}
		list.Sort(delegate(ManageContentScreen.ListItem o1, ManageContentScreen.ListItem o2)
		{
			if (o1.IsBuiltIn && !o2.IsBuiltIn)
			{
				return -1;
			}
			if (o2.IsBuiltIn && !o1.IsBuiltIn)
			{
				return 1;
			}
			if (string.IsNullOrEmpty(o1.Name) && !string.IsNullOrEmpty(o2.Name))
			{
				return -1;
			}
			if (string.IsNullOrEmpty(o1.Name) || !string.IsNullOrEmpty(o2.Name))
			{
				return string.Compare(o1.DisplayName, o2.DisplayName);
			}
			return 1;
		});
		this.m_contentList.ClearItems();
		foreach (ManageContentScreen.ListItem item in list)
		{
			this.m_contentList.AddItem(item);
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x0000262C File Offset: 0x0000082C
	public static string GetFilterDisplayName(ExternalContentType filter)
	{
		if (filter == ExternalContentType.Unknown)
		{
			return LanguageControl.Get(ManageContentScreen.fName, 8);
		}
		return ExternalContentManager.GetEntryTypeDescription(filter);
	}

	// Token: 0x04000001 RID: 1
	public static string fName = "ManageContentScreen";

	// Token: 0x04000002 RID: 2
	public ListPanelWidget m_contentList;

	// Token: 0x04000003 RID: 3
	public ButtonWidget m_deleteButton;

	// Token: 0x04000004 RID: 4
	public ButtonWidget m_uploadButton;

	// Token: 0x04000005 RID: 5
	public LabelWidget m_filterLabel;

	// Token: 0x04000006 RID: 6
	public ButtonWidget m_changeFilterButton;

	// Token: 0x04000007 RID: 7
	public BlocksTexturesCache m_blocksTexturesCache = new BlocksTexturesCache();

	// Token: 0x04000008 RID: 8
	public CharacterSkinsCache m_characterSkinsCache = new CharacterSkinsCache();

	// Token: 0x04000009 RID: 9
	public ExternalContentType m_filter;

	// Token: 0x020003AC RID: 940
	public class ListItem
	{
		// Token: 0x040013C2 RID: 5058
		public ExternalContentType Type;

		// Token: 0x040013C3 RID: 5059
		public bool IsBuiltIn;

		// Token: 0x040013C4 RID: 5060
		public string Name;

		// Token: 0x040013C5 RID: 5061
		public string DisplayName;

		// Token: 0x040013C6 RID: 5062
		public DateTime CreationTime;

		// Token: 0x040013C7 RID: 5063
		public int UseCount;
	}
}
