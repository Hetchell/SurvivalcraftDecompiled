using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200016A RID: 362
	public class SubsystemCarpetBlockBehavior : SubsystemPollableBlockBehavior
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0002DFFE File Offset: 0x0002C1FE
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

        // Token: 0x06000736 RID: 1846 RVA: 0x0002E006 File Offset: 0x0002C206
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0002E024 File Offset: 0x0002C224
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0002E068 File Offset: 0x0002C268
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_random.Float(0f, 1f) < 0.25f)
			{
				PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
				if (precipitationShaftInfo.Intensity > 0f && y >= precipitationShaftInfo.YLimit - 1)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, true, false);
				}
			}
		}

		// Token: 0x04000401 RID: 1025
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000402 RID: 1026
		public Random m_random = new Random();
	}
}
