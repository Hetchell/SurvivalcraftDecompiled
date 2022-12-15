using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F1 RID: 497
	public class ComponentLookAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000E68 RID: 3688 RVA: 0x0006F657 File Offset: 0x0006D857
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0006F65A File Offset: 0x0006D85A
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0006F662 File Offset: 0x0006D862
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000E6B RID: 3691 RVA: 0x0006F670 File Offset: 0x0006D870
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = this.m_random.Float(0f, 1f);
			}, delegate
			{
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_random.Float(0f, 1f) < 0.05f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_importanceLevel = this.m_random.Float(1f, 5f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("LookAround");
				}
			}, null);
			this.m_stateMachine.AddState("LookAround", delegate
			{
				this.m_lookAroundTime = this.m_random.Float(8f, 15f);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_lookAroundTime <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
				this.m_lookAroundTime -= this.m_subsystemTime.GameTimeDelta;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x04000922 RID: 2338
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000923 RID: 2339
		public ComponentCreature m_componentCreature;

		// Token: 0x04000924 RID: 2340
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000925 RID: 2341
		public Random m_random = new Random();

		// Token: 0x04000926 RID: 2342
		public float m_lookAroundTime;

		// Token: 0x04000927 RID: 2343
		public float m_importanceLevel;
	}
}
