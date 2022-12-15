using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E7 RID: 487
	public class ComponentHowlBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x00068087 File Offset: 0x00066287
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x0006808A File Offset: 0x0006628A
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x00068092 File Offset: 0x00066292
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

        // Token: 0x06000DB5 RID: 3509 RVA: 0x000680A0 File Offset: 0x000662A0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_howlSoundName = valuesDictionary.GetValue<string>("HowlSoundName");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Howl");
				}
				if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
				{
					if (this.m_random.Float(0f, 1f) < 0.015f * this.m_subsystemTime.GameTimeDelta)
					{
						this.m_importanceLevel = this.m_random.Float(1f, 3f);
						return;
					}
				}
				else
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.AddState("Howl", delegate
			{
				this.m_howlTime = 0f;
				this.m_howlDuration = this.m_random.Float(5f, 6f);
				this.m_componentPathfinding.Stop();
				this.m_importanceLevel = 10f;
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(this.m_componentCreature.ComponentLocomotion.LookOrder.X, 2f);
				float num = this.m_howlTime + this.m_subsystemTime.GameTimeDelta;
				if (this.m_howlTime <= 0.5f && num > 0.5f)
				{
					this.m_subsystemAudio.PlayRandomSound(this.m_howlSoundName, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, 10f, true);
				}
				this.m_howlTime = num;
				if (this.m_howlTime >= this.m_howlDuration)
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x04000880 RID: 2176
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000881 RID: 2177
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000882 RID: 2178
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000883 RID: 2179
		public ComponentCreature m_componentCreature;

		// Token: 0x04000884 RID: 2180
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000885 RID: 2181
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000886 RID: 2182
		public float m_importanceLevel;

		// Token: 0x04000887 RID: 2183
		public string m_howlSoundName;

		// Token: 0x04000888 RID: 2184
		public float m_howlTime;

		// Token: 0x04000889 RID: 2185
		public float m_howlDuration;

		// Token: 0x0400088A RID: 2186
		public Game.Random m_random = new Game.Random();
	}
}
