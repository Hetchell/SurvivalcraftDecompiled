using System;
using System.Collections.Generic;
using System.Globalization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019D RID: 413
	public class SubsystemPlayerStats : Subsystem
	{
		// Token: 0x060009D4 RID: 2516 RVA: 0x00046970 File Offset: 0x00044B70
		public PlayerStats GetPlayerStats(int playerIndex)
		{
			PlayerStats playerStats;
			if (!this.m_playerStats.TryGetValue(playerIndex, out playerStats))
			{
				playerStats = new PlayerStats();
				this.m_playerStats.Add(playerIndex, playerStats);
			}
			return playerStats;
		}

        // Token: 0x060009D5 RID: 2517 RVA: 0x000469A4 File Offset: 0x00044BA4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Stats"))
			{
				PlayerStats playerStats = new PlayerStats();
				playerStats.Load((ValuesDictionary)keyValuePair.Value);
				this.m_playerStats.Add(int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture), playerStats);
			}
		}

        // Token: 0x060009D6 RID: 2518 RVA: 0x00046A24 File Offset: 0x00044C24
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Stats", valuesDictionary2);
			foreach (KeyValuePair<int, PlayerStats> keyValuePair in this.m_playerStats)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(keyValuePair.Key.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				keyValuePair.Value.Save(valuesDictionary3);
			}
		}

		// Token: 0x04000535 RID: 1333
		public Dictionary<int, PlayerStats> m_playerStats = new Dictionary<int, PlayerStats>();
	}
}
