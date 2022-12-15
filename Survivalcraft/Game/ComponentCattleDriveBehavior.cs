using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C7 RID: 455
	public class ComponentCattleDriveBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000BC2 RID: 3010 RVA: 0x00058EBC File Offset: 0x000570BC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x00058EBF File Offset: 0x000570BF
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00058EC8 File Offset: 0x000570C8
		public void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 0.5f)
			{
				Vector3 v = this.m_componentCreature.ComponentBody.Position - sourcePosition;
				this.m_driveVector += Vector3.Normalize(v) * MathUtils.Max(8f - 0.25f * v.Length(), 1f);
				float num = 12f + this.m_random.Float(0f, 3f);
				if (this.m_driveVector.Length() > num)
				{
					this.m_driveVector = num * Vector3.Normalize(this.m_driveVector);
				}
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00058F71 File Offset: 0x00057171
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000BC6 RID: 3014 RVA: 0x00058F80 File Offset: 0x00057180
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_driveVector = Vector3.Zero;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Drive");
				}
				if (this.m_driveVector.Length() > 3f)
				{
					this.m_importanceLevel = 7f;
				}
				this.FadeDriveVector();
			}, null);
			this.m_stateMachine.AddState("Drive", delegate
			{
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_driveVector.LengthSquared() < 1f || this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_random.Float(0f, 1f) < 3f * this.m_subsystemTime.GameTimeDelta)
				{
					Vector3 v = this.CalculateDriveDirectionAndSpeed();
					float speed = MathUtils.Saturate(0.2f * v.Length());
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + 15f * Vector3.Normalize(v)), speed, 5f, 0, false, true, false, null);
				}
				this.FadeDriveVector();
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0005905C File Offset: 0x0005725C
		public void FadeDriveVector()
		{
			float num = this.m_driveVector.Length();
			if (num > 0.1f)
			{
				this.m_driveVector -= this.m_subsystemTime.GameTimeDelta * this.m_driveVector / num;
			}
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x000590AC File Offset: 0x000572AC
		public Vector3 CalculateDriveDirectionAndSpeed()
		{
			int num = 1;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = position;
			Vector3 vector2 = this.m_driveVector;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (componentCreature != this.m_componentCreature && componentCreature.ComponentHealth.Health > 0f)
				{
					ComponentCattleDriveBehavior componentCattleDriveBehavior = componentCreature.Entity.FindComponent<ComponentCattleDriveBehavior>();
					if (componentCattleDriveBehavior != null && componentCattleDriveBehavior.m_componentHerdBehavior.HerdName == this.m_componentHerdBehavior.HerdName)
					{
						Vector3 position2 = componentCreature.ComponentBody.Position;
						if (Vector3.DistanceSquared(position, position2) < 625f)
						{
							vector += position2;
							vector2 += componentCattleDriveBehavior.m_driveVector;
							num++;
						}
					}
				}
			}
			vector /= (float)num;
			vector2 /= (float)num;
			Vector3 v = vector - position;
			float s = MathUtils.Max(1.5f * v.Length() - 3f, 0f);
			return 0.33f * this.m_driveVector + 0.66f * vector2 + s * Vector3.Normalize(v);
		}

		// Token: 0x0400069A RID: 1690
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400069B RID: 1691
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x0400069C RID: 1692
		public ComponentCreature m_componentCreature;

		// Token: 0x0400069D RID: 1693
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400069E RID: 1694
		public ComponentHerdBehavior m_componentHerdBehavior;

		// Token: 0x0400069F RID: 1695
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040006A0 RID: 1696
		public Game.Random m_random = new Game.Random();

		// Token: 0x040006A1 RID: 1697
		public float m_importanceLevel;

		// Token: 0x040006A2 RID: 1698
		public Vector3 m_driveVector;
	}
}
