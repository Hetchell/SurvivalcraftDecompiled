using System;
using System.Collections.Generic;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000194 RID: 404
	public class SubsystemNames : Subsystem
	{
		// Token: 0x06000965 RID: 2405 RVA: 0x00042152 File Offset: 0x00040352
		public Component FindComponentByName(string name, Type componentType, string componentName)
		{
			Entity entity = this.FindEntityByName(name);
			if (entity == null)
			{
				return null;
			}
			return entity.FindComponent(componentType, componentName, false);
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0004216C File Offset: 0x0004036C
		public T FindComponentByName<T>(string name, string componentName) where T : Component
		{
			Entity entity = this.FindEntityByName(name);
			if (entity == null)
			{
				return default(T);
			}
			return entity.FindComponent<T>(componentName, false);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00042198 File Offset: 0x00040398
		public Entity FindEntityByName(string name)
		{
			ComponentName componentName;
			this.m_componentsByName.TryGetValue(name, out componentName);
			if (componentName == null)
			{
				return null;
			}
			return componentName.Entity;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x000421C0 File Offset: 0x000403C0
		public static string GetEntityName(Entity entity)
		{
			ComponentName componentName = entity.FindComponent<ComponentName>();
			if (componentName != null)
			{
				return componentName.Name;
			}
			return string.Empty;
		}

        // Token: 0x06000969 RID: 2409 RVA: 0x000421E4 File Offset: 0x000403E4
        public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentName componentName in entity.FindComponents<ComponentName>())
			{
				this.m_componentsByName.Add(componentName.Name, componentName);
			}
		}

        // Token: 0x0600096A RID: 2410 RVA: 0x00042248 File Offset: 0x00040448
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentName componentName in entity.FindComponents<ComponentName>())
			{
				this.m_componentsByName.Remove(componentName.Name);
			}
		}

		// Token: 0x040004F8 RID: 1272
		public Dictionary<string, ComponentName> m_componentsByName = new Dictionary<string, ComponentName>();
	}
}
