using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000200 RID: 512
	public class ComponentRunAwayBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x00075F87 File Offset: 0x00074187
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000F76 RID: 3958 RVA: 0x00075F8A File Offset: 0x0007418A
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x00075F92 File Offset: 0x00074192
		public void RunAwayFrom(ComponentBody componentBody)
		{
			this.m_attacker = componentBody;
			this.m_timeToForgetAttacker = this.m_random.Float(10f, 20f);
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x00075FB6 File Offset: 0x000741B6
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			this.m_heardNoise = false;
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00075FCA File Offset: 0x000741CA
		public void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 1f)
			{
				this.m_heardNoise = true;
				this.m_lastNoiseSourcePosition = new Vector3?(sourcePosition);
			}
		}

        // Token: 0x06000F7A RID: 3962 RVA: 0x00075FE8 File Offset: 0x000741E8
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>();
			this.m_componentCreature.ComponentHealth.Attacked += delegate(ComponentCreature attacker)
			{
				this.RunAwayFrom(attacker.ComponentBody);
			};
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_lastNoiseSourcePosition = null;
			}, delegate
			{
				if (this.m_attacker != null)
				{
					this.m_timeToForgetAttacker -= this.m_subsystemTime.GameTimeDelta;
					if (this.m_timeToForgetAttacker <= 0f)
					{
						this.m_attacker = null;
					}
				}
				if (this.m_componentCreature.ComponentHealth.HealthChange < 0f || (this.m_attacker != null && Vector3.DistanceSquared(this.m_attacker.Position, this.m_componentCreature.ComponentBody.Position) < 36f))
				{
					this.m_importanceLevel = MathUtils.Max(this.m_importanceLevel, (float)((this.m_componentCreature.ComponentHealth.Health < 0.33f) ? 300 : 100));
				}
				else if (this.m_heardNoise)
				{
					this.m_importanceLevel = MathUtils.Max(this.m_importanceLevel, 5f);
				}
				else if (!this.IsActive)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("RunningAway");
				}
			}, null);
			this.m_stateMachine.AddState("RunningAway", delegate
			{
				Vector3 value = this.FindSafePlace();
				this.m_componentPathfinding.SetDestination(new Vector3?(value), 1f, 1f, 0, false, true, false, null);
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 6f);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				if (this.m_attacker != null)
				{
					if (!this.m_attacker.IsAddedToProject)
					{
						this.m_importanceLevel = 0f;
						this.m_attacker = null;
						return;
					}
					ComponentHealth componentHealth = this.m_attacker.Entity.FindComponent<ComponentHealth>();
					if (componentHealth != null && componentHealth.Health == 0f)
					{
						this.m_importanceLevel = 0f;
						this.m_attacker = null;
					}
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x000760E0 File Offset: 0x000742E0
		public Vector3 FindSafePlace()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3? herdPosition = (this.m_componentHerdBehavior != null) ? this.m_componentHerdBehavior.FindHerdCenter() : null;
			if (herdPosition != null && Vector3.DistanceSquared(position, herdPosition.Value) < 144f)
			{
				herdPosition = null;
			}
			float num = float.NegativeInfinity;
			Vector3 result = position;
			for (int i = 0; i < 30; i++)
			{
				int num2 = Terrain.ToCell(position.X + this.m_random.Float(-25f, 25f));
				int num3 = Terrain.ToCell(position.Z + this.m_random.Float(-25f, 25f));
				int j = 255;
				while (j >= 0)
				{
					int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(num2, j, num3);
					if (BlocksManager.Blocks[cellContents].IsCollidable || cellContents == 18)
					{
						Vector3 vector = new Vector3((float)num2 + 0.5f, (float)j + 1.1f, (float)num3 + 0.5f);
						float num4 = this.ScoreSafePlace(position, vector, herdPosition, this.m_lastNoiseSourcePosition, cellContents);
						if (num4 > num)
						{
							num = num4;
							result = vector;
							break;
						}
						break;
					}
					else
					{
						j--;
					}
				}
			}
			return result;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x00076230 File Offset: 0x00074430
		public float ScoreSafePlace(Vector3 currentPosition, Vector3 safePosition, Vector3? herdPosition, Vector3? noiseSourcePosition, int contents)
		{
			float num = 0f;
			Vector2 vector = new Vector2(currentPosition.X, currentPosition.Z);
			Vector2 vector2 = new Vector2(safePosition.X, safePosition.Z);
			Segment2 s = new Segment2(vector, vector2);
			if (this.m_attacker != null)
			{
				Vector3 position = this.m_attacker.Position;
				Vector2 vector3 = new Vector2(position.X, position.Z);
				float num2 = Vector2.Distance(vector3, vector2);
				float num3 = Segment2.Distance(s, vector3);
				num += num2 + 3f * num3;
			}
			else
			{
				num += 2f * Vector2.Distance(vector, vector2);
			}
			Vector2? vector4 = (herdPosition != null) ? new Vector2?(new Vector2(herdPosition.Value.X, herdPosition.Value.Z)) : null;
			float num4 = (vector4 != null) ? Segment2.Distance(s, vector4.Value) : 0f;
			num -= num4;
			Vector2? vector5 = (noiseSourcePosition != null) ? new Vector2?(new Vector2(noiseSourcePosition.Value.X, noiseSourcePosition.Value.Z)) : null;
			float num5 = (vector5 != null) ? Segment2.Distance(s, vector5.Value) : 0f;
			num += 1.5f * num5;
			if (contents == 18)
			{
				num -= 4f;
			}
			return num;
		}

		// Token: 0x040009E7 RID: 2535
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009E8 RID: 2536
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009E9 RID: 2537
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040009EA RID: 2538
		public ComponentCreature m_componentCreature;

		// Token: 0x040009EB RID: 2539
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040009EC RID: 2540
		public ComponentHerdBehavior m_componentHerdBehavior;

		// Token: 0x040009ED RID: 2541
		public Game.Random m_random = new Game.Random();

		// Token: 0x040009EE RID: 2542
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040009EF RID: 2543
		public float m_importanceLevel;

		// Token: 0x040009F0 RID: 2544
		public ComponentFrame m_attacker;

		// Token: 0x040009F1 RID: 2545
		public float m_timeToForgetAttacker;

		// Token: 0x040009F2 RID: 2546
		public bool m_heardNoise;

		// Token: 0x040009F3 RID: 2547
		public Vector3? m_lastNoiseSourcePosition;
	}
}
