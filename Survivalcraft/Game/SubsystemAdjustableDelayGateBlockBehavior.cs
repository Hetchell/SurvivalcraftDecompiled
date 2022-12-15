using System;

namespace Game
{
	// Token: 0x02000156 RID: 342
	public class SubsystemAdjustableDelayGateBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x000298BF File Offset: 0x00027ABF
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					224
				};
			}
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x000298D0 File Offset: 0x00027AD0
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int data = Terrain.ExtractData(value);
			int delay = AdjustableDelayGateBlock.GetDelay(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditAdjustableDelayGateDialog(delay, delegate(int newDelay)
			{
				int data0 = AdjustableDelayGateBlock.SetDelay(data, newDelay);
				int num = Terrain.ReplaceData(value, data0);
				if (num != value)
				{
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, num, 1);
				}
			}));
			return true;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0002995C File Offset: 0x00027B5C
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			int data = Terrain.ExtractData(value);
			int delay = AdjustableDelayGateBlock.GetDelay(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditAdjustableDelayGateDialog(delay, delegate(int newDelay)
			{
				int num = AdjustableDelayGateBlock.SetDelay(data, newDelay);
				if (num != data)
				{
					int value2 = Terrain.ReplaceData(value, num);
					this.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
					int face = ((AdjustableDelayGateBlock)BlocksManager.Blocks[224]).GetFace(value);
					SubsystemElectricity subsystemElectricity = this.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}
			}));
			return true;
		}
	}
}
