using System;

namespace Game
{
	// Token: 0x0200015B RID: 347
	public class SubsystemBatteryBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0002B401 File Offset: 0x00029601
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					138
				};
			}
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0002B414 File Offset: 0x00029614
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int data = Terrain.ExtractData(value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditBatteryDialog(voltageLevel, delegate(int newVoltageLevel)
			{
				int data0 = BatteryBlock.SetVoltageLevel(data, newVoltageLevel);
				int num = Terrain.ReplaceData(value, data0);
				if (num != value)
				{
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, num, 1);
				}
			}));
			return true;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0002B4A0 File Offset: 0x000296A0
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			int data = Terrain.ExtractData(value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditBatteryDialog(voltageLevel, delegate(int newVoltageLevel)
			{
				int num = BatteryBlock.SetVoltageLevel(data, newVoltageLevel);
				if (num != data)
				{
					int value2 = Terrain.ReplaceData(value, num);
					this.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
					SubsystemElectricity subsystemElectricity = this.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, 4);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementConnectionsForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}
			}));
			return true;
		}
	}
}
