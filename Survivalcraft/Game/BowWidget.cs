using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200036D RID: 877
	public class BowWidget : CanvasWidget
	{
		// Token: 0x0600190A RID: 6410 RVA: 0x000C5A50 File Offset: 0x000C3C50
		public BowWidget(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			XElement node = ContentManager.Get<XElement>("Widgets/BowWidget");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_inventorySlotWidget = this.Children.Find<InventorySlotWidget>("InventorySlot", true);
			this.m_instructionsLabel = this.Children.Find<LabelWidget>("InstructionsLabel", true);
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget();
					this.m_inventoryGrid.Children.Add(widget);
					this.m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
			int num = 10;
			foreach (Widget widget2 in this.m_inventoryGrid.Children)
			{
				InventorySlotWidget inventorySlotWidget = widget2 as InventorySlotWidget;
				if (inventorySlotWidget != null)
				{
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
				}
			}
			this.m_inventorySlotWidget.AssignInventorySlot(inventory, slotIndex);
			this.m_inventorySlotWidget.CustomViewMatrix = new Matrix?(Matrix.CreateLookAt(new Vector3(-1f, 0.2f, 0.6f), new Vector3(0f, 0.2f, 0f), Vector3.UnitY));
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x000C5BD0 File Offset: 0x000C3DD0
		public override void Update()
		{
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			int num = Terrain.ExtractContents(slotValue);
			if (BowBlock.GetArrowType(Terrain.ExtractData(slotValue)) == null)
			{
				this.m_instructionsLabel.Text = LanguageControl.Get(BowWidget.fName, 0);
			}
			else
			{
				this.m_instructionsLabel.Text = LanguageControl.Get(BowWidget.fName, 1);
			}
			if (num != 191 || slotCount == 0)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x0400118F RID: 4495
		public IInventory m_inventory;

		// Token: 0x04001190 RID: 4496
		public int m_slotIndex;

		// Token: 0x04001191 RID: 4497
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001192 RID: 4498
		public InventorySlotWidget m_inventorySlotWidget;

		// Token: 0x04001193 RID: 4499
		public LabelWidget m_instructionsLabel;

		// Token: 0x04001194 RID: 4500
		public static string fName = "BowWidget";
	}
}
