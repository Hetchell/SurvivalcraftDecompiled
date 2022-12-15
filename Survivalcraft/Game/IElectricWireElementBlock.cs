using System;

namespace Game
{
	// Token: 0x02000081 RID: 129
	public interface IElectricWireElementBlock : IElectricElementBlock
	{
		// Token: 0x060002BE RID: 702
		int GetConnectedWireFacesMask(int value, int face);
	}
}
