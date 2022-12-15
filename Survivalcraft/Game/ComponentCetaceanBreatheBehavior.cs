using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C8 RID: 456
	public class ComponentCetaceanBreatheBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000BCD RID: 3021 RVA: 0x00059398 File Offset: 0x00057598
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000BCE RID: 3022 RVA: 0x0005939B File Offset: 0x0005759B
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x000593A3 File Offset: 0x000575A3
		public void Update(float dt)
		{
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			this.m_stateMachine.Update();
		}

        // Token: 0x06000BD0 RID: 3024 RVA: 0x000593C8 File Offset: 0x000575C8
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				this.m_importanceLevel = MathUtils.Lerp(0f, 400f, MathUtils.Saturate((0.75f - this.m_componentCreature.ComponentHealth.Air) / 0.75f));
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Surface");
				}
			}, null);
			this.m_stateMachine.AddState("Surface", delegate
			{
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination == null)
				{
					Vector3? destination = this.FindSurfaceDestination();
					if (destination != null)
					{
						float speed = (this.m_componentCreature.ComponentHealth.Air < 0.25f) ? 1f : this.m_random.Float(0.4f, 0.6f);
						this.m_componentPathfinding.SetDestination(destination, speed, 1f, 0, false, false, false, null);
					}
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_componentCreature.ComponentHealth.Air > 0.9f)
				{
					this.m_stateMachine.TransitionTo("Breathe");
				}
			}, null);
			this.m_stateMachine.AddState("Breathe", delegate
			{
				Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
				Vector3 value = this.m_componentCreature.ComponentBody.Matrix.Translation + 10f * forward + new Vector3(0f, 2f, 0f);
				this.m_componentPathfinding.SetDestination(new Vector3?(value), 0.6f, 1f, 0, false, false, false, null);
				this.m_particleSystem = new WhalePlumeParticleSystem(this.m_subsystemTerrain, this.m_random.Float(0.8f, 1.1f), this.m_random.Float(1f, 1.3f));
				this.m_subsystemParticles.AddParticleSystem(this.m_particleSystem);
				this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/WhaleBlow", 1f, this.m_random.Float(-0.2f, 0.2f), this.m_componentCreature.ComponentBody.Position, 10f, true);
			}, delegate
			{
				this.m_particleSystem.Position = this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0.8f * this.m_componentCreature.ComponentBody.BoxSize.Y, 0f);
				if (!this.m_subsystemParticles.ContainsParticleSystem(this.m_particleSystem))
				{
					this.m_importanceLevel = 0f;
				}
			}, delegate
			{
				this.m_particleSystem.IsStopped = true;
				this.m_particleSystem = null;
			});
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x000594AC File Offset: 0x000576AC
		public Vector3? FindSurfaceDestination()
		{
			Vector3 vector = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
			Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
			float s = 2f * this.m_componentCreature.ComponentBody.ImmersionDepth;
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector2 = (i < 4) ? (new Vector2(forward.X, forward.Z) + this.m_random.Vector2(0f, 0.25f)) : this.m_random.Vector2(0.5f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector2.X, 1f, vector2.Y));
				Vector3 end = vector + s * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, end, false, false, (int value, float d) => Terrain.ExtractContents(value) != 18);
				if (terrainRaycastResult != null && Terrain.ExtractContents(terrainRaycastResult.Value.Value) == 0)
				{
					return new Vector3?(new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f));
				}
			}
			return null;
		}

		// Token: 0x040006A3 RID: 1699
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040006A4 RID: 1700
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040006A5 RID: 1701
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040006A6 RID: 1702
		public ComponentCreature m_componentCreature;

		// Token: 0x040006A7 RID: 1703
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040006A8 RID: 1704
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040006A9 RID: 1705
		public Game.Random m_random = new Game.Random();

		// Token: 0x040006AA RID: 1706
		public WhalePlumeParticleSystem m_particleSystem;

		// Token: 0x040006AB RID: 1707
		public float m_importanceLevel;
	}
}
