using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000134 RID: 308
	public class CommunityContentScreen : Screen
	{
		// Token: 0x060005AB RID: 1451 RVA: 0x0001F820 File Offset: 0x0001DA20
		public CommunityContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/CommunityContentScreen");
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ListPanelWidget>("List", true);
			this.m_orderLabel = this.Children.Find<LabelWidget>("Order", true);
			this.m_changeOrderButton = this.Children.Find<ButtonWidget>("ChangeOrder", true);
			this.m_filterLabel = this.Children.Find<LabelWidget>("Filter", true);
			this.m_changeFilterButton = this.Children.Find<ButtonWidget>("ChangeFilter", true);
			this.m_downloadButton = this.Children.Find<ButtonWidget>("Download", true);
			this.m_deleteButton = this.Children.Find<ButtonWidget>("Delete", true);
			this.m_moreOptionsButton = this.Children.Find<ButtonWidget>("MoreOptions", true);
			this.m_listPanel.ItemWidgetFactory = delegate(object item)
			{
				CommunityContentEntry communityContentEntry = item as CommunityContentEntry;
				if (communityContentEntry != null)
				{
					XElement node2 = ContentManager.Get<XElement>("Widgets/CommunityContentItem");
					ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
					containerWidget.Children.Find<RectangleWidget>("CommunityContentItem.Icon", true).Subtexture = ExternalContentManager.GetEntryTypeIcon(communityContentEntry.Type);
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.Text", true).Text = communityContentEntry.Name;
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.Details", true).Text = ExternalContentManager.GetEntryTypeDescription(communityContentEntry.Type) + " " + DataSizeFormatter.Format(communityContentEntry.Size);
					containerWidget.Children.Find<StarRatingWidget>("CommunityContentItem.Rating", true).Rating = communityContentEntry.RatingsAverage;
					containerWidget.Children.Find<StarRatingWidget>("CommunityContentItem.Rating", true).IsVisible = (communityContentEntry.RatingsAverage > 0f);
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.ExtraText", true).Text = communityContentEntry.ExtraText;
					return containerWidget;
				}
				XElement node3 = ContentManager.Get<XElement>("Widgets/CommunityContentItemMore");
				ContainerWidget containerWidget2 = (ContainerWidget)Widget.LoadWidget(this, node3, null);
				this.m_moreLink = containerWidget2.Children.Find<LinkWidget>("CommunityContentItemMore.Link", true);
				this.m_moreLink.Tag = (item as string);
				return containerWidget2;
			};
			this.m_listPanel.SelectionChanged += delegate()
			{
				if (this.m_listPanel.SelectedItem != null && !(this.m_listPanel.SelectedItem is CommunityContentEntry))
				{
					this.m_listPanel.SelectedItem = null;
				}
			};
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0001F937 File Offset: 0x0001DB37
		public override void Enter(object[] parameters)
		{
			this.m_filter = string.Empty;
			this.m_order = CommunityContentScreen.Order.ByRank;
			this.PopulateList(null);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0001F954 File Offset: 0x0001DB54
		public override void Update()
		{
			CommunityContentEntry communityContentEntry = this.m_listPanel.SelectedItem as CommunityContentEntry;
			this.m_downloadButton.IsEnabled = (communityContentEntry != null);
			this.m_deleteButton.IsEnabled = (UserManager.ActiveUser != null && communityContentEntry != null && communityContentEntry.UserId == UserManager.ActiveUser.UniqueId);
			this.m_orderLabel.Text = CommunityContentScreen.GetOrderDisplayName(this.m_order);
			this.m_filterLabel.Text = CommunityContentScreen.GetFilterDisplayName(this.m_filter);
			if (this.m_changeOrderButton.IsClicked)
			{
				List<CommunityContentScreen.Order> items = EnumUtils.GetEnumValues(typeof(CommunityContentScreen.Order)).Cast<CommunityContentScreen.Order>().ToList<CommunityContentScreen.Order>();
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(CommunityContentScreen.fName, "Order Type"), items, 60f, (object item) => CommunityContentScreen.GetOrderDisplayName((CommunityContentScreen.Order)item), delegate(object item)
				{
					this.m_order = (CommunityContentScreen.Order)item;
					this.PopulateList(null);
				}));
			}
			if (this.m_changeFilterButton.IsClicked)
			{
				List<object> list = new List<object>();
				list.Add(string.Empty);
				foreach (ExternalContentType externalContentType in from ExternalContentType t in EnumUtils.GetEnumValues(typeof(ExternalContentType))
				where ExternalContentManager.IsEntryTypeDownloadSupported(t)
				select t)
				{
					list.Add(externalContentType);
				}
				if (UserManager.ActiveUser != null)
				{
					list.Add(UserManager.ActiveUser.UniqueId);
				}
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(CommunityContentScreen.fName, "Filter"), list, 60f, (object item) => CommunityContentScreen.GetFilterDisplayName(item), delegate(object item)
				{
					this.m_filter = item;
					this.PopulateList(null);
				}));
			}
			if (this.m_downloadButton.IsClicked && communityContentEntry != null)
			{
				this.DownloadEntry(communityContentEntry);
			}
			if (this.m_deleteButton.IsClicked && communityContentEntry != null)
			{
				this.DeleteEntry(communityContentEntry);
			}
			if (this.m_moreOptionsButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new MoreCommunityLinkDialog());
			}
			if (this.m_moreLink != null && this.m_moreLink.IsClicked)
			{
				this.PopulateList((string)this.m_moreLink.Tag);
			}
			if (base.Input.Back || this.Children.Find<BevelledButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
			if (base.Input.Hold != null && base.Input.HoldTime > 2f && base.Input.Hold.Value.Y < 20f)
			{
				this.m_contentExpiryTime = 0.0;
				Task.Delay(250).Wait();
			}
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0001FC54 File Offset: 0x0001DE54
		public void PopulateList(string cursor)
		{
			string text = string.Empty;
			if (SettingsManager.CommunityContentMode == CommunityContentMode.Strict)
			{
				text = "1";
			}
			if (SettingsManager.CommunityContentMode == CommunityContentMode.Normal)
			{
				text = "0";
			}
			string text2 = (this.m_filter is string) ? ((string)this.m_filter) : string.Empty;
			string text3 = (this.m_filter is ExternalContentType) ? LanguageControl.Get(CommunityContentScreen.fName, this.m_filter.ToString()) : string.Empty;
			string text4 = LanguageControl.Get(CommunityContentScreen.fName, this.m_order.ToString());
			string cacheKey = string.Concat(new string[]
			{
				text2,
				"\n",
				text3,
				"\n",
				text4,
				"\n",
				text
			});
			this.m_moreLink = null;
			if (string.IsNullOrEmpty(cursor))
			{
				this.m_listPanel.ClearItems();
				this.m_listPanel.ScrollPosition = 0f;
				IEnumerable<object> enumerable;
				if (this.m_contentExpiryTime != 0.0 && Time.RealTime < this.m_contentExpiryTime && this.m_itemsCache.TryGetValue(cacheKey, out enumerable))
				{
					foreach (object item in enumerable)
					{
						this.m_listPanel.AddItem(item);
					}
					return;
				}
			}
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(CommunityContentScreen.fName, 2), false);
			DialogsManager.ShowDialog(null, busyDialog);
			CommunityContentManager.List(cursor, text2, text3, text, text4, busyDialog.Progress, delegate(List<CommunityContentEntry> list, string nextCursor)
			{
				DialogsManager.HideDialog(busyDialog);
				this.m_contentExpiryTime = Time.RealTime + 300.0;
				while (this.m_listPanel.Items.Count > 0 && !(this.m_listPanel.Items[this.m_listPanel.Items.Count - 1] is CommunityContentEntry))
				{
					this.m_listPanel.RemoveItemAt(this.m_listPanel.Items.Count - 1);
				}
				foreach (CommunityContentEntry item2 in list)
				{
					this.m_listPanel.AddItem(item2);
				}
				if (list.Count > 0 && !string.IsNullOrEmpty(nextCursor))
				{
					this.m_listPanel.AddItem(nextCursor);
				}
				this.m_itemsCache[cacheKey] = new List<object>(this.m_listPanel.Items);
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
			});
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0001FE2C File Offset: 0x0001E02C
		public void DownloadEntry(CommunityContentEntry entry)
		{
			string userId = (UserManager.ActiveUser != null) ? UserManager.ActiveUser.UniqueId : string.Empty;
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(string.Format(LanguageControl.Get(CommunityContentScreen.fName, 1), entry.Name), false);
			DialogsManager.ShowDialog(null, busyDialog);
			CommunityContentManager.Download(entry.Address, entry.Name, entry.Type, userId, busyDialog.Progress, delegate
			{
				DialogsManager.HideDialog(busyDialog);
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
			});
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0001FEC4 File Offset: 0x0001E0C4
		public void DeleteEntry(CommunityContentEntry entry)
		{
			if (UserManager.ActiveUser != null)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(CommunityContentScreen.fName, 4), LanguageControl.Get(CommunityContentScreen.fName, 5), LanguageControl.Get("Usual", "yes"), LanguageControl.Get("Usual", "no"), delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						CancellableBusyDialog busyDialog = new CancellableBusyDialog(string.Format(LanguageControl.Get(CommunityContentScreen.fName, 3), entry.Name), false);
						DialogsManager.ShowDialog(null, busyDialog);
						CommunityContentManager.Delete(entry.Address, UserManager.ActiveUser.UniqueId, busyDialog.Progress, delegate
						{
							DialogsManager.HideDialog(busyDialog);
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(CommunityContentScreen.fName, 6), LanguageControl.Get(CommunityContentScreen.fName, 7), LanguageControl.Get("Usual", "ok"), null, null));
						}, delegate(Exception error)
						{
							DialogsManager.HideDialog(busyDialog);
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get("Usual", "error"), error.Message, LanguageControl.Get("Usual", "ok"), null, null));
						});
					}
				}));
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0001FF30 File Offset: 0x0001E130
		public static string GetFilterDisplayName(object filter)
		{
			if (filter is string)
			{
				if (!string.IsNullOrEmpty((string)filter))
				{
					return LanguageControl.Get(CommunityContentScreen.fName, 8);
				}
				return LanguageControl.Get(CommunityContentScreen.fName, 9);
			}
			else
			{
				if (filter is ExternalContentType)
				{
					return ExternalContentManager.GetEntryTypeDescription((ExternalContentType)filter);
				}
				throw new InvalidOperationException(LanguageControl.Get(CommunityContentScreen.fName, 10));
			}
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0001FF90 File Offset: 0x0001E190
		public static string GetOrderDisplayName(CommunityContentScreen.Order order)
		{
			if (order == CommunityContentScreen.Order.ByRank)
			{
				return LanguageControl.Get(CommunityContentScreen.fName, 11);
			}
			if (order != CommunityContentScreen.Order.ByTime)
			{
				throw new InvalidOperationException(LanguageControl.Get(CommunityContentScreen.fName, 13));
			}
			return LanguageControl.Get(CommunityContentScreen.fName, 12);
		}

		// Token: 0x0400028C RID: 652
		public ListPanelWidget m_listPanel;

		// Token: 0x0400028D RID: 653
		public LinkWidget m_moreLink;

		// Token: 0x0400028E RID: 654
		public LabelWidget m_orderLabel;

		// Token: 0x0400028F RID: 655
		public ButtonWidget m_changeOrderButton;

		// Token: 0x04000290 RID: 656
		public LabelWidget m_filterLabel;

		// Token: 0x04000291 RID: 657
		public ButtonWidget m_changeFilterButton;

		// Token: 0x04000292 RID: 658
		public ButtonWidget m_downloadButton;

		// Token: 0x04000293 RID: 659
		public ButtonWidget m_deleteButton;

		// Token: 0x04000294 RID: 660
		public ButtonWidget m_moreOptionsButton;

		// Token: 0x04000295 RID: 661
		public object m_filter;

		// Token: 0x04000296 RID: 662
		public CommunityContentScreen.Order m_order;

		// Token: 0x04000297 RID: 663
		public double m_contentExpiryTime;

		// Token: 0x04000298 RID: 664
		public static string fName = "CommunityContentScreen";

		// Token: 0x04000299 RID: 665
		public Dictionary<string, IEnumerable<object>> m_itemsCache = new Dictionary<string, IEnumerable<object>>();

		// Token: 0x020003ED RID: 1005
		public enum Order
		{
			// Token: 0x040014B8 RID: 5304
			ByRank,
			// Token: 0x040014B9 RID: 5305
			ByTime
		}
	}
}
