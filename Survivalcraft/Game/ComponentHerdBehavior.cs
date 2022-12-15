using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E6 RID: 486
	public class ComponentHerdBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00067A35 File Offset: 0x00065C35
		// (set) Token: 0x06000DA5 RID: 3493 RVA: 0x00067A3D File Offset: 0x00065C3D
		public string HerdName { get; set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00067A46 File Offset: 0x00065C46
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x00067A49 File Offset: 0x00065C49
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x00067A54 File Offset: 0x00065C54
		public void CallNearbyCreaturesHelp(ComponentCreature target, float maxRange, float maxChaseTime, bool isPersistent)
		{
			Vector3 position = target.ComponentBody.Position;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (Vector3.DistanceSquared(position, componentCreature.ComponentBody.Position) < 256f)
				{
					ComponentHerdBehavior componentHerdBehavior = componentCreature.Entity.FindComponent<ComponentHerdBehavior>();
					if (componentHerdBehavior != null && componentHerdBehavior.HerdName == this.HerdName && componentHerdBehavior.m_autoNearbyCreaturesHelp)
					{
						ComponentChaseBehavior componentChaseBehavior = componentCreature.Entity.FindComponent<ComponentChaseBehavior>();
						if (componentChaseBehavior != null && componentChaseBehavior.Target == null)
						{
							componentChaseBehavior.Attack(target, maxRange, maxChaseTime, isPersistent);
						}
					}
				}
			}
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x00067B1C File Offset: 0x00065D1C
		public Vector3? FindHerdCenter()
		{
			if (string.IsNullOrEmpty(this.HerdName))
			{
				return null;
			}
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			int num = 0;
			Vector3 vector = Vector3.Zero;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (componentCreature.ComponentHealth.Health > 0f)
				{
					ComponentHerdBehavior componentHerdBehavior = componentCreature.Entity.FindComponent<ComponentHerdBehavior>();
					if (componentHerdBehavior != null && componentHerdBehavior.HerdName == this.HerdName)
					{
						Vector3 position2 = componentCreature.ComponentBody.Position;
						if (Vector3.DistanceSquared(position, position2) < this.m_herdingRange * this.m_herdingRange)
						{
							vector += position2;
							num++;
						}
					}
				}
			}
			if (num > 0)
			{
				return new Vector3?(vector / (float)num);
			}
			return null;
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x00067C28 File Offset: 0x00065E28
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState) || !this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			this.m_dt = dt;
			this.m_stateMachine.Update();
		}

        // Token: 0x06000DAB RID: 3499 RVA: 0x00067C68 File Offset: 0x00065E68
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.HerdName = valuesDictionary.GetValue<string>("HerdName");
			this.m_herdingRange = valuesDictionary.GetValue<float>("HerdingRange");
			this.m_autoNearbyCreaturesHelp = valuesDictionary.GetValue<bool>("AutoNearbyCreaturesHelp");
			this.m_componentCreature.ComponentHealth.Attacked += delegate(ComponentCreature attacker)
			{
				this.CallNearbyCreaturesHelp(attacker, 20f, 30f, false);
			};
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)(1f * ((float)(this.GetHashCode() % 256) / 256f))))
				{
					Vector3? vector = this.FindHerdCenter();
					if (vector != null)
					{
						float num = Vector3.Distance(vector.Value, this.m_componentCreature.ComponentBody.Position);
						if (num > 10f)
						{
							this.m_importanceLevel = 1f;
						}
						if (num > 12f)
						{
							this.m_importanceLevel = 3f;
						}
						if (num > 16f)
						{
							this.m_importanceLevel = 50f;
						}
						if (num > 20f)
						{
							this.m_importanceLevel = 250f;
						}
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Herd");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Herd");
				if (this.m_random.Bool(0.5f))
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					this.m_importanceLevel = 0f;
				}
			}, null, null);
			this.m_stateMachine.AddState("Herd", delegate
			{
				Vector3? vector = this.FindHerdCenter();
				if (vector != null && Vector3.Distance(this.m_componentCreature.ComponentBody.Position, vector.Value) > 6f)
				{
					float speed = (this.m_importanceLevel > 10f) ? this.m_random.Float(0.9f, 1f) : this.m_random.Float(0.25f, 0.35f);
					int maxPathfindingPositions = (this.m_importanceLevel > 200f) ? 100 : 0;
					this.m_componentPathfinding.SetDestination(new Vector3?(vector.Value), speed, 7f, maxPathfindingPositions, false, true, false, null);
					return;
				}
				this.m_importanceLevel = 0f;
			}, delegate
			{
				this.m_componentCreature.ComponentLocomotion.LookOrder = this.m_look - this.m_componentCreature.ComponentLocomotion.LookAngles;
				if (this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("Stuck");
				}
				if (this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.05f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				if (this.m_random.Float(0f, 1f) < 1.5f * this.m_dt)
				{
					this.m_look = new Vector2(MathUtils.DegToRad(45f) * this.m_random.Float(-1f, 1f), MathUtils.DegToRad(10f) * this.m_random.Float(-1f, 1f));
				}
			}, null);
		}

		// Token: 0x04000874 RID: 2164
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x04000875 RID: 2165
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000876 RID: 2166
		public ComponentCreature m_componentCreature;

		// Token: 0x04000877 RID: 2167
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000878 RID: 2168
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000879 RID: 2169
		public float m_dt;

		// Token: 0x0400087A RID: 2170
		public float m_importanceLevel;

		// Token: 0x0400087B RID: 2171
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400087C RID: 2172
		public Vector2 m_look;

		// Token: 0x0400087D RID: 2173
		public float m_herdingRange;

		// Token: 0x0400087E RID: 2174
		public bool m_autoNearbyCreaturesHelp;
	}
}
