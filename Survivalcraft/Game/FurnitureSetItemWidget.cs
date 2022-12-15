using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000385 RID: 901
	public class FurnitureSetItemWidget : CanvasWidget, IDragTargetWidget
	{
		// Token: 0x060019BD RID: 6589 RVA: 0x000CA5CC File Offset: 0x000C87CC
		public FurnitureSetItemWidget(FurnitureInventoryPanel furnitureInventoryWidget, FurnitureSet furnitureSet)
		{
			this.m_furnitureInventoryPanel = furnitureInventoryWidget;
			this.m_furnitureSet = furnitureSet;
			XElement node = ContentManager.Get<XElement>("Widgets/FurnitureSetItemWidget");
			base.LoadContents(this, node);
			LabelWidget labelWidget = this.Children.Find<LabelWidget>("FurnitureSetItem.Name", true);
			LabelWidget labelWidget2 = this.Children.Find<LabelWidget>("FurnitureSetItem.DesignsCount", true);
			labelWidget.Text = ((furnitureSet == null) ? "Uncategorized" : furnitureSet.Name);
			labelWidget2.Text = string.Format("{0} design(s)", this.CountFurnitureDesigns());
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x000CA654 File Offset: 0x000C8854
		public void DragDrop(Widget dragWidget, object data)
		{
			FurnitureDesign furnitureDesign = this.GetFurnitureDesign(data);
			if (furnitureDesign != null)
			{
				this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.AddToFurnitureSet(furnitureDesign, this.m_furnitureSet);
				this.m_furnitureInventoryPanel.Invalidate();
			}
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x000CA68E File Offset: 0x000C888E
		public void DragOver(Widget dragWidget, object data)
		{
			this.m_highlighted = (this.GetFurnitureDesign(data) != null);
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x000CA6A0 File Offset: 0x000C88A0
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = this.m_highlighted;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x000CA6B8 File Offset: 0x000C88B8
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_highlighted)
			{
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(100, DepthStencilState.None, null, null);
				int count = flatBatch2D.TriangleVertices.Count;
				flatBatch2D.QueueQuad(Vector2.Zero, base.ActualSize, 0f, new Color(128, 128, 128, 128));
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				this.m_highlighted = false;
			}
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x000CA730 File Offset: 0x000C8930
		public FurnitureDesign GetFurnitureDesign(object dragData)
		{
			InventoryDragData inventoryDragData = dragData as InventoryDragData;
			if (inventoryDragData != null)
			{
				int slotValue = inventoryDragData.Inventory.GetSlotValue(inventoryDragData.SlotIndex);
				if (Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					return this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				}
			}
			return null;
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x000CA788 File Offset: 0x000C8988
		public int CountFurnitureDesigns()
		{
			int num = 0;
			for (int i = 0; i < this.m_furnitureInventoryPanel.ComponentFurnitureInventory.SlotsCount; i++)
			{
				int slotValue = this.m_furnitureInventoryPanel.ComponentFurnitureInventory.GetSlotValue(i);
				if (Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					FurnitureDesign design = this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
					if (design != null && design.FurnitureSet == this.m_furnitureSet)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x04001215 RID: 4629
		public FurnitureInventoryPanel m_furnitureInventoryPanel;

		// Token: 0x04001216 RID: 4630
		public FurnitureSet m_furnitureSet;

		// Token: 0x04001217 RID: 4631
		public bool m_highlighted;
	}
}
