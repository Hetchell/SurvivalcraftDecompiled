using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000203 RID: 515
	public class ComponentSickness : Component, IUpdateable
	{
		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x000779F1 File Offset: 0x00075BF1
		public bool IsSick
		{
			get
			{
				return this.m_sicknessDuration > 0f;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00077A00 File Offset: 0x00075C00
		public bool IsPuking
		{
			get
			{
				return this.m_pukeParticleSystem != null;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000FAA RID: 4010 RVA: 0x00077A0B File Offset: 0x00075C0B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00077A0E File Offset: 0x00075C0E
		public void StartSickness()
		{
			if (this.m_sicknessDuration == 0f)
			{
				this.m_componentPlayer.PlayerStats.TimesWasSick += 1L;
			}
			this.m_sicknessDuration = 900f;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00077A44 File Offset: 0x00075C44
		public void NauseaEffect()
		{
			this.m_lastNauseaTime = new double?(this.m_subsystemTime.GameTime);
			this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
			float injury = MathUtils.Min(0.1f, this.m_componentPlayer.ComponentHealth.Health - 0.075f);
			if (injury > 0f)
			{
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.75, delegate
				{
					this.m_componentPlayer.ComponentHealth.Injure(injury, null, false, LanguageControl.Get(ComponentSickness.fName, 1));
				});
			}
			if (this.m_pukeParticleSystem == null)
			{
				if (this.m_lastPukeTime != null)
				{
					double? num = this.m_subsystemTime.GameTime - this.m_lastPukeTime;
					double num2 = 50.0;
					if (!(num.GetValueOrDefault() > num2 & num != null))
					{
						goto IL_19B;
					}
				}
				this.m_lastPukeTime = new double?(this.m_subsystemTime.GameTime);
				this.m_pukeParticleSystem = new PukeParticleSystem(this.m_subsystemTerrain);
				this.m_subsystemParticles.AddParticleSystem(this.m_pukeParticleSystem);
				this.m_componentPlayer.ComponentCreatureSounds.PlayPukeSound();
				base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
				this.m_greenoutDuration = 0.8f;
				this.m_componentPlayer.PlayerStats.TimesPuked += 1L;
				return;
			}
			IL_19B:
			this.m_greenoutDuration = MathUtils.Lerp(4f, 2f, this.m_componentPlayer.ComponentHealth.Health);
			if (this.m_lastMessageTime != null)
			{
				double? num = Time.FrameStartTime - this.m_lastMessageTime;
				double num2 = 60.0;
				if (!(num.GetValueOrDefault() > num2 & num != null))
				{
					return;
				}
			}
			this.m_lastMessageTime = new double?(Time.FrameStartTime);
			this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.5, delegate
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentSickness.fName, 2), Color.White, true, true);
			});
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x00077CAC File Offset: 0x00075EAC
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.m_sicknessDuration = 0f;
				return;
			}
			if (this.m_sicknessDuration > 0f)
			{
				this.m_sicknessDuration = MathUtils.Max(this.m_sicknessDuration - dt, 0f);
				if (this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping && this.m_subsystemTime.PeriodicGameTimeEvent(3.0, -0.009999999776482582))
				{
					if (this.m_lastNauseaTime != null)
					{
						double? num = this.m_subsystemTime.GameTime - this.m_lastNauseaTime;
						double num2 = 15.0;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							goto IL_10E;
						}
					}
					this.NauseaEffect();
				}
			}
			IL_10E:
			if (this.m_pukeParticleSystem != null)
			{
				float num3 = MathUtils.DegToRad(MathUtils.Lerp(-35f, -60f, SimplexNoise.Noise(2f * (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 10000.0))));
				this.m_componentPlayer.ComponentLocomotion.LookOrder = new Vector2(this.m_componentPlayer.ComponentLocomotion.LookOrder.X, MathUtils.Clamp(num3 - this.m_componentPlayer.ComponentLocomotion.LookAngles.Y, -2f, 2f));
				Vector3 upVector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetUpVector();
				Vector3 forwardVector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector();
				this.m_pukeParticleSystem.Position = this.m_componentPlayer.ComponentCreatureModel.EyePosition - 0.08f * upVector + 0.3f * forwardVector;
				this.m_pukeParticleSystem.Direction = Vector3.Normalize(forwardVector + 0.5f * upVector);
				if (this.m_pukeParticleSystem.IsStopped)
				{
					this.m_pukeParticleSystem = null;
				}
			}
			if (this.m_greenoutDuration > 0f)
			{
				this.m_greenoutDuration = MathUtils.Max(this.m_greenoutDuration - dt, 0f);
				this.m_greenoutFactor = MathUtils.Min(this.m_greenoutFactor + 0.5f * dt, 0.95f);
			}
			else if (this.m_greenoutFactor > 0f)
			{
				this.m_greenoutFactor = MathUtils.Max(this.m_greenoutFactor - 0.5f * dt, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.GreenoutFactor = MathUtils.Max(this.m_greenoutFactor, this.m_componentPlayer.ComponentScreenOverlays.GreenoutFactor);
		}

        // Token: 0x06000FAE RID: 4014 RVA: 0x00077FA4 File Offset: 0x000761A4
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_sicknessDuration = valuesDictionary.GetValue<float>("SicknessDuration");
		}

        // Token: 0x06000FAF RID: 4015 RVA: 0x0007801C File Offset: 0x0007621C
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("SicknessDuration", this.m_sicknessDuration);
		}

		// Token: 0x04000A18 RID: 2584
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A19 RID: 2585
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A1A RID: 2586
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A1B RID: 2587
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000A1C RID: 2588
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000A1D RID: 2589
		public PukeParticleSystem m_pukeParticleSystem;

		// Token: 0x04000A1E RID: 2590
		public float m_sicknessDuration;

		// Token: 0x04000A1F RID: 2591
		public float m_greenoutDuration;

		// Token: 0x04000A20 RID: 2592
		public float m_greenoutFactor;

		// Token: 0x04000A21 RID: 2593
		public double? m_lastNauseaTime;

		// Token: 0x04000A22 RID: 2594
		public double? m_lastMessageTime;

		// Token: 0x04000A23 RID: 2595
		public double? m_lastPukeTime;

		// Token: 0x04000A24 RID: 2596
		public static string fName = "ComponentSickness";
	}
}
