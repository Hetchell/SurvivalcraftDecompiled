using System;

namespace Game
{
	// Token: 0x02000085 RID: 133
	public interface IPaintableBlock
	{
		// Token: 0x060002CF RID: 719
		int? GetPaintColor(int value);

		// Token: 0x060002D0 RID: 720
		int Paint(SubsystemTerrain subsystemTerrain, int value, int? color);
	}
}
