using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DD RID: 477
	public class ComponentFlyAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000CF8 RID: 3320 RVA: 0x000627C2 File Offset: 0x000609C2
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000CF9 RID: 3321 RVA: 0x000627C5 File Offset: 0x000609C5
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x000627D0 File Offset: 0x000609D0
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_random.Float(0f, 1f) < 0.05f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2f);
			}
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

        // Token: 0x06000CFB RID: 3323 RVA: 0x0006285C File Offset: 0x00060A5C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Fly");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Fly");
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					this.m_importanceLevel = 1f;
				}
			}, null, null);
			this.m_stateMachine.AddState("Fly", delegate
			{
				this.m_angle = this.m_random.Float(0f, 6.2831855f);
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination == null)
				{
					float num = (this.m_random.Float(0f, 1f) < 0.2f) ? this.m_random.Float(0.4f, 0.6f) : (0f - this.m_random.Float(0.4f, 0.6f));
					this.m_angle = MathUtils.NormalizeAngle(this.m_angle + num);
					Vector2 vector = Vector2.CreateFromAngle(this.m_angle);
					Vector3 vector2 = position + new Vector3(vector.X, 0f, vector.Y) * 10f;
					vector2.Y = this.EstimateHeight(new Vector2(vector2.X, vector2.Z), 8) + this.m_random.Float(3f, 5f);
					this.m_componentPathfinding.SetDestination(new Vector3?(vector2), this.m_random.Float(0.6f, 1.05f), 6f, 0, false, true, false, null);
					if (this.m_random.Float(0f, 1f) < 0.15f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
						return;
					}
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00062904 File Offset: 0x00060B04
		public float EstimateHeight(Vector2 position, int radius)
		{
			int num = 0;
			for (int i = 0; i < 15; i++)
			{
				int x = Terrain.ToCell(position.X) + this.m_random.Int(-radius, radius);
				int z = Terrain.ToCell(position.Y) + this.m_random.Int(-radius, radius);
				num = MathUtils.Max(num, this.m_subsystemTerrain.Terrain.GetTopHeight(x, z));
			}
			return (float)num;
		}

		// Token: 0x040007D7 RID: 2007
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040007D8 RID: 2008
		public ComponentCreature m_componentCreature;

		// Token: 0x040007D9 RID: 2009
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040007DA RID: 2010
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040007DB RID: 2011
		public float m_angle;

		// Token: 0x040007DC RID: 2012
		public float m_importanceLevel = 1f;

		// Token: 0x040007DD RID: 2013
		public Game.Random m_random = new Game.Random();
	}
}
