using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200009B RID: 155
	public class LargeDryBushBlock : CrossBlock
	{
		// Token: 0x060002FE RID: 766 RVA: 0x00011774 File Offset: 0x0000F974
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			dropValues.Add(new BlockDropValue
			{
				Value = 23,
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x04000160 RID: 352
		public const int Index = 99;
	}
}
