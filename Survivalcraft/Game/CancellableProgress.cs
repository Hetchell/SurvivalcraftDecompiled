using System;
using System.Threading;

namespace Game
{
	// Token: 0x02000237 RID: 567
	public class CancellableProgress : Progress
	{
		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06001174 RID: 4468 RVA: 0x0008761C File Offset: 0x0008581C
		// (remove) Token: 0x06001175 RID: 4469 RVA: 0x00087654 File Offset: 0x00085854
		public event Action Cancelled;

		// Token: 0x06001176 RID: 4470 RVA: 0x00087689 File Offset: 0x00085889
		public CancellableProgress()
		{
			this.CancellationToken = this.CancellationTokenSource.Token;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x000876AD File Offset: 0x000858AD
		public void Cancel()
		{
			this.CancellationTokenSource.Cancel();
			Action cancelled = this.Cancelled;
			if (cancelled == null)
			{
				return;
			}
			cancelled();
		}

		// Token: 0x04000BA3 RID: 2979
		public readonly CancellationToken CancellationToken;

		// Token: 0x04000BA4 RID: 2980
		public readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
	}
}
