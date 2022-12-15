using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x020000CC RID: 204
	public class PumpkinBlock : BasePumpkinBlock
	{
		// Token: 0x0600041D RID: 1053 RVA: 0x00016593 File Offset: 0x00014793
		public PumpkinBlock() : base(false)
		{
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0001659C File Offset: 0x0001479C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			int data = Terrain.ExtractData(oldValue);
			if (BasePumpkinBlock.GetSize(data) == 7 && !BasePumpkinBlock.GetIsDead(data) && this.Random.Bool(0.5f))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = 248,
					Count = 1
				});
			}
		}

		// Token: 0x040001CF RID: 463
		public const int Index = 131;
	}
}
