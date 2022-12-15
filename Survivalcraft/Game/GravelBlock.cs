using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000079 RID: 121
	public class GravelBlock : CubeBlock
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0000F61C File Offset: 0x0000D81C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel < this.RequiredToolLevel)
			{
				return;
			}
			if (this.Random.Float(0f, 1f) < 0.33f)
			{
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
				return;
			}
			int num = this.Random.Int(1, 3);
			for (int i = 0; i < num; i++)
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(79),
					Count = 1
				});
			}
		}

		// Token: 0x04000128 RID: 296
		public const int Index = 6;
	}
}
