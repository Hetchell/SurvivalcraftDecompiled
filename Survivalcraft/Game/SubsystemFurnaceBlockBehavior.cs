using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200017F RID: 383
	public class SubsystemFurnaceBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x0003B05C File Offset: 0x0003925C
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					64,
					65
				};
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0003B070 File Offset: 0x00039270
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != 64 && Terrain.ExtractContents(oldValue) != 65)
			{
				DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("Furnace", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
				Entity entity = base.SubsystemTerrain.Project.CreateEntity(valuesDictionary);
				base.SubsystemTerrain.Project.AddEntity(entity);
			}
			if (Terrain.ExtractContents(value) == 65)
			{
				this.AddFire(value, x, y, z);
			}
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x0003B134 File Offset: 0x00039334
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(newValue) != 64 && Terrain.ExtractContents(newValue) != 65)
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
			if (Terrain.ExtractContents(value) == 65)
			{
				this.RemoveFire(x, y, z);
			}
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x0003B210 File Offset: 0x00039410
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (Terrain.ExtractContents(value) == 65)
			{
				this.AddFire(value, x, y, z);
			}
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0003B228 File Offset: 0x00039428
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveFire(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0003B32C File Offset: 0x0003952C
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity != null && componentMiner.ComponentPlayer != null)
			{
				ComponentFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentFurnace>(true);
				componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FurnaceWidget(componentMiner.Inventory, componentFurnace);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				return true;
			}
			return false;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0003B3C1 File Offset: 0x000395C1
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}

        // Token: 0x0600089F RID: 2207 RVA: 0x0003B3D2 File Offset: 0x000395D2
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0003B3F0 File Offset: 0x000395F0
		public void AddFire(int value, int x, int y, int z)
		{
			Vector3 v = new Vector3(0.5f, 0.2f, 0.5f);
			float size = 0.15f;
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 16f);
			this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0003B458 File Offset: 0x00039658
		public void RemoveFire(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem particleSystem = this.m_particleSystemsByCell[key];
			this.m_subsystemParticles.RemoveParticleSystem(particleSystem);
			this.m_particleSystemsByCell.Remove(key);
		}

		// Token: 0x04000492 RID: 1170
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000493 RID: 1171
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
	}
}
