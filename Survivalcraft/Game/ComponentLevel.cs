using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EF RID: 495
	public class ComponentLevel : Component, IUpdateable
	{
		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000E0F RID: 3599 RVA: 0x0006CC30 File Offset: 0x0006AE30
		// (set) Token: 0x06000E10 RID: 3600 RVA: 0x0006CC38 File Offset: 0x0006AE38
		public float StrengthFactor { get; set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0006CC41 File Offset: 0x0006AE41
		// (set) Token: 0x06000E12 RID: 3602 RVA: 0x0006CC49 File Offset: 0x0006AE49
		public float ResilienceFactor { get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000E13 RID: 3603 RVA: 0x0006CC52 File Offset: 0x0006AE52
		// (set) Token: 0x06000E14 RID: 3604 RVA: 0x0006CC5A File Offset: 0x0006AE5A
		public float SpeedFactor { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000E15 RID: 3605 RVA: 0x0006CC63 File Offset: 0x0006AE63
		// (set) Token: 0x06000E16 RID: 3606 RVA: 0x0006CC6B File Offset: 0x0006AE6B
		public float HungerFactor { get; set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000E17 RID: 3607 RVA: 0x0006CC74 File Offset: 0x0006AE74
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x0006CC78 File Offset: 0x0006AE78
		public void AddExperience(int count, bool playSound)
		{
			if (playSound)
			{
				this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 0.2f, this.m_random.Float(-0.1f, 0.4f), 0f, 0f);
			}
			for (int i = 0; i < count; i++)
			{
				float num = 0.012f / MathUtils.Pow(1.08f, MathUtils.Floor(this.m_componentPlayer.PlayerData.Level - 1f));
				if (MathUtils.Floor(this.m_componentPlayer.PlayerData.Level + num) > MathUtils.Floor(this.m_componentPlayer.PlayerData.Level))
				{
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentLevel.fName, 1), Color.White, true, false);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.2f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.15000000596046448, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.30000001192092896, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.13333334f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.45000001788139343, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.38333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.75, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.9000000357627869, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.38333333f, 0f, 0f);
					});
				}
				this.m_componentPlayer.PlayerData.Level += num;
			}
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x0006CE6C File Offset: 0x0006B06C
		public float CalculateStrengthFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float stamina = this.m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			float num6 = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 3), string.Format("{0:0}", stamina * 100f))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = (float)((!this.m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0);
			float num10 = num8 * num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = (this.m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(ComponentLevel.fName, 6) : LanguageControl.Get(ComponentLevel.fName, 7))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num11 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num12 = num10 * num11;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num11,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num13 = (float)((!this.m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0);
			float num14 = num12 * num13;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num13,
					Description = (this.m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(ComponentLevel.fName, 10) : LanguageControl.Get(ComponentLevel.fName, 11))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num15 = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless) ? 1.25f : 1f;
			float result = num14 * num15;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num15,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x0006D22C File Offset: 0x0006B42C
		public float CalculateResilienceFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num5 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num6 = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = 1f;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num9 = 1.5f;
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num9 = float.PositiveInfinity;
			}
			float result = num8 * num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x0006D494 File Offset: 0x0006B694
		public float CalculateSpeedFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = 1f;
			float num2 = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 1.03f : 1f;
			num *= num2;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num2,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.02f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			num *= num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num4 = 1f;
			foreach (int clothingValue in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Head))
			{
				ComponentLevel.AddClothingFactor(clothingValue, ref num4, factors);
			}
			foreach (int clothingValue2 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Torso))
			{
				ComponentLevel.AddClothingFactor(clothingValue2, ref num4, factors);
			}
			foreach (int clothingValue3 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Legs))
			{
				ComponentLevel.AddClothingFactor(clothingValue3, ref num4, factors);
			}
			foreach (int clothingValue4 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Feet))
			{
				ComponentLevel.AddClothingFactor(clothingValue4, ref num4, factors);
			}
			num *= num4;
			float stamina = this.m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			num *= num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 3), string.Format("{0:0}", stamina * 100f))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num6 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			num *= num6;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num6,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = (float)((!this.m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0);
			num *= num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(ComponentLevel.fName, 6) : LanguageControl.Get(ComponentLevel.fName, 7))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num8 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			num *= num8;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num8,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = (float)((!this.m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0);
			num *= num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = (this.m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(ComponentLevel.fName, 10) : LanguageControl.Get(ComponentLevel.fName, 11))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return num;
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x0006D948 File Offset: 0x0006BB48
		public float CalculateHungerFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.7f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f - 0.01f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				factors.Add(item);
			}
			float num5 = 1f;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num5 = 0.66f;
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num5 = 0f;
			}
			float result = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x0006DACC File Offset: 0x0006BCCC
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(180.0, 179.0))
			{
				this.AddExperience(1, false);
			}
			this.StrengthFactor = this.CalculateStrengthFactor(null);
			this.SpeedFactor = this.CalculateSpeedFactor(null);
			this.HungerFactor = this.CalculateHungerFactor(null);
			this.ResilienceFactor = this.CalculateResilienceFactor(null);
			if (this.m_lastLevelTextValue == null || this.m_lastLevelTextValue.Value != MathUtils.Floor(this.m_componentPlayer.PlayerData.Level))
			{
				this.m_componentPlayer.ComponentGui.LevelLabelWidget.Text = "等级 " + MathUtils.Floor(this.m_componentPlayer.PlayerData.Level).ToString();
				this.m_lastLevelTextValue = new float?(MathUtils.Floor(this.m_componentPlayer.PlayerData.Level));
			}
			this.m_componentPlayer.PlayerStats.HighestLevel = MathUtils.Max(this.m_componentPlayer.PlayerStats.HighestLevel, this.m_componentPlayer.PlayerData.Level);
		}

        // Token: 0x06000E1E RID: 3614 RVA: 0x0006DBF4 File Offset: 0x0006BDF4
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.StrengthFactor = 1f;
			this.SpeedFactor = 1f;
			this.HungerFactor = 1f;
			this.ResilienceFactor = 1f;
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x0006DC78 File Offset: 0x0006BE78
		public static void AddClothingFactor(int clothingValue, ref float clothingFactor, ICollection<ComponentLevel.Factor> factors)
		{
			ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(clothingValue));
			if (clothingData.MovementSpeedFactor != 1f)
			{
				clothingFactor *= clothingData.MovementSpeedFactor;
				if (factors != null)
				{
					factors.Add(new ComponentLevel.Factor
					{
						Value = clothingData.MovementSpeedFactor,
						Description = clothingData.DisplayName
					});
				}
			}
		}

		// Token: 0x040008DD RID: 2269
		public Game.Random m_random = new Game.Random();

		// Token: 0x040008DE RID: 2270
		public static string fName = "ComponentLevel";

		// Token: 0x040008DF RID: 2271
		public List<ComponentLevel.Factor> m_factors = new List<ComponentLevel.Factor>();

		// Token: 0x040008E0 RID: 2272
		public float? m_lastLevelTextValue;

		// Token: 0x040008E1 RID: 2273
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040008E2 RID: 2274
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040008E3 RID: 2275
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008E4 RID: 2276
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040008E5 RID: 2277
		public const float FemaleStrengthFactor = 0.8f;

		// Token: 0x040008E6 RID: 2278
		public const float FemaleResilienceFactor = 0.8f;

		// Token: 0x040008E7 RID: 2279
		public const float FemaleSpeedFactor = 1.03f;

		// Token: 0x040008E8 RID: 2280
		public const float FemaleHungerFactor = 0.7f;

		// Token: 0x0200045B RID: 1115
		public struct Factor
		{
			// Token: 0x04001650 RID: 5712
			public string Description;

			// Token: 0x04001651 RID: 5713
			public float Value;
		}
	}
}
