using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000245 RID: 581
	public class CreativeInventoryPanel : CanvasWidget
	{
		// Token: 0x060011C0 RID: 4544 RVA: 0x000893AC File Offset: 0x000875AC
		public CreativeInventoryPanel(CreativeInventoryWidget creativeInventoryWidget)
		{
			this.m_creativeInventoryWidget = creativeInventoryWidget;
			this.m_componentCreativeInventory = creativeInventoryWidget.Entity.FindComponent<ComponentCreativeInventory>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/CreativeInventoryPanel");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget
					{
						HideEditOverlay = true,
						HideInteractiveOverlay = true,
						HideFoodOverlay = true
					};
					this.m_inventoryGrid.Children.Add(widget);
					this.m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x00089488 File Offset: 0x00087688
		public override void Update()
		{
			if (this.m_assignedCategoryIndex >= 0)
			{
				if (base.Input.Scroll != null)
				{
					Widget widget = base.HitTestGlobal(base.Input.Scroll.Value.XY, null);
					if (widget != null && widget.IsChildWidgetOf(this.m_inventoryGrid))
					{
						this.m_componentCreativeInventory.PageIndex -= (int)base.Input.Scroll.Value.Z;
					}
				}
				if (this.m_creativeInventoryWidget.PageDownButton.IsClicked)
				{
					ComponentCreativeInventory componentCreativeInventory = this.m_componentCreativeInventory;
					int pageIndex = componentCreativeInventory.PageIndex + 1;
					componentCreativeInventory.PageIndex = pageIndex;
				}
				if (this.m_creativeInventoryWidget.PageUpButton.IsClicked)
				{
					ComponentCreativeInventory componentCreativeInventory2 = this.m_componentCreativeInventory;
					int pageIndex = componentCreativeInventory2.PageIndex - 1;
					componentCreativeInventory2.PageIndex = pageIndex;
				}
				this.m_componentCreativeInventory.PageIndex = ((this.m_pagesCount > 0) ? MathUtils.Clamp(this.m_componentCreativeInventory.PageIndex, 0, this.m_pagesCount - 1) : 0);
			}
			if (this.m_componentCreativeInventory.CategoryIndex != this.m_assignedCategoryIndex)
			{
				if (this.m_creativeInventoryWidget.GetCategoryName(this.m_componentCreativeInventory.CategoryIndex) == LanguageControl.Get("CreativeInventoryWidget", 2))
				{
					this.m_slotIndices = new List<int>(Enumerable.Range(10, this.m_componentCreativeInventory.OpenSlotsCount - 10));
				}
				else
				{
					this.m_slotIndices.Clear();
					for (int i = this.m_componentCreativeInventory.OpenSlotsCount; i < this.m_componentCreativeInventory.SlotsCount; i++)
					{
						int slotValue = this.m_componentCreativeInventory.GetSlotValue(i);
						int num = Terrain.ExtractContents(slotValue);
						if (BlocksManager.Blocks[num].GetCategory(slotValue) == this.m_creativeInventoryWidget.GetCategoryName(this.m_componentCreativeInventory.CategoryIndex))
						{
							this.m_slotIndices.Add(i);
						}
					}
				}
				int num2 = this.m_inventoryGrid.ColumnsCount * this.m_inventoryGrid.RowsCount;
				this.m_pagesCount = (this.m_slotIndices.Count + num2 - 1) / num2;
				this.m_assignedCategoryIndex = this.m_componentCreativeInventory.CategoryIndex;
				this.m_assignedPageIndex = -1;
				this.m_componentCreativeInventory.PageIndex = 0;
			}
			if (this.m_componentCreativeInventory.PageIndex != this.m_assignedPageIndex)
			{
				int num3 = this.m_inventoryGrid.ColumnsCount * this.m_inventoryGrid.RowsCount;
				int num4 = this.m_componentCreativeInventory.PageIndex * num3;
				foreach (Widget widget2 in this.m_inventoryGrid.Children)
				{
					InventorySlotWidget inventorySlotWidget = widget2 as InventorySlotWidget;
					if (inventorySlotWidget != null)
					{
						if (num4 < this.m_slotIndices.Count)
						{
							inventorySlotWidget.AssignInventorySlot(this.m_componentCreativeInventory, this.m_slotIndices[num4++]);
						}
						else
						{
							inventorySlotWidget.AssignInventorySlot(null, 0);
						}
					}
				}
				this.m_assignedPageIndex = this.m_componentCreativeInventory.PageIndex;
			}
			this.m_creativeInventoryWidget.PageLabel.Text = ((this.m_pagesCount > 0) ? string.Format("{0}/{1}", this.m_componentCreativeInventory.PageIndex + 1, this.m_pagesCount) : string.Empty);
			this.m_creativeInventoryWidget.PageDownButton.IsEnabled = (this.m_componentCreativeInventory.PageIndex < this.m_pagesCount - 1);
			this.m_creativeInventoryWidget.PageUpButton.IsEnabled = (this.m_componentCreativeInventory.PageIndex > 0);
		}

		// Token: 0x04000BE9 RID: 3049
		public CreativeInventoryWidget m_creativeInventoryWidget;

		// Token: 0x04000BEA RID: 3050
		public ComponentCreativeInventory m_componentCreativeInventory;

		// Token: 0x04000BEB RID: 3051
		public List<int> m_slotIndices = new List<int>();

		// Token: 0x04000BEC RID: 3052
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04000BED RID: 3053
		public int m_pagesCount;

		// Token: 0x04000BEE RID: 3054
		public int m_assignedCategoryIndex = -1;

		// Token: 0x04000BEF RID: 3055
		public int m_assignedPageIndex = -1;
	}
}
