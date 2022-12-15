using System;
using Engine;

namespace Game
{
	// Token: 0x0200018F RID: 399
	public class SubsystemMemoryBankBlockBehavior : SubsystemEditableItemBehavior<MemoryBankData>
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x0003EABB File Offset: 0x0003CCBB
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					186
				};
			}
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0003EACB File Offset: 0x0003CCCB
		public SubsystemMemoryBankBlockBehavior() : base(186)
		{
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0003EAD8 File Offset: 0x0003CCD8
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int id = Terrain.ExtractData(value);
			MemoryBankData memoryBankData = base.GetItemData(id);
			if (memoryBankData != null)
			{
				memoryBankData = (MemoryBankData)memoryBankData.Copy();
			}
			else
			{
				memoryBankData = new MemoryBankData();
			}
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemeryDialogB(memoryBankData, delegate()
			{
				int data = this.StoreItemDataAtUniqueId(memoryBankData);
				int value0 = Terrain.ReplaceData(value, data);
				inventory.RemoveSlotItems(slotIndex, count);
				inventory.AddSlotItems(slotIndex, value0, 1);
			}));
			return true;
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x0003EB98 File Offset: 0x0003CD98
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			MemoryBankData memoryBankData = base.GetBlockData(new Point3(x, y, z)) ?? new MemoryBankData();
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemeryDialogB(memoryBankData, delegate()
			{
				this.SetBlockData(new Point3(x, y, z), memoryBankData);
				int face = ((MemoryBankBlock)BlocksManager.Blocks[186]).GetFace(value);
				SubsystemElectricity subsystemElectricity = this.SubsystemTerrain.Project.FindSubsystem<SubsystemElectricity>(true);
				ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
				if (electricElement != null)
				{
					subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
				}
			}));
			return true;
		}
	}
}
