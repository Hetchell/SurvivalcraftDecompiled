using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DE RID: 478
	public class ComponentFlyAwayBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000D02 RID: 3330 RVA: 0x00062BB3 File Offset: 0x00060DB3
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x00062BB6 File Offset: 0x00060DB6
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x17000160 RID: 352
		// (set) Token: 0x06000D04 RID: 3332 RVA: 0x00062BBE File Offset: 0x00060DBE
		public override bool IsActive
		{
			set
			{
				base.IsActive = value;
				if (this.IsActive)
				{
					this.m_nextUpdateTime = 0.0;
				}
			}
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x00062BE0 File Offset: 0x00060DE0
		public void Update(float dt)
		{
			if (this.m_componentCreature.ComponentHealth.HealthChange < 0f)
			{
				this.m_stateMachine.TransitionTo("DangerDetected");
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.5f, 1f);
				this.m_stateMachine.Update();
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x00062C5A File Offset: 0x00060E5A
		public void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 0.25f && this.m_stateMachine.CurrentState != "RunningAway")
			{
				this.m_stateMachine.TransitionTo("DangerDetected");
			}
		}

        // Token: 0x06000D07 RID: 3335 RVA: 0x00062C8C File Offset: 0x00060E8C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentCreature.ComponentBody.CollidedWithBody += delegate(ComponentBody p)
			{
				if (this.m_stateMachine.CurrentState != "RunningAway")
				{
					this.m_stateMachine.TransitionTo("DangerDetected");
				}
			};
			this.m_stateMachine.AddState("LookingForDanger", null, delegate
			{
				if (this.ScanForDanger())
				{
					this.m_stateMachine.TransitionTo("DangerDetected");
				}
			}, null);
			this.m_stateMachine.AddState("DangerDetected", delegate
			{
				this.m_importanceLevel = (float)((this.m_componentCreature.ComponentHealth.Health < 0.33f) ? 300 : 100);
				this.m_nextUpdateTime = 0.0;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("RunningAway");
					this.m_nextUpdateTime = 0.0;
				}
			}, null);
			this.m_stateMachine.AddState("RunningAway", delegate
			{
				this.m_componentPathfinding.SetDestination(new Vector3?(this.FindSafePlace()), 1f, 1f, 0, false, true, false, null);
				this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/Wings", 0.8f, this.m_random.Float(-0.1f, 0.2f), this.m_componentCreature.ComponentBody.Position, 3f, true);
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 6f);
			}, delegate
			{
				if (!this.IsActive || this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("LookingForDanger");
					return;
				}
				if (this.ScoreSafePlace(this.m_componentCreature.ComponentBody.Position, this.m_componentPathfinding.Destination.Value, null) < 4f)
				{
					this.m_componentPathfinding.SetDestination(new Vector3?(this.FindSafePlace()), 1f, 0.5f, 0, false, true, false, null);
				}
			}, delegate
			{
				this.m_importanceLevel = 0f;
			});
			this.m_stateMachine.TransitionTo("LookingForDanger");
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x00062DC0 File Offset: 0x00060FC0
		public bool ScanForDanger()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			Vector3 translation = matrix.Translation;
			Vector3 forward = matrix.Forward;
			return this.ScoreSafePlace(translation, translation, new Vector3?(forward)) < 7f;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x00062E08 File Offset: 0x00061008
		public Vector3 FindSafePlace()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			float num = float.NegativeInfinity;
			Vector3 result = position;
			for (int i = 0; i < 20; i++)
			{
				int num2 = Terrain.ToCell(position.X + this.m_random.Float(-20f, 20f));
				int num3 = Terrain.ToCell(position.Z + this.m_random.Float(-20f, 20f));
				int j = 255;
				while (j >= 0)
				{
					int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(num2, j, num3);
					if (BlocksManager.Blocks[cellContents].IsCollidable || cellContents == 18)
					{
						Vector3 vector = new Vector3((float)num2 + 0.5f, (float)j + 1.1f, (float)num3 + 0.5f);
						float num4 = this.ScoreSafePlace(position, vector, null);
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

		// Token: 0x06000D0A RID: 3338 RVA: 0x00062F10 File Offset: 0x00061110
		public float ScoreSafePlace(Vector3 currentPosition, Vector3 safePosition, Vector3? lookDirection)
		{
			float num = 16f;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), 16f, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (this.IsPredator(componentBody.Entity))
				{
					Vector3 position2 = componentBody.Position;
					Vector3 vector = safePosition - position2;
					if (lookDirection == null || 0f - Vector3.Dot(lookDirection.Value, vector) > 0f)
					{
						if (vector.Y >= 4f)
						{
							vector *= 2f;
						}
						num = MathUtils.Min(num, vector.Length());
					}
				}
			}
			float num2 = Vector3.Distance(currentPosition, safePosition);
			if (num2 < 8f)
			{
				return num * 0.5f;
			}
			return num * MathUtils.Lerp(1f, 0.75f, MathUtils.Saturate(num2 / 20f));
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x00063038 File Offset: 0x00061238
		public bool IsPredator(Entity entity)
		{
			if (entity != base.Entity)
			{
				ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && (componentCreature.Category == CreatureCategory.LandPredator || componentCreature.Category == CreatureCategory.WaterPredator || componentCreature.Category == CreatureCategory.LandOther))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040007DE RID: 2014
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040007DF RID: 2015
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040007E0 RID: 2016
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040007E1 RID: 2017
		public SubsystemTime m_subsystemTime;

		// Token: 0x040007E2 RID: 2018
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040007E3 RID: 2019
		public ComponentCreature m_componentCreature;

		// Token: 0x040007E4 RID: 2020
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040007E5 RID: 2021
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040007E6 RID: 2022
		public Game.Random m_random = new Game.Random();

		// Token: 0x040007E7 RID: 2023
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040007E8 RID: 2024
		public float m_importanceLevel;

		// Token: 0x040007E9 RID: 2025
		public double m_nextUpdateTime;
	}
}
