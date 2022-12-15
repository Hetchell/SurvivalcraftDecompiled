using System;
using Engine;
using GameEntitySystem;
using Survivalcraft.Game;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E5 RID: 485
	public class ComponentHealth : Component, IUpdateable
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000D81 RID: 3457 RVA: 0x00066C8C File Offset: 0x00064E8C
		// (set) Token: 0x06000D82 RID: 3458 RVA: 0x00066C94 File Offset: 0x00064E94
		public string CauseOfDeath { get; set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000D83 RID: 3459 RVA: 0x00066C9D File Offset: 0x00064E9D
		// (set) Token: 0x06000D84 RID: 3460 RVA: 0x00066CA5 File Offset: 0x00064EA5
		public bool IsInvulnerable { get; set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000D85 RID: 3461 RVA: 0x00066CAE File Offset: 0x00064EAE
		// (set) Token: 0x06000D86 RID: 3462 RVA: 0x00066CB6 File Offset: 0x00064EB6
		public float Health { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000D87 RID: 3463 RVA: 0x00066CBF File Offset: 0x00064EBF
		// (set) Token: 0x06000D88 RID: 3464 RVA: 0x00066CC7 File Offset: 0x00064EC7
		public float HealthChange { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000D89 RID: 3465 RVA: 0x00066CD0 File Offset: 0x00064ED0
		// (set) Token: 0x06000D8A RID: 3466 RVA: 0x00066CD8 File Offset: 0x00064ED8
		public BreathingMode BreathingMode { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00066CE1 File Offset: 0x00064EE1
		// (set) Token: 0x06000D8C RID: 3468 RVA: 0x00066CE9 File Offset: 0x00064EE9
		public float Air { get; set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000D8D RID: 3469 RVA: 0x00066CF2 File Offset: 0x00064EF2
		// (set) Token: 0x06000D8E RID: 3470 RVA: 0x00066CFA File Offset: 0x00064EFA
		public float AirCapacity { get; set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000D8F RID: 3471 RVA: 0x00066D03 File Offset: 0x00064F03
		// (set) Token: 0x06000D90 RID: 3472 RVA: 0x00066D0B File Offset: 0x00064F0B
		public bool CanStrand { get; set; }

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00066D14 File Offset: 0x00064F14
		// (set) Token: 0x06000D92 RID: 3474 RVA: 0x00066D1C File Offset: 0x00064F1C
		public float AttackResilience { get; set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000D93 RID: 3475 RVA: 0x00066D25 File Offset: 0x00064F25
		// (set) Token: 0x06000D94 RID: 3476 RVA: 0x00066D2D File Offset: 0x00064F2D
		public float FallResilience { get; set; }

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00066D36 File Offset: 0x00064F36
		// (set) Token: 0x06000D96 RID: 3478 RVA: 0x00066D3E File Offset: 0x00064F3E
		public float FireResilience { get; set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000D97 RID: 3479 RVA: 0x00066D47 File Offset: 0x00064F47
		// (set) Token: 0x06000D98 RID: 3480 RVA: 0x00066D4F File Offset: 0x00064F4F
		public double? DeathTime { get; set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000D99 RID: 3481 RVA: 0x00066D58 File Offset: 0x00064F58
		// (set) Token: 0x06000D9A RID: 3482 RVA: 0x00066D60 File Offset: 0x00064F60
		public float CorpseDuration { get; set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000D9B RID: 3483 RVA: 0x00066D69 File Offset: 0x00064F69
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000D9C RID: 3484 RVA: 0x00066D6C File Offset: 0x00064F6C
		// (remove) Token: 0x06000D9D RID: 3485 RVA: 0x00066DA4 File Offset: 0x00064FA4
		public event Action<ComponentCreature> Attacked;

		// Token: 0x06000D9E RID: 3486 RVA: 0x00066DD9 File Offset: 0x00064FD9
		public void Heal(float amount)
		{
			if (amount > 0f)
			{
				this.Health = MathUtils.Min(this.Health + amount, 1f);
			}
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x00066DFC File Offset: 0x00064FFC
		public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability, string cause)
		{
			if (amount <= 0f || (!ignoreInvulnerability && this.IsInvulnerable))
			{
				return;
			}
			if (this.Health > 0f)
			{
				if (this.m_componentCreature.PlayerStats != null)
				{
					if (attacker != null)
					{
						this.m_componentCreature.PlayerStats.HitsReceived += 1L;
					}
					this.m_componentCreature.PlayerStats.TotalHealthLost += (double)MathUtils.Min(amount, this.Health);
				}
				this.Health = MathUtils.Max(this.Health - amount, 0f);
				if (this.Health <= 0f)
				{
					this.CauseOfDeath = cause;
					if (this.m_componentCreature.PlayerStats != null)
					{
						this.m_componentCreature.PlayerStats.AddDeathRecord(new PlayerStats.DeathRecord
						{
							Day = this.m_subsystemTimeOfDay.Day,
							Location = this.m_componentCreature.ComponentBody.Position,
							Cause = cause
						});
					}
					if (attacker != null)
					{
						ComponentPlayer componentPlayer = attacker.Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer != null)
						{
							if (this.m_componentPlayer != null)
							{
								componentPlayer.PlayerStats.PlayerKills += 1L;
							}
							else if (this.m_componentCreature.Category == CreatureCategory.LandPredator || this.m_componentCreature.Category == CreatureCategory.LandOther)
							{
								componentPlayer.PlayerStats.LandCreatureKills += 1L;
							}
							else if (this.m_componentCreature.Category == CreatureCategory.WaterPredator || this.m_componentCreature.Category == CreatureCategory.WaterOther)
							{
								componentPlayer.PlayerStats.WaterCreatureKills += 1L;
							}
							else
							{
								componentPlayer.PlayerStats.AirCreatureKills += 1L;
							}
							int num = (int)MathUtils.Ceiling(this.m_componentCreature.ComponentHealth.AttackResilience / 12f);
							for (int i = 0; i < num; i++)
							{
								Vector2 vector = this.m_random.Vector2(2.5f, 3.5f);
								this.m_subsystemPickables.AddPickable(248, 1, this.m_componentCreature.ComponentBody.Position, new Vector3?(new Vector3(vector.X, 6f, vector.Y)), null);
							}
						}
					}
				}
			}
			if (attacker != null && this.Attacked != null)
			{
				this.Attacked(attacker);
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00067050 File Offset: 0x00065250
		public void Update(float dt)
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			if (this.Health > 0f && this.Health < 1f)
			{
				float num = 0f;
				if (this.m_componentPlayer != null)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
					{
						num = 0.016666668f;
					}
					else if (this.m_componentPlayer.ComponentSleep.SleepFactor == 1f && this.m_componentPlayer.ComponentVitalStats.Food > 0f)
					{
						num = 0.0016666667f;
					}
					else if (this.m_componentPlayer.ComponentVitalStats.Food > 0.5f)
					{
						num = 0.0011111111f;
					}
				}
				else
				{
					num = 0.0011111111f;
				}
				this.Heal(this.m_subsystemGameInfo.TotalElapsedGameTimeDelta * num);
			}
			if (this.BreathingMode == BreathingMode.Air)
			{
				int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(position.X), Terrain.ToCell(this.m_componentCreature.ComponentCreatureModel.EyePosition.Y), Terrain.ToCell(position.Z));
				if (BlocksManager.Blocks[cellContents] is FluidBlock || position.Y > 259f)
				{
					this.Air = MathUtils.Saturate(this.Air - dt / this.AirCapacity);
				}
				else
				{
					this.Air = 1f;
				}
			}
			else if (this.BreathingMode == BreathingMode.Water)
			{
				if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0.25f)
				{
					this.Air = 1f;
				}
				else
				{
					this.Air = MathUtils.Saturate(this.Air - dt / this.AirCapacity);
				}
			}
			if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0f && this.m_componentCreature.ComponentBody.ImmersionFluidBlock is MagmaBlock)
			{
				this.Injure(2f * this.m_componentCreature.ComponentBody.ImmersionFactor * dt, null, false, LanguageControl.Get("ComponentHealth", "1"));
				float num2 = 1.1f + 0.1f * (float)MathUtils.Sin(12.0 * this.m_subsystemTime.GameTime);
				this.m_redScreenFactor = MathUtils.Max(this.m_redScreenFactor, num2 * 1.5f * this.m_componentCreature.ComponentBody.ImmersionFactor);
			}
			float num3 = MathUtils.Abs(this.m_componentCreature.ComponentBody.CollisionVelocityChange.Y);
			if (!this.m_wasStanding && num3 > this.FallResilience)
			{
				float num4 = MathUtils.Sqr(MathUtils.Max(num3 - this.FallResilience, 0f)) / 15f;
				if (this.m_componentPlayer != null)
				{
					num4 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num4, null, false, LanguageControl.Get("ComponentHealth", "2"));
			}
			this.m_wasStanding = (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.StandingOnBody != null);
			if ((position.Y < 0f || position.Y > 296f) && this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0) && !ModifierHolder.allowUnrestrictedTravel)
			{
				this.Injure(0.1f, null, true, LanguageControl.Get("ComponentHealth", "3"));
				ComponentPlayer componentPlayer = this.m_componentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get("ComponentHealth", "4"), Color.White, true, false);
				}
			}
			bool flag = this.m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0);
			if (flag && this.Air == 0f)
			{
				float num5 = 0.12f;
				if (this.m_componentPlayer != null)
				{
					num5 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num5, null, false, LanguageControl.Get("ComponentHealth", "5"));
			}
			if (flag && (this.m_componentOnFire.IsOnFire || this.m_componentOnFire.TouchesFire))
			{
				float num6 = 1f / this.FireResilience;
				if (this.m_componentPlayer != null)
				{
					num6 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num6, this.m_componentOnFire.Attacker, false, LanguageControl.Get("ComponentHealth", "5"));
			}
			if (flag && this.CanStrand && this.m_componentCreature.ComponentBody.ImmersionFactor < 0.25f)
			{
				int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
				int num7 = 0;
				if (!(standingOnValue.GetValueOrDefault() == num7 & standingOnValue != null) || this.m_componentCreature.ComponentBody.StandingOnBody != null)
				{
					this.Injure(0.05f, null, false, LanguageControl.Get("ComponentHealth", "6"));
				}
			}
			this.HealthChange = this.Health - this.m_lastHealth;
			this.m_lastHealth = this.Health;
			if (this.m_redScreenFactor > 0.01f)
			{
				this.m_redScreenFactor *= MathUtils.Pow(0.2f, dt);
			}
			else
			{
				this.m_redScreenFactor = 0f;
			}
			if (this.HealthChange < 0f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_redScreenFactor += -4f * this.HealthChange;
				ComponentPlayer componentPlayer2 = this.m_componentPlayer;
				if (componentPlayer2 != null)
				{
					componentPlayer2.ComponentGui.HealthBarWidget.Flash(MathUtils.Clamp((int)((0f - this.HealthChange) * 30f), 0, 10));
				}
			}
			if (this.m_componentPlayer != null)
			{
				this.m_componentPlayer.ComponentScreenOverlays.RedoutFactor = MathUtils.Max(this.m_componentPlayer.ComponentScreenOverlays.RedoutFactor, this.m_redScreenFactor);
			}
			if (this.m_componentPlayer != null)
			{
				this.m_componentPlayer.ComponentGui.HealthBarWidget.Value = this.Health;
			}
			if (this.Health == 0f && this.HealthChange < 0f)
			{
				Vector3 position2 = this.m_componentCreature.ComponentBody.Position + new Vector3(0f, this.m_componentCreature.ComponentBody.BoxSize.Y / 2f, 0f);
				float x = this.m_componentCreature.ComponentBody.BoxSize.X;
				this.m_subsystemParticles.AddParticleSystem(new KillParticleSystem(this.m_subsystemTerrain, position2, x));
				Vector3 position3 = (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max) / 2f;
				foreach (IInventory inventory in base.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position3);
				}
				this.DeathTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
			if (this.Health <= 0f && this.CorpseDuration > 0f)
			{
				double? num8 = this.m_subsystemGameInfo.TotalElapsedGameTime - this.DeathTime;
				double num9 = (double)this.CorpseDuration;
				if (num8.GetValueOrDefault() > num9 & num8 != null)
				{
					this.m_componentCreature.ComponentSpawn.Despawn();
				}
			}
		}

        // Token: 0x06000DA1 RID: 3489 RVA: 0x00067804 File Offset: 0x00065A04
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			this.m_componentOnFire = base.Entity.FindComponent<ComponentOnFire>(true);
			this.AttackResilience = valuesDictionary.GetValue<float>("AttackResilience");
			this.FallResilience = valuesDictionary.GetValue<float>("FallResilience");
			this.FireResilience = valuesDictionary.GetValue<float>("FireResilience");
			this.CorpseDuration = valuesDictionary.GetValue<float>("CorpseDuration");
			this.BreathingMode = valuesDictionary.GetValue<BreathingMode>("BreathingMode");
			this.CanStrand = valuesDictionary.GetValue<bool>("CanStrand");
			this.Health = valuesDictionary.GetValue<float>("Health");
			this.Air = valuesDictionary.GetValue<float>("Air");
			this.AirCapacity = valuesDictionary.GetValue<float>("AirCapacity");
			double value = valuesDictionary.GetValue<double>("DeathTime");
			this.DeathTime = ((value >= 0.0) ? new double?(value) : null);
			this.CauseOfDeath = valuesDictionary.GetValue<string>("CauseOfDeath");
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && base.Entity.FindComponent<ComponentPlayer>() != null)
			{
				this.IsInvulnerable = true;
			}
		}

        // Token: 0x06000DA2 RID: 3490 RVA: 0x000679B4 File Offset: 0x00065BB4
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Health", this.Health);
			valuesDictionary.SetValue<float>("Air", this.Air);
			if (this.DeathTime != null)
			{
				valuesDictionary.SetValue<double?>("DeathTime", this.DeathTime);
			}
			if (!string.IsNullOrEmpty(this.CauseOfDeath))
			{
				valuesDictionary.SetValue<string>("CauseOfDeath", this.CauseOfDeath);
			}
		}

		// Token: 0x04000859 RID: 2137
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400085A RID: 2138
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x0400085B RID: 2139
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400085C RID: 2140
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400085D RID: 2141
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400085E RID: 2142
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400085F RID: 2143
		public ComponentCreature m_componentCreature;

		// Token: 0x04000860 RID: 2144
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000861 RID: 2145
		public ComponentOnFire m_componentOnFire;

		// Token: 0x04000862 RID: 2146
		public float m_lastHealth;

		// Token: 0x04000863 RID: 2147
		public bool m_wasStanding;

		// Token: 0x04000864 RID: 2148
		public float m_redScreenFactor;

		// Token: 0x04000865 RID: 2149
		public Game.Random m_random = new Game.Random();
	}
}
