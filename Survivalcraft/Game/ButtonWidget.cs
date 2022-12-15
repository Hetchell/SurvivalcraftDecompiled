using System;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200036F RID: 879
	public abstract class ButtonWidget : CanvasWidget
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001914 RID: 6420
		public abstract bool IsClicked { get; }

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001915 RID: 6421
		// (set) Token: 0x06001916 RID: 6422
		public abstract bool IsChecked { get; set; }

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001917 RID: 6423
		// (set) Token: 0x06001918 RID: 6424
		public abstract bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001919 RID: 6425
		// (set) Token: 0x0600191A RID: 6426
		public abstract string Text { get; set; }

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x0600191B RID: 6427
		// (set) Token: 0x0600191C RID: 6428
		public abstract BitmapFont Font { get; set; }

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600191D RID: 6429
		// (set) Token: 0x0600191E RID: 6430
		public abstract Color Color { get; set; }
	}
}
