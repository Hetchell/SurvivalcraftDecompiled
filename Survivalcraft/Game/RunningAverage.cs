using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x020002E8 RID: 744
	public class RunningAverage
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060014E3 RID: 5347 RVA: 0x000A1EFE File Offset: 0x000A00FE
		public float Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x000A1F06 File Offset: 0x000A0106
		public RunningAverage(float period)
		{
			this.m_period = (long)(period * (float)Stopwatch.Frequency);
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x000A1F20 File Offset: 0x000A0120
		public void AddSample(float sample)
		{
			this.m_sumValues += sample;
			this.m_countValues++;
			long timestamp = Stopwatch.GetTimestamp();
			if (timestamp >= this.m_startTicks + this.m_period)
			{
				this.m_value = this.m_sumValues / (float)this.m_countValues;
				this.m_sumValues = 0f;
				this.m_countValues = 0;
				this.m_startTicks = timestamp;
			}
		}

		// Token: 0x04000EE4 RID: 3812
		public long m_startTicks;

		// Token: 0x04000EE5 RID: 3813
		public long m_period;

		// Token: 0x04000EE6 RID: 3814
		public float m_sumValues;

		// Token: 0x04000EE7 RID: 3815
		public int m_countValues;

		// Token: 0x04000EE8 RID: 3816
		public float m_value;
	}
}
