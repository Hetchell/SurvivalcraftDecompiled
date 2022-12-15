using System;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000295 RID: 661
	public interface IInventory
	{
		// Token: 0x170002CB RID: 715
		// (get) Token: 0x0600134A RID: 4938
		Project Project { get; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x0600134B RID: 4939
		int SlotsCount { get; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600134C RID: 4940
		// (set) Token: 0x0600134D RID: 4941
		int VisibleSlotsCount { get; set; }

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x0600134E RID: 4942
		// (set) Token: 0x0600134F RID: 4943
		int ActiveSlotIndex { get; set; }

		// Token: 0x06001350 RID: 4944
		int GetSlotValue(int slotIndex);

		// Token: 0x06001351 RID: 4945
		int GetSlotCount(int slotIndex);

		// Token: 0x06001352 RID: 4946
		int GetSlotCapacity(int slotIndex, int value);

		// Token: 0x06001353 RID: 4947
		int GetSlotProcessCapacity(int slotIndex, int value);

		// Token: 0x06001354 RID: 4948
		void AddSlotItems(int slotIndex, int value, int count);

		// Token: 0x06001355 RID: 4949
		void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount);

		// Token: 0x06001356 RID: 4950
		int RemoveSlotItems(int slotIndex, int count);

		// Token: 0x06001357 RID: 4951
		void DropAllItems(Vector3 position);
	}
}
