using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200016D RID: 365
	public class SubsystemCraftingTableBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600074C RID: 1868 RVA: 0x0002E80D File Offset: 0x0002CA0D
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					27
				};
			}
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0002E81C File Offset: 0x0002CA1C
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("CraftingTable", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Entity entity = base.SubsystemTerrain.Project.CreateEntity(valuesDictionary);
			base.SubsystemTerrain.Project.AddEntity(entity);
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0002E8B0 File Offset: 0x0002CAB0
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position);
				}
				base.SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0002E95C File Offset: 0x0002CB5C
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity != null && componentMiner.ComponentPlayer != null)
			{
				ComponentCraftingTable componentCraftingTable = blockEntity.Entity.FindComponent<ComponentCraftingTable>(true);
				componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CraftingTableWidget(componentMiner.Inventory, componentCraftingTable);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				return true;
			}
			return false;
		}
	}
}
