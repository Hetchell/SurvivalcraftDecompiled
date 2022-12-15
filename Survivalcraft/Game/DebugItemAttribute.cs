using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x0200024A RID: 586
	[Conditional("DEBUG")]
	public class DebugItemAttribute : Attribute
	{
		// Token: 0x04000BFE RID: 3070
		public int Precision = 3;

		// Token: 0x04000BFF RID: 3071
		public string Unit;
	}
}
