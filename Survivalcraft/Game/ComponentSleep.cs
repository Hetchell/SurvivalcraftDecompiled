using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000205 RID: 517
	public class ComponentSleep : Component, IUpdateable
	{
		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0007817A File Offset: 0x0007637A
		public bool IsSleeping
		{
			get
			{
				return this.m_sleepStartTime != null;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x00078187 File Offset: 0x00076387
		public float SleepFactor
		{
			get
			{
				return this.m_sleepFactor;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0007818F File Offset: 0x0007638F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x00078194 File Offset: 0x00076394
		public bool CanSleep(out string reason)
		{
			Block block = (this.m_componentPlayer.ComponentBody.StandingOnValue != null) ? BlocksManager.Blocks[Terrain.ExtractContents(this.m_componentPlayer.ComponentBody.StandingOnValue.Value)] : null;
			if (block == null || this.m_componentPlayer.ComponentBody.ImmersionDepth > 0f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 1);
				return false;
			}
			if (block != null && block.SleepSuitability == 0f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 2);
				return false;
			}
			if (this.m_componentPlayer.ComponentVitalStats.Sleep > 0.99f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 3);
				return false;
			}
			if (this.m_componentPlayer.ComponentVitalStats.Wetness > 0.95f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 4);
				return false;
			}
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					Vector3 vector = this.m_componentPlayer.ComponentBody.Position + new Vector3((float)i, 1f, (float)j);
					Vector3 end = new Vector3(vector.X, 255f, vector.Z);
					if (this.m_subsystemTerrain.Raycast(vector, end, false, true, (int value, float distance) => Terrain.ExtractContents(value) != 0) == null)
					{
						reason = LanguageControl.Get(ComponentSleep.fName, 5);
						return false;
					}
				}
			}
			reason = string.Empty;
			return true;
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x0007832C File Offset: 0x0007652C
		public void Sleep(bool allowManualWakeup)
		{
			if (!this.IsSleeping)
			{
				this.m_sleepStartTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
				this.m_allowManualWakeUp = allowManualWakeup;
				this.m_minWetness = float.MaxValue;
				this.m_messageFactor = 0f;
				if (this.m_componentPlayer.PlayerStats != null)
				{
					this.m_componentPlayer.PlayerStats.TimesWentToSleep += 1L;
				}
			}
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x0007839C File Offset: 0x0007659C
		public void WakeUp()
		{
			if (this.m_sleepStartTime != null)
			{
				this.m_sleepStartTime = null;
				this.m_componentPlayer.PlayerData.SpawnPosition = this.m_componentPlayer.ComponentBody.Position + new Vector3(0f, 0.1f, 0f);
			}
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x000783FC File Offset: 0x000765FC
		public void Update(float dt)
		{
			if (this.IsSleeping && this.m_componentPlayer.ComponentHealth.Health > 0f)
			{
				this.m_sleepFactor = MathUtils.Min(this.m_sleepFactor + 0.33f * Time.FrameDuration, 1f);
				this.m_minWetness = MathUtils.Min(this.m_minWetness, this.m_componentPlayer.ComponentVitalStats.Wetness);
				this.m_componentPlayer.PlayerStats.TimeSlept += (double)this.m_subsystemGameInfo.TotalElapsedGameTimeDelta;
				if ((this.m_componentPlayer.ComponentVitalStats.Sleep >= 1f || this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) && this.m_subsystemTimeOfDay.TimeOfDay > 0.3f && this.m_subsystemTimeOfDay.TimeOfDay < 0.59999996f && this.m_sleepStartTime != null)
				{
					double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
					double? num = this.m_sleepStartTime + 180.0;
					if (totalElapsedGameTime > num.GetValueOrDefault() & num != null)
					{
						this.WakeUp();
					}
				}
				if (this.m_componentPlayer.ComponentHealth.HealthChange < 0f && (this.m_componentPlayer.ComponentHealth.Health < 0.5f || this.m_componentPlayer.ComponentVitalStats.Sleep > 0.5f))
				{
					this.WakeUp();
				}
				if (this.m_componentPlayer.ComponentVitalStats.Wetness > this.m_minWetness + 0.05f && this.m_componentPlayer.ComponentVitalStats.Sleep > 0.2f)
				{
					this.WakeUp();
					this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.0, delegate
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentSleep.fName, 6), Color.White, true, true);
					});
				}
				if (this.m_sleepStartTime != null)
				{
					float num2 = (float)(this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_sleepStartTime.Value);
					if (this.m_allowManualWakeUp && num2 > 10f)
					{
						if (this.m_componentPlayer.GameWidget.Input.Any && !DialogsManager.HasDialogs(this.m_componentPlayer.GameWidget))
						{
							this.m_componentPlayer.GameWidget.Input.Clear();
							this.WakeUp();
							this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 2.0, delegate
							{
								this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentSleep.fName, 7), Color.White, true, false);
							});
						}
						this.m_messageFactor = MathUtils.Min(this.m_messageFactor + 0.5f * Time.FrameDuration, 1f);
						this.m_componentPlayer.ComponentScreenOverlays.Message = LanguageControl.Get(ComponentSleep.fName, 8);
						this.m_componentPlayer.ComponentScreenOverlays.MessageFactor = this.m_messageFactor;
					}
					if (!this.m_allowManualWakeUp && num2 > 5f)
					{
						this.m_messageFactor = MathUtils.Min(this.m_messageFactor + 1f * Time.FrameDuration, 1f);
						this.m_componentPlayer.ComponentScreenOverlays.Message = LanguageControl.Get(ComponentSleep.fName, 9);
						this.m_componentPlayer.ComponentScreenOverlays.MessageFactor = this.m_messageFactor;
					}
				}
			}
			else
			{
				this.m_sleepFactor = MathUtils.Max(this.m_sleepFactor - 1f * Time.FrameDuration, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor, this.m_sleepFactor);
			if (this.m_sleepFactor > 0.01f)
			{
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentSleep.fName, 10);
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_sleepFactor - 0.9f));
			}
		}

        // Token: 0x06000FBC RID: 4028 RVA: 0x00078808 File Offset: 0x00076A08
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemUpdate = base.Project.FindSubsystem<SubsystemUpdate>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_sleepStartTime = new double?(valuesDictionary.GetValue<double>("SleepStartTime"));
			this.m_allowManualWakeUp = valuesDictionary.GetValue<bool>("AllowManualWakeUp");
			double? sleepStartTime = this.m_sleepStartTime;
			double num = 0.0;
			if (sleepStartTime.GetValueOrDefault() == num & sleepStartTime != null)
			{
				this.m_sleepStartTime = null;
			}
			if (this.m_sleepStartTime != null)
			{
				this.m_sleepFactor = 1f;
				this.m_minWetness = float.MaxValue;
			}
			this.m_componentPlayer.ComponentHealth.Attacked += delegate(ComponentCreature p)
			{
				if (this.IsSleeping && this.m_componentPlayer.ComponentVitalStats.Sleep > 0.25f)
				{
					this.WakeUp();
				}
			};
		}

        // Token: 0x06000FBD RID: 4029 RVA: 0x0007892C File Offset: 0x00076B2C
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("SleepStartTime", (this.m_sleepStartTime != null) ? this.m_sleepStartTime.Value : 0.0);
			valuesDictionary.SetValue<bool>("AllowManualWakeUp", this.m_allowManualWakeUp);
		}

		// Token: 0x04000A27 RID: 2599
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x04000A28 RID: 2600
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A29 RID: 2601
		public SubsystemUpdate m_subsystemUpdate;

		// Token: 0x04000A2A RID: 2602
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A2B RID: 2603
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000A2C RID: 2604
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A2D RID: 2605
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000A2E RID: 2606
		public double? m_sleepStartTime;

		// Token: 0x04000A2F RID: 2607
		public float m_sleepFactor;

		// Token: 0x04000A30 RID: 2608
		public bool m_allowManualWakeUp;

		// Token: 0x04000A31 RID: 2609
		public static string fName = "ComponentSleep";

		// Token: 0x04000A32 RID: 2610
		public float m_minWetness;

		// Token: 0x04000A33 RID: 2611
		public float m_messageFactor;
	}
}
