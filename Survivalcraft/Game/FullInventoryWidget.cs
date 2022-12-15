using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000382 RID: 898
	public class FullInventoryWidget : CanvasWidget
	{
		// Token: 0x060019AD RID: 6573 RVA: 0x000C96F8 File Offset: 0x000C78F8
		public FullInventoryWidget(IInventory inventory, ComponentCraftingTable componentCraftingTable)
		{
			XElement node = ContentManager.Get<XElement>("Widgets/FullInventoryWidget");
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
					inventorySlotWidget2.AssignInventorySlot(componentCraftingTable, num++);
					this.m_craftingGrid.Children.Add(inventorySlotWidget2);
					this.m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_craftingResultSlot.AssignInventorySlot(componentCraftingTable, componentCraftingTable.ResultSlotIndex);
			this.m_craftingRemainsSlot.AssignInventorySlot(componentCraftingTable, componentCraftingTable.RemainsSlotIndex);
		}

		// Token: 0x040011FF RID: 4607
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001200 RID: 4608
		public GridPanelWidget m_craftingGrid;

		// Token: 0x04001201 RID: 4609
		public InventorySlotWidget m_craftingResultSlot;

		// Token: 0x04001202 RID: 4610
		public InventorySlotWidget m_craftingRemainsSlot;
	}
}
