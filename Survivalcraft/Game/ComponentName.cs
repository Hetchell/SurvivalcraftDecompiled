using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F7 RID: 503
	public class ComponentName : Component
	{
		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000ECF RID: 3791 RVA: 0x00071B11 File Offset: 0x0006FD11
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

        // Token: 0x06000ED0 RID: 3792 RVA: 0x00071B19 File Offset: 0x0006FD19
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_name = valuesDictionary.GetValue<string>("Name");
		}

		// Token: 0x04000965 RID: 2405
		public string m_name;
	}
}
