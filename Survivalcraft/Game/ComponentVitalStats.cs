using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020E RID: 526
	public class ComponentVitalStats : Component, IUpdateable
	{
		// Token: 0x17000246 RID: 582
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x0007A99E File Offset: 0x00078B9E
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x0007A9A6 File Offset: 0x00078BA6
		public float Food
		{
			get
			{
				return this.m_food;
			}
			set
			{
				this.m_food = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x0007A9B4 File Offset: 0x00078BB4
		// (set) Token: 0x0600102E RID: 4142 RVA: 0x0007A9BC File Offset: 0x00078BBC
		public float Stamina
		{
			get
			{
				return this.m_stamina;
			}
			set
			{
				this.m_stamina = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x0007A9CA File Offset: 0x00078BCA
		// (set) Token: 0x06001030 RID: 4144 RVA: 0x0007A9D2 File Offset: 0x00078BD2
		public float Sleep
		{
			get
			{
				return this.m_sleep;
			}
			set
			{
				this.m_sleep = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x0007A9E0 File Offset: 0x00078BE0
		// (set) Token: 0x06001032 RID: 4146 RVA: 0x0007A9E8 File Offset: 0x00078BE8
		public float Temperature
		{
			get
			{
				return this.m_temperature;
			}
			set
			{
				this.m_temperature = MathUtils.Clamp(value, 0f, 24f);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06001033 RID: 4147 RVA: 0x0007AA00 File Offset: 0x00078C00
		// (set) Token: 0x06001034 RID: 4148 RVA: 0x0007AA08 File Offset: 0x00078C08
		public float Wetness
		{
			get
			{
				return this.m_wetness;
			}
			set
			{
				this.m_wetness = MathUtils.Saturate(value);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x0007AA16 File Offset: 0x00078C16
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x0007AA1C File Offset: 0x00078C1C
		public bool Eat(int value)
		{
			if (ComponentVitalStats.Eat1 != null)
			{
				return ComponentVitalStats.Eat1(value);
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			float num2 = block.GetNutritionalValue(value);
			float sicknessProbability = block.GetSicknessProbability(value);
			if (num2 <= 0f)
			{
				return ComponentVitalStats.Eat2 != null && ComponentVitalStats.Eat2(value);
			}
			if (this.m_componentPlayer.ComponentSickness.IsSick && sicknessProbability > 0f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 1), Color.White, true, true);
				return false;
			}
			if (this.Food >= 0.98f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 2), Color.White, true, true);
				return false;
			}
			this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/HumanEat", 1f, this.m_random.Float(-0.2f, 0.2f), this.m_componentPlayer.ComponentBody.Position, 2f, 0f);
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				num2 *= 0.75f;
			}
			this.Food += num2;
			float num3;
			this.m_satiation.TryGetValue(num, out num3);
			num3 += MathUtils.Max(num2, 0.5f);
			this.m_satiation[num] = num3;
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				this.m_componentPlayer.ComponentSickness.NauseaEffect();
			}
			else if (sicknessProbability >= 0.5f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 3), Color.White, true, true);
			}
			else if (sicknessProbability > 0f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 4), Color.White, true, true);
			}
			else if (num3 > 2.5f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 5), Color.White, true, true);
			}
			else if (num3 > 2f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 6), Color.White, true, true);
			}
			else if (this.Food > 0.85f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 7), Color.White, true, true);
			}
			else
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 8), Color.White, true, false);
			}
			if (this.m_random.Bool(sicknessProbability) || num3 > 3.5f)
			{
				this.m_componentPlayer.ComponentSickness.StartSickness();
			}
			this.m_componentPlayer.PlayerStats.FoodItemsEaten += 1L;
			return true;
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0007ACEE File Offset: 0x00078EEE
		public void MakeSleepy(float sleepValue)
		{
			this.Sleep = MathUtils.Min(this.Sleep, sleepValue);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x0007AD04 File Offset: 0x00078F04
		public void Update(float dt)
		{
			if (this.m_componentPlayer.ComponentHealth.Health > 0f)
			{
				this.UpdateFood();
				this.UpdateStamina();
				this.UpdateSleep();
				this.UpdateTemperature();
				this.UpdateWetness();
				return;
			}
			this.m_pantingSound.Stop();
		}

        // Token: 0x06001039 RID: 4153 RVA: 0x0007AD54 File Offset: 0x00078F54
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemMetersBlockBehavior = base.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_pantingSound = this.m_subsystemAudio.CreateSound("Audio/HumanPanting");
			this.m_pantingSound.IsLooped = true;
			this.Food = valuesDictionary.GetValue<float>("Food");
			this.Stamina = valuesDictionary.GetValue<float>("Stamina");
			this.Sleep = valuesDictionary.GetValue<float>("Sleep");
			this.Temperature = valuesDictionary.GetValue<float>("Temperature");
			this.Wetness = valuesDictionary.GetValue<float>("Wetness");
			ValuesDictionary satiation = valuesDictionary.GetValue<ValuesDictionary>("Satiation");
			this.m_lastFood = this.Food;
			this.m_lastStamina = this.Stamina;
			this.m_lastSleep = this.Sleep;
			this.m_lastTemperature = this.Temperature;
			this.m_lastWetness = this.Wetness;
			this.m_environmentTemperature = this.Temperature;
			//for(int i = 0; i < satiation.Count; i++)
			//{
			//	KeyValuePair<string, int> keyValuePair = satiation.
			//}
			foreach (KeyValuePair<string, object> keyValuePair in satiation)
			{
				this.m_satiation[int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture)] = (float)keyValuePair.Value;
			}
			this.m_componentPlayer.ComponentHealth.Attacked += delegate(ComponentCreature p)
			{
				this.m_lastAttackedTime = new double?(this.m_subsystemTime.GameTime);
			};
		}

        // Token: 0x0600103A RID: 4154 RVA: 0x0007AF10 File Offset: 0x00079110
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Food", this.Food);
			valuesDictionary.SetValue<float>("Stamina", this.Stamina);
			valuesDictionary.SetValue<float>("Sleep", this.Sleep);
			valuesDictionary.SetValue<float>("Temperature", this.Temperature);
			valuesDictionary.SetValue<float>("Wetness", this.Wetness);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Satiation", valuesDictionary2);
			foreach (KeyValuePair<int, float> keyValuePair in this.m_satiation)
			{
				if (keyValuePair.Value > 0f)
				{
					valuesDictionary2.SetValue<float>(keyValuePair.Key.ToString(CultureInfo.InvariantCulture), keyValuePair.Value);
				}
			}
		}

        // Token: 0x0600103B RID: 4155 RVA: 0x0007AFF4 File Offset: 0x000791F4
        public override void OnEntityRemoved()
		{
			this.m_pantingSound.Stop();
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x0007B004 File Offset: 0x00079204
		public void UpdateFood()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			float num = (this.m_componentPlayer.ComponentLocomotion.LastWalkOrder != null) ? this.m_componentPlayer.ComponentLocomotion.LastWalkOrder.Value.Length() : 0f;
			float lastJumpOrder = this.m_componentPlayer.ComponentLocomotion.LastJumpOrder;
			float num2 = this.m_componentPlayer.ComponentCreatureModel.EyePosition.Y - this.m_componentPlayer.ComponentBody.Position.Y;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionDepth > num2;
			bool flag2 = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.33f && this.m_componentPlayer.ComponentBody.StandingOnValue == null;
			bool flag3 = this.m_subsystemTime.PeriodicGameTimeEvent(240.0, 13.0) && !this.m_componentPlayer.ComponentSickness.IsSick;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				float hungerFactor = this.m_componentPlayer.ComponentLevel.HungerFactor;
				this.Food -= hungerFactor * gameTimeDelta / 2880f;
				if (flag2 || flag)
				{
					this.Food -= hungerFactor * gameTimeDelta * num / 1440f;
				}
				else
				{
					this.Food -= hungerFactor * gameTimeDelta * num / 2880f;
				}
				this.Food -= hungerFactor * lastJumpOrder / 1200f;
				if (this.m_componentPlayer.ComponentMiner.DigCellFace != null)
				{
					this.Food -= hungerFactor * gameTimeDelta / 2880f;
				}
				if (!this.m_componentPlayer.ComponentSleep.IsSleeping)
				{
					if (this.Food <= 0f)
					{
						if (this.m_subsystemTime.PeriodicGameTimeEvent(50.0, 0.0))
						{
							this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 9));
							this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 10), Color.White, true, false);
							this.m_componentPlayer.ComponentGui.FoodBarWidget.Flash(10);
						}
					}
					else if (this.Food < 0.1f && (this.m_lastFood >= 0.1f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 11), Color.White, true, true);
					}
					else if (this.Food < 0.25f && (this.m_lastFood >= 0.25f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 12), Color.White, true, true);
					}
					else if (this.Food < 0.5f && (this.m_lastFood >= 0.5f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 13), Color.White, true, false);
					}
				}
			}
			else
			{
				this.Food = 0.9f;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, -0.01))
			{
				this.m_satiationList.Clear();
				this.m_satiationList.AddRange(this.m_satiation);
				this.m_satiation.Clear();
				foreach (KeyValuePair<int, float> keyValuePair in this.m_satiationList)
				{
					float num3 = MathUtils.Max(keyValuePair.Value - 0.00041666668f, 0f);
					if (num3 > 0f)
					{
						this.m_satiation.Add(keyValuePair.Key, num3);
					}
				}
			}
			this.m_lastFood = this.Food;
			this.m_componentPlayer.ComponentGui.FoodBarWidget.Value = this.Food;
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0007B470 File Offset: 0x00079670
		public void UpdateStamina()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			float num = (this.m_componentPlayer.ComponentLocomotion.LastWalkOrder != null) ? this.m_componentPlayer.ComponentLocomotion.LastWalkOrder.Value.Length() : 0f;
			float lastJumpOrder = this.m_componentPlayer.ComponentLocomotion.LastJumpOrder;
			float num2 = this.m_componentPlayer.ComponentCreatureModel.EyePosition.Y - this.m_componentPlayer.ComponentBody.Position.Y;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionDepth > num2;
			bool flag2 = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.33f && this.m_componentPlayer.ComponentBody.StandingOnValue == null;
			bool isPuking = this.m_componentPlayer.ComponentSickness.IsPuking;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.Stamina = 1f;
				this.ApplyDensityModifier(0f);
				return;
			}
			float num3 = 1f / MathUtils.Max(this.m_componentPlayer.ComponentLevel.SpeedFactor, 0.75f);
			if (this.m_componentPlayer.ComponentSickness.IsSick || this.m_componentPlayer.ComponentFlu.HasFlu)
			{
				num3 *= 5f;
			}
			this.Stamina += gameTimeDelta * 0.07f;
			this.Stamina -= 0.025f * lastJumpOrder * num3;
			if (flag2 || flag)
			{
				this.Stamina -= gameTimeDelta * (0.07f + 0.006f * num3 + 0.008f * num);
			}
			else
			{
				this.Stamina -= gameTimeDelta * (0.07f + 0.006f * num3) * num;
			}
			if (!flag2 && !flag && this.Stamina < 0.33f && this.m_lastStamina >= 0.33f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 14), Color.White, true, false);
			}
			if ((flag2 || flag) && this.Stamina < 0.4f && this.m_lastStamina >= 0.4f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 15), Color.White, true, true);
			}
			if (this.Stamina < 0.1f)
			{
				if (flag2 || flag)
				{
					if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
					{
						this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 16));
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 17), Color.White, true, false);
					}
					if (this.m_random.Float(0f, 1f) < 1f * gameTimeDelta)
					{
						this.m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
					}
				}
				else if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
				{
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 18), Color.White, true, true);
				}
			}
			this.m_lastStamina = this.Stamina;
			float num4 = MathUtils.Saturate(2f * (0.5f - this.Stamina));
			if (!flag && num4 > 0f)
			{
				float num5 = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.2f : 0f;
				this.m_pantingSound.Volume = 1f * SettingsManager.SoundsVolume * MathUtils.Saturate(1f * num4) * MathUtils.Lerp(0.8f, 1f, SimplexNoise.Noise((float)MathUtils.Remainder(3.0 * Time.RealTime + 100.0, 1000.0)));
				this.m_pantingSound.Pitch = AudioManager.ToEnginePitch(num5 + MathUtils.Lerp(-0.15f, 0.05f, num4) * MathUtils.Lerp(0.8f, 1.2f, SimplexNoise.Noise((float)MathUtils.Remainder(3.0 * Time.RealTime + 200.0, 1000.0))));
				this.m_pantingSound.Play();
			}
			else
			{
				this.m_pantingSound.Stop();
			}
			float num6 = MathUtils.Saturate(3f * (0.33f - this.Stamina));
			if (num6 > 0f && SimplexNoise.Noise((float)MathUtils.Remainder(Time.RealTime, 1000.0)) < num6)
			{
				this.ApplyDensityModifier(0.6f);
				return;
			}
			this.ApplyDensityModifier(0f);
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x0007B990 File Offset: 0x00079B90
		public void UpdateSleep()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.05f;
			bool flag2 = this.m_subsystemTime.PeriodicGameTimeEvent(240.0, 9.0);
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				if (this.m_componentPlayer.ComponentSleep.SleepFactor == 1f)
				{
					this.Sleep += 0.05f * gameTimeDelta;
				}
				else if (!flag)
				{
					if (this.m_lastAttackedTime != null)
					{
						double? num = this.m_subsystemTime.GameTime - this.m_lastAttackedTime;
						double num2 = 10.0;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							goto IL_379;
						}
					}
					this.Sleep -= gameTimeDelta / 1800f;
					if (this.Sleep < 0.075f && (this.m_lastSleep >= 0.075f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 19), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
					else if (this.Sleep < 0.2f && (this.m_lastSleep >= 0.2f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 20), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
					else if (this.Sleep < 0.33f && (this.m_lastSleep >= 0.33f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 21), Color.White, true, false);
					}
					else if (this.Sleep < 0.5f && (this.m_lastSleep >= 0.5f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 22), Color.White, true, false);
					}
					if (this.Sleep < 0.075f)
					{
						float num3 = MathUtils.Lerp(0.05f, 0.2f, (0.075f - this.Sleep) / 0.075f);
						float x = (this.Sleep < 0.0375f) ? this.m_random.Float(3f, 6f) : this.m_random.Float(2f, 4f);
						if (this.m_random.Float(0f, 1f) < num3 * gameTimeDelta)
						{
							this.m_sleepBlackoutDuration = MathUtils.Max(this.m_sleepBlackoutDuration, x);
							this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
						}
					}
					if (this.Sleep <= 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping)
					{
						this.m_componentPlayer.ComponentSleep.Sleep(false);
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 23), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
				}
			}
			else
			{
				this.Sleep = 0.9f;
			}
			IL_379:
			this.m_lastSleep = this.Sleep;
			this.m_sleepBlackoutDuration -= gameTimeDelta;
			float num4 = MathUtils.Saturate(0.5f * this.m_sleepBlackoutDuration);
			this.m_sleepBlackoutFactor = MathUtils.Saturate(this.m_sleepBlackoutFactor + 2f * gameTimeDelta * (num4 - this.m_sleepBlackoutFactor));
			if (!this.m_componentPlayer.ComponentSleep.IsSleeping)
			{
				this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_sleepBlackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
				if ((double)this.m_sleepBlackoutFactor > 0.01)
				{
					this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentVitalStats.fName, 24);
					this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_sleepBlackoutFactor - 0.9f));
				}
			}
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x0007BDFC File Offset: 0x00079FFC
		public void UpdateTemperature()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			bool flag = this.m_subsystemTime.PeriodicGameTimeEvent(300.0, 17.0);
			float num = this.m_componentPlayer.ComponentClothing.Insulation * MathUtils.Lerp(1f, 0.05f, MathUtils.Saturate(4f * this.Wetness));
			string arg;
			switch (this.m_componentPlayer.ComponentClothing.LeastInsulatedSlot)
			{
			case ClothingSlot.Head:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 40);
				break;
			case ClothingSlot.Torso:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 41);
				break;
			case ClothingSlot.Legs:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 42);
				break;
			default:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 43);
				break;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 2.0 * (double)this.GetHashCode() % 1000.0 / 1000.0))
			{
				int x = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.X);
				int y = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Y + 0.1f);
				int z = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Z);
				this.m_subsystemMetersBlockBehavior.CalculateTemperature(x, y, z, 12f, num, out this.m_environmentTemperature, out this.m_environmentTemperatureFlux);
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				float num2 = this.m_environmentTemperature - this.Temperature;
				float num3 = 0.01f + 0.005f * this.m_environmentTemperatureFlux;
				this.Temperature += MathUtils.Saturate(num3 * gameTimeDelta) * num2;
			}
			else
			{
				this.Temperature = 12f;
			}
			if (this.Temperature <= 0f)
			{
				this.m_componentPlayer.ComponentHealth.Injure(1f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 25));
			}
			else if (this.Temperature < 3f)
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
				{
					this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 26));
					string text = (this.Wetness > 0f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 27), arg) : ((num >= 1f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 28), arg) : string.Format(LanguageControl.Get(ComponentVitalStats.fName, 29), arg));
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(text, Color.White, true, false);
					this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				}
			}
			else if (this.Temperature < 6f && (this.m_lastTemperature >= 6f || flag))
			{
				string text2 = (this.Wetness > 0f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 30), arg) : ((num >= 1f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 31), arg) : string.Format(LanguageControl.Get(ComponentVitalStats.fName, 32), arg));
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(text2, Color.White, true, true);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
			}
			else if (this.Temperature < 8f && (this.m_lastTemperature >= 8f || flag))
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 33), Color.White, true, false);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
			}
			if (this.Temperature >= 24f)
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
				{
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 34), Color.White, true, false);
					this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 35));
					this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(8.0, 0.0))
				{
					this.m_temperatureBlackoutDuration = MathUtils.Max(this.m_temperatureBlackoutDuration, 6f);
					this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
				}
			}
			else if (this.Temperature > 20f && this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 36), Color.White, true, false);
				this.m_temperatureBlackoutDuration = MathUtils.Max(this.m_temperatureBlackoutDuration, 3f);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
			}
			this.m_lastTemperature = this.Temperature;
			this.m_componentPlayer.ComponentScreenOverlays.IceFactor = MathUtils.Saturate(1f - this.Temperature / 6f);
			this.m_temperatureBlackoutDuration -= gameTimeDelta;
			float num4 = MathUtils.Saturate(0.5f * this.m_temperatureBlackoutDuration);
			this.m_temperatureBlackoutFactor = MathUtils.Saturate(this.m_temperatureBlackoutFactor + 2f * gameTimeDelta * (num4 - this.m_temperatureBlackoutFactor));
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_temperatureBlackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
			if ((double)this.m_temperatureBlackoutFactor > 0.01)
			{
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentVitalStats.fName, 37);
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_temperatureBlackoutFactor - 0.9f));
			}
			if (this.m_environmentTemperature > 22f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature6");
				return;
			}
			if (this.m_environmentTemperature > 18f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature5");
				return;
			}
			if (this.m_environmentTemperature > 14f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature4");
				return;
			}
			if (this.m_environmentTemperature > 10f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature3");
				return;
			}
			if (this.m_environmentTemperature > 6f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature2");
				return;
			}
			if (this.m_environmentTemperature > 2f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature1");
				return;
			}
			this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature0");
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x0007C58C File Offset: 0x0007A78C
		public void UpdateWetness()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			if (this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.2f && this.m_componentPlayer.ComponentBody.ImmersionFluidBlock is WaterBlock)
			{
				float num = 2f * this.m_componentPlayer.ComponentBody.ImmersionFactor;
				this.Wetness += MathUtils.Saturate(3f * gameTimeDelta) * (num - this.Wetness);
			}
			int x = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.X);
			int num2 = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Y + 0.1f);
			int z = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Z);
			PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
			if (num2 >= precipitationShaftInfo.YLimit && precipitationShaftInfo.Type == PrecipitationType.Rain)
			{
				this.Wetness += 0.05f * precipitationShaftInfo.Intensity * gameTimeDelta;
			}
			float num3 = 180f;
			if (this.m_environmentTemperature > 8f)
			{
				num3 = 120f;
			}
			if (this.m_environmentTemperature > 16f)
			{
				num3 = 60f;
			}
			if (this.m_environmentTemperature > 24f)
			{
				num3 = 30f;
			}
			this.Wetness -= gameTimeDelta / num3;
			if (this.Wetness > 0.8f && this.m_lastWetness <= 0.8f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness > 0.8f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 38), Color.White, true, true);
					}
				});
			}
			else if (this.Wetness > 0.2f && this.m_lastWetness <= 0.2f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness > 0.2f && this.Wetness <= 0.8f && this.Wetness > this.m_lastWetness)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 39), Color.White, true, true);
					}
				});
			}
			else if (this.Wetness <= 0f && this.m_lastWetness > 0f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness <= 0f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 40), Color.White, true, true);
					}
				});
			}
			this.m_lastWetness = this.Wetness;
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x0007C7B0 File Offset: 0x0007A9B0
		public void ApplyDensityModifier(float modifier)
		{
			float num = modifier - this.m_densityModifierApplied;
			if (num != 0f)
			{
				this.m_densityModifierApplied = modifier;
				this.m_componentPlayer.ComponentBody.Density += num;
			}
		}

		// Token: 0x04000A88 RID: 2696
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A89 RID: 2697
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A8A RID: 2698
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000A8B RID: 2699
		public SubsystemMetersBlockBehavior m_subsystemMetersBlockBehavior;

		// Token: 0x04000A8C RID: 2700
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000A8D RID: 2701
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000A8E RID: 2702
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A8F RID: 2703
		public Sound m_pantingSound;

		// Token: 0x04000A90 RID: 2704
		public float m_food;

		// Token: 0x04000A91 RID: 2705
		public float m_stamina;

		// Token: 0x04000A92 RID: 2706
		public float m_sleep;

		// Token: 0x04000A93 RID: 2707
		public static Func<int, bool> Eat1;

		// Token: 0x04000A94 RID: 2708
		public static Func<int, bool> Eat2;

		// Token: 0x04000A95 RID: 2709
		public float m_temperature;

		// Token: 0x04000A96 RID: 2710
		public float m_wetness;

		// Token: 0x04000A97 RID: 2711
		public float m_lastFood;

		// Token: 0x04000A98 RID: 2712
		public float m_lastStamina;

		// Token: 0x04000A99 RID: 2713
		public float m_lastSleep;

		// Token: 0x04000A9A RID: 2714
		public float m_lastTemperature;

		// Token: 0x04000A9B RID: 2715
		public float m_lastWetness;

		// Token: 0x04000A9C RID: 2716
		public Dictionary<int, float> m_satiation = new Dictionary<int, float>();

		// Token: 0x04000A9D RID: 2717
		public List<KeyValuePair<int, float>> m_satiationList = new List<KeyValuePair<int, float>>();

		// Token: 0x04000A9E RID: 2718
		public float m_densityModifierApplied;

		// Token: 0x04000A9F RID: 2719
		public double? m_lastAttackedTime;

		// Token: 0x04000AA0 RID: 2720
		public float m_sleepBlackoutFactor;

		// Token: 0x04000AA1 RID: 2721
		public float m_sleepBlackoutDuration;

		// Token: 0x04000AA2 RID: 2722
		public float m_environmentTemperature;

		// Token: 0x04000AA3 RID: 2723
		public float m_environmentTemperatureFlux;

		// Token: 0x04000AA4 RID: 2724
		public float m_temperatureBlackoutFactor;

		// Token: 0x04000AA5 RID: 2725
		public float m_temperatureBlackoutDuration;

		// Token: 0x04000AA6 RID: 2726
		public static string fName = "ComponentVitalStats";
	}
}
