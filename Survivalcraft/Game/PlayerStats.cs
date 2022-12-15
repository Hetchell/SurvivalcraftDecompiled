using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020002D1 RID: 721
	public class PlayerStats
	{
		// Token: 0x1700030F RID: 783
		// (get) Token: 0x0600146F RID: 5231 RVA: 0x0009E83A File Offset: 0x0009CA3A
		public IEnumerable<FieldInfo> Stats
		{
			get
			{
				foreach (FieldInfo fieldInfo in from f in typeof(PlayerStats).GetRuntimeFields()
				where f.GetCustomAttribute<PlayerStats.StatAttribute>() != null
				select f)
				{
					yield return fieldInfo;
				}
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06001470 RID: 5232 RVA: 0x0009E843 File Offset: 0x0009CA43
		public ReadOnlyList<PlayerStats.DeathRecord> DeathRecords
		{
			get
			{
				return new ReadOnlyList<PlayerStats.DeathRecord>(this.m_deathRecords);
			}
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0009E850 File Offset: 0x0009CA50
		public void AddDeathRecord(PlayerStats.DeathRecord deathRecord)
		{
			this.m_deathRecords.Add(deathRecord);
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0009E860 File Offset: 0x0009CA60
		public void Load(ValuesDictionary valuesDictionary)
		{
			foreach (FieldInfo fieldInfo in this.Stats)
			{
				if (valuesDictionary.ContainsKey(fieldInfo.Name))
				{
					object value = valuesDictionary.GetValue<object>(fieldInfo.Name);
					fieldInfo.SetValue(this, value);
				}
			}
			if (!string.IsNullOrEmpty(this.DeathRecordsString))
			{
				foreach (string s in this.DeathRecordsString.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					PlayerStats.DeathRecord item = default(PlayerStats.DeathRecord);
					item.Load(s);
					this.m_deathRecords.Add(item);
				}
			}
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x0009E924 File Offset: 0x0009CB24
		public void Save(ValuesDictionary valuesDictionary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PlayerStats.DeathRecord deathRecord in this.m_deathRecords)
			{
				stringBuilder.Append(deathRecord.Save());
				stringBuilder.Append(';');
			}
			this.DeathRecordsString = stringBuilder.ToString();
			foreach (FieldInfo fieldInfo in this.Stats)
			{
				object value = fieldInfo.GetValue(this);
				valuesDictionary.SetValue<object>(fieldInfo.Name, value);
			}
		}

		// Token: 0x04000E4B RID: 3659
		public List<PlayerStats.DeathRecord> m_deathRecords = new List<PlayerStats.DeathRecord>();

		// Token: 0x04000E4C RID: 3660
		[PlayerStats.StatAttribute]
		public double DistanceTravelled;

		// Token: 0x04000E4D RID: 3661
		[PlayerStats.StatAttribute]
		public double DistanceWalked;

		// Token: 0x04000E4E RID: 3662
		[PlayerStats.StatAttribute]
		public double DistanceFallen;

		// Token: 0x04000E4F RID: 3663
		[PlayerStats.StatAttribute]
		public double DistanceClimbed;

		// Token: 0x04000E50 RID: 3664
		[PlayerStats.StatAttribute]
		public double DistanceFlown;

		// Token: 0x04000E51 RID: 3665
		[PlayerStats.StatAttribute]
		public double DistanceSwam;

		// Token: 0x04000E52 RID: 3666
		[PlayerStats.StatAttribute]
		public double DistanceRidden;

		// Token: 0x04000E53 RID: 3667
		[PlayerStats.StatAttribute]
		public double LowestAltitude = double.PositiveInfinity;

		// Token: 0x04000E54 RID: 3668
		[PlayerStats.StatAttribute]
		public double HighestAltitude = double.NegativeInfinity;

		// Token: 0x04000E55 RID: 3669
		[PlayerStats.StatAttribute]
		public double DeepestDive;

		// Token: 0x04000E56 RID: 3670
		[PlayerStats.StatAttribute]
		public long Jumps;

		// Token: 0x04000E57 RID: 3671
		[PlayerStats.StatAttribute]
		public long BlocksDug;

		// Token: 0x04000E58 RID: 3672
		[PlayerStats.StatAttribute]
		public long BlocksPlaced;

		// Token: 0x04000E59 RID: 3673
		[PlayerStats.StatAttribute]
		public long BlocksInteracted;

		// Token: 0x04000E5A RID: 3674
		[PlayerStats.StatAttribute]
		public long PlayerKills;

		// Token: 0x04000E5B RID: 3675
		[PlayerStats.StatAttribute]
		public long LandCreatureKills;

		// Token: 0x04000E5C RID: 3676
		[PlayerStats.StatAttribute]
		public long WaterCreatureKills;

		// Token: 0x04000E5D RID: 3677
		[PlayerStats.StatAttribute]
		public long AirCreatureKills;

		// Token: 0x04000E5E RID: 3678
		[PlayerStats.StatAttribute]
		public long MeleeAttacks;

		// Token: 0x04000E5F RID: 3679
		[PlayerStats.StatAttribute]
		public long MeleeHits;

		// Token: 0x04000E60 RID: 3680
		[PlayerStats.StatAttribute]
		public long RangedAttacks;

		// Token: 0x04000E61 RID: 3681
		[PlayerStats.StatAttribute]
		public long RangedHits;

		// Token: 0x04000E62 RID: 3682
		[PlayerStats.StatAttribute]
		public long HitsReceived;

		// Token: 0x04000E63 RID: 3683
		[PlayerStats.StatAttribute]
		public long StruckByLightning;

		// Token: 0x04000E64 RID: 3684
		[PlayerStats.StatAttribute]
		public double TotalHealthLost;

		// Token: 0x04000E65 RID: 3685
		[PlayerStats.StatAttribute]
		public long FoodItemsEaten;

		// Token: 0x04000E66 RID: 3686
		[PlayerStats.StatAttribute]
		public long TimesWasSick;

		// Token: 0x04000E67 RID: 3687
		[PlayerStats.StatAttribute]
		public long TimesHadFlu;

		// Token: 0x04000E68 RID: 3688
		[PlayerStats.StatAttribute]
		public long TimesPuked;

		// Token: 0x04000E69 RID: 3689
		[PlayerStats.StatAttribute]
		public long TimesWentToSleep;

		// Token: 0x04000E6A RID: 3690
		[PlayerStats.StatAttribute]
		public double TimeSlept;

		// Token: 0x04000E6B RID: 3691
		[PlayerStats.StatAttribute]
		public long ItemsCrafted;

		// Token: 0x04000E6C RID: 3692
		[PlayerStats.StatAttribute]
		public long FurnitureItemsMade;

		// Token: 0x04000E6D RID: 3693
		[PlayerStats.StatAttribute]
		public GameMode EasiestModeUsed = (GameMode)2147483647;

		// Token: 0x04000E6E RID: 3694
		[PlayerStats.StatAttribute]
		public float HighestLevel;

		// Token: 0x04000E6F RID: 3695
		[PlayerStats.StatAttribute]
		public string DeathRecordsString;

		// Token: 0x020004CA RID: 1226
		public class StatAttribute : Attribute
		{
		}

		// Token: 0x020004CB RID: 1227
		public struct DeathRecord
		{
			// Token: 0x06002018 RID: 8216 RVA: 0x000E3A54 File Offset: 0x000E1C54
			public void Load(string s)
			{
				string[] array = s.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 5)
				{
					throw new InvalidOperationException("Invalid death record.");
				}
				this.Day = double.Parse(array[0], CultureInfo.InvariantCulture);
				this.Location.X = float.Parse(array[1], CultureInfo.InvariantCulture);
				this.Location.Y = float.Parse(array[2], CultureInfo.InvariantCulture);
				this.Location.Z = float.Parse(array[3], CultureInfo.InvariantCulture);
				this.Cause = array[4];
			}

			// Token: 0x06002019 RID: 8217 RVA: 0x000E3AEC File Offset: 0x000E1CEC
			public string Save()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.Day.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.X.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.Y.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.Z.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Cause);
				return stringBuilder.ToString();
			}

			// Token: 0x040017A1 RID: 6049
			public double Day;

			// Token: 0x040017A2 RID: 6050
			public Vector3 Location;

			// Token: 0x040017A3 RID: 6051
			public string Cause;
		}
	}
}
