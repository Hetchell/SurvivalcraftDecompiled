using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001ED RID: 493
	public abstract class ComponentInventoryBase : Component, IInventory
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000DEF RID: 3567 RVA: 0x0006C146 File Offset: 0x0006A346
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x0006C14E File Offset: 0x0006A34E
		public virtual int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x0006C15B File Offset: 0x0006A35B
		// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x0006C163 File Offset: 0x0006A363
		public virtual int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x0006C165 File Offset: 0x0006A365
		// (set) Token: 0x06000DF4 RID: 3572 RVA: 0x0006C168 File Offset: 0x0006A368
		public virtual int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x0006C16C File Offset: 0x0006A36C
		public static int FindAcquireSlotForItem(IInventory inventory, int value)
		{
			for (int i = 0; i < inventory.SlotsCount; i++)
			{
				if (inventory.GetSlotCount(i) > 0 && inventory.GetSlotValue(i) == value && inventory.GetSlotCount(i) < inventory.GetSlotCapacity(i, value))
				{
					return i;
				}
			}
			for (int j = 0; j < inventory.SlotsCount; j++)
			{
				if (inventory.GetSlotCount(j) == 0 && inventory.GetSlotCapacity(j, value) > 0)
				{
					return j;
				}
			}
			return -1;
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x0006C1DC File Offset: 0x0006A3DC
		public static int AcquireItems(IInventory inventory, int value, int count)
		{
			while (count > 0)
			{
				int num = ComponentInventoryBase.FindAcquireSlotForItem(inventory, value);
				if (num < 0)
				{
					break;
				}
				inventory.AddSlotItems(num, value, 1);
				count--;
			}
			return count;
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x0006C20C File Offset: 0x0006A40C
		public ComponentPlayer FindInteractingPlayer()
		{
			ComponentPlayer componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			if (componentPlayer == null)
			{
				ComponentBlockEntity componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>();
				if (componentBlockEntity != null)
				{
					Vector3 position = new Vector3(componentBlockEntity.Coordinates);
					componentPlayer = base.Project.FindSubsystem<SubsystemPlayers>(true).FindNearestPlayer(position);
				}
			}
			return componentPlayer;
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x0006C258 File Offset: 0x0006A458
		public static void DropSlotItems(IInventory inventory, int slotIndex, Vector3 position, Vector3 velocity)
		{
			int slotCount = inventory.GetSlotCount(slotIndex);
			if (slotCount > 0)
			{
				int slotValue = inventory.GetSlotValue(slotIndex);
				int num = inventory.RemoveSlotItems(slotIndex, slotCount);
				if (num > 0)
				{
					inventory.Project.FindSubsystem<SubsystemPickables>(true).AddPickable(slotValue, num, position, new Vector3?(velocity), null);
				}
			}
		}

        // Token: 0x06000DF9 RID: 3577 RVA: 0x0006C2AC File Offset: 0x0006A4AC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			int value = valuesDictionary.GetValue<int>("SlotsCount");
			for (int i = 0; i < value; i++)
			{
				this.m_slots.Add(new ComponentInventoryBase.Slot());
			}
			ValuesDictionary value2 = valuesDictionary.GetValue<ValuesDictionary>("Slots");
			for (int j = 0; j < this.m_slots.Count; j++)
			{
				ValuesDictionary value3 = value2.GetValue<ValuesDictionary>("Slot" + j.ToString(CultureInfo.InvariantCulture), null);
				if (value3 != null)
				{
					ComponentInventoryBase.Slot slot = this.m_slots[j];
					slot.Value = value3.GetValue<int>("Contents");
					slot.Count = value3.GetValue<int>("Count");
				}
			}
		}

        // Token: 0x06000DFA RID: 3578 RVA: 0x0006C358 File Offset: 0x0006A558
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Slots", valuesDictionary2);
			for (int i = 0; i < this.m_slots.Count; i++)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[i];
				if (slot.Count > 0)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
					valuesDictionary3.SetValue<int>("Contents", slot.Value);
					valuesDictionary3.SetValue<int>("Count", slot.Count);
				}
			}
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x0006C3E8 File Offset: 0x0006A5E8
		public virtual int GetSlotValue(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= this.m_slots.Count)
			{
				return 0;
			}
			if (this.m_slots[slotIndex].Count <= 0)
			{
				return 0;
			}
			return this.m_slots[slotIndex].Value;
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x0006C425 File Offset: 0x0006A625
		public virtual int GetSlotCount(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex].Count;
			}
			return 0;
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x0006C44C File Offset: 0x0006A64C
		public virtual int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return BlocksManager.Blocks[Terrain.ExtractContents(value)].MaxStacking;
			}
			return 0;
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x0006C474 File Offset: 0x0006A674
		public virtual int GetSlotProcessCapacity(int slotIndex, int value)
		{
			int slotCount = this.GetSlotCount(slotIndex);
			int slotValue = this.GetSlotValue(slotIndex);
			if (slotCount > 0 && slotValue != 0)
			{
				SubsystemBlockBehavior[] blockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true).GetBlockBehaviors(Terrain.ExtractContents(slotValue));
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					int processInventoryItemCapacity = blockBehaviors[i].GetProcessInventoryItemCapacity(this, slotIndex, value);
					if (processInventoryItemCapacity > 0)
					{
						return processInventoryItemCapacity;
					}
				}
			}
			return 0;
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x0006C4D0 File Offset: 0x0006A6D0
		public virtual void AddSlotItems(int slotIndex, int value, int count)
		{
			if (count > 0 && slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[slotIndex];
				if ((this.GetSlotCount(slotIndex) != 0 && this.GetSlotValue(slotIndex) != value) || this.GetSlotCount(slotIndex) + count > this.GetSlotCapacity(slotIndex, value))
				{
					throw new InvalidOperationException("Cannot add slot items.");
				}
				slot.Value = value;
				slot.Count += count;
			}
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x0006C544 File Offset: 0x0006A744
		public virtual void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			int slotCount = this.GetSlotCount(slotIndex);
			int slotValue = this.GetSlotValue(slotIndex);
			if (slotCount > 0 && slotValue != 0)
			{
				foreach (SubsystemBlockBehavior subsystemBlockBehavior in base.Project.FindSubsystem<SubsystemBlockBehaviors>(true).GetBlockBehaviors(Terrain.ExtractContents(slotValue)))
				{
					int processInventoryItemCapacity = subsystemBlockBehavior.GetProcessInventoryItemCapacity(this, slotIndex, value);
					if (processInventoryItemCapacity > 0)
					{
						subsystemBlockBehavior.ProcessInventoryItem(this, slotIndex, value, count, MathUtils.Min(processInventoryItemCapacity, processCount), out processedValue, out processedCount);
						return;
					}
				}
			}
			processedValue = value;
			processedCount = count;
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x0006C5C0 File Offset: 0x0006A7C0
		public virtual int RemoveSlotItems(int slotIndex, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[slotIndex];
				count = MathUtils.Min(count, this.GetSlotCount(slotIndex));
				slot.Count -= count;
				return count;
			}
			return 0;
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x0006C600 File Offset: 0x0006A800
		public void DropAllItems(Vector3 position)
		{
			for (int i = 0; i < this.SlotsCount; i++)
			{
				ComponentInventoryBase.DropSlotItems(this, i, position, this.m_random.Float(5f, 10f) * Vector3.Normalize(new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(1f, 2f), this.m_random.Float(-1f, 1f))));
			}
		}

		// Token: 0x040008CE RID: 2254
		public List<ComponentInventoryBase.Slot> m_slots = new List<ComponentInventoryBase.Slot>();

		// Token: 0x040008CF RID: 2255
		public Game.Random m_random = new Game.Random();

		// Token: 0x0200045A RID: 1114
		public class Slot
		{
			// Token: 0x0400164E RID: 5710
			public int Value;

			// Token: 0x0400164F RID: 5711
			public int Count;
		}
	}
}
