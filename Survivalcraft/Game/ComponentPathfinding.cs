using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FA RID: 506
	public class ComponentPathfinding : Component, IUpdateable
	{
		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x000723FE File Offset: 0x000705FE
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x00072406 File Offset: 0x00070606
		public Vector3? Destination { get; set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0007240F File Offset: 0x0007060F
		// (set) Token: 0x06000EE9 RID: 3817 RVA: 0x00072417 File Offset: 0x00070617
		public float Range { get; set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000EEA RID: 3818 RVA: 0x00072420 File Offset: 0x00070620
		// (set) Token: 0x06000EEB RID: 3819 RVA: 0x00072428 File Offset: 0x00070628
		public float Speed { get; set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x00072431 File Offset: 0x00070631
		// (set) Token: 0x06000EED RID: 3821 RVA: 0x00072439 File Offset: 0x00070639
		public int MaxPathfindingPositions { get; set; }

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x00072442 File Offset: 0x00070642
		// (set) Token: 0x06000EEF RID: 3823 RVA: 0x0007244A File Offset: 0x0007064A
		public bool UseRandomMovements { get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x00072453 File Offset: 0x00070653
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x0007245B File Offset: 0x0007065B
		public bool IgnoreHeightDifference { get; set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x00072464 File Offset: 0x00070664
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0007246C File Offset: 0x0007066C
		public bool RaycastDestination { get; set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x00072475 File Offset: 0x00070675
		// (set) Token: 0x06000EF5 RID: 3829 RVA: 0x0007247D File Offset: 0x0007067D
		public ComponentBody DoNotAvoidBody { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x00072486 File Offset: 0x00070686
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x0007248E File Offset: 0x0007068E
		public bool IsStuck { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x00072497 File Offset: 0x00070697
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0007249C File Offset: 0x0007069C
		public void SetDestination(Vector3? destination, float speed, float range, int maxPathfindingPositions, bool useRandomMovements, bool ignoreHeightDifference, bool raycastDestination, ComponentBody doNotAvoidBody)
		{
			this.Destination = destination;
			this.Speed = speed;
			this.Range = range;
			this.MaxPathfindingPositions = maxPathfindingPositions;
			this.UseRandomMovements = useRandomMovements;
			this.IgnoreHeightDifference = ignoreHeightDifference;
			this.RaycastDestination = raycastDestination;
			this.DoNotAvoidBody = doNotAvoidBody;
			this.m_destinationChanged = true;
			this.m_nextUpdateTime = 0.0;
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x000724FC File Offset: 0x000706FC
		public void Stop()
		{
			this.SetDestination(null, 0f, 0f, 0, false, false, false, null);
			this.m_componentPilot.Stop();
			this.IsStuck = false;
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0007253C File Offset: 0x0007073C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				float num = this.m_random.Float(0.08f, 0.12f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)num;
				this.m_pathfindingCongestion = MathUtils.Max(this.m_pathfindingCongestion - 20f * num, 0f);
				this.m_stateMachine.Update();
			}
		}

        // Token: 0x06000EFC RID: 3836 RVA: 0x000725B0 File Offset: 0x000707B0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemPathfinding = base.Project.FindSubsystem<SubsystemPathfinding>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPilot = base.Entity.FindComponent<ComponentPilot>(true);
			this.m_stateMachine.AddState("Stopped", delegate
			{
				this.Stop();
				this.m_randomMoveCount = 0;
			}, delegate
			{
				if (this.Destination != null)
				{
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.AddState("MovingDirect", delegate
			{
				this.IsStuck = false;
				this.m_destinationChanged = true;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_destinationChanged)
				{
					this.m_componentPilot.SetDestination(this.Destination, this.Speed, this.Range, this.IgnoreHeightDifference, this.RaycastDestination, this.Speed >= 1f, this.DoNotAvoidBody);
					this.m_destinationChanged = false;
					return;
				}
				if (this.m_componentPilot.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_componentPilot.IsStuck)
				{
					if (this.MaxPathfindingPositions > 0 && this.m_componentCreature.ComponentLocomotion.WalkSpeed > 0f)
					{
						this.m_stateMachine.TransitionTo("SearchingForPath");
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
			this.m_stateMachine.AddState("SearchingForPath", delegate
			{
				this.m_pathfindingResult.IsCompleted = false;
				this.m_pathfindingResult.IsInProgress = false;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
				}
				else
				{
					if (!this.m_pathfindingResult.IsInProgress)
					{
						if (this.m_lastPathfindingTime != null)
						{
							double? num = this.m_subsystemTime.GameTime - this.m_lastPathfindingTime;
							double num2 = 8.0;
							if (!(num.GetValueOrDefault() > num2 & num != null))
							{
								goto IL_158;
							}
						}
						if (this.m_pathfindingCongestion < 500f)
						{
							this.m_lastPathfindingDestination = new Vector3?(this.Destination.Value);
							this.m_lastPathfindingTime = new double?(this.m_subsystemTime.GameTime);
							this.m_subsystemPathfinding.QueuePathSearch(this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0.01f, 0f), this.Destination.Value + new Vector3(0f, 0.01f, 0f), 1f, this.m_componentCreature.ComponentBody.BoxSize, this.MaxPathfindingPositions, this.m_pathfindingResult);
							goto IL_170;
						}
					}
					IL_158:
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
					}
				}
				IL_170:
				if (this.m_pathfindingResult.IsCompleted)
				{
					this.m_pathfindingCongestion = MathUtils.Min(this.m_pathfindingCongestion + (float)this.m_pathfindingResult.PositionsChecked, 1000f);
					if (this.m_pathfindingResult.Path.Count > 0)
					{
						this.m_stateMachine.TransitionTo("MovingWithPath");
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
			this.m_stateMachine.AddState("MovingWithPath", delegate
			{
				this.m_componentPilot.Stop();
				this.m_randomMoveCount = 0;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_componentPilot.Destination == null)
				{
					if (this.m_pathfindingResult.Path.Count > 0)
					{
						Vector3 value = this.m_pathfindingResult.Path.Array[this.m_pathfindingResult.Path.Count - 1];
						this.m_componentPilot.SetDestination(new Vector3?(value), MathUtils.Min(this.Speed, 0.75f), 0.75f, false, false, this.Speed >= 1f, this.DoNotAvoidBody);
						this.m_pathfindingResult.Path.RemoveAt(this.m_pathfindingResult.Path.Count - 1);
						return;
					}
					this.m_stateMachine.TransitionTo("MovingDirect");
					return;
				}
				else
				{
					if (!this.m_componentPilot.IsStuck)
					{
						float num = Vector3.DistanceSquared(this.m_componentCreature.ComponentBody.Position, this.Destination.Value);
						if (Vector3.DistanceSquared(this.m_lastPathfindingDestination.Value, this.Destination.Value) > num)
						{
							this.m_stateMachine.TransitionTo("MovingDirect");
						}
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
					return;
				}
			}, null);
			this.m_stateMachine.AddState("MovingRandomly", delegate
			{
				this.m_componentPilot.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + new Vector3(5f * this.m_random.Float(-1f, 1f), 0f, 5f * this.m_random.Float(-1f, 1f))), 1f, 1f, true, false, false, this.DoNotAvoidBody);
				this.m_randomMoveCount++;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_randomMoveCount > 3)
				{
					this.m_stateMachine.TransitionTo("Stuck");
					return;
				}
				if (this.m_componentPilot.IsStuck || this.m_componentPilot.Destination == null)
				{
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.IsStuck = true;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_destinationChanged)
				{
					this.m_destinationChanged = false;
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.TransitionTo("Stopped");
		}

		// Token: 0x04000977 RID: 2423
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000978 RID: 2424
		public SubsystemPathfinding m_subsystemPathfinding;

		// Token: 0x04000979 RID: 2425
		public ComponentCreature m_componentCreature;

		// Token: 0x0400097A RID: 2426
		public ComponentPilot m_componentPilot;

		// Token: 0x0400097B RID: 2427
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400097C RID: 2428
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400097D RID: 2429
		public Vector3? m_lastPathfindingDestination;

		// Token: 0x0400097E RID: 2430
		public double? m_lastPathfindingTime;

		// Token: 0x0400097F RID: 2431
		public float m_pathfindingCongestion;

		// Token: 0x04000980 RID: 2432
		public PathfindingResult m_pathfindingResult = new PathfindingResult();

		// Token: 0x04000981 RID: 2433
		public double m_nextUpdateTime;

		// Token: 0x04000982 RID: 2434
		public int m_randomMoveCount;

		// Token: 0x04000983 RID: 2435
		public bool m_destinationChanged;

		// Token: 0x04000984 RID: 2436
		public const float m_minPathfindingPeriod = 8f;

		// Token: 0x04000985 RID: 2437
		public const float m_pathfindingCongestionCapacity = 500f;

		// Token: 0x04000986 RID: 2438
		public const float m_pathfindingCongestionCapacityLimit = 1000f;

		// Token: 0x04000987 RID: 2439
		public const float m_pathfindingCongestionDecayRate = 20f;

		// Token: 0x04000988 RID: 2440
		public static bool DrawPathfinding;
	}
}
