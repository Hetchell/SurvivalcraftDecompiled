using System;

namespace Game
{
	// Token: 0x0200032B RID: 811
	public enum UpdateOrder
	{
		// Token: 0x040010D3 RID: 4307
		Reset = -100,
		// Token: 0x040010D4 RID: 4308
		SubsystemPlayers = -20,
		// Token: 0x040010D5 RID: 4309
		Input = -10,
		// Token: 0x040010D6 RID: 4310
		Default = 0,
		// Token: 0x040010D7 RID: 4311
		Locomotion,
		// Token: 0x040010D8 RID: 4312
		Body,
		// Token: 0x040010D9 RID: 4313
		CreatureModels = 10,
		// Token: 0x040010DA RID: 4314
		FirstPersonModels = 20,
		// Token: 0x040010DB RID: 4315
		BlocksScanner = 99,
		// Token: 0x040010DC RID: 4316
		Terrain,
		// Token: 0x040010DD RID: 4317
		Views = 200,
		// Token: 0x040010DE RID: 4318
		BlockHighlight
	}
}
