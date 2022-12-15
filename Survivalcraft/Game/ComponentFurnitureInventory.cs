using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E2 RID: 482
	public class ComponentFurnitureInventory : Component, IInventory
	{
		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000D33 RID: 3379 RVA: 0x00064AA7 File Offset: 0x00062CA7
		// (set) Token: 0x06000D34 RID: 3380 RVA: 0x00064AAF File Offset: 0x00062CAF
		public int PageIndex { get; set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000D35 RID: 3381 RVA: 0x00064AB8 File Offset: 0x00062CB8
		// (set) Token: 0x06000D36 RID: 3382 RVA: 0x00064AC0 File Offset: 0x00062CC0
		public FurnitureSet FurnitureSet { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000D37 RID: 3383 RVA: 0x00064AC9 File Offset: 0x00062CC9
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000D38 RID: 3384 RVA: 0x00064AD1 File Offset: 0x00062CD1
		// (set) Token: 0x06000D39 RID: 3385 RVA: 0x00064AD4 File Offset: 0x00062CD4
		public int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000D3A RID: 3386 RVA: 0x00064AD6 File Offset: 0x00062CD6
		public int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000D3B RID: 3387 RVA: 0x00064AE3 File Offset: 0x00062CE3
		// (set) Token: 0x06000D3C RID: 3388 RVA: 0x00064AEB File Offset: 0x00062CEB
		public int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x00064AF0 File Offset: 0x00062CF0
		public void FillSlots()
		{
			this.m_subsystemFurnitureBlockBehavior.GarbageCollectDesigns();
			this.m_slots.Clear();
			for (int i = 0; i < 65535; i++)
			{
				FurnitureDesign design = this.m_subsystemFurnitureBlockBehavior.GetDesign(i);
				if (design != null)
				{
					int num = (from f in design.ListChain()
					select f.Index).Min();
					if (design.Index == num)
					{
						int data = FurnitureBlock.SetDesignIndex(0, i, design.ShadowStrengthFactor, design.IsLightEmitter);
						int item = Terrain.MakeBlockValue(227, 0, data);
						this.m_slots.Add(item);
					}
				}
			}
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00064B9E File Offset: 0x00062D9E
		public void ClearSlots()
		{
			this.m_slots.Clear();
		}

        // Token: 0x06000D3F RID: 3391 RVA: 0x00064BAC File Offset: 0x00062DAC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			string furnitureSetName = valuesDictionary.GetValue<string>("FurnitureSet");
			this.FurnitureSet = this.m_subsystemFurnitureBlockBehavior.FurnitureSets.FirstOrDefault((FurnitureSet f) => f.Name == furnitureSetName);
		}

        // Token: 0x06000D40 RID: 3392 RVA: 0x00064C09 File Offset: 0x00062E09
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<string>("FurnitureSet", (this.FurnitureSet != null) ? this.FurnitureSet.Name : string.Empty);
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x00064C30 File Offset: 0x00062E30
		public int GetSlotValue(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex];
			}
			return 0;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x00064C52 File Offset: 0x00062E52
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

		// Token: 0x06000D43 RID: 3395 RVA: 0x00064C7D File Offset: 0x00062E7D
		public int GetSlotCapacity(int slotIndex, int value)
		{
			return 99980001;
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00064C84 File Offset: 0x00062E84
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
			return 9999;
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00064CE3 File Offset: 0x00062EE3
		public void AddSlotItems(int slotIndex, int value, int count)
		{
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00064CE8 File Offset: 0x00062EE8
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
			processedValue = 0;
			processedCount = 0;
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00064D64 File Offset: 0x00062F64
		public int RemoveSlotItems(int slotIndex, int count)
		{
			return 1;
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x00064D67 File Offset: 0x00062F67
		public void DropAllItems(Vector3 position)
		{
		}

		// Token: 0x04000817 RID: 2071
		public SubsystemFurnitureBlockBehavior m_subsystemFurnitureBlockBehavior;

		// Token: 0x04000818 RID: 2072
		public List<int> m_slots = new List<int>();

		// Token: 0x04000819 RID: 2073
		public const int m_largeNumber = 9999;

		// Token: 0x0400081A RID: 2074
		public const int maxDesign = 65535;
	}
}
