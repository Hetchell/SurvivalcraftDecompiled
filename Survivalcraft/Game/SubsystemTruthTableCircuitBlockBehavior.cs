using System;
using Engine;

namespace Game
{
	// Token: 0x020001B5 RID: 437
	public class SubsystemTruthTableCircuitBlockBehavior : SubsystemEditableItemBehavior<TruthTableData>
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x000510FD File Offset: 0x0004F2FD
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					188
				};
			}
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0005110D File Offset: 0x0004F30D
		public SubsystemTruthTableCircuitBlockBehavior() : base(188)
		{
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0005111C File Offset: 0x0004F31C
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int id = Terrain.ExtractData(value);
			TruthTableData truthTableData = base.GetItemData(id);
			if (truthTableData != null)
			{
				truthTableData = (TruthTableData)truthTableData.Copy();
			}
			else
			{
				truthTableData = new TruthTableData();
			}
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditTruthTableDialog(truthTableData, delegate(bool p)
			{
				int data = this.StoreItemDataAtUniqueId(truthTableData);
				int value0 = Terrain.ReplaceData(value, data);
				inventory.RemoveSlotItems(slotIndex, count);
				inventory.AddSlotItems(slotIndex, value0, 1);
			}));
			return true;
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x000511DC File Offset: 0x0004F3DC
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			TruthTableData truthTableData = base.GetBlockData(new Point3(x, y, z)) ?? new TruthTableData();
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditTruthTableDialog(truthTableData, delegate(bool p)
			{
				this.SetBlockData(new Point3(x, y, z), truthTableData);
				int face = ((TruthTableCircuitBlock)BlocksManager.Blocks[188]).GetFace(value);
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
