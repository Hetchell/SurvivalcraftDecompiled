using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D3 RID: 467
	public class ComponentDigInMudBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0005E17E File Offset: 0x0005C37E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000C93 RID: 3219 RVA: 0x0005E181 File Offset: 0x0005C381
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0005E189 File Offset: 0x0005C389
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			this.m_collidedWithBody = null;
		}

        // Token: 0x06000C95 RID: 3221 RVA: 0x0005E1A0 File Offset: 0x0005C3A0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_componentFishModel = base.Entity.FindComponent<ComponentFishModel>(true);
			this.m_componentSwimAwayBehavior = base.Entity.FindComponent<ComponentSwimAwayBehavior>(true);
			string digInBlockName = valuesDictionary.GetValue<string>("DigInBlockName");
			this.m_digInBlockIndex = ((!string.IsNullOrEmpty(digInBlockName)) ? BlocksManager.Blocks.First((Block b) => b.GetType().Name == digInBlockName).BlockIndex : 0);
			this.m_maxDigInDepth = valuesDictionary.GetValue<float>("MaxDigInDepth");
			this.m_componentCreature.ComponentBody.CollidedWithBody += delegate(ComponentBody b)
			{
				this.m_collidedWithBody = b;
			};
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f * this.m_subsystemTime.GameTimeDelta && this.m_subsystemTime.GameTime > this.m_digOutTime + 15.0 && this.m_digInBlockIndex != 0)
				{
					int x = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.X);
					int y = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.Y - 0.9f);
					int z = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.Z);
					if (this.m_subsystemTerrain.Terrain.GetCellContents(x, y, z) == this.m_digInBlockIndex)
					{
						this.m_importanceLevel = this.m_random.Float(1f, 3f);
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Sink");
				}
			}, null);
			this.m_stateMachine.AddState("Sink", delegate
			{
				this.m_importanceLevel = 10f;
				this.m_sinkTime = this.m_subsystemTime.GameTime;
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 2f * this.m_subsystemTime.GameTimeDelta)
				{
					int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
					int digInBlockIndex = this.m_digInBlockIndex;
					if ((standingOnValue.GetValueOrDefault() == digInBlockIndex & standingOnValue != null) && this.m_componentCreature.ComponentBody.getVectorSpeed().LengthSquared() < 1f)
					{
						this.m_stateMachine.TransitionTo("DigIn");
					}
				}
				if (!this.IsActive || this.m_subsystemTime.GameTime > this.m_sinkTime + 6.0)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
			}, null);
			this.m_stateMachine.AddState("DigIn", delegate
			{
				this.m_digInTime = this.m_subsystemTime.GameTime;
				this.m_digOutTime = this.m_digInTime + (double)this.m_random.Float(30f, 60f);
			}, delegate
			{
				this.m_componentFishModel.DigInOrder = this.m_maxDigInDepth;
				if (this.m_collidedWithBody != null)
				{
					if (this.m_subsystemTime.GameTime - this.m_digInTime > 2.0 && this.m_collidedWithBody.Density < 0.95f)
					{
						this.m_componentMiner.Hit(this.m_collidedWithBody, this.m_collidedWithBody.Position, Vector3.Normalize(this.m_collidedWithBody.Position - this.m_componentCreature.ComponentBody.Position));
					}
					this.m_componentSwimAwayBehavior.SwimAwayFrom(this.m_collidedWithBody);
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.IsActive && this.m_subsystemTime.GameTime < this.m_digOutTime)
				{
					int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
					int digInBlockIndex = this.m_digInBlockIndex;
					if ((standingOnValue.GetValueOrDefault() == digInBlockIndex & standingOnValue != null) && this.m_componentCreature.ComponentBody.getVectorSpeed().LengthSquared() <= 1f)
					{
						return;
					}
				}
				this.m_stateMachine.TransitionTo("Inactive");
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x0005E334 File Offset: 0x0005C534
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

		// Token: 0x04000748 RID: 1864
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000749 RID: 1865
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400074A RID: 1866
		public ComponentCreature m_componentCreature;

		// Token: 0x0400074B RID: 1867
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400074C RID: 1868
		public ComponentMiner m_componentMiner;

		// Token: 0x0400074D RID: 1869
		public ComponentFishModel m_componentFishModel;

		// Token: 0x0400074E RID: 1870
		public ComponentSwimAwayBehavior m_componentSwimAwayBehavior;

		// Token: 0x0400074F RID: 1871
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000750 RID: 1872
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000751 RID: 1873
		public float m_importanceLevel;

		// Token: 0x04000752 RID: 1874
		public double m_sinkTime;

		// Token: 0x04000753 RID: 1875
		public double m_digInTime;

		// Token: 0x04000754 RID: 1876
		public double m_digOutTime = double.NegativeInfinity;

		// Token: 0x04000755 RID: 1877
		public float m_maxDigInDepth;

		// Token: 0x04000756 RID: 1878
		public int m_digInBlockIndex;

		// Token: 0x04000757 RID: 1879
		public ComponentBody m_collidedWithBody;
	}
}
