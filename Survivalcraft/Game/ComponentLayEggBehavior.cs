using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EE RID: 494
	public class ComponentLayEggBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x0006C6A7 File Offset: 0x0006A8A7
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0006C6AA File Offset: 0x0006A8AA
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x0006C6B4 File Offset: 0x0006A8B4
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Move");
			}
			if (this.m_eggType != null && this.m_random.Float(0f, 1f) < this.m_layFrequency * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2f);
			}
			this.m_dt = dt;
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

        // Token: 0x06000E07 RID: 3591 RVA: 0x0006C750 File Offset: 0x0006A950
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			EggBlock eggBlock = (EggBlock)BlocksManager.Blocks[118];
			this.m_layFrequency = valuesDictionary.GetValue<float>("LayFrequency");
			this.m_eggType = eggBlock.GetEggTypeByCreatureTemplateName(base.Entity.ValuesDictionary.DatabaseObject.Name);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Move");
			}, null, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				float num = 5f;
				Vector3 vector = position + new Vector3(num * this.m_random.Float(-1f, 1f), 0f, num * this.m_random.Float(-1f, 1f));
				vector.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Z)) + 1);
				this.m_componentPathfinding.SetDestination(new Vector3?(vector), this.m_random.Float(0.4f, 0.6f), 0.5f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Lay");
					return;
				}
				if (this.m_componentPathfinding.IsStuck)
				{
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_stateMachine.TransitionTo("Stuck");
						return;
					}
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.AddState("Lay", delegate
			{
				this.m_layTime = 0f;
			}, delegate
			{
				if (this.m_eggType != null)
				{
					this.m_layTime += this.m_dt;
					if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
					{
						this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(0f, 0.25f * (float)MathUtils.Sin(20.0 * this.m_subsystemTime.GameTime) + this.m_layTime / 3f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
						if (this.m_layTime >= 3f)
						{
							this.m_importanceLevel = 0f;
							int value = Terrain.MakeBlockValue(118, 0, EggBlock.SetIsLaid(EggBlock.SetEggType(0, this.m_eggType.EggTypeIndex), true));
							Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
							Vector3 position = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
							Vector3 value2 = 3f * Vector3.Normalize(-matrix.Forward + 0.1f * matrix.Up + 0.2f * this.m_random.Float(-1f, 1f) * matrix.Right);
							this.m_subsystemPickables.AddPickable(value, 1, position, new Vector3?(value2), null);
							this.m_subsystemAudio.PlaySound("Audio/EggLaid", 1f, this.m_random.Float(-0.1f, 0.1f), position, 2f, true);
							return;
						}
					}
					else if (this.m_layTime >= 3f)
					{
						this.m_importanceLevel = 0f;
						return;
					}
				}
				else
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
		}

		// Token: 0x040008D0 RID: 2256
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008D1 RID: 2257
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008D2 RID: 2258
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040008D3 RID: 2259
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040008D4 RID: 2260
		public ComponentCreature m_componentCreature;

		// Token: 0x040008D5 RID: 2261
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040008D6 RID: 2262
		public EggBlock.EggType m_eggType;

		// Token: 0x040008D7 RID: 2263
		public float m_layFrequency;

		// Token: 0x040008D8 RID: 2264
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040008D9 RID: 2265
		public float m_importanceLevel;

		// Token: 0x040008DA RID: 2266
		public float m_dt;

		// Token: 0x040008DB RID: 2267
		public float m_layTime;

		// Token: 0x040008DC RID: 2268
		public Game.Random m_random = new Game.Random();
	}
}
