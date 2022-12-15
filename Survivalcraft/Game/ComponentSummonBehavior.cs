using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020A RID: 522
	public class ComponentSummonBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x00079BD3 File Offset: 0x00077DD3
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x00079BDB File Offset: 0x00077DDB
		public ComponentBody SummonTarget { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06001003 RID: 4099 RVA: 0x00079BE4 File Offset: 0x00077DE4
		public bool IsEnabled
		{
			get
			{
				return this.m_isEnabled;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x00079BEC File Offset: 0x00077DEC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06001005 RID: 4101 RVA: 0x00079BEF File Offset: 0x00077DEF
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x00079BF7 File Offset: 0x00077DF7
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06001007 RID: 4103 RVA: 0x00079C04 File Offset: 0x00077E04
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_isEnabled = valuesDictionary.GetValue<bool>("IsEnabled");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.SummonTarget = null;
				this.m_summonedTime = 0.0;
			}, delegate
			{
				if (this.m_isEnabled && this.SummonTarget != null && this.m_summonedTime == 0.0)
				{
					this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.5, delegate
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
						this.m_importanceLevel = 270f;
						this.m_summonedTime = this.m_subsystemTime.GameTime;
					});
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("FollowTarget");
				}
			}, null);
			this.m_stateMachine.AddState("FollowTarget", delegate
			{
				this.FollowTarget(true);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.SummonTarget == null || this.m_componentPathfinding.IsStuck || this.m_subsystemTime.GameTime - this.m_summonedTime > 30.0)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_componentPathfinding.Destination == null)
				{
					if (this.m_stoppedTime < 0.0)
					{
						this.m_stoppedTime = this.m_subsystemTime.GameTime;
					}
					if (this.m_subsystemTime.GameTime - this.m_stoppedTime > 6.0)
					{
						this.m_importanceLevel = 0f;
					}
				}
				this.FollowTarget(false);
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x00079CBC File Offset: 0x00077EBC
		public void FollowTarget(bool noDelay)
		{
			if (this.SummonTarget != null && (noDelay || this.m_random.Float(0f, 1f) < 5f * this.m_subsystemTime.GameTimeDelta))
			{
				float num = Vector3.Distance(this.m_componentCreature.ComponentBody.Position, this.SummonTarget.Position);
				if (num > 4f)
				{
					Vector3 vector = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, this.SummonTarget.Position - this.m_componentCreature.ComponentBody.Position));
					vector *= 0.75f * (float)((this.GetHashCode() % 2 != 0) ? 1 : -1) * (float)(1 + this.GetHashCode() % 3);
					float speed = MathUtils.Lerp(0.4f, 1f, MathUtils.Saturate(0.25f * (num - 5f)));
					this.m_componentPathfinding.SetDestination(new Vector3?(this.SummonTarget.Position + vector), speed, 3.75f, 2000, true, false, true, null);
					this.m_stoppedTime = -1.0;
				}
			}
		}

		// Token: 0x04000A6A RID: 2666
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A6B RID: 2667
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A6C RID: 2668
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000A6D RID: 2669
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A6E RID: 2670
		public float m_importanceLevel;

		// Token: 0x04000A6F RID: 2671
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A70 RID: 2672
		public bool m_isEnabled;

		// Token: 0x04000A71 RID: 2673
		public double m_summonedTime;

		// Token: 0x04000A72 RID: 2674
		public double m_stoppedTime;
	}
}
