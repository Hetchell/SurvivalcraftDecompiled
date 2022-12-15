using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000383 RID: 899
	public class FurnaceWidget : CanvasWidget
	{
		// Token: 0x060019AE RID: 6574 RVA: 0x000C987C File Offset: 0x000C7A7C
		public FurnaceWidget(IInventory inventory, ComponentFurnace componentFurnace)
		{
			this.m_componentFurnace = componentFurnace;
			XElement node = ContentManager.Get<XElement>("Widgets/FurnaceWidget");
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_furnaceGrid = this.Children.Find<GridPanelWidget>("FurnaceGrid", true);
			this.m_fire = this.Children.Find<FireWidget>("Fire", true);
			this.m_progress = this.Children.Find<ValueBarWidget>("Progress", true);
			this.m_resultSlot = this.Children.Find<InventorySlotWidget>("ResultSlot", true);
			this.m_remainsSlot = this.Children.Find<InventorySlotWidget>("RemainsSlot", true);
			this.m_fuelSlot = this.Children.Find<InventorySlotWidget>("FuelSlot", true);
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
			for (int k = 0; k < this.m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_furnaceGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num++);
					this.m_furnaceGrid.Children.Add(inventorySlotWidget2);
					this.m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
			this.m_resultSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
			this.m_remainsSlot.AssignInventorySlot(componentFurnace, componentFurnace.RemainsSlotIndex);
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x000C9A5C File Offset: 0x000C7C5C
		public override void Update()
		{
			this.m_fire.ParticlesPerSecond = (float)((this.m_componentFurnace.HeatLevel > 0f) ? 24 : 0);
			this.m_progress.Value = this.m_componentFurnace.SmeltingProgress;
			if (!this.m_componentFurnace.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x04001203 RID: 4611
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001204 RID: 4612
		public GridPanelWidget m_furnaceGrid;

		// Token: 0x04001205 RID: 4613
		public InventorySlotWidget m_fuelSlot;

		// Token: 0x04001206 RID: 4614
		public InventorySlotWidget m_resultSlot;

		// Token: 0x04001207 RID: 4615
		public InventorySlotWidget m_remainsSlot;

		// Token: 0x04001208 RID: 4616
		public FireWidget m_fire;

		// Token: 0x04001209 RID: 4617
		public ValueBarWidget m_progress;

		// Token: 0x0400120A RID: 4618
		public ComponentFurnace m_componentFurnace;
	}
}
