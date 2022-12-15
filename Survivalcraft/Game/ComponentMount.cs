using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F5 RID: 501
	public class ComponentMount : Component
	{
		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000EBC RID: 3772 RVA: 0x000716D0 File Offset: 0x0006F8D0
		// (set) Token: 0x06000EBD RID: 3773 RVA: 0x000716D8 File Offset: 0x0006F8D8
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000EBE RID: 3774 RVA: 0x000716E1 File Offset: 0x0006F8E1
		// (set) Token: 0x06000EBF RID: 3775 RVA: 0x000716E9 File Offset: 0x0006F8E9
		public Vector3 MountOffset { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x000716F2 File Offset: 0x0006F8F2
		// (set) Token: 0x06000EC1 RID: 3777 RVA: 0x000716FA File Offset: 0x0006F8FA
		public Vector3 DismountOffset { get; set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000EC2 RID: 3778 RVA: 0x00071704 File Offset: 0x0006F904
		public ComponentRider Rider
		{
			get
			{
				foreach (ComponentBody componentBody in this.ComponentBody.ChildBodies)
				{
					ComponentRider componentRider = componentBody.Entity.FindComponent<ComponentRider>();
					if (componentRider != null)
					{
						return componentRider;
					}
				}
				return null;
			}
		}

        // Token: 0x06000EC3 RID: 3779 RVA: 0x0007176C File Offset: 0x0006F96C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.MountOffset = valuesDictionary.GetValue<Vector3>("MountOffset");
			this.DismountOffset = valuesDictionary.GetValue<Vector3>("DismountOffset");
		}
	}
}
