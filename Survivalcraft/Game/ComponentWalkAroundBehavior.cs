using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000210 RID: 528
	public class ComponentWalkAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06001053 RID: 4179 RVA: 0x0007CF19 File Offset: 0x0007B119
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06001054 RID: 4180 RVA: 0x0007CF1C File Offset: 0x0007B11C
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x0007CF24 File Offset: 0x0007B124
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06001056 RID: 4182 RVA: 0x0007CF34 File Offset: 0x0007B134
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = this.m_random.Float(0f, 1f);
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.05f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_importanceLevel = this.m_random.Float(1f, 2f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Walk");
				}
			}, null);
			this.m_stateMachine.AddState("Walk", delegate
			{
				float speed = (this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f) ? 1f : this.m_random.Float(0.25f, 0.35f);
				this.m_componentPathfinding.SetDestination(new Vector3?(this.FindDestination()), speed, 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.IsStuck || !this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_componentPathfinding.Destination == null)
				{
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_stateMachine.TransitionTo("Inactive");
					}
					else
					{
						this.m_stateMachine.TransitionTo(null);
						this.m_stateMachine.TransitionTo("Walk");
					}
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0007CFEC File Offset: 0x0007B1EC
		public Vector3 FindDestination()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			float num = 0f;
			Vector3 result = position;
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector = Vector2.Normalize(this.m_random.Vector2(1f)) * this.m_random.Float(6f, 12f);
				Vector3 vector2 = new Vector3(position.X + vector.X, 0f, position.Z + vector.Y);
				vector2.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector2.X), Terrain.ToCell(vector2.Z)) + 1);
				float num2 = this.ScoreDestination(vector2);
				if (num2 > num)
				{
					num = num2;
					result = vector2;
				}
			}
			return result;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0007D0C8 File Offset: 0x0007B2C8
		public float ScoreDestination(Vector3 destination)
		{
			float num = 8f - MathUtils.Abs(this.m_componentCreature.ComponentBody.Position.Y - destination.Y);
			if (this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(destination.X), Terrain.ToCell(destination.Y) - 1, Terrain.ToCell(destination.Z)) == 18)
			{
				num -= 5f;
			}
			return num;
		}

		// Token: 0x04000AB7 RID: 2743
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AB8 RID: 2744
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000AB9 RID: 2745
		public ComponentCreature m_componentCreature;

		// Token: 0x04000ABA RID: 2746
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000ABB RID: 2747
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000ABC RID: 2748
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000ABD RID: 2749
		public float m_importanceLevel;
	}
}
