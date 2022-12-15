using System;
using Engine;

namespace Game
{
	// Token: 0x020002D9 RID: 729
	public static class ProgressManager
	{
		// Token: 0x17000316 RID: 790
		// (get) Token: 0x0600149D RID: 5277 RVA: 0x000A02F0 File Offset: 0x0009E4F0
		// (set) Token: 0x0600149E RID: 5278 RVA: 0x000A02F7 File Offset: 0x0009E4F7
		public static string OperationName { get; set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x0600149F RID: 5279 RVA: 0x000A02FF File Offset: 0x0009E4FF
		// (set) Token: 0x060014A0 RID: 5280 RVA: 0x000A0306 File Offset: 0x0009E506
		public static float Progress { get; set; }

		// Token: 0x060014A1 RID: 5281 RVA: 0x000A030E File Offset: 0x0009E50E
		public static void UpdateProgress(string operationName, float progress)
		{
			ProgressManager.OperationName = operationName;
			ProgressManager.Progress = MathUtils.Saturate(progress);
		}
	}
}
