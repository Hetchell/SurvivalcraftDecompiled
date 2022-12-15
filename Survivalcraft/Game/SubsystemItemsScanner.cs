using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000189 RID: 393
	public class SubsystemItemsScanner : Subsystem, IUpdateable
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0003DB6B File Offset: 0x0003BD6B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060008FA RID: 2298 RVA: 0x0003DB70 File Offset: 0x0003BD70
		// (remove) Token: 0x060008FB RID: 2299 RVA: 0x0003DBA8 File Offset: 0x0003BDA8
		public event Action<ReadOnlyList<ScannedItemData>> ItemsScanned;

		// Token: 0x060008FC RID: 2300 RVA: 0x0003DBE0 File Offset: 0x0003BDE0
		public ReadOnlyList<ScannedItemData> ScanItems()
		{
			this.m_items.Clear();
			foreach (Subsystem subsystem in base.Project.Subsystems)
			{
				IInventory inventory = subsystem as IInventory;
				if (inventory != null)
				{
					this.ScanInventory(inventory, this.m_items);
				}
			}
			foreach (Entity entity in base.Project.Entities)
			{
				foreach (Component component in entity.Components)
				{
					IInventory inventory2 = component as IInventory;
					if (inventory2 != null)
					{
						this.ScanInventory(inventory2, this.m_items);
					}
				}
			}
			foreach (Pickable pickable in base.Project.FindSubsystem<SubsystemPickables>(true).Pickables)
			{
				if (pickable.Count > 0 && pickable.Value != 0)
				{
					List<ScannedItemData> items = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = pickable,
						Value = pickable.Value,
						Count = pickable.Count
					};
					items.Add(item);
				}
			}
			foreach (Projectile projectile in base.Project.FindSubsystem<SubsystemProjectiles>(true).Projectiles)
			{
				if (projectile.Value != 0)
				{
					List<ScannedItemData> items2 = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = projectile,
						Value = projectile.Value,
						Count = 1
					};
					items2.Add(item);
				}
			}
			foreach (IMovingBlockSet movingBlockSet in base.Project.FindSubsystem<SubsystemMovingBlocks>(true).MovingBlockSets)
			{
				for (int i = 0; i < movingBlockSet.Blocks.Count; i++)
				{
					List<ScannedItemData> items3 = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = movingBlockSet,
						Value = movingBlockSet.Blocks[i].Value,
						Count = 1,
						IndexInContainer = i
					};
					items3.Add(item);
				}
			}
			return new ReadOnlyList<ScannedItemData>(this.m_items);
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0003DED0 File Offset: 0x0003C0D0
		public bool TryModifyItem(ScannedItemData itemData, int newValue)
		{
			if (itemData.Container is IInventory)
			{
				IInventory inventory = (IInventory)itemData.Container;
				inventory.RemoveSlotItems(itemData.IndexInContainer, itemData.Count);
				int slotCapacity = inventory.GetSlotCapacity(itemData.IndexInContainer, newValue);
				inventory.AddSlotItems(itemData.IndexInContainer, newValue, MathUtils.Min(itemData.Count, slotCapacity));
				return true;
			}
			if (itemData.Container is WorldItem)
			{
				((WorldItem)itemData.Container).Value = newValue;
				return true;
			}
			if (itemData.Container is IMovingBlockSet)
			{
				IMovingBlockSet movingBlockSet = (IMovingBlockSet)itemData.Container;
				MovingBlock movingBlock = movingBlockSet.Blocks.ElementAt(itemData.IndexInContainer);
				movingBlockSet.SetBlock(movingBlock.Offset, newValue);
				return true;
			}
			return false;
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0003DF8E File Offset: 0x0003C18E
		public void Update(float dt)
		{
			if (Time.FrameStartTime >= this.m_nextAutomaticScanTime)
			{
				this.m_nextAutomaticScanTime = Time.FrameStartTime + 60.0;
				Action<ReadOnlyList<ScannedItemData>> itemsScanned = this.ItemsScanned;
				if (itemsScanned == null)
				{
					return;
				}
				itemsScanned(this.ScanItems());
			}
		}

        // Token: 0x060008FF RID: 2303 RVA: 0x0003DFC8 File Offset: 0x0003C1C8
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_nextAutomaticScanTime = Time.FrameStartTime + 60.0;
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x0003DFE0 File Offset: 0x0003C1E0
		public void ScanInventory(IInventory inventory, List<ScannedItemData> items)
		{
			for (int i = 0; i < inventory.SlotsCount; i++)
			{
				int slotCount = inventory.GetSlotCount(i);
				if (slotCount > 0)
				{
					int slotValue = inventory.GetSlotValue(i);
					if (slotValue != 0)
					{
						items.Add(new ScannedItemData
						{
							Container = inventory,
							IndexInContainer = i,
							Value = slotValue,
							Count = slotCount
						});
					}
				}
			}
		}

		// Token: 0x040004B3 RID: 1203
		public const float m_automaticScanPeriod = 60f;

		// Token: 0x040004B4 RID: 1204
		public double m_nextAutomaticScanTime;

		// Token: 0x040004B5 RID: 1205
		public List<ScannedItemData> m_items = new List<ScannedItemData>();
	}
}
