using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DC RID: 476
	public class ComponentFlu : Component, IUpdateable
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000CEB RID: 3307 RVA: 0x00061FAF File Offset: 0x000601AF
		public bool HasFlu
		{
			get
			{
				return this.m_fluDuration > 0f;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x00061FBE File Offset: 0x000601BE
		public bool IsCoughing
		{
			get
			{
				return this.m_coughDuration > 0f;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000CED RID: 3309 RVA: 0x00061FCD File Offset: 0x000601CD
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x00061FD0 File Offset: 0x000601D0
		public void StartFlu()
		{
			if (this.m_fluDuration == 0f)
			{
				this.m_componentPlayer.PlayerStats.TimesHadFlu += 1L;
			}
			this.m_fluDuration = 900f;
			this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 10.0, delegate
			{
				this.m_componentPlayer.ComponentVitalStats.MakeSleepy(0.2f);
			});
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0006203C File Offset: 0x0006023C
		public void Sneeze()
		{
			this.m_sneezeDuration = 1f;
			this.m_componentPlayer.ComponentCreatureSounds.PlaySneezeSound();
			base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x00062090 File Offset: 0x00060290
		public void Cough()
		{
			this.m_lastCoughTime = this.m_subsystemTime.GameTime;
			this.m_coughDuration = 4f;
			this.m_componentPlayer.ComponentCreatureSounds.PlayCoughSound();
			base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000620F4 File Offset: 0x000602F4
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.m_fluDuration = 0f;
				this.m_fluOnset = 0f;
				return;
			}
			if (this.m_fluDuration > 0f)
			{
				this.m_fluOnset = 0f;
				float num = 1f;
				if (this.m_componentPlayer.ComponentVitalStats.Temperature > 16f)
				{
					num = 2f;
				}
				else if (this.m_componentPlayer.ComponentVitalStats.Temperature > 12f)
				{
					num = 1.5f;
				}
				else if (this.m_componentPlayer.ComponentVitalStats.Temperature < 8f)
				{
					num = 0.5f;
				}
				this.m_fluDuration = MathUtils.Max(this.m_fluDuration - num * dt, 0f);
				if (this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping && this.m_subsystemTime.PeriodicGameTimeEvent(5.0, -0.009999999776482582) && this.m_subsystemTime.GameTime - this.m_lastEffectTime > 13.0)
				{
					this.FluEffect();
				}
			}
			else if (this.m_componentPlayer.ComponentVitalStats.Temperature < 6f)
			{
				float num2 = 13f;
				this.m_fluOnset += dt;
				if (this.m_fluOnset > 120f)
				{
					num2 = 9f;
					if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0) && this.m_random.Bool(0.025f))
					{
						this.StartFlu();
					}
					if (this.m_subsystemTime.GameTime - this.m_lastMessageTime > 60.0)
					{
						this.m_lastMessageTime = this.m_subsystemTime.GameTime;
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 1), Color.White, true, true);
					}
				}
				if (this.m_fluOnset > 60f && this.m_subsystemTime.PeriodicGameTimeEvent((double)num2, -0.009999999776482582) && this.m_random.Bool(0.75f))
				{
					this.Sneeze();
				}
			}
			else
			{
				this.m_fluOnset = 0f;
			}
			if ((this.m_coughDuration > 0f || this.m_sneezeDuration > 0f) && this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping)
			{
				this.m_coughDuration = MathUtils.Max(this.m_coughDuration - dt, 0f);
				this.m_sneezeDuration = MathUtils.Max(this.m_sneezeDuration - dt, 0f);
				float num3 = MathUtils.DegToRad(MathUtils.Lerp(-35f, -65f, SimplexNoise.Noise(4f * (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 10000.0))));
				this.m_componentPlayer.ComponentLocomotion.LookOrder = new Vector2(this.m_componentPlayer.ComponentLocomotion.LookOrder.X, MathUtils.Clamp(num3 - this.m_componentPlayer.ComponentLocomotion.LookAngles.Y, -3f, 3f));
				if (this.m_random.Bool(2f * dt))
				{
					this.m_componentPlayer.ComponentBody.ApplyImpulse(-1.2f * this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector());
				}
			}
			if (this.m_blackoutDuration > 0f)
			{
				this.m_blackoutDuration = MathUtils.Max(this.m_blackoutDuration - dt, 0f);
				this.m_blackoutFactor = MathUtils.Min(this.m_blackoutFactor + 0.5f * dt, 0.95f);
			}
			else if (this.m_blackoutFactor > 0f)
			{
				this.m_blackoutFactor = MathUtils.Max(this.m_blackoutFactor - 0.5f * dt, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_blackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
		}

        // Token: 0x06000CF2 RID: 3314 RVA: 0x0006254C File Offset: 0x0006074C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_fluDuration = valuesDictionary.GetValue<float>("FluDuration");
			this.m_fluOnset = valuesDictionary.GetValue<float>("FluOnset");
		}

        // Token: 0x06000CF3 RID: 3315 RVA: 0x000625E7 File Offset: 0x000607E7
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("FluDuration", this.m_fluDuration);
			valuesDictionary.SetValue<float>("FluOnset", this.m_fluOnset);
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0006260C File Offset: 0x0006080C
		public void FluEffect()
		{
			this.m_lastEffectTime = this.m_subsystemTime.GameTime;
			this.m_blackoutDuration = MathUtils.Lerp(4f, 2f, this.m_componentPlayer.ComponentHealth.Health);
			float injury = MathUtils.Min(0.1f, this.m_componentPlayer.ComponentHealth.Health - 0.175f);
			if (injury > 0f)
			{
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.75, delegate
				{
					this.m_componentPlayer.ComponentHealth.Injure(injury, null, false, LanguageControl.Get(ComponentFlu.fName, 4));
				});
			}
			if (Time.FrameStartTime - this.m_lastMessageTime > 60.0)
			{
				this.m_lastMessageTime = Time.FrameStartTime;
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.5, delegate
				{
					if (this.m_componentPlayer.ComponentVitalStats.Temperature < 8f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 2), Color.White, true, true);
						return;
					}
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 3), Color.White, true, true);
				});
			}
			if (this.m_coughDuration == 0f && (this.m_subsystemTime.GameTime - this.m_lastCoughTime > 40.0 || this.m_random.Bool(0.5f)))
			{
				this.Cough();
				return;
			}
			if (this.m_sneezeDuration == 0f)
			{
				this.Sneeze();
			}
		}

		// Token: 0x040007C6 RID: 1990
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040007C7 RID: 1991
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040007C8 RID: 1992
		public SubsystemTime m_subsystemTime;

		// Token: 0x040007C9 RID: 1993
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040007CA RID: 1994
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040007CB RID: 1995
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040007CC RID: 1996
		public Game.Random m_random = new Game.Random();

		// Token: 0x040007CD RID: 1997
		public float m_fluOnset;

		// Token: 0x040007CE RID: 1998
		public static string fName = "ComponentFlu";

		// Token: 0x040007CF RID: 1999
		public float m_fluDuration;

		// Token: 0x040007D0 RID: 2000
		public float m_coughDuration;

		// Token: 0x040007D1 RID: 2001
		public float m_sneezeDuration;

		// Token: 0x040007D2 RID: 2002
		public float m_blackoutDuration;

		// Token: 0x040007D3 RID: 2003
		public float m_blackoutFactor;

		// Token: 0x040007D4 RID: 2004
		public double m_lastEffectTime = -1000.0;

		// Token: 0x040007D5 RID: 2005
		public double m_lastCoughTime = -1000.0;

		// Token: 0x040007D6 RID: 2006
		public double m_lastMessageTime = -1000.0;
	}
}
