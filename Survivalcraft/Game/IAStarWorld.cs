using System;
using Engine;

namespace Game
{
	// Token: 0x02000291 RID: 657
	public interface IAStarWorld<T>
	{
		// Token: 0x06001335 RID: 4917
		float Cost(T p1, T p2);

		// Token: 0x06001336 RID: 4918
		void Neighbors(T p, DynamicArray<T> neighbors);

		// Token: 0x06001337 RID: 4919
		float Heuristic(T p1, T p2);

		// Token: 0x06001338 RID: 4920
		bool IsGoal(T p);
	}
}
