using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000376 RID: 886
	public class ClothingWidget : CanvasWidget
	{
		// Token: 0x0600195F RID: 6495 RVA: 0x000C69C4 File Offset: 0x000C4BC4
		public ClothingWidget(ComponentPlayer componentPlayer)
		{
			this.m_componentPlayer = componentPlayer;
			XElement node = ContentManager.Get<XElement>("Widgets/ClothingWidget");
			base.LoadContents(this, node);
			this.m_clothingStack = this.Children.Find<StackPanelWidget>("ClothingStack", true);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_vitalStatsButton = this.Children.Find<ButtonWidget>("VitalStatsButton", true);
			this.m_sleepButton = this.Children.Find<ButtonWidget>("SleepButton", true);
			this.m_innerClothingModelWidget = this.Children.Find<PlayerModelWidget>("InnerClothingModel", true);
			this.m_outerClothingModelWidget = this.Children.Find<PlayerModelWidget>("OuterClothingModel", true);
			for (int i = 0; i < 4; i++)
			{
				InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
				float y = float.PositiveInfinity;
				if (i == 0)
				{
					y = 68f;
				}
				if (i == 3)
				{
					y = 54f;
				}
				inventorySlotWidget.Size = new Vector2(float.PositiveInfinity, y);
				inventorySlotWidget.BevelColor = Color.Transparent;
				inventorySlotWidget.CenterColor = Color.Transparent;
				inventorySlotWidget.AssignInventorySlot(this.m_componentPlayer.ComponentClothing, i);
				inventorySlotWidget.HideEditOverlay = true;
				inventorySlotWidget.HideInteractiveOverlay = true;
				inventorySlotWidget.HideFoodOverlay = true;
				inventorySlotWidget.HideHighlightRectangle = true;
				inventorySlotWidget.HideBlockIcon = true;
				inventorySlotWidget.HideHealthBar = (this.m_componentPlayer.Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings.GameMode == GameMode.Creative);
				this.m_clothingStack.Children.Add(inventorySlotWidget);
			}
			int num = 10;
			for (int j = 0; j < this.m_inventoryGrid.RowsCount; j++)
			{
				for (int k = 0; k < this.m_inventoryGrid.ColumnsCount; k++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentPlayer.ComponentMiner.Inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget2);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(k, j));
				}
			}
			this.m_innerClothingModelWidget.PlayerClass = componentPlayer.PlayerData.PlayerClass;
			this.m_innerClothingModelWidget.CharacterSkinTexture = this.m_componentPlayer.ComponentClothing.InnerClothedTexture;
			this.m_outerClothingModelWidget.PlayerClass = componentPlayer.PlayerData.PlayerClass;
			this.m_outerClothingModelWidget.OuterClothingTexture = this.m_componentPlayer.ComponentClothing.OuterClothedTexture;
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x000C6C20 File Offset: 0x000C4E20
		public override void Update()
		{
			if (this.m_vitalStatsButton.IsClicked && this.m_componentPlayer != null)
			{
				this.m_componentPlayer.ComponentGui.ModalPanelWidget = new VitalStatsWidget(this.m_componentPlayer);
			}
			if (this.m_sleepButton.IsClicked && this.m_componentPlayer != null)
			{
				string text;
				if (!this.m_componentPlayer.ComponentSleep.CanSleep(out text))
				{
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(text, Color.White, false, false);
					return;
				}
				this.m_componentPlayer.ComponentSleep.Sleep(true);
			}
		}

		// Token: 0x040011BA RID: 4538
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x040011BB RID: 4539
		public StackPanelWidget m_clothingStack;

		// Token: 0x040011BC RID: 4540
		public ButtonWidget m_vitalStatsButton;

		// Token: 0x040011BD RID: 4541
		public ButtonWidget m_sleepButton;

		// Token: 0x040011BE RID: 4542
		public PlayerModelWidget m_innerClothingModelWidget;

		// Token: 0x040011BF RID: 4543
		public PlayerModelWidget m_outerClothingModelWidget;

		// Token: 0x040011C0 RID: 4544
		public ComponentPlayer m_componentPlayer;
	}
}
