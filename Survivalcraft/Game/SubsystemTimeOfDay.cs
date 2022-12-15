using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B1 RID: 433
	public class SubsystemTimeOfDay : Subsystem
	{
		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x0004F980 File Offset: 0x0004DB80
		public float TimeOfDay
		{
			get
			{
				if (!this.TimeOfDayEnabled)
				{
					return 0.5f;
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Changing)
				{
					return this.CalculateTimeOfDay(this.m_subsystemGameInfo.TotalElapsedGameTime);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Day)
				{
					return MathUtils.Remainder(0.5f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Night)
				{
					return MathUtils.Remainder(1f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Sunrise)
				{
					return MathUtils.Remainder(0.25f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Sunset)
				{
					return MathUtils.Remainder(0.75f, 1f);
				}
				return 0.5f;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x0004FA53 File Offset: 0x0004DC53
		public double Day
		{
			get
			{
				return this.CalculateDay(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x0004FA66 File Offset: 0x0004DC66
		// (set) Token: 0x06000ABC RID: 2748 RVA: 0x0004FA6E File Offset: 0x0004DC6E
		public double TimeOfDayOffset { get; set; }

		// Token: 0x06000ABD RID: 2749 RVA: 0x0004FA77 File Offset: 0x0004DC77
		public double CalculateDay(double totalElapsedGameTime)
		{
			return (totalElapsedGameTime + (this.TimeOfDayOffset + 0.30000001192092896) * 1200.0) / 1200.0;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0004FA9F File Offset: 0x0004DC9F
		public float CalculateTimeOfDay(double totalElapsedGameTime)
		{
			return (float)MathUtils.Remainder(totalElapsedGameTime + (this.TimeOfDayOffset + 0.30000001192092896) * 1200.0, 1200.0) / 1200f;
		}

        // Token: 0x06000ABF RID: 2751 RVA: 0x0004FAD2 File Offset: 0x0004DCD2
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.TimeOfDayOffset = valuesDictionary.GetValue<double>("TimeOfDayOffset");
		}

        // Token: 0x06000AC0 RID: 2752 RVA: 0x0004FAF7 File Offset: 0x0004DCF7
        public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<double>("TimeOfDayOffset", this.TimeOfDayOffset);
		}

		// Token: 0x040005E5 RID: 1509
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040005E6 RID: 1510
		public bool TimeOfDayEnabled = true;

		// Token: 0x040005E7 RID: 1511
		public const float DayDuration = 1200f;
	}
}
