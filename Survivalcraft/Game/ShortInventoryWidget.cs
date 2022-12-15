using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000399 RID: 921
	public class ShortInventoryWidget : CanvasWidget
	{
		// Token: 0x06001AE1 RID: 6881 RVA: 0x000D320C File Offset: 0x000D140C
		public ShortInventoryWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/ShortInventoryWidget");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x000D3249 File Offset: 0x000D1449
		public void AssignComponents(IInventory inventory)
		{
			if (inventory != this.m_inventory)
			{
				this.m_inventory = inventory;
				this.m_inventoryGrid.Children.Clear();
			}
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x000D326C File Offset: 0x000D146C
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			int max = (this.m_inventory is ComponentCreativeInventory) ? 10 : 7;
			this.m_inventory.VisibleSlotsCount = MathUtils.Clamp((int)((parentAvailableSize.X - 320f - 25f) / 72f), 7, max);
			if (this.m_inventory.VisibleSlotsCount != this.m_inventoryGrid.Children.Count)
			{
				this.m_inventoryGrid.Children.Clear();
				this.m_inventoryGrid.RowsCount = 1;
				this.m_inventoryGrid.ColumnsCount = this.m_inventory.VisibleSlotsCount;
				for (int i = 0; i < this.m_inventoryGrid.ColumnsCount; i++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(this.m_inventory, i);
					inventorySlotWidget.BevelColor = new Color(181, 172, 154) * 0.6f;
					inventorySlotWidget.CenterColor = new Color(181, 172, 154) * 0.33f;
					this.m_inventoryGrid.Children.Add(inventorySlotWidget);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(i, 0));
				}
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040012B6 RID: 4790
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x040012B7 RID: 4791
		public IInventory m_inventory;
	}
}
