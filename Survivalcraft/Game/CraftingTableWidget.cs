using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200037A RID: 890
	public class CraftingTableWidget : CanvasWidget
	{
		// Token: 0x06001975 RID: 6517 RVA: 0x000C7558 File Offset: 0x000C5758
		public CraftingTableWidget(IInventory inventory, ComponentCraftingTable componentCraftingTable)
		{
			this.m_componentCraftingTable = componentCraftingTable;
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingTableWidget");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_craftingGrid = this.Children.Find<GridPanelWidget>("CraftingGrid", true);
			this.m_craftingResultSlot = this.Children.Find<InventorySlotWidget>("CraftingResultSlot", true);
			this.m_craftingRemainsSlot = this.Children.Find<InventorySlotWidget>("CraftingRemainsSlot", true);
			int num = 10;
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			num = 0;
			for (int k = 0; k < this.m_craftingGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_craftingGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(this.m_componentCraftingTable, num++);
					this.m_craftingGrid.Children.Add(inventorySlotWidget2);
					this.m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_craftingResultSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.ResultSlotIndex);
			this.m_craftingRemainsSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.RemainsSlotIndex);
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x000C76F9 File Offset: 0x000C58F9
		public override void Update()
		{
			if (!this.m_componentCraftingTable.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x040011CE RID: 4558
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x040011CF RID: 4559
		public GridPanelWidget m_craftingGrid;

		// Token: 0x040011D0 RID: 4560
		public InventorySlotWidget m_craftingResultSlot;

		// Token: 0x040011D1 RID: 4561
		public InventorySlotWidget m_craftingRemainsSlot;

		// Token: 0x040011D2 RID: 4562
		public ComponentCraftingTable m_componentCraftingTable;
	}
}
