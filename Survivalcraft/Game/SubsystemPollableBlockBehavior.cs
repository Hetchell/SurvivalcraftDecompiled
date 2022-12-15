using System;

namespace Game
{
	// Token: 0x0200019E RID: 414
	public abstract class SubsystemPollableBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x060009D8 RID: 2520
		public abstract void OnPoll(int value, int x, int y, int z, int pollPass);
	}
}
