using System;
using Engine;

namespace Game
{
	// Token: 0x02000323 RID: 803
	public struct TouchInput
	{
		// Token: 0x040010B9 RID: 4281
		public TouchInputType InputType;

		// Token: 0x040010BA RID: 4282
		public Vector2 Position;

		// Token: 0x040010BB RID: 4283
		public Vector2 Move;

		// Token: 0x040010BC RID: 4284
		public Vector2 TotalMove;

		// Token: 0x040010BD RID: 4285
		public Vector2 TotalMoveLimited;

		// Token: 0x040010BE RID: 4286
		public float Duration;

		// Token: 0x040010BF RID: 4287
		public int DurationFrames;
	}
}
