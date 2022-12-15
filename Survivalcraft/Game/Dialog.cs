using System;
using Engine;

namespace Game
{
	// Token: 0x0200024F RID: 591
	public class Dialog : CanvasWidget
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x0008A118 File Offset: 0x00088318
		public Dialog()
		{
			this.IsHitTestVisible = true;
			base.Size = new Vector2(float.PositiveInfinity);
		}
	}
}
