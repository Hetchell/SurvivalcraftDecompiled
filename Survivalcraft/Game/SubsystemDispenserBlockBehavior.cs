using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000170 RID: 368
	public class SubsystemDispenserBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0003397D File Offset: 0x00031B7D
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					216
				};
			}
		}

        // Token: 0x060007E2 RID: 2018 RVA: 0x00033990 File Offset: 0x00031B90
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockEntities = base.Project.FindSubsystem<SubsystemBlockEntities>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x000339EC File Offset: 0x00031BEC
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.Project.GameDatabase.Database.FindDatabaseObject("Dispenser", base.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Entity entity = base.Project.CreateEntity(valuesDictionary);
			base.Project.AddEntity(entity);
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00033A6C File Offset: 0x00031C6C
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position);
				}
				base.Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00033B08 File Offset: 0x00031D08
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure)
			{
				ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
				if (blockEntity != null && componentMiner.ComponentPlayer != null)
				{
					ComponentDispenser componentDispenser = blockEntity.Entity.FindComponent<ComponentDispenser>(true);
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new DispenserWidget(componentMiner.Inventory, componentDispenser);
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00033BA8 File Offset: 0x00031DA8
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			if (blockEntity != null && DispenserBlock.GetAcceptsDrops(Terrain.ExtractData(this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z))))
			{
				IInventory inventory = blockEntity.Entity.FindComponent<ComponentDispenser>(true);
				Pickable pickable = worldItem as Pickable;
				int num = (pickable != null) ? pickable.Count : 1;
				int num2 = ComponentInventoryBase.AcquireItems(inventory, worldItem.Value, num);
				if (num2 < num)
				{
					this.m_subsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
				}
				if (num2 <= 0)
				{
					worldItem.ToRemove = true;
					return;
				}
				if (pickable != null)
				{
					pickable.Count = num2;
				}
			}
		}

		// Token: 0x04000425 RID: 1061
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000426 RID: 1062
		public SubsystemBlockEntities m_subsystemBlockEntities;

		// Token: 0x04000427 RID: 1063
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000428 RID: 1064
		public SubsystemAudio m_subsystemAudio;
	}
}
