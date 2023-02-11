using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000208 RID: 520
	public class ComponentSteedBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x0007915B File Offset: 0x0007735B
		// (set) Token: 0x06000FE6 RID: 4070 RVA: 0x00079163 File Offset: 0x00077363
		public int SpeedOrder { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x0007916C File Offset: 0x0007736C
		// (set) Token: 0x06000FE8 RID: 4072 RVA: 0x00079174 File Offset: 0x00077374
		public float TurnOrder { get; set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x0007917D File Offset: 0x0007737D
		// (set) Token: 0x06000FEA RID: 4074 RVA: 0x00079185 File Offset: 0x00077385
		public float JumpOrder { get; set; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000FEB RID: 4075 RVA: 0x0007918E File Offset: 0x0007738E
		// (set) Token: 0x06000FEC RID: 4076 RVA: 0x00079196 File Offset: 0x00077396
		public bool WasOrderIssued { get; set; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000FED RID: 4077 RVA: 0x0007919F File Offset: 0x0007739F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x000791A2 File Offset: 0x000773A2
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x000791AC File Offset: 0x000773AC
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			if (this.SpeedOrder != 0 || this.TurnOrder != 0f || this.JumpOrder != 0f)
			{
				this.SpeedOrder = 0;
				this.TurnOrder = 0f;
				this.JumpOrder = 0f;
				this.WasOrderIssued = true;
			}
			else
			{
				this.WasOrderIssued = false;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)((float)(this.GetHashCode() % 100) * 0.01f)))
			{
				this.m_importanceLevel = 0f;
				if (this.m_isEnabled)
				{
					if (this.m_componentMount.Rider != null)
					{
						this.m_importanceLevel = 275f;
					}
					else if (this.FindNearbyRider(7f) != null)
					{
						this.m_importanceLevel = 7f;
					}
				}
			}
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
		}

        // Token: 0x06000FF0 RID: 4080 RVA: 0x00079298 File Offset: 0x00077498
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_isEnabled = base.Entity.ValuesDictionary.DatabaseObject.Name.EndsWith("_Saddled");
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Wait");
				}
			}, null);
			this.m_stateMachine.AddState("Wait", delegate
			{
				ComponentRider componentRider = this.FindNearbyRider(6f);
				if (componentRider != null)
				{
					this.m_componentPathfinding.SetDestination(new Vector3?(componentRider.ComponentCreature.ComponentBody.Position), this.m_random.Float(0.2f, 0.3f), 3.25f, 0, false, true, false, null);
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
					}
				}
			}, delegate
			{
				if (this.m_componentMount.Rider != null)
				{
					this.m_stateMachine.TransitionTo("Steed");
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.AddState("Steed", delegate
			{
				this.m_componentPathfinding.Stop();
				this.m_speed = 0f;
				this.m_speedLevel = 1;
			}, delegate
			{
				this.ProcessRidingOrders();
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x000793A4 File Offset: 0x000775A4
		public ComponentRider FindNearbyRider(float range)
		{
			this.m_bodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(this.m_componentCreature.ComponentBody.Position.X, this.m_componentCreature.ComponentBody.Position.Z), range, this.m_bodies);
			foreach (ComponentBody componentBody in this.m_bodies)
			{
				if (Vector3.DistanceSquared(this.m_componentCreature.ComponentBody.Position, componentBody.Position) < range * range)
				{
					ComponentRider componentRider = componentBody.Entity.FindComponent<ComponentRider>();
					if (componentRider != null)
					{
						return componentRider;
					}
				}
			}
			return null;
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00079474 File Offset: 0x00077674
		public void ProcessRidingOrders()
		{
			this.m_speedLevel = MathUtils.Clamp(this.m_speedLevel + this.SpeedOrder, 0, this.m_speedLevels.Length - 1);
			if (this.m_speedLevel == this.m_speedLevels.Length - 1 && this.SpeedOrder > 0)
			{
				this.m_timeToSpeedReduction = this.m_random.Float(8f, 12f);
			}
			if (this.m_speedLevel == 0 && this.SpeedOrder < 0)
			{
				this.m_timeToSpeedReduction = 1.25f;
			}
			this.m_timeToSpeedReduction -= this.m_subsystemTime.GameTimeDelta;
			if (this.m_timeToSpeedReduction <= 0f && this.m_speedLevel == this.m_speedLevels.Length - 1)
			{
				this.m_speedLevel--;
				this.m_speedChangeFactor = 0.25f;
			}
			else if (this.m_timeToSpeedReduction <= 0f && this.m_speedLevel == 0)
			{
				this.m_speedLevel = 1;
				this.m_speedChangeFactor = 100f;
			}
			else
			{
				this.m_speedChangeFactor = 100f;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(0.25, 0.0))
			{
				float num = new Vector2(this.m_componentCreature.ComponentBody.CollisionVelocityChange.X, this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z).Length();
				if (this.m_speedLevel == 0 || num < 0.1f || this.m_componentCreature.ComponentBody.getVectorSpeed().Length() > MathUtils.Abs(0.5f * this.m_speed * this.m_componentCreature.ComponentLocomotion.WalkSpeed))
				{
					this.m_lastNotBlockedTime = this.m_subsystemTime.GameTime;
				}
				else if (this.m_subsystemTime.GameTime - this.m_lastNotBlockedTime > 0.75)
				{
					this.m_speedLevel = 1;
				}
			}
			this.m_speed += MathUtils.Saturate(this.m_speedChangeFactor * this.m_subsystemTime.GameTimeDelta) * (this.m_speedLevels[this.m_speedLevel] - this.m_speed);
			this.m_turnSpeed += 2f * this.m_subsystemTime.GameTimeDelta * (MathUtils.Clamp(this.TurnOrder, -0.5f, 0.5f) - this.m_turnSpeed);
			this.m_componentCreature.ComponentLocomotion.TurnOrder = new Vector2(this.m_turnSpeed, 0f);
			this.m_componentCreature.ComponentLocomotion.WalkOrder = new Vector2?(new Vector2(0f, this.m_speed));
			if (MathUtils.Abs(this.m_speed) > 0.01f || MathUtils.Abs(this.m_turnSpeed) > 0.01f)
			{
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(2f * this.m_turnSpeed, 0f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_componentCreature.ComponentLocomotion.JumpOrder, this.JumpOrder);
		}

		// Token: 0x04000A48 RID: 2632
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A49 RID: 2633
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000A4A RID: 2634
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A4B RID: 2635
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000A4C RID: 2636
		public ComponentMount m_componentMount;

		// Token: 0x04000A4D RID: 2637
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A4E RID: 2638
		public float m_importanceLevel;

		// Token: 0x04000A4F RID: 2639
		public bool m_isEnabled;

		// Token: 0x04000A50 RID: 2640
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A51 RID: 2641
		public DynamicArray<ComponentBody> m_bodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000A52 RID: 2642
		public float[] m_speedLevels = new float[]
		{
			-0.33f,
			0f,
			0.33f,
			0.66f,
			1f
		};

		// Token: 0x04000A53 RID: 2643
		public int m_speedLevel;

		// Token: 0x04000A54 RID: 2644
		public float m_speed;

		// Token: 0x04000A55 RID: 2645
		public float m_turnSpeed;

		// Token: 0x04000A56 RID: 2646
		public float m_speedChangeFactor;

		// Token: 0x04000A57 RID: 2647
		public float m_timeToSpeedReduction;

		// Token: 0x04000A58 RID: 2648
		public double m_lastNotBlockedTime;
	}
}
