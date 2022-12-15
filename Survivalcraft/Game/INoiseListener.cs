using System;
using Engine;

namespace Game
{
	// Token: 0x02000296 RID: 662
	public interface INoiseListener
	{
		// Token: 0x06001358 RID: 4952
		void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness);
	}
}
