using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200015C RID: 348
	public abstract class SubsystemBlockBehavior : Subsystem
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060006BB RID: 1723
		public abstract int[] HandledBlocks { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x0002B51B File Offset: 0x0002971B
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x0002B523 File Offset: 0x00029723
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x060006BE RID: 1726 RVA: 0x0002B52C File Offset: 0x0002972C
		public virtual void OnChunkInitialized(TerrainChunk chunk)
		{
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0002B52E File Offset: 0x0002972E
		public virtual void OnChunkDiscarding(TerrainChunk chunk)
		{
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0002B530 File Offset: 0x00029730
		public virtual void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0002B532 File Offset: 0x00029732
		public virtual void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0002B534 File Offset: 0x00029734
		public virtual void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0002B536 File Offset: 0x00029736
		public virtual void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0002B538 File Offset: 0x00029738
		public virtual void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0002B53A File Offset: 0x0002973A
		public virtual bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0002B53D File Offset: 0x0002973D
		public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0002B540 File Offset: 0x00029740
		public virtual bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			return false;
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0002B543 File Offset: 0x00029743
		public virtual bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			return false;
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0002B546 File Offset: 0x00029746
		public virtual bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			return false;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0002B549 File Offset: 0x00029749
		public virtual void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0002B54B File Offset: 0x0002974B
		public virtual void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x0002B54D File Offset: 0x0002974D
		public virtual void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0002B54F File Offset: 0x0002974F
		public virtual void OnExplosion(int value, int x, int y, int z, float damage)
		{
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0002B551 File Offset: 0x00029751
		public virtual void OnFiredAsProjectile(Projectile projectile)
		{
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x0002B553 File Offset: 0x00029753
		public virtual bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			return false;
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0002B556 File Offset: 0x00029756
		public virtual void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0002B558 File Offset: 0x00029758
		public virtual int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			return 0;
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x0002B55B File Offset: 0x0002975B
		public virtual void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			throw new InvalidOperationException("Cannot process items.");
		}

        // Token: 0x060006D3 RID: 1747 RVA: 0x0002B567 File Offset: 0x00029767
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
		}
	}
}
