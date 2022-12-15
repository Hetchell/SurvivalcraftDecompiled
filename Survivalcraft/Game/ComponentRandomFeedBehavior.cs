using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FD RID: 509
	public class ComponentRandomFeedBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x00074CCB File Offset: 0x00072ECB
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000F4E RID: 3918 RVA: 0x00074CCE File Offset: 0x00072ECE
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x00074CD6 File Offset: 0x00072ED6
		public void Feed(Vector3 feedPosition)
		{
			this.m_importanceLevel = 5f;
			this.m_feedPosition = new Vector3?(feedPosition);
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x00074CEF File Offset: 0x00072EEF
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000F51 RID: 3921 RVA: 0x00074CFC File Offset: 0x00072EFC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_autoFeed = valuesDictionary.GetValue<bool>("AutoFeed");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = this.m_random.Float(0f, 1f);
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.05f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_importanceLevel = this.m_random.Float(1f, 3f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				Vector3 vector;
				if (this.m_feedPosition != null)
				{
					vector = this.m_feedPosition.Value;
				}
				else
				{
					Vector3 position = this.m_componentCreature.ComponentBody.Position;
					Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
					float num = (this.m_random.Float(0f, 1f) < 0.2f) ? 5f : 1.5f;
					vector = position + num * forward + 0.5f * num * new Vector3(this.m_random.Float(-1f, 1f), 0f, this.m_random.Float(-1f, 1f));
				}
				vector.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Z)) + 1);
				this.m_componentPathfinding.SetDestination(new Vector3?(vector), this.m_random.Float(0.25f, 0.35f), 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.Destination != null)
				{
					if (!this.IsActive || this.m_componentPathfinding.IsStuck)
					{
						this.m_stateMachine.TransitionTo("Inactive");
					}
					return;
				}
				float num = this.m_random.Float(0f, 1f);
				if (num < 0.33f)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (num < 0.66f)
				{
					this.m_stateMachine.TransitionTo("LookAround");
					return;
				}
				this.m_stateMachine.TransitionTo("Feed");
			}, delegate
			{
				this.m_feedPosition = null;
			});
			this.m_stateMachine.AddState("LookAround", delegate
			{
				this.m_waitTime = this.m_random.Float(1f, 2f);
			}, delegate
			{
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
				this.m_waitTime -= this.m_subsystemTime.GameTimeDelta;
				if (this.m_waitTime <= 0f)
				{
					float num = this.m_random.Float(0f, 1f);
					if (num < 0.25f)
					{
						this.m_stateMachine.TransitionTo("Inactive");
					}
					if (num < 0.5f)
					{
						this.m_stateMachine.TransitionTo(null);
						this.m_stateMachine.TransitionTo("LookAround");
					}
					else if (num < 0.75f)
					{
						this.m_stateMachine.TransitionTo("Move");
						if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
						{
							this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
						}
					}
					else
					{
						this.m_stateMachine.TransitionTo("Feed");
					}
				}
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
			}, null);
			this.m_stateMachine.AddState("Feed", delegate
			{
				this.m_feedTime = this.m_random.Float(4f, 6f);
			}, delegate
			{
				this.m_feedTime -= this.m_subsystemTime.GameTimeDelta;
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
				{
					this.m_componentCreature.ComponentCreatureModel.FeedOrder = true;
					if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					}
					if (this.m_random.Float(0f, 1f) < 1.5f * this.m_subsystemTime.GameTimeDelta)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
					}
				}
				if (this.m_feedTime <= 0f)
				{
					if (this.m_autoFeed)
					{
						float num = this.m_random.Float(0f, 1f);
						if (num < 0.33f)
						{
							this.m_stateMachine.TransitionTo("Inactive");
						}
						if (num < 0.66f)
						{
							this.m_stateMachine.TransitionTo("Move");
						}
						else
						{
							this.m_stateMachine.TransitionTo("LookAround");
						}
					}
					else
					{
						this.m_importanceLevel = 0f;
					}
				}
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x040009C7 RID: 2503
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009C8 RID: 2504
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009C9 RID: 2505
		public ComponentCreature m_componentCreature;

		// Token: 0x040009CA RID: 2506
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040009CB RID: 2507
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040009CC RID: 2508
		public Game.Random m_random = new Game.Random();

		// Token: 0x040009CD RID: 2509
		public float m_importanceLevel;

		// Token: 0x040009CE RID: 2510
		public float m_feedTime;

		// Token: 0x040009CF RID: 2511
		public float m_waitTime;

		// Token: 0x040009D0 RID: 2512
		public Vector3? m_feedPosition;

		// Token: 0x040009D1 RID: 2513
		public bool m_autoFeed;
	}
}
