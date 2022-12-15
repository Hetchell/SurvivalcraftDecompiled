using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020B RID: 523
	public class ComponentSwimAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000241 RID: 577
		// (get) Token: 0x0600100F RID: 4111 RVA: 0x00079FB4 File Offset: 0x000781B4
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00079FB7 File Offset: 0x000781B7
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x00079FC0 File Offset: 0x000781C0
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_random.Float(0f, 1f) < 0.05f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 3f);
			}
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

        // Token: 0x06001012 RID: 4114 RVA: 0x0007A04C File Offset: 0x0007824C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Swim");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_importanceLevel = 1f;
				}
				this.m_stateMachine.TransitionTo("Swim");
			}, null, null);
			this.m_stateMachine.AddState("Swim", delegate
			{
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination != null)
				{
					if (this.m_componentPathfinding.IsStuck)
					{
						this.m_stateMachine.TransitionTo("Stuck");
					}
					return;
				}
				Vector3? destination = this.FindDestination();
				if (destination != null)
				{
					this.m_componentPathfinding.SetDestination(destination, this.m_random.Float(0.3f, 0.4f), 1f, 0, false, true, false, null);
					return;
				}
				this.m_importanceLevel = 1f;
			}, null);
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x0007A0F4 File Offset: 0x000782F4
		public Vector3? FindDestination()
		{
			Vector3 vector = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
			float num = 2f;
			Vector3? result = null;
			float num2 = this.m_random.Float(10f, 16f);
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector2 = this.m_random.Vector2(1f, 1f);
				float y = 0.3f * this.m_random.Float(-0.9f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector2.X, y, vector2.Y));
				Vector3 vector3 = vector + num2 * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, vector3, false, false, delegate(int value, float d)
				{
					int num3 = Terrain.ExtractContents(value);
					return !(BlocksManager.Blocks[num3] is WaterBlock);
				});
				if (terrainRaycastResult == null)
				{
					if (num2 > num)
					{
						result = new Vector3?(vector3);
						num = num2;
					}
				}
				else if (terrainRaycastResult.Value.Distance > num)
				{
					result = new Vector3?(vector + v * terrainRaycastResult.Value.Distance);
					num = terrainRaycastResult.Value.Distance;
				}
			}
			return result;
		}

		// Token: 0x04000A74 RID: 2676
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A75 RID: 2677
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A76 RID: 2678
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000A77 RID: 2679
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A78 RID: 2680
		public float m_importanceLevel = 1f;

		// Token: 0x04000A79 RID: 2681
		public Game.Random m_random = new Game.Random();
	}
}
