using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D7 RID: 471
	public class ComponentFindPlayerBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000CBD RID: 3261 RVA: 0x0005F920 File Offset: 0x0005DB20
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000CBE RID: 3262 RVA: 0x0005F923 File Offset: 0x0005DB23
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0005F92C File Offset: 0x0005DB2C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_dt = this.m_random.Float(1.25f, 1.75f) + MathUtils.Min((float)(this.m_subsystemTime.GameTime - this.m_nextUpdateTime), 0.1f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_dt;
				this.m_stateMachine.Update();
			}
		}

        // Token: 0x06000CC0 RID: 3264 RVA: 0x0005F9AC File Offset: 0x0005DBAC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_dayRange = valuesDictionary.GetValue<float>("DayRange");
			this.m_nightRange = valuesDictionary.GetValue<float>("NightRange");
			this.m_minRange = valuesDictionary.GetValue<float>("MinRange");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_target = null;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
				if (this.m_subsystemGameInfo.WorldSettings.GameMode > GameMode.Harmless)
				{
					this.m_target = this.FindTarget();
					if (this.m_target != null)
					{
						ComponentPlayer componentPlayer = this.m_target.Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer != null && componentPlayer.ComponentSleep.IsSleeping)
						{
							this.m_importanceLevel = 5f;
							return;
						}
						if (this.m_random.Float(0f, 1f) < 0.05f * this.m_dt)
						{
							this.m_importanceLevel = this.m_random.Float(1f, 4f);
							return;
						}
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
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_target.ComponentBody.Position), this.m_random.Float(0.5f, 0.7f), this.m_minRange, 500, true, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null || this.ScoreTarget(this.m_target) <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0005FABC File Offset: 0x0005DCBC
		public ComponentCreature FindTarget()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			ComponentCreature result = null;
			float num = 0f;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), MathUtils.Max(this.m_nightRange, this.m_dayRange), this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentCreature componentCreature = this.m_componentBodies.Array[i].Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					float num2 = this.ScoreTarget(componentCreature);
					if (num2 > num)
					{
						num = num2;
						result = componentCreature;
					}
				}
			}
			return result;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0005FB6C File Offset: 0x0005DD6C
		public float ScoreTarget(ComponentCreature target)
		{
			float num = (this.m_subsystemSky.SkyLightIntensity > 0.2f) ? this.m_dayRange : this.m_nightRange;
			if (!target.IsAddedToProject || target.ComponentHealth.Health <= 0f || target.Entity.FindComponent<ComponentPlayer>() == null)
			{
				return 0f;
			}
			float num2 = Vector3.DistanceSquared(target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
			if (num2 < this.m_minRange * this.m_minRange)
			{
				return 0f;
			}
			return num * num - num2;
		}

		// Token: 0x0400077C RID: 1916
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400077D RID: 1917
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400077E RID: 1918
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400077F RID: 1919
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000780 RID: 1920
		public ComponentCreature m_componentCreature;

		// Token: 0x04000781 RID: 1921
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000782 RID: 1922
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000783 RID: 1923
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000784 RID: 1924
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000785 RID: 1925
		public float m_importanceLevel;

		// Token: 0x04000786 RID: 1926
		public float m_dayRange;

		// Token: 0x04000787 RID: 1927
		public float m_nightRange;

		// Token: 0x04000788 RID: 1928
		public float m_minRange;

		// Token: 0x04000789 RID: 1929
		public float m_dt;

		// Token: 0x0400078A RID: 1930
		public ComponentCreature m_target;

		// Token: 0x0400078B RID: 1931
		public double m_nextUpdateTime;
	}
}
