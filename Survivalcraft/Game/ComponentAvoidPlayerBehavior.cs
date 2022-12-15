using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BF RID: 447
	public class ComponentAvoidPlayerBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000B3B RID: 2875 RVA: 0x00053FB7 File Offset: 0x000521B7
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000B3C RID: 2876 RVA: 0x00053FBA File Offset: 0x000521BA
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x00053FC4 File Offset: 0x000521C4
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_dt = this.m_random.Float(0.4f, 0.6f) + MathUtils.Min((float)(this.m_subsystemTime.GameTime - this.m_nextUpdateTime), 0.1f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_dt;
				this.m_stateMachine.Update();
			}
		}

        // Token: 0x06000B3E RID: 2878 RVA: 0x00054044 File Offset: 0x00052244
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_dayRange = valuesDictionary.GetValue<float>("DayRange");
			this.m_nightRange = valuesDictionary.GetValue<float>("NightRange");
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
				float importanceLevel;
				this.m_target = this.FindTarget(out importanceLevel);
				if (this.m_target != null)
				{
					Vector3.Distance(this.m_target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
					this.SetImportanceLevel(importanceLevel);
					return;
				}
				this.m_importanceLevel = 0f;
			}, null);
			this.m_stateMachine.AddState("Move", null, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				float num = this.ScoreTarget(this.m_target);
				this.SetImportanceLevel(num);
				Vector3 vector = Vector3.Normalize(this.m_componentCreature.ComponentBody.Position - this.m_target.ComponentBody.Position);
				Vector3 value = this.m_componentCreature.ComponentBody.Position + 10f * Vector3.Normalize(new Vector3(vector.X, 0f, vector.Z));
				this.m_componentPathfinding.SetDestination(new Vector3?(value), MathUtils.Lerp(0.6f, 1f, num), 1f, 0, false, true, false, null);
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x00054124 File Offset: 0x00052324
		public void SetImportanceLevel(float score)
		{
			this.m_importanceLevel = MathUtils.Lerp(4f, 8f, MathUtils.Sqrt(score));
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x00054144 File Offset: 0x00052344
		public ComponentCreature FindTarget(out float targetScore)
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
			targetScore = num;
			return result;
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x000541F4 File Offset: 0x000523F4
		public float ScoreTarget(ComponentCreature target)
		{
			float num = (this.m_subsystemSky.SkyLightIntensity > 0.2f) ? this.m_dayRange : this.m_nightRange;
			if (num <= 0f)
			{
				return 0f;
			}
			if (!target.IsAddedToProject || target.ComponentHealth.Health <= 0f || target.Entity.FindComponent<ComponentPlayer>() == null)
			{
				return 0f;
			}
			float num2 = Vector3.Distance(target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
			return MathUtils.Saturate(1f - num2 / num);
		}

		// Token: 0x04000634 RID: 1588
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000635 RID: 1589
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000636 RID: 1590
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000637 RID: 1591
		public ComponentCreature m_componentCreature;

		// Token: 0x04000638 RID: 1592
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000639 RID: 1593
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400063A RID: 1594
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x0400063B RID: 1595
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400063C RID: 1596
		public float m_importanceLevel;

		// Token: 0x0400063D RID: 1597
		public float m_dayRange;

		// Token: 0x0400063E RID: 1598
		public float m_nightRange;

		// Token: 0x0400063F RID: 1599
		public float m_dt;

		// Token: 0x04000640 RID: 1600
		public ComponentCreature m_target;

		// Token: 0x04000641 RID: 1601
		public double m_nextUpdateTime;
	}
}
