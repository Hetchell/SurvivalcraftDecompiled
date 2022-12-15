using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EC RID: 492
	public class ComponentInventory : ComponentInventoryBase, IInventory
	{
		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x0006C016 File Offset: 0x0006A216
		// (set) Token: 0x06000DE8 RID: 3560 RVA: 0x0006C01E File Offset: 0x0006A21E
		public override int ActiveSlotIndex
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

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x0006C035 File Offset: 0x0006A235
		// (set) Token: 0x06000DEA RID: 3562 RVA: 0x0006C040 File Offset: 0x0006A240
		public override int VisibleSlotsCount
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

        // Token: 0x06000DEB RID: 3563 RVA: 0x0006C0DD File Offset: 0x0006A2DD
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.ActiveSlotIndex = valuesDictionary.GetValue<int>("ActiveSlotIndex");
		}

        // Token: 0x06000DEC RID: 3564 RVA: 0x0006C0F8 File Offset: 0x0006A2F8
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<int>("ActiveSlotIndex", this.ActiveSlotIndex);
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x0006C113 File Offset: 0x0006A313
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= this.VisibleSlotsCount && slotIndex < 10)
			{
				return 0;
			}
			return BlocksManager.Blocks[Terrain.ExtractContents(value)].MaxStacking;
		}

		// Token: 0x040008CB RID: 2251
		public int m_activeSlotIndex;

		// Token: 0x040008CC RID: 2252
		public int m_visibleSlotsCount = 10;

		// Token: 0x040008CD RID: 2253
		public const int ShortInventorySlotsCount = 10;
	}
}
