using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C9 RID: 457
	public class ComponentChaseBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x0005994A File Offset: 0x00057B4A
		public ComponentCreature Target
		{
			get
			{
				return this.m_target;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000BDA RID: 3034 RVA: 0x00059952 File Offset: 0x00057B52
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000BDB RID: 3035 RVA: 0x00059955 File Offset: 0x00057B55
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0005995D File Offset: 0x00057B5D
		public void Attack(ComponentCreature componentCreature, float maxRange, float maxChaseTime, bool isPersistent)
		{
			this.m_target = componentCreature;
			this.m_nextUpdateTime = 0.0;
			this.m_range = maxRange;
			this.m_chaseTime = maxChaseTime;
			this.m_isPersistent = isPersistent;
			this.m_importanceLevel = 200f;
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00059998 File Offset: 0x00057B98
		public void Update(float dt)
		{
			this.m_autoChaseSuppressionTime -= dt;
			if (this.IsActive && this.m_target != null)
			{
				this.m_chaseTime -= dt;
				this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_target.ComponentCreatureModel.EyePosition);
				if (this.IsTargetInAttackRange(this.m_target.ComponentBody))
				{
					this.m_componentCreatureModel.AttackOrder = true;
				}
				if (this.m_componentCreatureModel.IsAttackHitMoment)
				{
					Vector3 hitPoint;
					ComponentBody hitBody = this.GetHitBody(this.m_target.ComponentBody, out hitPoint);
					if (hitBody != null)
					{
						//Console.WriteLine(hitBody);
						float x = this.m_isPersistent ? this.m_random.Float(8f, 10f) : 2f;
						this.m_chaseTime = MathUtils.Max(this.m_chaseTime, x);
						this.m_componentMiner.Hit(hitBody, hitPoint, this.m_componentCreature.ComponentBody.Matrix.Forward);
						this.m_componentCreature.ComponentCreatureSounds.PlayAttackSound();
					}
				}
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_dt = this.m_random.Float(0.25f, 0.35f) + MathUtils.Min((float)(this.m_subsystemTime.GameTime - this.m_nextUpdateTime), 0.1f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_dt;
				this.m_stateMachine.Update();
			}
		}

        // Token: 0x06000BDE RID: 3038 RVA: 0x00059B20 File Offset: 0x00057D20
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_componentFeedBehavior = base.Entity.FindComponent<ComponentRandomFeedBehavior>();
			this.m_componentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(true);
			this.m_dayChaseRange = valuesDictionary.GetValue<float>("DayChaseRange");
			this.m_nightChaseRange = valuesDictionary.GetValue<float>("NightChaseRange");
			this.m_dayChaseTime = valuesDictionary.GetValue<float>("DayChaseTime");
			this.m_nightChaseTime = valuesDictionary.GetValue<float>("NightChaseTime");
			this.m_autoChaseMask = valuesDictionary.GetValue<CreatureCategory>("AutoChaseMask");
			this.m_chaseNonPlayerProbability = valuesDictionary.GetValue<float>("ChaseNonPlayerProbability");
			this.m_chaseWhenAttackedProbability = valuesDictionary.GetValue<float>("ChaseWhenAttackedProbability");
			this.m_chaseOnTouchProbability = valuesDictionary.GetValue<float>("ChaseOnTouchProbability");
			this.m_componentCreature.ComponentHealth.Attacked += delegate(ComponentCreature attacker)
			{
				if (this.m_random.Float(0f, 1f) < this.m_chaseWhenAttackedProbability)
				{
					if (this.m_chaseWhenAttackedProbability >= 1f)
					{
						this.Attack(attacker, 30f, 60f, true);
						return;
					}
					this.Attack(attacker, 7f, 7f, false);
				}
			};
			this.m_componentCreature.ComponentBody.CollidedWithBody += delegate(ComponentBody body)
			{
				if (this.m_target == null && this.m_autoChaseSuppressionTime <= 0f && this.m_random.Float(0f, 1f) < this.m_chaseOnTouchProbability)
				{
					ComponentCreature componentCreature = body.Entity.FindComponent<ComponentCreature>();
					if (componentCreature != null)
					{
						bool flag = this.m_subsystemPlayers.IsPlayer(body.Entity);
						bool flag2 = (componentCreature.Category & this.m_autoChaseMask) > (CreatureCategory)0;
						if ((flag && this.m_subsystemGameInfo.WorldSettings.GameMode > GameMode.Harmless || this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) || (!flag && flag2))
						{
							this.Attack(componentCreature, 7f, 7f, false);
						}
					}
				}
				if (this.m_target != null && body == this.m_target.ComponentBody && body.StandingOnBody == this.m_componentCreature.ComponentBody)
				{
					this.m_componentCreature.ComponentLocomotion.JumpOrder = 1f;
				}
			};
			this.m_stateMachine.AddState("LookingForTarget", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_target = null;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Chasing");
					return;
				}
				if (this.m_autoChaseSuppressionTime <= 0f && (this.m_target == null || this.ScoreTarget(this.m_target) <= 0f) && this.m_componentCreature.ComponentHealth.Health > 0.4f)
				{
					this.m_range = ((this.m_subsystemSky.SkyLightIntensity < 0.2f) ? this.m_nightChaseRange : this.m_dayChaseRange);
					ComponentCreature componentCreature = this.FindTarget();
					if (componentCreature != null)
					{
						this.m_targetInRangeTime += this.m_dt;
					}
					else
					{
						this.m_targetInRangeTime = 0f;
					}
					if (this.m_targetInRangeTime > 3f)
					{
						bool flag = this.m_subsystemSky.SkyLightIntensity >= 0.1f;
						float maxRange = flag ? (this.m_dayChaseRange + 6f) : (this.m_nightChaseRange + 6f);
						float maxChaseTime = flag ? (this.m_dayChaseTime * this.m_random.Float(0.75f, 1f)) : (this.m_nightChaseTime * this.m_random.Float(0.75f, 1f));
						this.Attack(componentCreature, maxRange, maxChaseTime, !flag);
					}
				}
			}, null);
			this.m_stateMachine.AddState("RandomMoving", delegate
			{
				this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + new Vector3(6f * this.m_random.Float(-1f, 1f), 0f, 6f * this.m_random.Float(-1f, 1f))), 1f, 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Chasing");
				}
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("LookingForTarget");
				}
			}, delegate
			{
				this.m_componentPathfinding.Stop();
			});
			this.m_stateMachine.AddState("Chasing", delegate
			{
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 6f);
				this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				this.m_nextUpdateTime = 0.0;
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("LookingForTarget");
					return;
				}
				if (this.m_chaseTime <= 0f)
				{
					this.m_autoChaseSuppressionTime = this.m_random.Float(10f, 60f);
					this.m_importanceLevel = 0f;
					return;
				}
				if (this.m_target == null)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				if (this.m_target.ComponentHealth.Health <= 0f)
				{
					if (this.m_componentFeedBehavior != null)
					{
						this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + (double)this.m_random.Float(1f, 3f), delegate
						{
							if (this.m_target != null)
							{
								this.m_componentFeedBehavior.Feed(this.m_target.ComponentBody.Position);
							}
						});
					}
					this.m_importanceLevel = 0f;
					return;
				}
				if (!this.m_isPersistent && this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				if (this.m_isPersistent && this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("RandomMoving");
					return;
				}
				if (this.ScoreTarget(this.m_target) <= 0f)
				{
					this.m_targetUnsuitableTime += this.m_dt;
				}
				else
				{
					this.m_targetUnsuitableTime = 0f;
				}
				if (this.m_targetUnsuitableTime > 3f)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				int maxPathfindingPositions = 0;
				if (this.m_isPersistent)
				{
					maxPathfindingPositions = ((this.m_subsystemTime.FixedTimeStep != null) ? 1500 : 500);
				}
				BoundingBox boundingBox = this.m_componentCreature.ComponentBody.BoundingBox;
				BoundingBox boundingBox2 = this.m_target.ComponentBody.BoundingBox;
				Vector3 v = 0.5f * (boundingBox.Min + boundingBox.Max);
				Vector3 vector = 0.5f * (boundingBox2.Min + boundingBox2.Max);
				float num = Vector3.Distance(v, vector);
				float num2 = (num < 4f) ? 0.2f : 0f;
				this.m_componentPathfinding.SetDestination(new Vector3?(vector + num2 * num * this.m_target.ComponentBody.getVectorSpeed()), 1f, 1.5f, maxPathfindingPositions, true, false, true, this.m_target.ComponentBody);
				if (this.m_random.Float(0f, 1f) < 0.33f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayAttackSound();
				}
			}, null);
			this.m_stateMachine.TransitionTo("LookingForTarget");
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00059D48 File Offset: 0x00057F48
		public ComponentCreature FindTarget()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			ComponentCreature result = null;
			float num = 0f;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), this.m_range, this.m_componentBodies);
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

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00059DEC File Offset: 0x00057FEC
		public float ScoreTarget(ComponentCreature componentCreature)
		{
			bool flag = componentCreature.Entity.FindComponent<ComponentPlayer>() != null;
			bool flag2 = this.m_componentCreature.Category != CreatureCategory.WaterPredator && this.m_componentCreature.Category != CreatureCategory.WaterOther;
			bool flag3 = componentCreature == this.Target || this.m_subsystemGameInfo.WorldSettings.GameMode > GameMode.Harmless;
			bool flag4 = (componentCreature.Category & this.m_autoChaseMask) > (CreatureCategory)0;
			bool flag5 = componentCreature == this.Target || (flag4 && MathUtils.Remainder(0.004999999888241291 * this.m_subsystemTime.GameTime + (double)((float)(this.GetHashCode() % 1000) / 1000f) + (double)((float)(componentCreature.GetHashCode() % 1000) / 1000f), 1.0) < (double)this.m_chaseNonPlayerProbability);
			if (componentCreature != this.m_componentCreature && ((!flag && flag5) || (flag && flag3)) && componentCreature.Entity.IsAddedToProject && componentCreature.ComponentHealth.Health > 0f && (flag2 || this.IsTargetInWater(componentCreature.ComponentBody)))
			{
				float num = Vector3.Distance(this.m_componentCreature.ComponentBody.Position, componentCreature.ComponentBody.Position);
				if (num < this.m_range)
				{
					return this.m_range - num;
				}
			}
			return 0f;
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00059F48 File Offset: 0x00058148
		public bool IsTargetInWater(ComponentBody target)
		{
			return target.ImmersionDepth > 0f || (target.ParentBody != null && this.IsTargetInWater(target.ParentBody)) || (target.StandingOnBody != null && target.StandingOnBody.Position.Y < target.Position.Y && this.IsTargetInWater(target.StandingOnBody));
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00059FB4 File Offset: 0x000581B4
		public bool IsTargetInAttackRange(ComponentBody target)
		{
			if (this.IsBodyInAttackRange(target))
			{
				return true;
			}
			BoundingBox boundingBox = this.m_componentCreature.ComponentBody.BoundingBox;
			BoundingBox boundingBox2 = target.BoundingBox;
			Vector3 v = 0.5f * (boundingBox.Min + boundingBox.Max);
			Vector3 vector = 0.5f * (boundingBox2.Min + boundingBox2.Max) - v;
			float num = vector.Length();
			Vector3 v2 = vector / num;
			float num2 = 0.5f * (boundingBox.Max.X - boundingBox.Min.X + boundingBox2.Max.X - boundingBox2.Min.X);
			float num3 = 0.5f * (boundingBox.Max.Y - boundingBox.Min.Y + boundingBox2.Max.Y - boundingBox2.Min.Y);
			if (MathUtils.Abs(vector.Y) < num3 * 0.99f)
			{
				if (num < num2 + 0.99f && Vector3.Dot(v2, this.m_componentCreature.ComponentBody.Matrix.Forward) > 0.25f)
				{
					return true;
				}
			}
			else if (num < num3 + 0.3f && MathUtils.Abs(Vector3.Dot(v2, Vector3.UnitY)) > 0.8f)
			{
				return true;
			}
			return (target.ParentBody != null && this.IsTargetInAttackRange(target.ParentBody)) || (target.StandingOnBody != null && target.StandingOnBody.Position.Y < target.Position.Y && this.IsTargetInAttackRange(target.StandingOnBody));
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x0005A164 File Offset: 0x00058364
		public bool IsBodyInAttackRange(ComponentBody target)
		{
			BoundingBox boundingBox = this.m_componentCreature.ComponentBody.BoundingBox;
			BoundingBox boundingBox2 = target.BoundingBox;
			Vector3 v = 0.5f * (boundingBox.Min + boundingBox.Max);
			Vector3 vector = 0.5f * (boundingBox2.Min + boundingBox2.Max) - v;
			float num = vector.Length();
			Vector3 v2 = vector / num;
			float num2 = 0.5f * (boundingBox.Max.X - boundingBox.Min.X + boundingBox2.Max.X - boundingBox2.Min.X);
			float num3 = 0.5f * (boundingBox.Max.Y - boundingBox.Min.Y + boundingBox2.Max.Y - boundingBox2.Min.Y);
			if (MathUtils.Abs(vector.Y) < num3 * 0.99f)
			{
				if (num < num2 + 0.99f && Vector3.Dot(v2, this.m_componentCreature.ComponentBody.Matrix.Forward) > 0.25f)
				{
					return true;
				}
			}
			else if (num < num3 + 0.3f && MathUtils.Abs(Vector3.Dot(v2, Vector3.UnitY)) > 0.8f)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0005A2BC File Offset: 0x000584BC
		public ComponentBody GetHitBody(ComponentBody target, out Vector3 hitPoint)
		{
			Vector3 vector = this.m_componentCreature.ComponentBody.BoundingBox.Center();
			Vector3 v = target.BoundingBox.Center();
			Ray3 ray = new Ray3(vector, Vector3.Normalize(v - vector));
			BodyRaycastResult? bodyRaycastResult = this.m_componentMiner.Raycast<BodyRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (bodyRaycastResult != null && bodyRaycastResult.Value.Distance < 1.75f && (bodyRaycastResult.Value.ComponentBody == target || bodyRaycastResult.Value.ComponentBody.IsChildOfBody(target) || target.IsChildOfBody(bodyRaycastResult.Value.ComponentBody) || target.StandingOnBody == bodyRaycastResult.Value.ComponentBody))
			{
				hitPoint = bodyRaycastResult.Value.HitPoint();
				return bodyRaycastResult.Value.ComponentBody;
			}
			hitPoint = default(Vector3);
			return null;
		}

		// Token: 0x040006AC RID: 1708
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040006AD RID: 1709
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040006AE RID: 1710
		public SubsystemSky m_subsystemSky;

		// Token: 0x040006AF RID: 1711
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040006B0 RID: 1712
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006B1 RID: 1713
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040006B2 RID: 1714
		public ComponentCreature m_componentCreature;

		// Token: 0x040006B3 RID: 1715
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040006B4 RID: 1716
		public ComponentMiner m_componentMiner;

		// Token: 0x040006B5 RID: 1717
		public ComponentRandomFeedBehavior m_componentFeedBehavior;

		// Token: 0x040006B6 RID: 1718
		public ComponentCreatureModel m_componentCreatureModel;

		// Token: 0x040006B7 RID: 1719
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040006B8 RID: 1720
		public Game.Random m_random = new Game.Random();

		// Token: 0x040006B9 RID: 1721
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040006BA RID: 1722
		public float m_dayChaseRange;

		// Token: 0x040006BB RID: 1723
		public float m_nightChaseRange;

		// Token: 0x040006BC RID: 1724
		public float m_dayChaseTime;

		// Token: 0x040006BD RID: 1725
		public float m_nightChaseTime;

		// Token: 0x040006BE RID: 1726
		public float m_chaseNonPlayerProbability;

		// Token: 0x040006BF RID: 1727
		public float m_chaseWhenAttackedProbability;

		// Token: 0x040006C0 RID: 1728
		public float m_chaseOnTouchProbability;

		// Token: 0x040006C1 RID: 1729
		public CreatureCategory m_autoChaseMask;

		// Token: 0x040006C2 RID: 1730
		public float m_importanceLevel;

		// Token: 0x040006C3 RID: 1731
		public float m_targetUnsuitableTime;

		// Token: 0x040006C4 RID: 1732
		public float m_targetInRangeTime;

		// Token: 0x040006C5 RID: 1733
		public double m_nextUpdateTime;

		// Token: 0x040006C6 RID: 1734
		public ComponentCreature m_target;

		// Token: 0x040006C7 RID: 1735
		public float m_dt;

		// Token: 0x040006C8 RID: 1736
		public float m_range;

		// Token: 0x040006C9 RID: 1737
		public float m_chaseTime;

		// Token: 0x040006CA RID: 1738
		public bool m_isPersistent;

		// Token: 0x040006CB RID: 1739
		public float m_autoChaseSuppressionTime;
	}
}
