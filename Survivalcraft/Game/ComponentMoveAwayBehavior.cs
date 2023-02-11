using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F6 RID: 502
	public class ComponentMoveAwayBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x000717AA File Offset: 0x0006F9AA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000EC6 RID: 3782 RVA: 0x000717AD File Offset: 0x0006F9AD
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x000717B5 File Offset: 0x0006F9B5
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000EC8 RID: 3784 RVA: 0x000717C4 File Offset: 0x0006F9C4
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentCreature.ComponentBody.CollidedWithBody += delegate(ComponentBody body)
			{
				this.m_target = body;
				this.m_isFast = (MathUtils.Max(body.getVectorSpeed().Length(), this.m_componentCreature.ComponentBody.getVectorSpeed().Length()) > 3f);
			};
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
				if (this.m_target != null)
				{
					this.m_importanceLevel = 6f;
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_target != null)
				{
					Vector3 vector = this.m_target.Position + 0.5f * this.m_target.getVectorSpeed();
					Vector2 v = Vector2.Normalize(this.m_componentCreature.ComponentBody.Position.XZ - vector.XZ);
					Vector2 vector2 = Vector2.Zero;
					float num = float.MinValue;
					for (float num2 = 0f; num2 < 6.2831855f; num2 += 0.1f)
					{
						Vector2 vector3 = Vector2.CreateFromAngle(num2);
						if (Vector2.Dot(vector3, v) > 0.2f)
						{
							float num3 = Vector2.Dot(this.m_componentCreature.ComponentBody.Matrix.Forward.XZ, vector3);
							if (num3 > num)
							{
								vector2 = vector3;
								num = num3;
							}
						}
					}
					float s = this.m_random.Float(1.5f, 2f);
					float speed = this.m_isFast ? 0.7f : 0.35f;
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + s * new Vector3(vector2.X, 0f, vector2.Y)), speed, 1f, 0, false, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0400095E RID: 2398
		public ComponentCreature m_componentCreature;

		// Token: 0x0400095F RID: 2399
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000960 RID: 2400
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000961 RID: 2401
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000962 RID: 2402
		public float m_importanceLevel;

		// Token: 0x04000963 RID: 2403
		public ComponentBody m_target;

		// Token: 0x04000964 RID: 2404
		public bool m_isFast;
	}
}
