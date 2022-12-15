using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BE RID: 446
	public class ComponentAvoidFireBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x00053862 File Offset: 0x00051A62
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000B2E RID: 2862 RVA: 0x00053865 File Offset: 0x00051A65
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0005386D File Offset: 0x00051A6D
		public override string DebugInfo
		{
			get
			{
				if (this.m_ignoreFireUntil >= this.m_subsystemTime.GameTime)
				{
					return string.Empty;
				}
				return string.Format("ifu={0:0}", this.m_ignoreFireUntil - this.m_subsystemTime.GameTime);
			}
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x000538A9 File Offset: 0x00051AA9
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000B31 RID: 2865 RVA: 0x000538B8 File Offset: 0x00051AB8
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_subsystemCampfireBlockBehavior = base.Project.FindSubsystem<SubsystemCampfireBlockBehavior>(true);
			this.m_dayRange = valuesDictionary.GetValue<float>("DayRange");
			this.m_nightRange = valuesDictionary.GetValue<float>("NightRange");
			this.m_periodicEventOffset = this.m_random.Float(0f, 10f);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_target = null;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo((this.m_importanceLevel < 10f) ? "Circle" : "Move");
					return;
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)this.m_periodicEventOffset))
				{
					float num;
					this.m_target = this.FindTarget(out num);
					if (this.m_target != null)
					{
						if (this.m_random.Float(0f, 1f) < 0.015f)
						{
							this.m_ignoreFireUntil = this.m_subsystemTime.GameTime + 20.0;
						}
						Vector3.Distance(this.m_target.Value, this.m_componentCreature.ComponentBody.Position);
						if (this.m_subsystemTime.GameTime < this.m_ignoreFireUntil)
						{
							this.m_importanceLevel = 0f;
							return;
						}
						this.m_importanceLevel = ((num > 0.5f) ? 250f : this.m_random.Float(1f, 5f));
						return;
					}
					else
					{
						this.m_importanceLevel = 0f;
					}
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_target != null)
				{
					Vector3 vector = Vector3.Normalize(Vector3.Normalize(this.m_componentCreature.ComponentBody.Position - this.m_target.Value) + this.m_random.Vector3(0.5f));
					Vector3 value = this.m_componentCreature.ComponentBody.Position + this.m_random.Float(6f, 8f) * Vector3.Normalize(new Vector3(vector.X, 0f, vector.Z));
					this.m_componentPathfinding.SetDestination(new Vector3?(value), this.m_random.Float(0.6f, 0.8f), 1f, 0, false, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null || this.ScoreTarget(this.m_target.Value) <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.AddState("Circle", delegate
			{
				if (this.m_target != null)
				{
					Vector3 vector = Vector3.Cross(Vector3.Normalize(this.m_componentCreature.ComponentBody.Position - this.m_target.Value), Vector3.UnitY) * this.m_circlingDirection;
					Vector3 value = this.m_componentCreature.ComponentBody.Position + this.m_random.Float(6f, 8f) * Vector3.Normalize(new Vector3(vector.X, 0f, vector.Z));
					this.m_componentPathfinding.SetDestination(new Vector3?(value), this.m_random.Float(0.4f, 0.9f), 1f, 0, false, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_circlingDirection = 0f - this.m_circlingDirection;
					this.m_importanceLevel = 0f;
				}
				else if (this.m_target == null || this.m_componentPathfinding.Destination == null || this.ScoreTarget(this.m_target.Value) <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				this.m_componentCreature.ComponentCreatureModel.LookAtOrder = this.m_target;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x000539E8 File Offset: 0x00051BE8
		public Vector3? FindTarget(out float targetScore)
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3? result = null;
			float num = 0f;
			foreach (Point3 point in this.m_subsystemCampfireBlockBehavior.Campfires)
			{
				Vector3 vector = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
				float num2 = this.ScoreTarget(vector);
				if (num2 > num)
				{
					num = num2;
					result = new Vector3?(vector);
				}
			}
			targetScore = num;
			return result;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00053A94 File Offset: 0x00051C94
		public float ScoreTarget(Vector3 target)
		{
			float num = (this.m_subsystemSky.SkyLightIntensity > 0.2f) ? this.m_dayRange : this.m_nightRange;
			if (num > 0f)
			{
				float num2 = Vector3.Distance(target, this.m_componentCreature.ComponentBody.Position);
				return MathUtils.Saturate(1f - num2 / num);
			}
			return 0f;
		}

		// Token: 0x04000626 RID: 1574
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000627 RID: 1575
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000628 RID: 1576
		public SubsystemCampfireBlockBehavior m_subsystemCampfireBlockBehavior;

		// Token: 0x04000629 RID: 1577
		public ComponentCreature m_componentCreature;

		// Token: 0x0400062A RID: 1578
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400062B RID: 1579
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400062C RID: 1580
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400062D RID: 1581
		public float m_importanceLevel;

		// Token: 0x0400062E RID: 1582
		public float m_dayRange;

		// Token: 0x0400062F RID: 1583
		public float m_nightRange;

		// Token: 0x04000630 RID: 1584
		public Vector3? m_target;

		// Token: 0x04000631 RID: 1585
		public float m_circlingDirection = 1f;

		// Token: 0x04000632 RID: 1586
		public float m_periodicEventOffset;

		// Token: 0x04000633 RID: 1587
		public double m_ignoreFireUntil;
	}
}
