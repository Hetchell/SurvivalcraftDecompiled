using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CD RID: 461
	public class ComponentCreativeInventory : Component, IInventory
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000C1C RID: 3100 RVA: 0x0005C451 File Offset: 0x0005A651
		// (set) Token: 0x06000C1D RID: 3101 RVA: 0x0005C459 File Offset: 0x0005A659
		public int OpenSlotsCount { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000C1E RID: 3102 RVA: 0x0005C462 File Offset: 0x0005A662
		// (set) Token: 0x06000C1F RID: 3103 RVA: 0x0005C46A File Offset: 0x0005A66A
		public int CategoryIndex { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000C20 RID: 3104 RVA: 0x0005C473 File Offset: 0x0005A673
		// (set) Token: 0x06000C21 RID: 3105 RVA: 0x0005C47B File Offset: 0x0005A67B
		public int PageIndex { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000C22 RID: 3106 RVA: 0x0005C484 File Offset: 0x0005A684
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000C23 RID: 3107 RVA: 0x0005C48C File Offset: 0x0005A68C
		// (set) Token: 0x06000C24 RID: 3108 RVA: 0x0005C494 File Offset: 0x0005A694
		public int ActiveSlotIndex
		{
			get
			{
				return this.m_activeSlotIndex;
			}
			set
			{
				this.m_activeSlotIndex = MathUtils.Clamp(value, 0, this.VisibleSlotsCount - 1);
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000C25 RID: 3109 RVA: 0x0005C4AB File Offset: 0x0005A6AB
		public int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x0005C4B8 File Offset: 0x0005A6B8
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x0005C4C0 File Offset: 0x0005A6C0
		public int VisibleSlotsCount
		{
			get
			{
				return this.m_visibleSlotsCount;
			}
			set
			{
				value = MathUtils.Clamp(value, 0, 10);
				if (value == this.m_visibleSlotsCount)
				{
					return;
				}
				this.m_visibleSlotsCount = value;
				this.ActiveSlotIndex = this.ActiveSlotIndex;
				ComponentFrame componentFrame = base.Entity.FindComponent<ComponentFrame>();
				if (componentFrame != null)
				{
					Vector3 position = componentFrame.Position + new Vector3(0f, 0.5f, 0f);
					Vector3 velocity = 1f * componentFrame.Rotation.GetForwardVector();
					for (int i = this.m_visibleSlotsCount; i < 10; i++)
					{
						ComponentInventoryBase.DropSlotItems(this, i, position, velocity);
					}
				}
			}
		}

        // Token: 0x06000C28 RID: 3112 RVA: 0x0005C560 File Offset: 0x0005A760
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_activeSlotIndex = valuesDictionary.GetValue<int>("ActiveSlotIndex");
			this.OpenSlotsCount = valuesDictionary.GetValue<int>("OpenSlotsCount");
			this.CategoryIndex = valuesDictionary.GetValue<int>("CategoryIndex");
			this.PageIndex = valuesDictionary.GetValue<int>("PageIndex");
			for (int i = 0; i < this.OpenSlotsCount; i++)
			{
				this.m_slots.Add(0);
			}
			foreach (Block block in from b in BlocksManager.Blocks
			orderby b.DisplayOrder
			select b)
			{
				foreach (int item in block.GetCreativeValues())
				{
					this.m_slots.Add(item);
				}
			}
			ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("Slots", null);
			if (value == null)
			{
				return;
			}
			for (int j = 0; j < this.OpenSlotsCount; j++)
			{
				ValuesDictionary value2 = value.GetValue<ValuesDictionary>("Slot" + j.ToString(CultureInfo.InvariantCulture), null);
				if (value2 != null)
				{
					this.m_slots[j] = value2.GetValue<int>("Contents");
				}
			}
		}

        // Token: 0x06000C29 RID: 3113 RVA: 0x0005C6D0 File Offset: 0x0005A8D0
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<int>("ActiveSlotIndex", this.m_activeSlotIndex);
			valuesDictionary.SetValue<int>("CategoryIndex", this.CategoryIndex);
			valuesDictionary.SetValue<int>("PageIndex", this.PageIndex);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Slots", valuesDictionary2);
			for (int i = 0; i < this.OpenSlotsCount; i++)
			{
				if (this.m_slots[i] != 0)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
					valuesDictionary3.SetValue<int>("Contents", this.m_slots[i]);
				}
			}
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0005C77B File Offset: 0x0005A97B
		public int GetSlotValue(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex];
			}
			return 0;
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x0005C79D File Offset: 0x0005A99D
		public int GetSlotCount(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= this.m_slots.Count)
			{
				return 0;
			}
			if (this.m_slots[slotIndex] == 0)
			{
				return 0;
			}
			return 9999;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0005C7C8 File Offset: 0x0005A9C8
		public int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= this.VisibleSlotsCount && slotIndex < 10)
			{
				return 0;
			}
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				return 99980001;
			}
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num].IsNonDuplicable)
			{
				return 9999;
			}
			return 99980001;
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x0005C818 File Offset: 0x0005AA18
		public int GetSlotProcessCapacity(int slotIndex, int value)
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
			if (slotIndex < this.OpenSlotsCount)
			{
				return 0;
			}
			return 9999;
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x0005C882 File Offset: 0x0005AA82
		public void AddSlotItems(int slotIndex, int value, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				this.m_slots[slotIndex] = value;
			}
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x0005C8A0 File Offset: 0x0005AAA0
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
			if (slotIndex >= this.OpenSlotsCount)
			{
				processedValue = 0;
				processedCount = 0;
				return;
			}
			processedValue = value;
			processedCount = count;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0005C930 File Offset: 0x0005AB30
		public int RemoveSlotItems(int slotIndex, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				int num = Terrain.ExtractContents(this.m_slots[slotIndex]);
				if (BlocksManager.Blocks[num].IsNonDuplicable)
				{
					this.m_slots[slotIndex] = 0;
					return 1;
				}
				if (count >= 9999)
				{
					this.m_slots[slotIndex] = 0;
					return 1;
				}
			}
			return 1;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0005C991 File Offset: 0x0005AB91
		public void DropAllItems(Vector3 position)
		{
		}

		// Token: 0x040006EF RID: 1775
		public List<int> m_slots = new List<int>();

		// Token: 0x040006F0 RID: 1776
		public int m_activeSlotIndex;

		// Token: 0x040006F1 RID: 1777
		public int m_visibleSlotsCount = 10;

		// Token: 0x040006F2 RID: 1778
		public const int m_largeNumber = 9999;
	}
}
