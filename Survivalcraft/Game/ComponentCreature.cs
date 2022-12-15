using System;
using Engine;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CE RID: 462
	public class ComponentCreature : Component
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000C33 RID: 3123 RVA: 0x0005C9AE File Offset: 0x0005ABAE
		// (set) Token: 0x06000C34 RID: 3124 RVA: 0x0005C9B6 File Offset: 0x0005ABB6
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x0005C9BF File Offset: 0x0005ABBF
		// (set) Token: 0x06000C36 RID: 3126 RVA: 0x0005C9C7 File Offset: 0x0005ABC7
		public ComponentHealth ComponentHealth { get; set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000C37 RID: 3127 RVA: 0x0005C9D0 File Offset: 0x0005ABD0
		// (set) Token: 0x06000C38 RID: 3128 RVA: 0x0005C9D8 File Offset: 0x0005ABD8
		public ComponentSpawn ComponentSpawn { get; set; }

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000C39 RID: 3129 RVA: 0x0005C9E1 File Offset: 0x0005ABE1
		// (set) Token: 0x06000C3A RID: 3130 RVA: 0x0005C9E9 File Offset: 0x0005ABE9
		public ComponentCreatureModel ComponentCreatureModel { get; set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000C3B RID: 3131 RVA: 0x0005C9F2 File Offset: 0x0005ABF2
		// (set) Token: 0x06000C3C RID: 3132 RVA: 0x0005C9FA File Offset: 0x0005ABFA
		public ComponentCreatureSounds ComponentCreatureSounds { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000C3D RID: 3133 RVA: 0x0005CA03 File Offset: 0x0005AC03
		// (set) Token: 0x06000C3E RID: 3134 RVA: 0x0005CA0B File Offset: 0x0005AC0B
		public ComponentLocomotion ComponentLocomotion { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000C3F RID: 3135 RVA: 0x0005CA14 File Offset: 0x0005AC14
		public PlayerStats PlayerStats
		{
			get
			{
				ComponentPlayer componentPlayer = this as ComponentPlayer;
				if (componentPlayer != null)
				{
					return this.m_subsystemPlayerStats.GetPlayerStats(componentPlayer.PlayerData.PlayerIndex);
				}
				return null;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000C40 RID: 3136 RVA: 0x0005CA43 File Offset: 0x0005AC43
		// (set) Token: 0x06000C41 RID: 3137 RVA: 0x0005CA4B File Offset: 0x0005AC4B
		public bool ConstantSpawn { get; set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x0005CA54 File Offset: 0x0005AC54
		// (set) Token: 0x06000C43 RID: 3139 RVA: 0x0005CA5C File Offset: 0x0005AC5C
		public CreatureCategory Category { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x0005CA65 File Offset: 0x0005AC65
		// (set) Token: 0x06000C45 RID: 3141 RVA: 0x0005CA6D File Offset: 0x0005AC6D
		public string DisplayName { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x0005CA76 File Offset: 0x0005AC76
		public ReadOnlyList<string> KillVerbs
		{
			get
			{
				return new ReadOnlyList<string>(this.m_killVerbs);
			}
		}

        // Token: 0x06000C47 RID: 3143 RVA: 0x0005CA84 File Offset: 0x0005AC84
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.ComponentHealth = base.Entity.FindComponent<ComponentHealth>(true);
			this.ComponentSpawn = base.Entity.FindComponent<ComponentSpawn>(true);
			this.ComponentCreatureSounds = base.Entity.FindComponent<ComponentCreatureSounds>(true);
			this.ComponentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(true);
			this.ComponentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(true);
			this.m_subsystemPlayerStats = base.Project.FindSubsystem<SubsystemPlayerStats>(true);
			this.ConstantSpawn = valuesDictionary.GetValue<bool>("ConstantSpawn");
			this.Category = valuesDictionary.GetValue<CreatureCategory>("Category");
			this.DisplayName = valuesDictionary.GetValue<string>("DisplayName");
			if (this.DisplayName.StartsWith("[") && this.DisplayName.EndsWith("]"))
			{
				string[] array = this.DisplayName.Substring(1, this.DisplayName.Length - 2).Split(new string[]
				{
					":"
				}, StringSplitOptions.RemoveEmptyEntries);
				this.DisplayName = LanguageControl.GetDatabase("DisplayName", array[1]);
			}
			this.m_killVerbs = HumanReadableConverter.ValuesListFromString<string>(',', valuesDictionary.GetValue<string>("KillVerbs"));
			if (this.m_killVerbs.Length == 0)
			{
				throw new InvalidOperationException("Must have at least one KillVerb");
			}
			if (!MathUtils.IsPowerOf2((long)this.Category))
			{
				throw new InvalidOperationException("A single category must be assigned for creature.");
			}
		}

        // Token: 0x06000C48 RID: 3144 RVA: 0x0005CBEC File Offset: 0x0005ADEC
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("ConstantSpawn", this.ConstantSpawn);
		}

		// Token: 0x040006F6 RID: 1782
		public SubsystemPlayerStats m_subsystemPlayerStats;

		// Token: 0x040006F7 RID: 1783
		public string[] m_killVerbs;
	}
}
