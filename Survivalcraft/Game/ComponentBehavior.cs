using System;
using GameEntitySystem;

namespace Game
{
	// Token: 0x020001C0 RID: 448
	public abstract class ComponentBehavior : Component
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000B46 RID: 2886
		public abstract float ImportanceLevel { get; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x00054486 File Offset: 0x00052686
		// (set) Token: 0x06000B48 RID: 2888 RVA: 0x0005448E File Offset: 0x0005268E
		public virtual bool IsActive { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x00054497 File Offset: 0x00052697
		public virtual string DebugInfo
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
