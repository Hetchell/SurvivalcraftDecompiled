using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200015E RID: 350
	public class SubsystemBlockEntities : Subsystem
	{
		// Token: 0x060006D9 RID: 1753 RVA: 0x0002B6F4 File Offset: 0x000298F4
		public ComponentBlockEntity GetBlockEntity(int x, int y, int z)
		{
			ComponentBlockEntity result;
			this.m_blockEntities.TryGetValue(new Point3(x, y, z), out result);
			return result;
		}

        // Token: 0x060006DA RID: 1754 RVA: 0x0002B718 File Offset: 0x00029918
        public override void OnEntityAdded(Entity entity)
		{
			ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
			if (componentBlockEntity != null)
			{
				this.m_blockEntities.Add(componentBlockEntity.Coordinates, componentBlockEntity);
			}
		}

        // Token: 0x060006DB RID: 1755 RVA: 0x0002B744 File Offset: 0x00029944
        public override void OnEntityRemoved(Entity entity)
		{
			ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
			if (componentBlockEntity != null)
			{
				this.m_blockEntities.Remove(componentBlockEntity.Coordinates);
			}
		}

		// Token: 0x040003CC RID: 972
		public Dictionary<Point3, ComponentBlockEntity> m_blockEntities = new Dictionary<Point3, ComponentBlockEntity>();
	}
}
