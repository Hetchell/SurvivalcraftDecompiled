using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D6 RID: 470
	public class ComponentEatPickableBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000CAB RID: 3243 RVA: 0x0005EECC File Offset: 0x0005D0CC
		public float Satiation
		{
			get
			{
				return this.m_satiation;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x0005EED4 File Offset: 0x0005D0D4
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000CAD RID: 3245 RVA: 0x0005EED7 File Offset: 0x0005D0D7
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x0005EEE0 File Offset: 0x0005D0E0
		public void Update(float dt)
		{
			if (this.m_satiation > 0f)
			{
				this.m_satiation = MathUtils.Max(this.m_satiation - 0.01f * this.m_subsystemTime.GameTimeDelta, 0f);
			}
			this.m_stateMachine.Update();
		}

        // Token: 0x06000CAF RID: 3247 RVA: 0x0005EF30 File Offset: 0x0005D130
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_foodFactors = new float[EnumUtils.GetEnumValues(typeof(FoodType)).Max() + 1];
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("FoodFactors"))
			{
				FoodType foodType = (FoodType)Enum.Parse(typeof(FoodType), keyValuePair.Key, false);
				this.m_foodFactors[(int)foodType] = (float)keyValuePair.Value;
			}
			this.m_subsystemPickables.PickableAdded += delegate(Pickable pickable)
			{
				if (this.TryAddPickable(pickable) && this.m_pickable == null)
				{
					this.m_pickable = pickable;
				}
			};
			this.m_subsystemPickables.PickableRemoved += delegate(Pickable pickable)
			{
				this.m_pickables.Remove(pickable);
				if (this.m_pickable == pickable)
				{
					this.m_pickable = null;
				}
			};
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_pickable = null;
			}, delegate
			{
				if (this.m_satiation < 1f)
				{
					if (this.m_pickable == null)
					{
						if (this.m_subsystemTime.GameTime > this.m_nextFindPickableTime)
						{
							this.m_nextFindPickableTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(2f, 4f);
							this.m_pickable = this.FindPickable(this.m_componentCreature.ComponentBody.Position);
						}
					}
					else
					{
						this.m_importanceLevel = this.m_random.Float(5f, 10f);
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
					this.m_blockedCount = 0;
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_pickable != null)
				{
					float speed = (this.m_satiation == 0f) ? this.m_random.Float(0.5f, 0.7f) : 0.5f;
					int maxPathfindingPositions = (this.m_satiation == 0f) ? 1000 : 500;
					float num = Vector3.Distance(this.m_componentCreature.ComponentCreatureModel.EyePosition, this.m_componentCreature.ComponentBody.Position);
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_pickable.Position), speed, 1f + num, maxPathfindingPositions, true, false, true, null);
					if (this.m_random.Float(0f, 1f) < 0.66f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
					}
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_pickable == null)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
					this.m_satiation += 0.75f;
				}
				else if (this.m_componentPathfinding.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Eat");
				}
				else if (Vector3.DistanceSquared(this.m_componentPathfinding.Destination.Value, this.m_pickable.Position) > 0.0625f)
				{
					this.m_stateMachine.TransitionTo("PickableMoved");
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_pickable != null)
				{
					this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_pickable.Position);
					return;
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.AddState("PickableMoved", null, delegate
			{
				if (this.m_pickable != null)
				{
					this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_pickable.Position);
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(0.25, (double)(this.GetHashCode() % 100) * 0.01))
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Eat", delegate
			{
				this.m_eatTime = (double)this.m_random.Float(4f, 5f);
				this.m_blockedTime = 0f;
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_pickable == null)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_pickable != null)
				{
					if (Vector3.DistanceSquared(new Vector3(this.m_componentCreature.ComponentCreatureModel.EyePosition.X, this.m_componentCreature.ComponentBody.Position.Y, this.m_componentCreature.ComponentCreatureModel.EyePosition.Z), this.m_pickable.Position) < 0.64000005f)
					{
						this.m_eatTime -= (double)this.m_subsystemTime.GameTimeDelta;
						this.m_blockedTime = 0f;
						if (this.m_eatTime <= 0.0)
						{
							this.m_satiation += 1f;
							this.m_pickable.Count = MathUtils.Max(this.m_pickable.Count - 1, 0);
							if (this.m_pickable.Count == 0)
							{
								this.m_pickable.ToRemove = true;
								this.m_importanceLevel = 0f;
							}
							else if (this.m_random.Float(0f, 1f) < 0.5f)
							{
								this.m_importanceLevel = 0f;
							}
						}
					}
					else
					{
						float num = Vector3.Distance(this.m_componentCreature.ComponentCreatureModel.EyePosition, this.m_componentCreature.ComponentBody.Position);
						this.m_componentPathfinding.SetDestination(new Vector3?(this.m_pickable.Position), 0.3f, 0.5f + num, 0, false, true, false, null);
						this.m_blockedTime += this.m_subsystemTime.GameTimeDelta;
					}
					if (this.m_blockedTime > 3f)
					{
						this.m_blockedCount++;
						if (this.m_blockedCount >= 3)
						{
							this.m_importanceLevel = 0f;
							this.m_satiation += 0.75f;
						}
						else
						{
							this.m_stateMachine.TransitionTo("Move");
						}
					}
				}
				this.m_componentCreature.ComponentCreatureModel.FeedOrder = true;
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_random.Float(0f, 1f) < 1.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x0005F0EC File Offset: 0x0005D2EC
		public float GetFoodFactor(FoodType foodType)
		{
			return this.m_foodFactors[(int)foodType];
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0005F0F8 File Offset: 0x0005D2F8
		public Pickable FindPickable(Vector3 position)
		{
			if (this.m_subsystemTime.GameTime > this.m_nextPickablesUpdateTime)
			{
				this.m_nextPickablesUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(2f, 4f);
				this.m_pickables.Clear();
				foreach (Pickable pickable in this.m_subsystemPickables.Pickables)
				{
					this.TryAddPickable(pickable);
				}
				if (this.m_pickable != null && !this.m_pickables.ContainsKey(this.m_pickable))
				{
					this.m_pickable = null;
				}
			}
			foreach (Pickable pickable2 in this.m_pickables.Keys)
			{
				float num = Vector3.DistanceSquared(position, pickable2.Position);
				if (this.m_random.Float(0f, 1f) > num / 256f)
				{
					return pickable2;
				}
			}
			return null;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0005F238 File Offset: 0x0005D438
		public bool TryAddPickable(Pickable pickable)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(pickable.Value)];
			if (this.m_foodFactors[(int)block.FoodType] > 0f && Vector3.DistanceSquared(pickable.Position, this.m_componentCreature.ComponentBody.Position) < 256f)
			{
				this.m_pickables.Add(pickable, true);
				return true;
			}
			return false;
		}

		// Token: 0x0400076B RID: 1899
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400076C RID: 1900
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400076D RID: 1901
		public ComponentCreature m_componentCreature;

		// Token: 0x0400076E RID: 1902
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400076F RID: 1903
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000770 RID: 1904
		public Dictionary<Pickable, bool> m_pickables = new Dictionary<Pickable, bool>();

		// Token: 0x04000771 RID: 1905
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000772 RID: 1906
		public float[] m_foodFactors;

		// Token: 0x04000773 RID: 1907
		public float m_importanceLevel;

		// Token: 0x04000774 RID: 1908
		public double m_nextFindPickableTime;

		// Token: 0x04000775 RID: 1909
		public double m_nextPickablesUpdateTime;

		// Token: 0x04000776 RID: 1910
		public Pickable m_pickable;

		// Token: 0x04000777 RID: 1911
		public double m_eatTime;

		// Token: 0x04000778 RID: 1912
		public float m_satiation;

		// Token: 0x04000779 RID: 1913
		public float m_blockedTime;

		// Token: 0x0400077A RID: 1914
		public int m_blockedCount;

		// Token: 0x0400077B RID: 1915
		public const float m_range = 16f;
	}
}
