using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200027F RID: 639
	public class FurnitureInventoryPanel : CanvasWidget
	{
		// Token: 0x170002BD RID: 701
		// (get) Token: 0x060012DA RID: 4826 RVA: 0x00093104 File Offset: 0x00091304
		// (set) Token: 0x060012DB RID: 4827 RVA: 0x0009310C File Offset: 0x0009130C
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060012DC RID: 4828 RVA: 0x00093115 File Offset: 0x00091315
		// (set) Token: 0x060012DD RID: 4829 RVA: 0x0009311D File Offset: 0x0009131D
		public SubsystemFurnitureBlockBehavior SubsystemFurnitureBlockBehavior { get; set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060012DE RID: 4830 RVA: 0x00093126 File Offset: 0x00091326
		// (set) Token: 0x060012DF RID: 4831 RVA: 0x0009312E File Offset: 0x0009132E
		public ComponentFurnitureInventory ComponentFurnitureInventory { get; set; }

		// Token: 0x060012E0 RID: 4832 RVA: 0x00093138 File Offset: 0x00091338
		public FurnitureInventoryPanel(CreativeInventoryWidget creativeInventoryWidget)
		{
			this.m_creativeInventoryWidget = creativeInventoryWidget;
			this.ComponentFurnitureInventory = creativeInventoryWidget.Entity.FindComponent<ComponentFurnitureInventory>(true);
			this.m_componentPlayer = creativeInventoryWidget.Entity.FindComponent<ComponentPlayer>(true);
			this.SubsystemFurnitureBlockBehavior = this.ComponentFurnitureInventory.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			this.SubsystemTerrain = this.ComponentFurnitureInventory.Project.FindSubsystem<SubsystemTerrain>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/FurnitureInventoryPanel");
			base.LoadContents(this, node);
			this.m_furnitureSetList = this.Children.Find<ListPanelWidget>("FurnitureSetList", true);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_addButton = this.Children.Find<ButtonWidget>("AddButton", true);
			this.m_moreButton = this.Children.Find<ButtonWidget>("MoreButton", true);
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget();
					this.m_inventoryGrid.Children.Add(widget);
					this.m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
			ListPanelWidget furnitureSetList = this.m_furnitureSetList;
			furnitureSetList.ItemWidgetFactory = (Func<object, Widget>)Delegate.Combine(furnitureSetList.ItemWidgetFactory, new Func<object, Widget>((object item) => new FurnitureSetItemWidget(this, (FurnitureSet)item)));
			this.m_furnitureSetList.SelectionChanged += delegate()
			{
				if (!this.m_ignoreSelectionChanged && this.ComponentFurnitureInventory.FurnitureSet != this.m_furnitureSetList.SelectedItem as FurnitureSet)
				{
					this.ComponentFurnitureInventory.PageIndex = 0;
					this.ComponentFurnitureInventory.FurnitureSet = (this.m_furnitureSetList.SelectedItem as FurnitureSet);
					if (this.ComponentFurnitureInventory.FurnitureSet == null)
					{
						this.m_furnitureSetList.SelectedIndex = new int?(0);
					}
					this.AssignInventorySlots();
				}
			};
			this.m_populateNeeded = true;
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x000932B0 File Offset: 0x000914B0
		public override void Update()
		{
			if (this.m_populateNeeded)
			{
				this.Populate();
				this.m_populateNeeded = false;
			}
			if (this.ComponentFurnitureInventory.PageIndex != this.m_assignedPage)
			{
				this.AssignInventorySlots();
			}
			this.m_creativeInventoryWidget.PageUpButton.IsEnabled = (this.ComponentFurnitureInventory.PageIndex > 0);
			this.m_creativeInventoryWidget.PageDownButton.IsEnabled = (this.ComponentFurnitureInventory.PageIndex < this.m_pagesCount - 1);
			this.m_creativeInventoryWidget.PageLabel.Text = ((this.m_pagesCount > 0) ? string.Format("{0}/{1}", this.ComponentFurnitureInventory.PageIndex + 1, this.m_pagesCount) : string.Empty);
			this.m_moreButton.IsEnabled = (this.ComponentFurnitureInventory.FurnitureSet != null);
			if (base.Input.Scroll != null && base.HitTestGlobal(base.Input.Scroll.Value.XY, null).IsChildWidgetOf(this.m_inventoryGrid))
			{
				this.ComponentFurnitureInventory.PageIndex -= (int)base.Input.Scroll.Value.Z;
			}
			if (this.m_creativeInventoryWidget.PageUpButton.IsClicked)
			{
				ComponentFurnitureInventory componentFurnitureInventory = this.ComponentFurnitureInventory;
				int pageIndex = componentFurnitureInventory.PageIndex - 1;
				componentFurnitureInventory.PageIndex = pageIndex;
			}
			if (this.m_creativeInventoryWidget.PageDownButton.IsClicked)
			{
				ComponentFurnitureInventory componentFurnitureInventory2 = this.ComponentFurnitureInventory;
				int pageIndex = componentFurnitureInventory2.PageIndex + 1;
				componentFurnitureInventory2.PageIndex = pageIndex;
			}
			this.ComponentFurnitureInventory.PageIndex = ((this.m_pagesCount > 0) ? MathUtils.Clamp(this.ComponentFurnitureInventory.PageIndex, 0, this.m_pagesCount - 1) : 0);
			if (this.m_addButton.IsClicked)
			{
				List<Tuple<string, Action>> list = new List<Tuple<string, Action>>();
				list.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 6), delegate()
				{
					if (this.SubsystemFurnitureBlockBehavior.FurnitureSets.Count < 32)
					{
						this.NewFurnitureSet();
						return;
					}
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 24), LanguageControl.Get(FurnitureInventoryPanel.fName, 25), LanguageControl.Get("Usual", "ok"), null, null));
				}));
				list.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 7), delegate()
				{
					this.ImportFurnitureSet(this.SubsystemTerrain);
				}));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 8), list, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
				{
					((Tuple<string, Action>)t).Item2();
				}));
			}
			if (this.m_moreButton.IsClicked && this.ComponentFurnitureInventory.FurnitureSet != null)
			{
				List<Tuple<string, Action>> list2 = new List<Tuple<string, Action>>();
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 9), delegate()
				{
					this.RenameFurnitureSet();
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 28), delegate()
				{
					if (this.SubsystemFurnitureBlockBehavior.GetFurnitureSetDesigns(this.ComponentFurnitureInventory.FurnitureSet).Count<FurnitureDesign>() > 0)
					{
						DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get("Usual", "warning"), LanguageControl.Get(FurnitureInventoryPanel.fName, 26), LanguageControl.Get(FurnitureInventoryPanel.fName, 27), LanguageControl.Get(FurnitureInventoryPanel.fName, 28), delegate(MessageDialogButton b)
						{
							if (b == MessageDialogButton.Button1)
							{
								this.DeleteFurnitureSet();
							}
						}));
						return;
					}
					this.DeleteFurnitureSet();
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 11), delegate()
				{
					this.MoveFurnitureSet(-1);
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 12), delegate()
				{
					this.MoveFurnitureSet(1);
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 13), delegate()
				{
					this.ExportFurnitureSet();
				}));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 14), list2, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
				{
					((Tuple<string, Action>)t).Item2();
				}));
			}
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0009367A File Offset: 0x0009187A
		public override void UpdateCeases()
		{
			base.UpdateCeases();
			this.ComponentFurnitureInventory.ClearSlots();
			this.m_populateNeeded = true;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x00093694 File Offset: 0x00091894
		public void Invalidate()
		{
			this.m_populateNeeded = true;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x000936A0 File Offset: 0x000918A0
		public void Populate()
		{
			this.ComponentFurnitureInventory.FillSlots();
			try
			{
				this.m_ignoreSelectionChanged = true;
				this.m_furnitureSetList.ClearItems();
				this.m_furnitureSetList.AddItem(null);
				foreach (FurnitureSet item in this.SubsystemFurnitureBlockBehavior.FurnitureSets)
				{
					this.m_furnitureSetList.AddItem(item);
				}
			}
			finally
			{
				this.m_ignoreSelectionChanged = false;
			}
			this.m_furnitureSetList.SelectedItem = this.ComponentFurnitureInventory.FurnitureSet;
			this.AssignInventorySlots();
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0009375C File Offset: 0x0009195C
		public void AssignInventorySlots()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.ComponentFurnitureInventory.SlotsCount; i++)
			{
				int slotValue = this.ComponentFurnitureInventory.GetSlotValue(i);
				int slotCount = this.ComponentFurnitureInventory.GetSlotCount(i);
				if (slotValue != 0 && slotCount > 0 && Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					FurnitureDesign design = this.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
					if (design != null && design.FurnitureSet == this.ComponentFurnitureInventory.FurnitureSet)
					{
						list.Add(i);
					}
				}
			}
			List<InventorySlotWidget> list2 = new List<InventorySlotWidget>((from w in this.m_inventoryGrid.Children
			select w as InventorySlotWidget into w
			where w != null
			select w).Cast<InventorySlotWidget>());
			int num = this.ComponentFurnitureInventory.PageIndex * list2.Count;
			for (int j = 0; j < list2.Count; j++)
			{
				if (num < list.Count)
				{
					list2[j].AssignInventorySlot(this.ComponentFurnitureInventory, list[num]);
				}
				else
				{
					list2[j].AssignInventorySlot(null, 0);
				}
				num++;
			}
			this.m_pagesCount = (list.Count + list2.Count - 1) / list2.Count;
			this.m_assignedPage = this.ComponentFurnitureInventory.PageIndex;
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x000938E4 File Offset: 0x00091AE4
		public void NewFurnitureSet()
		{
			ComponentPlayer componentPlayer = this.ComponentFurnitureInventory.Entity.FindComponent<ComponentPlayer>(true);
			base.Input.EnterText(componentPlayer.GuiWidget, LanguageControl.Get(FurnitureInventoryPanel.fName, 15), LanguageControl.Get(FurnitureInventoryPanel.fName, 16), 20, delegate(string s)
			{
				if (s != null)
				{
					FurnitureSet furnitureSet = this.SubsystemFurnitureBlockBehavior.NewFurnitureSet(s, null);
					this.ComponentFurnitureInventory.FurnitureSet = furnitureSet;
					this.Populate();
					this.m_furnitureSetList.ScrollToItem(furnitureSet);
				}
			});
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0009393C File Offset: 0x00091B3C
		public void DeleteFurnitureSet()
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				int num = this.SubsystemFurnitureBlockBehavior.FurnitureSets.IndexOf(furnitureSet);
				this.SubsystemFurnitureBlockBehavior.DeleteFurnitureSet(furnitureSet);
				this.SubsystemFurnitureBlockBehavior.GarbageCollectDesigns();
				this.ComponentFurnitureInventory.FurnitureSet = ((num > 0) ? this.SubsystemFurnitureBlockBehavior.FurnitureSets[num - 1] : null);
				this.Invalidate();
			}
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x000939B8 File Offset: 0x00091BB8
		public void RenameFurnitureSet()
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				ComponentPlayer componentPlayer = this.ComponentFurnitureInventory.Entity.FindComponent<ComponentPlayer>(true);
				base.Input.EnterText(componentPlayer.GuiWidget, LanguageControl.Get(FurnitureInventoryPanel.fName, 17), furnitureSet.Name, 20, delegate(string s)
				{
					if (s != null)
					{
						furnitureSet.Name = s;
						this.Invalidate();
					}
				});
			}
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x00093A38 File Offset: 0x00091C38
		public void MoveFurnitureSet(int move)
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				this.SubsystemFurnitureBlockBehavior.MoveFurnitureSet(furnitureSet, move);
				this.Invalidate();
			}
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x00093A6C File Offset: 0x00091C6C
		public void ImportFurnitureSet(SubsystemTerrain subsystemTerrain)
		{
			FurniturePacksManager.UpdateFurniturePacksList();
			if (FurniturePacksManager.FurniturePackNames.Count<string>() == 0)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 18), LanguageControl.Get(FurnitureInventoryPanel.fName, 19), LanguageControl.Get("Usual", "ok"), null, null));
				return;
			}
			DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 20), FurniturePacksManager.FurniturePackNames, 64f, (object s) => FurniturePacksManager.GetDisplayName((string)s), delegate(object s)
			{
				try
				{
					int num = 0;
					int num2 = 0;
					string text = (string)s;
					List<List<FurnitureDesign>> list = FurnitureDesign.ListChains(FurniturePacksManager.LoadFurniturePack(subsystemTerrain, text));
					List<FurnitureDesign> list2 = new List<FurnitureDesign>();
					this.SubsystemFurnitureBlockBehavior.GarbageCollectDesigns();
					foreach (List<FurnitureDesign> list3 in list)
					{
						FurnitureDesign furnitureDesign = this.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list3[0], false);
						if (furnitureDesign == list3[0])
						{
							list2.Add(furnitureDesign);
						}
						else if (furnitureDesign == null)
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
					if (list2.Count > 0)
					{
						FurnitureSet furnitureSet = this.SubsystemFurnitureBlockBehavior.NewFurnitureSet(FurniturePacksManager.GetDisplayName(text), text);
						foreach (FurnitureDesign design in list2)
						{
							this.SubsystemFurnitureBlockBehavior.AddToFurnitureSet(design, furnitureSet);
						}
						this.ComponentFurnitureInventory.FurnitureSet = furnitureSet;
					}
					this.Invalidate();
					string text2 = string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 1), list2.Count);
					if (num > 0)
					{
						text2 += string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 2), num);
					}
					if (num2 > 0)
					{
						text2 += string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 3), num2, 65535);
					}
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 4), text2.Trim(), LanguageControl.Get("Usual", "ok"), null, null));
				}
				catch (Exception ex)
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 5), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
				}
			}));
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x00093B40 File Offset: 0x00091D40
		public void ExportFurnitureSet()
		{
			try
			{
				FurnitureDesign[] designs = this.SubsystemFurnitureBlockBehavior.GetFurnitureSetDesigns(this.ComponentFurnitureInventory.FurnitureSet).ToArray<FurnitureDesign>();
				string displayName = FurniturePacksManager.GetDisplayName(FurniturePacksManager.CreateFurniturePack(this.ComponentFurnitureInventory.FurnitureSet.Name, designs));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 21), string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 22), displayName), LanguageControl.Get("Usual", "ok"), null, null));
			}
			catch (Exception ex)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 23), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
			}
		}

		// Token: 0x04000CFC RID: 3324
		public CreativeInventoryWidget m_creativeInventoryWidget;

		// Token: 0x04000CFD RID: 3325
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000CFE RID: 3326
		public ListPanelWidget m_furnitureSetList;

		// Token: 0x04000CFF RID: 3327
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04000D00 RID: 3328
		public ButtonWidget m_addButton;

		// Token: 0x04000D01 RID: 3329
		public ButtonWidget m_moreButton;

		// Token: 0x04000D02 RID: 3330
		public int m_pagesCount;

		// Token: 0x04000D03 RID: 3331
		public int m_assignedPage;

		// Token: 0x04000D04 RID: 3332
		public bool m_ignoreSelectionChanged;

		// Token: 0x04000D05 RID: 3333
		public bool m_populateNeeded;

		// Token: 0x04000D06 RID: 3334
		public static string fName = "FurnitureInventoryPanel";
	}
}
