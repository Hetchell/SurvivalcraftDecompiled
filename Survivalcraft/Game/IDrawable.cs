using System;

namespace Game
{
	// Token: 0x02000292 RID: 658
	public interface IDrawable
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001339 RID: 4921
		int[] DrawOrders { get; }

		// Token: 0x0600133A RID: 4922
		void Draw(Camera camera, int drawOrder);
	}
}
