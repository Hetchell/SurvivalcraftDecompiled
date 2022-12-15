using System;

namespace Game
{
	// Token: 0x02000188 RID: 392
	public class SubsystemInWaterBlockBehavior : SubsystemWaterBlockBehavior
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0003DB11 File Offset: 0x0003BD11
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x0003DB1C File Offset: 0x0003BD1C
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			int level = FluidBlock.GetLevel(Terrain.ExtractData(blockValue));
			newBlockValue = Terrain.MakeBlockValue(18, 0, FluidBlock.SetLevel(0, level));
			dropValue.Value = Terrain.MakeBlockValue(Terrain.ExtractContents(blockValue));
			dropValue.Count = 1;
		}
	}
}
