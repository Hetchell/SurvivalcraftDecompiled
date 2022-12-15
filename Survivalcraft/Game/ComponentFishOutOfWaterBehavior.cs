using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DA RID: 474
	public class ComponentFishOutOfWaterBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000CDD RID: 3293 RVA: 0x00061246 File Offset: 0x0005F446
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x00061249 File Offset: 0x0005F449
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x00061251 File Offset: 0x0005F451
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000CE0 RID: 3296 RVA: 0x00061260 File Offset: 0x0005F460
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentFishModel = base.Entity.FindComponent<ComponentFishModel>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsOutOfWater())
				{
					this.m_outOfWaterTime += this.m_subsystemTime.GameTimeDelta;
				}
				else
				{
					this.m_outOfWaterTime = 0f;
				}
				if (this.m_outOfWaterTime > 3f)
				{
					this.m_importanceLevel = 1000f;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Jump");
				}
			}, null);
			this.m_stateMachine.AddState("Jump", null, delegate
			{
				this.m_componentFishModel.BendOrder = new float?(2f * (2f * MathUtils.Saturate(SimplexNoise.OctavedNoise((float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0), 1.2f * this.m_componentCreature.ComponentLocomotion.TurnSpeed, 1, 1f, 1f, false)) - 1f));
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (!this.IsOutOfWater())
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 2.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentLocomotion.JumpOrder = this.m_random.Float(0.33f, 1f);
					this.m_direction = new Vector2(MathUtils.Sign(this.m_componentFishModel.BendOrder.Value), 0f);
				}
				if (this.m_componentCreature.ComponentBody.StandingOnValue == null)
				{
					this.m_componentCreature.ComponentLocomotion.TurnOrder = new Vector2(0f - this.m_componentFishModel.BendOrder.Value, 0f);
					this.m_componentCreature.ComponentLocomotion.WalkOrder = new Vector2?(this.m_direction);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x00061301 File Offset: 0x0005F501
		public bool IsOutOfWater()
		{
			return this.m_componentCreature.ComponentBody.ImmersionFactor < 0.33f;
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x0006131C File Offset: 0x0005F51C
		public Vector3? FindDestination()
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector = this.m_random.Vector2(1f, 1f);
				float y = 0.2f * this.m_random.Float(-0.8f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector.X, y, vector.Y));
				Vector3 vector2 = this.m_componentCreature.ComponentBody.Position + this.m_random.Float(8f, 16f) * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(this.m_componentCreature.ComponentBody.Position, vector2, false, false, delegate(int value, float d)
				{
					int num = Terrain.ExtractContents(value);
					return !(BlocksManager.Blocks[num] is WaterBlock);
				});
				if (terrainRaycastResult == null)
				{
					return new Vector3?(vector2);
				}
				if (terrainRaycastResult.Value.Distance > 4f)
				{
					return new Vector3?(this.m_componentCreature.ComponentBody.Position + v * terrainRaycastResult.Value.Distance);
				}
			}
			return null;
		}

		// Token: 0x040007AE RID: 1966
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040007AF RID: 1967
		public SubsystemTime m_subsystemTime;

		// Token: 0x040007B0 RID: 1968
		public ComponentCreature m_componentCreature;

		// Token: 0x040007B1 RID: 1969
		public ComponentFishModel m_componentFishModel;

		// Token: 0x040007B2 RID: 1970
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040007B3 RID: 1971
		public float m_importanceLevel;

		// Token: 0x040007B4 RID: 1972
		public float m_outOfWaterTime;

		// Token: 0x040007B5 RID: 1973
		public Vector2 m_direction;

		// Token: 0x040007B6 RID: 1974
		public Game.Random m_random = new Game.Random();
	}
}
