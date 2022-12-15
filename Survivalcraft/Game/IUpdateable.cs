using System;

namespace Game
{
	// Token: 0x0200029D RID: 669
	public interface IUpdateable
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600137B RID: 4987
		UpdateOrder UpdateOrder { get; }

		// Token: 0x0600137C RID: 4988
		void Update(float dt);
	}
}
