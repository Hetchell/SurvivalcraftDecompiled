using System;

namespace Game
{
	// Token: 0x02000290 RID: 656
	public interface IAStarStorage<T>
	{
		// Token: 0x06001332 RID: 4914
		void Clear();

		// Token: 0x06001333 RID: 4915
		object Get(T p);

		// Token: 0x06001334 RID: 4916
		void Set(T p, object data);
	}
}
