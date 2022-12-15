using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F8 RID: 504
	public class ComponentOnFire : Component, IUpdateable
	{
		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x00071B34 File Offset: 0x0006FD34
		// (set) Token: 0x06000ED3 RID: 3795 RVA: 0x00071B3C File Offset: 0x0006FD3C
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x00071B45 File Offset: 0x0006FD45
		public bool IsOnFire
		{
			get
			{
				return this.m_fireDuration > 0f;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x00071B54 File Offset: 0x0006FD54
		// (set) Token: 0x06000ED6 RID: 3798 RVA: 0x00071B5C File Offset: 0x0006FD5C
		public bool TouchesFire { get; set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000ED7 RID: 3799 RVA: 0x00071B65 File Offset: 0x0006FD65
		// (set) Token: 0x06000ED8 RID: 3800 RVA: 0x00071B6D File Offset: 0x0006FD6D
		public ComponentCreature Attacker { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x00071B76 File Offset: 0x0006FD76
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x00071B79 File Offset: 0x0006FD79
		public void SetOnFire(ComponentCreature attacker, float duration)
		{
			if (!this.IsOnFire)
			{
				this.Attacker = attacker;
			}
			this.m_fireDuration = MathUtils.Max(this.m_fireDuration, duration);
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x00071B9C File Offset: 0x0006FD9C
		public void Extinguish()
		{
			this.Attacker = null;
			this.m_fireDuration = 0f;
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x00071BB0 File Offset: 0x0006FDB0
		public void Update(float dt)
		{
			if (!base.IsAddedToProject)
			{
				return;
			}
			if (this.IsOnFire)
			{
				this.m_fireDuration = MathUtils.Max(this.m_fireDuration - dt, 0f);
				if (this.m_onFireParticleSystem == null)
				{
					this.m_onFireParticleSystem = new OnFireParticleSystem();
					this.m_subsystemParticles.AddParticleSystem(this.m_onFireParticleSystem);
				}
				BoundingBox boundingBox = this.ComponentBody.BoundingBox;
				this.m_onFireParticleSystem.Position = 0.5f * (boundingBox.Min + boundingBox.Max);
				this.m_onFireParticleSystem.Radius = 0.5f * MathUtils.Min(boundingBox.Max.X - boundingBox.Min.X, boundingBox.Max.Z - boundingBox.Min.Z);
				if (this.ComponentBody.ImmersionFactor > 0.5f && this.ComponentBody.ImmersionFluidBlock is WaterBlock)
				{
					this.Extinguish();
					this.m_subsystemAudio.PlaySound("Audio/SizzleLong", 1f, 0f, this.m_onFireParticleSystem.Position, 4f, true);
				}
				if (Time.PeriodicEvent(0.5, 0.0))
				{
					float distance = this.m_subsystemAudio.CalculateListenerDistance(this.ComponentBody.Position);
					this.m_soundVolume = this.m_subsystemAudio.CalculateVolume(distance, 2f, 5f);
				}
				this.m_subsystemAmbientSounds.FireSoundVolume = MathUtils.Max(this.m_subsystemAmbientSounds.FireSoundVolume, this.m_soundVolume);
			}
			else
			{
				if (this.m_onFireParticleSystem != null)
				{
					this.m_onFireParticleSystem.IsStopped = true;
					this.m_onFireParticleSystem = null;
				}
				this.m_soundVolume = 0f;
			}
			if (this.m_subsystemTime.GameTime <= this.m_nextCheckTime)
			{
				return;
			}
			this.m_nextCheckTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.9f, 1.1f);
			this.TouchesFire = this.CheckIfBodyTouchesFire();
			if (this.TouchesFire)
			{
				this.m_fireTouchCount++;
				if (this.m_fireTouchCount >= 5)
				{
					this.SetOnFire(null, this.m_random.Float(12f, 15f));
				}
			}
			else
			{
				this.m_fireTouchCount = 0;
			}
			if (this.ComponentBody.ImmersionFactor > 0.2f && this.ComponentBody.ImmersionFluidBlock is MagmaBlock)
			{
				this.SetOnFire(null, this.m_random.Float(12f, 15f));
			}
		}

        // Token: 0x06000EDD RID: 3805 RVA: 0x00071E3C File Offset: 0x0007003C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>();
			float value = valuesDictionary.GetValue<float>("FireDuration");
			if (value > 0f)
			{
				this.SetOnFire(null, value);
			}
		}

        // Token: 0x06000EDE RID: 3806 RVA: 0x00071ED0 File Offset: 0x000700D0
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("FireDuration", this.m_fireDuration);
		}

        // Token: 0x06000EDF RID: 3807 RVA: 0x00071EE3 File Offset: 0x000700E3
        public override void OnEntityRemoved()
		{
			if (this.m_onFireParticleSystem != null)
			{
				this.m_onFireParticleSystem.IsStopped = true;
			}
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x00071EFC File Offset: 0x000700FC
		public bool CheckIfBodyTouchesFire()
		{
			BoundingBox boundingBox = this.ComponentBody.BoundingBox;
			boundingBox.Min -= new Vector3(0.25f);
			boundingBox.Max += new Vector3(0.25f);
			int num = Terrain.ToCell(boundingBox.Min.X);
			int num2 = Terrain.ToCell(boundingBox.Min.Y);
			int num3 = Terrain.ToCell(boundingBox.Min.Z);
			int num4 = Terrain.ToCell(boundingBox.Max.X);
			int num5 = Terrain.ToCell(boundingBox.Max.Y);
			int num6 = Terrain.ToCell(boundingBox.Max.Z);
			for (int i = num; i <= num4; i++)
			{
				for (int j = num2; j <= num5; j++)
				{
					for (int k = num3; k <= num6; k++)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(i, j, k);
						int num7 = Terrain.ExtractContents(cellValue);
						int num8 = Terrain.ExtractData(cellValue);
						if (num7 != 104)
						{
							if (num7 == 209)
							{
								if (num8 > 0)
								{
									BoundingBox box = new BoundingBox(new Vector3((float)i, (float)j, (float)k) + new Vector3(0.2f), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)) - new Vector3(0.2f));
									if (boundingBox.Intersection(box))
									{
										return true;
									}
								}
							}
						}
						else if (num8 == 0)
						{
							BoundingBox box2 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
							if (boundingBox.Intersection(box2))
							{
								return true;
							}
						}
						else
						{
							if ((num8 & 1) != 0)
							{
								BoundingBox box3 = new BoundingBox(new Vector3((float)i, (float)j, (float)k + 0.5f), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box3))
								{
									return true;
								}
							}
							if ((num8 & 2) != 0)
							{
								BoundingBox box4 = new BoundingBox(new Vector3((float)i + 0.5f, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box4))
								{
									return true;
								}
							}
							if ((num8 & 4) != 0)
							{
								BoundingBox box5 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)k + 0.5f));
								if (boundingBox.Intersection(box5))
								{
									return true;
								}
							}
							if ((num8 & 8) != 0)
							{
								BoundingBox box6 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)i + 0.5f, (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box6))
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04000966 RID: 2406
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000967 RID: 2407
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000968 RID: 2408
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000969 RID: 2409
		public SubsystemAmbientSounds m_subsystemAmbientSounds;

		// Token: 0x0400096A RID: 2410
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400096B RID: 2411
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400096C RID: 2412
		public double m_nextCheckTime;

		// Token: 0x0400096D RID: 2413
		public int m_fireTouchCount;

		// Token: 0x0400096E RID: 2414
		public OnFireParticleSystem m_onFireParticleSystem;

		// Token: 0x0400096F RID: 2415
		public float m_soundVolume;

		// Token: 0x04000970 RID: 2416
		public float m_fireDuration;
	}
}
