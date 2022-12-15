using System;
using System.Collections.Generic;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C1 RID: 449
	public class ComponentBehaviorSelector : Component, IUpdateable
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x000544A6 File Offset: 0x000526A6
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x000544AC File Offset: 0x000526AC
		public void Update(float dt)
		{
			ComponentBehavior componentBehavior = null;
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = 0f;
				foreach (ComponentBehavior componentBehavior2 in this.m_behaviors)
				{
					float importanceLevel = componentBehavior2.ImportanceLevel;
					if (importanceLevel > num)
					{
						num = importanceLevel;
						componentBehavior = componentBehavior2;
					}
				}
			}
			foreach (ComponentBehavior componentBehavior3 in this.m_behaviors)
			{
				if (componentBehavior3 == componentBehavior)
				{
					if (!componentBehavior3.IsActive)
					{
						componentBehavior3.IsActive = true;
					}
				}
				else if (componentBehavior3.IsActive)
				{
					componentBehavior3.IsActive = false;
				}
			}
		}

        // Token: 0x06000B4D RID: 2893 RVA: 0x00054590 File Offset: 0x00052790
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			foreach (ComponentBehavior item in base.Entity.FindComponents<ComponentBehavior>())
			{
				this.m_behaviors.Add(item);
			}
		}

		// Token: 0x04000643 RID: 1603
		public ComponentCreature m_componentCreature;

		// Token: 0x04000644 RID: 1604
		public List<ComponentBehavior> m_behaviors = new List<ComponentBehavior>();

		// Token: 0x04000645 RID: 1605
		public static bool ShowAIBehavior;
	}
}
