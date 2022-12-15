using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000393 RID: 915
	public class MusketWidget : CanvasWidget
	{
		// Token: 0x06001A94 RID: 6804 RVA: 0x000D12F0 File Offset: 0x000CF4F0
		public MusketWidget(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			XElement node = ContentManager.Get<XElement>("Widgets/MusketWidget");
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
			this.m_inventorySlotWidget.CustomViewMatrix = new Matrix?(Matrix.CreateLookAt(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 0f), -Vector3.UnitZ));
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x000D1474 File Offset: 0x000CF674
		public override void Update()
		{
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			if (Terrain.ExtractContents(slotValue) != 212 || slotCount <= 0)
			{
				base.ParentWidget.Children.Remove(this);
				return;
			}
			switch (MusketBlock.GetLoadState(Terrain.ExtractData(slotValue)))
			{
			case MusketBlock.LoadState.Empty:
				this.m_instructionsLabel.Text = LanguageControl.Get(MusketWidget.fName, 0);
				return;
			case MusketBlock.LoadState.Gunpowder:
				this.m_instructionsLabel.Text = LanguageControl.Get(MusketWidget.fName, 1);
				return;
			case MusketBlock.LoadState.Wad:
				this.m_instructionsLabel.Text = LanguageControl.Get(MusketWidget.fName, 2);
				return;
			case MusketBlock.LoadState.Loaded:
				this.m_instructionsLabel.Text = LanguageControl.Get(MusketWidget.fName, 3);
				return;
			default:
				this.m_instructionsLabel.Text = string.Empty;
				return;
			}
		}

		// Token: 0x04001287 RID: 4743
		public IInventory m_inventory;

		// Token: 0x04001288 RID: 4744
		public int m_slotIndex;

		// Token: 0x04001289 RID: 4745
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x0400128A RID: 4746
		public InventorySlotWidget m_inventorySlotWidget;

		// Token: 0x0400128B RID: 4747
		public LabelWidget m_instructionsLabel;

		// Token: 0x0400128C RID: 4748
		public static string fName = "MusketWidget";
	}
}
