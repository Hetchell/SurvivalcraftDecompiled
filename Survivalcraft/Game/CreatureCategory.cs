using System;

namespace Game
{
	// Token: 0x02000246 RID: 582
	[Flags]
	public enum CreatureCategory
	{
		// Token: 0x04000BF1 RID: 3057
		LandPredator = 1,
		// Token: 0x04000BF2 RID: 3058
		LandOther = 2,
		// Token: 0x04000BF3 RID: 3059
		WaterPredator = 4,
		// Token: 0x04000BF4 RID: 3060
		WaterOther = 8,
		// Token: 0x04000BF5 RID: 3061
		Bird = 16
	}
}
