using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000372 RID: 882
	public class ChestWidget : CanvasWidget
	{
		// Token: 0x0600193D RID: 6461 RVA: 0x000C6428 File Offset: 0x000C4628
		public ChestWidget(IInventory inventory, ComponentChest componentChest)
		{
			this.m_componentChest = componentChest;
			XElement node = ContentManager.Get<XElement>("Widgets/ChestWidget");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_chestGrid = this.Children.Find<GridPanelWidget>("ChestGrid", true);
			int num = 0;
			for (int i = 0; i < this.m_chestGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_chestGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentChest, num++);
					this.m_chestGrid.Children.Add(inventorySlotWidget);
					this.m_chestGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			num = 10;
			for (int k = 0; k < this.m_inventoryGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_inventoryGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget2);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x000C655E File Offset: 0x000C475E
		public override void Update()
		{
			if (!this.m_componentChest.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x040011A6 RID: 4518
		public ComponentChest m_componentChest;

		// Token: 0x040011A7 RID: 4519
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x040011A8 RID: 4520
		public GridPanelWidget m_chestGrid;
	}
}
