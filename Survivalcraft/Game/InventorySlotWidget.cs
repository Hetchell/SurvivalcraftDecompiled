using System;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200038A RID: 906
	public class InventorySlotWidget : CanvasWidget, IDragTargetWidget
	{
		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x060019EC RID: 6636 RVA: 0x000CCE54 File Offset: 0x000CB054
		// (set) Token: 0x060019ED RID: 6637 RVA: 0x000CCE5C File Offset: 0x000CB05C
		public bool HideBlockIcon { get; set; }

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x060019EE RID: 6638 RVA: 0x000CCE65 File Offset: 0x000CB065
		// (set) Token: 0x060019EF RID: 6639 RVA: 0x000CCE6D File Offset: 0x000CB06D
		public bool HideEditOverlay { get; set; }

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x060019F0 RID: 6640 RVA: 0x000CCE76 File Offset: 0x000CB076
		// (set) Token: 0x060019F1 RID: 6641 RVA: 0x000CCE7E File Offset: 0x000CB07E
		public bool HideInteractiveOverlay { get; set; }

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x060019F2 RID: 6642 RVA: 0x000CCE87 File Offset: 0x000CB087
		// (set) Token: 0x060019F3 RID: 6643 RVA: 0x000CCE8F File Offset: 0x000CB08F
		public bool HideFoodOverlay { get; set; }

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x060019F4 RID: 6644 RVA: 0x000CCE98 File Offset: 0x000CB098
		// (set) Token: 0x060019F5 RID: 6645 RVA: 0x000CCEA0 File Offset: 0x000CB0A0
		public bool HideHighlightRectangle { get; set; }

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x000CCEA9 File Offset: 0x000CB0A9
		// (set) Token: 0x060019F7 RID: 6647 RVA: 0x000CCEB1 File Offset: 0x000CB0B1
		public bool HideHealthBar { get; set; }

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x060019F8 RID: 6648 RVA: 0x000CCEBA File Offset: 0x000CB0BA
		// (set) Token: 0x060019F9 RID: 6649 RVA: 0x000CCEC2 File Offset: 0x000CB0C2
		public bool ProcessingOnly { get; set; }

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060019FA RID: 6650 RVA: 0x000CCECB File Offset: 0x000CB0CB
		// (set) Token: 0x060019FB RID: 6651 RVA: 0x000CCED8 File Offset: 0x000CB0D8
		public Color CenterColor
		{
			get
			{
				return this.m_rectangleWidget.CenterColor;
			}
			set
			{
				this.m_rectangleWidget.CenterColor = value;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060019FC RID: 6652 RVA: 0x000CCEE6 File Offset: 0x000CB0E6
		// (set) Token: 0x060019FD RID: 6653 RVA: 0x000CCEF3 File Offset: 0x000CB0F3
		public Color BevelColor
		{
			get
			{
				return this.m_rectangleWidget.BevelColor;
			}
			set
			{
				this.m_rectangleWidget.BevelColor = value;
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060019FE RID: 6654 RVA: 0x000CCF01 File Offset: 0x000CB101
		// (set) Token: 0x060019FF RID: 6655 RVA: 0x000CCF0E File Offset: 0x000CB10E
		public Matrix? CustomViewMatrix
		{
			get
			{
				return this.m_blockIconWidget.CustomViewMatrix;
			}
			set
			{
				this.m_blockIconWidget.CustomViewMatrix = value;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001A00 RID: 6656 RVA: 0x000CCF1C File Offset: 0x000CB11C
		public GameWidget GameWidget
		{
			get
			{
				if (this.m_gameWidget == null)
				{
					for (ContainerWidget parentWidget = base.ParentWidget; parentWidget != null; parentWidget = parentWidget.ParentWidget)
					{
						GameWidget gameWidget = parentWidget as GameWidget;
						if (gameWidget != null)
						{
							this.m_gameWidget = gameWidget;
							break;
						}
					}
				}
				return this.m_gameWidget;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001A01 RID: 6657 RVA: 0x000CCF5D File Offset: 0x000CB15D
		public DragHostWidget DragHostWidget
		{
			get
			{
				if (this.m_dragHostWidget == null)
				{
					this.m_dragHostWidget = ((this.GameWidget != null) ? this.GameWidget.Children.Find<DragHostWidget>(false) : null);
				}
				return this.m_dragHostWidget;
			}
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x000CCF90 File Offset: 0x000CB190
		public InventorySlotWidget()
		{
			base.Size = new Vector2(72f, 72f);
			WidgetsList children = this.Children;
			Widget[] array = new Widget[7];
			BevelledRectangleWidget bevelledRectangleWidget = new BevelledRectangleWidget
			{
				BevelSize = -2f,
				DirectionalLight = 0.15f,
				CenterColor = Color.Transparent
			};
			BevelledRectangleWidget bevelledRectangleWidget2 = bevelledRectangleWidget;
			this.m_rectangleWidget = bevelledRectangleWidget;
			array[0] = bevelledRectangleWidget2;
			RectangleWidget rectangleWidget = new RectangleWidget
			{
				FillColor = Color.Transparent,
				OutlineColor = Color.Transparent
			};
			RectangleWidget rectangleWidget2 = rectangleWidget;
			this.m_highlightWidget = rectangleWidget;
			array[1] = rectangleWidget2;
			BlockIconWidget blockIconWidget = new BlockIconWidget
			{
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(2f, 2f)
			};
			BlockIconWidget blockIconWidget2 = blockIconWidget;
			this.m_blockIconWidget = blockIconWidget;
			array[2] = blockIconWidget2;
			LabelWidget labelWidget = new LabelWidget
			{
				Font = ContentManager.Get<BitmapFont>("Fonts/Pericles"),
				FontScale = 1f,
				HorizontalAlignment = WidgetAlignment.Far,
				VerticalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(6f, 2f)
			};
			LabelWidget labelWidget2 = labelWidget;
			this.m_countWidget = labelWidget;
			array[3] = labelWidget2;
			ValueBarWidget valueBarWidget = new ValueBarWidget
			{
				LayoutDirection = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Far,
				BarsCount = 3,
				FlipDirection = true,
				LitBarColor = new Color(32, 128, 0),
				UnlitBarColor = new Color(24, 24, 24, 64),
				BarSize = new Vector2(12f, 12f),
				BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/ProgressBar"),
				Margin = new Vector2(4f, 4f)
			};
			ValueBarWidget valueBarWidget2 = valueBarWidget;
			this.m_healthBarWidget = valueBarWidget;
			array[4] = valueBarWidget2;
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(3f, 3f)
			};
			WidgetsList children2 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget3 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/InteractiveItemOverlay"),
				Size = new Vector2(13f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget3;
			this.m_interactiveOverlayWidget = rectangleWidget3;
			children2.Add(rectangleWidget2);
			WidgetsList children3 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget4 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/EditItemOverlay"),
				Size = new Vector2(12f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget4;
			this.m_editOverlayWidget = rectangleWidget4;
			children3.Add(rectangleWidget2);
			WidgetsList children4 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget5 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/FoodItemOverlay"),
				Size = new Vector2(11f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget5;
			this.m_foodOverlayWidget = rectangleWidget5;
			children4.Add(rectangleWidget2);
			array[5] = stackPanelWidget;
			LabelWidget labelWidget3 = new LabelWidget
			{
				Text = "Split",
				Font = ContentManager.Get<BitmapFont>("Fonts/Pericles"),
				Color = new Color(255, 64, 0),
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(2f, 0f)
			};
			labelWidget2 = labelWidget3;
			this.m_splitLabelWidget = labelWidget3;
			array[6] = labelWidget2;
			children.Add(array);
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x000CD338 File Offset: 0x000CB538
		public void AssignInventorySlot(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			this.m_subsystemTerrain = ((inventory != null) ? inventory.Project.FindSubsystem<SubsystemTerrain>(true) : null);
			if (inventory is Component)
			{
				this.m_componentPlayer = ((Component)inventory).Entity.FindComponent<ComponentPlayer>();
			}
			else
			{
				this.m_componentPlayer = null;
			}
			this.m_blockIconWidget.DrawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x000CD3A8 File Offset: 0x000CB5A8
		public override void Update()
		{
			if (this.m_inventory == null || this.DragHostWidget == null)
			{
				return;
			}
			WidgetInput input = base.Input;
			ComponentPlayer viewPlayer = this.GetViewPlayer();
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int num = Terrain.ExtractContents(slotValue);
			Block block = BlocksManager.Blocks[num];
			if (this.m_componentPlayer != null)
			{
				this.m_blockIconWidget.DrawBlockEnvironmentData.InWorldMatrix = this.m_componentPlayer.ComponentBody.Matrix;
			}
			if (this.m_focus && input.Press == null)
			{
				this.m_focus = false;
			}
			else if (input.Tap != null && base.HitTestGlobal(input.Tap.Value, null) == this)
			{
				this.m_focus = true;
			}
			if (input.SpecialClick != null && base.HitTestGlobal(input.SpecialClick.Value.Start, null) == this && base.HitTestGlobal(input.SpecialClick.Value.End, null) == this)
			{
				IInventory inventory = null;
				foreach (InventorySlotWidget inventorySlotWidget in ((ContainerWidget)base.RootWidget).AllChildren.OfType<InventorySlotWidget>())
				{
					if (inventorySlotWidget.m_inventory != null && inventorySlotWidget.m_inventory != this.m_inventory && inventorySlotWidget.Input == base.Input && inventorySlotWidget.IsEnabledGlobal && inventorySlotWidget.IsVisibleGlobal)
					{
						inventory = inventorySlotWidget.m_inventory;
						break;
					}
				}
				if (inventory != null)
				{
					int num2 = ComponentInventoryBase.FindAcquireSlotForItem(inventory, slotValue);
					if (num2 >= 0)
					{
						this.HandleMoveItem(this.m_inventory, this.m_slotIndex, inventory, num2, this.m_inventory.GetSlotCount(this.m_slotIndex));
					}
				}
			}
			if (input.Click != null && base.HitTestGlobal(input.Click.Value.Start, null) == this && base.HitTestGlobal(input.Click.Value.End, null) == this)
			{
				bool flag = false;
				if (viewPlayer != null)
				{
					if (viewPlayer.ComponentInput.SplitSourceInventory == this.m_inventory && viewPlayer.ComponentInput.SplitSourceSlotIndex == this.m_slotIndex)
					{
						viewPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(null, -1);
						flag = true;
					}
					else if (viewPlayer.ComponentInput.SplitSourceInventory != null)
					{
						flag = this.HandleMoveItem(viewPlayer.ComponentInput.SplitSourceInventory, viewPlayer.ComponentInput.SplitSourceSlotIndex, this.m_inventory, this.m_slotIndex, 1);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					}
				}
				if (!flag && this.m_inventory.ActiveSlotIndex != this.m_slotIndex && this.m_slotIndex < 10)
				{
					this.m_inventory.ActiveSlotIndex = this.m_slotIndex;
					if (this.m_inventory.ActiveSlotIndex == this.m_slotIndex)
					{
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					}
				}
			}
			if (!this.m_focus || this.ProcessingOnly || viewPlayer == null)
			{
				return;
			}
			Vector2? hold = input.Hold;
			if (hold != null && base.HitTestGlobal(hold.Value, null) == this && !this.DragHostWidget.IsDragInProgress && this.m_inventory.GetSlotCount(this.m_slotIndex) > 0 && (viewPlayer.ComponentInput.SplitSourceInventory != this.m_inventory || viewPlayer.ComponentInput.SplitSourceSlotIndex != this.m_slotIndex))
			{
				input.Clear();
				viewPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(this.m_inventory, this.m_slotIndex);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			}
			Vector2? drag = input.Drag;
			if (drag == null || base.HitTestGlobal(drag.Value, null) != this || this.DragHostWidget.IsDragInProgress)
			{
				return;
			}
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			if (slotCount > 0)
			{
				DragMode dragMode = input.DragMode;
				if (viewPlayer.ComponentInput.SplitSourceInventory == this.m_inventory && viewPlayer.ComponentInput.SplitSourceSlotIndex == this.m_slotIndex)
				{
					dragMode = DragMode.SingleItem;
				}
				int num3 = (dragMode != DragMode.AllItems) ? 1 : slotCount;
				SubsystemTerrain subsystemTerrain = this.m_inventory.Project.FindSubsystem<SubsystemTerrain>();
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(null, ContentManager.Get<XElement>("Widgets/InventoryDragWidget"), null);
				containerWidget.Children.Find<BlockIconWidget>("InventoryDragWidget.Icon", true).Value = Terrain.ReplaceLight(slotValue, 15);
				containerWidget.Children.Find<BlockIconWidget>("InventoryDragWidget.Icon", true).DrawBlockEnvironmentData.SubsystemTerrain = subsystemTerrain;
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Name", true).Text = block.GetDisplayName(subsystemTerrain, slotValue);
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Count", true).Text = num3.ToString();
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Count", true).IsVisible = (!(this.m_inventory is ComponentCreativeInventory) && !(this.m_inventory is ComponentFurnitureInventory));
				this.DragHostWidget.BeginDrag(containerWidget, new InventoryDragData
				{
					Inventory = this.m_inventory,
					SlotIndex = this.m_slotIndex,
					DragMode = dragMode
				}, delegate
				{
					this.m_dragMode = null;
				});
				this.m_dragMode = new DragMode?(dragMode);
			}
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x000CD944 File Offset: 0x000CBB44
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_inventory != null)
			{
				bool flag = this.m_inventory is ComponentCreativeInventory || this.m_inventory is ComponentFurnitureInventory;
				int num = this.m_inventory.GetSlotCount(this.m_slotIndex);
				if (!flag && this.m_dragMode != null)
				{
					num = ((this.m_dragMode.Value != DragMode.AllItems) ? MathUtils.Max(num - 1, 0) : 0);
				}
				this.m_rectangleWidget.IsVisible = true;
				if (num > 0)
				{
					int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
					int num2 = Terrain.ExtractContents(slotValue);
					Block block = BlocksManager.Blocks[num2];
					bool flag2 = block.GetRotPeriod(slotValue) > 0 && block.GetDamage(slotValue) > 0;
					this.m_blockIconWidget.Value = Terrain.ReplaceLight(slotValue, 15);
					this.m_blockIconWidget.IsVisible = !this.HideBlockIcon;
					if (num != this.m_lastCount)
					{
						this.m_countWidget.Text = num.ToString();
						this.m_lastCount = num;
					}
					this.m_countWidget.IsVisible = (num > 1 && !flag);
					this.m_editOverlayWidget.IsVisible = (!this.HideEditOverlay && block.IsEditable);
					this.m_interactiveOverlayWidget.IsVisible = (!this.HideInteractiveOverlay && ((this.m_subsystemTerrain != null) ? block.IsInteractive(this.m_subsystemTerrain, slotValue) : block.DefaultIsInteractive));
					this.m_foodOverlayWidget.IsVisible = (!this.HideFoodOverlay && block.GetRotPeriod(slotValue) > 0);
					this.m_foodOverlayWidget.FillColor = (flag2 ? new Color(128, 64, 0) : new Color(160, 160, 160));
					if (!flag && !this.HideHealthBar && block.Durability >= 0)
					{
						int damage = block.GetDamage(slotValue);
						this.m_healthBarWidget.IsVisible = true;
						this.m_healthBarWidget.Value = (float)(block.Durability - damage) / (float)block.Durability;
					}
					else
					{
						this.m_healthBarWidget.IsVisible = false;
					}
				}
				else
				{
					this.m_blockIconWidget.IsVisible = false;
					this.m_countWidget.IsVisible = false;
					this.m_healthBarWidget.IsVisible = false;
					this.m_editOverlayWidget.IsVisible = false;
					this.m_interactiveOverlayWidget.IsVisible = false;
					this.m_foodOverlayWidget.IsVisible = false;
				}
				this.m_highlightWidget.IsVisible = !this.HideHighlightRectangle;
				this.m_highlightWidget.OutlineColor = Color.Transparent;
				this.m_highlightWidget.FillColor = Color.Transparent;
				this.m_splitLabelWidget.IsVisible = false;
				if (this.m_slotIndex == this.m_inventory.ActiveSlotIndex)
				{
					this.m_highlightWidget.OutlineColor = new Color(0, 0, 0);
					this.m_highlightWidget.FillColor = new Color(0, 0, 0, 80);
				}
				if (this.IsSplitMode())
				{
					this.m_highlightWidget.OutlineColor = new Color(255, 64, 0);
					this.m_splitLabelWidget.IsVisible = true;
				}
			}
			else
			{
				this.m_rectangleWidget.IsVisible = false;
				this.m_highlightWidget.IsVisible = false;
				this.m_blockIconWidget.IsVisible = false;
				this.m_countWidget.IsVisible = false;
				this.m_healthBarWidget.IsVisible = false;
				this.m_editOverlayWidget.IsVisible = false;
				this.m_interactiveOverlayWidget.IsVisible = false;
				this.m_foodOverlayWidget.IsVisible = false;
				this.m_splitLabelWidget.IsVisible = false;
			}
			base.IsDrawRequired = (this.m_inventoryDragData != null);
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x000CDCDC File Offset: 0x000CBEDC
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_inventory != null && this.m_inventoryDragData != null)
			{
				int slotValue = this.m_inventoryDragData.Inventory.GetSlotValue(this.m_inventoryDragData.SlotIndex);
				if (this.m_inventory.GetSlotProcessCapacity(this.m_slotIndex, slotValue) >= 0 || this.m_inventory.GetSlotCapacity(this.m_slotIndex, slotValue) > 0)
				{
					float num = 80f * base.GlobalTransform.Right.Length();
					Vector2 center = Vector2.Transform(base.ActualSize / 2f, base.GlobalTransform);
					FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(100, null, null, null);
					flatBatch2D.QueueEllipse(center, new Vector2(num), 0f, new Color(0, 0, 0, 96) * base.GlobalColorTransform, 64, 0f, 6.2831855f);
					flatBatch2D.QueueEllipse(center, new Vector2(num - 0.5f), 0f, new Color(0, 0, 0, 64) * base.GlobalColorTransform, 64, 0f, 6.2831855f);
					flatBatch2D.QueueEllipse(center, new Vector2(num + 0.5f), 0f, new Color(0, 0, 0, 48) * base.GlobalColorTransform, 64, 0f, 6.2831855f);
					flatBatch2D.QueueDisc(center, new Vector2(num), 0f, new Color(0, 0, 0, 48) * base.GlobalColorTransform, 64, 0f, 6.2831855f);
				}
			}
			this.m_inventoryDragData = null;
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x000CDE70 File Offset: 0x000CC070
		public void DragOver(Widget dragWidget, object data)
		{
			this.m_inventoryDragData = (data as InventoryDragData);
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000CDE80 File Offset: 0x000CC080
		public void DragDrop(Widget dragWidget, object data)
		{
			InventoryDragData inventoryDragData = data as InventoryDragData;
			if (this.m_inventory != null && inventoryDragData != null)
			{
				this.HandleDragDrop(inventoryDragData.Inventory, inventoryDragData.SlotIndex, inventoryDragData.DragMode, this.m_inventory, this.m_slotIndex);
			}
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x000CDEC4 File Offset: 0x000CC0C4
		public ComponentPlayer GetViewPlayer()
		{
			if (this.GameWidget == null)
			{
				return null;
			}
			return this.GameWidget.PlayerData.ComponentPlayer;
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x000CDEE0 File Offset: 0x000CC0E0
		public bool IsSplitMode()
		{
			ComponentPlayer viewPlayer = this.GetViewPlayer();
			return viewPlayer != null && (this.m_inventory != null && this.m_inventory == viewPlayer.ComponentInput.SplitSourceInventory) && this.m_slotIndex == viewPlayer.ComponentInput.SplitSourceSlotIndex;
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x000CDF2C File Offset: 0x000CC12C
		public bool HandleMoveItem(IInventory sourceInventory, int sourceSlotIndex, IInventory targetInventory, int targetSlotIndex, int count)
		{
			int slotValue = sourceInventory.GetSlotValue(sourceSlotIndex);
			int slotValue2 = targetInventory.GetSlotValue(targetSlotIndex);
			int slotCount = sourceInventory.GetSlotCount(sourceSlotIndex);
			int slotCount2 = targetInventory.GetSlotCount(targetSlotIndex);
			if (slotCount2 == 0 || slotValue == slotValue2)
			{
				int num = MathUtils.Min(targetInventory.GetSlotCapacity(targetSlotIndex, slotValue) - slotCount2, slotCount, count);
				if (num > 0)
				{
					int count2 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num);
					targetInventory.AddSlotItems(targetSlotIndex, slotValue, count2);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x000CDF98 File Offset: 0x000CC198
		public bool HandleDragDrop(IInventory sourceInventory, int sourceSlotIndex, DragMode dragMode, IInventory targetInventory, int targetSlotIndex)
		{
			int slotValue = sourceInventory.GetSlotValue(sourceSlotIndex);
			int slotValue2 = targetInventory.GetSlotValue(targetSlotIndex);
			int num = sourceInventory.GetSlotCount(sourceSlotIndex);
			int slotCount = targetInventory.GetSlotCount(targetSlotIndex);
			int slotCapacity = targetInventory.GetSlotCapacity(targetSlotIndex, slotValue);
			int slotProcessCapacity = targetInventory.GetSlotProcessCapacity(targetSlotIndex, slotValue);
			if (dragMode == DragMode.SingleItem)
			{
				num = MathUtils.Min(num, 1);
			}
			bool flag = false;
			if (slotProcessCapacity > 0)
			{
				int processCount = sourceInventory.RemoveSlotItems(sourceSlotIndex, MathUtils.Min(num, slotProcessCapacity));
				int num2;
				int num3;
				targetInventory.ProcessSlotItems(targetSlotIndex, slotValue, num, processCount, out num2, out num3);
				if (num2 != 0 && num3 != 0)
				{
					int count = MathUtils.Min(sourceInventory.GetSlotCapacity(sourceSlotIndex, num2), num3);
					sourceInventory.AddSlotItems(sourceSlotIndex, num2, count);
				}
				flag = true;
			}
			else if (!this.ProcessingOnly && (slotCount == 0 || slotValue == slotValue2) && slotCount < slotCapacity)
			{
				int num4 = MathUtils.Min(slotCapacity - slotCount, num);
				if (num4 > 0)
				{
					int count2 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num4);
					targetInventory.AddSlotItems(targetSlotIndex, slotValue, count2);
					flag = true;
				}
			}
			else if (!this.ProcessingOnly && targetInventory.GetSlotCapacity(targetSlotIndex, slotValue) >= num && sourceInventory.GetSlotCapacity(sourceSlotIndex, slotValue2) >= slotCount && sourceInventory.GetSlotCount(sourceSlotIndex) == num)
			{
				int count3 = targetInventory.RemoveSlotItems(targetSlotIndex, slotCount);
				int count4 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num);
				targetInventory.AddSlotItems(targetSlotIndex, slotValue, count4);
				sourceInventory.AddSlotItems(sourceSlotIndex, slotValue2, count3);
				flag = true;
			}
			if (flag)
			{
				AudioManager.PlaySound("Audio/UI/ItemMoved", 1f, 0f, 0f);
			}
			return flag;
		}

		// Token: 0x04001225 RID: 4645
		public BevelledRectangleWidget m_rectangleWidget;

		// Token: 0x04001226 RID: 4646
		public RectangleWidget m_highlightWidget;

		// Token: 0x04001227 RID: 4647
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x04001228 RID: 4648
		public LabelWidget m_countWidget;

		// Token: 0x04001229 RID: 4649
		public ValueBarWidget m_healthBarWidget;

		// Token: 0x0400122A RID: 4650
		public RectangleWidget m_editOverlayWidget;

		// Token: 0x0400122B RID: 4651
		public RectangleWidget m_interactiveOverlayWidget;

		// Token: 0x0400122C RID: 4652
		public RectangleWidget m_foodOverlayWidget;

		// Token: 0x0400122D RID: 4653
		public LabelWidget m_splitLabelWidget;

		// Token: 0x0400122E RID: 4654
		public GameWidget m_gameWidget;

		// Token: 0x0400122F RID: 4655
		public DragHostWidget m_dragHostWidget;

		// Token: 0x04001230 RID: 4656
		public IInventory m_inventory;

		// Token: 0x04001231 RID: 4657
		public int m_slotIndex;

		// Token: 0x04001232 RID: 4658
		public DragMode? m_dragMode;

		// Token: 0x04001233 RID: 4659
		public bool m_focus;

		// Token: 0x04001234 RID: 4660
		public int m_lastCount = -1;

		// Token: 0x04001235 RID: 4661
		public InventoryDragData m_inventoryDragData;

		// Token: 0x04001236 RID: 4662
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001237 RID: 4663
		public ComponentPlayer m_componentPlayer;
	}
}
