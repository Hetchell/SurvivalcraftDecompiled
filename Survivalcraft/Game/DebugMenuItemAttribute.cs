using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x0200024C RID: 588
	[Conditional("DEBUG")]
	public class DebugMenuItemAttribute : DebugItemAttribute
	{
		// Token: 0x060011D2 RID: 4562 RVA: 0x00089F68 File Offset: 0x00088168
		public DebugMenuItemAttribute()
		{
			this.Step = 1.0;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00089F7F File Offset: 0x0008817F
		public DebugMenuItemAttribute(double step)
		{
			this.Step = step;
		}

		// Token: 0x04000C00 RID: 3072
		public double Step;
	}
}
