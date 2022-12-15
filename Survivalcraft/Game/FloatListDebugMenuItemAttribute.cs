using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x02000275 RID: 629
	[Conditional("DEBUG")]
	public class FloatListDebugMenuItemAttribute : DebugMenuItemAttribute
	{
		// Token: 0x06001291 RID: 4753 RVA: 0x0008FB28 File Offset: 0x0008DD28
		public FloatListDebugMenuItemAttribute(float[] items, int precision, string unit) : base(0.0)
		{
			this.Items = items;
			this.Precision = precision;
			this.Unit = unit;
		}

		// Token: 0x04000CC5 RID: 3269
		public float[] Items;
	}
}
