using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000221 RID: 545
	public class BestiaryCreatureInfo
	{
		// Token: 0x04000AE8 RID: 2792
		public int Order;

		// Token: 0x04000AE9 RID: 2793
		public string DisplayName;

		// Token: 0x04000AEA RID: 2794
		public string Description;

		// Token: 0x04000AEB RID: 2795
		public string ModelName;

		// Token: 0x04000AEC RID: 2796
		public string TextureOverride;

		// Token: 0x04000AED RID: 2797
		public float Mass;

		// Token: 0x04000AEE RID: 2798
		public float AttackResilience;

		// Token: 0x04000AEF RID: 2799
		public float AttackPower;

		// Token: 0x04000AF0 RID: 2800
		public float MovementSpeed;

		// Token: 0x04000AF1 RID: 2801
		public float JumpHeight;

		// Token: 0x04000AF2 RID: 2802
		public bool IsHerding;

		// Token: 0x04000AF3 RID: 2803
		public bool CanBeRidden;

		// Token: 0x04000AF4 RID: 2804
		public bool HasSpawnerEgg;

		// Token: 0x04000AF5 RID: 2805
		public List<ComponentLoot.Loot> Loot;
	}
}
