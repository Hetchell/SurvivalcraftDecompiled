using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FE RID: 510
	public class ComponentRandomPeckBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000F5C RID: 3932 RVA: 0x0007534E File Offset: 0x0007354E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x00075351 File Offset: 0x00073551
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0007535C File Offset: 0x0007355C
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Move");
			}
			if (this.m_random.Float(0f, 1f) < 0.033f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2.5f);
			}
			this.m_dt = dt;
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

        // Token: 0x06000F5F RID: 3935 RVA: 0x000753F0 File Offset: 0x000735F0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentBirdModel = base.Entity.FindComponent<ComponentBirdModel>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Move");
			}, null, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				float num = (this.m_random.Float(0f, 1f) < 0.2f) ? 8f : 3f;
				Vector3 vector = position + new Vector3(num * this.m_random.Float(-1f, 1f), 0f, num * this.m_random.Float(-1f, 1f));
				vector.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Z)) + 1);
				this.m_componentPathfinding.SetDestination(new Vector3?(vector), this.m_random.Float(0.5f, 0.7f), 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.Destination != null)
				{
					if (this.m_componentPathfinding.IsStuck)
					{
						this.m_stateMachine.TransitionTo("Stuck");
					}
					return;
				}
				if (this.m_random.Float(0f, 1f) < 0.33f)
				{
					this.m_stateMachine.TransitionTo("Wait");
					return;
				}
				this.m_stateMachine.TransitionTo("Peck");
			}, null);
			this.m_stateMachine.AddState("Wait", delegate
			{
				this.m_waitTime = this.m_random.Float(0.75f, 1f);
			}, delegate
			{
				this.m_waitTime -= this.m_dt;
				if (this.m_waitTime <= 0f)
				{
					if (this.m_random.Float(0f, 1f) < 0.25f)
					{
						this.m_stateMachine.TransitionTo("Move");
						if (this.m_random.Float(0f, 1f) < 0.33f)
						{
							this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
							return;
						}
					}
					else
					{
						this.m_stateMachine.TransitionTo("Peck");
					}
				}
			}, null);
			this.m_stateMachine.AddState("Peck", delegate
			{
				this.m_peckTime = this.m_random.Float(2f, 6f);
			}, delegate
			{
				this.m_peckTime -= this.m_dt;
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
				{
					this.m_componentBirdModel.FeedOrder = true;
				}
				if (this.m_peckTime <= 0f)
				{
					if (this.m_random.Float(0f, 1f) < 0.25f)
					{
						this.m_stateMachine.TransitionTo("Move");
						return;
					}
					this.m_stateMachine.TransitionTo("Wait");
				}
			}, null);
		}

		// Token: 0x040009D2 RID: 2514
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009D3 RID: 2515
		public ComponentCreature m_componentCreature;

		// Token: 0x040009D4 RID: 2516
		public ComponentBirdModel m_componentBirdModel;

		// Token: 0x040009D5 RID: 2517
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040009D6 RID: 2518
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040009D7 RID: 2519
		public float m_importanceLevel = 1f;

		// Token: 0x040009D8 RID: 2520
		public float m_dt;

		// Token: 0x040009D9 RID: 2521
		public float m_peckTime;

		// Token: 0x040009DA RID: 2522
		public float m_waitTime;

		// Token: 0x040009DB RID: 2523
		public Game.Random m_random = new Game.Random();
	}
}
