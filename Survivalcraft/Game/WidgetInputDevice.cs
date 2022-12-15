using System;

namespace Game
{
	// Token: 0x020003A8 RID: 936
	[Flags]
	public enum WidgetInputDevice
	{
		// Token: 0x0400137B RID: 4987
		None = 0,
		// Token: 0x0400137C RID: 4988
		Keyboard = 1,
		// Token: 0x0400137D RID: 4989
		Mouse = 2,
		// Token: 0x0400137E RID: 4990
		Touch = 4,
		// Token: 0x0400137F RID: 4991
		GamePad1 = 8,
		// Token: 0x04001380 RID: 4992
		GamePad2 = 16,
		// Token: 0x04001381 RID: 4993
		GamePad3 = 32,
		// Token: 0x04001382 RID: 4994
		GamePad4 = 64,
		// Token: 0x04001383 RID: 4995
		Gamepads = 120,
		// Token: 0x04001384 RID: 4996
		VrControllers = 128,
		// Token: 0x04001385 RID: 4997
		All = 255
	}
}
