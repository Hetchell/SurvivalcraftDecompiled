using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020D RID: 525
	public class ComponentUdder : Component
	{
		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06001026 RID: 4134 RVA: 0x0007A887 File Offset: 0x00078A87
		public bool HasMilk
		{
			get
			{
				return this.m_lastMilkingTime < 0.0 || this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_lastMilkingTime >= (double)this.m_milkRegenerationTime;
			}
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0007A8BC File Offset: 0x00078ABC
		public bool Milk(ComponentMiner milker)
		{
			if (milker != null)
			{
				ComponentHerdBehavior componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>();
				if (componentHerdBehavior != null)
				{
					componentHerdBehavior.CallNearbyCreaturesHelp(milker.ComponentCreature, 20f, 20f, true);
				}
			}
			if (this.HasMilk)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				this.m_lastMilkingTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
				return true;
			}
			this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
			return false;
		}

        // Token: 0x06001028 RID: 4136 RVA: 0x0007A930 File Offset: 0x00078B30
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_milkRegenerationTime = valuesDictionary.GetValue<float>("MilkRegenerationTime");
			this.m_lastMilkingTime = valuesDictionary.GetValue<double>("LastMilkingTime");
		}

        // Token: 0x06001029 RID: 4137 RVA: 0x0007A983 File Offset: 0x00078B83
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("LastMilkingTime", this.m_lastMilkingTime);
		}

		// Token: 0x04000A84 RID: 2692
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A85 RID: 2693
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A86 RID: 2694
		public float m_milkRegenerationTime;

		// Token: 0x04000A87 RID: 2695
		public double m_lastMilkingTime;
	}
}
