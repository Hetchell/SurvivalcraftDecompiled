using System;
using Engine;

namespace Game
{
	// Token: 0x0200029C RID: 668
	public interface ITrailParticleSystem
	{
		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06001377 RID: 4983
		// (set) Token: 0x06001378 RID: 4984
		Vector3 Position { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06001379 RID: 4985
		// (set) Token: 0x0600137A RID: 4986
		bool IsStopped { get; set; }
	}
}
