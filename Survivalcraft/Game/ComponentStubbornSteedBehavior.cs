using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000209 RID: 521
	public class ComponentStubbornSteedBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x000798D7 File Offset: 0x00077AD7
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x000798DA File Offset: 0x00077ADA
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x000798E4 File Offset: 0x00077AE4
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)this.m_periodicEventOffset))
			{
				if (this.m_subsystemGameInfo.TotalElapsedGameTime < this.m_stubbornEndTime && this.m_componentEatPickableBehavior.Satiation <= 0f && this.m_componentMount.Rider != null)
				{
					this.m_importanceLevel = 210f;
					return;
				}
				this.m_importanceLevel = 0f;
			}
		}

        // Token: 0x06000FFC RID: 4092 RVA: 0x0007997C File Offset: 0x00077B7C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_componentSteedBehavior = base.Entity.FindComponent<ComponentSteedBehavior>(true);
			this.m_componentEatPickableBehavior = base.Entity.FindComponent<ComponentEatPickableBehavior>(true);
			this.m_stubbornProbability = valuesDictionary.GetValue<float>("StubbornProbability");
			this.m_stubbornEndTime = valuesDictionary.GetValue<double>("StubbornEndTime");
			this.m_periodicEventOffset = this.m_random.Float(0f, 100f);
			this.m_isSaddled = base.Entity.ValuesDictionary.DatabaseObject.Name.EndsWith("_Saddled");
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)this.m_periodicEventOffset) && this.m_componentMount.Rider != null && this.m_random.Float(0f, 1f) < this.m_stubbornProbability && (!this.m_isSaddled || this.m_componentEatPickableBehavior.Satiation <= 0f))
				{
					this.m_stubbornEndTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)this.m_random.Float(60f, 120f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Stubborn");
				}
			}, null);
			this.m_stateMachine.AddState("Stubborn", null, delegate
			{
				if (this.m_componentSteedBehavior.WasOrderIssued)
				{
					this.m_componentCreature.ComponentCreatureModel.HeadShakeOrder = this.m_random.Float(0.6f, 1f);
					this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

        // Token: 0x06000FFD RID: 4093 RVA: 0x00079AA3 File Offset: 0x00077CA3
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("StubbornEndTime", this.m_stubbornEndTime);
		}

		// Token: 0x04000A5D RID: 2653
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A5E RID: 2654
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A5F RID: 2655
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A60 RID: 2656
		public ComponentMount m_componentMount;

		// Token: 0x04000A61 RID: 2657
		public ComponentSteedBehavior m_componentSteedBehavior;

		// Token: 0x04000A62 RID: 2658
		public ComponentEatPickableBehavior m_componentEatPickableBehavior;

		// Token: 0x04000A63 RID: 2659
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A64 RID: 2660
		public float m_importanceLevel;

		// Token: 0x04000A65 RID: 2661
		public bool m_isSaddled;

		// Token: 0x04000A66 RID: 2662
		public Random m_random = new Random();

		// Token: 0x04000A67 RID: 2663
		public float m_periodicEventOffset;

		// Token: 0x04000A68 RID: 2664
		public float m_stubbornProbability;

		// Token: 0x04000A69 RID: 2665
		public double m_stubbornEndTime;
	}
}
